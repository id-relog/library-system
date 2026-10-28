using cursuch2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace cursuch2
{
    public partial class BookRedactForm : Form
    {
        private int _adminId;
        private SqlConnection _connection;
        private int _bookId;
        private Dictionary<int, string> _originalValues = new Dictionary<int, string>();
        private List<int> _originalAuthorIds = new List<int>();
        private List<int> _originalGenreIds = new List<int>();
        private List<int> _selectedAuthorIds = new List<int>();
        private List<int> _selectedGenreIds = new List<int>();
        private Dictionary<int, string> _allAuthors = new Dictionary<int, string>();
        private Dictionary<int, string> _allGenres = new Dictionary<int, string>();
        private AutoCompleteStringCollection _authorSuggestions = new AutoCompleteStringCollection();
        private AutoCompleteStringCollection _genreSuggestions = new AutoCompleteStringCollection();

        public BookRedactForm(SqlConnection sqlConnection, int adminId, int bookId)
        {
            InitializeComponent();
            _connection = sqlConnection ?? throw new ArgumentNullException(nameof(sqlConnection));
            _adminId = adminId;
            _bookId = bookId;

            if (_connection.State == ConnectionState.Closed)
            {
                throw new InvalidOperationException("Соединение с базой данных закрыто перед открытием формы.");
            }
        }

        private void BookRedactForm_Load(object sender, EventArgs e)
        {
            LoadBookData();
            if (_originalValues.Count == 0)
            {
                MessageBox.Show($"Книга с ID {_bookId} не найдена или данные не загружены.", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            LoadPublishers();
            LoadAuthors();
            LoadGenres();

            textBoxAuthors.AutoCompleteCustomSource = _authorSuggestions;
            textBoxGenres.AutoCompleteCustomSource = _genreSuggestions;

            textBoxAuthors.KeyDown += textBoxAuthors_KeyDown;
            textBoxGenres.KeyDown += textBoxGenres_KeyDown;

            textBoxAuthors.Leave += textBoxAuthors_Leave;
            textBoxGenres.Leave += textBoxGenres_Leave;

            SyncSelectedIds();
            UpdateAuthorsTextBox();
            UpdateGenresTextBox();
        }

        private void LoadBookData()
        {
            try
            {
                using (var command = new SqlCommand(
                    @"SELECT b.Title, b.PublicationYear, b.CopiesAvailable, b.TotalCopies, p.PublisherId, p.PublisherName 
                      FROM Books b 
                      LEFT JOIN Publishers p ON b.PublisherId = p.PublisherId 
                      WHERE b.BookId = @BookId", _connection))
                {
                    command.Parameters.AddWithValue("@BookId", _bookId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            textBox1.Text = reader["Title"]?.ToString() ?? "";
                            textBox2.Text = reader["PublicationYear"]?.ToString() ?? "";
                            textBox3.Text = reader["CopiesAvailable"]?.ToString() ?? "";
                            textBox4.Text = reader["TotalCopies"]?.ToString() ?? "";

                            _originalValues.Clear();
                            _originalValues.Add(1, textBox1.Text);
                            _originalValues.Add(2, textBox2.Text);
                            _originalValues.Add(3, textBox3.Text);
                            _originalValues.Add(4, textBox4.Text);
                            _originalValues.Add(5, reader["PublisherId"]?.ToString() ?? "");
                        }
                        else
                        {
                            textBox1.Text = "";
                            textBox2.Text = "";
                            textBox3.Text = "";
                            textBox4.Text = "";
                            _originalValues.Clear();
                        }
                    }
                }

                _originalAuthorIds.Clear();
                _selectedAuthorIds.Clear();
                using (var command = new SqlCommand(
                    @"SELECT a.AuthorId, a.FullName 
                      FROM Authors a 
                      JOIN BookAuthors ba ON a.AuthorId = ba.AuthorId 
                      WHERE ba.BookId = @BookId", _connection))
                {
                    command.Parameters.AddWithValue("@BookId", _bookId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int authorId = (int)reader["AuthorId"];
                            _originalAuthorIds.Add(authorId);
                            _selectedAuthorIds.Add(authorId);
                        }
                    }
                }
                UpdateAuthorsTextBox();

                _originalGenreIds.Clear();
                _selectedGenreIds.Clear();
                using (var command = new SqlCommand(
                    @"SELECT g.GenreId, g.GenreName 
                      FROM Genres g 
                      JOIN BookGenres bg ON g.GenreId = bg.GenreId 
                      WHERE bg.BookId = @BookId", _connection))
                {
                    command.Parameters.AddWithValue("@BookId", _bookId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int genreId = (int)reader["GenreId"];
                            _originalGenreIds.Add(genreId);
                            _selectedGenreIds.Add(genreId);
                        }
                    }
                }
                UpdateGenresTextBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных книги: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void LoadPublishers()
        {
            try
            {
                comboBoxPublisher.DisplayMember = "Value";
                comboBoxPublisher.ValueMember = "Key";

                using (var cmd = new SqlCommand("SELECT PublisherId, PublisherName FROM Publishers", _connection))
                {
                    var publishers = new List<KeyValuePair<int, string>>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            publishers.Add(new KeyValuePair<int, string>(
                                reader.GetInt32(0),
                                reader.GetString(1)));
                        }
                    }
                    comboBoxPublisher.DataSource = publishers;
                }

                if (_originalValues.ContainsKey(5) && !string.IsNullOrEmpty(_originalValues[5]))
                {
                    comboBoxPublisher.SelectedValue = int.Parse(_originalValues[5]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке издателей: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAuthors()
        {
            try
            {
                _allAuthors.Clear();
                _authorSuggestions.Clear();
                using (var command = new SqlCommand("SELECT AuthorId, FullName FROM Authors", _connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int authorId = (int)reader["AuthorId"];
                            string fullName = reader["FullName"].ToString();
                            _allAuthors[authorId] = fullName;
                            _authorSuggestions.Add(fullName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке авторов: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGenres()
        {
            try
            {
                _allGenres.Clear();
                _genreSuggestions.Clear();
                using (var command = new SqlCommand("SELECT GenreId, GenreName FROM Genres", _connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int genreId = (int)reader["GenreId"];
                            string genreName = reader["GenreName"].ToString();
                            _allGenres[genreId] = genreName;
                            _genreSuggestions.Add(genreName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке жанров: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SyncSelectedIds()
        {
            _selectedAuthorIds = _selectedAuthorIds.Where(id => _allAuthors.ContainsKey(id)).ToList();
            _selectedGenreIds = _selectedGenreIds.Where(id => _allGenres.ContainsKey(id)).ToList();
        }

        private void ProcessMultiValues(TextBox textBox, Dictionary<int, string> sourceDict, List<int> targetList, AutoCompleteStringCollection suggestions)
        {
            string input = textBox.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                targetList.Clear();
                return;
            }

            var parts = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                             .Select(part => part.Trim())
                             .Distinct()
                             .ToList();

            targetList.Clear();

            bool hasError = false;
            List<string> validItems = new List<string>();

            foreach (var part in parts)
            {
                var item = sourceDict.FirstOrDefault(x =>
                    x.Value.Equals(part, StringComparison.OrdinalIgnoreCase));

                if (item.Key != 0)
                {
                    if (!targetList.Contains(item.Key))
                    {
                        targetList.Add(item.Key);
                        validItems.Add(item.Value);
                    }
                }
                else
                {
                    if (suggestions.Contains(part))
                    {
                        if (textBox == textBoxAuthors)
                            LoadAuthors();
                        else
                            LoadGenres();

                        item = sourceDict.FirstOrDefault(x =>
                            x.Value.Equals(part, StringComparison.OrdinalIgnoreCase));

                        if (item.Key != 0)
                        {
                            targetList.Add(item.Key);
                            validItems.Add(item.Value);
                        }
                        else
                        {
                            hasError = true;
                        }
                    }
                    else
                    {
                        hasError = true;
                    }
                }
            }

            if (hasError)
            {
                MessageBox.Show("Некоторые значения не найдены в базе данных. Проверьте ввод.",
                               "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            textBox.Text = string.Join(", ", validItems);
            textBox.SelectionStart = textBox.Text.Length;
        }

        private void textBoxAuthors_Leave(object sender, EventArgs e)
        {
            ProcessMultiValues(textBoxAuthors, _allAuthors, _selectedAuthorIds, _authorSuggestions);
        }

        private void textBoxGenres_Leave(object sender, EventArgs e)
        {
            ProcessMultiValues(textBoxGenres, _allGenres, _selectedGenreIds, _genreSuggestions);
        }

        private void UpdateAuthorsTextBox()
        {
            var validAuthors = _selectedAuthorIds
                .Where(id => _allAuthors.ContainsKey(id))
                .Select(id => _allAuthors[id])
                .ToList();
            textBoxAuthors.Text = string.Join(", ", validAuthors);
        }

        private void UpdateGenresTextBox()
        {
            var validGenres = _selectedGenreIds
                .Where(id => _allGenres.ContainsKey(id))
                .Select(id => _allGenres[id])
                .ToList();
            textBoxGenres.Text = string.Join(", ", validGenres);
        }

        private void textBoxAuthors_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Oemcomma)
            {
                e.SuppressKeyPress = true;
                ProcessMultiValues(textBoxAuthors, _allAuthors, _selectedAuthorIds, _authorSuggestions);
            }
        }

        private void textBoxGenres_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Oemcomma)
            {
                e.SuppressKeyPress = true;
                ProcessMultiValues(textBoxGenres, _allGenres, _selectedGenreIds, _genreSuggestions);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            textBox1.Text = _originalValues.ContainsKey(1) ? _originalValues[1] : "";
            textBox2.Text = _originalValues.ContainsKey(2) ? _originalValues[2] : "";
            textBox3.Text = _originalValues.ContainsKey(3) ? _originalValues[3] : "";
            textBox4.Text = _originalValues.ContainsKey(4) ? _originalValues[4] : "";

            if (_originalValues.ContainsKey(5) && !string.IsNullOrEmpty(_originalValues[5]))
            {
                comboBoxPublisher.SelectedValue = int.Parse(_originalValues[5]);
            }
            else
            {
                comboBoxPublisher.SelectedIndex = -1;
            }

            _selectedAuthorIds.Clear();
            _selectedAuthorIds.AddRange(_originalAuthorIds);
            SyncSelectedIds();
            UpdateAuthorsTextBox();

            _selectedGenreIds.Clear();
            _selectedGenreIds.AddRange(_originalGenreIds);
            SyncSelectedIds();
            UpdateGenresTextBox();
        }

        private bool HasChanges()
        {
            bool hasChanges = false;

            if (textBox1.Text != _originalValues[1] ||
                textBox2.Text != _originalValues[2] ||
                textBox3.Text != _originalValues[3] ||
                textBox4.Text != _originalValues[4])
            {
                hasChanges = true;
            }

            if (comboBoxPublisher.SelectedItem is KeyValuePair<int, string> selectedPublisher)
            {
                string currentPublisherId = selectedPublisher.Key.ToString();
                string originalPublisherId = _originalValues.ContainsKey(5) ? _originalValues[5] : string.Empty;
                if (currentPublisherId != originalPublisherId)
                {
                    hasChanges = true;
                }
            }
            else if (_originalValues.ContainsKey(5) && !string.IsNullOrEmpty(_originalValues[5]))
            {
                hasChanges = true;
            }

            var currentAuthors = _selectedAuthorIds.OrderBy(id => id).ToList();
            var originalAuthors = _originalAuthorIds.OrderBy(id => id).ToList();
            if (!currentAuthors.SequenceEqual(originalAuthors))
            {
                hasChanges = true;
            }

            var currentGenres = _selectedGenreIds.OrderBy(id => id).ToList();
            var originalGenres = _originalGenreIds.OrderBy(id => id).ToList();
            if (!currentGenres.SequenceEqual(originalGenres))
            {
                hasChanges = true;
            }

            return hasChanges;
        }

        private void SaveChangesToDatabase(SqlTransaction transaction)
        {
            using (SqlCommand cmd = new SqlCommand(
                "DELETE FROM BookAuthors WHERE BookId = @BookId", _connection, transaction))
            {
                cmd.Parameters.AddWithValue("@BookId", _bookId);
                cmd.ExecuteNonQuery();
            }

            foreach (int authorId in _selectedAuthorIds.Distinct())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO BookAuthors (BookId, AuthorId) VALUES (@BookId, @AuthorId)",
                    _connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@BookId", _bookId);
                    cmd.Parameters.AddWithValue("@AuthorId", authorId);
                    cmd.ExecuteNonQuery();
                }
            }

            using (SqlCommand cmd = new SqlCommand(
                "DELETE FROM BookGenres WHERE BookId = @BookId", _connection, transaction))
            {
                cmd.Parameters.AddWithValue("@BookId", _bookId);
                cmd.ExecuteNonQuery();
            }

            foreach (int genreId in _selectedGenreIds.Distinct())
            {
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO BookGenres (BookId, GenreId) VALUES (@BookId, @GenreId)",
                    _connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@BookId", _bookId);
                    cmd.Parameters.AddWithValue("@GenreId", genreId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Проверка заполнения обязательных полей
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                comboBoxPublisher.SelectedItem == null)
            {
                MessageBox.Show("Заполните обязательные поля: Название и Издатель", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Проверка числовых полей
            if (!int.TryParse(textBox2.Text, out int publicationYear) || publicationYear <= 0)
            {
                MessageBox.Show("Укажите корректный год публикации", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textBox3.Text, out int copiesAvailable) || copiesAvailable < 0)
            {
                MessageBox.Show("Укажите корректное количество доступных копий", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textBox4.Text, out int totalCopies) || totalCopies < copiesAvailable)
            {
                MessageBox.Show("Общее количество копий не может быть меньше доступных", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Обновляем списки перед сохранением
            ProcessMultiValues(textBoxAuthors, _allAuthors, _selectedAuthorIds, _authorSuggestions);
            ProcessMultiValues(textBoxGenres, _allGenres, _selectedGenreIds, _genreSuggestions);

            // Проверка изменений
            if (!HasChanges())
            {
                MessageBox.Show("Изменения отсутствуют", "Информация",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (SqlTransaction transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        int publisherId = ((KeyValuePair<int, string>)comboBoxPublisher.SelectedItem).Key;

                        using (SqlCommand cmd = new SqlCommand(
                            @"UPDATE Books SET 
                                Title = @Title, 
                                PublicationYear = @PublicationYear, 
                                CopiesAvailable = @CopiesAvailable, 
                                TotalCopies = @TotalCopies, 
                                PublisherId = @PublisherId 
                              WHERE BookId = @BookId", _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Title", textBox1.Text.Trim());
                            cmd.Parameters.AddWithValue("@PublicationYear", publicationYear);
                            cmd.Parameters.AddWithValue("@CopiesAvailable", copiesAvailable);
                            cmd.Parameters.AddWithValue("@TotalCopies", totalCopies);
                            cmd.Parameters.AddWithValue("@PublisherId", publisherId);
                            cmd.Parameters.AddWithValue("@BookId", _bookId);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected == 0)
                            {
                                throw new Exception("Не удалось обновить информацию о книге");
                            }
                        }

                        SaveChangesToDatabase(transaction);

                        using (SqlCommand cmd = new SqlCommand(
                            @"INSERT INTO AdminLogs 
                                (ActionTime, ActionType, ActionDetails, AdminId, AffectedId, AffectedType) 
                              VALUES 
                                (GETDATE(), 'Обновление книги', 'Данные книги были изменены', @AdminId, @BookId, 'Book')",
                            _connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@AdminId", _adminId);
                            cmd.Parameters.AddWithValue("@BookId", _bookId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show("Изменения успешно сохранены", "Успех",
                                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}", "Ошибка",
                                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка соединения с базой данных: {ex.Message}", "Ошибка",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
    }
}