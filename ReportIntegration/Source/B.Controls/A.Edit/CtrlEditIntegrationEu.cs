using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.Data;

using Ulee.Controls;

namespace Sgs.ReportIntegration
{
    public partial class CtrlEditIntegrationEu : UlUserControlEng
    {
        public IntegrationMainDataSet MainSet { get; set; }

        public IntegrationImageDataSet ImageSet { get; set; }

        public IntegrationT1DataSet T1Set { get; set; }

        public IntegrationT2DataSet T2Set { get; set; }

        public IntegrationT3DataSet T3Set { get; set; }

        public IntegrationT4DataSet T4Set { get; set; }

        public IntegrationT5DataSet T5Set { get; set; }

        public IntegrationT6DataSet T6Set { get; set; }

        public IntegrationT7DataSet T7Set { get; set; }

        public IntegrationLimitEnDataSet LimitSet { get; set; }

        public IntegrationResultEnDataSet ResultSet { get; set; }

        public List<IntegrationT1Row> T1Rows;

        public List<IntegrationT2Row> T2Rows;

        public List<IntegrationT2Row> T3Rows;

        public List<IntegrationT1Row> T4Rows;

        public List<IntegrationT1Row> T5Rows;

        public List<IntegrationT6Row> T6Rows;

        private StaffDataSet staffSet;

        private GridBookmark t1Bookmark;

        private GridBookmark t2Bookmark;

        private GridBookmark t6Bookmark;

        private Button findButton;

        public CtrlEditIntegrationEu(Button findButton)
        {
            this.findButton = findButton;

            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            staffSet = new StaffDataSet(AppRes.DB.Connect, null, null);

            t1Bookmark = new GridBookmark(t1GridView);
            T1Rows = new List<IntegrationT1Row>();
            t1Grid.DataSource = T1Rows;
            AppHelper.SetGridEvenRow(t1GridView);

            t2Bookmark = new GridBookmark(t2GridView);
            T2Rows = new List<IntegrationT2Row>();
            t2Grid.DataSource = T2Rows;
            AppHelper.SetGridEvenRow(t2GridView);

            T3Rows = new List<IntegrationT2Row>();
            t3Grid.DataSource = T3Rows;
            AppHelper.SetGridEvenRow(t3GridView);

            T4Rows = new List<IntegrationT1Row>();
            t4Grid.DataSource = T4Rows;

            T5Rows = new List<IntegrationT1Row>();
            t5Grid.DataSource = T5Rows;

            t6Bookmark = new GridBookmark(t6GridView);
            T6Rows = new List<IntegrationT6Row>();
            t6Grid.DataSource = T6Rows;
            AppHelper.SetGridEvenRow(t6GridView);
        }

        private void CtrlEditIntegrationEu_Enter(object sender, EventArgs e)
        {
            //AppHelper.SetGridDataSource(t3Grid, T3Set);
            //AppHelper.SetGridDataSource(t4Grid, T4Set);
            //AppHelper.SetGridDataSource(t5Grid, T5Set);
            AppHelper.SetGridDataSource(t7Grid, T7Set);

            AppHelper.SetGridDataSource(resultGrid, ResultSet);
            AppHelper.SetGridDataSource(limitGrid, LimitSet);
        }

        private void integration1Page_Resize(object sender, EventArgs e)
        {

        }

        private void integration2Page_Resize(object sender, EventArgs e)
        {

        }

        private void integration3Page_Resize(object sender, EventArgs e)
        {

        }

        private void integration4Page_Resize(object sender, EventArgs e)
        {

        }

        private void integration5Page_Resize(object sender, EventArgs e)
        {

        }

        private void integration6Page_Resize(object sender, EventArgs e)
        {

        }

        private void integration7Page_Resize(object sender, EventArgs e)
        {

        }

        private void integration8Page_Resize(object sender, EventArgs e)
        {

        }

        private void t1RowUpButton_Click(object sender, EventArgs e)
        {
            int index = t1GridView.FocusedRowHandle;

            if (index <= 0) return;

            IntegrationT1Row row = NewT1Row();
            row.No = T1Rows[index].No;
            row.Line = T1Rows[index].Line;
            row.Requested = T1Rows[index].Requested;
            row.Conclusion = T1Rows[index].Conclusion;

            t1Bookmark.Get();
            T1Rows.RemoveAt(index);
            T1Rows.Insert(index - 1, row);
            ReorderT1Rows();
            AppHelper.RefreshGridData(t1GridView);

            t1Bookmark.Goto();
            t1GridView.MoveBy(-1);

            t1Grid.Focus();
        }

        private void t1RowDownButton_Click(object sender, EventArgs e)
        {
            int index = t1GridView.FocusedRowHandle;

            if (index >= T1Rows.Count - 1) return;

            IntegrationT1Row row = NewT1Row();
            row.No = T1Rows[index].No;
            row.Line = T1Rows[index].Line;
            row.Requested = T1Rows[index].Requested;
            row.Conclusion = T1Rows[index].Conclusion;

            t1Bookmark.Get();
            T1Rows.RemoveAt(index);

            if (index < T1Rows.Count - 1)
            {
                T1Rows.Insert(index + 1, row);
            }
            else
            {
                T1Rows.Add(row);
            }

            ReorderT1Rows();
            AppHelper.RefreshGridData(t1GridView);

            t1Bookmark.Goto();
            t1GridView.MoveBy(1);

            t1Grid.Focus();
        }

        private void t1RowPluseButton_Click(object sender, EventArgs e)
        {
            int index = t1GridView.FocusedRowHandle;

            t1Bookmark.Get();

            if ((index < 0) || (index == T1Rows.Count - 1))
            {
                T1Rows.Add(NewT1Row());
            }
            else
            {
                T1Rows.Insert(index + 1, NewT1Row());
            }

            ReorderT1Rows();
            AppHelper.RefreshGridData(t1GridView);

            t1Bookmark.Goto();
            t1GridView.MoveBy(1);

            t1Grid.Focus();
        }

        private void t1RowMinusButton_Click(object sender, EventArgs e)
        {
            int index = t1GridView.FocusedRowHandle;

            if (index < 0) return;

            t1Bookmark.Get();
            T1Rows.RemoveAt(index);

            ReorderT1Rows();
            AppHelper.RefreshGridData(t1GridView);

            t1Bookmark.Goto();

            t1Grid.Focus();
        }

        private void t2RowUpButton_Click(object sender, EventArgs e)
        {
            int index = t2GridView.FocusedRowHandle;

            if (index <= 0) return;

            IntegrationT2Row row = NewT2Row();
            row.No = T2Rows[index].No;
            row.Line = T2Rows[index].Line;
            row.Clause = T2Rows[index].Clause;
            row.Description = T2Rows[index].Description;
            row.Result = T2Rows[index].Result;

            t2Bookmark.Get();
            T2Rows.RemoveAt(index);
            T2Rows.Insert(index - 1, row);
            ReorderT2Rows();
            AppHelper.RefreshGridData(t2GridView);

            t2Bookmark.Goto();
            t2GridView.MoveBy(-1);

            t2Grid.Focus();
        }

        private void t2RowDownButton_Click(object sender, EventArgs e)
        {
            int index = t2GridView.FocusedRowHandle;

            if (index >= T2Rows.Count - 1) return;

            IntegrationT2Row row = NewT2Row();
            row.No = T2Rows[index].No;
            row.Line = T2Rows[index].Line;
            row.Clause = T2Rows[index].Clause;
            row.Description = T2Rows[index].Description;
            row.Result = T2Rows[index].Result;

            t2Bookmark.Get();
            T2Rows.RemoveAt(index);

            if (index < T2Rows.Count - 1)
            {
                T2Rows.Insert(index + 1, row);
            }
            else
            {
                T2Rows.Add(row);
            }

            ReorderT2Rows();
            AppHelper.RefreshGridData(t2GridView);

            t2Bookmark.Goto();
            t2GridView.MoveBy(1);

            t2Grid.Focus();
        }

        private void t2RowPluseButton_Click(object sender, EventArgs e)
        {
            int index = t2GridView.FocusedRowHandle;

            t2Bookmark.Get();

            if ((index < 0) || (index == T2Rows.Count - 1))
            {
                T2Rows.Add(NewT2Row());
            }
            else
            {
                T2Rows.Insert(index + 1, NewT2Row());
            }

            ReorderT2Rows();
            AppHelper.RefreshGridData(t2GridView);

            t2Bookmark.Goto();
            t2GridView.MoveBy(1);

            t2Grid.Focus();
        }

        private void t2RowMinusButton_Click(object sender, EventArgs e)
        {
            int index = t2GridView.FocusedRowHandle;

            if (index < 0) return;

            t2Bookmark.Get();
            T2Rows.RemoveAt(index);

            ReorderT2Rows();
            AppHelper.RefreshGridData(t2GridView);

            t2Bookmark.Goto();

            t2Grid.Focus();
        }

        private void t6RowUpButton_Click(object sender, EventArgs e)
        {
            int index = t6GridView.FocusedRowHandle;

            if (index <= 0) return;

            IntegrationT6Row row = NewT6Row();
            row.No = T6Rows[index].No;
            row.Line = T6Rows[index].Line;
            row.TestItem = T6Rows[index].TestItem;
            row.Result = T6Rows[index].Result;
            row.Requirement = T6Rows[index].Requirement;

            t6Bookmark.Get();
            T6Rows.RemoveAt(index);
            T6Rows.Insert(index - 1, row);
            ReorderT6Rows();
            AppHelper.RefreshGridData(t6GridView);

            t6Bookmark.Goto();
            t6GridView.MoveBy(-1);

            t6Grid.Focus();
        }

        private void t6RowDownButton_Click(object sender, EventArgs e)
        {
            int index = t6GridView.FocusedRowHandle;

            if (index >= T6Rows.Count - 1) return;

            IntegrationT6Row row = NewT6Row();
            row.No = T6Rows[index].No;
            row.Line = T6Rows[index].Line;
            row.TestItem = T6Rows[index].TestItem;
            row.Result = T6Rows[index].Result;
            row.Requirement = T6Rows[index].Requirement;

            t6Bookmark.Get();
            T6Rows.RemoveAt(index);

            if (index < T6Rows.Count - 1)
            {
                T6Rows.Insert(index + 1, row);
            }
            else
            {
                T6Rows.Add(row);
            }

            ReorderT6Rows();
            AppHelper.RefreshGridData(t6GridView);

            t6Bookmark.Goto();
            t6GridView.MoveBy(1);

            t6Grid.Focus();
        }

        private void t6RowPluseButton_Click(object sender, EventArgs e)
        {
            int index = t6GridView.FocusedRowHandle;

            t6Bookmark.Get();

            if ((index < 0) || (index == T6Rows.Count - 1))
            {
                T6Rows.Add(NewT6Row());
            }
            else
            {
                T6Rows.Insert(index + 1, NewT6Row());
            }

            ReorderT6Rows();
            AppHelper.RefreshGridData(t6GridView);

            t6Bookmark.Goto();
            t6GridView.MoveBy(1);

            t6Grid.Focus();
        }

        private void t6RowMinusButton_Click(object sender, EventArgs e)
        {
            int index = t6GridView.FocusedRowHandle;

            if (index < 0) return;

            t6Bookmark.Get();
            T6Rows.RemoveAt(index);

            ReorderT6Rows();
            AppHelper.RefreshGridData(t6GridView);

            t6Bookmark.Goto();

            t6Grid.Focus();
        }

        private void approveButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MainSet.StaffNo) == false)
            {
                if (AppRes.UserId != MainSet.StaffNo)
                {
                    MessageBox.Show(
                        "You have no authority to disapprove this report!",
                        "SGS", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            if (MessageBox.Show(
                $"Would you like to {approveButton.Text.ToLower()} integration report of {MainSet.RecNo}?",
                "SGS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            switch (approveButton.Text)
            {
                case "Approve":
                    SetApproval(AppRes.UserId, true);
                    break;

                case "Disapprove":
                    SetApproval("", true);
                    break;
            }

            findButton.PerformClick();
        }

        public void SetDataSetToControl()
        {
            SetDataSetToMain();

            T1Set.MainNo = MainSet.RecNo;
            T1Set.Select();
            SetDataSetToT1();

            T2Set.MainNo = MainSet.RecNo;
            T2Set.Select();
            SetDataSetToT2();

            T3Set.MainNo = MainSet.RecNo;
            T3Set.Select();
            SetDataSetToT3();

            T4Set.MainNo = MainSet.RecNo;
            T4Set.Select();
            SetDataSetToT4();

            T5Set.MainNo = MainSet.RecNo;
            T5Set.Select();
            SetDataSetToT5();

            T6Set.MainNo = MainSet.RecNo;
            T6Set.Select();
            SetDataSetToT6();

            T7Set.MainNo = MainSet.RecNo;
            T7Set.Select();

            LimitSet.MainNo = MainSet.RecNo;
            LimitSet.Select();
            ResultSet.MainNo = MainSet.RecNo;
            ResultSet.Select();

            ImageSet.RecNo = MainSet.RecNo;
            ImageSet.Select();
            SetDataSetToImage();

            RefreshGrid();
        }

        private void SetDataSetToMain()
        {
            p1ClientNameEdit.Text = MainSet.P1ClientName;
            p1ClientAddressEdit.Text = MainSet.P1ClientAddress;
            p1FileNoEdit.Text = MainSet.P1FileNo;
            p1SampleDescriptionEdit.Text = MainSet.P1SampleDescription;
            p1DetailOfSampleEdit.Text = MainSet.P1DetailOfSample;
            p1ItemNoEdit.Text = MainSet.P1ItemNo;
            p1OrderNoEdit.Text = MainSet.P1OrderNo;
            p1PackagingEdit.Text = MainSet.P1Packaging;
            p1InstructionEdit.Text = MainSet.P1Instruction;
            p1BuyerEdit.Text = MainSet.P1Buyer;
            p1ManufacturerEdit.Text = MainSet.P1Manufacturer;
            p1CountryOfOriginEdit.Text = MainSet.P1CountryOfOrigin;
            p1CountryOfDestinationEdit.Text = MainSet.P1CountryOfDestination;
            p1LabeledAgeEdit.Text = MainSet.P1LabeledAge;
            p1TestAgeEdit.Text = MainSet.P1TestAge;
            p1AssessedAgeEdit.Text = MainSet.P1AssessedAge;
            p1ReceivedDateEdit.Text = MainSet.P1ReceivedDate;
            p1TestPeriodEdit.Text = MainSet.P1TestPeriod;
            p1TestMethodEdit.Text = MainSet.P1TestMethod;
            p1TestResultEdit.Text = MainSet.P1TestResults;
            p1ReportCommentEdit.Text = MainSet.P1Comments;

            desc1Edit.Text = MainSet.Description1;
            desc20Edit.Text = MainSet.Description2;
            desc21Edit.Text = MainSet.Description2;
            desc22Edit.Text = MainSet.Description2;
            desc3Edit.Text = MainSet.Description3;
            desc4Edit.Text = MainSet.Description4;
            desc5Edit.Text = MainSet.Description5;
            desc6Edit.Text = MainSet.Description6;
            desc7Edit.Text = MainSet.Description7;
            desc8Edit.Text = MainSet.Description8;
            desc9Edit.Text = MainSet.Description9;
            desc10Edit.Text = MainSet.Description10;
            desc11Panel.Text = MainSet.Description11;
        }

        private void SetDataSetToT1()
        {
            T1Rows.Clear();
            for (int i = 0; i < T1Set.RowCount; i++)
            {
                T1Set.Fetch(i);

                IntegrationT1Row t1Row = new IntegrationT1Row();
                t1Row.No = T1Set.No;
                t1Row.Line = T1Set.Line;
                t1Row.Requested = T1Set.Requested;
                t1Row.Conclusion = T1Set.Conclusion;
                T1Rows.Add(t1Row);
            }

            t1NoColumn.SortOrder = ColumnSortOrder.Ascending;
        }

        private void SetDataSetToT2()
        {
            T2Rows.Clear();
            for (int i = 0; i < T2Set.RowCount; i++)
            {
                T2Set.Fetch(i);

                IntegrationT2Row t2Row = new IntegrationT2Row();
                t2Row.No = T2Set.No;
                t2Row.Line = T2Set.Line;
                t2Row.Clause= T2Set.Clause;
                t2Row.Description = T2Set.Description;
                t2Row.Result = T2Set.Result;
                T2Rows.Add(t2Row);
            }

            t2NoColumn.SortOrder = ColumnSortOrder.Ascending;
        }

        private void SetDataSetToT3()
        {
            T3Rows.Clear();
            for (int i = 0; i < T3Set.RowCount; i++)
            {
                T3Set.Fetch(i);

                IntegrationT2Row t3Row = new IntegrationT2Row();
                t3Row.No = T3Set.No;
                t3Row.Line = T3Set.Line;
                t3Row.Clause = T3Set.Clause;
                t3Row.Description = T3Set.Description;
                t3Row.Result = T3Set.Result;
                T3Rows.Add(t3Row);
            }

            t3NoColumn.SortOrder = ColumnSortOrder.Ascending;
        }

        private void SetDataSetToT4()
        {
            T4Rows.Clear();
            for (int i = 0; i < T4Set.RowCount; i++)
            {
                T4Set.Fetch(i);

                IntegrationT1Row t4Row = new IntegrationT1Row();
                t4Row.No = T4Set.No;
                t4Row.Line = T4Set.Line;
                t4Row.Requested = T4Set.Sample;
                t4Row.Conclusion = T4Set.Result;
                T4Rows.Add(t4Row);
            }
        }

        private void SetDataSetToT5()
        {
            T5Rows.Clear();
            for (int i = 0; i < T5Set.RowCount; i++)
            {
                T5Set.Fetch(i);

                IntegrationT1Row t5Row = new IntegrationT1Row();
                t5Row.No = T5Set.No;
                t5Row.Line = T5Set.Line;
                t5Row.Requested = T5Set.Sample;
                t5Row.Conclusion = T5Set.BurningRate;
                T5Rows.Add(t5Row);
            }
        }

        private void SetDataSetToT6()
        {
            T6Rows.Clear();
            for (int i = 0; i < T6Set.RowCount; i++)
            {
                T6Set.Fetch(i);

                IntegrationT6Row t6Row = new IntegrationT6Row();
                t6Row.No = T6Set.No;
                t6Row.Line = T6Set.Line;
                t6Row.TestItem = T6Set.TestItem;
                t6Row.Result = T6Set.Result;
                t6Row.Requirement = T6Set.Requirement;
                T6Rows.Add(t6Row);
            }

            t6NoColumn.SortOrder = ColumnSortOrder.Ascending;
        }

        private void SetDataSetToImage()
        {
            p8FileNoPanel.Text = MainSet.P1FileNo;

            ImageSet.Fetch();
            p8ImageBox.Image = ImageSet.Picture;

            if (string.IsNullOrWhiteSpace(MainSet.StaffNo) == false)
            {
                SetApproval(MainSet.StaffNo);
            }

            approveButton.Visible = (AppRes.Authority == EReportAuthority.Manager) ? true : false;
        }

        public void SetControlToDataSet()
        {
            t1GridView.PostEditor();
            t2GridView.PostEditor();
            t3GridView.PostEditor();
            t4GridView.PostEditor();
            t5GridView.PostEditor();
            t6GridView.PostEditor();

            MainSet.P1ClientName = p1ClientNameEdit.Text;
            MainSet.P1ClientAddress = p1ClientAddressEdit.Text;
            MainSet.P1FileNo = p1FileNoEdit.Text;
            MainSet.P1SampleDescription = p1SampleDescriptionEdit.Text;
            MainSet.P1DetailOfSample = p1DetailOfSampleEdit.Text;
            MainSet.P1ItemNo = p1ItemNoEdit.Text;
            MainSet.P1OrderNo = p1OrderNoEdit.Text;
            MainSet.P1Packaging = p1PackagingEdit.Text;
            MainSet.P1Instruction = p1InstructionEdit.Text;
            MainSet.P1Buyer = p1BuyerEdit.Text;
            MainSet.P1Manufacturer = p1ManufacturerEdit.Text;
            MainSet.P1CountryOfOrigin = p1CountryOfOriginEdit.Text;
            MainSet.P1CountryOfDestination = p1CountryOfDestinationEdit.Text;
            MainSet.P1LabeledAge = p1LabeledAgeEdit.Text;
            MainSet.P1TestAge = p1TestAgeEdit.Text;
            MainSet.P1AssessedAge = p1AssessedAgeEdit.Text;
            MainSet.P1ReceivedDate = p1ReceivedDateEdit.Text;
            MainSet.P1TestPeriod = p1TestPeriodEdit.Text;
            MainSet.P1TestMethod = p1TestMethodEdit.Text;
            MainSet.P1TestResults = p1TestResultEdit.Text;
            MainSet.P1Comments = p1ReportCommentEdit.Text;
            MainSet.StaffName = signNameEdit.Text;
            MainSet.Description1 = desc1Edit.Text;
            MainSet.Description2 = desc20Edit.Text;
            MainSet.Description3 = desc3Edit.Text;
            MainSet.Description4 = desc4Edit.Text;
            MainSet.Description5 = desc5Edit.Text;
            MainSet.Description6 = desc6Edit.Text;
            MainSet.Description7 = desc7Edit.Text;
            MainSet.Description8 = desc8Edit.Text;
            MainSet.Description9 = desc9Edit.Text;
            MainSet.Description10 = desc10Edit.Text;
            MainSet.Description11 = desc11Panel.Text;
            MainSet.Description12 = "";
            MainSet.Description13 = "";
            MainSet.Description14 = "";
            MainSet.Description15 = "";
            MainSet.Description16 = "";
            MainSet.Description17 = "";
            MainSet.Description18 = "";
            MainSet.Description19 = "";
        }

        private IntegrationT1Row NewT1Row()
        {
            IntegrationT1Row row = new IntegrationT1Row();

            row.No = 0;
            row.Line = false;
            row.Requested = "";
            row.Conclusion = "";

            return row;
        }

        private IntegrationT2Row NewT2Row()
        {
            IntegrationT2Row row = new IntegrationT2Row();

            row.No = 0;
            row.Line = false;
            row.Clause = "";
            row.Description = "";
            row.Result = "";

            return row;
        }

        private IntegrationT6Row NewT6Row()
        {
            IntegrationT6Row row = new IntegrationT6Row();

            row.No = 0;
            row.Line = false;
            row.TestItem = "";
            row.Result = "";
            row.Requirement = "";

            return row;
        }

        private void ReorderT1Rows()
        {
            for (int i = 0; i < T1Rows.Count; i++)
            {
                T1Rows[i].No = i;
            }
        }

        private void ReorderT2Rows()
        {
            for (int i = 0; i < T2Rows.Count; i++)
            {
                T2Rows[i].No = i;
            }
        }

        private void ReorderT6Rows()
        {
            for (int i = 0; i < T6Rows.Count; i++)
            {
                T6Rows[i].No = i;
            }
        }

        private void RefreshGrid()
        {
            AppHelper.RefreshGridData(t1GridView);
            AppHelper.RefreshGridData(t2GridView);
            AppHelper.RefreshGridData(t3GridView);
            AppHelper.RefreshGridData(t4GridView);
            AppHelper.RefreshGridData(t5GridView);
            AppHelper.RefreshGridData(t6GridView);
            AppHelper.RefreshGridData(t7GridView);
            AppHelper.RefreshGridData(resultGrid);
            AppHelper.RefreshGridData(limitGridView);
        }

        private void SetApproval(string staffNo, bool isDbUpdate = false)
        {
            if (string.IsNullOrWhiteSpace(staffNo) == true)
            {
                staffNo = "";
                signImageBox.Image = null;
                signNameEdit.Text = "";
                approveButton.Text = "Approve";
            }
            else
            {
                staffSet.StaffNo = staffNo;
                staffSet.Select();
                staffSet.Fetch();

                if (staffSet.Signature != null)
                {
                    signImageBox.Image = staffSet.Signature;
                    signNameEdit.Text = staffSet.FirstName + " " + staffSet.LastName;
                    approveButton.Text = "Disapprove";
                }
                else
                {
                    staffNo = "";
                    signImageBox.Image = null;
                    signNameEdit.Text = "";
                    approveButton.Text = "Approve";
                }
            }

            if (isDbUpdate == true)
            {
                MainSet.Approval = (string.IsNullOrWhiteSpace(staffNo) == true) ? false : true;
                MainSet.StaffNo = staffNo;
                MainSet.StaffName = signNameEdit.Text;
                MainSet.UpdateApproval();
            }
        }
    }
}
