namespace Configurator_2._0
{
    partial class addMachine
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(addMachine));
            this.machineGrid = new System.Windows.Forms.DataGridView();
            this.label7 = new System.Windows.Forms.Label();
            this.DivisionCombo = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.LineCombo = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.TypeCombo = new System.Windows.Forms.ComboBox();
            this.clearButt = new System.Windows.Forms.Button();
            this.quitButt = new System.Windows.Forms.Button();
            this.addMachButt = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.gridButt = new System.Windows.Forms.Button();
            this.modelCombo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.addTypeButt = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.machineGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // machineGrid
            // 
            this.machineGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.machineGrid.Location = new System.Drawing.Point(12, 139);
            this.machineGrid.Name = "machineGrid";
            this.machineGrid.Size = new System.Drawing.Size(1602, 314);
            this.machineGrid.TabIndex = 0;
            this.machineGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.updateRow);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(267, 34);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "Division";
            // 
            // DivisionCombo
            // 
            this.DivisionCombo.FormattingEnabled = true;
            this.DivisionCombo.Location = new System.Drawing.Point(12, 31);
            this.DivisionCombo.Name = "DivisionCombo";
            this.DivisionCombo.Size = new System.Drawing.Size(248, 21);
            this.DivisionCombo.TabIndex = 24;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(267, 88);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 23;
            this.label9.Text = "Machine Line";
            // 
            // LineCombo
            // 
            this.LineCombo.FormattingEnabled = true;
            this.LineCombo.Location = new System.Drawing.Point(12, 85);
            this.LineCombo.Name = "LineCombo";
            this.LineCombo.Size = new System.Drawing.Size(248, 21);
            this.LineCombo.TabIndex = 22;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(267, 61);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(75, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "Machine Type";
            // 
            // TypeCombo
            // 
            this.TypeCombo.FormattingEnabled = true;
            this.TypeCombo.Location = new System.Drawing.Point(12, 58);
            this.TypeCombo.Name = "TypeCombo";
            this.TypeCombo.Size = new System.Drawing.Size(248, 21);
            this.TypeCombo.TabIndex = 20;
            // 
            // clearButt
            // 
            this.clearButt.Location = new System.Drawing.Point(1203, 40);
            this.clearButt.Name = "clearButt";
            this.clearButt.Size = new System.Drawing.Size(75, 23);
            this.clearButt.TabIndex = 31;
            this.clearButt.Text = "Clear";
            this.clearButt.UseVisualStyleBackColor = true;
            // 
            // quitButt
            // 
            this.quitButt.Location = new System.Drawing.Point(1203, 67);
            this.quitButt.Name = "quitButt";
            this.quitButt.Size = new System.Drawing.Size(75, 23);
            this.quitButt.TabIndex = 30;
            this.quitButt.Text = "Quit";
            this.quitButt.UseVisualStyleBackColor = true;
            this.quitButt.Click += new System.EventHandler(this.quitButt_Click);
            // 
            // addMachButt
            // 
            this.addMachButt.Location = new System.Drawing.Point(1203, 15);
            this.addMachButt.Name = "addMachButt";
            this.addMachButt.Size = new System.Drawing.Size(75, 23);
            this.addMachButt.TabIndex = 29;
            this.addMachButt.Text = "Add Machine";
            this.addMachButt.UseVisualStyleBackColor = true;
            this.addMachButt.Click += new System.EventHandler(this.addMachButt_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 13);
            this.label1.TabIndex = 32;
            this.label1.Text = "Select from the options below or type in a new one";
            // 
            // gridButt
            // 
            this.gridButt.Location = new System.Drawing.Point(363, 110);
            this.gridButt.Name = "gridButt";
            this.gridButt.Size = new System.Drawing.Size(92, 23);
            this.gridButt.TabIndex = 33;
            this.gridButt.Text = "Populate Grid";
            this.gridButt.UseVisualStyleBackColor = true;
            this.gridButt.Click += new System.EventHandler(this.gridButt_Click);
            // 
            // modelCombo
            // 
            this.modelCombo.FormattingEnabled = true;
            this.modelCombo.Location = new System.Drawing.Point(12, 112);
            this.modelCombo.Name = "modelCombo";
            this.modelCombo.Size = new System.Drawing.Size(248, 21);
            this.modelCombo.TabIndex = 34;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(267, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 35;
            this.label2.Text = "Machine Model";
            // 
            // addTypeButt
            // 
            this.addTypeButt.Location = new System.Drawing.Point(363, 78);
            this.addTypeButt.Name = "addTypeButt";
            this.addTypeButt.Size = new System.Drawing.Size(92, 23);
            this.addTypeButt.TabIndex = 36;
            this.addTypeButt.Text = "Add New Type";
            this.addTypeButt.UseVisualStyleBackColor = true;
            this.addTypeButt.Click += new System.EventHandler(this.addTypeButt_Click);
            // 
            // addMachine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(1626, 465);
            this.Controls.Add(this.addTypeButt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.modelCombo);
            this.Controls.Add(this.gridButt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.clearButt);
            this.Controls.Add(this.quitButt);
            this.Controls.Add(this.addMachButt);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.DivisionCombo);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.LineCombo);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.TypeCombo);
            this.Controls.Add(this.machineGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "addMachine";
            this.Text = "Add Machine";
            this.Load += new System.EventHandler(this.addMachine_Load);
            ((System.ComponentModel.ISupportInitialize)(this.machineGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView machineGrid;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox DivisionCombo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox LineCombo;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox TypeCombo;
        private System.Windows.Forms.Button clearButt;
        private System.Windows.Forms.Button quitButt;
        private System.Windows.Forms.Button addMachButt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button gridButt;
        private System.Windows.Forms.ComboBox modelCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button addTypeButt;
    }
}