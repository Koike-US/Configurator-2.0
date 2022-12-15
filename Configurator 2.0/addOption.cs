using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Configurator_2._0
{
    public partial class addOption : Form
    {
        public addOption()
        {
            InitializeComponent();
            Globals.utils.PopItem(DivisionCombo, Globals.machineData, Globals.machineData.Columns[0].ColumnName, "",
                "");
            Globals.utils.PopItem(optTypeCombo, Globals.cmdOptComp, "Type", "", "");
        }

        private void OptionSelection(object sender, EventArgs e)
        {
            Control co = (Control)sender;
            DataTable dt = Globals.machineData;
            string cName = co.Name;
            string parCol = cName;
            List<string> selVal =
                new List<string>(); //Changing selVal from a string to a List to handle Multi-selection elements (Listboxes)
            switch (co)
            {
                case ComboBox _:
                {
                    ComboBox c = (ComboBox)sender;
                    selVal.Add((string)c.SelectedValue);
                    break;
                }
                case ListBox _:
                {
                    ListBox lb = (ListBox)sender;
                    selVal.Clear();
                    selVal.AddRange(lb.SelectedItems.Cast<string>().ToList());
                    break;
                }
            }

            if (selVal.Count == 0 || selVal[0] == "")
                return;
            if (selVal.Count == 0 && selVal[0] == "") return;
            try
            {
                cName = dt.Columns[dt.Columns[cName].Ordinal + 1].ColumnName;
                string col = cName;
                dt = dt.Select(co.Name + " = '" + selVal[0] + "'").CopyToDataTable();
                machModelBox.Items.Clear();
                foreach (DataRow r in dt.Rows) machModelBox.Items.Add(r[3]);
                Control[] cFind = co.Parent.Controls.Find(cName, false);
                if (!cFind.Any()) return;
                ComboBox cb2 = (ComboBox)cFind[0];
                Globals.utils.PopItem(cb2, Globals.machineData, col, parCol, selVal[0]);
            }
            catch
            {
                // ignored
            }
        }

        private void addOption_Load(object sender, EventArgs e)
        {
            ControlCollection controlCollection = new ControlCollection(this);
            foreach (Control c in Controls)
                if (c.Controls.Count > 1)
                    foreach (Control c2 in c.Controls)
                        controlCollection.Add(c2);
                else
                    controlCollection.Add(c);
            Utilities.InitializeSelectionChange(controlCollection, OptionSelection);
        }

        private void quitButt_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void addPartsButt_Click(object sender, EventArgs e)
        {
            int tbH = 27;
            foreach (string item in machModelBox.SelectedItems)
            {
                Label lbl = new Label();
                TextBox tb = new TextBox();
                lbl.Text = item;
                tb.Name = item.Replace(' ', '_');
                tb.Width = 400;
                tb.Location = new Point(12, tbH);
                lbl.Location = new Point(tb.Width + 20, tbH);
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
            if (d.Any()) row = Globals.cmdOptComp.Rows.IndexOf(d[d.Count() - 1]);
            Globals.cmdOptComp.Rows.InsertAt(dr2, row);
            Globals.utils.WriteExcel(dr2.ItemArray, Globals.DbFile, "Option Compatability", 1,
                dr2.ItemArray.Count() - 1, row, 1, "");
            MessageBox.Show("Option Added to Database");
        }

        private void clearButt_Click(object sender, EventArgs e)
        {
            componentsGrid.DataSource = null;
            foreach (Control c in Controls)
            {
                switch (c)
                {
                    case GroupBox _:
                        clearButt_Click(sender, e);
                        break;
                    case TextBox _:
                        c.Text = "";
                        break;
                    case ComboBox box:
                    {
                        box.Text = "";
                        box.DataSource = null;
                        break;
                    }
                }
            }

            machModelBox.ClearSelected();
            machModelBox.Items.Clear();
            Globals.utils.PopItem(DivisionCombo, Globals.machineData, Globals.machineData.Columns[0].ColumnName, "",
                "");
            Globals.utils.PopItem(optTypeCombo, Globals.cmdOptComp, "Type", "", "");
        }

        private void DivisionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}