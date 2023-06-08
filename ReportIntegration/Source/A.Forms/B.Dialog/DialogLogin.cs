using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using DevExpress.XtraEditors;

using Ulee.Controls;

namespace Sgs.ReportIntegration
{
    public partial class DialogLogin : UlFormEng
    {
        public string UserId { get { return idEdit.Text.Trim(); } }

        public string Passwd { get { return passwdEdit.Text; } }

        public DialogLogin()
        {
            InitializeComponent();
        }

        private void DialogLogin_Load(object sender, EventArgs e)
        {
        }

        private void DialogLogin_Enter(object sender, EventArgs e)
        {
        }

        private void DialogLogin_Shown(object sender, EventArgs e)
        {
            this.ImeMode = ImeMode.Disable;
            idEdit.ImeMode = ImeMode.Disable;
            passwdEdit.ImeMode = ImeMode.Disable;
        }
    }
}
