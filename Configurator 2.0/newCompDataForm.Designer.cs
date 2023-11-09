namespace Configurator_2._0
{
    partial class newCompDataForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(newCompDataForm));
            this.compDataGridView = new System.Windows.Forms.DataGridView();
            this.partNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lineItem = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.qtySelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.maxQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qtyStep = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.okButt = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.compDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // compDataGridView
            // 
            this.compDataGridView.AllowUserToAddRows = false;
            this.compDataGridView.AllowUserToDeleteRows = false;
            this.compDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.compDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.compDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.partNumber,
            this.lineItem,
            this.qtySelect,
            this.maxQty,
            this.qtyStep});
            this.compDataGridView.Location = new System.Drawing.Point(323, 12);
            this.compDataGridView.Name = "compDataGridView";
            this.compDataGridView.Size = new System.Drawing.Size(547, 426);
            this.compDataGridView.TabIndex = 0;
            // 
            // partNumber
            // 
            this.partNumber.HeaderText = "Part Number";
            this.partNumber.Name = "partNumber";
            this.partNumber.ReadOnly = true;
            // 
            // lineItem
            // 
            this.lineItem.HeaderText = "Add as Line Item";
            this.lineItem.Name = "lineItem";
            // 
            // qtySelect
            // 
            this.qtySelect.HeaderText = "Quantity Selection";
            this.qtySelect.Name = "qtySelect";
            // 
            // maxQty
            // 
            this.maxQty.HeaderText = "Max Qty";
            this.maxQty.Name = "maxQty";
            // 
            // qtyStep
            // 
            this.qtyStep.HeaderText = "Qty Step";
            this.qtyStep.Name = "qtyStep";
            // 
            // okButt
            // 
            this.okButt.Location = new System.Drawing.Point(12, 12);
            this.okButt.Name = "okButt";
            this.okButt.Size = new System.Drawing.Size(75, 23);
            this.okButt.TabIndex = 1;
            this.okButt.Text = "Ok";
            this.okButt.UseVisualStyleBackColor = true;
            this.okButt.Click += new System.EventHandler(this.okButt_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(305, 130);
            this.label1.TabIndex = 2;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(230, 26);
            this.label2.TabIndex = 3;
            this.label2.Text = "Fill out the columns to the right according to the\r\ndirections below. If none app" +
    "ly, simply click \'Ok\'\r\n";
            // 
            // newCompDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(882, 450);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButt);
            this.Controls.Add(this.compDataGridView);
            this.Name = "newCompDataForm";
            this.Text = "New Component Data";
            ((System.ComponentModel.ISupportInitialize)(this.compDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView compDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn partNumber;
        private System.Windows.Forms.DataGridViewCheckBoxColumn lineItem;
        private System.Windows.Forms.DataGridViewCheckBoxColumn qtySelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn maxQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn qtyStep;
        private System.Windows.Forms.Button okButt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}