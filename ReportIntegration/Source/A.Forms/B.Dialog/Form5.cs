using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Xls;
using System.Linq;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using System.IO;
using System.Globalization;

using System.Data.OleDb;

namespace Sgs.ReportIntegration.Source.A.Forms.B.Dialog
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }

        private OleDbConnection Conn;
        private OleDbConnection Conn_bak;

        private string OpenFile()
        {
            openFileDialog1.InitialDirectory = "K:\\SL&HL\\HL\\★ REPORT_FINAL\\BOM LIST\\AURORA EN";
            openFileDialog1.Filter = "Excel Document (*.xls*)|*.xls*";
            openFileDialog1.Title = "Choose a document";

            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }

            return string.Empty;
        }

        private void Initialize_Grid()
        {
            dgvFinalMerge.RowCount = 43;
            dgvFinalMerge.ColumnCount = 100;

            dgvFinalMerge.Rows[1].Cells[0].Value = "Job No";
            dgvFinalMerge.Rows[2].Cells[0].Value = "Item No";
            dgvFinalMerge.Rows[3].Cells[0].Value = "Issue Date";
            dgvFinalMerge.Rows[4].Cells[0].Value = "Aurora Description";
            dgvFinalMerge.Rows[5].Cells[0].Value = "Screen Or Not";

            dgvFinalMerge.Rows[6].Cells[0].Value = "PartName";
            dgvFinalMerge.Rows[7].Cells[0].Value = "Materials";

            dgvFinalMerge.Rows[8].Cells[0].Value = "Sample No";
            dgvFinalMerge.Rows[9].Cells[0].Value = "Report Sample Description";
            dgvFinalMerge.Rows[10].Cells[0].Value = "Part3 Results";
            dgvFinalMerge.Rows[33].Cells[0].Value = "Organotin Results";

            dgvFinalMerge.Rows[10].Cells[1].Value = "Mass of trace amount";
            dgvFinalMerge.Rows[11].Cells[1].Value = "Soluble Aluminium";
            dgvFinalMerge.Rows[12].Cells[1].Value = "Soluble Antimony";
            dgvFinalMerge.Rows[13].Cells[1].Value = "Soluble Arsenic";
            dgvFinalMerge.Rows[14].Cells[1].Value = "Soluble Barium";
            dgvFinalMerge.Rows[15].Cells[1].Value = "Soluble Boron";
            dgvFinalMerge.Rows[16].Cells[1].Value = "Soluble Cadmium";
            dgvFinalMerge.Rows[17].Cells[1].Value = "Soluble Chromium";
            dgvFinalMerge.Rows[18].Cells[1].Value = "Soluble Chromium (III) #";
            dgvFinalMerge.Rows[19].Cells[1].Value = "Soluble Chromium (VI) #";
            dgvFinalMerge.Rows[20].Cells[1].Value = "Soluble Cobalt";
            dgvFinalMerge.Rows[21].Cells[1].Value = "Soluble Copper";
            dgvFinalMerge.Rows[22].Cells[1].Value = "Soluble Lead";
            dgvFinalMerge.Rows[23].Cells[1].Value = "Soluble Manganese";
            dgvFinalMerge.Rows[24].Cells[1].Value = "Soluble Mercury";
            dgvFinalMerge.Rows[25].Cells[1].Value = "Soluble Nickel";
            dgvFinalMerge.Rows[26].Cells[1].Value = "Soluble Selenium";
            dgvFinalMerge.Rows[27].Cells[1].Value = "Soluble Strontium";
            dgvFinalMerge.Rows[28].Cells[1].Value = "Soluble Tin";
            dgvFinalMerge.Rows[29].Cells[1].Value = "Soluble Organic Tin^";
            dgvFinalMerge.Rows[30].Cells[1].Value = "Soluble Zinc";

            dgvFinalMerge.Rows[33].Cells[1].Value = "Methyl tin";
            dgvFinalMerge.Rows[34].Cells[1].Value = "Di-n-propyl tin";
            dgvFinalMerge.Rows[35].Cells[1].Value = "Butyl tin";
            dgvFinalMerge.Rows[36].Cells[1].Value = "Dibutyl tin";
            dgvFinalMerge.Rows[37].Cells[1].Value = "Tributyl tin";
            dgvFinalMerge.Rows[38].Cells[1].Value = "n-Octyl tin";
            dgvFinalMerge.Rows[39].Cells[1].Value = "Tetrabutyl tin";
            dgvFinalMerge.Rows[40].Cells[1].Value = "Diphenyl tin";
            dgvFinalMerge.Rows[41].Cells[1].Value = "Di-n-octyl tin";
            dgvFinalMerge.Rows[42].Cells[1].Value = "Triphenyl tin";

            dgvFinalMerge.Rows[10].Cells[2].Value = "(mg)";
            dgvFinalMerge.Rows[11].Cells[2].Value = "(Al)";
            dgvFinalMerge.Rows[12].Cells[2].Value = "(Sb)";
            dgvFinalMerge.Rows[13].Cells[2].Value = "(As)";
            dgvFinalMerge.Rows[14].Cells[2].Value = "(Ba)";
            dgvFinalMerge.Rows[15].Cells[2].Value = "(B)";
            dgvFinalMerge.Rows[16].Cells[2].Value = "(Cd)";
            dgvFinalMerge.Rows[17].Cells[2].Value = "(Cr)";
            dgvFinalMerge.Rows[18].Cells[2].Value = "(Cr (III))";
            dgvFinalMerge.Rows[19].Cells[2].Value = "(Cr (VI))";
            dgvFinalMerge.Rows[20].Cells[2].Value = "(Co)";
            dgvFinalMerge.Rows[21].Cells[2].Value = "(Cu)";
            dgvFinalMerge.Rows[22].Cells[2].Value = "(Pb)";
            dgvFinalMerge.Rows[23].Cells[2].Value = "(Mn)";
            dgvFinalMerge.Rows[24].Cells[2].Value = "(Hg)";
            dgvFinalMerge.Rows[25].Cells[2].Value = "(Ni)";
            dgvFinalMerge.Rows[26].Cells[2].Value = "(Se)";
            dgvFinalMerge.Rows[27].Cells[2].Value = "(Sr)";
            dgvFinalMerge.Rows[28].Cells[2].Value = "(Sn)";
            dgvFinalMerge.Rows[29].Cells[2].Value = "--";
            dgvFinalMerge.Rows[30].Cells[2].Value = "(Zn)";

            dgvFinalMerge.Rows[33].Cells[2].Value = "(MeT)";
            dgvFinalMerge.Rows[34].Cells[2].Value = "(DProT)";
            dgvFinalMerge.Rows[35].Cells[2].Value = "(BuT)";
            dgvFinalMerge.Rows[36].Cells[2].Value = "(DBT)";
            dgvFinalMerge.Rows[37].Cells[2].Value = "(TBT)";
            dgvFinalMerge.Rows[38].Cells[2].Value = "(MOT)";
            dgvFinalMerge.Rows[39].Cells[2].Value = "(TeBT)";
            dgvFinalMerge.Rows[40].Cells[2].Value = "(DPhT)";
            dgvFinalMerge.Rows[41].Cells[2].Value = "(DOT)";
            dgvFinalMerge.Rows[42].Cells[2].Value = "(TPhT)";

            dgvFinal.RowCount = 44;
            dgvFinal.ColumnCount = 300;

            dgvFinal.Rows[1].Cells[0].Value = "Job No";
            dgvFinal.Rows[2].Cells[0].Value = "Item No";
            dgvFinal.Rows[3].Cells[0].Value = "Issue Date";
            dgvFinal.Rows[4].Cells[0].Value = "Aurora Description";
            dgvFinal.Rows[5].Cells[0].Value = "Screen Or Not";

            dgvFinal.Rows[6].Cells[0].Value = "PartName";
            dgvFinal.Rows[7].Cells[0].Value = "Materials";

            dgvFinal.Rows[8].Cells[0].Value = "Sample No";
            dgvFinal.Rows[9].Cells[0].Value = "Report Sample Description";
            dgvFinal.Rows[10].Cells[0].Value = "Part3 Results";
            dgvFinal.Rows[33].Cells[0].Value = "Organotin Results";

            dgvFinal.Rows[10].Cells[1].Value = "Mass of trace amount";
            dgvFinal.Rows[11].Cells[1].Value = "Soluble Aluminium";
            dgvFinal.Rows[12].Cells[1].Value = "Soluble Antimony";
            dgvFinal.Rows[13].Cells[1].Value = "Soluble Arsenic";
            dgvFinal.Rows[14].Cells[1].Value = "Soluble Barium";
            dgvFinal.Rows[15].Cells[1].Value = "Soluble Boron";
            dgvFinal.Rows[16].Cells[1].Value = "Soluble Cadmium";
            dgvFinal.Rows[17].Cells[1].Value = "Soluble Chromium";
            dgvFinal.Rows[18].Cells[1].Value = "Soluble Chromium (III) #";
            dgvFinal.Rows[19].Cells[1].Value = "Soluble Chromium (VI) #";
            dgvFinal.Rows[20].Cells[1].Value = "Soluble Cobalt";
            dgvFinal.Rows[21].Cells[1].Value = "Soluble Copper";
            dgvFinal.Rows[22].Cells[1].Value = "Soluble Lead";
            dgvFinal.Rows[23].Cells[1].Value = "Soluble Manganese";
            dgvFinal.Rows[24].Cells[1].Value = "Soluble Mercury";
            dgvFinal.Rows[25].Cells[1].Value = "Soluble Nickel";
            dgvFinal.Rows[26].Cells[1].Value = "Soluble Selenium";
            dgvFinal.Rows[27].Cells[1].Value = "Soluble Strontium";
            dgvFinal.Rows[28].Cells[1].Value = "Soluble Tin";
            dgvFinal.Rows[29].Cells[1].Value = "Soluble Organic Tin^";
            dgvFinal.Rows[30].Cells[1].Value = "Soluble Zinc";

            dgvFinal.Rows[33].Cells[1].Value = "Methyl tin";
            dgvFinal.Rows[34].Cells[1].Value = "Di-n-propyl tin";
            dgvFinal.Rows[35].Cells[1].Value = "Butyl tin";
            dgvFinal.Rows[36].Cells[1].Value = "Dibutyl tin";
            dgvFinal.Rows[37].Cells[1].Value = "Tributyl tin";
            dgvFinal.Rows[38].Cells[1].Value = "n-Octyl tin";
            dgvFinal.Rows[39].Cells[1].Value = "Tetrabutyl tin";
            dgvFinal.Rows[40].Cells[1].Value = "Diphenyl tin";
            dgvFinal.Rows[41].Cells[1].Value = "Di-n-octyl tin";
            dgvFinal.Rows[42].Cells[1].Value = "Triphenyl tin";

            dgvFinal.Rows[10].Cells[2].Value = "(mg)";
            dgvFinal.Rows[11].Cells[2].Value = "(Al)";
            dgvFinal.Rows[12].Cells[2].Value = "(Sb)";
            dgvFinal.Rows[13].Cells[2].Value = "(As)";
            dgvFinal.Rows[14].Cells[2].Value = "(Ba)";
            dgvFinal.Rows[15].Cells[2].Value = "(B)";
            dgvFinal.Rows[16].Cells[2].Value = "(Cd)";
            dgvFinal.Rows[17].Cells[2].Value = "(Cr)";
            dgvFinal.Rows[18].Cells[2].Value = "(Cr (III))";
            dgvFinal.Rows[19].Cells[2].Value = "(Cr (VI))";
            dgvFinal.Rows[20].Cells[2].Value = "(Co)";
            dgvFinal.Rows[21].Cells[2].Value = "(Cu)";
            dgvFinal.Rows[22].Cells[2].Value = "(Pb)";
            dgvFinal.Rows[23].Cells[2].Value = "(Mn)";
            dgvFinal.Rows[24].Cells[2].Value = "(Hg)";
            dgvFinal.Rows[25].Cells[2].Value = "(Ni)";
            dgvFinal.Rows[26].Cells[2].Value = "(Se)";
            dgvFinal.Rows[27].Cells[2].Value = "(Sr)";
            dgvFinal.Rows[28].Cells[2].Value = "(Sn)";
            dgvFinal.Rows[29].Cells[2].Value = "--";
            dgvFinal.Rows[30].Cells[2].Value = "(Zn)";

            dgvFinal.Rows[33].Cells[2].Value = "(MeT)";
            dgvFinal.Rows[34].Cells[2].Value = "(DProT)";
            dgvFinal.Rows[35].Cells[2].Value = "(BuT)";
            dgvFinal.Rows[36].Cells[2].Value = "(DBT)";
            dgvFinal.Rows[37].Cells[2].Value = "(TBT)";
            dgvFinal.Rows[38].Cells[2].Value = "(MOT)";
            dgvFinal.Rows[39].Cells[2].Value = "(TeBT)";
            dgvFinal.Rows[40].Cells[2].Value = "(DPhT)";
            dgvFinal.Rows[41].Cells[2].Value = "(DOT)";
            dgvFinal.Rows[42].Cells[2].Value = "(TPhT)";

            //dgvMergeFiles.Rows[5].Cells[4].Value = "Report No";
            //dgvMergeFiles.Rows[5].Cells[5].Value = "Issued Date";
            //dgvMergeFiles.Rows[5].Cells[6].Value = "Report Sample Description";
        }

        private void dgvFinalMerge_initialization()
        {
            dgvFinalMerge.RowCount = 43;
            dgvFinalMerge.ColumnCount = 100;

            dgvFinalMerge.Rows[0].Cells[0].Value = " ";
            dgvFinalMerge.Rows[1].Cells[0].Value = "Job No";
            dgvFinalMerge.Rows[2].Cells[0].Value = "Item No";
            dgvFinalMerge.Rows[3].Cells[0].Value = "Issue Date";
            dgvFinalMerge.Rows[4].Cells[0].Value = "Aurora Description";
            dgvFinalMerge.Rows[5].Cells[0].Value = "Screen Or Not";

            dgvFinalMerge.Rows[6].Cells[0].Value = "PartName";
            dgvFinalMerge.Rows[7].Cells[0].Value = "Materials";

            dgvFinalMerge.Rows[8].Cells[0].Value = "Sample No";
            dgvFinalMerge.Rows[9].Cells[0].Value = "Report Sample Description";
            dgvFinalMerge.Rows[10].Cells[0].Value = "Part3 Results";
            dgvFinalMerge.Rows[33].Cells[0].Value = "Organotin Results";

            dgvFinalMerge.Rows[10].Cells[1].Value = "Mass of trace amount";
            dgvFinalMerge.Rows[11].Cells[1].Value = "Soluble Aluminium";
            dgvFinalMerge.Rows[12].Cells[1].Value = "Soluble Antimony";
            dgvFinalMerge.Rows[13].Cells[1].Value = "Soluble Arsenic";
            dgvFinalMerge.Rows[14].Cells[1].Value = "Soluble Barium";
            dgvFinalMerge.Rows[15].Cells[1].Value = "Soluble Boron";
            dgvFinalMerge.Rows[16].Cells[1].Value = "Soluble Cadmium";
            dgvFinalMerge.Rows[17].Cells[1].Value = "Soluble Chromium";
            dgvFinalMerge.Rows[18].Cells[1].Value = "Soluble Chromium (III) #";
            dgvFinalMerge.Rows[19].Cells[1].Value = "Soluble Chromium (VI) #";
            dgvFinalMerge.Rows[20].Cells[1].Value = "Soluble Cobalt";
            dgvFinalMerge.Rows[21].Cells[1].Value = "Soluble Copper";
            dgvFinalMerge.Rows[22].Cells[1].Value = "Soluble Lead";
            dgvFinalMerge.Rows[23].Cells[1].Value = "Soluble Manganese";
            dgvFinalMerge.Rows[24].Cells[1].Value = "Soluble Mercury";
            dgvFinalMerge.Rows[25].Cells[1].Value = "Soluble Nickel";
            dgvFinalMerge.Rows[26].Cells[1].Value = "Soluble Selenium";
            dgvFinalMerge.Rows[27].Cells[1].Value = "Soluble Strontium";
            dgvFinalMerge.Rows[28].Cells[1].Value = "Soluble Tin";
            dgvFinalMerge.Rows[29].Cells[1].Value = "Soluble Organic Tin^";
            dgvFinalMerge.Rows[30].Cells[1].Value = "Soluble Zinc";

            dgvFinalMerge.Rows[33].Cells[1].Value = "Methyl tin";
            dgvFinalMerge.Rows[34].Cells[1].Value = "Di-n-propyl tin";
            dgvFinalMerge.Rows[35].Cells[1].Value = "Butyl tin";
            dgvFinalMerge.Rows[36].Cells[1].Value = "Dibutyl tin";
            dgvFinalMerge.Rows[37].Cells[1].Value = "Tributyl tin";
            dgvFinalMerge.Rows[38].Cells[1].Value = "n-Octyl tin";
            dgvFinalMerge.Rows[39].Cells[1].Value = "Tetrabutyl tin";
            dgvFinalMerge.Rows[40].Cells[1].Value = "Diphenyl tin";
            dgvFinalMerge.Rows[41].Cells[1].Value = "Di-n-octyl tin";
            dgvFinalMerge.Rows[42].Cells[1].Value = "Triphenyl tin";

            dgvFinalMerge.Rows[10].Cells[2].Value = "(mg)";
            dgvFinalMerge.Rows[11].Cells[2].Value = "(Al)";
            dgvFinalMerge.Rows[12].Cells[2].Value = "(Sb)";
            dgvFinalMerge.Rows[13].Cells[2].Value = "(As)";
            dgvFinalMerge.Rows[14].Cells[2].Value = "(Ba)";
            dgvFinalMerge.Rows[15].Cells[2].Value = "(B)";
            dgvFinalMerge.Rows[16].Cells[2].Value = "(Cd)";
            dgvFinalMerge.Rows[17].Cells[2].Value = "(Cr)";
            dgvFinalMerge.Rows[18].Cells[2].Value = "(Cr (III))";
            dgvFinalMerge.Rows[19].Cells[2].Value = "(Cr (VI))";
            dgvFinalMerge.Rows[20].Cells[2].Value = "(Co)";
            dgvFinalMerge.Rows[21].Cells[2].Value = "(Cu)";
            dgvFinalMerge.Rows[22].Cells[2].Value = "(Pb)";
            dgvFinalMerge.Rows[23].Cells[2].Value = "(Mn)";
            dgvFinalMerge.Rows[24].Cells[2].Value = "(Hg)";
            dgvFinalMerge.Rows[25].Cells[2].Value = "(Ni)";
            dgvFinalMerge.Rows[26].Cells[2].Value = "(Se)";
            dgvFinalMerge.Rows[27].Cells[2].Value = "(Sr)";
            dgvFinalMerge.Rows[28].Cells[2].Value = "(Sn)";
            dgvFinalMerge.Rows[29].Cells[2].Value = "--";
            dgvFinalMerge.Rows[30].Cells[2].Value = "(Zn)";

            dgvFinalMerge.Rows[33].Cells[2].Value = "(MeT)";
            dgvFinalMerge.Rows[34].Cells[2].Value = "(DProT)";
            dgvFinalMerge.Rows[35].Cells[2].Value = "(BuT)";
            dgvFinalMerge.Rows[36].Cells[2].Value = "(DBT)";
            dgvFinalMerge.Rows[37].Cells[2].Value = "(TBT)";
            dgvFinalMerge.Rows[38].Cells[2].Value = "(MOT)";
            dgvFinalMerge.Rows[39].Cells[2].Value = "(TeBT)";
            dgvFinalMerge.Rows[40].Cells[2].Value = "(DPhT)";
            dgvFinalMerge.Rows[41].Cells[2].Value = "(DOT)";
            dgvFinalMerge.Rows[42].Cells[2].Value = "(TPhT)";
        }  

        private void button10_Click(object sender, EventArgs e)
        {
            if (this.Width - textBox3.Location.X > 0)
                textBox3.Width = this.Width - textBox3.Location.X + 2;

            //if (this.Width - dgvFinal.Width > 0)
            //    dgvFinal.Width = this.Width - dgvFinal.Location.X + 2;

            dgvFinalMerge.RowCount = 44;
            dgvFinalMerge.ColumnCount = 100;

            dgvFinalMerge.Rows[1].Cells[0].Value = "Job No";
            dgvFinalMerge.Rows[2].Cells[0].Value = "Item No";
            dgvFinalMerge.Rows[3].Cells[0].Value = "Issue Date";
            dgvFinalMerge.Rows[4].Cells[0].Value = "Aurora Description";
            dgvFinalMerge.Rows[5].Cells[0].Value = "Screen Or Not";

            dgvFinalMerge.Rows[6].Cells[0].Value = "PartName";
            dgvFinalMerge.Rows[7].Cells[0].Value = "Materials";

            dgvFinalMerge.Rows[8].Cells[0].Value = "Sample No";
            dgvFinalMerge.Rows[9].Cells[0].Value = "Report Sample Description";
            dgvFinalMerge.Rows[10].Cells[0].Value = "Part3 Results";
            dgvFinalMerge.Rows[33].Cells[0].Value = "Organotin Results";

            dgvFinalMerge.Rows[10].Cells[1].Value = "Mass of trace amount";
            dgvFinalMerge.Rows[11].Cells[1].Value = "Soluble Aluminium";
            dgvFinalMerge.Rows[12].Cells[1].Value = "Soluble Antimony";
            dgvFinalMerge.Rows[13].Cells[1].Value = "Soluble Arsenic";
            dgvFinalMerge.Rows[14].Cells[1].Value = "Soluble Barium";
            dgvFinalMerge.Rows[15].Cells[1].Value = "Soluble Boron";
            dgvFinalMerge.Rows[16].Cells[1].Value = "Soluble Cadmium";
            dgvFinalMerge.Rows[17].Cells[1].Value = "Soluble Chromium";
            dgvFinalMerge.Rows[18].Cells[1].Value = "Soluble Chromium (III) #";
            dgvFinalMerge.Rows[19].Cells[1].Value = "Soluble Chromium (VI) #";
            dgvFinalMerge.Rows[20].Cells[1].Value = "Soluble Cobalt";
            dgvFinalMerge.Rows[21].Cells[1].Value = "Soluble Copper";
            dgvFinalMerge.Rows[22].Cells[1].Value = "Soluble Lead";
            dgvFinalMerge.Rows[23].Cells[1].Value = "Soluble Manganese";
            dgvFinalMerge.Rows[24].Cells[1].Value = "Soluble Mercury";
            dgvFinalMerge.Rows[25].Cells[1].Value = "Soluble Nickel";
            dgvFinalMerge.Rows[26].Cells[1].Value = "Soluble Selenium";
            dgvFinalMerge.Rows[27].Cells[1].Value = "Soluble Strontium";
            dgvFinalMerge.Rows[28].Cells[1].Value = "Soluble Tin";
            dgvFinalMerge.Rows[29].Cells[1].Value = "Soluble Organic Tin^";
            dgvFinalMerge.Rows[30].Cells[1].Value = "Soluble Zinc";

            dgvFinalMerge.Rows[34].Cells[1].Value = "Dimethyl tin";
            dgvFinalMerge.Rows[34].Cells[1].Value = "Methyl tin";
            dgvFinalMerge.Rows[35].Cells[1].Value = "Di-n-propyl tin";
            dgvFinalMerge.Rows[36].Cells[1].Value = "Butyl tin";
            dgvFinalMerge.Rows[37].Cells[1].Value = "Dibutyl tin";
            dgvFinalMerge.Rows[38].Cells[1].Value = "Tributyl tin";
            dgvFinalMerge.Rows[39].Cells[1].Value = "n-Octyl tin";
            dgvFinalMerge.Rows[40].Cells[1].Value = "Tetrabutyl tin";
            dgvFinalMerge.Rows[41].Cells[1].Value = "Diphenyl tin";
            dgvFinalMerge.Rows[42].Cells[1].Value = "Di-n-octyl tin";
            dgvFinalMerge.Rows[43].Cells[1].Value = "Triphenyl tin";

            dgvFinalMerge.Rows[10].Cells[2].Value = "(mg)";
            dgvFinalMerge.Rows[11].Cells[2].Value = "(Al)";
            dgvFinalMerge.Rows[12].Cells[2].Value = "(Sb)";
            dgvFinalMerge.Rows[13].Cells[2].Value = "(As)";
            dgvFinalMerge.Rows[14].Cells[2].Value = "(Ba)";
            dgvFinalMerge.Rows[15].Cells[2].Value = "(B)";
            dgvFinalMerge.Rows[16].Cells[2].Value = "(Cd)";
            dgvFinalMerge.Rows[17].Cells[2].Value = "(Cr)";
            dgvFinalMerge.Rows[18].Cells[2].Value = "(Cr (III))";
            dgvFinalMerge.Rows[19].Cells[2].Value = "(Cr (VI))";
            dgvFinalMerge.Rows[20].Cells[2].Value = "(Co)";
            dgvFinalMerge.Rows[21].Cells[2].Value = "(Cu)";
            dgvFinalMerge.Rows[22].Cells[2].Value = "(Pb)";
            dgvFinalMerge.Rows[23].Cells[2].Value = "(Mn)";
            dgvFinalMerge.Rows[24].Cells[2].Value = "(Hg)";
            dgvFinalMerge.Rows[25].Cells[2].Value = "(Ni)";
            dgvFinalMerge.Rows[26].Cells[2].Value = "(Se)";
            dgvFinalMerge.Rows[27].Cells[2].Value = "(Sr)";
            dgvFinalMerge.Rows[28].Cells[2].Value = "(Sn)";
            dgvFinalMerge.Rows[29].Cells[2].Value = "--";
            dgvFinalMerge.Rows[30].Cells[2].Value = "(Zn)";

            dgvFinalMerge.Rows[33].Cells[2].Value = "(DMT)";
            dgvFinalMerge.Rows[34].Cells[2].Value = "(MeT)";
            dgvFinalMerge.Rows[35].Cells[2].Value = "(DProT)";
            dgvFinalMerge.Rows[36].Cells[2].Value = "(BuT)";
            dgvFinalMerge.Rows[37].Cells[2].Value = "(DBT)";
            dgvFinalMerge.Rows[38].Cells[2].Value = "(TBT)";
            dgvFinalMerge.Rows[39].Cells[2].Value = "(MOT)";
            dgvFinalMerge.Rows[40].Cells[2].Value = "(TeBT)";
            dgvFinalMerge.Rows[41].Cells[2].Value = "(DPhT)";
            dgvFinalMerge.Rows[42].Cells[2].Value = "(DOT)";
            dgvFinalMerge.Rows[43].Cells[2].Value = "(TPhT)";

            dgvFinal.RowCount = 44;
            dgvFinal.ColumnCount = 300;

            dgvFinal.Rows[1].Cells[0].Value = "Job No";
            dgvFinal.Rows[2].Cells[0].Value = "Item No";
            dgvFinal.Rows[3].Cells[0].Value = "Issue Date";
            dgvFinal.Rows[4].Cells[0].Value = "Aurora Description";
            dgvFinal.Rows[5].Cells[0].Value = "Screen Or Not";

            dgvFinal.Rows[6].Cells[0].Value = "PartName";
            dgvFinal.Rows[7].Cells[0].Value = "Materials";

            dgvFinal.Rows[8].Cells[0].Value = "Sample No";
            dgvFinal.Rows[9].Cells[0].Value = "Report Sample Description";
            dgvFinal.Rows[10].Cells[0].Value = "Part3 Results";
            dgvFinal.Rows[33].Cells[0].Value = "Organotin Results";

            dgvFinal.Rows[10].Cells[1].Value = "Mass of trace amount";
            dgvFinal.Rows[11].Cells[1].Value = "Soluble Aluminium";
            dgvFinal.Rows[12].Cells[1].Value = "Soluble Antimony";
            dgvFinal.Rows[13].Cells[1].Value = "Soluble Arsenic";
            dgvFinal.Rows[14].Cells[1].Value = "Soluble Barium";
            dgvFinal.Rows[15].Cells[1].Value = "Soluble Boron";
            dgvFinal.Rows[16].Cells[1].Value = "Soluble Cadmium";
            dgvFinal.Rows[17].Cells[1].Value = "Soluble Chromium";
            dgvFinal.Rows[18].Cells[1].Value = "Soluble Chromium (III) #";
            dgvFinal.Rows[19].Cells[1].Value = "Soluble Chromium (VI) #";
            dgvFinal.Rows[20].Cells[1].Value = "Soluble Cobalt";
            dgvFinal.Rows[21].Cells[1].Value = "Soluble Copper";
            dgvFinal.Rows[22].Cells[1].Value = "Soluble Lead";
            dgvFinal.Rows[23].Cells[1].Value = "Soluble Manganese";
            dgvFinal.Rows[24].Cells[1].Value = "Soluble Mercury";
            dgvFinal.Rows[25].Cells[1].Value = "Soluble Nickel";
            dgvFinal.Rows[26].Cells[1].Value = "Soluble Selenium";
            dgvFinal.Rows[27].Cells[1].Value = "Soluble Strontium";
            dgvFinal.Rows[28].Cells[1].Value = "Soluble Tin";
            dgvFinal.Rows[29].Cells[1].Value = "Soluble Organic Tin^";
            dgvFinal.Rows[30].Cells[1].Value = "Soluble Zinc";

            dgvFinal.Rows[33].Cells[1].Value = "Dimethyl tin";
            dgvFinal.Rows[34].Cells[1].Value = "Methyl tin";
            dgvFinal.Rows[35].Cells[1].Value = "Di-n-propyl tin";
            dgvFinal.Rows[36].Cells[1].Value = "Butyl tin";
            dgvFinal.Rows[37].Cells[1].Value = "Dibutyl tin";
            dgvFinal.Rows[38].Cells[1].Value = "Tributyl tin";
            dgvFinal.Rows[39].Cells[1].Value = "n-Octyl tin";
            dgvFinal.Rows[40].Cells[1].Value = "Tetrabutyl tin";
            dgvFinal.Rows[41].Cells[1].Value = "Diphenyl tin";
            dgvFinal.Rows[42].Cells[1].Value = "Di-n-octyl tin";
            dgvFinal.Rows[43].Cells[1].Value = "Triphenyl tin";

            dgvFinal.Rows[10].Cells[2].Value = "(mg)";
            dgvFinal.Rows[11].Cells[2].Value = "(Al)";
            dgvFinal.Rows[12].Cells[2].Value = "(Sb)";
            dgvFinal.Rows[13].Cells[2].Value = "(As)";
            dgvFinal.Rows[14].Cells[2].Value = "(Ba)";
            dgvFinal.Rows[15].Cells[2].Value = "(B)";
            dgvFinal.Rows[16].Cells[2].Value = "(Cd)";
            dgvFinal.Rows[17].Cells[2].Value = "(Cr)";
            dgvFinal.Rows[18].Cells[2].Value = "(Cr (III))";
            dgvFinal.Rows[19].Cells[2].Value = "(Cr (VI))";
            dgvFinal.Rows[20].Cells[2].Value = "(Co)";
            dgvFinal.Rows[21].Cells[2].Value = "(Cu)";
            dgvFinal.Rows[22].Cells[2].Value = "(Pb)";
            dgvFinal.Rows[23].Cells[2].Value = "(Mn)";
            dgvFinal.Rows[24].Cells[2].Value = "(Hg)";
            dgvFinal.Rows[25].Cells[2].Value = "(Ni)";
            dgvFinal.Rows[26].Cells[2].Value = "(Se)";
            dgvFinal.Rows[27].Cells[2].Value = "(Sr)";
            dgvFinal.Rows[28].Cells[2].Value = "(Sn)";
            dgvFinal.Rows[29].Cells[2].Value = "--";
            dgvFinal.Rows[30].Cells[2].Value = "(Zn)";

            dgvFinal.Rows[33].Cells[2].Value = "(DMT)";
            dgvFinal.Rows[34].Cells[2].Value = "(MeT)";
            dgvFinal.Rows[35].Cells[2].Value = "(DProT)";
            dgvFinal.Rows[36].Cells[2].Value = "(BuT)";
            dgvFinal.Rows[37].Cells[2].Value = "(DBT)";
            dgvFinal.Rows[38].Cells[2].Value = "(TBT)";
            dgvFinal.Rows[39].Cells[2].Value = "(MOT)";
            dgvFinal.Rows[40].Cells[2].Value = "(TeBT)";
            dgvFinal.Rows[41].Cells[2].Value = "(DPhT)";
            dgvFinal.Rows[42].Cells[2].Value = "(DOT)";
            dgvFinal.Rows[43].Cells[2].Value = "(TPhT)";

            dgvMergeFiles.RowCount = 50;
            foreach (DataGridViewRow row1 in dgvValues.Rows)
            {
                if (row1.Index >= 0 & row1.Index <= 5)
                {
                    dgvMergeFiles.Rows.Add(1);
                    foreach (DataGridViewColumn col1 in dgvValues.Columns)
                    {
                        if (col1.Index >= 0 & col1.Index <= 3)
                        {
                            dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].Value = dgvValues.Rows[row1.Index].Cells[col1.Index].Value;
                            //if (dgvMergeFiles.Rows[row1.Index].Cells[col1.Index].ToString().Contains("Part") & row1.Index == 5)
                            //{
                            //    dgvMergeFiles.Rows[row1.Index].Cells[4].Value = "Report No";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[5].Value = "Issued Date";
                            //    dgvMergeFiles.Rows[row1.Index].Cells[6].Value = "Report Sample Description";
                            //}
                        }
                    }
                }
            }

            //dgvMergeFiles.Rows[5].Cells[4].Value = "Report No";
            //dgvMergeFiles.Rows[5].Cells[5].Value = "Issued Date";
            //dgvMergeFiles.Rows[5].Cells[6].Value = "Report Sample Description";

            string fileName = OpenFile();
            //string fileName = "E:\\T42\\WORK\\request\\오로라\\테스트\\복사본 KI2015019.xls";
            string plainText = string.Empty;
            string ext = System.IO.Path.GetExtension(fileName).ToUpper();
            string directoryPath = string.Empty;




            if (ext.Equals(".DOCX") || ext.Equals(".DOC"))
            {

                //Create word document

                Document document = new Document();




                //load a document

                document.LoadFromFile(@fileName);




                plainText = document.GetText();

                this.textBox3.Text = plainText;

            }

            else if (ext.Equals(".XLSX") || ext.Equals(".XLS"))
            {

                //Create Excel workbook

                Workbook workbook = new Workbook();




                //load a workbook

                workbook.LoadFromFile(@fileName);

                directoryPath = "c:\\temp\\";


                for (int i = 0; i < workbook.Worksheets.Count; i++)
                {

                    string tmpfilename = "tempSheet" + i.ToString() + ".txt";

                    Worksheet sheet = workbook.Worksheets[i];




                    if (!sheet.IsEmpty)
                    {

                        if (System.IO.File.Exists(directoryPath + tmpfilename))
                        {

                            System.IO.File.Delete(directoryPath + tmpfilename);

                        }


                        sheet.SaveToFile(directoryPath + tmpfilename, ", ", Encoding.UTF8);

                        plainText += "--[" + sheet.Name + "]--\r\n";

                        plainText += System.IO.File.ReadAllText(directoryPath + tmpfilename);

                        plainText += "\r\n";

                    }

                }
                this.textBox2.Text = plainText;
            }

        }

        private void button11_Click(object sender, EventArgs e)
        {
            // Get the file's text.
            string whole_file = textBox2.Text;

            // Split into lines.
            //\r\n
            whole_file = whole_file.Replace('\n', '\r');
            whole_file = whole_file.Replace('"', ' ');
            string[] lines = whole_file.Split(new char[] { '\r' },
                StringSplitOptions.RemoveEmptyEntries);

            // See how many rows and columns there are.
            int num_rows = lines.Length;
            //MessageBox.Show(lines[num_rows - 1]);
            //int num_cols = lines[num_rows - 1].Split(',').Length; //일반
            int num_cols = 10;  //오로라 전용

            // Allocate the data array.
            string[,] values = new string[num_rows, num_cols];

            // Load the array.
            for (int r = 1; r < num_rows; r++)
            {
                string[] line_r = lines[r].Split(',');
                int row_cols = lines[r].Split(',').Length;

                for (int c = 0; c < row_cols; c++)
                {
                    if (line_r[c] == null)
                        values[r, c] = "";
                    else
                        values[r, c] = line_r[c];
                }
            }

            int num1_rows = values.GetUpperBound(0) + 1;
            int num1_cols = values.GetUpperBound(1) + 1;

            // Display the data to show we have it.

            // Make column headers.
            // For this example, we assume the first row
            // contains the column names.

            // Clear previous results.
            dgvValues.Rows.Clear();

            for (int c = 0; c < num1_cols; c++)
                dgvValues.Columns.Add(values[0, c], values[0, c]);

            // Add the data.
            for (int r = 1; r < num1_rows; r++)
            {
                dgvValues.Rows.Add();
                for (int c = 0; c < num1_cols; c++)
                {
                    dgvValues.Rows[r - 1].Cells[c].Value = values[r, c];
                }
            }

            //// Make the columns autosize.
            //foreach (DataGridViewColumn col in dgvValues.Columns)
            //    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

        }

        private void Result_Merge()
        {
            string plainText = string.Empty;

            int RowChk, RowNo;
            int SampleTot;
            string sRptName;
            string sIssuedDate;

            sIssuedDate = "";
            sRptName = "";
            SampleTot = 0;
            RowNo = 0;
            RowChk = 0;

            dgvFinal.Rows.Clear();
            //dgvFinal.RowCount;  
            dgvFinalMerge.Rows.Clear();
            Initialize_Grid();

            foreach (DataGridViewRow row2 in dgvMergeFiles.Rows)
            {
                /////////////////
                //dgvFinalMerge.Rows.Clear();
                //dgvFinalMerge_initialization();

                progressBar1.Maximum = dgvMergeFiles.Rows.Count;
                //progressBar1.Value = (int)dgvMergeFiles.Rows.Count / 2;

                //Create word document
                if (row2.Index <= dgvMergeFiles.Rows.Count - 2)
                {
                    progressBar1.Value = row2.Index;
                    string wRptFileName = (string)row2.Cells[11].Value;
                    if (wRptFileName != null)
                        wRptFileName = wRptFileName.Trim();
                    else
                        wRptFileName = "";

                    if (wRptFileName == "" || wRptFileName == null)
                    {
                    }
                    else if (sRptName == wRptFileName)
                    {
                        dgvMergeFiles.Rows[row2.Index].Cells[8].Value = sIssuedDate;
                        sRptName = wRptFileName;
                    }
                    else if (wRptFileName == "Item Number")
                    {
                        dgvFinalMerge.Rows[0].Cells[3].Value = "";
                        dgvFinalMerge.Rows[0].Cells[3].Value = (string)row2.Cells[12].Value;
                    }
                    else if (wRptFileName == "Product Description")
                    {
                        int ItemCnt;
                        ItemCnt = 1;

                        dgvFinalMerge.Rows[1].Cells[3].Value = "";
                        dgvFinalMerge.Rows[1].Cells[3].Value = (string)row2.Cells[12].Value;
                        //개별 job의 샘플 결과를 취합하는 단계 시작 ---------------

                        SampleTot = SampleTot + ItemCnt;

                        for (int rr = 0; rr < 2; rr++)
                        {
                            //dgvFinal.Columns.Add(1);
                            for (int cc = 3; cc < ItemCnt + 3; cc++)
                            {
                                if (rr < 8)
                                    dgvFinal.Rows[rr].Cells[SampleTot + cc - ItemCnt].Value = dgvFinalMerge.Rows[rr].Cells[3].Value;
                                else
                                    dgvFinal.Rows[rr].Cells[SampleTot + cc - ItemCnt].Value = dgvFinalMerge.Rows[rr].Cells[cc].Value;
                            }
                        }
                        //개별 job의 샘플 결과를 취합하는 단계 시작 ---------------
                    }
                    else
                    {
                        dgvFinalMerge.Rows.Clear();
                        Initialize_Grid();

                        sRptName = wRptFileName;
                        wRptFileName = wRptFileName.Trim();

                        Document document = new Document();
                        string ext = System.IO.Path.GetExtension(wRptFileName).ToUpper();

                        if (ext.Equals(".DOCX") || ext.Equals(".DOC"))
                        {
                            document.LoadFromFile(@wRptFileName);

                            plainText = document.GetText();
                            this.textBox3.Text = plainText;

                            RowChk = RowChk + 1;
                            RowNo = RowChk + row2.Index;


                            //파일 내용을 분해하는 단계 시작------------------------
                            // Get the file's text.
                            string whole_file = textBox3.Text;

                            // Split into lines.
                            //\r\n
                            whole_file = whole_file.Replace('\n', '\r');
                            //whole_file = whole_file.Replace('"', ' ');
                            string[] lines = whole_file.Split(new char[] { '\r' },
                                StringSplitOptions.RemoveEmptyEntries);

                            // See how many rows and columns there are.
                            int num_rows = lines.Length;
                            int num_cols = 1;
                            //int num_cols = lines[num_rows - 1].Split(',').Length;

                            // Allocate the data array.
                            string[,] values = new string[num_rows, num_cols];

                            // Load the array.
                            for (int r = 0; r < num_rows; r++)
                            {
                                //string[] line_r = lines[r].Split(',');
                                string[] line_r = lines[r].Split(new char[] { '\r' },
                                StringSplitOptions.RemoveEmptyEntries);
                                for (int c = 0; c < num_cols; c++)
                                {
                                    if (line_r[c] == null)
                                        values[r, c] = "";
                                    else
                                        values[r, c] = line_r[c];
                                }
                            }

                            int num1_rows = values.GetUpperBound(0) + 1;
                            int num1_cols = values.GetUpperBound(1) + 1;

                            // Display the data to show we have it.

                            // Make column headers.
                            // For this example, we assume the first row
                            // contains the column names.

                            // Clear previous results.
                            dgvResults.Rows.Clear();

                            for (int c = 0; c < num1_cols; c++)
                                dgvResults.Columns.Add(values[0, c], values[0, c]);

                            // Add the data.
                            for (int r = 1; r <= num1_rows; r++)
                            {
                                dgvResults.Rows.Add();
                                for (int c = 0; c < num1_cols; c++)
                                {
                                    dgvResults.Rows[r - 1].Cells[c].Value = values[r - 1, c];
                                }
                            }

                            //파일 내용을 분해하는 단계 끝------------------------

                            //string Workchk;
                            //Workchk = "클릭하세요";
                            //MessageBox.Show(@txtDir1.Text + '\\' + row2.Cells[0].Value);

                            //분해된 내용을 정리하는 단계 시작 ----------------------------
                            string SourceValue, TargetValue, PreviousValue, AnalyteName;
                            int JobNoChk, JobNoRow, TargetRowNo, TargetColNo, SamResultchk, SampleCnt, SaveSampleCnt, SaveCurRow, i, j, ColNoChk;
                            int SampleTotCnt;
                            int[] nCol = new int[100];
                            int[] wCol = new int[100];
                            string[] nColValue = new string[100];

                            AnalyteName = "";
                            JobNoChk = 0;
                            JobNoRow = 0;
                            TargetRowNo = 0;
                            TargetColNo = 0;
                            SamResultchk = 0;
                            ColNoChk = 2;
                            SampleCnt = 0;
                            SaveSampleCnt = 0;
                            SaveCurRow = 0;
                            SampleTotCnt = 0;
                            i = 0;
                            j = 0;

                            SourceValue = "";
                            TargetValue = "";
                            PreviousValue = "";

                            foreach (DataGridViewRow row3 in dgvResults.Rows)
                            {
                                if (row3.Cells[0].Value != null)
                                    SourceValue = (string)row3.Cells[0].Value;
                                else
                                    SourceValue = "";

                                dgvFinalMerge.Rows[6].Cells[3].Value = dgvMergeFiles.Rows[row2.Index].Cells[0].Value;
                                dgvFinalMerge.Rows[7].Cells[3].Value = dgvMergeFiles.Rows[row2.Index].Cells[1].Value;

                                // SGS File No.
                                if (SourceValue == "SGS File No.")
                                {
                                    ColNoChk = ColNoChk + 1;

                                    TargetRowNo = 1;
                                    TargetColNo = ColNoChk;

                                    JobNoChk = 1;
                                    JobNoRow = row3.Index + 2;
                                    AnalyteName = SourceValue;
                                }

                                // Style no./Item no.
                                if (SourceValue == "Style no./Item no.")
                                {

                                    ColNoChk = ColNoChk + 1;

                                    TargetRowNo = 2;
                                    TargetColNo = ColNoChk;

                                    JobNoChk = 1;
                                    JobNoRow = row3.Index + 2;
                                    AnalyteName = SourceValue;
                                }

                                //?Issued Date
                                if (SourceValue.ToString().Contains("Issued Date"))
                                {
                                    sIssuedDate = "";

                                    ColNoChk = ColNoChk + 1;

                                    TargetRowNo = 3;
                                    TargetColNo = ColNoChk;

                                    JobNoChk = 1;
                                    JobNoRow = row3.Index + 2;

                                    TargetValue = SourceValue.Substring(SourceValue.IndexOf("Issued Date") + 13, 12);
                                    sIssuedDate = TargetValue;

                                    //검색된 워드 성적서 발행일자를 집계 파일에 넣어줌.
                                    dgvMergeFiles.Rows[row2.Index].Cells[8].Value = TargetValue;
                                    AnalyteName = SourceValue;
                                }

                                //?Sample Description
                                if (SourceValue == "Sample Description")
                                {

                                    ColNoChk = ColNoChk + 1;

                                    TargetRowNo = 4;
                                    TargetColNo = ColNoChk;

                                    JobNoChk = 1;
                                    JobNoRow = row3.Index + 2;
                                    AnalyteName = SourceValue;
                                }


                                //Permissible Limit (mg/kg)
                                if (SourceValue == "Permissible Limit (mg/kg)" || SourceValue == "(mg/kg)" || SourceValue == "Permissible Limit (EN71-3:2013 +A1:2014) (mg/kg)" || SourceValue == "(EN71-3:2013 +A1:2014) (mg/kg)")
                                {
                                    SamResultchk = 1;
                                    SampleCnt = 0;
                                    AnalyteName = SourceValue;
                                }
                                else if (SamResultchk == 1 & SourceValue != "Mass of trace amount")
                                {
                                    SampleCnt = SampleCnt + 1;
                                    ColNoChk = ColNoChk + SampleCnt;

                                    nCol[SampleCnt] = ColNoChk;
                                    nColValue[SampleCnt] = SourceValue;

                                    for (i = 1; i <= nCol.LongLength - 1; i++)
                                    {
                                        if (nColValue[i] == SourceValue)
                                        {
                                            wCol[SampleCnt] = nCol[i];
                                        }
                                    }

                                    TargetRowNo = 8;
                                    TargetColNo = ColNoChk;

                                    JobNoChk = 1;
                                    JobNoRow = row3.Index;
                                    AnalyteName = SourceValue;
                                }
                                else if (SamResultchk == 1 & SourceValue == "Mass of trace amount")
                                {
                                    SamResultchk = 0;
                                    SampleTotCnt = SampleCnt;
                                    SaveSampleCnt = SampleCnt;
                                    SampleCnt = 0;
                                    AnalyteName = SourceValue;
                                }
                                //(mg)
                                else if (SourceValue == "(mg)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 10;

                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Al)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 11;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Sb)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 12;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(As)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 13;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Ba)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 14;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(B)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 15;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Cd)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 16;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Cr)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 17;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Cr (III))")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 18;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Cr (VI))")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 19;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Co)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 20;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Cu)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 21;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Pb)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 22;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Mn)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 23;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Hg)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 24;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Ni)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 25;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Se)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 26;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Sr)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 27;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Sn)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 28;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "Soluble Organic Tin" || SourceValue == "Soluble Organic Tin^")
                                {
                                    //ExceptChk = 1;
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    //TargetRowNo = TargetRowNo + 1 ;
                                    PreviousValue = SourceValue;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "--" & (PreviousValue == "Soluble Organic Tin" || PreviousValue == "Soluble Organic Tin^"))
                                {
                                    //ExceptChk = 1;
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    PreviousValue = "";
                                    TargetRowNo = 29;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(Zn)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 30;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                //Soluble Organic Tin Result(s) (mg/kg)
                                else if (SourceValue == "Soluble Organic Tin Result(s) (mg/kg)")
                                {
                                    SamResultchk = 2;
                                    SampleCnt = 0;
                                    ColNoChk = 2;
                                    j = 0;
                                    AnalyteName = SourceValue;
                                }
                                else if (SamResultchk == 2 & SourceValue != "Methyl tin")
                                {
                                    SampleCnt = SampleCnt + 1;
                                    for (i = 1; i <= nCol.LongLength - 1; i++)
                                    {
                                        if (nColValue[i] == SourceValue)
                                        {
                                            j = j + 1;
                                            wCol[j] = nCol[i];
                                        }
                                    }
                                    JobNoChk = 1;
                                    JobNoRow = 0;
                                    AnalyteName = SourceValue;
                                }
                                else if (SamResultchk == 2 & SourceValue == "Methyl tin")
                                {
                                    SamResultchk = 0;
                                    SaveSampleCnt = SampleCnt - 1;  //샘플 수량 파악 SaveSampleCnt에 저장
                                    SampleCnt = 0;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(DMT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 33;
                                    //TargetRowNo = TargetRowNo + 3;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(MeT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 34;
                                    //TargetRowNo = TargetRowNo + 3;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(DProT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 35;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(BuT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 36;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(DBT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 37;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(TBT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 38;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(MOT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 39;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(TeBT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 40;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(DPhT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 41;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(DOT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 42;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                else if (SourceValue == "(TPhT)")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 43;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }
                                //?Sample Description :
                                else if (SourceValue == "Sample Description :")
                                {
                                    SamResultchk = 0;
                                    SaveSampleCnt = SampleTotCnt; //샘플 수량 파악 SaveSampleCnt에 저장
                                    SampleCnt = 0;
                                    AnalyteName = SourceValue;

                                    PreviousValue = "Sample Description :";
                                }
                                else if (PreviousValue == "Sample Description :" & SourceValue == (SampleCnt + 1).ToString().Trim() + ".")
                                {
                                    SaveCurRow = row3.Index;
                                    SamResultchk = SaveCurRow;
                                    TargetRowNo = 9;
                                    //TargetRowNo = TargetRowNo + 1;
                                    AnalyteName = SourceValue;
                                }

                            //else if (PreviousValue == "Sample Description :" & SourceValue == "2.")
                                //    {
                                //        SaveCurRow = row3.Index;
                                //        SamResultchk = SaveCurRow;
                                //        TargetRowNo = 9;
                                //        //TargetRowNo = TargetRowNo + 1;
                                //        AnalyteName = SourceValue;
                                //    }
                                else if (SamResultchk == SaveCurRow & SaveSampleCnt != SampleCnt)
                                {
                                    SampleCnt = SampleCnt + 1;
                                    ColNoChk = ColNoChk + SampleCnt;

                                    //TargetRowNo = 9;
                                    if (SampleCnt > 0)
                                        TargetColNo = wCol[SampleCnt];
                                    else
                                        TargetColNo = ColNoChk;

                                    JobNoChk = 1;
                                    JobNoRow = row3.Index;
                                    //MessageBox.Show(AnalyteName + " : " + SourceValue); 
                                }
                                else if (SamResultchk == SaveCurRow & SaveSampleCnt == SampleCnt)
                                {
                                    SamResultchk = 0;
                                    SampleCnt = 0;
                                    //PreviousValue = "";

                                }

                                // 각 항목별로 값 적용 
                                if (row3.Index == JobNoRow & JobNoChk == 1)
                                {
                                    JobNoChk = 0;
                                    JobNoRow = 0;
                                    if (TargetRowNo == 3)
                                    {
                                        dgvFinalMerge.Rows[TargetRowNo].Cells[3].Value = TargetValue;
                                        TargetValue = "";
                                        ColNoChk = ColNoChk - 1;
                                    }
                                    else
                                    {
                                        if (PreviousValue != "Soluble Organic Tin^" || PreviousValue != "Soluble Organic Tin")
                                        {
                                            dgvFinalMerge.Rows[TargetRowNo].Cells[0].Value = TargetRowNo;
                                            dgvFinalMerge.Rows[TargetRowNo].Cells[TargetColNo].Value = row3.Cells[0].Value;
                                            //return;
                                            if (SampleCnt < 1)
                                                ColNoChk = ColNoChk - 1;
                                            else
                                                ColNoChk = ColNoChk - SampleCnt;
                                            //return;
                                        }
                                    }
                                }

                            }
                            //return;
                            //분해된 내용을 정리하는 단계 끝 ----------------------------

                            //개별 job의 샘플 결과를 취합하는 단계 시작 ---------------

                            SampleTot = SampleTot + SampleTotCnt;

                            for (int rr = 0; rr < 43; rr++)
                            {
                                //dgvFinal.Columns.Add(1);
                                for (int cc = 3; cc < SampleTotCnt + 3; cc++)
                                {
                                    if (rr < 8)
                                        dgvFinal.Rows[rr].Cells[SampleTot + cc - SampleTotCnt].Value = dgvFinalMerge.Rows[rr].Cells[3].Value;
                                    else
                                        dgvFinal.Rows[rr].Cells[SampleTot + cc - SampleTotCnt].Value = dgvFinalMerge.Rows[rr].Cells[cc].Value;
                                }
                            }
                            //개별 job의 샘플 결과를 취합하는 단계 시작 ---------------

                        }
                    }
                }

            }
            progressBar1.Value = dgvMergeFiles.Rows.Count;
        }
  
        private void dgvMergeFiles_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show(e.RowIndex.ToString() + ", "+ e.ColumnIndex);  //jhm 2019-04-23


            //if (e.ColumnIndex == 11)
            //{
            //    string wRptFileName, plainText2;
            //    object value = dgvMergeFiles.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            //    if (value is DBNull) { return; }
            //    if (value.ToString().Trim().ToUpper().IndexOf(".DOC") < 1) { return; }

            //    wRptFileName = value.ToString();

            //    MessageBox.Show(wRptFileName);

            //    Document document = new Document();
            //    document.LoadFromFile(@wRptFileName);

            //    plainText2 = document.GetText();
            //    this.textBox3.Text = plainText2;

            //    WordDocViewer(wRptFileName);
            //}

            //if (e.ColumnIndex == 12)
            //{
            //    object value1 = dgvMergeFiles.Rows[e.RowIndex - 1].Cells[e.ColumnIndex].Value;
            //    object value2 = dgvMergeFiles.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

            //    if (value1 is DBNull || value2 is DBNull)
            //    {
            //        MessageBox.Show("성적서 이름 확인 바람");
            //        return;
            //    }

            //    if (value1.ToString().Trim() == "" || value2.ToString().Trim() == "")
            //    {
            //        MessageBox.Show("성적서 이름 확인 바람");
            //        return;
            //    }
            //    Combine_Rpt(value1.ToString(), value2.ToString());
            //}


        }
        
        private void Find_Slim_KRCTS01()
        {
            string connect_string = "Provider=SQLOLEDB.1;User ID=Aurora;pwd=Aurora2020;data source=krslimdb001;Persist Security Info=True;initial catalog=Aurora";
            
            Conn = new OleDbConnection(connect_string);

            // 1. copy first grid to second grid
            dgvMergeFiles.Rows.Clear();
            dgvMergeFiles.RowCount = 1000;
            dgvMergeFiles.ColumnCount = 13;

            int wRowNo, wEof;
            string wRowZero, sItemNumber;
            string wJobNo;
            wJobNo = "";

            wRowNo = 0;
            wRowZero = "";
            sItemNumber = "";

            wEof = 0;
            wRowNo = -1;

            foreach (DataGridViewRow row1 in dgvValues.Rows)
            {
                wRowNo = wRowNo + 1;


                ///---1st----///

                if (dgvValues.Rows[row1.Index].Cells[0].Value != null & (string)dgvValues.Rows[row1.Index].Cells[0].Value != "")
                {
                    dgvMergeFiles.Rows[wRowNo].Cells[0].Value = dgvValues.Rows[row1.Index].Cells[0].Value;

                    wRowZero = (string)dgvMergeFiles.Rows[wRowNo].Cells[0].Value;
                    wRowZero = wRowZero.Trim();
                }
                else if (wRowZero == "I&P1")
                {
                    dgvMergeFiles.Rows[wRowNo].Cells[11].Value = "Part 1,2";
                    dgvMergeFiles.Rows[wRowNo].Cells[12].Value = wJobNo;

                    wRowZero = "I&P2";
                }
                else if (wRowZero == "I&P2")
                {
                    dgvMergeFiles.Rows[wRowNo].Cells[11].Value = "Part 3";
                    dgvMergeFiles.Rows[wRowNo].Cells[12].Value = sItemNumber;
                    wRowZero = "";
                }
                ///----------///
                /// 
                ///---2nd----///
                if (dgvValues.Rows[row1.Index].Cells[1].Value != null & (string)dgvValues.Rows[row1.Index].Cells[1].Value != "")
                {
                    dgvMergeFiles.Rows[wRowNo].Cells[1].Value = dgvValues.Rows[row1.Index].Cells[1].Value;
                    if (wRowZero == "Item Number" || wRowZero == "Product Description")
                    {
                        dgvMergeFiles.Rows[wRowNo].Cells[11].Value = (string)dgvMergeFiles.Rows[wRowNo].Cells[0].Value;
                        dgvMergeFiles.Rows[wRowNo].Cells[12].Value = (string)dgvMergeFiles.Rows[wRowNo].Cells[1].Value;
                        if (wRowZero == "Item Number")
                        {
                            wJobNo = "";
                            sItemNumber = "";

                            sItemNumber = (string)dgvMergeFiles.Rows[wRowNo].Cells[12].Value;
                            sItemNumber = sItemNumber.Trim();

                            //물성이어서 필요가 없음
                            string query = "SELECT top 1 isnull(a.pro_proj,'') as pro_proj " + "\r" + "\n" +
                                         "  from  " + "\r" + "\n" +
                                         "  profjob_aurora a with (nolock) , profjob_cuid_aurora b with (nolock) , profjob_cuid_scheme_aurora c with (nolock) , KRCTS01.dbo.scheme d with (nolock)  " + "\r" + "\n" +
                                         "  WHERE a.labcode = b.labcode    " + "\r" + "\n" +
                                         "    and a.labcode = c.labcode    " + "\r" + "\n" +
                                         "    and a.labcode = d.labcode    " + "\r" + "\n" +
                                         "    and a.pro_job = b.pro_job    " + "\r" + "\n" +
                                         "    and a.pro_job = c.pro_job   " + "\r" + "\n" +
                                         "    and b.cuid = c.cuid    " + "\r" + "\n" +
                                "            and c.sch_code = d.sch_code " + "\r" + "\n" +
                                "            and c.schversion = d.schversion " + "\r" + "\n" +
                                "            and (d.schemename like '0324%'    or d.schemename like '0424%')  " + "\r" + "\n" +
                                "            and a.cli_code = '1600008533_INE' and  a.orderno like '%" + sItemNumber + "%'" + "\r" + "\n" +
                                "            order by a.received desc ";

                            OleDbCommand cmd = new OleDbCommand(query, Conn);

                            Conn.Open();
                            using (OleDbDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    if ((string)reader.GetValue(0) == null)
                                    {
                                        wJobNo = "";
                                    }
                                    else
                                    {
                                        wJobNo = (string)reader.GetValue(0);
                                        wJobNo = wJobNo.Trim();

                                    }

                                    wJobNo = wJobNo.Replace("\n", "");
                                    wEof = 1;
                                }
                                if (wEof == 1)
                                {
                                    wEof = 0;
                                }
                            }
                            Conn.Close();
                        }

                        if (wRowZero == "Product Description")
                        {
                            wRowZero = "I&P1";
                        }
                    }
                }

                ///----------///
                /// 
                ///---3rd & job----///
                if (dgvValues.Rows[row1.Index].Cells[2].Value != null || (string)dgvValues.Rows[row1.Index].Cells[2].Value != "")
                    dgvMergeFiles.Rows[wRowNo].Cells[2].Value = dgvValues.Rows[row1.Index].Cells[2].Value;

                if (dgvValues.Rows[row1.Index].Cells[3].Value != null || (string)dgvValues.Rows[row1.Index].Cells[3].Value != "")
                {
                    string Tjobno = "";
                    Tjobno = (string)dgvValues.Rows[row1.Index].Cells[3].Value;
                    dgvMergeFiles.Rows[wRowNo].Cells[3].Value = Tjobno;

                }

                ///----------///
                ///

                string wPartName = (string)dgvMergeFiles.Rows[wRowNo].Cells[0].Value;

                if (wPartName == null)
                {
                    wPartName = "";
                }

                wPartName = wPartName.Trim();
                wPartName = wPartName.Replace(" ", "");
                wPartName = wPartName.ToUpper();
                ///--------------///
                ///
                ///title--------------///
                if (wPartName == "PARTNAME")
                {

                    dgvMergeFiles.Rows[wRowNo].Cells[4].Value = "Sample Description";
                    dgvMergeFiles.Rows[wRowNo].Cells[5].Value = "Part Name";
                    dgvMergeFiles.Rows[wRowNo].Cells[6].Value = "Material No";
                    dgvMergeFiles.Rows[wRowNo].Cells[7].Value = "Report No";
                    dgvMergeFiles.Rows[wRowNo].Cells[8].Value = "Issued Date";
                    dgvMergeFiles.Rows[wRowNo].Cells[9].Value = "Received Date";
                    dgvMergeFiles.Rows[wRowNo].Cells[10].Value = "Aurora ItemNo";
                    dgvMergeFiles.Rows[wRowNo].Cells[11].Value = "Report File";
                    dgvMergeFiles.Rows[wRowNo].Cells[12].Value = "Sample No";
                }

                ///--------------///
                // Get slim data
                string wMaterial = (string)dgvValues.Rows[row1.Index].Cells[1].Value;
                string wMaterialName = (string)dgvValues.Rows[row1.Index].Cells[2].Value;

                if (wMaterial == null)
                {
                    wMaterial = "";
                }

                if (wMaterialName == null)
                {
                    wMaterialName = "";
                }

                wMaterial = wMaterial.Trim();
                wMaterialName = wMaterialName.Trim();

                if (wMaterial != "" & wMaterialName != "")
                {
                    //string query = "select distinct isnull(f1.sam_description,'') as sam_description, isnull(a1.pro_proj,'') as pro_proj, " +
                    //             "       a1.finalised, convert(nvarchar,a1.received,102), isnull(b1.jobcomments,''), convert(nvarchar, a1.required, 102),  c1.sampleident  " +
                    //             "  from  " +
                    //             "        (select distinct top 1 a9.labcode, a9.pro_job, a9.pro_proj, a9.jobcomments, a9.finalised, a9.received, a9.required from " +
                    //             "        (select distinct top 1 a.labcode, a.pro_job, a.pro_proj, isnull(b.jobcomments,'') jobcomments, " +
                    //             "              a.finalised, a.received, a.required " +
                    //             "           from profjob_aurora_20190108 a inner join profjobuser_aurora_20190108 b on (a.labcode = b.labcode and a.pro_job = b.pro_job and a.pro_proj like 'ayh%') " +
                    //             "                        inner join profjob_scheme_aurora_NEWSCHEME_2019 c on (a.labcode = c.labcode and a.pro_job = c.pro_job and a.pro_proj like 'ayh%') " +
                    //             "          where (c.sch_code = 'hceeenicp_15' or c.sch_code = 'hceeenicpms_02' or c.sch_code = 'hceeenicpms_03')  " + "\r" + "\n" +
                    //             "            and a.pro_job like 'ayn%' " +
                    //             "            and a.pro_proj like 'ayh%' " +
                    //             "            and ','+ replace(b.jobcomments,' ','') + ',' like '%," + wMaterial.Replace(" ", "") + ",%' " +
                    //             "          order by a.received desc " +
                    //             "         union " +
                    //             "         select distinct top 1 a.labcode, a.pro_job, a.pro_proj, isnull(b.jobcomments,'') jobcomments, " +
                    //             "              a.finalised, a.received, a.required " +
                    //             "           from KRCTS01.DBO.profjob_aurora_20190108 a inner join KRCTS01.DBO.profjobuser_aurora_20190108 b on (a.labcode = b.labcode and a.pro_job = b.pro_job and a.pro_proj like 'ayh%') " +
                    //             "                        inner join KRCTS01.DBO.profjob_scheme_aurora_NEWSCHEME_2019 c on (a.labcode = c.labcode and a.pro_job = c.pro_job and a.pro_proj like 'ayh%') " +
                    //             "          where (c.sch_code = 'hceeenicp_15' or c.sch_code = 'hceeenicpms_02' or c.sch_code = 'hceeenicpms_03')  " + "\r" + "\n" +
                    //             "            and a.pro_job like 'ayn%' " +
                    //             "            and a.cli_code = '1600008533_INE'  and a.pro_proj like 'ayh%' " +
                    //             "            and ','+ replace(b.jobcomments,' ','') + ',' like '%," + wMaterial.Replace(" ", "") + ",%' " +
                    //             "          order by a.received desc " +
                    //             "          ) as a9 " +
                    //             "          order by a9.received desc ) as a1 " +
                    //             "  inner join (select * from profjobuser_aurora_20190108 union select * from KRCTS01.DBO.profjobuser_aurora_20190108) b1 on (a1.labcode = b1.labcode and a1.pro_job = b1.pro_job and a1.pro_proj like 'ayh%') " +
                    //             "  inner join (select * from profjob_cuid_aurora_20190108 union select * from KRCTS01.DBO.profjob_cuid_aurora_20190108) c1 on (a1.labcode = c1.labcode and a1.pro_job = c1.pro_job and a1.pro_proj like 'ayh%') " +
                    //             "  inner join (select * from profjob_cuiduser_aurora_20190108 union select * from KRCTS01.DBO.profjob_cuiduser_aurora_20190108) f1 on (c1.labcode = f1.labcode and c1.pro_job = f1.pro_job and c1.cuid = f1.cuid) " +
                    //             "  where a1.pro_job like 'ayn%'" +
                    //             "    and a1.pro_proj like 'ayh%'" +
                    //             "  order by c1.sampleident ";

                    string query = "SELECT DISTINCT Isnull(f1.sam_description,'') AS sam_description, " +
                                 "                Isnull(a1.pro_proj,'')        AS pro_proj, " +
                                 "                a1.finalised, " +
                                 "                CONVERT(NVARCHAR,a1.received,102), " +
                                 "                Isnull(b1.jobcomments,''), " +
                                 "                CONVERT(NVARCHAR, a1.required, 102), " +
                                 "                c1.sampleident , c1 .EXTERNALIDENT, a1.completed " +
                                 "FROM            ( " +
                                 "                SELECT DISTINCT TOP 1 a.labcode, " +
                                 "                                a.pro_job, " +
                                 "                                a.pro_proj, " +
                                 "                                Isnull(b.jobcomments,'') jobcomments, " +
                                 "                                a.finalised, " +
                                 "                                a.received, " +
                                 "                                a.completed, " +
                                 "                                a.required " +
                                 "                FROM            profjob_aurora a  with (nolock) " +
                                 "                INNER JOIN      profjobuser_aurora b  with (nolock) " +
                                 "                ON              ( " +
                                 "                                                a.labcode = b.labcode " +
                                 "                                AND             a.pro_job = b.pro_job " +
                                 "                                ) " +
                                 "                INNER JOIN      profjob_scheme_aurora_NEWSCHEME_2019 c with (nolock) " +
                                 "                ON              ( " +
                                 "                                                a.labcode = c.labcode " +
                                 "                                AND             a.pro_job = c.pro_job " +
                                 "                                ) " +
                                 "                WHERE          " +
                                 "					            ','+ replace(replace(replace(b.jobcomments,char(13),''),char(10),''),' ', '') + ',' LIKE '%," + wMaterial.Replace(" ", "") + ",%' " +
                                 "                ORDER BY        a.received DESC, a.pro_job DESC ) AS a1 " +
                                 "INNER JOIN " +
                                 "                ( " +
                                 "                       SELECT * FROM   profjobuser_aurora with (nolock) " +
                                 "                       UNION " +
                                 "                       SELECT * FROM   profjobuser_aurora with (nolock)  ) b1 " +
                                 "ON              ( " +
                                 "                                a1.labcode = b1.labcode " +
                                 "                AND             a1.pro_job = b1.pro_job " +
                                 "                ) " +
                                 "INNER JOIN " +
                                 "                ( " +
                                 "                       SELECT * FROM   profjob_cuid_aurora with (nolock) " +
                                 "                       UNION " +
                                 "                       SELECT * FROM   profjob_cuid_aurora with (nolock) ) c1 " +
                                 "ON              ( " +
                                 "                                a1.labcode = c1.labcode " +
                                 "                AND             a1.pro_job = c1.pro_job " +
                                 "                ) " +
                                 "INNER JOIN " +
                                 "                ( " +
                                 "                       SELECT * FROM   profjob_cuiduser_aurora with (nolock) " +
                                 "                       UNION " +
                                 "                       SELECT * FROM   profjob_cuiduser_aurora with (nolock) ) f1 " +
                                 "ON              ( " +
                                 "                                c1.labcode = f1.labcode " +
                                 "                AND             c1.pro_job = f1.pro_job " +
                                 "                AND             c1.cuid = f1.cuid)  " +
                                 "ORDER BY        c1.sampleident ";


                    OleDbCommand cmd = new OleDbCommand(query, Conn);

                    Conn.Open();
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        string wAuroraItemName;
                        CompareInfo myComp = CultureInfo.InvariantCulture.CompareInfo;

                        bool b, c;
                        int c1;

                        c1 = 0;
                        wAuroraItemName = "";

                        while (reader.Read())
                        {
                            if ((string)reader.GetValue(4) == null)
                            {
                                wAuroraItemName = "";
                            }
                            else
                            {
                                wAuroraItemName = (string)reader.GetValue(4);
                            }

                            if (wAuroraItemName.Length > 0)
                            {
                                b = wAuroraItemName.Contains("ITEM NO");
                                c = wAuroraItemName.Contains("2.AURORA");
                                if (c == true)
                                {
                                    c1 = myComp.IndexOf(wAuroraItemName, "2.AURORA");
                                }
                                if (b == true & c1 > 14)
                                {
                                    wAuroraItemName = wAuroraItemName.Substring(12, c1 - 15);
                                }
                            }
                            wAuroraItemName = wAuroraItemName.Replace("\n", "");

                            //string query = "select distinct isnull(f1.sam_description,'') as sam_description, isnull(a1.pro_proj,'') as pro_proj, " +
                            //             "       a1.finalised, a1.received, isnull(b1.jobcomments,''), a1.lastreported,  c1.sampleident  " +
                            //dgvMergeFiles.Rows[wRowNo].Cells[4].Value = "Sample Description";
                            //dgvMergeFiles.Rows[wRowNo].Cells[5].Value = "Part Name";
                            //dgvMergeFiles.Rows[wRowNo].Cells[6].Value = "Material No";
                            //dgvMergeFiles.Rows[wRowNo].Cells[7].Value = "Report No";
                            //dgvMergeFiles.Rows[wRowNo].Cells[8].Value = "Issued Date";
                            //dgvMergeFiles.Rows[wRowNo].Cells[9].Value = "Received Date";
                            //dgvMergeFiles.Rows[wRowNo].Cells[10].Value = "Aurora ItemNo";
                            //dgvMergeFiles.Rows[wRowNo].Cells[11].Value = "Report File";


                            dgvMergeFiles.Rows[wRowNo].Cells[0].Value = dgvValues.Rows[row1.Index].Cells[0].Value;
                            dgvMergeFiles.Rows[wRowNo].Cells[1].Value = dgvValues.Rows[row1.Index].Cells[1].Value;
                            dgvMergeFiles.Rows[wRowNo].Cells[2].Value = dgvValues.Rows[row1.Index].Cells[2].Value;
                            dgvMergeFiles.Rows[wRowNo].Cells[3].Value = dgvValues.Rows[row1.Index].Cells[3].Value.ToString().Trim();
                            dgvMergeFiles.Rows[wRowNo].Cells[4].Value = (string)reader.GetValue(0);                 //"Sample Description";
                            dgvMergeFiles.Rows[wRowNo].Cells[5].Value = dgvMergeFiles.Rows[wRowNo].Cells[0].Value;  //"Part Name"
                            dgvMergeFiles.Rows[wRowNo].Cells[6].Value = dgvMergeFiles.Rows[wRowNo].Cells[1].Value;  //"Material No"
                            dgvMergeFiles.Rows[wRowNo].Cells[7].Value = (string)reader.GetValue(1);                 //"Report No"
                            dgvMergeFiles.Rows[wRowNo].Cells[8].Value = reader.GetValue(5);                         //"Issued Date";
                            dgvMergeFiles.Rows[wRowNo].Cells[9].Value = reader.GetValue(3);                         //"Received Date";
                            dgvMergeFiles.Rows[wRowNo].Cells[10].Value = wAuroraItemName;                           //"Aurora ItemNo";
                            dgvMergeFiles.Rows[wRowNo].Cells[11].Value = "KRCTS01";                                 //"Aurora ItemNo";
                            dgvMergeFiles.Rows[wRowNo].Cells[12].Value = reader.GetValue(7);


                            if ((string)dgvMergeFiles.Rows[wRowNo].Cells[3].Value != (string)dgvMergeFiles.Rows[wRowNo].Cells[7].Value)
                            {
                                dgvMergeFiles.Rows[wRowNo].Cells[7].Style.BackColor = Color.Red;
                            }
                            else
                            {
                                dgvMergeFiles.Rows[wRowNo].Cells[7].Style.BackColor = Color.White;
                            }
                            
                            if (!wMaterial.Equals("Materials"))
                            {
                                string test = reader.GetValue(8).ToString();

                                // COMPLETED가 NULL이거나 빈값인 경우 공백만 있는 경우, 1899년도인 경우 LightGreen
                                if (string.IsNullOrEmpty(test))
                                {
                                    dgvMergeFiles.Rows[wRowNo].Cells[10].Style.BackColor = Color.LightGreen;
                                }
                                else if (string.IsNullOrWhiteSpace(test))
                                {
                                    dgvMergeFiles.Rows[wRowNo].Cells[10].Style.BackColor = Color.LightGreen;
                                }
                                else if (test.Substring(0, 4).Equals("1899"))
                                {
                                    dgvMergeFiles.Rows[wRowNo].Cells[10].Style.BackColor = Color.LightGreen;
                                }
                            }
                            

                            //발행일이 9개월 전이면 색변환 or 발행일이 2021.09.15 이전이면 색변환
                            DateTime dt1 = Convert.ToDateTime(dgvMergeFiles.Rows[wRowNo].Cells[9].Value);
                            DateTime dt2 = DateTime.Today;

                            int Diffmonth = 12 * (dt2.Year - dt1.Year) + (dt2.Month - dt1.Month);

                            if (Diffmonth >= 8)
                            {
                                dgvMergeFiles.Rows[wRowNo].Cells[9].Style.BackColor = Color.Yellow;
                            }
                            else
                            {
                                if (dt1 < Convert.ToDateTime("2021.09.15"))
                                {
                                    dgvMergeFiles.Rows[wRowNo].Cells[9].Style.BackColor = Color.Yellow;
                                }
                                else
                                {
                                    dgvMergeFiles.Rows[wRowNo].Cells[9].Style.BackColor = Color.White;
                                }
                            }

                            wRowNo = wRowNo + 1;
                            wEof = 1;

                        }
                        if (wEof == 1)
                        {
                            wRowNo = wRowNo - 1;
                            wEof = 0;
                        }
                    }
                    Conn.Close();

                    //progressBar1.Value = row1.Index;

                }
                ///------////
            }
            dgvMergeFiles.RowCount = wRowNo + 1;
            progressBar1.Value = progressBar1.Maximum;

        }

        private void btn_DBSearch_Click(object sender, EventArgs e)
        {
            Find_Slim_KRCTS01();
            MessageBox.Show("SLIMDB검색완료");
        }

        private void btn_DBResult_Click(object sender, EventArgs e)
        {
            Result_Merge_By_DB();
            MessageBox.Show("DB결과취합완료");
        }

        private void setValue(int startX, int startY, int index, string resValue)
        {
            dgvFinal.Rows[startY + index].Cells[startX].Value = resValue;
        }

        private String getValue(OleDbDataReader reader, int startX, int startY, int index)
        {

            String resValue = "";
            if (reader.GetValue(6) != null)
            {

                //N.A.
                resValue = reader.GetValue(6).ToString();
                if (resValue.Contains("<")) resValue = "ND";
                else if (resValue.Contains("N.A.")) resValue = "";
                else if (resValue == "")
                {
                    if (reader.GetValue(4) != null)
                    {
                        String finalValue = reader.GetValue(4).ToString();
                        if (finalValue == "0") resValue = "ND";
                        else resValue = finalValue;
                    }
                }
                dgvFinal.Rows[startY + index].Cells[startX].Value = resValue;
            }



            return resValue;
        }

        private void Result_Merge_By_DB()
        {

            int startY = 0;
            int startX = 3;

            int startPreY = 0;
            int startPreX = 1;

            //dgvFinal.Rows.Clear();

            //dgvFinal.RowCount = 1000;
            //dgvFinal.ColumnCount = 100;

            String itemNumber = "";
            String productDesc = "";
            String resultString = "";


            int bomCount = 0;
            //int resultX = startX + 1;

            //start - get first bom



            for (int i = startPreY; i < dgvMergeFiles.Rows.Count; i++)
            {


                if ((dgvMergeFiles.Rows[i].Cells.Count > 10) && (dgvMergeFiles.Rows[i].Cells[11].Value != null))
                {
                    String title = dgvMergeFiles.Rows[i].Cells[11].Value.ToString();

                    if (title.Contains("Item Number"))
                    {
                        itemNumber = dgvMergeFiles.Rows[i].Cells[12].Value.ToString();

                        dgvFinal.Rows[startY].Cells[startX].Value = itemNumber;
                        resultString = resultString + "/ Y :" + startY + "/ X :" + (int)(startX) + " / value : " + itemNumber + "\n";

                        //}

                        //if (title.Contains("Product"))
                        //{

                        if (dgvMergeFiles.Rows[i + 1].Cells[12].Value != null)
                        {
                            productDesc = dgvMergeFiles.Rows[i + 1].Cells[12].Value.ToString();
                            dgvFinal.Rows[startY + 1].Cells[startX].Value = productDesc;
                            resultString = resultString + "/ Y :" + (int)(startY + 1) + "/ X :" + (int)(startX) + " / value : " + itemNumber + "\n";
                        }
                        i = i + 2;
                        startX = startX + 1;
                    }

                }


                // item data list start--------------------//


                if ((dgvMergeFiles.Rows[i].Cells.Count > 11) && (dgvMergeFiles.Rows[i].Cells[0].Value != null))
                {

                    String subTitle = dgvMergeFiles.Rows[i].Cells[0].Value.ToString();
                    if (subTitle.Contains("PartName"))
                    {
                        int rowNo = i + 2;
                        bomCount = 0;

                        if (dgvMergeFiles.Rows[rowNo].Cells[0].Value != null)
                        {
                            String partName = dgvMergeFiles.Rows[rowNo].Cells[0].Value.ToString();


                            while (partName != "")
                            {
                                DataGridViewRow row = dgvMergeFiles.Rows[rowNo];
                                partName = (row.Cells[0].Value != null) ? row.Cells[0].Value.ToString() : "";
                                if (partName.Contains("Item Number") || row.Cells[startPreX + 0].Value == null)
                                {
                                    //resultX = resultX + bomCount + 1;
                                    resultString = resultString + "/ Y :" + (int)(rowNo) + "/ X :" + (int)(startPreX + 3) + " / value :  break \n";
                                    i++;
                                    break;
                                }

                                String materials = (row.Cells[startPreX + 0].Value != null) ? row.Cells[startPreX + 0].Value.ToString() : "";
                                String material = (row.Cells[startPreX + 1].Value != null) ? row.Cells[startPreX + 1].Value.ToString() : "";
                                String sampleDesc = (row.Cells[startPreX + 3].Value != null) ? row.Cells[startPreX + 3].Value.ToString() : "";
                                String rowPartName = (row.Cells[startPreX + 4].Value != null) ? row.Cells[startPreX + 4].Value.ToString() : "";
                                String materialNo = (row.Cells[startPreX + 5].Value != null) ? row.Cells[startPreX + 5].Value.ToString() : "";
                                String jobNo = (row.Cells[startPreX + 6].Value != null) ? row.Cells[startPreX + 6].Value.ToString() : "";
                                String issueDate = (row.Cells[startPreX + 7].Value != null) ? row.Cells[startPreX + 7].Value.ToString() : "";
                                String receiveDate = (row.Cells[startPreX + 8].Value != null) ? row.Cells[startPreX + 8].Value.ToString() : "";
                                String jobDesc = (row.Cells[startPreX + 9].Value != null) ? row.Cells[startPreX + 9].Value.ToString() : "";
                                String samNO = (row.Cells[startPreX + 11].Value != null) ? row.Cells[startPreX + 11].Value.ToString() : "";




                                //---결과값 찍기 시작--//

                                //dgvFinal.Rows[startY + 8].Cells[startX].Value = SampleNo 입력해야함
                                string connect_string2 ="Provider=SQLOLEDB.1;User ID=Aurora;pwd=Aurora2020;data source=krslimdb001;Persist Security Info=True;initial catalog=Aurora";

                                Conn = new OleDbConnection(connect_string2);

                                //if (jobNo != ""){ -start
                                if (jobNo != "")
                                {

                                    dgvFinal.Rows[startY + 1].Cells[startX].Value = jobNo;
                                    dgvFinal.Rows[startY + 2].Cells[startX].Value = jobDesc;
                                    dgvFinal.Rows[startY + 3].Cells[startX].Value = issueDate;
                                    dgvFinal.Rows[startY + 4].Cells[startX].Value = material;
                                    dgvFinal.Rows[startY + 6].Cells[startX].Value = rowPartName;
                                    dgvFinal.Rows[startY + 7].Cells[startX].Value = materials;
                                    dgvFinal.Rows[startY + 9].Cells[startX].Value = sampleDesc;
                                    dgvFinal.Rows[startY + 8].Cells[startX].Value = bomCount + 1;
                                    //dgvFinal.Rows[startY + 8].Cells[startX].Value = samNO;


                                    //발행일이 9개월 전이면 색 변환?
                                    if (issueDate != "" && issueDate != null)
                                    {
                                        if (Convert.ToDateTime(issueDate).AddMonths(9) <= DateTime.Today)
                                        {
                                            dgvFinal.Rows[startY + 3].Cells[startX].Style.BackColor = Color.Red;
                                            //dgvFinal.Rows[startY + 3].Cells[startX].Style.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))); // 글꼴 굵게 표시 
                                        }
                                        else
                                        {
                                            dgvFinal.Rows[startY + 3].Cells[startX].Style.BackColor = Color.White;
                                        }
                                    }

                                    string query_sample = " select  TOP 1" +
                                                          " '" + jobNo + "', pro_job, EXTERNALIDENT, 'MG' as ANALYTECODE, description_4 as FINALVALUE " +
                                                          " ,ROUNDEDVALUE, FORMATTEDVALUE, SCH_CODE, description_4 from AURORA_RESULT_NEWSCHEME_2019 with(nolock) " +
                                                          " where EXTERNALIDENT = '" + samNO + "'and pro_proj = '" + jobNo + "'" +
                                                          " UNION ALL" +
                                                          " select  " +
                                                         " '" + jobNo + "', pro_job, EXTERNALIDENT, ANALYTECODE, FINALVALUE,ROUNDEDVALUE, FORMATTEDVALUE, SCH_CODE, description_4 from AURORA_RESULT_NEWSCHEME_2019 with(nolock)" +
                                        //1102jhm  " '" + jobNo + "', pro_job, EXTERNALIDENT, ANALYTECODE, FORMATTEDVALUE as FINALVALUE ,ROUNDEDVALUE, FORMATTEDVALUE, SCH_CODE, description_4 from AURORA_RESULT_NEWSCHEME_2019 with(nolock)" + // FINALVALUE 값 대신 FORMATTEDVALUE 값을 대신 사용 
                                                          " where EXTERNALIDENT = '" + samNO + "'and pro_proj = '" + jobNo + "'";

                                    if (sampleDesc != "" && sampleDesc != null)
                                    {
                                        query_sample += "and sam_description ='" + sampleDesc + "'";
                                    }


                                    OleDbCommand cmd_result = new OleDbCommand(query_sample, Conn);
                                    Conn.Open();
                                    String valueCR = "";
                                    String valueCR_3 = "";
                                    String valueCR_6 = "";
                                    String valueCR_3_2 = "";
                                    String valueCR_6_2 = "";
                                    String valueSN = "";

                                    String valueDMT = "";
                                    String valueMET = "";
                                    String valueDPROT = "";
                                    String valueBUT = "";
                                    String valueDBT = "";
                                    String valueTBT = "";
                                    String valueMOT = "";
                                    String valueTEBT = "";
                                    String valueDPHT = "";
                                    String valueDOT = "";
                                    String valueTPHT = "";



                                    //Conn.Open();
                                    using (OleDbDataReader reader = cmd_result.ExecuteReader())
                                    {

                                        while (reader.Read())
                                        {


                                            //dgvFinal.Rows[startY + 7].Cells[startX].Value = materials;
                                            //10:(MG)
                                            if ((string)reader.GetValue(3) == "MG")
                                            {
                                                if (reader.GetValue(8) != null)
                                                {
                                                    String resValue = reader.GetValue(8).ToString();
                                                    dgvFinal.Rows[startY + 10].Cells[startX].Value = resValue;

                                                    if (resValue == null || resValue == "")
                                                    {
                                                        dgvFinal.Rows[startY + 10].Cells[startX].Value = "--";
                                                    }
                                                }

                                            }
                                            //11:(Al)

                                            if ((string)reader.GetValue(3) == "AL") getValue(reader, startX, startY, 11);
                                            //12:(Sb)
                                            if ((string)reader.GetValue(3) == "SB") getValue(reader, startX, startY, 12);
                                            //13:(As)
                                            if ((string)reader.GetValue(3) == "AS") getValue(reader, startX, startY, 13);
                                            //14:(Ba)
                                            if ((string)reader.GetValue(3) == "BA") getValue(reader, startX, startY, 14);
                                            //15:(B)
                                            if ((string)reader.GetValue(3) == "B") getValue(reader, startX, startY, 15);
                                            //16:(Cd)
                                            if ((string)reader.GetValue(3) == "CD") getValue(reader, startX, startY, 16);
                                            //17:(Cr)
                                            if ((string)reader.GetValue(3) == "CR") valueCR = getValue(reader, startX, startY, 17);
                                            //18:(Cr (III))
                                            if ((string)reader.GetValue(3) == "CR(III)")
                                            {
                                                //2021.09.29 -필요없는 부분으로 판단됨 - valueCR_6, valueCR_6_2 가 사용되지 않음
                                                ////String val = getValue(reader, startX, startY, 18);
                                                ////if ((string)reader.GetValue(7) == "HCEEENICP_15") valueCR_3 = val;
                                                ////if ((string)reader.GetValue(7) == "HCEEENICUV_03") valueCR_3_2 = val;
                                                getValue(reader, startX, startY, 18);
                                                                                            }
                                            //19:(Cr (VI))
                                            if ((string)reader.GetValue(3) == "CR(VI)")
                                            {
                                                //2021.09.29 -필요없는 부분으로 판단됨 - valueCR_6, valueCR_6_2 가 사용되지 않음
                                                ////String val = getValue(reader, startX, startY, 19);
                                                ////if ((string)reader.GetValue(7) == "HCEEENICP_15") valueCR_6 = val;
                                                ////if ((string)reader.GetValue(7) == "HCEEENICUV_03") valueCR_6_2 = val;
                                                getValue(reader, startX, startY, 19);
                                            }
                                            //20:(Co)
                                            if ((string)reader.GetValue(3) == "CO") getValue(reader, startX, startY, 20);
                                            //21:(Cu)
                                            if ((string)reader.GetValue(3) == "CU") getValue(reader, startX, startY, 21);
                                            //22:(Pb)
                                            if ((string)reader.GetValue(3) == "PB") getValue(reader, startX, startY, 22);
                                            //23:(Mn)
                                            if ((string)reader.GetValue(3) == "MN") getValue(reader, startX, startY, 23);
                                            //24:(Hg)
                                            if ((string)reader.GetValue(3) == "HG") getValue(reader, startX, startY, 24);
                                            //25:(Ni)
                                            if ((string)reader.GetValue(3) == "NI") getValue(reader, startX, startY, 25);
                                            //26:(Se)
                                            if ((string)reader.GetValue(3) == "SE") getValue(reader, startX, startY, 26);
                                            //27:(Sr)
                                            if ((string)reader.GetValue(3) == "SR") getValue(reader, startX, startY, 27);
                                            //28:(Sn)
                                            if ((string)reader.GetValue(3) == "SN") valueSN = getValue(reader, startX, startY, 28);
                                            //--
                                            //30:(Zn)
                                            if ((string)reader.GetValue(3) == "ZN") getValue(reader, startX, startY, 30);


                                            //33:(DMT)
                                            if ((string)reader.GetValue(3) == "DMT") valueDMT = getValue(reader, startX, startY, 33);

                                            //34:(MeT)
                                            if ((string)reader.GetValue(3) == "MET") valueMET = getValue(reader, startX, startY, 34);

                                            //35:(DProT)
                                            if ((string)reader.GetValue(3) == "DPROT") valueDPROT = getValue(reader, startX, startY, 35);

                                            //36:(MBT)
                                            if ((string)reader.GetValue(3) == "MBT") valueBUT = getValue(reader, startX, startY, 36);
                                            
                                            //37:(DBT)
                                            if ((string)reader.GetValue(3) == "DBT") valueDBT = getValue(reader, startX, startY, 37);
                                           
                                            //38:(TBT)
                                            if ((string)reader.GetValue(3) == "TBT") valueTBT = getValue(reader, startX, startY, 38);
                                            
                                            //39:(MOT)
                                            if ((string)reader.GetValue(3) == "MOT") valueMOT = getValue(reader, startX, startY, 39);
                                            
                                            //40:(TeBT)
                                            if ((string)reader.GetValue(3) == "TEBT") valueTEBT = getValue(reader, startX, startY, 40);
                                           
                                            //41:(DPhT)
                                            if ((string)reader.GetValue(3) == "DPHT") valueDPHT = getValue(reader, startX, startY, 41);
                                            
                                            //42:(DOT)
                                            if ((string)reader.GetValue(3) == "DOT") valueDOT = getValue(reader, startX, startY, 42);
                                            
                                            //43:(TPhT)
                                            if ((string)reader.GetValue(3) == "TPHT") valueTPHT = getValue(reader, startX, startY, 43);

                                        }


                                        //4. (Cr)에서 ND면 (Cr (III)), (Cr (VI))는 "--"로 표기함
                                        //4-1. CR이 있을 경우는 하단에 있는 Cr (iii), Cr(6)결과값을 위에 올려서 표기함
                                        //AYHA18-00301의 경우  CR값이 있는 경우 스킴이 다른
                                        //스킴포드가HCEEENICUV_03 인값으로가져와  Cr (iii), Cr(6)로 표기함



                                        ///jhm ----------------
                                        //if (valueCR == "ND")
                                        //{
                                        //    //4. (Cr)에서 ND면 (Cr (III)), (Cr (VI))는 "--"로 표기함
                                        //    setValue(startX, startY, 18, "--"); //18:(Cr (III))
                                        //    setValue(startX, startY, 19, "--"); //19:(Cr (VI))
                                        //}
                                        //else
                                        //{

                                        //    setValue(startX, startY, 18, valueCR_3_2); //18:(Cr (III))
                                        //    setValue(startX, startY, 19, valueCR_6_2); //19:(Cr (VI))
                                        //}
                                        ///jhm ----------------
                                 


                                        //5-1. Soluble Tin이 ND면 Soluble Organic Tin^가 "--"
                                        //5-2. Soluble Tin이 검출값이 있으면 Organotin Results이 있는 경우 해당 10개의 함계를 상단에 Soluble Organic Tin^ 결과값에 들어가야함, 모두 ND면 그냥 ND

                                        if (valueSN == "ND" || valueSN == "")
                                        {
                                            setValue(startX, startY, 29, "--"); //Soluble Organic Tin^가 "--"
                                        }
                                        else
                                        {
                                            float sum = 0;
                                            bool allNDFlag = true;


                                            if (valueDMT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueDMT); }
                                            if (valueMET != "ND") { allNDFlag = false; sum = sum + float.Parse(valueMET); }
                                            if (valueDPROT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueDPROT); }
                                            if (valueBUT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueBUT); }
                                            if (valueDBT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueDBT); }
                                            if (valueTBT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueTBT); }
                                            if (valueMOT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueMOT); }
                                            if (valueTEBT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueTEBT); }
                                            if (valueDPHT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueDPHT); }
                                            if (valueDOT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueDOT); }
                                            if (valueTPHT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueTPHT); }


                                            if (allNDFlag) setValue(startX, startY, 29, "ND");
                                            else setValue(startX, startY, 29, sum.ToString());
                                        }


                                    }

                                    resultString = resultString + "/ Y :" + (int)(startY + 1) + "/ X :" + (int)(startX) + " / value : " + jobNo + "\n";
                                    startX = startX + 1;
                                    //---결과값 찍기 끝--//
                                    //i++;
                                    //rowNo++;
                                    //bomCount++;


                                    /* jhm 
                                    string query = " select distinct c.EXTERNALIDENT "+
                                    " from profjob_aurora_20190108 a inner join profjob_cuid_aurora_20190108 c on (a.labcode = c.labcode and a.pro_job = c.pro_job) " +
                                    " inner join PROFJOB_CUIDUSER_aurora_20190108 b on (c.labcode = b.labcode and c.pro_job = b.pro_job and c.cuid = b.cuid) " +
                                    " where b.pro_job = (select max(pro_job) from profjob_aurora_20190108 where cli_code = '1600008533_INE' and  pro_proj = '" + jobNo + "')";
                                                           //" union " +
                                           //"select EXTERNALIDENT  from KRCTS01_BAK.dbo.PROFJOB_CUID   " +
                                           //" where pro_job = (select max(pro_job) from profjob where pro_proj = '" + jobNo + "')";
                                            
                                    

                                    OleDbCommand cmd = new OleDbCommand(query, Conn);
                                    Conn.Open();
                                    using (OleDbDataReader reader_sample = cmd.ExecuteReader()) {

                                        // while - reader_sample.read - start
                                        while (reader_sample.Read())
                                        {

                                            // if (reader_sample.GetValue(0) != null) - start
                                            if (reader_sample.GetValue(0) != null)
                                            {

                                                dgvFinal.Rows[startY + 1].Cells[startX].Value = jobNo;
                                                dgvFinal.Rows[startY + 2].Cells[startX].Value = jobDesc;
                                                dgvFinal.Rows[startY + 3].Cells[startX].Value = issueDate;
                                                dgvFinal.Rows[startY + 4].Cells[startX].Value = material;
                                                dgvFinal.Rows[startY + 6].Cells[startX].Value = rowPartName;

                                                dgvFinal.Rows[startY + 7].Cells[startX].Value = materials;

                                                dgvFinal.Rows[startY + 9].Cells[startX].Value = sampleDesc;
                                                String SampleNo = reader_sample.GetValue(0).ToString();
                                                //MessageBox.Show(jobNo + " : " + SampleNo);
                                                //////
                                                // 값 가져와서 값 찍기
                                                dgvFinal.Rows[startY + 8].Cells[startX].Value = SampleNo;


                                                //string query_sample = " select  " +
                                                //                " '" + jobNo + "',  " +
                                                //                " A.pro_job,   " +
                                                //                " A.EXTERNALIDENT,   " +
                                                //                " A.ANALYTECODE,   " +
                                                //                " A.FINALVALUE,  " +
                                                //                " A.ROUNDEDVALUE,  " +
                                                //                " A.FORMATTEDVALUE,  " +
                                                //                " A.SCH_CODE,  " +
                                                //                " C.description_4 " +
                                                //                " from PJCSA_aurora_20190108 A   " +

                                                //                " join profjob_cuid_aurora_20190108 B   " +
                                                //                " on A.pro_job = B.pro_job   " +
                                                //                " and A.EXTERNALIDENT = B.EXTERNALIDENT   " +
                                                //                " join PROFJOB_CUIDUSER_aurora_20190108 C   " +
                                                //                " on A.pro_job = C.pro_job   " +
                                                //                " and B.cuid = c.cuid   " +
                                                //                " where A.sch_code in ('HCEEENICP_15','HCEEENICUV_03','HCEEORGANOTIN_03') and A.EXTERNALIDENT = '" + SampleNo + "' and A.pro_job = (select max(pro_job) from profjob_aurora_20190108 where cli_code = '1600008533_INE' and pro_proj = '" + jobNo + "')   ";




                                                //string query_sample = " select  " +
                                                //                      " '" + jobNo + "', pro_job, EXTERNALIDENT, ANALYTECODE, FINALVALUE,ROUNDEDVALUE, FORMATTEDVALUE, SCH_CODE, description_4 from AURORA_RESULT_NEWSCHEME_2019 " +
                                                //                      " where EXTERNALIDENT = '" + SampleNo + "'and pro_proj = '" + jobNo + "'";                                               

                                                string query_sample = " select  TOP 1" +
                                                                      " '" + jobNo + "', pro_job, EXTERNALIDENT, 'MG' as ANALYTECODE, description_4 as FINALVALUE "+
                                                                      " ,ROUNDEDVALUE, FORMATTEDVALUE, SCH_CODE, description_4 from AURORA_RESULT_NEWSCHEME_2019 with(nolock) " +
                                                                      " where EXTERNALIDENT = '" + SampleNo + "'and pro_proj = '" + jobNo + "'" +
                                                                      " UNION ALL" +

                                                                      " select  " +
                                                                      " '" + jobNo + "', pro_job, EXTERNALIDENT, ANALYTECODE, FINALVALUE,ROUNDEDVALUE, FORMATTEDVALUE, SCH_CODE, description_4 from AURORA_RESULT_NEWSCHEME_2019 with(nolock)" +
                                                                      " where EXTERNALIDENT = '" + SampleNo + "'and pro_proj = '" + jobNo + "'";                                                                                            

                                                
                                                                //" '' EXTERNALIDENT,    " +
                                                                //" 'MG' ANALYTECODE,    " +
                                                                //" '' FINALVALUE,   " +
                                                                //" '' ROUNDEDVALUE,   " +
                                                                //" b.description_4,  " +
                                                                //" '' SCH_CODE  " +
                                                                //" from profjob a inner join profjob_cuid c on (a.labcode = c.labcode and a.pro_job = c.pro_job)    " +
                                                                //" inner join PROFJOB_CUIDUSER b on (c.labcode = b.labcode and c.pro_job = b.pro_job and c.cuid = b.cuid)   " +     
                                                                //" where b.pro_job = (select max( pro_job ) from profjob where cli_code = '1600008533_INE' and  pro_proj = '" + jobNo + "')   ";


                                                OleDbCommand cmd_result = new OleDbCommand(query_sample, Conn);
                                                String valueCR = "";
                                                String valueCR_3 = "";
                                                String valueCR_6 = "";
                                                String valueCR_3_2 = "";
                                                String valueCR_6_2 = "";
                                                String valueSN = "";

                                                String valueMET = "";
                                                String valueDPROT = "";
                                                String valueBUT = "";
                                                String valueDBT = "";
                                                String valueTBT = "";
                                                String valueMOT = "";
                                                String valueTEBT = "";
                                                String valueDPHT = "";
                                                String valueDOT = "";
                                                String valueTPHT = "";
                                                
 

                                                //Conn.Open();
                                                using (OleDbDataReader reader = cmd_result.ExecuteReader())
                                                {

                                                    while (reader.Read())
                                                    {
                                                        

                                                        //dgvFinal.Rows[startY + 7].Cells[startX].Value = materials;
                                                        //10:(MG)
                                                        if ((string)reader.GetValue(3) == "MG")
                                                        {
                                                            if (reader.GetValue(8) != null)
                                                            {
                                                                String resValue = reader.GetValue(8).ToString();
                                                                dgvFinal.Rows[startY + 10].Cells[startX].Value = resValue;
                                                            }

                                                        }
                                                        //11:(Al)
                                                        
                                                        if ((string)reader.GetValue(3) == "AL") getValue(reader, startX, startY, 11);
                                                        //12:(Sb)
                                                        if ((string)reader.GetValue(3) == "SB") getValue(reader, startX, startY, 12);
                                                        //13:(As)
                                                        if ((string)reader.GetValue(3) == "AS") getValue(reader, startX, startY, 13);
                                                        //14:(Ba)
                                                        if ((string)reader.GetValue(3) == "BA") getValue(reader, startX, startY, 14);
                                                        //15:(B)
                                                        if ((string)reader.GetValue(3) == "B") getValue(reader, startX, startY, 15);
                                                        //16:(Cd)
                                                        if ((string)reader.GetValue(3) == "CD") getValue(reader, startX, startY, 16);
                                                        //17:(Cr)
                                                        if ((string)reader.GetValue(3) == "CR") valueCR = getValue(reader, startX, startY, 17);
                                                        //18:(Cr (III))
                                                        if ((string)reader.GetValue(3) == "CR(III)")
                                                        {                                                            
                                                            String val= getValue(reader, startX, startY, 18);
                                                            if((string)reader.GetValue(7) == "HCEEENICP_15") valueCR_3 = val;
                                                            if((string)reader.GetValue(7) == "HCEEENICUV_03") valueCR_3_2 = val;

                                                        }
                                                        //19:(Cr (VI))
                                                        if ((string)reader.GetValue(3) == "CR(VI)")
                                                        {
                                                            String val = getValue(reader, startX, startY, 19);
                                                            if ((string)reader.GetValue(7) == "HCEEENICP_15") valueCR_6 = val;
                                                            if ((string)reader.GetValue(7) == "HCEEENICUV_03") valueCR_6_2 = val;
                                                        }
                                                        //20:(Co)
                                                        if ((string)reader.GetValue(3) == "CO") getValue(reader, startX, startY, 20);
                                                        //21:(Cu)
                                                        if ((string)reader.GetValue(3) == "CU") getValue(reader, startX, startY, 21);
                                                        //22:(Pb)
                                                        if ((string)reader.GetValue(3) == "PB") getValue(reader, startX, startY, 22);
                                                        //23:(Mn)
                                                        if ((string)reader.GetValue(3) == "MN") getValue(reader, startX, startY, 23);
                                                        //24:(Hg)
                                                        if ((string)reader.GetValue(3) == "HG") getValue(reader, startX, startY, 24);
                                                        //25:(Ni)
                                                        if ((string)reader.GetValue(3) == "NI") getValue(reader, startX, startY, 25);
                                                        //26:(Se)
                                                        if ((string)reader.GetValue(3) == "SE") getValue(reader, startX, startY, 26);
                                                        //27:(Sr)
                                                        if ((string)reader.GetValue(3) == "SR") getValue(reader, startX, startY, 27);
                                                        //28:(Sn)
                                                        if ((string)reader.GetValue(3) == "SN") valueSN = getValue(reader, startX, startY, 28);
                                                        //--
                                                        //30:(Zn)
                                                        if ((string)reader.GetValue(3) == "ZN") getValue(reader, startX, startY, 30);

                                                        //33:(MeT)
                                                        if ((string)reader.GetValue(3) == "MET") valueMET = getValue(reader, startX, startY, 33);
                                                        //34:(DProT)
                                                        if ((string)reader.GetValue(3) == "DPROT") valueDPROT = getValue(reader, startX, startY, 34);
                                                        //if ((string)reader.GetValue(3) == "BUT")
                                                        if ((string)reader.GetValue(3) == "MBT") valueBUT = getValue(reader, startX, startY, 35);
                                                        //36:(DBT)
                                                        if ((string)reader.GetValue(3) == "DBT") valueDBT = getValue(reader, startX, startY, 36);
                                                        //37:(TBT)
                                                        if ((string)reader.GetValue(3) == "TBT") valueTBT = getValue(reader, startX, startY, 37);
                                                        //38:(MOT)
                                                        if ((string)reader.GetValue(3) == "MOT") valueMOT = getValue(reader, startX, startY, 38);
                                                        //39:(TeBT)
                                                        if ((string)reader.GetValue(3) == "TEBT") valueTEBT = getValue(reader, startX, startY, 39);
                                                        //40:(DPhT)
                                                        if ((string)reader.GetValue(3) == "DPHT") valueDPHT = getValue(reader, startX, startY, 40);
                                                        //41:(DOT)
                                                        if ((string)reader.GetValue(3) == "DOT") valueDOT = getValue(reader, startX, startY, 41);
                                                        //42:(TPhT)
                                                        if ((string)reader.GetValue(3) == "TPHT") valueTPHT = getValue(reader, startX, startY, 42);

                                                    }


                                                    //4. (Cr)에서 ND면 (Cr (III)), (Cr (VI))는 "--"로 표기함
                                                    //4-1. CR이 있을 경우는 하단에 있는 Cr (iii), Cr(6)결과값을 위에 올려서 표기함
                                                    //AYHA18-00301의 경우  CR값이 있는 경우 스킴이 다른
                                                    //스킴포드가HCEEENICUV_03 인값으로가져와  Cr (iii), Cr(6)로 표기함


                                                    if (valueCR == "ND")
                                                    {
                                                        //4. (Cr)에서 ND면 (Cr (III)), (Cr (VI))는 "--"로 표기함
                                                        setValue(startX, startY, 18, "--"); //18:(Cr (III))
                                                        setValue(startX, startY, 19, "--"); //19:(Cr (VI))
                                                    }
                                                    else
                                                    {
                                                        
                                                        setValue(startX, startY, 18, valueCR_3_2); //18:(Cr (III))
                                                        setValue(startX, startY, 19, valueCR_6_2); //19:(Cr (VI))
                                                    }

                                                    //5-1. Soluble Tin이 ND면 Soluble Organic Tin^가 "--"
                                                    //5-2. Soluble Tin이 검출값이 있으면 Organotin Results이 있는 경우 해당 10개의 함계를 상단에 Soluble Organic Tin^ 결과값에 들어가야함, 모두 ND면 그냥 ND

                                                    if (valueSN == "ND" || valueSN == "")
                                                    {
                                                        setValue(startX, startY, 29, "--"); //Soluble Organic Tin^가 "--"
                                                    }
                                                    else
                                                    {
                                                        float sum = 0;
                                                        bool allNDFlag = true;

                                                        if (valueMET != "ND") { allNDFlag = false; sum = sum + float.Parse(valueMET); }
                                                        if (valueDPROT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueDPROT); }
                                                        if (valueBUT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueBUT); }
                                                        if (valueDBT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueDBT); }
                                                        if (valueTBT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueTBT); }
                                                        if (valueMOT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueMOT); }
                                                        if (valueTEBT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueTEBT); }
                                                        if (valueDPHT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueDPHT); }
                                                        if (valueDOT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueDOT); }
                                                        if (valueTPHT != "ND") { allNDFlag = false; sum = sum + float.Parse(valueTPHT); }


                                                        if (allNDFlag) setValue(startX, startY, 29, "ND");
                                                        else setValue(startX, startY, 29, sum.ToString());
                                                    }


                                                }

                                                resultString = resultString + "/ Y :" + (int)(startY + 1) + "/ X :" + (int)(startX) + " / value : " + jobNo + "\n";
                                                startX = startX + 1;
                                                //---결과값 찍기 끝--//
                                                //i++;
                                                //rowNo++;
                                                //bomCount++;
                                                ///////
                                            }// if (reader_sample.GetValue(0) != null) - end
                                        }// while - reader_sample.read - end
                                    } --*/
                                    //jhm 

                                }
                                i++;
                                rowNo++;
                                bomCount++;

                            }

                        }

                    }

                }


                //resultX = resultX + bomCount + 1;
                // item data list end----------------------// 


            }


        }
 
        private void button19_Click(object sender, EventArgs e)
        {
            dgvValues.Rows.Clear();
            dgvMergeFiles.Rows.Clear();
            dgvFinal.Rows.Clear();
        }

    }

}