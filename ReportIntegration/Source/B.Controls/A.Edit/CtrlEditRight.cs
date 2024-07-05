﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using DevExpress.XtraEditors;

using Ulee.Controls;

namespace Sgs.ReportIntegration
{                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
    public partial class CtrlEditRight : UlUserControlEng
    {
        public CtrlEditRight()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            viewPanel.Controls.Clear();

            menuPanel.Controls.Add(bomMenuPanel);
            menuPanel.Controls.Add(physicalMenuPanel);
            menuPanel.Controls.Add(chemicalMenuPanel);
            menuPanel.Controls.Add(integMenuPanel);
            menuPanel.Controls.Add(receiptMenuPanel);

            bomMenuPanel.Left = 2;
            bomMenuPanel.Top = 326;

            physicalMenuPanel.Left = 2;
            physicalMenuPanel.Top = 268;

            chemicalMenuPanel.Left = 2;
            chemicalMenuPanel.Top = 268;

            integMenuPanel.Left = 2;
            integMenuPanel.Top = 268;

            DefMenu = new UlMenu(viewPanel);
            DefMenu.Add(new CtrlEditBom(this), bomButton);
            DefMenu.Add(new CtrlEditPhysical(this), physicalButton);
            DefMenu.Add(new CtrlEditChemical(this), chemicalButton);
            DefMenu.Add(new CtrlEditIntegration(this), integrationButton);
            DefMenu.Add(new CtrlEditReceipt(this), receiptButton);
            DefMenu.Index = 0;
        }

        private void CtrlEditRight_Resize(object sender, EventArgs e)
        {
            viewPanel.Size = new Size(Width - 88, Height);

            menuPanel.Size = new Size(84, Height);
            menuPanel.Left = Width - 84;

            bomMenuPanel.Top = menuPanel.Size.Height - 242;
            physicalMenuPanel.Top = menuPanel.Size.Height - 300;
            chemicalMenuPanel.Top = menuPanel.Size.Height - 300;
            integMenuPanel.Top = menuPanel.Size.Height - 300;
            receiptMenuPanel.Top = menuPanel.Size.Height - 300;
        }

        private void bomImportButton_Click(object sender, EventArgs e)
        {
            string fName = OpenBomFile();
            if (string.IsNullOrWhiteSpace(fName) == false)
            {
                EReportArea area = EReportArea.None;
                string dirName = Path.GetDirectoryName(fName);

                if (dirName.EndsWith("AURORA ASTM") == true) area = EReportArea.US;
                else if (dirName.EndsWith("AURORA EN") == true) area = EReportArea.EU;

                if (area != EReportArea.None)
                {
                    (DefMenu.Controls(0) as CtrlEditBom).Import(area, fName);
                }
            }
        }

        private void bomDeleteButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(0) as CtrlEditBom).Delete();
        }

        private void physicalImportButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(1) as CtrlEditPhysical).Import();
        }

        private void physicalDeleteButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(1) as CtrlEditPhysical).Delete();
        }

        private void physicalPrintButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(1) as CtrlEditPhysical).Print();
        }

        private void physicalSaveButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(1) as CtrlEditPhysical).Save();
        }

        private void physicalCancelButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(1) as CtrlEditPhysical).Cancel();
        }

        private void chemicalImportButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(2) as CtrlEditChemical).Import();
        }

        private void chemicalDeleteButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(2) as CtrlEditChemical).Delete();
        }

        private void chemicalPrintButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(2) as CtrlEditChemical).Print();
        }

        private void chemicalSaveButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(2) as CtrlEditChemical).Save();
        }

        private void chemicalCancelButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(2) as CtrlEditChemical).Cancel();
        }

        private void integImportButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(3) as CtrlEditIntegration).Import();
        }

        private void integDeleteButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(3) as CtrlEditIntegration).Delete();
        }

        private void integPrintButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(3) as CtrlEditIntegration).Print();
        }

        private void integSaveButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(3) as CtrlEditIntegration).Save();
        }

        private void integCancelButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(3) as CtrlEditIntegration).Cancel();
        }

        public void SetMenu()
        {
            SetMenu(DefMenu.Index);
        }

        public void SetMenu(int index)
        {
            bomMenuPanel.Visible = false;
            physicalMenuPanel.Visible = false;
            chemicalMenuPanel.Visible = false;
            integMenuPanel.Visible = false;

            switch (index)
            {
                // BOM
                case 0:
                    if (AppRes.Authority == EReportAuthority.Admin)
                    {
                        bomMenuPanel.Visible = true;
                    }
                    break;

                // Physical
                case 1:
                    if (AppRes.Authority == EReportAuthority.Admin)
                    {
                        physicalMenuPanel.Visible = true;
                    }
                    break;

                // Chemical
                case 2:
                    if (AppRes.Authority == EReportAuthority.Admin)
                    {
                        chemicalMenuPanel.Visible = true;
                    }
                    break;

                // Integration
                case 3:
                    if (AppRes.Authority == EReportAuthority.Admin)
                    {
                        integMenuPanel.Visible = true;
                    }
                    break;
            }
        }

        private string OpenBomFile()
        {
            string fName = string.Empty;
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.InitialDirectory = AppRes.Settings.BomPath;
            dialog.DefaultExt = "xls";
            dialog.Filter = "Excel files (*.xls)|*.xls";
            dialog.Multiselect = false;
            dialog.ShowReadOnly = false;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                fName = dialog.FileName;
            }

            return fName;
        }

        private void receiptImportButton_Click(object sender, EventArgs e)
        {
            string fName = OpenBomFile();
            if (string.IsNullOrWhiteSpace(fName) == false)
            {
                EReportArea area = EReportArea.None;
                string dirName = Path.GetDirectoryName(fName);

                if (dirName.EndsWith("AURORA ASTM") == true) area = EReportArea.US;
                else if (dirName.EndsWith("AURORA EN") == true) area = EReportArea.EU;

                if (area != EReportArea.None)
                {
                    (DefMenu.Controls(4) as CtrlEditReceipt).Import(area, fName);
                }
            }
        }

        private void receiptDeleteButton_Click(object sender, EventArgs e)
        {
            (DefMenu.Controls(4) as CtrlEditBom).Delete();
        }
    }
}