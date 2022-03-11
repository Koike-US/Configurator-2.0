
namespace Configurator_2._0
{
    partial class helpForm
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
            this.okButt = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.configMachList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // okButt
            // 
            this.okButt.Location = new System.Drawing.Point(713, 415);
            this.okButt.Name = "okButt";
            this.okButt.Size = new System.Drawing.Size(75, 23);
            this.okButt.TabIndex = 0;
            this.okButt.Text = "OK";
            this.okButt.UseVisualStyleBackColor = true;
            this.okButt.Click += new System.EventHandler(this.okButt_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Machines available to Configure";
            // 
            // configMachList
            // 
            this.configMachList.FormattingEnabled = true;
            this.configMachList.Location = new System.Drawing.Point(12, 25);
            this.configMachList.Name = "configMachList";
            this.configMachList.Size = new System.Drawing.Size(269, 368);
            this.configMachList.TabIndex = 2;
            // 
            // helpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.configMachList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButt);
            this.Name = "helpForm";
            this.Text = "Help";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox configMachList;
    }
}