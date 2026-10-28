namespace cursuch2
{
    partial class BookRedactForm
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
            label1 = new Label();
            textBox1 = new TextBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            textBox2 = new TextBox();
            comboBoxPublisher = new ComboBox();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            textBoxGenres = new TextBox();
            textBoxAuthors = new TextBox();
            buttonSave = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Monotype Corsiva", 14F, FontStyle.Italic, GraphicsUnit.Point, 204);
            label1.ForeColor = SystemColors.WindowText;
            label1.Location = new Point(830, 31);
            label1.Name = "label1";
            label1.Size = new Size(375, 51);
            label1.TabIndex = 1;
            label1.Text = "Редактирование книги";
            // 
            // textBox1
            // 
            textBox1.BackColor = SystemColors.Window;
            textBox1.Location = new Point(34, 174);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(167, 43);
            textBox1.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(34, 125);
            label2.Name = "label2";
            label2.Size = new Size(133, 37);
            label2.TabIndex = 5;
            label2.Text = "название";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(207, 125);
            label3.Name = "label3";
            label3.Size = new Size(181, 37);
            label3.TabIndex = 6;
            label3.Text = "дата издания";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(394, 125);
            label4.Name = "label4";
            label4.Size = new Size(127, 37);
            label4.TabIndex = 7;
            label4.Text = "издатель";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(674, 125);
            label5.Name = "label5";
            label5.Size = new Size(88, 37);
            label5.TabIndex = 8;
            label5.Text = "автор";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(957, 125);
            label6.Name = "label6";
            label6.Size = new Size(299, 37);
            label6.TabIndex = 9;
            label6.Text = "доступно экземпляров";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(1284, 125);
            label7.Name = "label7";
            label7.Size = new Size(213, 37);
            label7.TabIndex = 10;
            label7.Text = "все экземпляры";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(1589, 125);
            label8.Name = "label8";
            label8.Size = new Size(102, 37);
            label8.TabIndex = 11;
            label8.Text = "жанры";
            // 
            // textBox2
            // 
            textBox2.BackColor = SystemColors.Window;
            textBox2.Location = new Point(207, 174);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(181, 43);
            textBox2.TabIndex = 12;
            // 
            // comboBoxPublisher
            // 
            comboBoxPublisher.BackColor = SystemColors.Window;
            comboBoxPublisher.ForeColor = SystemColors.WindowText;
            comboBoxPublisher.FormattingEnabled = true;
            comboBoxPublisher.Location = new Point(394, 172);
            comboBoxPublisher.Name = "comboBoxPublisher";
            comboBoxPublisher.Size = new Size(274, 45);
            comboBoxPublisher.TabIndex = 13;
            // 
            // textBox3
            // 
            textBox3.BackColor = SystemColors.Window;
            textBox3.Location = new Point(957, 170);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(299, 43);
            textBox3.TabIndex = 14;
            // 
            // textBox4
            // 
            textBox4.BackColor = SystemColors.Window;
            textBox4.Location = new Point(1284, 174);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(299, 43);
            textBox4.TabIndex = 15;
            // 
            // textBoxGenres
            // 
            textBoxGenres.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBoxGenres.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBoxGenres.BackColor = SystemColors.Window;
            textBoxGenres.Location = new Point(1589, 172);
            textBoxGenres.Name = "textBoxGenres";
            textBoxGenres.Size = new Size(272, 43);
            textBoxGenres.TabIndex = 16;
            // 
            // textBoxAuthors
            // 
            textBoxAuthors.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBoxAuthors.AutoCompleteSource = AutoCompleteSource.CustomSource;
            textBoxAuthors.BackColor = SystemColors.Window;
            textBoxAuthors.Location = new Point(674, 170);
            textBoxAuthors.Name = "textBoxAuthors";
            textBoxAuthors.Size = new Size(277, 43);
            textBoxAuthors.TabIndex = 3;
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(915, 323);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(169, 52);
            buttonSave.TabIndex = 17;
            buttonSave.Text = "сохранить";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(1885, 162);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(184, 58);
            buttonCancel.TabIndex = 18;
            buttonCancel.Text = "вернуть всё";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += buttonCancel_Click;
            // 
            // BookRedactForm
            // 
            AutoScaleDimensions = new SizeF(15F, 37F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(2092, 470);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            Controls.Add(textBoxGenres);
            Controls.Add(textBox4);
            Controls.Add(textBox3);
            Controls.Add(comboBoxPublisher);
            Controls.Add(textBox2);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(textBox1);
            Controls.Add(textBoxAuthors);
            Controls.Add(label1);
            Name = "BookRedactForm";
            Text = "BookRedactForm";
            Load += BookRedactForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.ComboBox comboBoxPublisher;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBoxGenres;
        private System.Windows.Forms.TextBox textBoxAuthors;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
    }
}