﻿using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraReports.UI;
using System;
using System.Data;
using System.Drawing.Printing;
using System.Windows.Forms;
/*
namespace Sgs.ReportIntegration
{
    public partial class ReportEuChemical : XtraReport
    {
        private StaffDataSet staffSet;

        public ReportEuChemical()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            staffSet = new StaffDataSet(AppRes.DB.Connect, null, null);
        }

        private void Detail_BeforePrint(object sender, PrintEventArgs e)
        {
            DataSet set = (DataSource as BindingSource).DataSource as DataSet;
            string staffNo = Convert.ToString(set.Tables["P1"].Rows[0]["staffno"]);

            if (string.IsNullOrWhiteSpace(staffNo) == false)
            {
                staffSet.StaffNo = staffNo;
                staffSet.Select();
                staffSet.Fetch();

                if (staffSet.Signature != null)
                {
                    signImageBox.ImageSource = new ImageSource(staffSet.Signature);
                    signNameLabel.Text = staffSet.FirstName + " " + staffSet.LastName;
                }
            }
        }
    }
}
*/