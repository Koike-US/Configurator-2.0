using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using MongoDB.Bson;
using MongoDB.Driver;


using System.Text.RegularExpressions;
using Stand_Alone_Solidworks_Interface;

namespace Configurator_2._0
{
    class Utilities
    {
        public IMongoDatabase webdatabase;
        public void updateDBs()
        {
            string mongoDbAtlasString = "mongodb+srv://messer:5hoSjwIpbCwKSdH2@cluster0.gftmk.mongodb.net/myFirstDatabase?retryWrites=true&w=majority";
            MongoClient dbClient = new MongoClient(mongoDbAtlasString);
            webdatabase = (IMongoDatabase)dbClient.GetDatabase("configurator_db");
            var collectionList = webdatabase.ListCollectionNames().ToList();
            Globals.dataBase = new DataSet();

            foreach (var collectionName in collectionList)
            {
                var collection = webdatabase.GetCollection<BsonDocument>(collectionName);
                var documents = collection.Find(new BsonDocument()).ToList();

                DataTable dt = new DataTable();
                dt.TableName = collectionName;
                foreach (BsonDocument doc in documents)
                {
                    foreach (BsonElement elm in doc.Elements)
                    {
                        var collName = elm.Name.Replace("_"," ");
                        if (collName == "" || collName == " id")
                        {
                            if (collectionName.Contains("CONF") && !dt.Columns.Contains("Part Number"))
                            {
                                dt.Columns.Add(new DataColumn("Part Number"));
                            }

                            continue;
                        }

                        if (!dt.Columns.Contains(collName))
                        {
                            dt.Columns.Add(new DataColumn(collName));
                        }

                    }
                    Console.WriteLine(dt.Columns.ToString());
                    DataRow dr = dt.NewRow();
                    foreach (BsonElement elm in doc.Elements)
                    {
                        var collName = elm.Name.Replace("_", " ");
                        if (collName == "" || elm.Name == "_id")
                        {
                            if (collectionName.Contains("CONF") && !dt.Columns.Contains(collName))
                                dr["Part Number"] = elm.Value.AsString.Replace("\"", "");
                            continue;
                        }
                        if(elm.Value is BsonString)
                        {
                            dr[collName] = elm.Value.AsString.Replace("\"", "").Replace("-", "");
                            continue;
                        }
                        dr[collName] = elm.Value;
                    }
                    dt.Rows.Add(dr);
                }
                Globals.dataBase.Tables.Add(dt);
            }
            Globals.cmdOptComp = Globals.dataBase.Tables["Option Compatability"].AsEnumerable().OrderBy(r => (Convert.ToInt32(r["Order"]))).CopyToDataTable(); 
            Globals.compData = Globals.dataBase.Tables["Component Database"];
            Globals.machineData = Globals.dataBase.Tables["Machine Data"];
            return;
        }
        private void genDesc()
        {
            Globals.machine.description = Globals.machine.description.Substring(0, Globals.machine.description.IndexOf(",")+2);
            DataTable dt = Globals.machine.bom;
            option[] derp = Globals.machine.selOpts.ToArray();
            for (int i = 0; i < Globals.machine.selOpts.Count; ++i)
            {
                if (string.IsNullOrWhiteSpace(Globals.machine.selOpts[i].optDesc) == false)
                {
                    string ding = Globals.machine.selOpts[i].optDesc;
                    string ring = Globals.machine.selOpts[i].optName;
                    Globals.machine.description = Globals.machine.description + Globals.machine.selOpts[i].optDesc + ", ";
                }
            }
            Globals.machine.description = Globals.machine.description.Substring(0, Globals.machine.description.Length - 2);
        }

        public void writeMachine()
        {
            genBom();
            genDesc();
            if (Directory.Exists(@"C:\CONFIGURATOR EPICOR UPLOADS\") == false)
            {
                Directory.CreateDirectory(@"C:\CONFIGURATOR EPICOR UPLOADS\");
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            System.Data.DataTable dtl = new System.Data.DataTable();
            dt.Columns.Add("Part Number", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("MRP Type", typeof(string));
            dt.Columns.Add("Qty", typeof(string));
            dt.Columns.Add("Times Configured", typeof(string));
            dt.Columns.Add("Date Added", typeof(string));
            dt.Columns.Add("User Added", typeof(string));
            dt.Columns.Add("Last Configured", typeof(string));
            dt.Columns.Add("Notes", typeof(string));
            dt.Columns.Add("Sales Orders", typeof(string));

            dtl = dt.Copy();
            DateTime d = DateTime.Now;
            dt.Rows.Add(Globals.machine.SmartPartNumber, "", "","", "1", d.ToShortDateString(), Environment.UserName, d.ToShortDateString(), "", Globals.machine.soNum + ";");
            dt.Rows.Add(Globals.machine.EpicorPartNumber, Globals.machine.description, "", "","","","","", "","");
            foreach(component c in Globals.machine.bomComps)
            {
                dt.Rows.Add(c.number, c.desc, c.mrpType, c.qty, "", "", "", "", "", "");
            }
            if(Globals.machine.lineComps.Count > 0)
            {
                dt.Rows.Add("--", "Line Items", "", "", "", "", "", "", "");
                foreach (component c in Globals.machine.lineComps)
                {
                    dt.Rows.Add(c.number, c.desc, c.mrpType, c.qty, "", "", "", "", "", "");
                }
            }
            dt.Rows.Add("--","--", "", "", "", "", "", "", "");
            Globals.machine.bom = dt;

            object[,] arr = new object[dt.Rows.Count, dt.Columns.Count];
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                DataRow dr = dt.Rows[r];
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    arr[r, c] = dr[c];
                }
            }
            Globals.machine.bomObj = arr;
            if (Globals.machine.soNum != "")
            {
                Task.Run(() => checkListGen.writeSP());
            }
            return;
        }
        private void genBom()
        {
            Globals.machine.bomComps.Clear();
            Globals.machine.lineComps.Clear();
            Globals.machine.bomComps.Add(Globals.machine.machComp);
            List<string> doneComps = new List<string>();
            List<component> tempComps = new List<component>();
            component c2;
            epicorInterop eOp = new epicorInterop();
            foreach (option opt in Globals.machine.selOpts)
            {
                foreach (component comp in opt.optComps)
                {

                    Dictionary<string, string> data = eOp.getPartData(comp.number);
                    comp.revision = data["RevisionNum"];
                    comp.desc = data["PartDescription"];
                    comp.epicorRevDescription = "REVISION";
                    comp.epicorDrawNum = data["DrawNum"];
                    comp.epicorDrawRev = data["DrawRev_c"];
                    comp.epicorDrawSize = data["DrawSize_c"];
                    comp.epicorDrawSheetCount = data["SheetCount_c"];
                    if (comp.number == Globals.machine.machComp.number)
                    {
                        Globals.machine.bomComps[0].qty = Globals.machine.bomComps[0].qty + 1;
                        doneComps.Add(comp.number);
                        continue;
                    }
                    if (doneComps.Contains(comp.number))
                    {
                        c2 = tempComps[doneComps.IndexOf(comp.number)];
                        if (comp.maxQty == 0 || comp.maxQty >= c2.qty + comp.typQty)
                        {
                            c2.qty = c2.qty + comp.typQty;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(comp.typQty.ToString()) == true)
                        {
                            comp.typQty = 1;
                        }
                        comp.qty = comp.typQty;
                        doneComps.Add(comp.number);
                        comp.qty = comp.qty * opt.optQty;
                        tempComps.Add(comp);
                    }
                }
            }
            foreach (component c in tempComps)
            {
                if (c.qty != 0)
                {
                    if (c.addType == "BOM")
                    {
                        Globals.machine.bomComps.Add(c);
                    }
                    if (c.addType == "LINE")
                    {
                        Globals.machine.lineComps.Add(c);
                    }
                }
            }
            Globals.machine.bomComps = Globals.machine.bomComps.OrderBy(o => o.number).ToList();
            Globals.machine.lineComps = Globals.machine.lineComps.OrderBy(o => o.number).ToList();
            return;
        }
        public Tuple<object[,], DataTable> dbAddPrep(int r, int c, DataTable dt, DataGridView dg, string type)
        {
            object[,] arr = new object[r, c];
            DataTable dt2 = new DataTable();
            dt2 = dt.Clone();
            int i = 0;
            foreach (DataGridViewRow dr in dg.Rows)
            {
                DataRow row = dt2.NewRow();
                for (int j = 0; j < c; ++j)
                {
                    if (dr.Cells[j].Value != null)
                    {
                        string val = dr.Cells[j].Value.ToString();
                        row[j] = val;
                        arr[i, j] = val;
                    }
                }
                dt2.Rows.Add(row);
                ++i;
            }
            string valid = "";
            switch(type)
            {
                case "COMP":
                    valid = compValidity(dt2);
                    break;
                case "MACH":

                    break;
                case "OPT":

                    break;
            }
            if (valid != "")
            {
                MessageBox.Show(valid);
                return null;
            }


            return new Tuple<object[,], DataTable>(arr, dt2);
        }
        private string compValidity(DataTable dt)
        {
            string errors = "";
            int i = 0;
            for (i = 0; i < dt.Rows.Count - 1; ++i)
            {
                DataRow r = dt.Rows[i];
                if (r.Field<string>("Qty Select").ToUpper() == "Y")
                {
                    if (new Regex(@"^\d+").IsMatch(r.Field<string>("Qty Step")) == false)
                    {
                        errors = errors + "Field 'Qty Step' for part " + r.Field<string>("Part Number") + " Does not contain a valid integer." + System.Environment.NewLine;
                    }
                }
                if (r.Field<string>("MRP Type").ToUpper() != "P" && r.Field<string>("MRP Type").ToUpper() != "M")
                {
                    errors = errors + "Field 'MRP Type' for part " + r.Field<string>("Part Number") + " Does not contain a valid value, it should be either 'M' or 'P'." + System.Environment.NewLine;
                }
                if (r.Field<string>("Add Type").ToUpper() != "BOM" && r.Field<string>("Add Type").ToUpper() != "LINE")
                {
                    errors = errors + "Field 'Add Type' for part " + r.Field<string>("Part Number") + " Does not contain a valid value, it should be either 'BOM' or 'LINE'." + System.Environment.NewLine;
                }
                if (r.Field<string>("Part Type") != "Cutting Machine" && r.Field<string>("Part Type") != "Positioner" && r.Field<string>("Part Type") != "Portable")
                {
                    errors = errors + "Field 'Part Type' for part " + r.Field<string>("Part Number") + " Does not contain a valid value, it should be either 'Cutting Machine', 'Positioner'  or 'Portable'." + System.Environment.NewLine;
                }
                if (r.Field<string>("Standard Cost").ToUpper() != "")
                {
                    if (new Regex(@"^\d+").IsMatch(r.Field<string>("Standard Cost")) == false)
                    {
                        errors = errors + "Field 'Standard Cost' for part " + r.Field<string>("Part Number") + " Does not contain a valid number." + System.Environment.NewLine;
                    }
                }
                if (r.Field<string>("Est List Price").ToUpper() != "")
                {
                    if (new Regex(@"^\d+").IsMatch(r.Field<string>("Est List Price")) == false)
                    {
                        errors = errors + "Field 'Est List Price' for part " + r.Field<string>("Part Number") + " Does not contain a valid number." + System.Environment.NewLine;
                    }
                }
                if (r.Field<string>("Typ Qty").ToUpper() != "")
                {
                    if (new Regex(@"^\d+").IsMatch(r.Field<string>("Typ Qty")) == false)
                    {
                        errors = errors + "Typ Qty' for part " + r.Field<string>("Part Number") + " Does not contain a valid number." + System.Environment.NewLine;
                    }
                }

            }

            return errors;
        }
        private string machValidity(DataTable dt)
        {
            string errors = "";
            int i = 0;
            for (i = 0; i < dt.Rows.Count - 1; ++i)
            {
                DataRow r = dt.Rows[i];
                //if (r.Field<string>("Qty Select").ToUpper() == "Y")
                //{
                //    if (new Regex(@"^\d+").IsMatch(r.Field<string>("Qty Step")) == false)
                //    {
                //        errors = errors + "Field 'Qty Step' for part " + r.Field<string>("Part Number") + " Does not contain a valid integer." + System.Environment.NewLine;
                //    }
                //}
            }
            return errors;
        }
        public void writeExcel(object arr, string bkName, string shtName,  int ht, int len, int r, int c, string chkName)
        {
            if (Globals.prevConf == true)
            {
                MessageBox.Show("Machine was Previously Configured. BOM Should have been imported into MRP System");
                if (c == 1)
                {
                    return;
                }
            }
            Excel.Application xl = new Excel.Application();
            Excel.Workbook wrkbk;
            if (File.Exists(bkName) == false)
            {
                wrkbk = xl.Workbooks.Add(Type.Missing);
                wrkbk.SaveAs(bkName);
            }
            else
            {
                wrkbk = xl.Workbooks.Open(bkName);
            }
            Excel.Worksheet wrksht = null;
            if (shtName == "EpdmBOMTable")
            {
                wrksht = wrkbk.Sheets[1];
                wrksht.Name = "EpdmBOMTable";
            }
            else
            {
                try
                {
                    wrksht = wrkbk.Worksheets[shtName];
                }
                catch//if(wrksht == null)
                {
                    wrksht = wrkbk.Worksheets["CONF TEMPLATE"];
                    wrksht.Copy(wrkbk.Worksheets[wrkbk.Worksheets.Count]);
                    wrksht = wrkbk.Worksheets[wrkbk.Worksheets.Count - 1];
                    wrksht.Name = shtName;
                }
            }
            int row = r + 1;
            if (c == 1)
            {
                row = 1;
                bool insert = true;
                if (r == -1)
                {
                    insert = false;
                }
                while (wrksht.Cells[row, 1].Value2 != null)
                {
                    if (insert == true)
                    {
                        if (wrksht.Cells[row, 2].Value2 == null && (wrksht.Cells[row, 1].Value2.Equals(chkName)))
                        {
                            wrksht.Rows[row + 1].Insert();
                            break;
                        }
                    }
                    else if (wrksht.Cells[row, 1].Value2.Equals(chkName))
                    {
                        break;
                    }
                    ++row;
                }
            }
            Excel.Range c1 = (Excel.Range)wrksht.Cells[row, c];
            Excel.Range c2 = (Excel.Range)wrksht.Cells[row + ht - 1, c + len - 1];
            Excel.Range rng = wrksht.Range[c1, c2];
            if (wrksht.Name.Contains("CONF"))
            {
                Excel.Range r1 = (Excel.Range)wrksht.Cells[2, 1];
                Excel.Range r2 = (Excel.Range)wrksht.Cells[row, 1];
                r2.Rows.EntireRow.Interior.Color = System.Drawing.Color.LightGreen;
            }
            rng.Value = arr;
            wrkbk.Save();
            wrkbk.Close(0);
            xl.Quit();
            GC.Collect();
            Marshal.FinalReleaseComObject(wrkbk);
            Marshal.FinalReleaseComObject(xl);

            Globals.utils.updateDBs();
            return;
        }

        public SimpleMachineData CreateSimpleMachine(MachineData _machineData)
        {
            SimpleMachineData sm = new SimpleMachineData
            {
                _id = _machineData.SmartPartNumber,
                User_Added = _machineData.configuredBy,
                Epicor_Part_Number = _machineData.EpicorPartNumber,
                Description = _machineData.description,
                Date_Configured = _machineData.configuredDate,
                Last_Configured = DateTime.Now.ToShortDateString(),
                Times_Configured = _machineData.timesConfigured,
                BOM = new List<SimplePartData>(),
                Line_Items = new List<SimplePartData>(),
                Sales_Orders = _machineData.salesOrders,
            };
            foreach (var part in _machineData.bomComps)
            {
                SimplePartData sp = new SimplePartData
                {
                    Part_Number = part.number,
                    Part_Description = part.desc,
                    MRP_Type = part.mrpType,
                    Qty = part.qty.ToString()
                };
                sm.BOM.Add(sp);
            }
            foreach (var part in _machineData.lineComps)
            {
                SimplePartData sp = new SimplePartData
                {
                    Part_Number = part.number,
                    Part_Description = part.desc,
                    MRP_Type = part.mrpType,
                    Qty = part.qty.ToString()
                };
                sm.BOM.Add(sp);
            }
            sm.Times_Configured += 1;

            if (string.IsNullOrEmpty(sm.User_Added))
                sm.User_Added = Environment.UserName;
            if (string.IsNullOrEmpty(sm.Date_Configured))
                sm.Date_Configured = DateTime.Now.ToShortDateString();

            return sm;
        }

        public bool WriteMachineToDatabase(MachineData _machineData)
        {
            SimpleMachineData simpleMachine = CreateSimpleMachine(_machineData);
                        
            var collectionName = Globals.machine.prefix + " CONF";
            var collection = webdatabase.GetCollection<BsonDocument>(collectionName);

            var filter = Builders<BsonDocument>.Filter
                .Eq("_id", simpleMachine._id);

            var replaceOption = new ReplaceOptions { IsUpsert = true };

            collection.ReplaceOne(filter, simpleMachine.ToBsonDocument(), replaceOption);

            return true;
        }


        public int[] popItem(Control cb, DataTable dt, string col, string parCol, string val)
        {
            DataTable dt2 =  getDT2(dt, col, parCol, val);
            //Determine if there is a max quantity for this option
            List<int> maxItems = new List<int>();
            if (Globals.machine.machName != null)
            {
                foreach (DataRow r in dt2.Rows)
                {
                    string dat = r.Field<string>(Globals.machine.machName);
                    if (dat.Contains("{"))
                    {
                        string result = dat.Split(new string[] { "{", "}" }, 3, StringSplitOptions.None)[1];
                        maxItems.Add(Convert.ToInt32(result));
                    }
                }
            }
            if(cb is ListBox)
            {
                ListBox lb = (ListBox)cb;
                lb.Items.Clear();
                for(int i = 0; i <= dt2.Rows.Count-1; ++i)
                {
                    lb.Items.Add(dt2.Rows[i][1].ToString());
                }
            }
            if (cb is ComboBox)
            {
                ComboBox lb = (ComboBox)cb;
                lb.DataSource = dt2;
                //if (dt.TableName == "Option Compatability")
                //{
                //    lb.ValueMember = dt2.Columns[dt2.Columns[col].Ordinal + 1].ColumnName;
                //}
                //else
                //{
                lb.ValueMember = col;
                ////}
                lb.DisplayMember = col;
                lb.SelectedValue = "";
            }
            return maxItems.ToArray();
        }
        public DataTable getDT2(DataTable dt, string col, string parCol, string val)
        {
            DataTable dt2 = new DataTable();            
            if (parCol == "" && val == "")
            {
                //This is for initial population
                dt2 = distTable(dt, col);
            }
            else if (parCol == "Type" && val != "")
            {
                //This section will handle the compatability DB. Going to pass the CB name through val, and use that in a dt.Select.
                //Also need to do a dt.Select on the machine name, then grab all of the data and input to the Option datatype (will do that in the selChange method)
                DataView dv = new DataView();
                try
                {
                    dv = new DataView(dt.Select("Type = '" + val + "'").CopyToDataTable());
                }
                catch (Exception ex)
                {
                    return null;
                }
                dv.Sort = col;
                List<string> headers = new List<string>();
                for (int k = 0; k < dt2.Columns.Count; ++k)
                {
                    headers.Add(dt2.Columns[k].ColumnName);
                }
                dv.RowFilter = '[' + Globals.machine.machName + ']' +" <> ''";
                

                dt2 = dv.ToTable(false, headers.ToArray());
                int r = 0;
                for (r = 0; r < dt2.Rows.Count; ++r)
                {
                    bool reqMatch = false;
                    bool bomConts = false;
                    DataRow dr = dt2.Rows[r];
                    string req1 = dr.Field<string>("OptionReqs");
                    if (req1 == "+")
                    {
                        if (dr.Field<string>(Globals.machine.machName).Contains("["))
                        {
                            string[] req2 = dr.Field<string>(Globals.machine.machName).Split('[', ']');
                            foreach (option opt in Globals.machine.selOpts)
                            {
                                foreach (component c in opt.optComps)
                                {
                                    string reqTest = req2[1];
                                    if (req2[1][0] == '-')
                                    {
                                        reqTest = req2[1].Remove(0, 1);
                                    }
                                    if (c.number == reqTest)
                                    {
                                        bomConts = true;
                                        if (req2[1].Contains("-") == false)
                                        {
                                            reqMatch = true;
                                        }
                                    }
                                }
                            }
                            if (bomConts == false && req2[1].Contains("-"))
                            {
                                reqMatch = true;
                            }
                        }
                        else
                        {
                            reqMatch = true;
                        }
                    }
                    else if (string.IsNullOrEmpty(req1) || req1 == "-")
                    {
                        reqMatch = true;
                    }
                    else if (string.IsNullOrEmpty(req1) == false && string.IsNullOrWhiteSpace(req1) == false)
                    {
                        string[] reqs = req1.Split(',');
                        foreach (string req in reqs)
                        {
                            if (req.Contains('-') == false && Globals.machine.snList.Contains(req))
                            {
                                reqMatch = true;
                            }
                        }
                    }
                    if (reqMatch == false)
                    {
                        dt2.Rows.Remove(dr);
                        --r;
                    }
                }//end foreach
                int i = 0;
                DataTable dt3 = dt2.Copy();
                foreach (DataColumn d in dt3.Columns)
                {
                    if (i > 5 && d.ColumnName.ToUpper() != Globals.machine.machName.ToUpper())
                    {
                        dt2.Columns.Remove(d.ColumnName);
                    }
                    ++i;
                }
            }
            else
            {
                //This section handles the Machine Data DB
                try
                {
                    dt2 = distTable(dt.Select(parCol + " = '" + val + "'").CopyToDataTable(), col);
                }
                catch
                {
                    dt2 = dt;
                }
            }

            return dt2;
        }
        public bool validOpt(string col, string opt)
        {
            bool valid = false;
            if (opt != null)
            {
                DataTable dt = Globals.cmdOptComp;
                DataView dv = new DataView(dt.Select(col + " = '" + opt + "'").CopyToDataTable());
                DataTable dt2 = new DataTable();
                dv.Sort = col;
                List<string> headers = new List<string>();
                for (int k = 0; k < dt2.Columns.Count; ++k)
                {
                    headers.Add(dt2.Columns[k].ColumnName);
                }
                try
                {


                    dv.RowFilter = "Isnull([" + Globals.machine.machName + "],'') <> ''";
                }
                catch { return valid; }
                dt2 = dv.ToTable(false, headers.ToArray());
                if (dt2.Rows.Count > 0)
                {
                    valid = true;
                }
            }
            return valid;
        }
        private DataTable distTable(DataTable dt, string col)
        {
            DataTable dt2;
            DataView dv = new DataView(dt);
            //dv.Sort = col;
            dt2 = dv.ToTable(true, col);

            return dt2;
        }
        public void initSelChange(System.Windows.Forms.Control.ControlCollection conts, EventHandler hand)
        {
            foreach (Control c in conts)
            {
                string[] contTypes = { "ComboBox", "ListBox", "NumericUpDown" };
                string type = c.GetType().ToString();
                switch (contTypes.FirstOrDefault<string>(s => type.Contains(s)))
                {
                    case "ComboBox":
                        ComboBox cb = (ComboBox)c;
                        cb.SelectionChangeCommitted += new System.EventHandler(hand);
                        break;
                    case "ListBox":
                        ListBox lb = (ListBox)c;
                        lb.SelectedIndexChanged += new System.EventHandler(hand);
                        break;
                    case "NumericUpDown":
                        NumericUpDown nb = (NumericUpDown)c;
                        nb.ValueChanged += new System.EventHandler(hand);
                        break;
                    default:
                        //Do work
                        break;
                }
            }
            return;
        }
        public string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }
        public bool isDrEmpty(DataRow dr)
        {
            if(dr == null)
            {
                return (true);
            }
            bool empty = false;
            foreach(var val in dr.ItemArray)
            {
                if (val != null && val.ToString() != "")
                {
                    return false;
                }
            }
            return empty;
        }
        public bool isOptName(string optName)
        {
            List<string> optTypes = new List<string>(){ "Combo", "List" };
            foreach(string t in optTypes)
            {
                if(optName.Contains(t))
                {
                    return true;
                }
            }
            return false;
        }
        public DataTable delEmptyRows(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (isDrEmpty(dr) == true)
                {
                    dt.Rows.Remove(dr);
                }
            }
            return dt;
        }
        public void checkDB()
        {
            DataTable optComp = Globals.cmdOptComp.Copy();
            DataTable comps = Globals.compData.Copy();
            StringBuilder missingComps = new StringBuilder();
            missingComps.Append("Missing Component Entries for;").AppendLine();
            List<string> optComps = new List<string>();
            List<string> dbComps = new List<string>();
            string message = "No Missing Components";
            int i = 0;
            int j = 0;
            for(i = 6; i < optComp.Columns.Count;++i)
            {
                for (j = 0; j < optComp.Rows.Count; ++j)
                {
                    string[] cell = optComp.Rows[j][i].ToString().Split(',');
                    foreach (string s in cell)
                    {
                        if (s.Contains("{") == false && s.Contains("[") == false && s.ToUpper() != "X" && s != "")
                        {
                            optComps.Add(s);
                        }
                    }
                }
            }
            optComps = optComps.Distinct().ToList();
            dbComps = comps.AsEnumerable().Select(p => p.Field<string>("Part Number")).ToList();
            dbComps.Sort();
            optComps.Sort();
            foreach(string s in optComps)
            {
                if(dbComps.Contains(s) == false)
                {
                    missingComps.Append(s.ToString()).AppendLine();
                }
            }
            if (missingComps.Length > 0)
            {
                message = missingComps.ToString();
            }
            if(dbComps.Count() > optComps.Count())
            {
                int diff = dbComps.Count() - optComps.Count();
                message = "There are " + diff + " more Components in DB than Accounted for in Compatability";
            }
            MessageBox.Show(message);
        }
    }
}
