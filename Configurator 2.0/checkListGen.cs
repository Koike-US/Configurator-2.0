using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EPDM.Interop.epdm;
using Microsoft.Office.Interop.Word;
using Application = Microsoft.Office.Interop.Word.Application;

namespace Configurator_2._0
{
    internal class checkListGen
    {
        public static string clLoc = @"C:\EPDM\MANUALS\Checklist Segments\";

        public static string clLoc2 = @"W:\Engineering\Machine Configurator\Checklist Segments\";

        private static EdmVault5 _vault = new EdmVault5();
        private static IEdmFolder5 _fol;

        public static void WriteSp()
        {
            string salesOrder = Globals.machine.soNum;
            if (salesOrder == "") return;
            _vault.LoginAuto("EPDM", 0);
            new List<string>();

            Application ap = new Application();
            string writePath = @"C:\EPDM\MANUALS\CONFIGURED CHECKLISTS\";
            string spCl = writePath + salesOrder + " CHECK LIST.doc";
            Document cl = null;
            Paragraph para = null;
            IEdmFile5 f;
            string name = "";
            string endName = clLoc + Globals.machine.checkEnd + ".doc";
            if (Globals.machine.checkName != "")
            {
                string fName = clLoc + Globals.machine.checkName + ".doc";
                f = _vault.GetFileFromPath(fName, out _fol);
                if (f == null) return;
                f.GetFileCopy(0);
                cl = ap.Documents.Open(fName);
                para = cl.Content.Paragraphs.Add();
                para.Range.InsertBreak(WdBreakType.wdPageBreak);
            }

            for (int i = 0; i < Globals.machine.selOpts.Count(); ++i)
            {
                option opt = Globals.machine.selOpts[i];
                if (opt.checkName != "" && opt.checkName != null)
                {
                    if (cl == null)
                    {
                        string fName = clLoc + Globals.machine.selOpts[i].checkName + ".docx";
                        f = _vault.GetFileFromPath(fName, out _fol);
                        f.GetFileCopy(0);
                        cl = ap.Documents.Open(fName);
                        para = cl.Content.Paragraphs.Add();
                        continue;
                    }

                    name = clLoc + opt.checkName + ".docx";
                    f = _vault.GetFileFromPath(name, out _fol);
                    f.GetFileCopy(0);
                    if (File.Exists(name)) para.Range.InsertFile(name);
                }
            }

            para = cl.Content.Paragraphs.Add();
            para.Range.InsertBreak(WdBreakType.wdPageBreak);
            f = _vault.GetFileFromPath(endName, out _fol);
            f.GetFileCopy(0);
            para.Range.InsertFile(endName);
            para = cl.Content.Paragraphs.Add();
            para.Range.InsertBreak(WdBreakType.wdPageBreak);
            f = _vault.GetFileFromPath(@"C:\EPDM\MANUALS\CHECKLISTS-CMD\New Machine Verification Checklist.docx",
                out _fol);
            f.GetFileCopy(0);
            para.Range.InsertFile(@"C:\EPDM\MANUALS\CHECKLISTS-CMD\New Machine Verification Checklist.docx");
            cl.SaveAs2(spCl);
            cl.Close();

            try
            {
                IEdmFile5 file = default(IEdmFile5);
                _fol = _vault.GetFolderFromPath(writePath);
                _fol.AddFile(0, spCl, null);
                file = _vault.GetFileFromPath(spCl, out _fol);
                file.UnlockFile(0, "", 42);
            }
            catch
            {
                MessageBox.Show(
                    "Check List Not Generated or not Checked in!!!! \n If this was a test configuration you can ignore this error. \n If this is for a sales order, please check C:\\EPDM\\MANUALS\\CONFIGURED CHECKLISTS for the sales order labelled checklist.");
            }
        }

        public static void WriteSp2()
        {
            string salesOrder = Globals.machine.soNum;
            if (salesOrder == "") return;
            try
            {
                new List<string>();

                Application ap = new Application();
                string writePath = @"W:\Engineering\Machine Configurator\CONFIGURED CHECKLISTS\";
                string spCl = writePath + salesOrder + " CHECK LIST.doc";
                Document cl = null;
                Paragraph para = null;
                string name = "";
                string endName = clLoc2 + Globals.machine.checkEnd + ".doc";
                if (Globals.machine.checkName != "")
                {
                    string fName = clLoc2 + Globals.machine.checkName + ".doc";
                    cl = ap.Documents.Open(fName);
                    para = cl.Content.Paragraphs.Add();
                    para.Range.InsertBreak(WdBreakType.wdPageBreak);
                }

                for (int i = 0; i < Globals.machine.selOpts.Count(); ++i)
                {
                    option opt = Globals.machine.selOpts[i];
                    if (opt.checkName != "" && opt.checkName != null)
                    {
                        if (cl == null)
                        {
                            string fName = clLoc2 + Globals.machine.selOpts[i].checkName + ".docx";
                            cl = ap.Documents.Open(fName);
                            para = cl.Content.Paragraphs.Add();
                            continue;
                        }

                        name = clLoc2 + opt.checkName + ".docx";
                        if (File.Exists(name)) para.Range.InsertFile(name);
                    }
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

        public static string SearchPdm(string folName)
        {
            string path = "";
            string file = folName + ".SO-link.cvd";
            if (File.Exists(folName) == false)
            {
                SetVault();
                IEdmSearch5 search = _vault.CreateSearch();

                search.FileName = file;
                IEdmSearchResult5 res = search.GetFirstResult();
                if (res != null)
                    path = Path.GetDirectoryName(res.Path) + "\\";
                else
                    path = clLoc;
            }

            return path;
        }

        public static void SetVault()
        {
            if (_vault == null || _vault.IsLoggedIn == false) _vault = new EdmVault5();
            if (_vault.IsLoggedIn == false)
                try
                {
                    _vault.LoginAuto("EPDM", 0);
                }
                catch
                {
                }
        }
    }
}