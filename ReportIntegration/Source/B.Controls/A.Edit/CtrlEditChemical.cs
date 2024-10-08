﻿using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraReports.UI;

using Ulee.Controls;
using Ulee.Utils;
using DevExpress.CodeParser;
using LoadingIndicator.WinForms;

namespace Sgs.ReportIntegration
{
    public partial class CtrlEditChemical : UlUserControlEng
    {
        private CtrlEditRight parent;

        private GridBookmark bookmark;

        private ChemicalMainDataSet cheCheckSet;

        private ChemicalMainDataSet cheMainSet;

        private ChemicalItemJoinDataSet cheJoinSet;

        private ChemicalImageDataSet cheImageSet;

        private ChemicalP2DataSet cheP2Set;

        private ChemicalP2ExtendDataSet cheP2ExtendSet;

        private ChemicalReportDataSet cheReportSet;

        private ChemicalReportDataSet cheReportUsSet;

        private ProfJobDataSet profJobSet;

        private ProfJobImageDataSet profJobImageSet;

        private ProfJobSchemeDataSet profJobSchemeSet;

        private ChemicalQuery cheQuery;

        private CtrlEditChemicalUs ctrlUs;

        private CtrlEditChemicalEu ctrlEu;

        public bool ChkAlready;

        private LongOperation _longOperation;

        public CtrlEditChemical(CtrlEditRight parent)
        {
            this.parent = parent;

            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            cheCheckSet = new ChemicalMainDataSet(AppRes.DB.Connect, null, null);
            cheMainSet = new ChemicalMainDataSet(AppRes.DB.Connect, null, null);
            cheJoinSet = new ChemicalItemJoinDataSet(AppRes.DB.Connect, null, null);
            cheImageSet = new ChemicalImageDataSet(AppRes.DB.Connect, null, null);
            cheP2Set = new ChemicalP2DataSet(AppRes.DB.Connect, null, null);
            cheP2ExtendSet = new ChemicalP2ExtendDataSet(AppRes.DB.Connect, null, null);
            cheReportSet = new ChemicalReportDataSet(AppRes.DB.Connect, null, null);
            cheReportUsSet = new ChemicalReportDataSet(AppRes.DB.Connect, null, null);
            profJobSet = new ProfJobDataSet(AppRes.DB.Connect, null, null);
            profJobSchemeSet = new ProfJobSchemeDataSet(AppRes.DB.Connect, null, null);
            profJobImageSet = new ProfJobImageDataSet(AppRes.DB.Connect, null, null);

            ctrlUs = new CtrlEditChemicalUs(findButton);
            ctrlUs.MainSet = cheMainSet;
            ctrlUs.P2Set = cheP2Set;
            ctrlUs.P2ExtendSet = cheP2ExtendSet;
            ctrlUs.ImageSet = cheImageSet;

            ctrlEu = new CtrlEditChemicalEu(findButton);
            ctrlEu.MainSet = cheMainSet;
            ctrlEu.P2Set = cheP2Set;
            ctrlEu.ImageSet = cheImageSet;

            cheQuery = new ChemicalQuery();
            cheQuery.MainSet = cheMainSet;
            cheQuery.ImageSet = cheImageSet;
            cheQuery.JoinSet = cheJoinSet;
            cheQuery.P2Set = cheP2Set;
            cheQuery.P2ExtendSet = cheP2ExtendSet;
            cheQuery.ProfJobSet = profJobSet;
            cheQuery.ProfJobSchemeSet = profJobSchemeSet;
            cheQuery.CtrlUs = ctrlUs;
            cheQuery.CtrlEu = ctrlEu;
            cheQuery.ProfImageJobSet = profJobImageSet;

            bookmark = new GridBookmark(chemicalGridView);
            AppHelper.SetGridEvenRow(chemicalGridView);

            chemicalAreaColumn.DisplayFormat.FormatType = FormatType.Custom;
            chemicalAreaColumn.DisplayFormat.Format = new ReportAreaFormat();

            chemicalRegTimeColumn.DisplayFormat.FormatType = FormatType.Custom;
            chemicalRegTimeColumn.DisplayFormat.Format = new ReportDateTimeFormat();

            approvalCombo.DataSource = EnumHelper.GetNameValues<EReportApproval>();
            approvalCombo.DisplayMember = "Name";
            approvalCombo.ValueMember = "Value";

            areaCombo.DataSource = EnumHelper.GetNameValues<EReportArea>();
            areaCombo.DisplayMember = "Name";
            areaCombo.ValueMember = "Value";            

            SetControl(null);

            ChkAlready = false;

            // Initialize long operation with control which will
            // be overlayed during long operations
            _longOperation = new LongOperation(this);

            // You can pass settings to customize indicator view/behavior
            // _longOperation = new LongOperation(this, LongOperationSettings.Default);
        }

        private void CtrlEditChemical_Enter(object sender, EventArgs e)
        {
            parent.SetMenu(2);
            //resetButton.PerformClick();

            ChemicalMainDataSet set = cheMainSet;
            bookmark.Get();

            AppHelper.ResetGridDataSource(chemicalGrid);
            
            set.From = "";
            set.To = "";
            set.RecNo = jobNoEdit.Text.Trim();

            if (!string.IsNullOrEmpty(set.RecNo)) 
            {
                set.RecNo = Program.sGetPhysicalJobno();

                set.AreaNo = (EReportArea)Program.iGetPhysicalAreano();
                set.ReportApproval = (EReportApproval)approvalCombo.SelectedValue;
                set.MaterialNo = Program.sGetPartMaterialNo();
                
                //set.Select();

                set.P1FileNo = "";
                set.Select();

                AppHelper.SetGridDataSource(chemicalGrid, set);

                bookmark.Goto();
                chemicalGrid.Focus();
            }

            DateTime MonthFirstDay = DateTime.Now.AddDays(1 - DateTime.Now.Day);

            fromDateEdit.Text = MonthFirstDay.ToString("yyyy-MM-dd");
            //fromDateEdit.Text = MonthFirstDay.ToString("yyyy-01-dd");   // 1월 1일로 변경 요청 - 조재식 과장

            //parent.SetMenu(2);
            //resetButton.PerformClick();
        }

        private void findButton_Click(object sender, EventArgs e)
        {
            ChemicalMainDataSet set = cheMainSet;
            bookmark.Get();

            AppHelper.ResetGridDataSource(chemicalGrid);

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
            set.MaterialNo = itemNoEdit.Text.Trim();
            set.RecNo = jobNoEdit.Text.Trim();
            set.P1FileNo = OmNoEdit.Text.Trim();
            set.Select();

            AppHelper.SetGridDataSource(chemicalGrid, set);

            bookmark.Goto();
            chemicalGrid.Focus();
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
            findButton.PerformClick();
            OmNoEdit.Text = string.Empty;

            chemicalRegTimeColumn.SortOrder = ColumnSortOrder.Descending;
        }

        private void chemicalGridView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0)
            {
                SetReportView(EReportArea.None);
                return;
            }

            DataRow row = chemicalGridView.GetDataRow(e.FocusedRowHandle);
            cheMainSet.Fetch(row);

            SetReportView(cheMainSet.AreaNo);
        }

        private void gridPanel_Resize(object sender, EventArgs e)
        {
            int width = gridPanel.Width;

            findButton.Left = width - 86;
            resetButton.Left = width - 86;

            itemNoEdit.Width = width - 174;
            jobNoEdit.Width = width - 174;
            OmNoEdit.Width = width - 174;

            chemicalGrid.Size = new Size(width, gridPanel.Height - 113);
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
            reportNoEdit.Text = $"F690101/LF-CTS{cheMainSet.P1FileNo}";
            issuedDateEdit.Text = $"{cheMainSet.ReportedTime.ToString("yyyy. MM. dd")}";

            SetControl(ctrlUs);
            ctrlUs.SetDataSetToControl();
        }

        private void SetReportEu()
        {
            areaPanel.Text = EReportArea.EU.ToDescription();
            reportNoEdit.Text = $"F690101/LF-CTS{cheMainSet.P1FileNo}";
            issuedDateEdit.Text = $"{cheMainSet.ReportedTime.ToString("yyyy. MM. dd")}";

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
                    dialog.Type = EReportType.Chemical;
                    dialog.ShowDialog();
                }
                finally
                {
                    if (dialog.DialogResult == DialogResult.OK)
                    {
                        if (dialog.AreaNo == EReportArea.None)
                        {
                            MessageBox.Show("Can't import chemical report because AreaNo is none!",
                                "SGS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (string.IsNullOrWhiteSpace(dialog.ItemNo) == true)
                        {
                            MessageBox.Show("Can't import chemical report because MaterialNo is none!",
                                "SGS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            Insert(dialog.AreaNo, dialog.ItemNo.Split(',')[0], dialog.JobNo);

                            if (!ChkAlready)
                            {
                                if (cheQuery.ChkErr)
                                {
                                    MessageBox.Show("Import failed!");
                                }
                                else
                                {
                                    MessageBox.Show("Import completed successfully!");
                                }
                            }
                            //MessageBox.Show("Chemical Completed!");
                        }
                    }
                }
            }
        }

        public void Delete()
        {
            if (chemicalGridView.FocusedRowHandle < 0) return;
            if (MessageBox.Show($"Would you like to delete chemical report of {cheMainSet.RecNo}?",
                "SGS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            cheQuery.Delete();

            findButton.PerformClick();
        }

        public void Print()
        {
            using (_longOperation.Start())
            {
                if (chemicalGridView.FocusedRowHandle < 0) return;

                this.Cursor = Cursors.WaitCursor;

                if (cheMainSet.AreaNo == EReportArea.EU)
                {
                    cheReportSet.RecNo = cheMainSet.RecNo;
                    //cheReportSet.SampleIdent = profJobSchemeSet.SAMPLEIDENT;
                    cheReportSet.Select();

                    cheReportSet.DataSet.Tables[0].TableName = "P1";
                    cheReportSet.DataSet.Tables[1].TableName = "P2";
                    cheReportSet.DataSet.Tables[2].TableName = "P2EXTEND";
                    cheReportSet.DataSet.Tables[3].TableName = "Image";

                    cheReportSet.DataSet.Tables[4].TableName = "RT_EU";
                    cheReportSet.DataSet.Tables[5].TableName = "RT_EU6";
                    cheReportSet.DataSet.Tables[6].TableName = "RT_EU11";
                    cheReportSet.DataSet.Tables[7].TableName = "RT_EU16";
                    cheReportSet.DataSet.Tables[8].TableName = "RT_EU21";
                    cheReportSet.DataSet.Tables[9].TableName = "RT_EU26";

                    cheReportSet.DataSet.Tables[10].TableName = "LT_Al";
                    cheReportSet.DataSet.Tables[11].TableName = "LT_As";
                    cheReportSet.DataSet.Tables[12].TableName = "LT_B";
                    cheReportSet.DataSet.Tables[13].TableName = "LT_Ba";
                    cheReportSet.DataSet.Tables[14].TableName = "LT_Cd";
                    cheReportSet.DataSet.Tables[15].TableName = "LT_Co";
                    cheReportSet.DataSet.Tables[16].TableName = "LT_Cr";
                    cheReportSet.DataSet.Tables[17].TableName = "LT_lll";
                    cheReportSet.DataSet.Tables[18].TableName = "LT_Vl";
                    cheReportSet.DataSet.Tables[19].TableName = "LT_Cu";
                    cheReportSet.DataSet.Tables[20].TableName = "LT_Hg";
                    cheReportSet.DataSet.Tables[21].TableName = "LT_Mn";
                    cheReportSet.DataSet.Tables[22].TableName = "LT_Ni";
                    cheReportSet.DataSet.Tables[23].TableName = "LT_Pb";
                    cheReportSet.DataSet.Tables[24].TableName = "LT_Sb";
                    cheReportSet.DataSet.Tables[25].TableName = "LT_Se";
                    cheReportSet.DataSet.Tables[26].TableName = "LT_Sn";
                    cheReportSet.DataSet.Tables[27].TableName = "LT_Sr";
                    cheReportSet.DataSet.Tables[28].TableName = "LT_Zn";
                    cheReportSet.DataSet.Tables[29].TableName = "LT_Organic";

                    cheReportSet.DataSet.Tables[30].TableName = "RT_TIN";
                    cheReportSet.DataSet.Tables[31].TableName = "RT_TIN6";
                    cheReportSet.DataSet.Tables[32].TableName = "RT_TIN11";
                    cheReportSet.DataSet.Tables[33].TableName = "RT_TIN16";
                    cheReportSet.DataSet.Tables[34].TableName = "RT_TIN21";
                    cheReportSet.DataSet.Tables[35].TableName = "RT_TIN26";

                    cheReportSet.DataSet.Tables[36].TableName = "LT_MET";
                    cheReportSet.DataSet.Tables[37].TableName = "LT_DBT";
                    cheReportSet.DataSet.Tables[38].TableName = "LT_TBT";
                    cheReportSet.DataSet.Tables[39].TableName = "LT_TeBT";
                    cheReportSet.DataSet.Tables[40].TableName = "LT_MOT";
                    cheReportSet.DataSet.Tables[41].TableName = "LT_DOT";
                    cheReportSet.DataSet.Tables[42].TableName = "LT_DProT";
                    cheReportSet.DataSet.Tables[43].TableName = "LT_DPhT";
                    cheReportSet.DataSet.Tables[44].TableName = "LT_TPhT";
                    cheReportSet.DataSet.Tables[45].TableName = "LT_DMT";
                    cheReportSet.DataSet.Tables[46].TableName = "LT_MBT";
                }
                else
                {
                    cheReportSet.RecNo = cheMainSet.RecNo;
                    cheReportSet.Pro_proj = cheMainSet.P1FileNo;
                    //cheReportSet.Sam_Remarks = profJobSchemeSet.SampleRemarks;
                    //cheReportSet.SampleIdent = profJobSchemeSet.SAMPLEIDENT;
                    cheReportSet.Select_US();

                    cheReportSet.DataSet.Tables[0].TableName = "P1";
                    cheReportSet.DataSet.Tables[1].TableName = "P2";
                    cheReportSet.DataSet.Tables[2].TableName = "P2EXTEND";
                    cheReportSet.DataSet.Tables[3].TableName = "Image";

                    cheReportSet.DataSet.Tables[4].TableName = "Coating_Lead_Limit";
                    cheReportSet.DataSet.Tables[5].TableName = "NoCoating_Lead_Limit_Plastic";
                    cheReportSet.DataSet.Tables[6].TableName = "NoCoating_Lead_Limit_Metal";

                    cheReportSet.DataSet.Tables[7].TableName = "Coating_Lead_Result";
                    cheReportSet.DataSet.Tables[8].TableName = "NoCoating_Lead_Result_Plastic";
                    cheReportSet.DataSet.Tables[9].TableName = "NoCoating_Lead_Result_Metal";

                    cheReportSet.DataSet.Tables[10].TableName = "Coating_NoLead_Limit";
                    cheReportSet.DataSet.Tables[11].TableName = "NoCoating_NoLead_Limit";

                    cheReportSet.DataSet.Tables[12].TableName = "Coating_NoLead_Result";
                    cheReportSet.DataSet.Tables[13].TableName = "NoCoating_NoLead_Result";

                    cheReportSet.DataSet.Tables[14].TableName = "NoCoating_Lead_Result";

                    cheReportSet.DataSet.Tables[15].TableName = "RT_PHY";
                    cheReportSet.DataSet.Tables[16].TableName = "P1_SubJob1";
                    cheReportSet.DataSet.Tables[17].TableName = "P1_SubJob2";
                    cheReportSet.DataSet.Tables[18].TableName = "P1_requiredTime_Last";
                    cheReportSet.DataSet.Tables[19].TableName = "P1_testPeriod_Last";

                    /*
                    cheReportSet.DataSet.Tables[10].TableName = "LT_Al";
                    cheReportSet.DataSet.Tables[11].TableName = "LT_As";
                    cheReportSet.DataSet.Tables[12].TableName = "LT_B";
                    cheReportSet.DataSet.Tables[13].TableName = "LT_Ba";
                    cheReportSet.DataSet.Tables[14].TableName = "LT_Cd";
                    cheReportSet.DataSet.Tables[15].TableName = "LT_Co";
                    cheReportSet.DataSet.Tables[16].TableName = "LT_Cr";
                    cheReportSet.DataSet.Tables[17].TableName = "LT_lll";
                    cheReportSet.DataSet.Tables[18].TableName = "LT_Vl";
                    cheReportSet.DataSet.Tables[19].TableName = "LT_Cu";
                    cheReportSet.DataSet.Tables[20].TableName = "LT_Hg";
                    cheReportSet.DataSet.Tables[21].TableName = "LT_Mn";
                    cheReportSet.DataSet.Tables[22].TableName = "LT_Ni";
                    cheReportSet.DataSet.Tables[23].TableName = "LT_Pb";
                    cheReportSet.DataSet.Tables[24].TableName = "LT_Sb";
                    cheReportSet.DataSet.Tables[25].TableName = "LT_Se";
                    cheReportSet.DataSet.Tables[26].TableName = "LT_Sn";
                    cheReportSet.DataSet.Tables[27].TableName = "LT_Sr";
                    cheReportSet.DataSet.Tables[28].TableName = "LT_Zn";
                    cheReportSet.DataSet.Tables[29].TableName = "LT_Organic";

                    cheReportSet.DataSet.Tables[30].TableName = "RT_TIN";
                    cheReportSet.DataSet.Tables[31].TableName = "RT_TIN6";
                    cheReportSet.DataSet.Tables[32].TableName = "RT_TIN11";
                    cheReportSet.DataSet.Tables[33].TableName = "RT_TIN16";
                    cheReportSet.DataSet.Tables[34].TableName = "RT_TIN21";
                    cheReportSet.DataSet.Tables[35].TableName = "RT_TIN26";

                    cheReportSet.DataSet.Tables[36].TableName = "LT_MET";
                    cheReportSet.DataSet.Tables[37].TableName = "LT_DBT";
                    cheReportSet.DataSet.Tables[38].TableName = "LT_TBT";
                    cheReportSet.DataSet.Tables[39].TableName = "LT_TeBT";
                    cheReportSet.DataSet.Tables[40].TableName = "LT_MOT";
                    cheReportSet.DataSet.Tables[41].TableName = "LT_DOT";
                    cheReportSet.DataSet.Tables[42].TableName = "LT_DProT";
                    cheReportSet.DataSet.Tables[43].TableName = "LT_DPhT";
                    cheReportSet.DataSet.Tables[44].TableName = "LT_TPhT";
                    cheReportSet.DataSet.Tables[45].TableName = "LT_DMT";
                    cheReportSet.DataSet.Tables[46].TableName = "LT_MBT";
                    */
                }

                BindingSource bind = new BindingSource();
                bind.DataSource = cheReportSet.DataSet;

                XtraReport report;

                if (cheMainSet.AreaNo == EReportArea.US)
                    report = new ReportUsChemical();
                else
                    report = new ReportEuChemical();

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
            if (chemicalGridView.FocusedRowHandle < 0) return;
            if (MessageBox.Show($"Would you like to save chemical report of {cheMainSet.RecNo}?",
                "SGS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            cheQuery.Update();
            findButton.PerformClick();
        }

        public void Cancel()
        {
            if (chemicalGridView.FocusedRowHandle < 0) return;
            if (MessageBox.Show($"Would you like to cancel chemical report of {cheMainSet.RecNo}?",
                "SGS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            SetReportView(cheMainSet.AreaNo);
        }

        private void Insert(EReportArea areaNo, string itemNo, string jobNo)
        {
            //string jobNo = "";
            string extendJobNo = "";
            string fileNo = "";

            profJobSet.Type = EReportType.Chemical;
            profJobSet.JobNo = jobNo;
            profJobSet.AreaNo = areaNo;
            profJobSet.ItemNo = itemNo;
            profJobSet.ExtendASTM = true;
            profJobSet.Select_KRSCT01();

            int rowCount = profJobSet.RowCount;
            if (rowCount > 0)
            {
                profJobSet.Fetch(0);
                jobNo = profJobSet.JobNo;
                fileNo = profJobSet.FileNo;

                if (string.IsNullOrWhiteSpace(jobNo) == false)
                {
                    cheCheckSet.Select(jobNo);

                    if (cheCheckSet.Empty == false)
                    {
                        ChkAlready = true;

                        MessageBox.Show("Can't import chemical report because this report already exist in DB!",
                            "SGS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        //if (rowCount > 1)
                        //{
                        //    // Find jobno for additional test results
                        //    for (int i = 1; i < profJobSet.RowCount; i++)
                        //    {
                        //        profJobSet.Fetch(i);

                        //        if (profJobSet.Image == null)
                        //        {
                        //            extendJobNo = profJobSet.JobNo;
                        //            break;
                        //        }
                        //    }
                        //    profJobSet.Fetch(0);
                        //}
                        //cheQuery.Insert(areaNo, extendJobNo);
                        cheQuery.Insert_Chemical_Import(areaNo, jobNo, fileNo);

                        //cheQuery.Insert(extendJobNo);
                    }
                }
            }
            findButton.PerformClick();
        }
    }
}