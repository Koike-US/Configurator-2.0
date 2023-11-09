namespace Configurator_2._0
{
    partial class newOptionGroupForm
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
            this.label3 = new System.Windows.Forms.Label();
            this.groupNameBox = new System.Windows.Forms.TextBox();
            this.createGroupBox = new System.Windows.Forms.Button();
            this.comboBoxRadio = new System.Windows.Forms.RadioButton();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxRadio = new System.Windows.Forms.RadioButton();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.optionGroupsBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(564, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Group Name (NO Spaces):";
            // 
            // groupNameBox
            // 
            this.groupNameBox.Location = new System.Drawing.Point(567, 52);
            this.groupNameBox.Name = "groupNameBox";
            this.groupNameBox.Size = new System.Drawing.Size(139, 20);
            this.groupNameBox.TabIndex = 3;
            // 
            // createGroupBox
            // 
            this.createGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.createGroupBox.Location = new System.Drawing.Point(567, 144);
            this.createGroupBox.Name = "createGroupBox";
            this.createGroupBox.Size = new System.Drawing.Size(75, 23);
            this.createGroupBox.TabIndex = 4;
            this.createGroupBox.Text = "Create Group";
            this.createGroupBox.UseVisualStyleBackColor = true;
            this.createGroupBox.Click += new System.EventHandler(this.createGroupBox_Click);
            // 
            // comboBoxRadio
            // 
            this.comboBoxRadio.AutoSize = true;
            this.comboBoxRadio.Checked = true;
            this.comboBoxRadio.Location = new System.Drawing.Point(15, 10);
            this.comboBoxRadio.Name = "comboBoxRadio";
            this.comboBoxRadio.Size = new System.Drawing.Size(14, 13);
            this.comboBoxRadio.TabIndex = 7;
            this.comboBoxRadio.TabStop = true;
            this.comboBoxRadio.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Option 1",
            "Option 2",
            "Option 3",
            "Option 4"});
            this.comboBox1.Location = new System.Drawing.Point(15, 68);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(247, 21);
            this.comboBox1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(252, 26);
            this.label1.TabIndex = 5;
            this.label1.Text = "A Combo Box, or Drop Down;\r\nAllows for a singular option selection from this grou" +
    "p.";
            // 
            // listBoxRadio
            // 
            this.listBoxRadio.AutoSize = true;
            this.listBoxRadio.Location = new System.Drawing.Point(310, 10);
            this.listBoxRadio.Name = "listBoxRadio";
            this.listBoxRadio.Size = new System.Drawing.Size(14, 13);
            this.listBoxRadio.TabIndex = 10;
            this.listBoxRadio.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "Option 1",
            "Option 2",
            "Option 3",
            "Option 4"});
            this.listBox1.Location = new System.Drawing.Point(310, 68);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(209, 95);
            this.listBox1.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(307, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(212, 26);
            this.label2.TabIndex = 8;
            this.label2.Text = "A List Box;\r\nAllows for multiple selections from this group\r\n";
            // 
            // optionGroupsBox
            // 
            this.optionGroupsBox.FormattingEnabled = true;
            this.optionGroupsBox.Location = new System.Drawing.Point(567, 117);
            this.optionGroupsBox.Name = "optionGroupsBox";
            this.optionGroupsBox.Size = new System.Drawing.Size(139, 21);
            this.optionGroupsBox.TabIndex = 11;
            this.optionGroupsBox.SelectedIndexChanged += new System.EventHandler(this.optionGroupsBox_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(564, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Insert Group After:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(564, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(199, 26);
            this.label5.TabIndex = 13;
            this.label5.Text = "The Option Group name must be unique,\r\n and contain no spaces";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(564, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(191, 26);
            this.label6.TabIndex = 14;
            this.label6.Text = "The Option Groups position is critical \r\nfor determining any option requirements";
            // 
            // newOptionGroupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(784, 170);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.optionGroupsBox);
            this.Controls.Add(this.listBoxRadio);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBoxRadio);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.createGroupBox);
            this.Controls.Add(this.groupNameBox);
            this.Controls.Add(this.label3);
            this.Name = "newOptionGroupForm";
            this.Text = "New option Group";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox groupNameBox;
        private System.Windows.Forms.Button createGroupBox;
        private System.Windows.Forms.RadioButton comboBoxRadio;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton listBoxRadio;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox optionGroupsBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}