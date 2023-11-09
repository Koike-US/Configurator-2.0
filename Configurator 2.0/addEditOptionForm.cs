using Microsoft.Office.Interop.Word;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Configurator_2._0
{
    public partial class addEditOptionForm : Form
    {
        int defaultCols = 0;
        bool loaded = false;
        bool errors = false;

        List<string> insertLocations = new List<string>(); // based on "Order" column
        List<SimpleOption> options = new List<SimpleOption>();
        List<SimpleComponent> newComps = new List<SimpleComponent>();

        DataGridViewComboBoxColumn col;

        public IMongoDatabase webdatabase;


        Dictionary<string, int> typeOrder = new Dictionary<string, int>();
        public addEditOptionForm()
        {
            InitializeComponent();
            System.Data.DataTable dt = Globals.machineData.DefaultView.ToTable(true,new string[] {"LineCombo"});
            System.Data.DataTable dt2 = Globals.cmdOptComp.DefaultView.ToTable(true, new string[] { "Type" });
            lineCombo.DataSource = dt;
            lineCombo.ValueMember = "LineCombo";
            lineCombo.DisplayMember = "LineCombo";
            lineCombo.SelectedValue = "";
            col = (DataGridViewComboBoxColumn)optionGridView.Columns["Type"];
            col.Items.AddRange(dt2.AsEnumerable().Select(r => r.Field<string>("Type")).ToArray());
            col.DisplayMember = "Type";
            col.ValueMember = "Type";
            col.Items.Add("New Option Group");
            optionGridView.Rows.Add();
            defaultCols = optionGridView.Columns.Count;
            loaded = true;

            string mongoDbAtlasString = "mongodb+srv://messer:5hoSjwIpbCwKSdH2@cluster0.gftmk.mongodb.net/myFirstDatabase?retryWrites=true&w=majority";
            MongoClient dbClient = new MongoClient(mongoDbAtlasString);
            webdatabase = (IMongoDatabase)dbClient.GetDatabase("configurator_db");

            // Add the events to listen for
            optionGridView.CellValueChanged += new DataGridViewCellEventHandler(optionGridView_CellValueChanged);
            optionGridView.CurrentCellDirtyStateChanged += new EventHandler(optionGridView_CurrentCellDirtyStateChanged);

        }
        private void CommitEdit()
        {
            errors = false;
            Utilities dbs = new Utilities();
            List<string> pns = new List<string>();
            dbs.updateDBs();
            DataValidation();
            DataColumn[] cols = Globals.cmdOptComp.Columns.Cast<DataColumn>().ToArray();
            foreach (SimpleOption o in options)
            {
                foreach (KeyValuePair<string, string> k in o.optPartsData)
                {
                    foreach (string s in k.Value.Split(','))
                    {
                        int j = 0;
                        bool dobreak = false;
                        while (dobreak == false && j < Globals.cmdOptComp.Rows.Count)// foreach (DataGridViewRow r in optionGridView.Rows)
                        {
                            DataRow r = Globals.cmdOptComp.Rows[j];
                            int i = 7;
                            while (dobreak == false && i < r.ItemArray.Count()) //for (int i = 6; i < r.Cells.Count; i++)
                            {
                                if (r[i].ToString().ToUpper().Contains(s.ToUpper()) == true)
                                {
                                    dobreak = true;
                                }
                                ++i;
                            }
                            ++j;
                        }
                        if(dobreak == false)
                        {
                            pns.Add(s);
                        }
                    }
                }
            }
            List<string> distinctPns = new List<string>();
            distinctPns.AddRange(pns.Distinct().ToArray());
            if (newComps.Count != distinctPns.Count && distinctPns.Count >0)
            {
                newCompDataForm comp = new newCompDataForm(distinctPns);

                var r = comp.ShowDialog();
                if (r == DialogResult.OK)
                {
                    var collectionName = "Component Database";
                    var collection = webdatabase.GetCollection<BsonDocument>(collectionName);
                    foreach (SimpleComponent c in comp.data)
                    {
                        BsonDocument bsonDoc = new BsonDocument();
                        bsonDoc.Add("Part Number", BsonValue.Create(c.pn));
                        bsonDoc.Add("Add Type", BsonValue.Create(c.lineAdd));
                        bsonDoc.Add("Qty Select", BsonValue.Create(c.qtySelect));
                        bsonDoc.Add("Max Qty", BsonValue.Create(c.maxQty));
                        bsonDoc.Add("Qty Step", BsonValue.Create(c.qtyStep));
                        bsonDoc.Add("Typ Qty", BsonValue.Create("1"));
                        collection.InsertOne(bsonDoc);
                    }
                }
            }
            if (errors == false)
            {
                int i = 0;
                int nextOrder = -1;
                string prevOptType = options[0].Type;
                var collectionName = "Option Compatability";
                var collection = webdatabase.GetCollection<BsonDocument>(collectionName);
                foreach (SimpleOption opt in options)
                {
                    BsonDocument bsonDoc;
                    var jsonDoc = Newtonsoft.Json.JsonConvert.SerializeObject(opt.optPartsData);
                    bsonDoc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(jsonDoc);

                    int order = opt.Order;
                    if(order == 0)
                    {
                        if (nextOrder == -1)
                        {
                            order = FindInsertLocation(opt.prevType);
                            nextOrder = order;
                            ++nextOrder;
                        }
                        else
                        {
                            order = nextOrder;
                            ++nextOrder;
                        }
                    }
                    else if (order == -1)
                    {
                        order = 1;
                    }

                    var filter = Builders<BsonDocument>.Filter.Gt("Order", order-1);
                    var update = Builders<BsonDocument>.Update.Inc("Order", 1);
                    var result = collection.UpdateMany(filter, update);


                    bsonDoc.Add("_id",BsonValue.Create( opt._id));
                    bsonDoc.Add("Type", BsonValue.Create(opt.Type));
                    bsonDoc.Add("Name", BsonValue.Create(opt.Name));
                    bsonDoc.Add("Smart Designator", BsonValue.Create(opt.Smart_Designator));
                    bsonDoc.Add("OptionChecklist", BsonValue.Create(opt.OptionCheckList));
                    bsonDoc.Add("OptionReqs", BsonValue.Create(opt.OptionReqs));
                    bsonDoc.Add("Short Description", BsonValue.Create(opt.Short_Description));
                    bsonDoc.Add("Order", BsonValue.Create(order));
                    collection.InsertOne(bsonDoc);
                    if (opt.Type != prevOptType)
                    {
                        prevOptType = opt.Type;
                        ++i;
                    }
                }
                ResetGrid();
                modelListView.Items.Clear();
                lineCombo.SelectedText = "";
                typeOrder.Clear();
                Utilities ut = new Utilities();
                ut.updateDBs();
                MessageBox.Show("Database Update Complete");
                errors = false;
            }
            else
            {
                MessageBox.Show("There are errors in your data. Please review the red highlighted section below and resubmit. \n Hover your mouse over the highlighted cells for pertinent information.");
            }
            return;
        }
        private void DataValidation()
        {
            options.Clear();
            options.Capacity = 0;
            int j = 0;
            Dictionary<string, int> typeLevel = new Dictionary<string, int>();
            foreach (DataGridViewRow row in optionGridView.Rows)
            {
                SimpleOption opt = new SimpleOption();
                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)row.Cells[0];
                if (row.Cells[0].Value != null)
                {
                    opt.prevType = row.Cells[0].Value.ToString();
                    opt.Type = row.Cells[0].Value.ToString();
                    if (row.Cells[0].Value.ToString() == "New Option Group")
                    {
                        newOptionGroupForm form = new newOptionGroupForm();
                        var result = form.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            string[] groupData = form.optionGroupName.Split(':');
                            cell.Items.Add(groupData[1]);
                            cell.Value = groupData[1];
                            if (groupData[0] == "At Top")
                            {
                                opt.Order = -1;
                            }
                        }
                        else
                        {
                            errors = true;
                            cell.FlatStyle = FlatStyle.Popup;
                            cell.Style.BackColor = Color.Red;
                            cell.ToolTipText = "You must select one option group to add this option to. If you need to add a new option group please select that option from this list.";
                        }
                    }
                }
                else
                { 
                    errors = true;
                    cell.FlatStyle = FlatStyle.Popup;
                    cell.Style.BackColor = Color.Red;
                    cell.ToolTipText = "You must select one option group to add this option to. If you need to add a new option group please select that option from this list.";
                }
                if (row.Cells[1].Value == null)//Name
                {
                    errors = true;
                    row.Cells[1].Style.BackColor = Color.Red;
                }
                else if (row.Cells[1].Value.ToString().Contains('\'') == true)
                {
                    opt.Name = row.Cells[1].Value.ToString().Replace("'","FT");
                }
                else
                {
                    opt.Name = row.Cells[1].Value.ToString();
                }
                if (row.Cells[1].Value.ToString().ToUpper() == "NONE")
                {
                    opt.Smart_Designator = " ";
                    opt.Short_Description = " ";
                }
                else if (row.Cells[2].Value == null )//Smart Designator
                {
                    errors = true;
                    row.Cells[2].Style.BackColor = Color.Red;
                }                
                else
                {
                    DataRow[] dr = Globals.cmdOptComp.Select("[Smart Designator] = '" + row.Cells[2].Value + "'");
                    if (dr.Length > 0)
                    {
                        DialogResult result = MessageBox.Show("Smart Designators need to be unique to each option, this designator already exists at \nOrder#: " + dr[0].Field<string>("Order") +
                            "\n Do you want to add this under the same option? If so click 'Yes'. Otherwise click 'No' and enter a new deisgnator", "", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            opt.Order = dr[0].Field<int>("Order");
                        }
                        else
                        {
                            errors = true;
                            row.Cells[2].Style.BackColor = Color.Red;
                        }

                    }
                    else
                    {
                        opt.Smart_Designator = row.Cells[2].Value.ToString();
                    }
                }
                if (row.Cells[5].Value == null)//Description
                {
                    errors = true;
                    row.Cells[5].Style.BackColor = Color.Red;
                }
                else if (row.Cells[1].Value.ToString().ToUpper() != "NONE")
                {
                    opt.Short_Description = row.Cells[5].Value.ToString();
                    if (row.Cells[5].Value.ToString().Contains('\'') == true)
                    {
                        opt.Name = row.Cells[5].Value.ToString().Replace("'", "FT");
                    }
                }
                if (Convert.ToBoolean(row.Cells[6].Value) == true )//Qty Select
                {
                    if (row.Cells[7].Value == null )//Max Qty
                    {
                        errors = true;
                        row.Cells[7].Style.BackColor = Color.Red;
                    }
                    if (row.Cells[8].Value == null)//Qty Step
                    {
                        errors = true;
                        row.Cells[8].Style.BackColor = Color.Red;
                    }
                }
                if (row.Cells[9].Value != null && row.Cells[9].Value is string )//Order
                {
                    opt.prevType = row.Cells[9].Value.ToString();
                }
                for (int i = defaultCols; i < optionGridView.Columns.Count; ++i)//part number fields
                {
                    if (row.Cells[i].Value == null || (row.Cells[1].Value.ToString().ToUpper() != "NONE" && row.Cells[i].Value.ToString().ToUpper() == "X"))
                    {
                        errors = true;
                        row.Cells[i].Style.BackColor = Color.Red;
                    }
                    else
                    {
                        opt.optPartsData.Add(optionGridView.Columns[i].HeaderText.Replace(" Required Part Numbers", ""), row.Cells[i].Value.ToString());
                    }
                }
                if (row.Cells[3].Value != null )
                {
                    opt.OptionCheckList = row.Cells[3].Value.ToString();
                }
                if (row.Cells[4].Value != null )
                {
                    opt.OptionReqs = row.Cells[4].Value.ToString();
                }

                if (row.Cells[6].Value != null )
                { 
                    opt.optQtySelect = row.Cells[6].Value.ToString();
                    opt.optMaxQty = row.Cells[7].Value.ToString();
                    opt.optQtyStep = row.Cells[8].Value.ToString();
                }
                if(typeLevel.ContainsKey(opt.Type) == false)
                {
                    typeLevel.Add(opt.Type, 1);
                }
                else
                {
                    typeLevel[opt.Type] = typeLevel[opt.Type] + 1;
                }
                DataRow[] dr2 = Globals.cmdOptComp.Select("Type = '" + opt.Type + "'");
                // Use LINQ to find the DataRow with the highest 'order' value
                DataRow maxOrderDataRow = dr2
                    .OrderByDescending(r => int.Parse(r.Field<string>("Order")))
                    .FirstOrDefault();
                string s = maxOrderDataRow.Field<string>(" id");
                opt._id = opt.Type + (Convert.ToInt32(s.Substring(18, s.Length - 18)) + typeLevel[opt.Type]).ToString();
                ++j;
                options.Add(opt);
            }
            List<SimpleOption> tList = options.OrderBy(r => r.Type).ToList();
            options.Clear();
            options.Capacity = 0;
            options.AddRange(tList.ToArray());
            optionGridView.ClearSelection();
            return;
        }
        private int FindInsertLocation(string type)
        {
            int order = 0;
            DataRow[] dr = Globals.cmdOptComp.Select("Type = '" + type + "'");
            if(typeOrder.ContainsKey(type) == false)
            {
                typeOrder.Add(type, 1); 
            }
            else
            {
                typeOrder[type] = typeOrder[type] + 1;
            }
            if (dr.Length > 0)
            {
                List<int> orders = new List<int>();
                foreach (DataRow dr2 in dr)
                {
                    orders.Add(Convert.ToInt32(dr2.Field<string>("Order")));
                }
                order = orders.Max() + typeOrder[type];
            }
            return order;
        }
        private void ResetGrid()
        {
            if (loaded == true)
            {
                optionGridView.Rows.Clear();
                if (optionGridView.Columns.Count > defaultCols)
                {
                    for (int i = defaultCols; i < optionGridView.Columns.Count; ++i)
                    {
                        optionGridView.Columns.RemoveAt(i);
                    }
                }
                options.Clear();
                options.Capacity = 0;
                optionGridView.Rows.Add();
                optionGridView.Enabled = false;
            }
            return;
        }
        private void addRowButt_Click(object sender, EventArgs e)
        {
            optionGridView.Rows.Add();
        }
        private void lineCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetGrid();
            modelListView.Items.Clear();
            if (lineCombo.SelectedValue != null && string.IsNullOrEmpty(lineCombo.SelectedValue.ToString()) == false)
            {
                DataRow[] dr = Globals.machineData.Select("[LineCombo] = '" + lineCombo.SelectedValue.ToString() + "'");
                foreach (DataRow row in dr)
                {
                    ListViewItem item = new ListViewItem(row[3].ToString());
                    modelListView.Items.Add(item);
                }
            }
        }
        private void setMachinesButt_Click(object sender, EventArgs e)
        {
            ResetGrid();
            foreach (ListViewItem i in modelListView.Items)
            {
                if(i.Checked == true)
                {
                    DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
                    c.HeaderText = i.Text + " Required Part Numbers";
                    c.Width = 150;
                    optionGridView.Columns.Add(c);
                    optionGridView.Enabled = true;
                    c.ToolTipText = "Comma delimited list of the part numbers required for this option for this machine, or 'X' if no part numbers are required. \\n Ex; T8900124701,0818313602 to add a 35' torch for a Powermax125";
                }

            }
        }
        private void commitEditButt_Click(object sender, EventArgs e)
        {
            if (optionGridView.Columns.Count > defaultCols)
            {
                CommitEdit();
            }
        }

        // This event handler manually raises the CellValueChanged event 
        // by calling the CommitEdit method. 
        void optionGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (optionGridView.IsCurrentCellDirty)
            {
                // This fires the cell value changed handler below
                optionGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void optionGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)optionGridView.Rows[e.RowIndex].Cells[0];
            if (cell.Value != null && cell.Value.ToString() == "New Option Group")
            {
                newOptionGroupForm form = new newOptionGroupForm();
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    DataRow[] dr;
                    string[] groupData = form.optionGroupName.Split(':');
                    cell.Items.Add(groupData[1]);
                    col.Items.Add(groupData[1]);
                    cell.Value = groupData[1];
                    if (groupData[0] == "At Top")
                    {
                        optionGridView.Rows[e.RowIndex].Cells[9].Value = "0";
                    }
                    else
                    {
                        dr = Globals.cmdOptComp.Select("[Type] = '" + groupData[0] + "'");
                        optionGridView.Rows[e.RowIndex].Cells[9].Value = groupData[0];// FindInsertLocation(dr[0].Field<string>("Type"));
                    }
                    optionGridView.Rows[e.RowIndex].Cells[1].Value = "NONE";
                    optionGridView.Rows[e.RowIndex].Cells[2].Value = " ";
                    optionGridView.Rows[e.RowIndex].Cells[2].ReadOnly = true;
                    optionGridView.Rows[e.RowIndex].Cells[5].Value = " ";
                    optionGridView.Rows[e.RowIndex].Cells[3].ReadOnly = true;
                    optionGridView.Rows[e.RowIndex].Cells[4].ReadOnly = true;
                    optionGridView.Rows[e.RowIndex].Cells[5].ReadOnly = true;
                    for (int i = defaultCols; i < optionGridView.Columns.Count; ++i)
                    {
                        optionGridView.Rows[e.RowIndex].Cells[i].Value = "X";
                    }
                    optionGridView.Rows.Add();
                }
                else
                {
                    errors = true;
                    cell.FlatStyle = FlatStyle.Popup;
                    cell.Style.BackColor = Color.Red;
                    cell.ToolTipText = "You must select one option group to add this option to. If you need to add a new option group please select that option from this list.";
                }

                optionGridView.Invalidate();
            }
        }

    }
}
