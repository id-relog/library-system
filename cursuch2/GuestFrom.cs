namespace cursuch2
{
    using cursuch2.Models;
    using Microsoft.Data.SqlClient;
    using System.Configuration;
    using System.Data;
    
    public partial class GuestFrom : Form
    {
        private SqlConnection _connection;
        private User _currentUser;

        public GuestFrom()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            _connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString);
            _currentUser = new User();
        }



        private void LoadBooks(string searchText = "")
        {
            try
            {
                dataGridView1.DataSource = SearchBooks(searchText);
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки книг: " + ex.Message);
            }
        }

        private void GuestFrom_Load(object sender, EventArgs e)
        {
            try
            {
                _connection.Open();
                LoadBooks();
                LoadPopulationGroups();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}");
                Close();
            }
        }

        private void LoadPopulationGroups()
        {
            foreach (PopulationGroup group in Enum.GetValues(typeof(PopulationGroup)))
            {
                cmbGroup.Items.Add(group.ToFriendlyString());
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            string searchText = textBox1.Text.Trim();

            if (searchText==null)
            {
                MessageBox.Show("Введите текст для поиска");
                return;
            }

            try
            {
                dataGridView1.DataSource = SearchBooks(searchText);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка поиска: " + ex.Message);
            }
        }

        private DataTable SearchBooks(string searchText)
        {
            var dataTable = new DataTable();

            string query = @"SELECT 
                b.Title AS 'Название книги',
                a.FullName AS 'Автор',
                p.PublisherName AS 'Издательство',
                b.PublicationYear AS 'Год издания',
                b.CopiesAvailable AS 'Доступно экземпляров'
                FROM Books b
                JOIN BookAuthors ba ON b.BookId = ba.BookId
                JOIN Authors a ON ba.AuthorId = a.AuthorId
                JOIN Publishers p ON b.PublisherId = p.PublisherId
                WHERE b.Title LIKE N'%' + @searchText + '%' 
                    OR a.FullName LIKE N'%' + @searchText + '%'
                ORDER BY b.Title";

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@searchText", searchText);
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }
            }

            return dataTable;
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (!ValidateRegistration())
                return;

            try
            {
                if (RegisterUser())
                {
                    MessageBox.Show("Регистрация успешна! Теперь вы можете войти в систему.");
                    tabControl1.SelectedIndex = 2;
                    ClearRegistrationForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка регистрации: " + ex.Message);
            }
        }

        private bool RegisterUser()
        {
            string query = @"INSERT INTO Users 
        (FullName, Email, PhoneNumber, Username, PasswordHash, GroupId, RoleId)
        VALUES 
        (@FullName, @Email, @Phone, @Username, @PasswordHash, @GroupId, 1)"; // По умолчанию RoleId = 1 (обычный пользователь)

            using (SqlCommand cmd = new SqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@FullName", txtFullName.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
                cmd.Parameters.AddWithValue("@PasswordHash", User.HashPassword(txtPassword.Text));
                cmd.Parameters.AddWithValue("@GroupId", GetGroupId(cmbGroup.Text));

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private bool ValidateRegistration()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИО");
                return false;
            }
            if (!txtFullName.Text.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            {
                MessageBox.Show("ФИО должно содержать только буквы и пробелы");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Введите номер телефона");
                return false;
            }
            if (!txtPhone.Text.All(char.IsDigit))
            {
                MessageBox.Show("Номер телефона должен содержать только цифры");
                return false;
            }

            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Введите корректный email");
                return false;
            }

            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов");
                return false;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Пароли не совпадают");
                return false;
            }

            return true;
        }

        private int GetGroupId(string groupName)
        {
            string query = "SELECT GroupId FROM PopulationGroups WHERE GroupName = @GroupName";

            using (SqlCommand cmd = new SqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@GroupName", groupName);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ClearRegistrationForm()
        {
            txtFullName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string username = loginTxt.Text.Trim();
            string password = passwordTxt.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль");
                return;
            }

            try
            {
                _currentUser = AuthenticateUser(username, password);

                if (_currentUser != null)
                {
                    MessageBox.Show($"Вход выполнен успешно! Добро пожаловать, {_currentUser.FullName}");

                    if (_currentUser.Role == UserRole.User)
                    {
                        this.Hide();

                        ReaderForm readerForm = new ReaderForm(_currentUser, _connection);
                        readerForm.FormClosed += (s, args) => this.Show();
                        readerForm.Show();
                    }
                    else if (_currentUser.Role == UserRole.Admin)
                    {
                        this.Hide();

                        AdminForm adminForm = new AdminForm(_currentUser, _connection);

                        adminForm.FormClosed += (s, args) => this.Show();

                        adminForm.Show();
                    }
                    else if(_currentUser.Role == UserRole.Super_admin)
                    {
                        AdminForm adminForm = new AdminForm(_currentUser, _connection);

                        adminForm.FormClosed += (s, args) => this.Show();

                        adminForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Ваш аккаунт заблакирован, обратитеть лично к админестратору.","Ограничение доступа",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при входе: {ex.Message}");
            }
        }

        private User AuthenticateUser(string username, string password)
        {
            string query = @"SELECT ReaderId, FullName, Email, PhoneNumber, Username, PasswordHash, GroupId, RoleId
             FROM Users
             WHERE Username = @Username";

            using (SqlCommand cmd = new SqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@Username", username);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string storedHash = reader.GetString(5); // PasswordHash
                        string inputHash = User.HashPassword(password);

                        if (storedHash == inputHash)
                        {
                            int groupId = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                            int roleId = reader.GetInt32(7); // RoleId

                            return new User
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Email = reader.GetString(2),
                                PhoneNumber = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Username = reader.GetString(4),
                                PasswordHash = storedHash,
                                GroupId = groupId,
                                Group = PopulationGroupExtensions.GetPopulationGroupByValue(groupId),
                                Role = (UserRole)roleId
                            };
                        }
                    }
                }
            }
            return null;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }
    }
}

