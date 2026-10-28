using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using System.Data.Common;

namespace cursuch2
{
    public partial class FormNewBook : Form
    {
        private SqlConnection _connection;
        private int currentAdminId;

        public FormNewBook(int adminId, SqlConnection connection)
        {
            InitializeComponent();
            currentAdminId = adminId;
            _connection = connection;
            LoadAutoCompleteData();
        }

        private void LogAction(string actionType, string actionDetails, int? affectedId = null, string affectedType = null)
        {
            try
            {

                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                SqlCommand command = new SqlCommand(
                    "INSERT INTO AdminLogs (ActionTime, ActionType, ActionDetails, AdminId, AffectedId, AffectedType) " +
                    "VALUES (GETDATE(), @ActionType, @ActionDetails, @AdminId, @AffectedId, @AffectedType)", _connection);

                command.Parameters.AddWithValue("@ActionType", actionType);
                command.Parameters.AddWithValue("@ActionDetails", actionDetails);
                command.Parameters.AddWithValue("@AdminId", currentAdminId);

                if (affectedId.HasValue)
                    command.Parameters.AddWithValue("@AffectedId", affectedId.Value);
                else
                    command.Parameters.AddWithValue("@AffectedId", DBNull.Value);

                if (!string.IsNullOrEmpty(affectedType))
                    command.Parameters.AddWithValue("@AffectedType", affectedType);
                else
                    command.Parameters.AddWithValue("@AffectedType", DBNull.Value);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при логировании: {ex.Message}");
            }
        }

        private void LoadAutoCompleteData()
        {
            // Автодополнение для авторов
            var authors = GetDataFromDatabase("SELECT FullName FROM Authors");
            txtAushor.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            txtAushor.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtAushor.AutoCompleteSource = AutoCompleteSource.CustomSource;
            foreach (DataRow row in authors.Rows)
            {
                txtAushor.AutoCompleteCustomSource.Add(row["FullName"].ToString());
            }

            // Автодополнение для издателей
            var publishers = GetDataFromDatabase("SELECT PublisherName FROM Publishers");
            txtPublysher.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            txtPublysher.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtPublysher.AutoCompleteSource = AutoCompleteSource.CustomSource;
            foreach (DataRow row in publishers.Rows)
            {
                txtPublysher.AutoCompleteCustomSource.Add(row["PublisherName"].ToString());
            }

            var genres = GetDataFromDatabase("SELECT GenreName FROM Genres");
            txtGenreName.AutoCompleteCustomSource = new AutoCompleteStringCollection();
            txtGenreName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtGenreName.AutoCompleteSource = AutoCompleteSource.CustomSource;
            foreach (DataRow row in genres.Rows)
            {
                txtGenreName.AutoCompleteCustomSource.Add(row["GenreName"].ToString());
            }
        }

        private DataTable GetDataFromDatabase(string query)
        {
            DataTable dataTable = new DataTable();
            // Используем существующее соединение
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            SqlCommand command = new SqlCommand(query, _connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable);

            return dataTable;
        }

        private void buttonNewAuthor_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageAddAuthor;
            txtNewAuthor.Focus();
        }

        private void buttonNewPublisher_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageAddPublisher;
            txtPublisherName.Focus();
        }

        private void buttonAddGenres_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPageAddGenres;
            txtGenreName.Focus();
        }

        private void buttonAddNewAuthor_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewAuthor.Text))
            {
                MessageBox.Show("Введите ФИО автора", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Проверка на существующего автора
            if (AuthorExists(txtNewAuthor.Text))
            {
                MessageBox.Show("Такой автор уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                int authorId;
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                SqlCommand command = new SqlCommand(
                    "INSERT INTO Authors (FullName) OUTPUT INSERTED.AuthorId VALUES (@FullName)", _connection);
                command.Parameters.AddWithValue("@FullName", txtNewAuthor.Text);
                authorId = (int)command.ExecuteScalar();

                // Логирование действия
                LogAction("AddAuthor", $"Добавлен новый автор: {txtNewAuthor.Text}", authorId, "Author");

                MessageBox.Show("Автор успешно добавлен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtNewAuthor.Clear();
                LoadAutoCompleteData(); // Обновляем автодополнение
                tabControl1.SelectedTab = tabPageAddBook;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении автора: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogAction("Error", $"Ошибка при добавлении автора: {ex.Message}");
            }
        }

        private bool AuthorExists(string authorName)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            SqlCommand command = new SqlCommand(
                "SELECT COUNT(*) FROM Authors WHERE FullName = @FullName", _connection);
            command.Parameters.AddWithValue("@FullName", authorName);
            int count = (int)command.ExecuteScalar();
            return count > 0;
        }

        private void buttonAddNewPublisher_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPublisherName.Text))
            {
                MessageBox.Show("Введите название издательства", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPublisherContactInfo.Text))
            {
                MessageBox.Show("Введите контактную информацию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPublisherAdress.Text))
            {
                MessageBox.Show("Введите адрес", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            // Проверка на существующее издательство
            if (PublisherExists(txtPublisherName.Text))
            {
                MessageBox.Show("Такое издательство уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                int publisherId;
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                SqlCommand command = new SqlCommand(
                    "INSERT INTO Publishers (PublisherName, Address, ContactInfo) " +
                    "OUTPUT INSERTED.PublisherId VALUES (@PublisherName, @Address, @ContactInfo)", _connection);
                command.Parameters.AddWithValue("@PublisherName", txtPublisherName.Text);
                command.Parameters.AddWithValue("@Address", txtPublisherAdress.Text);
                command.Parameters.AddWithValue("@ContactInfo", txtPublisherContactInfo.Text);
                publisherId = (int)command.ExecuteScalar();

                // Логирование действия
                LogAction("AddPublisher", $"Добавлено новое издательство: {txtPublisherName.Text}", publisherId, "Publisher");

                MessageBox.Show("Издательство успешно добавлено", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPublisherName.Clear();
                txtPublisherAdress.Clear();
                txtPublisherContactInfo.Clear();
                LoadAutoCompleteData(); // Обновляем автодополнение
                tabControl1.SelectedTab = tabPageAddBook;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении издательства: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogAction("Error", $"Ошибка при добавлении издательства: {ex.Message}");
            }
        }

        private bool PublisherExists(string publisherName)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            SqlCommand command = new SqlCommand(
                "SELECT COUNT(*) FROM Publishers WHERE PublisherName = @PublisherName", _connection);
            command.Parameters.AddWithValue("@PublisherName", publisherName);
            int count = (int)command.ExecuteScalar();
            return count > 0;
        }

        private void buttonAddNewBook_Click_1(object sender, EventArgs e)
        {
            // Валидация данных
            if (string.IsNullOrWhiteSpace(txtBookName.Text))
            {
                MessageBox.Show("Введите название книги", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAushor.Text))
            {
                MessageBox.Show("Введите автора книги", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPublysher.Text))
            {
                MessageBox.Show("Введите издательство", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtPublicYear.Text, out int year) || year < 1000 || year > DateTime.Now.Year)
            {
                MessageBox.Show("Введите корректный год публикации", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtCountCopies.Text, out int copies) || copies <= 0)
            {
                MessageBox.Show("Введите корректное количество копий", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtGenre.Text))
            {
                MessageBox.Show("Введите издательство", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                // Проверяем/добавляем автора
                int authorId = GetAuthor(txtAushor.Text);

                if (authorId == -1)
                {
                    MessageBox.Show("Этого автора нету в базе, вы можете его добавить в разделе: Добавление авторов");
                    return;
                }
                int genreId = GetGenres(txtGenreName.Text);
                if (genreId == -1)
                {
                    MessageBox.Show("Этого жанра нету в базе, вы можете его добавить в разделе: Добавление жанров");
                    return;
                }
                // Проверяем/добавляем издательство
                int publisherId = GetPublisher(txtPublysher.Text);
                if (publisherId == -1)
                {
                    MessageBox.Show("Этого издателя нету в базе, вы можете его добавить в разделе: Добавление издательства");
                    return;
                }
                // Добавляем книгу
                int bookId = AddBook(txtBookName.Text, publisherId, year, copies);

                // Связываем книгу с автором и жанром
                LinkBookWithAuthor(bookId, authorId);
                LinkBookWithGenres(bookId, genreId);

                // Логирование действия
                LogAction("AddBook", $"Добавлена новая книга: {txtBookName.Text}", bookId, "Book");
                LogAction("LinkAuthor", $"Связь книги с автором: {txtAushor.Text}", bookId, "BookAuthor");
                LogAction("LinkGenre", $"Связь книги с жанром: {txtGenre.Text}", bookId, "BookGenres");
                LogAction("LinkPublysher", $"Связь книги с издателем: {txtPublysher.Text}", bookId, "Book");


                MessageBox.Show($"Книга успешно добавлена:\nНазвание: {txtBookName.Text}\nАвтор: {txtAushor.Text}\nИздательство: {txtPublysher.Text}",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearBookForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении книги: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogAction("Error", $"Ошибка при добавлении книги: {ex.Message}");
            }
        }

        private int GetAuthor(string authorName)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            // Проверяем существование автора
            SqlCommand checkCmd = new SqlCommand(
                "SELECT AuthorId FROM Authors WHERE FullName = @FullName", _connection);
            checkCmd.Parameters.AddWithValue("@FullName", authorName);
            object result = checkCmd.ExecuteScalar();

            if (result != null)
            {
                return (int)result;
            }





            return -1;
        }

        private int GetPublisher(string publisherName)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            // Проверяем существование издательства
            SqlCommand checkCmd = new SqlCommand(
                "SELECT PublisherId FROM Publishers WHERE PublisherName = @PublisherName", _connection);
            checkCmd.Parameters.AddWithValue("@PublisherName", publisherName);
            object result = checkCmd.ExecuteScalar();

            if (result != null)
            {
                return (int)result;
            }

            return -1;
        }



        private int AddBook(string title, int publisherId, int year, int copies)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            SqlCommand command = new SqlCommand(
                "INSERT INTO Books (Title, PublisherId, PublicationYear, CopiesAvailable, TotalCopies) " +
                "OUTPUT INSERTED.BookId VALUES (@Title, @PublisherId, @PublicationYear, @CopiesAvailable, @TotalCopies)", _connection);
            command.Parameters.AddWithValue("@Title", title);
            command.Parameters.AddWithValue("@PublisherId", publisherId);
            command.Parameters.AddWithValue("@PublicationYear", year);
            command.Parameters.AddWithValue("@CopiesAvailable", copies);
            command.Parameters.AddWithValue("@TotalCopies", copies);
            return (int)command.ExecuteScalar();
        }

        private void LinkBookWithAuthor(int bookId, int authorId)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            SqlCommand command = new SqlCommand(
                "INSERT INTO BookAuthors (AuthorId, BookId) VALUES (@AuthorId, @BookId)", _connection);
            command.Parameters.AddWithValue("@AuthorId", authorId);
            command.Parameters.AddWithValue("@BookId", bookId);
            command.ExecuteNonQuery();
        }

        private void LinkBookWithGenres(int bookId, int GenreId)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            SqlCommand command = new SqlCommand(
                "INSERT INTO BookGenres (GenreId, BookId) VALUES (@GenreId, @BookId)", _connection);
            command.Parameters.AddWithValue("@GenreId", GenreId);
            command.Parameters.AddWithValue("@BookId", bookId);
            command.ExecuteNonQuery();
        }

        

        private void ClearBookForm()
        {
            txtBookName.Clear();
            txtAushor.Clear();
            txtPublysher.Clear();
            txtPublicYear.Clear();
            txtCountCopies.Clear();
            txtBookName.Focus();
        }

        private void button1_Click(object sender, EventArgs e) // добавление жанров
        {
            if (string.IsNullOrEmpty(txtGenreName.Text))
            {
                MessageBox.Show("Введите название");
                return; // Exit the method if the input is empty
            }

            if (GetGenres(txtGenreName.Text) != -1)
            {
                MessageBox.Show("Такой жанр уже присутствует в базе");
                return; // Exit the method if the genre already exists
            }

            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                SqlCommand command = new SqlCommand("INSERT INTO Genres (GenreName) VALUES (@GenreName)", _connection);
                command.Parameters.AddWithValue("@GenreName", txtGenreName.Text);
                command.ExecuteNonQuery();

                MessageBox.Show("Жанр успешно добавлен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtGenreName.Clear();
                LoadAutoCompleteData(); // Refresh autocomplete data
                tabControl1.SelectedTab = tabPageAddBook; // Return to the book tab
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении жанра: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private int GetGenres(string GenresName)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            SqlCommand checkCmd = new SqlCommand(
                "SELECT GenreId FROM Genres WHERE GenreName = @GenreName", _connection);
            checkCmd.Parameters.AddWithValue("@GenreName", GenresName);
            object result = checkCmd.ExecuteScalar();

            if (result != null)
            {
                return (int)result;
            }

            return -1;
        }


    }
}