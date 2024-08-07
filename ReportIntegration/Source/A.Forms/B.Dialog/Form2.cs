using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Xls;
using System.Linq;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using System.IO;
using System.Globalization;

using System.Data.OleDb;

namespace Sgs.ReportIntegration.Source.A.Forms.B.Dialog
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private OleDbConnection Conn;
        // If we launched from the Send To menu,
        // use the argument as our directory.

        private void Form2_Load(object sender, EventArgs e)
        {
            //txtStartDirectory.Text = Properties.Settings.Default.StartDirectory;
           // cboPattern2.Text = Properties.Settings.Default.Pattern;

            if (System.Environment.GetCommandLineArgs().Length > 1)
            {
                txtDirectory.Text = System.Environment.GetCommandLineArgs()[1];
                txtTarget.Focus();
            }


        }

        // Save current settings.
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Properties.Settings.Default.StartDirectory = txtStartDirectory.Text;
            //Properties.Settings.Default.Pattern = cboPattern2.Text;
            Properties.Settings.Default.Save();
        }

        // Search.
        private void ListStudents()
        {
            string query = "SELECT pro_job, pro_proj, orderno " +
                "FROM profjob " +
                "WHERE PRO_PROJ LIKE 'AYH%' " +
                "  AND received >= '2015-01-18'" +
                "  ORDER BY PRO_PROJ";
            OleDbCommand cmd = new OleDbCommand(query, Conn);

            Conn.Open();
            using (OleDbDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    SLIM projob = new SLIM(
                        (string)reader.GetValue(0),
                        (string)reader.GetValue(1),
                        (string)reader.GetValue(2));
                    cboStudents.Items.Add(projob);
                }
            }
            Conn.Close();
        }

        // Add the files in this directory's subtree 
        // that match the pattern to the ListBox.
        private void ListFiles(ListBox lst, string pattern, DirectoryInfo dir_info, string target)
        {
            // Get the files in this directory.
            FileInfo[] fs_infos = dir_info.GetFiles(pattern);
            foreach (FileInfo fs_info in fs_infos)
            {
                if (target.Length == 0)
                {
                    lstResults.Items.Add(fs_info.FullName);
                }
                else
                {
                    string txt = File.ReadAllText(fs_info.FullName);
                    if (txt.IndexOf(target, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        lstResults.Items.Add(fs_info.FullName);
                    }
                }
            }

            // Search subdirectories.
            DirectoryInfo[] subdirs = dir_info.GetDirectories();
            foreach (DirectoryInfo subdir in subdirs)
            {
                ListFiles(lst, pattern, subdir, target);
            }
        }
        private void btnRead_Click(object sender, EventArgs e)
        {
            string fileName = OpenFile();
            string fileMerge = OpenFile();
            if ((!string.IsNullOrEmpty(fileName)) && (!string.IsNullOrEmpty(fileMerge)))
            {
                //Create word document
                Document document = new Document();
                document.LoadFromFile(fileName,  Spire.Doc.FileFormat.Doc);

                Document documentMerge = new Document();
                documentMerge.LoadFromFile(fileMerge,  Spire.Doc.FileFormat.Doc);

                foreach (Section sec in documentMerge.Sections)
                {
                    document.Sections.Add(sec.Clone());
                }

                //Save doc file.
                document.SaveToFile("Sample.doc",  Spire.Doc.FileFormat.Doc);

                //Launching the MS Word file.
                WordDocViewer("Sample.doc");
            }


        }
        private string OpenFile()
        {
            openFileDialog1.InitialDirectory = "K:\\SL&HL\\HL\\★ REPORT_FINAL\\BOM LIST\\AURORA ASTM";
            openFileDialog1.Filter = "Excel Document (*.xls*)|*.xls*";
            openFileDialog1.Title = "Choose a document";

            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }

            return string.Empty;
        }

        private void WordDocViewer(string fileName)
        {
            try
            {
                System.Diagnostics.Process.Start(fileName);
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            object oMissing = System.Reflection.Missing.Value;
            object oEndOfDoc = "\\endofdoc"; /* \endofdoc is a predefined bookmark */

            //Start Word and create a new document.
            Word._Application oWord;
            Word._Document oDoc;
            oWord = new Word.Application();
            oWord.Visible = true;
            oDoc = oWord.Documents.Add(ref oMissing, ref oMissing,
                ref oMissing, ref oMissing);

            //Insert a paragraph at the beginning of the document.
            Word.Paragraph oPara1;
            oPara1 = oDoc.Content.Paragraphs.Add(ref oMissing);
            oPara1.Range.Text = "Heading 1";
            oPara1.Range.Font.Bold = 1;
            oPara1.Format.SpaceAfter = 24;    //24 pt spacing after paragraph.
            oPara1.Range.InsertParagraphAfter();

            //Insert a paragraph at the end of the document.
            Word.Paragraph oPara2;
            object oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara2 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara2.Range.Text = "Heading 2";
            oPara2.Format.SpaceAfter = 6;
            oPara2.Range.InsertParagraphAfter();

            //Insert another paragraph.
            Word.Paragraph oPara3;
            oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara3 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara3.Range.Text = "This is a sentence of normal text. Now here is a table:";
            oPara3.Range.Font.Bold = 0;
            oPara3.Format.SpaceAfter = 24;
            oPara3.Range.InsertParagraphAfter();

            //Insert a 3 x 5 table, fill it with data, and make the first row
            //bold and italic.
            Word.Table oTable;
            Word.Range wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oTable = oDoc.Tables.Add(wrdRng, 3, 5, ref oMissing, ref oMissing);
            oTable.Range.ParagraphFormat.SpaceAfter = 6;
            int r, c;
            string strText;
            for (r = 1; r <= 3; r++)
                for (c = 1; c <= 5; c++)
                {
                    strText = "r" + r + "c" + c;
                    oTable.Cell(r, c).Range.Text = strText;
                }
            oTable.Rows[1].Range.Font.Bold = 1;
            oTable.Rows[1].Range.Font.Italic = 1;

            //Add some text after the table.
            Word.Paragraph oPara4;
            oRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oPara4 = oDoc.Content.Paragraphs.Add(ref oRng);
            oPara4.Range.InsertParagraphBefore();
            oPara4.Range.Text = "And here's another table:";
            oPara4.Format.SpaceAfter = 24;
            oPara4.Range.InsertParagraphAfter();

            //Insert a 5 x 2 table, fill it with data, and change the column widths.
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oTable = oDoc.Tables.Add(wrdRng, 5, 2, ref oMissing, ref oMissing);
            oTable.Range.ParagraphFormat.SpaceAfter = 6;
            for (r = 1; r <= 5; r++)
                for (c = 1; c <= 2; c++)
                {
                    strText = "r" + r + "c" + c;
                    oTable.Cell(r, c).Range.Text = strText;
                }
            oTable.Columns[1].Width = oWord.InchesToPoints(2); //Change width of columns 1 & 2
            oTable.Columns[2].Width = oWord.InchesToPoints(3);

            //Keep inserting text. When you get to 7 inches from top of the
            //document, insert a hard page break.
            object oPos;
            double dPos = oWord.InchesToPoints(7);
            oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range.InsertParagraphAfter();
            do
            {
                wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                wrdRng.ParagraphFormat.SpaceAfter = 6;
                wrdRng.InsertAfter("A line of text");
                wrdRng.InsertParagraphAfter();
                oPos = wrdRng.get_Information
                               (Word.WdInformation.wdVerticalPositionRelativeToPage);
            }
            while (dPos >= Convert.ToDouble(oPos));
            object oCollapseEnd = Word.WdCollapseDirection.wdCollapseEnd;
            object oPageBreak = Word.WdBreakType.wdPageBreak;
            wrdRng.Collapse(ref oCollapseEnd);
            wrdRng.InsertBreak(ref oPageBreak);
            wrdRng.Collapse(ref oCollapseEnd);
            wrdRng.InsertAfter("We're now on page 2. Here's my chart:");
            wrdRng.InsertParagraphAfter();

            //Insert a chart.
            Word.InlineShape oShape;
            object oClassType = "MSGraph.Chart.8";
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            oShape = wrdRng.InlineShapes.AddOLEObject(ref oClassType, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing);

            //Demonstrate use of late bound oChart and oChartApp objects to
            //manipulate the chart object with MSGraph.
            object oChart;
            object oChartApp;
            oChart = oShape.OLEFormat.Object;
            oChartApp = oChart.GetType().InvokeMember("Application",
                BindingFlags.GetProperty, null, oChart, null);

            //Change the chart type to Line.
            object[] Parameters = new Object[1];
            Parameters[0] = 4; //xlLine = 4
            oChart.GetType().InvokeMember("ChartType", BindingFlags.SetProperty,
                null, oChart, Parameters);

            //Update the chart image and quit MSGraph.
            oChartApp.GetType().InvokeMember("Update",
                BindingFlags.InvokeMethod, null, oChartApp, null);
            oChartApp.GetType().InvokeMember("Quit",
                BindingFlags.InvokeMethod, null, oChartApp, null);
            //... If desired, you can proceed from here using the Microsoft Graph 
            //Object model on the oChart and oChartApp objects to make additional
            //changes to the chart.

            //Set the width of the chart.
            oShape.Width = oWord.InchesToPoints(6.25f);
            oShape.Height = oWord.InchesToPoints(3.57f);

            //Add text after the chart.
            wrdRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            wrdRng.InsertParagraphAfter();
            wrdRng.InsertAfter("THE END.");

            //Close this form.
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Create word document
            Document document = new Document();

            //load a document
            string fileName = OpenFile();
            //document.LoadFromFile(@"..\..\..\..\..\..\Data\FindAndReplace.doc");
            document.LoadFromFile(@fileName);


            //Find text
            TextSelection[] textSelections = document.FindAllString(this.textBox1.Text.Trim(), true, true);
            if (textSelections == null)
            {
                MessageBox.Show("Not found.");
            }
            else
            //Set hightlight
            {
                foreach (TextSelection selection in textSelections)
                {
                    selection.GetAsOneRange().CharacterFormat.HighlightColor = Color.Yellow;
                }

                //Save doc file.
                document.SaveToFile("Sample.doc",  Spire.Doc.FileFormat.Doc);

                //Launching the MS Word file.
                WordDocViewer("Sample.doc");
            }
        }

        private void Initialize_Grid()
        {
            dgvFinalMerge.RowCount = 1000;
            dgvFinalMerge.ColumnCount = 100;

            dgvFinalMerge.Rows[1].Cells[0].Value = "Item Number";
            dgvFinalMerge.Rows[1].Cells[1].Value = "Related Item No.";
            dgvFinalMerge.Rows[1].Cells[2].Value = "Description";
            dgvFinalMerge.Rows[1].Cells[3].Value = "no.";
            dgvFinalMerge.Rows[1].Cells[4].Value = "Sample description";
            dgvFinalMerge.Rows[1].Cells[5].Value = "Part name";

            dgvFinalMerge.Rows[1].Cells[6].Value = "Materials no.";
            dgvFinalMerge.Rows[1].Cells[7].Value = "Referred from Test report no.";

            dgvFinalMerge.Rows[1].Cells[8].Value = "Issued date";

            dgvFinalMerge.Rows[0].Cells[9].Value = "P4351";
            dgvFinalMerge.Rows[0].Cells[10].Value = "P4351";
            dgvFinalMerge.Rows[0].Cells[11].Value = "P4351";

            dgvFinalMerge.Rows[0].Cells[12].Value = "P4351";
            dgvFinalMerge.Rows[0].Cells[13].Value = "P4351";
            dgvFinalMerge.Rows[0].Cells[14].Value = "P4351";
            dgvFinalMerge.Rows[0].Cells[15].Value = "P4351";
            dgvFinalMerge.Rows[0].Cells[16].Value = "P4351";
            dgvFinalMerge.Rows[0].Cells[17].Value = "P4351";
            dgvFinalMerge.Rows[0].Cells[18].Value = "P4351";
            dgvFinalMerge.Rows[0].Cells[19].Value = "P4351";


            dgvFinalMerge.Rows[1].Cells[9].Value = "MDL";
            dgvFinalMerge.Rows[1].Cells[10].Value = "Lead(Pb)";
            dgvFinalMerge.Rows[1].Cells[11].Value = "Mass of trace amount (mg)";
            dgvFinalMerge.Rows[1].Cells[12].Value = "Pb";
            dgvFinalMerge.Rows[1].Cells[13].Value = "Sb";
            dgvFinalMerge.Rows[1].Cells[14].Value = "As";
            dgvFinalMerge.Rows[1].Cells[15].Value = "Ba";
            dgvFinalMerge.Rows[1].Cells[16].Value = "Cd";
            dgvFinalMerge.Rows[1].Cells[17].Value = "Cr";
            dgvFinalMerge.Rows[1].Cells[18].Value = "Hg";
            dgvFinalMerge.Rows[1].Cells[19].Value = "Se";

            dgvFinalMerge.Rows[0].Cells[20].Value = "P4352";
            dgvFinalMerge.Rows[0].Cells[21].Value = "P4352";
            dgvFinalMerge.Rows[0].Cells[22].Value = "P4352";

            dgvFinalMerge.Rows[0].Cells[23].Value = "P4352";
            dgvFinalMerge.Rows[0].Cells[24].Value = "P4352";
            dgvFinalMerge.Rows[0].Cells[25].Value = "P4352";
            dgvFinalMerge.Rows[0].Cells[26].Value = "P4352";
            dgvFinalMerge.Rows[0].Cells[27].Value = "P4352";
            dgvFinalMerge.Rows[0].Cells[28].Value = "P4352";
            dgvFinalMerge.Rows[0].Cells[29].Value = "P4352";
            dgvFinalMerge.Rows[0].Cells[30].Value = "P4352";


            dgvFinalMerge.Rows[1].Cells[20].Value = "MDL";
            dgvFinalMerge.Rows[1].Cells[21].Value = "Lead(Pb)";
            dgvFinalMerge.Rows[1].Cells[22].Value = "Mass of trace amount (mg)";
            dgvFinalMerge.Rows[1].Cells[23].Value = "Pb";
            dgvFinalMerge.Rows[1].Cells[24].Value = "Sb";
            dgvFinalMerge.Rows[1].Cells[25].Value = "As";
            dgvFinalMerge.Rows[1].Cells[26].Value = "Ba";
            dgvFinalMerge.Rows[1].Cells[27].Value = "Cd";
            dgvFinalMerge.Rows[1].Cells[28].Value = "Cr";
            dgvFinalMerge.Rows[1].Cells[29].Value = "Hg";
            dgvFinalMerge.Rows[1].Cells[30].Value = "Se";

            //dgvFinalMerge.Rows[18].Cells[1].Value = "Soluble Chromium (III) #";
            //dgvFinalMerge.Rows[19].Cells[1].Value = "Soluble Chromium (VI) #";
            //dgvFinalMerge.Rows[20].Cells[1].Value = "Soluble Cobalt";
            //dgvFinalMerge.Rows[21].Cells[1].Value = "Soluble Copper";
            //dgvFinalMerge.Rows[22].Cells[1].Value = "Soluble Lead";
            //dgvFinalMerge.Rows[23].Cells[1].Value = "Soluble Manganese";
            //dgvFinalMerge.Rows[24].Cells[1].Value = "Soluble Mercury";
            //dgvFinalMerge.Rows[25].Cells[1].Value = "Soluble Nickel";
            //dgvFinalMerge.Rows[26].Cells[1].Value = "Soluble Selenium";
            //dgvFinalMerge.Rows[27].Cells[1].Value = "Soluble Strontium";
            //dgvFinalMerge.Rows[28].Cells[1].Value = "Soluble Tin";
            //dgvFinalMerge.Rows[29].Cells[1].Value = "Soluble Organic Tin^";
            //dgvFinalMerge.Rows[30].Cells[1].Value = "Soluble Zinc";

            //dgvFinalMerge.Rows[33].Cells[1].Value = "Methyl tin";
            //dgvFinalMerge.Rows[34].Cells[1].Value = "Di-n-propyl tin";
            //dgvFinalMerge.Rows[35].Cells[1].Value = "Butyl tin";
            //dgvFinalMerge.Rows[36].Cells[1].Value = "Dibutyl tin";
            //dgvFinalMerge.Rows[37].Cells[1].Value = "Tributyl tin";
            //dgvFinalMerge.Rows[38].Cells[1].Value = "n-Octyl tin";
            //dgvFinalMerge.Rows[39].Cells[1].Value = "Tetrabutyl tin";
            //dgvFinalMerge.Rows[40].Cells[1].Value = "Diphenyl tin";
            //dgvFinalMerge.Rows[41].Cells[1].Value = "Di-n-octyl tin";
            //dgvFinalMerge.Rows[42].Cells[1].Value = "Triphenyl tin";

            //dgvFinalMerge.Rows[10].Cells[2].Value = "(mg)";
            //dgvFinalMerge.Rows[11].Cells[2].Value = "(Al)";
            //dgvFinalMerge.Rows[12].Cells[2].Value = "(Sb)";
            //dgvFinalMerge.Rows[13].Cells[2].Value = "(As)";
            //dgvFinalMerge.Rows[14].Cells[2].Value = "(Ba)";
            //dgvFinalMerge.Rows[15].Cells[2].Value = "(B)";
            //dgvFinalMerge.Rows[16].Cells[2].Value = "(Cd)";
            //dgvFinalMerge.Rows[17].Cells[2].Value = "(Cr)";
            //dgvFinalMerge.Rows[18].Cells[2].Value = "(Cr (III))";
            //dgvFinalMerge.Rows[19].Cells[2].Value = "(Cr (VI))";
            //dgvFinalMerge.Rows[20].Cells[2].Value = "(Co)";
            //dgvFinalMerge.Rows[21].Cells[2].Value = "(Cu)";
            //dgvFinalMerge.Rows[22].Cells[2].Value = "(Pb)";
            //dgvFinalMerge.Rows[23].Cells[2].Value = "(Mn)";
            //dgvFinalMerge.Rows[24].Cells[2].Value = "(Hg)";
            //dgvFinalMerge.Rows[25].Cells[2].Value = "(Ni)";
            //dgvFinalMerge.Rows[26].Cells[2].Value = "(Se)";
            //dgvFinalMerge.Rows[27].Cells[2].Value = "(Sr)";
            //dgvFinalMerge.Rows[28].Cells[2].Value = "(Sn)";
            //dgvFinalMerge.Rows[29].Cells[2].Value = "--";
            //dgvFinalMerge.Rows[30].Cells[2].Value = "(Zn)";

            //dgvFinalMerge.Rows[33].Cells[2].Value = "(MeT)";
            //dgvFinalMerge.Rows[34].Cells[2].Value = "(DProT)";
            //dgvFinalMerge.Rows[35].Cells[2].Value = "(BuT)";
            //dgvFinalMerge.Rows[36].Cells[2].Value = "(DBT)";
            //dgvFinalMerge.Rows[37].Cells[2].Value = "(TBT)";
            //dgvFinalMerge.Rows[38].Cells[2].Value = "(MOT)";
            //dgvFinalMerge.Rows[39].Cells[2].Value = "(TeBT)";
            //dgvFinalMerge.Rows[40].Cells[2].Value = "(DPhT)";
            //dgvFinalMerge.Rows[41].Cells[2].Value = "(DOT)";
            //dgvFinalMerge.Rows[42].Cells[2].Value = "(TPhT)";

            dgvFinal.RowCount = 1000;
            dgvFinal.ColumnCount = 100;

            dgvFinal.Rows[1].Cells[0].Value = "Item Number";
            dgvFinal.Rows[1].Cells[1].Value = "Related Item No.";
            dgvFinal.Rows[1].Cells[2].Value = "Description";
            dgvFinal.Rows[1].Cells[3].Value = "no.";
            dgvFinal.Rows[1].Cells[4].Value = "Sample description";
            dgvFinal.Rows[1].Cells[5].Value = "Part name";

            dgvFinal.Rows[1].Cells[6].Value = "Materials no.";
            dgvFinal.Rows[1].Cells[7].Value = "Referred from Test report no.";

            dgvFinal.Rows[1].Cells[8].Value = "Issued date";

            dgvFinal.Rows[0].Cells[9].Value = "P4351";
            dgvFinal.Rows[0].Cells[10].Value = "P4351";
            dgvFinal.Rows[0].Cells[11].Value = "P4351";

            dgvFinal.Rows[0].Cells[12].Value = "P4351";
            dgvFinal.Rows[0].Cells[13].Value = "P4351";
            dgvFinal.Rows[0].Cells[14].Value = "P4351";
            dgvFinal.Rows[0].Cells[15].Value = "P4351";
            dgvFinal.Rows[0].Cells[16].Value = "P4351";
            dgvFinal.Rows[0].Cells[17].Value = "P4351";
            dgvFinal.Rows[0].Cells[18].Value = "P4351";
            dgvFinal.Rows[0].Cells[19].Value = "P4351";


            dgvFinal.Rows[1].Cells[9].Value = "MDL";
            dgvFinal.Rows[1].Cells[10].Value = "Lead(Pb)";
            dgvFinal.Rows[1].Cells[11].Value = "Mass of trace amount (mg)";
            dgvFinal.Rows[1].Cells[12].Value = "Pb";
            dgvFinal.Rows[1].Cells[13].Value = "Sb";
            dgvFinal.Rows[1].Cells[14].Value = "As";
            dgvFinal.Rows[1].Cells[15].Value = "Ba";
            dgvFinal.Rows[1].Cells[16].Value = "Cd";
            dgvFinal.Rows[1].Cells[17].Value = "Cr";
            dgvFinal.Rows[1].Cells[18].Value = "Hg";
            dgvFinal.Rows[1].Cells[19].Value = "Se";

            dgvFinal.Rows[0].Cells[20].Value = "P4352";
            dgvFinal.Rows[0].Cells[21].Value = "P4352";
            dgvFinal.Rows[0].Cells[22].Value = "P4352";

            dgvFinal.Rows[0].Cells[23].Value = "P4352";
            dgvFinal.Rows[0].Cells[24].Value = "P4352";
            dgvFinal.Rows[0].Cells[25].Value = "P4352";
            dgvFinal.Rows[0].Cells[26].Value = "P4352";
            dgvFinal.Rows[0].Cells[27].Value = "P4352";
            dgvFinal.Rows[0].Cells[28].Value = "P4352";
            dgvFinal.Rows[0].Cells[29].Value = "P4352";
            dgvFinal.Rows[0].Cells[30].Value = "P4352";


            dgvFinal.Rows[1].Cells[20].Value = "MDL";
            dgvFinal.Rows[1].Cells[21].Value = "Lead(Pb)";
            dgvFinal.Rows[1].Cells[22].Value = "Mass of trace amount (mg)";
            dgvFinal.Rows[1].Cells[23].Value = "Pb";
            dgvFinal.Rows[1].Cells[24].Value = "Sb";
            dgvFinal.Rows[1].Cells[25].Value = "As";
            dgvFinal.Rows[1].Cells[26].Value = "Ba";
            dgvFinal.Rows[1].Cells[27].Value = "Cd";
            dgvFinal.Rows[1].Cells[28].Value = "Cr";
            dgvFinal.Rows[1].Cells[29].Value = "Hg";
            dgvFinal.Rows[1].Cells[30].Value = "Se";
            //dgvFinal.Rows[0].Cells[8].Value = "MDL";
            //dgvFinal.Rows[0].Cells[9].Value = "Lead(Pb)";
            //dgvFinal.Rows[0].Cells[10].Value = "Mass of trace amount (mg)";

            //dgvFinal.Rows[0].Cells[11].Value = "Pb";
            //dgvFinal.Rows[0].Cells[12].Value = "Sb";
            //dgvFinal.Rows[0].Cells[13].Value = "As";
            //dgvFinal.Rows[0].Cells[14].Value = "Ba";
            //dgvFinal.Rows[0].Cells[15].Value = "Cd";
            //dgvFinal.Rows[0].Cells[16].Value = "Cr";
            //dgvFinal.Rows[0].Cells[17].Value = "Hg";
            //dgvFinal.Rows[0].Cells[18].Value = "Se";

            //dgvFinal.RowCount = 500;
            //dgvFinal.ColumnCount = 100;

            //dgvFinal.Rows[1].Cells[0].Value = "Job No";
            //dgvFinal.Rows[2].Cells[0].Value = "Item No";
            //dgvFinal.Rows[3].Cells[0].Value = "Issue Date";
            //dgvFinal.Rows[4].Cells[0].Value = "Aurora Description";
            //dgvFinal.Rows[5].Cells[0].Value = "Screen Or Not";

            //dgvFinal.Rows[6].Cells[0].Value = "PartName";
            //dgvFinal.Rows[7].Cells[0].Value = "Materials";

            //dgvFinal.Rows[8].Cells[0].Value = "Sample No";
            //dgvFinal.Rows[9].Cells[0].Value = "Report Sample Description";
            //dgvFinal.Rows[10].Cells[0].Value = "Part3 Results";
            //dgvFinal.Rows[33].Cells[0].Value = "Organotin Results";

            //dgvFinal.Rows[10].Cells[1].Value = "Mass of trace amount";
            //dgvFinal.Rows[11].Cells[1].Value = "Soluble Aluminium";
            //dgvFinal.Rows[12].Cells[1].Value = "Soluble Antimony";
            //dgvFinal.Rows[13].Cells[1].Value = "Soluble Arsenic";
            //dgvFinal.Rows[14].Cells[1].Value = "Soluble Barium";
            //dgvFinal.Rows[15].Cells[1].Value = "Soluble Boron";
            //dgvFinal.Rows[16].Cells[1].Value = "Soluble Cadmium";
            //dgvFinal.Rows[17].Cells[1].Value = "Soluble Chromium";
            //dgvFinal.Rows[18].Cells[1].Value = "Soluble Chromium (III) #";
            //dgvFinal.Rows[19].Cells[1].Value = "Soluble Chromium (VI) #";
            //dgvFinal.Rows[20].Cells[1].Value = "Soluble Cobalt";
            //dgvFinal.Rows[21].Cells[1].Value = "Soluble Copper";
            //dgvFinal.Rows[22].Cells[1].Value = "Soluble Lead";
            //dgvFinal.Rows[23].Cells[1].Value = "Soluble Manganese";
            //dgvFinal.Rows[24].Cells[1].Value = "Soluble Mercury";
            //dgvFinal.Rows[25].Cells[1].Value = "Soluble Nickel";
            //dgvFinal.Rows[26].Cells[1].Value = "Soluble Selenium";
            //dgvFinal.Rows[27].Cells[1].Value = "Soluble Strontium";
            //dgvFinal.Rows[28].Cells[1].Value = "Soluble Tin";
            //dgvFinal.Rows[29].Cells[1].Value = "Soluble Organic Tin^";
            //dgvFinal.Rows[30].Cells[1].Value = "Soluble Zinc";

            //dgvFinal.Rows[33].Cells[1].Value = "Methyl tin";
            //dgvFinal.Rows[34].Cells[1].Value = "Di-n-propyl tin";
            //dgvFinal.Rows[35].Cells[1].Value = "Butyl tin";
            //dgvFinal.Rows[36].Cells[1].Value = "Dibutyl tin";
            //dgvFinal.Rows[37].Cells[1].Value = "Tributyl tin";
            //dgvFinal.Rows[38].Cells[1].Value = "n-Octyl tin";
            //dgvFinal.Rows[39].Cells[1].Value = "Tetrabutyl tin";
            //dgvFinal.Rows[40].Cells[1].Value = "Diphenyl tin";
            //dgvFinal.Rows[41].Cells[1].Value = "Di-n-octyl tin";
            //dgvFinal.Rows[42].Cells[1].Value = "Triphenyl tin";

            //dgvFinal.Rows[10].Cells[2].Value = "(mg)";
            //dgvFinal.Rows[11].Cells[2].Value = "(Al)";
            //dgvFinal.Rows[12].Cells[2].Value = "(Sb)";
            //dgvFinal.Rows[13].Cells[2].Value = "(As)";
            //dgvFinal.Rows[14].Cells[2].Value = "(Ba)";
            //dgvFinal.Rows[15].Cells[2].Value = "(B)";
            //dgvFinal.Rows[16].Cells[2].Value = "(Cd)";
            //dgvFinal.Rows[17].Cells[2].Value = "(Cr)";
            //dgvFinal.Rows[18].Cells[2].Value = "(Cr (III))";
            //dgvFinal.Rows[19].Cells[2].Value = "(Cr (VI))";
            //dgvFinal.Rows[20].Cells[2].Value = "(Co)";
            //dgvFinal.Rows[21].Cells[2].Value = "(Cu)";
            //dgvFinal.Rows[22].Cells[2].Value = "(Pb)";
            //dgvFinal.Rows[23].Cells[2].Value = "(Mn)";
            //dgvFinal.Rows[24].Cells[2].Value = "(Hg)";
            //dgvFinal.Rows[25].Cells[2].Value = "(Ni)";
            //dgvFinal.Rows[26].Cells[2].Value = "(Se)";
            //dgvFinal.Rows[27].Cells[2].Value = "(Sr)";
            //dgvFinal.Rows[28].Cells[2].Value = "(Sn)";
            //dgvFinal.Rows[29].Cells[2].Value = "--";
            //dgvFinal.Rows[30].Cells[2].Value = "(Zn)";

            //dgvFinal.Rows[33].Cells[2].Value = "(MeT)";
            //dgvFinal.Rows[34].Cells[2].Value = "(DProT)";
            //dgvFinal.Rows[35].Cells[2].Value = "(BuT)";
            //dgvFinal.Rows[36].Cells[2].Value = "(DBT)";
            //dgvFinal.Rows[37].Cells[2].Value = "(TBT)";
            //dgvFinal.Rows[38].Cells[2].Value = "(MOT)";
            //dgvFinal.Rows[39].Cells[2].Value = "(TeBT)";
            //dgvFinal.Rows[40].Cells[2].Value = "(DPhT)";
            //dgvFinal.Rows[41].Cells[2].Value = "(DOT)";
            //dgvFinal.Rows[42].Cells[2].Value = "(TPhT)";

            ////dgvMergeFiles.Rows[5].Cells[4].Value = "Report No";
            ////dgvMergeFiles.Rows[5].Cells[5].Value = "Issued Date";
            ////dgvMergeFiles.Rows[5].Cells[6].Value = "Report Sample Description";
        }

        private void dgvFinal_initialization()
        {
            dgvFinalMerge.RowCount = 43;
            dgvFinalMerge.ColumnCount = 100;

            dgvFinalMerge.Rows[0].Cells[0].Value = " ";
            dgvFinalMerge.Rows[1].Cells[0].Value = "Job No";
            dgvFinalMerge.Rows[2].Cells[0].Value = "Item No";
            dgvFinalMerge.Rows[3].Cells[0].Value = "Issue Date";
            dgvFinalMerge.Rows[4].Cells[0].Value = "Aurora Description";
            dgvFinalMerge.Rows[5].Cells[0].Value = "Screen Or Not";

            dgvFinalMerge.Rows[6].Cells[0].Value = "PartName";
            dgvFinalMerge.Rows[7].Cells[0].Value = "Materials";

            dgvFinalMerge.Rows[8].Cells[0].Value = "Sample No";
            dgvFinalMerge.Rows[9].Cells[0].Value = "Report Sample Description";
            dgvFinalMerge.Rows[10].Cells[0].Value = "Part3 Results";
            dgvFinalMerge.Rows[33].Cells[0].Value = "Organotin Results";

            dgvFinalMerge.Rows[10].Cells[1].Value = "Mass of trace amount";
            dgvFinalMerge.Rows[11].Cells[1].Value = "Soluble Aluminium";
            dgvFinalMerge.Rows[12].Cells[1].Value = "Soluble Antimony";
            dgvFinalMerge.Rows[13].Cells[1].Value = "Soluble Arsenic";
            dgvFinalMerge.Rows[14].Cells[1].Value = "Soluble Barium";
            dgvFinalMerge.Rows[15].Cells[1].Value = "Soluble Boron";
            dgvFinalMerge.Rows[16].Cells[1].Value = "Soluble Cadmium";
            dgvFinalMerge.Rows[17].Cells[1].Value = "Soluble Chromium";
            dgvFinalMerge.Rows[18].Cells[1].Value = "Soluble Chromium (III) #";
            dgvFinalMerge.Rows[19].Cells[1].Value = "Soluble Chromium (VI) #";
            dgvFinalMerge.Rows[20].Cells[1].Value = "Soluble Cobalt";
            dgvFinalMerge.Rows[21].Cells[1].Value = "Soluble Copper";
            dgvFinalMerge.Rows[22].Cells[1].Value = "Soluble Lead";
            dgvFinalMerge.Rows[23].Cells[1].Value = "Soluble Manganese";
            dgvFinalMerge.Rows[24].Cells[1].Value = "Soluble Mercury";
            dgvFinalMerge.Rows[25].Cells[1].Value = "Soluble Nickel";
            dgvFinalMerge.Rows[26].Cells[1].Value = "Soluble Selenium";
            dgvFinalMerge.Rows[27].Cells[1].Value = "Soluble Strontium";
            dgvFinalMerge.Rows[28].Cells[1].Value = "Soluble Tin";
            dgvFinalMerge.Rows[29].Cells[1].Value = "Soluble Organic Tin^";
            dgvFinalMerge.Rows[30].Cells[1].Value = "Soluble Zinc";

            dgvFinalMerge.Rows[33].Cells[1].Value = "Methyl tin";
            dgvFinalMerge.Rows[34].Cells[1].Value = "Di-n-propyl tin";
            dgvFinalMerge.Rows[35].Cells[1].Value = "Butyl tin";
            dgvFinalMerge.Rows[36].Cells[1].Value = "Dibutyl tin";
            dgvFinalMerge.Rows[37].Cells[1].Value = "Tributyl tin";
            dgvFinalMerge.Rows[38].Cells[1].Value = "n-Octyl tin";
            dgvFinalMerge.Rows[39].Cells[1].Value = "Tetrabutyl tin";
            dgvFinalMerge.Rows[40].Cells[1].Value = "Diphenyl tin";
            dgvFinalMerge.Rows[41].Cells[1].Value = "Di-n-octyl tin";
            dgvFinalMerge.Rows[42].Cells[1].Value = "Triphenyl tin";

            dgvFinalMerge.Rows[10].Cells[2].Value = "(mg)";
            dgvFinalMerge.Rows[11].Cells[2].Value = "(Al)";
            dgvFinalMerge.Rows[12].Cells[2].Value = "(Sb)";
            dgvFinalMerge.Rows[13].Cells[2].Value = "(As)";
            dgvFinalMerge.Rows[14].Cells[2].Value = "(Ba)";
            dgvFinalMerge.Rows[15].Cells[2].Value = "(B)";
            dgvFinalMerge.Rows[16].Cells[2].Value = "(Cd)";
            dgvFinalMerge.Rows[17].Cells[2].Value = "(Cr)";
            dgvFinalMerge.Rows[18].Cells[2].Value = "(Cr (III))";
            dgvFinalMerge.Rows[19].Cells[2].Value = "(Cr (VI))";
            dgvFinalMerge.Rows[20].Cells[2].Value = "(Co)";
            dgvFinalMerge.Rows[21].Cells[2].Value = "(Cu)";
            dgvFinalMerge.Rows[22].Cells[2].Value = "(Pb)";
            dgvFinalMerge.Rows[23].Cells[2].Value = "(Mn)";
            dgvFinalMerge.Rows[24].Cells[2].Value = "(Hg)";
            dgvFinalMerge.Rows[25].Cells[2].Value = "(Ni)";
            dgvFinalMerge.Rows[26].Cells[2].Value = "(Se)";
            dgvFinalMerge.Rows[27].Cells[2].Value = "(Sr)";
            dgvFinalMerge.Rows[28].Cells[2].Value = "(Sn)";
            dgvFinalMerge.Rows[29].Cells[2].Value = "--";
            dgvFinalMerge.Rows[30].Cells[2].Value = "(Zn)";

            dgvFinalMerge.Rows[33].Cells[2].Value = "(MeT)";
            dgvFinalMerge.Rows[34].Cells[2].Value = "(DProT)";
            dgvFinalMerge.Rows[35].Cells[2].Value = "(BuT)";
            dgvFinalMerge.Rows[36].Cells[2].Value = "(DBT)";
            dgvFinalMerge.Rows[37].Cells[2].Value = "(TBT)";
            dgvFinalMerge.Rows[38].Cells[2].Value = "(MOT)";
            dgvFinalMerge.Rows[39].Cells[2].Value = "(TeBT)";
            dgvFinalMerge.Rows[40].Cells[2].Value = "(DPhT)";
            dgvFinalMerge.Rows[41].Cells[2].Value = "(DOT)";
            dgvFinalMerge.Rows[42].Cells[2].Value = "(TPhT)";
        }

        private void button3_Click(object sender, EventArgs e)
        {

            dgvFinalMerge.RowCount = 43;
            dgvFinalMerge.ColumnCount = 100;

            dgvFinalMerge.Rows[0].Cells[0].Value = " ";
            dgvFinalMerge.Rows[1].Cells[0].Value = "Job No";
            dgvFinalMerge.Rows[2].Cells[0].Value = "Item No";
            dgvFinalMerge.Rows[3].Cells[0].Value = "Issue Date";
            dgvFinalMerge.Rows[4].Cells[0].Value = "Aurora Description";
            dgvFinalMerge.Rows[5].Cells[0].Value = "Screen Or Not";

            dgvFinalMerge.Rows[7].Cells[0].Value = "Sample No";
            dgvFinalMerge.Rows[8].Cells[0].Value = "Report Sample Description";
            dgvFinalMerge.Rows[9].Cells[0].Value = "Part3 Results";
            dgvFinalMerge.Rows[32].Cells[0].Value = "Organotin Results";

            dgvFinalMerge.Rows[9].Cells[1].Value = "Mass of trace amount";
            dgvFinalMerge.Rows[10].Cells[1].Value = "Soluble Aluminium";
            dgvFinalMerge.Rows[11].Cells[1].Value = "Soluble Antimony";
            dgvFinalMerge.Rows[12].Cells[1].Value = "Soluble Arsenic";
            dgvFinalMerge.Rows[13].Cells[1].Value = "Soluble Barium";
            dgvFinalMerge.Rows[14].Cells[1].Value = "Soluble Boron";
            dgvFinalMerge.Rows[15].Cells[1].Value = "Soluble Cadmium";
            dgvFinalMerge.Rows[16].Cells[1].Value = "Soluble Chromium";
            dgvFinalMerge.Rows[17].Cells[1].Value = "Soluble Chromium (III) #";
            dgvFinalMerge.Rows[18].Cells[1].Value = "Soluble Chromium (VI) #";
            dgvFinalMerge.Rows[19].Cells[1].Value = "Soluble Cobalt";
            dgvFinalMerge.Rows[20].Cells[1].Value = "Soluble Copper";
            dgvFinalMerge.Rows[21].Cells[1].Value = "Soluble Lead";
            dgvFinalMerge.Rows[22].Cells[1].Value = "Soluble Manganese";
            dgvFinalMerge.Rows[23].Cells[1].Value = "Soluble Mercury";
            dgvFinalMerge.Rows[24].Cells[1].Value = "Soluble Nickel";
            dgvFinalMerge.Rows[25].Cells[1].Value = "Soluble Selenium";
            dgvFinalMerge.Rows[26].Cells[1].Value = "Soluble Strontium";
            dgvFinalMerge.Rows[27].Cells[1].Value = "Soluble Tin";
            dgvFinalMerge.Rows[28].Cells[1].Value = "Soluble Organic Tin^";
            dgvFinalMerge.Rows[29].Cells[1].Value = "Soluble Zinc";

            dgvFinalMerge.Rows[32].Cells[1].Value = "Methyl tin";
            dgvFinalMerge.Rows[33].Cells[1].Value = "Di-n-propyl tin";
            dgvFinalMerge.Rows[34].Cells[1].Value = "Butyl tin";
            dgvFinalMerge.Rows[35].Cells[1].Value = "Dibutyl tin";
            dgvFinalMerge.Rows[36].Cells[1].Value = "Tributyl tin";
            dgvFinalMerge.Rows[37].Cells[1].Value = "n-Octyl tin";
            dgvFinalMerge.Rows[38].Cells[1].Value = "Tetrabutyl tin";
            dgvFinalMerge.Rows[39].Cells[1].Value = "Diphenyl tin";
            dgvFinalMerge.Rows[40].Cells[1].Value = "Di-n-octyl tin";
            dgvFinalMerge.Rows[41].Cells[1].Value = "Triphenyl tin";

            dgvFinalMerge.Rows[9].Cells[2].Value = "(mg)";
            dgvFinalMerge.Rows[10].Cells[2].Value = "(Al)";
            dgvFinalMerge.Rows[11].Cells[2].Value = "(Sb)";
            dgvFinalMerge.Rows[12].Cells[2].Value = "(As)";
            dgvFinalMerge.Rows[13].Cells[2].Value = "(Ba)";
            dgvFinalMerge.Rows[14].Cells[2].Value = "(B)";
            dgvFinalMerge.Rows[15].Cells[2].Value = "(Cd)";
            dgvFinalMerge.Rows[16].Cells[2].Value = "(Cr)";
            dgvFinalMerge.Rows[17].Cells[2].Value = "(Cr (III))";
            dgvFinalMerge.Rows[18].Cells[2].Value = "(Cr (VI))";
            dgvFinalMerge.Rows[19].Cells[2].Value = "(Co)";
            dgvFinalMerge.Rows[20].Cells[2].Value = "(Cu)";
            dgvFinalMerge.Rows[21].Cells[2].Value = "(Pb)";
            dgvFinalMerge.Rows[22].Cells[2].Value = "(Mn)";
            dgvFinalMerge.Rows[23].Cells[2].Value = "(Hg)";
            dgvFinalMerge.Rows[24].Cells[2].Value = "(Ni)";
            dgvFinalMerge.Rows[25].Cells[2].Value = "(Se)";
            dgvFinalMerge.Rows[26].Cells[2].Value = "(Sr)";
            dgvFinalMerge.Rows[27].Cells[2].Value = "(Sn)";
            dgvFinalMerge.Rows[28].Cells[2].Value = "--";
            dgvFinalMerge.Rows[29].Cells[2].Value = "(Zn)";

            dgvFinalMerge.Rows[32].Cells[2].Value = "(MeT)";
            dgvFinalMerge.Rows[33].Cells[2].Value = "(DProT)";
            dgvFinalMerge.Rows[34].Cells[2].Value = "(BuT)";
            dgvFinalMerge.Rows[35].Cells[2].Value = "(DBT)";
            dgvFinalMerge.Rows[36].Cells[2].Value = "(TBT)";
            dgvFinalMerge.Rows[37].Cells[2].Value = "(MOT)";
            dgvFinalMerge.Rows[38].Cells[2].Value = "(TeBT)";
            dgvFinalMerge.Rows[39].Cells[2].Value = "(DPhT)";
            dgvFinalMerge.Rows[40].Cells[2].Value = "(DOT)";
            dgvFinalMerge.Rows[41].Cells[2].Value = "(TPhT)";

            dgvFinal.RowCount = 43;
            dgvFinal.ColumnCount = 100;

            dgvFinal.Rows[0].Cells[0].Value = " ";
            dgvFinal.Rows[1].Cells[0].Value = "Job No";
            dgvFinal.Rows[2].Cells[0].Value = "Item No";
            dgvFinal.Rows[3].Cells[0].Value = "Issue Date";
            dgvFinal.Rows[4].Cells[0].Value = "Aurora Description";
            dgvFinal.Rows[5].Cells[0].Value = "Screen Or Not";

            dgvFinal.Rows[7].Cells[0].Value = "Sample No";
            dgvFinal.Rows[8].Cells[0].Value = "Report Sample Description";
            dgvFinal.Rows[9].Cells[0].Value = "Part3 Results";
            dgvFinal.Rows[32].Cells[0].Value = "Organotin Results";

            dgvFinal.Rows[9].Cells[1].Value = "Mass of trace amount";
            dgvFinal.Rows[10].Cells[1].Value = "Soluble Aluminium";
            dgvFinal.Rows[11].Cells[1].Value = "Soluble Antimony";
            dgvFinal.Rows[12].Cells[1].Value = "Soluble Arsenic";
            dgvFinal.Rows[13].Cells[1].Value = "Soluble Barium";
            dgvFinal.Rows[14].Cells[1].Value = "Soluble Boron";
            dgvFinal.Rows[15].Cells[1].Value = "Soluble Cadmium";
            dgvFinal.Rows[16].Cells[1].Value = "Soluble Chromium";
            dgvFinal.Rows[17].Cells[1].Value = "Soluble Chromium (III) #";
            dgvFinal.Rows[18].Cells[1].Value = "Soluble Chromium (VI) #";
            dgvFinal.Rows[19].Cells[1].Value = "Soluble Cobalt";
            dgvFinal.Rows[20].Cells[1].Value = "Soluble Copper";
            dgvFinal.Rows[21].Cells[1].Value = "Soluble Lead";
            dgvFinal.Rows[22].Cells[1].Value = "Soluble Manganese";
            dgvFinal.Rows[23].Cells[1].Value = "Soluble Mercury";
            dgvFinal.Rows[24].Cells[1].Value = "Soluble Nickel";
            dgvFinal.Rows[25].Cells[1].Value = "Soluble Selenium";
            dgvFinal.Rows[26].Cells[1].Value = "Soluble Strontium";
            dgvFinal.Rows[27].Cells[1].Value = "Soluble Tin";
            dgvFinal.Rows[28].Cells[1].Value = "Soluble Organic Tin^";
            dgvFinal.Rows[29].Cells[1].Value = "Soluble Zinc";

            dgvFinal.Rows[32].Cells[1].Value = "Methyl tin";
            dgvFinal.Rows[33].Cells[1].Value = "Di-n-propyl tin";
            dgvFinal.Rows[34].Cells[1].Value = "Butyl tin";
            dgvFinal.Rows[35].Cells[1].Value = "Dibutyl tin";
            dgvFinal.Rows[36].Cells[1].Value = "Tributyl tin";
            dgvFinal.Rows[37].Cells[1].Value = "n-Octyl tin";
            dgvFinal.Rows[38].Cells[1].Value = "Tetrabutyl tin";
            dgvFinal.Rows[39].Cells[1].Value = "Diphenyl tin";
            dgvFinal.Rows[40].Cells[1].Value = "Di-n-octyl tin";
            dgvFinal.Rows[41].Cells[1].Value = "Triphenyl tin";

            dgvFinal.Rows[9].Cells[2].Value = "(mg)";
            dgvFinal.Rows[10].Cells[2].Value = "(Al)";
            dgvFinal.Rows[11].Cells[2].Value = "(Sb)";
            dgvFinal.Rows[12].Cells[2].Value = "(As)";
            dgvFinal.Rows[13].Cells[2].Value = "(Ba)";
            dgvFinal.Rows[14].Cells[2].Value = "(B)";
            dgvFinal.Rows[15].Cells[2].Value = "(Cd)";
            dgvFinal.Rows[16].Cells[2].Value = "(Cr)";
            dgvFinal.Rows[17].Cells[2].Value = "(Cr (III))";
            dgvFinal.Rows[18].Cells[2].Value = "(Cr (VI))";
            dgvFinal.Rows[19].Cells[2].Value = "(Co)";
            dgvFinal.Rows[20].Cells[2].Value = "(Cu)";
            dgvFinal.Rows[21].Cells[2].Value = "(Pb)";
            dgvFinal.Rows[22].Cells[2].Value = "(Mn)";
            dgvFinal.Rows[23].Cells[2].Value = "(Hg)";
            dgvFinal.Rows[24].Cells[2].Value = "(Ni)";
            dgvFinal.Rows[25].Cells[2].Value = "(Se)";
            dgvFinal.Rows[26].Cells[2].Value = "(Sr)";
            dgvFinal.Rows[27].Cells[2].Value = "(Sn)";
            dgvFinal.Rows[28].Cells[2].Value = "--";
            dgvFinal.Rows[29].Cells[2].Value = "(Zn)";

            dgvFinal.Rows[32].Cells[2].Value = "(MeT)";
            dgvFinal.Rows[33].Cells[2].Value = "(DProT)";
            dgvFinal.Rows[34].Cells[2].Value = "(BuT)";
            dgvFinal.Rows[35].Cells[2].Value = "(DBT)";
            dgvFinal.Rows[36].Cells[2].Value = "(TBT)";
            dgvFinal.Rows[37].Cells[2].Value = "(MOT)";
            dgvFinal.Rows[38].Cells[2].Value = "(TeBT)";
            dgvFinal.Rows[39].Cells[2].Value = "(DPhT)";
            dgvFinal.Rows[40].Cells[2].Value = "(DOT)";
            dgvFinal.Rows[41].Cells[2].Value = "(TPhT)";

            dgvMergeFiles.RowCount = 50;
            foreach (DataGridViewRow row1 in dgvValues.Rows)
            {
                if (row1.Index >= 0 & row1.Index <= 5)
                {
                    dgvMergeFiles.Rows.Add(1);
                    foreach (DataGridViewColumn col1 in dgvValues.Columns)
                    {
                        if (col1.Index >= 0 & col1.Index <= 3)
                        {
                            dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].Value = dgvValues.Rows[row1.Index].Cells[col1.Index].Value;
                            //if (dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].ToString().Contains("Part") & row1.Index == 5)
                            //{
                            //    dgvMergeFiles.Rows[row1.Index].Cells[4].Value = "Report No";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[5].Value = "Issued Date";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[6].Value = "Report Sample Description";
                            //}
                        }
                    }
                }
            }

            dgvMergeFiles.Rows[5].Cells[4].Value = "Report No";
            dgvMergeFiles.Rows[5].Cells[5].Value = "Issued Date";
            dgvMergeFiles.Rows[5].Cells[6].Value = "Report Sample Description";

            string fileName = OpenFile();
            //string fileName = "E:\\T42\\WORK\\request\\오로라\\테스트\\복사본 KI2015019.xls";
            string plainText = string.Empty;
            string ext = System.IO.Path.GetExtension(fileName).ToUpper();
            string directoryPath = string.Empty;




            if (ext.Equals(".DOCX") || ext.Equals(".DOC"))
            {

                //Create word document

                Document document = new Document();




                //load a document

                document.LoadFromFile(@fileName);




                plainText = document.GetText();

                this.textBox3.Text = plainText;

            }

            else if (ext.Equals(".XLSX") || ext.Equals(".XLS"))
            {

                //Create Excel workbook

                Workbook workbook = new Workbook();




                //load a workbook

                workbook.LoadFromFile(@fileName);

                directoryPath = "c:\\temp\\";


                for (int i = 0; i < workbook.Worksheets.Count; i++)
                {

                    string tmpfilename = "tempSheet" + i.ToString() + ".txt";

                    Worksheet sheet = workbook.Worksheets[i];




                    if (!sheet.IsEmpty)
                    {

                        if (System.IO.File.Exists(directoryPath + tmpfilename))
                        {

                            System.IO.File.Delete(directoryPath + tmpfilename);

                        }


                        sheet.SaveToFile(directoryPath + tmpfilename, ", ", Encoding.UTF8);

                        plainText += "--[" + sheet.Name + "]--\r\n";

                        plainText += System.IO.File.ReadAllText(directoryPath + tmpfilename);

                        plainText += "\r\n";

                    }

                }
                this.textBox2.Text = plainText;
                Get_Data_From_Excel();
                Get_File_List();
                Find_Material();
            }

        }

        private void Get_Data_From_Excel()
        {
            // Get the file's text.
            string whole_file = textBox2.Text;

            // Split into lines.
            //\r\n
            whole_file = whole_file.Replace('\n', '\r');
            whole_file = whole_file.Replace('"', ' ');
            string[] lines = whole_file.Split(new char[] { '\r' },
                StringSplitOptions.RemoveEmptyEntries);

            // See how many rows and columns there are.
            int num_rows = lines.Length;
            int num_cols = lines[num_rows - 1].Split(',').Length;

            // Allocate the data array.
            string[,] values = new string[num_rows, num_cols];

            // Load the array.
            for (int r = 1; r < num_rows; r++)
            {
                string[] line_r = lines[r].Split(',');
                for (int c = 0; c < num_cols; c++)
                {
                    if (line_r[c] == null)
                        values[r, c] = "";
                    else
                        values[r, c] = line_r[c];
                }
            }

            int num1_rows = values.GetUpperBound(0) + 1;
            int num1_cols = values.GetUpperBound(1) + 1;

            // Display the data to show we have it.

            // Make column headers.
            // For this example, we assume the first row
            // contains the column names.

            // Clear previous results.
            dgvValues.Rows.Clear();

            for (int c = 0; c < num1_cols; c++)
                dgvValues.Columns.Add(values[0, c], values[0, c]);

            // Add the data.
            for (int r = 1; r < num1_rows; r++)
            {
                dgvValues.Rows.Add();
                for (int c = 0; c < num1_cols; c++)
                {
                    dgvValues.Rows[r - 1].Cells[c].Value = values[r, c];
                }
            }

            //// Make the columns autosize.
            //foreach (DataGridViewColumn col in dgvValues.Columns)
            //    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Get the file's text.
            string whole_file = textBox2.Text;

            // Split into lines.
            //\r\n
            whole_file = whole_file.Replace('\n', '\r');
            whole_file = whole_file.Replace('"', ' ');
            string[] lines = whole_file.Split(new char[] { '\r' },
                StringSplitOptions.RemoveEmptyEntries);

            // See how many rows and columns there are.
            int num_rows = lines.Length;
            MessageBox.Show(lines[num_rows - 1]);
            int num_cols = lines[num_rows - 1].Split(',').Length;

            // Allocate the data array.
            string[,] values = new string[num_rows, num_cols];

            // Load the array.
            for (int r = 1; r < num_rows; r++)
            {
                string[] line_r = lines[r].Split(',');
                for (int c = 0; c < num_cols; c++)
                {
                    if (line_r[c] == null)
                        values[r, c] = "";
                    else
                        values[r, c] = line_r[c];
                }
            }

            int num1_rows = values.GetUpperBound(0) + 1;
            int num1_cols = values.GetUpperBound(1) + 1;

            // Display the data to show we have it.

            // Make column headers.
            // For this example, we assume the first row
            // contains the column names.

            // Clear previous results.
            dgvValues.Rows.Clear();

            for (int c = 0; c < num1_cols; c++)
                dgvValues.Columns.Add(values[0, c], values[0, c]);

            // Add the data.
            for (int r = 1; r < num1_rows; r++)
            {
                dgvValues.Rows.Add();
                for (int c = 0; c < num1_cols; c++)
                {
                    dgvValues.Rows[r - 1].Cells[c].Value = values[r, c];
                }
            }

            //// Make the columns autosize.
            //foreach (DataGridViewColumn col in dgvValues.Columns)
            //    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string fileName = OpenFile();
            string plainText = string.Empty;
            string ext = System.IO.Path.GetExtension(fileName).ToUpper();
            string directoryPath = string.Empty;




            if (ext.Equals(".DOCX") || ext.Equals(".DOC"))
            {

                //Create word document

                Document document = new Document();




                //load a document

                document.LoadFromFile(@fileName);




                plainText = document.GetText();

                this.textBox3.Text = plainText;

            }

            else if (ext.Equals(".XLSX") || ext.Equals(".XLS"))
            {

                //Create Excel workbook

                Workbook workbook = new Workbook();




                //load a workbook

                workbook.LoadFromFile(@fileName);

                directoryPath = "c:\\temp\\";


                for (int i = 0; i < workbook.Worksheets.Count; i++)
                {

                    string tmpfilename = "tempSheet" + i.ToString() + ".txt";

                    Worksheet sheet = workbook.Worksheets[i];




                    if (!sheet.IsEmpty)
                    {

                        if (System.IO.File.Exists(directoryPath + tmpfilename))
                        {

                            System.IO.File.Delete(directoryPath + tmpfilename);

                        }


                        sheet.SaveToFile(directoryPath + tmpfilename, ", ", Encoding.UTF8);

                        plainText += "--[" + sheet.Name + "]--\r\n";

                        plainText += System.IO.File.ReadAllText(directoryPath + tmpfilename);

                        plainText += "\r\n";

                    }

                }
                this.textBox2.Text = plainText;
            }

        }

        private void Get_File_List()
        {
            int k;

            string fileName = OpenFile();
            string[] wdir = fileName.Split('\\');

            int j = fileName.Split('\\').Length - 2;

            txtDir1.Text = "";
            for (k = 0; k < fileName.Split('\\').Length - 1; k++)
            {
                txtDir1.Text = txtDir1.Text + wdir[k] + "\\";
            }

            //return;
            // Clear previous results.
            dgvFiles.Rows.Clear();

            //string fileName = OpenFile();
            //txtDir1.Text = fileName;

            // Get sorted lists of files in the directories.
            string dir1 = txtDir1.Text;
            if (!dir1.EndsWith("\\")) dir1 += "\\";
            string[] file_names1 = Directory.GetFiles(dir1);
            for (int i = 0; i < file_names1.Length; i++)
            {
                file_names1[i] = file_names1[i].Replace(dir1, "");
            }
            Array.Sort(file_names1);

            int i1 = 0;
            while (i1 < file_names1.Length)
            {
                dgvFiles.Rows.Add(new Object[] { file_names1[i1], null });
                i1++;
            }

        }


        private void button6_Click(object sender, EventArgs e)
        {
            // Clear previous results.
            dgvFiles.Rows.Clear();

            // Get sorted lists of files in the directories.
            string dir1 = txtDir1.Text;
            if (!dir1.EndsWith("\\")) dir1 += "\\";
            string[] file_names1 = Directory.GetFiles(dir1);
            for (int i = 0; i < file_names1.Length; i++)
            {
                file_names1[i] = file_names1[i].Replace(dir1, "");
            }
            Array.Sort(file_names1);

            int i1 = 0;
            while (i1 < file_names1.Length)
            {
                dgvFiles.Rows.Add(new Object[] { file_names1[i1], null });
                i1++;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            lstResults.Items.Clear();
            DirectoryInfo dir_info = new DirectoryInfo(txtDirectory.Text);

            ListFiles(lstResults, cboPattern2.Text, dir_info, txtTarget.Text);

            System.Media.SystemSounds.Beep.Play();
        }

        private void btnFindMaterial_Click(object sender, EventArgs e)
        {
            string plainText = string.Empty;

            //load a document
            //string fileName = OpenFile();
            //document.LoadFromFile(@"..\..\..\..\..\..\Data\FindAndReplace.doc");

            dgvMergeFiles.RowCount = 50;
            foreach (DataGridViewRow row1 in dgvValues.Rows)
            {
                if (row1.Index >= 0 & row1.Index <= 5)
                {
                    dgvMergeFiles.Rows.Add(1);
                    foreach (DataGridViewColumn col1 in dgvValues.Columns)
                    {
                        if (col1.Index >= 0 & col1.Index <= 3)
                        {
                            dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].Value = dgvValues.Rows[row1.Index].Cells[col1.Index].Value;
                            //if (dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].ToString().Contains("Part") & row1.Index == 5)
                            //{
                            //    dgvMergeFiles.Rows[row1.Index].Cells[4].Value = "Report No";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[5].Value = "Issued Date";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[6].Value = "Report Sample Description";
                            //}
                        }
                    }
                }
            }

            dgvMergeFiles.Rows[5].Cells[4].Value = "Report No";
            dgvMergeFiles.Rows[5].Cells[5].Value = "Issued Date";
            dgvMergeFiles.Rows[5].Cells[6].Value = "Report Sample Description";

            int RowChk, RowNo;
            int SampleTot;

            SampleTot = 0;
            RowNo = 0;
            RowChk = 0;

            foreach (DataGridViewRow row1 in dgvValues.Rows)
            {
                if (row1.Index >= 6 & row1.Index <= dgvValues.Rows.Count - 2)
                {
                    dgvValues.Rows[row1.Index].Cells[4].Value = "";
                    dgvValues.Rows[row1.Index].Cells[5].Value = "";

                    if (RowChk >= 1)
                    {
                        RowChk = RowChk - 1;
                    }
                    RowNo = RowChk + row1.Index;
                    foreach (DataGridViewColumn col1 in dgvValues.Columns)
                    {
                        if (col1.Index >= 0 & col1.Index <= 3)
                        {
                            dgvMergeFiles.Rows[RowNo].Cells[col1.Index].Value = dgvValues.Rows[row1.Index].Cells[col1.Index].Value;
                        }
                    }

                    foreach (DataGridViewRow row2 in dgvFiles.Rows)
                    {
                        //Create word document
                        if (row2.Index <= dgvFiles.Rows.Count - 2)
                        {
                            Document document = new Document();
                            string ext = System.IO.Path.GetExtension((string)row2.Cells[0].Value).ToUpper();

                            if (ext.Equals(".DOCX") || ext.Equals(".DOC"))
                            {

                                document.LoadFromFile(@txtDir1.Text + '\\' + row2.Cells[0].Value);

                                //Find text
                                textBox1.Text = (string)row1.Cells[1].Value;
                                TextSelection[] textSelections = document.FindAllString(this.textBox1.Text.Trim(), true, true);
                                if (textSelections == null)
                                {
                                    //MessageBox.Show("Not found.");
                                }
                                else
                                //Set hightlight
                                {
                                    plainText = document.GetText();
                                    this.textBox3.Text = plainText;

                                    //// Make the columns autosize.
                                    //foreach (DataGridViewColumn col in dgvValues.Columns)
                                    //    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;


                                    if ((string)dgvValues.Rows[row1.Index].Cells[4].Value == "")
                                        dgvValues.Rows[row1.Index].Cells[4].Value = row2.Cells[0].Value;
                                    else
                                        dgvValues.Rows[row1.Index].Cells[5].Value = row2.Cells[0].Value;

                                    foreach (DataGridViewColumn col1 in dgvValues.Columns)
                                    {
                                        if (col1.Index >= 0 & col1.Index <= 3)
                                        {
                                            dgvMergeFiles.Rows[RowNo].Cells[col1.Index].Value = dgvValues.Rows[row1.Index].Cells[col1.Index].Value;
                                        }
                                    }
                                    dgvMergeFiles.Rows[RowNo].Cells[4].Value = row2.Cells[0].Value;
                                    RowChk = RowChk + 1;
                                    RowNo = RowChk + row1.Index;


                                    //파일 내용을 분해하는 단계 시작------------------------
                                    // Get the file's text.
                                    string whole_file = textBox3.Text;

                                    // Split into lines.
                                    //\r\n
                                    whole_file = whole_file.Replace('\n', '\r');
                                    //whole_file = whole_file.Replace('"', ' ');
                                    string[] lines = whole_file.Split(new char[] { '\r' },
                                        StringSplitOptions.RemoveEmptyEntries);

                                    // See how many rows and columns there are.
                                    int num_rows = lines.Length;
                                    int num_cols = 1;
                                    //int num_cols = lines[num_rows - 1].Split(',').Length;

                                    // Allocate the data array.
                                    string[,] values = new string[num_rows, num_cols];

                                    // Load the array.
                                    for (int r = 0; r < num_rows; r++)
                                    {
                                        //string[] line_r = lines[r].Split(',');
                                        string[] line_r = lines[r].Split(new char[] { '\r' },
                                        StringSplitOptions.RemoveEmptyEntries);
                                        for (int c = 0; c < num_cols; c++)
                                        {
                                            if (line_r[c] == null)
                                                values[r, c] = "";
                                            else
                                                values[r, c] = line_r[c];
                                        }
                                    }

                                    int num1_rows = values.GetUpperBound(0) + 1;
                                    int num1_cols = values.GetUpperBound(1) + 1;

                                    // Display the data to show we have it.

                                    // Make column headers.
                                    // For this example, we assume the first row
                                    // contains the column names.

                                    // Clear previous results.
                                    dgvResults.Rows.Clear();

                                    for (int c = 0; c < num1_cols; c++)
                                        dgvResults.Columns.Add(values[0, c], values[0, c]);

                                    // Add the data.
                                    for (int r = 1; r <= num1_rows; r++)
                                    {
                                        dgvResults.Rows.Add();
                                        for (int c = 0; c < num1_cols; c++)
                                        {
                                            dgvResults.Rows[r - 1].Cells[c].Value = values[r - 1, c];
                                        }
                                    }

                                    //파일 내용을 분해하는 단계 끝------------------------

                                    //string Workchk;
                                    //Workchk = "클릭하세요";
                                    MessageBox.Show(@txtDir1.Text + '\\' + row2.Cells[0].Value);

                                    //분해된 내용을 정리하는 단계 시작 ----------------------------
                                    string SourceValue, TargetValue, PreviousValue;
                                    int JobNoChk, JobNoRow, TargetRowNo, TargetColNo, SamResultchk, SampleCnt, SaveSampleCnt, SaveCurRow, i, j, ColNoChk;
                                    int SampleTotCnt;
                                    int[] nCol = new int[100];
                                    int[] wCol = new int[100];
                                    string[] nColValue = new string[100];

                                    JobNoChk = 0;
                                    JobNoRow = 0;
                                    TargetRowNo = 0;
                                    TargetColNo = 0;
                                    SamResultchk = 0;
                                    ColNoChk = 2;
                                    SampleCnt = 0;
                                    SaveSampleCnt = 0;
                                    SaveCurRow = 0;
                                    SampleTotCnt = 0;
                                    i = 0;
                                    j = 0;

                                    SourceValue = "";
                                    TargetValue = "";
                                    PreviousValue = "";

                                    foreach (DataGridViewRow row3 in dgvResults.Rows)
                                    {
                                        if (row3.Cells[0].Value != null)
                                            SourceValue = (string)row3.Cells[0].Value;
                                        else
                                            SourceValue = "";

                                        // SGS File No.
                                        if (SourceValue == "SGS File No.")
                                        {
                                            ColNoChk = ColNoChk + 1;

                                            TargetRowNo = 1;
                                            TargetColNo = ColNoChk;

                                            JobNoChk = 1;
                                            JobNoRow = row3.Index + 2;
                                        }

                                        // Style no./Item no.
                                        if (SourceValue == "Style no./Item no.")
                                        {

                                            ColNoChk = ColNoChk + 1;

                                            TargetRowNo = 2;
                                            TargetColNo = ColNoChk;

                                            JobNoChk = 1;
                                            JobNoRow = row3.Index + 2;
                                        }

                                        // Issued Date
                                        if (SourceValue.ToString().Contains("Issued Date"))
                                        {

                                            ColNoChk = ColNoChk + 1;

                                            TargetRowNo = 3;
                                            TargetColNo = ColNoChk;

                                            JobNoChk = 1;
                                            JobNoRow = row3.Index + 2;

                                            TargetValue = SourceValue.Substring(SourceValue.IndexOf("20"), 12);
                                        }

                                        // Aurora Sample Description
                                        if (SourceValue == "Sample Description")
                                        {

                                            ColNoChk = ColNoChk + 1;

                                            TargetRowNo = 4;
                                            TargetColNo = ColNoChk;

                                            JobNoChk = 1;
                                            JobNoRow = row3.Index + 2;
                                        }

                                        //Permissible Limit (mg/kg)
                                        if (SourceValue == "Permissible Limit (mg/kg)")
                                        {
                                            SamResultchk = 1;
                                            SampleCnt = 0;
                                        }
                                        else if (SamResultchk == 1 & SourceValue != "Mass of trace amount")
                                        {
                                            SampleCnt = SampleCnt + 1;
                                            ColNoChk = ColNoChk + SampleCnt;

                                            nCol[SampleCnt] = ColNoChk;
                                            nColValue[SampleCnt] = SourceValue;

                                            for (i = 1; i <= nCol.LongLength - 1; i++)
                                            {
                                                if (nColValue[i] == SourceValue)
                                                {
                                                    wCol[SampleCnt] = nCol[i];
                                                }
                                            }

                                            TargetRowNo = 7;
                                            TargetColNo = ColNoChk;

                                            JobNoChk = 1;
                                            JobNoRow = row3.Index;
                                        }
                                        else if (SamResultchk == 1 & SourceValue == "Mass of trace amount")
                                        {
                                            SamResultchk = 0;
                                            SampleTotCnt = SampleCnt;
                                            SaveSampleCnt = SampleCnt;
                                            SampleCnt = 0;
                                        }
                                        //(mg)
                                        else if (SourceValue == "(mg)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = 9;

                                        }
                                        else if (SourceValue == "(Al)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Sb)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(As)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Ba)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(B)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Cd)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Cr)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Cr (III))")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Cr (VI))")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Co)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Cu)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Pb)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Mn)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Hg)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Ni)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Se)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Sr)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Sn)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "Soluble Organic Tin" || SourceValue == "Soluble Organic Tin^")
                                        {
                                            //ExceptChk = 1;
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            //TargetRowNo = TargetRowNo + 1 ;
                                            PreviousValue = SourceValue;
                                        }
                                        else if (SourceValue == "--" & (PreviousValue == "Soluble Organic Tin" || PreviousValue == "Soluble Organic Tin^"))
                                        {
                                            //ExceptChk = 1;
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            PreviousValue = "";
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Zn)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        //Soluble Organic Tin Result(s) (mg/kg)
                                        else if (SourceValue == "Soluble Organic Tin Result(s) (mg/kg)")
                                        {
                                            SamResultchk = 2;
                                            SampleCnt = 0;
                                            ColNoChk = 2;
                                            j = 0;
                                        }
                                        else if (SamResultchk == 2 & SourceValue != "Methyl tin")
                                        {
                                            SampleCnt = SampleCnt + 1;
                                            for (i = 1; i <= nCol.LongLength - 1; i++)
                                            {
                                                if (nColValue[i] == SourceValue)
                                                {
                                                    j = j + 1;
                                                    wCol[j] = nCol[i];
                                                }
                                            }
                                            JobNoChk = 1;
                                            JobNoRow = 0;
                                        }
                                        else if (SamResultchk == 2 & SourceValue == "Methyl tin")
                                        {
                                            SamResultchk = 0;
                                            SaveSampleCnt = SampleCnt - 1;
                                            SampleCnt = 0;
                                        }
                                        else if (SourceValue == "(MeT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 3;
                                        }
                                        else if (SourceValue == "(DProT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(BuT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(DBT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(TBT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(MOT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(TeBT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(DPhT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(DOT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(TPhT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SamResultchk == SaveCurRow & SaveSampleCnt != SampleCnt)
                                        {
                                            SampleCnt = SampleCnt + 1;
                                            ColNoChk = ColNoChk + SampleCnt;

                                            //TargetRowNo = 9;
                                            if (SampleCnt > 0)
                                                TargetColNo = wCol[SampleCnt];
                                            else
                                                TargetColNo = ColNoChk;

                                            JobNoChk = 1;
                                            JobNoRow = row3.Index;
                                        }
                                        else if (SamResultchk == SaveCurRow & SaveSampleCnt == SampleCnt)
                                        {
                                            SamResultchk = 0;
                                            SampleCnt = 0;

                                        }

                                        // 각 항목별로 값 적용 
                                        if (row3.Index == JobNoRow & JobNoChk == 1)
                                        {
                                            JobNoChk = 0;
                                            JobNoRow = 0;
                                            if (TargetRowNo == 3)
                                            {
                                                dgvFinalMerge.Rows[TargetRowNo].Cells[3].Value = TargetValue;
                                                TargetValue = "";
                                                ColNoChk = ColNoChk - 1;
                                            }
                                            else
                                            {
                                                if (PreviousValue != "Soluble Organic Tin^")
                                                {
                                                    dgvFinalMerge.Rows[TargetRowNo].Cells[0].Value = TargetRowNo;
                                                    dgvFinalMerge.Rows[TargetRowNo].Cells[TargetColNo].Value = row3.Cells[0].Value;
                                                    if (SampleCnt < 1)
                                                        ColNoChk = ColNoChk - 1;
                                                    else
                                                        ColNoChk = ColNoChk - SampleCnt;
                                                }
                                            }
                                        }

                                    }

                                    //분해된 내용을 정리하는 단계 시작 ----------------------------

                                    //개별 job의 샘플 결과를 취합하는 단계 시작 ---------------

                                    SampleTot = SampleTot + SampleTotCnt;

                                    for (int rr = 0; rr < 42; rr++)
                                    {
                                        //dgvFinal.Columns.Add(1);
                                        for (int cc = 3; cc < SampleTotCnt + 3; cc++)
                                        {
                                            if (rr < 9)
                                                dgvFinal.Rows[rr].Cells[SampleTot + cc - SampleTotCnt].Value = dgvFinalMerge.Rows[rr].Cells[3].Value;
                                            else
                                                dgvFinal.Rows[rr].Cells[SampleTot + cc - SampleTotCnt].Value = dgvFinalMerge.Rows[rr].Cells[cc].Value;
                                        }
                                    }
                                    //개별 job의 샘플 결과를 취합하는 단계 시작 ---------------


                                }
                            }
                        }

                    }
                }
            }

        }

        private void Find_Slim()
        {

            // 원래의 connect
            string connect_string = "Provider=SQLOLEDB.1;User ID=sa;pwd=krslimdb001;data source=krslimdb001;Persist Security Info=True;initial catalog=KRCTS01";
            //"Provider=SQLOLEDB.1;User ID=sa;pwd=krslimdb001;data source=krslimdb001;Persist Security Info=True;initial catalog=KRCTS01_BAK";

            //string connect_string = "Provider=SQLOLEDB.1;User ID=Aurora;pwd=Aurora2020;data source=krslimdb001;Persist Security Info=True;initial catalog=Aurora";
            Conn = new OleDbConnection(connect_string);

            int wRowNo, wEof;
            string wRowZero, sItemNumber;
            string wJobNo;
            wJobNo = "";

            wRowNo = 0;
            wRowZero = "";
            sItemNumber = "";

            dgvMergeFiles.Rows.Clear();


            //dgvMergeFiles.Rows[wRowNo].Cells[12].Value = "Item No";
            //dgvMergeFiles.Rows[wRowNo].Cells[12].Value = "Item No";
            //dgvMergeFiles.Rows[wRowNo].Cells[12].Value = "Item No";
            //dgvMergeFiles.Rows[wRowNo].Cells[11].Value = (string)dgvMergeFiles.Rows[wRowNo].Cells[0].Value;
            //dgvMergeFiles.Rows[wRowNo].Cells[12].Value = (string)dgvMergeFiles.Rows[wRowNo].Cells[1].Value;
            //return;
            dgvMergeFiles.RowCount = 1000;
            dgvMergeFiles.ColumnCount = 13;


            wEof = 0;
            wRowNo = -1;


            progressBar1.Maximum = dgvValues.RowCount;

            foreach (DataGridViewRow row1 in dgvValues.Rows)
            {


                wRowNo = wRowNo + 1;

                if (dgvValues.Rows[row1.Index].Cells[0].Value != null & (string)dgvValues.Rows[row1.Index].Cells[0].Value != "")
                {
                    dgvMergeFiles.Rows[wRowNo].Cells[0].Value = dgvValues.Rows[row1.Index].Cells[0].Value;

                    wRowZero = (string)dgvMergeFiles.Rows[wRowNo].Cells[0].Value;
                    wRowZero = wRowZero.Trim();
                }
                else if (wRowZero == "I&P1")
                {
                    dgvMergeFiles.Rows[wRowNo].Cells[11].Value = "Part 1,2";
                    dgvMergeFiles.Rows[wRowNo].Cells[12].Value = wJobNo;

                    wRowZero = "I&P2";
                }
                else if (wRowZero == "I&P2")
                {
                    dgvMergeFiles.Rows[wRowNo].Cells[11].Value = "Part 3";
                    dgvMergeFiles.Rows[wRowNo].Cells[12].Value = sItemNumber;
                    wRowZero = "";
                }

                if (dgvValues.Rows[row1.Index].Cells[1].Value != null & (string)dgvValues.Rows[row1.Index].Cells[1].Value != "")
                {
                    dgvMergeFiles.Rows[wRowNo].Cells[1].Value = dgvValues.Rows[row1.Index].Cells[1].Value;
                    if (wRowZero == "Item Number" || wRowZero == "Product Description")
                    {
                        dgvMergeFiles.Rows[wRowNo].Cells[11].Value = (string)dgvMergeFiles.Rows[wRowNo].Cells[0].Value;
                        dgvMergeFiles.Rows[wRowNo].Cells[12].Value = (string)dgvMergeFiles.Rows[wRowNo].Cells[1].Value;
                        if (wRowZero == "Item Number")
                        {
                            wJobNo = "";
                            sItemNumber = "";

                            sItemNumber = (string)dgvMergeFiles.Rows[wRowNo].Cells[12].Value;
                            sItemNumber = sItemNumber.Trim();

                            string query = "SELECT top 1 isnull(a.pro_proj,'') as pro_proj " + "\r" + "\n" +
                                         "  from  " + "\r" + "\n" +
                                         "  profjob a with (nolock), profjob_cuid b with (nolock), profjob_cuid_scheme c with (nolock), scheme d  with (nolock)" + "\r" + "\n" +
                                         "  WHERE a.labcode = b.labcode    " + "\r" + "\n" +
                                         "    and a.labcode = c.labcode    " + "\r" + "\n" +
                                         "    and a.labcode = d.labcode    " + "\r" + "\n" +
                                         "    and a.pro_job = b.pro_job    " + "\r" + "\n" +
                                         "    and a.pro_job = c.pro_job   " + "\r" + "\n" +
                                         "    and b.cuid = c.cuid    " + "\r" + "\n" +
                                "            and c.sch_code = d.sch_code " + "\r" + "\n" +
                                "            and c.schversion = d.schversion " + "\r" + "\n" +
                                "            and (d.schemename like '1809%' or d.schemename like '1831%')  " + "\r" + "\n" +
                                "            and a.orderno like '%" + sItemNumber + "%'" + "\r" + "\n" +
                                "            order by a.received desc ";

                            //string query = "select top 1 isnull(a1.pro_proj,'') as pro_proj  " +
                            //             "  from  " +
                            //             "  (SELECT a.*   " +
                            //             "     from     " +
                            //             "     profjob a, profjob_cuid b, profjob_cuid_scheme c, scheme d    " +
                            //             "     WHERE a.labcode = b.labcode     " +
                            //             "       and a.labcode = c.labcode    " +
                            //             "       and a.labcode = d.labcode   " +
                            //             "       and a.pro_job = b.pro_job   " +
                            //             "       and a.pro_job = c.pro_job    " +
                            //             "       and b.cuid = c.cuid       " +
                            //             "      and c.sch_code = d.sch_code    " +
                            //             "      and c.schversion = d.schversion    " +
                            //             "      and (d.schemename like '0324%'    or d.schemename like '0424%')     " +
                            //             "      and a.orderno like '%" + sItemNumber + "%'" +
                            //             "  union  " +
                            //             "  SELECT a.*   " +
                            //             "     from     " +
                            //             "     krcts01_bak.dbo.profjob a,
                            //             .profjob_cuid b, krcts01.dbo.profjob_cuid_scheme c, krcts01.dbo.scheme d    " +
                            //             "     WHERE a.labcode = b.labcode       " +
                            //             "       and a.labcode = c.labcode   " +
                            //             "       and a.labcode = d.labcode    " +
                            //             "       and a.pro_job = b.pro_job     " +
                            //             "       and a.pro_job = c.pro_job   " +
                            //             "       and b.cuid = c.cuid      " +
                            //             "      and c.sch_code = d.sch_code    " +
                            //             "      and c.schversion = d.schversion    " +
                            //             "      and (d.schemename like '0324%'    or d.schemename like '0424%')     " +
                            //             "      and a.orderno like '%" + sItemNumber + "%') as a1  " +
                            //             "      order by a1.received desc"; 

                            OleDbCommand cmd = new OleDbCommand(query, Conn);

                            Conn.Open();
                            using (OleDbDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if ((string)reader.GetValue(0) == null)
                                    {
                                        wJobNo = "";
                                    }
                                    else
                                    {
                                        wJobNo = (string)reader.GetValue(0);
                                        wJobNo = wJobNo.Trim();

                                    }

                                    wJobNo = wJobNo.Replace("\n", "");
                                    wEof = 1;
                                }
                                if (wEof == 1)
                                {
                                    wEof = 0;
                                }
                            }
                            Conn.Close();
                        }

                        if (wRowZero == "Product Description")
                        {
                            wRowZero = "I&P1";
                        }
                    }
                }

                if (dgvValues.Rows[row1.Index].Cells[2].Value != null || (string)dgvValues.Rows[row1.Index].Cells[2].Value != "")
                    dgvMergeFiles.Rows[wRowNo].Cells[2].Value = dgvValues.Rows[row1.Index].Cells[2].Value;

                if (dgvValues.Rows[row1.Index].Cells[3].Value != null || (string)dgvValues.Rows[row1.Index].Cells[3].Value != "")
                {
                    string Tjobno = "";
                    Tjobno = (string)dgvValues.Rows[row1.Index].Cells[3].Value;
                    dgvMergeFiles.Rows[wRowNo].Cells[3].Value = Tjobno;
                }

                string wPartName = (string)dgvMergeFiles.Rows[wRowNo].Cells[0].Value;

                if (wPartName == null)
                {
                    wPartName = "";
                }

                wPartName = wPartName.Trim();
                wPartName = wPartName.Replace(" ", "");
                wPartName = wPartName.ToUpper();

                if (wPartName == "PARTNAME")
                {

                    dgvMergeFiles.Rows[wRowNo].Cells[4].Value = "Sample Description";
                    dgvMergeFiles.Rows[wRowNo].Cells[5].Value = "Part Name";
                    dgvMergeFiles.Rows[wRowNo].Cells[6].Value = "Material No";
                    dgvMergeFiles.Rows[wRowNo].Cells[7].Value = "Report No";
                    dgvMergeFiles.Rows[wRowNo].Cells[8].Value = "Issued Date";
                    //dgvMergeFiles.Rows[wRowNo].Cells[8].Value = "Final Report";
                    dgvMergeFiles.Rows[wRowNo].Cells[9].Value = "Received Date";
                    dgvMergeFiles.Rows[wRowNo].Cells[10].Value = "Aurora ItemNo";
                    dgvMergeFiles.Rows[wRowNo].Cells[11].Value = "Report File";
                }

                // Get slim data
                string wMaterial = (string)dgvValues.Rows[row1.Index].Cells[1].Value;
                string wMaterialName = (string)dgvValues.Rows[row1.Index].Cells[2].Value;

                if (wMaterial == null)
                {
                    wMaterial = "";
                }

                if (wMaterialName == null)
                {
                    wMaterialName = "";
                }

                wMaterial = wMaterial.Trim();
                wMaterialName = wMaterialName.Trim();

                if (wMaterial != "" && wMaterialName != "")
                {
                    string query = "select distinct isnull(f1.sam_description,'') as sam_description, isnull(a1.pro_proj,'') as pro_proj, " +
                                 "       a1.finalised, a1.received, isnull(b1.jobcomments,''), c1.sampleident, a1.completed, a1.REQUIRED  " +
                                 "  from  " +
                                 "        (select distinct top 1 a9.labcode, a9.pro_job, a9.pro_proj, a9.jobcomments, a9.finalised, a9.received, a9.COMPLETED, a9.REQUIRED from " +
                                 "        (select distinct top 1 a.labcode, a.pro_job, a.pro_proj, isnull(b.jobcomments,'') jobcomments, " +
                                 "              a.finalised, a.received, a.COMPLETED, a.REQUIRED " +
                                 "           from profjob a with (nolock) inner join profjobuser b with (nolock) on (a.labcode = b.labcode and a.pro_job = b.pro_job and a.pro_proj like 'ayh%') " +
                                 "                        inner join profjob_scheme c  with (nolock) on (a.labcode = c.labcode and a.pro_job = c.pro_job and a.pro_proj like 'ayh%') " +
                                 "          where (c.SCH_CODE = 'HCEEASTMICP_09')  " + "\r" + "\n" +
                                 //"          where (c.SCH_CODE = 'HCEEASTMICP_05')  " + "\r" + "\n" +
                                 "            and a.pro_job like 'ayn%' " +
                                 "            and a.pro_proj like 'ayh%' " +
                                 "            and ','+ replace(b.jobcomments,' ','') + ',' like '%," + wMaterial.Replace(" ", "") + ",%' " +
                                 "          order by a.received desc " +
                                 "         union " +
                                 "         select distinct top 1 a.labcode, a.pro_job, a.pro_proj, isnull(b.jobcomments,'') jobcomments, " +
                                 "              a.finalised, a.received, a.COMPLETED, a.REQUIRED " +
                                 "           from KRCTS01.DBO.profjob a  with (nolock) inner join KRCTS01.DBO.profjobuser b  with (nolock) on (a.labcode = b.labcode and a.pro_job = b.pro_job and a.pro_proj like 'ayh%') " +
                                 "                        inner join KRCTS01.DBO.profjob_scheme c  with (nolock) on (a.labcode = c.labcode and a.pro_job = c.pro_job and a.pro_proj like 'ayh%') " +
                                 "          where (c.SCH_CODE = 'HCEEASTMICP_09')  " + "\r" + "\n" +
                                 //"          where (c.SCH_CODE = 'HCEEASTMICP_05')  " + "\r" + "\n" +
                                 "            and a.pro_job like 'ayn%' " +
                                 "            and a.pro_proj like 'ayh%' " +
                                 "            and ','+ replace(b.jobcomments,' ','') + ',' like '%," + wMaterial.Replace(" ", "") + ",%' " +
                                 "          order by a.received desc " +
                                 "          ) as a9 " +
                                 "          order by a9.received desc ) as a1 " +
                                 "  inner join (select * from profjobuser  with (nolock) union select * from KRCTS01.DBO.profjobuser with (nolock)) b1 on (a1.labcode = b1.labcode and a1.pro_job = b1.pro_job and a1.pro_proj like 'ayh%') " +
                                 "  inner join (select * from profjob_cuid  with (nolock) union select * from KRCTS01.DBO.profjob_cuid with (nolock)) c1 on (a1.labcode = c1.labcode and a1.pro_job = c1.pro_job and a1.pro_proj like 'ayh%') " +
                                 "  inner join (select * from profjob_cuiduser  with (nolock) union select * from KRCTS01.DBO.profjob_cuiduser with (nolock)) f1 on (c1.labcode = f1.labcode and c1.pro_job = f1.pro_job and c1.cuid = f1.cuid) " +
                                 "  where a1.pro_job like 'ayn%'" +
                                 "    and a1.pro_proj like 'ayh%'" +
                                 "  order by c1.sampleident ";

                    //2016-05-20 수정
                    //string query = "select distinct isnull(f1.sam_description,'') as sam_description, isnull(a1.pro_proj,'') as pro_proj, " + 
                    //             "       a1.finalised, a1.received, isnull(b1.jobcomments,''), c1.sampleident  " + 
                    //             "  from  " + 
                    //             "        (select distinct top 1 a.labcode, a.pro_job, a.pro_proj, isnull(b.jobcomments,'') jobcomments, " +
                    //             "              a.finalised, a.received " + 
                    //             "           from profjob a inner join profjobuser b on (a.labcode = b.labcode and a.pro_job = b.pro_job and a.pro_proj like 'ayh%') " +
                    //             "                        inner join profjob_scheme c on (a.labcode = c.labcode and a.pro_job = c.pro_job and a.pro_proj like 'ayh%') " +
                    //             "          where (c.sch_code = 'hceeenicp_15' or c.sch_code = 'hceeenicpms_02' or c.sch_code = 'hceeenicpms_03')  " + "\r" + "\n" +
                    //             "            and a.pro_job like 'ayn%' " +
                    //             "            and a.pro_proj like 'ayh%' " +
                    //             "            and ','+ replace(b.jobcomments,' ','') + ',' like '%," + wMaterial.Replace(" ", "") + ",%' " + 
                    //             "          order by a.received desc ) as a1 " + 
                    //             "  inner join profjobuser b1 on (a1.labcode = b1.labcode and a1.pro_job = b1.pro_job and a1.pro_proj like 'ayh%') " +
                    //             "  inner join profjob_cuid c1 on (a1.labcode = c1.labcode and a1.pro_job = c1.pro_job and a1.pro_proj like 'ayh%') " +
                    //             "  inner join profjob_cuiduser f1 on (c1.labcode = f1.labcode and c1.pro_job = f1.pro_job and c1.cuid = f1.cuid) " + 
                    //             "  where a1.pro_job like 'ayn%'" +
                    //             "    and a1.pro_proj like 'ayh%'" + 
                    //             "  order by c1.sampleident ";

                    //       string query = "SELECT distinct isnull(f1.sam_description,'') as sam_description, isnull(a1.pro_proj,'') as pro_proj, " + "\r" + "\n" +
                    //"       a1.finalised, a1.received, isnull(b1.jobcomments,''), c1.sampleident  " + "\r" + "\n" +
                    //"  from  " + "\r" + "\n" +
                    //"        (SELECT distinct top 1 a.labcode, a.pro_job, a.pro_proj, isnull(b.jobcomments,'') jobcomments, " +
                    //"              a.finalised, a.received " + "\r" + "\n" +
                    //"           from profjob a, profjobuser b, profjob_scheme c, scheme e " + "\r" + "\n" +
                    //"          where a.labcode = b.labcode " + "\r" + "\n" +
                    //"            and a.labcode = c.labcode " + "\r" + "\n" +
                    //"            and a.labcode = e.labcode " + "\r" + "\n" +
                    //"            and a.pro_job = b.pro_job " + "\r" + "\n" +
                    //"            and a.pro_job = c.pro_job " + "\r" + "\n" +
                    //"            and c.sch_code = e.sch_code " + "\r" + "\n" +
                    //"            and c.schversion = e.schversion " + "\r" + "\n" +
                    //"            and (e.schemename like '1523%'    or e.schemename like '1524%'    or e.schemename like '1525%')  " + "\r" + "\n" +
                    //"            and b.jobcomments + ',' like '%" + wMaterial + ",%' " + "\r" + "\n" +
                    //"       order by a.received desc) as a1, " + "\r" + "\n" +
                    //"  profjobuser b1, profjob_cuid c1, profjob_cuiduser f1 " + "\r" + "\n" +
                    //"  WHERE a1.labcode = b1.labcode    " + "\r" + "\n" +
                    //"    and a1.labcode = c1.labcode    " + "\r" + "\n" +
                    //"    and a1.labcode = f1.labcode   " + "\r" + "\n" +
                    //"    and a1.pro_job = b1.pro_job    " + "\r" + "\n" +
                    //"    and a1.pro_job = c1.pro_job   " + "\r" + "\n" +
                    //"    and a1.pro_job = f1.pro_job    " + "\r" + "\n" +
                    //"    and c1.cuid = f1.cuid " + "\r" + "\n" +
                    //"  order by c1.sampleident ";

                    OleDbCommand cmd = new OleDbCommand(query, Conn);

                    Conn.Open();
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        string wAuroraItemName;
                        CompareInfo myComp = CultureInfo.InvariantCulture.CompareInfo;

                        bool b, c;
                        int c1;

                        c1 = 0;
                        wAuroraItemName = "";

                        while (reader.Read())
                        {
                            if ((string)reader.GetValue(4) == null)
                            {
                                wAuroraItemName = "";
                            }
                            else
                            {
                                wAuroraItemName = (string)reader.GetValue(4);

                            }

                            if (wAuroraItemName.Length > 0)
                            {
                                b = wAuroraItemName.Contains("ITEM NO");
                                c = wAuroraItemName.Contains("2.AURORA");
                                if (c == true)
                                {
                                    c1 = myComp.IndexOf(wAuroraItemName, "2.AURORA");
                                }
                                if (b == true & c1 > 14)
                                {
                                    wAuroraItemName = wAuroraItemName.Substring(12, c1 - 15);
                                }
                            }
                            wAuroraItemName = wAuroraItemName.Replace("\n", "");

                            dgvMergeFiles.Rows[wRowNo].Cells[0].Value = dgvValues.Rows[row1.Index].Cells[0].Value;
                            dgvMergeFiles.Rows[wRowNo].Cells[1].Value = dgvValues.Rows[row1.Index].Cells[1].Value;
                            dgvMergeFiles.Rows[wRowNo].Cells[2].Value = dgvValues.Rows[row1.Index].Cells[2].Value;
                            // Bom파일에서 Country of Origin Column에 해당되는 값
                            dgvMergeFiles.Rows[wRowNo].Cells[3].Value = dgvValues.Rows[row1.Index].Cells[3].Value.ToString().Trim();
                            dgvMergeFiles.Rows[wRowNo].Cells[4].Value = (string)reader.GetValue(0);
                            dgvMergeFiles.Rows[wRowNo].Cells[5].Value = dgvMergeFiles.Rows[wRowNo].Cells[0].Value;
                            dgvMergeFiles.Rows[wRowNo].Cells[6].Value = dgvMergeFiles.Rows[wRowNo].Cells[1].Value;
                            dgvMergeFiles.Rows[wRowNo].Cells[7].Value = (string)reader.GetValue(1);
                            dgvMergeFiles.Rows[wRowNo].Cells[8].Value = (DateTime)reader.GetValue(7); // profjob - required
                            dgvMergeFiles.Rows[wRowNo].Cells[9].Value = (DateTime)reader.GetValue(3);
                            dgvMergeFiles.Rows[wRowNo].Cells[10].Value = wAuroraItemName;
                            //dgvMergeFiles.Rows[wRowNo].Cells[9].Value = wRowNo ;

                            // Bom파일에서 Country of Origin Column에 해당되는 값과 FileNo가 같지 않다면 빨간색
                            if ((string)dgvMergeFiles.Rows[wRowNo].Cells[3].Value != (string)dgvMergeFiles.Rows[wRowNo].Cells[7].Value)
                            {
                                // 이부분은 강정은 차장님께서 기능없애달라고 한 부분 24.04.26
                                //dgvMergeFiles.Rows[wRowNo].Cells[7].Style.BackColor = Color.Red;
                            }
                            else
                            {
                                dgvMergeFiles.Rows[wRowNo].Cells[7].Style.BackColor = Color.White;
                            }

                            if (!wMaterial.Equals("Materials"))
                            {
                                // Slim의 profjob에 finalised (컴플릿 되어도, 성적서 발행하지 않았다는 의미)
                                string test = reader.GetValue(2).ToString();

                                // COMPLETED가 NULL이거나 빈값인 경우 공백만 있는 경우, 1899년도인 경우 LightGreen
                                if (string.IsNullOrEmpty(test))
                                {
                                    dgvMergeFiles.Rows[wRowNo].Cells[10].Style.BackColor = Color.LightGreen;
                                }
                                else if (string.IsNullOrWhiteSpace(test))
                                {
                                    dgvMergeFiles.Rows[wRowNo].Cells[10].Style.BackColor = Color.LightGreen;
                                }
                                else if (test.Substring(0, 4).Equals("1899"))
                                {
                                    dgvMergeFiles.Rows[wRowNo].Cells[10].Style.BackColor = Color.LightGreen;
                                }
                            }

                            //발행일이 9 개월 전이면 색변환 
                            DateTime dt1 = Convert.ToDateTime(dgvMergeFiles.Rows[wRowNo].Cells[9].Value);
                            DateTime dt2 = DateTime.Today;

                            int Diffmonth = 12*(dt2.Year - dt1.Year) + (dt2.Month - dt1.Month);

                            if (Diffmonth >= 8 )
                            {
                                dgvMergeFiles.Rows[wRowNo].Cells[9].Style.BackColor = Color.Yellow;
                            }
                            else
                            {
                                dgvMergeFiles.Rows[wRowNo].Cells[9].Style.BackColor = Color.White;
                            }
                           
                            wRowNo = wRowNo + 1;
                            wEof = 1;                           

                        }
                        if (wEof == 1)
                        {
                            wRowNo = wRowNo - 1;
                            wEof = 0;
                        }
                    }
                    Conn.Close();

                    progressBar1.Value = row1.Index;

                }
            }
            dgvMergeFiles.RowCount = wRowNo + 1;
            progressBar1.Value = progressBar1.Maximum;
        }

        private void Find_Slim2()
        {
            string wMaterial = "";
            string Cellchk = "";
            string wMaterialName = "";
            string wRptNo = "";
            int wRowNo = -1;
            int wEof = 0;

            string connect_string2 =
                //"Provider=SQLOLEDB.1;User ID=sa;pwd=krslimdb001;data source=krslimdb001;Persist Security Info=True;initial catalog=KRCTS01";
            "Provider=SQLOLEDB.1;User ID=sa;pwd=KRSLIMDBOLD001;data source=KRSLIMDBOLD001;Persist Security Info=True;initial catalog=KRCTS01_BAK2";
            Conn = new OleDbConnection(connect_string2);

            progressBar1.Maximum = dgvMergeFiles.RowCount;

            foreach (DataGridViewRow row1 in dgvMergeFiles.Rows)
            {
                wRowNo = row1.Index;

                if (row1.Cells[0].Value == null)
                {
                    Cellchk = "";
                }
                else
                {
                    Cellchk = row1.Cells[0].Value.ToString();
                }

                if (Cellchk.ToString().IndexOf("Item") > -1)
                {

                }
                else if (Cellchk.ToString().IndexOf("Product") > -1)
                {

                }
                else if (Cellchk.ToString().IndexOf("Part") > -1)
                {

                }
                else if (Cellchk.ToString().IndexOf("Name") > -1)
                {

                }
                else if (Cellchk.ToString().Trim().Length == 0)
                {

                }
                else
                {

                    
                    if (row1.Cells[7].Value == null)
                    {
                        wRptNo = "";
                    }
                    else
                    {
                        wRptNo = row1.Cells[7].Value.ToString();
                    }

                    if (row1.Cells[1].Value != null)
                    {
                        wMaterial = row1.Cells[1].Value.ToString();
                    }

                    if (wMaterial != "" && wMaterial != null && wRptNo == "")
                    {
                        string query = "select distinct isnull(f1.sam_description,'') as sam_description, isnull(a1.pro_proj,'') as pro_proj, " + "\r" + "\n" +
                                     "       a1.finalised, a1.received, isnull(b1.jobcomments,''), c1.sampleident, a1.completed  " + "\r" + "\n" +
                                     "  from  " + "\r" + "\n" +
                                     "        (select distinct top 1 a.labcode, a.pro_job, a.pro_proj, isnull(b.jobcomments,'') jobcomments, " +
                                     "              a.finalised, a.completed, a.received " + "\r" + "\n" +
                                     "           from profjob a  with (nolock) inner join profjobuser b  with (nolock) on (a.labcode = b.labcode and a.pro_job = b.pro_job and a.pro_proj like 'ayh%') " +
                                     "                        inner join profjob_scheme c with (nolock) on (a.labcode = c.labcode and a.pro_job = c.pro_job and a.pro_proj like 'ayh%') " +
                                     "          where (c.sch_code = 'HCEEASTMICP_09')  " + "\r" + "\n" +
                                     //"          where (c.sch_code = 'HCEEASTMICP_05')  " + "\r" + "\n" +
                                     "            and a.pro_job like 'ayn%' " +
                                     "            and a.pro_proj like 'ayh%' " +
                                     "            and ','+ replace(b.jobcomments,' ','') + ',' like '%," + wMaterial.Replace(" ", "") + ",%' " + "\r" + "\n" +
                                     "          order by a.received desc ) as a1 " + "\r" + "\n" +
                                     "  inner join profjobuser b1  with (nolock) on (a1.labcode = b1.labcode and a1.pro_job = b1.pro_job and a1.pro_proj like 'ayh%') " +
                                     "  inner join profjob_cuid c1  with (nolock) on (a1.labcode = c1.labcode and a1.pro_job = c1.pro_job and a1.pro_proj like 'ayh%') " +
                                     "  inner join profjob_cuiduser f1  with (nolock) on (c1.labcode = f1.labcode and c1.pro_job = f1.pro_job and c1.cuid = f1.cuid) " + "\r" + "\n" +
                                     "  where a1.pro_job like 'ayn%'" + "\r" + "\n" +
                                     "    and a1.pro_proj like 'ayh%'" + "\r" + "\n" +
                                     "  order by c1.sampleident ";

                        OleDbCommand cmd = new OleDbCommand(query, Conn);

                        Conn.Open();
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            string wAuroraItemName;
                            CompareInfo myComp = CultureInfo.InvariantCulture.CompareInfo;

                            bool b, c;
                            int c1;

                            c1 = 0;
                            wAuroraItemName = "";

                            while (reader.Read())
                            {
                                if ((string)reader.GetValue(4) == null)
                                {
                                    wAuroraItemName = "";
                                }
                                else
                                {
                                    wAuroraItemName = (string)reader.GetValue(4);

                                }

                                if (wAuroraItemName.Length > 0)
                                {
                                    b = wAuroraItemName.Contains("ITEM NO");
                                    c = wAuroraItemName.Contains("2.AURORA");
                                    if (c == true)
                                    {
                                        c1 = myComp.IndexOf(wAuroraItemName, "2.AURORA");
                                    }
                                    if (b == true & c1 > 14)
                                    {
                                        wAuroraItemName = wAuroraItemName.Substring(12, c1 - 15);
                                    }
                                }
                                wAuroraItemName = wAuroraItemName.Replace("\n", "");

                                dgvMergeFiles.Rows[wRowNo].Cells[5].Value = dgvMergeFiles.Rows[wRowNo].Cells[0].Value;
                                dgvMergeFiles.Rows[wRowNo].Cells[6].Value = dgvMergeFiles.Rows[wRowNo].Cells[1].Value;

                                row1.Cells[4].Value = (string)reader.GetValue(0);
                                row1.Cells[7].Value = (string)reader.GetValue(1);
                                row1.Cells[8].Value = (DateTime)reader.GetValue(2);
                                row1.Cells[9].Value = (DateTime)reader.GetValue(3);
                                row1.Cells[10].Value = wAuroraItemName;
                                //dgvMergeFiles.Rows[wRowNo].Cells[9].Value = wRowNo ;

                                if ((string)row1.Cells[3].Value != (string)row1.Cells[7].Value)
                                {
                                    row1.Cells[7].Style.BackColor = Color.Red;
                                }
                                else
                                {
                                    row1.Cells[7].Style.BackColor = Color.White;
                                }
                                                                
                                //발행일이 9 개월 전이면 색변환 
                                DateTime dt1 = Convert.ToDateTime(dgvMergeFiles.Rows[wRowNo].Cells[9].Value);
                                DateTime dt2 = DateTime.Today;

                                int Diffmonth = 12 * (dt2.Year - dt1.Year) + (dt2.Month - dt1.Month);

                                if (Diffmonth >= 8)
                                {
                                    dgvMergeFiles.Rows[wRowNo].Cells[9].Style.BackColor = Color.Yellow;
                                }
                                else
                                {
                                    dgvMergeFiles.Rows[wRowNo].Cells[9].Style.BackColor = Color.White;
                                }


                                wEof = 1;

                            }
                            if (wEof == 1)
                            {
                                wEof = 0;
                            }
                        }
                        Conn.Close();

                        progressBar1.Value = row1.Index;
                    }
                }
            }
            dgvMergeFiles.RowCount = wRowNo + 1;
            progressBar1.Value = progressBar1.Maximum;
        }

        private void Find_Report()
        {
            string plainText = string.Empty;

            //load a document
            //string fileName = OpenFile();
            //document.LoadFromFile(@"..\..\..\..\..\..\Data\FindAndReplace.doc");

            dgvMergeFiles.RowCount = 50;
            foreach (DataGridViewRow row1 in dgvValues.Rows)
            {
                if (row1.Index >= 0 & row1.Index <= 5)
                {
                    dgvMergeFiles.Rows.Add(1);
                    foreach (DataGridViewColumn col1 in dgvValues.Columns)
                    {
                        if (col1.Index >= 0 & col1.Index <= 3)
                        {
                            dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].Value = dgvValues.Rows[row1.Index].Cells[col1.Index].Value;
                            //if (dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].ToString().Contains("Part") & row1.Index == 5)
                            //{
                            //    dgvMergeFiles.Rows[row1.Index].Cells[4].Value = "Report No";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[5].Value = "Issued Date";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[6].Value = "Report Sample Description";
                            //}
                        }
                    }
                }
            }

            dgvMergeFiles.Rows[5].Cells[4].Value = "Report No";
            dgvMergeFiles.Rows[5].Cells[5].Value = "Issued Date";
            dgvMergeFiles.Rows[5].Cells[6].Value = "Report Sample Description";

            int RowChk, RowNo;

            RowNo = 0;
            RowChk = 0;

            foreach (DataGridViewRow row1 in dgvValues.Rows)
            {
                progressBar1.Maximum = dgvValues.Rows.Count;
                progressBar1.Value = (int)dgvValues.Rows.Count / 2;
                if (row1.Index >= 6 & row1.Index <= dgvValues.Rows.Count - 2)
                {
                    dgvValues.Rows[row1.Index].Cells[4].Value = "";
                    dgvValues.Rows[row1.Index].Cells[5].Value = "";

                    if (RowChk >= 1)
                    {
                        RowChk = RowChk - 1;
                    }
                    RowNo = RowChk + row1.Index;
                    foreach (DataGridViewColumn col1 in dgvValues.Columns)
                    {
                        if (col1.Index >= 0 & col1.Index <= 3)
                        {
                            dgvMergeFiles.Rows[RowNo].Cells[col1.Index].Value = dgvValues.Rows[row1.Index].Cells[col1.Index].Value;
                        }
                    }

                    foreach (DataGridViewRow row2 in dgvFiles.Rows)
                    {
                        //Create word document
                        if (row2.Index <= dgvFiles.Rows.Count - 2)
                        {
                            Document document = new Document();
                            string ext = System.IO.Path.GetExtension((string)row2.Cells[0].Value).ToUpper();

                            if (ext.Equals(".DOCX") || ext.Equals(".DOC"))
                            {

                                document.LoadFromFile(@txtDir1.Text + '\\' + row2.Cells[0].Value);

                                //Find text
                                textBox1.Text = (string)row1.Cells[1].Value;
                                TextSelection[] textSelections = document.FindAllString(this.textBox1.Text.Trim(), true, true);
                                if (textSelections == null)
                                {
                                    //MessageBox.Show("Not found.");
                                }
                                else
                                //Set hightlight
                                {
                                    plainText = document.GetText();
                                    this.textBox3.Text = plainText;

                                    //// Make the columns autosize.
                                    //foreach (DataGridViewColumn col in dgvValues.Columns)
                                    //    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;


                                    if ((string)dgvValues.Rows[row1.Index].Cells[4].Value == "")
                                        dgvValues.Rows[row1.Index].Cells[4].Value = row2.Cells[0].Value;
                                    else
                                        dgvValues.Rows[row1.Index].Cells[5].Value = row2.Cells[0].Value;

                                    foreach (DataGridViewColumn col1 in dgvValues.Columns)
                                    {
                                        if (col1.Index >= 0 & col1.Index <= 3)
                                        {
                                            dgvMergeFiles.Rows[RowNo].Cells[col1.Index].Value = dgvValues.Rows[row1.Index].Cells[col1.Index].Value;
                                        }
                                    }
                                    dgvMergeFiles.Rows[RowNo].Cells[4].Value = row2.Cells[0].Value;
                                    RowChk = RowChk + 1;
                                    RowNo = RowChk + row1.Index;

                                }
                            }
                        }
                    }
                }
            }
            progressBar1.Value = dgvValues.Rows.Count;
        }


        private void Find_Material()
        {
            string plainText = string.Empty;

            //load a document
            //string fileName = OpenFile();
            //document.LoadFromFile(@"..\..\..\..\..\..\Data\FindAndReplace.doc");

            dgvMergeFiles.RowCount = 50;
            foreach (DataGridViewRow row1 in dgvValues.Rows)
            {
                if (row1.Index >= 0 & row1.Index <= 5)
                {
                    dgvMergeFiles.Rows.Add(1);
                    foreach (DataGridViewColumn col1 in dgvValues.Columns)
                    {
                        if (col1.Index >= 0 & col1.Index <= 3)
                        {
                            dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].Value = dgvValues.Rows[row1.Index].Cells[col1.Index].Value;
                            //if (dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].ToString().Contains("Part") & row1.Index == 5)
                            //{
                            //    dgvMergeFiles.Rows[row1.Index].Cells[4].Value = "Report No";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[5].Value = "Issued Date";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[6].Value = "Report Sample Description";
                            //}
                        }
                    }
                }
            }

            dgvMergeFiles.Rows[5].Cells[4].Value = "Report No";
            dgvMergeFiles.Rows[5].Cells[5].Value = "Issued Date";
            dgvMergeFiles.Rows[5].Cells[6].Value = "Report Sample Description";

            int RowChk, RowNo;
            int SampleTot;

            SampleTot = 0;
            RowNo = 0;
            RowChk = 0;

            foreach (DataGridViewRow row1 in dgvValues.Rows)
            {
                progressBar1.Maximum = dgvValues.Rows.Count;
                progressBar1.Value = (int)dgvValues.Rows.Count / 2;

                if (row1.Index >= 6 & row1.Index <= dgvValues.Rows.Count - 2)
                {
                    dgvValues.Rows[row1.Index].Cells[4].Value = "";
                    dgvValues.Rows[row1.Index].Cells[5].Value = "";

                    if (RowChk >= 1)
                    {
                        RowChk = RowChk - 1;
                    }
                    RowNo = RowChk + row1.Index;
                    foreach (DataGridViewColumn col1 in dgvValues.Columns)
                    {
                        if (col1.Index >= 0 & col1.Index <= 3)
                        {
                            dgvMergeFiles.Rows[RowNo].Cells[col1.Index].Value = dgvValues.Rows[row1.Index].Cells[col1.Index].Value;
                        }
                    }

                    foreach (DataGridViewRow row2 in dgvFiles.Rows)
                    {
                        //Create word document
                        if (row2.Index <= dgvFiles.Rows.Count - 2)
                        {
                            Document document = new Document();
                            string ext = System.IO.Path.GetExtension((string)row2.Cells[0].Value).ToUpper();

                            if (ext.Equals(".DOCX") || ext.Equals(".DOC"))
                            {

                                document.LoadFromFile(@txtDir1.Text + '\\' + row2.Cells[0].Value);

                                //Find text
                                textBox1.Text = (string)row1.Cells[1].Value;
                                TextSelection[] textSelections = document.FindAllString(this.textBox1.Text.Trim(), true, true);
                                if (textSelections == null)
                                {
                                    //MessageBox.Show("Not found.");
                                }
                                else
                                //Set hightlight
                                {
                                    plainText = document.GetText();
                                    this.textBox3.Text = plainText;

                                    //// Make the columns autosize.
                                    //foreach (DataGridViewColumn col in dgvValues.Columns)
                                    //    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;


                                    if ((string)dgvValues.Rows[row1.Index].Cells[4].Value == "")
                                        dgvValues.Rows[row1.Index].Cells[4].Value = row2.Cells[0].Value;
                                    else
                                        dgvValues.Rows[row1.Index].Cells[5].Value = row2.Cells[0].Value;

                                    foreach (DataGridViewColumn col1 in dgvValues.Columns)
                                    {
                                        if (col1.Index >= 0 & col1.Index <= 3)
                                        {
                                            dgvMergeFiles.Rows[RowNo].Cells[col1.Index].Value = dgvValues.Rows[row1.Index].Cells[col1.Index].Value;
                                        }
                                    }
                                    dgvMergeFiles.Rows[RowNo].Cells[4].Value = row2.Cells[0].Value;
                                    RowChk = RowChk + 1;
                                    RowNo = RowChk + row1.Index;


                                    //파일 내용을 분해하는 단계 시작------------------------
                                    // Get the file's text.
                                    string whole_file = textBox3.Text;

                                    // Split into lines.
                                    //\r\n
                                    whole_file = whole_file.Replace('\n', '\r');
                                    //whole_file = whole_file.Replace('"', ' ');
                                    string[] lines = whole_file.Split(new char[] { '\r' },
                                        StringSplitOptions.RemoveEmptyEntries);

                                    // See how many rows and columns there are.
                                    int num_rows = lines.Length;
                                    int num_cols = 1;
                                    //int num_cols = lines[num_rows - 1].Split(',').Length;

                                    // Allocate the data array.
                                    string[,] values = new string[num_rows, num_cols];

                                    // Load the array.
                                    for (int r = 0; r < num_rows; r++)
                                    {
                                        //string[] line_r = lines[r].Split(',');
                                        string[] line_r = lines[r].Split(new char[] { '\r' },
                                        StringSplitOptions.RemoveEmptyEntries);
                                        for (int c = 0; c < num_cols; c++)
                                        {
                                            if (line_r[c] == null)
                                                values[r, c] = "";
                                            else
                                                values[r, c] = line_r[c];
                                        }
                                    }

                                    int num1_rows = values.GetUpperBound(0) + 1;
                                    int num1_cols = values.GetUpperBound(1) + 1;

                                    // Display the data to show we have it.

                                    // Make column headers.
                                    // For this example, we assume the first row
                                    // contains the column names.

                                    // Clear previous results.
                                    dgvResults.Rows.Clear();

                                    for (int c = 0; c < num1_cols; c++)
                                        dgvResults.Columns.Add(values[0, c], values[0, c]);

                                    // Add the data.
                                    for (int r = 1; r <= num1_rows; r++)
                                    {
                                        dgvResults.Rows.Add();
                                        for (int c = 0; c < num1_cols; c++)
                                        {
                                            dgvResults.Rows[r - 1].Cells[c].Value = values[r - 1, c];
                                        }
                                    }

                                    //파일 내용을 분해하는 단계 끝------------------------

                                    //string Workchk;
                                    //Workchk = "클릭하세요";
                                    MessageBox.Show(@txtDir1.Text + '\\' + row2.Cells[0].Value);

                                    //분해된 내용을 정리하는 단계 시작 ----------------------------
                                    string SourceValue, TargetValue, PreviousValue;
                                    int JobNoChk, JobNoRow, TargetRowNo, TargetColNo, SamResultchk, SampleCnt, SaveSampleCnt, SaveCurRow, i, j, ColNoChk;
                                    int SampleTotCnt;
                                    int[] nCol = new int[100];
                                    int[] wCol = new int[100];
                                    string[] nColValue = new string[100];

                                    JobNoChk = 0;
                                    JobNoRow = 0;
                                    TargetRowNo = 0;
                                    TargetColNo = 0;
                                    SamResultchk = 0;
                                    ColNoChk = 2;
                                    SampleCnt = 0;
                                    SaveSampleCnt = 0;
                                    SaveCurRow = 0;
                                    SampleTotCnt = 0;
                                    i = 0;
                                    j = 0;

                                    SourceValue = "";
                                    TargetValue = "";
                                    PreviousValue = "";

                                    foreach (DataGridViewRow row3 in dgvResults.Rows)
                                    {
                                        if (row3.Cells[0].Value != null)
                                            SourceValue = (string)row3.Cells[0].Value;
                                        else
                                            SourceValue = "";

                                        // SGS File No.
                                        if (SourceValue == "SGS File No.")
                                        {
                                            ColNoChk = ColNoChk + 1;

                                            TargetRowNo = 1;
                                            TargetColNo = ColNoChk;

                                            JobNoChk = 1;
                                            JobNoRow = row3.Index + 2;
                                        }

                                        // Style no./Item no.
                                        if (SourceValue == "Style no./Item no.")
                                        {

                                            ColNoChk = ColNoChk + 1;

                                            TargetRowNo = 2;
                                            TargetColNo = ColNoChk;

                                            JobNoChk = 1;
                                            JobNoRow = row3.Index + 2;
                                        }

                                        // Issued Date
                                        if (SourceValue.ToString().Contains("Issued Date"))
                                        {

                                            ColNoChk = ColNoChk + 1;

                                            TargetRowNo = 3;
                                            TargetColNo = ColNoChk;

                                            JobNoChk = 1;
                                            JobNoRow = row3.Index + 2;

                                            TargetValue = SourceValue.Substring(SourceValue.IndexOf("20"), 12);
                                        }

                                        // Aurora Sample Description
                                        if (SourceValue == "Sample Description")
                                        {

                                            ColNoChk = ColNoChk + 1;

                                            TargetRowNo = 4;
                                            TargetColNo = ColNoChk;

                                            JobNoChk = 1;
                                            JobNoRow = row3.Index + 2;
                                        }

                                        //Permissible Limit (mg/kg)
                                        if (SourceValue == "Permissible Limit (mg/kg)" || SourceValue == "(mg/kg)")
                                        {
                                            SamResultchk = 1;
                                            SampleCnt = 0;
                                        }
                                        else if (SamResultchk == 1 & SourceValue != "Mass of trace amount")
                                        {
                                            SampleCnt = SampleCnt + 1;
                                            ColNoChk = ColNoChk + SampleCnt;

                                            nCol[SampleCnt] = ColNoChk;
                                            nColValue[SampleCnt] = SourceValue;

                                            for (i = 1; i <= nCol.LongLength - 1; i++)
                                            {
                                                if (nColValue[i] == SourceValue)
                                                {
                                                    wCol[SampleCnt] = nCol[i];
                                                }
                                            }

                                            TargetRowNo = 7;
                                            TargetColNo = ColNoChk;

                                            JobNoChk = 1;
                                            JobNoRow = row3.Index;
                                        }
                                        else if (SamResultchk == 1 & SourceValue == "Mass of trace amount")
                                        {
                                            SamResultchk = 0;
                                            SampleTotCnt = SampleCnt;
                                            SaveSampleCnt = SampleCnt;
                                            SampleCnt = 0;
                                        }
                                        //(mg)
                                        else if (SourceValue == "(mg)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = 9;

                                        }
                                        else if (SourceValue == "(Al)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Sb)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(As)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Ba)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(B)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Cd)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Cr)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Cr (III))")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Cr (VI))")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Co)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Cu)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Pb)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Mn)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Hg)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Ni)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Se)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Sr)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Sn)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "Soluble Organic Tin" || SourceValue == "Soluble Organic Tin^")
                                        {
                                            //ExceptChk = 1;
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            //TargetRowNo = TargetRowNo + 1 ;
                                            PreviousValue = SourceValue;
                                        }
                                        else if (SourceValue == "--" & (PreviousValue == "Soluble Organic Tin" || PreviousValue == "Soluble Organic Tin^"))
                                        {
                                            //ExceptChk = 1;
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            PreviousValue = "";
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(Zn)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        //Soluble Organic Tin Result(s) (mg/kg)
                                        else if (SourceValue == "Soluble Organic Tin Result(s) (mg/kg)")
                                        {
                                            SamResultchk = 2;
                                            SampleCnt = 0;
                                            ColNoChk = 2;
                                            j = 0;
                                        }
                                        else if (SamResultchk == 2 & SourceValue != "Methyl tin")
                                        {
                                            SampleCnt = SampleCnt + 1;
                                            for (i = 1; i <= nCol.LongLength - 1; i++)
                                            {
                                                if (nColValue[i] == SourceValue)
                                                {
                                                    j = j + 1;
                                                    wCol[j] = nCol[i];
                                                }
                                            }
                                            JobNoChk = 1;
                                            JobNoRow = 0;
                                        }
                                        else if (SamResultchk == 2 & SourceValue == "Methyl tin")
                                        {
                                            SamResultchk = 0;
                                            SaveSampleCnt = SampleCnt - 1;
                                            SampleCnt = 0;
                                        }
                                        else if (SourceValue == "(MeT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 3;
                                        }
                                        else if (SourceValue == "(DProT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(BuT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(DBT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(TBT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(MOT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(TeBT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(DPhT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(DOT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SourceValue == "(TPhT)")
                                        {
                                            SaveCurRow = row3.Index;
                                            SamResultchk = SaveCurRow;
                                            TargetRowNo = TargetRowNo + 1;
                                        }
                                        else if (SamResultchk == SaveCurRow & SaveSampleCnt != SampleCnt)
                                        {
                                            SampleCnt = SampleCnt + 1;
                                            ColNoChk = ColNoChk + SampleCnt;

                                            //TargetRowNo = 9;
                                            if (SampleCnt > 0)
                                                TargetColNo = wCol[SampleCnt];
                                            else
                                                TargetColNo = ColNoChk;

                                            JobNoChk = 1;
                                            JobNoRow = row3.Index;
                                        }
                                        else if (SamResultchk == SaveCurRow & SaveSampleCnt == SampleCnt)
                                        {
                                            SamResultchk = 0;
                                            SampleCnt = 0;

                                        }

                                        // 각 항목별로 값 적용 
                                        if (row3.Index == JobNoRow & JobNoChk == 1)
                                        {
                                            JobNoChk = 0;
                                            JobNoRow = 0;
                                            if (TargetRowNo == 3)
                                            {
                                                dgvFinalMerge.Rows[TargetRowNo].Cells[3].Value = TargetValue;
                                                TargetValue = "";
                                                ColNoChk = ColNoChk - 1;
                                            }
                                            else
                                            {
                                                if (PreviousValue != "Soluble Organic Tin^")
                                                {
                                                    dgvFinalMerge.Rows[TargetRowNo].Cells[0].Value = TargetRowNo;
                                                    dgvFinalMerge.Rows[TargetRowNo].Cells[TargetColNo].Value = row3.Cells[0].Value;
                                                    if (SampleCnt < 1)
                                                        ColNoChk = ColNoChk - 1;
                                                    else
                                                        ColNoChk = ColNoChk - SampleCnt;
                                                }
                                            }
                                        }

                                    }

                                    //분해된 내용을 정리하는 단계 시작 ----------------------------

                                    //개별 job의 샘플 결과를 취합하는 단계 시작 ---------------

                                    SampleTot = SampleTot + SampleTotCnt;

                                    for (int rr = 0; rr < 42; rr++)
                                    {
                                        //dgvFinal.Columns.Add(1);
                                        for (int cc = 3; cc < SampleTotCnt + 3; cc++)
                                        {
                                            if (rr < 9)
                                                dgvFinal.Rows[rr].Cells[SampleTot + cc - SampleTotCnt].Value = dgvFinalMerge.Rows[rr].Cells[3].Value;
                                            else
                                                dgvFinal.Rows[rr].Cells[SampleTot + cc - SampleTotCnt].Value = dgvFinalMerge.Rows[rr].Cells[cc].Value;
                                        }
                                    }
                                    //개별 job의 샘플 결과를 취합하는 단계 시작 ---------------


                                }
                            }
                        }

                    }
                }
            }
            progressBar1.Value = dgvValues.Rows.Count;
        }


        private void btnExtractResult_Click(object sender, EventArgs e)
        {
            // Get the file's text.
            string whole_file = textBox3.Text;

            // Split into lines.
            //\r\n
            whole_file = whole_file.Replace('\n', '\r');
            //whole_file = whole_file.Replace('"', ' ');
            string[] lines = whole_file.Split(new char[] { '\r' },
                StringSplitOptions.RemoveEmptyEntries);

            // See how many rows and columns there are.
            int num_rows = lines.Length;
            int num_cols = 1;
            //int num_cols = lines[num_rows - 1].Split(',').Length;

            // Allocate the data array.
            string[,] values = new string[num_rows, num_cols];

            // Load the array.
            for (int r = 0; r < num_rows; r++)
            {
                //string[] line_r = lines[r].Split(',');
                string[] line_r = lines[r].Split(new char[] { '\r' },
                StringSplitOptions.RemoveEmptyEntries);
                for (int c = 0; c < num_cols; c++)
                {
                    if (line_r[c] == null)
                        values[r, c] = "";
                    else
                        values[r, c] = line_r[c];
                }
            }

            int num1_rows = values.GetUpperBound(0) + 1;
            int num1_cols = values.GetUpperBound(1) + 1;

            // Display the data to show we have it.

            // Make column headers.
            // For this example, we assume the first row
            // contains the column names.

            // Clear previous results.
            dgvResults.Rows.Clear();

            for (int c = 0; c < num1_cols; c++)
                dgvResults.Columns.Add(values[0, c], values[0, c]);

            // Add the data.
            for (int r = 1; r <= num1_rows; r++)
            {
                dgvResults.Rows.Add();
                for (int c = 0; c < num1_cols; c++)
                {
                    dgvResults.Rows[r - 1].Cells[c].Value = values[r - 1, c];
                }
            }

            //// Make the columns autosize.
            //foreach (DataGridViewColumn col in dgvValues.Columns)
            //    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

        }

        private void button7_Click(object sender, EventArgs e)
        {
            dgvFinalMerge.RowCount = 43;
            dgvFinalMerge.ColumnCount = 100;

            dgvFinalMerge.Rows[1].Cells[0].Value = "Job No";
            dgvFinalMerge.Rows[2].Cells[0].Value = "Item No";
            dgvFinalMerge.Rows[3].Cells[0].Value = "Issue Date";
            dgvFinalMerge.Rows[4].Cells[0].Value = "Aurora Description";
            dgvFinalMerge.Rows[5].Cells[0].Value = "Screen Or Not";

            dgvFinalMerge.Rows[7].Cells[0].Value = "Sample No";
            dgvFinalMerge.Rows[8].Cells[0].Value = "Report Sample Description";
            dgvFinalMerge.Rows[9].Cells[0].Value = "Part3 Results";
            dgvFinalMerge.Rows[32].Cells[0].Value = "Organotin Results";

            dgvFinalMerge.Rows[9].Cells[1].Value = "Mass of trace amount";
            dgvFinalMerge.Rows[10].Cells[1].Value = "Soluble Aluminium";
            dgvFinalMerge.Rows[11].Cells[1].Value = "Soluble Antimony";
            dgvFinalMerge.Rows[12].Cells[1].Value = "Soluble Arsenic";
            dgvFinalMerge.Rows[13].Cells[1].Value = "Soluble Barium";
            dgvFinalMerge.Rows[14].Cells[1].Value = "Soluble Boron";
            dgvFinalMerge.Rows[15].Cells[1].Value = "Soluble Cadmium";
            dgvFinalMerge.Rows[16].Cells[1].Value = "Soluble Chromium";
            dgvFinalMerge.Rows[17].Cells[1].Value = "Soluble Chromium (III) #";
            dgvFinalMerge.Rows[18].Cells[1].Value = "Soluble Chromium (VI) #";
            dgvFinalMerge.Rows[19].Cells[1].Value = "Soluble Cobalt";
            dgvFinalMerge.Rows[20].Cells[1].Value = "Soluble Copper";
            dgvFinalMerge.Rows[21].Cells[1].Value = "Soluble Lead";
            dgvFinalMerge.Rows[22].Cells[1].Value = "Soluble Manganese";
            dgvFinalMerge.Rows[23].Cells[1].Value = "Soluble Mercury";
            dgvFinalMerge.Rows[24].Cells[1].Value = "Soluble Nickel";
            dgvFinalMerge.Rows[25].Cells[1].Value = "Soluble Selenium";
            dgvFinalMerge.Rows[26].Cells[1].Value = "Soluble Strontium";
            dgvFinalMerge.Rows[27].Cells[1].Value = "Soluble Tin";
            dgvFinalMerge.Rows[28].Cells[1].Value = "Soluble Organic Tin^";
            dgvFinalMerge.Rows[29].Cells[1].Value = "Soluble Zinc";

            dgvFinalMerge.Rows[32].Cells[1].Value = "Methyl tin";
            dgvFinalMerge.Rows[33].Cells[1].Value = "Di-n-propyl tin";
            dgvFinalMerge.Rows[34].Cells[1].Value = "Butyl tin";
            dgvFinalMerge.Rows[35].Cells[1].Value = "Dibutyl tin";
            dgvFinalMerge.Rows[36].Cells[1].Value = "Tributyl tin";
            dgvFinalMerge.Rows[37].Cells[1].Value = "n-Octyl tin";
            dgvFinalMerge.Rows[38].Cells[1].Value = "Tetrabutyl tin";
            dgvFinalMerge.Rows[39].Cells[1].Value = "Diphenyl tin";
            dgvFinalMerge.Rows[40].Cells[1].Value = "Di-n-octyl tin";
            dgvFinalMerge.Rows[41].Cells[1].Value = "Triphenyl tin";

            dgvFinalMerge.Rows[9].Cells[2].Value = "(mg)";
            dgvFinalMerge.Rows[10].Cells[2].Value = "(Al)";
            dgvFinalMerge.Rows[11].Cells[2].Value = "(Sb)";
            dgvFinalMerge.Rows[12].Cells[2].Value = "(As)";
            dgvFinalMerge.Rows[13].Cells[2].Value = "(Ba)";
            dgvFinalMerge.Rows[14].Cells[2].Value = "(B)";
            dgvFinalMerge.Rows[15].Cells[2].Value = "(Cd)";
            dgvFinalMerge.Rows[16].Cells[2].Value = "(Cr)";
            dgvFinalMerge.Rows[17].Cells[2].Value = "(Cr (III))";
            dgvFinalMerge.Rows[18].Cells[2].Value = "(Cr (VI))";
            dgvFinalMerge.Rows[19].Cells[2].Value = "(Co)";
            dgvFinalMerge.Rows[20].Cells[2].Value = "(Cu)";
            dgvFinalMerge.Rows[21].Cells[2].Value = "(Pb)";
            dgvFinalMerge.Rows[22].Cells[2].Value = "(Mn)";
            dgvFinalMerge.Rows[23].Cells[2].Value = "(Hg)";
            dgvFinalMerge.Rows[24].Cells[2].Value = "(Ni)";
            dgvFinalMerge.Rows[25].Cells[2].Value = "(Se)";
            dgvFinalMerge.Rows[26].Cells[2].Value = "(Sr)";
            dgvFinalMerge.Rows[27].Cells[2].Value = "(Sn)";
            dgvFinalMerge.Rows[28].Cells[2].Value = "--";
            dgvFinalMerge.Rows[29].Cells[2].Value = "(Zn)";

            dgvFinalMerge.Rows[32].Cells[2].Value = "(MeT)";
            dgvFinalMerge.Rows[33].Cells[2].Value = "(DProT)";
            dgvFinalMerge.Rows[34].Cells[2].Value = "(BuT)";
            dgvFinalMerge.Rows[35].Cells[2].Value = "(DBT)";
            dgvFinalMerge.Rows[36].Cells[2].Value = "(TBT)";
            dgvFinalMerge.Rows[37].Cells[2].Value = "(MOT)";
            dgvFinalMerge.Rows[38].Cells[2].Value = "(TeBT)";
            dgvFinalMerge.Rows[39].Cells[2].Value = "(DPhT)";
            dgvFinalMerge.Rows[40].Cells[2].Value = "(DOT)";
            dgvFinalMerge.Rows[41].Cells[2].Value = "(TPhT)";

            string SourceValue, TargetValue, PreviousValue;
            int JobNoChk, JobNoRow, TargetRowNo, TargetColNo, SamResultchk, ColNoChk, SampleCnt, SaveSampleCnt, SaveCurRow, i, j;
            int[] nCol = new int[100];
            int[] wCol = new int[100];
            string[] nColValue = new string[100];

            JobNoChk = 0;
            JobNoRow = 0;
            TargetRowNo = 0;
            TargetColNo = 0;
            SamResultchk = 0;
            ColNoChk = 2;
            SampleCnt = 0;
            SaveSampleCnt = 0;
            SaveCurRow = 0;
            i = 0;
            j = 0;

            SourceValue = "";
            TargetValue = "";
            PreviousValue = "";

            foreach (DataGridViewRow row1 in dgvResults.Rows)
            {
                if (row1.Cells[0].Value != null)
                    SourceValue = (string)row1.Cells[0].Value;
                else
                    SourceValue = "";

                // SGS File No.
                if (SourceValue == "SGS File No.")
                {
                    ColNoChk = ColNoChk + 1;

                    TargetRowNo = 1;
                    TargetColNo = ColNoChk;

                    JobNoChk = 1;
                    JobNoRow = row1.Index + 2;
                }

                // Style no./Item no.
                if (SourceValue == "Style no./Item no.")
                {

                    ColNoChk = ColNoChk + 1;

                    TargetRowNo = 2;
                    TargetColNo = ColNoChk;

                    JobNoChk = 1;
                    JobNoRow = row1.Index + 2;
                }

                // Issued Date
                if (SourceValue.ToString().Contains("Issued Date"))
                {

                    ColNoChk = ColNoChk + 1;

                    TargetRowNo = 3;
                    TargetColNo = ColNoChk;

                    JobNoChk = 1;
                    JobNoRow = row1.Index + 2;

                    TargetValue = SourceValue.Substring(SourceValue.IndexOf("20"), 12);
                }

                // Aurora Sample Description
                if (SourceValue == "Sample Description")
                {

                    ColNoChk = ColNoChk + 1;

                    TargetRowNo = 4;
                    TargetColNo = ColNoChk;

                    JobNoChk = 1;
                    JobNoRow = row1.Index + 2;
                }

                //Permissible Limit (mg/kg)
                if (SourceValue == "Permissible Limit (mg/kg)")
                {
                    SamResultchk = 1;
                    SampleCnt = 0;
                }
                else if (SamResultchk == 1 & SourceValue != "Mass of trace amount")
                {
                    SampleCnt = SampleCnt + 1;
                    ColNoChk = ColNoChk + SampleCnt;

                    nCol[SampleCnt] = ColNoChk;
                    nColValue[SampleCnt] = SourceValue;

                    for (i = 1; i <= nCol.LongLength - 1; i++)
                    {
                        if (nColValue[i] == SourceValue)
                        {
                            wCol[SampleCnt] = nCol[i];
                        }
                    }

                    TargetRowNo = 7;
                    TargetColNo = ColNoChk;

                    JobNoChk = 1;
                    JobNoRow = row1.Index;
                }
                else if (SamResultchk == 1 & SourceValue == "Mass of trace amount")
                {
                    //for (i=0 ; i <= SampleCnt ; i++)
                    //{
                    //    nCol[i] = i;
                    //}

                    SamResultchk = 0;
                    SaveSampleCnt = SampleCnt;
                    SampleCnt = 0;
                }
                //(mg)
                else if (SourceValue == "(mg)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = 9;

                }
                else if (SourceValue == "(Al)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Sb)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(As)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Ba)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(B)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Cd)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Cr)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Cr (III))")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Cr (VI))")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Co)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Cu)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Pb)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Mn)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Hg)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Ni)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Se)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Sr)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Sn)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "Soluble Organic Tin" || SourceValue == "Soluble Organic Tin^")
                {
                    //ExceptChk = 1;
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    //TargetRowNo = TargetRowNo + 1 ;
                    PreviousValue = SourceValue;
                }
                else if (SourceValue == "--" & (PreviousValue == "Soluble Organic Tin" || PreviousValue == "Soluble Organic Tin^"))
                {
                    //ExceptChk = 1;
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    PreviousValue = "";
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(Zn)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                //Soluble Organic Tin Result(s) (mg/kg)
                else if (SourceValue == "Soluble Organic Tin Result(s) (mg/kg)")
                {
                    SamResultchk = 2;
                    SampleCnt = 0;
                    ColNoChk = 2;
                    j = 0;
                }
                else if (SamResultchk == 2 & SourceValue != "Methyl tin")
                {
                    SampleCnt = SampleCnt + 1;
                    for (i = 1; i <= nCol.LongLength - 1; i++)
                    {
                        if (nColValue[i] == SourceValue)
                        {
                            j = j + 1;
                            wCol[j] = nCol[i];
                        }
                    }
                    //ColNoChk = ColNoChk + SampleCnt;

                    //TargetRowNo = TargetRowNo + 1;
                    //TargetColNo = ColNoChk;

                    JobNoChk = 1;
                    JobNoRow = 0;
                }
                else if (SamResultchk == 2 & SourceValue == "Methyl tin")
                {
                    SamResultchk = 0;
                    SaveSampleCnt = SampleCnt - 1;
                    SampleCnt = 0;
                }
                else if (SourceValue == "(MeT)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 3;
                }
                else if (SourceValue == "(DProT)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(BuT)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(DBT)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(TBT)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(MOT)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(TeBT)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(DPhT)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(DOT)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SourceValue == "(TPhT)")
                {
                    SaveCurRow = row1.Index;
                    SamResultchk = SaveCurRow;
                    TargetRowNo = TargetRowNo + 1;
                }
                else if (SamResultchk == SaveCurRow & SaveSampleCnt != SampleCnt)
                {
                    //if (ExceptChk == 1)
                    //{
                    //    SampleCnt = -1;
                    //    ExceptChk = 0;
                    //}
                    //else
                    //    SampleCnt = 0;
                    SampleCnt = SampleCnt + 1;
                    ColNoChk = ColNoChk + SampleCnt;

                    //TargetRowNo = 9;
                    if (SampleCnt > 0)
                        TargetColNo = wCol[SampleCnt];
                    else
                        TargetColNo = ColNoChk;

                    JobNoChk = 1;
                    JobNoRow = row1.Index;
                }
                else if (SamResultchk == SaveCurRow & SaveSampleCnt == SampleCnt)
                {
                    SamResultchk = 0;
                    SampleCnt = 0;

                }

                ////((Al))
                //if (SourceValue == "(Al)")
                //{
                //    SaveCurRow = row1.Index;
                //    SamResultchk = SaveCurRow;
                //}
                //else if (SamResultchk == SaveCurRow  & SaveSampleCnt != SampleCnt)
                //{
                //    SampleCnt = SampleCnt + 1;
                //    ColNoChk = ColNoChk + SampleCnt;

                //    TargetRowNo = 10;
                //    TargetColNo = ColNoChk;

                //    JobNoChk = 1;
                //    JobNoRow = row1.Index;
                //}
                //else if (SamResultchk == SaveCurRow  & SaveSampleCnt == SampleCnt)
                //{
                //    SamResultchk = 0;
                //    SampleCnt = 0;
                //}

                ////(Sb)
                //if (SourceValue == "(Sb)")
                //{
                //    SaveCurRow = row1.Index;
                //    SamResultchk = SaveCurRow;
                //}
                //else if (SamResultchk == SaveCurRow  & SaveSampleCnt != SampleCnt)
                //{
                //    SampleCnt = SampleCnt + 1;
                //    ColNoChk = ColNoChk + SampleCnt;

                //    TargetRowNo = 11;
                //    TargetColNo = ColNoChk;

                //    JobNoChk = 1;
                //    JobNoRow = row1.Index;
                //}
                //else if (SamResultchk == SaveCurRow  & SaveSampleCnt == SampleCnt)
                //{
                //    SamResultchk = 0;
                //    SampleCnt = 0;
                //}


                ////(As)
                //if (SourceValue == "(As)")
                //{
                //    SaveCurRow = row1.Index;
                //    SamResultchk = SaveCurRow;
                //}
                //else if (SamResultchk == SaveCurRow & SaveSampleCnt != SampleCnt)
                //{
                //    SampleCnt = SampleCnt + 1;
                //    ColNoChk = ColNoChk + SampleCnt;

                //    TargetRowNo = 12;
                //    TargetColNo = ColNoChk;

                //    JobNoChk = 1;
                //    JobNoRow = row1.Index;
                //}
                //else if (SamResultchk == SaveCurRow & SaveSampleCnt == SampleCnt)
                //{
                //    SamResultchk = 0;
                //    SampleCnt = 0;
                //}


                ////(Ba)
                //if (SourceValue == "(Ba)")
                //{
                //    SaveCurRow = row1.Index;
                //    SamResultchk = SaveCurRow;
                //}
                //else if (SamResultchk == SaveCurRow & SaveSampleCnt != SampleCnt)
                //{
                //    SampleCnt = SampleCnt + 1;
                //    ColNoChk = ColNoChk + SampleCnt;

                //    TargetRowNo = 13;
                //    TargetColNo = ColNoChk;

                //    JobNoChk = 1;
                //    JobNoRow = row1.Index;
                //}
                //else if (SamResultchk == SaveCurRow & SaveSampleCnt == SampleCnt)
                //{
                //    SamResultchk = 0;
                //    SampleCnt = 0;
                //}
                // 각 항목별로 값 적용 
                if (row1.Index == JobNoRow & JobNoChk == 1)
                {
                    JobNoChk = 0;
                    JobNoRow = 0;
                    if (TargetRowNo == 3)
                    {
                        dgvFinalMerge.Rows[TargetRowNo].Cells[3].Value = TargetValue;
                        TargetValue = "";
                        ColNoChk = ColNoChk - 1;
                    }
                    else
                    {
                        if (PreviousValue != "Soluble Organic Tin^")
                        {
                            dgvFinalMerge.Rows[TargetRowNo].Cells[0].Value = TargetRowNo;
                            dgvFinalMerge.Rows[TargetRowNo].Cells[TargetColNo].Value = row1.Cells[0].Value;
                            if (SampleCnt < 1)
                                ColNoChk = ColNoChk - 1;
                            else
                                ColNoChk = ColNoChk - SampleCnt;
                        }
                    }
                }


            }
            // Add the data.
            //for (int r = 1; r <= dgvResults.Rows  ; r++)
            //{
            //    //dgvFinalMerge.Rows.Add();
            //    //for (int c = 0; c < num1_cols; c++)
            //    //{
            //    //    dgvFinalMerge.Rows[r - 1].Cells[c].Value = values[r - 1, c];
            //    //}
            //}
        }

        private void dgvValues_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Prepare the connection.
            //string connect_string =
            //    "Provider=Microsoft.ACE.OLEDB.12.0;" +
            //    "Data Source=Students.mdb;" +
            //    "Mode=Share Deny None";
            string connect_string =
                "Provider=SQLOLEDB.1;User ID=sa;pwd=krslimdb001;data source=krslimdb001;Persist Security Info=True;initial catalog=KRCTS01";
            Conn = new OleDbConnection(connect_string);

            // List the students.
            ListStudents();
        }

        private void cboStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected student.
            SLIM SLIM = cboStudents.SelectedItem as SLIM;

            // Get the rest of the student's address data.
            string address_query = "SELECT labcode, pro_job " +
                "FROM profjobuser WHERE pro_job = '" + SLIM.Pro_Job + "'";
            OleDbCommand address_cmd = new OleDbCommand(address_query, Conn);

            Conn.Open();
            using (OleDbDataReader reader = address_cmd.ExecuteReader())
            {
                // Get the first (and hopefully last) row.
                if (reader.Read())
                {
                    txtStreet.Text = (string)reader.GetValue(0);
                    txtCity.Text = (string)reader.GetValue(1);
                    txtState.Text = (string)reader.GetValue(0);
                    txtZip.Text = (string)reader.GetValue(1);
                }
            }

            // Get the student's test scores.
            lstScores.Items.Clear();
            string scores_query = "SELECT pro_job, labcode " +
                "FROM profjob_cuid WHERE Pro_Job= '" + SLIM.Pro_Job + "' " +
                "ORDER BY Pro_job";
            OleDbCommand scores_cmd = new OleDbCommand(scores_query, Conn);

            //int test_total = 0;
            //int num_scores = 0;
            using (OleDbDataReader reader = scores_cmd.ExecuteReader())
            {
                // Get the next row.
                while (reader.Read())
                {
                    lstScores.Items.Add("Test " +
                        (string)reader.GetValue(0) + ": " +
                        (string)reader.GetValue(1));
                    //test_total += (int)reader.GetValue(1);
                    //num_scores++;
                }
            }
            Conn.Close();

            // Display the calculated average.
            //if (num_scores == 0) txtAverage.Text = "0";
            //else txtAverage.Text = (test_total / num_scores).ToString("0.0");
        }

        private void button9_Click(object sender, EventArgs e)
        {


            dgvFinalMerge.RowCount = 43;
            dgvFinalMerge.ColumnCount = 100;

            dgvFinalMerge.Rows[1].Cells[0].Value = "Job No";
            dgvFinalMerge.Rows[2].Cells[0].Value = "Item No";
            dgvFinalMerge.Rows[3].Cells[0].Value = "Issue Date";
            dgvFinalMerge.Rows[4].Cells[0].Value = "Aurora Description";
            dgvFinalMerge.Rows[5].Cells[0].Value = "Screen Or Not";

            dgvFinalMerge.Rows[7].Cells[0].Value = "Sample No";
            dgvFinalMerge.Rows[8].Cells[0].Value = "Report Sample Description";
            dgvFinalMerge.Rows[9].Cells[0].Value = "Part3 Results";
            dgvFinalMerge.Rows[32].Cells[0].Value = "Organotin Results";

            dgvFinalMerge.Rows[9].Cells[1].Value = "Mass of trace amount";
            dgvFinalMerge.Rows[10].Cells[1].Value = "Soluble Aluminium";
            dgvFinalMerge.Rows[11].Cells[1].Value = "Soluble Antimony";
            dgvFinalMerge.Rows[12].Cells[1].Value = "Soluble Arsenic";
            dgvFinalMerge.Rows[13].Cells[1].Value = "Soluble Barium";
            dgvFinalMerge.Rows[14].Cells[1].Value = "Soluble Boron";
            dgvFinalMerge.Rows[15].Cells[1].Value = "Soluble Cadmium";
            dgvFinalMerge.Rows[16].Cells[1].Value = "Soluble Chromium";
            dgvFinalMerge.Rows[17].Cells[1].Value = "Soluble Chromium (III) #";
            dgvFinalMerge.Rows[18].Cells[1].Value = "Soluble Chromium (VI) #";
            dgvFinalMerge.Rows[19].Cells[1].Value = "Soluble Cobalt";
            dgvFinalMerge.Rows[20].Cells[1].Value = "Soluble Copper";
            dgvFinalMerge.Rows[21].Cells[1].Value = "Soluble Lead";
            dgvFinalMerge.Rows[22].Cells[1].Value = "Soluble Manganese";
            dgvFinalMerge.Rows[23].Cells[1].Value = "Soluble Mercury";
            dgvFinalMerge.Rows[24].Cells[1].Value = "Soluble Nickel";
            dgvFinalMerge.Rows[25].Cells[1].Value = "Soluble Selenium";
            dgvFinalMerge.Rows[26].Cells[1].Value = "Soluble Strontium";
            dgvFinalMerge.Rows[27].Cells[1].Value = "Soluble Tin";
            dgvFinalMerge.Rows[28].Cells[1].Value = "Soluble Organic Tin^";
            dgvFinalMerge.Rows[29].Cells[1].Value = "Soluble Zinc";

            dgvFinalMerge.Rows[32].Cells[1].Value = "Methyl tin";
            dgvFinalMerge.Rows[33].Cells[1].Value = "Di-n-propyl tin";
            dgvFinalMerge.Rows[34].Cells[1].Value = "Butyl tin";
            dgvFinalMerge.Rows[35].Cells[1].Value = "Dibutyl tin";
            dgvFinalMerge.Rows[36].Cells[1].Value = "Tributyl tin";
            dgvFinalMerge.Rows[37].Cells[1].Value = "n-Octyl tin";
            dgvFinalMerge.Rows[38].Cells[1].Value = "Tetrabutyl tin";
            dgvFinalMerge.Rows[39].Cells[1].Value = "Diphenyl tin";
            dgvFinalMerge.Rows[40].Cells[1].Value = "Di-n-octyl tin";
            dgvFinalMerge.Rows[41].Cells[1].Value = "Triphenyl tin";

            dgvFinalMerge.Rows[9].Cells[2].Value = "(mg)";
            dgvFinalMerge.Rows[10].Cells[2].Value = "(Al)";
            dgvFinalMerge.Rows[11].Cells[2].Value = "(Sb)";
            dgvFinalMerge.Rows[12].Cells[2].Value = "(As)";
            dgvFinalMerge.Rows[13].Cells[2].Value = "(Ba)";
            dgvFinalMerge.Rows[14].Cells[2].Value = "(B)";
            dgvFinalMerge.Rows[15].Cells[2].Value = "(Cd)";
            dgvFinalMerge.Rows[16].Cells[2].Value = "(Cr)";
            dgvFinalMerge.Rows[17].Cells[2].Value = "(Cr (III))";
            dgvFinalMerge.Rows[18].Cells[2].Value = "(Cr (VI))";
            dgvFinalMerge.Rows[19].Cells[2].Value = "(Co)";
            dgvFinalMerge.Rows[20].Cells[2].Value = "(Cu)";
            dgvFinalMerge.Rows[21].Cells[2].Value = "(Pb)";
            dgvFinalMerge.Rows[22].Cells[2].Value = "(Mn)";
            dgvFinalMerge.Rows[23].Cells[2].Value = "(Hg)";
            dgvFinalMerge.Rows[24].Cells[2].Value = "(Ni)";
            dgvFinalMerge.Rows[25].Cells[2].Value = "(Se)";
            dgvFinalMerge.Rows[26].Cells[2].Value = "(Sr)";
            dgvFinalMerge.Rows[27].Cells[2].Value = "(Sn)";
            dgvFinalMerge.Rows[28].Cells[2].Value = "--";
            dgvFinalMerge.Rows[29].Cells[2].Value = "(Zn)";

            dgvFinalMerge.Rows[32].Cells[2].Value = "(MeT)";
            dgvFinalMerge.Rows[33].Cells[2].Value = "(DProT)";
            dgvFinalMerge.Rows[34].Cells[2].Value = "(BuT)";
            dgvFinalMerge.Rows[35].Cells[2].Value = "(DBT)";
            dgvFinalMerge.Rows[36].Cells[2].Value = "(TBT)";
            dgvFinalMerge.Rows[37].Cells[2].Value = "(MOT)";
            dgvFinalMerge.Rows[38].Cells[2].Value = "(TeBT)";
            dgvFinalMerge.Rows[39].Cells[2].Value = "(DPhT)";
            dgvFinalMerge.Rows[40].Cells[2].Value = "(DOT)";
            dgvFinalMerge.Rows[41].Cells[2].Value = "(TPhT)";

            dgvFinal.RowCount = 43;
            dgvFinal.ColumnCount = 100;

            dgvFinal.Rows[1].Cells[0].Value = "Job No";
            dgvFinal.Rows[2].Cells[0].Value = "Item No";
            dgvFinal.Rows[3].Cells[0].Value = "Issue Date";
            dgvFinal.Rows[4].Cells[0].Value = "Aurora Description";
            dgvFinal.Rows[5].Cells[0].Value = "Screen Or Not";

            dgvFinal.Rows[7].Cells[0].Value = "Sample No";
            dgvFinal.Rows[8].Cells[0].Value = "Report Sample Description";
            dgvFinal.Rows[9].Cells[0].Value = "Part3 Results";
            dgvFinal.Rows[32].Cells[0].Value = "Organotin Results";

            dgvFinal.Rows[9].Cells[1].Value = "Mass of trace amount";
            dgvFinal.Rows[10].Cells[1].Value = "Soluble Aluminium";
            dgvFinal.Rows[11].Cells[1].Value = "Soluble Antimony";
            dgvFinal.Rows[12].Cells[1].Value = "Soluble Arsenic";
            dgvFinal.Rows[13].Cells[1].Value = "Soluble Barium";
            dgvFinal.Rows[14].Cells[1].Value = "Soluble Boron";
            dgvFinal.Rows[15].Cells[1].Value = "Soluble Cadmium";
            dgvFinal.Rows[16].Cells[1].Value = "Soluble Chromium";
            dgvFinal.Rows[17].Cells[1].Value = "Soluble Chromium (III) #";
            dgvFinal.Rows[18].Cells[1].Value = "Soluble Chromium (VI) #";
            dgvFinal.Rows[19].Cells[1].Value = "Soluble Cobalt";
            dgvFinal.Rows[20].Cells[1].Value = "Soluble Copper";
            dgvFinal.Rows[21].Cells[1].Value = "Soluble Lead";
            dgvFinal.Rows[22].Cells[1].Value = "Soluble Manganese";
            dgvFinal.Rows[23].Cells[1].Value = "Soluble Mercury";
            dgvFinal.Rows[24].Cells[1].Value = "Soluble Nickel";
            dgvFinal.Rows[25].Cells[1].Value = "Soluble Selenium";
            dgvFinal.Rows[26].Cells[1].Value = "Soluble Strontium";
            dgvFinal.Rows[27].Cells[1].Value = "Soluble Tin";
            dgvFinal.Rows[28].Cells[1].Value = "Soluble Organic Tin^";
            dgvFinal.Rows[29].Cells[1].Value = "Soluble Zinc";

            dgvFinal.Rows[32].Cells[1].Value = "Methyl tin";
            dgvFinal.Rows[33].Cells[1].Value = "Di-n-propyl tin";
            dgvFinal.Rows[34].Cells[1].Value = "Butyl tin";
            dgvFinal.Rows[35].Cells[1].Value = "Dibutyl tin";
            dgvFinal.Rows[36].Cells[1].Value = "Tributyl tin";
            dgvFinal.Rows[37].Cells[1].Value = "n-Octyl tin";
            dgvFinal.Rows[38].Cells[1].Value = "Tetrabutyl tin";
            dgvFinal.Rows[39].Cells[1].Value = "Diphenyl tin";
            dgvFinal.Rows[40].Cells[1].Value = "Di-n-octyl tin";
            dgvFinal.Rows[41].Cells[1].Value = "Triphenyl tin";

            dgvFinal.Rows[9].Cells[2].Value = "(mg)";
            dgvFinal.Rows[10].Cells[2].Value = "(Al)";
            dgvFinal.Rows[11].Cells[2].Value = "(Sb)";
            dgvFinal.Rows[12].Cells[2].Value = "(As)";
            dgvFinal.Rows[13].Cells[2].Value = "(Ba)";
            dgvFinal.Rows[14].Cells[2].Value = "(B)";
            dgvFinal.Rows[15].Cells[2].Value = "(Cd)";
            dgvFinal.Rows[16].Cells[2].Value = "(Cr)";
            dgvFinal.Rows[17].Cells[2].Value = "(Cr (III))";
            dgvFinal.Rows[18].Cells[2].Value = "(Cr (VI))";
            dgvFinal.Rows[19].Cells[2].Value = "(Co)";
            dgvFinal.Rows[20].Cells[2].Value = "(Cu)";
            dgvFinal.Rows[21].Cells[2].Value = "(Pb)";
            dgvFinal.Rows[22].Cells[2].Value = "(Mn)";
            dgvFinal.Rows[23].Cells[2].Value = "(Hg)";
            dgvFinal.Rows[24].Cells[2].Value = "(Ni)";
            dgvFinal.Rows[25].Cells[2].Value = "(Se)";
            dgvFinal.Rows[26].Cells[2].Value = "(Sr)";
            dgvFinal.Rows[27].Cells[2].Value = "(Sn)";
            dgvFinal.Rows[28].Cells[2].Value = "--";
            dgvFinal.Rows[29].Cells[2].Value = "(Zn)";

            dgvFinal.Rows[32].Cells[2].Value = "(MeT)";
            dgvFinal.Rows[33].Cells[2].Value = "(DProT)";
            dgvFinal.Rows[34].Cells[2].Value = "(BuT)";
            dgvFinal.Rows[35].Cells[2].Value = "(DBT)";
            dgvFinal.Rows[36].Cells[2].Value = "(TBT)";
            dgvFinal.Rows[37].Cells[2].Value = "(MOT)";
            dgvFinal.Rows[38].Cells[2].Value = "(TeBT)";
            dgvFinal.Rows[39].Cells[2].Value = "(DPhT)";
            dgvFinal.Rows[40].Cells[2].Value = "(DOT)";
            dgvFinal.Rows[41].Cells[2].Value = "(TPhT)";

            dgvMergeFiles.RowCount = 50;
            foreach (DataGridViewRow row1 in dgvValues.Rows)
            {
                if (row1.Index >= 0 & row1.Index <= 5)
                {
                    dgvMergeFiles.Rows.Add(1);
                    foreach (DataGridViewColumn col1 in dgvValues.Columns)
                    {
                        if (col1.Index >= 0 & col1.Index <= 3)
                        {
                            dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].Value = dgvValues.Rows[row1.Index].Cells[col1.Index].Value;
                            //if (dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].ToString().Contains("Part") & row1.Index == 5)
                            //{
                            //    dgvMergeFiles.Rows[row1.Index].Cells[4].Value = "Report No";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[5].Value = "Issued Date";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[6].Value = "Report Sample Description";
                            //}
                        }
                    }
                }
            }

            dgvMergeFiles.Rows[5].Cells[4].Value = "Report No";
            dgvMergeFiles.Rows[5].Cells[5].Value = "Issued Date";
            dgvMergeFiles.Rows[5].Cells[6].Value = "Report Sample Description";

            string fileName = OpenFile();
            //string fileName = "E:\\T42\\WORK\\request\\오로라\\테스트\\복사본 KI2015019.xls";
            string plainText = string.Empty;
            string ext = System.IO.Path.GetExtension(fileName).ToUpper();
            string directoryPath = string.Empty;




            if (ext.Equals(".DOCX") || ext.Equals(".DOC"))
            {

                //Create word document

                Document document = new Document();




                //load a document

                document.LoadFromFile(@fileName);




                plainText = document.GetText();

                this.textBox3.Text = plainText;

            }

            else if (ext.Equals(".XLSX") || ext.Equals(".XLS"))
            {

                //Create Excel workbook

                Workbook workbook = new Workbook();




                //load a workbook

                workbook.LoadFromFile(@fileName);

                directoryPath = "c:\\temp\\";


                for (int i = 0; i < workbook.Worksheets.Count; i++)
                {

                    string tmpfilename = "tempSheet" + i.ToString() + ".txt";

                    Worksheet sheet = workbook.Worksheets[i];




                    if (!sheet.IsEmpty)
                    {

                        if (System.IO.File.Exists(directoryPath + tmpfilename))
                        {

                            System.IO.File.Delete(directoryPath + tmpfilename);

                        }


                        sheet.SaveToFile(directoryPath + tmpfilename, ", ", Encoding.UTF8);

                        plainText += "--[" + sheet.Name + "]--\r\n";

                        plainText += System.IO.File.ReadAllText(directoryPath + tmpfilename);

                        plainText += "\r\n";

                    }

                }
                this.textBox2.Text = plainText;
            }


        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (this.Width - textBox3.Location.X > 0)
                textBox3.Width = this.Width - textBox3.Location.X + 2;

            if (this.Width - dgvFinal.Width > 0)
                dgvFinal.Width = this.Width - dgvFinal.Location.X + 2;

            dgvFinalMerge.RowCount = 43;
            dgvFinalMerge.ColumnCount = 100;

            dgvFinalMerge.Rows[1].Cells[0].Value = "Job No";
            dgvFinalMerge.Rows[2].Cells[0].Value = "Item No";
            dgvFinalMerge.Rows[3].Cells[0].Value = "Issue Date";
            dgvFinalMerge.Rows[4].Cells[0].Value = "Aurora Description";
            dgvFinalMerge.Rows[5].Cells[0].Value = "Screen Or Not";

            dgvFinalMerge.Rows[6].Cells[0].Value = "PartName";
            dgvFinalMerge.Rows[7].Cells[0].Value = "Materials";

            dgvFinalMerge.Rows[8].Cells[0].Value = "Sample No";
            dgvFinalMerge.Rows[9].Cells[0].Value = "Report Sample Description";
            dgvFinalMerge.Rows[10].Cells[0].Value = "Part3 Results";
            dgvFinalMerge.Rows[33].Cells[0].Value = "Organotin Results";

            dgvFinalMerge.Rows[10].Cells[1].Value = "Mass of trace amount";
            dgvFinalMerge.Rows[11].Cells[1].Value = "Soluble Aluminium";
            dgvFinalMerge.Rows[12].Cells[1].Value = "Soluble Antimony";
            dgvFinalMerge.Rows[13].Cells[1].Value = "Soluble Arsenic";
            dgvFinalMerge.Rows[14].Cells[1].Value = "Soluble Barium";
            dgvFinalMerge.Rows[15].Cells[1].Value = "Soluble Boron";
            dgvFinalMerge.Rows[16].Cells[1].Value = "Soluble Cadmium";
            dgvFinalMerge.Rows[17].Cells[1].Value = "Soluble Chromium";
            dgvFinalMerge.Rows[18].Cells[1].Value = "Soluble Chromium (III) #";
            dgvFinalMerge.Rows[19].Cells[1].Value = "Soluble Chromium (VI) #";
            dgvFinalMerge.Rows[20].Cells[1].Value = "Soluble Cobalt";
            dgvFinalMerge.Rows[21].Cells[1].Value = "Soluble Copper";
            dgvFinalMerge.Rows[22].Cells[1].Value = "Soluble Lead";
            dgvFinalMerge.Rows[23].Cells[1].Value = "Soluble Manganese";
            dgvFinalMerge.Rows[24].Cells[1].Value = "Soluble Mercury";
            dgvFinalMerge.Rows[25].Cells[1].Value = "Soluble Nickel";
            dgvFinalMerge.Rows[26].Cells[1].Value = "Soluble Selenium";
            dgvFinalMerge.Rows[27].Cells[1].Value = "Soluble Strontium";
            dgvFinalMerge.Rows[28].Cells[1].Value = "Soluble Tin";
            dgvFinalMerge.Rows[29].Cells[1].Value = "Soluble Organic Tin^";
            dgvFinalMerge.Rows[30].Cells[1].Value = "Soluble Zinc";

            dgvFinalMerge.Rows[33].Cells[1].Value = "Methyl tin";
            dgvFinalMerge.Rows[34].Cells[1].Value = "Di-n-propyl tin";
            dgvFinalMerge.Rows[35].Cells[1].Value = "Butyl tin";
            dgvFinalMerge.Rows[36].Cells[1].Value = "Dibutyl tin";
            dgvFinalMerge.Rows[37].Cells[1].Value = "Tributyl tin";
            dgvFinalMerge.Rows[38].Cells[1].Value = "n-Octyl tin";
            dgvFinalMerge.Rows[39].Cells[1].Value = "Tetrabutyl tin";
            dgvFinalMerge.Rows[40].Cells[1].Value = "Diphenyl tin";
            dgvFinalMerge.Rows[41].Cells[1].Value = "Di-n-octyl tin";
            dgvFinalMerge.Rows[42].Cells[1].Value = "Triphenyl tin";

            dgvFinalMerge.Rows[10].Cells[2].Value = "(mg)";
            dgvFinalMerge.Rows[11].Cells[2].Value = "(Al)";
            dgvFinalMerge.Rows[12].Cells[2].Value = "(Sb)";
            dgvFinalMerge.Rows[13].Cells[2].Value = "(As)";
            dgvFinalMerge.Rows[14].Cells[2].Value = "(Ba)";
            dgvFinalMerge.Rows[15].Cells[2].Value = "(B)";
            dgvFinalMerge.Rows[16].Cells[2].Value = "(Cd)";
            dgvFinalMerge.Rows[17].Cells[2].Value = "(Cr)";
            dgvFinalMerge.Rows[18].Cells[2].Value = "(Cr (III))";
            dgvFinalMerge.Rows[19].Cells[2].Value = "(Cr (VI))";
            dgvFinalMerge.Rows[20].Cells[2].Value = "(Co)";
            dgvFinalMerge.Rows[21].Cells[2].Value = "(Cu)";
            dgvFinalMerge.Rows[22].Cells[2].Value = "(Pb)";
            dgvFinalMerge.Rows[23].Cells[2].Value = "(Mn)";
            dgvFinalMerge.Rows[24].Cells[2].Value = "(Hg)";
            dgvFinalMerge.Rows[25].Cells[2].Value = "(Ni)";
            dgvFinalMerge.Rows[26].Cells[2].Value = "(Se)";
            dgvFinalMerge.Rows[27].Cells[2].Value = "(Sr)";
            dgvFinalMerge.Rows[28].Cells[2].Value = "(Sn)";
            dgvFinalMerge.Rows[29].Cells[2].Value = "--";
            dgvFinalMerge.Rows[30].Cells[2].Value = "(Zn)";

            dgvFinalMerge.Rows[33].Cells[2].Value = "(MeT)";
            dgvFinalMerge.Rows[34].Cells[2].Value = "(DProT)";
            dgvFinalMerge.Rows[35].Cells[2].Value = "(BuT)";
            dgvFinalMerge.Rows[36].Cells[2].Value = "(DBT)";
            dgvFinalMerge.Rows[37].Cells[2].Value = "(TBT)";
            dgvFinalMerge.Rows[38].Cells[2].Value = "(MOT)";
            dgvFinalMerge.Rows[39].Cells[2].Value = "(TeBT)";
            dgvFinalMerge.Rows[40].Cells[2].Value = "(DPhT)";
            dgvFinalMerge.Rows[41].Cells[2].Value = "(DOT)";
            dgvFinalMerge.Rows[42].Cells[2].Value = "(TPhT)";

            dgvFinal.RowCount = 44;
            dgvFinal.ColumnCount = 100;

            dgvFinal.Rows[1].Cells[0].Value = "Job No";
            dgvFinal.Rows[2].Cells[0].Value = "Item No";
            dgvFinal.Rows[3].Cells[0].Value = "Issue Date";
            dgvFinal.Rows[4].Cells[0].Value = "Aurora Description";
            dgvFinal.Rows[5].Cells[0].Value = "Screen Or Not";

            dgvFinal.Rows[6].Cells[0].Value = "PartName";
            dgvFinal.Rows[7].Cells[0].Value = "Materials";

            dgvFinal.Rows[8].Cells[0].Value = "Sample No";
            dgvFinal.Rows[9].Cells[0].Value = "Report Sample Description";
            dgvFinal.Rows[10].Cells[0].Value = "Part3 Results";
            dgvFinal.Rows[33].Cells[0].Value = "Organotin Results";

            dgvFinal.Rows[10].Cells[1].Value = "Mass of trace amount";
            dgvFinal.Rows[11].Cells[1].Value = "Soluble Aluminium";
            dgvFinal.Rows[12].Cells[1].Value = "Soluble Antimony";
            dgvFinal.Rows[13].Cells[1].Value = "Soluble Arsenic";
            dgvFinal.Rows[14].Cells[1].Value = "Soluble Barium";
            dgvFinal.Rows[15].Cells[1].Value = "Soluble Boron";
            dgvFinal.Rows[16].Cells[1].Value = "Soluble Cadmium";
            dgvFinal.Rows[17].Cells[1].Value = "Soluble Chromium";
            dgvFinal.Rows[18].Cells[1].Value = "Soluble Chromium (III) #";
            dgvFinal.Rows[19].Cells[1].Value = "Soluble Chromium (VI) #";
            dgvFinal.Rows[20].Cells[1].Value = "Soluble Cobalt";
            dgvFinal.Rows[21].Cells[1].Value = "Soluble Copper";
            dgvFinal.Rows[22].Cells[1].Value = "Soluble Lead";
            dgvFinal.Rows[23].Cells[1].Value = "Soluble Manganese";
            dgvFinal.Rows[24].Cells[1].Value = "Soluble Mercury";
            dgvFinal.Rows[25].Cells[1].Value = "Soluble Nickel";
            dgvFinal.Rows[26].Cells[1].Value = "Soluble Selenium";
            dgvFinal.Rows[27].Cells[1].Value = "Soluble Strontium";
            dgvFinal.Rows[28].Cells[1].Value = "Soluble Tin";
            dgvFinal.Rows[29].Cells[1].Value = "Soluble Organic Tin^";
            dgvFinal.Rows[30].Cells[1].Value = "Soluble Zinc";

            dgvFinal.Rows[33].Cells[1].Value = "Methyl tin";
            dgvFinal.Rows[34].Cells[1].Value = "Di-n-propyl tin";
            dgvFinal.Rows[35].Cells[1].Value = "Butyl tin";
            dgvFinal.Rows[36].Cells[1].Value = "Dibutyl tin";
            dgvFinal.Rows[37].Cells[1].Value = "Tributyl tin";
            dgvFinal.Rows[38].Cells[1].Value = "n-Octyl tin";
            dgvFinal.Rows[39].Cells[1].Value = "Tetrabutyl tin";
            dgvFinal.Rows[40].Cells[1].Value = "Diphenyl tin";
            dgvFinal.Rows[41].Cells[1].Value = "Di-n-octyl tin";
            dgvFinal.Rows[42].Cells[1].Value = "Triphenyl tin";

            dgvFinal.Rows[10].Cells[2].Value = "(mg)";
            dgvFinal.Rows[11].Cells[2].Value = "(Al)";
            dgvFinal.Rows[12].Cells[2].Value = "(Sb)";
            dgvFinal.Rows[13].Cells[2].Value = "(As)";
            dgvFinal.Rows[14].Cells[2].Value = "(Ba)";
            dgvFinal.Rows[15].Cells[2].Value = "(B)";
            dgvFinal.Rows[16].Cells[2].Value = "(Cd)";
            dgvFinal.Rows[17].Cells[2].Value = "(Cr)";
            dgvFinal.Rows[18].Cells[2].Value = "(Cr (III))";
            dgvFinal.Rows[19].Cells[2].Value = "(Cr (VI))";
            dgvFinal.Rows[20].Cells[2].Value = "(Co)";
            dgvFinal.Rows[21].Cells[2].Value = "(Cu)";
            dgvFinal.Rows[22].Cells[2].Value = "(Pb)";
            dgvFinal.Rows[23].Cells[2].Value = "(Mn)";
            dgvFinal.Rows[24].Cells[2].Value = "(Hg)";
            dgvFinal.Rows[25].Cells[2].Value = "(Ni)";
            dgvFinal.Rows[26].Cells[2].Value = "(Se)";
            dgvFinal.Rows[27].Cells[2].Value = "(Sr)";
            dgvFinal.Rows[28].Cells[2].Value = "(Sn)";
            dgvFinal.Rows[29].Cells[2].Value = "--";
            dgvFinal.Rows[30].Cells[2].Value = "(Zn)";

            dgvFinal.Rows[33].Cells[2].Value = "(MeT)";
            dgvFinal.Rows[34].Cells[2].Value = "(DProT)";
            dgvFinal.Rows[35].Cells[2].Value = "(BuT)";
            dgvFinal.Rows[36].Cells[2].Value = "(DBT)";
            dgvFinal.Rows[37].Cells[2].Value = "(TBT)";
            dgvFinal.Rows[38].Cells[2].Value = "(MOT)";
            dgvFinal.Rows[39].Cells[2].Value = "(TeBT)";
            dgvFinal.Rows[40].Cells[2].Value = "(DPhT)";
            dgvFinal.Rows[41].Cells[2].Value = "(DOT)";
            dgvFinal.Rows[42].Cells[2].Value = "(TPhT)";

            dgvMergeFiles.RowCount = 50;
            foreach (DataGridViewRow row1 in dgvValues.Rows)
            {
                if (row1.Index >= 0 & row1.Index <= 5)
                {
                    dgvMergeFiles.Rows.Add(1);
                    foreach (DataGridViewColumn col1 in dgvValues.Columns)
                    {
                        if (col1.Index >= 0 & col1.Index <= 3)
                        {
                            dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].Value = dgvValues.Rows[row1.Index].Cells[col1.Index].Value;
                            //if (dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].ToString().Contains("Part") & row1.Index == 5)
                            //{
                            //    dgvMergeFiles.Rows[row1.Index].Cells[4].Value = "Report No";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[5].Value = "Issued Date";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[6].Value = "Report Sample Description";
                            //}
                        }
                    }
                }
            }

            //dgvMergeFiles.Rows[5].Cells[4].Value = "Report No";
            //dgvMergeFiles.Rows[5].Cells[5].Value = "Issued Date";
            //dgvMergeFiles.Rows[5].Cells[6].Value = "Report Sample Description";

            string fileName = OpenFile();
            //string fileName = "E:\\T42\\WORK\\request\\오로라\\테스트\\복사본 KI2015019.xls";
            string plainText = string.Empty;
            string ext = System.IO.Path.GetExtension(fileName).ToUpper();
            string directoryPath = string.Empty;




            if (ext.Equals(".DOCX") || ext.Equals(".DOC"))
            {

                //Create word document

                Document document = new Document();




                //load a document

                document.LoadFromFile(@fileName);




                plainText = document.GetText();

                this.textBox3.Text = plainText;

            }

            else if (ext.Equals(".XLSX") || ext.Equals(".XLS"))
            {

                //Create Excel workbook

                Workbook workbook = new Workbook();




                //load a workbook

                workbook.LoadFromFile(@fileName);

                directoryPath = "c:\\temp\\";


                for (int i = 0; i < workbook.Worksheets.Count; i++)
                {

                    string tmpfilename = "tempSheet" + i.ToString() + ".txt";

                    Worksheet sheet = workbook.Worksheets[i];




                    if (!sheet.IsEmpty)
                    {

                        if (System.IO.File.Exists(directoryPath + tmpfilename))
                        {

                            System.IO.File.Delete(directoryPath + tmpfilename);

                        }


                        sheet.SaveToFile(directoryPath + tmpfilename, ", ", Encoding.UTF8);

                        plainText += "--[" + sheet.Name + "]--\r\n";

                        plainText += System.IO.File.ReadAllText(directoryPath + tmpfilename);

                        plainText += "\r\n";

                    }

                }
                this.textBox2.Text = plainText;
            }

        }

        private void button11_Click(object sender, EventArgs e)
        {
            // Get the file's text.
            string whole_file = textBox2.Text;

            // Split into lines.
            //\r\n
            whole_file = whole_file.Replace('\n', '\r');
            whole_file = whole_file.Replace('"', ' ');
            string[] lines = whole_file.Split(new char[] { '\r' },
                StringSplitOptions.RemoveEmptyEntries);

            // See how many rows and columns there are.
            int num_rows = lines.Length;
            //MessageBox.Show(lines[num_rows - 1]);
            //int num_cols = lines[num_rows - 1].Split(',').Length; //일반
            int num_cols = 10;  //오로라 전용

            // Allocate the data array.
            string[,] values = new string[num_rows, num_cols];

            // Load the array.
            for (int r = 1; r < num_rows; r++)
            {
                string[] line_r = lines[r].Split(',');
                int row_cols = lines[r].Split(',').Length;

                for (int c = 0; c < row_cols; c++)
                {
                    if (line_r[c] == null)
                        values[r, c] = "";
                    else
                        values[r, c] = line_r[c];
                }
            }

            int num1_rows = values.GetUpperBound(0) + 1;
            int num1_cols = values.GetUpperBound(1) + 1;

            // Display the data to show we have it.

            // Make column headers.
            // For this example, we assume the first row
            // contains the column names.

            // Clear previous results.
            dgvValues.Rows.Clear();

            for (int c = 0; c < num1_cols; c++)
                dgvValues.Columns.Add(values[0, c], values[0, c]);

            // Add the data.
            for (int r = 1; r < num1_rows; r++)
            {
                dgvValues.Rows.Add();
                for (int c = 0; c < num1_cols; c++)
                {
                    dgvValues.Rows[r - 1].Cells[c].Value = values[r, c];
                }
            }

            //// Make the columns autosize.
            //foreach (DataGridViewColumn col in dgvValues.Columns)
            //    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

        }

        private void button12_Click(object sender, EventArgs e)
        {


            Get_File_List();
            MessageBox.Show("Keep Going");
            Find_Report();
        }

        private void Result_Merge1()
        {
            string plainText = string.Empty;
            int wRow = 1;
            int RowChk, RowNo;
            int SampleTot;

            string sRptName;
            string sIssuedDate;
            string wItem_Number;
            string wRpt = "";

            string wPartName = "";
            string wMaterialName = "";
            string wReportNo = "";
            string wIssuedDate = "";
            string wSamDesc = "";
            string wStyleNo = "";
            string wFileNo = "";
            string wMDL1 = "";
            string wMDL2 = "";

            int ij = 0;

            sIssuedDate = "";
            sRptName = "";
            SampleTot = 0;
            RowNo = 0;
            RowChk = 0;
            wItem_Number = "";

            dgvFinal.Rows.Clear();
            //dgvFinal.RowCount;  
            dgvFinalMerge.Rows.Clear();
            Initialize_Grid();

            progressBar1.Maximum = dgvMergeFiles.Rows.Count;
            foreach (DataGridViewRow row2 in dgvMergeFiles.Rows)
            {
                progressBar1.Value = row2.Index;

                wPartName = (string)row2.Cells[4].Value;
                wMaterialName = (string)row2.Cells[0].Value;
                wReportNo = (string)row2.Cells[1].Value;

                string wRptFileName = (string)row2.Cells[11].Value;

                if (wRptFileName != null)
                    wRptFileName = wRptFileName.Trim();
                else
                    wRptFileName = "";

                if (wRptFileName == "" || wRptFileName == null)
                {
                    sRptName = wRptFileName;
                }

                else if (wRptFileName.ToLower() == "report file")
                {
                    sRptName = "report file"; //report file이라는 이름을 sRptName에 표시해 두고, next line을 읽는다.
                }

                else if (wRptFileName.ToLower() == "")
                {
                    sRptName = wRptFileName;
                }
                else if (wRptFileName == "Item Number")
                {
                    wItem_Number = (string)row2.Cells[12].Value;
                    //dgvFinal.Rows[0].Cells[3].Value = "";
                    //dgvFinal.Rows[0].Cells[3].Value = (string)row2.Cells[12].Value;
                }

                else if (sRptName == "report file")  //report file다음 라인에 있는 파일이름을 읽어서 필요한 자료를 가져온다.
                {


                    wRow = wRow + ij + 1;
                    ij = 0;

                    Document document = new Document();
                    string ext = System.IO.Path.GetExtension(wRptFileName).ToUpper();

                    if (ext.Equals(".DOCX") || ext.Equals(".DOC"))
                    {
                        document.LoadFromFile(@wRptFileName);

                        plainText = document.GetText();
                        this.textBox3.Text = plainText;

                        RowChk = RowChk + 1;
                        RowNo = RowChk + row2.Index;


                        //파일 내용을 분해하는 단계 시작------------------------
                        // Get the file's text.
                        string whole_file = textBox3.Text;

                        // Split into lines.
                        //\r\n
                        whole_file = whole_file.Replace('\n', '\r');
                        //whole_file = whole_file.Replace('"', ' ');
                        string[] lines = whole_file.Split(new char[] { '\r' },
                            StringSplitOptions.RemoveEmptyEntries);

                        // See how many rows and columns there are.
                        int num_rows = lines.Length;
                        int num_cols = 1;
                        //int num_cols = lines[num_rows - 1].Split(',').Length;

                        // Allocate the data array.
                        string[,] values = new string[num_rows, num_cols];

                        // Load the array.
                        for (int r = 0; r < num_rows; r++)
                        {
                            //string[] line_r = lines[r].Split(',');
                            string[] line_r = lines[r].Split(new char[] { '\r' },
                            StringSplitOptions.RemoveEmptyEntries);
                            for (int c = 0; c < num_cols; c++)
                            {
                                if (line_r[c] == null)
                                    values[r, c] = "";
                                else
                                    values[r, c] = line_r[c];
                            }
                        }

                        int num1_rows = values.GetUpperBound(0) + 1;
                        int num1_cols = values.GetUpperBound(1) + 1;

                        // Display the data to show we have it.

                        // Make column headers.
                        // For this example, we assume the first row
                        // contains the column names.

                        // Clear previous results.
                        dgvResults.Rows.Clear();

                        for (int c = 0; c < num1_cols; c++)
                            dgvResults.Columns.Add(values[0, c], values[0, c]);

                        // Add the data.
                        for (int r = 1; r <= num1_rows; r++)
                        {
                            dgvResults.Rows.Add();
                            for (int c = 0; c < num1_cols; c++)
                            {
                                dgvResults.Rows[r - 1].Cells[c].Value = values[r - 1, c];
                            }
                        }

                        //파일 내용을 분해하는 단계 끝------------------------

                        //분해된 내용을 정리하는 단계 시작 ----------------------------
                        string SourceValue, TargetValue, PreviousValue, AnalyteName;
                        string P4351;
                        string P4352;
                        int JobNoChk, JobNoRow, TargetRowNo, TargetColNo, SamResultchk, SampleCnt, SaveSampleCnt, SaveCurRow, i, j, ColNoChk;
                        int SampleTotCnt, JobNoRow2;
                        int[] nCol = new int[100];
                        int[] wCol = new int[100];
                        string[] nColValue = new string[100];
                        int P4351_Lead_Pb = 0;
                        int P4352_Lead_Pb = 0;
                        int P4351_sol = 0;
                        int P4352_sol = 0;
                        int ik = 0;


                        AnalyteName = "";

                        JobNoChk = 0;
                        JobNoRow = 0;
                        JobNoRow = 2;

                        TargetRowNo = 0;
                        TargetColNo = 0;
                        SamResultchk = 0;
                        ColNoChk = 2;
                        SampleCnt = 0;
                        SaveSampleCnt = 0;
                        SaveCurRow = 0;
                        SampleTotCnt = 0;
                        i = 0;
                        j = 0;

                        P4351 = "";
                        P4352 = "";

                        SourceValue = "";
                        TargetValue = "";
                        PreviousValue = "";

                        foreach (DataGridViewRow row3 in dgvResults.Rows)
                        {

                            if (row3.Cells[0].Value != null)
                                SourceValue = (string)row3.Cells[0].Value;
                            else
                                SourceValue = "";


                            dgvFinal.Rows[wRow].Cells[0].Value = wItem_Number;
                            dgvFinal.Rows[wRow].Cells[4].Value = wPartName;
                            dgvFinal.Rows[wRow].Cells[5].Value = wMaterialName;
                            dgvFinal.Rows[wRow].Cells[6].Value = wReportNo;

                            // SGS File No.
                            if (SourceValue == "SGS File No.")
                            {
                                TargetColNo = 7;
                                TargetRowNo = wRow;
                                JobNoRow = row3.Index + 2;
                                wFileNo = (string)dgvResults.Rows[row3.Index + 2].Cells[0].Value;
                            }

                            // Sample Description
                            if (SourceValue == "Sample Description")
                            {
                                TargetColNo = 2;
                                TargetRowNo = wRow;
                                JobNoRow = row3.Index + 2;
                                wSamDesc = (string)dgvResults.Rows[row3.Index + 2].Cells[0].Value;
                            }

                            // Style no./ Item no.
                            if (SourceValue.IndexOf("Style no.") > -1)
                            {
                                TargetColNo = 1;
                                TargetRowNo = wRow;
                                JobNoRow = row3.Index + 2;
                                wStyleNo = (string)dgvResults.Rows[row3.Index + 2].Cells[0].Value;
                            }


                            // Issued Date
                            if (SourceValue.IndexOf("Issued Date") > -1)
                            {
                                string[] line_cell = SourceValue.Split(new char[] { ':' },
                            StringSplitOptions.RemoveEmptyEntries);
                                wIssuedDate = line_cell[1].Replace(" ", "").Substring(0, 10);
                                dgvFinal.Rows[wRow].Cells[8].Value = line_cell[1].Replace(" ", "").Substring(0, 10);
                            }

                            if (SourceValue.IndexOf("ASTM F963-11, Clause 4.3.5.1") > -1)
                            {
                                P4351 = "4351";
                                P4352 = "";
                            }

                            if (SourceValue.IndexOf("ASTM F963-11, Clause 4.3.5.2") > -1)
                            {
                                P4351 = "";
                                P4352 = "4352";
                            }

                            //if (SourceValue == "Sample No.")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            if (SourceValue == "Method Detection Limit")
                            {
                                if (P4351 == "4351" && AnalyteName == "lead(pb)")
                                {
                                    TargetColNo = 9;
                                    TargetRowNo = wRow;
                                    JobNoRow = row3.Index + 1; //Method Detection Limit
                                    wMDL1 = (string)dgvResults.Rows[row3.Index + 1].Cells[0].Value;
                                }

                                if (P4352 == "4352" && AnalyteName == "lead(pb)")
                                {
                                    TargetColNo = 20;
                                    TargetRowNo = wRow;
                                    JobNoRow = row3.Index + 1;  //Method Detection Limit
                                    wMDL2 = (string)dgvResults.Rows[row3.Index + 1].Cells[0].Value;
                                }

                                //AnalyteName = SourceValue;
                            }

                            if (P4351_Lead_Pb == 1)
                            {
                                if (SourceValue == "Soluble Heavy Metals" || SourceValue == "Sample Description:")
                                {
                                    P4351_Lead_Pb = 0;
                                    AnalyteName = "";
                                    ij = ij - 1;
                                }
                                else if (ik == (3 * ij) + 3)
                                {
                                    ij = ij + 1;
                                }
                                else
                                {
                                    if (ik == (3 * ij) + 1)
                                    {
                                        TargetColNo = 3;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sample No.

                                    }
                                    else if (ik == (3 * ij) + 2)
                                    {
                                        TargetColNo = 10;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //결과.
                                    }

                                }
                                ik = ik + 1;

                            }

                            if (P4352_Lead_Pb == 1)
                            {
                                if (SourceValue == "Soluble Heavy Metals" || SourceValue == "Sample Description:")
                                {
                                    AnalyteName = "";
                                    P4352_Lead_Pb = 0;
                                    ij = ij - 1;
                                }
                                else if (ik == (3 * ij) + 3)
                                {
                                    ij = ij + 1;
                                }
                                else
                                {
                                    if (ik == (3 * ij) + 1)
                                    {
                                        TargetColNo = 3;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sample No.

                                    }
                                    else if (ik == (3 * ij) + 2)
                                    {
                                        TargetColNo = 21;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //결과.
                                    }

                                }
                                ik = ik + 1;

                            }

                            if (P4351_sol == 1)
                            {
                                if (SourceValue == "Sample Description:")
                                {
                                    P4351_sol = 0;
                                    AnalyteName = "";
                                    ij = ij - 1;
                                }
                                else if (ik == (11 * ij) + 11)
                                {
                                    ij = ij + 1;
                                }
                                else
                                {
                                    if (ik == (11 * ij) + 1)
                                    {
                                        TargetColNo = 3;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sample No.

                                    }
                                    else if (ik == (11 * ij) + 2)
                                    {
                                        TargetColNo = 11;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Pb
                                    }
                                    else if (ik == (11 * ij) + 3)
                                    {
                                        TargetColNo = 12;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sb
                                    }
                                    else if (ik == (11 * ij) + 4)
                                    {
                                        TargetColNo = 13;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //As
                                    }
                                    else if (ik == (11 * ij) + 5)
                                    {
                                        TargetColNo = 14;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Ba
                                    }
                                    else if (ik == (11 * ij) + 6)
                                    {
                                        TargetColNo = 15;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Cd
                                    }
                                    else if (ik == (11 * ij) + 7)
                                    {
                                        TargetColNo = 16;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Cr
                                    }
                                    else if (ik == (11 * ij) + 8)
                                    {
                                        TargetColNo = 17;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Hg
                                    }
                                    else if (ik == (11 * ij) + 9)
                                    {
                                        TargetColNo = 18;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Se
                                    }
                                    else if (ik == (11 * ij) + 10)
                                    {
                                        TargetColNo = 19;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Se
                                    }
                                }
                                ik = ik + 1;

                            }

                            if (P4352_sol == 1)
                            {
                                if (SourceValue == "Sample Description:")
                                {
                                    P4352_sol = 0;
                                    AnalyteName = "";
                                    ij = ij - 1;
                                }
                                else if (ik == (11 * ij) + 11)
                                {
                                    ij = ij + 1;
                                }
                                else
                                {
                                    if (ik == (11 * ij) + 1)
                                    {
                                        TargetColNo = 3;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sample No.

                                    }
                                    else if (ik == (11 * ij) + 2)
                                    {
                                        TargetColNo = 22;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Pb
                                    }
                                    else if (ik == (11 * ij) + 3)
                                    {
                                        TargetColNo = 23;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sb
                                    }
                                    else if (ik == (11 * ij) + 4)
                                    {
                                        TargetColNo = 24;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //As
                                    }
                                    else if (ik == (11 * ij) + 5)
                                    {
                                        TargetColNo = 25;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Ba
                                    }
                                    else if (ik == (11 * ij) + 6)
                                    {
                                        TargetColNo = 26;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Cd
                                    }
                                    else if (ik == (11 * ij) + 7)
                                    {
                                        TargetColNo = 27;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Cr
                                    }
                                    else if (ik == (11 * ij) + 8)
                                    {
                                        TargetColNo = 28;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Hg
                                    }
                                    else if (ik == (11 * ij) + 9)
                                    {
                                        TargetColNo = 29;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Se
                                    }
                                    else if (ik == (11 * ij) + 10)
                                    {
                                        TargetColNo = 30;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Se
                                    }

                                }
                                ik = ik + 1;

                            }


                            if (SourceValue.IndexOf("Total Result(s)") > -1)
                            {

                                if (P4351 == "4351" && AnalyteName == "lead(pb)")
                                {
                                    P4351_Lead_Pb = 1;
                                    P4352_Lead_Pb = 0;
                                    JobNoRow = 0;
                                    ij = 0;
                                    ik = 1;
                                }

                                if (P4351 == "4351" && AnalyteName == "solubleheavymetals")
                                {
                                    P4351_sol = 1;
                                    P4352_sol = 0;
                                    JobNoRow = 0;
                                    ij = 0;
                                    ik = 1;
                                }

                                if (P4352 == "4352" && AnalyteName == "lead(pb)")
                                {
                                    P4352_Lead_Pb = 1;
                                    P4351_Lead_Pb = 0;
                                    JobNoRow = 0;
                                    ij = 0;
                                    ik = 1;
                                }
                                if (P4352 == "4352" && AnalyteName == "solubleheavymetals")
                                {
                                    P4352_sol = 1;
                                    P4351_sol = 0;
                                    JobNoRow = 0;
                                    ij = 0;
                                    ik = 1;
                                }
                                //AnalyteName = SourceValue;
                            }

                            if (SourceValue.Replace(" ", "").ToLower() == "lead(pb)")
                            {
                                AnalyteName = SourceValue.Replace(" ", "").ToLower();
                            }

                            if (SourceValue.Replace(" ", "").ToLower() == "solubleheavymetals")
                            {
                                AnalyteName = SourceValue.Replace(" ", "").ToLower();
                            }

                            //if (SourceValue == "Pb")
                            //{
                            //    AnalyteName = SourceValue;

                            //    if (P4351 == "4351" && AnalyteName == "Pb")
                            //    {
                            //        TargetColNo = 8;
                            //        TargetRowNo = wRow;
                            //        JobNoRow = row3.Index + 1; //Method Detection Limit
                            //    }

                            //    if (P4352 == "4352" && AnalyteName == "Pb")
                            //    {
                            //        TargetColNo = 20;
                            //        TargetRowNo = wRow;
                            //        JobNoRow = row3.Index + 1;  //Method Detection Limit
                            //    }
                            //}

                            //if (SourceValue == "Sb")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "As")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "Ba")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "Cd")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "Cr")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "Hg")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "Se")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            if (JobNoRow == row3.Index)
                            {
                                dgvFinal.Rows[TargetRowNo].Cells[0].Value = wItem_Number;
                                dgvFinal.Rows[TargetRowNo].Cells[4].Value = wPartName;
                                dgvFinal.Rows[TargetRowNo].Cells[5].Value = wMaterialName;
                                dgvFinal.Rows[TargetRowNo].Cells[6].Value = wReportNo;
                                dgvFinal.Rows[TargetRowNo].Cells[8].Value = wIssuedDate;
                                dgvFinal.Rows[TargetRowNo].Cells[7].Value = wFileNo;
                                dgvFinal.Rows[TargetRowNo].Cells[2].Value = wSamDesc;
                                dgvFinal.Rows[TargetRowNo].Cells[1].Value = wStyleNo;
                                dgvFinal.Rows[TargetRowNo].Cells[9].Value = wMDL1;
                                dgvFinal.Rows[TargetRowNo].Cells[20].Value = wMDL2;

                                dgvFinal.Rows[TargetRowNo].Cells[TargetColNo].Value = SourceValue;
                                JobNoRow = 0;
                            }

                        }
                    }
                }

            }

            dgvFinal.Rows[0].Cells[0].Value = "";
            dgvFinal.Rows[0].Cells[1].Value = "";
            dgvFinal.Rows[0].Cells[2].Value = "";
            dgvFinal.Rows[0].Cells[3].Value = "";
            dgvFinal.Rows[0].Cells[4].Value = "";
            dgvFinal.Rows[0].Cells[5].Value = "";
            dgvFinal.Rows[0].Cells[6].Value = "";
            dgvFinal.Rows[0].Cells[7].Value = "";
            dgvFinal.Rows[0].Cells[8].Value = "";
            dgvFinal.Rows[0].Cells[9].Value = "";


        }

        private void Result_Merge11()
        {
            string plainText = string.Empty;
            int wRow = 1;
            int RowChk, RowNo;
            int SampleTot;

            string sRptName;
            string sIssuedDate;
            string wItem_Number;
            string wRpt = "";

            string wPartName = "";
            string wMaterialName = "";
            string wReportNo = "";
            string wIssuedDate = "";
            string wSamDesc = "";
            string wStyleNo = "";
            string wFileNo = "";
            string wMDL1 = "";
            string wMDL2 = "";

            int ij = 0;

            sIssuedDate = "";
            sRptName = "";
            SampleTot = 0;
            RowNo = 0;
            RowChk = 0;
            wItem_Number = "";

            dgvFinal.Rows.Clear();
            //dgvFinal.RowCount;  
            dgvFinalMerge.Rows.Clear();
            Initialize_Grid();

            progressBar1.Maximum = dgvMergeFiles.Rows.Count;
            foreach (DataGridViewRow row2 in dgvMergeFiles.Rows)
            {
                progressBar1.Value = row2.Index;

                wPartName = (string)row2.Cells[4].Value;
                wMaterialName = (string)row2.Cells[0].Value;
                wReportNo = (string)row2.Cells[1].Value;

                string wRptFileName = (string)row2.Cells[11].Value;

                if (wRptFileName != null)
                    wRptFileName = wRptFileName.Trim();
                else
                    wRptFileName = "";

                if (wRptFileName == "" || wRptFileName == null)
                {
                    sRptName = wRptFileName;
                }

                else if (wRptFileName.ToLower()  == "report file")
                {
                    sRptName = "report file"; //report file이라는 이름을 sRptName에 표시해 두고, next line을 읽는다.
                }

                else if (wRptFileName.ToLower() == "")
                {
                    sRptName = wRptFileName;
                }
                else if (wRptFileName == "Item Number")
                {
                    wItem_Number = (string)row2.Cells[12].Value;
                    //dgvFinal.Rows[0].Cells[3].Value = "";
                    //dgvFinal.Rows[0].Cells[3].Value = (string)row2.Cells[12].Value;
                }

                else if (wRptFileName.IndexOf(":") > -1)  //report file다음 라인에 있는 파일이름을 읽어서 필요한 자료를 가져온다.
                {


                    wRow = wRow + ij + 1;
                    ij = 0;

                    Document document = new Document();
                    string ext = System.IO.Path.GetExtension(wRptFileName).ToUpper();

                    if (ext.Equals(".DOCX") || ext.Equals(".DOC"))
                    {
                        document.LoadFromFile(@wRptFileName);

                        plainText = document.GetText();
                        this.textBox3.Text = plainText;

                        RowChk = RowChk + 1;
                        RowNo = RowChk + row2.Index;


                        //파일 내용을 분해하는 단계 시작------------------------
                        // Get the file's text.
                        string whole_file = textBox3.Text;

                        // Split into lines.
                        //\r\n
                        whole_file = whole_file.Replace('\n', '\r');
                        //whole_file = whole_file.Replace('"', ' ');
                        string[] lines = whole_file.Split(new char[] { '\r' },
                            StringSplitOptions.RemoveEmptyEntries);

                        // See how many rows and columns there are.
                        int num_rows = lines.Length;
                        int num_cols = 1;
                        //int num_cols = lines[num_rows - 1].Split(',').Length;

                        // Allocate the data array.
                        string[,] values = new string[num_rows, num_cols];

                        // Load the array.
                        for (int r = 0; r < num_rows; r++)
                        {
                            //string[] line_r = lines[r].Split(',');
                            string[] line_r = lines[r].Split(new char[] { '\r' },
                            StringSplitOptions.RemoveEmptyEntries);
                            for (int c = 0; c < num_cols; c++)
                            {
                                if (line_r[c] == null)
                                    values[r, c] = "";
                                else
                                    values[r, c] = line_r[c];
                            }
                        }

                        int num1_rows = values.GetUpperBound(0) + 1;
                        int num1_cols = values.GetUpperBound(1) + 1;

                        // Display the data to show we have it.

                        // Make column headers.
                        // For this example, we assume the first row
                        // contains the column names.

                        // Clear previous results.
                        dgvResults.Rows.Clear();

                        for (int c = 0; c < num1_cols; c++)
                            dgvResults.Columns.Add(values[0, c], values[0, c]);

                        // Add the data.
                        for (int r = 1; r <= num1_rows; r++)
                        {
                            dgvResults.Rows.Add();
                            for (int c = 0; c < num1_cols; c++)
                            {
                                dgvResults.Rows[r - 1].Cells[c].Value = values[r - 1, c];
                            }
                        }

                        //파일 내용을 분해하는 단계 끝------------------------

                        //분해된 내용을 정리하는 단계 시작 ----------------------------
                        string SourceValue, TargetValue, PreviousValue, AnalyteName;
                        string P4351 ;
                        string P4352 ;
                        int JobNoChk, JobNoRow, TargetRowNo, TargetColNo, SamResultchk, SampleCnt, SaveSampleCnt, SaveCurRow, i, j, ColNoChk;
                        int SampleTotCnt, JobNoRow2 ;
                        int[] nCol = new int[100];
                        int[] wCol = new int[100];
                        string[] nColValue = new string[100];
                        int P4351_Lead_Pb = 0;
                        int P4352_Lead_Pb = 0;
                        int P4351_sol = 0;
                        int P4352_sol = 0;
                        int ik = 0;


                        AnalyteName = "";

                        JobNoChk = 0;
                        JobNoRow = 0;
                        JobNoRow = 2;

                        TargetRowNo = 0;
                        TargetColNo = 0;
                        SamResultchk = 0;
                        ColNoChk = 2;
                        SampleCnt = 0;
                        SaveSampleCnt = 0;
                        SaveCurRow = 0;
                        SampleTotCnt = 0;
                        i = 0;
                        j = 0;

                        P4351 = "";
                        P4352 = "";

                        SourceValue = "";
                        TargetValue = "";
                        PreviousValue = "";

                        foreach (DataGridViewRow row3 in dgvResults.Rows)
                        {

                            if (row3.Cells[0].Value != null)
                                SourceValue = (string)row3.Cells[0].Value;
                            else
                                SourceValue = "";
                            

                            dgvFinal.Rows[wRow].Cells[0].Value = wItem_Number;
                            dgvFinal.Rows[wRow].Cells[4].Value = wPartName;
                            dgvFinal.Rows[wRow].Cells[5].Value = wMaterialName;
                            dgvFinal.Rows[wRow].Cells[6].Value = wReportNo;

                            // SGS File No.
                            if (SourceValue == "SGS File No.")
                            {
                                TargetColNo = 7;
                                TargetRowNo = wRow;
                                JobNoRow = row3.Index + 2;
                                wFileNo = (string)dgvResults.Rows[row3.Index + 2].Cells[0].Value; 
                            }

                            // Sample Description
                            if (SourceValue == "Sample Description")
                            {
                                TargetColNo = 2;
                                TargetRowNo = wRow;
                                JobNoRow = row3.Index + 2;
                                wSamDesc = (string)dgvResults.Rows[row3.Index + 2].Cells[0].Value; 
                            }

                            // Style no./ Item no.
                            if (SourceValue.IndexOf("Style no.") > -1)
                            {
                                TargetColNo = 1;
                                TargetRowNo = wRow;
                                JobNoRow = row3.Index + 2;
                                wStyleNo = (string)dgvResults.Rows[row3.Index + 2].Cells[0].Value; 
                            }


                            // Issued Date
                            if (SourceValue.IndexOf("Issued Date") > -1)
                            {
                                string[] line_cell = SourceValue.Split(new char[] { ':' },
                            StringSplitOptions.RemoveEmptyEntries);
                                wIssuedDate = line_cell[1].Replace(" ", "").Substring(0, 10);
                                dgvFinal.Rows[wRow].Cells[8].Value = line_cell[1].Replace(" ", "").Substring(0, 10);
                            }

                            if (SourceValue.IndexOf("ASTM F963-11, Clause 4.3.5.1") > -1)
                            {
                                P4351 = "4351";
                                P4352 = "";
                            }

                            if (SourceValue.IndexOf("ASTM F963-11, Clause 4.3.5.2") > -1)
                            {
                                P4351 = "";
                                P4352 = "4352";
                            }

                            //if (SourceValue == "Sample No.")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            if (SourceValue == "Method Detection Limit")
                            {
                                if (P4351 == "4351" && AnalyteName == "lead(pb)")
                                {
                                    TargetColNo = 9;
                                    TargetRowNo = wRow;
                                    JobNoRow = row3.Index + 1; //Method Detection Limit
                                    wMDL1 = (string)dgvResults.Rows[row3.Index + 1].Cells[0].Value; 
                                }

                                if (P4352 == "4352" && AnalyteName == "lead(pb)")
                                {
                                    TargetColNo = 20;
                                    TargetRowNo = wRow;
                                    JobNoRow = row3.Index + 1;  //Method Detection Limit
                                    wMDL2 = (string)dgvResults.Rows[row3.Index + 1].Cells[0].Value; 
                                }

                                //AnalyteName = SourceValue;
                            }

                            if (P4351_Lead_Pb == 1)
                            {
                                if (SourceValue == "Soluble Heavy Metals" || SourceValue == "Sample Description:")
                                {
                                    P4351_Lead_Pb = 0;
                                    AnalyteName = "";
                                    ij = ij - 1;
                                }
                                else if (ik == (3 * ij) + 3)
                                {
                                    ij = ij + 1;
                                }
                                else
                                {
                                    if (ik == (3 * ij) + 1)
                                    {
                                        TargetColNo = 3;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sample No.

                                    }
                                    else if (ik == (3 * ij) + 2)
                                    {
                                        TargetColNo = 10;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //결과.
                                    }

                                }
                                ik = ik + 1;
                                 
                            }

                            if (P4352_Lead_Pb == 1)
                            {
                                if (SourceValue == "Soluble Heavy Metals" || SourceValue == "Sample Description:")
                                {
                                    AnalyteName = "";
                                    P4352_Lead_Pb = 0;
                                    ij = ij - 1;
                                }
                                else if (ik == (3 * ij) + 3)
                                {
                                    ij = ij + 1;
                                }
                                else
                                {
                                    if (ik == (3 * ij) + 1)
                                    {
                                        TargetColNo = 3;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sample No.

                                    }
                                    else if (ik == (3 * ij) + 2)
                                    {
                                        TargetColNo = 21;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //결과.
                                    }

                                }
                                ik = ik + 1;

                            }

                            if (P4351_sol == 1)
                            {
                                if (SourceValue == "Sample Description:")
                                {
                                    P4351_sol = 0;
                                    AnalyteName = "";
                                    ij = ij - 1;
                                }
                                else if (ik == (11 * ij) + 11)
                                {
                                    ij = ij + 1;
                                }
                                else
                                {
                                    if (ik == (11 * ij) + 1)
                                    {
                                        TargetColNo = 3;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sample No.

                                    }
                                    else if (ik == (11 * ij) + 2)
                                    {
                                        TargetColNo = 11;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Pb
                                    }
                                    else if (ik == (11 * ij) + 3)
                                    {
                                        TargetColNo = 12;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sb
                                    }
                                    else if (ik == (11 * ij) + 4)
                                    {
                                        TargetColNo = 13;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //As
                                    }
                                    else if (ik == (11 * ij) + 5)
                                    {
                                        TargetColNo = 14;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Ba
                                    }
                                    else if (ik == (11 * ij) + 6)
                                    {
                                        TargetColNo = 15;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Cd
                                    }
                                    else if (ik == (11 * ij) + 7)
                                    {
                                        TargetColNo = 16;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Cr
                                    }
                                    else if (ik == (11 * ij) + 8)
                                    {
                                        TargetColNo = 17;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Hg
                                    }
                                    else if (ik == (11 * ij) + 9)
                                    {
                                        TargetColNo = 18;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Se
                                    }
                                    else if (ik == (11 * ij) + 10)
                                    {
                                        TargetColNo = 19;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Se
                                    }
                                }
                                ik = ik + 1;

                            }

                            if (P4352_sol == 1)
                            {
                                if (SourceValue == "Sample Description:")
                                {
                                    P4352_sol = 0;
                                    AnalyteName = "";
                                    ij = ij - 1;
                                }
                                else if (ik == (11 * ij) + 11)
                                {
                                    ij = ij + 1;
                                }
                                else
                                {
                                    if (ik == (11 * ij) + 1)
                                    {
                                        TargetColNo = 3;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sample No.

                                    }
                                    else if (ik == (11 * ij) + 2)
                                    {
                                        TargetColNo = 22;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Pb
                                    }
                                    else if (ik == (11 * ij) + 3)
                                    {
                                        TargetColNo = 23;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sb
                                    }
                                    else if (ik == (11 * ij) + 4)
                                    {
                                        TargetColNo = 24;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //As
                                    }
                                    else if (ik == (11 * ij) + 5)
                                    {
                                        TargetColNo = 25;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Ba
                                    }
                                    else if (ik == (11 * ij) + 6)
                                    {
                                        TargetColNo = 26;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Cd
                                    }
                                    else if (ik == (11 * ij) + 7)
                                    {
                                        TargetColNo = 27;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Cr
                                    }
                                    else if (ik == (11 * ij) + 8)
                                    {
                                        TargetColNo = 28;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Hg
                                    }
                                    else if (ik == (11 * ij) + 9)
                                    {
                                        TargetColNo = 29;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Se
                                    }
                                    else if (ik == (11 * ij) + 10)
                                    {
                                        TargetColNo = 30;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Se
                                    }

                                }
                                ik = ik + 1;

                            }


                            if (SourceValue.IndexOf("Total Result(s)") > -1 || SourceValue.IndexOf("Adjusted Migration Result(s)") > -1)
                            {

                                if (P4351 == "4351" && AnalyteName == "lead(pb)")
                                {
                                    P4351_Lead_Pb = 1;
                                    P4352_Lead_Pb = 0;
                                    JobNoRow = 0;
                                    ij = 0;
                                    ik = 1;
                                }

                                if (P4351 == "4351" && AnalyteName == "solubleheavymetals")
                                {
                                    P4351_sol = 1;
                                    P4352_sol = 0;
                                    JobNoRow = 0;
                                    ij = 0;
                                    ik = 1;
                                }

                                if (P4352 == "4352" && AnalyteName == "lead(pb)")
                                {
                                    P4352_Lead_Pb = 1;
                                    P4351_Lead_Pb = 0;
                                    JobNoRow = 0;
                                    ij = 0;
                                    ik = 1;
                                }
                                if (P4352 == "4352" && AnalyteName == "solubleheavymetals")
                                {
                                    P4352_sol = 1;
                                    P4351_sol = 0;
                                    JobNoRow = 0;
                                    ij = 0;
                                    ik = 1;
                                }
                                //AnalyteName = SourceValue;
                            }

                            if (SourceValue.Replace(" ","").ToLower()  == "lead(pb)")
                            {
                                AnalyteName = SourceValue.Replace(" ", "").ToLower();
                            }

                            if (SourceValue.Replace(" ", "").ToLower() == "solubleheavymetals")
                            {
                                AnalyteName = SourceValue.Replace(" ", "").ToLower();
                            }

                            //if (SourceValue == "Pb")
                            //{
                            //    AnalyteName = SourceValue;

                            //    if (P4351 == "4351" && AnalyteName == "Pb")
                            //    {
                            //        TargetColNo = 8;
                            //        TargetRowNo = wRow;
                            //        JobNoRow = row3.Index + 1; //Method Detection Limit
                            //    }

                            //    if (P4352 == "4352" && AnalyteName == "Pb")
                            //    {
                            //        TargetColNo = 20;
                            //        TargetRowNo = wRow;
                            //        JobNoRow = row3.Index + 1;  //Method Detection Limit
                            //    }
                            //}

                            //if (SourceValue == "Sb")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "As")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "Ba")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "Cd")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "Cr")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "Hg")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "Se")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            if (JobNoRow == row3.Index)
                            {
                                dgvFinal.Rows[TargetRowNo].Cells[0].Value = wItem_Number;
                                dgvFinal.Rows[TargetRowNo].Cells[4].Value = wPartName;
                                dgvFinal.Rows[TargetRowNo].Cells[5].Value = wMaterialName;
                                dgvFinal.Rows[TargetRowNo].Cells[6].Value = wReportNo;
                                dgvFinal.Rows[TargetRowNo].Cells[8].Value = wIssuedDate;
                                dgvFinal.Rows[TargetRowNo].Cells[7].Value = wFileNo ;
                                dgvFinal.Rows[TargetRowNo].Cells[2].Value = wSamDesc ;
                                dgvFinal.Rows[TargetRowNo].Cells[1].Value = wStyleNo ;
                                dgvFinal.Rows[TargetRowNo].Cells[9].Value = wMDL1;
                                dgvFinal.Rows[TargetRowNo].Cells[20].Value = wMDL2; 

                                dgvFinal.Rows[TargetRowNo].Cells[TargetColNo].Value = SourceValue;
                                JobNoRow = 0;
                            }

                        }
                    }
                }

            }

            dgvFinal.Rows[0].Cells[0].Value = "";
            dgvFinal.Rows[0].Cells[1].Value = "";
            dgvFinal.Rows[0].Cells[2].Value = "";
            dgvFinal.Rows[0].Cells[3].Value = "";
            dgvFinal.Rows[0].Cells[4].Value = "";
            dgvFinal.Rows[0].Cells[5].Value = "";
            dgvFinal.Rows[0].Cells[6].Value = "";
            dgvFinal.Rows[0].Cells[7].Value = "";
            dgvFinal.Rows[0].Cells[8].Value = "";
            dgvFinal.Rows[0].Cells[9].Value = "";


        }

        private void Result_Merge2()
        {
            string plainText = string.Empty;
            int wRow = 1;
            int RowChk, RowNo;
            int SampleTot;

            string sRptName;
            string sIssuedDate;
            string wItem_Number;
            string wRpt = "";
            string sRpt = "";

            string wPartName = "";
            string wMaterialName = "";
            string wReportNo = "";
            string wIssuedDate = "";
            string wSamDesc = "";
            string wStyleNo = "";
            string wFileNo = "";
            string wMDL1 = "";
            string wMDL2 = "";

            int ij = 0;

            sIssuedDate = "";
            sRptName = "";
            SampleTot = 0;
            RowNo = 0;
            RowChk = 0;
            wItem_Number = "";

            dgvFinal.Rows.Clear();
            //dgvFinal.RowCount;  
            dgvFinalMerge.Rows.Clear();
            Initialize_Grid();

            progressBar1.Maximum = dgvMergeFiles.Rows.Count;
            foreach (DataGridViewRow row2 in dgvMergeFiles.Rows)
            {
                progressBar1.Value = row2.Index;

                wPartName = (string)row2.Cells[4].Value;
                wMaterialName = (string)row2.Cells[0].Value;
                wReportNo = (string)row2.Cells[1].Value;

                string wRptFileName = (string)row2.Cells[11].Value;

                if (wRptFileName != null && wRptFileName != "")
                {
                    wRptFileName = wRptFileName.Trim();
                    wRpt = wRptFileName.Trim();
                }
                else
                {
                    wRptFileName = "";
                    wRpt = wRptFileName.Trim();
                }

                //if (wRptFileName == "" || wRptFileName == null)
                //{
                //    sRptName = wRptFileName;
                //}

                //else if (wRptFileName.ToLower() == "report file")
                //{
                //    sRptName = "report file"; //report file이라는 이름을 sRptName에 표시해 두고, next line을 읽는다.
                //}

                //else if (wRptFileName.ToLower() == "")
                //{
                //    sRptName = wRptFileName;
                //}
                if (wRptFileName == "Item Number")
                {
                    wItem_Number = (string)row2.Cells[12].Value;
                    //dgvFinal.Rows[0].Cells[3].Value = "";
                    //dgvFinal.Rows[0].Cells[3].Value = (string)row2.Cells[12].Value;
                }

                //else if (sRptName == "report file")  //report file다음 라인에 있는 파일이름을 읽어서 필요한 자료를 가져온다.
                //{

                //}
                if (wRpt.IndexOf(":") > -1 && wRpt.ToString() != sRpt.ToString()  )  
                {
                    sRpt = wRpt; 
                    wRow = wRow + ij + 1;
                    ij = 0;

                    Document document = new Document();
                    string ext = System.IO.Path.GetExtension(wRptFileName).ToUpper();

                    if (ext.Equals(".DOCX") || ext.Equals(".DOC"))
                    {
                        document.LoadFromFile(@wRptFileName);

                        plainText = document.GetText();
                        this.textBox3.Text = plainText;

                        RowChk = RowChk + 1;
                        RowNo = RowChk + row2.Index;


                        //파일 내용을 분해하는 단계 시작------------------------
                        // Get the file's text.
                        string whole_file = textBox3.Text;

                        // Split into lines.
                        //\r\n
                        whole_file = whole_file.Replace('\n', '\r');
                        //whole_file = whole_file.Replace('"', ' ');
                        string[] lines = whole_file.Split(new char[] { '\r' },
                            StringSplitOptions.RemoveEmptyEntries);

                        // See how many rows and columns there are.
                        int num_rows = lines.Length;
                        int num_cols = 1;
                        //int num_cols = lines[num_rows - 1].Split(',').Length;

                        // Allocate the data array.
                        string[,] values = new string[num_rows, num_cols];

                        // Load the array.
                        for (int r = 0; r < num_rows; r++)
                        {
                            //string[] line_r = lines[r].Split(',');
                            string[] line_r = lines[r].Split(new char[] { '\r' },
                            StringSplitOptions.RemoveEmptyEntries);
                            for (int c = 0; c < num_cols; c++)
                            {
                                if (line_r[c] == null)
                                    values[r, c] = "";
                                else
                                    values[r, c] = line_r[c];
                            }
                        }

                        int num1_rows = values.GetUpperBound(0) + 1;
                        int num1_cols = values.GetUpperBound(1) + 1;

                        // Display the data to show we have it.

                        // Make column headers.
                        // For this example, we assume the first row
                        // contains the column names.

                        // Clear previous results.
                        dgvResults.Rows.Clear();

                        for (int c = 0; c < num1_cols; c++)
                            dgvResults.Columns.Add(values[0, c], values[0, c]);

                        // Add the data.
                        for (int r = 1; r <= num1_rows; r++)
                        {
                            dgvResults.Rows.Add();
                            for (int c = 0; c < num1_cols; c++)
                            {
                                dgvResults.Rows[r - 1].Cells[c].Value = values[r - 1, c];
                            }
                        }

                        //파일 내용을 분해하는 단계 끝------------------------

                        //분해된 내용을 정리하는 단계 시작 ----------------------------
                        string SourceValue, TargetValue, PreviousValue, AnalyteName;
                        string P4351;
                        string P4352;
                        int JobNoChk, JobNoRow, TargetRowNo, TargetColNo, SamResultchk, SampleCnt, SaveSampleCnt, SaveCurRow, i, j, ColNoChk;
                        int SampleTotCnt, JobNoRow2;
                        int[] nCol = new int[100];
                        int[] wCol = new int[100];
                        string[] nColValue = new string[100];
                        int P4351_Lead_Pb = 0;
                        int P4352_Lead_Pb = 0;
                        int P4351_sol = 0;
                        int P4352_sol = 0;
                        int ik = 0;


                        AnalyteName = "";

                        JobNoChk = 0;
                        JobNoRow = 0;
                        JobNoRow = 2;

                        TargetRowNo = 0;
                        TargetColNo = 0;
                        SamResultchk = 0;
                        ColNoChk = 2;
                        SampleCnt = 0;
                        SaveSampleCnt = 0;
                        SaveCurRow = 0;
                        SampleTotCnt = 0;
                        i = 0;
                        j = 0;

                        P4351 = "";
                        P4352 = "";

                        SourceValue = "";
                        TargetValue = "";
                        PreviousValue = "";

                        foreach (DataGridViewRow row3 in dgvResults.Rows)
                        {

                            if (row3.Cells[0].Value != null)
                                SourceValue = (string)row3.Cells[0].Value;
                            else
                                SourceValue = "";


                            dgvFinal.Rows[wRow].Cells[0].Value = wItem_Number;
                            dgvFinal.Rows[wRow].Cells[4].Value = wPartName;
                            dgvFinal.Rows[wRow].Cells[5].Value = wMaterialName;
                            dgvFinal.Rows[wRow].Cells[6].Value = wReportNo;

                            // SGS File No.
                            if (SourceValue == "SGS File No.")
                            {
                                TargetColNo = 7;
                                TargetRowNo = wRow;
                                JobNoRow = row3.Index + 2;
                                wFileNo = (string)dgvResults.Rows[row3.Index + 2].Cells[0].Value;
                            }

                            // Sample Description
                            if (SourceValue == "Sample Description")
                            {
                                TargetColNo = 2;
                                TargetRowNo = wRow;
                                JobNoRow = row3.Index + 2;
                                wSamDesc = (string)dgvResults.Rows[row3.Index + 2].Cells[0].Value;
                            }

                            // Style no./ Item no.
                            if (SourceValue.IndexOf("Style no.") > -1)
                            {
                                TargetColNo = 1;
                                TargetRowNo = wRow;
                                JobNoRow = row3.Index + 2;
                                wStyleNo = (string)dgvResults.Rows[row3.Index + 2].Cells[0].Value;
                            }


                            // Issued Date
                            if (SourceValue.IndexOf("Issued Date") > -1)
                            {
                                string[] line_cell = SourceValue.Split(new char[] { ':' },
                            StringSplitOptions.RemoveEmptyEntries);
                                wIssuedDate = line_cell[1].Replace(" ", "").Substring(0, 10);
                                dgvFinal.Rows[wRow].Cells[8].Value = line_cell[1].Replace(" ", "").Substring(0, 10);
                            }

                            if (SourceValue.IndexOf("ASTM F963-11, Clause 4.3.5.1") > -1)
                            {
                                P4351 = "4351";
                                P4352 = "";
                            }

                            if (SourceValue.IndexOf("ASTM F963-11, Clause 4.3.5.2") > -1)
                            {
                                P4351 = "";
                                P4352 = "4352";
                            }

                            //if (SourceValue == "Sample No.")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            if (SourceValue == "Method Detection Limit")
                            {
                                if (P4351 == "4351" && AnalyteName == "lead(pb)")
                                {
                                    TargetColNo = 9;
                                    TargetRowNo = wRow;
                                    JobNoRow = row3.Index + 1; //Method Detection Limit
                                    wMDL1 = (string)dgvResults.Rows[row3.Index + 1].Cells[0].Value;
                                }

                                if (P4352 == "4352" && AnalyteName == "lead(pb)")
                                {
                                    TargetColNo = 20;
                                    TargetRowNo = wRow;
                                    JobNoRow = row3.Index + 1;  //Method Detection Limit
                                    wMDL2 = (string)dgvResults.Rows[row3.Index + 1].Cells[0].Value;
                                }

                                //AnalyteName = SourceValue;
                            }

                            if (P4351_Lead_Pb == 1)
                            {
                                if (SourceValue == "Soluble Heavy Metals" || SourceValue == "Sample Description:")
                                {
                                    P4351_Lead_Pb = 0;
                                    AnalyteName = "";
                                    ij = ij - 1;
                                }
                                else if (ik == (3 * ij) + 3)
                                {
                                    ij = ij + 1;
                                }
                                else
                                {
                                    if (ik == (3 * ij) + 1)
                                    {
                                        TargetColNo = 3;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sample No.

                                    }
                                    else if (ik == (3 * ij) + 2)
                                    {
                                        TargetColNo = 10;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //결과.
                                    }

                                }
                                ik = ik + 1;

                            }

                            if (P4352_Lead_Pb == 1)
                            {
                                if (SourceValue == "Soluble Heavy Metals" || SourceValue == "Sample Description:")
                                {
                                    AnalyteName = "";
                                    P4352_Lead_Pb = 0;
                                    ij = ij - 1;
                                }
                                else if (ik == (3 * ij) + 3)
                                {
                                    ij = ij + 1;
                                }
                                else
                                {
                                    if (ik == (3 * ij) + 1)
                                    {
                                        TargetColNo = 3;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sample No.

                                    }
                                    else if (ik == (3 * ij) + 2)
                                    {
                                        TargetColNo = 21;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //결과.
                                    }

                                }
                                ik = ik + 1;

                            }

                            if (P4351_sol == 1)
                            {
                                if (SourceValue == "Sample Description:")
                                {
                                    P4351_sol = 0;
                                    AnalyteName = "";
                                    ij = ij - 1;
                                }
                                else if (ik == (11 * ij) + 11)
                                {
                                    ij = ij + 1;
                                }
                                else
                                {
                                    if (ik == (11 * ij) + 1)
                                    {
                                        TargetColNo = 3;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sample No.

                                    }
                                    else if (ik == (11 * ij) + 2)
                                    {
                                        TargetColNo = 11;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Pb
                                    }
                                    else if (ik == (11 * ij) + 3)
                                    {
                                        TargetColNo = 12;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sb
                                    }
                                    else if (ik == (11 * ij) + 4)
                                    {
                                        TargetColNo = 13;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //As
                                    }
                                    else if (ik == (11 * ij) + 5)
                                    {
                                        TargetColNo = 14;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Ba
                                    }
                                    else if (ik == (11 * ij) + 6)
                                    {
                                        TargetColNo = 15;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Cd
                                    }
                                    else if (ik == (11 * ij) + 7)
                                    {
                                        TargetColNo = 16;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Cr
                                    }
                                    else if (ik == (11 * ij) + 8)
                                    {
                                        TargetColNo = 17;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Hg
                                    }
                                    else if (ik == (11 * ij) + 9)
                                    {
                                        TargetColNo = 18;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Se
                                    }
                                    else if (ik == (11 * ij) + 10)
                                    {
                                        TargetColNo = 19;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Se
                                    }
                                }
                                ik = ik + 1;

                            }

                            if (P4352_sol == 1)
                            {
                                if (SourceValue == "Sample Description:")
                                {
                                    P4352_sol = 0;
                                    AnalyteName = "";
                                    ij = ij - 1;
                                }
                                else if (ik == (11 * ij) + 11)
                                {
                                    ij = ij + 1;
                                }
                                else
                                {
                                    if (ik == (11 * ij) + 1)
                                    {
                                        TargetColNo = 3;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sample No.

                                    }
                                    else if (ik == (11 * ij) + 2)
                                    {
                                        TargetColNo = 22;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Pb
                                    }
                                    else if (ik == (11 * ij) + 3)
                                    {
                                        TargetColNo = 23;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Sb
                                    }
                                    else if (ik == (11 * ij) + 4)
                                    {
                                        TargetColNo = 24;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //As
                                    }
                                    else if (ik == (11 * ij) + 5)
                                    {
                                        TargetColNo = 25;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Ba
                                    }
                                    else if (ik == (11 * ij) + 6)
                                    {
                                        TargetColNo = 26;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Cd
                                    }
                                    else if (ik == (11 * ij) + 7)
                                    {
                                        TargetColNo = 27;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Cr
                                    }
                                    else if (ik == (11 * ij) + 8)
                                    {
                                        TargetColNo = 28;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Hg
                                    }
                                    else if (ik == (11 * ij) + 9)
                                    {
                                        TargetColNo = 29;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Se
                                    }
                                    else if (ik == (11 * ij) + 10)
                                    {
                                        TargetColNo = 30;
                                        TargetRowNo = wRow + ij;
                                        JobNoRow = row3.Index;  //Se
                                    }

                                }
                                ik = ik + 1;

                            }


                            if (SourceValue.IndexOf("Total Result(s)") > -1)
                            {

                                if (P4351 == "4351" && AnalyteName == "lead(pb)")
                                {
                                    P4351_Lead_Pb = 1;
                                    P4352_Lead_Pb = 0;
                                    JobNoRow = 0;
                                    ij = 0;
                                    ik = 1;
                                }

                                if (P4351 == "4351" && AnalyteName == "solubleheavymetals")
                                {
                                    P4351_sol = 1;
                                    P4352_sol = 0;
                                    JobNoRow = 0;
                                    ij = 0;
                                    ik = 1;
                                }

                                if (P4352 == "4352" && AnalyteName == "lead(pb)")
                                {
                                    P4352_Lead_Pb = 1;
                                    P4351_Lead_Pb = 0;
                                    JobNoRow = 0;
                                    ij = 0;
                                    ik = 1;
                                }
                                if (P4352 == "4352" && AnalyteName == "solubleheavymetals")
                                {
                                    P4352_sol = 1;
                                    P4351_sol = 0;
                                    JobNoRow = 0;
                                    ij = 0;
                                    ik = 1;
                                }
                                //AnalyteName = SourceValue;
                            }

                            if (SourceValue.Replace(" ", "").ToLower() == "lead(pb)")
                            {
                                AnalyteName = SourceValue.Replace(" ", "").ToLower();
                            }

                            if (SourceValue.Replace(" ", "").ToLower() == "solubleheavymetals")
                            {
                                AnalyteName = SourceValue.Replace(" ", "").ToLower();
                            }

                            //if (SourceValue == "Pb")
                            //{
                            //    AnalyteName = SourceValue;

                            //    if (P4351 == "4351" && AnalyteName == "Pb")
                            //    {
                            //        TargetColNo = 8;
                            //        TargetRowNo = wRow;
                            //        JobNoRow = row3.Index + 1; //Method Detection Limit
                            //    }

                            //    if (P4352 == "4352" && AnalyteName == "Pb")
                            //    {
                            //        TargetColNo = 20;
                            //        TargetRowNo = wRow;
                            //        JobNoRow = row3.Index + 1;  //Method Detection Limit
                            //    }
                            //}

                            //if (SourceValue == "Sb")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "As")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "Ba")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "Cd")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "Cr")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "Hg")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            //if (SourceValue == "Se")
                            //{
                            //    AnalyteName = SourceValue;
                            //}

                            if (JobNoRow == row3.Index)
                            {
                                dgvFinal.Rows[TargetRowNo].Cells[0].Value = wItem_Number;
                                dgvFinal.Rows[TargetRowNo].Cells[4].Value = wPartName;
                                dgvFinal.Rows[TargetRowNo].Cells[5].Value = wMaterialName;
                                dgvFinal.Rows[TargetRowNo].Cells[6].Value = wReportNo;
                                dgvFinal.Rows[TargetRowNo].Cells[8].Value = wIssuedDate;
                                dgvFinal.Rows[TargetRowNo].Cells[7].Value = wFileNo;
                                dgvFinal.Rows[TargetRowNo].Cells[2].Value = wSamDesc;
                                dgvFinal.Rows[TargetRowNo].Cells[1].Value = wStyleNo;
                                dgvFinal.Rows[TargetRowNo].Cells[9].Value = wMDL1;
                                dgvFinal.Rows[TargetRowNo].Cells[20].Value = wMDL2;

                                dgvFinal.Rows[TargetRowNo].Cells[TargetColNo].Value = SourceValue;
                                JobNoRow = 0;
                            }

                        }
                    }
                }

            }

            dgvFinal.Rows[0].Cells[0].Value = "";
            dgvFinal.Rows[0].Cells[1].Value = "";
            dgvFinal.Rows[0].Cells[2].Value = "";
            dgvFinal.Rows[0].Cells[3].Value = "";
            dgvFinal.Rows[0].Cells[4].Value = "";
            dgvFinal.Rows[0].Cells[5].Value = "";
            dgvFinal.Rows[0].Cells[6].Value = "";
            dgvFinal.Rows[0].Cells[7].Value = "";
            dgvFinal.Rows[0].Cells[8].Value = "";
            dgvFinal.Rows[0].Cells[9].Value = "";


        }

        private void Result_Merge()
        {
            string plainText = string.Empty;

            int RowChk, RowNo;
            int SampleTot;
            string sRptName;
            string sIssuedDate;

            sIssuedDate = "";
            sRptName = "";
            SampleTot = 0;
            RowNo = 0;
            RowChk = 0;

            dgvFinal.Rows.Clear();
            //dgvFinal.RowCount;  
            dgvFinalMerge.Rows.Clear();
            Initialize_Grid();



            foreach (DataGridViewRow row2 in dgvMergeFiles.Rows)
            {
                /////////////////
                //dgvFinalMerge.Rows.Clear();
                //dgvFinalMerge_initialization();

                progressBar1.Maximum = dgvMergeFiles.Rows.Count;
                //progressBar1.Value = (int)dgvMergeFiles.Rows.Count / 2;

                //Create word document
                if (row2.Index <= dgvMergeFiles.Rows.Count - 2)
                {
                    progressBar1.Value = row2.Index;
                    string wRptFileName = (string)row2.Cells[11].Value;
                    if (wRptFileName != null)
                        wRptFileName = wRptFileName.Trim();
                    else
                        wRptFileName = "";

                    if (wRptFileName == "" || wRptFileName == null)
                    {
                    }
                    else if (sRptName == wRptFileName)
                    {
                        //dgvMergeFiles.Rows[row2.Index].Cells[8].Value = sIssuedDate;
                        //sRptName = wRptFileName;
                    }
                    else if (wRptFileName == "Item Number")
                    {
                        //dgvFinalMerge.Rows[0].Cells[3].Value = "";
                        //dgvFinalMerge.Rows[0].Cells[3].Value = (string)row2.Cells[12].Value;
                    }
                    else if (wRptFileName == "Product Description")
                    {
                        //int ItemCnt;
                        //ItemCnt = 1;

                        //dgvFinalMerge.Rows[1].Cells[3].Value = "";
                        //dgvFinalMerge.Rows[1].Cells[3].Value = (string)row2.Cells[12].Value;
                        ////개별 job의 샘플 결과를 취합하는 단계 시작 ---------------

                        //SampleTot = SampleTot + ItemCnt;

                        //for (int rr = 0; rr < 2; rr++)
                        //{
                        //    //dgvFinal.Columns.Add(1);
                        //    for (int cc = 3; cc < ItemCnt + 3; cc++)
                        //    {
                        //        if (rr < 8)
                        //            dgvFinal.Rows[rr].Cells[SampleTot + cc - ItemCnt].Value = dgvFinalMerge.Rows[rr].Cells[3].Value;
                        //        else
                        //            dgvFinal.Rows[rr].Cells[SampleTot + cc - ItemCnt].Value = dgvFinalMerge.Rows[rr].Cells[cc].Value;
                        //    }
                        //}
                        ////개별 job의 샘플 결과를 취합하는 단계 끝 ---------------
                    }
                    else
                    {
                        dgvFinalMerge.Rows.Clear();
                        Initialize_Grid();

                        sRptName = wRptFileName;
                        wRptFileName = wRptFileName.Trim();

                        Document document = new Document();
                        string ext = System.IO.Path.GetExtension(wRptFileName).ToUpper();

                        if (ext.Equals(".DOCX") || ext.Equals(".DOC"))
                        {
                            document.LoadFromFile(@wRptFileName);

                            plainText = document.GetText();
                            this.textBox3.Text = plainText;

                            RowChk = RowChk + 1;
                            RowNo = RowChk + row2.Index;


                            //파일 내용을 분해하는 단계 시작------------------------
                            // Get the file's text.
                            string whole_file = textBox3.Text;

                            // Split into lines.
                            //\r\n
                            whole_file = whole_file.Replace('\n', '\r');
                            //whole_file = whole_file.Replace('"', ' ');
                            string[] lines = whole_file.Split(new char[] { '\r' },
                                StringSplitOptions.RemoveEmptyEntries);

                            // See how many rows and columns there are.
                            int num_rows = lines.Length;
                            int num_cols = 1;
                            //int num_cols = lines[num_rows - 1].Split(',').Length;

                            // Allocate the data array.
                            string[,] values = new string[num_rows, num_cols];

                            // Load the array.
                            for (int r = 0; r < num_rows; r++)
                            {
                                //string[] line_r = lines[r].Split(',');
                                string[] line_r = lines[r].Split(new char[] { '\r' },
                                StringSplitOptions.RemoveEmptyEntries);
                                for (int c = 0; c < num_cols; c++)
                                {
                                    if (line_r[c] == null)
                                        values[r, c] = "";
                                    else
                                        values[r, c] = line_r[c];
                                }
                            }

                            int num1_rows = values.GetUpperBound(0) + 1;
                            int num1_cols = values.GetUpperBound(1) + 1;

                            // Display the data to show we have it.

                            // Make column headers.
                            // For this example, we assume the first row
                            // contains the column names.

                            // Clear previous results.
                            dgvResults.Rows.Clear();

                            for (int c = 0; c < num1_cols; c++)
                                dgvResults.Columns.Add(values[0, c], values[0, c]);

                            // Add the data.
                            for (int r = 1; r <= num1_rows; r++)
                            {
                                dgvResults.Rows.Add();
                                for (int c = 0; c < num1_cols; c++)
                                {
                                    dgvResults.Rows[r - 1].Cells[c].Value = values[r - 1, c];
                                }
                            }

                            //파일 내용을 분해하는 단계 끝------------------------

                            //string Workchk;
                            //Workchk = "클릭하세요";
                            //MessageBox.Show(@txtDir1.Text + '\\' + row2.Cells[0].Value);

                            //분해된 내용을 정리하는 단계 시작 ----------------------------
                            string SourceValue, TargetValue, PreviousValue, AnalyteName;
                            int JobNoChk, JobNoRow, TargetRowNo, TargetColNo, SamResultchk, SampleCnt, SaveSampleCnt, SaveCurRow, i, j, ColNoChk;
                            int SampleTotCnt;
                            int[] nCol = new int[100];
                            int[] wCol = new int[100];
                            string[] nColValue = new string[100];

                            AnalyteName = "";
                            JobNoChk = 0;
                            JobNoRow = 0;
                            TargetRowNo = 0;
                            TargetColNo = 0;
                            SamResultchk = 0;
                            ColNoChk = 2;
                            SampleCnt = 0;
                            SaveSampleCnt = 0;
                            SaveCurRow = 0;
                            SampleTotCnt = 0;
                            i = 0;
                            j = 0;

                            SourceValue = "";
                            TargetValue = "";
                            PreviousValue = "";

                            foreach (DataGridViewRow row3 in dgvResults.Rows)
                            {
                                if (row3.Cells[0].Value != null)
                                    SourceValue = (string)row3.Cells[0].Value;
                                else
                                    SourceValue = "";

                                dgvFinalMerge.Rows[6].Cells[3].Value = dgvMergeFiles.Rows[row2.Index].Cells[0].Value;
                                dgvFinalMerge.Rows[7].Cells[3].Value = dgvMergeFiles.Rows[row2.Index].Cells[1].Value;

                                // SGS File No.
                                if (SourceValue == "SGS File No.")
                                {
                                    ColNoChk = ColNoChk + 1;

                                    TargetRowNo = 1;
                                    TargetColNo = ColNoChk;

                                    JobNoChk = 1;
                                    JobNoRow = row3.Index + 2;
                                    AnalyteName = SourceValue;
                                }

                                // Style no./Item no.
                                if (SourceValue == "Style no./Item no.")
                                {

                                    ColNoChk = ColNoChk + 1;

                                    TargetRowNo = 2;
                                    TargetColNo = ColNoChk;

                                    JobNoChk = 1;
                                    JobNoRow = row3.Index + 2;
                                    AnalyteName = SourceValue;
                                }

                                // Issued Date
                                if (SourceValue.ToString().Contains("Issued Date"))
                                {
                                    sIssuedDate = "";

                                    ColNoChk = ColNoChk + 1;

                                    TargetRowNo = 3;
                                    TargetColNo = ColNoChk;

                                    JobNoChk = 1;
                                    JobNoRow = row3.Index + 2;

                                    TargetValue = SourceValue.Substring(SourceValue.IndexOf("Issued Date") + 13, 12);
                                    sIssuedDate = TargetValue;

                                    //검색된 워드 성적서 발행일자를 집계 파일에 넣어줌.
                                    dgvMergeFiles.Rows[row2.Index].Cells[8].Value = TargetValue;
                                    AnalyteName = SourceValue;
                                }

                                // Sample Description
                                if (SourceValue == "Sample Description")
                                {

                                    ColNoChk = ColNoChk + 1;

                                    TargetRowNo = 4;
                                    TargetColNo = ColNoChk;

                                    JobNoChk = 1;
                                    JobNoRow = row3.Index + 2;
                                    AnalyteName = SourceValue;
                                }


                                //Permissible Limit (mg/kg)
                                if (SourceValue == "Permissible Limit (mg/kg)" || SourceValue == "(mg/kg)" || SourceValue == "Permissible Limit (EN71-3:2013 +A1:2014) (mg/kg)" || SourceValue == "(EN71-3:2013 +A1:2014) (mg/kg)")
                                {
                                    SamResultchk = 1;
                                    SampleCnt = 0;
                                    AnalyteName = SourceValue;
                                }
                                else if (SamResultchk == 1 & SourceValue != "Mass of trace amount")
                                {
                                    SampleCnt = SampleCnt + 1;
                                    ColNoChk = ColNoChk + SampleCnt;

                                    nCol[SampleCnt] = ColNoChk;
                                    nColValue[SampleCnt] = SourceValue;

                                    for (i = 1; i <= nCol.LongLength - 1; i++)
                                    {
                                        if (nColValue[i] == SourceValue)
                                        {
                                            wCol[SampleCnt] = nCol[i];
                                        }
                                    }

                                    TargetRowNo = 8;
                                    TargetColNo = ColNoChk;

                                    JobNoChk = 1;
                                    JobNoRow = row3.Index;
                                    AnalyteName = SourceValue;
                                }
                                else if (SamResultchk == 1 & SourceValue == "Mass of trace amount")
                                {
                                    SamResultchk = 0;
                                    SampleTotCnt = SampleCnt;
                                    SaveSampleCnt = SampleCnt;
                                    SampleCnt = 0;
                                    AnalyteName = SourceValue;
                                }
                                //(mg)
                                else if (SourceValue == "(mg)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 10;

                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Al)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 11;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Sb)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 12;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(As)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 13;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Ba)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 14;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(B)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 15;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Cd)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 16;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Cr)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 17;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Cr (III))")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 18;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Cr (VI))")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 19;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Co)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 20;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Cu)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 21;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Pb)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 22;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Mn)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 23;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Hg)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 24;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Ni)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 25;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Se)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 26;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Sr)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 27;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Sn)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 28;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "Soluble Organic Tin" || SourceValue == "Soluble Organic Tin^")
                                {
                                    //ExceptChk = 1;
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    //TargetRowNo = TargetRowNo + 1 ;
                                    PreviousValue = SourceValue;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "--" & (PreviousValue == "Soluble Organic Tin" || PreviousValue == "Soluble Organic Tin^"))
                                {
                                    //ExceptChk = 1;
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    PreviousValue = "";
                                    TargetRowNo = 29;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Zn)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 30;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                //Soluble Organic Tin Result(s) (mg/kg)
                                else if (SourceValue == "Soluble Organic Tin Result(s) (mg/kg)")
                                {
                                    SamResultchk = 2;
                                    SampleCnt = 0;
                                    ColNoChk = 2;
                                    j = 0;
                                    AnalyteName = SourceValue;
                                }
                                else if (SamResultchk == 2 & SourceValue != "Methyl tin")
                                {
                                    SampleCnt = SampleCnt + 1;
                                    for (i = 1; i <= nCol.LongLength - 1; i++)
                                    {
                                        if (nColValue[i] == SourceValue)
                                        {
                                            j = j + 1;
                                            wCol[j] = nCol[i];
                                        }
                                    }
                                    JobNoChk = 1;
                                    JobNoRow = 0;
                                    AnalyteName = SourceValue;
                                }
                                else if (SamResultchk == 2 & SourceValue == "Methyl tin")
                                {
                                    SamResultchk = 0;
                                    SaveSampleCnt = SampleCnt - 1;  //샘플 수량 파악 SaveSampleCnt에 저장
                                    SampleCnt = 0;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(MeT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 33;
                                    //TargetRowNo = TargetRowNo + 3;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(DProT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 34;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(BuT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 35;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(DBT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 36;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(TBT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 37;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(MOT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 38;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(TeBT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 39;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(DPhT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 40;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(DOT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 41;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(TPhT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 42;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                // Sample Description :
                                else if (SourceValue == "Sample Description :")
                                {
                                    SamResultchk = 0;
                                    SaveSampleCnt = SampleTotCnt; //샘플 수량 파악 SaveSampleCnt에 저장
                                    SampleCnt = 0;
                                    AnalyteName = SourceValue;

                                    PreviousValue = "Sample Description :";
                                }
                                else if (PreviousValue == "Sample Description :" & SourceValue == (SampleCnt + 1).ToString().Trim() + ".")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 9;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }

                            //else if (PreviousValue == "Sample Description :" & SourceValue == "2.")
                                //    {
                                //        SaveCurRow = row3.Index;
                                //        SamResultchk = SaveCurRow;
                                //        TargetRowNo = 9;
                                //        //TargetRowNo = TargetRowNo + 1;
                                //        AnalyteName = SourceValue;
                                //    }
                                else if (SamResultchk == SaveCurRow & SaveSampleCnt != SampleCnt)
                                {
                                    SampleCnt = SampleCnt + 1;
                                    ColNoChk = ColNoChk + SampleCnt;

                                    //TargetRowNo = 9;
                                    if (SampleCnt > 0)
                                        TargetColNo = wCol[SampleCnt];
                                    else
                                        TargetColNo = ColNoChk;

                                    JobNoChk = 1;
                                    JobNoRow = row3.Index;
                                    //MessageBox.Show(AnalyteName + " : " + SourceValue); 
                                }
                                else if (SamResultchk == SaveCurRow & SaveSampleCnt == SampleCnt)
                                {
                                    SamResultchk = 0;
                                    SampleCnt = 0;
                                    //PreviousValue = "";

                                }

                                // 각 항목별로 값 적용 
                                if (row3.Index == JobNoRow & JobNoChk == 1)
                                {
                                    JobNoChk = 0;
                                    JobNoRow = 0;
                                    if (TargetRowNo == 3)
                                    {
                                        dgvFinalMerge.Rows[TargetRowNo].Cells[3].Value = TargetValue;
                                        TargetValue = "";
                                        ColNoChk = ColNoChk - 1;
                                    }
                                    else
                                    {
                                        if (PreviousValue != "Soluble Organic Tin^" || PreviousValue != "Soluble Organic Tin")
                                        {
                                            dgvFinalMerge.Rows[TargetRowNo].Cells[0].Value = TargetRowNo;
                                            dgvFinalMerge.Rows[TargetRowNo].Cells[TargetColNo].Value = row3.Cells[0].Value;
                                            //return;
                                            if (SampleCnt < 1)
                                                ColNoChk = ColNoChk - 1;
                                            else
                                                ColNoChk = ColNoChk - SampleCnt;
                                            //return;
                                        }
                                    }
                                }

                            }
                            //return;
                            //분해된 내용을 정리하는 단계 끝 ----------------------------

                            //개별 job의 샘플 결과를 취합하는 단계 시작 ---------------

                            SampleTot = SampleTot + SampleTotCnt;

                            for (int rr = 0; rr < 43; rr++)
                            {
                                //dgvFinal.Columns.Add(1);
                                for (int cc = 3; cc < SampleTotCnt + 3; cc++)
                                {
                                    if (rr < 8)
                                        dgvFinal.Rows[rr].Cells[SampleTot + cc - SampleTotCnt].Value = dgvFinalMerge.Rows[rr].Cells[3].Value;
                                    else
                                        dgvFinal.Rows[rr].Cells[SampleTot + cc - SampleTotCnt].Value = dgvFinalMerge.Rows[rr].Cells[cc].Value;
                                }
                            }
                            //개별 job의 샘플 결과를 취합하는 단계 시작 ---------------

                        }
                    }
                }

            }
            progressBar1.Value = dgvMergeFiles.Rows.Count;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Result_Merge();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            Find_Slim();
            Find_Slim2();
        }

        private void Search_Report()
        {
            foreach (DataGridViewRow row1 in dgvMergeFiles.Rows)
            {
                progressBar1.Maximum = dgvMergeFiles.RowCount;
                //progressBar1.Value = (int)dgvMergeFiles.Rows.Count / 2;

                //기존 검색된 성적서 파일명 지우기 - Item Number 와 Product Description 을 지움.
                string wCol11;

                wCol11 = (string)dgvMergeFiles.Rows[row1.Index].Cells[11].Value;
                if (wCol11 == null)
                    wCol11 = "";

                wCol11 = wCol11.Trim();
                if (wCol11 == "Item Number" || wCol11 == "Product Description" || wCol11 == "Part 1,2" || wCol11 == "Part 3")
                {
                }
                //else
                //{
                //    dgvMergeFiles.Rows[row1.Index].Cells[11].Value = "";
                //}

                string wProProj = (string)dgvMergeFiles.Rows[row1.Index].Cells[7].Value;
                if (wProProj == null)
                    wProProj = "";

                wProProj = wProProj.Trim();
                wProProj = wProProj.Replace(" ", "");
                if (wProProj.Length > 3)
                {
                    if (wProProj != "" & wProProj.Substring(0, 3).ToUpper() == "AYH")
                    {
                        //if (wProProj == "AYHA14-07698")
                        //{
                        //    MessageBox.Show (wProProj );
                        //}
                        cboPattern.Text = "*" + wProProj + "*.*";
                        SearchForFiles(lstFiles, txtStartDirectory.Text, cboPattern.Text, txtFind.Text, null);
                        if (lstFiles.Items.Count > 0)
                        {
                            lstFiles.SetSelected(0, true);
                            dgvMergeFiles.Rows[row1.Index].Cells[11].Value = lstFiles.SelectedItems[0].ToString();
                        }
                        else
                        {
                            //dgvMergeFiles.Rows[row1.Index].Cells[11].Value = "";
                        }
                    }
                    else
                    {
                        //dgvMergeFiles.Rows[row1.Index].Cells[11].Value = "";
                    }
                }
                progressBar1.Value = row1.Index;
            }
            progressBar1.Value = progressBar1.Maximum;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Find_Material();
        }

        private void btnPickStartDirectory_Click(object sender, EventArgs e)
        {
            fbdStartDirectory.SelectedPath = txtStartDirectory.Text;
            if (fbdStartDirectory.ShowDialog() == DialogResult.OK)
            {
                txtStartDirectory.Text = fbdStartDirectory.SelectedPath;
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            SearchForFiles(lstFiles, txtStartDirectory.Text,
    cboPattern.Text, txtFind.Text, null);
        }
        private void SearchForFiles(ListBox lst, string start_dir,
    string pattern, string from_string, string to_string)
        {
            try
            {
                // Clear the result ListBox.
                lstFiles.Items.Clear();

                // Parse the patterns.
                string[] patterns = ParsePatterns(pattern);

                // If from_string is blank, don't replace.
                if (from_string.Length < 1) from_string = null;

                DirectoryInfo dir_info = new DirectoryInfo(start_dir);
                SearchDirectory(lst, dir_info, patterns, from_string, to_string);

                if (from_string == null)
                {
                    //    MessageBox.Show("Found " + lst.Items.Count + " files.");
                }
                else
                {
                    //    MessageBox.Show("Made replacements in " + lst.Items.Count + " files.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Find files matching the pattern that contain the target string
        // and make the replacement if appropriate.
        private string[] ParsePatterns(string pattern_string)
        {
            // Take whatever is between the parentheses (if there are any).
            if (pattern_string.Contains("("))
            {
                pattern_string = TextBetween(pattern_string, "(", ")");
            }

            // Split the string at semi-colons.
            string[] result = pattern_string.Split(';');

            // Trim all of the patterns to remove extra space.
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = result[i].Trim();
            }

            return result;
        }

        // Find files matching the pattern that contain the target string
        // and make the replacement if appropriate.
        private void SearchDirectory(ListBox lst, DirectoryInfo dir_info,
            string[] patterns, string from_string, string to_string)
        {
            // Search this directory.
            foreach (string pattern in patterns)
            {
                // Check this pattern.
                foreach (FileInfo file_info in dir_info.GetFiles(pattern))
                {
                    // Process this file.
                    ProcessFile(lst, file_info, from_string, to_string);
                }
            }

            //// Search subdirectories.-2015.07.21 서브디렉터리 검색 기능 막음 -> 필요시 해제 하면 됨
            //foreach (DirectoryInfo subdir_info in dir_info.GetDirectories())
            //{
            //    SearchDirectory(lst, subdir_info, patterns, from_string, to_string);
            //}
        }

        // Replace all occurrences of from_string with to_string.
        // Return true if there was a problem and we should stop.
        private void ProcessFile(ListBox lst, FileInfo file_info, string from_string, string to_string)
        {
            try
            {
                if (from_string == null)
                {
                    // Add the file to the list.
                    lst.Items.Add(file_info.FullName);
                }
                else
                {
                    // See if the file contains from_string.
                    string txt = File.ReadAllText(file_info.FullName);
                    if (txt.Contains(from_string))
                    {
                        // Add the file to the list.
                        lst.Items.Add(file_info.FullName);

                        // See if we should make a replacement.
                        if (to_string != null)
                        {
                            File.WriteAllText(file_info.FullName,
                                txt.Replace(from_string, to_string));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing file " +
                    file_info.FullName + "\n" + ex.Message);
            }
        }

        // Get the text between two delimiters.
        // Let the code throw an error if a delimiter is not found.
        private string TextBetween(string txt, string delimiter1, string delimiter2)
        {
            // Find the starting delimiter.
            int pos1 = txt.IndexOf(delimiter1);
            int text_start = pos1 + delimiter1.Length;
            int pos2 = txt.IndexOf(delimiter2, text_start);
            return txt.Substring(text_start, pos2 - text_start);
        }

        private void cboPattern_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtStartDirectory_TextChanged(object sender, EventArgs e)
        {
            txtDir1.Text = txtStartDirectory.Text;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (txtStartDirectory.Text.Trim() == "")
            {
                MessageBox.Show("성적서가 저장된 경로를 선택 바람.");
                return;
            }
            Search_Report();
        }

        private void dgvMergeFiles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 11)
            {
                string wRptFileName, plainText2;
                object value = dgvMergeFiles.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (value is DBNull) { return; }
                if (value.ToString().Trim().ToUpper().IndexOf(".DOC") < 1) { return; }

                wRptFileName = value.ToString();

                MessageBox.Show(wRptFileName);

                Document document = new Document();
                document.LoadFromFile(@wRptFileName);

                plainText2 = document.GetText();
                this.textBox3.Text = plainText2;

                WordDocViewer(wRptFileName);
            }

            if (e.ColumnIndex == 12)
            {
                object value1 = dgvMergeFiles.Rows[e.RowIndex - 1].Cells[e.ColumnIndex].Value;
                object value2 = dgvMergeFiles.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                if (value1 is DBNull || value2 is DBNull)
                {
                    MessageBox.Show("성적서 이름 확인 바람");
                    return;
                }

                if (value1.ToString().Trim() == "" || value2.ToString().Trim() == "")
                {
                    MessageBox.Show("성적서 이름 확인 바람");
                    return;
                }
                Combine_Rpt(value1.ToString(), value2.ToString());
            }


        }

        private void fbdStartDirectory_HelpRequest(object sender, EventArgs e)
        {

        }

        private void button17_Click(object sender, EventArgs e)
        {
            dgvMergeFiles.Rows.Clear();
            dgvFinal.Rows.Clear();
        }

        private void Combine_Rpt(string mrgRpt1, string mrgRpt2)
        {
            Document documentMerge = new Document();
            Document document = new Document();

            cboPattern.Text = "*" + mrgRpt1 + "*.*";
            SearchForFiles(lstFiles, txtStartDirectory.Text, cboPattern.Text, txtFind.Text, null);
            if (lstFiles.Items.Count > 0)
            {
                lstFiles.SetSelected(0, true);
                //mrgRpt1 = lstFiles.SelectedItems[0].ToString();
                //Create word document
                document.LoadFromFile(lstFiles.SelectedItems[0].ToString(),  Spire.Doc.FileFormat.Doc);
            }
            else
            {
                MessageBox.Show(mrgRpt1 + " 파일확인");
                return;
            }

            cboPattern.Text = "*COMBINE_#" + mrgRpt2 + "*.*";
            SearchForFiles(lstFiles, txtStartDirectory.Text, cboPattern.Text, txtFind.Text, null);
            if (lstFiles.Items.Count > 0)
            {
                lstFiles.SetSelected(0, true);
                //mrgRpt2 = lstFiles.SelectedItems[0].ToString();
                documentMerge.LoadFromFile(lstFiles.SelectedItems[0].ToString(),  Spire.Doc.FileFormat.Doc);

                foreach (Section sec in documentMerge.Sections)
                {
                    document.Sections.Add(sec.Clone());
                }

                //Save doc file.
                document.SaveToFile(mrgRpt2 + "_Sample.doc",  Spire.Doc.FileFormat.Doc);

                //Launching the MS Word file.
                WordDocViewer(mrgRpt2 + "_Sample.doc");
            }
        }


        private void btnCombine_Click(object sender, EventArgs e)
        {

            Document documentMerge = new Document();
            Document document = new Document();

            string mrgRpt1, mrgRpt2;

            mrgRpt1 = "";
            mrgRpt2 = "";

            foreach (DataGridViewRow row2 in dgvMergeFiles.Rows)
            {
                if (row2.Index <= dgvMergeFiles.Rows.Count - 2)
                {
                    if ((string)row2.Cells[11].Value == "Part 1,2")
                    {
                        if ((string)row2.Cells[12].Value.ToString().Trim() != "")
                        {
                            mrgRpt1 = (string)row2.Cells[12].Value;

                            cboPattern.Text = "*" + mrgRpt1 + "*.*";
                            SearchForFiles(lstFiles, txtStartDirectory.Text, cboPattern.Text, txtFind.Text, null);
                            if (lstFiles.Items.Count > 0)
                            {
                                lstFiles.SetSelected(0, true);
                                //mrgRpt1 = lstFiles.SelectedItems[0].ToString();
                                //Create word document
                                document.LoadFromFile(lstFiles.SelectedItems[0].ToString(),  Spire.Doc.FileFormat.Doc);
                            }
                            else
                            {
                                MessageBox.Show(mrgRpt1 + " 파일확인");
                                return;
                            }

                        }
                    }

                    if ((string)row2.Cells[11].Value == "Part 3")
                    {
                        if ((string)row2.Cells[12].Value.ToString().Trim() != "")
                        {
                            mrgRpt2 = (string)row2.Cells[12].Value;

                            cboPattern.Text = "*COMBINE_#" + mrgRpt2 + "*.*";
                            SearchForFiles(lstFiles, txtStartDirectory.Text, cboPattern.Text, txtFind.Text, null);
                            if (lstFiles.Items.Count > 0)
                            {
                                lstFiles.SetSelected(0, true);
                                //mrgRpt2 = lstFiles.SelectedItems[0].ToString();
                                documentMerge.LoadFromFile(lstFiles.SelectedItems[0].ToString(),  Spire.Doc.FileFormat.Doc);

                                foreach (Section sec in documentMerge.Sections)
                                {
                                    document.Sections.Add(sec.Clone());
                                }

                                //Save doc file.
                                document.SaveToFile(mrgRpt2 + "_Sample.doc",  Spire.Doc.FileFormat.Doc);

                                //Launching the MS Word file.
                                WordDocViewer(mrgRpt2 + "_Sample.doc");
                            }

                            else
                            {
                                MessageBox.Show(mrgRpt2 + " 파일확인");
                                return;
                            }

                        }

                    }

                }
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Find_Slim2();
        }

        private void btnPickStartDirectory_Click_1(object sender, EventArgs e)
        {
            fbdStartDirectory.SelectedPath = txtStartDirectory.Text;
            if (fbdStartDirectory.ShowDialog() == DialogResult.OK)
            {
                txtStartDirectory.Text = fbdStartDirectory.SelectedPath;
            }
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            if (this.Width - textBox3.Location.X > 0)
                textBox3.Width = this.Width - textBox3.Location.X + 2;

            if (this.Width - dgvFinal.Width > 0)
                dgvFinal.Width = this.Width - dgvFinal.Location.X + 2;

            Initialize_Grid();

            //dgvFinalMerge.RowCount = 43;
            //dgvFinalMerge.ColumnCount = 100;

            //dgvFinalMerge.Rows[1].Cells[0].Value = "Job No";
            //dgvFinalMerge.Rows[2].Cells[0].Value = "Item No";
            //dgvFinalMerge.Rows[3].Cells[0].Value = "Issue Date";
            //dgvFinalMerge.Rows[4].Cells[0].Value = "Aurora Description";
            //dgvFinalMerge.Rows[5].Cells[0].Value = "Screen Or Not";

            //dgvFinalMerge.Rows[6].Cells[0].Value = "PartName";
            //dgvFinalMerge.Rows[7].Cells[0].Value = "Materials";

            //dgvFinalMerge.Rows[8].Cells[0].Value = "Sample No";
            //dgvFinalMerge.Rows[9].Cells[0].Value = "Report Sample Description";
            //dgvFinalMerge.Rows[10].Cells[0].Value = "Part3 Results";
            //dgvFinalMerge.Rows[33].Cells[0].Value = "Organotin Results";

            //dgvFinalMerge.Rows[10].Cells[1].Value = "Mass of trace amount";
            //dgvFinalMerge.Rows[11].Cells[1].Value = "Soluble Aluminium";
            //dgvFinalMerge.Rows[12].Cells[1].Value = "Soluble Antimony";
            //dgvFinalMerge.Rows[13].Cells[1].Value = "Soluble Arsenic";
            //dgvFinalMerge.Rows[14].Cells[1].Value = "Soluble Barium";
            //dgvFinalMerge.Rows[15].Cells[1].Value = "Soluble Boron";
            //dgvFinalMerge.Rows[16].Cells[1].Value = "Soluble Cadmium";
            //dgvFinalMerge.Rows[17].Cells[1].Value = "Soluble Chromium";
            //dgvFinalMerge.Rows[18].Cells[1].Value = "Soluble Chromium (III) #";
            //dgvFinalMerge.Rows[19].Cells[1].Value = "Soluble Chromium (VI) #";
            //dgvFinalMerge.Rows[20].Cells[1].Value = "Soluble Cobalt";
            //dgvFinalMerge.Rows[21].Cells[1].Value = "Soluble Copper";
            //dgvFinalMerge.Rows[22].Cells[1].Value = "Soluble Lead";
            //dgvFinalMerge.Rows[23].Cells[1].Value = "Soluble Manganese";
            //dgvFinalMerge.Rows[24].Cells[1].Value = "Soluble Mercury";
            //dgvFinalMerge.Rows[25].Cells[1].Value = "Soluble Nickel";
            //dgvFinalMerge.Rows[26].Cells[1].Value = "Soluble Selenium";
            //dgvFinalMerge.Rows[27].Cells[1].Value = "Soluble Strontium";
            //dgvFinalMerge.Rows[28].Cells[1].Value = "Soluble Tin";
            //dgvFinalMerge.Rows[29].Cells[1].Value = "Soluble Organic Tin^";
            //dgvFinalMerge.Rows[30].Cells[1].Value = "Soluble Zinc";

            //dgvFinalMerge.Rows[33].Cells[1].Value = "Methyl tin";
            //dgvFinalMerge.Rows[34].Cells[1].Value = "Di-n-propyl tin";
            //dgvFinalMerge.Rows[35].Cells[1].Value = "Butyl tin";
            //dgvFinalMerge.Rows[36].Cells[1].Value = "Dibutyl tin";
            //dgvFinalMerge.Rows[37].Cells[1].Value = "Tributyl tin";
            //dgvFinalMerge.Rows[38].Cells[1].Value = "n-Octyl tin";
            //dgvFinalMerge.Rows[39].Cells[1].Value = "Tetrabutyl tin";
            //dgvFinalMerge.Rows[40].Cells[1].Value = "Diphenyl tin";
            //dgvFinalMerge.Rows[41].Cells[1].Value = "Di-n-octyl tin";
            //dgvFinalMerge.Rows[42].Cells[1].Value = "Triphenyl tin";

            //dgvFinalMerge.Rows[10].Cells[2].Value = "(mg)";
            //dgvFinalMerge.Rows[11].Cells[2].Value = "(Al)";
            //dgvFinalMerge.Rows[12].Cells[2].Value = "(Sb)";
            //dgvFinalMerge.Rows[13].Cells[2].Value = "(As)";
            //dgvFinalMerge.Rows[14].Cells[2].Value = "(Ba)";
            //dgvFinalMerge.Rows[15].Cells[2].Value = "(B)";
            //dgvFinalMerge.Rows[16].Cells[2].Value = "(Cd)";
            //dgvFinalMerge.Rows[17].Cells[2].Value = "(Cr)";
            //dgvFinalMerge.Rows[18].Cells[2].Value = "(Cr (III))";
            //dgvFinalMerge.Rows[19].Cells[2].Value = "(Cr (VI))";
            //dgvFinalMerge.Rows[20].Cells[2].Value = "(Co)";
            //dgvFinalMerge.Rows[21].Cells[2].Value = "(Cu)";
            //dgvFinalMerge.Rows[22].Cells[2].Value = "(Pb)";
            //dgvFinalMerge.Rows[23].Cells[2].Value = "(Mn)";
            //dgvFinalMerge.Rows[24].Cells[2].Value = "(Hg)";
            //dgvFinalMerge.Rows[25].Cells[2].Value = "(Ni)";
            //dgvFinalMerge.Rows[26].Cells[2].Value = "(Se)";
            //dgvFinalMerge.Rows[27].Cells[2].Value = "(Sr)";
            //dgvFinalMerge.Rows[28].Cells[2].Value = "(Sn)";
            //dgvFinalMerge.Rows[29].Cells[2].Value = "--";
            //dgvFinalMerge.Rows[30].Cells[2].Value = "(Zn)";

            //dgvFinalMerge.Rows[33].Cells[2].Value = "(MeT)";
            //dgvFinalMerge.Rows[34].Cells[2].Value = "(DProT)";
            //dgvFinalMerge.Rows[35].Cells[2].Value = "(BuT)";
            //dgvFinalMerge.Rows[36].Cells[2].Value = "(DBT)";
            //dgvFinalMerge.Rows[37].Cells[2].Value = "(TBT)";
            //dgvFinalMerge.Rows[38].Cells[2].Value = "(MOT)";
            //dgvFinalMerge.Rows[39].Cells[2].Value = "(TeBT)";
            //dgvFinalMerge.Rows[40].Cells[2].Value = "(DPhT)";
            //dgvFinalMerge.Rows[41].Cells[2].Value = "(DOT)";
            //dgvFinalMerge.Rows[42].Cells[2].Value = "(TPhT)";

            //dgvFinal.RowCount = 44;
            //dgvFinal.ColumnCount = 100;

            //dgvFinal.Rows[1].Cells[0].Value = "Job No";
            //dgvFinal.Rows[2].Cells[0].Value = "Item No";
            //dgvFinal.Rows[3].Cells[0].Value = "Issue Date";
            //dgvFinal.Rows[4].Cells[0].Value = "Aurora Description";
            //dgvFinal.Rows[5].Cells[0].Value = "Screen Or Not";

            //dgvFinal.Rows[6].Cells[0].Value = "PartName";
            //dgvFinal.Rows[7].Cells[0].Value = "Materials";

            //dgvFinal.Rows[8].Cells[0].Value = "Sample No";
            //dgvFinal.Rows[9].Cells[0].Value = "Report Sample Description";
            //dgvFinal.Rows[10].Cells[0].Value = "Part3 Results";
            //dgvFinal.Rows[33].Cells[0].Value = "Organotin Results";

            //dgvFinal.Rows[10].Cells[1].Value = "Mass of trace amount";
            //dgvFinal.Rows[11].Cells[1].Value = "Soluble Aluminium";
            //dgvFinal.Rows[12].Cells[1].Value = "Soluble Antimony";
            //dgvFinal.Rows[13].Cells[1].Value = "Soluble Arsenic";
            //dgvFinal.Rows[14].Cells[1].Value = "Soluble Barium";
            //dgvFinal.Rows[15].Cells[1].Value = "Soluble Boron";
            //dgvFinal.Rows[16].Cells[1].Value = "Soluble Cadmium";
            //dgvFinal.Rows[17].Cells[1].Value = "Soluble Chromium";
            //dgvFinal.Rows[18].Cells[1].Value = "Soluble Chromium (III) #";
            //dgvFinal.Rows[19].Cells[1].Value = "Soluble Chromium (VI) #";
            //dgvFinal.Rows[20].Cells[1].Value = "Soluble Cobalt";
            //dgvFinal.Rows[21].Cells[1].Value = "Soluble Copper";
            //dgvFinal.Rows[22].Cells[1].Value = "Soluble Lead";
            //dgvFinal.Rows[23].Cells[1].Value = "Soluble Manganese";
            //dgvFinal.Rows[24].Cells[1].Value = "Soluble Mercury";
            //dgvFinal.Rows[25].Cells[1].Value = "Soluble Nickel";
            //dgvFinal.Rows[26].Cells[1].Value = "Soluble Selenium";
            //dgvFinal.Rows[27].Cells[1].Value = "Soluble Strontium";
            //dgvFinal.Rows[28].Cells[1].Value = "Soluble Tin";
            //dgvFinal.Rows[29].Cells[1].Value = "Soluble Organic Tin^";
            //dgvFinal.Rows[30].Cells[1].Value = "Soluble Zinc";

            //dgvFinal.Rows[33].Cells[1].Value = "Methyl tin";
            //dgvFinal.Rows[34].Cells[1].Value = "Di-n-propyl tin";
            //dgvFinal.Rows[35].Cells[1].Value = "Butyl tin";
            //dgvFinal.Rows[36].Cells[1].Value = "Dibutyl tin";
            //dgvFinal.Rows[37].Cells[1].Value = "Tributyl tin";
            //dgvFinal.Rows[38].Cells[1].Value = "n-Octyl tin";
            //dgvFinal.Rows[39].Cells[1].Value = "Tetrabutyl tin";
            //dgvFinal.Rows[40].Cells[1].Value = "Diphenyl tin";
            //dgvFinal.Rows[41].Cells[1].Value = "Di-n-octyl tin";
            //dgvFinal.Rows[42].Cells[1].Value = "Triphenyl tin";

            //dgvFinal.Rows[10].Cells[2].Value = "(mg)";
            //dgvFinal.Rows[11].Cells[2].Value = "(Al)";
            //dgvFinal.Rows[12].Cells[2].Value = "(Sb)";
            //dgvFinal.Rows[13].Cells[2].Value = "(As)";
            //dgvFinal.Rows[14].Cells[2].Value = "(Ba)";
            //dgvFinal.Rows[15].Cells[2].Value = "(B)";
            //dgvFinal.Rows[16].Cells[2].Value = "(Cd)";
            //dgvFinal.Rows[17].Cells[2].Value = "(Cr)";
            //dgvFinal.Rows[18].Cells[2].Value = "(Cr (III))";
            //dgvFinal.Rows[19].Cells[2].Value = "(Cr (VI))";
            //dgvFinal.Rows[20].Cells[2].Value = "(Co)";
            //dgvFinal.Rows[21].Cells[2].Value = "(Cu)";
            //dgvFinal.Rows[22].Cells[2].Value = "(Pb)";
            //dgvFinal.Rows[23].Cells[2].Value = "(Mn)";
            //dgvFinal.Rows[24].Cells[2].Value = "(Hg)";
            //dgvFinal.Rows[25].Cells[2].Value = "(Ni)";
            //dgvFinal.Rows[26].Cells[2].Value = "(Se)";
            //dgvFinal.Rows[27].Cells[2].Value = "(Sr)";
            //dgvFinal.Rows[28].Cells[2].Value = "(Sn)";
            //dgvFinal.Rows[29].Cells[2].Value = "--";
            //dgvFinal.Rows[30].Cells[2].Value = "(Zn)";

            //dgvFinal.Rows[33].Cells[2].Value = "(MeT)";
            //dgvFinal.Rows[34].Cells[2].Value = "(DProT)";
            //dgvFinal.Rows[35].Cells[2].Value = "(BuT)";
            //dgvFinal.Rows[36].Cells[2].Value = "(DBT)";
            //dgvFinal.Rows[37].Cells[2].Value = "(TBT)";
            //dgvFinal.Rows[38].Cells[2].Value = "(MOT)";
            //dgvFinal.Rows[39].Cells[2].Value = "(TeBT)";
            //dgvFinal.Rows[40].Cells[2].Value = "(DPhT)";
            //dgvFinal.Rows[41].Cells[2].Value = "(DOT)";
            //dgvFinal.Rows[42].Cells[2].Value = "(TPhT)";

            dgvMergeFiles.RowCount = 50;
            foreach (DataGridViewRow row1 in dgvValues.Rows)
            {
                if (row1.Index >= 0 & row1.Index <= 5)
                {
                    dgvMergeFiles.Rows.Add(1);
                    foreach (DataGridViewColumn col1 in dgvValues.Columns)
                    {
                        if (col1.Index >= 0 & col1.Index <= 3)
                        {
                            dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].Value = dgvValues.Rows[row1.Index].Cells[col1.Index].Value;
                            //if (dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].ToString().Contains("Part") & row1.Index == 5)
                            //{
                            //    dgvMergeFiles.Rows[row1.Index].Cells[4].Value = "Report No";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[5].Value = "Issued Date";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[6].Value = "Report Sample Description";
                            //}
                        }
                    }
                }
            }

            //dgvMergeFiles.Rows[5].Cells[4].Value = "Report No";
            //dgvMergeFiles.Rows[5].Cells[5].Value = "Issued Date";
            //dgvMergeFiles.Rows[5].Cells[6].Value = "Report Sample Description";

            string fileName = OpenFile();
            //string fileName = "E:\\T42\\WORK\\request\\오로라\\테스트\\복사본 KI2015019.xls";
            string plainText = string.Empty;
            string ext = System.IO.Path.GetExtension(fileName).ToUpper();
            string directoryPath = string.Empty;




            if (ext.Equals(".DOCX") || ext.Equals(".DOC"))
            {

                //Create word document

                Document document = new Document();




                //load a document

                document.LoadFromFile(@fileName);




                plainText = document.GetText();

                this.textBox3.Text = plainText;

            }

            else if (ext.Equals(".XLSX") || ext.Equals(".XLS"))
            {

                //Create Excel workbook

                

                Spire.Xls.Workbook workbook = new Workbook();

                


                //load a workbook

                workbook.LoadFromFile(@fileName);

                directoryPath = "c:\\temp\\";


                for (int i = 0; i < workbook.Worksheets.Count; i++)
                {

                    string tmpfilename = "tempSheet" + i.ToString() + ".txt";

                    Worksheet sheet = workbook.Worksheets[i];




                    if (!sheet.IsEmpty)
                    {

                        if (System.IO.File.Exists(directoryPath + tmpfilename))
                        {

                            System.IO.File.Delete(directoryPath + tmpfilename);

                        }


                        sheet.SaveToFile(directoryPath + tmpfilename, ", ", Encoding.UTF8);

                        plainText += "--[" + sheet.Name + "]--\r\n";

                        plainText += System.IO.File.ReadAllText(directoryPath + tmpfilename);

                        plainText += "\r\n";

                    }

                }
                this.textBox2.Text = plainText;
            }


        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            // Get the file's text.
            string whole_file = textBox2.Text;

            // Split into lines.
            //\r\n
            whole_file = whole_file.Replace('\n', '\r');
            whole_file = whole_file.Replace('"', ' ');
            string[] lines = whole_file.Split(new char[] { '\r' },
                StringSplitOptions.RemoveEmptyEntries);

            // See how many rows and columns there are.
            int num_rows = lines.Length;
            //MessageBox.Show(lines[num_rows - 1]);
            //int num_cols = lines[num_rows - 1].Split(',').Length; //일반
            int num_cols = 10;  //오로라 전용

            // Allocate the data array.
            string[,] values = new string[num_rows, num_cols];

            // Load the array.
            for (int r = 1; r < num_rows; r++)
            {
                string[] line_r = lines[r].Split(',');
                int row_cols = lines[r].Split(',').Length;

                for (int c = 0; c < row_cols; c++)
                {
                    if (line_r[c] == null)
                        values[r, c] = "";
                    else
                        values[r, c] = line_r[c];
                }
            }

            int num1_rows = values.GetUpperBound(0) + 1;
            int num1_cols = values.GetUpperBound(1) + 1;

            // Display the data to show we have it.

            // Make column headers.
            // For this example, we assume the first row
            // contains the column names.

            // Clear previous results.
            dgvValues.Rows.Clear();

            for (int c = 0; c < num1_cols; c++)
                dgvValues.Columns.Add(values[0, c], values[0, c]);

            // Add the data.
            for (int r = 1; r < num1_rows; r++)
            {
                dgvValues.Rows.Add();
                for (int c = 0; c < num1_cols; c++)
                {
                    dgvValues.Rows[r - 1].Cells[c].Value = values[r, c];
                }
            }

            //// Make the columns autosize.
            //foreach (DataGridViewColumn col in dgvValues.Columns)
            //    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            Find_Slim();
            Find_Slim2();
        }

        private void button16_Click_1(object sender, EventArgs e)
        {
            if (txtStartDirectory.Text.Trim() == "")
            {
                MessageBox.Show("성적서가 저장된 경로를 선택 바람.");
                return;
            }
            Search_Report();
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            //Result_Merge1(); // v1-orgine
            //Result_Merge11(); // v2-V1 UPGRADE

            //Result_Merge2(); // v3
            Result_Merge3();//v4최신버전
        }

        private void Result_Merge3()
        {
            string plainText = string.Empty;

            string wItem_Number = "";
            string wProduct_Desc = "";
            string sRpt = "";

            string wPartName = "";
            string wMaterialName = "";
            string wReportNo = "";
            string swReportNo = "";
            string wPart12 = "";
            string wPart3 = "";

            string wRptFileName = "";

            //string wCellValue = "";

            int wRowNo1 = 1;

            progressBar1.Maximum = dgvMergeFiles.Rows.Count;

            foreach (DataGridViewRow row2 in dgvMergeFiles.Rows)
            {
                progressBar1.Value = row2.Index;

                string wCellValue = (string)row2.Cells[11].Value;
                if (wCellValue != null && wCellValue != "")
                {
                    if (wCellValue.Trim()  == "Item Number")
                    {
                        wItem_Number = (string)row2.Cells[12].Value;
                    }

                    if (wCellValue.Trim() == "Product Description")
                    {
                        wProduct_Desc = (string)row2.Cells[12].Value;
                    }

                    if (wCellValue.Trim() == "Part 1,2")
                    {
                        wPart12 = (string)row2.Cells[12].Value;
                    }

                    if (wCellValue.Trim() == "Part 3")
                    {
                        wPart3 = (string)row2.Cells[12].Value;
                    }

                    if (wCellValue.Trim() == "Report File")
                    {
                        wRptFileName = "";
                    }

                    wRptFileName = wCellValue.Trim();

                    //if (wRptFileName.IndexOf(":") > -1 && wRptFileName.ToString() != sRpt.ToString()) //성적서 중복 체크 - 이전 취합된 1건의 성적서만 비교
                    //if (wRptFileName.IndexOf(":") > -1)  // 성적서 중복 체크 기능 제거 - 전체 성적서 출력
                    if (wRptFileName.ToString() != sRpt.ToString()) 
                    {
                        wPartName = (string)row2.Cells[0].Value;
                        wMaterialName = (string)row2.Cells[1].Value;
                        wReportNo = (string)row2.Cells[7].Value;

                        sRpt = wRptFileName;

                        Document document = new Document();
                        string ext = System.IO.Path.GetExtension(wRptFileName).ToUpper();

                        if (ext.Equals(".DOCX") || ext.Equals(".DOC"))
                        {
                            document.LoadFromFile(@wRptFileName);

                            plainText = document.GetText();
                            textBox3.Text = "";

                            this.textBox3.Text = plainText;

                            //파일 내용을 분해하는 단계 시작------------------------
                            // Get the file's text.
                            string whole_file = textBox3.Text;

                            // Split into lines.
                            //\r\n
                            whole_file = whole_file.Replace('\n', '\r');
                            //whole_file = whole_file.Replace('"', ' ');
                            string[] lines = whole_file.Split(new char[] { '\r' },
                                StringSplitOptions.RemoveEmptyEntries);

                            // See how many rows and columns there are.
                            int num_rows = lines.Length;
                            int num_cols = 1;
                            //int num_cols = lines[num_rows - 1].Split(',').Length;

                            // Allocate the data array.
                            string[,] values = new string[num_rows, num_cols];

                            // Load the array.
                            for (int r = 0; r < num_rows; r++)
                            {
                                //string[] line_r = lines[r].Split(',');
                                string[] line_r = lines[r].Split(new char[] { '\r' },
                                StringSplitOptions.RemoveEmptyEntries);
                                for (int c = 0; c < num_cols; c++)
                                {
                                    if (line_r[c] == null)
                                        values[r, c] = "";
                                    else
                                        values[r, c] = line_r[c];
                                }
                            }

                            int num1_rows = values.GetUpperBound(0) + 1;
                            int num1_cols = values.GetUpperBound(1) + 1;

                            // Display the data to show we have it.

                            // Make column headers.
                            // For this example, we assume the first row
                            // contains the column names.

                            // Clear previous results.
                            dgvResults.Rows.Clear();

                            for (int c = 0; c < num1_cols; c++)
                                dgvResults.Columns.Add(values[0, c], values[0, c]);

                            // Add the data.
                            for (int r = 1; r <= num1_rows; r++)
                            {
                                dgvResults.Rows.Add();
                                for (int c = 0; c < num1_cols; c++)
                                {
                                    dgvResults.Rows[r - 1].Cells[c].Value = values[r - 1, c];
                                }
                            }

                            //파일 내용을 분해하는 단계 끝------------------------

                            //분해된 내용을 정리하는 단계 시작 ----------------------------
                            int ccc = 1;
                            //if(ccc == 1 && swReportNo.IndexOf(wReportNo) < 0 ) //성적서 중복 체크(이전에 취합된 모든 성적서에 대한 중복 체크)
                            if (ccc == 1)  //성적서 중복 체크 기능 제거
                            {
                                swReportNo = swReportNo + "," + wReportNo;

                                //wRowNo1 = wRowNo1 + 1;
                                //txtRowNo.Text = wRowNo1.ToString();    

                                dgvFinalMerge.RowCount = 50;
                                dgvFinalMerge.ColumnCount = 50;

                                //dgvFinalMerge.Rows[wRowNo1].Cells[0].Value = wReportNo;
                                //dgvFinalMerge.Rows[wRowNo1].Cells[1].Value = wItem_Number;
                                //dgvFinalMerge.Rows[wRowNo1].Cells[2].Value = wProduct_Desc;
                                //dgvFinalMerge.Rows[wRowNo1].Cells[3].Value = wPartName;
                                //dgvFinalMerge.Rows[wRowNo1].Cells[4].Value = wMaterialName;

                                Merge_Step2(wItem_Number, wProduct_Desc, wPartName, wMaterialName, wReportNo);

                                //break;
                                foreach (DataGridViewRow row4 in dgvFinalMerge.Rows)
                                {
                                    if((string)row4.Cells[0].Value != "" && (string)row4.Cells[0].Value != null)
                                    {
                                        wRowNo1 = wRowNo1 + 1;
                                        for (int c = 0; c < 31; c++)
                                        {
                                            dgvFinal.Rows[wRowNo1].Cells[c].Value = (string)row4.Cells[c].Value;
                                        }
                                    }
                                }
                                //issued Date 변경1단계 - 2단계 루틴 확인 필요
                                dgvMergeFiles.Rows[row2.Index].Cells[8].Value = dgvFinalMerge.Rows[0].Cells[8].Value;

                            }
                            //분해된 내용을 정리하는 단계 끝 ----------------------------

                        }
                    }
                    else
                    {
                        //issued Date 변경2단계 - 성적서 비교하여 같은 성적서가 있으면 이전 성적서와 동일한날짜로 변경
                        if (row2.Index > 0)
                        {
                            string wRptName = (string)dgvMergeFiles.Rows[row2.Index - 1].Cells[8].Value;
                            string wRowValue = (string)dgvMergeFiles.Rows[row2.Index].Cells[7].Value;

                            if (wRptName == null)
                            {
                                wRptName = "";
                            }

                            if (wRowValue == null)
                            {
                                wRowValue = "";
                            }
                            if (wRowValue.ToString().Trim() != "" && wRptName.ToString().Trim() != "")
                            {
                                dgvMergeFiles.Rows[row2.Index].Cells[8].Value = wRptName.ToString().Trim();
                            }
                        }
                    }
                }

            }
        }

        private void Merge_Step2(string sItem_Number, string sProduct_Desc, string sPartName, string sMaterialName, string sReportNo)
        {
            string SourceValue = "";
            string wJob_FileNo = "";
            string wJob_SamDesc = "";
            string wJob_SamDesc2 = "";
            string wJob_StyleNo = "";
            string wJob_IssuedDate = "";
            string P4351 = "";
            string P4352 = "";
            string AnalyteName = "";
            string wMDL1 = "";
            string wMDL2 = "";
            
            int P4351_Lead_Pb = 0;
            int P4352_Lead_Pb = 0;
            int ik = 0;
            int ij = 0;
            int TargetColNo = 0;
            int TargetRowNo = 0;
            int wRow = 0;
            int JobNoRow = 0;
            int P4351_sol = 0;
            int P4352_sol = 0;
            int optSolRow = 0;

            int Lead_4351_RowNo_B = 0;  //4351 LEAD 의 결과 시작 줄 번호
            int Lead_4351_RowNo_E = 0;  //4351 LEAD 의 결과 끝 줄 번호

            int Lead_4352_RowNo_B = 0;  //4352 LEAD 의 결과 시작 줄 번호
            int Lead_4352_RowNo_E = 0;  //4352 LEAD 의 결과 끝 줄 번호

            int Sol_4351_RowNo_B = 0;  //4351 SOL 의 결과 시작 줄 번호
            int Sol_4351_RowNo_E = 0;  //4351 SOL 의 결과 끝 줄 번호

            int Sol_4352_RowNo_B = 0;  //4352 SOL 의 결과 시작 줄 번호
            int Sol_4352_RowNo_E = 0;  //4352 SOL 의 결과 끝 줄 번호

            int SD_4351_RowNo_B = 0;  //4351 Sample Description 의 시작 줄 번호
            int SD_4351_RowNo_E = 0;  //4351 Sample Description 의 끝 줄 번호

            int SD_4352_RowNo_B = 0;  //4352 Sample Description 의 시작 줄 번호
            int SD_4352_RowNo_E = 0;  //4352 Sample Description 의 끝 줄 번호

            string[] SD_4351_Sam_No = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] SD_4352_Sam_No = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

            string[] Lead_4351_Sam_No = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Lead_4351_Sam_Result = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

            string[] Lead_4352_Sam_No = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Lead_4352_Sam_Result = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

            string[] Sol_4351_Sam_No = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4351_Sam_Item = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4351_Mass_Amt = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

            string[] Sol_4351_Sam_Result1 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4351_Sam_Result2 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4351_Sam_Result3 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4351_Sam_Result4 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4351_Sam_Result5 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4351_Sam_Result6 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4351_Sam_Result7 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4351_Sam_Result8 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4351_Sam_Result9 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4351_Sam_Result10 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };



            string[] Sol_4352_Sam_No = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4352_Sam_Item = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4352_Mass_Amt = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

            string[] Sol_4352_Sam_Result1 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4352_Sam_Result2 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4352_Sam_Result3 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4352_Sam_Result4 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4352_Sam_Result5 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4352_Sam_Result6 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4352_Sam_Result7 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4352_Sam_Result8 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4352_Sam_Result9 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] Sol_4352_Sam_Result10 = new string[20] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

            dgvFinal.RowCount = 1000;
            
            //1단계 - 결과 위치 찾는 루틴
            foreach (DataGridViewRow row3 in dgvResults.Rows)
            {
                if (row3.Cells[0].Value != null)
                    SourceValue = (string)row3.Cells[0].Value;
                else
                    SourceValue = "";

                // SGS File No. = Referred from Test report No
                if (SourceValue.Trim()  == "SGS File No.")
                {
                    wJob_FileNo = (string)dgvResults.Rows[row3.Index + 2].Cells[0].Value;
                    //dgvFinal.Rows[wFinalRowNo].Cells[5].Value = wJob_FileNo.Trim();
                }

                // Sample Description = Description
                if (SourceValue.Trim() == "Sample Description")                
                {
                    wJob_SamDesc = (string)dgvResults.Rows[row3.Index + 2].Cells[0].Value;
                }

                // Sample Description = Description
                if (SourceValue.Trim() == "Sample Description:")
                {
                    wJob_SamDesc2 = (string)dgvResults.Rows[row3.Index + 1].Cells[0].Value.ToString().Substring(3).Trim();
                }

                // Style no./ Item no. = Related Item No
                if (SourceValue.Trim().IndexOf("Style no.") > -1)
                {
                    wJob_StyleNo = (string)dgvResults.Rows[row3.Index + 2].Cells[0].Value;
                }

                // Issued Date = Issued Date
                if (SourceValue.Trim().IndexOf("Issued Date") > -1)
                {
                    string[] line_cell = SourceValue.Split(new char[] { ':' },
                StringSplitOptions.RemoveEmptyEntries);
                    wJob_IssuedDate = line_cell[1].Replace(" ", "").Substring(0, 10);
                }

                //if ((SourceValue.Trim().Replace(" ", "").IndexOf("ASTMF963-16,Clause4.3.5.1") > -1) || (SourceValue.Trim().Replace(" ", "").IndexOf("ASTMF963-17,Clause4.3.5.1") > -1))
                if ((SourceValue.Trim().Replace(" ", "").IndexOf("ASTMF963-23,Clause4.3.5.1") > -1) || (SourceValue.Trim().Replace(" ", "").IndexOf("ASTMF963-23,Clause4.3.5.1") > -1))
                {
                    P4351 = "4351";
                    P4352 = "";
                }

                //if ((SourceValue.Trim().Replace(" ", "").IndexOf("ASTMF963-16,Clause4.3.5.2") > -1) || (SourceValue.Trim().Replace(" ", "").IndexOf("ASTMF963-17,Clause4.3.5.2") > -1))
                if ((SourceValue.Trim().Replace(" ", "").IndexOf("ASTMF963-23,Clause4.3.5.2") > -1) || (SourceValue.Trim().Replace(" ", "").IndexOf("ASTMF963-23,Clause4.3.5.2") > -1))
                {
                    P4351 = "";
                    P4352 = "4352";
                }


                // 4351 체크 루틴
                if (P4351 == "4351" && SourceValue.Replace(" ", "").ToLower() == "lead(pb)")
                {
                    Lead_4351_RowNo_B = row3.Index + 8; //4351 LEAD 결과가 시작되는 줄 번호 저장
                    P4351_Lead_Pb = 1;
                    P4351_sol = 0;
                }

                if (P4351 == "4351" && SourceValue.Replace(" ", "").ToLower() == "solubleheavymetals")
                {
                    if (Lead_4351_RowNo_B > 0)
                    {
                        Lead_4351_RowNo_E = row3.Index - 1; //4351 LEAD 결과가 끝나는 줄 번호 저장
                    }
                    else
                    {
                        Lead_4351_RowNo_E = 0; //4351 LEAD 결과가 끝나는 줄 번호 저장
                    }

                    Sol_4351_RowNo_B = row3.Index + 1; //4351 Sol 결과가 시작되는 줄 번호 저장
                    P4351_sol = 1;  //soluble heavy metals이 있음을 표시
                    P4351_Lead_Pb = 0;
                }

                if (P4351 == "4351" && SourceValue.Trim().Replace(" ", "").ToLower() == "sampledescription")
                {
                    if (P4351_sol == 0)  //soluble heavy metals가 없을 경우
                    {
                        Lead_4351_RowNo_E = row3.Index - 1; //4351 LEAD 결과가 끝나는 줄 번호 저장
                    }
                    if (P4351_sol == 1)  //soluble heavy metals가 있을 경우
                    {
                        Sol_4351_RowNo_E = row3.Index - 1; //4351 Sol 결과가 끝나는 줄 번호 저장
                    }

                    SD_4351_RowNo_B = row3.Index + 1; //4351 Sample Description 시작되는 줄 번호 저장
                }

                if (P4351 == "4351" && SourceValue.Replace(" ", "").ToLower() == "note")
                {
                    SD_4351_RowNo_E = row3.Index - 1; //4351 Sample Description 끝나는 줄 번호 저장
                    P4351_sol = 0;
                    P4351 = "";
                }


                //4352 체크 루틴
                if (P4352 == "4352" && SourceValue.Replace(" ", "").ToLower() == "lead(pb)")
                {
                    Lead_4352_RowNo_B = row3.Index + 8; //4352 LEAD 결과가 시작되는 줄 번호 저장
                    P4352_Lead_Pb = 1;
                    P4352_sol = 0;
                }

                //if (P4352 == "4352" && SourceValue.Replace(" ", "").ToLower() == "solubleheavymetals")
                if (P4352 == "4352" && SourceValue.Replace(" ", "").ToLower().Contains("solubleheavymetal"))
                {
                    if (Lead_4352_RowNo_B > 0)
                    {
                        Lead_4352_RowNo_E = row3.Index - 1; //4352 LEAD 결과가 끝나는 줄 번호 저장
                    }
                    else
                    {
                        Lead_4352_RowNo_E = 0;
                    }

                    Sol_4352_RowNo_B = row3.Index + 1; //4352 Sol 결과가 시작되는 줄 번호 저장
                    P4352_sol = 1;  //soluble heavy metals이 있음을 표시
                    P4352_Lead_Pb = 0;
                }

                if (P4352 == "4352" && Sol_4352_RowNo_B > 0 && SourceValue.Replace(" ", "").ToLower() == "migrationlimit(mg/kg)-modelingclays")
                {
                    // int optSolRow ;  //Sol의 결과 값 선택 위치를 조정하기 위한 옵션 - 경우에 따라 4351과 동일한 위치의 결과 취합해야함.  
                    optSolRow = 1; //2020.09.15 이전에 발행한 성적서에 해당
                    //optSolRow = 0; //2020.09.15 이후에취발행한 성적서에 해당 - 4351과 동일한 방식으로 결과 취합
                }

                if (P4352 == "4352" && SourceValue.Trim().Replace(" ", "").ToLower().Contains("sampledescription"))
                {
                    if (P4352_sol == 0)  //soluble heavy metals가 없을 경우
                    {
                        Lead_4352_RowNo_E = row3.Index - 1; //4352 LEAD 결과가 끝나는 줄 번호 저장
                    }
                    if (P4352_sol == 1)  //soluble heavy metals가 있을 경우
                    {
                        Sol_4352_RowNo_E = row3.Index - 1; //4352 Sol 결과가 끝나는 줄 번호 저장
                    }

                    SD_4352_RowNo_B = row3.Index + 1; //4352 Sample Description 시작되는 줄 번호 저장
                }

                // 변경됨
                if (P4352 == "4352" && SourceValue.Replace(" ", "").ToLower().Contains("note"))
                {
                    SD_4352_RowNo_E = row3.Index - 1; //4352 Sample Description 끝나는 줄 번호 저장
                    P4352_sol = 0;
                    P4352 = "";
                }

                // 4351, 4352 Method Detection Limit
                if (SourceValue.Replace(" ","").Trim().ToLower().Contains("methoddetectionlimit"))
                {
                    if (P4351 == "4351" && P4351_Lead_Pb == 1)
                    {
                        wMDL1 = (string)dgvResults.Rows[row3.Index + 1].Cells[0].Value;
                    }

                    if (P4352 == "4352" && P4352_Lead_Pb == 1)
                    {
                        wMDL2 = (string)dgvResults.Rows[row3.Index + 1].Cells[0].Value;
                    }
                }
            }

            // 2단계 - 찾은 결과 위치를 이용하여 결과 값 저장
            int ik_4351 = 0;
            if (Lead_4351_RowNo_B > 0)
            {
                for (ij = Lead_4351_RowNo_B; ij < Lead_4351_RowNo_E; ij = ij + 3)
                {
                    Lead_4351_Sam_No[ik_4351] = dgvResults.Rows[ij].Cells[0].Value.ToString();  //샘플번호
                    Lead_4351_Sam_Result[ik_4351] = dgvResults.Rows[ij + 1].Cells[0].Value.ToString(); // 샘플아이템 결과(for lead)

                    ik_4351 = ik_4351 + 1; // 배열 변수 증가
                    //ij = ij + 3; // 샘플번호, 샘플결과값, 샘플판정값의 조합이기 때문에 3을 더하여, 위치를 조정한다.
                }
            }
            else
            {
                if (Sol_4351_RowNo_B > 0)
                {
                    for (ij = Sol_4351_RowNo_B + 31; ij < Sol_4351_RowNo_E; ij = ij + 11)
                    {
                        //Sol_4351_Sam_No[ik_4351] = dgvResults.Rows[ij].Cells[0].Value.ToString();//샘플번호
                        //Sol_4351_Sam_Result1[ik_4351] = dgvResults.Rows[ij + 1].Cells[0].Value.ToString();// 샘플아이템 결과(for lead)

                        ik_4351 = ik_4351 + 1; // 배열 변수 증가
                        //ij = ij + 3; // 샘플번호, 샘플결과값, 샘플판정값의 조합이기 때문에 1을 더하여, 위치를 조정한다.
                    }
                }
            }

            int ik_4352 = 0;
            if (Lead_4352_RowNo_B > 0)
            {
                for (ij = Lead_4352_RowNo_B; ij < Lead_4352_RowNo_E; ij = ij + 3)
                {
                    Lead_4352_Sam_No[ik_4352] = dgvResults.Rows[ij].Cells[0].Value.ToString();//샘플번호
                    Lead_4352_Sam_Result[ik_4352] = dgvResults.Rows[ij + 1].Cells[0].Value.ToString();// 샘플아이템 결과(for lead)

                    ik_4352 = ik_4352 + 1; // 배열 변수 증가
                    //ij = ij + 3; // 샘플번호, 샘플결과값, 샘플판정값의 조합이기 때문에 1을 더하여, 위치를 조정한다.
                }
            }
            else
            {
                if (optSolRow == 1)
                {
                    if (Sol_4352_RowNo_B > 0)
                    {
                        for (ij = Sol_4352_RowNo_B + 41; ij < Sol_4352_RowNo_E; ij = ij + 11)
                        {
                            //Sol_4352_Sam_No[ik_4352] = dgvResults.Rows[ij].Cells[0].Value.ToString();//샘플번호
                            //Sol_4352_Sam_Result1[ik_4352] = dgvResults.Rows[ij + 1].Cells[0].Value.ToString();// 샘플아이템 결과(for lead)

                            ik_4352 = ik_4352 + 1; // 배열 변수 증가
                            //ij = ij + 3; // 샘플번호, 샘플결과값, 샘플판정값의 조합이기 때문에 1을 더하여, 위치를 조정한다.
                        }
                    }
                }
                else //if (optSolRow == 0)
                {
                    if (Sol_4352_RowNo_B > 0)
                    {
                        for (ij = Sol_4352_RowNo_B + 31; ij < Sol_4352_RowNo_E; ij = ij + 11)
                        {
                            //Sol_4352_Sam_No[ik_4352] = dgvResults.Rows[ij].Cells[0].Value.ToString();//샘플번호
                            //Sol_4352_Sam_Result1[ik_4352] = dgvResults.Rows[ij + 1].Cells[0].Value.ToString();// 샘플아이템 결과(for lead)

                            ik_4352 = ik_4352 + 1; // 배열 변수 증가
                            //ij = ij + 3; // 샘플번호, 샘플결과값, 샘플판정값의 조합이기 때문에 1을 더하여, 위치를 조정한다.
                        }
                    }
                }

            }

            // Sol 아이템 이름 저장
            if (Sol_4351_RowNo_B > 0)
            {
                ik = 0;
                for (int c = Sol_4351_RowNo_B + 2; c < Sol_4351_RowNo_B + 10; c++)
                {
                    Sol_4351_Sam_Item[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                    ik = ik + 1;
                }
            }

            // Sol 아이템 이름 저장
            if (Sol_4352_RowNo_B > 0)
            {
                ik = 0;
                for (int c = Sol_4352_RowNo_B + 2; c < Sol_4352_RowNo_B + 10; c++)
                {
                    Sol_4352_Sam_Item[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                    ik = ik + 1;
                }
            }

            // Sol 샘플 번호 저장
            if (Sol_4351_RowNo_B > 0)
            {
                ik = 0;
                for (int c = Sol_4351_RowNo_B + 32; c < Sol_4351_RowNo_E; c = c + 11)
                {
                    Sol_4351_Sam_No[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                    ik = ik + 1;
                }
            }

            // Sol 샘플 번호 저장
            if (optSolRow == 1)
            {
                if (Sol_4352_RowNo_B > 0)
                {
                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 41; c < Sol_4352_RowNo_E; c = c + 11)
                    {
                        Sol_4352_Sam_No[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }
            else //if (optSolRow == 0)
            {
                if (Sol_4352_RowNo_B > 0)
                {
                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 32; c < Sol_4352_RowNo_E; c = c + 11)
                    {
                        Sol_4352_Sam_No[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }

            // Sol 샘플 Mass_Amt 저장
            if (Sol_4351_RowNo_B > 0)
            {
                ik = 0;
                for (int c = Sol_4351_RowNo_B + 33; c < Sol_4351_RowNo_E ; c = c + 11)
                {
                    Sol_4351_Mass_Amt[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                    ik = ik + 1;
                }
            }

            // Sol 샘플 Mass_Amt 저장
            if (optSolRow == 1)
            {
                if (Sol_4352_RowNo_B > 0)
                {
                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 42; c < Sol_4352_RowNo_E ; c = c + 11)
                    {
                        Sol_4352_Mass_Amt[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }
            else //if (optSolRow == 0)
            {
                if (Sol_4352_RowNo_B > 0)
                {
                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 33; c < Sol_4352_RowNo_E ; c = c + 11)
                    {
                        Sol_4352_Mass_Amt[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }

            // Sol 샘플 결과1 저장 = 1번 아이템에 대한 결과 저장
            if (Sol_4351_RowNo_B > 0)
            {
                ik = 0;
                for (int c = Sol_4351_RowNo_B + 34; c < Sol_4351_RowNo_E ; c = c + 11)
                {
                    Sol_4351_Sam_Result1[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                    ik = ik + 1;
                }
            }

            // Sol 샘플 결과1 저장 = 1번 아이템에 대한 결과 저장
            if (optSolRow == 1)
            {
                if (Sol_4352_RowNo_B > 0)
                {

                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 43; c < Sol_4352_RowNo_E ; c = c + 11)
                    {
                        Sol_4352_Sam_Result1[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }
            else //if (optSolRow == 0)
            {
                if (Sol_4352_RowNo_B > 0)
                {

                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 34; c < Sol_4352_RowNo_E ; c = c + 11)
                    {
                        Sol_4352_Sam_Result1[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }

            // Sol 샘플 결과2 저장 = 2번 아이템에 대한 결과 저장
            if (Sol_4351_RowNo_B > 0)
            {
                ik = 0;
                for (int c = Sol_4351_RowNo_B + 35; c < Sol_4351_RowNo_E ; c = c + 11)
                {
                    Sol_4351_Sam_Result2[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                    ik = ik + 1;
                }
            }

            // Sol 샘플 결과2 저장 = 2번 아이템에 대한 결과 저장
            if (optSolRow == 1)
            {
                if (Sol_4352_RowNo_B > 0)
                {

                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 44; c < Sol_4352_RowNo_E ; c = c + 11)
                    {
                        Sol_4352_Sam_Result2[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }
            else //if (optSolRow == 0)
            {
                if (Sol_4352_RowNo_B > 0)
                {

                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 35; c < Sol_4352_RowNo_E ; c = c + 11)
                    {
                        Sol_4352_Sam_Result2[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }
            
            // Sol 샘플 결과3 저장 = 3번 아이템에 대한 결과 저장
            if (Sol_4351_RowNo_B > 0)
            {

                ik = 0;
                for (int c = Sol_4351_RowNo_B + 36; c < Sol_4351_RowNo_E ; c = c + 11)
                {
                    Sol_4351_Sam_Result3[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                    ik = ik + 1;
                }
            }

            // Sol 샘플 결과3 저장 = 3번 아이템에 대한 결과 저장
            if (optSolRow == 1)
            {
                if (Sol_4352_RowNo_B > 0)
                {

                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 45; c < Sol_4352_RowNo_E; c = c + 11)
                    {
                        Sol_4352_Sam_Result3[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }
            else //if (optSolRow == 0)
            {
                if (Sol_4352_RowNo_B > 0)
                {

                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 36; c < Sol_4352_RowNo_E ; c = c + 11)
                    {
                        Sol_4352_Sam_Result3[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }

            // Sol 샘플 결과4 저장 = 4번 아이템에 대한 결과 저장
            if (Sol_4351_RowNo_B > 0)
            {

                ik = 0;
                for (int c = Sol_4351_RowNo_B + 37; c < Sol_4351_RowNo_E; c = c + 11)
                {
                    Sol_4351_Sam_Result4[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                    ik = ik + 1;
                }
            }

            // Sol 샘플 결과4 저장 = 4번 아이템에 대한 결과 저장
            if (optSolRow == 1)
            {
                if (Sol_4352_RowNo_B > 0)
                {

                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 46; c < Sol_4352_RowNo_E ; c = c + 11)
                    {
                        Sol_4352_Sam_Result4[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }
            else //if (optSolRow == 0)
            {
                if (Sol_4352_RowNo_B > 0)
                {

                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 37; c < Sol_4352_RowNo_E ; c = c + 11)
                    {
                        Sol_4352_Sam_Result4[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }

            // Sol 샘플 결과5 저장 = 5번 아이템에 대한 결과 저장
            if (Sol_4351_RowNo_B > 0)
            {

                ik = 0;
                for (int c = Sol_4351_RowNo_B + 38; c < Sol_4351_RowNo_E ; c = c + 11)
                {
                    Sol_4351_Sam_Result5[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                    ik = ik + 1;
                }
            }

            // Sol 샘플 결과5 저장 = 5번 아이템에 대한 결과 저장
            if (optSolRow == 1)
            {
                if (Sol_4352_RowNo_B > 0)
                {

                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 47; c < Sol_4352_RowNo_E ; c = c + 11)
                    {
                        Sol_4352_Sam_Result5[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }
            else //if (optSolRow == 0)
            {
                if (Sol_4352_RowNo_B > 0)
                {

                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 38; c < Sol_4352_RowNo_E ; c = c + 11)
                    {
                        Sol_4352_Sam_Result5[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }

            // Sol 샘플 결과6 저장 = 6번 아이템에 대한 결과 저장
            if (Sol_4351_RowNo_B > 0)
            {

                ik = 0;
                for (int c = Sol_4351_RowNo_B + 39; c < Sol_4351_RowNo_E ; c = c + 11)
                {
                    Sol_4351_Sam_Result6[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                    ik = ik + 1;
                }
            }

            // Sol 샘플 결과6 저장 = 6번 아이템에 대한 결과 저장
            if (optSolRow == 1)
            {

                if (Sol_4352_RowNo_B > 0)
                {

                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 48; c < Sol_4352_RowNo_E ; c = c + 11)
                    {
                        Sol_4352_Sam_Result6[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }
            else //if (optSolRow == 0)
            {

                if (Sol_4352_RowNo_B > 0)
                {

                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 39; c < Sol_4352_RowNo_E ; c = c + 11)
                    {
                        Sol_4352_Sam_Result6[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }

            // Sol 샘플 결과7 저장 = 7번 아이템에 대한 결과 저장
            if (Sol_4351_RowNo_B > 0)
            {

                ik = 0;
                for (int c = Sol_4351_RowNo_B + 40; c < Sol_4351_RowNo_E ; c = c + 11)
                {
                    Sol_4351_Sam_Result7[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                    ik = ik + 1;
                }
            }

            // Sol 샘플 결과7 저장 = 7번 아이템에 대한 결과 저장
            if (optSolRow == 1)
            {
                if (Sol_4352_RowNo_B > 0)
                {
                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 49; c < Sol_4352_RowNo_E; c = c + 11)
                    {
                        Sol_4352_Sam_Result7[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }
            else //if (optSolRow == 0)
            {
                if (Sol_4352_RowNo_B > 0)
                {
                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 40; c < Sol_4352_RowNo_E; c = c + 11)
                    {
                        Sol_4352_Sam_Result7[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }

            // Sol 샘플 결과8 저장 = 8번 아이템에 대한 결과 저장
            if (Sol_4351_RowNo_B > 0)
            {
                ik = 0;
                for (int c = Sol_4351_RowNo_B + 41; c < Sol_4351_RowNo_E; c = c + 11)
                {
                    Sol_4351_Sam_Result8[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                    ik = ik + 1;
                }
            }
            // Sol 샘플 결과8 저장 = 8번 아이템에 대한 결과 저장
            if (optSolRow == 1)
            {
                if (Sol_4352_RowNo_B > 0)
                {

                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 50; c < Sol_4352_RowNo_E ; c = c + 11)
                    {
                        Sol_4352_Sam_Result8[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }
            else //if (optSolRow == 0)
            {
                if (Sol_4352_RowNo_B > 0)
                {

                    ik = 0;
                    for (int c = Sol_4352_RowNo_B + 41; c < Sol_4352_RowNo_E ; c = c + 11)
                    {
                        Sol_4352_Sam_Result8[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                        ik = ik + 1;
                    }
                }
            }

            //SD_4351_Sam_No
            if (SD_4351_RowNo_B > 0)
            {
                ik = 0;
                for (int c = SD_4351_RowNo_B; c < SD_4351_RowNo_E; c = c + 1)
                {
                    SD_4351_Sam_No[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                    ik = ik + 1;
                }
            }

            //SD_4352_Sam_No
            if (SD_4352_RowNo_B > 0)
            {
                ik = 0;
                for (int c = SD_4352_RowNo_B; c <= SD_4352_RowNo_E ; c = c + 1)
                {
                    SD_4352_Sam_No[ik] = dgvResults.Rows[c].Cells[0].Value.ToString();
                    ik = ik + 1;
                }
            }

            //3단계 - 저장된 값들을 원하는 위치에 배치

            dgvFinalMerge.Rows.Clear();

            dgvFinalMerge.RowCount = 20;
            dgvFinalMerge.ColumnCount = 50;

            for (int c = 0; c < ik_4351 + ik_4352; c++)
            {
                dgvFinalMerge.Rows[c].Cells[0].Value = sItem_Number; //Item No
                dgvFinalMerge.Rows[c].Cells[1].Value = wJob_StyleNo; //Related Item No
                dgvFinalMerge.Rows[c].Cells[2].Value = wJob_SamDesc;  //Description                

                if (c < ik_4351)
                {
                    dgvFinalMerge.Rows[c].Cells[3].Value = Lead_4351_Sam_No[c].Replace(".","") ; //Sample No - 4351
                    dgvFinalMerge.Rows[c].Cells[4].Value = SD_4351_Sam_No[c]; //Sample Description - 4351
                }
                else
                {
                    //dgvFinalMerge.Rows[c].Cells[3].Value = Lead_4352_Sam_No[c - ik_4351].Replace(".","") ; //Sample No -4352
                    dgvFinalMerge.Rows[c].Cells[3].Value = SD_4352_Sam_No[c - ik_4351].ToString().Substring(0,1).Trim(); //Sample No -4352
                    dgvFinalMerge.Rows[c].Cells[4].Value = SD_4352_Sam_No[c - ik_4351].ToString().Substring(3).Trim(); //Sample Description -4352
                }
                //dgvFinalMerge.Rows[c].Cells[4].Value = wJob_SamDesc2;  //Description
                dgvFinalMerge.Rows[c].Cells[5].Value = sPartName;
                dgvFinalMerge.Rows[c].Cells[6].Value = sMaterialName;
                dgvFinalMerge.Rows[c].Cells[7].Value = wJob_FileNo;
                dgvFinalMerge.Rows[c].Cells[8].Value = wJob_IssuedDate;

                if (c < ik_4351)
                {
                    dgvFinalMerge.Rows[c].Cells[9].Value = wMDL1;
                    dgvFinalMerge.Rows[c].Cells[10].Value = Lead_4351_Sam_Result[c];
                    dgvFinalMerge.Rows[c].Cells[11].Value = Sol_4351_Mass_Amt[c];
                    dgvFinalMerge.Rows[c].Cells[12].Value = Sol_4351_Sam_Result1[c];
                    dgvFinalMerge.Rows[c].Cells[13].Value = Sol_4351_Sam_Result2[c];
                    dgvFinalMerge.Rows[c].Cells[14].Value = Sol_4351_Sam_Result3[c];
                    dgvFinalMerge.Rows[c].Cells[15].Value = Sol_4351_Sam_Result4[c];
                    dgvFinalMerge.Rows[c].Cells[16].Value = Sol_4351_Sam_Result5[c];
                    dgvFinalMerge.Rows[c].Cells[17].Value = Sol_4351_Sam_Result6[c];
                    dgvFinalMerge.Rows[c].Cells[18].Value = Sol_4351_Sam_Result7[c];
                    dgvFinalMerge.Rows[c].Cells[19].Value = Sol_4351_Sam_Result8[c];
                }
                else
                {
                    dgvFinalMerge.Rows[c].Cells[20].Value = wMDL2;
                    dgvFinalMerge.Rows[c].Cells[21].Value = Lead_4352_Sam_Result[c - ik_4351];
                    dgvFinalMerge.Rows[c].Cells[22].Value = Sol_4352_Mass_Amt[c - ik_4351];
                    dgvFinalMerge.Rows[c].Cells[23].Value = Sol_4352_Sam_Result1[c - ik_4351];
                    dgvFinalMerge.Rows[c].Cells[24].Value = Sol_4352_Sam_Result2[c - ik_4351];
                    dgvFinalMerge.Rows[c].Cells[25].Value = Sol_4352_Sam_Result3[c - ik_4351];
                    dgvFinalMerge.Rows[c].Cells[26].Value = Sol_4352_Sam_Result4[c - ik_4351];
                    dgvFinalMerge.Rows[c].Cells[27].Value = Sol_4352_Sam_Result5[c - ik_4351];
                    dgvFinalMerge.Rows[c].Cells[28].Value = Sol_4352_Sam_Result6[c - ik_4351];
                    dgvFinalMerge.Rows[c].Cells[29].Value = Sol_4352_Sam_Result7[c - ik_4351];
                    dgvFinalMerge.Rows[c].Cells[30].Value = Sol_4352_Sam_Result8[c - ik_4351];
                }
            }
            dgvFinalMerge.RowCount = ik_4351 + ik_4352 + 1 ;
        }

        private void Merge_Step3()
        {
            int wRow = 1;
            int RowChk, RowNo;

            int ij = 0;
            int SampleTot;

            string sRptName;
            string sIssuedDate;

            string wIssuedDate = "";
            string wSamDesc = "";
            string wStyleNo = "";
            string wFileNo = "";
            string wMDL1 = "";
            string wMDL2 = "";

            string SourceValue, TargetValue, PreviousValue, AnalyteName;
            string P4351;
            string P4352;
            int JobNoChk, JobNoRow, TargetRowNo, TargetColNo, SamResultchk, SampleCnt, SaveSampleCnt, SaveCurRow, i, j, ColNoChk;
            int SampleTotCnt, JobNoRow2;
            int[] nCol = new int[100];
            int[] wCol = new int[100];
            string[] nColValue = new string[100];
            int P4351_Lead_Pb = 0;
            int P4352_Lead_Pb = 0;
            int P4351_sol = 0;
            int P4352_sol = 0;
            int ik = 0;

            dgvFinal.Rows.Clear();
            //dgvFinal.RowCount;  
            dgvFinalMerge.Rows.Clear();
            Initialize_Grid();


            AnalyteName = "";

            JobNoChk = 0;
            JobNoRow = 0;
            JobNoRow = 2;

            TargetRowNo = 0;
            TargetColNo = 0;
            SamResultchk = 0;
            ColNoChk = 2;
            SampleCnt = 0;
            SaveSampleCnt = 0;
            SaveCurRow = 0;
            SampleTotCnt = 0;
            i = 0;
            j = 0;

            P4351 = "";
            P4352 = "";

            SourceValue = "";
            TargetValue = "";
            PreviousValue = "";


        }

        private void btnCombine_Click_1(object sender, EventArgs e)
        {
            Document documentMerge = new Document();
            Document document = new Document();

            string mrgRpt1, mrgRpt2;

            mrgRpt1 = "";
            mrgRpt2 = "";

            foreach (DataGridViewRow row2 in dgvMergeFiles.Rows)
            {
                if (row2.Index <= dgvMergeFiles.Rows.Count - 2)
                {
                    if ((string)row2.Cells[11].Value == "Part 1,2")
                    {
                        if ((string)row2.Cells[12].Value.ToString().Trim() != "")
                        {
                            mrgRpt1 = (string)row2.Cells[12].Value;

                            cboPattern.Text = "*" + mrgRpt1 + "*.*";
                            SearchForFiles(lstFiles, txtStartDirectory.Text, cboPattern.Text, txtFind.Text, null);
                            if (lstFiles.Items.Count > 0)
                            {
                                lstFiles.SetSelected(0, true);
                                //mrgRpt1 = lstFiles.SelectedItems[0].ToString();
                                //Create word document
                                document.LoadFromFile(lstFiles.SelectedItems[0].ToString(),  Spire.Doc.FileFormat.Doc);
                            }
                            else
                            {
                                MessageBox.Show(mrgRpt1 + " 파일확인");
                                return;
                            }

                        }
                    }

                    if ((string)row2.Cells[11].Value == "Part 3")
                    {
                        if ((string)row2.Cells[12].Value.ToString().Trim() != "")
                        {
                            mrgRpt2 = (string)row2.Cells[12].Value;

                            cboPattern.Text = "*COMBINE_#" + mrgRpt2 + "*.*";
                            SearchForFiles(lstFiles, txtStartDirectory.Text, cboPattern.Text, txtFind.Text, null);
                            if (lstFiles.Items.Count > 0)
                            {
                                lstFiles.SetSelected(0, true);
                                //mrgRpt2 = lstFiles.SelectedItems[0].ToString();
                                documentMerge.LoadFromFile(lstFiles.SelectedItems[0].ToString(),  Spire.Doc.FileFormat.Doc);

                                foreach (Section sec in documentMerge.Sections)
                                {
                                    document.Sections.Add(sec.Clone());
                                }

                                //Save doc file.
                                document.SaveToFile(mrgRpt2 + "_Sample.doc",  Spire.Doc.FileFormat.Doc);

                                //Launching the MS Word file.
                                WordDocViewer(mrgRpt2 + "_Sample.doc");
                            }

                            else
                            {
                                MessageBox.Show(mrgRpt2 + " 파일확인");
                                return;
                            }

                        }

                    }

                }
            }

        }

        private void button17_Click_1(object sender, EventArgs e)
        {
            dgvMergeFiles.Rows.Clear();
            dgvFinal.Rows.Clear();
        }

        private void dgvMergeFiles_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 11)
            {
                string wRptFileName, plainText2;
                object value = dgvMergeFiles.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (value is DBNull) { return; }
                if (value.ToString().Trim().ToUpper().IndexOf(".DOC") < 1) { return; }

                wRptFileName = value.ToString();

                MessageBox.Show(wRptFileName);

                Document document = new Document();
                document.LoadFromFile(@wRptFileName);

                plainText2 = document.GetText();
                this.textBox3.Text = plainText2;

                WordDocViewer(wRptFileName);
            }

            if (e.ColumnIndex == 12)
            {
                object value1 = dgvMergeFiles.Rows[e.RowIndex - 1].Cells[e.ColumnIndex].Value;
                object value2 = dgvMergeFiles.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                if (value1 is DBNull || value2 is DBNull)
                {
                    MessageBox.Show("성적서 이름 확인 바람");
                    return;
                }

                if (value1.ToString().Trim() == "" || value2.ToString().Trim() == "")
                {
                    MessageBox.Show("성적서 이름 확인 바람");
                    return;
                }
                Combine_Rpt(value1.ToString(), value2.ToString());
            }
        }

        private void txtStartDirectory_TextChanged_1(object sender, EventArgs e)
        {
            txtDir1.Text = txtStartDirectory.Text;
        }

    }

}