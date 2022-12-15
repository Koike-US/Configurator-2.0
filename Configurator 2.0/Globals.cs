using System.Data;
using MongoDB.Driver;

namespace Configurator_2._0
{
    internal class Globals
    {
        private const string DbDir = @"W:\Engineering\Machine Configurator\";
        private const string DbName = "Configurator DB.xlsx";
        public const string DbFile = DbDir + DbName;

        public static Utilities utils = new Utilities();
        public static MachineData machine = new MachineData();

        public static DataSet dataBase = new DataSet();
        public static DataTable machineData = new DataTable(); //This contains the base data of each machine type

        public static DataTable
            compData = new DataTable(); //This contains all of the data of each option component by part number

        public static DataTable
            cmdOptComp =
                new DataTable(); //This contains all of the option compatability data, and is the "heart" of the configurator

        public static DataTable
            confMachs =
                new DataTable(); //This contains all of the pre-configured Shop pro data, and new configurations will be written to

        public static DataTable
            machineOptComp =
                new DataTable(); //This contains all of the option compatability data for the currently selected machine, and is the "heart" of the configurator

        public static int expRows = 30;
        public static bool prevConf = false;
        public static int foundRow = 0;

        public IMongoDatabase webdatabase { get; set; } // = dbClient.GetDatabase("configurator_db");
    }
}