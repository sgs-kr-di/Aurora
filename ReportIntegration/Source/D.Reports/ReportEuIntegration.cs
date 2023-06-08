using System;
using System.Data;
using System.Windows.Forms;

using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Drawing;

namespace Sgs.ReportIntegration
{
    public partial class ReportEuIntegration : XtraReport
    {
        private StaffDataSet staffSet;

        public ReportEuIntegration()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            staffSet = new StaffDataSet(AppRes.DB.Connect, null, null);
        }

        private void DetailReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
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
