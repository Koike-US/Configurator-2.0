namespace Configurator_2._0
{
    partial class addEditOptionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(addEditOptionForm));
            this.optionGridView = new System.Windows.Forms.DataGridView();
            this.Type = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.OptionName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SmartDesignator = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OptionChecklist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OptionReqs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShortDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QtySelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.MaxQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QtyStep = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Order = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.setMachinesButt = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.modelListView = new System.Windows.Forms.ListView();
            this.lineCombo = new System.Windows.Forms.ComboBox();
            this.addRowButt = new System.Windows.Forms.Button();
            this.commitEditButt = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.optionGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // optionGridView
            // 
            this.optionGridView.AllowUserToAddRows = false;
            this.optionGridView.AllowUserToDeleteRows = false;
            this.optionGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.optionGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.optionGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Type,
            this.OptionName,
            this.SmartDesignator,
            this.OptionChecklist,
            this.OptionReqs,
            this.ShortDescription,
            this.QtySelect,
            this.MaxQty,
            this.QtyStep,
            this.Order});
            this.optionGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.optionGridView.Enabled = false;
            this.optionGridView.Location = new System.Drawing.Point(261, 12);
            this.optionGridView.Name = "optionGridView";
            this.optionGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.optionGridView.Size = new System.Drawing.Size(1137, 463);
            this.optionGridView.TabIndex = 4;
            // 
            // Type
            // 
            this.Type.HeaderText = "Type";
            this.Type.Name = "Type";
            this.Type.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Type.ToolTipText = "Option Type Group";
            this.Type.Width = 150;
            // 
            // OptionName
            // 
            this.OptionName.HeaderText = "Name";
            this.OptionName.Name = "OptionName";
            this.OptionName.ToolTipText = "Option Name";
            // 
            // SmartDesignator
            // 
            this.SmartDesignator.HeaderText = "Smart Designator";
            this.SmartDesignator.Name = "SmartDesignator";
            this.SmartDesignator.ToolTipText = "Unique, short string that will identify this option. Can be reused for multiple m" +
    "achines and machine types with different part numbers";
            // 
            // OptionChecklist
            // 
            this.OptionChecklist.HeaderText = "Option Checklist";
            this.OptionChecklist.Name = "OptionChecklist";
            this.OptionChecklist.ToolTipText = "Configured Checklist filename, if it exists. Ex; SPCL PLASMA for the ShopPro Plas" +
    "ma checklist";
            // 
            // OptionReqs
            // 
            this.OptionReqs.HeaderText = "Option Requirements";
            this.OptionReqs.Name = "OptionReqs";
            this.OptionReqs.ToolTipText = resources.GetString("OptionReqs.ToolTipText");
            // 
            // ShortDescription
            // 
            this.ShortDescription.HeaderText = "Short Description";
            this.ShortDescription.Name = "ShortDescription";
            this.ShortDescription.ToolTipText = "A short description of this option. Ex; 480V MAXPRO200 WITH LEADS";
            this.ShortDescription.Width = 150;
            // 
            // QtySelect
            // 
            this.QtySelect.HeaderText = "Qty Select";
            this.QtySelect.Name = "QtySelect";
            this.QtySelect.Visible = false;
            // 
            // MaxQty
            // 
            this.MaxQty.HeaderText = "Max Qty";
            this.MaxQty.Name = "MaxQty";
            this.MaxQty.ToolTipText = "Maximmum quantity of this option that can be added";
            this.MaxQty.Visible = false;
            // 
            // QtyStep
            // 
            this.QtyStep.HeaderText = "Qty Step";
            this.QtyStep.Name = "QtyStep";
            this.QtyStep.Visible = false;
            // 
            // Order
            // 
            this.Order.HeaderText = "Order";
            this.Order.Name = "Order";
            this.Order.ReadOnly = true;
            this.Order.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.setMachinesButt);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.modelListView);
            this.groupBox1.Controls.Add(this.lineCombo);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(243, 316);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add Option(s)";
            // 
            // setMachinesButt
            // 
            this.setMachinesButt.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.setMachinesButt.Location = new System.Drawing.Point(9, 245);
            this.setMachinesButt.Name = "setMachinesButt";
            this.setMachinesButt.Size = new System.Drawing.Size(75, 23);
            this.setMachinesButt.TabIndex = 12;
            this.setMachinesButt.Text = "Set Machines";
            this.setMachinesButt.UseVisualStyleBackColor = true;
            this.setMachinesButt.Click += new System.EventHandler(this.setMachinesButt_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Select Models";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Select Machine Line";
            // 
            // modelListView
            // 
            this.modelListView.CheckBoxes = true;
            this.modelListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.modelListView.HideSelection = false;
            this.modelListView.Location = new System.Drawing.Point(9, 72);
            this.modelListView.Name = "modelListView";
            this.modelListView.Size = new System.Drawing.Size(221, 167);
            this.modelListView.TabIndex = 8;
            this.modelListView.UseCompatibleStateImageBehavior = false;
            this.modelListView.View = System.Windows.Forms.View.List;
            // 
            // lineCombo
            // 
            this.lineCombo.FormattingEnabled = true;
            this.lineCombo.Location = new System.Drawing.Point(6, 32);
            this.lineCombo.Name = "lineCombo";
            this.lineCombo.Size = new System.Drawing.Size(224, 21);
            this.lineCombo.TabIndex = 7;
            this.lineCombo.SelectedIndexChanged += new System.EventHandler(this.lineCombo_SelectedIndexChanged);
            // 
            // addRowButt
            // 
            this.addRowButt.Location = new System.Drawing.Point(180, 452);
            this.addRowButt.Name = "addRowButt";
            this.addRowButt.Size = new System.Drawing.Size(75, 23);
            this.addRowButt.TabIndex = 9;
            this.addRowButt.Text = "Add Row";
            this.addRowButt.UseVisualStyleBackColor = true;
            this.addRowButt.Click += new System.EventHandler(this.addRowButt_Click);
            // 
            // commitEditButt
            // 
            this.commitEditButt.Location = new System.Drawing.Point(12, 452);
            this.commitEditButt.Name = "commitEditButt";
            this.commitEditButt.Size = new System.Drawing.Size(75, 23);
            this.commitEditButt.TabIndex = 8;
            this.commitEditButt.Text = "Commit Edit";
            this.commitEditButt.UseVisualStyleBackColor = true;
            this.commitEditButt.Click += new System.EventHandler(this.commitEditButt_Click);
            // 
            // addEditOptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1410, 487);
            this.Controls.Add(this.commitEditButt);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.optionGridView);
            this.Controls.Add(this.addRowButt);
            this.Name = "addEditOptionForm";
            this.Text = "Add/Edit Options";
            ((System.ComponentModel.ISupportInitialize)(this.optionGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView optionGridView;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button addRowButt;
        private System.Windows.Forms.ListView modelListView;
        private System.Windows.Forms.ComboBox lineCombo;
        private System.Windows.Forms.Button setMachinesButt;
        private System.Windows.Forms.Button commitEditButt;
        private System.Windows.Forms.DataGridViewComboBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn OptionName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SmartDesignator;
        private System.Windows.Forms.DataGridViewTextBoxColumn OptionChecklist;
        private System.Windows.Forms.DataGridViewTextBoxColumn OptionReqs;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShortDescription;
        private System.Windows.Forms.DataGridViewCheckBoxColumn QtySelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn QtyStep;
        private System.Windows.Forms.DataGridViewTextBoxColumn Order;
    }
}