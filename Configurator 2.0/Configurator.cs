using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Configurator_2._0
{
    public partial class Configurator : Form
    {
        private readonly List<Control> _addedControls = new List<Control>();
        private readonly List<string> _baseOpts = new List<string>();
        private readonly List<Label> _lbls = new List<Label>();
        private readonly List<string> _optBoxes = new List<string>();

        private readonly string _optDb = "Option Compatability";
        private readonly List<string> _runTypes = new List<string>();
        private List<Label> _addedLabels = new List<Label>();
        private int _cBoxH = 265;
        private int _cntrlHt;
        private int _cntrlWidth;

        private int _iMax;
        private int _initOpts;

        private List<ListBox> _lBoxes = new List<ListBox>();
        private int[] _maxOptQty;

        public Configurator()
        {
            LoadingForm lf = new LoadingForm();
            lf.Show();
            Globals.utils.UpdateDBs();
            InitializeComponent();
            FormClosing += Configurator_FormClosing;
            _baseOpts.AddRange(Globals.machineData.Columns.Cast<DataColumn>().Select(x => x.ColumnName)
                .Where(x => x.Contains("Combo")).ToArray());
            _optBoxes.AddRange(_baseOpts.ToArray());
            _initOpts = _baseOpts.Count() - 1;
            Utilities.InitializeSelectionChange(Controls, SelectionChange);
            _iMax = Globals.cmdOptComp.Rows.Count;
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Text = string.Format(Text, version.Major, version.Minor, version.Build, version.Revision);
            Refresh();
            lf.Close();

            string version1 = AssemblyName
                .GetAssemblyName(@"W:\Engineering\Machine Configurator\Machine Configurator.exe")
                .Version.ToString();
            Assembly.GetExecutingAssembly();
            string version2 = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Version client = new Version(version2);
            Version server = new Version(version1);
            if (server > client)
            {
                MessageBox.Show(
                    "Configurator Update is required!! Configurator will now close, update, and restart when complete.");
                UpdateConfigurator();
                //TODO: Auto check for updates
            }

            Globals.utils.PopItem(DivisionCombo, Globals.machineData, Globals.machineData.Columns[0].ColumnName, "",
                "");
        }
        
        private void ResetAllControls(Control form)
        {
            foreach (Control control in form.Controls)
            {
                switch (control)
                {
                    case TextBox box:
                    {
                        box.Text = null;
                        box.BackColor = Color.White;
                        break;
                    }
                    case ComboBox box:
                    {
                        box.Text = "";
                        break;
                    }
                    case CheckBox box:
                    {
                        box.Checked = false;
                        break;
                    }
                    case ListBox box:
                    {
                        box.ClearSelected();
                        break;
                    }
                    case Button button1:
                    {
                        button1.BackColor = SystemColors.Control;
                        break;
                    }
                }
            }

            foreach (Control control in _addedControls)
            {
                form.Controls.Remove(control);
                control.Dispose();
            }

            _addedControls.Clear();
            completeConfigurationButton.BackColor = SystemColors.Control;
            checkConfigurationButton.BackColor = SystemColors.Control;
        }

        private void ClearAndResetForm()
        {
            Task.Run(() => Globals.utils.UpdateDBs());
            Globals.machine = new MachineData();
            Globals.prevConf = false;
            Globals.utils.PopItem(DivisionCombo, Globals.machineData, Globals.machineData.Columns[0].ColumnName, "",
                "");
            _cBoxH = 265;
            dataGridView1.DataSource = null;
            ResetAllControls(this);
            _runTypes.Clear();
            _runTypes.Capacity = 0;
        }

        private void SelectionChange(object sender, EventArgs e)
        {
            checkConfigurationButton.BackColor = SystemColors.Control;
            completeConfigurationButton.BackColor = SystemColors.Control;
            Control co = (Control)sender;
            List<string> selVal =
                new List<string>(); //Changing selVal from a string to a List to handle Multi-selection elements (Listboxes)
            string curOpt = co.Name; //Name of the currently selected option that fired off this event
            string nextOpt = ""; //Name of the next option available
            string dbName = "MachineData";
            switch (co)
            {
                case NumericUpDown down:
                {
                    foreach (option o in Globals.machine.selOpts.Where(o => o.optType == down.AccessibleName))
                    {
                        o.optQty = Convert.ToInt32(down.Value);
                        Globals.machine.snList.Remove(o.optFinSn);
                        o.optFinSn = o.optSnDes + "_" + down.Value;
                        Globals.machine.snList.Add(o.optSnDes + "_" + down.Value);
                    }

                    return;
                }
                case ComboBox box:
                {
                    if (box.SelectedValue == null) return;
                    selVal.Add(box.SelectedValue.ToString());
                    if (box.Name == "ModelCombo") SetMach(selVal[0]);
                    if (_maxOptQty != null && _maxOptQty.Any())
                    {
                        int maxQ = _maxOptQty[box.SelectedIndex];
                        NextCont(box.Name.Replace("Combo", "Numeric"), maxQ, selVal[0], "");
                    }

                    break;
                }
                case ListBox box:
                {
                    int lastSelectedIndex = (int)typeof(ListBox)
                        .GetProperty("FocusedIndex", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.GetValue(box, null);
                    string sel = box.Items[lastSelectedIndex].ToString();
                    if (sel.ToUpper() == "NONE" || sel.ToUpper().Contains("STANDARD"))
                        for (int k = 0; k < box.SelectedItems.Count; ++k)
                        {
                            string item = box.SelectedItems[k].ToString();
                            if (item.ToUpper() != "NONE" && sel.ToUpper().Contains("STANDARD") == false)
                                box.SelectedItems.Remove(item);
                        }

                    if (sel.ToUpper() != "NONE")
                        for (int k = 0; k < box.SelectedItems.Count; ++k)
                        {
                            string item = box.SelectedItems[k].ToString();
                            if (item.ToUpper() == "NONE") box.SelectedItems.Remove(item);
                        }

                    selVal.AddRange(box.SelectedItems.Cast<string>().ToList());
                    if (_maxOptQty != null && _maxOptQty.Any())
                    {
                        int maxQ = _maxOptQty[0];
                        NextCont(box.Name.Replace("List", "Numeric"), maxQ, selVal[0], box.Name);
                    }

                    break;
                }
            }

            if (selVal.Count == 0 || selVal[0] == "") return;

            if (co.Name != "ModelCombo" && Globals.machine.machName != null)
            {
                foreach (option o in Globals.machine.selOpts.Where(o => o.optType == co.Name && co is ListBox == false))
                {
                    Globals.machine.snList.Remove(o.optSnDes);
                    Globals.machine.selOpts.Remove(o);
                    break;
                }

                foreach (string val in selVal) AddOpt(val, co.Name);
            }

            Tuple<DataTable, string, string> tup = NextOption(curOpt, selVal[0]);
            if (tup == null) return;
            DataTable dtNext = tup.Item1; //Datatable to get data for the next option
            nextOpt = tup.Item2;
            dbName = tup.Item3;
            Control coNext = NextCont(nextOpt, 0, "", "");

            if (dbName == _optDb)
            {
                if (nextOpt == "") return;
                selVal[0] = nextOpt;
                nextOpt = "Name";
                curOpt = "Type";
            }

            _maxOptQty = Globals.utils.PopItem(coNext, dtNext, nextOpt, curOpt, selVal[0]);
        }

        private void SetMach(string selVal)
        {
            DataTable dr2 = Globals.machineData.Select("ModelCombo = '" + selVal + "'").CopyToDataTable();
            Globals.machine.prefix = dr2.Rows[0].Field<string>("Name Prefix");
            Globals.machine.smartPartNumber = dr2.Rows[0].Field<string>("Smart PN");
            Globals.machine.partNumber = dr2.Rows[0].Field<string>("Base Part Number");
            Globals.machine.drawingName = dr2.Rows[0].Field<string>("Drawing Name");
            Globals.machine.drawingSize = dr2.Rows[0].Field<string>("Drawing Size");

            Globals.machine.dwgRev = dr2.Rows[0].Field<string>("Drawing Rev");
            Globals.machine.revision = dr2.Rows[0].Field<string>("Revision");

            Globals.machine.checkName = dr2.Rows[0].Field<string>("Base CL Name");
            Globals.machine.machCode = dr2.Rows[0].Field<string>("MachCode");
            Globals.machine.checkEnd = dr2.Rows[0].Field<string>("End CL Name");
            Globals.machine.machName = selVal;
            Globals.machine.partType = DivisionCombo.Text;
            Globals.machine.description = dr2.Rows[0].Field<string>("ModelCombo") + ", ";
            Globals.machine.snList[0] = dr2.Rows[0].Field<string>("Smart PN");
            Globals.machine.soNum = soBox.Text;
            component c = new component
            {
                desc = dr2.Rows[0].Field<string>("Description"),
                number = dr2.Rows[0].Field<string>("Base Part Number"),
                qty = 1,
                mrpType = "M",
                partType = DivisionCombo.Text,
                revision = dr2.Rows[0].Field<string>("Revision")
            };
            Globals.machine.machComp = c;
        }

        private void AddOpt(string selVal, string cName)
        {
            DataRow[] drs = Globals.cmdOptComp.Select("Name = '" + selVal + "'");
            if (!drs.Any()) return;
            int rowIndex = 0;
            if (drs.Count() > 1)
                for (int i = 0; i < drs.Count(); ++i)
                    if (Globals.machine.snList.Contains(drs[i][4]))
                        rowIndex = i;

            DataRow[] drs2 = { drs[rowIndex] };

            DataTable dr2 = drs2.CopyToDataTable();
            if (Globals.machine.snList.Contains(dr2.Rows[0].Field<string>("Smart Designator")) != false) return;
            option opt = new option
            {
                optType = dr2.Rows[0].Field<string>("Type"),
                optName = dr2.Rows[0].Field<string>("Name"),
                optSnDes = dr2.Rows[0].Field<string>("Smart Designator"),
                checkName = dr2.Rows[0].Field<string>("OptionChecklist"),
                optDesc = dr2.Rows[0].Field<string>("Short Description")
            };
            opt.optFinDesc = opt.optDesc;
            opt.optFinSn = opt.optSnDes;

            //This needs to change to accomodate the numeric selector
            opt.optQty = 1;
            //This needs to change to accomodate the numeric selector

            if (selVal.ToUpper() != "NONE")
            {
                opt.optComps.AddRange(GetComponentData(dr2.Rows[0], opt.optQty));
                if (dr2.Rows[0].Field<string>("OptionReqs") != null)
                    try
                    {
                        opt.optReqs.AddRange(dr2.Rows[0].Field<string>("OptionReqs").Split(','));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
            }

            //This section checks for and removes an option if it was already selected.
            if (string.IsNullOrEmpty(opt.optSnDes) == false) Globals.machine.snList.Add(opt.optSnDes);
            Globals.machine.selOpts.Add(opt);
        }

        private void AddLabel(string name)
        {
            Label lbl = new Label();
            lbl.Text = Utilities.AddSpacesToSentence(name.Replace("Combo", ""));
            lbl.Size = new Size(200, 15);
            lbl.Name = name.Replace("Combo", "Label");
            lbl.Location = new Point(12, _cBoxH - 15);
            _lbls.Add(lbl);
            Controls.Add(lbl);
            lbl.Show();
            _addedControls.Add(lbl);
        }

        private static ComboBox AddCombo()
        {
            return new ComboBox();
        }

        private static ListBox AddList(string name)
        {
            ListBox cb = new ListBox();
            cb.SelectionMode = SelectionMode.MultiSimple;
            cb.SelectedItem = "";
            return cb;
        }

        private NumericUpDown AddNumeric(string name, int mQty)
        {
            NumericUpDown cb = new NumericUpDown();
            cb.Maximum = mQty;
            cb.Minimum = 1;
            return cb;
        }

        private Tuple<DataTable, string, string> NextOption(string optName, string selVal)
        {
            //optName should be the name of the current option box that fired off the event in selecChange()
            //Going to handle determining which DB I need to use in here, this should help clean up the selecChange() some
            //As well as improving my accuracy.
            DataTable dt = new DataTable();
            const string nextOpt = "";
            string parCol = "";
            Tuple<DataTable, string, string> opt = new Tuple<DataTable, string, string>(dt, nextOpt, "MachineData");
            if (optName == null)
            {
                return new Tuple<DataTable, string, string>(dt, "", _optDb);
                ;
            }

            bool machDb = false;
            bool optFound = false;
            if (Globals.machineData.Columns.Contains(optName))
            {
                //If the current option name is in the MachineData DB;
                dt = Globals.machineData;
                machDb = true;
                if (Globals.utils.IsOption(dt.Columns[dt.Columns[optName].Ordinal + 1].ColumnName))
                    //If the next Machine DB header is an option Name
                    opt = new Tuple<DataTable, string, string>(dt,
                        dt.Columns[dt.Columns[optName].Ordinal + 1].ColumnName, "MachineData");
                else
                    machDb = false;
            }

            if (machDb == false)
            {
                dt = Globals.cmdOptComp;
                int i = 0;
                try
                {
                    while (i < dt.Rows.Count && optFound == false)
                    {
                        if (dt.Rows[i].Field<string>(0) == optName)
                            while (dt.Rows[i].Field<string>(0) == optName && optFound == false)
                            {
                                if (i + 1 < dt.Rows.Count &&
                                    dt.Rows[i + 1][0].ToString() != optName) //&& dt.Rows[i + 1][0] != DBNull.Value)
                                {
                                    opt = new Tuple<DataTable, string, string>(dt, dt.Rows[i + 1].Field<string>(0),
                                        _optDb);
                                    parCol = "Type";
                                    optFound = true;
                                }

                                if (i + 1 == dt.Rows.Count)
                                {
                                    return new Tuple<DataTable, string, string>(dt, "", _optDb);
                                    ;
                                }

                                ++i;
                            }

                        ++i;
                    }
                }
                catch
                {
                    // ignored
                }

                if (optFound == false) opt = new Tuple<DataTable, string, string>(dt, dt.Rows[0][0].ToString(), _optDb);
            }

            if (opt.Item3 != _optDb) return opt;
            DataTable dt2 = Globals.utils.GetDt2(dt, "Name", parCol, opt.Item2);
            if (dt2.Rows.Count != 0 && Globals.utils.IsValidOption("Type", opt.Item2) != false) return opt;
            if (_runTypes.Contains(opt.Item2) == false)
            {
                _runTypes.Add(opt.Item2);
                opt = NextOption(opt.Item2, selVal);
            }
            else
            {
                opt = null;
            }

            return opt;
        }

        private Control NextCont(string optName, int mQty, string selVal, string parName)
        {
            int newCBoxH = 0;
            Control co = new Control();
            _cntrlWidth = 288;
            if (optName == "") return co;
            Control[] found = Controls.Find(optName, false);
            if (!found.Any())
            {
                string[] contTypes = { "Combo", "List", "Numeric" };
                switch (contTypes.FirstOrDefault(s => optName.Contains(s)))
                {
                    case "Combo":
                        co = AddCombo();
                        _cntrlHt = 21;
                        newCBoxH = _cBoxH + 40;
                        break;
                    case "List":
                        co = AddList(optName);
                        _cntrlHt = 80;
                        newCBoxH = _cBoxH + 90;
                        break;
                    case "Numeric":
                        co = AddNumeric(optName, mQty);
                        co.AccessibleName = parName;
                        optName = optName.Replace("Numeric", "Quantity");
                        _cntrlHt = 21;
                        _cntrlWidth = 128;
                        newCBoxH = _cBoxH + 40;
                        break;
                }

                co.Size = new Size(_cntrlWidth, _cntrlHt);
                co.Location = new Point(12, _cBoxH);
                co.Name = optName;
                co.TabIndex = 0;
                _optBoxes.Add(optName);
                AddLabel(optName);
                Controls.Add(co);
                Utilities.InitializeSelectionChange(Controls, SelectionChange);
                co.Show();
                Refresh();
                _cBoxH = newCBoxH;
                _addedControls.Add(co);
            }
            else
            {
                co = found[0];
            }

            return co;
        }

        private static IEnumerable<component> GetComponentData(DataRow dr, int optQty)
        {
            List<component> comps = new List<component>();
            string[] compList = dr.Field<string>(Globals.machine.machName).Split(',');
            DataTable dt = Globals.compData;
            foreach (string lComp in compList)
            {
                string comp = lComp.Replace(" ", "");
                if (comp.Contains("]")) comp = comp.Split(']')[1];
                if (comp.Contains("}")) comp = comp.Split('}')[1];
                if (comp.ToUpper() == "X") continue;
                DataRow dr2 = null;
                component c = new component();
                try
                {
                    DataRow[] drA = dt.Select("[Part Number] = '" + comp + "'");
                    dr2 = drA[0];
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error getting component data. Component " + comp +
                                    ", may not be entered in Configurator Component Database! \n" + e);
                }

                c.addType = dr2.Field<string>("Add Type");
                c.desc = dr2.Field<string>("Part Description");
                if (string.IsNullOrWhiteSpace(dr2.Field<string>("Max Qty")) == false)
                    c.maxQty = Convert.ToInt32(dr2.Field<string>("Max Qty"));
                if (string.IsNullOrWhiteSpace(dr2.Field<string>("Typ Qty")) == false)
                    c.typQty = Convert.ToInt32(dr2.Field<string>("Typ Qty"));
                c.mrpType = dr2.Field<string>("MRP Type");
                c.number = dr2.Field<string>("Part Number");
                c.partType = dr2.Field<string>("Part Type");
                c.revision = dr2.Field<string>("Revision");
                if (c.typQty * optQty <= c.maxQty)
                    c.qty = c.typQty * optQty;
                else
                    c.qty = c.typQty;
                comps.Add(c);
            }

            return comps.ToArray();
        }

        private void UpdateConfigurator()
        {
            string version1 = AssemblyName
                .GetAssemblyName(@"W:\Engineering\Machine Configurator\Machine Configurator.exe")
                .Version.ToString();
            Assembly.GetExecutingAssembly();
            string version2 = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Version client = new Version(version2);
            Version server = new Version(version1);
            if (server <= client)
            {
                MessageBox.Show("Latest Version already installed");
                return;
            }

            ProcessStartInfo info = new ProcessStartInfo(@"W:\Engineering\Apps - Calculators\Koike Update.exe")
                {
                    Arguments = "Configurator 1"
                };
            Process.Start(info);

            Application.Exit();
        }

        private static void FindDumNum()
        {
            string prefix = Globals.machine.prefix;
            for (int i = 1; i < 10000; ++i)
            {
                string suffix = i.ToString();
                suffix = suffix.PadLeft(4, '0');
                string dNum = prefix + suffix;
                if (Globals.confMachs != null)
                {
                    DataRow[] dr = Globals.confMachs.Select("[Epicor Part Number] = '" + dNum + "'");
                    if (dr.Any()) continue;
                    Globals.machine.epicorPartNumber = dNum;
                    return;
                }

                Globals.machine.epicorPartNumber = dNum;
                return;
            }
        }

        private void ExportBom()
        {
            string[,] expBom = new string[Globals.machine.bom.Rows.Count + 2, Globals.expRows];
            int j;
            for (j = 0; j < Globals.expRows; ++j) expBom[0, j] = j.ToString();
            string pType = "POS Machines";
            if (Globals.machine.partType == "Cutting Machine") pType = "CM";
            //Number PartDescription Epicor_Mfgcomment Epicor_Purcomment   Epicor_Mfg_name Epicor_MFGPartNum   Epicor_RandD_c Epicor_createdbylegacy_c    Epicor_PartType_c Epicor_EngComment_c Epicor_Confreq_c Epicor_EA_Manf_c    Epicor_EA_Volts_c Epicor_EA_Phase_c   Epicor_EA_Freq_c Epicor_EA_FLA_Supply_c  Epicor_EA_FLA_LgMot_c Epicor_EA_ProtDevRating_c   Epicor_EA_PannelSCCR_c Epicor_EA_EncRating_c   Revision Epicor_RevisionDescription  Dwg.Rev.Epicor_FullRel_c Reference Count PartRev.DrawNum_c Part.Model_c PartTypeElectrical  PartRev.DrawSize_c PartRev.SheetCount_c
            string[] mRow = (Globals.machine.epicorPartNumber + "," + Globals.machine.description.Replace(',', ' ') +
                             ",,,KOIKE,,False," + userBox.Text.ToUpper() + "," + pType +
                             ",,False, , , , , , , , , ,-,New Machine," + Globals.machine.dwgRev + ",1,1," +
                             Globals.machine.drawingName + "," + Globals.machine.machName + ",FALSE," +
                             Globals.machine.drawingSize + ",").Split(',');

            //string[] mRow = (Globals.machine.dumNum + "," + Globals.machine.desc.Replace(',', ' ') + ",,,KOIKE,,False," + userBox.Text.ToUpper() +"," + Globals.machine.partType + ",,False, , , , , , , , , ," + Globals.machine.dumNum + ",A," + "New Machine" + "," + Globals.machine.dwgRev + ",1," + Globals.machine.dumNum + ",A,1,,,FALSE,,FALSE," + Globals.machine.dwgName + "," + Globals.machine.dwgSize + ",,").Split(',');
            int i = 0;
            foreach (string s in mRow)
            {
                expBom[1, i] = s;
                ++i;
            }

            j = 2;
            string manu = "";
            foreach (component c in Globals.machine.bomComps)
            {
                if (c.mrpType == "M")
                {
                    manu = "KOIKE";
                }

                //Number PartDescription Epicor_Mfgcomment Epicor_Purcomment   Epicor_Mfg_name Epicor_MFGPartNum   Epicor_RandD_c Epicor_createdbylegacy_c    Epicor_PartType_c Epicor_EngComment_c Epicor_Confreq_c Epicor_EA_Manf_c    Epicor_EA_Volts_c Epicor_EA_Phase_c   Epicor_EA_Freq_c Epicor_EA_FLA_Supply_c  Epicor_EA_FLA_LgMot_c Epicor_EA_ProtDevRating_c   Epicor_EA_PannelSCCR_c Epicor_EA_EncRating_c   Revision Epicor_RevisionDescription  Dwg.Rev.Epicor_FullRel_c Reference Count PartRev.DrawNum_c Part.Model_c PartTypeElectrical  PartRev.DrawSize_c PartRev.SheetCount_c
                string[] row = (c.number + "," + c.desc.Replace(',', ' ') + ",,," + manu + ",,False,," + c.partType +
                                ",,False, , , , , , , , , ," + c.revision + ",CONF PART," + c.revision + ",1," + c.qty +
                                "," + c.number + ",,TRUE,,").Split(',');
                //string[] row = (c.number + "," + c.desc.Replace(',', ' ') + ",,," + manu + ",,False,," + c.partType + ",,False, , , , , , , , , ," + c.number + "," + c.revision + "," + c.desc.Replace(',', ' ') + "," + c.revision + ",1," + Globals.machine.dumNum + ",A," + c.qty.ToString() + ",," + c.number + ",TRUE," + c.partClass + "," + purch + "," + c.number + ",,,").Split(',');
                i = 0;
                foreach (string s in row)
                {
                    expBom[j, i] = s;
                    ++i;
                }

                ++j;
            }

            if (epicorExportCheckbox.Checked && Globals.prevConf == false)
                Globals.utils.WriteExcel(expBom,
                    @"\\manifest\BOM_Import\Mechanical\Import\" + Globals.machine.epicorPartNumber + "_BOM_0000.xlsx",
                    "EpdmBOMTable", Globals.machine.bom.Rows.Count + 2, Globals.expRows, -1, 1, "");
            if (!testExportCheckbox.Checked) return;
            Globals.prevConf = false;
            Globals.utils.WriteExcel(expBom, @"C:\Temp\BOM\" + Globals.machine.epicorPartNumber + "_BOM_0000.xlsx",
                "EpdmBOMTable", Globals.machine.bom.Rows.Count + 2, Globals.expRows, -1, 1, "");
        }

        private void updateConfButt_Click(object sender, EventArgs e)
        {
            UpdateConfigurator();
        }

        private static void Configurator_FormClosing(object sender, FormClosingEventArgs e)
        {
            ProcessStartInfo info = new ProcessStartInfo(@"W:\Engineering\Apps - Calculators\Koike Update.exe");
            info.Arguments = "Machine Configurator 0";
            Process.Start(info);
        }

        private void addMachineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addMachine addMach = new addMachine();
            addMach.Show();
        }

        private void addOptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addOption aOpt = new addOption();
            aOpt.Show();
        }

        private void addComponentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addComponent add = new addComponent();
            add.Show();
        }

        private void checkDBsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Utilities.CheckDatabase();
        }

        private void refreshDBsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals.utils.UpdateDBs();
        }

        private void featureRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://forms.clickup.com/f/85n7e-434/7LVVFXREY1COZ692U6");
        }

        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://app.clickup.com/8574190/v/l/li/82070636?pr=14768824");
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            helpForm help = new helpForm();
            help.Show();
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string version1 = AssemblyName
                .GetAssemblyName(@"W:\Engineering\Machine Configurator\Machine Configurator.exe")
                .Version.ToString();
            Assembly.GetExecutingAssembly();
            string version2 = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Version client = new Version(version2);
            Version server = new Version(version1);
            if (server <= client)
            {
                MessageBox.Show("Latest Version already installed");
                return;
            }

            ProcessStartInfo info = new ProcessStartInfo(@"W:\Engineering\Apps - Calculators\Koike Update.exe");
            info.Arguments = "Configurator 1";
            Process.Start(info);

            Application.Exit();
        }

        private void checkConfigurationButton_Click(object sender, EventArgs e)
        {
            timesConfBox.BackColor = Color.Red;
            confNumBox.BackColor = Color.Red;
            dataGridView1.DataSource = null;
            if (Globals.machine.selOpts.Count < 1 && _optBoxes.Count > _baseOpts.Count)
            {
                MessageBox.Show("No machine options have been selected yet.");
                return;
            }

            Globals.confMachs = Globals.dataBase.Tables[Globals.machine.prefix + " CONF"];

            string smartNum = Globals.machine.snList[0]; // + "-";
            string smartStart = Globals.machine.snList[0];

            List<string> sortedSNs = new List<string>();
            sortedSNs.AddRange(Globals.machine.snList.ToArray());
            sortedSNs.Sort();
            sortedSNs.RemoveAll(item => item == null);
            sortedSNs.RemoveAll(item => item == "");
            int j = 0;
            smartNum = sortedSNs.Where(i => i != smartStart && string.IsNullOrWhiteSpace(i) == false).Aggregate(smartNum, (current, i) => current + "-" + i);
            FindDumNum();
            timesConfBox.Text = "0";
            confNumBox.Text = "";
            dwgNumBox.Text = Globals.machine.drawingName;
            dwgSizeBox.Text = Globals.machine.drawingSize;
            if (Globals.confMachs != null)
            {
                DataRow[] dr = Globals.confMachs.Select("[Part Number] = '" + smartNum + "'");
                if (dr.Count() == 1)
                {
                    j = 0;
                    for (j = 0; j < Globals.confMachs.Rows.Count; ++j)
                        if (Globals.confMachs.Rows[j][0] !=
                            DBNull.Value) // && (string)Globals.confMachs.Rows[j][0] == smartNum)
                        {
                            List<string> confNum = new List<string>();
                            confNum.AddRange(Globals.confMachs.Rows[j][0].ToString().Split('-'));
                            confNum.Sort();
                            
                            if (!confNum.SequenceEqual(sortedSNs)) continue;
                            
                            Globals.machine.epicorPartNumber = dr[0].Field<string>("Epicor Part Number");
                            timesConfBox.Text = Convert.ToInt32(dr[0].Field<string>("Times Configured")).ToString();
                            Globals.machine.configuredDate = dr[0].Field<string>("Date Configured");
                            Globals.machine.configuredBy = dr[0].Field<string>("User Added");
                            string[] salesOrders = dr[0].Field<string>("Sales Orders").Replace("[", "")
                                .Replace("]", "")
                                .Replace(" ", "").Split(',');
                            timesConfBox.BackColor = Color.LightGreen;
                            confNumBox.Text = Globals.machine.epicorPartNumber;
                            confNumBox.BackColor = Color.LightGreen;
                            Globals.machine.salesOrders = salesOrders.ToList();
                            Globals.foundRow = j;
                            Globals.prevConf = true;
                            break;
                        }
                }
            }

            Globals.machine.smartPartNumber = smartNum;
            Globals.machine.timesConfigured = Convert.ToInt32(timesConfBox.Text);
            Globals.machine.configuredDate = DateTime.Now.ToShortDateString();
            Globals.machine.configuredBy = Environment.UserName;
            Globals.utils.WriteMachine();
            DataTable dt = Globals.machine.bom;
            dataGridView1.DataSource = dt;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Columns[0].Width = 175;
            dataGridView1.Columns[1].Width = 350;
            dataGridView1.Columns[2].Width = 50;
            dataGridView1.Columns[3].Width = 50;
            for (int k = 0; k < dataGridView1.Columns.Count; ++k)
                dataGridView1.Columns.Remove(dataGridView1.Columns[4]);
            checkConfigurationButton.BackColor = Color.LawnGreen;
        }

        private void completeConfigurationButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(userBox.Text))
            {
                MessageBox.Show(
                    "No User initials entered!!!! Please enter your three character User Initials and try again.",
                    "Can I See Your ID Error");
                return;
            }

            Globals.machine.salesOrders.Add(soBox.Text);
            Globals.utils.WriteMachineToDatabase(Globals.machine);

            if (epicorExportCheckbox.Checked || testExportCheckbox.Checked) Task.Run(() => ExportBom());
            //MessageBox.Show("BOM Exported for Epicor.");
            MessageBox.Show("Configuration Complete");
            completeConfigurationButton.BackColor = Color.LawnGreen;
        }

        private void clearConfigButton_Click(object sender, EventArgs e)
        {
            ClearAndResetForm();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }
    }
}