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
    public partial class addMachine : Form
    {
        List<string> cmdData = new List<string> {"CUT WIDTH", "CUT LENGTH","MACH WIDTH", "MACH LENGTH", "WEIGHT", "TOOL CAP", "WATER CAP", "THICKNESS CAP", "MACH POWER"};
        List<string> posData = new List<string>();
        List<string> colData = new List<string>();
        DataTable dt;
        int r = 0;
        bool cellCBs = true;
        public addMachine()
        {
            InitializeComponent();
        }
        private void popGrid()
        {
            switch (DivisionCombo.Text)
            {
                case "POSITIONERS":
                    colData.AddRange(posData.ToArray());
                    break;
                case "CUTTING MACHINES":
                    colData.AddRange(cmdData.ToArray());
                    break;
            }
            int i = 0;
            string colName = "";
            foreach (DataColumn dc in Globals.machineData.Columns)
            {
                colName = dc.ColumnName;
                if (colData.Count() > 0 && dc.ColumnName == "CUT WIDTH" || i > 0 && i < colData.Count())
                {
                    colName = colData[i];
                    ++i;
                }
                machineGrid.Columns.Add(dc.ColumnName, dc.ColumnName);
            }
            DataGridViewComboBoxCell cbc = new DataGridViewComboBoxCell();
            cbc.DataSource = Globals.utils.getDT2(Globals.machineData, Globals.machineData.Columns[0].ColumnName,"","");
            cbc.DisplayMember = Globals.machineData.Columns[0].ColumnName;
            cbc.ValueMember = Globals.machineData.Columns[0].ColumnName;
            machineGrid.Rows[0].Cells[0] = cbc;
            updateRow(null, null);
            return;
        }
        private void optSels(object sender, EventArgs e)
        {
            ComboBox c = (ComboBox)sender;
            dt = Globals.machineData; ;
            ComboBox cb2;
            string cName = c.Name;
            string parCol = cName;
            string col = "";
            if (c.SelectedValue == System.DBNull.Value)
            {
                return;
            }
            string selVal = (string)c.SelectedValue;

            if (c.Name == "optTypeCombo")
            {

            }
            else
            {
                cName = dt.Columns[dt.Columns[cName].Ordinal + 1].ColumnName;
                col = cName;
                dt = dt.Select(c.Name + " = '" + selVal + "'").CopyToDataTable();

                Control[] cFind = this.Controls.Find(cName, false);
                if (cFind.Count() > 0)
                {
                    cb2 = (ComboBox)cFind[0];
                    Globals.utils.popItem(cb2, Globals.machineData, col, parCol, selVal);
                }
            }
        }
        private void addMachine_Load(object sender, EventArgs e)
        {
            Globals.utils.popItem(DivisionCombo, Globals.machineData, Globals.machineData.Columns[0].ColumnName, "", "");
            Globals.utils.initSelChange(this.Controls, optSels);
            popGrid();
        }
        private void updateRow(object sender, DataGridViewCellEventArgs e)
        {
            machineGrid.Rows[r].Cells[0].Value = DivisionCombo.Text;
            machineGrid.Rows[r].Cells[1].Value = TypeCombo.Text;
            machineGrid.Rows[r].Cells[2].Value = LineCombo.Text;
            ++r;
        }
        private void addMachButt_Click(object sender, EventArgs e)
        {
            Tuple<object[,],DataTable> tup = Globals.utils.dbAddPrep(machineGrid.Rows.Count, machineGrid.Columns.Count, Globals.machineData, machineGrid,"MACH");
            Globals.machineData.Merge(tup.Item2);
            Globals.utils.writeExcel(tup.Item1, Globals.dbFile, "Machine Data", machineGrid.Rows.Count - 1, machineGrid.Columns.Count - 1, Globals.machineData.Rows.Count+1,1,"");
        }
        private void quitButt_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void gridButt_Click(object sender, EventArgs e)
        {
            popGrid();
        }
        
        void machineGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (machineGrid.IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                machineGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        private void machineGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (cellCBs == true)
            //{
            //    DataGridViewComboBoxCell cb = (DataGridViewComboBoxCell)machineGrid.Rows[e.RowIndex].Cells[0];
            //    if (cb.Value != null && e.ColumnIndex < 2)
            //    {
            //        if (e.ColumnIndex == 0)
            //        {
            //            try
            //            {
            //                DataGridViewComboBoxCell cbc = new DataGridViewComboBoxCell();
            //                cbc.DataSource = cb.DataSource;
            //                cbc.DisplayMember = cb.DisplayMember;
            //                cbc.ValueMember = cb.ValueMember;
            //                machineGrid.Rows[e.RowIndex + 1].Cells[0] = cbc;
            //            }
            //            catch { }
            //        }
            //        if (cb.Value != null && cb.Value.ToString() != "")
            //        {
            //            cb = (DataGridViewComboBoxCell)machineGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            //            DataGridViewComboBoxCell cbc1 = new DataGridViewComboBoxCell();
            //            cbc1.DataSource = Globals.utils.getDT2(Globals.machineData, Globals.machineData.Columns[e.ColumnIndex + 1].ColumnName, Globals.machineData.Columns[e.ColumnIndex].ColumnName, cb.Value.ToString());
            //            cbc1.DisplayMember = Globals.machineData.Columns[e.ColumnIndex + 1].ColumnName;
            //            cbc1.ValueMember = Globals.machineData.Columns[e.ColumnIndex + 1].ColumnName;
            //            machineGrid.Rows[e.RowIndex].Cells[e.ColumnIndex + 1] = cbc1;
            //        }
            //        machineGrid.Invalidate();
            //    }
            //}
        }
        private void addTypeButt_Click(object sender, EventArgs e)
        {
            if(modelCombo.Text == "")
            {
                cellCBs = false;
                int i = machineGrid.Rows.Count - 1;
                if (machineGrid.Rows.Count == 1 && machineGrid.Rows[i].Cells[0].Value.ToString() != "")
                {
                    machineGrid.Rows.Add();
                }
                machineGrid.Rows[i].Cells[0].Value = DivisionCombo.Text;
                machineGrid.Rows[i].Cells[1].Value = TypeCombo.Text;
                machineGrid.Rows[i].Cells[2].Value = LineCombo.Text;
                DivisionCombo.Text = "";
                TypeCombo.Text = "";
                LineCombo.Text = "";
                cellCBs = true;
            }
            return;
        }
    }
}
