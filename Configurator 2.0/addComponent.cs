using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.VisualBasic;

namespace Configurator_2._0
{
    public partial class addComponent : Form
    {
        
        bool editSel = false;
        int row = -1;
        bool prep = false;
        public addComponent()
        {
            InitializeComponent();
            foreach (DataColumn dc in Globals.compData.Columns)
            {
                compGrid.Columns.Add(dc.ColumnName, dc.ColumnName);
            }
        }
        private void prepCells(int r, object[] vals)
        {
            if(prep == true)
            {
                return;
            }
            prep = true;
            List<int> skipRows = new List<int>{ 0, 1, 3, 4, 5, 9 };
            string[,] ddVals = new string[,] { { "", "", "" }, { "", "", "" }, { "Y", "N", "" }, { "", "", "" }, { "", "", "" }, { "", "", "" }, { "M", "P", "" }, { "BOM", "LINE", "" }, { "Cutting Machine", "Positioner","Portable" }, { "", "", "" }, { "", "", "" } };
            for (int i = 0; i < 10; ++i)
            {
                if (vals != null)
                {
                    compGrid.Rows[r].Cells[i].Value = vals[i];
                }
                if (skipRows.Contains(i) == false)
                {
                    DataGridViewComboBoxCell cbc = new DataGridViewComboBoxCell();
                    for(int j = 0; j < 3; ++ j)
                    {
                        if (ddVals[i, j] != "")
                        {
                            cbc.Items.Add(ddVals[i, j]);
                            if(vals != null)
                            {
                                cbc.Value = vals[i];
                            }
                        }
                    }
                    compGrid.Rows[r].Cells[i] = cbc;
                }
            }
            prep = false;
        }

        private void addButt_Click(object sender, EventArgs e)
        {
            string chkName = "";
            Tuple<object[,], DataTable> tup = Globals.utils.dbAddPrep(compGrid.Rows.Count, compGrid.Columns.Count, Globals.compData, compGrid,"COMP");
            if (tup.Item2 != null)
            {
                if (compGrid.Rows.Count == 2)
                {
                    chkName = tup.Item1[0, 0].ToString();
                }
                Globals.compData.Merge(tup.Item2);
                Globals.utils.delEmptyRows(Globals.compData);
                Globals.utils.writeExcel(tup.Item1, Globals.dbFile, "Component Database", tup.Item2.Rows.Count - 1, tup.Item2.Columns.Count, -1, 1, chkName);
                compGrid.DataSource = null;
                compGrid.Rows.Clear();
                compGrid.Refresh();
                if (editSel == true)
                {
                    editSel = false;
                }
            }
        }
        private void cancelButt_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void addComponent_Load(object sender, EventArgs e)
        {
            this.compGrid.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.compGrid_RowsAdded);
            prepCells(compGrid.Rows.Count - 1, null);  
        }
        private void compGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (editSel == true)
            {
                DataGridViewCell dCell = compGrid.SelectedCells[0];
                DataTable dGV = (DataTable)compGrid.DataSource;
                DataGridViewRow dgVR = dCell.OwningRow;
                string pNum = dgVR.Cells[0].Value.ToString();
                DataTable dt2 = Globals.compData.Select("[Part Number] = '" + pNum + "'").CopyToDataTable();
                if(dt2.Rows.Count == 0)
                {
                    MessageBox.Show("No match for Part Number found. Please try again.", "Existential Threat Error");
                }
                else
                {
                    compGrid.DataSource = null;
                    prepCells(0, dt2.Rows[0].ItemArray);
                }
                compGrid.ReadOnly = false;
                compGrid.ClearSelection();
            }
        }
        private void updateItemButt_Click(object sender, EventArgs e)
        {
            string partNum = Microsoft.VisualBasic.Interaction.InputBox("Input part number to edit, or leave blank for a list of components.");
            if (partNum != "")
            {
                DataRow[] dr = Globals.compData.Select("[Part Number] = '" + partNum + "'");
                DataTable dt2 = Globals.compData.Select("[Part Number] = '" + partNum + "'").CopyToDataTable();
                compGrid.DataSource = dt2;
                row = Globals.compData.Rows.IndexOf(dr[0]);
                if (compGrid.Rows.Count == 0)
                {
                    MessageBox.Show("No match for Part Number found. Please try again.", "Existential Threat Error");
                    compGrid.DataSource = null;
                }
                prepCells(0, dr[0].ItemArray);
            }
            else if(partNum == "")
            {
                DataView dV = new DataView(Globals.compData);
                compGrid.DataSource = dV.ToTable("Component Database", false, "Part Number", "Part Description");
                compGrid.ReadOnly = true;
                int i = 0;
                foreach(DataRow dr in Globals.compData.Rows)
                {
                    prepCells(i, dr.ItemArray);
                    ++i;
                }
                editSel = true;
            }
        }
        private void compGrid_RowsAdded(object sender, System.Windows.Forms.DataGridViewRowsAddedEventArgs e)
        {
            prepCells(compGrid.Rows.Count - 1,null);
        }
    }
}
