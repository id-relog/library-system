using System;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using cursuch2.Models;

namespace cursuch2
{
    public partial class NewUserForm : Form
    {
        private readonly User _currentUser;
        private readonly SqlConnection _connection;

        public NewUserForm(SqlConnection connection, User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            
            InitializeForm();
        }

        private void InitializeForm()
        {
            LoadPopulationGroups();
            LoadRoles();
        }

        private void LoadPopulationGroups()
        {
            comboBoxGroup.Items.Clear();

            foreach (PopulationGroup group in Enum.GetValues(typeof(PopulationGroup)))
            {
                if (group != PopulationGroup._)
                {
                    comboBoxGroup.Items.Add(new
                    {
                        Id = (int)group,
                        Name = group.ToFriendlyString()
                    });
                }
            }

            comboBoxGroup.DisplayMember = "Name";
            comboBoxGroup.ValueMember = "Id";
            comboBoxGroup.SelectedIndex = 0;
        }

        private void LoadRoles()
        {
            comboBoxRole.Items.Clear();

            // Базовые роли для всех администраторов
            comboBoxRole.Items.Add(new { Id = (int)UserRole.User, Name = "Пользователь" });
            

            // Дополнительные роли для супер-админа
            if (_currentUser.Role == UserRole.Super_admin)
            {
                comboBoxRole.Items.Add(new { Id = (int)UserRole.Admin, Name = "Администратор" });
            }

            comboBoxRole.DisplayMember = "Name";
            comboBoxRole.ValueMember = "Id";
            comboBoxRole.SelectedIndex = 0;
        }

        private void buttonCreateUser_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            try
            {
                using (SqlTransaction transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        int newUserId = CreateUser(transaction);
                        LogAdminAction(transaction, "Создание пользователя", 
                                     $"Создан новый пользователь: {txtUserName.Text}");

                        transaction.Commit();
                        
                        MessageBox.Show($"Пользователь успешно создан с ID: {newUserId}", 
                                      "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        HandleException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка соединения с базой данных: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtFullname.Text) ||
                string.IsNullOrWhiteSpace(txtUserName.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Введите корректный email адрес!", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            var selectedRole = (UserRole)((dynamic)comboBoxRole.SelectedItem).Id;
            if ((selectedRole == UserRole.Admin || selectedRole == UserRole.Super_admin) &&
                _currentUser.Role != UserRole.Super_admin)
            {
                MessageBox.Show("Только супер администратор может создавать администраторов!", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private int CreateUser(SqlTransaction transaction)
        {
            string passwordHash = User.HashPassword(txtPassword.Text);
            var selectedRole = (UserRole)((dynamic)comboBoxRole.SelectedItem).Id;

            string query = @"
            INSERT INTO Users (Email, FullName, PhoneNumber, Username, PasswordHash, GroupId, RoleID)
            VALUES (@Email, @FullName, @PhoneNumber, @Username, @PasswordHash, @GroupId, @RoleID);
            SELECT SCOPE_IDENTITY();";

            using (var command = new SqlCommand(query, _connection, transaction))
            {
                command.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                command.Parameters.AddWithValue("@FullName", txtFullname.Text.Trim());
                command.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(txtPhone.Text) 
                    ? (object)DBNull.Value : txtPhone.Text.Trim());
                command.Parameters.AddWithValue("@Username", txtUserName.Text.Trim());
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                command.Parameters.AddWithValue("@GroupId", ((dynamic)comboBoxGroup.SelectedItem).Id);
                command.Parameters.AddWithValue("@RoleID", (int)selectedRole);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private void LogAdminAction(SqlTransaction transaction, string actionType, string actionDetails)
        {
            string query = @"
            INSERT INTO AdminLogs (ActionTime, ActionType, ActionDetails, AdminId, AffectedType)
            VALUES (GETDATE(), N@ActionType, N@ActionDetails, @AdminId, 'User')";

            using (var command = new SqlCommand(query, _connection, transaction))
            {
                command.Parameters.AddWithValue("@ActionType", actionType);
                command.Parameters.AddWithValue("@ActionDetails", actionDetails);
                command.Parameters.AddWithValue("@AdminId", _currentUser.Id);
                command.ExecuteNonQuery();
            }
        }

        private void HandleException(Exception ex)
        {
            if (ex is SqlException sqlEx)
            {
                switch (sqlEx.Number)
                {
                    case 2627: // Ошибка уникальности
                        MessageBox.Show("Пользователь с таким email или именем уже существует!", "Ошибка",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    default:
                        MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", "Ошибка",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            else
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email.Trim();
            }
            catch
            {
                return false;
            }
        }
    }
}