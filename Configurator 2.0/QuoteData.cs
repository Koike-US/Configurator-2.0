using System;
using System.Collections.Generic;

namespace Configurator_2._0
{
    public class QuoteData
    {
        public int errCode { get; set; }
        public string errMsg { get; set; }
        public Data data { get; set; }
    }
    public class BillTo
    {
        public int accountID { get; set; }
        public string legalName { get; set; }
        public string commercialName { get; set; }
        public string legalID { get; set; }
        public string streetAddr1 { get; set; }
        public string streetAddr2 { get; set; }
        public int countryID { get; set; }
        public string country { get; set; }
        public int stateID { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string zipCode { get; set; }
        public int contactID { get; set; }
        public string contact { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
    }

    public class Data
    {
        public string id { get; set; }
        public string quoteExtID { get; set; }
        public string opportunityExtID { get; set; }
        public string opportunity { get; set; }
        public bool FmainOpportunity { get; set; }
        public string salesOrderExtID { get; set; }
        public int categoryID { get; set; }
        public string category { get; set; }
        public string shortDescription { get; set; }
        public string memo { get; set; }
        public int currencyID { get; set; }
        public string currency { get; set; }
        public int printCurrencyID { get; set; }
        public string printCurrency { get; set; }
        public double conversionRate { get; set; }
        public bool FfixedConversionRate { get; set; }
        public string printOptions { get; set; }
        public DateTime date { get; set; }
        public int salesAgentUID { get; set; }
        public string salesAgent { get; set; }
        public string status { get; set; }
        public int stageID { get; set; }
        public string stage { get; set; }
        public double pctProbability { get; set; }
        public int timeProbability { get; set; }
        public DateTime dateClosed { get; set; }
        public BillTo billTo { get; set; }
        public ShipTo shipTo { get; set; }
        public int priceBookID { get; set; }
        public string priceBook { get; set; }
        public int taxID { get; set; }
        public string taxName { get; set; }
        public double pctTax { get; set; }
        public int payTermsID { get; set; }
        public string payTerms { get; set; }
        public int payType { get; set; }
        public int payTimeSpan { get; set; }
        public string payTimeSpanUnit { get; set; }
        public double payPctAdvance { get; set; }
        public int warrantyID { get; set; }
        public string warranty { get; set; }
        public int warrTimeSpan { get; set; }
        public string warrTimeSpanUnit { get; set; }
        public int validTimeSpanID { get; set; }
        public int validTimeSpan { get; set; }
        public string validTimeSpanUnit { get; set; }
        public int deliveryTimeSpanID { get; set; }
        public int deliveryTimeSpan { get; set; }
        public string deliveryTimeSpanUnit { get; set; }
        public double multiplier { get; set; }
        public bool Fmultiplier { get; set; }
        public bool FcalcLabor { get; set; }
        public bool FcalcMaterials { get; set; }
        public bool FdraftApproval { get; set; }
        public bool FlineApproval { get; set; }
        public bool FglobalApproval { get; set; }
        public bool FcondApproval { get; set; }
        public bool FapprovalRequired { get; set; }
        public double subtotal { get; set; }
        public double totalDiscount { get; set; }
        public double totalOther { get; set; }
        public double totalTax { get; set; }
        public double total { get; set; }
        public double pctDiscount { get; set; }
        public bool FfixedDiscount { get; set; }
        public bool FfixedTotal { get; set; }
        public string taxList { get; set; }
        public string sumList { get; set; }
        public int createdByUID { get; set; }
        public DateTime dateCreated { get; set; }
        public string createdByUser { get; set; }
        public int modifiedByUID { get; set; }
        public string modifiedByUser { get; set; }
        public DateTime dateModified { get; set; }
        public List<Item> items { get; set; }
    }

    public class Item
    {
        public int id { get; set; }
        public string type { get; set; }
        public string itemID { get; set; }
        public string itemName { get; set; }
        public string itemType { get; set; }
        public double qty { get; set; }
        public double price { get; set; }
        public double priceStd { get; set; }
        public double costStd { get; set; }
        public int discountType { get; set; }
        public double pctDiscount { get; set; }
        public bool FfixedDiscount { get; set; }
        public double totalDiscount { get; set; }
        public double totalGlobalDiscount { get; set; }
        public double totalExtended { get; set; }
        public double totalCost { get; set; }
        public double maxLimit { get; set; }
        public double minLimit { get; set; }
        public bool Foptional { get; set; }
        public bool Ftax { get; set; }
        public int taxID { get; set; }
        public double pctTax { get; set; }
        public double totalTax { get; set; }
        public double zIndex { get; set; }
        public int sectionID { get; set; }
        public int laborID { get; set; }
        public int laborTypeID { get; set; }
        public double laborSalesRate { get; set; }
        public double laborCostRate { get; set; }
        public double laborQty { get; set; }
        public string laborUnit { get; set; }
        public string discountLegend { get; set; }
        public string locationLegend { get; set; }
        public string itemMemo { get; set; }
        public string @params { get; set; }
        public int? id_parent { get; set; }
    }
    public class ShipTo
    {
        public int accountID { get; set; }
        public string legalName { get; set; }
        public string commercialName { get; set; }
        public string streetAddr1 { get; set; }
        public string streetAddr2 { get; set; }
        public int countryID { get; set; }
        public string country { get; set; }
        public int stateID { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string zipCode { get; set; }
        public int contactID { get; set; }
        public string contact { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
    }



}