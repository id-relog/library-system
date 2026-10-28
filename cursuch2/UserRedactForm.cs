using System;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using cursuch2.Models;

namespace cursuch2
{
    public partial class UserRedactForm : Form
    {
        private SqlConnection _connection;
        private User _currentUser;
        private User _userToEdit;

        public UserRedactForm(SqlConnection connection, User currentUser, User userToEdit = null)
        {
            InitializeComponent();
            _connection = connection;
            _currentUser = currentUser;
            _userToEdit = userToEdit;

            
            if (userToEdit == null)
            {
                this.Close();
                return;
            }

            if (!CheckAccess())
            {
                this.Close();
                return;
            }

            InitializeForm();
        }

        private bool CheckAccess()
        {
            if (_userToEdit.Role == UserRole.Super_admin || _userToEdit.Role == UserRole.Admin)
            {
                if (_currentUser.Role != UserRole.Super_admin)
                {
                    MessageBox.Show("Только супер-администратор может редактировать пользователей с ролью админа",
                        "Ошибка доступа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        private void InitializeForm()
        {
            // Загрузка данных в комбобоксы
            comboBoxGroup.DataSource = Enum.GetValues(typeof(PopulationGroup));
            comboBoxRole.DataSource = Enum.GetValues(typeof(UserRole));

            // Заполнение полей данными пользователя
            txtEmail.Text = _userToEdit.Email;
            txtFullname.Text = _userToEdit.FullName;
            txtPhone.Text = _userToEdit.PhoneNumber;
            txtUserName.Text = _userToEdit.Username;
            txtPassword.Text = _userToEdit.PasswordHash;
            comboBoxGroup.SelectedItem = _userToEdit.Group;
            comboBoxRole.SelectedItem = _userToEdit.Role;
            label8.Text = $"Редактирование пользователя: {_userToEdit.Username}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAdminPas.Text))
            {
                MessageBox.Show("Введите пароль для подтверждения");
                return;
            }

            if (!(User.HashPassword(txtAdminPas.Text).Equals(_currentUser.PasswordHash)))
            {
                MessageBox.Show("Не верный пароль для подтверждения");
                return;
            }
            
            try
            {
                if (!ValidateInput()) return;
                if (!string.Equals(User.HashPassword(txtPassword.Text), _userToEdit.PasswordHash))
                {
                     var a = MessageBox.Show("Вы точно хотите изменить пароль пользователю?", "внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (a == DialogResult.No) return;
                }
                
                UpdateExistingUser();

                MessageBox.Show("Данные успешно сохранены", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtFullname.Text) ||
                string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                MessageBox.Show("Заполните все обязательные поля",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtPassword.Text) && txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            var selectedRole = (UserRole)comboBoxRole.SelectedItem;
            if ((selectedRole == UserRole.Admin || selectedRole == UserRole.Super_admin) &&
                _currentUser.Role != UserRole.Super_admin)
            {
                MessageBox.Show("Только супер-администратор может назначать администраторов",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void UpdateExistingUser()
        {
            string passwordPart = string.IsNullOrWhiteSpace(txtPassword.Text) ? "" :
                ", PasswordHash = @PasswordHash";

            string query = $@"UPDATE Users SET 
                            Email = @Email, 
                            FullName = @FullName, 
                            PhoneNumber = @PhoneNumber, 
                            Username = @Username, 
                            GroupId = @GroupId, 
                            RoleID = @RoleID
                            {passwordPart}
                            WHERE ReaderId = @ReaderId";

            using (var command = new SqlCommand(query, _connection))
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                    _connection.Open();

                command.Parameters.AddWithValue("@Email", txtEmail.Text);
                command.Parameters.AddWithValue("@FullName", txtFullname.Text);
                command.Parameters.AddWithValue("@PhoneNumber", txtPhone.Text ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Username", txtUserName.Text);
                command.Parameters.AddWithValue("@GroupId", (int)comboBoxGroup.SelectedItem);
                command.Parameters.AddWithValue("@RoleID", (int)comboBoxRole.SelectedItem);
                command.Parameters.AddWithValue("@ReaderId", _userToEdit.Id);

                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    command.Parameters.AddWithValue("@PasswordHash", User.HashPassword(txtPassword.Text));
                }

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    LogAction($"Обновлен пользователь ID: {_userToEdit.Id}");
                }
            }
        }

        private void LogAction(string actionDetails)
        {
            string query = @"INSERT INTO AdminLogs 
                            (ActionTime, ActionType, ActionDetails, AdminId, AffectedId, AffectedType)
                            VALUES 
                            (GETDATE(), 'UserEdit', @ActionDetails, @AdminId, @AffectedId, 'User')";

            using (var command = new SqlCommand(query, _connection))
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                    _connection.Open();

                command.Parameters.AddWithValue("@ActionDetails", actionDetails);
                command.Parameters.AddWithValue("@AdminId", _currentUser.Id);
                command.Parameters.AddWithValue("@AffectedId", _userToEdit.Id);

                command.ExecuteNonQuery();
            }
        }

        private void comboBoxRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxRole.SelectedItem is UserRole selectedRole &&
                (selectedRole == UserRole.Admin || selectedRole == UserRole.Super_admin) &&
                _currentUser.Role != UserRole.Super_admin)
            {
                MessageBox.Show("Только супер-администратор может назначать администраторов",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                comboBoxRole.SelectedItem = UserRole.User;
            }
        }
    }
}