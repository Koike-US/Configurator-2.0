using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Configurator_2._0
{
    class dataTypes
    {
    }
    class option
    {
        public string optType { get; set; }
        public string optName { get; set; }
        public string optSnDes { get; set; }
        public string checkName { get; set; }
        public string optDesc { get; set; }
        public string optFinDesc { get; set; }
        public string optFinSn { get; set; }
        public int optQty { get; set; }
        public List<component> optComps = new List<component>();
        public List<string> optReqs = new List<string>();

    }
    class component
    {
        public string number { get; set; }
        public string revision { get; set; }
        public string desc { get; set; }
        public int typQty { get; set; }
        public int maxQty { get; set; }
        public string mrpType { get; set; }
        public string addType { get; set; }
        public string partType { get; set; }
        public string eePart { get; set; }
        public string partClass { get; set; }
        public int qty { get; set; }

        public string epicorRev { get; set; } = "";
        public string epicorRevDescription { get; set; } = "";
        public string epicorDrawNum { get; set; } = "";
        public string epicorDrawRev { get; set; } = "";
        public string epicorDrawSize { get; set; } = "";
        public string epicorDrawSheetCount { get; set; } = "";
        public string epicorFullRelease { get; set; } = "";


        public string epicorDesc { get; set; }
        public string epicorMfgName { get; set; }
        public string epicorMrpType { get; set; }
        public component()
        {
            qty = 0;
            revision = "-";
        }
    }
    class cmdData
    {
        public int cutWidth { get; set; }
        public int cutLength { get; set; }
        public int machinWidth { get; set; }
        public int machLength { get; set; }
        public string machWeight { get; set; }
    }
    class MachineData
    {
        public string description { get; set; }
        public string prefix { get; set; }
        public string SmartPartNumber { get; set; }
        public string EpicorPartNumber { get; set; }
        public string PartNumber { get; set; }
        public string ModelName { get; set; }
        public string drawingName { get; set; }
        public string drawingSize { get; set; }
        public string revision { get; set; }
        public string dwgRev { get; set; }
        public string machName { get; set; }
        public string machCode { get; set; }
        public string soNum { get; set; }
        public string checkName { get; set; }
        public string checkEnd { get; set; }
        public string partType { get; set; }
        public int timesConfigured { get; set; }
        public string configuredDate { get; set; }
        public string configuredBy { get; set; }
        public DataTable bom = new DataTable();
        public DataTable lines = new DataTable();
        public object[,] bomObj;
        public List<string> snList = new List<string>();
        public List<String> salesOrders = new List<string>();
        public List<component> bomComps = new List<component>();
        public List<component> lineComps = new List<component>();
        public List<option> selOpts = new List<option>();
        public component machComp = new component();
        public MachineData()
        {
            description = "";
            snList.Add("");
        }
           
    }

    class SimpleMachineData
    {
        public string _id { get; set; }
        public List<SimplePartData> BOM { get; set; }
        public List<SimplePartData> Line_Items { get; set; }
        public string User_Added { get; set; }
        public List<String> Sales_Orders { get; set; }
        public string Epicor_Part_Number { get; set; }
        public string Description { get; set; }
        public string Date_Configured { get; set; }
        public string Last_Configured { get; set; }
        public int Times_Configured { get; set; }
    }

    class SimplePartData
    {
        public string Part_Number { get; set; }
        public string Part_Description { get; set; }
        public string MRP_Type { get; set; }
        public string Qty { get; set; }
    }
}
