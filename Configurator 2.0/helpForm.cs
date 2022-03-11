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
    public partial class helpForm : Form
    {
        public helpForm()
        {
            InitializeComponent();
            configMachList.DataSource = Globals.machineData;
            configMachList.DisplayMember = "ModelCombo";
        }

        private void okButt_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
