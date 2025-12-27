namespace AGPS
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxProject = new System.Windows.Forms.ComboBox();
            this.comboBoxPartList = new System.Windows.Forms.ComboBox();
            this.textBoxMadeBy = new System.Windows.Forms.TextBox();
            this.comboBoxTypeOfWork = new System.Windows.Forms.ComboBox();
            this.textBoxComments = new System.Windows.Forms.TextBox();
            this.dateTimePickerDate = new System.Windows.Forms.DateTimePicker();
            this.button1 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxDone = new System.Windows.Forms.TextBox();
            this.comboBoxRemaining = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(16, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(157, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "Project:";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(15, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 30);
            this.label2.TabIndex = 1;
            this.label2.Text = "Part list:";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(15, 204);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(157, 30);
            this.label3.TabIndex = 2;
            this.label3.Text = "Made by:";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(15, 257);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(157, 30);
            this.label4.TabIndex = 3;
            this.label4.Text = "Type of work:";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.Control;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(15, 309);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(157, 30);
            this.label5.TabIndex = 4;
            this.label5.Text = "Date:";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.Control;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(15, 356);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(157, 30);
            this.label6.TabIndex = 5;
            this.label6.Text = "Comments:";
            // 
            // comboBoxProject
            // 
            this.comboBoxProject.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxProject.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxProject.FormattingEnabled = true;
            this.comboBoxProject.Location = new System.Drawing.Point(212, 15);
            this.comboBoxProject.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBoxProject.Name = "comboBoxProject";
            this.comboBoxProject.Size = new System.Drawing.Size(283, 24);
            this.comboBoxProject.TabIndex = 8;
            this.comboBoxProject.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBoxPartList
            // 
            this.comboBoxPartList.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxPartList.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxPartList.FormattingEnabled = true;
            this.comboBoxPartList.Location = new System.Drawing.Point(212, 59);
            this.comboBoxPartList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBoxPartList.Name = "comboBoxPartList";
            this.comboBoxPartList.Size = new System.Drawing.Size(283, 24);
            this.comboBoxPartList.TabIndex = 9;
            // 
            // textBoxMadeBy
            // 
            this.textBoxMadeBy.Location = new System.Drawing.Point(212, 209);
            this.textBoxMadeBy.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxMadeBy.Name = "textBoxMadeBy";
            this.textBoxMadeBy.Size = new System.Drawing.Size(283, 22);
            this.textBoxMadeBy.TabIndex = 10;
            // 
            // comboBoxTypeOfWork
            // 
            this.comboBoxTypeOfWork.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxTypeOfWork.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTypeOfWork.FormattingEnabled = true;
            this.comboBoxTypeOfWork.Items.AddRange(new object[] {
            "Assemble",
            "Assemble control",
            "Weld",
            "Weld control",
            "Saw cut",
            "Plasma cut"});
            this.comboBoxTypeOfWork.Location = new System.Drawing.Point(212, 257);
            this.comboBoxTypeOfWork.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBoxTypeOfWork.Name = "comboBoxTypeOfWork";
            this.comboBoxTypeOfWork.Size = new System.Drawing.Size(283, 24);
            this.comboBoxTypeOfWork.TabIndex = 11;
            // 
            // textBoxComments
            // 
            this.textBoxComments.Location = new System.Drawing.Point(212, 358);
            this.textBoxComments.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxComments.Name = "textBoxComments";
            this.textBoxComments.Size = new System.Drawing.Size(283, 22);
            this.textBoxComments.TabIndex = 12;
            // 
            // dateTimePickerDate
            // 
            this.dateTimePickerDate.Location = new System.Drawing.Point(212, 308);
            this.dateTimePickerDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dateTimePickerDate.Name = "dateTimePickerDate";
            this.dateTimePickerDate.Size = new System.Drawing.Size(283, 22);
            this.dateTimePickerDate.TabIndex = 13;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(212, 404);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(284, 30);
            this.button1.TabIndex = 15;
            this.button1.Text = "Update";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label7.Location = new System.Drawing.Point(16, 111);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 25);
            this.label7.TabIndex = 16;
            this.label7.Text = "Done";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.label8.Location = new System.Drawing.Point(16, 158);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(104, 25);
            this.label8.TabIndex = 17;
            this.label8.Text = "Remaining";
            // 
            // textBoxDone
            // 
            this.textBoxDone.Location = new System.Drawing.Point(212, 111);
            this.textBoxDone.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxDone.Name = "textBoxDone";
            this.textBoxDone.Size = new System.Drawing.Size(283, 22);
            this.textBoxDone.TabIndex = 18;
            // 
            // comboBoxRemaining
            // 
            this.comboBoxRemaining.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBoxRemaining.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxRemaining.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.comboBoxRemaining.FormattingEnabled = true;
            this.comboBoxRemaining.Location = new System.Drawing.Point(212, 160);
            this.comboBoxRemaining.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBoxRemaining.Name = "comboBoxRemaining";
            this.comboBoxRemaining.Size = new System.Drawing.Size(283, 29);
            this.comboBoxRemaining.TabIndex = 19;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(565, 447);
            this.Controls.Add(this.comboBoxRemaining);
            this.Controls.Add(this.textBoxDone);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dateTimePickerDate);
            this.Controls.Add(this.textBoxComments);
            this.Controls.Add(this.comboBoxTypeOfWork);
            this.Controls.Add(this.textBoxMadeBy);
            this.Controls.Add(this.comboBoxPartList);
            this.Controls.Add(this.comboBoxProject);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AGPS(user)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxProject;
        private System.Windows.Forms.ComboBox comboBoxPartList;
        private System.Windows.Forms.TextBox textBoxMadeBy;
        private System.Windows.Forms.ComboBox comboBoxTypeOfWork;
        private System.Windows.Forms.TextBox textBoxComments;
        private System.Windows.Forms.DateTimePicker dateTimePickerDate;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxDone;
        private System.Windows.Forms.ComboBox comboBoxRemaining;
    }
}

