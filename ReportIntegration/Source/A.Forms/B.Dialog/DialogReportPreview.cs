using System;
//using System.Windows;
using System.Windows.Forms;
using System.Diagnostics;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraReports.UI;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using WordToPDF;
using System.IO;

namespace Sgs.ReportIntegration
{
    public partial class DialogReportPreview : XtraForm
    {
        public string DefFName { get; set; }

        public object Source
        {
            get { return docPreview.DocumentSource; }
            set { docPreview.DocumentSource = value; }
        }

        public DialogReportPreview()
        {
            InitializeComponent();
        }

        private void ReportWork_Load(object sender, EventArgs e)
        {
        }

        private void nBIofficeList_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            //ReportForm rpt = new ReportForm();
            //rpt.CreateDocument();
            //ReportPrintTool tool = new ReportPrintTool(rpt);
            //documentViewer1.DocumentSource = rpt;
        }

        private void barBtnWordtoWord_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //string strappname = Application.StartupPath + "\\wordtoword.application";
            //string strappname = "C:\\Users\\hiel_kim\\OneDrive - SGS\\Documents\\All\\Aurora\\wordtoword\\wordtoword\\wordtoword\\publish\\wordtoword.application";
            string strappname = @"\\10.153.4.98\KR_Automation\SourceCode\Aurora\wordtoword.application";
            Process.Start(strappname);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //폴더 경로를 입력
            string Path_EN   = @"C:\Projects\Projects\Sgs\Remote_One\ReportIntegration\ReportIntegration\Bom\EN_Physical";
            string Path_ASTM = @"C:\Projects\Projects\Sgs\Remote_One\ReportIntegration\ReportIntegration\Bom\ASTM_Physical";
            string[] files_EN = Directory.GetFiles(@"C:\Projects\Projects\Sgs\Remote_One\ReportIntegration\ReportIntegration\Bom\\EN_Physical_pdf", "*.pdf");
            string[] files_ASTM = Directory.GetFiles(@"C:\Projects\Projects\Sgs\Remote_One\ReportIntegration\ReportIntegration\Bom\\ASTM_Physical_pdf", "*.pdf");
            string FileExtension = "";
            string ChangeExtension = "";
            object FromLocation = "";
            object ToLocation = "";
            bool bCheckPdf = false;

            //EN 폴더가 존재하는지 확인
            if (System.IO.Directory.Exists(Path_EN))
            {
                //DirectoryInfo 객체 생성
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Path_EN);

                //해당 폴더에 있는 파일이름을 출력
                foreach (System.IO.FileInfo File in di.GetFiles())
                {
                    // Word2PDF 객체 생성
                    Word2Pdf objWorPdf = new Word2Pdf();
                    bCheckPdf = false;

                    if (File.Extension.ToLower().Equals(".doc") || File.Extension.ToLower().Equals(".docx"))
                    {
                        bCheckPdf = true;
                        FileExtension = Path.GetExtension(File.Name);
                        ChangeExtension = File.Name.Replace(FileExtension, ".pdf");

                        FromLocation = Path_EN + @"\" + File.Name;
                        ToLocation = Path_EN + @"_pdf\" + ChangeExtension;

                        foreach (string file_EN in files_EN)
                        {
                            if (ToLocation.Equals(file_EN))
                            {
                                bCheckPdf = false;
                                break;
                            }
                        }
                    }

                    // PDF 파일이 없으면 Word to PDF
                    if (bCheckPdf)
                    {
                        objWorPdf.InputLocation = FromLocation;
                        objWorPdf.OutputLocation = ToLocation;
                        objWorPdf.Word2PdfCOnversion();
                    }
                }
            }

            //ASTM 폴더가 존재하는지 확인
            if (System.IO.Directory.Exists(Path_ASTM))
            {
                bCheckPdf = false;

                //DirectoryInfo 객체 생성
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Path_ASTM);

                //해당 폴더에 있는 파일이름을 출력
                foreach (System.IO.FileInfo File in di.GetFiles())
                {
                    // Word2PDF 객체 생성
                    Word2Pdf objWorPdf = new Word2Pdf();
                    bCheckPdf = false;

                    if (File.Extension.ToLower().Equals(".doc") || File.Extension.ToLower().Equals(".docx"))
                    {
                        bCheckPdf = true;
                        FileExtension = Path.GetExtension(File.Name);
                        ChangeExtension = File.Name.Replace(FileExtension, ".pdf");

                        FromLocation = Path_ASTM + "\\" + File.Name;
                        ToLocation = Path_ASTM + "_pdf\\" + ChangeExtension;

                        foreach (string file_ASTM in files_ASTM)
                        {
                            if (ToLocation.Equals(file_ASTM))
                            {
                                bCheckPdf = false;
                                break;
                            }
                        }
                    }

                    // PDF 파일이 없으면 Word to PDF
                    if (bCheckPdf)
                    {
                        objWorPdf.InputLocation = FromLocation;
                        objWorPdf.OutputLocation = ToLocation;
                        objWorPdf.Word2PdfCOnversion();
                    }
                }
            }
            MessageBox.Show("PDF 변환이 완료되었습니다.");
        }
    }
}