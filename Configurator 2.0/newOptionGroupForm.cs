using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Configurator_2._0
{
    public partial class newOptionGroupForm : Form
    {
        public string optionGroupName { get; set; }
        public newOptionGroupForm()
        {
            InitializeComponent();

            DataTable dt = Globals.cmdOptComp.DefaultView.ToTable(true, new string[] { "Type" });
            //optionGroupsBox.Items.Add("At Top");
            optionGroupsBox.Items.AddRange(dt.AsEnumerable().Select(r => r.Field<string>("Type")).ToArray());
            optionGroupsBox.DisplayMember = "Type";
            optionGroupsBox.ValueMember = "Type";

        }

        private void createGroupBox_Click(object sender, EventArgs e)
        {
            if (groupNameBox.Text != "")
            {
                string ring = optionGroupsBox.Text;
                optionGroupName = optionGroupsBox.Text + ":" + groupNameBox.Text.Replace(" ", "");
                if (comboBoxRadio.Checked == true)
                {
                    optionGroupName = optionGroupName + "Combo";
                }
                else if (listBoxRadio.Checked == true)
                {
                    optionGroupName = optionGroupName + "List";
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void optionGroupsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("DERP");
            //MessageBox.Show(optionGroupsBox.SelectedText);
        }
    }
}
