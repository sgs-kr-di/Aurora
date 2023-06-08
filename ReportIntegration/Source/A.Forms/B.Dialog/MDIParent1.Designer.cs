namespace Sgs.ReportIntegration.Source.A.Forms.B.Dialog
{
    partial class MDIParent1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.aurora1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aurora2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eNDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eNDBToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.aSTMDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aurora1ToolStripMenuItem,
            this.aurora2ToolStripMenuItem,
            this.eNDBToolStripMenuItem,
            this.eNDBToolStripMenuItem2,
            this.aSTMDBToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(632, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "MenuStrip";
            // 
            // aurora1ToolStripMenuItem
            // 
            this.aurora1ToolStripMenuItem.Enabled = false;
            this.aurora1ToolStripMenuItem.Name = "aurora1ToolStripMenuItem";
            this.aurora1ToolStripMenuItem.Size = new System.Drawing.Size(34, 20);
            this.aurora1ToolStripMenuItem.Text = "EN";
            this.aurora1ToolStripMenuItem.Click += new System.EventHandler(this.aurora1ToolStripMenuItem_Click);
            // 
            // aurora2ToolStripMenuItem
            // 
            this.aurora2ToolStripMenuItem.Name = "aurora2ToolStripMenuItem";
            this.aurora2ToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.aurora2ToolStripMenuItem.Text = "ASTM";
            this.aurora2ToolStripMenuItem.Click += new System.EventHandler(this.aurora2ToolStripMenuItem_Click);
            // 
            // eNDBToolStripMenuItem
            // 
            this.eNDBToolStripMenuItem.Enabled = false;
            this.eNDBToolStripMenuItem.Name = "eNDBToolStripMenuItem";
            this.eNDBToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.eNDBToolStripMenuItem.Text = "EN_DB";
            this.eNDBToolStripMenuItem.Click += new System.EventHandler(this.eNDBToolStripMenuItem_Click);
            // 
            // eNDBToolStripMenuItem2
            // 
            this.eNDBToolStripMenuItem2.Name = "eNDBToolStripMenuItem2";
            this.eNDBToolStripMenuItem2.Size = new System.Drawing.Size(60, 20);
            this.eNDBToolStripMenuItem2.Text = "EN_DB2";
            this.eNDBToolStripMenuItem2.Click += new System.EventHandler(this.eNDBToolStripMenuItem2_Click);
            // 
            // aSTMDBToolStripMenuItem
            // 
            this.aSTMDBToolStripMenuItem.Enabled = false;
            this.aSTMDBToolStripMenuItem.Name = "aSTMDBToolStripMenuItem";
            this.aSTMDBToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
            this.aSTMDBToolStripMenuItem.Text = "ASTM_DB";
            this.aSTMDBToolStripMenuItem.Click += new System.EventHandler(this.aSTMDBToolStripMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 431);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(632, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "StatusStrip";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(31, 17);
            this.toolStripStatusLabel.Text = "상태";
            // 
            // MDIParent1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 453);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MDIParent1";
            this.Text = "DI_023 Aurora_20210929(ver35)";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolTip ToolTip;
        private System.Windows.Forms.ToolStripMenuItem aurora1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aurora2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eNDBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aSTMDBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eNDBToolStripMenuItem2;
    }
}



