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
        public IMongoDatabase webdatabase;

        public void updateDBs()
        {
            var mongoDbAtlasString =
                "mongodb+srv://messer:5hoSjwIpbCwKSdH2@cluster0.gftmk.mongodb.net/myFirstDatabase?retryWrites=true&w=majority";
            var dbClient = new MongoClient(mongoDbAtlasString);
            webdatabase = dbClient.GetDatabase("configurator_db");
            var collectionList = webdatabase.ListCollectionNames().ToList();
            Globals.dataBase = new DataSet();

            foreach (var collectionName in collectionList)
            {
                var collection = webdatabase.GetCollection<BsonDocument>(collectionName);
                var documents = collection.Find(new BsonDocument()).ToList();

                var dt = new DataTable();
                dt.TableName = collectionName;
                foreach (var doc in documents)
                {
                    foreach (var elm in doc.Elements)
                    {
                        var collName = elm.Name.Replace("_", " ");
                        if (collName == "" || collName == " id")
                        {
                            if (collectionName.Contains("CONF") && !dt.Columns.Contains("Part Number"))
                                dt.Columns.Add(new DataColumn("Part Number"));

                            continue;
                        }

                        if (!dt.Columns.Contains(collName)) dt.Columns.Add(new DataColumn(collName));
                    }

                    Console.WriteLine(dt.Columns.ToString());
                    var dr = dt.NewRow();
                    foreach (var elm in doc.Elements)
                    {
                        var collName = elm.Name.Replace("_", " ");
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

        private void genDesc()
        {
            Globals.machine.description =
                Globals.machine.description.Substring(0, Globals.machine.description.IndexOf(",") + 2);
            var dt = Globals.machine.bom;
            var derp = Globals.machine.selOpts.ToArray();
            for (var i = 0; i < Globals.machine.selOpts.Count; ++i)
                if (string.IsNullOrWhiteSpace(Globals.machine.selOpts[i].optDesc) == false)
                {
                    var ding = Globals.machine.selOpts[i].optDesc;
                    var ring = Globals.machine.selOpts[i].optName;
                    Globals.machine.description =
                        Globals.machine.description + Globals.machine.selOpts[i].optDesc + ", ";
                }

            Globals.machine.description =
                Globals.machine.description.Substring(0, Globals.machine.description.Length - 2);
        }

        public void writeMachine()
        {
            genBom();
            genDesc();
            if (Directory.Exists(@"C:\CONFIGURATOR EPICOR UPLOADS\") == false)
                Directory.CreateDirectory(@"C:\CONFIGURATOR EPICOR UPLOADS\");
            var dt = new DataTable();
            var dtl = new DataTable();
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
            var d = DateTime.Now;
            dt.Rows.Add(Globals.machine.SmartPartNumber, "", "", "", "1", d.ToShortDateString(), Environment.UserName,
                d.ToShortDateString(), "", Globals.machine.soNum + ";");
            dt.Rows.Add(Globals.machine.EpicorPartNumber, Globals.machine.description, "", "", "", "", "", "", "", "");
            foreach (var c in Globals.machine.bomComps)
                dt.Rows.Add(c.number, c.desc, c.mrpType, c.qty, "", "", "", "", "", "");
            if (Globals.machine.lineComps.Count > 0)
            {
                dt.Rows.Add("--", "Line Items", "", "", "", "", "", "", "");
                foreach (var c in Globals.machine.lineComps)
                    dt.Rows.Add(c.number, c.desc, c.mrpType, c.qty, "", "", "", "", "", "");
            }

            dt.Rows.Add("--", "--", "", "", "", "", "", "", "");
            Globals.machine.bom = dt;

            var arr = new object[dt.Rows.Count, dt.Columns.Count];
            for (var r = 0; r < dt.Rows.Count; r++)
            {
                var dr = dt.Rows[r];
                for (var c = 0; c < dt.Columns.Count; c++) arr[r, c] = dr[c];
            }

            Globals.machine.bomObj = arr;
            if (Globals.machine.soNum != "") Task.Run(() => checkListGen.writeSP2());
        }

        private void genBom()
        {
            Globals.machine.bomComps.Clear();
            Globals.machine.lineComps.Clear();
            Globals.machine.bomComps.Add(Globals.machine.machComp);
            var doneComps = new List<string>();
            var tempComps = new List<component>();
            component c2;
            foreach (var opt in Globals.machine.selOpts)
            foreach (var comp in opt.optComps)
            {
                if (comp.number == Globals.machine.machComp.number)
                {
                    Globals.machine.bomComps[0].qty = Globals.machine.bomComps[0].qty + 1;
                    doneComps.Add(comp.number);
                    continue;
                }

                if (doneComps.Contains(comp.number))
                {
                    c2 = tempComps[doneComps.IndexOf(comp.number)];
                    if (comp.maxQty == 0 || comp.maxQty >= c2.qty + comp.typQty) c2.qty = c2.qty + comp.typQty;
                }
                else
                {
                    if (string.IsNullOrEmpty(comp.typQty.ToString())) comp.typQty = 1;
                    comp.qty = comp.typQty;
                    doneComps.Add(comp.number);
                    comp.qty = comp.qty * opt.optQty;
                    tempComps.Add(comp);
                }
            }

            foreach (var c in tempComps)
                if (c.qty != 0)
                {
                    if (c.addType == "BOM") Globals.machine.bomComps.Add(c);
                    if (c.addType == "LINE") Globals.machine.lineComps.Add(c);
                }

            Globals.machine.bomComps = Globals.machine.bomComps.OrderBy(o => o.number).ToList();
            Globals.machine.lineComps = Globals.machine.lineComps.OrderBy(o => o.number).ToList();
        }

        public Tuple<object[,], DataTable> dbAddPrep(int r, int c, DataTable dt, DataGridView dg, string type)
        {
            var arr = new object[r, c];
            var dt2 = new DataTable();
            dt2 = dt.Clone();
            var i = 0;
            foreach (DataGridViewRow dr in dg.Rows)
            {
                var row = dt2.NewRow();
                for (var j = 0; j < c; ++j)
                    if (dr.Cells[j].Value != null)
                    {
                        var val = dr.Cells[j].Value.ToString();
                        row[j] = val;
                        arr[i, j] = val;
                    }

                dt2.Rows.Add(row);
                ++i;
            }

            var valid = "";
            switch (type)
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
            var errors = "";
            var i = 0;
            for (i = 0; i < dt.Rows.Count - 1; ++i)
            {
                var r = dt.Rows[i];
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

        private string machValidity(DataTable dt)
        {
            var errors = "";
            var i = 0;
            for (i = 0; i < dt.Rows.Count - 1; ++i)
            {
                var r = dt.Rows[i];
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

        public void writeExcel(object arr, string bkName, string shtName, int ht, int len, int r, int c, string chkName)
        {
            if (Globals.prevConf)
            {
                MessageBox.Show("Machine was Previously Configured. BOM Should have been imported into MRP System");
                if (c == 1) return;
            }

            var xl = new Application();
            Workbook wrkbk;
            if (File.Exists(bkName) == false)
            {
                wrkbk = xl.Workbooks.Add(Type.Missing);
                wrkbk.SaveAs(bkName);
            }
            else
            {
                wrkbk = xl.Workbooks.Open(bkName);
            }

            Worksheet wrksht = null;
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
                catch //if(wrksht == null)
                {
                    wrksht = wrkbk.Worksheets["CONF TEMPLATE"];
                    wrksht.Copy(wrkbk.Worksheets[wrkbk.Worksheets.Count]);
                    wrksht = wrkbk.Worksheets[wrkbk.Worksheets.Count - 1];
                    wrksht.Name = shtName;
                }
            }

            var row = r + 1;
            if (c == 1)
            {
                row = 1;
                var insert = true;
                if (r == -1) insert = false;
                while (wrksht.Cells[row, 1].Value2 != null)
                {
                    if (insert)
                    {
                        if (wrksht.Cells[row, 2].Value2 == null && wrksht.Cells[row, 1].Value2.Equals(chkName))
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

            var c1 = (Range)wrksht.Cells[row, c];
            var c2 = (Range)wrksht.Cells[row + ht - 1, c + len - 1];
            var rng = wrksht.Range[c1, c2];
            if (wrksht.Name.Contains("CONF"))
            {
                var r1 = (Range)wrksht.Cells[2, 1];
                var r2 = (Range)wrksht.Cells[row, 1];
                r2.Rows.EntireRow.Interior.Color = Color.LightGreen;
            }

            rng.Value = arr;
            wrkbk.Save();
            wrkbk.Close(0);
            xl.Quit();
            GC.Collect();
            Marshal.FinalReleaseComObject(wrkbk);
            Marshal.FinalReleaseComObject(xl);

            Globals.utils.updateDBs();
        }

        public SimpleMachineData CreateSimpleMachine(MachineData _machineData)
        {
            var sm = new SimpleMachineData
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
                Sales_Orders = _machineData.salesOrders
            };
            foreach (var part in _machineData.bomComps)
            {
                var sp = new SimplePartData
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
                var sp = new SimplePartData
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
            var simpleMachine = CreateSimpleMachine(_machineData);

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
            var dt2 = getDT2(dt, col, parCol, val);
            //Determine if there is a max quantity for this option
            var maxItems = new List<int>();
            if (Globals.machine.machName != null)
                foreach (DataRow r in dt2.Rows)
                {
                    var dat = r.Field<string>(Globals.machine.machName);
                    if (dat.Contains("{"))
                    {
                        var result = dat.Split(new[] { "{", "}" }, 3, StringSplitOptions.None)[1];
                        maxItems.Add(Convert.ToInt32(result));
                    }
                }

            if (cb is ListBox)
            {
                var lb = (ListBox)cb;
                lb.Items.Clear();
                for (var i = 0; i <= dt2.Rows.Count - 1; ++i) lb.Items.Add(dt2.Rows[i][1].ToString());
            }

            if (cb is ComboBox)
            {
                var lb = (ComboBox)cb;
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
            var dt2 = new DataTable();
            if (parCol == "" && val == "")
            {
                //This is for initial population
                dt2 = distTable(dt, col);
            }
            else if (parCol == "Type" && val != "")
            {
                //This section will handle the compatability DB. Going to pass the CB name through val, and use that in a dt.Select.
                //Also need to do a dt.Select on the machine name, then grab all of the data and input to the Option datatype (will do that in the selChange method)
                var dv = new DataView();
                try
                {
                    dv = new DataView(dt.Select("Type = '" + val + "'").CopyToDataTable());
                }
                catch (Exception ex)
                {
                    return null;
                }

                dv.Sort = col;
                var headers = new List<string>();
                for (var k = 0; k < dt2.Columns.Count; ++k) headers.Add(dt2.Columns[k].ColumnName);
                dv.RowFilter = '[' + Globals.machine.machName + ']' + " <> ''";


                dt2 = dv.ToTable(false, headers.ToArray());
                var r = 0;
                for (r = 0; r < dt2.Rows.Count; ++r)
                {
                    var reqMatch = false;
                    var bomConts = false;
                    var dr = dt2.Rows[r];
                    var req1 = dr.Field<string>("OptionReqs");
                    if (req1 == "+")
                    {
                        if (dr.Field<string>(Globals.machine.machName).Contains("["))
                        {
                            var req2 = dr.Field<string>(Globals.machine.machName).Split('[', ']');
                            foreach (var opt in Globals.machine.selOpts)
                            foreach (var c in opt.optComps)
                            {
                                var reqTest = req2[1];
                                if (req2[1][0] == '-') reqTest = req2[1].Remove(0, 1);
                                if (c.number == reqTest)
                                {
                                    bomConts = true;
                                    if (req2[1].Contains("-") == false) reqMatch = true;
                                }
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
                        var reqs = req1.Split(',');
                        foreach (var req in reqs)
                            if (req.Contains('-') == false && Globals.machine.snList.Contains(req))
                                reqMatch = true;
                    }
                    else if (req1 == null || req1 == "")
                    {
                        reqMatch = true;
                    }

                    if (reqMatch == false)
                    {
                        dt2.Rows.Remove(dr);
                        --r;
                    }
                } //end foreach

                var i = 0;
                var dt3 = dt2.Copy();
                foreach (DataColumn d in dt3.Columns)
                {
                    if (i > 5 && d.ColumnName.ToUpper() != Globals.machine.machName.ToUpper())
                        dt2.Columns.Remove(d.ColumnName);
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
            var valid = false;
            if (opt != null)
            {
                var dt = Globals.cmdOptComp;
                var dv = new DataView(dt.Select(col + " = '" + opt + "'").CopyToDataTable());
                var dt2 = new DataTable();
                dv.Sort = col;
                var headers = new List<string>();
                for (var k = 0; k < dt2.Columns.Count; ++k) headers.Add(dt2.Columns[k].ColumnName);
                dv.RowFilter = "Isnull([" + Globals.machine.machName + "],'') <> ''";
                dt2 = dv.ToTable(false, headers.ToArray());
                if (dt2.Rows.Count > 0) valid = true;
            }

            return valid;
        }

        private DataTable distTable(DataTable dt, string col)
        {
            DataTable dt2;
            var dv = new DataView(dt);
            //dv.Sort = col;
            dt2 = dv.ToTable(true, col);

            return dt2;
        }

        public void initSelChange(Control.ControlCollection conts, EventHandler hand)
        {
            foreach (Control c in conts)
            {
                string[] contTypes = { "ComboBox", "ListBox", "NumericUpDown" };
                var type = c.GetType().ToString();
                switch (contTypes.FirstOrDefault(s => type.Contains(s)))
                {
                    case "ComboBox":
                        var cb = (ComboBox)c;
                        cb.SelectionChangeCommitted += hand;
                        break;
                    case "ListBox":
                        var lb = (ListBox)c;
                        lb.SelectedIndexChanged += hand;
                        break;
                    case "NumericUpDown":
                        var nb = (NumericUpDown)c;
                        nb.ValueChanged += hand;
                        break;
                }
            }
        }

        public string AddSpacesToSentence(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            var newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (var i = 1; i < text.Length; i++)
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
            if (dr == null) return true;
            var empty = false;
            foreach (var val in dr.ItemArray)
                if (val != null && val.ToString() != "")
                    return false;
            return empty;
        }

        public bool isOptName(string optName)
        {
            var optTypes = new List<string> { "Combo", "List" };
            foreach (var t in optTypes)
                if (optName.Contains(t))
                    return true;
            return false;
        }

        public DataTable delEmptyRows(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
                if (isDrEmpty(dr))
                    dt.Rows.Remove(dr);
            return dt;
        }

        public void checkDB()
        {
            var optComp = Globals.cmdOptComp.Copy();
            var comps = Globals.compData.Copy();
            var missingComps = new StringBuilder();
            missingComps.Append("Missing Component Entries for;").AppendLine();
            var optComps = new List<string>();
            var dbComps = new List<string>();
            var message = "No Missing Components";
            var i = 0;
            var j = 0;
            for (i = 6; i < optComp.Columns.Count; ++i)
            for (j = 0; j < optComp.Rows.Count; ++j)
            {
                var cell = optComp.Rows[j][i].ToString().Split(',');
                foreach (var s in cell)
                    if (s.Contains("{") == false && s.Contains("[") == false && s.ToUpper() != "X" && s != "")
                        optComps.Add(s);
            }

            optComps = optComps.Distinct().ToList();
            dbComps = comps.AsEnumerable().Select(p => p.Field<string>("Part Number")).ToList();
            dbComps.Sort();
            optComps.Sort();
            foreach (var s in optComps)
                if (dbComps.Contains(s) == false)
                    missingComps.Append(s).AppendLine();
            if (missingComps.Length > 0) message = missingComps.ToString();
            if (dbComps.Count() > optComps.Count())
            {
                var diff = dbComps.Count() - optComps.Count();
                message = "There are " + diff + " more Components in DB than Accounted for in Compatability";
            }

            MessageBox.Show(message);
        }
    }
}