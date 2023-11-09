using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Stand_Alone_Solidworks_Interface;

using EPDM.Interop.epdm;
using MongoDB.Driver;

namespace Configurator_2._0
{
    public partial class Configurator : Form
    {
        List<string> baseOpts = new List<string>();
        List<string> optBoxes = new List<string>();
        List<string> runTypes = new List<string>();

        List<ListBox> lBoxes = new List<ListBox>();
        List<Label> lbls = new List<Label>();

        List<Control> addedControls = new List<Control>();
        List<Label> addedLabels = new List<Label>();

        epicorInterop eOp = new epicorInterop();
        int iMax = 0;
        int cBoxH = 265;
        int initOpts = 0;
        int cntrlHt = 0;
        int cntrlWidth = 0;
        int[] maxOptQty;

        bool configurationComplete = false;

        string optDB = "Option Compatability";
        public Configurator()
        {
            LoadingForm lf = new LoadingForm();
            lf.Show();
            Globals.utils.updateDBs();
            InitializeComponent();
            this.FormClosing += Configurator_FormClosing;
            baseOpts.AddRange(Globals.machineData.Columns.Cast<DataColumn>().Select(x => x.ColumnName).Where(x => x.Contains("Combo")).ToArray());
            optBoxes.AddRange(baseOpts.ToArray());
            initOpts = baseOpts.Count() - 1;
            Globals.utils.initSelChange(this.Controls, selecChange);
            iMax = Globals.cmdOptComp.Rows.Count;
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = String.Format(this.Text, version.Major, version.Minor, version.Build, version.Revision);
            this.Refresh();
            lf.Close();

            string version1 = AssemblyName.GetAssemblyName(@"W:\Engineering\Machine Configurator\Machine Configurator.exe").Version.ToString();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string version2 = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Version client = new Version(version2);
            Version server = new Version(version1);
            if (server > client)
            {
                MessageBox.Show("Configurator Update is required!! Configurator will now close, update, and restart when complete.");
                updateConfigurator();
                //TODO: Auto check for updates
                //updateConfButt.BackColor = Color.Red;
                //updateConfButt.Text = "Update Required!";
            }
            Globals.utils.popItem(DivisionCombo, Globals.machineData, Globals.machineData.Columns[0].ColumnName, "", "");

        }
        private void clearCombos(string cur)
        {
            List<string> conNames = new List<string>();
            int i = 0;
            //int j = 0;
            foreach (string c in optBoxes)//(j = 0; j < optBoxes.Count;++j)//
            {
                //string c = optBoxes[j];
                //if (i > optBoxes.IndexOf(cur))
                //{
                List<Control> cb = new List<Control>();
                cb.AddRange(this.Controls.Find(c, false));
                cb.AddRange(this.Controls.Find(c.Replace("Combo", "Label"), false));
                cb.AddRange(this.Controls.Find(c.Replace("List", "Label"), false));
                if (cb.Count == 0)
                {
                    return;
                }
                Control co = cb[0];



                //if (cb[0].Text != "")
                //{
                if (baseOpts.Contains(c) == true)
                {
                    ComboBox cb0 = (ComboBox)cb[0];
                    cb0.BeginInvoke(new Action(() => { cb0.Text = ""; }));
                    if (cb[0].Name != "DivisionCombo")
                    {
                        cb0.BeginInvoke(new Action(() => { cb0.DataSource = null; }));
                    }
                }
                else
                {
                    foreach (Control con in cb)
                    {
                        this.BeginInvoke(new Action(() => { this.Controls.Remove(con); }));
                    }
                    //optBoxes.Remove(c);
                    //--i;
                }
                //}
                //}
                ++i;
            }
            this.BeginInvoke(new Action(() => { this.Refresh(); }));
            optBoxes.Clear();
            optBoxes.AddRange(baseOpts.ToArray());

            return;
        }

        private void ResetAllControls(Control form)
        {
            foreach (Control control in form.Controls)
            {
                if (control is TextBox)
                {
                    TextBox textBox = (TextBox)control;
                    textBox.Text = null;
                    textBox.BackColor = System.Drawing.Color.White;
                }

                if (control is ComboBox)
                {
                    ComboBox comboBox = (ComboBox)control;
                    comboBox.Text = "";
                }

                if (control is CheckBox)
                {
                    CheckBox checkBox = (CheckBox)control;
                    checkBox.Checked = false;
                }

                if (control is ListBox)
                {
                    ListBox listBox = (ListBox)control;
                    listBox.ClearSelected();
                }
                if(control is Button)
                {
                    Button button = (Button)control;
                    button.BackColor = SystemColors.Control;
                }
            }
            foreach(Control control in addedControls)
            {
                form.Controls.Remove(control);
                control.Dispose();
            }
            addedControls.Clear();
            completeConfigurationButton.BackColor = SystemColors.Control;
            checkConfigurationButton.BackColor = SystemColors.Control;
        }

        private void clearAndResetForm()
        {
            Task.Run(() => Globals.utils.updateDBs());
            Globals.machine = new MachineData();
            Globals.prevConf = false;
            Globals.utils.popItem(DivisionCombo, Globals.machineData, Globals.machineData.Columns[0].ColumnName, "", "");
            cBoxH = 265;
            dataGridView1.DataSource = null;
            ResetAllControls(this);
            runTypes.Clear();
            runTypes.Capacity = 0;
            configurationComplete = false;
            return;
        }
        private void selecChange(object sender, EventArgs e)
        {
            checkConfigurationButton.BackColor = SystemColors.Control;
            completeConfigurationButton.BackColor = SystemColors.Control;
            Control co = (Control)sender;
            //clearCombos(co.Name);
            List<string> sns = Globals.machine.snList;
            Control coNext = new Control();
            ComboBox cb = new ComboBox();
            ListBox lb = new ListBox();
            NumericUpDown nb = new NumericUpDown();
            DataTable dtCur = new DataTable();//Datatable to get data for the current option
            DataTable dtNext = new DataTable();//Datatable to get data for the next option
            List<string> selVal = new List<string>();//Changing selVal from a string to a List to handle Multi-selection elements (Listboxes)
            string curOpt = co.Name; //Name of the currently selected option that fired off this event
            string nextOpt = "";//Name of the next option available
            string dbName = "MachineData";
            if (co is NumericUpDown)
            {
                nb = (NumericUpDown)co;
                foreach (option o in Globals.machine.selOpts)
                {
                    if (o.optType == nb.AccessibleName)
                    {
                        o.optQty = Convert.ToInt32(nb.Value);
                        Globals.machine.snList.Remove(o.optFinSn);
                        o.optFinSn = o.optSnDes + "_" + nb.Value.ToString();
                        Globals.machine.snList.Add(o.optSnDes + "_" + nb.Value.ToString());
                    }
                }
                return;
            }
            if (co is ComboBox)
            {
                cb = (ComboBox)co;
                if (cb.SelectedValue == null)
                {
                    return;
                }
                selVal.Add(cb.SelectedValue.ToString());
                if (cb.Name == "ModelCombo")
                {
                    setMach(selVal[0]);
                }
                if (maxOptQty != null && maxOptQty.Count() > 0)
                {
                    int maxQ = maxOptQty[cb.SelectedIndex];
                    nextCont(cb.Name.Replace("Combo", "Numeric"), maxQ, selVal[0], "");
                }
            }
            if (co is ListBox)
            {
                lb = (ListBox)co;
                if(co.Name.ToUpper().Contains("FLASHCUT"))
                {
                    string derp = "herp";
                }
                int lastSelectedIndex = (int)typeof(ListBox).GetProperty("FocusedIndex", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(lb, null);
                string sel = lb.Items[lastSelectedIndex].ToString();
                if (sel.ToUpper() == "NONE" || sel.ToUpper().Contains("STANDARD"))
                {
                    for (int k = 0; k < lb.SelectedItems.Count; ++k)
                    {
                        string item = lb.SelectedItems[k].ToString();
                        if (item.ToUpper() != "NONE" && sel.ToUpper().Contains("STANDARD") == false)
                        {
                            lb.SelectedItems.Remove(item);
                        }
                    }
                }
                if (sel.ToUpper() != "NONE")
                {
                    for (int k = 0; k < lb.SelectedItems.Count; ++k)
                    {
                        string item = lb.SelectedItems[k].ToString();
                        if (item.ToUpper() == "NONE")
                        {
                            lb.SelectedItems.Remove(item);
                        }
                    }
                }
                selVal.AddRange(lb.SelectedItems.Cast<String>().ToList());
                if (maxOptQty != null && maxOptQty.Count() > 0)
                {
                    int maxQ = maxOptQty[0];
                    nextCont(lb.Name.Replace("List", "Numeric"), maxQ, selVal[0], lb.Name);
                }
            }
            if (selVal.Count == 0 || selVal[0] == "")
            {
                return;
            }
            //next_opt:


            //if (dbName == optDB)
            //{
                if (co.Name != "ModelCombo" && Globals.machine.machName != null)
                {
                    foreach (option o in Globals.machine.selOpts)
                    {
                        if (o.optType == co.Name && co is ListBox == false)
                        {
                            Globals.machine.snList.Remove(o.optSnDes);
                            Globals.machine.selOpts.Remove(o);
                            break;
                        }
                    }
                    foreach (string val in selVal)
                    {
                        addOpt(val, co.Name);
                    }
                }
            //}

            Tuple<DataTable, string, string> tup = nextOption(curOpt, selVal[0]);
            if(tup == null)
            {
                return;
            }
            dtNext = tup.Item1;
            nextOpt = tup.Item2;
            dbName = tup.Item3;
            coNext = nextCont(nextOpt, 0, "", "");

            if (dbName == optDB)
            {
                if (nextOpt == "")
                {
                    return;
                }
                selVal[0] = nextOpt;
                nextOpt = "Name";
                curOpt = "Type";
            }
            maxOptQty = Globals.utils.popItem(coNext, dtNext, nextOpt, curOpt, selVal[0]);
            //int coIC = 0;
            //if (coNext is ComboBox)
            //{
            //    ComboBox cbt = (ComboBox)coNext;
            //    coIC = cbt.Items.Count;
            //}
            //else if (coNext is ListBox)
            //{
            //    ListBox cbt = (ListBox)coNext;
            //    coIC = cbt.Items.Count;
            //}
            //else if (coNext is Label)
            //{
            //    return;
            //}
            //if (coIC == 0)
            //{
            //    coNext.Text = "Not Available";
            //    curOpt = selVal[0];
            //    goto next_opt;
            //}


            return;
        }
        private void setMach(string selVal)
        {
            DataTable dr2 = Globals.machineData.Select("ModelCombo = '" + selVal + "'").CopyToDataTable();
            Globals.machine.prefix = dr2.Rows[0].Field<string>("Name Prefix");
            Globals.machine.SmartPartNumber = dr2.Rows[0].Field<string>("Smart PN");
            Globals.machine.PartNumber = dr2.Rows[0].Field<string>("Base Part Number");
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
            component c = new component();
            c.desc = dr2.Rows[0].Field<string>("Description");
            c.number = dr2.Rows[0].Field<string>("Base Part Number");
            c.qty = 1;
            c.mrpType = "M";
            c.partType = DivisionCombo.Text;
            c.revision = dr2.Rows[0].Field<string>("Revision"); 
            c.epicorRevDescription = "REVISION";
            Globals.machine.machComp = c;
            return;
        }
        private void addOpt(string selVal, string cName)
        {
            DataRow[] drs = Globals.cmdOptComp.Select("Name = '" + selVal + "'");
            if (drs.Count() == 0)
            {
                return;
            }
            int RowIndex = 0;
            if (drs.Count() >1)
            {
                for(int i = 0; i < drs.Count();++i)
                {
                    if (Globals.machine.snList.Contains(drs[i][5]) == true)
                    {
                        RowIndex = i;
                    }
                }
            }
            //Condense option rows across machine lines
            //for (int i = 1; i < drs.Count();++i)
            //{
            //    if (drs[i][1].ToString() == drs[0][1].ToString())
            //    {
            //        for(int j = 3; j < drs[0].ItemArray.Count();++j)
            //        {
            //            if (string.IsNullOrEmpty(drs[0][j].ToString()) == true)
            //            {
            //                drs[0][j] = drs[i][j];
            //            }
            //        }
            //    }
            //}
            //Condense option rows across machine lines

            DataRow[] drs2 = new DataRow[] { drs[RowIndex] };

            DataTable dr2 = drs2.CopyToDataTable();
            if (Globals.machine.snList.Contains(dr2.Rows[0].Field<string>("Smart Designator")) == false)
            {
                option opt = new option();
                opt.optType = dr2.Rows[0].Field<string>("Type");
                opt.optName = dr2.Rows[0].Field<string>("Name");
                opt.optSnDes = dr2.Rows[0].Field<string>("Smart Designator");
                opt.checkName = dr2.Rows[0].Field<string>("OptionChecklist");
                opt.optDesc = dr2.Rows[0].Field<string>("Short Description");
                opt.optFinDesc = opt.optDesc;
                opt.optFinSn = opt.optSnDes;

                //This needs to change to accomodate the numeric selector
                opt.optQty = 1;
                //This needs to change to accomodate the numeric selector

                if (selVal.ToUpper() != "NONE")
                {
                    opt.optComps.AddRange(getCompData(dr2.Rows[0], opt.optQty));
                    if (dr2.Rows[0].Field<string>("OptionReqs") != null)
                    {
                        try
                        {
                            opt.optReqs.AddRange(dr2.Rows[0].Field<string>("OptionReqs").Split(','));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
                //This section checks for and removes an option if it was already selected.
                if (string.IsNullOrEmpty(opt.optSnDes) == false)
                {
                    Globals.machine.snList.Add(opt.optSnDes);
                }
                Globals.machine.selOpts.Add(opt);
            }
            return;
        }
        private void addLabel(string name)
        {
            Label lbl = new Label();
            lbl.Text = Globals.utils.AddSpacesToSentence(name.Replace("Combo", ""));
            lbl.Size = new System.Drawing.Size(200, 15);
            lbl.Name = name.Replace("Combo", "Label");
            lbl.Location = new System.Drawing.Point(12, cBoxH - 15);
            lbls.Add(lbl);
            this.Controls.Add(lbl);
            lbl.Show();
            addedControls.Add(lbl);
        }
        private ComboBox addCombo(string name)
        {
            ComboBox cb = new ComboBox();
            return cb;
        }
        private ListBox addList(string name)
        {
            ListBox cb = new ListBox();
            cb.SelectionMode = SelectionMode.MultiSimple;
            cb.SelectedItem = "";
            return cb;
        }
        private CheckedListBox AddCheckListBox(string name)
        {
            CheckedListBox checkedListBox = new CheckedListBox();
            checkedListBox.SelectionMode = SelectionMode.MultiSimple;
            return checkedListBox;
        }

        private NumericUpDown addNumeric(string name, int mQty)
        {
            NumericUpDown cb = new NumericUpDown();
            cb.Maximum = mQty;
            cb.Minimum = 1;
            return cb;
        }
        private Tuple<DataTable, string, string> nextOption(string optName, string selVal)
        {
            //optName should be the name of the current option box that fired off the event in selecChange()
            //Going to handle determining which DB I need to use in here, this should help clean up the selecChange() some
            //As well as improving my accuracy.
            DataTable dt = new DataTable();
            string nextOpt = "";
            string parCol = "";
            Tuple<DataTable, string, string> opt = new Tuple<DataTable, string, string>(dt, nextOpt, "MachineData");
            if (optName == null)
            {
                return new Tuple<DataTable, string, string>(dt, "", optDB); ;
            }
            bool machDB = false;
            bool optFound = false;
            if (Globals.machineData.Columns.Contains(optName))
            {
                //If the current option name is in the MachineData DB;
                dt = Globals.machineData;
                machDB = true;
                if (Globals.utils.isOptName(dt.Columns[dt.Columns[optName].Ordinal + 1].ColumnName) == true)
                {
                    //If the next Machine DB header is an option Name
                    opt = new Tuple<DataTable, string, string>(dt, dt.Columns[dt.Columns[optName].Ordinal + 1].ColumnName, "MachineData");
                }
                else
                {
                    machDB = false;
                }
            }

            if (machDB == false)
            {
                dt = Globals.cmdOptComp;
                int i;
                //int i;
                //if (Globals.MachineOptComp.Rows.Count < 1)
                //{
                //    dt = Globals.cmdOptComp.Select('[' + Globals.machine.machName + ']' + " <> ''").CopyToDataTable();
                //    //dt.DefaultView.Sort = "[Type]";
                //    //dt = dt.DefaultView.ToTable();
                //    //int optRow = 0;
                //    //string optType = "";
                //    //List<string> done = new List<string>();
                //    for (i = 1; i < dt.Rows.Count; ++i)
                //    {
                //        //DataRow row = dt.Rows[i];
                //        //if(optRow == 1 || row[1].ToString() != optType)
                //        //{
                //        //    optRow = i;
                //        //    optType = row[1].ToString();
                //        //}
                //        ////Determine if requirements and options are out of order from each other
                //        //DataRow[] ReqRows = dt.Select("[OptionReqs] = '" +  row[2].ToString() + "'");
                //        //if((ReqRows != null && ReqRows.Count() > 0)) 
                //        //{
                //        //    if (row[0].ToString() == "PlasmaTypeCombo")
                //        //    {
                //        //        string ring = "";
                //        //    }
                //        //    DataRow[] MoveRows = dt.Select("[Type] = '" + ReqRows[0][0].ToString() + "'");
                //        //    DataRow[] SwapRows = dt.Select("[Type] = '" + row[0].ToString() + "'");
                //        //    for (int k = 0; k < MoveRows.Count(); ++k)
                //        //    {
                //        //        int LowRow = dt.Rows.IndexOf(SwapRows[SwapRows.Count() - 1]) + 1;
                //        //        DataRow Row = dt.NewRow();
                //        //        object[] RowData = MoveRows[k].ItemArray;
                //        //        Row.ItemArray = RowData;
                //        //        dt.Rows.RemoveAt(dt.Rows.IndexOf(MoveRows[k]));
                //        //        dt.Rows.InsertAt(Row, LowRow +2);
                //        //    }

                //        //}

                //    }

                //    Globals.MachineOptComp = dt;
                //}
                //else
                //{
                //    dt = Globals.MachineOptComp;
                //}
                i = 0;
                try
                {
                    while (i < dt.Rows.Count && optFound == false)
                    {
                        if (dt.Rows[i].Field<string>(1) == optName)
                        {
                            while (dt.Rows[i].Field<string>(1) == optName && optFound == false)
                            {
                                if (i + 1 < dt.Rows.Count && dt.Rows[i + 1][1].ToString() != optName)//&& dt.Rows[i + 1][0] != DBNull.Value)
                                {
                                    string ring = dt.Rows[i + 1][1].ToString();
                                    opt = new Tuple<DataTable, string, string>(dt, dt.Rows[i + 1].Field<string>(1), optDB);
                                    parCol = "Type";
                                    optFound = true;
                                }
                                if (i + 1 == dt.Rows.Count)
                                {
                                    return new Tuple<DataTable, string, string>(dt, "", optDB); ;
                                }
                                ++i;
                            }
                        }
                        ++i;
                    }
                }
                catch { }
                if (optFound == false)
                {
                    opt = new Tuple<DataTable, string, string>(dt, dt.Rows[0][1].ToString(), optDB);
                }
            }

            if (opt.Item3 == optDB)
            {
                DataTable dt2 = Globals.utils.getDT2(dt, "Name", parCol, opt.Item2);
                if (dt2.Rows.Count == 0 || Globals.utils.validOpt("Type", opt.Item2) == false)
                {
                    if (runTypes.Contains(opt.Item2) == false)
                    {
                        runTypes.Add(opt.Item2);
                        opt = nextOption(opt.Item2, selVal);
                    }
                    else
                    {
                        opt = null;
                    }
                }
            }
            return opt;
        }
        private Control nextCont(string optName, int mQty, string selVal, string parName)
        {
            int newCBoxH = 0;
            Control co = new Control();
            cntrlWidth = 288;
            if (optName != "")
            {
                Control[] found = this.Controls.Find(optName, false);
                if (found.Count() == 0)
                {
                    string[] contTypes = { "Combo", "List", "Numeric" };
                    switch (contTypes.FirstOrDefault<string>(s => optName.Contains(s)))
                    {
                        case "Combo":
                            co = (ComboBox)addCombo(optName);
                            cntrlHt = 21;
                            newCBoxH = cBoxH + 40;
                            break;
                        case "List":
                            co = (ListBox)addList(optName);
                            cntrlHt = 80;
                            newCBoxH = cBoxH + 90;
                            break;
                        case "Numeric":
                            co = (NumericUpDown)addNumeric(optName, mQty);
                            co.AccessibleName = parName;
                            optName = optName.Replace("Numeric", "Quantity");
                            cntrlHt = 21;
                            cntrlWidth = 128;
                            newCBoxH = cBoxH + 40;
                            break;
                        default:
                            //Do work
                            break;
                    }
                    co.Size = new System.Drawing.Size(cntrlWidth, cntrlHt);
                    co.Location = new System.Drawing.Point(12, cBoxH);
                    co.Name = optName;
                    co.TabIndex = 0;
                    optBoxes.Add(optName);
                    addLabel(optName);
                    this.Controls.Add(co);
                    Globals.utils.initSelChange(this.Controls, selecChange);
                    co.Show();
                    this.Refresh();
                    cBoxH = newCBoxH;
                    addedControls.Add(co);
                }
                else
                {
                    co = found[0];
                }
            }

            return co;
        }
        private component[] getCompData(DataRow dr, int optQty)
        {
            List<component> comps = new List<component>();
            string[] compList = dr.Field<string>(Globals.machine.machName).Split(',');
            DataTable dt = Globals.compData;
            string comp = "";
            foreach (string lComp in compList)
            {
                comp = lComp.Replace(" ", "");
                if (comp.Contains("]"))
                {
                    comp = comp.Split(']')[1];
                }
                if (comp.Contains("}"))
                {
                    comp = comp.Split('}')[1];
                }
                if (comp.ToUpper() != "X")
                {
                    DataRow dr2 = null;
                    DataRow[] drA;
                    component c = new component();
                    try
                    {
                        drA = dt.Select("[Part Number] = '" + comp + "'");
                        dr2 = drA[0];
                        c.addType = dr2.Field<string>("Add Type");
                        if (string.IsNullOrWhiteSpace(dr2.Field<string>("Max Qty")) == false)
                        {
                            c.maxQty = Convert.ToInt32(dr2.Field<string>("Max Qty"));
                        }
                        if (string.IsNullOrWhiteSpace(dr2.Field<string>("Typ Qty")) == false)
                        {
                            c.typQty = Convert.ToInt32(dr2.Field<string>("Typ Qty"));
                        }
                        c.mrpType = dr2.Field<string>("MRP Type");
                        c.number = dr2.Field<string>("Part Number");
                        c.epicorRevDescription = "REVISION";
                        c.partType = dr2.Field<string>("Part Type");
                    }
                    catch (Exception e)
                    {
                        c.maxQty = 1;
                        c.typQty = 1;
                        c.addType = "$";
                        c.desc = "$";
                        c.mrpType = "$";
                        c.number = comp;
                        c.epicorRevDescription = "REVISION";
                        c.partType = "$";
                        // MessageBox.Show("Error getting component data. Component " + comp + ", may not be entered in Configurator Component Database! \n" + e.ToString());
                    }

                    //c.revision = dr2.Field<string>("Revision");



                    //if(c.revision == "" || c.revision == null)
                    //{
                    //    c.revision = "-";
                    //}
                    if (c.typQty * optQty <= c.maxQty)
                    {
                        c.qty = c.typQty * optQty;
                    }
                    else
                    {
                        c.qty = c.typQty;
                    }
                    comps.Add(c);
                }
            }
            return comps.ToArray();
        }
        private bool updateConfigurator()
        {
            string version1 = AssemblyName.GetAssemblyName(@"W:\Engineering\Machine Configurator\Machine Configurator.exe").Version.ToString();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string version2 = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Version client = new Version(version2);
            Version server = new Version(version1);
            if (server <= client)
            {
                MessageBox.Show("Latest Version already installed");
                return false;
            }
            ProcessStartInfo Info = new ProcessStartInfo(@"W:\Engineering\Apps - Calculators\Koike Update.exe");
            Info.Arguments = "Configurator 1";
            //Info.UseShellExecute = true;
            Process.Start(Info);

            Application.Exit();
            return true;
        }
        private void findDumNum()
        {
            string dNum = "";
            string prefix = Globals.machine.prefix;
            string suffix = "";
            DataTable dt = Globals.confMachs;
            for (int i = 1; i < 10000; ++i)
            {
                suffix = i.ToString();
                suffix = suffix.PadLeft(4, '0');
                dNum = prefix + suffix;
                if (Globals.confMachs != null)
                {
                    DataRow[] dr = Globals.confMachs.Select("[Epicor Part Number] = '" + dNum + "'");
                    if (dr.Count() <= 0)
                    {
                        Globals.machine.EpicorPartNumber = dNum;
                        return;
                    }
                }
                else
                {
                    Globals.machine.EpicorPartNumber = dNum;
                    return;
                }
            }
            return;
        }
        private void exportBOM()
        {
            string[,] expBOM = new string[Globals.machine.bom.Rows.Count + 2, Globals.expRows];
            int j = 0;
            for (j = 0; j < Globals.expRows; ++j)
            {
                expBOM[0, j] = j.ToString();
            }
            string pType = "POS Machines";
            if (Globals.machine.partType == "Cutting Machine")
            {
                pType = "CM";
            }
            //Number PartDescription Epicor_Mfgcomment Epicor_Purcomment   Epicor_Mfg_name Epicor_MFGPartNum   Epicor_RandD_c Epicor_createdbylegacy_c    Epicor_PartType_c Epicor_EngComment_c Epicor_Confreq_c Epicor_EA_Manf_c    Epicor_EA_Volts_c Epicor_EA_Phase_c   Epicor_EA_Freq_c Epicor_EA_FLA_Supply_c  Epicor_EA_FLA_LgMot_c Epicor_EA_ProtDevRating_c   Epicor_EA_PannelSCCR_c Epicor_EA_EncRating_c   Revision Epicor_RevisionDescription  Dwg.Rev.Epicor_FullRel_c Reference Count PartRev.DrawNum_c Part.Model_c PartTypeElectrical  PartRev.DrawSize_c PartRev.SheetCount_c
            string[] mRow = (Globals.machine.EpicorPartNumber + "," + Globals.machine.description.Replace(',', ' ') + ",,,KOIKE," + Globals.machine.SmartPartNumber +  ",False," + userBox.Text.ToUpper() + "," + pType + ",,False, , , , , , , , , ,-,New Machine," + Globals.machine.dwgRev + ",1,1," + Globals.machine.drawingName + "," + Globals.machine.machCode + ",FALSE," + Globals.machine.drawingSize + ",").Split(',');

            //string[] mRow = (Globals.machine.dumNum + "," + Globals.machine.desc.Replace(',', ' ') + ",,,KOIKE,,False," + userBox.Text.ToUpper() +"," + Globals.machine.partType + ",,False, , , , , , , , , ," + Globals.machine.dumNum + ",A," + "New Machine" + "," + Globals.machine.dwgRev + ",1," + Globals.machine.dumNum + ",A,1,,,FALSE,,FALSE," + Globals.machine.dwgName + "," + Globals.machine.dwgSize + ",,").Split(',');
            int i = 0;
            foreach (string s in mRow)
            {
                expBOM[1, i] = s;
                ++i;
            }
            j = 2;
            string manu = "";
            string purch = "True";
            foreach (component c in Globals.machine.bomComps)
            {
                if (c.mrpType == "M")
                {
                    manu = "KOIKE";
                    purch = "False";
                }
                if(string.IsNullOrEmpty(c.revision)== true)
                {
                    c.revision = "-";
                }
                //Number PartDescription Epicor_Mfgcomment Epicor_Purcomment   Epicor_Mfg_name Epicor_MFGPartNum   Epicor_RandD_c Epicor_createdbylegacy_c    Epicor_PartType_c Epicor_EngComment_c Epicor_Confreq_c Epicor_EA_Manf_c    Epicor_EA_Volts_c Epicor_EA_Phase_c   Epicor_EA_Freq_c Epicor_EA_FLA_Supply_c  Epicor_EA_FLA_LgMot_c Epicor_EA_ProtDevRating_c   Epicor_EA_PannelSCCR_c Epicor_EA_EncRating_c   Revision Epicor_RevisionDescription  Dwg.Rev.Epicor_FullRel_c Reference Count PartRev.DrawNum_c Part.Model_c PartTypeElectrical  PartRev.DrawSize_c PartRev.SheetCount_c
                string[] row = (c.number + "%" + c.desc + "%%%" + manu + "%%False%%" + c.partType + "%%False% % % % % % % % % %" + c.revision + "%" + c.epicorRevDescription + "%" + c.epicorDrawRev + "%1%" + c.qty.ToString() + "%" + c.epicorDrawNum + "%%TRUE%" + c.epicorDrawSize + "%" + c.epicorDrawSheetCount).Split('%');
                //string[] row = (c.number + "," + c.desc.Replace(',', ' ') + ",,," + manu + ",,False,," + c.partType + ",,False, , , , , , , , , ," + c.number + "," + c.revision + "," + c.desc.Replace(',', ' ') + "," + c.revision + ",1," + Globals.machine.dumNum + ",A," + c.qty.ToString() + ",," + c.number + ",TRUE," + c.partClass + "," + purch + "," + c.number + ",,,").Split(',');
                i = 0;
                foreach (string s in row)
                {
                    expBOM[j, i] = s;
                    ++i;
                }
                ++j;
            }
            if (epicorExportCheckbox.Checked == true && Globals.prevConf == false)
            {
                Globals.utils.writeExcel(expBOM, @"\\manifest\BOM_Import\Mechanical\Import\" + Globals.machine.EpicorPartNumber + "_BOM_0000.xlsx", "EpdmBOMTable", Globals.machine.bom.Rows.Count + 2, Globals.expRows, -1, 1, "");
            }
            if (testExportCheckbox.Checked == true)
            {
                Globals.prevConf = false;
                Globals.utils.writeExcel(expBOM, @"C:\Temp\BOM\" + Globals.machine.EpicorPartNumber + "_BOM_0000.xlsx", "EpdmBOMTable", Globals.machine.bom.Rows.Count + 2, Globals.expRows, -1, 1, "");
            }
        }
        private void updateConfButt_Click(object sender, EventArgs e)
        {
            updateConfigurator();
        }

        private void Configurator_FormClosing(object sender, FormClosingEventArgs e)
        {
            ProcessStartInfo Info = new ProcessStartInfo(@"W:\Engineering\Apps - Calculators\Koike Update.exe");
            Info.Arguments = "Machine Configurator 0";
            Process.Start(Info);
        }

        private void addMachineToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void addOptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void addComponentToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void checkDBsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals.utils.checkDB();
        }

        private void refreshDBsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Globals.utils.updateDBs();
        }

        private void featureRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://forms.clickup.com/f/85n7e-434/7LVVFXREY1COZ692U6");
        }

        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://app.clickup.com/8574190/v/l/li/82070636?pr=14768824");
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            helpForm help = new helpForm();
            help.Show();
        }

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string version1 = AssemblyName.GetAssemblyName(@"W:\Engineering\Machine Configurator\Machine Configurator.exe").Version.ToString();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string version2 = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Version client = new Version(version2);
            Version server = new Version(version1);
            if (server <= client)
            {
                MessageBox.Show("Latest Version already installed");
                return;
            }
            ProcessStartInfo Info = new ProcessStartInfo(@"W:\Engineering\Apps - Calculators\Koike Update.exe");
            Info.Arguments = "Configurator 1";
            //Info.UseShellExecute = true;
            Process.Start(Info);

            Application.Exit();
        }

        private void checkConfigurationButton_Click(object sender, EventArgs e)
        {
            timesConfBox.BackColor = System.Drawing.Color.Red;
            confNumBox.BackColor = System.Drawing.Color.Red;
            dataGridView1.DataSource = null;
            if (Globals.machine.selOpts.Count < 1 && optBoxes.Count > baseOpts.Count)
            {
                MessageBox.Show("No machine options have been selected yet.");
                return;
            }
            Globals.confMachs = Globals.dataBase.Tables[Globals.machine.prefix + " CONF"];

            string smartNum = Globals.machine.snList[0];// + "-";
            string smartStart = Globals.machine.snList[0];
            string[] derp = Globals.machine.snList.ToArray();

            List<string> sortedSNs = new List<string>();
            sortedSNs.AddRange(Globals.machine.snList.ToArray());
            sortedSNs.Sort();
            sortedSNs.RemoveAll(item => item == null);
            sortedSNs.RemoveAll(item => item == "");
            int j = 0;
            foreach (string i in sortedSNs)
            {
                if (i != smartStart &&  String.IsNullOrWhiteSpace(i) == false)
                {
                    smartNum = smartNum + "-" + i;
                }
            }
            findDumNum();
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
                    {
                        if (Globals.confMachs.Rows[j][0] != DBNull.Value)// && (string)Globals.confMachs.Rows[j][0] == smartNum)
                        {
                            List<string> confNum = new List<string>();
                            confNum.AddRange(Globals.confMachs.Rows[j][0].ToString().Split('-'));
                            confNum.Sort();
                            if (confNum.SequenceEqual(sortedSNs))
                            {
                                Globals.machine.EpicorPartNumber = dr[0].Field<string>("Epicor Part Number");
                                timesConfBox.Text = Convert.ToInt32(dr[0].Field<string>("Times Configured")).ToString();
                                Globals.machine.configuredDate = dr[0].Field<string>("Date Configured").ToString();
                                Globals.machine.configuredBy = dr[0].Field<string>("User Added").ToString();
                                var sales_orders = dr[0].Field<string>("Sales Orders").ToString().Replace("[", "").Replace("]", "").Replace(" ", "").Split(',');
                                timesConfBox.BackColor = System.Drawing.Color.LightGreen;
                                confNumBox.Text = Globals.machine.EpicorPartNumber;
                                confNumBox.BackColor = System.Drawing.Color.LightGreen;
                                Globals.machine.salesOrders = sales_orders.ToList();
                                Globals.foundRow = j;
                                Globals.prevConf = true;
                                break;
                            }
                        }
                    }
                }
            }

            Globals.machine.SmartPartNumber = smartNum;
            Globals.machine.timesConfigured = Convert.ToInt32(timesConfBox.Text);
            Globals.machine.configuredDate = DateTime.Now.ToShortDateString();
            Globals.machine.configuredBy = Environment.UserName;
            Globals.utils.writeMachine();
            DataTable dt = Globals.machine.bom;
            dataGridView1.DataSource = dt;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Columns[0].Width = 175;
            dataGridView1.Columns[1].Width = 350;
            dataGridView1.Columns[2].Width = 50;
            dataGridView1.Columns[3].Width = 50;
            for (int k = 0; k < dataGridView1.Columns.Count; ++k)
            {
                dataGridView1.Columns.Remove(dataGridView1.Columns[4]);
            }
            checkConfigurationButton.BackColor = Color.LawnGreen;
        }

        private void completeConfigurationButton_Click(object sender, EventArgs e)
        {
            if (userBox.Text == "" || userBox.Text == null)
            {
                MessageBox.Show("No User initials entered!!!! Please enter your three character User Initials and try again.", "Can I See Your ID Error");
                return;
            }
            if (configurationComplete == false)
            {
                createFolder();
                configurationComplete = true;
                Globals.machine.salesOrders.Add(soBox.Text);
                Globals.utils.WriteMachineToDatabase(Globals.machine);

                if (epicorExportCheckbox.Checked == true || testExportCheckbox.Checked == true)
                {
                    Task.Run(() => exportBOM());
                    //MessageBox.Show("BOM Exported for Epicor.");
                }
                MessageBox.Show("Configuration Complete");
                completeConfigurationButton.BackColor = Color.LawnGreen;
            }
        }
        private void createFolder()
        {
            string smartPN = Globals.machine.SmartPartNumber.Split('-')[0];
            string folPath = "";
            EdmVault5 vault = new EdmVault5();
            try
            {
                vault.LoginAuto("EPDM", 0);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
            IEdmFolder9 folder = (IEdmFolder9)vault.RootFolder;
            switch(smartPN)
            {
                case "SP44":
                    folPath = @"\CUTTING MACHINES\SHOP-PRO\4X4";
                    break;
                case "SP48":
                    folPath = @"\CUTTING MACHINES\SHOP-PRO\4X8";
                    break;
                case "SP51":
                    folPath = @"\CUTTING MACHINES\SHOP-PRO\5x10";
                    break;
                case "CII10":
                    folPath = @"\POSITIONERS\MANIPULATORS 014-614\CRICKET-II\10X10";
                    break;
                case "CII66":
                    folPath = @"\POSITIONERS\MANIPULATORS 014-614\CRICKET-II\6X6";
                    break;
                case "CII88":
                    folPath = @"\POSITIONERS\MANIPULATORS 014-614\CRICKET-II\8X8";
                    break;
                case "CII12":
                    folPath = @"\POSITIONERS\MANIPULATORS 014-614\CRICKET-II\12X12";
                    break;
                case "CI66":
                    folPath = @"\POSITIONERS\MANIPULATORS 014-614\CRICKET";
                    break;
                case "MRD5":
                    folPath = @"\POSITIONERS\TURN ROLLS - 013-613\MRD_MRI";
                    break;
                case "MRD10":
                    folPath = @"\POSITIONERS\TURN ROLLS - 013-613\MRD_MRI";
                    break;
                case "MRD20":
                    folPath = @"\POSITIONERS\TURN ROLLS - 013-613\MRD_MRI";
                    break;
                case "MD15":
                    folPath = @"\POSITIONERS\G-DRIVEN 007-607-630\MD\MD15";
                    break;
                case "MD30":
                    folPath = @"\POSITIONERS\G-DRIVEN 007-607-630\MD\MD30";
                    break;
                case "MD50":
                    folPath = @"\POSITIONERS\G-DRIVEN 007-607-630\MD\MD50";
                    break;
                case "PRD3":
                    folPath = @"\POSITIONERS\TURN ROLLS - 013-613\PRD_PRI";
                    break;
                case "HTX10":
                    folPath = @"\POSITIONERS\HTS  009-609\MDHTS10";
                    break;
                case "LD4":
                    folPath = @"\POSITIONERS\LD  030\LD 4";
                    break;
                case "SPXHD2612":
                    folPath = @"\CUTTING MACHINES\SHOP-PRO HD\6X12";
                    break;
                case "SPXHD510":
                    folPath = @"\CUTTING MACHINES\SHOP-PRO HD\5X10";
                    break;
                case "HTX5":
                    folPath = @"\POSITIONERS\HTS  009-609\MDHTS5";
                    break;
                case "HTX15":
                    folPath = @"\POSITIONERS\HTS  009-609\MDHTS15";
                    break;
                default:
                    MessageBox.Show("Folder creation error. Please manually create the proper folder in EPDM");
                    break;
            }
            folPath = folPath + "\\" + Globals.machine.soNum;
            try
            {
                IEdmFolder5 fol2 = folder.CreateFolderPath(folPath, this.Handle.ToInt32());
            }
            catch
            {
                MessageBox.Show("Folder creation error. Please manually create the proper folder in EPDM");
            }
            return;
        }
        private void clearConfigButton_Click(object sender, EventArgs e)
        {
            clearAndResetForm();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void viewDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            databaseViewer dbView = new databaseViewer();
            dbView.Show();
        }

        private void addEditOptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addEditOptionForm edit = new addEditOptionForm();
            edit.Show();
        }
    }
}