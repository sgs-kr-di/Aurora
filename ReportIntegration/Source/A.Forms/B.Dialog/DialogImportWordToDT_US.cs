using System;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace Sgs.ReportIntegration.Source.A.Forms.B.Dialog
{
    public partial class frmImportWord : Form
    {
        //DT1
        DataTable dtSet1;
        //DT2
        DataTable dtSet2;
        //DT3
        DataTable dtSet3;
        //DT4
        DataTable dtSet4;
        //DT5
        DataTable dtSet5;
        //DT6
        DataTable dtSet6;

        public frmImportWord()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            dtSet1 = new DataTable();
            dtSet2 = new DataTable();
            dtSet3 = new DataTable();
            dtSet4 = new DataTable();
            dtSet5 = new DataTable();
            dtSet6 = new DataTable();
        }

        private DataRow Readvaluefordt1(Table cellTable, DataRow dtr)
        {
            //get nested table

            for (int c = 0; c < 6; c++)
            {
                for (int n = 0; n < cellTable.Rows[c].Cells[1].Paragraphs.Count; n++)
                {
                    Paragraph p = cellTable.Rows[c].Cells[1].Paragraphs[n];
                    if (n == 0)
                    {
                        dtr[c] += p.Text;

                    }
                    else
                    { dtr[c] += "\n" + p.Text; }
                }

            }

            return dtr;

        }
        private DataRow Readvaluefordt1_2(Table cellTable, DataRow dtr)
        {
            //get nested table


            for (int n = 0; n < cellTable.Rows[0].Cells[1].Paragraphs.Count; n++)
            {
                Paragraph p = cellTable.Rows[0].Cells[1].Paragraphs[n];
                if (n == 0)
                {
                    dtr[6] += p.Text;

                }
                else
                { dtr[6] += "\n" + p.Text; }
            }



            return dtr;

        }


        private DataRow Readvaluefordt2(TableRow row, DataRow dtrow)
        {
            //번호 설명
            for (int c = 0; c < 2; c++)
            {
                for (int n = 0; n < row.Cells[c].Paragraphs.Count; n++)
                {
                    Paragraph p = row.Cells[c].Paragraphs[n];

                    if (n == 0)
                    {
                        dtrow[c] += p.Text;
                    }
                    else
                    {
                        dtrow[c] += "\n" + p.Text;
                    }
                }

            }

            //결과
            foreach (Paragraph p in row.Cells[2].Paragraphs)
            {
                if (p.Text.ToUpper().Trim() == "P")
                {
                    dtrow[2] = "PASS";
                }
                else if (p.Text.ToUpper().Trim() == "F")
                {
                    dtrow[2] = "FAIL";
                }
                else if (p.Text.ToUpper().Trim() == "Y")
                {
                    dtrow[2] = "YES";
                }
                else if (p.Text.ToUpper().Trim() == "N")
                {
                    dtrow[2] = "NO";
                }
                else if (p.Text.ToUpper().Trim() == "SR")
                {
                    dtrow[2] = "See Remark";
                }
                else if (p.Text.ToUpper().Trim() == "RE 1")
                {
                    dtrow[2] = "Remark";
                    dtrow[3] = "Any toy or game that is intended for use by children who are at least three years old (36 months) but less than six years of age (72 months) and includes a small part is subject to the labeling requirements in accordance with 5.11.2.";
                }
                else if (p.Text.ToUpper().Trim() == "RE 2")
                {
                    dtrow[2] = "Remark";
                    dtrow[3] = "Toys containing non-replaceable batteries shall be labeled in accordance with 5.15.";
                }
                else if (p.Text.ToUpper().Trim() == "RE 3")
                {
                    dtrow[2] = "Remark";
                    dtrow[3] = "Toys with non-replaceable batteries that are accessible with the use of a coin, screwdriver, or other common household tool shall bear a statement that the battery is not replaceable";

                }
                else if (p.Text.ToUpper().Trim() == "RE 4")
                {
                    dtrow[2] = "Remark";
                    dtrow[3] = "The toy or package should be age labeled";
                }
                else if (p.Text.ToUpper().Trim() == "RE 5")
                {
                    dtrow[2] = "Remark";
                    dtrow[3] = "It is drawn to your attention that the toy or its packaging shall be marked with appropriate small part warning in accordance with 16 CFR 1500.19";
                }
                else if (p.Text.ToUpper().Trim() == "RE 6")
                {
                    dtrow[2] = "Remark";
                    dtrow[3] = "The toy should be marked with name and address of the producer or the distributor";
                }
                else if (p.Text.ToUpper().Trim() == "RE 7")
                {
                    dtrow[2] = "Remark";
                    dtrow[3] = "Washing was conducted in one trial as per client’s request";
                }
                else
                {
                    dtrow[2] += p.Text;
                }
            }
            return dtrow;
        }

        private DataRow Readvaluefordt3(TableRow row, DataRow dtrow)
        {
            for (int c = 0; c < 2; c++)
            {
                for (int n = 0; n < row.Cells[c].Paragraphs.Count; n++)
                {
                    Paragraph p = row.Cells[c].Paragraphs[n];

                    if (n == 0)
                    {
                        dtrow[c] += p.Text;

                    }
                    else
                    {
                        dtrow[c] += "\n" + p.Text;
                    }

                }


            }

            return dtrow;


        }


        private DataRow Readvaluefordt4(DataRow dtr, TableRow row)
        {
            for (int c = 0; c < 11; c++)
            {

                for (int n = 0; n < row.Cells[c].Paragraphs.Count; n++)
                {
                    Paragraph p = row.Cells[c].Paragraphs[n];
                    if (n == 0)
                    {
                        dtr[c] += p.Text;

                    }
                    else
                    {
                        dtr[c] += "\n" + p.Text;
                    }

                }

            }


            return dtr;
        }


        private DataRow Readvaluefordt5(Table cellTable, DataRow dtr)
        {
            //get nested table

            for (int n = 0; n < cellTable.Rows[0].Cells[0].Paragraphs.Count; n++)
            {
                Paragraph p = cellTable.Rows[0].Cells[0].Paragraphs[n];
                if (n == 0)
                { dtr[0] += p.Text; }
                else
                { dtr[0] += "\n" + p.Text; }


            }

            return dtr;
        }
        private DataTable getdt1(Document document)
        {  //DT1
            DataTable dt1 = new DataTable();
            dt1.Locale = CultureInfo.InvariantCulture;
            dt1.Columns.Add(new DataColumn("Report Job No.", typeof(string)));
            dt1.Columns.Add(new DataColumn("Labeled Age Grading", typeof(string)));
            dt1.Columns.Add(new DataColumn("Requested Age Grading", typeof(string)));
            dt1.Columns.Add(new DataColumn("Age Group Applied in Testing", typeof(string)));
            dt1.Columns.Add(new DataColumn("Sample description", typeof(string)));
            dt1.Columns.Add(new DataColumn("Detail of sample", typeof(string)));
            dt1.Columns.Add(new DataColumn("Report Comments", typeof(string)));

            foreach (Section sec in document.Sections)
            {
                for (int i = 0; i < sec.Tables.Count; i++)
                {
                    Table table = (Table)sec.Tables[i];

                    foreach (TableRow tableRow in table.Rows)
                    {
                        foreach (TableCell tableCell in tableRow.Cells)
                        {
                            foreach (DocumentObject documentObject in tableCell.ChildObjects)
                            {
                                if (documentObject.DocumentObjectType == DocumentObjectType.Table)
                                {
                                    Table cellTable = (Table)documentObject;
                                    if (cellTable.Rows[0].Cells[0].Paragraphs[0].Text.Trim() == "Report Job No.")
                                    {
                                        DataRow dtr = dt1.Rows.Add();
                                        dtr = Readvaluefordt1(cellTable, dtr);
                                    }

                                    if (cellTable.Rows[0].Cells[0].Paragraphs[0].Text.Trim() == "Report Comments")
                                    {

                                        Readvaluefordt1_2(cellTable, dt1.Rows[0]);
                                    }


                                }
                            }

                        }
                    }


                }

            }


            return dt1;

        }

        private DataTable getdt2(Document document)
        {
            DataTable dt2 = new DataTable();
            dt2.Locale = CultureInfo.InvariantCulture;
            dt2.Columns.Add(new DataColumn("Clause", typeof(string)));
            dt2.Columns.Add(new DataColumn("Description", typeof(string)));
            dt2.Columns.Add(new DataColumn("Result", typeof(string)));
            dt2.Columns.Add(new DataColumn("Remark", typeof(string)));
            foreach (Section sec in document.Sections)
            {
                for (int i = 0; i < sec.Tables.Count; i++)
                {
                    Table table = (Table)sec.Tables[i];

                    foreach (TableRow row in table.Rows)
                    {
                        if (row.Cells.Count > 2)
                        {
                            DataRow dtrow = dt2.Rows.Add();

                            if (row.Cells[0].Paragraphs[0].ChildObjects.Count > 0)
                            {
                                TextRange range = row.Cells[0].Paragraphs[0].ChildObjects[0] as TextRange;
                                if (range != null)
                                {
                                    if (range.CharacterFormat.Bold == true && row.Cells.Count == 3)
                                    {

                                        dtrow = Readvaluefordt2(row, dtrow);

                                    }



                                }


                            }
                        }
                    }
                }
            }

            if (dt2.AsEnumerable().Where(r => r.IsNull(0) == false).Where(r => r[2].ToString().ToUpper().Trim() != "NA").Count() > 0)
            {
                dt2 = dt2.AsEnumerable()
                   .Where(r => r.IsNull(0) == false)
                   .Where(r => r[2].ToString().ToUpper().Trim() != "NA")
                   .CopyToDataTable();
            }
            else
            {
                dt2.Clear();
            }

            return dt2;

        }

        private DataTable getdt3(Document document)
        {
            //dt3-result summary
            //DT3
            DataTable dt3 = new DataTable();
            dt3.Locale = CultureInfo.InvariantCulture;
            dt3.Columns.Add(new DataColumn("Test Requested", typeof(string)));
            dt3.Columns.Add(new DataColumn("Conclusion", typeof(string)));

            foreach (Section sec in document.Sections)
            {
                for (int i = 0; i < sec.Tables.Count; i++)
                {
                    Table table = (Table)sec.Tables[i];

                    foreach (TableRow tableRow in table.Rows)
                    {
                        foreach (TableCell tableCell in tableRow.Cells)
                        {
                            foreach (DocumentObject documentObject in tableCell.ChildObjects)
                            {
                                if (documentObject.DocumentObjectType == DocumentObjectType.Table)
                                {
                                    //get nested table
                                    Table cellTable = (Table)documentObject;
                                    if (cellTable.Rows[0].Cells[0].Paragraphs[0].Text.Trim() == "Result Summary")
                                    {
                                        foreach (TableRow row in cellTable.Rows)
                                        {
                                            if (row.GetRowIndex() == 0 || row.GetRowIndex() == 1)
                                            {
                                                continue;

                                            }

                                            DataRow dtrow = dt3.Rows.Add();

                                            dtrow = Readvaluefordt3(row, dtrow);

                                        }

                                    }

                                }
                            }

                        }
                    }


                }
            }

            if (dt3.AsEnumerable().Where(r => r.IsNull(0) == false).Where(r => r[1].ToString().ToUpper().Trim() != "NA").Count() > 0)
            {
                dt3 = dt3.AsEnumerable()
               .Where(r => r.IsNull(0) == false).Where(r => r[1].ToString().ToUpper().Trim() != "NA")
               .CopyToDataTable();
            }
            else
            {
                dt3.Rows.Clear();
            }




            return dt3;

        }

        private DataTable getdt4(Document document)
        {   //DT4 -- sample table
            DataTable dt4 = new DataTable();
            dt4.Locale = CultureInfo.InvariantCulture;
            dt4.Columns.Add(new DataColumn("Sample", typeof(string)));
            dt4.Columns.Add(new DataColumn("Sample Length (mm)", typeof(string)));
            dt4.Columns.Add(new DataColumn("Sample Length (in.) ", typeof(string)));
            dt4.Columns.Add(new DataColumn("Ignition Point", typeof(string)));
            dt4.Columns.Add(new DataColumn("Burnt Length(mm)", typeof(string)));
            dt4.Columns.Add(new DataColumn("Burnt Length(in.)", typeof(string)));
            dt4.Columns.Add(new DataColumn("Time(s)", typeof(string)));
            dt4.Columns.Add(new DataColumn("Actual Burn Rate (in./s)", typeof(string)));
            dt4.Columns.Add(new DataColumn("Round up* Burn Rate (in./s)", typeof(string)));
            dt4.Columns.Add(new DataColumn("Burnning Rate (in./s))", typeof(string)));
            dt4.Columns.Add(new DataColumn("Result", typeof(string)));
            bool hasStrikeOut = false;

            foreach (Section sec in document.Sections)
            {
                for (int i = 0; i < sec.Tables.Count; i++)
                {
                    Table table = (Table)sec.Tables[i];
                    if (table.Rows[0].Cells[0].Paragraphs[0].Text.Trim().Contains("Note 1. Clause 4.2 Flammability test of toys (16 CFR 1500.44 / ASTM F963)"))
                    {
                        foreach (TableRow row in table.Rows)
                        {
                            if (row.GetRowIndex() == 0 || row.GetRowIndex() == 1 || row.GetRowIndex() == 2)
                            {
                                continue;
                            }
                            if (row.Cells.Count != 11)
                            {
                                break;
                            }

                            //check if pr in sample cell has a line through a text
                            foreach (var obj in row.Cells[0].Paragraphs[0].ChildObjects)
                            {
                                if (obj is TextRange)
                                {
                                    var tr = obj as TextRange;
                                    //remove text with strike through
                                    if (tr.CharacterFormat.IsStrikeout)
                                    {
                                        hasStrikeOut = true;
                                        break;
                                    }
                                }

                            }

                            if (hasStrikeOut == false)
                            {
                                DataRow dtr = dt4.Rows.Add();
                                dtr = Readvaluefordt4(dtr, row);

                            }
                            hasStrikeOut = false;


                        }


                    }
                }
            }


            if (dt4.AsEnumerable().Where(r => r.IsNull(0) == false && r[0].ToString() != "").Where(r => r[9].ToString().ToUpper().Trim() != "NA").Count() > 0)
            {
                dt4 = dt4.AsEnumerable()
                    .Where(r => r.IsNull(0) == false && r[0].ToString() != "").Where(r => r[9].ToString().ToUpper().Trim() != "NA")
                    .CopyToDataTable();
            }
            else
            {
                dt4.Rows.Clear();
            }
            return dt4;

        }

        private DataTable getdt5(Document document)
        {  //-	What kinds of stuffing
            DataTable dt5 = new DataTable();
            dt5.Locale = CultureInfo.InvariantCulture;
            dt5.Columns.Add(new DataColumn("Stuffing materials", typeof(string)));
            foreach (Section sec in document.Sections)
            {
                for (int i = 0; i < sec.Tables.Count; i++)
                {
                    Table table = (Table)sec.Tables[i];

                    foreach (TableRow tableRow in table.Rows)
                    {
                        foreach (TableCell tableCell in tableRow.Cells)
                        {
                            if (tableCell.Paragraphs[0].Text.Trim() == "a) Stuffing materials")
                            {
                                foreach (DocumentObject documentObject in tableCell.ChildObjects)
                                {
                                    if (documentObject.DocumentObjectType == DocumentObjectType.Table)
                                    {
                                        Table cellTable = (Table)documentObject;
                                        DataRow dtr = dt5.Rows.Add();
                                        dtr = Readvaluefordt5(cellTable, dtr);


                                    }
                                }



                            }
                        }

                    }
                }


            }


            return dt5;


        }



        private DataTable getdt6(Document document)
        {
            //PAGE COMPLETE
            DataTable dt6 = new DataTable();
            dt6.Locale = CultureInfo.InvariantCulture;
            dt6.Columns.Add(new DataColumn("COMPLETE_실무자", typeof(string)));
            dt6.Columns.Add(new DataColumn("COMPLETE_기술책임자", typeof(string)));
            Table tb = (Table)document.Sections[document.Sections.Count - 1].Tables[document.Sections[document.Sections.Count - 1].Tables.Count - 1];

            DataRow drow = dt6.Rows.Add();

            //실무자
            foreach (Paragraph p in tb.Rows[2].Cells[1].Paragraphs)
            {
                drow[0] += p.Text;

            }

            //기술책임자

            foreach (Paragraph p in tb.Rows[2].Cells[tb.Rows[2].Cells.Count - 1].Paragraphs)
            {
                drow[1] += p.Text;

            }

            return dt6;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox1.Text == null)
            {
                MessageBox.Show("파일을 선택해주세요.");
            }

            Document document = new Document();
            document.LoadFromFile(textBox1.Text);

            DataTable dt1 = getdt1(document);
            DataTable dt2 = getdt2(document);
            DataTable dt3 = getdt3(document);
            DataTable dt4 = getdt4(document);
            DataTable dt5 = getdt5(document);
            DataTable dt6 = getdt6(document);

            dtSet1 = dt1;
            dtSet2 = dt2;
            dtSet3 = dt3;
            dtSet4 = dt4;
            dtSet5 = dt5;
            dtSet6 = dt6;

            //MessageBox.Show("변환이 완료되었습니다!");
            Close();
        }

        public DataTable GetVariable1()
        {
            return dtSet1;
        }

        public DataTable GetVariable2()
        {
            return dtSet2;
        }

        public DataTable GetVariable3()
        {
            return dtSet3;
        }

        public DataTable GetVariable4()
        {
            return dtSet4;
        }

        public DataTable GetVariable5()
        {
            return dtSet5;
        }

        public DataTable GetVariable6()
        {
            return dtSet6;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        //private string nestedtable(DocumentObject documentObject, string str)
        //{
        //    //get nested table
        //    Table cellTable = (Table)documentObject;
        //    for (int m = 0; m < cellTable.Rows.Count; m++)
        //    {
        //        for (int n = 0; n < cellTable.Rows[m].Cells.Count; n++)
        //        {
        //            foreach (DocumentObject doc in cellTable.Rows[m].Cells[n].ChildObjects)
        //            {
        //                if (doc.DocumentObjectType == DocumentObjectType.Table)
        //                {
        //                    nestedtable(doc, str);
        //                }
        //            }

        //            for (int p = 0; p < cellTable.Rows[m].Cells[n].Paragraphs.Count; p++)
        //            {
        //                Paragraph pr = cellTable.Rows[m].Cells[n].Paragraphs[p];
        //                str += "\n" + pr.Text;
        //            }
        //        }
        //    }
        //    return str;
        //}
    }
}
