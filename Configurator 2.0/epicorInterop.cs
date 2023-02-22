using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace Stand_Alone_Solidworks_Interface
{
    internal class epicorInterop
    {
        //private List<string> constants = new List<string>(File.ReadAllLines(@"G:\Customer Service\Python\constants.py"));
        //private Dictionary<string,string> derp = File.ReadAllLines(@"G:\Customer Service\Python\constants.py").ToDictionary(p => p.Split(new string[] { " = " }, StringSplitOptions.None)[0]);

        
        public string baseURL = "https://centralusdtapp01.epicorsaas.com/saas515/api/v2/odata/19565/";
        public string contentType = "application/json";

        public static string apiKeyHeader = "";//"x-api-key:lKTxHNMKP9xdPjBwgr4SBPxZuLdcVdmeUN0pGRAdLbgd8";
        public static string networkID = "";
        public static string networkPass = "";

        NetworkCredential cred;

        public epicorInterop()
        {
            string[] constants = File.ReadAllLines(@"G:\Customer Service\Python\constants.py");
            foreach(string s in constants)
            {
                string[] s2 = s.Replace("'","").Split(new[] { " = " }, StringSplitOptions.None);
                switch (s2[0]) 
                {
                    case "EPICOR_LIVE_API_KEY":
                        apiKeyHeader = "x-api-key:" + s2[1];
                        break;
                    case "EPICOR_USERNAME":
                        networkID = s2[1];
                        break;
                    case "EPICOR_USER_PASS":
                        networkPass = s2[1];
                        break;
                }
            }
            cred = new NetworkCredential(networkID, networkPass);
            return;
        }
        public Dictionary<string, string> getPartData(string pn)
        {
            string[] output = new string[10];
            Dictionary<string, string> Revision = getLatestPartRevision(pn);
            Dictionary<string, string> PartData = new Dictionary<string, string>();
            //string url = "https://centralusdtapp01.epicorsaas.com/saas515/api/v1/Erp.BO.PartSvc/Parts(19565," + pn + ")";
            string url = "https://centralusdtapp01.epicorsaas.com/saas515/api/v1/Erp.BO.PartSvc/Parts?$filter=+PartNum+eq+'" + pn + "'";
            string strData = epicorRequest(url);


            if (strData.Contains("PartNum") == true)
            {
                string[] ding = strData.Split(new string[] { "{", "}" }, StringSplitOptions.None);
                List<Dictionary<string, string>> RevisionList = new List<Dictionary<string, string>>();
                foreach (string s in ding)
                {
                    if (s.Contains("PartNum") == true)
                    {
                        PartData =  Revision.Concat(GetPartData(s)).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());
                    }
                }
            }
            return PartData;
        }
        public Dictionary<string, string> getLatestPartRevision(string pn)
        {
            string url = @"https://centralusdtapp01.epicorsaas.com/saas515/api/v1/Erp.BO.PartRevSearchSvc/PartRevSearches?$filter=+PartNum+eq+'" + pn + "'";
            string strData = epicorRequest(url);
            Dictionary<string, string> Revision = null;
            if (strData != null)
            {
                string[] ding = strData.Split(new string[] { "{", "}" }, StringSplitOptions.None);
                int lastRev = 0;
                int i = 0;
                foreach (string s1 in ding)
                {
                    if (s1.Contains(@",""RevisionNum") == true)
                    {
                        lastRev = i;
                    }
                    ++i;
                }
                Revision = new Dictionary<string, string>();
                if (ding[lastRev].Contains("PartNum") == true)
                {
                    Revision = GetPartData(ding[lastRev]);
                }
            }
            return Revision;
        }

        private string epicorRequest(string url)
        {
            //url = @"https://centralusdtapp01.epicorsaas.com/saas515/api/v1/Erp.BO.PartRevSearchSvc/PartRevSearches?$filter=+PartNum+eq+'1148123900'";
            //url = @"https://centralusdtapp01.epicorsaas.com/saas515/api/v1/Erp.BO.PartSvc/Parts?$filter=+PartNum+eq+'1148123900'";

            List<string> strData = new List<string>();
            WebRequest req = WebRequest.Create(url);
            HttpWebResponse response;
            req.Headers.Add(apiKeyHeader);
            req.PreAuthenticate = true;
            req.ContentType = "application/json";
            req.Credentials = cred;
            var data = "";
            try
            {
                response = (HttpWebResponse)req.GetResponse();
                Stream recvStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(recvStream, Encoding.ASCII);
                data = readStream.ReadToEnd();





            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);                
            }
            req = null;
            response = null;    
            return data;
        }

        private Dictionary<string,string> GetPartData(string DataIn)
        {
            Dictionary<string, string> Data = new Dictionary<string, string>();
            string data = "{\"Data1\":\"DataVal\"}";
            data = "{" + DataIn + "}";
            Data = JsonConvert.DeserializeObject<Dictionary<string,string>>("{" + DataIn + "}");

            return Data;
        }
        public class BomComp
        {

            //Epicor Data Fields in Order
            //Number	PartDescription	Epicor_Mfgcomment	Epicor_Purcomment	Epicor_Mfg_name	Epicor_MFGPartNum	Epicor_RandD_c	Epicor_createdbylegacy_c	Epicor_PartType_c	Epicor_EngComment_c	Epicor_Confreq_c	Epicor_EA_MANF_c	Epicor_EA_Volts_c	Epicor_EA_Phase_c	Epicor_EA_Freq_c	Epicor_EA_FLA_Supply_c	
            //Epicor_EA_FLA_LgMot_c	Epicor_EA_ProtDevRating_c	Epicor_EA_PannelSCCR_c	Epicor_EA_EncRating_c	Revision	Epicor_RevisionDescription	Dwg. Rev.	Epicor_FullRel_c	Reference Count	Drawing #	Machine Model	Electrical Part	Drawing Size	Sheet Count
            public string pNum { get; set; }
            public string pDesc { get; set; }
            public string mfgComm { get; set; }
            public string purComm { get; set; }
            public string mfgName { get; set; }
            public string mfgPNum { get; set; }
            public string randD { get; set; }
            public string createBy { get; set; }
            public string pType { get; set; }
            public string engComm { get; set; }

            //EE only fields
            public string confReq = "";
            public string eaManf = "";
            public string eaVolts = "";
            public string eaPhase = "";
            public string eaFreq = "";
            public string eaFLA = "";
            public string flaLgMot = "";
            public string eaProtDevRat = "";
            public string eaPanSCCR = "";
            public string eaEncRat = "";
            //EE only fields

            public string rev { get; set; }
            public string revDesc { get; set; }
            public string dwgRev { get; set; }
            public string fullRel { get; set; }
            public string partQty { get; set; }
            public string dwgNum { get; set; }
            public string machModel { get; set; }
            public string eePart { get; set; }
            public string dwgSize { get; set; }
            public string shtCount { get; set; }
            //Epicor Data Fields in Order

            public double qty { get; set; }
            public bool pkgPart { get; set; }
            public string pkgName { get; set; }
            public string pkgDesc { get; set; }
            public string pathName { get; set; }
            public string parentNumber { get; set; }
            public string bomName { get; set; }


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






            public string[] pData { get; set; }

            public List<string> epicorRevs { get; set; }

            public int compLevel { get; set; }

            public BomComp()
            {
                return;
            }
            public BomComp(IGrouping<string, BomComp> bComp)
            {
                qty = bComp.ElementAt(0).qty;
                pNum = bComp.ElementAt(0).pNum;
                pDesc = bComp.ElementAt(0).pDesc;
                mfgComm = bComp.ElementAt(0).mfgComm;
                purComm = bComp.ElementAt(0).purComm;
                mfgName = bComp.ElementAt(0).mfgName;
                mfgPNum = bComp.ElementAt(0).mfgPNum;
                randD = bComp.ElementAt(0).randD;
                createBy = bComp.ElementAt(0).createBy;
                pType = bComp.ElementAt(0).pType;
                engComm = bComp.ElementAt(0).engComm;
                rev = bComp.ElementAt(0).rev;
                revDesc = bComp.ElementAt(0).revDesc;
                dwgRev = bComp.ElementAt(0).dwgRev;
                fullRel = bComp.ElementAt(0).fullRel;
                partQty = bComp.ElementAt(0).partQty;
                dwgNum = bComp.ElementAt(0).dwgNum;
                machModel = bComp.ElementAt(0).machModel;
                eePart = bComp.ElementAt(0).eePart;
                dwgSize = bComp.ElementAt(0).dwgSize;
                shtCount = bComp.ElementAt(0).shtCount;

                epicorRev = bComp.ElementAt(0).epicorRev;
                epicorDesc = bComp.ElementAt(0).epicorDesc;
                epicorMfgName = bComp.ElementAt(0).epicorMfgName;
                epicorMrpType = bComp.ElementAt(0).epicorMrpType;



                if (string.IsNullOrEmpty(bComp.ElementAt(0).pkgName) == false)
                {
                    if (bComp.ElementAt(0).pkgName != null && bComp.ElementAt(0).pkgName != pNum)//&& string.IsNullOrWhiteSpace(bComp.ElementAt(0).mfgComm) == false)
                    {
                        mfgComm = mfgComm + "\n" + bComp.ElementAt(0).mfgComm;
                        try
                        {
                            pDesc = pDesc.ToUpper().Substring(0, pDesc.ToUpper().IndexOf(", PART"));
                        }
                        catch
                        {
                            try
                            {
                                pDesc = pDesc.ToUpper().Substring(0, pDesc.ToUpper().IndexOf(" PART"));
                            }
                            catch { }
                        }
                        pkgName = bComp.ElementAt(0).pkgName;
                        partQty = "1";
                        pNum = bComp.ElementAt(0).pkgName;//pNum = dwgNum = bComp.ElementAt(0).pkgName;
                        dwgNum = bComp.ElementAt(0).pkgName;
                    }
                }

                return;
            }
            public void setComp(string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9, string p10, string p11, string p12, string p13, string p14, string p15, string p16, string p17, string p18, string p19, string p20)
            {
                qty = Convert.ToDouble(p15);
                pNum = p1;
                pDesc = p2;
                mfgComm = p3;
                purComm = p4;
                mfgName = p5;
                mfgPNum = p6;
                randD = p7;
                createBy = p8;
                pType = p9;
                engComm = p10;
                rev = p11;
                revDesc = p12;
                dwgRev = p13;
                fullRel = p14;
                partQty = p15;
                dwgNum = p16;
                machModel = p17;
                eePart = p18;
                dwgSize = p19;
                shtCount = p20;
                return;
            }
            public string[] getArray()
            {
                return new string[] { pNum, pDesc, mfgComm, purComm, mfgName, mfgPNum, randD, createBy, pType, engComm, "", "", "", "", "", "", "", "", "", "", rev, revDesc, dwgRev, fullRel, qty.ToString(), dwgNum, machModel, eePart, dwgSize, shtCount };
            }
            public void incQty(string qtyIn)
            {
                qty = qty + Convert.ToDouble(qtyIn);
            }


        }

    }
}
