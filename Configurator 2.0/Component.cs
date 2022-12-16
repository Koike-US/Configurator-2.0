using System;
using MongoDB.Bson;

namespace Configurator_2._0
{
    public class Component
    {
        public string partNumber { get;  set; }
        public string revision { get;  set; }
        public string partDescription { get; set; }

        public string quantitySelect { get;  set; }
        public double typicalQuantity { get; set; }
        public double maxQuantity { get;  set; }
        public double quantityStep { get;  set; }
        public string mrpType { get;  set; }
        public string addType { get;  set; }
        public string partType { get;  set; }
        public double standardCost { get;  set; }
        public double listPrice { get;  set; }


        public Component(BsonDocument document)
        {
            partNumber = document.GetValue("Part Number").ToString();
            revision = document.GetValue("Revision").ToString();
            partDescription = document.GetValue("Part Description").ToString();
            quantitySelect = document.GetValue("Qty Select").ToString();
            typicalQuantity = StringToDouble(document.GetValue("Typ Qty").ToString());
            maxQuantity = StringToDouble(document.GetValue("Max Qty").ToString());
            quantityStep = StringToDouble(document.GetValue("Qty Step").ToString());
            mrpType = document.GetValue("MRP Type").ToString();
            addType = document.GetValue("Add Type").ToString();
            partType = document.GetValue("Part Type").ToString();
            standardCost = StringToDouble(document.GetValue("Standard Cost").ToString());
            listPrice = StringToDouble(document.GetValue("Est List Price").ToString());
        }
        
        public Component()
        {
            
        }

        private double StringToDouble(string input)
        {
            if (string.IsNullOrEmpty(input) || input == "-")
                return 0;

            return double.Parse(input);

        }
    }
}