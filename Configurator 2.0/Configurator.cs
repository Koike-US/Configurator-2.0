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

namespace Configurator_2._0
{
    public partial class Configurator : Form
    {
        List<string> baseOpts = new List<string>();
        List<string> optBoxes = new List<string>();

        List<ListBox> lBoxes = new List<ListBox>();
        List<Label> lbls = new List<Label>();
        int iMax = 0;
        int cBoxH = 224;
        int initOpts = 0;
        int cntrlHt = 0;
        int cntrlWidth = 0;
        int[] maxOptQty;
        public Configurator()
        {
            Globals.utils.updateDBs();

            InitializeComponent();
            baseOpts.AddRange(Globals.machineData.Columns.Cast<DataColumn>().Select(x => x.ColumnName).Where(x => x.Contains("Combo")).ToArray());
            optBoxes.AddRange(baseOpts.ToArray()); 
            initOpts = baseOpts.Count() - 1;
            Globals.utils.initSelChange(this.Controls,selecChange);
            iMax = Globals.cmdOptComp.Rows.Count;
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            this.Text = String.Format(this.Text, version.Major, version.Minor, version.Build, version.Revision);
            this.Refresh();
            
            string version1 = AssemblyName.GetAssemblyName(@"W:\Engineering\Machine Configurator\Machine Configurator.exe").Version.ToString();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string version2 = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Version client = new Version(version2);
            Version server = new Version(version1);
            if (server > client)
            {
                MessageBox.Show("Configurator Update is required!! Please update to ensure correct operation!");
                updateConfButt.BackColor = Color.Red;
                updateConfButt.Text = "Update Required!";
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
                    if(cb.Count == 0)
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
        private void clearSels(string cur)
        {
            Task.Run(() => clearCombos("DivisionCombo"));
            soBox.Text = "";
            prevSoCombo.Items.Clear();
            dwgNumBox.Text = "";
            dwgSizeBox.Text = "";
            soBox.Text = "";
            timesConfBox.Text = "";
            userBox.Text = "";
            timesConfBox.BackColor = System.Drawing.Color.White;
            confNumBox.Text = "";
            confNumBox.BackColor = System.Drawing.Color.White;
            confButt.BackColor = SystemColors.Control;
            exportButt.BackColor = SystemColors.Control;
            cBoxH = 220;
            dataGridView1.DataSource = null;
            Task.Run(() => Globals.utils.updateDBs());
            Globals.machine = new MachineData();
            Globals.prevConf = false;
            Globals.utils.popItem(DivisionCombo, Globals.machineData, Globals.machineData.Columns[0].ColumnName, "", "");
            return;
        }
        private void selecChange(object sender, EventArgs e)
        {
            confButt.BackColor = SystemColors.Control;
            exportButt.BackColor = SystemColors.Control;
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
                foreach(option o in Globals.machine.selOpts)
                {
                    if(o.optType == nb.AccessibleName)
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
                if(maxOptQty != null && maxOptQty.Count() > 0)
                {
                    int maxQ = maxOptQty[cb.SelectedIndex];
                    nextCont(cb.Name.Replace("Combo","Numeric"), maxQ,selVal[0],"");
                }
            }
            if(co is ListBox)
            {
                lb = (ListBox)co;
                int lastSelectedIndex = (int)typeof(ListBox).GetProperty("FocusedIndex", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(lb, null);
                string sel = lb.Items[lastSelectedIndex].ToString();
                if (sel.ToUpper() == "NONE"|| sel.ToUpper().Contains("STANDARD"))
                {
                    for(int k = 0; k <lb.SelectedItems.Count;++k)
                    {
                        string item = lb.SelectedItems[k].ToString();
                        if(item.ToUpper() != "NONE" && sel.ToUpper().Contains("STANDARD") == false)
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
                    nextCont(lb.Name.Replace("List", "Numeric"), maxQ, selVal[0],lb.Name);
                }
            }
            if (selVal.Count == 0 || selVal[0] == "")
            {
                return;
            }
      next_opt:
            Tuple<DataTable, string, string> tup = nextOption(curOpt);
            dtNext = tup.Item1;
            nextOpt = tup.Item2;
            dbName = tup.Item3;
            coNext = nextCont(nextOpt,0,"","");

            if (dbName == "OptionCompatability")
            {
                if (co.Name != "ModelCombo" && Globals.machine.machineName != null)
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
                if (nextOpt == "")
                {
                    return;
                }
                selVal[0] = nextOpt;
                nextOpt = "Name";
                curOpt = "Type";
            }

            maxOptQty = Globals.utils.popItem(coNext, dtNext, nextOpt, curOpt, selVal[0]);
            int coIC = 0;
            if(coNext is ComboBox)
            {
                ComboBox cbt = (ComboBox)coNext;
                coIC = cbt.Items.Count;
            }
            if (coNext is ListBox)
            {
                ListBox cbt = (ListBox)coNext;
                coIC = cbt.Items.Count;
            }
            if(coIC == 0)
            {
                coNext.Text = "Not Available";
                curOpt = selVal[0];                
                goto next_opt;
            }


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
            Globals.machine.machineName = selVal;
            Globals.machine.partType = DivisionCombo.Text;
            Globals.machine.desc = dr2.Rows[0].Field<string>("ModelCombo") + ", ";
            Globals.machine.snList[0] =dr2.Rows[0].Field<string>("Smart PN");
            Globals.machine.soNum = soBox.Text;
            component c = new component();
            c.desc = dr2.Rows[0].Field<string>("Description") ;
            c.number = dr2.Rows[0].Field<string>("Base Part Number");
            c.qty = 1;
            c.mrpType = "M";
            c.partType = DivisionCombo.Text;
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
            DataTable dr2 = drs.CopyToDataTable();
            if (dr2.Rows[0].Field<string>("Smart Designator") == null || Globals.machine.snList.Contains(dr2.Rows[0].Field<string>("Smart Designator")) == false)
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
                Globals.machine.snList.Add(opt.optSnDes);
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
            lbl.Location = new System.Drawing.Point(307, cBoxH);
            lbls.Add(lbl);
            this.Controls.Add(lbl);
            lbl.Show();
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
        private NumericUpDown addNumeric(string name, int mQty)
        {
            NumericUpDown cb = new NumericUpDown();
            cb.Maximum = mQty;
            cb.Minimum = 1;
            return cb;
        }
        private Tuple<DataTable,string,string> nextOption(string optName)
        {
            //optName should be the name of the current option box that fired off the event in selecChange()
            //Going to handle determining which DB I need to use in here, this should help clean up the selecChange() some
            //As well as improving my accuracy.
            DataTable dt = new DataTable();
            string nextOpt = "";
            Tuple<DataTable, string,string> opt = new Tuple<DataTable, string,string>(dt,nextOpt,"MachineData");
            if (optName == null)
            {
                return new Tuple<DataTable, string, string>(dt, "", "OptionCompatability"); ;
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
                    opt = new Tuple<DataTable, string,string>(dt, dt.Columns[dt.Columns[optName].Ordinal + 1].ColumnName, "MachineData");
                }
                else
                {
                    machDB = false;
                }
            }

            if (machDB == false)
            {
                dt = Globals.cmdOptComp;
                int i = 0;
                try
                {
                    while (i < dt.Rows.Count && optFound == false)
                    {
                        if (dt.Rows[i].Field<string>(0) == optName)
                        {
                            while (dt.Rows[i].Field<string>(0) == optName && optFound == false)
                            {
                                if (i + 1 < dt.Rows.Count && dt.Rows[i+1][0].ToString() != optName)//&& dt.Rows[i + 1][0] != DBNull.Value)
                                {
                                    opt = new Tuple<DataTable, string, string>(dt, dt.Rows[i + 1].Field<string>(0), "OptionCompatability");
                                    optFound = true;
                                }
                                if (i + 1 == dt.Rows.Count)
                                {
                                    return new Tuple<DataTable, string, string>(dt, "", "OptionCompatability"); ;
                                }
                               ++i;
                            }
                        }
                        ++i;
                    }
                }
                catch { }
                if(optFound == false)
                {
                    opt = new Tuple<DataTable, string,string>(dt, "TableCombo", "OptionCompatability");
                }
            }


            if (opt.Item3 == "OptionCompatability" && Globals.utils.validOpt("Type", opt.Item2) == false)
            {
                opt = nextOption(opt.Item2);
            }
            return opt;
        }
        private Control nextCont(string optName, int mQty, string selVal,string parName)
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
                            newCBoxH = cBoxH + +27;
                            break;
                        case "List":
                            co = (ListBox)addList(optName);
                            cntrlHt = 128;
                            newCBoxH = cBoxH + co.Height + 27;
                            break;
                        case "Numeric":
                            co = (NumericUpDown)addNumeric(optName, mQty);
                            co.AccessibleName = parName;
                            optName = optName.Replace("Numeric", "Quantity");
                            cntrlHt = 21;
                            cntrlWidth = 128;
                            newCBoxH = cBoxH + +27;
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
            string[] compList = dr.Field<string>(Globals.machine.machineName).Split(',');
            DataTable dt = Globals.compData;
            string comp = "";
            foreach(string lComp in compList)
            {
                comp = lComp.Replace(" ","");
                if(comp.Contains("]"))
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
                    }
                    catch(Exception e)
                    {                        
                        MessageBox.Show("Error getting component data. Component " + comp + ", may not be entered in Configurator Component Database! \n" + e.ToString());
                    }
                    c.addType = dr2.Field<string>("Add Type");
                    c.desc = dr2.Field<string>("Part Description");
                    if (string.IsNullOrWhiteSpace(dr2.Field<string>("Max Qty")) == false)
                    {
                        c.maxQty = Convert.ToInt32(dr2.Field<string>("Max Qty"));
                    }
                    c.mrpType = dr2.Field<string>("MRP Type");
                    c.number = dr2.Field<string>("Part Number");
                    c.typQty = Convert.ToInt32(dr2.Field<string>("Typ Qty"));
                    c.partType = dr2.Field<string>("Part Type");
                    c.revision = dr2.Field<string>("Revision");
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
        private void findDumNum()
        {
            string dNum = "";
            string prefix = Globals.machine.prefix;
            string suffix = "";
            DataTable dt = Globals.confMachs;
            for(int i = 1; i < 10000; ++ i)
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
            if(Globals.machine.partType == "Cutting Machine")
            {
                pType = "CM";
            }
            //Number PartDescription Epicor_Mfgcomment Epicor_Purcomment   Epicor_Mfg_name Epicor_MFGPartNum   Epicor_RandD_c Epicor_createdbylegacy_c    Epicor_PartType_c Epicor_EngComment_c Epicor_Confreq_c Epicor_EA_Manf_c    Epicor_EA_Volts_c Epicor_EA_Phase_c   Epicor_EA_Freq_c Epicor_EA_FLA_Supply_c  Epicor_EA_FLA_LgMot_c Epicor_EA_ProtDevRating_c   Epicor_EA_PannelSCCR_c Epicor_EA_EncRating_c   Revision Epicor_RevisionDescription  Dwg.Rev.Epicor_FullRel_c Reference Count PartRev.DrawNum_c Part.Model_c PartTypeElectrical  PartRev.DrawSize_c PartRev.SheetCount_c
            string[] mRow = (Globals.machine.EpicorPartNumber + "," + Globals.machine.description.Replace(',', ' ') + ",,,KOIKE,,False," + userBox.Text.ToUpper() + "," + pType + ",,False, , , , , , , , , ,A,New Machine," + Globals.machine.dwgRev + ",1,1," + Globals.machine.drawingName + "," + Globals.machine.machineName + ",FALSE," + Globals.machine.drawingSize + ",").Split(',');

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
                //Number PartDescription Epicor_Mfgcomment Epicor_Purcomment   Epicor_Mfg_name Epicor_MFGPartNum   Epicor_RandD_c Epicor_createdbylegacy_c    Epicor_PartType_c Epicor_EngComment_c Epicor_Confreq_c Epicor_EA_Manf_c    Epicor_EA_Volts_c Epicor_EA_Phase_c   Epicor_EA_Freq_c Epicor_EA_FLA_Supply_c  Epicor_EA_FLA_LgMot_c Epicor_EA_ProtDevRating_c   Epicor_EA_PannelSCCR_c Epicor_EA_EncRating_c   Revision Epicor_RevisionDescription  Dwg.Rev.Epicor_FullRel_c Reference Count PartRev.DrawNum_c Part.Model_c PartTypeElectrical  PartRev.DrawSize_c PartRev.SheetCount_c
                string[] row = (c.number + "," + c.desc.Replace(',', ' ') + ",,," + manu + ",,False,," + c.partType + ",,False, , , , , , , , , ," + c.revision + ",CONF PART," + c.revision + ",1," + c.qty.ToString() + "," + c.number + ",,TRUE,,").Split(',');
                //string[] row = (c.number + "," + c.desc.Replace(',', ' ') + ",,," + manu + ",,False,," + c.partType + ",,False, , , , , , , , , ," + c.number + "," + c.revision + "," + c.desc.Replace(',', ' ') + "," + c.revision + ",1," + Globals.machine.dumNum + ",A," + c.qty.ToString() + ",," + c.number + ",TRUE," + c.partClass + "," + purch + "," + c.number + ",,,").Split(',');
                i = 0;
                foreach (string s in row)
                {
                    expBOM[j, i] = s;
                    ++i;
                }
                ++j;
            }
            if (expEpicorCheck.Checked == true && Globals.prevConf == false)
            {
                Globals.utils.writeExcel(expBOM, @"\\manifest\BOM_Import\Mechanical\Import\" + Globals.machine.EpicorPartNumber + "_BOM_0000.xlsx", "EpdmBOMTable", Globals.machine.bom.Rows.Count + 2, Globals.expRows, -1, 1, "");
            }
            if (testExpCheck.Checked == true)
            {
                Globals.prevConf = false;
                Globals.utils.writeExcel(expBOM, @"C:\Temp\BOM\" + Globals.machine.EpicorPartNumber + "_BOM_0000.xlsx", "EpdmBOMTable", Globals.machine.bom.Rows.Count + 2, Globals.expRows, -1, 1, "");
            }
        }

        
        private void addCompButt_Click(object sender, EventArgs e)
        {
            addComponent add = new addComponent();
            add.Show();
        }
        private void clearButt_Click(object sender, EventArgs e)
        {
            clearSels("");
        }
        private void confButt_Click(object sender, EventArgs e)
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
                if (i != smartStart && i != null)
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
                                prevSoCombo.Items.AddRange(sales_orders);
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
            //dataGridView1.Columns.Insert(0, new DataGridViewColumn(dataGridView1.Rows[0].Cells[0]));
            //i = 1;
            //foreach(DataGridViewRow r in dataGridView1.Rows)
            //{
            //    if(r.Cells[2].Value != DBNull.Value)
            //    {
            //        r.Cells[0].Value = i;
            //        ++i;
            //    }
            //}
            confButt.BackColor = Color.LawnGreen;
        }
        private void exportButt_Click(object sender, EventArgs e)
        {
            if(userBox.Text == "" || userBox.Text == null)
            {
                MessageBox.Show("No User initials entered!!!! Please enter your three character User Initials and try again.", "Can I See Your ID Error");
                return;
            }

            Globals.machine.salesOrders.Add(soBox.Text);
            Globals.utils.WriteMachineToDatabase(Globals.machine);

            if (expEpicorCheck.Checked == true || testExpCheck.Checked == true)
            { 
                Task.Run(() => exportBOM());
                //MessageBox.Show("BOM Exported for Epicor.");
            }
            MessageBox.Show("Configuration Complete");
            exportButt.BackColor = Color.LawnGreen;
        }
        private void addOptButt_Click(object sender, EventArgs e)
        {
            addOption aOpt = new addOption();
            aOpt.Show();
        }
        private void quitButt_Click(object sender, EventArgs e)
        {

            ProcessStartInfo Info = new ProcessStartInfo(@"W:\Engineering\Apps - Calculators\Koike Update.exe");
            Info.Arguments = "Machine Configurator 0";
            Process.Start(Info);
            Application.Exit();
        }
        private void addMachButt_Click(object sender, EventArgs e)
        {
            addMachine addMach = new addMachine();
            addMach.Show();
        }
        private void soBox_TextChanged(object sender, EventArgs e)
        {

        }
        private void checkDBButt_Click(object sender, EventArgs e)
        {
            Globals.utils.checkDB();
        }
        private void refreshDBButt_Click(object sender, EventArgs e)
        {
            Globals.utils.updateDBs();
        }
        private void updateConfButt_Click(object sender, EventArgs e)
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
        private void requestsButt_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://forms.clickup.com/f/85n7e-434/7LVVFXREY1COZ692U6");
        }
        private void faqButt_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://app.clickup.com/8574190/v/l/li/82070636?pr=14768824");
        }
        private void helpButt_Click(object sender, EventArgs e)
        {
            helpForm help = new helpForm();
            help.Show();
        }
    }
}
