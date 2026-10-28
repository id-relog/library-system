using cursuch2.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace cursuch2
{
    public partial class AdminForm : Form
    {
        private SqlConnection _connection;
        private User _currentAdminId;
        private bool _addingNewBook = false;
        private bool _isEditingBook = false;
        private int _editingBookRowIndex = -1;
        private bool _isEditingMode = false;

        public AdminForm(User adminId, SqlConnection connection)
        {
            InitializeComponent();
            _currentAdminId = adminId;
            _connection = connection;

            this.KeyPreview = true;
            this.KeyDown += AdminForm_KeyDown;

            // Настройки DataGridView for Books
            dataGridViewBookEdit.Dock = DockStyle.Fill;
            dataGridViewBookEdit.ReadOnly = true;
            dataGridViewBookEdit.AllowUserToAddRows = false;
            dataGridViewBookEdit.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Добавляем обработчики событий
            dataGridViewBookEdit.CellEndEdit += dataGridViewBookEdit_CellEndEdit;
            dataGridViewBookEdit.CellValidating += dataGridViewBookEdit_CellValidating;
            dataGridViewBookEdit.RowValidated += dataGridViewBookEdit_RowValidated;

            LoadBooks();
            LoadUsers();
            LoadRequests();
        }

        private void CancelBookEditing()
        {
            if (!_isEditingBook) return;

            var result = MessageBox.Show("Отменить добавление новой книги?", "Подтверждение",
                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    DataTable dt = (DataTable)dataGridViewBookEdit.DataSource;
                    dt.Rows.RemoveAt(_editingBookRowIndex);
                    dataGridViewBookEdit.DataSource = dt;

                    ResetBookEditingState();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отмене: {ex.Message}", "Ошибка",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void AdminForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && _isEditingBook)
            {
                CancelBookEditing();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter && _isEditingBook)
            {
                if (ValidateCurrentBook())
                {
                    SaveCurrentBook();
                    ResetBookEditingState();
                    LoadBooks();
                }
                e.Handled = true;
            }
        }

        private void LogAdminAction(string actionType, string actionDetails, int? affectedId = null, string affectedType = null, SqlTransaction transaction = null)
        {
            try
            {
                string query = @"
            INSERT INTO AdminLogs 
                (ActionTime, ActionType, ActionDetails, AdminId, AffectedId, AffectedType)
            VALUES 
                (@ActionTime, N@ActionType, N@ActionDetails, @AdminId, @AffectedId, N@AffectedType)";

                using (SqlCommand cmd = new SqlCommand(query, _connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@ActionTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ActionType", actionType);
                    cmd.Parameters.AddWithValue("@ActionDetails", actionDetails);
                    cmd.Parameters.AddWithValue("@AdminId", _currentAdminId.Id);

                    if (affectedId.HasValue)
                        cmd.Parameters.AddWithValue("@AffectedId", affectedId.Value);
                    else
                        cmd.Parameters.AddWithValue("@AffectedId", DBNull.Value);

                    if (!string.IsNullOrEmpty(affectedType))
                        cmd.Parameters.AddWithValue("@AffectedType", affectedType);
                    else
                        cmd.Parameters.AddWithValue("@AffectedType", DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при логировании действия администратора: {ex.Message}");
            }
        }


        //первая вкладка: управление книгами
        private void buttonSearchBooks_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText = txtBookSearch.Text.Trim();

                string query = @"
            SELECT 
                b.BookId,
                b.Title,
                p.PublisherName AS Publisher,
                b.PublicationYear,
                b.CopiesAvailable,
                b.TotalCopies
            FROM Books b
            LEFT JOIN Publishers p ON b.PublisherId = p.PublisherId
            WHERE 
                b.Title LIKE @search OR
                p.PublisherName LIKE @search OR
                (b.PublicationYear IS NOT NULL AND 
                 CAST(b.PublicationYear AS NVARCHAR(10)) LIKE @search)"; // Увеличили длину NVARCHAR до 10

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@search", "%" + searchText + "%");

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                // Добавляем информацию о жанрах и авторах (аналогично LoadBooks)
                dt.Columns.Add("Genres", typeof(string));
                dt.Columns.Add("Authors", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    int bookId = Convert.ToInt32(row["BookId"]);

                    // Получаем жанры
                    string genresQuery = @"
                SELECT g.GenreName 
                FROM BookGenres bg 
                INNER JOIN Genres g ON bg.GenreId = g.GenreId 
                WHERE bg.BookId = @bookId";

                    using (SqlCommand genreCmd = new SqlCommand(genresQuery, _connection))
                    {
                        genreCmd.Parameters.AddWithValue("@bookId", bookId);
                        using (SqlDataReader reader = genreCmd.ExecuteReader())
                        {
                            List<string> genres = new List<string>();
                            while (reader.Read())
                            {
                                genres.Add(reader["GenreName"].ToString());
                            }
                            row["Genres"] = string.Join(", ", genres);
                        }
                    }

                    // Получаем авторов
                    string authorsQuery = @"
                SELECT a.FullName 
                FROM BookAuthors ba 
                INNER JOIN Authors a ON ba.AuthorId = a.AuthorId 
                WHERE ba.BookId = @bookId";

                    using (SqlCommand authorCmd = new SqlCommand(authorsQuery, _connection))
                    {
                        authorCmd.Parameters.AddWithValue("@bookId", bookId);
                        using (SqlDataReader reader = authorCmd.ExecuteReader())
                        {
                            List<string> authors = new List<string>();
                            while (reader.Read())
                            {
                                authors.Add(reader["FullName"].ToString());
                            }
                            row["Authors"] = string.Join(", ", authors);
                        }
                    }
                }

                dataGridViewBookEdit.DataSource = dt;
                ConfigureDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске книг: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //

        private void LoadBooks()
        {
            try
            {
                // Основной запрос для получения информации о книгах
                string query = @"
            SELECT 
                b.BookId,
                b.Title,
                p.PublisherName AS Publisher,
                b.PublicationYear,
                b.CopiesAvailable,
                b.TotalCopies
            FROM Books b
            LEFT JOIN Publishers p ON b.PublisherId = p.PublisherId";

                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }

                    // Добавляем колонки для жанров и авторов
                    dt.Columns.Add("Genres", typeof(string));
                    dt.Columns.Add("Authors", typeof(string));

                    // Для каждой книги получаем жанры и авторов
                    foreach (DataRow row in dt.Rows)
                    {
                        int bookId = Convert.ToInt32(row["BookId"]);

                        // Запрос для жанров
                        string genresQuery = @"
                    SELECT g.GenreName 
                    FROM BookGenres bg
                    INNER JOIN Genres g ON bg.GenreId = g.GenreId
                    WHERE bg.BookId = @bookId";

                        using (SqlCommand genreCmd = new SqlCommand(genresQuery, _connection))
                        {
                            genreCmd.Parameters.AddWithValue("@bookId", bookId);
                            using (SqlDataReader reader = genreCmd.ExecuteReader())
                            {
                                List<string> genres = new List<string>();
                                while (reader.Read())
                                {
                                    genres.Add(reader["GenreName"].ToString());
                                }
                                row["Genres"] = string.Join(", ", genres);
                            }
                        }

                        // Запрос для авторов
                        string authorsQuery = @"
                    SELECT a.FullName 
                    FROM BookAuthors ba
                    INNER JOIN Authors a ON ba.AuthorId = a.AuthorId
                    WHERE ba.BookId = @bookId";

                        using (SqlCommand authorCmd = new SqlCommand(authorsQuery, _connection))
                        {
                            authorCmd.Parameters.AddWithValue("@bookId", bookId);
                            using (SqlDataReader reader = authorCmd.ExecuteReader())
                            {
                                List<string> authors = new List<string>();
                                while (reader.Read())
                                {
                                    authors.Add(reader["FullName"].ToString());
                                }
                                row["Authors"] = string.Join(", ", authors);
                            }
                        }
                    }

                    dataGridViewBookEdit.DataSource = dt;
                    ConfigureDataGridView();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке книг: " + ex.Message);
            }
        }

        private void ConfigureDataGridView()
        {
            dataGridViewBookEdit.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewBookEdit.Dock = DockStyle.Fill;
            dataGridViewBookEdit.ReadOnly = true;
            dataGridViewBookEdit.AllowUserToAddRows = false;

            if (dataGridViewBookEdit.Columns.Contains("BookId"))
                dataGridViewBookEdit.Columns["BookId"].Visible = false;

            dataGridViewBookEdit.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewBookEdit.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10);
            dataGridViewBookEdit.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        }

        private void buttonAddNewBook_Click(object sender, EventArgs e)
        {

            this.Hide();
            FormNewBook formNewBook = new FormNewBook(_currentAdminId.Id, _connection);
            formNewBook.FormClosed += (s, args) => this.Show();

            formNewBook.Show();
        }



        private bool ValidateCurrentBook()
        {
            DataGridViewRow row = dataGridViewBookEdit.Rows[_editingBookRowIndex];
            bool isValid = true;

            // Очистка предыдущих ошибок
            foreach (DataGridViewCell cell in row.Cells)
            {
                cell.ErrorText = string.Empty;
            }

            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(row.Cells["Title"].Value?.ToString()))
            {
                row.Cells["Title"].ErrorText = "Название книги обязательно";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(row.Cells["Publisher"].Value?.ToString()))
            {
                row.Cells["Publisher"].ErrorText = "Издатель обязателен";
                isValid = false;
            }

            // Проверка числовых полей
            if (!int.TryParse(row.Cells["CopiesAvailable"].Value?.ToString(), out int copies) || copies < 0)
            {
                row.Cells["CopiesAvailable"].ErrorText = "Должно быть положительное число";
                isValid = false;
            }

            if (!int.TryParse(row.Cells["TotalCopies"].Value?.ToString(), out int total) || total < 0)
            {
                row.Cells["TotalCopies"].ErrorText = "Должно быть положительное число";
                isValid = false;
            }
            else if (total < copies)
            {
                row.Cells["TotalCopies"].ErrorText = "Общее количество не может быть меньше доступного";
                isValid = false;
            }

            // Проверка года публикации
            if (!string.IsNullOrWhiteSpace(row.Cells["PublicationYear"].Value?.ToString()))
            {
                if (!int.TryParse(row.Cells["PublicationYear"].Value.ToString(), out int year) ||
                    year < 1000 || year > DateTime.Now.Year + 1)
                {
                    row.Cells["PublicationYear"].ErrorText = $"Год должен быть между 1000 и {DateTime.Now.Year + 1}";
                    isValid = false;
                }
            }

            return isValid;
        }

        private void SaveCurrentBook()
        {
            DataGridViewRow row = dataGridViewBookEdit.Rows[_editingBookRowIndex];
            string title = row.Cells["Title"].Value.ToString();
            string publisherName = row.Cells["Publisher"].Value.ToString();
            string publicationYear = row.Cells["PublicationYear"].Value?.ToString();
            int copiesAvailable = Convert.ToInt32(row.Cells["CopiesAvailable"].Value);
            int totalCopies = Convert.ToInt32(row.Cells["TotalCopies"].Value);
            string genres = row.Cells["Genres"].Value?.ToString();
            string authors = row.Cells["Authors"].Value?.ToString();

            // Начинаем транзакцию
            SqlTransaction transaction = _connection.BeginTransaction();

            try
            {
                // Получаем или создаем издателя
                int publisherId = GetOrCreatePublisher(publisherName, transaction);

                // Добавляем книгу
                int newBookId = InsertBook(title, publisherId, publicationYear, copiesAvailable, totalCopies, transaction);

                // Обрабатываем жанры
                if (!string.IsNullOrWhiteSpace(genres))
                {
                    ProcessBookGenres(newBookId, genres, transaction);
                }

                // Обрабатываем авторов
                if (!string.IsNullOrWhiteSpace(authors))
                {
                    ProcessBookAuthors(newBookId, authors, transaction);
                }

                // Фиксируем транзакцию
                transaction.Commit();

                // Логируем действие
                LogAdminAction("AddBook", $"Добавлена новая книга: {title}", newBookId, "Book");

                MessageBox.Show("Книга успешно добавлена!", "Успех",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Ошибка при сохранении книги: " + ex.Message);
            }
        }

        private int GetOrCreatePublisher(string publisherName, SqlTransaction transaction)
        {
            string checkQuery = "SELECT PublisherId FROM Publishers WHERE PublisherName = @PublisherName";
            using (SqlCommand cmd = new SqlCommand(checkQuery, _connection, transaction))
            {
                cmd.Parameters.AddWithValue("@PublisherName", publisherName);
                object result = cmd.ExecuteScalar();
                if (result != null) return Convert.ToInt32(result);
            }

            string insertQuery = "INSERT INTO Publishers (PublisherName) VALUES (@PublisherName); SELECT SCOPE_IDENTITY();";
            using (SqlCommand cmd = new SqlCommand(insertQuery, _connection, transaction))
            {
                cmd.Parameters.AddWithValue("@PublisherName", publisherName);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private int InsertBook(string title, int publisherId, string publicationYear,
                              int copiesAvailable, int totalCopies, SqlTransaction transaction)
        {
            string insertQuery = @"
        INSERT INTO Books (Title, PublisherId, PublicationYear, CopiesAvailable, TotalCopies)
        VALUES (@Title, @PublisherId, @PublicationYear, @CopiesAvailable, @TotalCopies);
        SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmd = new SqlCommand(insertQuery, _connection, transaction))
            {
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@PublisherId", publisherId);
                cmd.Parameters.AddWithValue("@PublicationYear",
                    int.TryParse(publicationYear, out int year) ? year : (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CopiesAvailable", copiesAvailable);
                cmd.Parameters.AddWithValue("@TotalCopies", totalCopies);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void ProcessBookGenres(int bookId, string genresString, SqlTransaction transaction)
        {
            string[] genres = genresString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                         .Select(g => g.Trim())
                                         .ToArray();

            foreach (string genreName in genres)
            {
                int genreId = GetOrCreateGenre(genreName, transaction);

                string linkQuery = @"
            IF NOT EXISTS (SELECT 1 FROM BookGenres WHERE BookId = @BookId AND GenreId = @GenreId)
            INSERT INTO BookGenres (BookId, GenreId) VALUES (@BookId, @GenreId)";

                using (SqlCommand cmd = new SqlCommand(linkQuery, _connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@BookId", bookId);
                    cmd.Parameters.AddWithValue("@GenreId", genreId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private int GetOrCreateGenre(string genreName, SqlTransaction transaction)
        {
            string checkQuery = "SELECT GenreId FROM Genres WHERE GenreName = @GenreName";
            using (SqlCommand cmd = new SqlCommand(checkQuery, _connection, transaction))
            {
                cmd.Parameters.AddWithValue("@GenreName", genreName);
                object result = cmd.ExecuteScalar();
                if (result != null) return Convert.ToInt32(result);
            }

            string insertQuery = "INSERT INTO Genres (GenreName) VALUES (@GenreName); SELECT SCOPE_IDENTITY();";
            using (SqlCommand cmd = new SqlCommand(insertQuery, _connection, transaction))
            {
                cmd.Parameters.AddWithValue("@GenreName", genreName);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void ProcessBookAuthors(int bookId, string authorsString, SqlTransaction transaction)
        {
            string[] authors = authorsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                           .Select(a => a.Trim())
                                           .ToArray();

            foreach (string authorName in authors)
            {
                int authorId = GetOrCreateAuthor(authorName, transaction);

                string linkQuery = @"
            IF NOT EXISTS (SELECT 1 FROM BookAuthors WHERE BookId = @BookId AND AuthorId = @AuthorId)
            INSERT INTO BookAuthors (BookId, AuthorId) VALUES (@BookId, @AuthorId)";

                using (SqlCommand cmd = new SqlCommand(linkQuery, _connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@BookId", bookId);
                    cmd.Parameters.AddWithValue("@AuthorId", authorId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private int GetOrCreateAuthor(string authorName, SqlTransaction transaction)
        {
            string checkQuery = "SELECT AuthorId FROM Authors WHERE FullName = @FullName";
            using (SqlCommand cmd = new SqlCommand(checkQuery, _connection, transaction))
            {
                cmd.Parameters.AddWithValue("@FullName", authorName);
                object result = cmd.ExecuteScalar();
                if (result != null) return Convert.ToInt32(result);
            }

            string insertQuery = "INSERT INTO Authors (FullName) VALUES (@FullName); SELECT SCOPE_IDENTITY();";
            using (SqlCommand cmd = new SqlCommand(insertQuery, _connection, transaction))
            {
                cmd.Parameters.AddWithValue("@FullName", authorName);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void ResetBookEditingState()
        {
            _isEditingBook = false;
            _editingBookRowIndex = -1;
            dataGridViewBookEdit.ReadOnly = true;
            UpdateButtonState(false);
            dataGridViewBookEdit.ClearSelection();

            // Возвращаем фокус на кнопку добавления
            buttonAddNewBook.Focus();
        }

        private void UpdateButtonState(bool isEditing)
        {
            buttonAddNewBook.Text = isEditing ? "Сохранить книгу (Enter)" : "Добавить книгу";
            buttonAddNewBook.BackColor = isEditing ? Color.FromArgb(200, 255, 200) : SystemColors.Control;
            buttonAddNewBook.ForeColor = isEditing ? Color.DarkGreen : SystemColors.ControlText;

            // Делаем кнопку более заметной в режиме редактирования
            if (isEditing)
            {
                buttonAddNewBook.FlatStyle = FlatStyle.Flat;
                buttonAddNewBook.FlatAppearance.BorderColor = Color.Green;
                buttonAddNewBook.FlatAppearance.BorderSize = 1;
            }
            else
            {
                buttonAddNewBook.FlatStyle = FlatStyle.Standard;
            }
        }

        private void dataGridViewBookEdit_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (_addingNewBook)
                {
                    // Получаем данные из новой строки
                    DataGridViewRow row = dataGridViewBookEdit.Rows[e.RowIndex];
                    string title = row.Cells["Title"].Value?.ToString();
                    string publisher = row.Cells["Publisher"].Value?.ToString();
                    string publicationYear = row.Cells["PublicationYear"].Value?.ToString();
                    int copiesAvailable = Convert.ToInt32(row.Cells["CopiesAvailable"].Value);
                    int totalCopies = Convert.ToInt32(row.Cells["TotalCopies"].Value);

                    if (string.IsNullOrWhiteSpace(title))
                    {
                        MessageBox.Show("Название книги не может быть пустым", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Добавляем книгу в базу данных
                    string insertQuery = @"
                INSERT INTO Books (Title, PublisherId, PublicationYear, CopiesAvailable, TotalCopies)
                VALUES (@Title, 
                        (SELECT PublisherId FROM Publishers WHERE PublisherName = @PublisherName), 
                        @PublicationYear, @CopiesAvailable, @TotalCopies);
                SELECT SCOPE_IDENTITY();";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, _connection))
                    {
                        cmd.Parameters.AddWithValue("@Title", title);
                        cmd.Parameters.AddWithValue("@PublisherName", publisher ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PublicationYear",
                            int.TryParse(publicationYear, out int year) ? year : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CopiesAvailable", copiesAvailable);
                        cmd.Parameters.AddWithValue("@TotalCopies", totalCopies);

                        int newBookId = Convert.ToInt32(cmd.ExecuteScalar());

                        // Логируем действие
                        LogAdminAction("AddBook", $"Добавлена новая книга: {title}", newBookId, "Book");
                    }

                    // Возвращаем таблицу в обычный режим
                    _addingNewBook = false;
                    dataGridViewBookEdit.ReadOnly = true;

                    // Обновляем данные
                    LoadBooks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении новой книги: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                // В случае ошибки удаляем добавленную строку
                if (_addingNewBook)
                {
                    ((DataTable)dataGridViewBookEdit.DataSource).Rows.RemoveAt(e.RowIndex);
                    _addingNewBook = false;
                    dataGridViewBookEdit.ReadOnly = true;
                }
            }
        }

        private void dataGridViewBookEdit_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!_addingNewBook) return;

            // Проверяем обязательные поля
            if (e.ColumnIndex == dataGridViewBookEdit.Columns["Title"].Index)
            {
                if (string.IsNullOrWhiteSpace(e.FormattedValue.ToString()))
                {
                    dataGridViewBookEdit.Rows[e.RowIndex].ErrorText = "Название книги не может быть пустым";
                    e.Cancel = true;
                }
            }
            else if (e.ColumnIndex == dataGridViewBookEdit.Columns["CopiesAvailable"].Index ||
                     e.ColumnIndex == dataGridViewBookEdit.Columns["TotalCopies"].Index)
            {
                if (!int.TryParse(e.FormattedValue.ToString(), out int value) || value < 0)
                {
                    dataGridViewBookEdit.Rows[e.RowIndex].ErrorText = "Должно быть положительное число";
                    e.Cancel = true;
                }
            }
        }

        private void dataGridViewBookEdit_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewBookEdit.Rows[e.RowIndex].ErrorText = string.Empty;
        }


        private void DataGridViewBookEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (_addingNewBook && e.KeyCode == Keys.Escape)
            {
                if (dataGridViewBookEdit.DataSource is DataTable dt && dt.Rows.Count > 0)
                {
                    dt.Rows.RemoveAt(0);
                    dataGridViewBookEdit.ReadOnly = true;
                    dataGridViewBookEdit.AllowUserToAddRows = false;
                    _addingNewBook = false;
                    buttonAddNewBook.Text = "Добавить новую книгу";
                    e.Handled = true;
                }
            }
        }





        private void buttonWriteOffBookAndCopies_Click(object sender, EventArgs e)
        {
            SqlTransaction transaction = null;
            try
            {
                if (dataGridViewBookEdit.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите книгу для списания", "Информация",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Получаем данные о выбранной книге
                int selectedBookId = Convert.ToInt32(dataGridViewBookEdit.SelectedRows[0].Cells["BookId"].Value);
                string bookTitle = dataGridViewBookEdit.SelectedRows[0].Cells["Title"].Value.ToString();

                // Начинаем транзакцию
                transaction = _connection.BeginTransaction();

                // Проверяем, есть ли активные выдачи этой книги (внутри транзакции)
                if (HasActiveLoans(selectedBookId, transaction))
                {
                    MessageBox.Show("Нельзя списать книгу, так как она находится на руках у читателей",
                                  "Ошибка списания", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    transaction.Rollback();
                    return;
                }

                // Подтверждение удаления
                if (MessageBox.Show($"Вы уверены, что хотите полностью списать книгу '{bookTitle}'?",
                                  "Подтверждение списания", MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Удаляем книгу
                    WriteOffBookCompletely(selectedBookId, bookTitle, transaction);

                    // Фиксируем транзакцию
                    transaction.Commit();

                    // Обновляем список книг
                    LoadBooks();

                    MessageBox.Show("Книга успешно списана", "Успех",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    transaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                // Откатываем транзакцию в случае ошибки
                transaction?.Rollback();
                MessageBox.Show($"Ошибка при списании книги: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                transaction?.Dispose();
            }
        }

        private bool HasActiveLoans(int bookId, SqlTransaction transaction)
        {
            string query = @"
        SELECT COUNT(*) 
        FROM LoanCards 
        WHERE BookId = @BookId AND Status = 'Active' AND ReturnDate IS NULL";

            using (SqlCommand cmd = new SqlCommand(query, _connection, transaction))
            {
                cmd.Parameters.AddWithValue("@BookId", bookId);
                int activeLoans = Convert.ToInt32(cmd.ExecuteScalar());
                return activeLoans > 0;
            }
        }

        private void WriteOffBookCompletely(int bookId, string bookTitle, SqlTransaction transaction)
        {
            // 1. Удаляем связанные записи
            string[] queries = {
        "DELETE FROM BookGenres WHERE BookId = @BookId",
        "DELETE FROM BookAuthors WHERE BookId = @BookId",
        "DELETE FROM BookRequests WHERE BookId = @BookId",
        "DELETE FROM LoanCards WHERE BookId = @BookId",
        "DELETE FROM Books WHERE BookId = @BookId"
    };

            foreach (string query in queries)
            {
                using (SqlCommand cmd = new SqlCommand(query, _connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@BookId", bookId);
                    cmd.ExecuteNonQuery();
                }
            }

            // 2. Логируем действие
            LogAdminAction("WriteOffBook", $"Списана книга: {bookTitle}", bookId, "Book");
        }

        private bool HasActiveLoans(int bookId)
        {
            string query = @"
        SELECT COUNT(*) 
        FROM LoanCards 
        WHERE BookId = @BookId AND Status = 'Active' AND ReturnDate IS NULL";

            using (SqlCommand cmd = new SqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@BookId", bookId);
                int activeLoans = Convert.ToInt32(cmd.ExecuteScalar());
                return activeLoans > 0;
            }
        }



        private void buttonEditAllBook_Click(object sender, EventArgs e)
        {
            if (!_isEditingMode)
            {
                // Проверяем, выбрана ли строка в dataGridViewBookEdit
                if (dataGridViewBookEdit.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите книгу для редактирования.", "Информация",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Получаем BookId из выбранной строки
                int selectedBookId = Convert.ToInt32(dataGridViewBookEdit.SelectedRows[0].Cells["BookId"].Value);

                // Убеждаемся, что соединение открыто перед передачей
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                // Открываем форму редактирования книги
                BookRedactForm form = new BookRedactForm(_connection, _currentAdminId.Id, selectedBookId);

                // Подписываемся на событие FormClosed, чтобы обновить список книг и состояние соединения
                form.FormClosed += (s, args) =>
                {
                    LoadBooks(); // Обновляем список книг
                    this.Show(); // Показываем основную форму
                    if (_connection.State != ConnectionState.Open)
                    {
                        _connection.Open(); // Восстанавливаем соединение, если оно закрыто
                    }
                };

                // Скрываем текущую форму и показываем форму редактирования
                this.Hide();
                form.Show();
            }
            else
            {
                MessageBox.Show("Завершите текущий режим редактирования.", "Внимание",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }














        //вторая вкладка: Управление пользователями
        private void buttonNewUser_Click(object sender, EventArgs e)
        {
            // Убеждаемся, что соединение открыто перед передачей
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            // Открываем форму добавления нового пользователя
            NewUserForm form = new NewUserForm(_connection, _currentAdminId);

            // Подписываемся на событие FormClosed, чтобы обновить список пользователей
            form.FormClosed += (s, args) =>
            {
                try
                {
                    LoadUsers(); // Обновляем список пользователей
                    this.Show(); // Показываем основную форму
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении списка пользователей: {ex.Message}", "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (_connection.State != ConnectionState.Open)
                    {
                        _connection.Open(); // Восстанавливаем соединение, если оно закрыто
                    }
                }
            };

            // Скрываем текущую форму и показываем форму добавления пользователя
            this.Hide();
            form.Show();
        }
        private void buttonSearchUsers_Click(object sender, EventArgs e)
        {
            string searchText = txtUserSearch.Text.Trim();

            string query = @"
        SELECT 
            r.ReaderId,
            r.Username,
            r.Email,
            r.FullName,
            r.PhoneNumber,
            r.GroupId,
            pg.GroupName,
            Roles.RoleName,
            (SELECT COUNT(*) FROM LoanCards l WHERE l.ReaderId = r.ReaderId AND l.ReturnDate IS NULL) AS ActiveLoans,
            (SELECT COUNT(*) FROM LoanCards l WHERE l.ReaderId = r.ReaderId AND l.ReturnDate IS NULL AND l.DueDate < GETDATE()) AS OverdueLoans
        FROM Users r
        LEFT JOIN PopulationGroups pg ON r.GroupId = pg.GroupId
        LEFT JOIN Roles ON r.RoleId = Roles.RoleId
        WHERE r.Username LIKE @Search OR r.Email LIKE @Search
        ORDER BY r.Username
        ";

            DataTable dt = new DataTable();
            using (SqlCommand cmd = new SqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@Search", $"%{searchText}%");

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }



            dataGridViewUsers.DataSource = dt;
            ConfigureUsersGridView();
        }

        private void LoadUsers()
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                }

                string query = @"
            SELECT 
                r.ReaderId,
                r.Username,
                r.Email,
                r.FullName,
                r.PhoneNumber,
                r.GroupId,
                pg.GroupName,
                ro.RoleName,
                (SELECT COUNT(*) FROM LoanCards l WHERE l.ReaderId = r.ReaderId AND l.ReturnDate IS NULL) AS ActiveLoans,
                (SELECT COUNT(*) FROM LoanCards l WHERE l.ReaderId = r.ReaderId AND l.ReturnDate IS NULL AND l.DueDate < GETDATE()) AS OverdueLoans
            FROM Users r
            LEFT JOIN PopulationGroups pg ON r.GroupId = pg.GroupId
            LEFT JOIN Roles ro ON r.RoleId = ro.RoleId
            ORDER BY r.Username";

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                // Диагностика: выводим количество строк
                Debug.WriteLine($"Загружено строк: {dt.Rows.Count}");
                foreach (DataRow row in dt.Rows)
                {
                    Debug.WriteLine($"ReaderId: {row["ReaderId"]}, Username: {row["Username"]}, GroupId: {row["GroupId"]}, RoleName: {row["RoleName"]}");
                }

                dataGridViewUsers.DataSource = null;
                dataGridViewUsers.DataSource = dt;
                ConfigureUsersGridView();
                dataGridViewUsers.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ConfigureUsersGridView()
        {
            dataGridViewUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewUsers.ReadOnly = true;
            dataGridViewUsers.AllowUserToAddRows = false;
            dataGridViewUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Включён выбор всей строки
            dataGridViewUsers.MultiSelect = false; // Только одна строка может быть выбрана

            if (dataGridViewUsers.Columns.Contains("ReaderId"))
                dataGridViewUsers.Columns["ReaderId"].Visible = false;

            if (dataGridViewUsers.Columns.Contains("GroupId"))
                dataGridViewUsers.Columns["GroupId"].Visible = false;

            if (dataGridViewUsers.Columns.Contains("Username"))
                dataGridViewUsers.Columns["Username"].HeaderText = "Логин";

            if (dataGridViewUsers.Columns.Contains("Email"))
                dataGridViewUsers.Columns["Email"].HeaderText = "Email";

            if (dataGridViewUsers.Columns.Contains("FullName"))
                dataGridViewUsers.Columns["FullName"].HeaderText = "ФИО";

            if (dataGridViewUsers.Columns.Contains("PhoneNumber"))
                dataGridViewUsers.Columns["PhoneNumber"].HeaderText = "Телефон";

            if (dataGridViewUsers.Columns.Contains("GroupName"))
                dataGridViewUsers.Columns["GroupName"].HeaderText = "Группа";

            if (dataGridViewUsers.Columns.Contains("RoleName"))
                dataGridViewUsers.Columns["RoleName"].HeaderText = "Роль";

            if (dataGridViewUsers.Columns.Contains("ActiveLoans"))
                dataGridViewUsers.Columns["ActiveLoans"].HeaderText = "Книг на руках";

            if (dataGridViewUsers.Columns.Contains("OverdueLoans"))
                dataGridViewUsers.Columns["OverdueLoans"].HeaderText = "Просрочено";

            dataGridViewUsers.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
        }

        private void buttonBanUser_Click(object sender, EventArgs e)
        {

        }

        private void buttonUnban_Click(object sender, EventArgs e)
        {

        }

        private void buttonDeleteUser_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, выбрана ли строка
                if (dataGridViewUsers.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите пользователя для удаления.",
                                  "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Получаем данные выбранного пользователя
                DataGridViewRow selectedRow = dataGridViewUsers.SelectedRows[0];
                int userId = Convert.ToInt32(selectedRow.Cells["ReaderId"].Value);
                string username = selectedRow.Cells["Username"].Value.ToString();
                string roleName = selectedRow.Cells["RoleName"].Value?.ToString();

                // Проверка прав доступа
                if (roleName == "Super_admin" || roleName == "Admin")
                {
                    if (_currentAdminId.Role != UserRole.Super_admin)
                    {
                        MessageBox.Show("Только супер-администратор может удалять администраторов.",
                                      "Ошибка доступа", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // Проверяем активные выдачи книг
                if (HasActiveLoansUser(userId))
                {
                    MessageBox.Show("Нельзя удалить пользователя, у которого есть не возвращённые книги.",
                                  "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Подтверждение удаления
                DialogResult confirmResult = MessageBox.Show(
                    $"Вы уверены, что хотите удалить пользователя '{username}'? Это действие нельзя отменить.",
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmResult != DialogResult.Yes)
                {
                    return;
                }

                // Начинаем транзакцию
                using (SqlTransaction transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        // Удаляем связанные записи (запросы на книги)
                        string deleteRequestsQuery = "DELETE FROM BookRequests WHERE ReaderId = @UserId";
                        using (SqlCommand cmd = new SqlCommand(deleteRequestsQuery, _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.ExecuteNonQuery();
                        }

                        // Удаляем записи о выдачах книг
                        string deleteLoansQuery = "DELETE FROM LoanCards WHERE ReaderId = @UserId";
                        using (SqlCommand cmd = new SqlCommand(deleteLoansQuery, _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.ExecuteNonQuery();
                        }

                        // Удаляем записи в логах (если нужно)
                        string deleteLogsQuery = "DELETE FROM AdminLogs WHERE AdminId = @UserId OR AffectedId = @UserId";
                        using (SqlCommand cmd = new SqlCommand(deleteLogsQuery, _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.ExecuteNonQuery();
                        }

                        // Удаляем самого пользователя
                        string deleteUserQuery = "DELETE FROM Users WHERE ReaderId = @UserId";
                        using (SqlCommand cmd = new SqlCommand(deleteUserQuery, _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                throw new Exception("Пользователь не найден в базе данных.");
                            }
                        }

                        // Логируем действие
                        LogAdminAction("DeleteUser", $"Удалён пользователь: {username}", userId, "User");

                        // Фиксируем транзакцию
                        transaction.Commit();

                        // Обновляем список пользователей
                        LoadUsers();

                        MessageBox.Show("Пользователь успешно удалён.", "Успех",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Ошибка при удалении пользователя: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Вспомогательный метод для проверки активных выдач
        private bool HasActiveLoansUser(int userId)
        {
            string query = @"
        SELECT COUNT(*) 
        FROM LoanCards 
        WHERE ReaderId = @UserId AND ReturnDate IS NULL";

            using (SqlCommand cmd = new SqlCommand(query, _connection))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                int activeLoans = Convert.ToInt32(cmd.ExecuteScalar());
                return activeLoans > 0;
            }
        }

        //третья вкладка: Обработка запросов
        private void buttonSearchRequests_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText = txtRequesrSearch.Text.Trim(); // Assuming a TextBox named txtRequestSearch

                string query = @"
            SELECT 
                br.RequestId,
                br.BookId,
                b.Title AS BookTitle,
                br.ReaderId,
                u.Username AS Requester,
                br.RequestDate,
                br.ResponseDate,
                br.Target,
                br.Result,
                admin.Username AS AdminUsername
            FROM BookRequests br
            INNER JOIN Books b ON br.BookId = b.BookId
            INNER JOIN Users u ON br.ReaderId = u.ReaderId
            LEFT JOIN Users admin ON br.AdminId = admin.ReaderId
            WHERE 
                b.Title LIKE @Search OR
                u.Username LIKE @Search OR
                br.Target LIKE @Search OR
                br.Result LIKE @Search
            ORDER BY br.RequestDate DESC";

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@Search", "%" + searchText + "%");

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                dataGridView3.DataSource = dt; // Assuming a DataGridView named dataGridViewRequests
                ConfigureRequestsGridView();

                // Log the search action
                LogAdminAction("SearchRequests", $"Поиск запросов: {searchText}", null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске запросов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonAcceptRequest_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if a request is selected
                if (dataGridView3.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите запрос для принятия.", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Get the selected request details
                DataGridViewRow selectedRow = dataGridView3.SelectedRows[0];
                int requestId = Convert.ToInt32(selectedRow.Cells["RequestId"].Value);
                int bookId = Convert.ToInt32(selectedRow.Cells["BookId"].Value);
                int readerId = Convert.ToInt32(selectedRow.Cells["ReaderId"].Value);
                string target = selectedRow.Cells["Target"].Value.ToString();
                string bookTitle = selectedRow.Cells["BookTitle"].Value.ToString(); // Assuming this column exists after the search implementation
                string requester = selectedRow.Cells["Requester"].Value.ToString();

                // Check if the request has already been processed
                if (selectedRow.Cells["Result"].Value != DBNull.Value)
                {
                    MessageBox.Show("Этот запрос уже обработан.", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Start a transaction
                using (SqlTransaction transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        // Update the BookRequests table
                        string updateRequestQuery = @"
                    UPDATE BookRequests
                    SET AdminId = @AdminId,
                        ResponseDate = @ResponseDate,
                        Result = 'Accepted'
                    WHERE RequestId = @RequestId";

                        using (SqlCommand cmd = new SqlCommand(updateRequestQuery, _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@AdminId", _currentAdminId.Id);
                            cmd.Parameters.AddWithValue("@ResponseDate", DateTime.Now);
                            cmd.Parameters.AddWithValue("@RequestId", requestId);
                            cmd.ExecuteNonQuery();
                        }

                        // Handle the action based on the Target
                        if (target == "Borrow")
                        {
                            // Check if the book is available
                            string checkAvailabilityQuery = @"
                        SELECT CopiesAvailable
                        FROM Books
                        WHERE BookId = @BookId";

                            int copiesAvailable;
                            using (SqlCommand cmd = new SqlCommand(checkAvailabilityQuery, _connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@BookId", bookId);
                                copiesAvailable = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            if (copiesAvailable <= 0)
                            {
                                throw new Exception("Нет доступных копий книги для выдачи.");
                            }

                            // Create a LoanCard entry
                            string insertLoanQuery = @"
                        INSERT INTO LoanCards (IssueDate, DueDate, BookId, ReaderId, Status)
                        VALUES (@IssueDate, @DueDate, @BookId, @ReaderId, 'Active')";

                            using (SqlCommand cmd = new SqlCommand(insertLoanQuery, _connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@IssueDate", DateTime.Now);
                                cmd.Parameters.AddWithValue("@DueDate", DateTime.Now.AddDays(14)); // Assuming a 14-day loan period
                                cmd.Parameters.AddWithValue("@BookId", bookId);
                                cmd.Parameters.AddWithValue("@ReaderId", readerId);
                                cmd.ExecuteNonQuery();
                            }

                            // Update CopiesAvailable
                            string updateCopiesQuery = @"
                        UPDATE Books
                        SET CopiesAvailable = CopiesAvailable - 1
                        WHERE BookId = @BookId";

                            using (SqlCommand cmd = new SqlCommand(updateCopiesQuery, _connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@BookId", bookId);
                                cmd.ExecuteNonQuery();
                            }

                            // Log the action
                            LogAdminAction("AcceptBorrowRequest", $"Принят запрос на выдачу книги: {bookTitle} пользователю {requester}", requestId, "Request", transaction);
                        }
                        else if (target == "Return")
                        {
                            // Find the active loan for this book and reader
                            string findLoanQuery = @"
                        SELECT LoanId
                        FROM LoanCards
                        WHERE BookId = @BookId AND ReaderId = @ReaderId AND Status = 'Active' AND ReturnDate IS NULL";

                            int? loanId = null;
                            using (SqlCommand cmd = new SqlCommand(findLoanQuery, _connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@BookId", bookId);
                                cmd.Parameters.AddWithValue("@ReaderId", readerId);
                                object result = cmd.ExecuteScalar();
                                if (result != null)
                                    loanId = Convert.ToInt32(result);
                            }

                            if (!loanId.HasValue)
                            {
                                throw new Exception("Активная выдача книги для этого пользователя не найдена.");
                            }

                            // Update the LoanCard to mark as returned
                            string updateLoanQuery = @"
                        UPDATE LoanCards
                        SET ReturnDate = @ReturnDate,
                            Status = 'Returned'
                        WHERE LoanId = @LoanId";

                            using (SqlCommand cmd = new SqlCommand(updateLoanQuery, _connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ReturnDate", DateTime.Now);
                                cmd.Parameters.AddWithValue("@LoanId", loanId.Value);
                                cmd.ExecuteNonQuery();
                            }

                            // Update CopiesAvailable
                            string updateCopiesQuery = @"
                        UPDATE Books
                        SET CopiesAvailable = CopiesAvailable + 1
                        WHERE BookId = @BookId";

                            using (SqlCommand cmd = new SqlCommand(updateCopiesQuery, _connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@BookId", bookId);
                                cmd.ExecuteNonQuery();
                            }

                            // Log the action
                            LogAdminAction("AcceptReturnRequest", $"Принят запрос на возврат книги: {bookTitle} от пользователя {requester}", requestId, "Request", transaction);
                        }
                        else
                        {
                            throw new Exception($"Неизвестная цель запроса: {target}");
                        }

                        // Commit the transaction
                        transaction.Commit();

                        // Refresh the requests list
                        LoadRequests();

                        MessageBox.Show("Запрос успешно принят.", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при принятии запроса: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonRejectRequest_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if a request is selected
                if (dataGridView3.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите запрос для отклонения.", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Get the selected request details
                DataGridViewRow selectedRow = dataGridView3.SelectedRows[0];
                int requestId = Convert.ToInt32(selectedRow.Cells["RequestId"].Value);
                string bookTitle = selectedRow.Cells["BookTitle"].Value.ToString(); // From previous search implementation
                string requester = selectedRow.Cells["Requester"].Value.ToString();
                string target = selectedRow.Cells["Target"].Value.ToString();

                // Check if the request has already been processed
                if (selectedRow.Cells["Result"].Value != DBNull.Value)
                {
                    MessageBox.Show("Этот запрос уже обработан.", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Start a transaction
                using (SqlTransaction transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        // Update the BookRequests table
                        string updateRequestQuery = @"
                    UPDATE BookRequests
                    SET AdminId = @AdminId,
                        ResponseDate = @ResponseDate,
                        Result = 'Rejected'
                    WHERE RequestId = @RequestId";

                        using (SqlCommand cmd = new SqlCommand(updateRequestQuery, _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@AdminId", _currentAdminId.Id);
                            cmd.Parameters.AddWithValue("@ResponseDate", DateTime.Now);
                            cmd.Parameters.AddWithValue("@RequestId", requestId);
                            cmd.ExecuteNonQuery();
                        }

                        // Log the action
                        LogAdminAction("RejectRequest", $"Отклонён запрос на {target.ToLower()} книги: {bookTitle} от пользователя {requester}", requestId, "Request", transaction);

                        // Commit the transaction
                        transaction.Commit();

                        // Refresh the requests list
                        LoadRequests();

                        MessageBox.Show("Запрос успешно отклонён.", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при отклонении запроса: {ex.Message}", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCheckUserDebts_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if a user is selected
                if (dataGridViewUsers.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите пользователя для проверки долгов.", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    buttonCheckUserDebts.Enabled = false; // Disable button if no user is selected
                    return;
                }

                // Get the selected user's details
                DataGridViewRow selectedRow = dataGridViewUsers.SelectedRows[0];
                int readerId = Convert.ToInt32(selectedRow.Cells["ReaderId"].Value);
                string username = selectedRow.Cells["Username"].Value.ToString();

                // Calculate overdue loans
                string query = @"
            SELECT COUNT(*) 
            FROM LoanCards 
            WHERE ReaderId = @ReaderId 
            AND ReturnDate IS NULL 
            AND DueDate < @CurrentDate";

                int overdueLoans;
                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@ReaderId", readerId);
                    cmd.Parameters.AddWithValue("@CurrentDate", DateTime.Now); // 04:05 PM CEST, June 05, 2025
                    overdueLoans = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Update the selected row with the overdue loans count
                selectedRow.Cells["OverdueLoans"].Value = overdueLoans;

                // Log the action
                LogAdminAction("CheckUserDebts", $"Проверка долгов для пользователя: {username}", readerId, "User");

                MessageBox.Show($"Долги пользователя {username} проверены. Просрочено книг: {overdueLoans}.", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Enable button for future use (reset state)
                buttonCheckUserDebts.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке долгов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonCheckUserDebts.Enabled = true; // Ensure button remains usable after error
            }
        }

        private void buttonCleardataGridView3_Click(object sender, EventArgs e)
        {
            LoadRequests();
        }

        //четвёртая вкладка: Журнал действий
        private void buttonSearchActions_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText = txtSearchActions.Text.Trim();// Предполагается, что у вас есть TextBox с именем txtActionSearch

                string query = @"
            SELECT 
                al.LogId,
                al.ActionTime,
                al.ActionType,
                al.ActionDetails,
                u.Username AS AdminUsername,
                al.AffectedId,
                al.AffectedType
            FROM AdminLogs al
            LEFT JOIN Users u ON al.AdminId = u.ReaderId
            WHERE 
                al.ActionType LIKE @Search OR
                al.ActionDetails LIKE @Search OR
                u.Username LIKE @Search
            ORDER BY al.ActionTime DESC";

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@Search", "%" + searchText + "%");

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                dataGridView1.DataSource = dt; // Предполагается, что у вас есть DataGridView с именем dataGridView1
                ConfigureLogsGridView();

                // Логируем действие поиска
                LogAdminAction("SearchLogs", $"Поиск в журнале действий: {searchText}", null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске логов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadLogs()
        {
            try
            {
                string query = @"
            SELECT 
                al.LogId,
                al.ActionTime,
                al.ActionType,
                al.ActionDetails,
                u.Username AS AdminUsername,
                al.AffectedId,
                al.AffectedType
            FROM AdminLogs al
            LEFT JOIN Users u ON al.AdminId = u.ReaderId
            ORDER BY al.ActionTime DESC";

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                dataGridView1.DataSource = dt; // Предполагается, что у вас есть DataGridView с именем dataGridView1
                ConfigureLogsGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке логов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ConfigureLogsGridView()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            // Скрываем идентификаторы
            if (dataGridView1.Columns.Contains("LogId"))
                dataGridView1.Columns["LogId"].Visible = false;
            if (dataGridView1.Columns.Contains("AffectedId"))
                dataGridView1.Columns["AffectedId"].Visible = false;

            // Задаём отображаемые имена столбцов
            if (dataGridView1.Columns.Contains("ActionTime"))
                dataGridView1.Columns["ActionTime"].HeaderText = "Время действия";
            if (dataGridView1.Columns.Contains("ActionType"))
                dataGridView1.Columns["ActionType"].HeaderText = "Тип действия";
            if (dataGridView1.Columns.Contains("ActionDetails"))
                dataGridView1.Columns["ActionDetails"].HeaderText = "Детали действия";
            if (dataGridView1.Columns.Contains("AdminUsername"))
                dataGridView1.Columns["AdminUsername"].HeaderText = "Администратор";
            if (dataGridView1.Columns.Contains("AffectedType"))
                dataGridView1.Columns["AffectedType"].HeaderText = "Тип объекта";

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        }
        private void buttonEditUser_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, выбрана ли строка в dataGridViewUsers
                if (dataGridViewUsers.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Пожалуйста, выберите пользователя для редактирования.", "Информация",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Debug.WriteLine($"SelectedRows.Count: {dataGridViewUsers.SelectedRows.Count}, Rows.Count: {dataGridViewUsers.Rows.Count}");
                    return;
                }

                // Получаем данные из выбранной строки
                DataGridViewRow selectedRow = dataGridViewUsers.SelectedRows[0];
                int readerId = Convert.ToInt32(selectedRow.Cells["ReaderId"].Value);

                // Получаем полные данные пользователя из базы данных
                string query = @"
            SELECT 
                u.ReaderId, 
                u.Username, 
                u.Email, 
                u.FullName, 
                u.PhoneNumber, 
                u.GroupId, 
                u.RoleID, 
                u.PasswordHash,
                pg.GroupName
            FROM Users u
            LEFT JOIN PopulationGroups pg ON u.GroupId = pg.GroupId
            WHERE u.ReaderId = @ReaderId";

                User userToEdit = null;

                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@ReaderId", readerId);

                    if (_connection.State != ConnectionState.Open)
                    {
                        _connection.Open();
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userToEdit = new User
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ReaderId")),
                                Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? null : reader.GetString(reader.GetOrdinal("Username")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                                FullName = reader.IsDBNull(reader.GetOrdinal("FullName")) ? null : reader.GetString(reader.GetOrdinal("FullName")),
                                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                GroupId = reader.IsDBNull(reader.GetOrdinal("GroupId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("GroupId")),
                                Role = reader.IsDBNull(reader.GetOrdinal("RoleID")) ? UserRole.User : (UserRole)reader.GetInt32(reader.GetOrdinal("RoleID")),
                                PasswordHash = reader.IsDBNull(reader.GetOrdinal("PasswordHash")) ? null : reader.GetString(reader.GetOrdinal("PasswordHash"))
                            };

                            // Если есть GroupId, преобразуем в PopulationGroup
                            if (userToEdit.GroupId.HasValue)
                            {
                                userToEdit.Group = (PopulationGroup)userToEdit.GroupId.Value;
                            }
                        }
                    }
                }

                if (userToEdit == null)
                {
                    MessageBox.Show("Не удалось загрузить данные пользователя.", "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Открываем форму редактирования пользователя
                UserRedactForm form = new UserRedactForm(_connection, _currentAdminId, userToEdit);

                // Подписываемся на событие FormClosed
                form.FormClosed += (s, args) =>
                {
                    try
                    {
                        LoadUsers(); // Обновляем список пользователей
                        this.Show(); // Показываем основную форму
                        if (_connection.State != ConnectionState.Open)
                        {
                            _connection.Open(); // Восстанавливаем соединение
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении списка пользователей: {ex.Message}", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                // Скрываем текущую форму и показываем форму редактирования
                this.Hide();
                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии формы редактирования: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureRequestsGridView()
        {
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView3.ReadOnly = true;
            dataGridView3.AllowUserToAddRows = false;
            dataGridView3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView3.MultiSelect = false;

            // Hide ID columns
            if (dataGridView3.Columns.Contains("RequestId"))
                dataGridView3.Columns["RequestId"].Visible = false;
            if (dataGridView3.Columns.Contains("BookId"))
                dataGridView3.Columns["BookId"].Visible = false;
            if (dataGridView3.Columns.Contains("ReaderId"))
                dataGridView3.Columns["ReaderId"].Visible = false;

            // Set display names for columns
            if (dataGridView3.Columns.Contains("BookTitle"))
                dataGridView3.Columns["BookTitle"].HeaderText = "Название книги";
            if (dataGridView3.Columns.Contains("Requester"))
                dataGridView3.Columns["Requester"].HeaderText = "Запросивший";
            if (dataGridView3.Columns.Contains("RequestDate"))
                dataGridView3.Columns["RequestDate"].HeaderText = "Дата запроса";
            if (dataGridView3.Columns.Contains("ResponseDate"))
                dataGridView3.Columns["ResponseDate"].HeaderText = "Дата ответа";
            if (dataGridView3.Columns.Contains("Target"))
                dataGridView3.Columns["Target"].HeaderText = "Цель";
            if (dataGridView3.Columns.Contains("Result"))
                dataGridView3.Columns["Result"].HeaderText = "Результат";
            if (dataGridView3.Columns.Contains("AdminUsername"))
                dataGridView3.Columns["AdminUsername"].HeaderText = "Обработал";

            dataGridView3.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dataGridView3.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10);
            dataGridView3.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        }

        private void LoadRequests()
        {
            try
            {
                string query = @"
            SELECT 
                br.RequestId,
                br.BookId,
                b.Title AS BookTitle,
                br.ReaderId,
                u.Username AS Requester,
                br.RequestDate,
                br.ResponseDate,
                br.Target,
                br.Result,
                admin.Username AS AdminUsername
            FROM BookRequests br
            INNER JOIN Books b ON br.BookId = b.BookId
            INNER JOIN Users u ON br.ReaderId = u.ReaderId
            LEFT JOIN Users admin ON br.AdminId = admin.ReaderId
            ORDER BY br.RequestDate DESC";

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                dataGridView3.DataSource = dt;
                ConfigureRequestsGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке запросов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPageStats) // Используем имя вкладки tabPageStats
            {
                LoadStatistics();
            }
            else if (tabControl1.SelectedTab == tabPage3) // RequestsTab
            {
                LoadRequests();
            }
            else if (tabControl1.SelectedTab == tabPage4) // LogsTab
            {
                LoadLogs();
            }
            else if (tabControl1.SelectedTab == tabPage10)
            {
                button1_Click(sender, e);
            }
        }


        private void LoadStatistics()
        {
            LoadVisitVolumeChart();
            LoadOverdueBooksReport();
            LoadBookLoansByGroups();
            LoadBookLoansByGenres();
        }

        private void LoadVisitVolumeChart()
        {
            chartVisitVolume.Series.Clear();
            chartVisitVolume.ChartAreas.Clear();
            chartVisitVolume.Titles.Clear();

            ChartArea chartArea = new ChartArea("ChartArea1");
            chartVisitVolume.ChartAreas.Add(chartArea);
            Series series = new Series("Посещения")
            {
                ChartType = SeriesChartType.Column,
                XValueType = ChartValueType.Date
            };
            chartVisitVolume.Series.Add(series);

            try
            {
                string query = @"
            SELECT CAST(l.IssueDate AS DATE) AS VisitDate, COUNT(*) AS VisitCount
            FROM LoanCards l
            WHERE l.IssueDate >= DATEADD(MONTH, -12, GETDATE())
            GROUP BY CAST(l.IssueDate AS DATE)
            ORDER BY CAST(l.IssueDate AS DATE)";

                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime visitDate = reader.GetDateTime(0);
                            int visitCount = reader.GetInt32(1);
                            series.Points.AddXY(visitDate, visitCount);
                        }
                    }
                }

                chartVisitVolume.Titles.Add("Объём посещений (выдачи книг) за последний год");
                chartArea.AxisX.Title = "Дата";
                chartArea.AxisY.Title = "Количество выдач";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отчёта по посещениям: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadOverdueBooksReport()
        {
            dataGridViewOverdue.DataSource = null;

            try
            {
                string query = @"
            SELECT 
                b.Title AS BookTitle,
                u.Username AS Reader,
                l.IssueDate,
                l.DueDate,
                DATEDIFF(DAY, l.DueDate, GETDATE()) AS DaysOverdue
            FROM LoanCards l
            INNER JOIN Books b ON l.BookId = b.BookId
            INNER JOIN Users u ON l.ReaderId = u.ReaderId
            WHERE l.ReturnDate IS NULL AND l.DueDate < GETDATE()";

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                dataGridViewOverdue.DataSource = dt;
                ConfigureOverdueGridView(dataGridViewOverdue);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отчёта по задержанным книгам: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureOverdueGridView(DataGridView gridView)
        {
            if (gridView.Columns.Contains("BookTitle"))
                gridView.Columns["BookTitle"].HeaderText = "Название книги";
            if (gridView.Columns.Contains("Reader"))
                gridView.Columns["Reader"].HeaderText = "Читатель";
            if (gridView.Columns.Contains("IssueDate"))
                gridView.Columns["IssueDate"].HeaderText = "Дата выдачи";
            if (gridView.Columns.Contains("DueDate"))
                gridView.Columns["DueDate"].HeaderText = "Дата возврата";
            if (gridView.Columns.Contains("DaysOverdue"))
                gridView.Columns["DaysOverdue"].HeaderText = "Дней просрочки";

            gridView.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            gridView.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10);
            gridView.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        }

        private void LoadBookLoansByGroups()
        {
            chartLoansByGroups.Series.Clear();
            chartLoansByGroups.ChartAreas.Clear();
            chartLoansByGroups.Titles.Clear();

            ChartArea chartArea = new ChartArea("ChartAreaGroups");
            chartLoansByGroups.ChartAreas.Add(chartArea);
            Series series = new Series("Выдачи по группам")
            {
                ChartType = SeriesChartType.Pie
            };
            chartLoansByGroups.Series.Add(series);

            try
            {
                string query = @"
            SELECT pg.GroupName, COUNT(l.LoanId) AS LoanCount
            FROM LoanCards l
            INNER JOIN Users u ON l.ReaderId = u.ReaderId
            LEFT JOIN PopulationGroups pg ON u.GroupId = pg.GroupId
            GROUP BY pg.GroupName";

                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string groupName = reader.IsDBNull(0) ? "Без группы" : reader.GetString(0);
                            int loanCount = reader.GetInt32(1);
                            series.Points.AddXY(groupName, loanCount);
                        }
                    }
                }

                chartLoansByGroups.Titles.Add("Анализ выдачи книг по группам населения");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отчёта по группам: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void LoadBookLoansByGenres()
        {
            chartLoansByGenres.Series.Clear();
            chartLoansByGenres.ChartAreas.Clear();
            chartLoansByGenres.Titles.Clear();

            ChartArea chartArea = new ChartArea("ChartAreaGenres");
            chartLoansByGenres.ChartAreas.Add(chartArea);
            Series series = new Series("Выдачи по жанрам")
            {
                ChartType = SeriesChartType.Pie
            };
            chartLoansByGenres.Series.Add(series);

            try
            {
                string query = @"
            SELECT g.GenreName, COUNT(l.LoanId) AS LoanCount
            FROM LoanCards l
            INNER JOIN Books b ON l.BookId = b.BookId
            INNER JOIN BookGenres bg ON b.BookId = bg.BookId
            INNER JOIN Genres g ON bg.GenreId = g.GenreId
            GROUP BY g.GenreName";

                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string genreName = reader.GetString(0);
                            int loanCount = reader.GetInt32(1);
                            series.Points.AddXY(genreName, loanCount);
                        }
                    }
                }

                chartLoansByGenres.Titles.Add("Анализ выдачи книг по жанрам");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке отчёта по жанрам: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText = txtTermins.Text.Trim();

                // Формируем SQL-запрос
                string query = @"
            SELECT 
                TermId,
                Term,
                Definition,
                RelatedField
            FROM Glossary
            WHERE @SearchText = '' OR 
                  Term LIKE @SearchText OR 
                  Definition LIKE @SearchText OR 
                  RelatedField LIKE @SearchText
            ORDER BY Term";

                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand(query, _connection))
                {
                    // Используем параметр с символами % только если поле не пустое
                    cmd.Parameters.AddWithValue("@SearchText", string.IsNullOrEmpty(searchText) ? "" : $"%{searchText}%");

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                // Привязываем данные к DataGridView (предполагается, что у вас есть dataGridViewTermins)
                dataGridViewTermins.DataSource = dt;
                ConfigureGlossaryGridView();

                // Логируем действие поиска
                LogAdminAction("SearchGlossary", $"Поиск терминов: {searchText}", null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске терминов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ConfigureGlossaryGridView()
        {
            dataGridViewTermins.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewTermins.ReadOnly = true; // По умолчанию редактирование запрещено
            dataGridViewTermins.AllowUserToAddRows = false; // По умолчанию нельзя добавлять строки
            dataGridViewTermins.MultiSelect = false;

            // Включаем отслеживание изменений
            if (dataGridViewTermins.DataSource is DataTable dt)
            {
                dt.AcceptChanges();
            }

            // Скрываем TermId
            if (dataGridViewTermins.Columns.Contains("TermId"))
                dataGridViewTermins.Columns["TermId"].Visible = false;

            // Устанавливаем заголовки столбцов
            if (dataGridViewTermins.Columns.Contains("Term"))
                dataGridViewTermins.Columns["Term"].HeaderText = "Термин";
            if (dataGridViewTermins.Columns.Contains("Definition"))
                dataGridViewTermins.Columns["Definition"].HeaderText = "Определение";
            if (dataGridViewTermins.Columns.Contains("RelatedField"))
                dataGridViewTermins.Columns["RelatedField"].HeaderText = "Область применения";

            dataGridViewTermins.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dataGridViewTermins.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10);
            dataGridViewTermins.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        }

        private void buttonBoolChange_Click(object sender, EventArgs e)
        {
            try
            {
                // Переключаем режим редактирования
                dataGridViewTermins.ReadOnly = !dataGridViewTermins.ReadOnly;

                // Обновляем текст кнопки
                buttonBoolChange.Text = dataGridViewTermins.ReadOnly ? "Редактировать" : "Завершить редактирование";

                // Изменяем цвет кнопки для визуального отличия режимов
                buttonBoolChange.BackColor = dataGridViewTermins.ReadOnly ? SystemColors.Control : Color.LightGreen;

                // Если включаем редактирование, разрешаем добавление новых строк
                dataGridViewTermins.AllowUserToAddRows = !dataGridViewTermins.ReadOnly;

                // Если выключаем редактирование, отменяем все текущие изменения
                if (dataGridViewTermins.ReadOnly)
                {
                    if (dataGridViewTermins.DataSource is DataTable dt)
                    {
                        dt.RejectChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при изменении режима редактирования: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSaveGlossary_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, есть ли изменения для сохранения
                if (dataGridViewTermins.DataSource is DataTable dataTable && dataTable.GetChanges() != null)
                {
                    // Начинаем транзакцию
                    using (SqlTransaction transaction = _connection.BeginTransaction())
                    {
                        try
                        {
                            // Обрабатываем каждую строку в DataTable
                            foreach (DataRow row in dataTable.Rows)
                            {
                                switch (row.RowState)
                                {
                                    case DataRowState.Added:
                                        InsertGlossaryTerm(row, transaction);
                                        break;
                                    case DataRowState.Modified:
                                        UpdateGlossaryTerm(row, transaction);
                                        break;
                                    case DataRowState.Deleted:
                                        // Для удаленных строк нужно использовать оригинальные значения
                                        DeleteGlossaryTerm(dataTable.NewRow().ItemArray, transaction);
                                        break;
                                }
                            }

                            // Фиксируем транзакцию
                            transaction.Commit();

                            // Принимаем изменения
                            dataTable.AcceptChanges();

                            // Обновляем данные в DataGridView
                            button1_Click(sender, e); // Перезагружаем данные

                            MessageBox.Show("Изменения успешно сохранены!", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Логируем действие
                            LogAdminAction("SaveGlossary", "Сохранение изменений в глоссарии", null, "Glossary");
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Нет изменений для сохранения.", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InsertGlossaryTerm(DataRow row, SqlTransaction transaction)
        {
            // Проверяем обязательные поля
            if (string.IsNullOrWhiteSpace(row["Term"].ToString()))
            {
                throw new Exception("Термин не может быть пустым.");
            }

            if (string.IsNullOrWhiteSpace(row["Definition"].ToString()))
            {
                throw new Exception("Определение не может быть пустым.");
            }

            string query = @"
        INSERT INTO Glossary (Term, Definition, RelatedField)
        VALUES (@Term, @Definition, @RelatedField)";

            using (SqlCommand cmd = new SqlCommand(query, _connection, transaction))
            {
                cmd.Parameters.AddWithValue("@Term", row["Term"]);
                cmd.Parameters.AddWithValue("@Definition", row["Definition"]);
                cmd.Parameters.AddWithValue("@RelatedField",
                    row.IsNull("RelatedField") ? DBNull.Value : row["RelatedField"]);

                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateGlossaryTerm(DataRow row, SqlTransaction transaction)
        {
            // Проверяем обязательные поля
            if (string.IsNullOrWhiteSpace(row["Term"].ToString()))
            {
                throw new Exception("Термин не может быть пустым.");
            }

            if (string.IsNullOrWhiteSpace(row["Definition"].ToString()))
            {
                throw new Exception("Определение не может быть пустым.");
            }

            string query = @"
        UPDATE Glossary
        SET Term = @Term,
            Definition = @Definition,
            RelatedField = @RelatedField
        WHERE TermId = @TermId";

            using (SqlCommand cmd = new SqlCommand(query, _connection, transaction))
            {
                cmd.Parameters.AddWithValue("@TermId", row["TermId"]);
                cmd.Parameters.AddWithValue("@Term", row["Term"]);
                cmd.Parameters.AddWithValue("@Definition", row["Definition"]);
                cmd.Parameters.AddWithValue("@RelatedField",
                    row.IsNull("RelatedField") ? DBNull.Value : row["RelatedField"]);

                cmd.ExecuteNonQuery();
            }
        }

        private void DeleteGlossaryTerm(object[] originalValues, SqlTransaction transaction)
        {
            // originalValues содержит оригинальные значения удаленной строки
            int termId = (int)originalValues[0]; // TermId всегда первый в массиве

            string query = "DELETE FROM Glossary WHERE TermId = @TermId";

            using (SqlCommand cmd = new SqlCommand(query, _connection, transaction))
            {
                cmd.Parameters.AddWithValue("@TermId", termId);
                cmd.ExecuteNonQuery();
            }
        }
    }


}