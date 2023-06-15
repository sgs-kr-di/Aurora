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

        private ProfJobDataSet profJobSet;

        private ProfJobSchemeDataSet profJobSchemeSet;

        private ChemicalQuery cheQuery;

        private CtrlEditChemicalUs ctrlUs;

        private CtrlEditChemicalEu ctrlEu;

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
            profJobSet = new ProfJobDataSet(AppRes.DB.Connect, null, null);
            profJobSchemeSet = new ProfJobSchemeDataSet(AppRes.DB.Connect, null, null);

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
        }

        private void CtrlEditChemical_Enter(object sender, EventArgs e)
        {
            parent.SetMenu(2);
            resetButton.PerformClick();

            ChemicalMainDataSet set = cheMainSet;
            bookmark.Get();

            AppHelper.ResetGridDataSource(chemicalGrid);
            
            set.From = "";
            set.To = "";

            set.RecNo = Program.sGetPhysicalJobno();

            set.AreaNo = (EReportArea)Program.iGetPhysicalAreano();
            set.ReportApproval = (EReportApproval)approvalCombo.SelectedValue;
            set.MaterialNo = Program.sGetPartMaterialNo();
            set.RecNo = jobNoEdit.Text.Trim();
            set.Select();

            AppHelper.SetGridDataSource(chemicalGrid, set);

            bookmark.Goto();
            chemicalGrid.Focus();

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
                        MessageBox.Show("Chemical Completed!");
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
            if (chemicalGridView.FocusedRowHandle < 0) return;

            this.Cursor = Cursors.WaitCursor;

            cheReportSet.RecNo = cheMainSet.RecNo;
            cheReportSet.Select();

            cheReportSet.DataSet.Tables[0].TableName = "P1";
            cheReportSet.DataSet.Tables[1].TableName = "P2";
            cheReportSet.DataSet.Tables[2].TableName = "P2EXTEND";
            cheReportSet.DataSet.Tables[3].TableName = "Image";

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

            profJobSet.Type = EReportType.Chemical;
            profJobSet.JobNo = jobNo;
            profJobSet.AreaNo = areaNo;
            profJobSet.ItemNo = itemNo;
            profJobSet.ExtendASTM = true;
            profJobSet.Select();

            int rowCount = profJobSet.RowCount;
            if (rowCount > 0)
            {
                profJobSet.Fetch(0);
                jobNo = profJobSet.JobNo;

                if (string.IsNullOrWhiteSpace(jobNo) == false)
                {
                    cheCheckSet.Select(jobNo);

                    if (cheCheckSet.Empty == false)
                    {
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
                        cheQuery.Insert_Chemical_Import(areaNo, jobNo);
                        //cheQuery.Insert(extendJobNo);
                    }
                }
            }

            findButton.PerformClick();
        }
    }
}
