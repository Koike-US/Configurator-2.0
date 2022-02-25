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
    public partial class addOption : Form
    {
        public addOption()
        {
            InitializeComponent();
            Globals.utils.popItem(DivisionCombo, Globals.machineData, Globals.machineData.Columns[0].ColumnName, "", "");
            Globals.utils.popItem(optTypeCombo, Globals.cmdOptComp, "Type", "", "");
            
        }
        private void optSels(object sender, EventArgs e)
        {
            ComboBox c;
            ListBox lb;
            Control co = (Control)sender;
            DataTable dt = Globals.machineData; ;
            ComboBox cb2;
            string cName = co.Name;
            string parCol = cName;
            string col = "";
            List<string> selVal = new List<string>();//Changing selVal from a string to a List to handle Multi-selection elements (Listboxes)
            if (co is ComboBox)
            {
                c = (ComboBox)sender;
                selVal.Add((string)c.SelectedValue);
            }
            if(co is ListBox)
            {
                lb = (ListBox)sender;
                selVal.Clear();
                selVal.AddRange(lb.SelectedItems.Cast<String>().ToList());
            }
            if (selVal.Count == 0 || selVal[0] == "")
            {
                return;
            }
            else if(selVal.Count != 0 || selVal[0] != "")
            {
                try
                {
                    cName = dt.Columns[dt.Columns[cName].Ordinal + 1].ColumnName;
                    col = cName;
                    dt = dt.Select(co.Name + " = '" + selVal[0] + "'").CopyToDataTable();
                    machModelBox.Items.Clear();
                    foreach (DataRow r in dt.Rows)
                    {
                        machModelBox.Items.Add(r[3]);
                    }
                    Control[] cFind = co.Parent.Controls.Find(cName, false);
                    if (cFind.Count() > 0)
                    {
                        cb2 = (ComboBox)cFind[0];
                        Globals.utils.popItem(cb2, Globals.machineData, col, parCol, selVal[0]);
                    }
                }
                catch { }
            }
        }
        private void addOption_Load(object sender, EventArgs e)
        {
            // List<Control> conts = new List<Control>();
            ControlCollection conts = new ControlCollection(this);
            foreach (Control c in this.Controls)
            {
                if(c.Controls.Count > 1)
                {
                    foreach(Control c2 in c.Controls)
                    {
                        conts.Add(c2);
                    }
                }
                else
                {
                    conts.Add(c);
                }
            }
            Globals.utils.initSelChange(conts,optSels);
        }
        private void quitButt_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void addPartsButt_Click(object sender, EventArgs e)
        {
            int tbH = 27;
            foreach(string item in machModelBox.SelectedItems)
            {
                Label lbl = new Label();
                TextBox tb = new TextBox();
                lbl.Text = item;
                tb.Name = (item.Replace(' ', '_'));
                tb.Width = 400;
                tb.Location = new System.Drawing.Point(12, tbH);
                lbl.Location = new System.Drawing.Point(tb.Width + 20, tbH);
                lbl.Width = 200;
                tbH = tbH + 27;
            }
        }
        private void addButt_Click(object sender, EventArgs e)
        {
            int row = Globals.cmdOptComp.Rows.Count;
            DataRow dr2 = Globals.cmdOptComp.NewRow();
            dr2[0] = optTypeCombo.Text;
            dr2[1] = nameBox.Text;
            dr2[2] = snDesBox.Text;
            dr2[3] = checkListBox.Text;
            dr2[4] = reqsBox.Text;
            dr2[5] = shortDescBox.Text;
            DataRow[] d = Globals.cmdOptComp.Select("Type = '" + optTypeCombo.Text + "'");
            if(d.Count() > 0)
            {
                row = Globals.cmdOptComp.Rows.IndexOf(d[d.Count()-1]);
            }
            Globals.cmdOptComp.Rows.InsertAt(dr2,row);
            Globals.utils.writeExcel(dr2.ItemArray, Globals.dbFile, "Option Compatability", 1, dr2.ItemArray.Count() - 1,row,1, "");
            MessageBox.Show("Option Added to Database");
        }
        private void clearButt_Click(object sender, EventArgs e)
        {
            componentsGrid.DataSource = null;
            foreach(Control c in this.Controls)
            {
                if(c is GroupBox)
                {
                    clearButt_Click(sender,e);
                }
                if(c is TextBox)
                {
                    c.Text = "";
                }
                if(c is ComboBox)
                {
                    ComboBox cb = (ComboBox)c;
                    cb.Text = "";
                    cb.DataSource = null;
                }
            }
            machModelBox.ClearSelected();
            machModelBox.Items.Clear();
            Globals.utils.popItem(DivisionCombo, Globals.machineData, Globals.machineData.Columns[0].ColumnName, "", "");
            Globals.utils.popItem(optTypeCombo, Globals.cmdOptComp, "Type", "", "");
        }

        private void DivisionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
