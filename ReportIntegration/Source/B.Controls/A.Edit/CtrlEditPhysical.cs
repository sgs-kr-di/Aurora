using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraReports.UI;
using DevExpress.XtraGrid.Views.Base;

using Ulee.Controls;
using Ulee.Utils;
using LoadingIndicator.WinForms;

namespace Sgs.ReportIntegration
{
    public partial class CtrlEditPhysical : UlUserControlEng
    {
        private CtrlEditRight parent;

        private GridBookmark bookmark;

        private PhysicalMainDataSet phyCheckSet;

        private PhysicalMainDataSet phyMainSet;

        private PhysicalImageDataSet phyImageSet;

        private PhysicalP2DataSet phyP2Set;

        private PhysicalP3DataSet phyP3Set;

        private PhysicalP40DataSet phyP40Set;

        private PhysicalP41DataSet phyP41Set;

        private PhysicalP42DataSet phyP42Set;

        private PhysicalP44DataSet phyP44Set;

        private PhysicalP422DataSet phyP422Set;

        private PhysicalP423DataSet phyP423Set;

        private PhysicalP45DataSet phyP45Set;

        private PhysicalP5DataSet phyP5Set;

        private PhysicalP6DataSet phyP6Set;

        private PhysicalP7DataSet phyP7Set;

        private PhysicalReportDataSet phyReportSet;

        private ProfJobDataSet profJobSet;

        private CtrlEditPhysicalUs ctrlUs;

        private CtrlEditPhysicalEu ctrlEu;

        private PhysicalQuery phyQuery;

        private ProductDataSet productSet;

        private LongOperation _longOperation;

        public CtrlEditPhysical(CtrlEditRight parent)
        {
            this.parent = parent;

            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            phyCheckSet = new PhysicalMainDataSet(AppRes.DB.Connect, null, null);
            phyMainSet = new PhysicalMainDataSet(AppRes.DB.Connect, null, null);
            phyImageSet = new PhysicalImageDataSet(AppRes.DB.Connect, null, null);
            phyP2Set = new PhysicalP2DataSet(AppRes.DB.Connect, null, null);
            phyP3Set = new PhysicalP3DataSet(AppRes.DB.Connect, null, null);
            phyP40Set = new PhysicalP40DataSet(AppRes.DB.Connect, null, null);
            phyP41Set = new PhysicalP41DataSet(AppRes.DB.Connect, null, null);
            phyP42Set = new PhysicalP42DataSet(AppRes.DB.Connect, null, null);
            phyP44Set = new PhysicalP44DataSet(AppRes.DB.Connect, null, null);
            phyP45Set = new PhysicalP45DataSet(AppRes.DB.Connect, null, null);
            phyP5Set = new PhysicalP5DataSet(AppRes.DB.Connect, null, null);
            phyP6Set = new PhysicalP6DataSet(AppRes.DB.Connect, null, null);
            phyP7Set = new PhysicalP7DataSet(AppRes.DB.Connect, null, null);
            phyReportSet = new PhysicalReportDataSet(AppRes.DB.Connect, null, null);
            profJobSet = new ProfJobDataSet(AppRes.DB.Connect, null, null);
            productSet = new ProductDataSet(AppRes.DB.Connect, null, null);

            ctrlUs = new CtrlEditPhysicalUs(findButton);
            ctrlUs.MainSet = phyMainSet;
            ctrlUs.ImageSet = phyImageSet;
            ctrlUs.P2Set = phyP2Set;
            ctrlUs.P3Set = phyP3Set;
            ctrlUs.P4Set = phyP41Set;
            ctrlUs.P5Set = phyP5Set;

            ctrlEu = new CtrlEditPhysicalEu(findButton);
            ctrlEu.MainSet = phyMainSet;
            ctrlEu.ImageSet = phyImageSet;
            ctrlEu.P2Set = phyP2Set;
            ctrlEu.P3Set = phyP3Set;
            ctrlEu.P40Set = phyP40Set;
            ctrlEu.P41Set = phyP41Set;
            ctrlEu.P42Set = phyP42Set;
            ctrlEu.P44Set = phyP44Set;
            ctrlEu.P45Set = phyP45Set;
            ctrlEu.P5Set = phyP5Set;
            ctrlEu.P6Set = phyP6Set;

            phyQuery = new PhysicalQuery();
            phyQuery.MainSet = phyMainSet;
            phyQuery.ImageSet = phyImageSet;
            phyQuery.P2Set = phyP2Set;
            phyQuery.P3Set = phyP3Set;
            phyQuery.P40Set = phyP40Set;
            phyQuery.P41Set = phyP41Set;
            phyQuery.P42Set = phyP42Set;
            phyQuery.P44Set = phyP44Set;
            phyQuery.P45Set = phyP45Set;
            phyQuery.P5Set = phyP5Set;
            phyQuery.P6Set = phyP6Set;
            phyQuery.P7Set = phyP7Set;
            phyQuery.ProfJobSet = profJobSet;
            phyQuery.CtrlUs = ctrlUs;
            phyQuery.CtrlEu = ctrlEu;

            bookmark = new GridBookmark(physicalGridView);
            AppHelper.SetGridEvenRow(physicalGridView);

            physicalAreaColumn.DisplayFormat.FormatType = FormatType.Custom;
            physicalAreaColumn.DisplayFormat.Format = new ReportAreaFormat();

            physicalRegTimeColumn.DisplayFormat.FormatType = FormatType.Custom;
            physicalRegTimeColumn.DisplayFormat.Format = new ReportDateTimeFormat();

            approvalCombo.DataSource = EnumHelper.GetNameValues<EReportApproval>();
            approvalCombo.DisplayMember = "Name";
            approvalCombo.ValueMember = "Value";

            areaCombo.DataSource = EnumHelper.GetNameValues<EReportArea>();
            areaCombo.DisplayMember = "Name";
            areaCombo.ValueMember = "Value";

            SetControl(null);

            // Initialize long operation with control which will
            // be overlayed during long operations
            _longOperation = new LongOperation(this);

            // You can pass settings to customize indicator view/behavior
            // _longOperation = new LongOperation(this, LongOperationSettings.Default);
        }

        private void CtrlEditPhysical_Enter(object sender, EventArgs e)
        {
            parent.SetMenu(1);
            //resetButton.PerformClick();

            PhysicalMainDataSet set = phyMainSet;
            bookmark.Get();

            AppHelper.ResetGridDataSource(physicalGrid);

            set.From = "";
            set.To = "";

            set.RecNo = Program.sGetPhysicalJobno();

            if (!string.IsNullOrEmpty(set.RecNo))
            {
                set.AreaNo = (EReportArea)Program.iGetPhysicalAreano();
                set.ReportApproval = (EReportApproval)approvalCombo.SelectedValue;
                set.ProductNo = Program.sGetPhysicalCode();
                
                set.P1FileNo = "";
                //set.RecNo = Program.sGetPhysicalJobno();

                //set.AreaNo = (EReportArea)areaCombo.SelectedValue;
                //set.ReportApproval = (EReportApproval)approvalCombo.SelectedValue;
                //set.ProductNo = itemNoEdit.Text.Trim();
                //set.RecNo = jobNoEdit.Text.Trim();

                set.Select();

                AppHelper.SetGridDataSource(physicalGrid, set);

                bookmark.Goto();
                physicalGrid.Focus();
            }

            DateTime MonthFirstDay = DateTime.Now.AddDays(1 - DateTime.Now.Day);

            //fromDateEdit.Text = MonthFirstDay.ToString("yyyy-MM-dd");
            fromDateEdit.Text = MonthFirstDay.ToString("yyyy-01-dd");   // 1월 1일로 변경 요청 - 조재식 과장
        }

        private void CtrlEditPhysical_Resize(object sender, EventArgs e)
        {
        }

        private void gridPanel_Resize(object sender, EventArgs e)
        {
            int width = gridPanel.Width;

            findButton.Left = width - 86;
            resetButton.Left = width - 86;

            itemNoEdit.Width = width - 174;
            jobNoEdit.Width = width - 174;
            OmNoEdit.Width = width - 174;

            physicalGrid.Size = new Size(width, gridPanel.Height - 113);
        }

        private void reportPanel_Resize(object sender, EventArgs e)
        {
            reportPagePanel.Size = new Size(reportPanel.Width, reportPanel.Height - 30);
        }

        private void findButton_Click(object sender, EventArgs e)
        {
            PhysicalMainDataSet set = phyMainSet;
            bookmark.Get();

            AppHelper.ResetGridDataSource(physicalGrid);

            if (dateCheck.Checked == true)
            {
                set.From = fromDateEdit.Value.ToString(AppRes.csDateFormat);
                set.To = toDateEdit.Value.ToString(AppRes.csDateFormat);
            }
            else
            {
                set.From = "";
                set.To = "";
            }

            set.AreaNo = (EReportArea)areaCombo.SelectedValue;
            set.ReportApproval = (EReportApproval)approvalCombo.SelectedValue;
            set.ProductNo = itemNoEdit.Text.Trim();
            set.RecNo = jobNoEdit.Text.Trim();
            set.P1FileNo = OmNoEdit.Text.Trim();
            set.Select();

            AppHelper.SetGridDataSource(physicalGrid, set);

            bookmark.Goto();
            physicalGrid.Focus();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            dateCheck.Checked = true;
            fromDateEdit.Value = DateTime.Now.AddMonths(-1);
            toDateEdit.Value = DateTime.Now;
            approvalCombo.SelectedIndex = 0;
            areaCombo.SelectedIndex = 0;
            itemNoEdit.Text = string.Empty;
            jobNoEdit.Text = string.Empty;
            OmNoEdit.Text = string.Empty;
            findButton.PerformClick();

            physicalRegTimeColumn.SortOrder = ColumnSortOrder.Descending;
        }

        private void physicalGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0)
            {
                SetReportView(EReportArea.None);
                return;
            }

            DataRow row = physicalGridView.GetDataRow(e.FocusedRowHandle);
            phyMainSet.Fetch(row);

            SetReportView(phyMainSet.AreaNo);
        }

        private void fromDateEdit_ValueChanged(object sender, EventArgs e)
        {
            if (fromDateEdit.Value > toDateEdit.Value)
            {
                toDateEdit.Value = fromDateEdit.Value;
            }
        }

        private void toDateEdit_ValueChanged(object sender, EventArgs e)
        {
            if (toDateEdit.Value < fromDateEdit.Value)
            {
                fromDateEdit.Value = toDateEdit.Value;
            }
        }

        private void SetReportView(EReportArea area)
        {
            switch (area)
            {
                case EReportArea.None:
                    ClearReport();
                    break;

                case EReportArea.US:
                    SetReportUs();
                    break;

                case EReportArea.EU:
                    SetReportEu();
                    break;
            }
        }

        private void ClearReport()
        {
            areaPanel.Text = EReportArea.None.ToDescription();
            reportNoEdit.Text = "";
            issuedDateEdit.Text = "";
            SetControl(null);
        }

        private void SetReportUs()
        {
            areaPanel.Text = EReportArea.US.ToDescription();
            reportNoEdit.Text = $"F690101/LF-CTS{phyMainSet.P1FileNo}";
            issuedDateEdit.Text = $"{phyMainSet.RequiredTime.ToString("yyyy. MM. dd")}";

            SetControl(ctrlUs);
            ctrlUs.SetDataSetToControl();
        }

        private void SetReportEu()
        {
            areaPanel.Text = EReportArea.EU.ToDescription();
            reportNoEdit.Text = $"F690101/LF-CTS{phyMainSet.P1FileNo}";
            //issuedDateEdit.Text = $"{phyMainSet.ReportedTime.ToString("yyyy. MM. dd")}";
            issuedDateEdit.Text = $"{phyMainSet.RequiredTime.ToString("yyyy. MM. dd")}";

            SetControl(ctrlEu);
            ctrlEu.SetDataSetToControl();
        }

        private void SetControl(UlUserControlEng ctrl)
        {
            reportPagePanel.Controls.Clear();

            if (ctrl == null)
            {
                reportPagePanel.BevelOuter = EUlBevelStyle.Single;
            }
            else
            {
                reportPagePanel.Controls.Add(ctrl);
                reportPagePanel.BevelInner = EUlBevelStyle.None;
                reportPagePanel.BevelOuter = EUlBevelStyle.None;
                ctrl.Dock = DockStyle.Fill;
            }
        }

        public void Import()
        {
            using (_longOperation.Start())
            {
                DialogProfJobListView dialog = new DialogProfJobListView();

                try
                {
                    dialog.Type = EReportType.Physical;
                    dialog.ShowDialog();
                }
                finally
                {
                    if (dialog.DialogResult == DialogResult.OK)
                    {
                        profJobSet.Type = EReportType.Physical;
                        profJobSet.JobNo = dialog.JobNo;
                        //profJobSet.FileNo = dialog.OmNo;
                        //profJobSet.OmNo = dialog.OmNo;
                        profJobSet.ExtendASTM = false;
                        profJobSet.Select_Physical_Import();
                        profJobSet.Fetch();
                        Insert();
                        MessageBox.Show("Physical Completed!");
                    }
                }
            }            
        }

        public void Delete()
        {
            if (physicalGridView.FocusedRowHandle < 0) return;
            if (MessageBox.Show($"Would you like to delete physical report of {phyMainSet.RecNo}?",
                "SGS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            phyQuery.Delete();
            findButton.PerformClick();
        }

        public void Print()
        {
            using (_longOperation.Start())
            {
                if (physicalGridView.FocusedRowHandle < 0) return;

                this.Cursor = Cursors.WaitCursor;

                phyReportSet.RecNo = phyMainSet.RecNo;
                phyReportSet.Select();

                phyReportSet.DataSet.Tables[0].TableName = "P1";
                phyReportSet.DataSet.Tables[1].TableName = "P2";
                phyReportSet.DataSet.Tables[2].TableName = "P3";
                phyReportSet.DataSet.Tables[3].TableName = "P40";
                phyReportSet.DataSet.Tables[4].TableName = "P41";
                phyReportSet.DataSet.Tables[5].TableName = "P42";
                phyReportSet.DataSet.Tables[6].TableName = "P5";
                phyReportSet.DataSet.Tables[7].TableName = "Image";
                phyReportSet.DataSet.Tables[8].TableName = "PReportView_Page5";
                phyReportSet.DataSet.Tables[9].TableName = "PReportView_Page6";
                phyReportSet.DataSet.Tables[10].TableName = "P6";
                phyReportSet.DataSet.Tables[11].TableName = "P45";
                phyReportSet.DataSet.Tables[12].TableName = "PReportView_Page5_1";
                phyReportSet.DataSet.Tables[13].TableName = "PReportView_Page7";
                phyReportSet.DataSet.Tables[14].TableName = "P7";
                phyReportSet.DataSet.Tables[15].TableName = "PReportView_Page4_1";
                phyReportSet.DataSet.Tables[16].TableName = "P3_Clause4";
                phyReportSet.DataSet.Tables[17].TableName = "P3_Clause5";

                BindingSource bind = new BindingSource();
                bind.DataSource = phyReportSet.DataSet;

                XtraReport report;

                if (phyMainSet.AreaNo == EReportArea.US)
                    report = new ReportUsPhysical();
                else
                    report = new ReportEuPhysical();

                report.DataSource = bind;
                report.CreateDocument();
                new ReportPrintTool(report);

                this.Cursor = Cursors.Default;

                DialogReportPreview dialog = new DialogReportPreview();
                dialog.Source = report;
                dialog.WindowState = FormWindowState.Maximized;
                dialog.ShowDialog();
            }
        }

        public void Save()
        {
            if (physicalGridView.FocusedRowHandle < 0) return;
            if (MessageBox.Show($"Would you like to save physical report of {phyMainSet.RecNo}?",
                "SGS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            phyQuery.Update();
            findButton.PerformClick();
            MessageBox.Show("Save Complete!");
        }

        public void Cancel()
        {
            if (physicalGridView.FocusedRowHandle < 0) return;
            if (MessageBox.Show($"Would you like to cancel physical report of {phyMainSet.RecNo}?",
                "SGS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            SetReportView(phyMainSet.AreaNo);
        }

        private void Insert()
        {
            EReportArea area = profJobSet.AreaNo;

            if (profJobSet.Empty == true) return;
            if (area == EReportArea.None) return;
            if (string.IsNullOrWhiteSpace(profJobSet.ItemNo) == true) return;

            //phyCheckSet.From = "";
            //phyCheckSet.To = "";
            //phyCheckSet.AreaNo = area;
            //phyCheckSet.ReportApproval = EReportApproval.None;
            //phyCheckSet.ProductNo = profJobSet.ItemNo;
            phyCheckSet.RecNo = $"*{profJobSet.JobNo}";
            phyCheckSet.AreaNo = profJobSet.AreaNo;
            phyCheckSet.Select_ImportPysical();

            if (phyCheckSet.Empty == false)
            {
                MessageBox.Show("Can't import physical report because this report already exist in DB!",
                    "SGS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            phyQuery.Insert();
            findButton.PerformClick();
        }
    }
}
