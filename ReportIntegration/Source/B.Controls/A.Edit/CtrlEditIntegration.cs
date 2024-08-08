using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using LoadingIndicator.WinForms;
using Ulee.Controls;
using Ulee.Utils;

namespace Sgs.ReportIntegration
{
    public partial class CtrlEditIntegration : UlUserControlEng
    {
        private CtrlEditRight parent;

        private GridBookmark bookmark;

        private IntegrationMainDataSet integCheckSet;

        private IntegrationMainDataSet integMainSet;

        private IntegrationImageDataSet integImageSet;

        private IntegrationT1DataSet integT1Set;

        private IntegrationT2DataSet integT2Set;

        private IntegrationT3DataSet integT3Set;

        private IntegrationT4DataSet integT4Set;

        private IntegrationT5DataSet integT5Set;

        private IntegrationT6DataSet integT6Set;

        private IntegrationT61DataSet integT61Set;

        private IntegrationT7DataSet integT7Set;

        private IntegrationLimitASTMDataSet LimitASTMSet;

        private IntegrationResultASTMDataSet ResultASTMSet;

        private IntegrationLimitEnDataSet integLimitEnSet;

        private IntegrationResultEnDataSet integResultEnSet;

        private IntegrationLeadLimitAstmDataSet integSurfaceLeadLimitAstmSet;

        private IntegrationLeadResultAstmDataSet integSurfaceLeadResultAstmSet;

        private IntegrationLimitAstmDataSet integSurfaceLimitAstmSet;

        private IntegrationResultAstmDataSet integSurfaceResultAstmSet;

        private IntegrationLeadLimitAstmDataSet integSubstrateLeadLimitAstmSet;

        private IntegrationLeadResultAstmDataSet integSubstrateLeadResultAstmSet;

        private IntegrationLimitAstmDataSet integSubstrateLimitAstmSet;

        private IntegrationResultAstmDataSet integSubstrateResultAstmSet;

        private IntegrationReportDataSet integReportSet;

        private ProfJobDataSet profJobSet;

        private ProductDataSet productSet;

        private CtrlEditIntegrationUs ctrlUs;

        private CtrlEditIntegrationEu ctrlEu;

        private IntegrationQuery integQuery;

        private LongOperation _longOperation;

        public CtrlEditIntegration(CtrlEditRight parent)
        {
            this.parent = parent;

            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            integCheckSet = new IntegrationMainDataSet(AppRes.DB.Connect, null, null);
            integMainSet = new IntegrationMainDataSet(AppRes.DB.Connect, null, null);
            integImageSet = new IntegrationImageDataSet(AppRes.DB.Connect, null, null);
            integT1Set = new IntegrationT1DataSet(AppRes.DB.Connect, null, null);
            integT2Set = new IntegrationT2DataSet(AppRes.DB.Connect, null, null);
            integT3Set = new IntegrationT3DataSet(AppRes.DB.Connect, null, null);
            integT4Set = new IntegrationT4DataSet(AppRes.DB.Connect, null, null);
            integT5Set = new IntegrationT5DataSet(AppRes.DB.Connect, null, null);
            integT6Set = new IntegrationT6DataSet(AppRes.DB.Connect, null, null);
            integT61Set = new IntegrationT61DataSet(AppRes.DB.Connect, null, null);
            integT7Set = new IntegrationT7DataSet(AppRes.DB.Connect, null, null);

            LimitASTMSet = new IntegrationLimitASTMDataSet(AppRes.DB.Connect, null, null);
            ResultASTMSet = new IntegrationResultASTMDataSet(AppRes.DB.Connect, null, null);

            integLimitEnSet = new IntegrationLimitEnDataSet(AppRes.DB.Connect, null, null);
            integResultEnSet = new IntegrationResultEnDataSet(AppRes.DB.Connect, null, null);
            integSurfaceLeadLimitAstmSet = new IntegrationLeadLimitAstmDataSet(AppRes.DB.Connect, null, null);
            integSurfaceLeadResultAstmSet = new IntegrationLeadResultAstmDataSet(AppRes.DB.Connect, null, null);
            integSurfaceLimitAstmSet = new IntegrationLimitAstmDataSet(AppRes.DB.Connect, null, null);
            integSurfaceResultAstmSet = new IntegrationResultAstmDataSet(AppRes.DB.Connect, null, null);
            integSubstrateLeadLimitAstmSet = new IntegrationLeadLimitAstmDataSet(AppRes.DB.Connect, null, null);
            integSubstrateLeadResultAstmSet = new IntegrationLeadResultAstmDataSet(AppRes.DB.Connect, null, null);
            integSubstrateLimitAstmSet = new IntegrationLimitAstmDataSet(AppRes.DB.Connect, null, null);
            integSubstrateResultAstmSet = new IntegrationResultAstmDataSet(AppRes.DB.Connect, null, null);
            integReportSet = new IntegrationReportDataSet(AppRes.DB.Connect, null, null);
            profJobSet = new ProfJobDataSet(AppRes.DB.Connect, null, null);
            productSet = new ProductDataSet(AppRes.DB.Connect, null, null);

            ctrlUs = new CtrlEditIntegrationUs(findButton);
            ctrlUs.MainSet = integMainSet;
            ctrlUs.ImageSet = integImageSet;
            ctrlUs.T1Set = integT1Set;
            ctrlUs.T2Set = integT2Set;
            ctrlUs.T5Set = integT5Set;
            ctrlUs.T6Set = integT6Set;
            ctrlUs.T7Set = integT7Set;

            ctrlUs.LimitASTMSet = LimitASTMSet;
            ctrlUs.ResultASTMSet = ResultASTMSet;

            ctrlUs.SurfaceLeadLimitSet = integSurfaceLeadLimitAstmSet;
            ctrlUs.SurfaceLeadResultSet = integSurfaceLeadResultAstmSet;
            ctrlUs.SurfaceLimitSet = integSurfaceLimitAstmSet;
            ctrlUs.SurfaceResultSet = integSurfaceResultAstmSet;
            ctrlUs.SubstrateLeadLimitSet = integSubstrateLeadLimitAstmSet;
            ctrlUs.SubstrateLeadResultSet = integSubstrateLeadResultAstmSet;
            ctrlUs.SubstrateLimitSet = integSubstrateLimitAstmSet;
            ctrlUs.SubstrateResultSet = integSubstrateResultAstmSet;

            ctrlEu = new CtrlEditIntegrationEu(findButton);
            ctrlEu.MainSet = integMainSet;
            ctrlEu.ImageSet = integImageSet;
            ctrlEu.T1Set = integT1Set;
            ctrlEu.T2Set = integT2Set;
            ctrlEu.T3Set = integT3Set;
            ctrlEu.T4Set = integT4Set;
            ctrlEu.T5Set = integT5Set;
            ctrlEu.T6Set = integT6Set;
            ctrlEu.T7Set = integT7Set;
            ctrlEu.LimitSet = integLimitEnSet;
            ctrlEu.ResultSet = integResultEnSet;

            integQuery = new IntegrationQuery();
            integQuery.MainSet = integMainSet;
            integQuery.ImageSet = integImageSet;
            integQuery.T1Set = integT1Set;
            integQuery.T2Set = integT2Set;
            integQuery.T3Set = integT3Set;
            integQuery.T4Set = integT4Set;
            integQuery.T5Set = integT5Set;
            integQuery.T6Set = integT6Set;
            integQuery.T61Set = integT61Set;
            integQuery.T7Set = integT7Set;

            integQuery.LimitASTMSet = LimitASTMSet;
            integQuery.ResultASTMSet = ResultASTMSet;

            integQuery.LimitEnSet = integLimitEnSet;
            integQuery.ResultEnSet = integResultEnSet;
            integQuery.SurfaceLeadLimitAstmSet = integSurfaceLeadLimitAstmSet;
            integQuery.SurfaceLeadResultAstmSet = integSurfaceLeadResultAstmSet;
            integQuery.SurfaceLimitAstmSet = integSurfaceLimitAstmSet;
            integQuery.SurfaceResultAstmSet = integSurfaceResultAstmSet;
            integQuery.SubstrateLeadLimitAstmSet = integSubstrateLeadLimitAstmSet;
            integQuery.SubstrateLeadResultAstmSet = integSubstrateLeadResultAstmSet;
            integQuery.SubstrateLimitAstmSet = integSubstrateLimitAstmSet;
            integQuery.SubstrateResultAstmSet = integSubstrateResultAstmSet;
            integQuery.ProductSet = productSet;
            integQuery.ProfJobSet = profJobSet;
            integQuery.CtrlUs = ctrlUs;
            integQuery.CtrlEu = ctrlEu;

            bookmark = new GridBookmark(integrationGridView);
            AppHelper.SetGridEvenRow(integrationGridView);

            integAreaColumn.DisplayFormat.FormatType = FormatType.Custom;
            integAreaColumn.DisplayFormat.Format = new ReportAreaFormat();

            integRegTimeColumn.DisplayFormat.FormatType = FormatType.Custom;
            integRegTimeColumn.DisplayFormat.Format = new ReportDateTimeFormat();

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

        private void CtrlEditIntegration_Enter(object sender, EventArgs e)
        {
            parent.SetMenu(3);

            IntegrationMainDataSet set = integMainSet;

            bookmark.Get();
            AppHelper.ResetGridDataSource(integrationGrid);
            
            set.From = "";
            set.To = "";
            set.RecNo = Program.sGetPhysicalIntegJobno();

            if (!string.IsNullOrEmpty(set.RecNo))
            {
                set.AreaNo = (EReportArea)Program.iGetPhysicalAreano();
                set.ReportApproval = (EReportApproval)approvalCombo.SelectedValue;
                set.ProductNo = Program.sGetPhysicalCode();

                set.Select();

                AppHelper.SetGridDataSource(integrationGrid, set);

                bookmark.Goto();
                integrationGrid.Focus();
            }

            DateTime MonthFirstDay = DateTime.Now.AddDays(1 - DateTime.Now.Day);
            fromDateEdit.Text = MonthFirstDay.ToString("yyyy-MM-dd");
            //fromDateEdit.Text = MonthFirstDay.ToString("yyyy-01-dd");   // 1월 1일로 변경 요청 - 조재식 과장

            //parent.SetMenu(3);
            //resetButton.PerformClick();
        }

        private void gridPanel_Resize(object sender, EventArgs e)
        {
            int width = gridPanel.Width;

            findButton.Left = width - 86;
            resetButton.Left = width - 86;

            itemNoEdit.Width = width - 174;
            jobNoEdit.Width = width - 174;

            integrationGrid.Size = new Size(width, gridPanel.Height - 113);
        }

        private void reportPanel_Resize(object sender, EventArgs e)
        {
            reportPagePanel.Size = new Size(reportPanel.Width, reportPanel.Height - 30);
        }

        private void fromDateEdit_ValueChanged(object sender, EventArgs e)
        {
            if (fromDateEdit.Value > toDateEdit.Value)
            {
                toDateEdit.Value = fromDateEdit.Value;
            }
        }

        private void findButton_Click(object sender, EventArgs e)
        {
            IntegrationMainDataSet set = integMainSet;
            
            bookmark.Get();
            AppHelper.ResetGridDataSource(integrationGrid);

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
            set.Select();

            AppHelper.SetGridDataSource(integrationGrid, set);

            bookmark.Goto();
            integrationGrid.Focus();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            dateCheck.Checked = true;
            fromDateEdit.Value = DateTime.Now.AddMonths(-1);
            toDateEdit.Value = DateTime.Now;
            areaCombo.SelectedIndex = 0;
            itemNoEdit.Text = string.Empty;
            jobNoEdit.Text = string.Empty;
            findButton.PerformClick();

            integRegTimeColumn.SortOrder = ColumnSortOrder.Descending;
        }

        private void integrationGridView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0)
            {
                SetReportView(EReportArea.None);
                return;
            }

            DataRow row = integrationGridView.GetDataRow(e.FocusedRowHandle);
            integMainSet.Fetch(row);

            SetReportView(integMainSet.AreaNo);
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
            reportNoEdit.Text = $"F690101/LF-CTS{integMainSet.P1FileNo}";
            issuedDateEdit.Text = $"{integMainSet.ReportedTime.ToString("yyyy. MM. dd")}";

            SetControl(ctrlUs);
            ctrlUs.SetDataSetToControl();
        }

        private void SetReportEu()
        {
            areaPanel.Text = EReportArea.EU.ToDescription();
            reportNoEdit.Text = $"F690101/LF-CTS{integMainSet.P1FileNo}";
            issuedDateEdit.Text = $"{integMainSet.ReportedTime.ToString("yyyy. MM. dd")}";

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
            DialogProfJobListView dialog = new DialogProfJobListView();

            try
            {
                dialog.Type = EReportType.Integration;
                dialog.ShowDialog();
            }
            finally
            {
                if (dialog.DialogResult == DialogResult.OK)
                {
                    profJobSet.Type = EReportType.Integration;
                    profJobSet.JobNo = dialog.JobNo;
                    profJobSet.Select();
                    profJobSet.Fetch();
                    Insert();                    
                }
            }
        }

        public void Delete()
        {
            if (integrationGridView.FocusedRowHandle < 0) return;
            if (MessageBox.Show($"Would you like to delete integration report of {integMainSet.RecNo}?",
                "SGS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            integQuery.Delete();
            findButton.PerformClick();
        }

        public void Print()
        {
            using (_longOperation.Start())
            {
                if (integrationGridView.FocusedRowHandle < 0) return;

                this.Cursor = Cursors.WaitCursor;

                integReportSet.RecNo = integMainSet.RecNo;
                integReportSet.RecNo_Chemical = profJobSet.JobNo;
                integReportSet.Pro_proj = integMainSet.P1FileNo;

                if (integMainSet.AreaNo == EReportArea.EU)
                {
                    integReportSet.Select();

                    integReportSet.DataSet.Tables[0].TableName = "P1";
                    integReportSet.DataSet.Tables[1].TableName = "T1";
                    integReportSet.DataSet.Tables[2].TableName = "T2";
                    integReportSet.DataSet.Tables[3].TableName = "T3";
                    integReportSet.DataSet.Tables[4].TableName = "T4";
                    integReportSet.DataSet.Tables[5].TableName = "T5";
                    integReportSet.DataSet.Tables[6].TableName = "T6";
                    integReportSet.DataSet.Tables[7].TableName = "T7";
                    integReportSet.DataSet.Tables[8].TableName = "LT_EU";
                    integReportSet.DataSet.Tables[9].TableName = "RT_EU";
                    integReportSet.DataSet.Tables[10].TableName = "SFLEADLT_US";
                    integReportSet.DataSet.Tables[11].TableName = "SFLEADRT_US";
                    integReportSet.DataSet.Tables[12].TableName = "SFLT_US";
                    integReportSet.DataSet.Tables[13].TableName = "SFRT_US";
                    integReportSet.DataSet.Tables[14].TableName = "SBLEADLT_US";
                    integReportSet.DataSet.Tables[15].TableName = "SBLEADRT_US";
                    integReportSet.DataSet.Tables[16].TableName = "SBLT_US";
                    integReportSet.DataSet.Tables[17].TableName = "SBRT_US";
                    integReportSet.DataSet.Tables[18].TableName = "Image";
                    integReportSet.DataSet.Tables[19].TableName = "RT_EU6";
                    integReportSet.DataSet.Tables[20].TableName = "RT_EU11";
                    integReportSet.DataSet.Tables[21].TableName = "RT_EU16";
                    integReportSet.DataSet.Tables[22].TableName = "RT_EU21";
                    integReportSet.DataSet.Tables[23].TableName = "RT_EU26";

                    integReportSet.DataSet.Tables[24].TableName = "LT_Al";
                    integReportSet.DataSet.Tables[25].TableName = "LT_As";
                    integReportSet.DataSet.Tables[26].TableName = "LT_B";
                    integReportSet.DataSet.Tables[27].TableName = "LT_Ba";
                    integReportSet.DataSet.Tables[28].TableName = "LT_Cd";
                    integReportSet.DataSet.Tables[29].TableName = "LT_Co";
                    integReportSet.DataSet.Tables[30].TableName = "LT_Cr";
                    integReportSet.DataSet.Tables[31].TableName = "LT_lll";
                    integReportSet.DataSet.Tables[32].TableName = "LT_Vl";
                    integReportSet.DataSet.Tables[33].TableName = "LT_Cu";
                    integReportSet.DataSet.Tables[34].TableName = "LT_Hg";
                    integReportSet.DataSet.Tables[35].TableName = "LT_Mn";
                    integReportSet.DataSet.Tables[36].TableName = "LT_Ni";
                    integReportSet.DataSet.Tables[37].TableName = "LT_Pb";
                    integReportSet.DataSet.Tables[38].TableName = "LT_Sb";
                    integReportSet.DataSet.Tables[39].TableName = "LT_Se";
                    integReportSet.DataSet.Tables[40].TableName = "LT_Sn";
                    integReportSet.DataSet.Tables[41].TableName = "LT_Sr";
                    integReportSet.DataSet.Tables[42].TableName = "LT_Zn";
                    integReportSet.DataSet.Tables[43].TableName = "LT_Organic";
                    integReportSet.DataSet.Tables[44].TableName = "SBLEADLT_Metal_US";
                    integReportSet.DataSet.Tables[45].TableName = "SBLEADRT_Metal_US";
                    integReportSet.DataSet.Tables[46].TableName = "T61";
                    integReportSet.DataSet.Tables[47].TableName = "RT_TIN";
                    integReportSet.DataSet.Tables[48].TableName = "RT_TIN2";
                    integReportSet.DataSet.Tables[49].TableName = "RT_TIN3";
                    integReportSet.DataSet.Tables[50].TableName = "RT_TIN4";
                    integReportSet.DataSet.Tables[51].TableName = "RT_TIN5";
                    integReportSet.DataSet.Tables[52].TableName = "RT_TIN6";

                    integReportSet.DataSet.Tables[53].TableName = "LT_MET";
                    integReportSet.DataSet.Tables[54].TableName = "LT_DBT";
                    integReportSet.DataSet.Tables[55].TableName = "LT_TBT";
                    integReportSet.DataSet.Tables[56].TableName = "LT_TeBT";
                    integReportSet.DataSet.Tables[57].TableName = "LT_MOT";
                    integReportSet.DataSet.Tables[58].TableName = "LT_DOT";
                    integReportSet.DataSet.Tables[59].TableName = "LT_DProT";
                    integReportSet.DataSet.Tables[60].TableName = "LT_DPhT";
                    integReportSet.DataSet.Tables[61].TableName = "LT_TPhT";
                    integReportSet.DataSet.Tables[62].TableName = "LT_DMT";
                    integReportSet.DataSet.Tables[63].TableName = "LT_MBT";

                    integReportSet.DataSet.Tables[64].TableName = "RT_TIN_Long_1";
                    integReportSet.DataSet.Tables[65].TableName = "RT_TIN_Long_6";
                    integReportSet.DataSet.Tables[66].TableName = "RT_TIN_Long_11";
                    integReportSet.DataSet.Tables[67].TableName = "RT_TIN_Long_16";
                    integReportSet.DataSet.Tables[68].TableName = "RT_TIN_Long_21";
                    integReportSet.DataSet.Tables[69].TableName = "T7_EU16";
                    integReportSet.DataSet.Tables[70].TableName = "T2_Clause4";
                    integReportSet.DataSet.Tables[71].TableName = "T2_Clause5";
                    integReportSet.DataSet.Tables[72].TableName = "NoCoating_NoLead_Limit";
                    integReportSet.DataSet.Tables[73].TableName = "NoCoating_NoLead_Result";
                    integReportSet.DataSet.Tables[74].TableName = "RT_PHY";
                }
                else
                {
                    integReportSet.Select_US();

                    integReportSet.DataSet.Tables[0].TableName = "P1";
                    integReportSet.DataSet.Tables[1].TableName = "T1";
                    integReportSet.DataSet.Tables[2].TableName = "T2";
                    integReportSet.DataSet.Tables[3].TableName = "T3";
                    integReportSet.DataSet.Tables[4].TableName = "T4";
                    integReportSet.DataSet.Tables[5].TableName = "T5";
                    integReportSet.DataSet.Tables[6].TableName = "T6";
                    integReportSet.DataSet.Tables[7].TableName = "T7";
                    integReportSet.DataSet.Tables[8].TableName = "LT_EU";
                    integReportSet.DataSet.Tables[9].TableName = "RT_EU";
                    integReportSet.DataSet.Tables[10].TableName = "SFLEADLT_US";
                    integReportSet.DataSet.Tables[11].TableName = "SFLEADRT_US";
                    integReportSet.DataSet.Tables[12].TableName = "SFLT_US";
                    integReportSet.DataSet.Tables[13].TableName = "SFRT_US";
                    integReportSet.DataSet.Tables[14].TableName = "SBLEADLT_US";
                    integReportSet.DataSet.Tables[15].TableName = "SBLEADRT_US";
                    integReportSet.DataSet.Tables[16].TableName = "SBLT_US";
                    integReportSet.DataSet.Tables[17].TableName = "SBRT_US";
                    integReportSet.DataSet.Tables[18].TableName = "Image";
                    //integReportSet.DataSet.Tables[19].TableName = "RT_EU6";
                    //integReportSet.DataSet.Tables[20].TableName = "RT_EU11";
                    //integReportSet.DataSet.Tables[21].TableName = "RT_EU16";
                    //integReportSet.DataSet.Tables[22].TableName = "RT_EU21";
                    //integReportSet.DataSet.Tables[23].TableName = "RT_EU26";

                    //integReportSet.DataSet.Tables[24].TableName = "LT_Al";
                    //integReportSet.DataSet.Tables[25].TableName = "LT_As";
                    //integReportSet.DataSet.Tables[26].TableName = "LT_B";
                    //integReportSet.DataSet.Tables[27].TableName = "LT_Ba";
                    //integReportSet.DataSet.Tables[28].TableName = "LT_Cd";
                    //integReportSet.DataSet.Tables[29].TableName = "LT_Co";
                    //integReportSet.DataSet.Tables[30].TableName = "LT_Cr";
                    //integReportSet.DataSet.Tables[31].TableName = "LT_lll";
                    //integReportSet.DataSet.Tables[32].TableName = "LT_Vl";
                    //integReportSet.DataSet.Tables[33].TableName = "LT_Cu";
                    //integReportSet.DataSet.Tables[34].TableName = "LT_Hg";
                    //integReportSet.DataSet.Tables[35].TableName = "LT_Mn";
                    //integReportSet.DataSet.Tables[36].TableName = "LT_Ni";
                    //integReportSet.DataSet.Tables[37].TableName = "LT_Pb";
                    //integReportSet.DataSet.Tables[38].TableName = "LT_Sb";
                    //integReportSet.DataSet.Tables[39].TableName = "LT_Se";
                    //integReportSet.DataSet.Tables[40].TableName = "LT_Sn";
                    //integReportSet.DataSet.Tables[41].TableName = "LT_Sr";
                    //integReportSet.DataSet.Tables[42].TableName = "LT_Zn";
                    //integReportSet.DataSet.Tables[43].TableName = "LT_Organic";

                    //integReportSet.DataSet.Tables[44].TableName = "SBLEADLT_Metal_US";
                    //integReportSet.DataSet.Tables[45].TableName = "SBLEADRT_Metal_US";

                    //integReportSet.DataSet.Tables[46].TableName = "T61";
                    //integReportSet.DataSet.Tables[47].TableName = "RT_TIN";
                    //integReportSet.DataSet.Tables[48].TableName = "RT_TIN2";
                    //integReportSet.DataSet.Tables[49].TableName = "RT_TIN3";
                    //integReportSet.DataSet.Tables[50].TableName = "RT_TIN4";
                    //integReportSet.DataSet.Tables[51].TableName = "RT_TIN5";
                    //integReportSet.DataSet.Tables[52].TableName = "RT_TIN6";

                    //integReportSet.DataSet.Tables[53].TableName = "LT_MET";
                    //integReportSet.DataSet.Tables[54].TableName = "LT_DBT";
                    //integReportSet.DataSet.Tables[55].TableName = "LT_TBT";
                    //integReportSet.DataSet.Tables[56].TableName = "LT_TeBT";
                    //integReportSet.DataSet.Tables[57].TableName = "LT_MOT";
                    //integReportSet.DataSet.Tables[58].TableName = "LT_DOT";
                    //integReportSet.DataSet.Tables[59].TableName = "LT_DProT";
                    //integReportSet.DataSet.Tables[60].TableName = "LT_DPhT";
                    //integReportSet.DataSet.Tables[61].TableName = "LT_TPhT";
                    //integReportSet.DataSet.Tables[62].TableName = "LT_DMT";
                    //integReportSet.DataSet.Tables[63].TableName = "LT_MBT";

                    //integReportSet.DataSet.Tables[64].TableName = "RT_TIN_Long_1";
                    //integReportSet.DataSet.Tables[65].TableName = "RT_TIN_Long_6";
                    //integReportSet.DataSet.Tables[66].TableName = "RT_TIN_Long_11";
                    //integReportSet.DataSet.Tables[67].TableName = "RT_TIN_Long_16";
                    //integReportSet.DataSet.Tables[68].TableName = "RT_TIN_Long_21";

                    //integReportSet.DataSet.Tables[69].TableName = "T7_EU16";

                    integReportSet.DataSet.Tables[19].TableName = "T2_Clause4";
                    integReportSet.DataSet.Tables[20].TableName = "T2_Clause5";

                    integReportSet.DataSet.Tables[21].TableName = "Coating_Lead_Limit";
                    integReportSet.DataSet.Tables[22].TableName = "NoCoating_Lead_Limit_Plastic";
                    integReportSet.DataSet.Tables[23].TableName = "NoCoating_Lead_Limit_Metal";

                    integReportSet.DataSet.Tables[24].TableName = "Coating_Lead_Result";
                    integReportSet.DataSet.Tables[25].TableName = "NoCoating_Lead_Result_Plastic";
                    integReportSet.DataSet.Tables[26].TableName = "NoCoating_Lead_Result_Metal";

                    integReportSet.DataSet.Tables[27].TableName = "Coating_NoLead_Limit";
                    integReportSet.DataSet.Tables[28].TableName = "NoCoating_NoLead_Limit";

                    integReportSet.DataSet.Tables[29].TableName = "Coating_NoLead_Result";
                    integReportSet.DataSet.Tables[30].TableName = "NoCoating_NoLead_Result";

                    integReportSet.DataSet.Tables[31].TableName = "NoCoating_Lead_Result";

                    integReportSet.DataSet.Tables[32].TableName = "RT_PHY";

                    integReportSet.DataSet.Tables[33].TableName = "T7_2";

                    integReportSet.DataSet.Tables[34].TableName = "NoCoating_NoLead_Result_20_39";
                    integReportSet.DataSet.Tables[35].TableName = "NoCoating_NoLead_Result_40_59";

                    integReportSet.DataSet.Tables[36].TableName = "RT_PHY_3";
                }

                BindingSource bind = new BindingSource();
                bind.DataSource = integReportSet.DataSet;

                XtraReport report;

                if (integMainSet.AreaNo == EReportArea.US)
                    report = new ReportUsIntegration();
                else
                    report = new ReportEuIntegration();

                ReportPrintTool tool = new ReportPrintTool(report);

                report.DataSource = bind;
                report.CreateDocument();

                this.Cursor = Cursors.Default;

                DialogReportPreview dialog = new DialogReportPreview();
                dialog.Source = report;
                dialog.WindowState = FormWindowState.Maximized;
                dialog.ShowDialog();
            }            
        }

        public void Save()
        {
            if (integrationGridView.FocusedRowHandle < 0) return;
            if (MessageBox.Show($"Would you like to save integration report of {integMainSet.RecNo}?",
                "SGS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            integQuery.Update();
            findButton.PerformClick();
        }

        public void Cancel()
        {
            if (integrationGridView.FocusedRowHandle < 0) return;
            if (MessageBox.Show($"Would you like to cancel physical report of {integMainSet.RecNo}?",
                "SGS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            SetReportView(integMainSet.AreaNo);
        }

        private void Insert()
        {
            EReportArea area = profJobSet.AreaNo;

            if (profJobSet.Empty == true) return;
            if (area == EReportArea.None) return;

            integCheckSet.RecNo = $"*{profJobSet.JobNo}";
            integCheckSet.Select();

            if (integCheckSet.Empty == false)
            {
                MessageBox.Show("Can't import integration report because this report already exist in DB!",
                    "SGS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            productSet.Valid = false;
            productSet.AreaNo = profJobSet.AreaNo;
            productSet.Code = profJobSet.ItemNo;
            productSet.IntegJobNo = "";
            productSet.SelectDetail();
            productSet.Fetch();

            if (productSet.Empty == true)
            {
                MessageBox.Show("Can't import integration report!\r\nPlease check physical or chemical reports being linked BOM!",
                    "SGS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            integQuery.Insert();
            findButton.PerformClick();
        }
    }
}
