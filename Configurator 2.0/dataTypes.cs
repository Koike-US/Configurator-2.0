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
    class machData
    {
        public string desc { get; set; }
        public string prefix { get; set; }
        public string smartNum { get; set; }
        public string dumNum { get; set; }
        public string partNum { get; set; }
        public string dwgName { get; set; }
        public string dwgSize { get; set; }
        public string revision { get; set; }
        public string dwgRev { get; set; }
        public string machName { get; set; }
        public string soNum { get; set; }
        public string checkName { get; set; }
        public string checkEnd { get; set; }
        public string partType { get; set; }
        public DataTable bom = new DataTable();
        public DataTable lines = new DataTable();
        public object[,] bomObj;
        public List<string> snList = new List<string>();
        public List<component> bomComps = new List<component>();
        public List<component> lineComps = new List<component>();
        public List<option> selOpts = new List<option>();
        public component machComp = new component();
        public machData()
        {
            desc = "";
            snList.Add("");
        }
           
    }
}
