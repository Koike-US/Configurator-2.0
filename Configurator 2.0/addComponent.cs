using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Configurator_2._0
{
    public partial class addComponent : Form
    {
        private bool _editSel;
        private bool _prep;
        private int _row = -1;

        public addComponent()
        {
            InitializeComponent();
            foreach (DataColumn dc in Globals.compData.Columns) compGrid.Columns.Add(dc.ColumnName, dc.ColumnName);
        }

        private void PrepCells(int r, object[] vals)
        {
            if (_prep) return;
            _prep = true;
            List<int> skipRows = new List<int> { 0, 1, 3, 4, 5, 9 };
            string[,] ddVals =
            {
                { "", "", "" }, { "", "", "" }, { "Y", "N", "" }, { "", "", "" }, { "", "", "" }, { "", "", "" },
                { "M", "P", "" }, { "BOM", "LINE", "" }, { "Cutting Machine", "Positioner", "Portable" },
                { "", "", "" }, { "", "", "" }
            };
            for (int i = 0; i < 10; ++i)
            {
                if (vals != null) compGrid.Rows[r].Cells[i].Value = vals[i];
                if (skipRows.Contains(i) == false)
                {
                    DataGridViewComboBoxCell cbc = new DataGridViewComboBoxCell();
                    for (int j = 0; j < 3; ++j)
                        if (ddVals[i, j] != "")
                        {
                            cbc.Items.Add(ddVals[i, j]);
                            if (vals != null) cbc.Value = vals[i];
                        }

                    compGrid.Rows[r].Cells[i] = cbc;
                }
            }

            _prep = false;
        }

        private void addButt_Click(object sender, EventArgs e)
        {
            string chkName = "";
            Tuple<object[,], DataTable> tup = Globals.utils.DbAddPrep(compGrid.Rows.Count, compGrid.Columns.Count,
                Globals.compData, compGrid,
                "COMP");
            if (tup.Item2 != null)
            {
                if (compGrid.Rows.Count == 2) chkName = tup.Item1[0, 0].ToString();
                Globals.compData.Merge(tup.Item2);
                Utilities.DeleteEmptyRows(Globals.compData);
                Globals.utils.WriteExcel(tup.Item1, Globals.DbFile, "Component Database", tup.Item2.Rows.Count - 1,
                    tup.Item2.Columns.Count, -1, 1, chkName);
                compGrid.DataSource = null;
                compGrid.Rows.Clear();
                compGrid.Refresh();
                if (_editSel) _editSel = false;
            }
        }

        private void cancelButt_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void addComponent_Load(object sender, EventArgs e)
        {
            compGrid.RowsAdded += compGrid_RowsAdded;
            PrepCells(compGrid.Rows.Count - 1, null);
        }

        private void compGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_editSel)
            {
                DataGridViewCell dCell = compGrid.SelectedCells[0];
                DataGridViewRow dgVr = dCell.OwningRow;
                string pNum = dgVr.Cells[0].Value.ToString();
                DataTable dt2 = Globals.compData.Select("[Part Number] = '" + pNum + "'").CopyToDataTable();
                if (dt2.Rows.Count == 0)
                {
                    MessageBox.Show("No match for Part Number found. Please try again.", "Existential Threat Error");
                }
                else
                {
                    compGrid.DataSource = null;
                    PrepCells(0, dt2.Rows[0].ItemArray);
                }

                compGrid.ReadOnly = false;
                compGrid.ClearSelection();
            }
        }

        private void updateItemButt_Click(object sender, EventArgs e)
        {
            string partNum =
                Interaction.InputBox("Input part number to edit, or leave blank for a list of components.");
            if (partNum != "")
            {
                DataRow[] dr = Globals.compData.Select("[Part Number] = '" + partNum + "'");
                DataTable dt2 = Globals.compData.Select("[Part Number] = '" + partNum + "'").CopyToDataTable();
                compGrid.DataSource = dt2;
                _row = Globals.compData.Rows.IndexOf(dr[0]);
                if (compGrid.Rows.Count == 0)
                {
                    MessageBox.Show("No match for Part Number found. Please try again.", "Existential Threat Error");
                    compGrid.DataSource = null;
                }

                PrepCells(0, dr[0].ItemArray);
            }
            else if (partNum == "")
            {
                DataView dV = new DataView(Globals.compData);
                compGrid.DataSource = dV.ToTable("Component Database", false, "Part Number", "Part Description");
                compGrid.ReadOnly = true;
                int i = 0;
                foreach (DataRow dr in Globals.compData.Rows)
                {
                    PrepCells(i, dr.ItemArray);
                    ++i;
                }

                _editSel = true;
            }
        }

        private void compGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            PrepCells(compGrid.Rows.Count - 1, null);
        }
    }
}