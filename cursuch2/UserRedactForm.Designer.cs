namespace cursuch2
{
    partial class UserRedactForm
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
            comboBoxRole = new ComboBox();
            comboBoxGroup = new ComboBox();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            txtPassword = new TextBox();
            txtUserName = new TextBox();
            txtPhone = new TextBox();
            txtFullname = new TextBox();
            txtEmail = new TextBox();
            button1 = new Button();
            label8 = new Label();
            label9 = new Label();
            txtAdminPas = new TextBox();
            SuspendLayout();
            // 
            // comboBoxRole
            // 
            comboBoxRole.FormattingEnabled = true;
            comboBoxRole.Location = new Point(1008, 236);
            comboBoxRole.Name = "comboBoxRole";
            comboBoxRole.Size = new Size(263, 45);
            comboBoxRole.TabIndex = 31;
            // 
            // comboBoxGroup
            // 
            comboBoxGroup.FormattingEnabled = true;
            comboBoxGroup.Location = new Point(1008, 185);
            comboBoxGroup.Name = "comboBoxGroup";
            comboBoxGroup.Size = new Size(263, 45);
            comboBoxGroup.TabIndex = 30;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(848, 234);
            label7.Name = "label7";
            label7.Size = new Size(76, 37);
            label7.TabIndex = 29;
            label7.Text = "Роль";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(763, 185);
            label6.Name = "label6";
            label6.Size = new Size(239, 37);
            label6.TabIndex = 28;
            label6.Text = "группа населения";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(848, 136);
            label5.Name = "label5";
            label5.Size = new Size(107, 37);
            label5.TabIndex = 27;
            label5.Text = "пароль";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(122, 283);
            label4.Name = "label4";
            label4.Size = new Size(181, 37);
            label4.TabIndex = 26;
            label4.Text = "имя аккаунта";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(98, 234);
            label3.Name = "label3";
            label3.Size = new Size(223, 37);
            label3.TabIndex = 25;
            label3.Text = "номер телефона";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(156, 185);
            label2.Name = "label2";
            label2.Size = new Size(165, 37);
            label2.TabIndex = 24;
            label2.Text = "полное имя";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(190, 130);
            label1.Name = "label1";
            label1.Size = new Size(82, 37);
            label1.TabIndex = 23;
            label1.Text = "Email";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(1008, 130);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(263, 43);
            txtPassword.TabIndex = 22;
            // 
            // txtUserName
            // 
            txtUserName.Location = new Point(327, 277);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(225, 43);
            txtUserName.TabIndex = 21;
            // 
            // txtPhone
            // 
            txtPhone.Location = new Point(327, 228);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(225, 43);
            txtPhone.TabIndex = 20;
            // 
            // txtFullname
            // 
            txtFullname.Location = new Point(327, 179);
            txtFullname.Name = "txtFullname";
            txtFullname.Size = new Size(225, 43);
            txtFullname.TabIndex = 19;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(327, 130);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(225, 43);
            txtEmail.TabIndex = 18;
            // 
            // button1
            // 
            button1.Location = new Point(592, 450);
            button1.Name = "button1";
            button1.Size = new Size(232, 46);
            button1.TabIndex = 32;
            button1.Text = "обновить";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(466, 45);
            label8.Name = "label8";
            label8.Size = new Size(398, 37);
            label8.TabIndex = 33;
            label8.Text = "редактирование пользователя";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(53, 391);
            label9.Name = "label9";
            label9.Size = new Size(678, 37);
            label9.TabIndex = 34;
            label9.Text = "Введите свой пароль для подтверждения изменений\r\n";
            // 
            // txtAdminPas
            // 
            txtAdminPas.Location = new Point(746, 385);
            txtAdminPas.Name = "txtAdminPas";
            txtAdminPas.Size = new Size(365, 43);
            txtAdminPas.TabIndex = 35;
            // 
            // UserRedactForm
            // 
            AutoScaleDimensions = new SizeF(15F, 37F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1408, 508);
            Controls.Add(txtAdminPas);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(button1);
            Controls.Add(comboBoxRole);
            Controls.Add(comboBoxGroup);
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
            Name = "UserRedactForm";
            Text = "UserRedactForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox comboBoxRole;
        private ComboBox comboBoxGroup;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private TextBox txtPassword;
        private TextBox txtUserName;
        private TextBox txtPhone;
        private TextBox txtFullname;
        private TextBox txtEmail;
        private Button button1;
        private Label label8;
        private Label label9;
        private TextBox txtAdminPas;
    }
}