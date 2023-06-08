using System;
using System.Data;
using System.Windows.Forms;

using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraReports.UI;

namespace Sgs.ReportIntegration
{
    public partial class ReportUsIntegration : XtraReport
    {
        private StaffDataSet staffSet;

        public ReportUsIntegration()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            staffSet = new StaffDataSet(AppRes.DB.Connect, null, null);
        }

        private void Detail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
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

        private void SurfaceDetailReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            DataSet set = (DataSource as BindingSource).DataSource as DataSet;

            if (set.Tables["SFRT_US"].Rows.Count == 0)
            {
                SurfaceDetailReport.Visible = false;
            }
            else if (set.Tables["SFLEADRT_US"].Rows.Count == 0)
            {
                SurfaceLeadGroupHeader.Visible = false;
                SurfaceLeadDetailReport.Visible = false;
            }
        }

        private void SubstrateDetailReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            DataSet set = (DataSource as BindingSource).DataSource as DataSet;

            if (set.Tables["SBRT_US"].Rows.Count == 0)
            {
                SurfaceDetailReport.Visible = false;
            }
            else if (set.Tables["SBLEADRT_US"].Rows.Count == 0)
            {
                SubstrateLeadGroupHeader.Visible = false;
                SubstrateLeadDetailReport.Visible = false;
            }
        }
    }
}
