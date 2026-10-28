namespace cursuch2
{
    partial class FormNewBook
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPageAddBook = new TabPage();
            buttonAddGenres = new Button();
            label13 = new Label();
            txtGenre = new TextBox();
            txtPublysher = new TextBox();
            txtAushor = new TextBox();
            label6 = new Label();
            buttonAddNewBook = new Button();
            buttonNewPublisher = new Button();
            buttonNewAuthor = new Button();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            txtCountCopies = new TextBox();
            txtPublicYear = new TextBox();
            txtBookName = new TextBox();
            tabPageAddAuthor = new TabPage();
            buttonAddNewAuthor = new Button();
            txtNewAuthor = new TextBox();
            label7 = new Label();
            label8 = new Label();
            tabPageAddPublisher = new TabPage();
            buttonAddNewPublisher = new Button();
            label12 = new Label();
            txtPublisherContactInfo = new TextBox();
            txtPublisherAdress = new TextBox();
            txtPublisherName = new TextBox();
            label11 = new Label();
            label10 = new Label();
            label9 = new Label();
            tabPageAddGenres = new TabPage();
            button1 = new Button();
            label14 = new Label();
            txtGenreName = new TextBox();
            label17 = new Label();
            tabControl1.SuspendLayout();
            tabPageAddBook.SuspendLayout();
            tabPageAddAuthor.SuspendLayout();
            tabPageAddPublisher.SuspendLayout();
            tabPageAddGenres.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPageAddBook);
            tabControl1.Controls.Add(tabPageAddAuthor);
            tabControl1.Controls.Add(tabPageAddPublisher);
            tabControl1.Controls.Add(tabPageAddGenres);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1117, 596);
            tabControl1.TabIndex = 0;
            // 
            // tabPageAddBook
            // 
            tabPageAddBook.Controls.Add(buttonAddGenres);
            tabPageAddBook.Controls.Add(label13);
            tabPageAddBook.Controls.Add(txtGenre);
            tabPageAddBook.Controls.Add(txtPublysher);
            tabPageAddBook.Controls.Add(txtAushor);
            tabPageAddBook.Controls.Add(label6);
            tabPageAddBook.Controls.Add(buttonAddNewBook);
            tabPageAddBook.Controls.Add(buttonNewPublisher);
            tabPageAddBook.Controls.Add(buttonNewAuthor);
            tabPageAddBook.Controls.Add(label5);
            tabPageAddBook.Controls.Add(label4);
            tabPageAddBook.Controls.Add(label3);
            tabPageAddBook.Controls.Add(label2);
            tabPageAddBook.Controls.Add(label1);
            tabPageAddBook.Controls.Add(txtCountCopies);
            tabPageAddBook.Controls.Add(txtPublicYear);
            tabPageAddBook.Controls.Add(txtBookName);
            tabPageAddBook.Location = new Point(10, 55);
            tabPageAddBook.Name = "tabPageAddBook";
            tabPageAddBook.Padding = new Padding(3);
            tabPageAddBook.Size = new Size(1097, 531);
            tabPageAddBook.TabIndex = 0;
            tabPageAddBook.Text = "Добавление книги";
            tabPageAddBook.UseVisualStyleBackColor = true;
            // 
            // buttonAddGenres
            // 
            buttonAddGenres.Location = new Point(723, 284);
            buttonAddGenres.Name = "buttonAddGenres";
            buttonAddGenres.Size = new Size(262, 48);
            buttonAddGenres.TabIndex = 34;
            buttonAddGenres.Text = "Добавить жанр";
            buttonAddGenres.UseVisualStyleBackColor = true;
            buttonAddGenres.Click += buttonAddGenres_Click;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(341, 284);
            label13.Name = "label13";
            label13.Size = new Size(86, 37);
            label13.TabIndex = 33;
            label13.Text = "Жанр";
            // 
            // txtGenre
            // 
            txtGenre.Location = new Point(434, 284);
            txtGenre.Name = "txtGenre";
            txtGenre.Size = new Size(256, 43);
            txtGenre.TabIndex = 32;
            // 
            // txtPublysher
            // 
            txtPublysher.Location = new Point(434, 235);
            txtPublysher.Name = "txtPublysher";
            txtPublysher.Size = new Size(257, 43);
            txtPublysher.TabIndex = 31;
            // 
            // txtAushor
            // 
            txtAushor.Location = new Point(434, 184);
            txtAushor.Name = "txtAushor";
            txtAushor.Size = new Size(257, 43);
            txtAushor.TabIndex = 30;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Sitka Banner", 13.999999F, FontStyle.Italic, GraphicsUnit.Point, 204);
            label6.Location = new Point(409, 3);
            label6.Margin = new Padding(0);
            label6.Name = "label6";
            label6.Size = new Size(308, 61);
            label6.TabIndex = 29;
            label6.Text = "Добавление книг";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // buttonAddNewBook
            // 
            buttonAddNewBook.Location = new Point(434, 393);
            buttonAddNewBook.Name = "buttonAddNewBook";
            buttonAddNewBook.Size = new Size(238, 52);
            buttonAddNewBook.TabIndex = 26;
            buttonAddNewBook.Text = "Добавить книгу";
            buttonAddNewBook.UseVisualStyleBackColor = true;
            buttonAddNewBook.Click += buttonAddNewBook_Click_1;
            // 
            // buttonNewPublisher
            // 
            buttonNewPublisher.Location = new Point(723, 230);
            buttonNewPublisher.Name = "buttonNewPublisher";
            buttonNewPublisher.Size = new Size(262, 48);
            buttonNewPublisher.TabIndex = 25;
            buttonNewPublisher.Text = "Добавить издателя";
            buttonNewPublisher.UseVisualStyleBackColor = true;
            buttonNewPublisher.Click += buttonNewPublisher_Click_1;
            // 
            // buttonNewAuthor
            // 
            buttonNewAuthor.Location = new Point(723, 181);
            buttonNewAuthor.Name = "buttonNewAuthor";
            buttonNewAuthor.Size = new Size(262, 43);
            buttonNewAuthor.TabIndex = 24;
            buttonNewAuthor.Text = "Добавить автора";
            buttonNewAuthor.UseVisualStyleBackColor = true;
            buttonNewAuthor.Click += buttonNewAuthor_Click_1;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(169, 333);
            label5.Name = "label5";
            label5.Size = new Size(259, 37);
            label5.TabIndex = 23;
            label5.Text = "Колличество копий";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(301, 233);
            label4.Name = "label4";
            label4.Size = new Size(131, 37);
            label4.TabIndex = 22;
            label4.Text = "Издатель";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(323, 184);
            label3.Name = "label3";
            label3.Size = new Size(105, 37);
            label3.TabIndex = 21;
            label3.Text = "Автора";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(210, 135);
            label2.Name = "label2";
            label2.Size = new Size(218, 37);
            label2.TabIndex = 20;
            label2.Text = "Год публикации";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(111, 86);
            label1.Name = "label1";
            label1.Size = new Size(317, 37);
            label1.TabIndex = 19;
            label1.Text = "Введите название книги";
            // 
            // txtCountCopies
            // 
            txtCountCopies.Location = new Point(434, 333);
            txtCountCopies.Name = "txtCountCopies";
            txtCountCopies.Size = new Size(256, 43);
            txtCountCopies.TabIndex = 18;
            // 
            // txtPublicYear
            // 
            txtPublicYear.Location = new Point(434, 135);
            txtPublicYear.Name = "txtPublicYear";
            txtPublicYear.Size = new Size(257, 43);
            txtPublicYear.TabIndex = 17;
            // 
            // txtBookName
            // 
            txtBookName.Location = new Point(434, 86);
            txtBookName.Name = "txtBookName";
            txtBookName.Size = new Size(257, 43);
            txtBookName.TabIndex = 16;
            // 
            // tabPageAddAuthor
            // 
            tabPageAddAuthor.Controls.Add(buttonAddNewAuthor);
            tabPageAddAuthor.Controls.Add(txtNewAuthor);
            tabPageAddAuthor.Controls.Add(label7);
            tabPageAddAuthor.Controls.Add(label8);
            tabPageAddAuthor.Location = new Point(10, 55);
            tabPageAddAuthor.Name = "tabPageAddAuthor";
            tabPageAddAuthor.Padding = new Padding(3);
            tabPageAddAuthor.Size = new Size(1097, 531);
            tabPageAddAuthor.TabIndex = 1;
            tabPageAddAuthor.Text = "Добавление автора";
            tabPageAddAuthor.UseVisualStyleBackColor = true;
            // 
            // buttonAddNewAuthor
            // 
            buttonAddNewAuthor.Location = new Point(456, 318);
            buttonAddNewAuthor.Name = "buttonAddNewAuthor";
            buttonAddNewAuthor.Size = new Size(192, 52);
            buttonAddNewAuthor.TabIndex = 32;
            buttonAddNewAuthor.Text = "Добавить";
            buttonAddNewAuthor.UseVisualStyleBackColor = true;
            buttonAddNewAuthor.Click += buttonAddNewAuthor_Click_1;
            // 
            // txtNewAuthor
            // 
            txtNewAuthor.Location = new Point(381, 197);
            txtNewAuthor.Name = "txtNewAuthor";
            txtNewAuthor.Size = new Size(355, 43);
            txtNewAuthor.TabIndex = 31;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Sitka Banner", 13.999999F, FontStyle.Italic, GraphicsUnit.Point, 204);
            label7.Location = new Point(381, 3);
            label7.Margin = new Padding(0);
            label7.Name = "label7";
            label7.Size = new Size(355, 61);
            label7.TabIndex = 30;
            label7.Text = "Добавление автора";
            label7.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(67, 203);
            label8.Name = "label8";
            label8.Size = new Size(308, 37);
            label8.TabIndex = 1;
            label8.Text = "Введите: Фамилия И. О.";
            // 
            // tabPageAddPublisher
            // 
            tabPageAddPublisher.Controls.Add(buttonAddNewPublisher);
            tabPageAddPublisher.Controls.Add(label12);
            tabPageAddPublisher.Controls.Add(txtPublisherContactInfo);
            tabPageAddPublisher.Controls.Add(txtPublisherAdress);
            tabPageAddPublisher.Controls.Add(txtPublisherName);
            tabPageAddPublisher.Controls.Add(label11);
            tabPageAddPublisher.Controls.Add(label10);
            tabPageAddPublisher.Controls.Add(label9);
            tabPageAddPublisher.Location = new Point(10, 55);
            tabPageAddPublisher.Name = "tabPageAddPublisher";
            tabPageAddPublisher.Padding = new Padding(3);
            tabPageAddPublisher.Size = new Size(1097, 531);
            tabPageAddPublisher.TabIndex = 2;
            tabPageAddPublisher.Text = "Добавление исдательства";
            tabPageAddPublisher.UseVisualStyleBackColor = true;
            // 
            // buttonAddNewPublisher
            // 
            buttonAddNewPublisher.Location = new Point(406, 390);
            buttonAddNewPublisher.Name = "buttonAddNewPublisher";
            buttonAddNewPublisher.Size = new Size(301, 45);
            buttonAddNewPublisher.TabIndex = 33;
            buttonAddNewPublisher.Text = "Добавить";
            buttonAddNewPublisher.UseVisualStyleBackColor = true;
            buttonAddNewPublisher.Click += buttonAddNewPublisher_Click_1;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Sitka Banner", 13.999999F, FontStyle.Italic, GraphicsUnit.Point, 204);
            label12.Location = new Point(348, 3);
            label12.Margin = new Padding(0);
            label12.Name = "label12";
            label12.Size = new Size(472, 61);
            label12.TabIndex = 31;
            label12.Text = "Добавление исдательства";
            label12.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtPublisherContactInfo
            // 
            txtPublisherContactInfo.Location = new Point(496, 250);
            txtPublisherContactInfo.Name = "txtPublisherContactInfo";
            txtPublisherContactInfo.Size = new Size(301, 43);
            txtPublisherContactInfo.TabIndex = 5;
            // 
            // txtPublisherAdress
            // 
            txtPublisherAdress.Location = new Point(496, 194);
            txtPublisherAdress.Name = "txtPublisherAdress";
            txtPublisherAdress.Size = new Size(301, 43);
            txtPublisherAdress.TabIndex = 4;
            // 
            // txtPublisherName
            // 
            txtPublisherName.Location = new Point(496, 137);
            txtPublisherName.Name = "txtPublisherName";
            txtPublisherName.Size = new Size(301, 43);
            txtPublisherName.TabIndex = 3;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(16, 250);
            label11.Name = "label11";
            label11.Size = new Size(465, 37);
            label11.TabIndex = 2;
            label11.Text = "Контактная информация (моб. тел.):";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(384, 194);
            label10.Name = "label10";
            label10.Size = new Size(97, 37);
            label10.TabIndex = 1;
            label10.Text = "Адрес:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(348, 137);
            label9.Name = "label9";
            label9.Size = new Size(142, 37);
            label9.TabIndex = 0;
            label9.Text = "Название:";
            // 
            // tabPageAddGenres
            // 
            tabPageAddGenres.Controls.Add(button1);
            tabPageAddGenres.Controls.Add(label14);
            tabPageAddGenres.Controls.Add(txtGenreName);
            tabPageAddGenres.Controls.Add(label17);
            tabPageAddGenres.Location = new Point(10, 55);
            tabPageAddGenres.Name = "tabPageAddGenres";
            tabPageAddGenres.Padding = new Padding(3);
            tabPageAddGenres.Size = new Size(1097, 531);
            tabPageAddGenres.TabIndex = 3;
            tabPageAddGenres.Text = "Добавление жанров";
            tabPageAddGenres.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(414, 418);
            button1.Name = "button1";
            button1.Size = new Size(301, 45);
            button1.TabIndex = 41;
            button1.Text = "Добавить";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new Font("Sitka Banner", 13.999999F, FontStyle.Italic, GraphicsUnit.Point, 204);
            label14.Location = new Point(414, 31);
            label14.Margin = new Padding(0);
            label14.Name = "label14";
            label14.Size = new Size(360, 61);
            label14.TabIndex = 40;
            label14.Text = "Добавление жанров";
            label14.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtGenreName
            // 
            txtGenreName.Location = new Point(473, 221);
            txtGenreName.Name = "txtGenreName";
            txtGenreName.Size = new Size(301, 43);
            txtGenreName.TabIndex = 37;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(325, 221);
            label17.Name = "label17";
            label17.Size = new Size(142, 37);
            label17.TabIndex = 34;
            label17.Text = "Название:";
            // 
            // FormNewBook
            // 
            AutoScaleDimensions = new SizeF(15F, 37F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1117, 596);
            Controls.Add(tabControl1);
            Name = "FormNewBook";
            Text = "FormNewBook";
            tabControl1.ResumeLayout(false);
            tabPageAddBook.ResumeLayout(false);
            tabPageAddBook.PerformLayout();
            tabPageAddAuthor.ResumeLayout(false);
            tabPageAddAuthor.PerformLayout();
            tabPageAddPublisher.ResumeLayout(false);
            tabPageAddPublisher.PerformLayout();
            tabPageAddGenres.ResumeLayout(false);
            tabPageAddGenres.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPageAddBook;
        private Button buttonAddNewBook;
        private Button buttonNewPublisher;
        private Button buttonNewAuthor;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private TextBox txtCountCopies;
        private TextBox txtPublicYear;
        private TextBox txtBookName;
        private TabPage tabPageAddAuthor;
        private Label label6;
        private Label label8;
        private TextBox txtPublysher;
        private TextBox txtAushor;
        private Label label7;
        private Button buttonAddNewAuthor;
        private TextBox txtNewAuthor;
        private TabPage tabPageAddPublisher;
        private Label label11;
        private Label label10;
        private Label label9;
        private TextBox txtPublisherContactInfo;
        private TextBox txtPublisherAdress;
        private TextBox txtPublisherName;
        private Button buttonAddNewPublisher;
        private Label label12;
        private Label label13;
        private TextBox txtGenre;
        private TabPage tabPageAddGenres;
        private Button button1;
        private Label label14;
        private TextBox txtGenreName;
        private Label label17;
        private Button buttonAddGenres;
    }
}