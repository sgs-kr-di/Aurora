using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Ulee.Controls;
using DevExpress.Data;
using DevExpress.XtraEditors;
using Sgs.ReportIntegration.Source.A.Forms.B.Dialog;

namespace Sgs.ReportIntegration
{
    public partial class CtrlEditPhysicalEu : UlUserControlEng
    {
        public CtrlEditPhysical ctrlPhysicalMain;

        public PhysicalMainTapDataSet ctrlPhysicalMainTap;

        public PhysicalMainDataSet MainSet;

        public PhysicalImageDataSet ImageSet;

        public PhysicalP2DataSet P2Set;

        public PhysicalP3DataSet P3Set;

        public PhysicalP40DataSet P40Set;

        public PhysicalP41DataSet P41Set;

        public PhysicalP42DataSet P42Set;

        public PhysicalP44DataSet P44Set;

        public PhysicalP45DataSet P45Set;

        public PhysicalP5DataSet P5Set;

        public PhysicalP6DataSet P6Set;

        public List<PhysicalPage2Row> P2Rows;

        public List<PhysicalPage3Row> P3Rows;

        public List<PhysicalPage3Row> P40Rows;

        public List<PhysicalPage4Row> P41Rows;

        public List<PhysicalPage4Row> P42Rows;

        public List<PhysicalPage4_4_4_2_2_4Row> P422Rows;

        public List<PhysicalPage4_4_4_2_2_4Row> P423Rows;

        public List<PhysicalPage4_4_4_2_2_4Row> P424Rows;

        public List<PhysicalPage4_5_4Row> P45_425Rows;

        public List<PhysicalPage4_5_4Row> P45_43Rows;

        public List<PhysicalPage4_5_4Row> P45_44Rows;

        public List<PhysicalPage4_5_4Row> P45_45Rows;

        public List<PhysicalPage5Row> P5Rows;

        public List<PhysicalPage6Row> P6Rows;

        private GridBookmark p2Bookmark;

        private GridBookmark p3Bookmark;

        private GridBookmark p5Bookmark;

        private GridBookmark p6Bookmark;

        private StaffDataSet staffSet;

        private Button findButton;

        public DataTable dtGet1;

        public DataTable dtGet2;

        public DataTable dtGet3;

        public DataTable dtGet4;

        public DataTable dtGet5_1;

        public DataTable dtGet5_2;

        public DataTable dtGet6;

        public DataTable dtGet7;

        public DataTable dtGet8;

        public DataTable dtGet9;

        public DataTable dtGet10;

        public CtrlEditPhysicalEu(Button findButton)
        {
            this.findButton = findButton;

            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            ctrlPhysicalMainTap = new PhysicalMainTapDataSet(AppRes.DB.Connect, null, null);

            progressBar_PhyEU_ImportWORD.Style = ProgressBarStyle.Continuous;
            progressBar_PhyEU_ImportWORD.Minimum = 0;
            progressBar_PhyEU_ImportWORD.Maximum = 120;
            progressBar_PhyEU_ImportWORD.Step = 10;
            progressBar_PhyEU_ImportWORD.Value = 0;           

            staffSet = new StaffDataSet(AppRes.DB.Connect, null, null);

            p2Bookmark = new GridBookmark(p2ResultGridView);
            P2Rows = new List<PhysicalPage2Row>();
            p2ResultGrid.DataSource = P2Rows;
            AppHelper.SetGridEvenRow(p2ResultGridView);

            p3Bookmark = new GridBookmark(p3ClauseGridView);
            P3Rows = new List<PhysicalPage3Row>();
            p3ClauseGrid.DataSource = P3Rows;
            AppHelper.SetGridEvenRow(p3ClauseGridView);

            P40Rows = new List<PhysicalPage3Row>();
            p4ClauseGrid.DataSource = P40Rows;
            AppHelper.SetGridEvenRow(p4ClauseGridView);

            P41Rows = new List<PhysicalPage4Row>();
            p4Sample1Grid.DataSource = P41Rows;
            AppHelper.SetGridEvenRow(p4Sample1GridView);

            /*
            P42Rows = new List<PhysicalPage4Row>();
            p4_4Sample2Grid.DataSource = P42Rows;
            AppHelper.SetGridEvenRow(p4_4Sample2GridView);
            */

            P422Rows = new List<PhysicalPage4_4_4_2_2_4Row>();
            p4_4Sample2Grid.DataSource = P422Rows;
            AppHelper.SetGridEvenRow(p4_4Sample2GridView);

            P423Rows = new List<PhysicalPage4_4_4_2_2_4Row>();
            p4_4Sample3Grid.DataSource = P423Rows;
            AppHelper.SetGridEvenRow(p4_4Sample3GridView);

            P424Rows = new List<PhysicalPage4_4_4_2_2_4Row>();
            p4_4Sample4Grid.DataSource = P424Rows;
            AppHelper.SetGridEvenRow(p4_4Sample4GridView);

            P45_425Rows = new List<PhysicalPage4_5_4Row>();
            p4_5Sample425Grid.DataSource = P45_425Rows;
            AppHelper.SetGridEvenRow(p4_5Sample425GridView);

            P45_43Rows = new List<PhysicalPage4_5_4Row>();
            p4_5Sample43Grid.DataSource = P45_43Rows;
            AppHelper.SetGridEvenRow(p4_5Sample43GridView);

            P45_44Rows = new List<PhysicalPage4_5_4Row>();
            p4_5Sample44Grid.DataSource = P45_44Rows;
            AppHelper.SetGridEvenRow(p4_5Sample44GridView);

            P45_45Rows = new List<PhysicalPage4_5_4Row>();
            p4_5Sample45Grid.DataSource = P45_45Rows;
            AppHelper.SetGridEvenRow(p4_5Sample45GridView);

            p5Bookmark = new GridBookmark(p5StuffGridView);
            P5Rows = new List<PhysicalPage5Row>();
            p5StuffGrid.DataSource = P5Rows;
            AppHelper.SetGridEvenRow(p5StuffGridView);

            p6Bookmark = new GridBookmark(p6StuffGridView);
            P6Rows = new List<PhysicalPage6Row>();
            p6StuffGrid.DataSource = P6Rows;
            AppHelper.SetGridEvenRow(p6StuffGridView);            
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

        private void physical1Page_Resize(object sender, EventArgs e)
        {
            int width = physical1Page.Width;

            p1ClientNameEdit.Width = width - 172;
            p1ClientAddressEdit.Width = width - 172;
            p1SampleDescriptionEdit.Width = width - 410;
            //p1DetailOfSampleEdit.Width = width - 172;
            p1DetailOfSamplecomboBoxEdit.Width = width - 275;
            p1OrderNoEdit.Width = width - 410;
            p1BuyerEdit.Width = width - 410;
            p1InstructionEdit.Width = width - 172;
            //p1ManufacturerEdit.Width = width - 172;
            p1ManufacturerComboBox.Width = width - 172;
            p1CountryOfDestinationEdit.Width = width - 410;
            p1LabeledAgeEdit.Width = width - 172;
            p1TestAgeEdit.Width = width - 172;
            //p1AssessedAgeEdit.Width = width - 172;
            p1AssessedAgecomboBoxEdit.Width = width - 172;
            p1TestPeriodEdit.Width = width - 410;
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

            p3ClauseGrid.Size = new Size(width - 8, height - 128);
            p3ClauseDescriptionColumn.Width = width - 242;

            p3RowUpButton.Left = width - 106;
            p3RowDownButton.Left = width - 80;
            p3RowPluseButton.Left = width - 54;
            p3RowMinusButton.Left = width - 28;

            p3Desc1Edit.Width = width - 8;
        }

        private void physical4Page_Resize(object sender, EventArgs e)
        {
            int width = physical4Page.Width;
            int height = physical4Page.Height;
            int colWidth = width - 12;

            p4ClauseGrid.Width = width - 8;
            //p4ClauseGrid.Height = height - 730;
            p40DescriptionColumn.Width = width - 238;

            p4Sample1Grid.Width = width - 8;
            p41SampleColumn.Width = colWidth / 2;
            p41ResultColumn.Width = colWidth / 2;

            p4_4Sample2Grid.Width = width - 8;
            p42SampleColumn.Width = colWidth / 2;
            p42ResultColumn.Width = colWidth / 2;

            if (colWidth % 2 == 0)
            {
                p41SampleColumn.Width++;
                p42SampleColumn.Width++;
            }
            else
            {
                p41SampleColumn.Width++;
                p41ResultColumn.Width++;
                p42SampleColumn.Width++;
                p42ResultColumn.Width++;
            }

            p4Desc1Edit.Width = width - 8;
            p4Desc2Edit.Width = width - 8;
            p4_4Desc2Edit.Width = width - 8;
            p4Desc4Edit.Size = new Size(width - 8, height - 400);
        }

        private void physical4_4Page_Resize(object sender, EventArgs e)
        {
            int width = physical4_4Page.Width;
            int height = physical4_4Page.Height;
            int colWidth = width - 12;

            p4_4Sample2Grid.Width = width - 8;
            p42SampleColumn.Width = colWidth / 2;
            p42ResultColumn.Width = colWidth / 2;

            p4_4Sample3Grid.Width = width - 8;
            gridColumn2.Width = colWidth / 2;
            gridColumn3.Width = colWidth / 2;

            p4_4Sample4Grid.Width = width - 8;
            gridColumn11.Width = colWidth / 2;
            gridColumn12.Width = colWidth / 2;

            if (colWidth % 2 == 0)
            {
                p42SampleColumn.Width++;
                gridColumn2.Width++;
                gridColumn11.Width++;
            }
            else
            {
                p42SampleColumn.Width++;
                gridColumn2.Width++;
                gridColumn11.Width++;
                gridColumn3.Width++;
                gridColumn12.Width++;
            }

            p4_4Desc3Edit.Width = width - 8;
            p4_4Desc4Edit.Width = width - 8;

            p4Desc4_2_2Edit.Width = width - 8;
            p4Desc4_2_3Edit.Width = width - 8;
            p4Desc4_2_4Edit.Width = width - 8;
            p4_4Desc1Edit.Width = width - 8;
            p4_4Desc2Edit.Width = width - 8;
            //p4Desc4_2_2Edit.Size = new Size(width - 8, height - 870);
            //p4Desc4_2_3Edit.Size = new Size(width - 8, height - 870);
            //p4Desc4_2_4Edit.Size = new Size(width - 8, height - 870);
        }

        private void physical4_5Page_Resize(object sender, EventArgs e)
        {
            int width = physical4_5Page.Width;
            int height = physical4_5Page.Height;
            int colWidth = width - 12;

            p4_5Sample425Grid.Width = width - 8;
            p4_5Sample43Grid.Width = width - 8;
            p4_5Sample44Grid.Width = width - 8;
            p4_5Sample45Grid.Width = width - 8;

            p45_425SampleColumn.Width = colWidth / 3;
            p45_425BurningRateColumn.Width = colWidth / 3;
            p45_425ResultColumn.Width = colWidth / 3;

            p45_43SampleColumn.Width = colWidth / 3;
            p45_43BurningRateColumn.Width = colWidth / 3;
            p45_43ResultColumn.Width = colWidth / 3;

            p45_44SampleColumn.Width = colWidth / 3;
            p45_44BurningRateColumn.Width = colWidth / 3;
            p45_44ResultColumn.Width = colWidth / 3;

            p45_45SampleColumn.Width = colWidth / 3;
            p45_45BurningRateColumn.Width = colWidth / 3;
            p45_45ResultColumn.Width = colWidth / 3;

            if (colWidth % 3 == 0)
            {
                p45_425SampleColumn.Width++;
                p45_425BurningRateColumn.Width++;
                p45_425ResultColumn.Width++;
            }
            else
            {
                p45_425SampleColumn.Width++;
                p45_425BurningRateColumn.Width++;
                p45_425ResultColumn.Width++;

                p45_43SampleColumn.Width++;
                p45_43BurningRateColumn.Width++;
                p45_43ResultColumn.Width++;

                p45_44SampleColumn.Width++;
                p45_44BurningRateColumn.Width++;
                p45_44ResultColumn.Width++;

                p45_45SampleColumn.Width++;
                p45_45BurningRateColumn.Width++;
                p45_45ResultColumn.Width++;
            }

            p4_5Desc1Edit.Width = width - 8;
            p4_5Desc425Edit.Width = width - 8;
            p4_5Desc425MemoEdit.Width = width - 8;

            p4_5Desc43Edit.Width = width - 8;
            p4_5Desc43MemoEdit.Width = width - 8;

            p4_5Desc44Edit.Width = width - 8;
            p4_5Desc44MemoEdit.Width = width - 8;

            p4_5Desc45Edit.Width = width - 8;
            p4_5Desc45MemoEdit.Width = width - 8;
        }

        private void physical5Page_Resize(object sender, EventArgs e)
        {
            int width = physical5Page.Width;
            int height = physical5Page.Height;

            p5StuffGrid.Width = width - 8;
            p5StuffTestItemColumn.Width = 183 + (width - 548) / 2;
            p5StuffRequirementColumn.Width = 183 + (width - 548) / 2;

            p5RowUpButton.Left = width - 106;
            p5RowDownButton.Left = width - 80;
            p5RowPluseButton.Left = width - 54;
            p5RowMinusButton.Left = width - 28;

            p5Desc1Edit.Width = width - 8;
            p5Desc2Edit.Size = new Size(width - 8, height - 313);
        }

        private void physical6Page_Resize(object sender, EventArgs e)
        {
            imagePanel.Size = new Size(physical7Page.Width - 16, physical7Page.Height - 70);
            p6ImageBox.Size = new Size(imagePanel.Width - 16, imagePanel.Height - 74);
            p6DescPanel.Width = imagePanel.Width - 16;

            p6FileNoPanel.Top = physical7Page.Height - 56;
            p6FileNoPanel.Width = physical7Page.Width - 16;
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

        //private void p40RowUpButton_Click(object sender, EventArgs e)
        //{
        //    int index = p4ClauseGridView.FocusedRowHandle;

        //    if (index <= 0) return;

        //    PhysicalPage3Row row = NewP40Row();
        //    row.No = P40Rows[index].No;
        //    row.Line = P40Rows[index].Line;
        //    row.Clause = P40Rows[index].Clause;
        //    row.Description = P40Rows[index].Description;
        //    row.Result = P40Rows[index].Result;

        //    p3Bookmark.Get();
        //    P40Rows.RemoveAt(index);
        //    P40Rows.Insert(index - 1, row);
        //    ReorderP40Rows();
        //    AppHelper.RefreshGridData(p4ClauseGridView);

        //    p3Bookmark.Goto();
        //    p4ClauseGridView.MoveBy(-1);

        //    p4ClauseGridView.Focus();
        //}

        //private void p40RowDownButton_Click(object sender, EventArgs e)
        //{
        //    int index = p4ClauseGridView.FocusedRowHandle;

        //    if (index >= P40Rows.Count - 1) return;

        //    PhysicalPage3Row row = NewP40Row();
        //    row.No = P40Rows[index].No;
        //    row.Line = P40Rows[index].Line;
        //    row.Clause = P40Rows[index].Clause;
        //    row.Description = P40Rows[index].Description;
        //    row.Result = P40Rows[index].Result;

        //    p3Bookmark.Get();
        //    P40Rows.RemoveAt(index);

        //    if (index < P40Rows.Count - 1)
        //    {
        //        P40Rows.Insert(index + 1, row);
        //    }
        //    else
        //    {
        //        P40Rows.Add(row);
        //    }

        //    ReorderP40Rows();
        //    AppHelper.RefreshGridData(p4ClauseGridView);

        //    p3Bookmark.Goto();
        //    p4ClauseGridView.MoveBy(1);

        //    p4ClauseGrid.Focus();
        //}

        //private void p40RowPluseButton_Click(object sender, EventArgs e)
        //{
        //    int index = p4ClauseGridView.FocusedRowHandle;

        //    p3Bookmark.Get();

        //    if ((index < 0) || (index == P40Rows.Count - 1))
        //    {
        //        P40Rows.Add(NewP40Row());
        //    }
        //    else
        //    {
        //        P40Rows.Insert(index + 1, NewP40Row());
        //    }

        //    ReorderP40Rows();
        //    AppHelper.RefreshGridData(p4ClauseGridView);

        //    p3Bookmark.Goto();
        //    p4ClauseGridView.MoveBy(1);

        //    p4ClauseGrid.Focus();
        //}

        //private void p40RowMinusButton_Click(object sender, EventArgs e)
        //{
        //    int index = p4ClauseGridView.FocusedRowHandle;

        //    if (index < 0) return;

        //    p3Bookmark.Get();
        //    P40Rows.RemoveAt(index);

        //    ReorderP40Rows();
        //    AppHelper.RefreshGridData(p4ClauseGridView);

        //    p3Bookmark.Goto();

        //    p4ClauseGrid.Focus();
        //}

        //private void p41RowUpButton_Click(object sender, EventArgs e)
        //{
        //    int index = p4SampleGridView.FocusedRowHandle;

        //    if (index <= 0) return;

        //    PhysicalPage4Row row = NewP41Row();
        //    row.No = P41Rows[index].No;
        //    row.Line = P41Rows[index].Line;
        //    row.Sample = P41Rows[index].Sample;
        //    row.BurningRate = P41Rows[index].BurningRate;

        //    p41Bookmark.Get();
        //    P41Rows.RemoveAt(index);
        //    P41Rows.Insert(index - 1, row);
        //    ReorderP41Rows();
        //    AppHelper.RefreshGridData(p4SampleGridView);

        //    p41Bookmark.Goto();
        //    p4SampleGridView.MoveBy(-1);

        //    p4Sample1Grid.Focus();
        //}

        //private void p41RowDownButton_Click(object sender, EventArgs e)
        //{
        //    int index = p4SampleGridView.FocusedRowHandle;

        //    if (index >= P41Rows.Count - 1) return;

        //    PhysicalPage4Row row = NewP41Row();
        //    row.No = P41Rows[index].No;
        //    row.Line = P41Rows[index].Line;
        //    row.Sample = P41Rows[index].Sample;
        //    row.BurningRate = P41Rows[index].BurningRate;

        //    p41Bookmark.Get();
        //    P41Rows.RemoveAt(index);

        //    if (index < P41Rows.Count - 1)
        //    {
        //        P41Rows.Insert(index + 1, row);
        //    }
        //    else
        //    {
        //        P41Rows.Add(row);
        //    }

        //    ReorderP41Rows();
        //    AppHelper.RefreshGridData(p4SampleGridView);

        //    p41Bookmark.Goto();
        //    p4SampleGridView.MoveBy(1);

        //    p4Sample1Grid.Focus();
        //}

        //private void p41RowPluseButton_Click(object sender, EventArgs e)
        //{
        //    int index = p4SampleGridView.FocusedRowHandle;

        //    p41Bookmark.Get();

        //    if ((index < 0) || (index == P41Rows.Count - 1))
        //    {
        //        P41Rows.Add(NewP41Row());
        //    }
        //    else
        //    {
        //        P41Rows.Insert(index + 1, NewP41Row());
        //    }

        //    ReorderP41Rows();
        //    AppHelper.RefreshGridData(p4SampleGridView);

        //    p41Bookmark.Goto();
        //    p4SampleGridView.MoveBy(1);

        //    p4Sample1Grid.Focus();
        //}

        //private void p41RowMinusButton_Click(object sender, EventArgs e)
        //{
        //    int index = p4SampleGridView.FocusedRowHandle;

        //    if (index < 0) return;

        //    p41Bookmark.Get();
        //    P41Rows.RemoveAt(index);

        //    ReorderP41Rows();
        //    AppHelper.RefreshGridData(p4SampleGridView);

        //    p41Bookmark.Goto();

        //    p4Sample1Grid.Focus();
        //}

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

        //private PhysicalPage3Row NewP40Row()
        //{
        //    PhysicalPage3Row row = new PhysicalPage3Row();

        //    row.No = 0;
        //    row.Line = false;
        //    row.Clause = "";
        //    row.Description = "";
        //    row.Result = "";

        //    return row;
        //}

        //private PhysicalPage4Row NewP41Row()
        //{
        //    PhysicalPage4Row row = new PhysicalPage4Row();

        //    row.No = 0;
        //    row.Line = false;
        //    row.Sample = "";
        //    row.BurningRate = "";

        //    return row;
        //}

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

        //private void ReorderP40Rows()
        //{
        //    for (int i = 0; i < P40Rows.Count; i++)
        //    {
        //        P40Rows[i].No = i;
        //    }
        //}

        //private void ReorderP41Rows()
        //{
        //    for (int i = 0; i < P41Rows.Count; i++)
        //    {
        //        P41Rows[i].No = i;
        //    }
        //}

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
            AppHelper.RefreshGridData(p4ClauseGridView);
            AppHelper.RefreshGridData(p4Sample1GridView);
            AppHelper.RefreshGridData(p4_4Sample2GridView);
            AppHelper.RefreshGridData(p5StuffGridView);
                        
            AppHelper.RefreshGridData(p4_4Sample2GridView);
            AppHelper.RefreshGridData(p4_4Sample3GridView);
            AppHelper.RefreshGridData(p4_4Sample4GridView);

            AppHelper.RefreshGridData(p4_5Sample425GridView);
            AppHelper.RefreshGridData(p4_5Sample43GridView);
            AppHelper.RefreshGridData(p4_5Sample44GridView);
            AppHelper.RefreshGridData(p4_5Sample45GridView);

            AppHelper.RefreshGridData(p6StuffGridView);
        }

        public void SetControlToDataSet()
        {
            p2ResultGridView.PostEditor();
            p3ClauseGridView.PostEditor();
            p4ClauseGridView.PostEditor();
            p4Sample1GridView.PostEditor();
            p4_4Sample2GridView.PostEditor();
            p5StuffGridView.PostEditor();

            MainSet.P1ClientName = p1ClientNameEdit.Text;
            MainSet.P1ClientAddress = p1ClientAddressEdit.Text;
            MainSet.P1FileNo = p1FileNoEdit.Text;
            MainSet.P1SampleDescription = p1SampleDescriptionEdit.Text;
            //MainSet.P1DetailOfSample = p1DetailOfSampleEdit.Text;
            MainSet.P1DetailOfSample = p1DetailOfSamplecomboBoxEdit.Text;
            MainSet.P1ItemNo = p1ItemNoEdit.Text;
            MainSet.P1OrderNo = p1OrderNoEdit.Text;
            MainSet.P1Packaging = p1PackagingEdit.Text;
            MainSet.P1Instruction = p1InstructionEdit.Text;
            MainSet.P1Buyer = p1BuyerEdit.Text;
            //MainSet.P1Manufacturer = p1ManufacturerEdit.Text;
            MainSet.P1Manufacturer = p1ManufacturerComboBox.Text;
            MainSet.P1CountryOfOrigin = p1CountryOfOriginEdit.Text;
            MainSet.P1CountryOfDestination = p1CountryOfDestinationEdit.Text;
            MainSet.P1LabeledAge = p1LabeledAgeEdit.Text;
            MainSet.P1TestAge = p1TestAgeEdit.Text;
            //MainSet.P1AssessedAge = p1AssessedAgeEdit.Text;
            MainSet.P1AssessedAge = p1AssessedAgecomboBoxEdit.Text;
            MainSet.P1ReceivedDate = p1ReceivedDateEdit.Text;
            MainSet.P1TestPeriod = p1TestPeriodEdit.Text;
            MainSet.P1TestMethod = p1TestMethodEdit.Text;
            MainSet.P1TestResults = p1TestResultEdit.Text;
            MainSet.P1Comments = p1ReportCommentEdit.Text;
            MainSet.P2Name = p2NameEdit.Text;
            MainSet.P3Description1 = p3Desc1Edit.Text;
            MainSet.P3Description2 = "";
            MainSet.P4Description1 = p4Desc1Edit.Text;
            MainSet.P4Description2 = p4Desc2Edit.Text;
            MainSet.P4Description3 = p4_4Desc2Edit.Text;
            MainSet.P4Description4 = p4Desc4Edit.Text;
            MainSet.P5Description1 = p5Desc1Edit.Text;
            MainSet.P5Description2 = p5Desc2Edit.Text;
        }

        public void SetDataSetToControl()
        {
            //Initialize();

            //findButton.PerformClick();

            //MainSet.RecNo = MainSet.RecNo;
            //MainSet.Select();
            ctrlPhysicalMainTap.RecNo = MainSet.RecNo;
            ctrlPhysicalMainTap.Select_Left();
            ctrlPhysicalMainTap.Fetch();
            SetDataSetToPage1("SelectTap");

            P2Set.MainNo = MainSet.RecNo;
            P2Set.Select();
            SetDataSetToPage2();

            P3Set.MainNo = MainSet.RecNo;
            P3Set.Select();
            SetDataSetToPage3();

            P40Set.MainNo = MainSet.RecNo;
            P40Set.Select();

            P41Set.MainNo = MainSet.RecNo;
            P41Set.Select();
            P42Set.MainNo = MainSet.RecNo;
            P42Set.Select();
            SetDataSetToPage4();
            
            P44Set.MainNo = MainSet.RecNo;
            P44Set.Select();
            SetDataSetToPage44();

            P45Set.MainNo = MainSet.RecNo;
            P45Set.Select();
            SetDataSetToPage45();

            P5Set.MainNo = MainSet.RecNo;
            P5Set.Select();
            SetDataSetToPage5();

            P6Set.MainNo = MainSet.RecNo;
            P6Set.Select();

            if (P6Set.Empty == false)
            {
                SetDataSetToPage6_Report();
            }
            else
            {

            }

            ImageSet.RecNo = MainSet.RecNo;
            ImageSet.Select();
            SetDataSetToPage6();

            P5Set.MainNo = MainSet.RecNo;
            P5Set.Select_PhyComplete();
            SetDataSetToPageComplete();

            EnterReportDescription();

            RefreshGrid();

            progressBar_PhyEU_ImportWORD.Value = 0;
        }

        private void SetDataSetToPage1(string sCase = "")
        {
            if (sCase.Equals("SelectTap"))
            {
                p1ClientNameEdit.Text = ctrlPhysicalMainTap.P1ClientName;
                p1ClientAddressEdit.Text = ctrlPhysicalMainTap.P1ClientAddress;
                p1FileNoEdit.Text = ctrlPhysicalMainTap.P1FileNo;
                p1SampleDescriptionEdit.Text = ctrlPhysicalMainTap.P1SampleDescription;
                //p1DetailOfSampleEdit.Text = ctrlPhysicalMainTap.P1DetailOfSample;
                p1DetailOfSamplecomboBoxEdit.Text = ctrlPhysicalMainTap.P1DetailOfSample;
                p1ItemNoEdit.Text = ctrlPhysicalMainTap.P1ItemNo;
                p1OrderNoEdit.Text = ctrlPhysicalMainTap.P1OrderNo;
                p1PackagingEdit.Text = ctrlPhysicalMainTap.P1Packaging;
                p1InstructionEdit.Text = ctrlPhysicalMainTap.P1Instruction;
                p1BuyerEdit.Text = ctrlPhysicalMainTap.P1Buyer;
                //p1ManufacturerEdit.Text = ctrlPhysicalMainTap.P1Manufacturer;
                p1ManufacturerComboBox.Text = ctrlPhysicalMainTap.P1Manufacturer;
                p1CountryOfOriginEdit.Text = ctrlPhysicalMainTap.P1CountryOfOrigin;
                p1CountryOfDestinationEdit.Text = ctrlPhysicalMainTap.P1CountryOfDestination;
                p1LabeledAgeEdit.Text = ctrlPhysicalMainTap.P1LabeledAge;
                p1TestAgeEdit.Text = ctrlPhysicalMainTap.P1TestAge;
                //p1AssessedAgeEdit.Text = ctrlPhysicalMainTap.P1AssessedAge;
                p1AssessedAgecomboBoxEdit.Text = ctrlPhysicalMainTap.P1AssessedAge;
                p1ReceivedDateEdit.Text = ctrlPhysicalMainTap.P1ReceivedDate;
                p1TestPeriodEdit.Text = ctrlPhysicalMainTap.P1TestPeriod;
                p1TestMethodEdit.Text = ctrlPhysicalMainTap.P1TestMethod;
                p1TestResultEdit.Text = ctrlPhysicalMainTap.P1TestResults;
                p1ReportCommentEdit.Text = ctrlPhysicalMainTap.P1Comments;
            }
            else 
            {
                p1ClientNameEdit.Text = MainSet.P1ClientName;
                p1ClientAddressEdit.Text = MainSet.P1ClientAddress;
                p1FileNoEdit.Text = MainSet.P1FileNo;
                p1SampleDescriptionEdit.Text = MainSet.P1SampleDescription;
                //p1DetailOfSampleEdit.Text = MainSet.P1DetailOfSample;
                p1DetailOfSamplecomboBoxEdit.Text = MainSet.P1DetailOfSample;
                p1ItemNoEdit.Text = MainSet.P1ItemNo;
                p1OrderNoEdit.Text = MainSet.P1OrderNo;
                p1PackagingEdit.Text = MainSet.P1Packaging;
                p1InstructionEdit.Text = MainSet.P1Instruction;
                p1BuyerEdit.Text = MainSet.P1Buyer;
                //p1ManufacturerEdit.Text = MainSet.P1Manufacturer;
                p1ManufacturerComboBox.Text = MainSet.P1Manufacturer;
                p1CountryOfOriginEdit.Text = MainSet.P1CountryOfOrigin;
                p1CountryOfDestinationEdit.Text = MainSet.P1CountryOfDestination;
                p1LabeledAgeEdit.Text = MainSet.P1LabeledAge;
                p1TestAgeEdit.Text = MainSet.P1TestAge;
                //p1AssessedAgeEdit.Text = MainSet.P1AssessedAge;
                p1AssessedAgecomboBoxEdit.Text = MainSet.P1AssessedAge;
                p1ReceivedDateEdit.Text = MainSet.P1ReceivedDate;
                p1TestPeriodEdit.Text = MainSet.P1TestPeriod;
                p1TestMethodEdit.Text = MainSet.P1TestMethod;
                p1TestResultEdit.Text = MainSet.P1TestResults;
                p1ReportCommentEdit.Text = MainSet.P1Comments;
            }
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
                P3Rows.Add(p3Row);
            }

            p3ClauseNoColumn.SortOrder = ColumnSortOrder.Ascending;
        }

        private void SetDataSetToPage4()
        {
            p4Desc1Edit.Text = MainSet.P4Description1;
            p4Desc2Edit.Text = MainSet.P4Description2;
            //p4_4Desc2Edit.Text = MainSet.P4Description3;
            p4Desc4Edit.Text = MainSet.P4Description4;            

            P40Rows.Clear();
            for (int i = 0; i < P40Set.RowCount; i++)
            {
                P40Set.Fetch(i);

                PhysicalPage3Row p3Row = new PhysicalPage3Row();
                p3Row.No = P40Set.No;
                p3Row.Line = P40Set.Line;
                p3Row.Clause = P40Set.Clause;
                p3Row.Description = P40Set.Description;
                p3Row.Result = P40Set.Result;
                P40Rows.Add(p3Row);
            }

            p40NoColumn.SortOrder = ColumnSortOrder.Ascending;

            P41Rows.Clear();
            for (int i = 0; i < P41Set.RowCount; i++)
            {
                P41Set.Fetch(i);

                PhysicalPage4Row p4Row = new PhysicalPage4Row();
                p4Row.No = P41Set.No;
                p4Row.Line = P41Set.Line;
                p4Row.Sample = P41Set.Sample;
                p4Row.Result = P41Set.Result;
                P41Rows.Add(p4Row);
            }

            p41NoColumn.SortOrder = ColumnSortOrder.Ascending;

            /*
            P42Rows.Clear();
            for (int i = 0; i < P42Set.RowCount; i++)
            {
                P42Set.Fetch(i);

                PhysicalPage4Row p4Row = new PhysicalPage4Row();
                p4Row.No = P42Set.No;
                p4Row.Line = P42Set.Line;
                p4Row.Sample = P42Set.Sample;
                p4Row.BurningRate = P42Set.BurningRate;
                P42Rows.Add(p4Row);
            }

            p42NoColumn.SortOrder = ColumnSortOrder.Ascending;
            */
        }

        private void SetDataSetToPage44()
        {
            p4_4Desc1Edit.Text = MainSet.P4Description1;

            P422Rows.Clear();
            P423Rows.Clear();
            P424Rows.Clear();

            p4Desc4_2_2Edit.Text = "";
            p4_4Desc2Edit.Text = "";

            p4Desc4_2_3Edit.Text = "";
            p4_4Desc3Edit.Text = "";

            p4Desc4_2_4Edit.Text = "";
            p4_4Desc4Edit.Text = "";

            for (int i = 0; i < P44Set.RowCount; i++)
            {
                P44Set.Fetch(i);

                if (P44Set.Clause.Trim().Equals("4.2.2"))
                {
                    PhysicalPage4_4_4_2_2_4Row p4_4_4222Row = new PhysicalPage4_4_4_2_2_4Row();
                    p4_4_4222Row.No = P44Set.No;
                    p4_4_4222Row.Line = P44Set.Line;
                    p4_4_4222Row.Sample = P44Set.Sample;
                    p4_4_4222Row.BurningRate = P44Set.BurningRate;
                    p4_4_4222Row.Clause = P44Set.Clause;
                    p4_4_4222Row.Desc = P44Set.Desc;
                    p4_4_4222Row.Note = P44Set.Note;

                    if (string.IsNullOrWhiteSpace(p4_4_4222Row.Note) == false)
                    {
                        p4Desc4_2_2Edit.Text = p4_4_4222Row.Note;
                        p4_4Desc2Edit.Text = p4_4_4222Row.Clause + " " + p4_4_4222Row.Desc;
                    }
                    P422Rows.Add(p4_4_4222Row);

                    //p40NoColumn.SortOrder = ColumnSortOrder.Ascending;                
                }

                if (P44Set.Clause.Trim().Equals("4.2.3"))
                {
                    PhysicalPage4_4_4_2_2_4Row p4_4_423Row = new PhysicalPage4_4_4_2_2_4Row();
                    p4_4_423Row.No = P44Set.No;
                    p4_4_423Row.Line = P44Set.Line;
                    p4_4_423Row.Sample = P44Set.Sample;
                    p4_4_423Row.Clause = P44Set.Clause;
                    p4_4_423Row.Desc = P44Set.Desc;
                    p4_4_423Row.BurningRate = P44Set.BurningRate;
                    p4_4_423Row.Note = P44Set.Note;

                    if (string.IsNullOrWhiteSpace(p4_4_423Row.Note) == false)
                    {
                        p4Desc4_2_3Edit.Text = p4_4_423Row.Note;
                        p4_4Desc3Edit.Text = p4_4_423Row.Clause + " " + p4_4_423Row.Desc;
                    }

                    P423Rows.Add(p4_4_423Row);
                }

                //p41NoColumn.SortOrder = ColumnSortOrder.Ascending;
                if (P44Set.Clause.Trim().Equals("4.2.4"))
                {
                    PhysicalPage4_4_4_2_2_4Row p4_4_424Row = new PhysicalPage4_4_4_2_2_4Row();
                    p4_4_424Row.No = P44Set.No;
                    p4_4_424Row.Line = P44Set.Line;
                    p4_4_424Row.Sample = P44Set.Sample;
                    p4_4_424Row.Clause = P44Set.Clause;
                    p4_4_424Row.Desc = P44Set.Desc;
                    p4_4_424Row.BurningRate = P44Set.BurningRate;
                    p4_4_424Row.Note = P44Set.Note;

                    if (string.IsNullOrWhiteSpace(p4_4_424Row.Note) == false)
                    {
                        p4Desc4_2_4Edit.Text = p4_4_424Row.Note;
                        p4_4Desc4Edit.Text = p4_4_424Row.Clause + " " + p4_4_424Row.Desc;
                    }

                    P424Rows.Add(p4_4_424Row);
                    //p42NoColumn.SortOrder = ColumnSortOrder.Ascending;
                }
            }
        }

        private void SetDataSetToPage45()
        {
            p4_5Desc1Edit.Text = MainSet.P4Description1;
            P45_425Rows.Clear();
            P45_43Rows.Clear();
            P45_44Rows.Clear();
            P45_45Rows.Clear();

            p4_5Desc425MemoEdit.Text = "";
            p4_5Desc425Edit.Text = "";

            p4_5Desc43MemoEdit.Text = "";
            p4_5Desc43Edit.Text = "";

            p4_5Desc44MemoEdit.Text = "";
            p4_5Desc44Edit.Text = "";

            p4_5Desc45MemoEdit.Text = "";
            p4_5Desc45Edit.Text = "";

            for (int i = 0; i < P45Set.RowCount; i++)
            {
                P45Set.Fetch(i);

                //if (P45Set.Clause.Trim().Split(' ').First().Equals("4.2.5"))
                if (P45Set.Clause.Trim().Equals("4.2.5"))
                {
                    PhysicalPage4_5_4Row p4_5_425Row = new PhysicalPage4_5_4Row();
                    p4_5_425Row.No = P45Set.No;
                    p4_5_425Row.Line = P45Set.Line;
                    p4_5_425Row.Sample = P45Set.Sample;
                    p4_5_425Row.BurningRate = P45Set.BurningRate;
                    p4_5_425Row.Result = P45Set.Result;
                    p4_5_425Row.Clause = P45Set.Clause;
                    p4_5_425Row.Desc = P45Set.Desc;
                    p4_5_425Row.Note = P45Set.Note;

                    if (string.IsNullOrWhiteSpace(p4_5_425Row.Note) == false)
                    {
                        p4_5Desc425MemoEdit.Text = p4_5_425Row.Note;
                        p4_5Desc425Edit.Text = p4_5_425Row.Clause + " " + p4_5_425Row.Desc;
                    }
                    P45_425Rows.Add(p4_5_425Row);
                }
                //else if (P45Set.Clause.Trim().Split(' ').First().Equals("4.3"))
                else if (P45Set.Clause.Trim().Equals("4.3"))
                {
                    PhysicalPage4_5_4Row p4_5_43Row = new PhysicalPage4_5_4Row();
                    p4_5_43Row.No = P45Set.No;
                    p4_5_43Row.Line = P45Set.Line;
                    p4_5_43Row.Sample = P45Set.Sample;
                    p4_5_43Row.BurningRate = P45Set.BurningRate;
                    p4_5_43Row.Result = P45Set.Result;
                    p4_5_43Row.Clause = P45Set.Clause;
                    p4_5_43Row.Desc = P45Set.Desc;
                    p4_5_43Row.Note = P45Set.Note;

                    if (string.IsNullOrWhiteSpace(p4_5_43Row.Note) == false)
                    {
                        p4_5Desc43MemoEdit.Text = p4_5_43Row.Note;
                        p4_5Desc43Edit.Text = p4_5_43Row.Clause + " " + p4_5_43Row.Desc;
                    }
                    P45_43Rows.Add(p4_5_43Row);
                }
                //else if (P45Set.Clause.Trim().Split(' ').First().Equals("4.4"))
                else if (P45Set.Clause.Trim().Equals("4.4"))
                {
                    PhysicalPage4_5_4Row p4_5_44Row = new PhysicalPage4_5_4Row();
                    p4_5_44Row.No = P45Set.No;
                    p4_5_44Row.Line = P45Set.Line;
                    p4_5_44Row.Sample = P45Set.Sample;
                    p4_5_44Row.BurningRate = P45Set.BurningRate;
                    p4_5_44Row.Result = P45Set.Result;
                    p4_5_44Row.Clause = P45Set.Clause;
                    p4_5_44Row.Desc = P45Set.Desc;
                    p4_5_44Row.Note = P45Set.Note;

                    if (string.IsNullOrWhiteSpace(p4_5_44Row.Note) == false)
                    {
                        p4_5Desc44MemoEdit.Text = p4_5_44Row.Note;
                        p4_5Desc44Edit.Text = p4_5_44Row.Clause + " " + p4_5_44Row.Desc;
                    }
                    P45_44Rows.Add(p4_5_44Row);
                }
                else if (P45Set.Clause.Trim().Equals("4.5"))
                {
                    PhysicalPage4_5_4Row p4_5_45Row = new PhysicalPage4_5_4Row();
                    p4_5_45Row.No = P45Set.No;
                    p4_5_45Row.Line = P45Set.Line;
                    p4_5_45Row.Sample = P45Set.Sample;
                    p4_5_45Row.BurningRate = P45Set.BurningRate;
                    p4_5_45Row.Result = P45Set.Result;
                    p4_5_45Row.Clause = P45Set.Clause;
                    p4_5_45Row.Desc = P45Set.Desc;
                    p4_5_45Row.Note = P45Set.Note;

                    if (string.IsNullOrWhiteSpace(p4_5_45Row.Note) == false)
                    {
                        p4_5Desc45MemoEdit.Text = p4_5_45Row.Note;
                        p4_5Desc45Edit.Text = p4_5_45Row.Clause + " " + p4_5_45Row.Desc;
                    }
                    P45_45Rows.Add(p4_5_45Row);
                }
                else 
                {

                }
            }
        }

        private void SetDataSetToPage5()
        {
            p5Desc1Edit.Text = MainSet.P5Description1;
            p5Desc2Edit.Text = MainSet.P5Description2;

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

            p5StuffNoColumn.SortOrder = ColumnSortOrder.Ascending;
        }

        private void SetDataSetToPage6_Report()
        {
            p6Desc1Edit.Text = "Labeling requirement (UKCA marking, name and address, product identification) for the market of Great Britain (England, Wales and\r\n" +
                               "Scotland) with reference to guidance published by department for business, energy & industrial strategy";
            p6Desc2Edit.Text = "1. The UKCA marking should be at least 5 mm in height – unless a different minimum dimension is specified in the relevant legislation.\r\n" +
                               "The UKCA marking should be easily visible, legible(from 1 January 2023 it must be permanently attached).To reduce or enlarge the size of\r\n" +
                               "your marking, the letters forming the UKCA marking must be in proportion to the version.\r\n\r\n" +
                               "2.UK importer shall label company’s details, including company’s name and a contact address after 1 January 2021.Until 31 December\r\n" +
                               "2022, UK importer can provide these details on the accompanying documentation rather on the good itself.\r\n\r\n" +
                               "3.Manufacturers must ensure that their toys bear a type, batch, serial or model number or other element allowing their identification, or\r\n" +
                               "where the size or nature of the toy does not allow it, that the required information is provided on the packaging or in a document\r\n" +
                               "accompanying the toy.";

            P6Rows.Clear();
            for (int i = 0; i < P6Set.RowCount; i++)
            {
                P6Set.Fetch(i);

                PhysicalPage6Row p6Row = new PhysicalPage6Row();
                p6Row.No = P6Set.No;
                p6Row.Line = P6Set.Line;
                p6Row.TestItem = P6Set.TestItem;
                p6Row.Result = P6Set.Result;
                p6Row.Requirement = P6Set.Requirement;
                P6Rows.Add(p6Row);
            }

            //p5StuffNoColumn.SortOrder = ColumnSortOrder.Ascending;
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

        public void SetPhyComplete()
        {
            P5Set.sPhyComplete = Convert.ToString(Convert.ToInt32(chkPhyComplete.Checked));
            P5Set.sPhyComplete_GiSool = Convert.ToString(Convert.ToInt32(chkPhyComplete_Gisool.Checked));
        }

        private void SetDataSetToPageComplete()
        {
            P5Set.Fetch(0, 0, "PhyComplete");

            chkPhyComplete.Checked = Convert.ToBoolean(Convert.ToInt32(P5Set.sPhyComplete));
            chkPhyComplete_Gisool.Checked = Convert.ToBoolean(Convert.ToInt32(P5Set.sPhyComplete_GiSool));
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

        private void CtrlEditPhysicalEu_Load(object sender, EventArgs e)
        {
            LoadReportDescription();
        }

        public void LoadReportDescription()
        {
            // page1 - Detail of Sample
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 1;
            P2Set.sReportCategory = "eu";
            P2Set.sColumnName = "detailofsample";
            P2Set.Select_ReportDesc();

            // Initialize
            p1DetailOfSamplecomboBoxEdit.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p1DetailOfSamplecomboBoxEdit.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page1 - manuFacturer
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 1;
            P2Set.sReportCategory = "eu";
            P2Set.sColumnName = "manuFacturer";
            P2Set.Select_ReportDesc();

            // Initialize
            p1AssessedAgecomboBoxEdit.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p1ManufacturerComboBox.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page1 - Assessed Age
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 1;
            P2Set.sReportCategory = "eu";
            P2Set.sColumnName = "assessedAge";
            P2Set.Select_ReportDesc();

            // Initialize
            p1AssessedAgecomboBoxEdit.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p1AssessedAgecomboBoxEdit.Items.Add(P2Set.sColumnDesc.ToString());
            }

            /*
            // page2 - Assessed Age
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 1;
            P2Set.sReportCategory = "eu";
            P2Set.sColumnName = "assessedAge";
            P2Set.Select_ReportDesc();

            // Initialize
            p2ComboEdit2.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p2ComboEdit2.Items.Add(P2Set.sColumnDesc.ToString());
            }
            */

            // page2 - Test Requested
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 2;
            P2Set.sReportCategory = "eu";
            P2Set.sColumnName = "testRequested";
            P2Set.Select_ReportDesc();

            // Initialize
            p2ComboEdit1.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p2ComboEdit1.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page2 - Conclusion
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 2;
            P2Set.sReportCategory = "eu";
            P2Set.sColumnName = "conclusion";
            P2Set.Select_ReportDesc();

            // Initialize
            p2ComboEdit2.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p2ComboEdit2.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page3 - Clause
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 3;
            P2Set.sReportCategory = "eu";
            P2Set.sColumnName = "clauseDesc";
            P2Set.Select_ReportNonEmptyClause();

            // Initialize
            p3ClauseComboBox.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p3ClauseComboBox.Items.Add(P2Set.sColumnClause.ToString());
            }

            // page3 - Description
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 3;
            P2Set.sReportCategory = "eu";
            P2Set.sColumnName = "clauseDesc";
            P2Set.Select_ReportEmptyClause();

            // Initialize
            p3DescriptionComboBox.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p3DescriptionComboBox.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page3 - Result
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 3;
            P2Set.sReportCategory = "eu";
            P2Set.sColumnName = "result";
            P2Set.Select_ReportDesc();

            // Initialize
            p3ResultComboBox.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p3ResultComboBox.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page4 - Clause
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 4;
            P2Set.sReportCategory = "eu";
            P2Set.sColumnName = "clauseDesc";
            P2Set.Select_ReportNonEmptyClause();

            // Initialize
            p40ClauseComboBox.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p40ClauseComboBox.Items.Add(P2Set.sColumnClause.ToString());
            }

            // page4 - Result
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 4;
            P2Set.sReportCategory = "eu";
            P2Set.sColumnName = "result";
            P2Set.Select_ReportDesc();

            // Initialize
            p40ResultComboBox.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p40ResultComboBox.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page4 - Burning Rate(mm/s)
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 4;
            P2Set.sReportCategory = "eu";
            P2Set.sColumnName = "burningRateMs";
            P2Set.Select_ReportDesc();

            // Initialize
            p42BurningRateComboBox.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p42BurningRateComboBox.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page5 - Observation Result
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 5;
            P2Set.sReportCategory = "eu";
            P2Set.sColumnName = "observationResult";
            P2Set.Select_ReportDesc();

            // Initialize
            p5observationResultComboBox.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p5observationResultComboBox.Items.Add(P2Set.sColumnDesc.ToString());
            }

            // page5 - Observation Result
            P2Set.sDepartment = "physical";
            P2Set.iReportPage = 5;
            P2Set.sReportCategory = "eu";
            P2Set.sColumnName = "location";
            P2Set.Select_ReportDesc();

            // Initialize
            p5LocationComboBox.Items.Clear();
            for (int i = 0; i < P2Set.RowCount; i++)
            {
                P2Set.Fetch(i, 0, "1");
                p5LocationComboBox.Items.Add(P2Set.sColumnDesc.ToString());
            }
        }

        private void p40ClauseComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxEdit editor = sender as ComboBoxEdit;
            if (editor != null)
            {
                object value = editor.SelectedItem;

                // page4 - Description
                P2Set.sDepartment = "physical";
                P2Set.iReportPage = 4;
                P2Set.sReportCategory = "eu";
                P2Set.sColumnName = "clauseDesc";
                P2Set.sColumnClause = value.ToString();
                P2Set.Select_ReportClause();

                P2Set.Fetch(0, 0, "1");
                p4ClauseGridView.SetFocusedRowCellValue("Clause", P2Set.sColumnClause.ToString());
                p4ClauseGridView.SetFocusedRowCellValue("Description", P2Set.sColumnDesc.ToString());
            }
        }

        private void p3ClauseComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxEdit editor = sender as ComboBoxEdit;
            if (editor != null)
            {
                object value = editor.SelectedItem;

                // page3 - Description
                P2Set.sDepartment = "physical";
                P2Set.iReportPage = 3;
                P2Set.sReportCategory = "eu";
                P2Set.sColumnName = "clauseDesc";
                P2Set.sColumnClause = value.ToString();
                P2Set.Select_ReportClause();

                P2Set.Fetch(0, 0, "1");
                p3ClauseGridView.SetFocusedRowCellValue("Clause", P2Set.sColumnClause.ToString());
                p3ClauseGridView.SetFocusedRowCellValue("Description", P2Set.sColumnDesc.ToString());
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

        private void btnImportWord_Click(object sender, EventArgs e)
        {
            // Initialize - Progress bar 
            progressBar_PhyEU_ImportWORD.Value = 0;

            DialogImportWordToDT_EU fDialogImportWordToDT_EU = new DialogImportWordToDT_EU();
            fDialogImportWordToDT_EU.ShowDialog();

            if (fDialogImportWordToDT_EU.bChkToDT == false)
            {
                SetDataSetToControl();
                return;
            }

            //page 1
            dtGet1 = fDialogImportWordToDT_EU.GetVariable1();

            //page 2
            dtGet2 = fDialogImportWordToDT_EU.GetVariable2();

            //page 3
            dtGet3 = fDialogImportWordToDT_EU.GetVariable3();

            //page 4-2
            dtGet4 = fDialogImportWordToDT_EU.GetVariable4();
            
            //page 4-3
            dtGet5_1 = fDialogImportWordToDT_EU.GetVariable5_1();

            //page.4-4
            dtGet5_2 = fDialogImportWordToDT_EU.GetVariable5_2();

            //page 4-5
            dtGet6 = fDialogImportWordToDT_EU.GetVariable6();

            //page5
            dtGet7 = fDialogImportWordToDT_EU.GetVariable7();

            //page 6
            dtGet8 = fDialogImportWordToDT_EU.GetVariable8();

            //page complete
            dtGet9 = fDialogImportWordToDT_EU.GetVariable9();

            //result2
            dtGet10 = fDialogImportWordToDT_EU.GetVariable10();

            P2Set.MainNo = MainSet.RecNo;
            //P2Set.sColumnDesc_4_3_7_Result = "";

            // Step1
            progressBar_PhyEU_ImportWORD.PerformStep();

            if (dtGet1 != null)
            {
                foreach (DataRow row in dtGet1.Rows)
                {
                    P2Set.sColumnClause = row["Report Job No."].ToString();
                    P2Set.sLabeledAge = row["Labeled Age Grading"].ToString();
                    P2Set.sRequestAge = row["Requested Age Grading"].ToString();
                    P2Set.sTestAge = row["Age Group Applied in Testing"].ToString();
                    P2Set.sSampleDescription = row["Sample description"].ToString();
                    P2Set.sDetailOfSample = row["Detail of sample"].ToString();
                    P2Set.sReportComments = row["Report comments"].ToString();
                    P2Set.Update_Main();
                }
            }

            // Step2
            progressBar_PhyEU_ImportWORD.PerformStep();
            if (dtGet2 != null)
            {
                P2Set.Delete_PhyPage2();

                int i = 0;
                foreach (DataRow row in dtGet2.Rows)
                {
                    P2Set.iColumnNo = i;
                    P2Set.bColumnLine = false;
                    P2Set.sTestRequested = row["Test Requested"].ToString();
                    P2Set.sConclusion = row["Conclusion"].ToString();
                    P2Set.Insert_PhyPage2();
                    i++;
                }
            }

            // Step3
            progressBar_PhyEU_ImportWORD.PerformStep();
            if (dtGet3 != null)
            {                
                P2Set.Delete_PhyPage3();

                int i = 0;
                foreach (DataRow row in dtGet3.Rows)
                {
                    P2Set.sColumnRemark = "";

                    P2Set.iColumnNo = i;
                    P2Set.bColumnLine = false;
                    P2Set.sColumnClause = row["Clause"].ToString();
                    P2Set.sColumnDesc = row["Description"].ToString();
                    P2Set.sColumnResult = row["Result"].ToString();
                    P2Set.sColumnRemark = row["Remark"].ToString();
                    P2Set.sColumnComment = "";
                    
                    P2Set.Insert_PhyPage3();

                    if (string.IsNullOrWhiteSpace(P2Set.sColumnRemark) == false)
                    {
                        P2Set.sColumnClause = "";
                        P2Set.sColumnDesc = P2Set.sColumnRemark;
                        P2Set.sColumnResult = "";
                        P2Set.Insert_PhyPage3();
                    }

                    i++;
                }
            }

            // Step4
            progressBar_PhyEU_ImportWORD.PerformStep();
            if (dtGet4 != null)
            {
                P2Set.Delete_PhyPage40();

                int i = 0;
                foreach (DataRow row in dtGet4.Rows)
                {
                    P40Set.MainNo = MainSet.RecNo;
                    P40Set.No = i;
                    P40Set.Line = true;
                    P40Set.Clause = row["Clause"].ToString();
                    P40Set.Description = row["Desc"].ToString();
                    P40Set.Result = row["Result"].ToString();

                    P40Set.Insert();

                    i++;
                }
            }

            // Step5
            progressBar_PhyEU_ImportWORD.PerformStep();
            if (dtGet5_1 != null)
            {
                P2Set.Delete_PhyPage41();

                int i = 0;
                foreach (DataRow row in dtGet5_1.Rows)
                {
                    P41Set.MainNo = MainSet.RecNo;
                    P41Set.No = i;
                    P41Set.Line = true;
                    P41Set.Sample = row["sample"].ToString();
                    P41Set.BurningRate = "";// row["Result"].ToString();
                    P41Set.Result = row["result"].ToString();

                    P41Set.Insert();

                    i++;
                }
            }

            // Step6
            progressBar_PhyEU_ImportWORD.PerformStep();
            if (dtGet5_2 != null)
            {
                P44Set.MainNo = MainSet.RecNo;
                P44Set.Delete();

                int i = 0;
                int j = 0;
                int k = 0;

                foreach (DataRow row in dtGet5_2.Rows)
                {
                    if (row["Clause"].ToString().Trim().Equals("4.2.2"))
                    {
                        P44Set.No = i;
                        P44Set.Line = true;
                        P44Set.Sample = row["Sample"].ToString();
                        P44Set.Clause = row["Clause"].ToString().Trim();
                        P44Set.Desc = row["Desc"].ToString().Trim();
                        P44Set.BurningRate = row["Result"].ToString();
                        if (i == 0)
                        {
                            P44Set.Note = "DNI = Did not Ignite\r\n" +
                                           "FAIL: Exceed the limit\r\n\r\n" +
                                           "N.B.: Only applicable clauses were shown.";
                        }
                        else
                        {
                            P44Set.Note = "";
                        }
                        
                        P44Set.Insert();

                        i++;
                    }
                    else if (row["Clause"].ToString().Trim().Equals("4.2.3"))
                    {
                        P44Set.No = j;
                        P44Set.Line = true;
                        P44Set.Sample = row["Sample"].ToString();
                        P44Set.Clause = row["Clause"].ToString().Trim();
                        P44Set.Desc = row["Desc"].ToString().Trim();
                        P44Set.BurningRate = row["Result"].ToString();
                        if (j == 0)
                        {
                            P44Set.Note = "DNI = Did not Ignite\r\n" +
                                           "FAIL: Exceed the limit\r\n\r\n" +
                                           "N.B.: Only applicable clauses were shown.";
                        }
                        else
                        {
                            P44Set.Note = "";
                        }
                        P44Set.Insert();

                        j++;
                    }
                    else if (row["Clause"].ToString().Trim().Equals("4.2.4"))
                    {
                        P44Set.No = k;
                        P44Set.Line = true;
                        P44Set.Sample = row["Sample"].ToString();
                        P44Set.Clause = row["Clause"].ToString().Trim();
                        P44Set.Desc = row["Desc"].ToString().Trim();
                        P44Set.BurningRate = row["Result"].ToString();
                        if (k == 0)
                        {
                            P44Set.Note = "DNI = Did not Ignite\r\n" +
                                           "FAIL: Exceed the limit\r\n\r\n" +
                                           "N.B.: Only applicable clauses were shown.";
                        }
                        else
                        {
                            P44Set.Note = "";
                        }                        
                        P44Set.Insert();

                        k++;
                    }
                    else 
                    {

                    }
                }
            }

            // Step7
            progressBar_PhyEU_ImportWORD.PerformStep();
            if (dtGet6 != null)
            {
                P45Set.MainNo = MainSet.RecNo;
                P45Set.Delete();

                int i = 0;
                int j = 0;
                int k = 0;
                int l = 0;

                foreach (DataRow row in dtGet6.Rows)
                {
                    if (row["Clause"].ToString().Trim().Equals("4.2.5"))
                    //if (row["Clause"].ToString().Trim().Split(' ').First().Equals("4.2.5"))
                    {
                        P45Set.No = i;
                        P45Set.Line = true;
                        P45Set.Sample = row["Sample"].ToString();
                        P45Set.Clause = row["Clause"].ToString().Trim();
                        P45Set.Desc = row["Desc"].ToString().Trim();
                        P45Set.BurningRate = row["Burn rate"].ToString();
                        P45Set.Result = row["Result"].ToString();
                        if (i == 0)
                        {
                            P45Set.Note = "DNI = Did not Ignite\r\n" +
                                          "SE = Self-Extinguishing\r\n" +
                                          "FAIL: Exceed the limit\r\n" +
                                          "Remark: The sample(s) was (were) not tested since the sample as it appears in the toy, did not produce sufficient material for testing.";
                                          //"N.B.: Only applicable clauses were shown.";
                        }
                        else
                        {
                            P45Set.Note = "";
                        }

                        P45Set.Insert();

                        i++;
                    }
                    //else if (row["Clause"].ToString().Trim().Split(' ').First().Equals("4.3"))
                    else if (row["Clause"].ToString().Trim().Equals("4.3"))
                    {
                        P45Set.No = j;
                        P45Set.Line = true;
                        P45Set.Sample = row["Sample"].ToString();
                        P45Set.Clause = row["Clause"].ToString().Trim();
                        P45Set.Desc = row["Desc"].ToString().Trim();
                        P45Set.BurningRate = row["Burn rate"].ToString();
                        P45Set.Result = row["Result"].ToString();
                        if (j == 0)
                        {
                            P45Set.Note = "SE1) = Self-extinguished within 50 mm of flame application\r\n" +
                                            "SE2) = Self-Extinguishing\r\n" +
                                            "DNI = Did not Ignite\r\n" +
                                            "FAIL: Exceed the limit\r\n" +
                                            "Remark1: The sample(s) was (were) not tested since the sample as it appears in the toy, did not produce sufficient material for testing.\r\n" +
                                            "Remark2: Tested burning rate is between 10 mm/s and 30 mm/s. Therefore, both the toy and the packaging shall be permanently marked\r\n" +
                                            "with the following warning: 'Warning! Keep away from fire'";
                        }
                        else
                        {
                            P45Set.Note = "";
                        }

                        P45Set.Insert();

                        j++;
                    }
                    //else if (row["Clause"].ToString().Trim().Split(' ').First().Equals("4.4"))
                    else if (row["Clause"].ToString().Trim().Equals("4.4"))
                    {
                        P45Set.No = k;
                        P45Set.Line = true;
                        P45Set.Sample = row["Sample"].ToString();
                        P45Set.Clause = row["Clause"].ToString().Trim();
                        P45Set.Desc = row["Desc"].ToString().Trim();
                        P45Set.BurningRate = row["Burn rate"].ToString();
                        P45Set.Result = row["Result"].ToString();
                        if (k == 0)
                        {
                            P45Set.Note = "DNI = Did not Ignite\r\n" +
                                          "SE = Self-Extinguishing\r\n" +
                                          "FAIL: Exceed the limit\r\n" +
                                          "Remark1: The sample(s) was (were) not tested since the sample as it appears in the toy, did not produce sufficient material for testing.\r\n" +
                                          "Remark2: Tested burning rate is between 10 mm/s and 30 mm/s. Therefore, both the toy and the packaging shall be permanently marked\r\n" +
                                          "with the following warning: 'Warning! Keep away from fire'";
                        }
                        else
                        {
                            P45Set.Note = "";
                        }
                        P45Set.Insert();

                        k++;
                    }
                    else if (row["Clause"].ToString().Trim().Equals("4.5"))
                    //else if (row["Clause"].ToString().Trim().Split(' ').First().Equals("4.5"))
                    {
                        P45Set.No = l;
                        P45Set.Line = true;
                        P45Set.Sample = row["Sample"].ToString();
                        P45Set.Clause = row["Clause"].ToString().Trim();
                        P45Set.Desc = row["Desc"].ToString().Trim();
                        //P45Set.Clause = row["Clause"].ToString().Trim().Split(' ').First();
                        //P45Set.Desc = row["Clause"].ToString().Replace(P45Set.Clause, "").Trim();
                        P45Set.BurningRate = row["Burn rate"].ToString();
                        P45Set.Result = row["Result"].ToString();
                        if (l == 0)
                        {
                            //P45Set.Note = "SE = Self-Extinguishing\r\n" +
                            //              "DNI = Did not Ignite\r\n" +
                            //              "FAIL: Exceed the limit\r\n" +
                            //              "* The sample(s) was (were) not tested as its maximum dimension is 150 mm or less.\r\n" +
                            //              //"(The rate of spread of flame on the surface of toy shall not be greater than 30 mm/sec.)\r\n\r\n" +
                            //              "Requirement: The rate of spread of flame on the surface of toy shall not be greater than 30 mm/sec.\r\n\r\n" +
                            //              "N.B.: Only applicable clauses were shown.";

                            P45Set.Note = "SE = Self-Extinguishing\r\n" +
                                          "DNI = Did not Ignite\r\n" +
                                          "FAIL: Exceed the limit\r\n" +
                                          "* The sample(s) was (were) not tested as its maximum dimension is 150 mm or less.\r\n" +
                                          //"(The rate of spread of flame on the surface of toy shall not be greater than 30 mm/sec.)\r\n\r\n" +
                                          "Requirement: The rate of spread of flame on the surface of toy shall not be greater than 30 mm/sec.";

                        }
                        else
                        {
                            P45Set.Note = "";
                        }
                        P45Set.Insert();

                        l++;
                    }
                    else
                    {

                    }
                }
            }

            // Step8
            progressBar_PhyEU_ImportWORD.PerformStep();
            // Page5
            if (dtGet7 != null)
            {
                P5Set.MainNo = MainSet.RecNo;
                P5Set.Delete();

                int i = 0;
                foreach (DataRow row in dtGet7.Rows)
                {   
                    P5Set.No = i;
                    P5Set.Line = true;
                    P5Set.TestItem = row["Observation"].ToString();
                    P5Set.Result = row["Result"].ToString();
                    P5Set.Requirement = row["Location"].ToString();
                    P5Set.Insert();

                    i++;
                }
            }

            // Step9
            progressBar_PhyEU_ImportWORD.PerformStep();
            // Page6
            if (dtGet8 != null)
            {
                P6Set.MainNo = MainSet.RecNo;
                P6Set.Delete();

                int i = 0;
                foreach (DataRow row in dtGet8.Rows)
                {
                    P6Set.No = i;
                    P6Set.Line = true;
                    P6Set.TestItem = row["Observation"].ToString();
                    P6Set.Result = row["Result"].ToString();
                    P6Set.Requirement = row["Location"].ToString();
                    P6Set.Note = "";
                    P6Set.Description = "";

                    P6Set.Insert();

                    i++;
                }
            }

            // Step10
            progressBar_PhyEU_ImportWORD.PerformStep();
            if (dtGet9 != null)
            {
                foreach (DataRow row in dtGet9.Rows)
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
            progressBar_PhyEU_ImportWORD.PerformStep();
            if (dtGet10.Rows.Count > 0)
            {
                P2Set.Delete_PHYRPTVIEW();

                foreach (DataRow row in dtGet10.Rows)
                {
                    if (row["Result"].ToString().ToUpper().Equals("RESULT 1"))
                    {
                        if (row["V"].ToString().ToUpper().Equals("V"))
                        {
                            P2Set.sColumnDesc_4_3_7_Report_View = "0";
                            P2Set.sColumnDesc_4_3_7_Report_Page = "5";
                            P2Set.Insert_ReportView();
                            //P2Set.Update_ReportView();
                        }
                    }
                    else if (row["Result"].ToString().ToUpper().Equals("RESULT 2"))
                    {
                        if (row["V"].ToString().ToUpper().Equals("V"))
                        {
                            P2Set.sColumnDesc_4_3_7_Report_View = "0";
                            P2Set.sColumnDesc_4_3_7_Report_Page = "6";
                            P2Set.Insert_ReportView();
                            //P2Set.Update_ReportView();
                        }
                    }
                    else
                    {
                        //P2Set.sColumnDesc_4_3_7_Report_View = "1";
                        //P2Set.Update_ReportView();
                    }
                }
            }

            // Step12
            progressBar_PhyEU_ImportWORD.PerformStep();
            SetDataSetToControl();

            MessageBox.Show("Completed!");
        }

        private void physical6Page_Resize_1(object sender, EventArgs e)
        {
            int width = physical6Page.Width;
            int height = physical6Page.Height;

            p6StuffGrid.Width = width - 8;
            p6LocationColumn.Width = 183 + (width - 548) / 3;
            p6ObservationColumn.Width = 183 + (width - 548) / 3;
            p6ResultColumn.Width = 183 + (width - 548) / 3;

            p6RowUpButton.Left = width - 106;
            p6RowDownButton.Left = width - 80;
            p6RowPlusButton.Left = width - 54;
            p6RowMinusButton.Left = width - 28;

            p6Desc1Edit.Width = width - 8;
            p6Desc2Edit.Size = new Size(width - 8, height - 313);
        }

        private void CtrlEditPhysicalEu_Enter(object sender, EventArgs e)
        {
            SetDataSetToControl();
        }
    }
}