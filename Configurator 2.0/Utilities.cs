using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using MongoDB.Bson;
using MongoDB.Driver;
using Application = Microsoft.Office.Interop.Excel.Application;
using DataTable = System.Data.DataTable;
using ListBox = System.Windows.Forms.ListBox;

namespace Configurator_2._0
{
    internal class Utilities
    {
        private IMongoDatabase _webDatabase;

        public void UpdateDBs()
        {
            const string mongoDbAtlasString =
                "mongodb+srv://messer:5hoSjwIpbCwKSdH2@cluster0.gftmk.mongodb.net/myFirstDatabase?retryWrites=true&w=majority";
            MongoClient dbClient = new MongoClient(mongoDbAtlasString);
            _webDatabase = dbClient.GetDatabase("configurator_db");
            List<string> collectionList = _webDatabase.ListCollectionNames().ToList();
            Globals.dataBase = new DataSet();

            foreach (string collectionName in collectionList)
            {
                IMongoCollection<BsonDocument> collection = _webDatabase.GetCollection<BsonDocument>(collectionName);
                List<BsonDocument> documents = collection.Find(new BsonDocument()).ToList();

                DataTable dt = new DataTable();
                dt.TableName = collectionName;
                foreach (BsonDocument doc in documents)
                {
                    foreach (BsonElement elm in doc.Elements)
                    {
                        string collName = elm.Name.Replace("_", " ");
                        if (collName == "" || collName == " id")
                        {
                            if (collectionName.Contains("CONF") && !dt.Columns.Contains("Part Number"))
                                dt.Columns.Add(new DataColumn("Part Number"));

                            continue;
                        }

                        if (!dt.Columns.Contains(collName)) dt.Columns.Add(new DataColumn(collName));
                    }
                    DataRow dr = dt.NewRow();
                    foreach (BsonElement elm in doc.Elements)
                    {
                        string collName = elm.Name.Replace("_", " ");
                        if (collName == "" || elm.Name == "_id")
                        {
                            if (collectionName.Contains("CONF") && !dt.Columns.Contains(collName))
                                dr["Part Number"] = elm.Value.AsString.Replace("\"", "");
                            continue;
                        }

                        if (elm.Value is BsonString)
                        {
                            dr[collName] = elm.Value.AsString.Replace("\"", "");
                            continue;
                        }

                        dr[collName] = elm.Value;
                    }

                    dt.Rows.Add(dr);
                }

                Globals.dataBase.Tables.Add(dt);
            }

            Globals.cmdOptComp = Globals.dataBase.Tables["Option Compatability"].AsEnumerable()
                .OrderBy(r => Convert.ToInt32(r["Order"])).CopyToDataTable();
            Globals.compData = Globals.dataBase.Tables["Component Database"];
            Globals.machineData = Globals.dataBase.Tables["Machine Data"];
        }

        private static void GenerateDescription()
        {
            Globals.machine.description =
                Globals.machine.description.Substring(0, Globals.machine.description.IndexOf(",", StringComparison.Ordinal) + 2);
            foreach (option t in Globals.machine.selOpts.Where(t => string.IsNullOrWhiteSpace(t.optDesc) == false))
                Globals.machine.description =
                    Globals.machine.description + t.optDesc + ", ";

            Globals.machine.description =
                Globals.machine.description.Substring(0, Globals.machine.description.Length - 2);
        }

        public void WriteMachine()
        {
            GenerateBom();
            GenerateDescription();
            if (Directory.Exists(@"C:\CONFIGURATOR EPICOR UPLOADS\") == false)
                Directory.CreateDirectory(@"C:\CONFIGURATOR EPICOR UPLOADS\");
            DataTable dt = new DataTable();
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
            
            DateTime d = DateTime.Now;
            dt.Rows.Add(Globals.machine.smartPartNumber, "", "", "", "1", d.ToShortDateString(), Environment.UserName,
                d.ToShortDateString(), "", Globals.machine.soNum + ";");
            dt.Rows.Add(Globals.machine.epicorPartNumber, Globals.machine.description, "", "", "", "", "", "", "", "");
            foreach (component c in Globals.machine.bomComps)
                dt.Rows.Add(c.number, c.desc, c.mrpType, c.qty, "", "", "", "", "", "");
            if (Globals.machine.lineComps.Count > 0)
            {
                dt.Rows.Add("--", "Line Items", "", "", "", "", "", "", "");
                foreach (component c in Globals.machine.lineComps)
                    dt.Rows.Add(c.number, c.desc, c.mrpType, c.qty, "", "", "", "", "", "");
            }

            dt.Rows.Add("--", "--", "", "", "", "", "", "", "");
            Globals.machine.bom = dt;

            object[,] arr = new object[dt.Rows.Count, dt.Columns.Count];
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                DataRow dr = dt.Rows[r];
                for (int c = 0; c < dt.Columns.Count; c++) arr[r, c] = dr[c];
            }

            Globals.machine.bomObj = arr;
            if (Globals.machine.soNum != "") Task.Run(checkListGen.WriteSp2);
        }

        private static void GenerateBom()
        {
            Globals.machine.bomComps.Clear();
            Globals.machine.lineComps.Clear();
            Globals.machine.bomComps.Add(Globals.machine.machComp);
            List<string> doneComps = new List<string>();
            List<component> tempComps = new List<component>();
            foreach (option opt in Globals.machine.selOpts)
            foreach (component comp in opt.optComps)
            {
                if (comp.number == Globals.machine.machComp.number)
                {
                    Globals.machine.bomComps[0].qty += 1;
                    doneComps.Add(comp.number);
                    continue;
                }

                if (doneComps.Contains(comp.number))
                {
                    component c2 = tempComps[doneComps.IndexOf(comp.number)];
                    if (comp.maxQty == 0 || comp.maxQty >= c2.qty + comp.typQty) c2.qty += comp.typQty;
                }
                else
                {
                    if (string.IsNullOrEmpty(comp.typQty.ToString())) comp.typQty = 1;
                    comp.qty = comp.typQty;
                    doneComps.Add(comp.number);
                    comp.qty *= opt.optQty;
                    tempComps.Add(comp);
                }
            }

            foreach (component c in tempComps.Where(c => c.qty != 0))
                switch (c.addType)
                {
                    case "BOM":
                        Globals.machine.bomComps.Add(c);
                        break;
                    case "LINE":
                        Globals.machine.lineComps.Add(c);
                        break;
                }

            Globals.machine.bomComps = Globals.machine.bomComps.OrderBy(o => o.number).ToList();
            Globals.machine.lineComps = Globals.machine.lineComps.OrderBy(o => o.number).ToList();
        }

        public Tuple<object[,], DataTable> DbAddPrep(int r, int c, DataTable dt, DataGridView dg, string type)
        {
            object[,] arr = new object[r, c];
            DataTable dt2 = dt.Clone();
            int i = 0;
            foreach (DataGridViewRow dr in dg.Rows)
            {
                DataRow row = dt2.NewRow();
                for (int j = 0; j < c; ++j)
                    if (dr.Cells[j].Value != null)
                    {
                        string val = dr.Cells[j].Value.ToString();
                        row[j] = val;
                        arr[i, j] = val;
                    }

                dt2.Rows.Add(row);
                ++i;
            }

            string valid = "";
            switch (type)
            {
                case "COMP":
                    valid = ValidateComp(dt2);
                    break;
                case "MACH":

                    break;
                case "OPT":

                    break;
            }

            if (valid == "") return new Tuple<object[,], DataTable>(arr, dt2);
            MessageBox.Show(valid);
            return null;


        }

        private static string ValidateComp(DataTable dt)
        {
            string errors = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                                DataRow r = dt.Rows[i];
                if (r.Field<string>("Qty Select").ToUpper() == "Y")
                    if (new Regex(@"^\d+").IsMatch(r.Field<string>("Qty Step")) == false)
                        errors = errors + "Field 'Qty Step' for part " + r.Field<string>("Part Number") +
                                 " Does not contain a valid integer." + Environment.NewLine;
                if (r.Field<string>("MRP Type").ToUpper() != "P" && r.Field<string>("MRP Type").ToUpper() != "M")
                    errors = errors + "Field 'MRP Type' for part " + r.Field<string>("Part Number") +
                             " Does not contain a valid value, it should be either 'M' or 'P'." + Environment.NewLine;
                if (r.Field<string>("Add Type").ToUpper() != "BOM" && r.Field<string>("Add Type").ToUpper() != "LINE")
                    errors = errors + "Field 'Add Type' for part " + r.Field<string>("Part Number") +
                             " Does not contain a valid value, it should be either 'BOM' or 'LINE'." +
                             Environment.NewLine;
                if (r.Field<string>("Part Type") != "Cutting Machine" && r.Field<string>("Part Type") != "Positioner" &&
                    r.Field<string>("Part Type") != "Portable")
                    errors = errors + "Field 'Part Type' for part " + r.Field<string>("Part Number") +
                             " Does not contain a valid value, it should be either 'Cutting Machine', 'Positioner'  or 'Portable'." +
                             Environment.NewLine;
                if (r.Field<string>("Standard Cost").ToUpper() != "")
                    if (new Regex(@"^\d+").IsMatch(r.Field<string>("Standard Cost")) == false)
                        errors = errors + "Field 'Standard Cost' for part " + r.Field<string>("Part Number") +
                                 " Does not contain a valid number." + Environment.NewLine;
                if (r.Field<string>("Est List Price").ToUpper() != "")
                    if (new Regex(@"^\d+").IsMatch(r.Field<string>("Est List Price")) == false)
                        errors = errors + "Field 'Est List Price' for part " + r.Field<string>("Part Number") +
                                 " Does not contain a valid number." + Environment.NewLine;
                if (r.Field<string>("Typ Qty").ToUpper() != "")
                    if (new Regex(@"^\d+").IsMatch(r.Field<string>("Typ Qty")) == false)
                        errors = errors + "Typ Qty' for part " + r.Field<string>("Part Number") +
                                 " Does not contain a valid number." + Environment.NewLine;
            }
            
            return errors;
        }
        
        public void WriteExcel(object arr, string bkName, string shtName, int ht, int len, int r, int c, string chkName)
        {
            if (Globals.prevConf)
            {
                MessageBox.Show("Machine was Previously Configured. BOM Should have been imported into MRP System");
                if (c == 1) return;
            }

            Application xl = new Application();
            Workbook workBook;
            if (File.Exists(bkName) == false)
            {
                workBook = xl.Workbooks.Add(Type.Missing);
                workBook.SaveAs(bkName);
            }
            else
            {
                workBook = xl.Workbooks.Open(bkName);
            }

            Worksheet workSheet = null;
            if (shtName == "EpdmBOMTable")
            {
                workSheet = workBook.Sheets[1];
                workSheet.Name = "EpdmBOMTable";
            }
            else
            {
                try
                {
                    workSheet = workBook.Worksheets[shtName];
                }
                catch //if(workSheet == null)
                {
                    workSheet = workBook.Worksheets["CONF TEMPLATE"];
                    workSheet.Copy(workBook.Worksheets[workBook.Worksheets.Count]);
                    workSheet = workBook.Worksheets[workBook.Worksheets.Count - 1];
                    workSheet.Name = shtName;
                }
            }

            int row = r + 1;
            if (c == 1)
            {
                row = 1;
                bool insert = r != -1;
                while (workSheet.Cells[row, 1].Value2 != null)
                {
                    if (insert)
                    {
                        if (workSheet.Cells[row, 2].Value2 == null && workSheet.Cells[row, 1].Value2.Equals(chkName))
                        {
                            workSheet.Rows[row + 1].Insert();
                            break;
                        }
                    }
                    else if (workSheet.Cells[row, 1].Value2.Equals(chkName))
                    {
                        break;
                    }

                    ++row;
                }
            }

            Range c1 = (Range)workSheet.Cells[row, c];
            Range c2 = (Range)workSheet.Cells[row + ht - 1, c + len - 1];
            Range rng = workSheet.Range[c1, c2];
            if (workSheet.Name.Contains("CONF"))
            {
                Range r2 = (Range)workSheet.Cells[row, 1];
                r2.Rows.EntireRow.Interior.Color = Color.LightGreen;
            }

            rng.Value = arr;
            workBook.Save();
            workBook.Close(0);
            xl.Quit();
            GC.Collect();
            Marshal.FinalReleaseComObject(workBook);
            Marshal.FinalReleaseComObject(xl);

            Globals.utils.UpdateDBs();
        }

        private static SimpleMachineData CreateSimpleMachine(MachineData machineData)
        {
            SimpleMachineData sm = new SimpleMachineData
            {
                id = machineData.smartPartNumber,
                userAdded = machineData.configuredBy,
                epicorPartNumber = machineData.epicorPartNumber,
                description = machineData.description,
                dateConfigured = machineData.configuredDate,
                lastConfigured = DateTime.Now.ToShortDateString(),
                timesConfigured = machineData.timesConfigured,
                bom = new List<SimplePartData>(),
                lineItems = new List<SimplePartData>(),
                salesOrders = machineData.salesOrders
            };
            foreach (component part in machineData.bomComps)
            {
                SimplePartData sp = new SimplePartData
                {
                    partNumber = part.number,
                    partDescription = part.desc,
                    mrpType = part.mrpType,
                    qty = part.qty.ToString()
                };
                sm.bom.Add(sp);
            }

            foreach (SimplePartData sp in machineData.lineComps.Select(part => new SimplePartData
                     {
                         partNumber = part.number,
                         partDescription = part.desc,
                         mrpType = part.mrpType,
                         qty = part.qty.ToString()
                     }))
            {
                sm.bom.Add(sp);
            }

            sm.timesConfigured += 1;

            if (string.IsNullOrEmpty(sm.userAdded))
                sm.userAdded = Environment.UserName;
            if (string.IsNullOrEmpty(sm.dateConfigured))
                sm.dateConfigured = DateTime.Now.ToShortDateString();

            return sm;
        }

        public void WriteMachineToDatabase(MachineData machineData)
        {
            SimpleMachineData simpleMachine = CreateSimpleMachine(machineData);

            string collectionName = Globals.machine.prefix + " CONF";
            IMongoCollection<BsonDocument> collection = _webDatabase.GetCollection<BsonDocument>(collectionName);

            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter
                .Eq("_id", simpleMachine.id);

            ReplaceOptions replaceOption = new ReplaceOptions { IsUpsert = true };

            collection.ReplaceOne(filter, simpleMachine.ToBsonDocument(), replaceOption);
        }


        public int[] PopItem(Control cb, DataTable dt, string col, string parCol, string val)
        {
            DataTable dt2 = GetDt2(dt, col, parCol, val);
            //Determine if there is a max quantity for this option
            List<int> maxItems = new List<int>();
            if (Globals.machine.machName != null) maxItems.AddRange(from DataRow r in dt2.Rows select r.Field<string>(Globals.machine.machName) into dat where dat.Contains("{") select dat.Split(new[] { "{", "}" }, 3, StringSplitOptions.None)[1] into result select Convert.ToInt32(result));

            switch (cb)
            {
                case ListBox box:
                {
                    ListBox lb = box;
                    lb.Items.Clear();
                    for (int i = 0; i <= dt2.Rows.Count - 1; ++i) lb.Items.Add(dt2.Rows[i][1].ToString());
                    break;
                }
                case ComboBox box:
                {
                    ComboBox lb = box;
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
                    break;
                }
            }

            return maxItems.ToArray();
        }

        public DataTable GetDt2(DataTable dt, string col, string parCol, string val)
        {
            DataTable dt2 = new DataTable();
            switch (parCol)
            {
                case "" when val == "":
                    //This is for initial population
                    dt2 = DistTable(dt, col);
                    break;
                case "Type" when val != "":
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
                    for (int k = 0; k < dt2.Columns.Count; ++k) headers.Add(dt2.Columns[k].ColumnName);
                    dv.RowFilter = '[' + Globals.machine.machName + ']' + " <> ''";


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
                                foreach (component c in opt.optComps)
                                {
                                    string reqTest = req2[1];
                                    if (req2[1][0] == '-') reqTest = req2[1].Remove(0, 1);
                                    if (c.number != reqTest) continue;
                                    bomConts = true;
                                    if (req2[1].Contains("-") == false) reqMatch = true;
                                }

                                if (bomConts == false && req2[1].Contains("-")) reqMatch = true;
                            }
                            else
                            {
                                reqMatch = true;
                            }
                        }
                        else if (string.IsNullOrEmpty(req1) == false && string.IsNullOrWhiteSpace(req1) == false)
                        {
                            string[] reqs = req1.Split(',');
                            foreach (string req in reqs)
                                if (req.Contains('-') == false && Globals.machine.snList.Contains(req))
                                    reqMatch = true;
                        }
                        else if (string.IsNullOrEmpty(req1))
                        {
                            reqMatch = true;
                        }

                        if (reqMatch == false)
                        {
                            dt2.Rows.Remove(dr);
                            --r;
                        }
                    } //end foreach

                    int i = 0;
                    DataTable dt3 = dt2.Copy();
                    foreach (DataColumn d in dt3.Columns)
                    {
                        if (i > 5 && !string.Equals(d.ColumnName, Globals.machine.machName, StringComparison.CurrentCultureIgnoreCase))
                            dt2.Columns.Remove(d.ColumnName);
                        ++i;
                    }

                    break;
                }
                default:
                    //This section handles the Machine Data DB
                    try
                    {
                        dt2 = DistTable(dt.Select(parCol + " = '" + val + "'").CopyToDataTable(), col);
                    }
                    catch
                    {
                        dt2 = dt;
                    }

                    break;
            }

            return dt2;
        }

        public bool IsValidOption(string col, string opt)
        {
            bool valid = false;
            if (opt == null) return valid;
            DataTable dt = Globals.cmdOptComp;
            DataView dv = new DataView(dt.Select(col + " = '" + opt + "'").CopyToDataTable());
            DataTable dt2 = new DataTable();
            dv.Sort = col;
            List<string> headers = new List<string>();
            for (int k = 0; k < dt2.Columns.Count; ++k) headers.Add(dt2.Columns[k].ColumnName);
            dv.RowFilter = "Isnull([" + Globals.machine.machName + "],'') <> ''";
            dt2 = dv.ToTable(false, headers.ToArray());
            if (dt2.Rows.Count > 0) valid = true;

            return valid;
        }

        private static DataTable DistTable(DataTable dt, string col)
        {
            DataTable dt2;
            DataView dv = new DataView(dt);
            //dv.Sort = col;
            dt2 = dv.ToTable(true, col);

            return dt2;
        }

        public static void InitSelChange(Control.ControlCollection conts, EventHandler hand)
        {
            foreach (Control c in conts)
            {
                string[] contTypes = { "ComboBox", "ListBox", "NumericUpDown" };
                string type = c.GetType().ToString();
                switch (contTypes.FirstOrDefault(s => type.Contains(s)))
                {
                    case "ComboBox":
                        ComboBox cb = (ComboBox)c;
                        cb.SelectionChangeCommitted += hand;
                        break;
                    case "ListBox":
                        ListBox lb = (ListBox)c;
                        lb.SelectedIndexChanged += hand;
                        break;
                    case "NumericUpDown":
                        NumericUpDown nb = (NumericUpDown)c;
                        nb.ValueChanged += hand;
                        break;
                }
            }
        }

        public static string AddSpacesToSentence(string text)
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

        private static bool IsDataRowEmpty(DataRow dr)
        {
            return (dr == null || dr.ToString() == "");
        }

        public bool IsOption(string optName)
        {
            List<string> optTypes = new List<string> { "Combo", "List" };
            return optTypes.Any(optName.Contains);
        }

        public static void DeleteEmptyRows(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
                if (IsDataRowEmpty(dr))
                    dt.Rows.Remove(dr);
        }

        public static void CheckDatabase()
        {
            DataTable optComp = Globals.cmdOptComp.Copy();
            DataTable comps = Globals.compData.Copy();
            StringBuilder missingComps = new StringBuilder();
            missingComps.Append("Missing Component Entries for;").AppendLine();
            List<string> optComps = new List<string>();
            string message = "No Missing Components";
            int i = 0;
            int j = 0;
            for (i = 6; i < optComp.Columns.Count; ++i)
            for (j = 0; j < optComp.Rows.Count; ++j)
            {
                string[] cell = optComp.Rows[j][i].ToString().Split(',');
                optComps.AddRange(cell.Where(s => s.Contains("{") == false && s.Contains("[") == false && s.ToUpper() != "X" && s != ""));
            }

            optComps = optComps.Distinct().ToList();
            List<string> dbComps = comps.AsEnumerable().Select(p => p.Field<string>("Part Number")).ToList();
            dbComps.Sort();
            optComps.Sort();
            foreach (string s in optComps.Where(s => dbComps.Contains(s) == false))
                missingComps.Append(s).AppendLine();
            if (missingComps.Length > 0) message = missingComps.ToString();
            if (dbComps.Count() > optComps.Count())
            {
                int diff = dbComps.Count() - optComps.Count();
                message = "There are " + diff + " more Components in DB than Accounted for in Compatability";
            }

            MessageBox.Show(message);
        }
    }
}