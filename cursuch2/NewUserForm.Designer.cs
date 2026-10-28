namespace cursuch2
{
    partial class NewUserForm
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
            buttonCreateUser = new Button();
            txtEmail = new TextBox();
            txtFullname = new TextBox();
            txtPhone = new TextBox();
            txtUserName = new TextBox();
            txtPassword = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            comboBoxGroup = new ComboBox();
            comboBoxRole = new ComboBox();
            SuspendLayout();
            // 
            // buttonCreateUser
            // 
            buttonCreateUser.Location = new Point(660, 363);
            buttonCreateUser.Name = "buttonCreateUser";
            buttonCreateUser.Size = new Size(169, 52);
            buttonCreateUser.TabIndex = 0;
            buttonCreateUser.Text = "создать";
            buttonCreateUser.UseVisualStyleBackColor = true;
            buttonCreateUser.Click += buttonCreateUser_Click;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(340, 130);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(225, 43);
            txtEmail.TabIndex = 1;
            // 
            // txtFullname
            // 
            txtFullname.Location = new Point(340, 179);
            txtFullname.Name = "txtFullname";
            txtFullname.Size = new Size(225, 43);
            txtFullname.TabIndex = 2;
            // 
            // txtPhone
            // 
            txtPhone.Location = new Point(340, 228);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(225, 43);
            txtPhone.TabIndex = 3;
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(340, 277);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(225, 43);
            txtUserName.TabIndex = 4;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(1021, 130);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(263, 43);
            txtPassword.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(203, 130);
            label1.Name = "label1";
            label1.Size = new Size(82, 37);
            label1.TabIndex = 8;
            label1.Text = "Email";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(169, 185);
            label2.Name = "label2";
            label2.Size = new Size(165, 37);
            label2.TabIndex = 9;
            label2.Text = "полное имя";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(111, 234);
            label3.Name = "label3";
            label3.Size = new Size(223, 37);
            label3.TabIndex = 10;
            label3.Text = "номер телефона";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(135, 283);
            label4.Name = "label4";
            label4.Size = new Size(181, 37);
            label4.TabIndex = 11;
            label4.Text = "имя аккаунта";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(861, 136);
            label5.Name = "label5";
            label5.Size = new Size(107, 37);
            label5.TabIndex = 12;
            label5.Text = "пароль";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(776, 185);
            label6.Name = "label6";
            label6.Size = new Size(239, 37);
            label6.TabIndex = 13;
            label6.Text = "группа населения";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(861, 234);
            label7.Name = "label7";
            label7.Size = new Size(76, 37);
            label7.TabIndex = 14;
            label7.Text = "Роль";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(577, 23);
            label8.Name = "label8";
            label8.Size = new Size(358, 37);
            label8.TabIndex = 15;
            label8.Text = "добавление пользователей";
            // 
            // comboBoxGroup
            // 
            comboBoxGroup.FormattingEnabled = true;
            comboBoxGroup.Location = new Point(1021, 185);
            comboBoxGroup.Name = "comboBoxGroup";
            comboBoxGroup.Size = new Size(263, 45);
            comboBoxGroup.TabIndex = 16;
            // 
            // comboBoxRole
            // 
            comboBoxRole.FormattingEnabled = true;
            comboBoxRole.Location = new Point(1021, 236);
            comboBoxRole.Name = "comboBoxRole";
            comboBoxRole.Size = new Size(263, 45);
            comboBoxRole.TabIndex = 17;
            // 
            // NewUserForm
            // 
            AutoScaleDimensions = new SizeF(15F, 37F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1506, 461);
            Controls.Add(comboBoxRole);
            Controls.Add(comboBoxGroup);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtPassword);
            Controls.Add(txtUserName);
            Controls.Add(txtPhone);
            Controls.Add(txtFullname);
            Controls.Add(txtEmail);
            Controls.Add(buttonCreateUser);
            Name = "NewUserForm";
            Text = "NewUser";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonCreateUser;
        private TextBox txtEmail;
        private TextBox txtFullname;
        private TextBox txtPhone;
        private TextBox txtUserName;
        private TextBox txtPassword;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private ComboBox comboBoxGroup;
        private ComboBox comboBoxRole;
    }
}