using System;
using System.Collections.Generic;
using System.Text;

namespace Sgs.ReportIntegration.Source.A.Forms.B.Dialog
{
    class SLIM
    {
        public string Pro_Job;
        public string Pro_Proj, OrderNo;

        public SLIM(string pro_job, string pro_proj, string orderno)
        {
            Pro_Job = pro_job;
            Pro_Proj = pro_proj;
            OrderNo = orderno;
        }

        // Help the ComboBox display the student's name.
        public override string ToString()
        {
            return Pro_Job + ", " + Pro_Proj;
        }
    }
}
