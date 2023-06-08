using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

using DevExpress.Data;
using DevExpress.XtraEditors;
using Sgs.ReportIntegration.Source.A.Forms.B.Dialog;
using Ulee.Controls;

namespace Sgs.ReportIntegration
{
    public partial class CtrlEditPhysicalUs : UlUserControlEng
    {
        public PhysicalMainDataSet MainSet;

        public PhysicalImageDataSet ImageSet;

        public PhysicalP2DataSet P2Set;

        public PhysicalP3DataSet P3Set;

        public PhysicalP41DataSet P4Set;

        public PhysicalP5DataSet P5Set;

        public List<PhysicalPage2Row> P2Rows;

        public List<PhysicalPage3Row> P3Rows;

        public List<PhysicalPage4Row> P4Rows;

        public List<PhysicalPage5Row> P5Rows;

        private GridBookmark p2Bookmark;

        private GridBookmark p3Bookmark;

        private GridBookmark p4Bookmark;

        private GridBookmark p5Bookmark;

        private StaffDataSet staffSet;

        private Button findButton;

        private PhysicalQuery phyQuery;

        public DataTable dtGet1;

        public DataTable dtGet2;

        public DataTable dtGet3;

        public DataTable dtGet4;

        public DataTable dtGet5;

        public DataTable dtGet6;

        public CtrlEditPhysicalUs(Button findButton)
        {

            this.findButton = findButton;

            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            progressBar_PhyASTM_ImportWORD.Style = ProgressBarStyle.Continuous;
            progressBar_PhyASTM_ImportWORD.Minimum = 0;
            progressBar_PhyASTM_ImportWORD.Maximum = 110;
            progressBar_PhyASTM_ImportWORD.Step = 10;
            progressBar_PhyASTM_ImportWORD.Value = 0;

            P2Set = new PhysicalP2DataSet(AppRes.DB.Connect, null, null);

            staffSet = new StaffDataSet(AppRes.DB.Connect, null, null);

            phyQuery = new PhysicalQuery();
            phyQuery.P2Set = P2Set;

            p2Bookmark = new GridBookmark(p2ResultGridView);
            P2Rows = new List<PhysicalPage2Row>();
            p2ResultGrid.DataSource = P2Rows;
            AppHelper.SetGridEvenRow(p2ResultGridView);

            p3Bookmark = new GridBookmark(p3ClauseGridView);
            P3Rows = new List<PhysicalPage3Row>();
            p3ClauseGrid.DataSource = P3Rows;
            AppHelper.SetGridEvenRow(p3ClauseGridView);

            //p4Bookmark = new GridBookmark(p4FlameGridView);
            P4Rows = new List<PhysicalPage4Row>();
            p4FlameGrid.DataSource = P4Rows;
            AppHelper.SetGridEvenRow(p4FlameGridView);

            p5Bookmark = new GridBookmark(p5StuffGridView);
            P5Rows = new List<PhysicalPage5Row>();
            p5StuffGrid.DataSource = P5Rows;
            AppHelper.SetGridEvenRow(p5StuffGridView);
        }

        private void CtrlEditPhysicalUs_Load(object sender, EventArgs e)
        {
            LoadReportDescription();
        }

        private void CtrlEditPhysicalUs_Enter(object sender, EventArgs e)
        {
            //EnterReportDescription();
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
                $"Would you like to {approveButton.Text.ToLower()} physical report of {MainSet.RecNo}?",
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

        private void physicalTab_Resize(object sender, EventArgs e)
        {
        }

        private void physical1Page_Resize(object sender, EventArgs e)
        {
            int width = physical1Page.Width;

            p1ClientNameEdit.Width = width - 172;
            p1ClientAddressEdit.Width = width - 172;
            p1SampleDescriptionEdit.Width = width - 410;
            p1DetailOfSampleCombo.Width = width - 275;
            //p1DetailOfSampleEdit.Width = width - 172;
            p1OrderNoEdit.Width = width - 410;
            //p1ManufacturerEdit.Width = width - 172;
            p1ManufacturerComboBox.Width = width - 172;
            p1CountryOfDestinationEdit.Width = width - 410;
            p1LabeledAgeEdit.Width = width - 172;
            p1TestAgeEdit.Width = width - 172;
            //p1AssessedAgeEdit.Width = width - 172;
            p1AssessedAgeCombo.Width = width - 172;
            p1ReceivedDateEdit.Width = width - 172;
            p1TestPeriodEdit.Width = width - 172;
            p1TestMethodEdit.Width = width - 172;
            p1TestResultEdit.Width = width - 172;
            p1ReportCommentEdit.Width = width - 172;
        }

        private void physical2Page_Resize(object sender, EventArgs e)
        {
            int width = physical2Page.Width;
            int height = physical2Page.Height;

            p2ResultGrid.Size = new Size(width - 8, height - 37);
            p2ResultTestRequestedColumn.Width = width - 142;
            p2RowUpButton.Left = width - 106;
            p2RowDownButton.Left = width - 80;
            p2RowPluseButton.Left = width - 54;
            p2RowMinusButton.Left = width - 28;
        }

        private void physical3Page_Resize(object sender, EventArgs e)
        {
            int width = physical3Page.Width;
            int height = physical3Page.Height;

            p3ClauseGrid.Size = new Size(width - 8, height - 201);
            p3ClauseDescriptionColumn.Width = width - 295;

            p3RowUpButton.Left = width - 106;
            p3RowDownButton.Left = width - 80;
            p3RowPluseButton.Left = width - 54;
            p3RowMinusButton.Left = width - 28;

            p3Desc1Edit.Width = width - 8;
            p3Desc2Edit.Top = height - 72;
            p3Desc2Edit.Width = width - 8;
        }

        private void physical4Page_Resize(object sender, EventArgs e)
        {
            int width = physical4Page.Width;
            int height = physical4Page.Height;

            p4FlameGrid.Size = new Size(width - 8, height - 161);
            p4FlameSampleColumn.Width = (width - 62) / 2;
            p4FlameBurningRateColumn.Width = (width - 62) / 2;

            p4RowUpButton.Left = width - 106;
            p4RowDownButton.Left = width - 80;
            p4RowPluseButton.Left = width - 54;
            p4RowMinusButton.Left = width - 28;

            p4Desc1Edit.Width = width - 8;
            p4Desc2Edit.Top = height - 69;
            p4Desc2Edit.Width = width - 8;
        }

        private void physical5Page_Resize(object sender, EventArgs e)
        {
            int width = physical5Page.Width;
            int height = physical5Page.Height;

            p5StuffGrid.Size = new Size(width - 8, height - 143);
            p5StuffTestItemColumn.Width = width - 242;

            p5RowUpButton.Left = width - 106;
            p5RowDownButton.Left = width - 80;
            p5RowPluseButton.Left = width - 54;
            p5RowMinusButton.Left = width - 28;

            p5Desc1Edit.Width = width - 8;
        }

        private void physical6Page_Resize(object sender, EventArgs e)
        {
            imagePanel.Size = new Size(physical6Page.Width - 16, physical6Page.Height - 70);
            p6ImageBox.Size = new Size(imagePanel.Width - 16, imagePanel.Height - 74);
            p6DescPanel.Width = imagePanel.Width - 16;

            p6FileNoPanel.Top = physical6Page.Height - 56;
            p6FileNoPanel.Width = physical6Page.Width - 16;
        }

        private void physicalTab_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void p2RowUpButton_Click(object sender, EventArgs e)
        {
            int index = p2ResultGridView.FocusedRowHandle;

            if (index <= 0) return;

            PhysicalPage2Row row = NewP2Row();
            row.No = P2Rows[index].No;
            row.Line = P2Rows[index].Line;
            row.Requested = P2Rows[index].Requested;
            row.Conclusion = P2Rows[index].Conclusion;

            p2Bookmark.Get();
            P2Rows.RemoveAt(index);
            P2Rows.Insert(index - 1, row);
            ReorderP2Rows();
            AppHelper.RefreshGridData(p2ResultGridView);

            p2Bookmark.Goto();
            p2ResultGridView.MoveBy(-1);

            p2ResultGrid.Focus();
        }

        private void p2RowDownButton_Click(object sender, EventArgs e)
        {
            int index = p2ResultGridView.FocusedRowHandle;

            if (index >= P2Rows.Count - 1) return;

            PhysicalPage2Row row = NewP2Row();
            row.No = P2Rows[index].No;
            row.Line = P2Rows[index].Line;
            row.Requested = P2Rows[index].Requested;
            row.Conclusion = P2Rows[index].Conclusion;

            p2Bookmark.Get();
            P2Rows.RemoveAt(index);

            if (index < P2Rows.Count - 1)
            {
                P2Rows.Insert(index + 1, row);
            }
            else
            {
                P2Rows.Add(row);
            }

            ReorderP2Rows();
            AppHelper.RefreshGridData(p2ResultGridView);

            p2Bookmark.Goto();
            p2ResultGridView.MoveBy(1);

            p2ResultGrid.Focus();
        }

        private void p2RowPluseButton_Click(object sender, EventArgs e)
        {
            int index = p2ResultGridView.FocusedRowHandle;

            p2Bookmark.Get();

            if ((index < 0) || (index == P2Rows.Count - 1))
            {
                P2Rows.Add(NewP2Row());
            }
            else
            {
                P2Rows.Insert(index + 1, NewP2Row());
            }

            ReorderP2Rows();
            AppHelper.RefreshGridData(p2ResultGridView);

            p2Bookmark.Goto();
            p2ResultGridView.MoveBy(1);

            p2ResultGrid.Focus();
        }

        private void p2RowMinusButton_Click(object sender, EventArgs e)
        {
            int index = p2ResultGridView.FocusedRowHandle;

            if (index < 0) return;

            p2Bookmark.Get();
            P2Rows.RemoveAt(index);

            ReorderP2Rows();
            AppHelper.RefreshGridData(p2ResultGridView);

            p2Bookmark.Goto();

            p2ResultGrid.Focus();
        }

        private void p3RowUpButton_Click(object sender, EventArgs e)
        {
            int index = p3ClauseGridView.FocusedRowHandle;

            if (index <= 0) return;

            PhysicalPage3Row row = NewP3Row();
            row.No = P3Rows[index].No;
            row.Line = P3Rows[index].Line;
            row.Clause = P3Rows[index].Clause;
            row.Description = P3Rows[index].Description;
            row.Result = P3Rows[index].Result;
            row.Comment = P3Rows[index].Comment;

            p3Bookmark.Get();
            P3Rows.RemoveAt(index);
            P3Rows.Insert(index - 1, row);
            ReorderP3Rows();
            AppHelper.RefreshGridData(p3ClauseGridView);

            p3Bookmark.Goto();
            p3ClauseGridView.MoveBy(-1);

            p3ClauseGridView.Focus();
        }

        private void p3RowDownButton_Click(object sender, EventArgs e)
        {
            int index = p3ClauseGridView.FocusedRowHandle;

            if (index >= P3Rows.Count - 1) return;

            PhysicalPage3Row row = NewP3Row();
            row.No = P3Rows[index].No;
            row.Line = P3Rows[index].Line;
            row.Clause = P3Rows[index].Clause;
            row.Description = P3Rows[index].Description;
            row.Result = P3Rows[index].Result;

            p3Bookmark.Get();
            P3Rows.RemoveAt(index);

            if (index < P3Rows.Count - 1)
            {
                P3Rows.Insert(index + 1, row);
            }
            else
            {
                P3Rows.Add(row);
            }

            ReorderP3Rows();
            AppHelper.RefreshGridData(p3ClauseGridView);

            p3Bookmark.Goto();
            p3ClauseGridView.MoveBy(1);

            p3ClauseGrid.Focus();
        }

        private void p3RowPluseButton_Click(object sender, EventArgs e)
        {
            int index = p3ClauseGridView.FocusedRowHandle;

            p3Bookmark.Get();

            if ((index < 0) || (index == P3Rows.Count - 1))
            {
                P3Rows.Add(NewP3Row());
            }
            else
            {
                P3Rows.Insert(index + 1, NewP3Row());
            }

            ReorderP3Rows();
            AppHelper.RefreshGridData(p3ClauseGridView);

            p3Bookmark.Goto();
            p3ClauseGridView.MoveBy(1);

            p3ClauseGrid.Focus();
        }

        private void p3RowMinusButton_Click(object sender, EventArgs e)
        {
            int index = p3ClauseGridView.FocusedRowHandle;

            if (index < 0) return;

            p3Bookmark.Get();
            P3Rows.RemoveAt(index);

            ReorderP3Rows();
            AppHelper.RefreshGridData(p3ClauseGridView);

            p3Bookmark.Goto();

            p3ClauseGrid.Focus();
        }

        private void p4RowUpButton_Click(object sender, EventArgs e)
        {
            int index = p4FlameGridView.FocusedRowHandle;

            if (index <= 0) return;

            PhysicalPage4Row row = NewP4Row();
            row.No = P4Rows[index].No;
            row.Line = P4Rows[index].Line;
            row.Sample = P4Rows[index].Sample;
            row.BurningRate = P4Rows[index].BurningRate;

            p4Bookmark.Get();
            P4Rows.RemoveAt(index);
            P4Rows.Insert(index - 1, row);
            ReorderP4Rows();
            AppHelper.RefreshGridData(p4FlameGridView);

            p4Bookmark.Goto();
            p4FlameGridView.MoveBy(-1);

            p4FlameGrid.Focus();
        }

        private void p4RowDownButton_Click(object sender, EventArgs e)
        {
            int index = p4FlameGridView.FocusedRowHandle;

            if (index >= P4Rows.Count - 1) return;

            PhysicalPage4Row row = NewP4Row();
            row.No = P4Rows[index].No;
            row.Line = P4Rows[index].Line;
            row.Sample = P4Rows[index].Sample;
            row.BurningRate = P4Rows[index].BurningRate;

            p4Bookmark.Get();
            P4Rows.RemoveAt(index);

            if (index < P4Rows.Count - 1)
            {
                P4Rows.Insert(index + 1, row);
            }
            else
            {
                P4Rows.Add(row);
            }

            ReorderP4Rows();
            AppHelper.RefreshGridData(p4FlameGridView);

            p4Bookmark.Goto();
            p4FlameGridView.MoveBy(1);

            p4FlameGrid.Focus();
        }

        private void p4RowPluseButton_Click(object sender, EventArgs e)
        {
            int index = p4FlameGridView.FocusedRowHandle;

            p4Bookmark.Get();

            if ((index < 0) || (index == P4Rows.Count - 1))
            {
                P4Rows.Add(NewP4Row());
            }
            else
            {
                P4Rows.Insert(index + 1, NewP4Row());
            }

            ReorderP4Rows();
            AppHelper.RefreshGridData(p4FlameGridView);

            p4Bookmark.Goto();
            p4FlameGridView.MoveBy(1);

            p4FlameGrid.Focus();
        }

        private void p4RowMinusButton_Click(object sender, EventArgs e)
        {
            int index = p4FlameGridView.FocusedRowHandle;

            if (index < 0) return;

            p4Bookmark.Get();
            P4Rows.RemoveAt(index);

            ReorderP4Rows();
            AppHelper.RefreshGridData(p4FlameGridView);

            p4Bookmark.Goto();

            p4FlameGrid.Focus();
        }

        private void p5RowUpButton_Click(object sender, EventArgs e)
        {
            int index = p5StuffGridView.FocusedRowHandle;

            if (index <= 0) return;

            PhysicalPage5Row row = NewP5Row();
            row.No = P5Rows[index].No;
            row.Line = P5Rows[index].Line;
            row.TestItem = P5Rows[index].TestItem;
            row.Result = P5Rows[index].Result;
            row.Requirement = P5Rows[index].Requirement;

            p5Bookmark.Get();
            P5Rows.RemoveAt(index);
            P5Rows.Insert(index - 1, row);
            ReorderP5Rows();
            AppHelper.RefreshGridData(p5StuffGridView);

            p5Bookmark.Goto();
            p5StuffGridView.MoveBy(-1);

            p5StuffGrid.Focus();
        }

        private void p5RowDownButton_Click(object sender, EventArgs e)
        {
            int index = p5StuffGridView.FocusedRowHandle;

            if (index >= P5Rows.Count - 1) return;

            PhysicalPage5Row row = NewP5Row();
            row.No = P5Rows[index].No;
            row.Line = P5Rows[index].Line;
            row.TestItem = P5Rows[index].TestItem;
            row.Result = P5Rows[index].Result;
            row.Requirement = P5Rows[index].Requirement;

            p5Bookmark.Get();
            P5Rows.RemoveAt(index);

            if (index < P5Rows.Count - 1)
            {
                P5Rows.Insert(index + 1, row);
            }
            else
            {
                P5Rows.Add(row);
            }

            ReorderP5Rows();
            AppHelper.RefreshGridData(p5StuffGridView);

            p5Bookmark.Goto();
            p5StuffGridView.MoveBy(1);

            p5StuffGrid.Focus();
        }

        private void p5RowPluseButton_Click(object sender, EventArgs e)
        {
            int index = p5StuffGridView.FocusedRowHandle;

            p5Bookmark.Get();

            if ((index < 0) || (index == P5Rows.Count - 1))
            {
                P5Rows.Add(NewP5Row());
            }
            else
            {
                P5Rows.Insert(index + 1, NewP5Row());
            }

            ReorderP5Rows();
            AppHelper.RefreshGridData(p5StuffGridView);

            p5Bookmark.Goto();
            p5StuffGridView.MoveBy(1);

            p5StuffGrid.Focus();
        }

        private void p5RowMinusButton_Click(object sender, EventArgs e)
        {
            int index = p5StuffGridView.FocusedRowHandle;

            if (index < 0) return;

            p5Bookmark.Get();
            P5Rows.RemoveAt(index);

            ReorderP5Rows();
            AppHelper.RefreshGridData(p5StuffGridView);

            p5Bookmark.Goto();

            p5StuffGrid.Focus();
        }

        private PhysicalPage2Row NewP2Row()
        {
            PhysicalPage2Row row = new PhysicalPage2Row();

            row.No = 0;
            row.Line = false;
            row.Requested = "";
            row.Conclusion = "";

            return row;
        }

        private PhysicalPage3Row NewP3Row()
        {
            PhysicalPage3Row row = new PhysicalPage3Row();

            row.No = 0;
            row.Line = false;
            row.Clause = "";
            row.Description = "";
            row.Result = "";

            return row;
        }

        private PhysicalPage4Row NewP4Row()
        {
            PhysicalPage4Row row = new PhysicalPage4Row();

            row.No = 0;
            row.Line = false;
            row.Sample = "";
            row.BurningRate = "";

            return row;
        }

        private PhysicalPage5Row NewP5Row()
        {
            PhysicalPage5Row row = new PhysicalPage5Row();

            row.No = 0;
            row.Line = false;
            row.TestItem = "";
            row.Result = "";
            row.Requirement = "";

            return row;
        }

        private void ReorderP2Rows()
        {
            for (int i = 0; i < P2Rows.Count; i++)
            {
                P2Rows[i].No = i;
            }
        }

        private void ReorderP3Rows()
        {
            for (int i = 0; i < P3Rows.Count; i++)
            {
                P3Rows[i].No = i;
            }
        }

        private void ReorderP4Rows()
        {
            for (int i = 0; i < P4Rows.Count; i++)
            {
                P4Rows[i].No = i;
            }
        }

        private void ReorderP5Rows()
        {
            for (int i = 0; i < P5Rows.Count; i++)
            {
                P5Rows[i].No = i;
            }
        }

        private void RefreshGrid()
        {
            AppHelper.RefreshGridData(p2ResultGridView);
            AppHelper.RefreshGridData(p3ClauseGridView);
            AppHelper.RefreshGridData(p4FlameGridView);
            AppHelper.RefreshGridData(p5StuffGridView);
        }

        public void SetControlToDataSet()
        {
            p2ResultGridView.PostEditor();
            p3ClauseGridView.PostEditor();
            p4FlameGridView.PostEditor();
            p5StuffGridView.PostEditor();

            MainSet.P1ClientName = p1ClientNameEdit.Text;
            MainSet.P1ClientAddress = p1ClientAddressEdit.Text;
            MainSet.P1FileNo = p1FileNoEdit.Text;
            MainSet.P1SampleDescription = p1SampleDescriptionEdit.Text;
            //MainSet.P1DetailOfSample = p1DetailOfSampleEdit.Text;
            /*
            if (p1SampleDescriptionEdit.Text.Equals(""))
            {
                MainSet.P1DetailOfSample = "toy " + p1DetailOfSampleCombo.Text;
            }
            else
            {
                MainSet.P1DetailOfSample = p1SampleDescriptionEdit.Text + " toy " + p1DetailOfSampleCombo.Text;
            }
            */
            MainSet.P1DetailOfSample = p1DetailOfSampleCombo.Text;
            MainSet.P1ItemNo = p1ItemNoEdit.Text;
            //MainSet.P1Manufacturer = p1ManufacturerEdit.Text;
            MainSet.P1Manufacturer = p1ManufacturerComboBox.Text;
            //MainSet.P1CountryOfOrigin = p1CountryOfOriginEdit.Text;
            MainSet.P1CountryOfOrigin = p1CountryOfOriginComboBox.Text;
            MainSet.P1CountryOfDestination = p1CountryOfDestinationEdit.Text;
            MainSet.P1LabeledAge = p1LabeledAgeEdit.Text;
            MainSet.P1TestAge = p1TestAgeEdit.Text;
            //MainSet.P1AssessedAge = p1AssessedAgeEdit.Text;
            MainSet.P1AssessedAge = p1AssessedAgeCombo.Text;
            MainSet.P1ReceivedDate = p1ReceivedDateEdit.Text;
            MainSet.P1TestPeriod = p1TestPeriodEdit.Text;
            MainSet.P1TestMethod = p1TestMethodEdit.Text;
            MainSet.P1TestResults = p1TestResultEdit.Text;
            MainSet.P1Comments = p1ReportCommentEdit.Text;
            MainSet.P2Name = p2NameEdit.Text;

            MainSet.P3Description1 = p3Desc1Edit.Text;
            MainSet.P3Description2 = p3Desc2Edit.Text;
            MainSet.P4Description1 = p4Desc1Edit.Text;
            MainSet.P4Description2 = p4Desc2Edit.Text;
            MainSet.P5Description1 = p5Desc1Edit.Text;
            //MainSet.P5Description2 = p5Desc2Edit.Text;
            MainSet.P5Description2 = p5Desc2Combo.Text;
        }

        public void SetDataSetToControl()
        {
            SetDataSetToPage1();

            P2Set.MainNo = MainSet.RecNo;
            P2Set.Select();
            SetDataSetToPage2();

            P3Set.MainNo = MainSet.RecNo;
            P3Set.Select();
            SetDataSetToPage3();

            P4Set.MainNo = MainSet.RecNo;
            P4Set.Select();
            SetDataSetToPage4();

            P5Set.MainNo = MainSet.RecNo;
            P5Set.Select();
            SetDataSetToPage5();

            ImageSet.RecNo = MainSet.RecNo;
            ImageSet.Select();
            SetDataSetToPage6();

            P5Set.MainNo = MainSet.RecNo;
            P5Set.Select_PhyComplete();
            SetDataSetToPageComplete();

            EnterReportDescription();

            RefreshGrid();

            progressBar_PhyASTM_ImportWORD.Value = 0;
        }

        private void SetDataSetToPage1()
        {
            p1ClientNameEdit.Text = MainSet.P1ClientName;
            p1ClientAddressEdit.Text = MainSet.P1ClientAddress;
            p1FileNoEdit.Text = MainSet.P1FileNo;
            p1SampleDescriptionEdit.Text = MainSet.P1SampleDescription;
            //p1DetailOfSampleEdit.Text = MainSet.P1DetailOfSample;
            p1DetailOfSampleCombo.Text = MainSet.P1DetailOfSample;
            p1ItemNoEdit.Text = MainSet.P1ItemNo;
            p1OrderNoEdit.Text = MainSet.P1OrderNo;
            //p1ManufacturerEdit.Text = MainSet.P1Manufacturer;
            p1ManufacturerComboBox.Text = MainSet.P1Manufacturer;
            //p1CountryOfOriginEdit.Text = MainSet.P1CountryOfOrigin;
            p1CountryOfOriginComboBox.Text = MainSet.P1CountryOfOrigin;
            p1CountryOfDestinationEdit.Text = MainSet.P1CountryOfDestination;
            p1LabeledAgeEdit.Text = MainSet.P1LabeledAge;
            p1TestAgeEdit.Text = MainSet.P1TestAge;
            p1AssessedAgeCombo.Text = MainSet.P1AssessedAge;
            //p1AssessedAgeEdit.Text = MainSet.P1AssessedAge;
            p1ReceivedDateEdit.Text = MainSet.P1ReceivedDate;
            p1TestPeriodEdit.Text = MainSet.P1TestPeriod;
            p1TestMethodEdit.Text = MainSet.P1TestMethod;
            p1TestResultEdit.Text = MainSet.P1TestResults;
            p1ReportCommentEdit.Text = MainSet.P1Comments;

            p3Desc1Edit.Text = MainSet.P3Description1;
            p3Desc2Edit.Text = MainSet.P3Description2;
            p4Desc1Edit.Text = MainSet.P4Description1;
            p4Desc2Edit.Text = MainSet.P4Description2;
            p5Desc1Edit.Text = MainSet.P5Description1;
            p5Desc2Combo.Text = MainSet.P5Description2;
        }

        private void SetDataSetToPage2()
        {
            P2Rows.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i);

                PhysicalPage2Row p2Row = new PhysicalPage2Row();
                p2Row.No = P2Set.No;
                p2Row.Line = P2Set.Line;
                p2Row.Requested = P2Set.Requested;
                p2Row.Conclusion = P2Set.Conclusion;
                P2Rows.Add(p2Row);
            }

            p2ResultNoColumn.SortOrder = ColumnSortOrder.Ascending;
        }

        private void SetDataSetToPage3()
        {
            p3Desc1Edit.Text = MainSet.P3Description1;
            p3Desc2Edit.Text = MainSet.P3Description2;

            P3Rows.Clear();
            for (int i = 0; i < P3Set.RowCount; i++)
            {
                P3Set.Fetch(i);

                PhysicalPage3Row p3Row = new PhysicalPage3Row();
                p3Row.No = P3Set.No;
                p3Row.Line = P3Set.Line;
                p3Row.Clause = P3Set.Clause;
                p3Row.Description = P3Set.Description;
                p3Row.Result = P3Set.Result;
                p3Row.Comment = P3Set.Comment;
                P3Rows.Add(p3Row);
            }

            p3ClauseNoColumn.SortOrder = ColumnSortOrder.Ascending;
        }

        private void SetDataSetToPage4()
        {
            p4Desc1Edit.Text = MainSet.P4Description1;
            p4Desc2Edit.Text = MainSet.P4Description2;

            P4Rows.Clear();
            for (int i = 0; i < P4Set.RowCount; i++)
            {
                P4Set.Fetch(i);

                PhysicalPage4Row p4Row = new PhysicalPage4Row();
                p4Row.No = P4Set.No;
                p4Row.Line = P4Set.Line;
                p4Row.Sample = P4Set.Sample;
                p4Row.BurningRate = P4Set.BurningRate;
                P4Rows.Add(p4Row);
            }

            p4FlameNoColumn.SortOrder = ColumnSortOrder.Ascending;
        }

        private void SetDataSetToPage5()
        {
            p5Desc1Edit.Text = MainSet.P5Description1;
            p5Desc2Combo.Text = MainSet.P5Description2;

            P5Rows.Clear();
            for (int i = 0; i < P5Set.RowCount; i++)
            {
                P5Set.Fetch(i);

                PhysicalPage5Row p5Row = new PhysicalPage5Row();
                p5Row.No = P5Set.No;
                p5Row.Line = P5Set.Line;
                p5Row.TestItem = P5Set.TestItem;
                p5Row.Result = P5Set.Result;
                p5Row.Requirement = P5Set.Requirement;
                P5Rows.Add(p5Row);
            }

            //P5Set.MainNo = main;
            P5Set.Select_ReportView();
            P5Set.Fetch(0, 0, "ReportView");

            chkVisibleReport.Checked = Convert.ToBoolean(P5Set.iReportView);
            p5StuffNoColumn.SortOrder = ColumnSortOrder.Ascending;
        }

        private void SetDataSetToPage6()
        {
            ImageSet.Fetch();

            p6ImageBox.Image = ImageSet.Picture;
            p6FileNoPanel.Text = MainSet.P1FileNo;

            if (string.IsNullOrWhiteSpace(MainSet.StaffNo) == false)
            {
                SetApproval(MainSet.StaffNo);
            }

            approveButton.Visible = (AppRes.Authority == EReportAuthority.Manager) ? true : false;
        }

        private void SetDataSetToPageComplete()
        {
            //P5Set.Select_PhyComplete();
            P5Set.Fetch(0, 0, "PhyComplete");

            chkPhyComplete.Checked = Convert.ToBoolean(Convert.ToInt32(P5Set.sPhyComplete));
            chkPhyComplete_Gisool.Checked = Convert.ToBoolean(Convert.ToInt32(P5Set.sPhyComplete_GiSool));
        }

        public void SetReportView()
        {
            P5Set.iReportView = Convert.ToInt32(chkVisibleReport.Checked);
        }

        public void SetPhyComplete()
        {
            P5Set.sPhyComplete = Convert.ToString(Convert.ToInt32(chkPhyComplete.Checked));
            P5Set.sPhyComplete_GiSool = Convert.ToString(Convert.ToInt32(chkPhyComplete_Gisool.Checked));
        }

        private void SetApproval(string staffNo, bool isDbUpdate = false)
        {
            if (string.IsNullOrWhiteSpace(staffNo) == true)
            {
                staffNo = "";
                p2ImageBox.Image = null;
                p2NameEdit.Text = "";
                approveButton.Text = "Approve";
            }
            else
            {
                staffSet.StaffNo = staffNo;
                staffSet.Select();
                staffSet.Fetch();

                if (staffSet.Signature != null)
                {
                    p2ImageBox.Image = staffSet.Signature;
                    p2NameEdit.Text = staffSet.FirstName + " " + staffSet.LastName;
                    approveButton.Text = "Disapprove";
                }
                else
                {
                    staffNo = "";
                    p2ImageBox.Image = null;
                    p2NameEdit.Text = "";
                    approveButton.Text = "Approve";
                }
            }

            if (isDbUpdate == true)
            {
                MainSet.Approval = (string.IsNullOrWhiteSpace(staffNo) == true) ? false : true;
                MainSet.StaffNo = staffNo;
                MainSet.P2Name = p2NameEdit.Text;
                MainSet.UpdateApproval();
            }
        }

        private void p3ClauseComboEdit_EditValueChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(p3ClauseComboEdit.Items.IndexOf(e.NewValue).ToString());
            //int index = p3ClauseGridView.FocusedRowHandle;
            //string test = p3ClauseGridView.GetFocusedRowCellValue("Clause").ToString();


            //MessageBox.Show(p3ClauseComboEdit.Items[index].ToString());
            //MessageBox.Show(index.ToString());
            // page3 - Description
            /*
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 3;
            P2Set.sReportCategory = "astm";
            P2Set.sColumnName = "clauseDesc";
            P2Set.sColumnClause = p3ClauseComboEdit.Items[0].ToString();
            P2Set.Select_ReportDesc();

            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p3ResultComboEdit.Items.Add(P2Set.sColumnDesc.ToString());
            }
            */
        }

        private void p3ClauseComboEdit_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            /*
            // page3 - Description            
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 3;
            P2Set.sReportCategory = "astm";
            P2Set.sColumnName = "clauseDesc";
            P2Set.sColumnClause = p3ClauseComboEdit.Items[p3ClauseComboEdit.Items.IndexOf(e.NewValue)].ToString();
            P2Set.Select_ReportClause();

            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p3ClauseGridView.SetFocusedRowCellValue("Description", P2Set.sColumnDesc.ToString());

                //p3ResultComboEdit.Items.Add(P2Set.sColumnDesc.ToString());
            }
            */
        }

        private void p3ClauseComboEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxEdit editor = sender as ComboBoxEdit;
            if (editor != null)
            {
                object value = editor.SelectedItem;

                // page3 - Description
                P2Set.sDepartment = "physical";
                P2Set.iReportPage = 3;
                P2Set.sReportCategory = "astm";
                P2Set.sColumnName = "clauseDesc";
                P2Set.sColumnClause = value.ToString();
                P2Set.Select_ReportClause();

                P2Set.Fetch(0, 0, "1");
                p3ClauseGridView.SetFocusedRowCellValue("Clause", P2Set.sColumnClause.ToString());
                p3ClauseGridView.SetFocusedRowCellValue("Description", P2Set.sColumnDesc.ToString());
            }
        }

        private void p1DetailOfSampleCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnInsert.Visible = false;
            btnDelete.Visible = true;
            phyQuery.P2Set.sColumnDesc = p1DetailOfSampleCombo.SelectedItem.ToString();
            phyQuery.P2Set.sColumnCase = "detailofSample";
            //P2Set.sButtonCase = "";
        }

        private void p1AssessedAgeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnInsert.Visible = false;
            btnDelete.Visible = true;
            phyQuery.P2Set.sColumnDesc = p1AssessedAgeCombo.SelectedItem.ToString();
            phyQuery.P2Set.sColumnCase = "assessedAge";
            //P2Set.sButtonCase = "";
        }

        private void p1DetailOfSampleCombo_TextUpdate(object sender, EventArgs e)
        {
            btnInsert.Visible = true;
            btnDelete.Visible = false;
            phyQuery.P2Set.sColumnCase = "detailofSample";
            //P2Set.sButtonCase = "";
        }

        private void p1AssessedAgeCombo_TextUpdate(object sender, EventArgs e)
        {
            btnInsert.Visible = true;
            btnDelete.Visible = false;
            phyQuery.P2Set.sColumnCase = "assessedAge";
            //P2Set.sButtonCase = "";
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (phyQuery.P2Set.sColumnCase.Equals("detailofSample"))
            {
                phyQuery.P2Set.sDepartment = "physical";
                phyQuery.P2Set.iReportPage = 1;
                phyQuery.P2Set.sReportCategory = "astm";
                phyQuery.P2Set.sColumnName = "detailofSample";
                phyQuery.P2Set.sColumnDesc = p1DetailOfSampleCombo.Text;
                Insert_ReportDesc();
                btnInsert.Visible = false;
                LoadReportDescription();
                MessageBox.Show("Insert : " + phyQuery.P2Set.sColumnCase);

            }
            else if (phyQuery.P2Set.sColumnCase.Equals("assessedAge"))
            {
                phyQuery.P2Set.sDepartment = "physical";
                phyQuery.P2Set.iReportPage = 1;
                phyQuery.P2Set.sReportCategory = "astm";
                phyQuery.P2Set.sColumnName = "assessedAge";
                phyQuery.P2Set.sColumnDesc = p1AssessedAgeCombo.Text;
                Insert_ReportDesc();
                btnInsert.Visible = false;
                LoadReportDescription();
                MessageBox.Show("Insert : " + phyQuery.P2Set.sColumnCase);
            }
            else
            {
                MessageBox.Show("Insert else!");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (phyQuery.P2Set.sColumnCase.Equals("detailofSample"))
            {
                phyQuery.P2Set.sDepartment = "physical";
                phyQuery.P2Set.iReportPage = 1;
                phyQuery.P2Set.sReportCategory = "astm";
                phyQuery.P2Set.sColumnName = "detailofSample";
                Delete_ReportDesc();
                btnDelete.Visible = false;
                LoadReportDescription();
                MessageBox.Show("Delete : " + phyQuery.P2Set.sColumnCase);
            }
            else if (phyQuery.P2Set.sColumnCase.Equals("assessedAge"))
            {
                phyQuery.P2Set.sDepartment = "physical";
                phyQuery.P2Set.iReportPage = 1;
                phyQuery.P2Set.sReportCategory = "astm";
                phyQuery.P2Set.sColumnName = "assessedAge";
                Delete_ReportDesc();
                btnDelete.Visible = false;
                LoadReportDescription();
                MessageBox.Show("Delete : " + phyQuery.P2Set.sColumnCase);
            }
            else
            {
                MessageBox.Show("Delete else!");
            }
        }

        public void Insert_ReportDesc()
        {
            SqlTransaction trans = AppRes.DB.BeginTrans();

            try
            {
                phyQuery.P2Set.Insert_ReportDesc(trans);
                AppRes.DB.CommitTrans();
            }
            catch
            {
                AppRes.DB.RollbackTrans();
            }
        }

        public void Delete_ReportDesc()
        {
            SqlTransaction trans = AppRes.DB.BeginTrans();

            try
            {
                phyQuery.P2Set.Delete_ReportDesc(trans);
                AppRes.DB.CommitTrans();
            }
            catch
            {
                AppRes.DB.RollbackTrans();
            }
        }

        public void LoadReportDescription()
        {
            // page1 - Detail of Sample
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 1;
            P2Set.sReportCategory = "astm";
            P2Set.sColumnName = "detailofsample";
            P2Set.Select_ReportDesc();

            // Initialize
            p1DetailOfSampleCombo.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p1DetailOfSampleCombo.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page1 - Manufacturer
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 1;
            P2Set.sReportCategory = "astm";
            P2Set.sColumnName = "manuFacturer";
            P2Set.Select_ReportDesc();

            // Initialize
            p1ManufacturerComboBox.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p1ManufacturerComboBox.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page1 - Country Of Origin
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 1;
            P2Set.sReportCategory = "astm";
            P2Set.sColumnName = "countryOfOrigin";
            P2Set.Select_ReportDesc();

            // Initialize
            p1CountryOfOriginComboBox.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p1CountryOfOriginComboBox.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page1 - Assessed Age
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 1;
            P2Set.sReportCategory = "astm";
            P2Set.sColumnName = "assessedAge";
            P2Set.Select_ReportDesc();

            // Initialize
            p1AssessedAgeCombo.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p1AssessedAgeCombo.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page2 - Test Requested
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 2;
            P2Set.sReportCategory = "astm";
            P2Set.sColumnName = "testRequested";
            P2Set.Select_ReportDesc();

            // Initialize
            p2TestRequestedTextCombo.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p2TestRequestedTextCombo.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page2 - Conclusion
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 2;
            P2Set.sReportCategory = "astm";
            P2Set.sColumnName = "conclusion";
            P2Set.Select_ReportDesc();

            // Initialize
            p2ConclusionTextCombo.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p2ConclusionTextCombo.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page3 - Clause
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 3;
            P2Set.sReportCategory = "astm";
            P2Set.sColumnName = "clauseDesc";
            P2Set.Select_ReportNonEmptyClause();

            // Initialize
            p3ClauseComboEdit.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p3ClauseComboEdit.Items.Add(P2Set.sColumnClause.ToString());
            }

            // page3 - Description
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 3;
            P2Set.sReportCategory = "astm";
            P2Set.sColumnName = "clauseDesc";
            P2Set.Select_ReportEmptyClause();

            // Initialize
            p3ResultComboEdit.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p3DescComboEdit.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page3 - Result
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 3;
            P2Set.sReportCategory = "astm";
            P2Set.sColumnName = "result";
            P2Set.Select_ReportDesc();

            // Initialize
            p3ResultComboEdit.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p3ResultComboEdit.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page4 - Burnning Rate
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 4;
            P2Set.sReportCategory = "astm";
            P2Set.sColumnName = "burnningRate";
            P2Set.Select_ReportDesc();

            // Initialize
            p4BurnningRateComboEdit.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p4BurnningRateComboEdit.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page5 - polyester
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 5;
            P2Set.sReportCategory = "astm";
            P2Set.sColumnName = "polyester";
            P2Set.Select_ReportDesc();

            // Initialize
            p5Desc2Combo.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p5Desc2Combo.Items.Add(P2Set.sColumnDesc.ToString());
            }
        }

        public void EnterReportDescription()
        {
            if (p1SampleDescriptionEdit.Text.Equals(""))
            {
                p1DetailOfSampleEdit2.Text = "";
            }
            else
            {
                p1DetailOfSampleEdit2.Text = p1SampleDescriptionEdit.Text + " toy";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnImportWord_ASTM_Click(object sender, EventArgs e)
        {
            progressBar_PhyASTM_ImportWORD.Value = 0;

            frmImportWord fImportWordToDT = new frmImportWord();
            fImportWordToDT.ShowDialog();

            dtGet1 = fImportWordToDT.GetVariable1();
            dtGet2 = fImportWordToDT.GetVariable2();
            //DT3-result summary
            dtGet3 = fImportWordToDT.GetVariable3();
            //DT4 -- sample table
            dtGet4 = fImportWordToDT.GetVariable4();
            //-	What kinds of stuffing
            dtGet5 = fImportWordToDT.GetVariable5();
            //PAGE COMPLETE
            dtGet6 = fImportWordToDT.GetVariable6();

            P2Set.MainNo = MainSet.RecNo;
            P2Set.sColumnDesc_4_3_7_Result = "";

            progressBar_PhyASTM_ImportWORD.PerformStep();

            try
            {
                if (dtGet1.Rows.Count > 0)
                {
                    foreach (DataRow row in dtGet1.Rows)
                    {
                        P2Set.sColumnClause = row["Report Job No."].ToString();
                        P2Set.sLabeledAge = row["Labeled Age Grading"].ToString();
                        P2Set.sRequestAge = row["Requested Age Grading"].ToString();
                        P2Set.sTestAge = row["Age Group Applied in Testing"].ToString();
                        P2Set.sSampleDescription = row["Sample description"].ToString();
                        P2Set.sDetailOfSample = row["Detail of sample"].ToString();
                        P2Set.Update_Main();
                    }
                }

                progressBar_PhyASTM_ImportWORD.PerformStep();

                if (dtGet2.Rows.Count > 0)
                {
                    int i = 0;
                    P2Set.Delete_PhyPage3();

                    foreach (DataRow row in dtGet2.Rows)
                    {
                        P2Set.iColumnNo = i;
                        P2Set.bColumnLine = false;
                        P2Set.sColumnClause = row["Clause"].ToString();
                        P2Set.sColumnDesc = row["Description"].ToString();
                        P2Set.sColumnResult = row["Result"].ToString();
                        P2Set.sColumnRemark = row["Remark"].ToString();
                        P2Set.sColumnComment = "";


                        if (P2Set.sColumnClause.Trim().Equals("4.1"))
                        {
                            P2Set.sColumnDesc = "Material Quality **";
                        }

                        if (P2Set.sColumnClause.Trim().Equals("4.2"))
                        {
                            P2Set.sColumnDesc = "Flammability Test (16 CFR 1500.44)";
                        }

                        if (P2Set.sColumnClause.Trim().Equals("4.3.5"))
                        {
                            P2Set.sColumnDesc = "Heavy Elements";
                        }

                        if (P2Set.sColumnClause.Trim().Equals("4.3.7") && P2Set.sColumnResult.Trim().Equals(""))
                        {
                            P2Set.sColumnDesc_4_3_7_Result = "None";
                        }

                        if (P2Set.sColumnClause.Trim().Equals("4.7"))
                        {
                            P2Set.sColumnDesc = "Accessible Edges (16 CFR 1500.49)";
                        }

                        if (P2Set.sColumnClause.Trim().Equals("4.9"))
                        {
                            P2Set.sColumnDesc = "Accessible Points (16 CFR 1500.48)";
                        }
                        /*
                        if (P2Set.sColumnClause.Trim().Equals("4.3.5.1"))
                        {
                            P2Set.sColumnResult = "-";
                        }

                        if (P2Set.sColumnClause.Trim().Equals("4.3.5.2"))
                        {
                            P2Set.sColumnResult = "-";
                        }
                        */
                        P2Set.Insert_PhyPage3();

                        if (P2Set.sColumnRemark.Trim().Equals("Remark: Any toy or game that is intended for use by children who are at least three years old (36 months) but less than six years of age (72 months) and includes a small part is subject to the labeling requirements in accordance with 5.11.2."))
                        {
                            P2Set.sColumnClause = "";
                            P2Set.sColumnDesc = P2Set.sColumnRemark;
                            P2Set.sColumnResult = "";
                            P2Set.Insert_PhyPage3();
                        }
                        else if (P2Set.sColumnRemark.Trim().Equals("Remark: Toys containing non-replaceable batteries shall be labeled in accordance with 5.15."))
                        {
                            P2Set.sColumnClause = "";
                            P2Set.sColumnDesc = P2Set.sColumnRemark;
                            P2Set.sColumnResult = "";
                            P2Set.Insert_PhyPage3();
                        }
                        else if (P2Set.sColumnRemark.Trim().Equals("Remark: Toys with non-replaceable batteries that are accessible with the use of a coin, screwdriver, or other common household tool shall bear a statement that the battery is not replaceable"))
                        {
                            P2Set.sColumnClause = "";
                            P2Set.sColumnDesc = P2Set.sColumnRemark;
                            P2Set.sColumnResult = "";
                            P2Set.Insert_PhyPage3();
                        }
                        else if (P2Set.sColumnRemark.Trim().Equals("Remark: The toy or package should be age labeled"))
                        {
                            P2Set.sColumnClause = "";
                            P2Set.sColumnDesc = P2Set.sColumnRemark;
                            P2Set.sColumnResult = "";
                            P2Set.Insert_PhyPage3();
                        }
                        else if (P2Set.sColumnRemark.Trim().Equals("Remark: It is drawn to your attention that the toy or its packaging shall be marked with appropriate small part warning in accordance with 16 CFR 1500.19"))
                        {
                            P2Set.sColumnClause = "";
                            P2Set.sColumnDesc = P2Set.sColumnRemark;
                            P2Set.sColumnResult = "";
                            P2Set.Insert_PhyPage3();
                        }
                        else if (P2Set.sColumnRemark.Trim().Equals("Remark: The toy should be marked with name and address of the producer or the distributor"))
                        {
                            P2Set.sColumnClause = "";
                            P2Set.sColumnDesc = P2Set.sColumnRemark;
                            P2Set.sColumnResult = "";
                            P2Set.Insert_PhyPage3();
                        }
                        else if (P2Set.sColumnRemark.Trim().Equals("Remark: Washing was conducted in one trial as per client’s request"))
                        {
                            P2Set.sColumnClause = "";
                            P2Set.sColumnDesc = P2Set.sColumnRemark;
                            P2Set.sColumnResult = "";
                            P2Set.Insert_PhyPage3();
                        }
                        else
                        {
                            Console.WriteLine("insert none!");
                        }
                        i++;
                    }
                }

                progressBar_PhyASTM_ImportWORD.PerformStep();

                if (dtGet3.Rows.Count > 0)
                {
                    int i = 0;
                    P2Set.Delete_PhyPage2();

                    foreach (DataRow row in dtGet3.Rows)
                    {
                        P2Set.iColumnNo = i;
                        P2Set.bColumnLine = false;
                        P2Set.sTestRequested = row["Test Requested"].ToString();
                        P2Set.sConclusion = row["Conclusion"].ToString();
                        P2Set.Insert_PhyPage2();
                        i++;
                    }
                }

                progressBar_PhyASTM_ImportWORD.PerformStep();

                if (dtGet4.Rows.Count > 0)
                {
                    //int i = 0, j = 0;
                    int i = 0;
                    P2Set.Delete_PhyPage41();
                    //P2Set.Select_Phy41();
                    //P2Set.Fetch();
                    //j = P2Set.RowCount;

                    foreach (DataRow row in dtGet4.Rows)
                    {
                        //P2Set.iColumnNo = j + i;
                        P2Set.iColumnNo = i;
                        P2Set.bColumnLine = false;
                        //P2Set.sBurningRate = row["Actual Burn Rate (in./s)"].ToString();
                        P2Set.sBurningRate = row["Result"].ToString();
                        P2Set.sSample = row["Sample"].ToString();
                        P2Set.Insert_PhyPage41();
                        i++;
                    }
                }

                progressBar_PhyASTM_ImportWORD.PerformStep();

                if (dtGet5.Rows.Count > 0)
                {
                    foreach (DataRow row in dtGet5.Rows)
                    {
                        if (!P2Set.sColumnDesc_4_3_7_Result.Equals("None"))
                        {
                            P2Set.sP5desc2 = row["what kind of stuffing"].ToString();
                            P2Set.sColumnDesc_4_3_7_Report_View = "0";

                            P2Set.Update_Polyester();
                            P2Set.Update_ReportView();
                        }
                        else
                        {
                            P2Set.sP5desc2 = "";
                            P2Set.sColumnDesc_4_3_7_Report_View = "1";

                            P2Set.Update_Polyester();
                            P2Set.Update_ReportView();
                        }
                    }
                }

                progressBar_PhyASTM_ImportWORD.PerformStep();

                if (dtGet6.Rows.Count > 0)
                {
                    foreach (DataRow row in dtGet6.Rows)
                    {
                        if (row["COMPLETE_실무자"].ToString().ToUpper().Equals("C"))
                        {
                            P2Set.iComplete = 1;
                            P2Set.Update_Complete();
                        }
                        else if (row["COMPLETE_실무자"].ToString().ToUpper().Equals(""))
                        {
                            P2Set.iComplete = 0;
                            P2Set.Update_Complete();
                        }
                        if (row["COMPLETE_기술책임자"].ToString().ToUpper().Equals("C"))
                        {
                            P2Set.iComplete_Gisool = 1;
                            P2Set.Update_Complete_Gisool();
                        }
                        else if (row["COMPLETE_기술책임자"].ToString().ToUpper().Equals(""))
                        {
                            P2Set.iComplete_Gisool = 0;
                            P2Set.Update_Complete_Gisool();
                        }
                    }
                }

                // Step11
                progressBar_PhyASTM_ImportWORD.PerformStep();
                SetDataSetToControl();

                //findButton.PerformClick();
                MessageBox.Show("Completed!");
            }
            catch (Exception f)
            {
                MessageBox.Show(f.Source + Environment.NewLine + f.Message);
            }
        }
    }
}