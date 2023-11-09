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
    public partial class newCompDataForm : Form
    {
        public List<SimpleComponent> data = new List<SimpleComponent>();
        public newCompDataForm()
        {
            InitializeComponent();
        }
        public newCompDataForm(List<string> pns)
        {
            InitializeComponent();
            int i = 0;
            foreach(string s in pns)
            {
                compDataGridView.Rows.Add();
                compDataGridView.Rows[i].Cells[0].Value = s;
                ++i;
            }
        }

        private void okButt_Click(object sender, EventArgs e)
        {
            foreach(DataGridViewRow row in compDataGridView.Rows)
            {
                if (row.Cells[1].Value != null || row.Cells[2].Value != null)
                {
                    SimpleComponent s = new SimpleComponent();
                    s.pn = row.Cells[0].Value.ToString();
                    DataGridViewCheckBoxCell chk = new DataGridViewCheckBoxCell();
                    chk = (DataGridViewCheckBoxCell)row.Cells[1];
                    if(chk.Value!= null && (bool)chk.Value == true)
                    {
                        s.lineAdd = "LINE";
                    }
                    chk = (DataGridViewCheckBoxCell)row.Cells[2];
                    if (chk.Value != null && (bool)chk.Value == true)
                    {
                        s.qtySelect = "Y";
                        s.maxQty = row.Cells[3].Value.ToString();
                        s.qtyStep = row.Cells[4].Value.ToString();
                    }
                    data.Add(s);
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
