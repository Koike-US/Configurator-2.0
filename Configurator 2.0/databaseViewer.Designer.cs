namespace Configurator_2._0
{
    partial class databaseViewer
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
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.databaseCombo = new System.Windows.Forms.ComboBox();
            this.databaseGridView = new System.Windows.Forms.DataGridView();
            this.refreshDBsButt = new System.Windows.Forms.Button();
            this.modelListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.databaseGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Machine Models";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Database";
            // 
            // databaseCombo
            // 
            this.databaseCombo.FormattingEnabled = true;
            this.databaseCombo.Location = new System.Drawing.Point(15, 26);
            this.databaseCombo.Name = "databaseCombo";
            this.databaseCombo.Size = new System.Drawing.Size(288, 21);
            this.databaseCombo.TabIndex = 18;
            this.databaseCombo.SelectedIndexChanged += new System.EventHandler(this.databaseCombo_SelectedIndexChanged);
            // 
            // databaseGridView
            // 
            this.databaseGridView.AllowUserToAddRows = false;
            this.databaseGridView.AllowUserToDeleteRows = false;
            this.databaseGridView.AllowUserToOrderColumns = true;
            this.databaseGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.databaseGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.databaseGridView.Location = new System.Drawing.Point(315, 9);
            this.databaseGridView.Name = "databaseGridView";
            this.databaseGridView.ReadOnly = true;
            this.databaseGridView.Size = new System.Drawing.Size(1067, 733);
            this.databaseGridView.TabIndex = 22;
            // 
            // refreshDBsButt
            // 
            this.refreshDBsButt.Location = new System.Drawing.Point(15, 243);
            this.refreshDBsButt.Name = "refreshDBsButt";
            this.refreshDBsButt.Size = new System.Drawing.Size(75, 23);
            this.refreshDBsButt.TabIndex = 23;
            this.refreshDBsButt.Text = "Refresh DBs";
            this.refreshDBsButt.UseVisualStyleBackColor = true;
            this.refreshDBsButt.Click += new System.EventHandler(this.refreshDBsButt_Click);
            // 
            // modelListView
            // 
            this.modelListView.CheckBoxes = true;
            this.modelListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.modelListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.modelListView.HideSelection = false;
            this.modelListView.Location = new System.Drawing.Point(15, 70);
            this.modelListView.Name = "modelListView";
            this.modelListView.Size = new System.Drawing.Size(285, 167);
            this.modelListView.TabIndex = 24;
            this.modelListView.UseCompatibleStateImageBehavior = false;
            this.modelListView.View = System.Windows.Forms.View.Details;
            this.modelListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.modelListView_ItemChecked);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 280;
            // 
            // databaseViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1394, 757);
            this.Controls.Add(this.modelListView);
            this.Controls.Add(this.refreshDBsButt);
            this.Controls.Add(this.databaseGridView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.databaseCombo);
            this.Controls.Add(this.label4);
            this.Name = "databaseViewer";
            this.Text = "Configurator Database Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.databaseGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox databaseCombo;
        private System.Windows.Forms.DataGridView databaseGridView;
        private System.Windows.Forms.Button refreshDBsButt;
        private System.Windows.Forms.ListView modelListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}