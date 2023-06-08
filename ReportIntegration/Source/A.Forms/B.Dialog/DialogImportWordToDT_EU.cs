using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System.Globalization;

namespace Sgs.ReportIntegration.Source.A.Forms.B.Dialog
{
    public partial class DialogImportWordToDT_EU : Form
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
        DataTable dtSet5_1;
        //DT5
        DataTable dtSet5_2;
        //DT6
        DataTable dtSet6;
        //DT7
        DataTable dtSet7;
        //DT8
        DataTable dtSet8;
        //DT9
        DataTable dtSet9;
        //DT10
        DataTable dtSet10;

        public bool bChkToDT { get; set; }

        public DialogImportWordToDT_EU()
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
            dtSet5_1 = new DataTable();
            dtSet5_2 = new DataTable();
            dtSet6 = new DataTable();
            dtSet7 = new DataTable();
            dtSet8 = new DataTable();
            dtSet9 = new DataTable();
            dtSet10 = new DataTable();
            bChkToDT = false;
        }

        Dictionary<string, string> remark_dict = new Dictionary<string, string>();
        
        string[,] titleArr = new string[,]
        {
            {"4.1","4.1 General requirements"},
            {"4.2","4.2 Toys to be worn on the head" },
            {"4.2.1","4.2.1 General" },
            {"4.2.2","4.2.2 Beards, moustaches, wigs, etc., made from pile or flowing elements which protrude 50 mm or more from the surface of the toy" },
            {"4.2.3","4.2.3 Beards, moustaches, wigs, etc., made from pile or flowing elements which protrude less than 50 mm from the surface of the toy"},
            {"4.2.4","4.2.4 Full or partial moulded head masks"},
            {"4.2.5","4.2.5 Flowing elements of toys to be worn on the head (except those covered by 4.2.2 and 4.2.3), hoods, head-dresses etc. and fabric masks which partially or fully cover the head, but excluding those items covered by 4.3" },
            {"4.3","4.3 Toy disguise costumes and toys intended to be worn by a child in play"},
            {"4.4","4.4 Toys designed to be entered by a child" },
            {"4.5","4.5 Soft-filled toys "}
        };

        //string[,] remarks = new string[,]
        //{   {"RE1","Remark: Small part(s) was/were found. It is acceptable because appropriate age w is found on packaging." },
        //    {"RE2","Remark: Small part(s) was/were found after test. It is acceptable because appropriate age warning is found on packaging." },
        //    {"RE3","Remark: Small part(s) was/were found. Therefore, the toy should be warned in accordance with 7.2." },
        //    {"RE4","Remark: Small parts were found before and after abuse test. But, Age warning should not be required for toy manifestly unsuitable for children under 3 years / due to this is not a toy"},
        //    {"RE5","Remark: No small part was found before and after abuse test." },
        //    {"RE6","Remark: No small part was found before and after test. Therefore, small part warning should not be shown on the packaging." },
        //    {"RE7","Remark: In order to comply with the requirements of this clause, the toy or its packaging must be marked with “WARNING! Not suitable for children under 36 months. Small part.”"},
        //    {"RE8", "Remark: Age warning should not be required for toy manifestly unsuitable for children under 36 months / due to this is not a toy."}
        //};

        private DataTable Getdt1(Document document)
        {
            //DT1
            DataTable dt1 = new DataTable();
            dt1.Locale = CultureInfo.InvariantCulture;
            dt1.Columns.Add(new DataColumn("Report Job No.", typeof(string)));
            dt1.Columns.Add(new DataColumn("Labeled Age Grading", typeof(string)));
            dt1.Columns.Add(new DataColumn("Requested Age Grading", typeof(string)));
            dt1.Columns.Add(new DataColumn("Age Group Applied in Testing", typeof(string)));
            dt1.Columns.Add(new DataColumn("Sample description", typeof(string)));
            dt1.Columns.Add(new DataColumn("Detail of sample", typeof(string)));
            dt1.Columns.Add(new DataColumn("Report comments", typeof(string)));

            foreach (Section sec in document.Sections)
            {
                for (int i = 0; i < sec.Tables.Count; i++)
                {
                    Table table = (Table)sec.Tables[i];
                    if (table.Rows[0].Cells[0].Paragraphs[0].Text.Trim().ToLower().Replace(" ", "") == "Report Job No.".Trim().ToLower().Replace(" ", ""))
                    {
                        DataRow dtr = dt1.Rows.Add();
                        dtr = Readvaluefordt1(table, dtr);
                    }
                }
            }

            //report comment
            Table rctb = GetTableByTitlte(document, "reportcomment");
            if (dt1.Rows.Count > 0 && rctb != null)
            {
                string prtext = "";
                foreach (Paragraph pr in rctb.Rows[0].Cells[1].Paragraphs)
                {
                    prtext += pr.Text;
                }
                dt1.Rows[0][6] = prtext;
            }

            return dt1;
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

        private DataTable Getdt2(Document document)
        {
            //dt3-result summary
            //DT3
            DataTable dt2 = new DataTable();
            dt2.Locale = CultureInfo.InvariantCulture;
            dt2.Columns.Add(new DataColumn("Test Requested", typeof(string)));
            dt2.Columns.Add(new DataColumn("Conclusion", typeof(string)));

            foreach (Section sec in document.Sections)
            {
                for (int i = 0; i < sec.Tables.Count; i++)
                {
                    Table table = (Table)sec.Tables[i];
                    if (table.Rows[0].Cells[0].Paragraphs[0].Text.Trim().ToLower().Replace(" ", "") == "Result Summary".Trim().ToLower().Replace(" ", ""))
                    {
                        foreach (TableRow row in table.Rows)
                        {
                            if (row.GetRowIndex() < 2)
                            {
                                continue;
                            }

                            DataRow dtrow = dt2.Rows.Add();

                            dtrow = Readvaluefordt2(row, dtrow);
                        }
                    }
                }
            }

            return dt2;
        }

        private DataRow Readvaluefordt2(TableRow row, DataRow dtrow)
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

        private DataTable Getdt3(Document document)
        {
            DataTable dt3 = new DataTable();
            dt3.Locale = CultureInfo.InvariantCulture;
            dt3.Columns.Add(new DataColumn("Clause", typeof(string)));
            dt3.Columns.Add(new DataColumn("Description", typeof(string)));
            dt3.Columns.Add(new DataColumn("Result", typeof(string)));
            dt3.Columns.Add(new DataColumn("Remark", typeof(string)));
            foreach (Section sec in document.Sections)
            {
                for (int i = 0; i < sec.Tables.Count; i++)
                {
                    Table table = (Table)sec.Tables[i];
                    if (table.Title != null) 
                    {
                        if (table.Title.Trim() == "read할때 사용하는 title".Trim())
                        {
                            foreach (TableRow row in table.Rows)
                            {
                                if (row.Cells.Count > 2 && IsBold(row) == true)
                                {
                                    DataRow dtrow = dt3.Rows.Add();
                                    dtrow =
                                        Readvaluefordt3(row, dtrow);
                                }
                            }
                        }
                    }                    
                }
            }

            if (dt3.AsEnumerable().Where(r => r.IsNull(0) == false).Where(r => r[2].ToString().ToUpper().Trim() != "NA").Count() > 0)
            {
                dt3 = dt3.AsEnumerable()
                   .Where(r => r.IsNull(0) == false)
                   .Where(r => r[2].ToString().ToUpper().Trim() != "NA")
                   .CopyToDataTable();
            }
            else
            {
                dt3.Clear();
            }

            return dt3;

        }

        private bool IsBold(TableRow row)
        {
            foreach (Paragraph pr in row.Cells[0].Paragraphs)
            {
                foreach (DocumentObject obc in pr.ChildObjects)
                {
                    if (obc.DocumentObjectType == DocumentObjectType.TextRange)
                    {
                        TextRange tr = obc as TextRange;
                        if (tr.CharacterFormat.Bold == true)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private DataRow Readvaluefordt3(TableRow row, DataRow dtrow)
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
            string resulttext = "";
            for (int i = 0; i < row.Cells[2].Paragraphs.Count; i++)
            {
                Paragraph p = row.Cells[2].Paragraphs[i];
                if (p.Text != "")
                {
                    if (i != 0)
                    {
                        dtrow[2] += "\n";
                    }
                    resulttext = p.Text;
                    if (p.Text.ToUpper().Trim() == "P")
                    {
                        dtrow[2] += "PASS";

                    }
                    else if (p.Text.ToUpper().Trim() == "F")
                    {
                        dtrow[2] += "FAIL";

                    }
                    else if (p.Text.ToUpper().Trim() == "Y")
                    {
                        dtrow[2] += "YES";

                    }
                    else if (p.Text.ToUpper().Trim() == "N")
                    {
                        dtrow[2] += "NO";
                    }
                    else if (p.Text.ToUpper().Trim().Replace(" ", "") == "SR")
                    {
                        dtrow[2] += "See Remark";
                    }
                    else if (remark_dict.ContainsKey(p.Text.ToUpper().Trim()) == true)
                    {
                        if (!dtrow[2].ToString().Contains("Remark"))
                        {
                            dtrow[2] += "Remark";
                        }
                    }
                    else
                    {
                        dtrow[2] += p.Text;
                    }

                    if (remark_dict.ContainsKey(resulttext.Trim().ToUpper()) == true)
                    {
                        if (dtrow[3].ToString().Trim() != "")
                        {
                            dtrow[3] += "\n";
                        }
                        dtrow[3] += remark_dict[resulttext.Trim().ToUpper()];
                    }
                }
            }

            return dtrow;
        }

        //Summary of EN71-2: 2020 Test Result
        private DataTable Getdt4(Document document)
        {
            DataTable dt4 = new DataTable();
            dt4.Columns.Add("Clause");
            dt4.Columns.Add("Desc");
            dt4.Columns.Add("Result");

            foreach (Section section in document.Sections)
            {
                foreach (Table tb in section.Tables)
                {
                    string header = tb.Rows[0].Cells[0].Paragraphs[0].Text.ToString().Trim().ToLower().Replace(" ", "");
                    if (header == "Summary of EN71-2: 2020 Test Results".Trim().ToLower().Replace(" ", ""))
                    {
                        foreach (TableRow tbr in tb.Rows)
                        {
                            if (tbr.GetRowIndex() > 1)
                            {
                                DataRow dr = dt4.Rows.Add();
                                //clause
                                foreach (Paragraph pr in tbr.Cells[0].Paragraphs)
                                {
                                    dr[0] += pr.Text;
                                }
                                //des
                                foreach (Paragraph pr in tbr.Cells[1].Paragraphs)
                                {
                                    dr[1] += pr.Text;
                                }
                                //result
                                foreach (Paragraph pr in tbr.Cells[2].Paragraphs)
                                {
                                    dr[2] += pr.Text;
                                }
                            }
                        }
                    }
                }
            }

            if (dt4.AsEnumerable().Where(r => r.IsNull(0) == false).Where(r => r[0].ToString().ToUpper().Trim() != "").Where(r => r[2].ToString().ToUpper().Trim() != "NA").Count() > 0)
            {
                dt4 = dt4.AsEnumerable()
                   .Where(r => r.IsNull(0) == false)
                   .Where(r => r[0].ToString().ToUpper().Trim() != "")
                   .Where(r => r[2].ToString().ToUpper().Trim() != "NA")
                   .CopyToDataTable();
            }
            else
            {
                dt4.Clear();
            }

            return dt4;

        }

        //page 4-3
        private DataTable Getdt5_1(Document document, DataTable dt4)
        {
            DataTable dt5_1 = new DataTable();
            dt5_1.Columns.Add("Clause");
            dt5_1.Columns.Add("Desc");
            dt5_1.Columns.Add("Sample");
            dt5_1.Columns.Add("Result");

            //4.1의 경우
            if (dt4.AsEnumerable().Where(r => r[0].ToString().ToUpper().Trim() == "4.1").Count() > 0)
            {
                DataRow refrow = dt4.AsEnumerable().Where(r => r[0].ToString().ToUpper().Trim() == "4.1").CopyToDataTable().Rows[0];
                string clausetitle = "";
                string clause = refrow[0].ToString();
                DataRow dr = dt5_1.Rows.Add();

                for (int i = 0; i < titleArr.GetLength(0); i++)
                {
                    if (titleArr[i, 0].ToString() == clause)
                    {
                        clausetitle = titleArr[i, 1];
                        break;
                    }
                }
                //clause
                dr[0] = clausetitle.Trim().Split(' ').First();
                //Desc
                dr[1] = clausetitle.Replace(dr[0].ToString(), "");
                //sample
                dr[2] = GetTableByTitlte(document, "4.1 Sample").Rows[0].Cells[1].Paragraphs[0].Text;
                //result
                dr[3] = GetTableByTitlte(document, "4.1 Result").Rows[GetTableByTitlte(document, "4.1 Result").Rows.Count - 2].Cells[1].Paragraphs[0].Text;
            }
            return dt5_1;
        }

        //Page 4-4
        private DataTable Getdt5_2(Document document, DataTable dt4)
        {
            DataTable dt5_2 = new DataTable();
            dt5_2.Columns.Add("Clause");
            dt5_2.Columns.Add("Desc");
            dt5_2.Columns.Add("Sample");
            dt5_2.Columns.Add("Result");

            //4.2.2~4.2.4의 경우
            if (dt4.AsEnumerable().Where(r => r[0].ToString().ToUpper().Trim() == "4.2.2" || r[0].ToString().ToUpper().Trim() == "4.2.3" || r[0].ToString().ToUpper().Trim() == "4.2.4").Count() > 0)
            {
                foreach (DataRow refrow in dt4.AsEnumerable().Where(r => r[0].ToString().ToUpper().Trim() == "4.2.2" || r[0].ToString().ToUpper().Trim() == "4.2.3" || r[0].ToString().ToUpper().Trim() == "4.2.4").CopyToDataTable().Rows)
                {
                    string clause = refrow[0].ToString();
                    string tablenmae = $"{clause} sample and result";
                    string clausetitle = "";
                    Table resulttb = GetTableByTitlte(document, tablenmae);
                    if (resulttb != null)
                    {
                        for (int i = 0; i < titleArr.Length; i++)
                        {
                            if (titleArr[i, 0].ToString() == clause)
                            {
                                clausetitle = titleArr[i, 1];
                                break;
                            }
                        }

                        foreach (TableRow tr in resulttb.Rows)
                        {
                            if (tr.Cells.Count > 5 && tr.GetRowIndex() > 2)
                            {
                                DataRow dr = dt5_2.Rows.Add();
                                //clause
                                dr[0] = clausetitle.Trim().Split(' ').First();
                                //Desc
                                dr[1] = clausetitle.Replace(dr[0].ToString(), "");
                                //sample
                                dr[2] = "";
                                foreach (Paragraph pr in tr.Cells[0].Paragraphs)
                                {
                                    dr[2] += pr.Text;
                                }
                                //Result
                                dr[3] = "";
                                foreach (Paragraph pr in tr.Cells[tr.Cells.Count - 1].Paragraphs)
                                {
                                    dr[3] += pr.Text;
                                }
                            }
                        }
                    }
                }
            }

            if (dt5_2.AsEnumerable().Where(r => r[2].ToString().ToUpper().Trim() != "").Where(r => r[3].ToString().ToUpper().Trim() != "").Count() > 0)
            {
                dt5_2 = dt5_2.AsEnumerable()
                   .Where(r => r[2].ToString().ToUpper().Trim() != "")
                   .Where(r => r[3].ToString().ToUpper().Trim() != "")
                   .CopyToDataTable();
            }

            return dt5_2;
        }

        //page 4-5
        private DataTable Getdt6(Document document, DataTable dt4)
        {
            DataTable dt6 = new DataTable();
            dt6.Columns.Add("Clause");
            dt6.Columns.Add("Desc");
            dt6.Columns.Add("Sample");
            dt6.Columns.Add("Burn rate");
            dt6.Columns.Add("Result");

            //4.2.5,4.3,4.4,4.5의 경우
            if (dt4.AsEnumerable().Where(r => r[0].ToString().ToUpper().Trim() == "4.2.5" || r[0].ToString().ToUpper().Trim() == "4.3" || r[0].ToString().ToUpper().Trim() == "4.4" || r[0].ToString().ToUpper().Trim() == "4.5").Count() > 0)
            {
                foreach (DataRow row in dt4.AsEnumerable().Where(r => r[0].ToString().ToUpper().Trim() == "4.2.5" || r[0].ToString().ToUpper().Trim() == "4.3" || r[0].ToString().ToUpper().Trim() == "4.4" || r[0].ToString().ToUpper().Trim() == "4.5").CopyToDataTable().Rows)
                {
                    string clause = row[0].ToString();
                    string tablenmae = $"{clause} sample and result";
                    string clausetitle = "";
                    Table resulttb = GetTableByTitlte(document, tablenmae);
                    if (resulttb != null)
                    {
                        for (int i = 0; i < titleArr.Length; i++)
                        {
                            if (titleArr[i, 0].ToString() == clause)
                            {
                                clausetitle = titleArr[i, 1];
                                break;
                            }
                        }

                        foreach (TableRow tr in resulttb.Rows)
                        {
                            if (tr.GetRowIndex() > 2 && tr.Cells.Count > 3)
                            {
                                DataRow dr = dt6.Rows.Add();
                                //clause title
                                dr[0] = clausetitle.Trim().Split(' ').First();
                                //Desc
                                dr[1] = clausetitle.Replace(dr[0].ToString(), "");
                                //sample
                                dr[2] = tr.Cells[GetColumnIndex(resulttb.Rows[2], "SAMPLE")].Paragraphs[0].Text;
                                //burn rate
                                dr[3] = tr.Cells[GetColumnIndex(resulttb.Rows[2], "BURN")].Paragraphs[0].Text;
                                //result
                                dr[4] = tr.Cells[GetColumnIndex(resulttb.Rows[2], "RESULT")].Paragraphs[0].Text;
                            }
                        }
                    }
                }
            }

            if (dt6.AsEnumerable().Where(r => r[2].ToString().ToUpper().Trim() != "").Where(r => r[3].ToString().ToUpper().Trim() != "").Where(r => r[4].ToString().ToUpper().Trim() != "").Count() > 0)
            {
                dt6 = dt6.AsEnumerable()
                   .Where(r => r[2].ToString().ToUpper().Trim() != "")
                   .Where(r => r[3].ToString().ToUpper().Trim() != "")
                   .Where(r => r[4].ToString().ToUpper().Trim() != "")
                   .CopyToDataTable();
            }

            return dt6;
        }

        private int GetColumnIndex(TableRow tr, string header)
        {
            foreach (TableCell tc in tr.Cells)
            {
                foreach (Paragraph pr in tc.Paragraphs)
                {
                    if (pr.Text.ToUpper().Replace(" ", "").StartsWith(header))
                    {
                        return tc.GetCellIndex();
                    }
                }
            }
            return 0;

        }

        //page 5 and 6
        private DataTable Getdt7(Document document, string tbname)
        {
            DataTable dt7 = new DataTable();
            dt7.Columns.Add("Observation");
            dt7.Columns.Add("Result");
            dt7.Columns.Add("Location");

            List<Table> resulttb = GetTablesByTitlte(document, tbname);
            if (resulttb != null)
            {
                foreach (Table tb in resulttb)
                {
                    foreach (TableRow tr in tb.Rows)
                    {
                        if (tr.Cells.Count == 3 && tr.Cells[0].Paragraphs[0].ChildObjects.Count > 0)
                        {
                            TextRange range = tr.Cells[0].Paragraphs[0].ChildObjects[0] as TextRange;
                            if (range.CharacterFormat.Bold == true)
                            {
                                DataRow dr = dt7.Rows.Add();
                                //observation
                                dr[0] = tr.Cells[0].Paragraphs[0].Text;
                                //result
                                dr[1] = tr.Cells[1].Paragraphs[0].Text;
                                //location
                                dr[2] = tr.Cells[2].Paragraphs[0].Text;
                            }
                        }
                    }

                }
            }
            else
            { MessageBox.Show($"Cannot find table {tbname}"); }
            return dt7;


        }


        //Page Complete
        private DataTable Getdt9(Document document)
        {
            DataTable dt9 = new DataTable();
            dt9.Columns.Add(new DataColumn("COMPLETE_실무자", typeof(string)));
            dt9.Columns.Add(new DataColumn("COMPLETE_기술책임자", typeof(string)));
            Table tb = (Table)document.LastSection.Tables[document.LastSection.Tables.Count - 1];
            DataRow drow = dt9.Rows.Add();
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

            return dt9;
        }

        // GET V
        private DataTable Getdt10(Document document)
        {
            DataTable dt10 = new DataTable();
            dt10.Columns.Add(new DataColumn("Result", typeof(string)));
            dt10.Columns.Add(new DataColumn("V", typeof(string)));

            //result 1
            Table vtb1 = GetTableByParagraph(document, 0, 0, "[See Result 1]");
            if (vtb1 != null)
            {
                DataRow dtr1 = dt10.Rows.Add();
                if (vtb1 != null)
                {
                    dtr1[0] = "Result 1";
                    if (vtb1.Rows[1].Cells[1].Paragraphs[0].Text.ToUpper().Contains("V"))
                    {
                        dtr1[1] = "V";
                    }
                    else
                    {
                        dtr1[1] = "";
                    }
                }
                else
                {
                    dtr1[0] = "Table Not found";
                }
            }


            //result 2
            Table vtb2 = GetTableByFirstCell(document, "[See Result 2]");
            if (vtb2 != null)
            {
                DataRow dtr2 = dt10.Rows.Add();
                if (vtb2 != null)
                {
                    dtr2[0] = "Result 2";
                    if (vtb2.Rows[1].Cells[1].Paragraphs[0].Text.ToUpper().Contains("V"))
                    {
                        dtr2[1] = "V";
                    }
                    else
                    {
                        dtr2[1] = "";

                    }
                }
                else
                {
                    dtr2[0] = "Table Not found";

                }
            }


            return dt10;
        }

        private Table GetTableByParagraph(Document document, int rowIndex, int cellIndex, string text)
        {
            foreach (Section section in document.Sections)
            {
                for (int i = 0; i < section.Tables.Count; i++)
                {
                    Table table = section.Tables[i] as Table;
                    foreach (Paragraph pr in table.Rows[rowIndex].Cells[cellIndex].Paragraphs)
                    {
                        if (pr.Text.Trim() == text.Trim())
                        {
                            return table;

                        }

                    }

                }
            }

            return null;

        }

        private Table GetTableByFirstCell(Document document, string prText, int sectionIndex = default)
        {
            if (sectionIndex != default)
            {
                Section section = document.Sections[sectionIndex];
                for (int i = 0; i < section.Tables.Count; i++)
                {
                    Table table = section.Tables[i] as Table;
                    if (table.Rows[0].Cells[0].Paragraphs[0].Text.Trim() == prText)
                    {
                        return table;

                    }
                }
            }
            else
            {
                foreach (Section section in document.Sections)
                {
                    for (int i = 0; i < section.Tables.Count; i++)
                    {
                        Table table = section.Tables[i] as Table;
                        if (table.Rows[0].Cells[0].Paragraphs[0].Text.Trim() == prText)
                        {
                            return table;

                        }
                    }
                }

            }

            return null;

        }

        private List<Table> GetTablesByTitlte(Document document, string title)
        {
            List<Table> tbs = new List<Table>();

            foreach (Section section in document.Sections)
            {
                foreach (Table tb in section.Tables)
                {
                    if (tb.Title == title)
                    {
                        tbs.Add(tb);
                    }
                }
            }
            return tbs;
        }

        private Table GetTableByTitlte(Document document, string title)
        {
            foreach (Section section in document.Sections)
            {
                foreach (Table tb in section.Tables)
                {
                    if (tb.Title == title)
                    {
                        return tb;
                    }
                }
            }
            return null;
        }

        private void GetRemarks(Document document)
        {
            Table tb = GetTableByTitlte(document, "remarks");
            for (int i = 2; i < tb.Rows.Count; i++)
            {
                string remarkName = "";
                string remarkDesc = "";
                foreach (Paragraph pr in tb.Rows[i].Cells[0].Paragraphs)
                {
                    remarkName += pr.Text.Trim().ToUpper();
                }
                foreach (Paragraph pr in tb.Rows[i].Cells[1].Paragraphs)
                {
                    remarkDesc += pr.Text;
                }
                if (remark_dict.ContainsKey(remarkName) == false)
                {
                    remark_dict.Add(remarkName, remarkDesc);
                }
            }
        }

        private void btnToDT_EU_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) == true)
            {
                MessageBox.Show("파일을 선택해주세요.");
                return;
            }

            Document document = new Document();
            document.LoadFromFile(textBox1.Text);

            GetRemarks(document);
            DataTable dt1 = Getdt1(document);                                   //page 1
            DataTable dt2 = Getdt2(document);                                   //page 2
            DataTable dt3 = Getdt3(document);                                   //page 3
            DataTable dt4 = Getdt4(document);                                   //page 4-2
            DataTable dt5_1 = Getdt5_1(document, dt4);                          //page 4-3
            DataTable dt5_2 = Getdt5_2(document, dt4);                          //page.4-4
            DataTable dt6 = Getdt6(document, dt4);                              //page 4-5
            DataTable dt7 = Getdt7(document, "Labelling Requirement1");         //page 5
            DataTable dt8 = Getdt7(document, "Labelling Requirement2");         //page 6
            DataTable dt9 = Getdt9(document);                                   //page complete
            DataTable dt10 = Getdt10(document); //page complete

            dtSet1 = dt1;
            dtSet2 = dt2;
            dtSet3 = dt3;
            dtSet4 = dt4;
            dtSet5_1 = dt5_1;
            dtSet5_2 = dt5_2;
            dtSet6 = dt6;
            dtSet7 = dt7;
            dtSet8 = dt8;
            dtSet9 = dt9;
            dtSet10 = dt10;

            bChkToDT = true;

            this.Close();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
            else
            {
                return;
            }
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

        public DataTable GetVariable5_1()
        {
            return dtSet5_1;
        }

        public DataTable GetVariable5_2()
        {
            return dtSet5_2;
        }

        public DataTable GetVariable6()
        {
            return dtSet6;
        }

        public DataTable GetVariable7()
        {
            return dtSet7;
        }

        public DataTable GetVariable8()
        {
            return dtSet8;
        }

        public DataTable GetVariable9()
        {
            return dtSet9;
        }

        public DataTable GetVariable10()
        {
            return dtSet10;
        }
    }
}
