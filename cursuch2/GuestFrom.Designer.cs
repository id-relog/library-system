using System.Windows.Forms;

namespace cursuch2
{
    partial class GuestFrom
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            textBox1 = new TextBox();
            label1 = new Label();
            buttonSearch = new Button();
            dataGridView1 = new DataGridView();
            tableLayoutPanel14 = new TableLayoutPanel();
            linkLabel1 = new LinkLabel();
            linkLabel2 = new LinkLabel();
            tabPage2 = new TabPage();
            tableLayoutPanel3 = new TableLayoutPanel();
            label2 = new Label();
            tableLayoutPanel4 = new TableLayoutPanel();
            label3 = new Label();
            txtFullName = new TextBox();
            tableLayoutPanel5 = new TableLayoutPanel();
            label4 = new Label();
            txtEmail = new TextBox();
            tableLayoutPanel6 = new TableLayoutPanel();
            label5 = new Label();
            txtPhone = new TextBox();
            tableLayoutPanel7 = new TableLayoutPanel();
            label6 = new Label();
            txtUsername = new TextBox();
            tableLayoutPanel8 = new TableLayoutPanel();
            label7 = new Label();
            txtPassword = new TextBox();
            tableLayoutPanel9 = new TableLayoutPanel();
            label8 = new Label();
            txtConfirmPassword = new TextBox();
            tableLayoutPanel10 = new TableLayoutPanel();
            label9 = new Label();
            cmbGroup = new ComboBox();
            btnRegister = new Button();
            tabPage3 = new TabPage();
            tableLayoutPanel11 = new TableLayoutPanel();
            label10 = new Label();
            tableLayoutPanel12 = new TableLayoutPanel();
            label11 = new Label();
            loginTxt = new TextBox();
            tableLayoutPanel13 = new TableLayoutPanel();
            passwordTxt = new TextBox();
            label12 = new Label();
            loginButton = new Button();
            sqlCommand1 = new Microsoft.Data.SqlClient.SqlCommand();
            sqlCommand2 = new Microsoft.Data.SqlClient.SqlCommand();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            tableLayoutPanel14.SuspendLayout();
            tabPage2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            tableLayoutPanel8.SuspendLayout();
            tableLayoutPanel9.SuspendLayout();
            tableLayoutPanel10.SuspendLayout();
            tabPage3.SuspendLayout();
            tableLayoutPanel11.SuspendLayout();
            tableLayoutPanel12.SuspendLayout();
            tableLayoutPanel13.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1685, 950);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(tableLayoutPanel1);
            tabPage1.Location = new Point(10, 55);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1665, 885);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Окно поиска книг";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.Transparent;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(dataGridView1, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel14, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            tableLayoutPanel1.Size = new Size(1659, 879);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 381F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 288F));
            tableLayoutPanel2.Controls.Add(textBox1, 1, 0);
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Controls.Add(buttonSearch, 2, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(1653, 52);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // textBox1
            // 
            textBox1.ForeColor = SystemColors.InactiveCaptionText;
            textBox1.Location = new Point(384, 3);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Введите пустое поля для просмотра всех книг";
            textBox1.Size = new Size(976, 43);
            textBox1.TabIndex = 3;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 6);
            label1.Margin = new Padding(3, 6, 3, 0);
            label1.Name = "label1";
            label1.Size = new Size(342, 37);
            label1.TabIndex = 0;
            label1.Text = "Введите автора или книгу:";
            // 
            // buttonSearch
            // 
            buttonSearch.Location = new Point(1368, 3);
            buttonSearch.Name = "buttonSearch";
            buttonSearch.Size = new Size(282, 44);
            buttonSearch.TabIndex = 1;
            buttonSearch.Text = "Искать";
            buttonSearch.UseVisualStyleBackColor = true;
            buttonSearch.Click += buttonSearch_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BackgroundColor = SystemColors.ControlLightLight;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Enabled = false;
            dataGridView1.ImeMode = ImeMode.NoControl;
            dataGridView1.Location = new Point(3, 61);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidth = 82;
            dataGridView1.Size = new Size(1653, 757);
            dataGridView1.TabIndex = 1;
            // 
            // tableLayoutPanel14
            // 
            tableLayoutPanel14.ColumnCount = 4;
            tableLayoutPanel14.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel14.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel14.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel14.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel14.Controls.Add(linkLabel1, 1, 0);
            tableLayoutPanel14.Controls.Add(linkLabel2, 2, 0);
            tableLayoutPanel14.Dock = DockStyle.Fill;
            tableLayoutPanel14.Location = new Point(3, 824);
            tableLayoutPanel14.Name = "tableLayoutPanel14";
            tableLayoutPanel14.RowCount = 1;
            tableLayoutPanel14.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel14.Size = new Size(1653, 52);
            tableLayoutPanel14.TabIndex = 2;
            // 
            // linkLabel1
            // 
            linkLabel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            linkLabel1.AutoSize = true;
            linkLabel1.LinkColor = SystemColors.HotTrack;
            linkLabel1.Location = new Point(552, 9);
            linkLabel1.Margin = new Padding(0, 0, 6, 6);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(268, 37);
            linkLabel1.TabIndex = 0;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Зарегистрироваться";
            linkLabel1.VisitedLinkColor = Color.Purple;
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // linkLabel2
            // 
            linkLabel2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            linkLabel2.AutoSize = true;
            linkLabel2.LinkColor = SystemColors.HotTrack;
            linkLabel2.Location = new Point(832, 9);
            linkLabel2.Margin = new Padding(6, 0, 0, 6);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(91, 37);
            linkLabel2.TabIndex = 1;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "Войти";
            linkLabel2.VisitedLinkColor = Color.Purple;
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(tableLayoutPanel3);
            tabPage2.Location = new Point(10, 55);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1665, 885);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Окно регистрации";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17.1070938F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58.83171F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24F));
            tableLayoutPanel3.Controls.Add(label2, 1, 0);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel4, 1, 1);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel5, 1, 2);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel6, 1, 3);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel7, 1, 4);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel8, 1, 5);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel9, 1, 6);
            tableLayoutPanel3.Controls.Add(tableLayoutPanel10, 1, 7);
            tableLayoutPanel3.Controls.Add(btnRegister, 1, 8);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 9;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 6.67000055F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 6.67000055F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 6.67000055F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 6.67000055F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 6.67000055F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 6.67000055F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 6.67000055F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 6.67000055F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 46.6400032F));
            tableLayoutPanel3.Size = new Size(1659, 879);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.AutoSize = true;
            label2.Location = new Point(555, 12);
            label2.Margin = new Padding(0, 12, 0, 0);
            label2.Name = "label2";
            label2.Size = new Size(431, 37);
            label2.TabIndex = 0;
            label2.Text = "Введите данные для регистрации";
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.63519F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66.3648148F));
            tableLayoutPanel4.Controls.Add(label3, 0, 0);
            tableLayoutPanel4.Controls.Add(txtFullName, 1, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(286, 61);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 51F));
            tableLayoutPanel4.Size = new Size(970, 52);
            tableLayoutPanel4.TabIndex = 1;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(246, 7);
            label3.Name = "label3";
            label3.Size = new Size(77, 37);
            label3.TabIndex = 0;
            label3.Text = "ФИО";
            // 
            // txtFullName
            // 
            txtFullName.Dock = DockStyle.Fill;
            txtFullName.Location = new Point(329, 3);
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(638, 43);
            txtFullName.TabIndex = 1;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.8095245F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66.1904755F));
            tableLayoutPanel5.Controls.Add(label4, 0, 0);
            tableLayoutPanel5.Controls.Add(txtEmail, 1, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(286, 119);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Size = new Size(970, 52);
            tableLayoutPanel5.TabIndex = 2;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(242, 7);
            label4.Name = "label4";
            label4.Size = new Size(82, 37);
            label4.TabIndex = 0;
            label4.Text = "Email";
            // 
            // txtEmail
            // 
            txtEmail.Dock = DockStyle.Fill;
            txtEmail.Location = new Point(330, 3);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(637, 43);
            txtEmail.TabIndex = 1;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 2;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34.04762F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65.95238F));
            tableLayoutPanel6.Controls.Add(label5, 0, 0);
            tableLayoutPanel6.Controls.Add(txtPhone, 1, 0);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(286, 177);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 1;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.Size = new Size(970, 52);
            tableLayoutPanel6.TabIndex = 3;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new Point(203, 7);
            label5.Name = "label5";
            label5.Size = new Size(124, 37);
            label5.TabIndex = 0;
            label5.Text = "Телефон";
            // 
            // txtPhone
            // 
            txtPhone.Dock = DockStyle.Fill;
            txtPhone.Location = new Point(333, 3);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(634, 43);
            txtPhone.TabIndex = 1;
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.ColumnCount = 2;
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34.04762F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65.95238F));
            tableLayoutPanel7.Controls.Add(label6, 0, 0);
            tableLayoutPanel7.Controls.Add(txtUsername, 1, 0);
            tableLayoutPanel7.Dock = DockStyle.Fill;
            tableLayoutPanel7.Location = new Point(286, 235);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 1;
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.Size = new Size(970, 52);
            tableLayoutPanel7.TabIndex = 4;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new Point(234, 7);
            label6.Name = "label6";
            label6.Size = new Size(93, 37);
            label6.TabIndex = 0;
            label6.Text = "Логин";
            // 
            // txtUsername
            // 
            txtUsername.Dock = DockStyle.Fill;
            txtUsername.Location = new Point(333, 3);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(634, 43);
            txtUsername.TabIndex = 1;
            // 
            // tableLayoutPanel8
            // 
            tableLayoutPanel8.ColumnCount = 2;
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34.04762F));
            tableLayoutPanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65.95238F));
            tableLayoutPanel8.Controls.Add(label7, 0, 0);
            tableLayoutPanel8.Controls.Add(txtPassword, 1, 0);
            tableLayoutPanel8.Dock = DockStyle.Fill;
            tableLayoutPanel8.Location = new Point(286, 293);
            tableLayoutPanel8.Name = "tableLayoutPanel8";
            tableLayoutPanel8.RowCount = 1;
            tableLayoutPanel8.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel8.Size = new Size(970, 52);
            tableLayoutPanel8.TabIndex = 5;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Right;
            label7.AutoSize = true;
            label7.Location = new Point(217, 7);
            label7.Name = "label7";
            label7.Size = new Size(110, 37);
            label7.TabIndex = 0;
            label7.Text = "Пароль";
            // 
            // txtPassword
            // 
            txtPassword.Dock = DockStyle.Fill;
            txtPassword.Location = new Point(333, 3);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(634, 43);
            txtPassword.TabIndex = 1;
            // 
            // tableLayoutPanel9
            // 
            tableLayoutPanel9.ColumnCount = 2;
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34.1666679F));
            tableLayoutPanel9.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65.8333359F));
            tableLayoutPanel9.Controls.Add(label8, 0, 0);
            tableLayoutPanel9.Controls.Add(txtConfirmPassword, 1, 0);
            tableLayoutPanel9.Dock = DockStyle.Fill;
            tableLayoutPanel9.Location = new Point(286, 351);
            tableLayoutPanel9.Name = "tableLayoutPanel9";
            tableLayoutPanel9.RowCount = 1;
            tableLayoutPanel9.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel9.Size = new Size(970, 52);
            tableLayoutPanel9.TabIndex = 6;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Location = new Point(14, 7);
            label8.Name = "label8";
            label8.Size = new Size(314, 37);
            label8.TabIndex = 0;
            label8.Text = "Подтверждение пароля";
            // 
            // txtConfirmPassword
            // 
            txtConfirmPassword.Dock = DockStyle.Fill;
            txtConfirmPassword.Location = new Point(334, 3);
            txtConfirmPassword.Name = "txtConfirmPassword";
            txtConfirmPassword.Size = new Size(633, 43);
            txtConfirmPassword.TabIndex = 1;
            // 
            // tableLayoutPanel10
            // 
            tableLayoutPanel10.ColumnCount = 2;
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34.4047623F));
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65.59524F));
            tableLayoutPanel10.Controls.Add(label9, 0, 0);
            tableLayoutPanel10.Controls.Add(cmbGroup, 1, 0);
            tableLayoutPanel10.Dock = DockStyle.Fill;
            tableLayoutPanel10.Location = new Point(286, 409);
            tableLayoutPanel10.Name = "tableLayoutPanel10";
            tableLayoutPanel10.RowCount = 1;
            tableLayoutPanel10.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel10.Size = new Size(970, 52);
            tableLayoutPanel10.TabIndex = 7;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Right;
            label9.AutoSize = true;
            label9.Location = new Point(225, 7);
            label9.Name = "label9";
            label9.Size = new Size(105, 37);
            label9.TabIndex = 0;
            label9.Text = "Группа";
            // 
            // cmbGroup
            // 
            cmbGroup.Dock = DockStyle.Fill;
            cmbGroup.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGroup.FormattingEnabled = true;
            cmbGroup.Location = new Point(336, 3);
            cmbGroup.Name = "cmbGroup";
            cmbGroup.Size = new Size(631, 45);
            cmbGroup.TabIndex = 1;
            // 
            // btnRegister
            // 
            btnRegister.Anchor = AnchorStyles.None;
            btnRegister.Location = new Point(628, 645);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(286, 52);
            btnRegister.TabIndex = 8;
            btnRegister.Text = "Зарегистрироваться";
            btnRegister.UseVisualStyleBackColor = true;
            btnRegister.Click += btnRegister_Click;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(tableLayoutPanel11);
            tabPage3.Location = new Point(10, 55);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(1665, 885);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Окно входа";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel11
            // 
            tableLayoutPanel11.ColumnCount = 3;
            tableLayoutPanel11.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24.6870651F));
            tableLayoutPanel11.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 41.93324F));
            tableLayoutPanel11.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel11.Controls.Add(label10, 1, 0);
            tableLayoutPanel11.Controls.Add(tableLayoutPanel12, 1, 1);
            tableLayoutPanel11.Controls.Add(tableLayoutPanel13, 1, 2);
            tableLayoutPanel11.Controls.Add(loginButton, 1, 3);
            tableLayoutPanel11.Dock = DockStyle.Fill;
            tableLayoutPanel11.Location = new Point(3, 3);
            tableLayoutPanel11.Name = "tableLayoutPanel11";
            tableLayoutPanel11.RowCount = 4;
            tableLayoutPanel11.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel11.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel11.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel11.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel11.Size = new Size(1659, 879);
            tableLayoutPanel11.TabIndex = 0;
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.None;
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 10F);
            label10.Location = new Point(644, 89);
            label10.Name = "label10";
            label10.Size = new Size(224, 41);
            label10.TabIndex = 0;
            label10.Text = "Вход в систему";
            // 
            // tableLayoutPanel12
            // 
            tableLayoutPanel12.ColumnCount = 2;
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38.1909561F));
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 61.8090439F));
            tableLayoutPanel12.Controls.Add(label11, 0, 0);
            tableLayoutPanel12.Controls.Add(loginTxt, 1, 0);
            tableLayoutPanel12.Dock = DockStyle.Fill;
            tableLayoutPanel12.Location = new Point(412, 222);
            tableLayoutPanel12.Name = "tableLayoutPanel12";
            tableLayoutPanel12.RowCount = 1;
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel12.Size = new Size(689, 213);
            tableLayoutPanel12.TabIndex = 1;
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label11.AutoSize = true;
            label11.Location = new Point(56, 170);
            label11.Margin = new Padding(0, 0, 6, 6);
            label11.Name = "label11";
            label11.Size = new Size(201, 37);
            label11.TabIndex = 0;
            label11.Text = "Введите логин:";
            // 
            // loginTxt
            // 
            loginTxt.Anchor = AnchorStyles.Bottom;
            loginTxt.Location = new Point(267, 167);
            loginTxt.Name = "loginTxt";
            loginTxt.Size = new Size(418, 43);
            loginTxt.TabIndex = 1;
            // 
            // tableLayoutPanel13
            // 
            tableLayoutPanel13.Anchor = AnchorStyles.Left;
            tableLayoutPanel13.ColumnCount = 2;
            tableLayoutPanel13.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38.1909561F));
            tableLayoutPanel13.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 61.8090439F));
            tableLayoutPanel13.Controls.Add(passwordTxt, 1, 0);
            tableLayoutPanel13.Controls.Add(label12, 0, 0);
            tableLayoutPanel13.Location = new Point(412, 441);
            tableLayoutPanel13.Name = "tableLayoutPanel13";
            tableLayoutPanel13.RowCount = 1;
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel13.Size = new Size(689, 213);
            tableLayoutPanel13.TabIndex = 2;
            // 
            // passwordTxt
            // 
            passwordTxt.Anchor = AnchorStyles.Top;
            passwordTxt.Location = new Point(267, 3);
            passwordTxt.Name = "passwordTxt";
            passwordTxt.Size = new Size(418, 43);
            passwordTxt.TabIndex = 1;
            // 
            // label12
            // 
            label12.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label12.AutoSize = true;
            label12.Location = new Point(41, 3);
            label12.Margin = new Padding(3);
            label12.Name = "label12";
            label12.Size = new Size(219, 37);
            label12.TabIndex = 0;
            label12.Text = "Введите пароль:";
            // 
            // loginButton
            // 
            loginButton.Anchor = AnchorStyles.None;
            loginButton.Location = new Point(612, 745);
            loginButton.Name = "loginButton";
            loginButton.Size = new Size(288, 46);
            loginButton.TabIndex = 3;
            loginButton.Text = "Войти";
            loginButton.UseVisualStyleBackColor = true;
            loginButton.Click += loginButton_Click;
            // 
            // sqlCommand1
            // 
            sqlCommand1.CommandTimeout = 30;
            sqlCommand1.EnableOptimizedParameterBinding = false;
            // 
            // sqlCommand2
            // 
            sqlCommand2.CommandTimeout = 30;
            sqlCommand2.EnableOptimizedParameterBinding = false;
            // 
            // GuestFrom
            // 
            AutoScaleDimensions = new SizeF(15F, 37F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1685, 950);
            Controls.Add(tabControl1);
            Name = "GuestFrom";
            Text = "Гостевое окно";
            Load += GuestFrom_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            tableLayoutPanel14.ResumeLayout(false);
            tableLayoutPanel14.PerformLayout();
            tabPage2.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            tableLayoutPanel7.ResumeLayout(false);
            tableLayoutPanel7.PerformLayout();
            tableLayoutPanel8.ResumeLayout(false);
            tableLayoutPanel8.PerformLayout();
            tableLayoutPanel9.ResumeLayout(false);
            tableLayoutPanel9.PerformLayout();
            tableLayoutPanel10.ResumeLayout(false);
            tableLayoutPanel10.PerformLayout();
            tabPage3.ResumeLayout(false);
            tableLayoutPanel11.ResumeLayout(false);
            tableLayoutPanel11.PerformLayout();
            tableLayoutPanel12.ResumeLayout(false);
            tableLayoutPanel12.PerformLayout();
            tableLayoutPanel13.ResumeLayout(false);
            tableLayoutPanel13.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TableLayoutPanel tableLayoutPanel3;
        private Label label2;
        private TableLayoutPanel tableLayoutPanel4;
        private Label label3;
        private TableLayoutPanel tableLayoutPanel5;
        private Label label4;
        private TableLayoutPanel tableLayoutPanel6;
        private Label label5;
        private TableLayoutPanel tableLayoutPanel7;
        private Label label6;
        private TableLayoutPanel tableLayoutPanel8;
        private Label label7;
        private TableLayoutPanel tableLayoutPanel9;
        private TableLayoutPanel tableLayoutPanel10;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Label label8;
        private TextBox txtConfirmPassword;
        private Label label9;
        private ComboBox cmbGroup;
        private Button btnRegister;
        private TabPage tabPage3;
        private Microsoft.Data.SqlClient.SqlCommand sqlCommand1;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TextBox textBox1;
        private Label label1;
        private Button buttonSearch;
        private DataGridView dataGridView1;
        private TableLayoutPanel tableLayoutPanel14;
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel2;
        private Microsoft.Data.SqlClient.SqlCommand sqlCommand2;
        private TableLayoutPanel tableLayoutPanel11;
        private Label label10;
        private TableLayoutPanel tableLayoutPanel12;
        private Label label11;
        private TextBox loginTxt;
        private TableLayoutPanel tableLayoutPanel13;
        private TextBox passwordTxt;
        private Label label12;
        private Button loginButton;
    }
}
