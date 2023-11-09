using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Configurator_2._0
{
    public partial class databaseViewer : Form
    {
        public string[] baseHeaders = { "Type", "Name", "Smart Designator", "OptionChecklist", "OptionReqs", "Short Description" };

        public databaseViewer()
        {

            InitializeComponent();
            openSesame();
        }
        private void openSesame()
        {
            List<string> tables = new List<string>();
            foreach (DataTable dt in Globals.dataBase.Tables)
            {
                tables.Add(dt.TableName);
            }
            databaseCombo.DataSource = tables;

            for(int i = 8; i < Globals.cmdOptComp.Columns.Count; i++)
            {
                modelListView.Items.Add(Globals.cmdOptComp.Columns[i].ColumnName);

            }
            databaseGridView.DataSource = Globals.cmdOptComp;
            databaseGridView.Refresh();
            databaseCombo.SelectedIndex = databaseCombo.FindStringExact("Option Compatability");

            return;
        }
        private void databaseCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            databaseGridView.DataSource = Globals.dataBase.Tables[databaseCombo.SelectedValue.ToString()];
            if(databaseCombo.SelectedValue.ToString() != "Option Compatability")
            {
                modelListView.Enabled = false;
            }
            else
            {
                modelListView.Enabled = true;
            }
        }

        private void refreshDBsButt_Click(object sender, EventArgs e)
        {
            Utilities ut = new Utilities();
            ut.updateDBs();
            modelListView.Items.Clear();
            openSesame();
        }

        private void modelListView_ItemChecked(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DataView dv = new DataView(Globals.cmdOptComp);
            string filter = "";// "[" + item.Text + "] <> ''";
            databaseGridView.Columns[6].ValueType = typeof(int);
            for (int i = 7; i < databaseGridView.Columns.Count; i++)
            {
                databaseGridView.Columns[i].Visible = false;
            }
            foreach (ListViewItem item in modelListView.Items)
            {
                if (item.Checked == true)
                {
                    if (string.IsNullOrEmpty(item.Text) == false && Globals.cmdOptComp.Columns.Contains(item.Text))
                    {
                        databaseGridView.Columns[item.Text].Visible = true;
                        if (filter != "")
                        {
                            filter = filter + "OR [" + item.Text + "] <> ''";
                        }
                        else
                        {
                            filter = filter + "[" + item.Text + "] <> ''";
                        }
                    }
                }
                else
                {
                    try
                    {
                        databaseGridView.Columns[item.Text].Visible = false;
                    }
                    catch { }
                }
            }
            dv.RowFilter = filter;
            databaseGridView.DataSource = dv;
        }
    }
}
