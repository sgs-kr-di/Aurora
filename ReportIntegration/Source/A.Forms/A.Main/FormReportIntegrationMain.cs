﻿using LoadingIndicator.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ulee.Controls;
using Ulee.Utils;

namespace Sgs.ReportIntegration
{
    public partial class FormReportIntegrationMain : UlFormEng
    {
        private bool First;

        private const int csInvalidTime = 250;
        
        private const string csDateFormat = "yyyy-MM-dd";
        
        private const string csTimeFormat = "HH:mm:ss";

        private StaffDataSet staffSet;

        private InvalidThread invalidThread;

        private LongOperation _longOperation;

        public FormReportIntegrationMain()
        {
            InitializeComponent();
            Initialize();            
        }
        
        private void Initialize()
        {
            First = true;
            invalidThread = null;

            staffSet = new StaffDataSet(AppRes.DB.Connect, null, null);

            DefMenu = new UlMenu(viewPanel);
            DefMenu.Add(new CtrlEditRight(), editButton);
            DefMenu.Add(new CtrlSysLogRight(), logButton);
            //DefMenu.Add(new CtrlSettingsRight(), settingsButton);
            DefMenu.Index = 0;

            AppRes.TotalLog[ELogTag.Note] = "Create application mainform";
            AppRes.DbLog[ELogTag.Note] = $"MS-SQL Server ConnectionString - '{AppRes.DB.ConnectString}'";

            // Initialize long operation with control which will
            // be overlayed during long operations
            _longOperation = new LongOperation(this);

            // You can pass settings to customize indicator view/behavior
            // _longOperation = new LongOperation(this, LongOperationSettings.Default);
        }

        private void FormReportIntegrationMain_Load(object sender, EventArgs e)
        {
            using (_longOperation.Start())
            {                
                if (IsLogin() == false)
                {
                    Close();
                    return;
                }

                First = false;
                DispCaption();

                AppRes.TotalLog[ELogTag.Note] = "Resume screen invalidation thread";
                invalidThread = new InvalidThread(csInvalidTime);
                invalidThread.InvalidControls += InvalidForm;
                invalidThread.Resume();
            }
        }

        private void FormReportIntegrationMain_Leave(object sender, EventArgs e)
        {
        }

        private void FormReportIntegrationMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (First == true) return;

            if (MessageBox.Show("Would you like to exit this program?",
                "SGS", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void FormReportIntegrationMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (invalidThread?.IsAlive == true)
            {
                invalidThread.Terminate();
                AppRes.TotalLog[ELogTag.Note] = "Terminate screen invalidation thread";
            }

            AppRes.TotalLog[ELogTag.Note] = "Destroy application mainform";
        }

        private void FormReportIntegrationMain_Resize(object sender, EventArgs e)
        {
            if (Width < 1024) Width = 1024;
            if (Height < 640) Height = 640;

            menuPanel.Size = new Size(84, Height - 116);
            viewPanel.Size = new Size(Width - 116, Height - 72);

            loginButton.Top = Height - 236;
            exitButton.Top = Height - 176;
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            IsLogin();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool IsLogin()
        {
            for (int i = 0; i < 3; i++)
            {
                DialogLogin dialog = new DialogLogin();
                
                dialog.ShowDialog();

                if (dialog.DialogResult == DialogResult.OK)
                {
                    if (string.IsNullOrWhiteSpace(dialog.Passwd) == false)
                    {
                        try
                        {
                            staffSet.StaffNo = dialog.UserId;
                            staffSet.Select();
                            staffSet.Fetch();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }

                        string passwd = staffSet.Passwd;
                        //string passwd = Encoding.ASCII.GetString(Convert.FromBase64String(staffSet.Passwd));

                        if (dialog.Passwd == passwd)
                        {
                            AppRes.UserId = dialog.UserId;
                            AppRes.Authority = staffSet.Authority;
                            SetAuthority();
                 
                            return true;
                        }
                    }

                    MessageBox.Show("Invalid password!\r\nPlease keyin password again!",
                        AppRes.Caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    break;
                }
            }

            return false;
        }
        
        public override void InvalidForm(object sender, EventArgs e)
        {
            if (this.InvokeRequired == true)
            {
                EventHandler func = new EventHandler(InvalidForm);
                this.BeginInvoke(func, new object[] { sender, e });
            }
            else
            {
                InvalidDateTime();
                InvalidUserControls(DefMenu);
            }
        }
        
        private void InvalidDateTime()
        {
            string dateTime = DateTime.Now.ToString(csDateFormat + " " + csTimeFormat);

            if (dateTimeStatusLabel.Text != dateTime)
            {
                dateTimeStatusLabel.Text = dateTime;
            }
        }

        private void InvalidUserControls(UlMenu menu)
        {
            if (menu == null) return;

            UlUserControlEng ctrl = menu.ActiveControl as UlUserControlEng;

            ctrl.InvalidControl(this, null);
            InvalidUserControls(ctrl.DefMenu);
        }

        private void DispCaption()
        {
            Text = "Test Report Integration Program Ver " + GetVersion();
        }

        private string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void SetAuthority()
        {
            userStatusLabel.Text = AppRes.UserId;
            authorityStatusLabel.Text = AppRes.Authority.ToDescription();

            (DefMenu.Controls(0) as CtrlEditRight).SetMenu();
        }

        private void menuPanel_Paint(object sender, PaintEventArgs e)
        {

        }

    }
}
