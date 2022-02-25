namespace Configurator_2._0
{
    partial class addComponent
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(addComponent));
            this.addButt = new System.Windows.Forms.Button();
            this.cancelButt = new System.Windows.Forms.Button();
            this.compGrid = new System.Windows.Forms.DataGridView();
            this.updateItemButt = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.compGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // addButt
            // 
            this.addButt.Location = new System.Drawing.Point(1136, 12);
            this.addButt.Name = "addButt";
            this.addButt.Size = new System.Drawing.Size(91, 23);
            this.addButt.TabIndex = 2;
            this.addButt.Text = "Add Item(s)";
            this.addButt.UseVisualStyleBackColor = true;
            this.addButt.Click += new System.EventHandler(this.addButt_Click);
            // 
            // cancelButt
            // 
            this.cancelButt.Location = new System.Drawing.Point(1136, 176);
            this.cancelButt.Name = "cancelButt";
            this.cancelButt.Size = new System.Drawing.Size(91, 23);
            this.cancelButt.TabIndex = 3;
            this.cancelButt.Text = "Cancel";
            this.cancelButt.UseVisualStyleBackColor = true;
            this.cancelButt.Click += new System.EventHandler(this.cancelButt_Click);
            // 
            // compGrid
            // 
            this.compGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.compGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.compGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.compGrid.Location = new System.Drawing.Point(12, 12);
            this.compGrid.Name = "compGrid";
            this.compGrid.Size = new System.Drawing.Size(1121, 520);
            this.compGrid.TabIndex = 4;
            this.compGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.compGrid_CellContentClick);
            // 
            // updateItemButt
            // 
            this.updateItemButt.Location = new System.Drawing.Point(1136, 41);
            this.updateItemButt.Name = "updateItemButt";
            this.updateItemButt.Size = new System.Drawing.Size(91, 23);
            this.updateItemButt.TabIndex = 5;
            this.updateItemButt.Text = "Update Item(s)";
            this.updateItemButt.UseVisualStyleBackColor = true;
            this.updateItemButt.Click += new System.EventHandler(this.updateItemButt_Click);
            // 
            // addComponent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(1243, 544);
            this.Controls.Add(this.updateItemButt);
            this.Controls.Add(this.compGrid);
            this.Controls.Add(this.cancelButt);
            this.Controls.Add(this.addButt);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "addComponent";
            this.Text = "Add New Component";
            this.Load += new System.EventHandler(this.addComponent_Load);
            ((System.ComponentModel.ISupportInitialize)(this.compGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button addButt;
        private System.Windows.Forms.Button cancelButt;
        private System.Windows.Forms.DataGridView compGrid;
        private System.Windows.Forms.Button updateItemButt;
    }
}