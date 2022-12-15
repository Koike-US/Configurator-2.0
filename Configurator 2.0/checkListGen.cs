using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EPDM.Interop.epdm;
using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;

namespace Configurator_2._0
{
    internal abstract class checkListGen
    {
        private const string CheckListSegmentLocation = @"W:\Engineering\Machine Configurator\Checklist Segments\";
        
        public static void WriteSp2()
        {
            string salesOrder = Globals.machine.soNum;
            if (salesOrder == "") return;
            try
            {
                Application ap = new Application();
                const string writePath = @"W:\Engineering\Machine Configurator\CONFIGURED CHECKLISTS\";
                string spCl = writePath + salesOrder + " CHECK LIST.doc";
                Document cl = null;
                Paragraph para = null;
                string endName = CheckListSegmentLocation + Globals.machine.checkEnd + ".doc";
                if (Globals.machine.checkName != "")
                {
                    string fName = CheckListSegmentLocation + Globals.machine.checkName + ".doc";
                    cl = ap.Documents.Open(fName);
                    para = cl.Content.Paragraphs.Add();
                    para.Range.InsertBreak(WdBreakType.wdPageBreak);
                }

                for (int i = 0; i < Globals.machine.selOpts.Count(); ++i)
                {
                    option opt = Globals.machine.selOpts[i];
                    if (string.IsNullOrEmpty(opt.checkName)) continue;
                    if (cl == null)
                    {
                        string fName = CheckListSegmentLocation + Globals.machine.selOpts[i].checkName + ".docx";
                        cl = ap.Documents.Open(fName);
                        para = cl.Content.Paragraphs.Add();
                        continue;
                    }

                    string name = CheckListSegmentLocation + opt.checkName + ".docx";
                    if (File.Exists(name)) para.Range.InsertFile(name);
                }

                para = cl.Content.Paragraphs.Add();
                para.Range.InsertBreak(WdBreakType.wdPageBreak);
                para.Range.InsertFile(endName);
                para = cl.Content.Paragraphs.Add();
                para.Range.InsertBreak(WdBreakType.wdPageBreak);
                para.Range.InsertFile(
                    @"W:\Engineering\Machine Configurator\Checklist Segments\New Machine Verification Checklist.docx");
                cl.SaveAs2(spCl);
                cl.Close();
            }
            catch
            {
                MessageBox.Show(
                    "Check List Not Generated or not Checked in!!!! \n If this was a test configuration you can ignore this error. \n If this is for a sales order, please check C:\\EPDM\\MANUALS\\CONFIGURED CHECKLISTS for the sales order labelled checklist.");
            }
        }
    }
}