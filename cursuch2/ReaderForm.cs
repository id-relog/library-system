using cursuch2.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace cursuch2
{
    public partial class ReaderForm : Form
    {
        private SqlConnection _connection;
        private User _currentUser;
        private int _selectedBookId = -1;

        public ReaderForm(User user, SqlConnection connection)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            _currentUser = user;
            _connection = connection;
            btnBorrow.Enabled = false;

            // Set default tab to History (tabPage4 based on designer)
            if (tabPageHistory.TabPages.Contains(tabPage4))
            {
                tabPageHistory.SelectedTab = tabPage4;
            }
        }

        private void ReaderForm_Load_1(object sender, EventArgs e)
        {
            LoadBooks();
            ConfigureDataGridView();
            GetReaderData();
            LoadPopulationGroups();
            LoadUserRequestsToDataGrid(); // Load history data on form load
        }

        private void LoadPopulationGroups()
        {
            foreach (PopulationGroup group in Enum.GetValues(typeof(PopulationGroup)))
            {
                newComboBoxGroup.Items.Add(group.ToFriendlyString());
            }
            try
            {
                newComboBoxGroup.SelectedIndex = (int)_currentUser.GroupId - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void GetReaderData()
        {
            txtEmail.Cursor = Cursors.No;
            txtFullName.Cursor = Cursors.No;
            txtGroup.Cursor = Cursors.No;
            txtPhone.Cursor = Cursors.No;

            txtEmail.Text = _currentUser.Email;
            txtFullName.Text = _currentUser.FullName;
            txtGroup.Text = PopulationGroupExtensions.ToFriendlyString(_currentUser.Group);
            txtPhone.Text = _currentUser.PhoneNumber;

            newtxtLogin.Text = _currentUser.Username;
            newTxtFullname.Text = txtFullName.Text;
            newTxtEmail.Text = txtEmail.Text;
            newTxtPhone.Text = txtPhone.Text;
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

        private void ConfigureDataGridView()
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void LoadBooks(string searchText = "")
        {
            try
            {
                dataGridView1.DataSource = SearchBooks(searchText);
                dataGridView1.ClearSelection();
                btnBorrow.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки книг: " + ex.Message);
            }
        }

        private DataTable SearchBooks(string searchText)
        {
            var dataTable = new DataTable();

            string query = @"SELECT 
                b.BookId,
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
                command.Parameters.AddWithValue("@searchText", string.IsNullOrEmpty(searchText) ? "" : searchText);
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(dataTable);
                }
            }

            return dataTable;
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                _selectedBookId = Convert.ToInt32(row.Cells["BookId"].Value);
                int availableCopies = Convert.ToInt32(row.Cells["Доступно экземпляров"].Value);

                if (availableCopies > 0)
                {
                    btnBorrow.Enabled = true;
                    btnBorrow.Text = $"Взять книгу ({availableCopies} доступно)";
                }
                else
                {
                    btnBorrow.Enabled = false;
                    btnBorrow.Text = $"Взять книгу ({availableCopies} доступно)";
                }
            }
        }

        private void btnBorrow_Click_1(object sender, EventArgs e)
        {
            if (_selectedBookId == -1)
            {
                MessageBox.Show("Выберите книгу из списка");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите взять эту книгу?",
                                       "Подтверждение",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    BorrowBook(_selectedBookId);
                    LoadBooks(textBox1.Text.Trim());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BorrowBook(int bookId)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Для выполнения этой операции необходимо авторизоваться",
                               "Ошибка авторизации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string bookTitle;
            try
            {
                using (SqlCommand titleCmd = new SqlCommand(
                    "SELECT Title FROM Books WHERE BookId = @BookId", _connection))
                {
                    titleCmd.Parameters.AddWithValue("@BookId", bookId);
                    bookTitle = titleCmd.ExecuteScalar()?.ToString()
                               ?? throw new Exception("Книга не найдена в базе данных");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске книги: {ex.Message}",
                               "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlTransaction transaction = _connection.BeginTransaction())
            {
                try
                {
                    using (SqlCommand checkLoanCmd = new SqlCommand(
                        @"SELECT COUNT(*) FROM LoanCards 
                  WHERE BookId = @BookId 
                    AND ReaderId = @ReaderId
                    AND ReturnDate IS NULL",
                        _connection, transaction))
                    {
                        checkLoanCmd.Parameters.AddWithValue("@BookId", bookId);
                        checkLoanCmd.Parameters.AddWithValue("@ReaderId", _currentUser.Id);

                        int existingLoans = Convert.ToInt32(checkLoanCmd.ExecuteScalar());
                        if (existingLoans > 0)
                        {
                            throw new Exception("У вас уже есть эта книга");
                        }
                    }

                    using (SqlCommand checkCmd = new SqlCommand(
                        "SELECT CopiesAvailable FROM Books WHERE BookId = @BookId AND CopiesAvailable > 0",
                        _connection, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@BookId", bookId);
                        if (checkCmd.ExecuteScalar() == null)
                        {
                            throw new Exception("Все экземпляры этой книги уже заняты");
                        }
                    }

                    using (SqlCommand checkRequestCmd = new SqlCommand(
                        @"SELECT COUNT(*) FROM BookRequests 
                  WHERE BookId = @BookId 
                    AND ReaderId = @ReaderId 
                    AND Target = 'Borrow'
                    AND Result IS NULL",
                        _connection, transaction))
                    {
                        checkRequestCmd.Parameters.AddWithValue("@BookId", bookId);
                        checkRequestCmd.Parameters.AddWithValue("@ReaderId", _currentUser.Id);

                        int existingRequests = Convert.ToInt32(checkRequestCmd.ExecuteScalar());
                        if (existingRequests > 0)
                        {
                            throw new Exception("У вас уже есть активный запрос на эту книгу");
                        }
                    }

                    string query = @"
                INSERT INTO BookRequests 
                (BookId, ReaderId, AdminId, RequestDate, Target, Result) 
                VALUES 
                (@BookId, @ReaderId, NULL, GETDATE(), 'Borrow', NULL);
                
                SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(query, _connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@BookId", bookId);
                        cmd.Parameters.AddWithValue("@ReaderId", _currentUser.Id);
                        int requestId = Convert.ToInt32(cmd.ExecuteScalar());

                        transaction.Commit();

                        Console.WriteLine($"Создан запрос #{requestId} на книгу '{bookTitle}'");
                        MessageBox.Show($"Ваш запрос на книгу '{bookTitle}' успешно создан.\nНомер запроса: {requestId}",
                                       "Запрос создан", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine($"Ошибка при создании запроса: {ex.Message}");
                    MessageBox.Show($"Не удалось создать запрос: {ex.Message}",
                                   "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            LoadBooks(textBox1.Text.Trim());
        }

        private void tabPageHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabPageHistory.SelectedTab == tabPage4) // History tab
            {
                GetReaderData();
                LoadPopulationGroups();
            }
            else if (tabPageHistory.SelectedTab == tabPage2) // Settings tab
            {
                LoadUserRequestsToDataGrid();
            }
        }

        private void LoadUserRequestsToDataGrid()
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Ошибка: Пользователь не авторизован.");
                return;
            }

            string query = @"
        SELECT 
            b.Title AS 'Название книги',
            STRING_AGG(a.FullName, ', ') AS 'Автор(ы)',
            br.RequestDate AS 'Дата запроса',
            br.ResponseDate AS 'Дата ответа',
            CASE 
                WHEN br.Target = 'Borrow' THEN N'Взятие'
                WHEN br.Target = 'Return' THEN N'Возврат'
                ELSE br.Target
            END AS 'Тип запроса',
            CASE 
                WHEN br.Result = 'Accepted' THEN N'Одобрено'
                WHEN br.Result = 'Rejected' THEN N'Отклонено'
                WHEN br.Result IS NULL THEN N'В обработке'
                ELSE br.Result
            END AS 'Статус',
            CASE
                WHEN br.ResponseDate IS NOT NULL THEN DATEDIFF(day, br.RequestDate, br.ResponseDate)
                ELSE NULL
            END AS 'Дней на рассмотрение'
        FROM BookRequests br
        JOIN Books b ON br.BookId = b.BookId
        LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId
        LEFT JOIN Authors a ON ba.AuthorId = a.AuthorId
        WHERE br.ReaderId = @ReaderId
        GROUP BY b.Title, br.RequestDate, br.ResponseDate, br.Target, br.Result
        ORDER BY br.RequestDate DESC";

            try
            {
                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@ReaderId", _currentUser.Id);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    dataGridHistoryAndReturn.DataSource = dataTable;
                    FormatDataGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке истории запросов: {ex.Message}");
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                var changedFields = new Dictionary<string, object>();
                bool hasChanges = false;
                var changesSummary = new StringBuilder("Будут изменены следующие данные:\n\n");

                if (newTxtFullname.Enabled && newTxtFullname.Text != _currentUser.FullName)
                {
                    if (string.IsNullOrWhiteSpace(newTxtFullname.Text))
                    {
                        MessageBox.Show("ФИО не может быть пустым");
                        return;
                    }
                    changedFields.Add("FullName", newTxtFullname.Text.Trim());
                    changesSummary.AppendLine($"ФИО: {_currentUser.FullName} → {newTxtFullname.Text.Trim()}");
                    hasChanges = true;
                }

                if (newTxtEmail.Enabled && newTxtEmail.Text != _currentUser.Email)
                {
                    if (!IsValidEmail(newTxtEmail.Text))
                    {
                        MessageBox.Show("Введите корректный email");
                        return;
                    }

                    string checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email AND ReaderId != @ReaderId";
                    using (SqlCommand checkCmd = new SqlCommand(checkEmailQuery, _connection))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", newTxtEmail.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@ReaderId", _currentUser.Id);

                        if ((int)checkCmd.ExecuteScalar() > 0)
                        {
                            MessageBox.Show("Пользователь с таким email уже существует");
                            return;
                        }
                    }
                    changedFields.Add("Email", newTxtEmail.Text.Trim());
                    changesSummary.AppendLine($"Email: {_currentUser.Email} → {newTxtEmail.Text.Trim()}");
                    hasChanges = true;
                }

                if (newTxtPhone.Enabled)
                {
                    string newPhone = string.IsNullOrWhiteSpace(newTxtPhone.Text) ? null : newTxtPhone.Text.Trim();
                    if (newPhone != _currentUser.PhoneNumber)
                    {
                        if (newPhone != null && !IsValidPhoneNumber(newPhone))
                        {
                            MessageBox.Show("Введите корректный номер телефона");
                            return;
                        }
                        changedFields.Add("PhoneNumber", newPhone ?? (object)DBNull.Value);
                        string currentPhone = _currentUser.PhoneNumber ?? "[не указан]";
                        string newPhoneDisplay = newPhone ?? "[не указан]";
                        changesSummary.AppendLine($"Телефон: {currentPhone} → {newPhoneDisplay}");
                        hasChanges = true;
                    }
                }

                if (newComboBoxGroup.Enabled && newComboBoxGroup.SelectedIndex + 1 != _currentUser.GroupId)
                {
                    changedFields.Add("GroupId", newComboBoxGroup.SelectedIndex + 1);
                    var newGroupName = newComboBoxGroup.Text;
                    var currentGroupName = PopulationGroupExtensions.ToFriendlyString(_currentUser.Group);
                    changesSummary.AppendLine($"Группа: {currentGroupName} → {newGroupName}");
                    hasChanges = true;
                }

                if (!hasChanges)
                {
                    MessageBox.Show("Нет изменений для сохранения");
                    return;
                }

                if (!User.HashPassword(txtPasswordConfirm2.Text).Equals(_currentUser.PasswordHash))
                {
                    MessageBox.Show("Неверный пароль для подтверждения изменений");
                    return;
                }

                changesSummary.Append("\nВы уверены, что хотите сохранить эти изменения?");
                var confirmResult = MessageBox.Show(
                    changesSummary.ToString(),
                    "Подтверждение изменений",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmResult != DialogResult.Yes)
                {
                    return;
                }

                var setClauses = new List<string>();
                var parameters = new List<SqlParameter>();

                foreach (var field in changedFields)
                {
                    setClauses.Add($"{field.Key} = @{field.Key}");
                    parameters.Add(new SqlParameter($"@{field.Key}", field.Value));
                }

                string query = $"UPDATE Users SET {string.Join(", ", setClauses)} WHERE ReaderId = @ReaderId";
                parameters.Add(new SqlParameter("@ReaderId", _currentUser.Id));

                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        if (changedFields.ContainsKey("FullName"))
                            _currentUser.FullName = newTxtFullname.Text.Trim();
                        if (changedFields.ContainsKey("Email"))
                            _currentUser.Email = newTxtEmail.Text.Trim();
                        if (changedFields.ContainsKey("PhoneNumber"))
                            _currentUser.PhoneNumber = string.IsNullOrWhiteSpace(newTxtPhone.Text) ? null : newTxtPhone.Text.Trim();
                        if (changedFields.ContainsKey("GroupId"))
                        {
                            _currentUser.GroupId = newComboBoxGroup.SelectedIndex + 1;
                            _currentUser.Group = (PopulationGroup)(newComboBoxGroup.SelectedIndex + 1);
                        }

                        MessageBox.Show("Данные успешно обновлены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        GetReaderData();

                        newTxtFullname.Enabled = false;
                        newTxtEmail.Enabled = false;
                        newTxtPhone.Enabled = false;
                        newComboBoxGroup.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Не удалось обновить данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidPhoneNumber(string phone)
        {
            string digitsOnly = new string(phone.Where(c => char.IsDigit(c)).ToArray());
            return digitsOnly.Length >= 10 && digitsOnly.Length <= 15;
        }

        private void buttonChangeValue1_Click(object sender, EventArgs e)
        {
            if (newTxtFullname.Enabled == false)
            {
                newTxtFullname.Enabled = true;
            }
            else
            {
                newTxtFullname.Enabled = false;
                newTxtFullname.Text = _currentUser.FullName;
            }
        }

        private void buttonChangeValue2_Click(object sender, EventArgs e)
        {
            if (newTxtEmail.Enabled == false)
            {
                newTxtEmail.Enabled = true;
            }
            else
            {
                newTxtEmail.Enabled = false;
                newTxtEmail.Text = _currentUser.Email;
            }
        }

        private void buttonChangeValue3_Click(object sender, EventArgs e)
        {
            if (newTxtPhone.Enabled == false)
            {
                newTxtPhone.Enabled = true;
            }
            else
            {
                newTxtPhone.Enabled = false;
                newTxtPhone.Text = _currentUser.PhoneNumber;
            }
        }

        private void buttonChangeValue4_Click(object sender, EventArgs e)
        {
            if (newComboBoxGroup.Enabled == false)
            {
                newComboBoxGroup.Enabled = true;
            }
            else
            {
                newComboBoxGroup.Enabled = false;
                newComboBoxGroup.Text = PopulationGroupExtensions.ToFriendlyString(_currentUser.Group);
            }
        }

        private void buttonUpdate2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!User.HashPassword(txtPasswordConfirm1.Text).Equals(_currentUser.PasswordHash))
                {
                    MessageBox.Show("Введите верный текущий пароль чтобы изменить данные", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool loginChanged = newtxtLogin.Enabled && newtxtLogin.Text.Trim() != _currentUser.Username;
                bool passwordChanged = newtxtPassword.Enabled && !string.IsNullOrEmpty(newtxtPassword.Text);

                if (!loginChanged && !passwordChanged)
                {
                    MessageBox.Show("Нет изменений для сохранения", "Информация",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var changesSummary = new StringBuilder("Будут изменены следующие данные:\n\n");

                if (loginChanged)
                {
                    if (string.IsNullOrWhiteSpace(newtxtLogin.Text))
                    {
                        MessageBox.Show("Логин не может быть пустым", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string checkLoginQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND ReaderId != @ReaderId";
                    using (SqlCommand checkCmd = new SqlCommand(checkLoginQuery, _connection))
                    {
                        checkCmd.Parameters.AddWithValue("@Username", newtxtLogin.Text.Trim());
                        checkCmd.Parameters.AddWithValue("@ReaderId", _currentUser.Id);

                        if ((int)checkCmd.ExecuteScalar() > 0)
                        {
                            MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    changesSummary.AppendLine($"Логин: {_currentUser.Username} → {newtxtLogin.Text.Trim()}");
                }

                if (passwordChanged)
                {
                    if (newtxtPassword.Text != newtxtConfirmPassword.Text)
                    {
                        MessageBox.Show("Пароли не совпадают", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (newtxtPassword.Text.Length < 6)
                    {
                        MessageBox.Show("Пароль должен содержать минимум 6 символов", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    changesSummary.AppendLine("Пароль: [будет изменен]");
                }

                changesSummary.Append("\nВы уверены, что хотите сохранить эти изменения?");
                var confirmResult = MessageBox.Show(
                    changesSummary.ToString(),
                    "Подтверждение изменений",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmResult != DialogResult.Yes)
                {
                    return;
                }

                var setClauses = new List<string>();
                var parameters = new List<SqlParameter>();

                if (loginChanged)
                {
                    setClauses.Add("Username = @Username");
                    parameters.Add(new SqlParameter("@Username", newtxtLogin.Text.Trim()));
                }

                if (passwordChanged)
                {
                    setClauses.Add("PasswordHash = @PasswordHash");
                    parameters.Add(new SqlParameter("@PasswordHash", User.HashPassword(newtxtPassword.Text)));
                }

                string query = $"UPDATE Users SET {string.Join(", ", setClauses)} WHERE ReaderId = @ReaderId";
                parameters.Add(new SqlParameter("@ReaderId", _currentUser.Id));

                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        if (loginChanged)
                            _currentUser.Username = newtxtLogin.Text.Trim();
                        if (passwordChanged)
                            _currentUser.PasswordHash = User.HashPassword(newtxtPassword.Text);

                        MessageBox.Show("Данные успешно обновлены!", "Успех",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ResetPasswordChangeFields();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось обновить данные", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonChangeLogin1_Click(object sender, EventArgs e)
        {
            if (newtxtLogin.Enabled == false)
            {
                newtxtLogin.Enabled = true;
            }
            else
            {
                newtxtLogin.Enabled = false;
                newtxtLogin.Text = _currentUser.Username;
            }
        }

        private void buttonChangePassword2_Click(object sender, EventArgs e)
        {
            if (newtxtPassword.Enabled == false)
            {
                newtxtPassword.Enabled = true;
                labeConfirmPassword.Visible = true;
                newtxtConfirmPassword.Visible = true;
            }
            else
            {
                ResetPasswordChangeFields();
            }
        }

        private void ResetPasswordChangeFields()
        {
            newtxtPassword.Enabled = false;
            newtxtLogin.Enabled = false;
            newtxtLogin.Text = _currentUser.Username;
            txtPasswordConfirm1.Text = "";
            labeConfirmPassword.Visible = false;
            newtxtConfirmPassword.Visible = false;
            newtxtConfirmPassword.Text = "";
            newtxtPassword.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (txtSearchHistory.Text == null)
            {
                throw new Exception("ошибка textbox");
            }

            string searchText = $"%{txtSearchHistory.Text.Trim()}%";

            try
            {
                string query = @"
            SELECT 
                b.Title AS 'Название книги',
                (
                    SELECT STRING_AGG(a.FullName, ', ')
                    FROM Authors a
                    JOIN BookAuthors ba ON a.AuthorId = ba.AuthorId
                    WHERE ba.BookId = b.BookId
                ) AS 'Автор(ы)',
                br.RequestDate AS 'Дата запроса',
                br.ResponseDate AS 'Дата ответа',
                CASE 
                    WHEN br.Target = 'Borrow' THEN N'Взятие'
                    WHEN br.Target = 'Return' THEN N'Возврат'
                    ELSE br.Target
                END AS 'Тип запроса',
                CASE 
                    WHEN br.Result = 'Accepted' THEN N'Одобрено'
                    WHEN br.Result = 'Rejected' THEN N'Отклонено'
                    WHEN br.Result IS NULL THEN N'В обработке'
                    ELSE br.Result
                END AS 'Статус',
                CASE
                    WHEN br.ResponseDate IS NOT NULL THEN DATEDIFF(day, br.RequestDate, br.ResponseDate)
                    ELSE NULL
                END AS 'Дней на рассмотрение'
            FROM BookRequests br
            JOIN Books b ON br.BookId = b.BookId
            WHERE br.ReaderId = @ReaderId
              AND (b.Title LIKE @SearchText 
                   OR EXISTS (
                       SELECT 1 
                       FROM BookAuthors ba 
                       JOIN Authors a ON ba.AuthorId = a.AuthorId
                       WHERE ba.BookId = b.BookId 
                         AND a.FullName LIKE @SearchText
                   ))
            GROUP BY b.BookId, b.Title, br.RequestDate, br.ResponseDate, br.Target, br.Result
            ORDER BY br.RequestDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@ReaderId", _currentUser.Id);
                    cmd.Parameters.AddWithValue("@SearchText", searchText);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("Ничего не найдено по вашему запросу", "Результаты поиска",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    dataGridHistoryAndReturn.DataSource = dataTable;
                    FormatDataGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске: {ex.Message}", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatDataGridView()
        {
            if (dataGridHistoryAndReturn.Columns.Count == 0) return;

            dataGridHistoryAndReturn.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridHistoryAndReturn.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 9);

            if (dataGridHistoryAndReturn.Columns.Contains("Дата запроса"))
            {
                dataGridHistoryAndReturn.Columns["Дата запроса"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
            }

            if (dataGridHistoryAndReturn.Columns.Contains("Дата ответа"))
            {
                dataGridHistoryAndReturn.Columns["Дата ответа"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
                dataGridHistoryAndReturn.Columns["Дата ответа"].DefaultCellStyle.NullValue = "Ещё не обработано";
            }

            dataGridHistoryAndReturn.RowPrePaint += (sender, e) =>
            {
                if (dataGridHistoryAndReturn.Rows[e.RowIndex].Cells["Статус"].Value?.ToString() == "Отклонено")
                {
                    dataGridHistoryAndReturn.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightPink;
                }
                else if (dataGridHistoryAndReturn.Rows[e.RowIndex].Cells["Статус"].Value?.ToString() == "Одобрено")
                {
                    dataGridHistoryAndReturn.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                }
            };
        }

        private void buttonSearchReturnBooks_Click(object sender, EventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Пользователь не авторизован", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string searchText = string.IsNullOrWhiteSpace(textBoxReturnBook.Text)
                    ? "%"
                    : $"%{textBoxReturnBook.Text.Trim()}%";

                string query = @"
            SELECT 
                b.BookId AS 'ID книги',
                b.Title AS 'Название книги',
                STRING_AGG(a.FullName, ', ') AS 'Автор(ы)',
                COUNT(*) AS 'Количество у вас',
                lc.IssueDate AS 'Дата выдачи',
                lc.DueDate AS 'Дата возврата',
                DATEDIFF(day, GETDATE(), lc.DueDate) AS 'Осталось дней',
                CASE 
                    WHEN lc.ReturnDate IS NOT NULL THEN N'Возвращена'
                    WHEN GETDATE() > lc.DueDate THEN N'Просрочена'
                    ELSE N'На руках'
                END AS 'Статус'
            FROM LoanCards lc
            JOIN Books b ON lc.BookId = b.BookId
            LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId
            LEFT JOIN Authors a ON ba.AuthorId = a.AuthorId
            WHERE lc.ReaderId = @ReaderId
              AND lc.ReturnDate IS NULL
              AND lc.Status = 'Active'
              AND (b.Title LIKE @SearchText 
                   OR EXISTS (
                       SELECT 1 FROM BookAuthors ba2
                       JOIN Authors a2 ON ba2.AuthorId = a2.AuthorId
                       WHERE ba2.BookId = b.BookId
                       AND a2.FullName LIKE @SearchText
                   ))
            GROUP BY b.BookId, b.Title, lc.IssueDate, lc.DueDate, lc.ReturnDate
            ORDER BY lc.DueDate ASC";

                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@ReaderId", _currentUser.Id);
                    cmd.Parameters.AddWithValue("@SearchText", searchText);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count == 0)
                    {
                        string message = string.IsNullOrWhiteSpace(textBoxReturnBook.Text)
                            ? "У вас нет книг на руках"
                            : $"Не найдено книг по запросу: '{textBoxReturnBook.Text.Trim()}'";

                        MessageBox.Show(message, "Информация",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    dataGridViewReturnBooks.DataSource = dataTable;
                    FormatReturnBooksDataGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatReturnBooksDataGrid()
        {
            if (dataGridViewReturnBooks.Columns.Count == 0) return;

            dataGridViewReturnBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewReturnBooks.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 9);
            dataGridViewReturnBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewReturnBooks.MultiSelect = true;

            string[] dateColumns = { "Дата выдачи", "Дата возврата" };
            foreach (var colName in dateColumns)
            {
                if (dataGridViewReturnBooks.Columns.Contains(colName))
                {
                    dataGridViewReturnBooks.Columns[colName].DefaultCellStyle.Format = "dd.MM.yyyy";
                }
            }

            if (dataGridViewReturnBooks.Columns.Contains("Количество у вас"))
            {
                dataGridViewReturnBooks.Columns["Количество у вас"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
            }

            dataGridViewReturnBooks.RowPrePaint += (sender, e) =>
            {
                var grid = sender as DataGridView;
                var row = grid.Rows[e.RowIndex];

                if (row.Cells["Статус"].Value?.ToString() == "Просрочена")
                {
                    row.DefaultCellStyle.BackColor = Color.LightPink;
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
                else if (row.Cells["Статус"].Value?.ToString() == "На руках")
                {
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                }

                if (row.Selected)
                {
                    row.DefaultCellStyle.SelectionBackColor = Color.LightBlue;
                    row.DefaultCellStyle.SelectionForeColor = Color.Black;
                }
            };

            if (dataGridViewReturnBooks.Columns.Contains("ID книги"))
            {
                dataGridViewReturnBooks.Columns["ID книги"].Visible = false;
            }

            dataGridViewReturnBooks.Columns["Количество у вас"].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewReturnBooks.Columns["Осталось дней"].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void buttonReturnBook_Click(object sender, EventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Пользователь не авторизован", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dataGridViewReturnBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одну книгу для возврата", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlTransaction transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        List<int> processedBooks = new List<int>();
                        bool hasDuplicates = false;
                        StringBuilder duplicateMessage = new StringBuilder();

                        foreach (DataGridViewRow row in dataGridViewReturnBooks.SelectedRows)
                        {
                            if (row.Cells["ID книги"].Value != null)
                            {
                                int bookId = Convert.ToInt32(row.Cells["ID книги"].Value);
                                string bookTitle = row.Cells["Название книги"].Value.ToString();

                                if (processedBooks.Contains(bookId))
                                {
                                    duplicateMessage.AppendLine($"- {bookTitle} (уже выбран)");
                                    hasDuplicates = true;
                                    continue;
                                }

                                string checkQuery = @"
                            SELECT COUNT(*) 
                            FROM BookRequests 
                            WHERE BookId = @BookId 
                              AND ReaderId = @ReaderId 
                              AND Target = 'Return'
                              AND Result IS NULL";

                                using (SqlCommand checkCmd = new SqlCommand(checkQuery, _connection, transaction))
                                {
                                    checkCmd.Parameters.AddWithValue("@BookId", bookId);
                                    checkCmd.Parameters.AddWithValue("@ReaderId", _currentUser.Id);

                                    int existingRequests = Convert.ToInt32(checkCmd.ExecuteScalar());
                                    if (existingRequests > 0)
                                    {
                                        duplicateMessage.AppendLine($"- {bookTitle} (уже есть активный запрос)");
                                        hasDuplicates = true;
                                        continue;
                                    }
                                }

                                string requestQuery = @"
                            INSERT INTO BookRequests 
                            (BookId, ReaderId, AdminId, RequestDate, Target, Result) 
                            VALUES 
                            (@BookId, @ReaderId, NULL, GETDATE(), 'Return', NULL)";

                                using (SqlCommand cmd = new SqlCommand(requestQuery, _connection, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@BookId", bookId);
                                    cmd.Parameters.AddWithValue("@ReaderId", _currentUser.Id);
                                    cmd.ExecuteNonQuery();
                                }

                                processedBooks.Add(bookId);
                                Console.WriteLine($"Создан запрос на возврат книги '{bookTitle}'");
                            }
                        }

                        if (hasDuplicates)
                        {
                            MessageBox.Show($"Некоторые книги не были обработаны:\n{duplicateMessage}",
                                          "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        if (processedBooks.Count > 0)
                        {
                            transaction.Commit();
                            MessageBox.Show($"Запросы на возврат {processedBooks.Count} книг успешно созданы",
                                         "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            buttonSearchReturnBooks_Click(null, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при создании запроса на возврат: {ex.Message}",
                                     "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}