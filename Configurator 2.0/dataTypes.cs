using System.Collections.Generic;
using System.Data;

namespace Configurator_2._0
{
    internal class option
    {
        public List<Component> optComps = new List<Component>();
        public List<string> optReqs = new List<string>();
        public string optType { get; set; }
        public string optName { get; set; }
        public string optSnDes { get; set; }
        public string checkName { get; set; }
        public string optDesc { get; set; }
        public string optFinDesc { get; set; }
        public string optFinSn { get; set; }
        public int optQty { get; set; }
    }

    internal class component
    {
        public component()
        {
            qty = 0;
            revision = "-";
        }

        public string partNumber { get; set; }
        public string revision { get; set; }
        public string partDescription { get; set; }
        public int typQty { get; set; }
        public int maxQty { get; set; }
        public string mrpType { get; set; }
        public string addType { get; set; }
        public string partType { get; set; }
        public string eePart { get; set; }
        public string partClass { get; set; }
        public int qty { get; set; }
    }

    internal class MachineData
    {
        public DataTable bom = new DataTable();
        public List<Component> bomComps = new List<Component>();
        public object[,] bomObj;
        public List<Component> lineComps = new List<Component>();
        public DataTable lines = new DataTable();
        public Component machComp = new Component();
        public List<string> salesOrders = new List<string>();
        public List<option> selOpts = new List<option>();
        public List<string> selectedOptionsList = new List<string>();

        public MachineData()
        {
            description = "";
            selectedOptionsList.Add("");
        }

        public string description { get; set; }
        public string prefix { get; set; }
        public string smartPartNumber { get; set; }
        public string epicorPartNumber { get; set; }
        public string partNumber { get; set; }
        public string modelName { get; set; }
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
    }

    internal class SimpleMachineData
    {
        public string id { get; set; }
        public List<SimplePartData> bom { get; set; }
        public List<SimplePartData> lineItems { get; set; }
        public string userAdded { get; set; }
        public List<string> salesOrders { get; set; }
        public string epicorPartNumber { get; set; }
        public string description { get; set; }
        public string dateConfigured { get; set; }
        public string lastConfigured { get; set; }
        public int timesConfigured { get; set; }
    }

    internal class SimplePartData
    {
        public string partNumber { get; set; }
        public string partDescription { get; set; }
        public string mrpType { get; set; }
        public string qty { get; set; }
    }
}