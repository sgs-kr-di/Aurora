
namespace Sgs.ReportIntegration.Source.A.Forms.B.Dialog
{
    partial class DialogImportWordToDT_EU
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.btnToDT_EU = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(11, 12);
            this.textBox1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(412, 20);
            this.textBox1.TabIndex = 8;
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(427, 12);
            this.btnOpenFile.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(98, 51);
            this.btnOpenFile.TabIndex = 7;
            this.btnOpenFile.Text = "파일 선택";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // btnToDT_EU
            // 
            this.btnToDT_EU.Location = new System.Drawing.Point(427, 69);
            this.btnToDT_EU.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnToDT_EU.Name = "btnToDT_EU";
            this.btnToDT_EU.Size = new System.Drawing.Size(98, 53);
            this.btnToDT_EU.TabIndex = 6;
            this.btnToDT_EU.Text = "toDT";
            this.btnToDT_EU.UseVisualStyleBackColor = true;
            this.btnToDT_EU.Click += new System.EventHandler(this.btnToDT_EU_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // DialogImportWordToDT_EU
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(529, 146);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.btnToDT_EU);
            this.Name = "DialogImportWordToDT_EU";
            this.Text = "frmImportWordToDT_EU";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Button btnToDT_EU;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}