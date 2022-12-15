using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Configurator_2._0
{
    public partial class addMachine : Form
    {
        private readonly List<string> _cmdData = new List<string>
        {
            "CUT WIDTH", "CUT LENGTH", "MACH WIDTH", "MACH LENGTH", "WEIGHT", "TOOL CAP", "WATER CAP", "THICKNESS CAP",
            "MACH POWER"
        };

        private readonly List<string> _colData = new List<string>();
        private readonly List<string> _posData = new List<string>();
        private bool _cellCBs = true;
        private DataTable _dt;
        private int _r;

        public addMachine()
        {
            InitializeComponent();
        }

        private void PopGrid()
        {
            switch (DivisionCombo.Text)
            {
                case "POSITIONERS":
                    _colData.AddRange(_posData.ToArray());
                    break;
                case "CUTTING MACHINES":
                    _colData.AddRange(_cmdData.ToArray());
                    break;
            }

            int i = 0;
            string colName = "";
            foreach (DataColumn dc in Globals.machineData.Columns)
            {
                colName = dc.ColumnName;
                if ((_colData.Any() && dc.ColumnName == "CUT WIDTH") || (i > 0 && i < _colData.Count()))
                {
                    colName = _colData[i];
                    ++i;
                }

                machineGrid.Columns.Add(dc.ColumnName, dc.ColumnName);
            }

            DataGridViewComboBoxCell cbc = new DataGridViewComboBoxCell();
            cbc.DataSource =
                Globals.utils.GetDt2(Globals.machineData, Globals.machineData.Columns[0].ColumnName, "", "");
            cbc.DisplayMember = Globals.machineData.Columns[0].ColumnName;
            cbc.ValueMember = Globals.machineData.Columns[0].ColumnName;
            machineGrid.Rows[0].Cells[0] = cbc;
            UpdateRow(null, null);
        }

        private void OptionSelection(object sender, EventArgs e)
        {
            ComboBox c = (ComboBox)sender;
            _dt = Globals.machineData;
            ;
            string cName = c.Name;
            string parCol = cName;
            string col = "";
            if (c.SelectedValue == DBNull.Value) return;
            string selVal = (string)c.SelectedValue;

            if (c.Name == "optTypeCombo")
            {
            }
            else
            {
                cName = _dt.Columns[_dt.Columns[cName].Ordinal + 1].ColumnName;
                col = cName;
                _dt = _dt.Select(c.Name + " = '" + selVal + "'").CopyToDataTable();

                Control[] cFind = Controls.Find(cName, false);
                if (!cFind.Any()) return;
                ComboBox cb2 = (ComboBox)cFind[0];
                Globals.utils.PopItem(cb2, Globals.machineData, col, parCol, selVal);
            }
        }

        private void addMachine_Load(object sender, EventArgs e)
        {
            Globals.utils.PopItem(DivisionCombo, Globals.machineData, Globals.machineData.Columns[0].ColumnName, "",
                "");
            Utilities.InitializeSelectionChange(Controls, OptionSelection);
            PopGrid();
        }

        private void UpdateRow(object sender, DataGridViewCellEventArgs e)
        {
            machineGrid.Rows[_r].Cells[0].Value = DivisionCombo.Text;
            machineGrid.Rows[_r].Cells[1].Value = TypeCombo.Text;
            machineGrid.Rows[_r].Cells[2].Value = LineCombo.Text;
            ++_r;
        }

        private void addMachButt_Click(object sender, EventArgs e)
        {
            Tuple<object[,], DataTable> tup = Globals.utils.DbAddPrep(machineGrid.Rows.Count, machineGrid.Columns.Count,
                Globals.machineData,
                machineGrid, "MACH");
            Globals.machineData.Merge(tup.Item2);
            Globals.utils.WriteExcel(tup.Item1, Globals.DbFile, "Machine Data", machineGrid.Rows.Count - 1,
                machineGrid.Columns.Count - 1, Globals.machineData.Rows.Count + 1, 1, "");
        }

        private void quitButt_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void gridButt_Click(object sender, EventArgs e)
        {
            PopGrid();
        }

        private void addTypeButt_Click(object sender, EventArgs e)
        {
            if (modelCombo.Text != "") return;
            _cellCBs = false;
            int i = machineGrid.Rows.Count - 1;
            if (machineGrid.Rows.Count == 1 && machineGrid.Rows[i].Cells[0].Value.ToString() != "")
                machineGrid.Rows.Add();
            machineGrid.Rows[i].Cells[0].Value = DivisionCombo.Text;
            machineGrid.Rows[i].Cells[1].Value = TypeCombo.Text;
            machineGrid.Rows[i].Cells[2].Value = LineCombo.Text;
            DivisionCombo.Text = "";
            TypeCombo.Text = "";
            LineCombo.Text = "";
            _cellCBs = true;
        }
    }
}