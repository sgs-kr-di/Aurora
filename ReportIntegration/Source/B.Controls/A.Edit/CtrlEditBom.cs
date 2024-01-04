using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using DevExpress.Data;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

using Ulee.Controls;
using Ulee.Utils;
using Sgs.ReportIntegration.Source.A.Forms.B.Dialog;

namespace Sgs.ReportIntegration
{
    public partial class CtrlEditBom : UlUserControlEng
    {
        private CtrlEditRight parent;

        private GridBookmark bookmark;

        private BomDataSet bomSet;

        private BomDataSet bomCheckSet;

        private ProductDataSet productSet;

        private PartDataSet partSet;

        private IntegrationMainDataSet integMainSet;

        private PhysicalMainDataSet phyMainSet;

        private ChemicalMainDataSet cheMainSet;

        private BomColumns bomRec;

        private IntegrationQuery integQuery;

        private PhysicalQuery phyQuery;

        private ChemicalQuery cheQuery;

        public CtrlEditBom(CtrlEditRight parent)
        {
            this.parent = parent;

            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            AppRes.TotalLog.LoggerMessage += DoLoggerMessage;

            bomSet = new BomDataSet(AppRes.DB.Connect, null, null);
            bomCheckSet = new BomDataSet(AppRes.DB.Connect, null, null);
            productSet = new ProductDataSet(AppRes.DB.Connect, null, null);
            partSet = new PartDataSet(AppRes.DB.Connect, null, null);
            integMainSet = new IntegrationMainDataSet(AppRes.DB.Connect, null, null);
            phyMainSet = new PhysicalMainDataSet(AppRes.DB.Connect, null, null);
            cheMainSet = new ChemicalMainDataSet(AppRes.DB.Connect, null, null);

            integQuery = new IntegrationQuery(true);
            phyQuery = new PhysicalQuery(true);
            cheQuery = new ChemicalQuery(true);

            bomRec = new BomColumns();

            bookmark = new GridBookmark(bomGridView);

            AppHelper.SetGridEvenRow(bomGridView);
            AppHelper.SetGridEvenRow(productGridView);
            AppHelper.SetGridEvenRow(partGridView);

            bomAreaColumn.DisplayFormat.FormatType = FormatType.Custom;
            bomAreaColumn.DisplayFormat.Format = new ReportAreaFormat();

            bomRegTimeColumn.DisplayFormat.FormatType = FormatType.Custom;
            bomRegTimeColumn.DisplayFormat.Format = new ReportDateTimeFormat();

            bomAreaCombo.DataSource = EnumHelper.GetNameValues<EReportArea>();
            bomAreaCombo.DisplayMember = "Name";
            bomAreaCombo.ValueMember = "Value";
        }

        private void CtrlEditBom_Load(object sender, EventArgs e)
        {
        }

        private void CtrlEditBom_Enter(object sender, EventArgs e)
        {
            parent.SetMenu(0);
            messageLogEdit.Clear();
            bomResetButton.PerformClick();
        }

        private void CtrlEditBom_Resize(object sender, EventArgs e)
        {
        }

        private void gridPanel_Resize(object sender, EventArgs e)
        {
            bomFindButton.Left = gridPanel.Width - 86;
            bomResetButton.Left = gridPanel.Width - 86;
            btnChkBom.Left = gridPanel.Width - 86;
            bomNameEdit.Width = gridPanel.Width - 56;
            bomGrid.Size = new Size(gridPanel.Width, gridPanel.Height - 110);
        }

        private void bomProductPage_Resize(object sender, EventArgs e)
        {
            int width = bomProductPage.Width;

            productNameColumn.Width = width - 374;
            partNameColumn.Width = 171 + (width - 377 - 171) / 2;
            partMaterialNameColumn.Width = 171 + (width - 377 - 171) / 2;
        }

        private void bomFindButton_Click(object sender, EventArgs e)
        {
            BomDataSet set = bomSet;
            bookmark.Get();

            AppHelper.ResetGridDataSource(bomGrid);

            if (bomDateCheck.Checked == true)
            {
                set.From = bomFromDateEdit.Value.ToString(AppRes.csDateFormat);
                set.To = bomToDateEdit.Value.ToString(AppRes.csDateFormat);
            }
            else
            {
                set.To = "";
                set.From = "";
            }

            set.AreaNo = (EReportArea)bomAreaCombo.SelectedValue;
            set.FName = bomNameEdit.Text.Trim();
            set.Select();

            AppHelper.SetGridDataSource(bomGrid, set);

            bookmark.Goto();
            bomGrid.Focus();
        }

        private void bomResetButton_Click(object sender, EventArgs e)
        {
            bomDateCheck.Checked = true;
            bomFromDateEdit.Value = DateTime.Now.AddMonths(-1);
            bomToDateEdit.Value = DateTime.Now;
            bomAreaCombo.SelectedIndex = 0;
            bomNameEdit.Text = string.Empty;
            bomFindButton.PerformClick();

            bomRegTimeColumn.SortOrder = ColumnSortOrder.Descending;
            AppHelper.ResetGridSortOrder(productGridView);
            AppHelper.ResetGridSortOrder(partGridView);
        }

        private void bomGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            AppHelper.ResetGridDataSource(productGrid);

            if (e.FocusedRowHandle < 0) return;

            DataRow row = bomGridView.GetDataRow(e.FocusedRowHandle);
            bomSet.Fetch(row);

            productSet.BomNo = bomSet.RecNo;
            productSet.Select();

            AppHelper.SetGridDataSource(productGrid, productSet);

            if (bomTab.SelectedIndex == 1)
            {
                LoadExcel();
            }
        }

        private void productGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            AppHelper.ResetGridDataSource(partGrid);

            if (e.FocusedRowHandle < 0) return;

            DataRow row = productGridView.GetDataRow(e.FocusedRowHandle);
            productSet.Fetch(row);

            partSet.ProductNo = productSet.RecNo;
            partSet.Select();
            partSet.Fetch();

            AppHelper.SetGridDataSource(partGrid, partSet);
        }

        private void productGrid_Click(object sender, EventArgs e)
        {
            Program.sSetPhysicalCode(productSet.Code);
            Program.sSetPhysicalJobno(productSet.PhyJobNo);
            Program.sSetPhysicalIntegJobno(productSet.IntegJobNo);
            Program.iSetPhysicalAreano((int)productSet.AreaNo);
            /*
            MessageBox.Show(productSet.Code);
            MessageBox.Show(productSet.PhyJobNo);
            MessageBox.Show(productSet.IntegJobNo);
            MessageBox.Show(productSet.AreaNo.ToString());
            */
        }

        private void productGrid_DoubleClick(object sender, EventArgs e)
        {
            if (productGridView.DataRowCount == 0) return;

            DialogProductView view = new DialogProductView();
            view.ItemNo = productSet.Code;
            view.Product = productSet.Name;
            view.ImageBox = productSet.Image;
            view.ShowDialog();
        }

        private void partGrid_Click(object sender, EventArgs e)
        {
            Program.sSetPartMaterialNo(partSet.MaterialNo);
        }

        private void bomFromDateEdit_ValueChanged(object sender, EventArgs e)
        {
            if (bomFromDateEdit.Value > bomToDateEdit.Value)
            {
                bomToDateEdit.Value = bomFromDateEdit.Value;
            }
        }

        private void bomToDateEdit_ValueChanged(object sender, EventArgs e)
        {
            if (bomToDateEdit.Value < bomFromDateEdit.Value)
            {
                bomFromDateEdit.Value = bomToDateEdit.Value;
            }
        }

        private void bomTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bomTab.SelectedIndex == 1)
            {
                LoadExcel();
            }
        }

        public void Import(EReportArea area, string fName)
        {
            bomCheckSet.AreaNo = area;
            bomCheckSet.FName = Path.GetFileName(fName);
            bomCheckSet.From = "";
            bomCheckSet.To = "";
            bomCheckSet.Select();

            // if BOM already exist in DB?
            if (bomCheckSet.Empty == false)
            {
                AppRes.TotalLog["ERROR"] = $"Can't add BOM because that already exists in DB! - {area.ToDescription()}, {bomRec.FName}";

                MessageBox.Show("Can't add BOM because that already exists in DB!",
                    "SGS", MessageBoxButtons.OK, MessageBoxIcon.Error);

                bomFindButton.PerformClick();
                return;
            }

            messageLogEdit.Clear();

            if (bomExcelSheet.LoadDocument(fName) == true)
            {
                bomRec.Clear();
                bomRec.RegTime = DateTime.Now;
                bomRec.AreaNo = area;
                bomRec.FName = Path.GetFileName(fName);
                bomRec.FPath = Path.GetDirectoryName(fName);
                AppRes.TotalLog["NOTE"] = $"Loaded BOM file - {area.ToDescription()}, {bomRec.FName}";

                if (ExtractProduct(bomExcelSheet.Document.Worksheets[0]) == true)
                {
                    try
                    {
                        InsertBom();
                    }
                    finally
                    {
                        bomFindButton.PerformClick();
                    }
                    MessageBox.Show("Import completed successfully!");
                }
                else
                {
                    AppRes.TotalLog["ERROR"] = $"Extracting BOM file is failed";
                    MessageBox.Show("Can't load BOM file because of its invalid format!",
                        "SGS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void Delete()
        {
            if (bomGridView.FocusedRowHandle < 0) return;
            if (MessageBox.Show($"Would you like to delete chosen BOM?",
                "SGS", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            SqlTransaction trans = AppRes.DB.BeginTrans();

            try
            {
                for (int i = 0; i < productSet.RowCount; i++)
                {
                    productSet.Fetch(i);

                    if (string.IsNullOrWhiteSpace(productSet.IntegJobNo) == false)
                    {
                        DeleteIntegration(productSet.RecNo, productSet.AreaNo, trans);
                    }

                    partSet.ProductNo = productSet.RecNo;
                    partSet.Delete(trans);
                }

                productSet.BomNo = bomSet.RecNo;
                productSet.Delete(trans);

                bomSet.Delete(trans);

                AppRes.DB.CommitTrans();
            }
            catch
            {
                AppRes.DB.RollbackTrans();
            }

            bomFindButton.PerformClick();
        }

        private void DeleteIntegration(Int64 recNo, EReportArea areaNo, SqlTransaction trans)
        {

        }

        private void LoadExcel()
        {
            if (bomSet.Empty == true) return;

            string newFName = Path.Combine(bomSet.FPath, bomSet.FName);
            string curFName = bomExcelSheet.Options.Save.CurrentFileName;

            if (curFName != newFName)
            {
                Cursor.Current = Cursors.WaitCursor;

                try
                {
                    bomExcelSheet.LoadDocument(newFName);
                    bomExcelSheet.ActiveViewZoom = 70;
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private bool ExtractProduct(Worksheet sheet)
        {
            SearchOptions options = new SearchOptions();
            options.MatchEntireCellContents = true;
            options.SearchIn = SearchIn.Values;

            // Find Cells in spreadsheet by 'Item Number' 
            IEnumerable<Cell> productCells = sheet.Search("Item Number", options);
            if (productCells == null) return false;

            // Find Cells in spreadsheet by 'PartName' 
            IEnumerable<Cell> partCells = sheet.Search("PartName", options);
            if (partCells == null) return false;

            int i = 0;
            foreach (Cell c1 in productCells)
            {
                ProductColumns productRec = new ProductColumns();
                // Read Item Number
                productRec.Code = sheet.Cells[c1.RowIndex, c1.ColumnIndex + 1].Value.ToString().Trim();
                // Read Product Description
                productRec.Name = sheet.Cells[c1.RowIndex + 1, c1.ColumnIndex + 1].Value.ToString().Trim();
                // Read Product Picture
                productRec.Image = new Bitmap(sheet.Pictures[i].Image.NativeImage,
                    new Size(300, (int)(sheet.Pictures[i].Image.NativeImage.Height * (300.0 / sheet.Pictures[i].Image.NativeImage.Width))));

                int j = 1;
                Cell c2 = partCells.ElementAt<Cell>(i);

                while (j > 0)
                {
                    PartColumns partRec = new PartColumns();
                    // Read PartName
                    partRec.Name = sheet.Cells[c2.RowIndex + j, c2.ColumnIndex].Value.ToString().Trim();
                    // Read Materials
                    partRec.MaterialNo = sheet.Cells[c2.RowIndex + j, c2.ColumnIndex + 1].Value.ToString().Trim();
                    // Read Materiral Name
                    partRec.MaterialName = sheet.Cells[c2.RowIndex + j, c2.ColumnIndex + 2].Value.ToString().Trim();

                    if (string.IsNullOrWhiteSpace(partRec.Name) == false)
                    {
                        productRec.Add(partRec);
                        j++;
                    }
                    else
                    {
                        j = 0;
                    }
                }

                bomRec.Add(productRec);
                i++;
            }

            return true;
        }

        private void InsertBom()
        {
            bool valid = true;
            SqlTransaction trans = AppRes.DB.BeginTrans();

            try
            {
                bomSet.RegTime = bomRec.RegTime;
                bomSet.AreaNo = bomRec.AreaNo;
                bomSet.FName = bomRec.FName;
                bomSet.FPath = bomRec.FPath;
                bomSet.Insert(trans);

                AppRes.TotalLog["NOTE"] = $"Inserted BOM in DB - {bomRec.AreaNo.ToDescription()}, {bomRec.FName}";

                foreach (ProductColumns productRec in bomRec.Products)
                {
                    InsertProduct(productRec, trans);
                    
                    valid = true;
                    foreach (PartColumns partRec in productRec.Parts)
                    {
                        if (InsertPart(partRec, trans) == false)
                        {
                            valid = false;
                        }
                    }

                    if (valid == true)
                    {
                        InsertIntegration(productRec, trans);
                    }
                }

                productSet.UpdateValidSet(trans);
                AppRes.TotalLog["NOTE"] = $"Set report validation flag to true";

                AppRes.DB.CommitTrans();
            }
            catch (Exception e)
            {
                AppRes.DbLog["ERROR"] = e.ToString();
                AppRes.DB.RollbackTrans();
            }
        }

        private void InsertIntegration(ProductColumns col, SqlTransaction trans)
        {
            integQuery.ProfJobSet.Type = EReportType.Integration;
            integQuery.ProfJobSet.JobNo = "";
            integQuery.ProfJobSet.AreaNo = bomRec.AreaNo;
            integQuery.ProfJobSet.ItemNo = col.Code;
            integQuery.ProfJobSet.ExtendASTM = false;            
            //integQuery.ProfJobSet.Select(trans);
            //integQuery.ProfJobSet.Select_Aurora(trans);
            integQuery.ProfJobSet.Select_KRSCT01(trans);
            integQuery.ProfJobSet.Fetch();
            string jobNo = integQuery.ProfJobSet.JobNo;

            if (string.IsNullOrWhiteSpace(jobNo) == false)
            {
                integMainSet.RecNo = $"*{jobNo}";
                integMainSet.Select(trans);

                if (integMainSet.Empty == true)
                {
                    integQuery.ProductSet.Valid = false;
                    integQuery.ProductSet.AreaNo = bomRec.AreaNo;
                    integQuery.ProductSet.Code = col.Code;
                    integQuery.ProductSet.IntegJobNo = "";
                    integQuery.ProductSet.SelectDetail(trans);
                    integQuery.ProductSet.Fetch();

                    if (integQuery.ProductSet.Empty == false)
                    {
                        // Integ에 insert하는 곳
                        integQuery.Insert(trans);
                        AppRes.TotalLog["NOTE"] = $"Created integration report - {jobNo}, {bomSet.AreaNo.ToDescription()}, {col.Code}, {col.Name}";
                    }
                }
                else
                {
                    AppRes.TotalLog["ERROR"] = $"Creating integration report is failed because that already exists in DB - {jobNo}, {bomSet.AreaNo.ToDescription()}, {col.Code}, {col.Name}";
                }
            }
            else
            {
                AppRes.TotalLog["ERROR"] = $"There is no Integration JobNo in DB - {bomSet.AreaNo.ToDescription()}, {col.Code}, {col.Name}";
            }

            productSet.AreaNo = bomRec.AreaNo;
            productSet.Code = col.Code;
            productSet.IntegJobNo = jobNo;
            productSet.UpdateIntegJobNoSet(trans);

            AppRes.TotalLog["NOTE"] = $"Updated Integration JobNo of Product - {jobNo}, {bomSet.AreaNo.ToDescription()}, {col.Code}, {col.Name}";
        }

        private void InsertProduct(ProductColumns col, SqlTransaction trans)
        {
            phyQuery.ProfJobSet.Type = EReportType.Physical;
            phyQuery.ProfJobSet.JobNo = "";
            phyQuery.ProfJobSet.AreaNo = bomSet.AreaNo;
            phyQuery.ProfJobSet.ItemNo = col.Code;
            phyQuery.ProfJobSet.ExtendASTM = false;
            //phyQuery.ProfJobSet.Select(trans);
            //phyQuery.ProfJobSet.Select_Aurora(trans);
            phyQuery.ProfJobSet.Select_KRSCT01(trans);
            phyQuery.ProfJobSet.Fetch();
            string jobNo = phyQuery.ProfJobSet.JobNo;

            if (string.IsNullOrWhiteSpace(jobNo) == false)
            {
                phyMainSet.RecNo = $"*{jobNo}";
                phyMainSet.Select(trans);

                if (phyMainSet.Empty == true)
                {
                    phyQuery.Insert(trans);
                    AppRes.TotalLog["NOTE"] = $"Created physical report - {jobNo}, {bomSet.AreaNo.ToDescription()}, {col.Code}, {col.Name}";
                }
                else
                {
                    AppRes.TotalLog["ERROR"] = $"Creating physical report is failed because that already exists in DB - {jobNo}, {bomSet.AreaNo.ToDescription()}, {col.Code}, {col.Name}";
                }
            }
            else
            {
                AppRes.TotalLog["ERROR"] = $"There is no Physical JobNo in DB - {bomSet.AreaNo.ToDescription()}, {col.Code}, {col.Name}";
            }

            productSet.BomNo = bomSet.RecNo;
            productSet.Valid = false;
            productSet.AreaNo = bomSet.AreaNo;
            productSet.Code = col.Code;
            productSet.PhyJobNo = jobNo;
            productSet.IntegJobNo = "";
            productSet.Name = col.Name;
            productSet.Image = col.Image;
            productSet.Insert(trans);

            AppRes.TotalLog["NOTE"] = $"Inserted Product in DB - {jobNo}, {bomSet.AreaNo.ToDescription()}, {col.Code}, {col.Name}";
        }

        private bool InsertPart(PartColumns col, SqlTransaction trans)
        {
            string jobNo = "";
            string sProjJobNo = "";
            string extendJobNo = "";

            try
            {
                cheQuery.ProfJobSet.Type = EReportType.Chemical;
                cheQuery.ProfJobSet.JobNo = "";
                cheQuery.ProfJobSet.AreaNo = bomSet.AreaNo;
                cheQuery.ProfJobSet.ItemNo = col.MaterialNo;
                cheQuery.ProfJobSet.ExtendASTM = true;

                //cheQuery.ProfJobSet.Select_Aurora(trans);
                cheQuery.ProfJobSet.Select_KRSCT01(trans);

                int rowCount = cheQuery.ProfJobSet.RowCount;
                if (rowCount > 0)
                {
                    cheQuery.ProfJobSet.Fetch(0);
                    jobNo = cheQuery.ProfJobSet.JobNo;
                    sProjJobNo = cheQuery.ProfJobSet.FileNo;

                    if (string.IsNullOrWhiteSpace(sProjJobNo) == false)
                    {
                        cheMainSet.RecNo = jobNo;
                        cheMainSet.ReportApproval = EReportApproval.None;
                        cheMainSet.AreaNo = EReportArea.None;
                        cheMainSet.From = "";
                        cheMainSet.To = "";
                        cheMainSet.MaterialNo = "";
                        cheMainSet.Select(trans);

                        if (cheMainSet.Empty == true)
                        {
                            extendJobNo = sProjJobNo;

                            // Final, Formatted가 있는 Insert
                            if (string.IsNullOrWhiteSpace(extendJobNo) == false)
                            {
                                //cheQuery.Insert(bomSet.AreaNo, extendJobNo, trans);
                                cheQuery.Insert_Chemical_Import(bomSet.AreaNo, jobNo, extendJobNo, trans);
                                //cheQuery.Insert(extendJobNo, trans);
                                AppRes.TotalLog["NOTE"] = $"Created chemical report - {jobNo}, {bomSet.AreaNo.ToDescription()}, {col.MaterialNo}, {col.Name}";
                            }
                        }
                        else
                        {
                            AppRes.TotalLog["ERROR"] = $"Creating chemical report is failed because that already exists in DB - {jobNo}, {bomSet.AreaNo.ToDescription()}, {col.MaterialNo}, {col.Name}";
                        }
                    }
                }
                else
                {
                    AppRes.TotalLog["ERROR"] = $"There is no Chemical JobNo in DB - {bomSet.AreaNo.ToDescription()}, {col.MaterialNo}, {col.Name}";
                }

                partSet.ProductNo = productSet.RecNo;
                partSet.JobNo = jobNo;
                partSet.MaterialNo = col.MaterialNo;
                partSet.Name = col.Name;
                partSet.MaterialName = col.MaterialName;
                partSet.Insert(trans);

                AppRes.TotalLog["NOTE"] = $"Inserted Part in DB - {jobNo}, {bomSet.AreaNo.ToDescription()}, {col.MaterialNo}, {col.Name}";                
            }
            catch (Exception e) 
            {
                MessageBox.Show("Message : " + e.Message.ToString() + Environment.NewLine + "Source : " + e.Source.ToString());
                AppRes.DB.RollbackTrans();
            }

            return (string.IsNullOrWhiteSpace(jobNo) == false) ? true : false;
        }

        private void DoLoggerMessage(string message)
        {
            if (this.InvokeRequired == true)
            {
                LoggerMessageHandler func = new LoggerMessageHandler(DoLoggerMessage);
                this.BeginInvoke(func, new object[] { message });
            }
            else
            {
                messageLogEdit.Text += message + "\r\n";
            }
        }

        private void btnChkBom_Click(object sender, EventArgs e)
        {
            MDIParent1 fCheckBom = new MDIParent1();
            fCheckBom.ShowDialog();
        }
    }
}
