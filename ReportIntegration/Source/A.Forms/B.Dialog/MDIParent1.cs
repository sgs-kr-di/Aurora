using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sgs.ReportIntegration.Source.A.Forms.B.Dialog
{
    public partial class MDIParent1 : Form
    {
        private int childFormNumber = 0;

        public MDIParent1()
        {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            // 자식 폼의 새 인스턴스를 만듭니다.
            Form childForm = new Form();
            // 이 인스턴스를 표시하기 전에 MDI 폼의 자식으로 만듭니다. 
            childForm.MdiParent = this;
            childForm.Text = "창" + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
                // TODO: 파일을 여는 코드를 추가합니다.
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
                // TODO: 폼의 현재 내용을 파일에 저장하는 코드를 여기에 추가합니다.
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: System.Windows.Forms.Clipboard를 사용하여 선택한 텍스트나 이미지를 클립보드에 삽입합니다.
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: System.Windows.Forms.Clipboard를 사용하여 선택한 텍스트나 이미지를 클립보드에 삽입합니다.
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: 클립보드에서 정보를 검색하려면 System.Windows.Forms.Clipboard.GetText() 또는 System.Windows.Forms.GetData를 사용합니다.
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void aurora1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            Form Form11 = new Form1();
            Form11.Show();
            Form11.WindowState = FormWindowState.Maximized;
            */
        }

        private void aurora2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form Form22 = new Form2();
            Form22.ShowDialog();
            Form22.WindowState = FormWindowState.Maximized;
        }

        private void eNDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Form Form33 = new Form3();
            //Form33.Show();
            //Form33.WindowState = FormWindowState.Maximized;
        }

        private void aSTMDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            Form Form44 = new Form4();
            Form44.Show();
            Form44.WindowState = FormWindowState.Maximized;
            */
        }

        private void eNDBToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Form Form55 = new Form5();
            Form55.ShowDialog();
            Form55.WindowState = FormWindowState.Maximized;
        }
    }
}
