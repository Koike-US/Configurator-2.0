using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;

namespace Configurator_2._0
{
    class Globals
    {

        public static Utilities utils = new Utilities();
        public static MachineData machine = new MachineData();

        public static DataSet dataBase = new DataSet();
        public static DataTable machineData = new DataTable(); //This contains the base data of each machine type
        public static DataTable compData = new DataTable(); //This contains all of the data of each option component by part number
        public static DataTable cmdOptComp = new DataTable(); //This contains all of the option compatability data, and is the "heart" of the configurator
        public static DataTable confMachs = new DataTable(); //This contains all of the pre-configured Shop pro data, and new configurations will be written to

        public static int expRows = 30;

        public const string dbDir = @"W:\Engineering\Machine Configurator\";
        public const string dbName = "Configurator DB.xlsx";
        public const string dbFile = dbDir + dbName;
        public static bool prevConf = false;
        public static int foundRow = 0;



    }
    
}
