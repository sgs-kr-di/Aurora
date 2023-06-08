using DevExpress.LookAndFeel.Design;
using DevExpress.Utils.CodedUISupport;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Ulee.Utils;

namespace Sgs.ReportIntegration
{
    public enum ELogType { Total, Database }

    public static class AppRes
    {
        //private const string csIniFName = @"..\..\Config\Sgs.ReportIntegration.ini";
        private const string csIniFName = @"C:\\Projects\\Projects\\Sgs\Remote_One\\ReportIntegration\\ReportIntegration\\Config\\Sgs.ReportIntegration.ini";

        public const string csDateFormat = "yyyy-MM-dd";

        public const string csDateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public const string csDateTimeFormatSec = "yyyy-MM-dd HH:mm:ss";

        public const string Caption = "SGS";

        public static bool Busy { get; set; }

        public static string UserId { get; set; }

        public static EReportAuthority Authority { get; set; }

        public static UlIniFile Ini { get; private set; }

        public static UlLogger TotalLog { get; private set; }

        public static UlLogger DbLog { get; private set; }

        public static AppDatabase DB { get; private set; }

        public static AppSettings Settings { get; private set; }

        private static UlStringCrypto crypto;

        public static void Create()
        {
            Program.iSetPhysicalAreano(-999);

            Busy = false;

            crypto = new UlStringCrypto("!rpting@");

            Ini = new UlIniFile(csIniFName);

            TotalLog = new UlLogger();
            //MessageBox.Show(System.Windows.Forms.Application.StartupPath);
            TotalLog.Path = Path.GetFullPath(Ini.GetString("Log", "TotalPath"));
            TotalLog.FName = Ini.GetString("Log", "TotalFileName");

            DbLog = new UlLogger();
            DbLog.Path = Path.GetFullPath(Ini.GetString("Log", "DatabasePath"));
            DbLog.FName = Ini.GetString("Log", "DatabaseFileName");

            DB = new AppDatabase(crypto.Decrypt(Ini.GetString("Database", "ConnectString")));
            DB.Open();

            Settings = new AppSettings();
            TotalLog[ELogTag.Note] = $"Create application resource";

            string folderPath_Integr_astm = "C:\\Projects\\Projects\\Sgs\\Remote_One\\ReportIntegration\\ReportIntegration\\Bom\\ASTM_Integr";
            string folderPath_Integr_en = "C:\\Projects\\Projects\\Sgs\\Remote_One\\ReportIntegration\\ReportIntegration\\Bom\\EN_Integr";
            string folderPath_Physical_astm = "C:\\Projects\\Projects\\Sgs\\Remote_One\\ReportIntegration\\ReportIntegration\\Bom\\ASTM_Physical";
            string folderPath_Physical_en = "C:\\Projects\\Projects\\Sgs\\Remote_One\\ReportIntegration\\ReportIntegration\\Bom\\EN_Physical";
            string folderPath_Physical_astm_pdf = "C:\\Projects\\Projects\\Sgs\\Remote_One\\ReportIntegration\\ReportIntegration\\Bom\\ASTM_Physical_pdf";
            string folderPath_Physical_en_pdf = "C:\\Projects\\Projects\\Sgs\\Remote_One\\ReportIntegration\\ReportIntegration\\Bom\\EN_Physical_pdf";

            DirectoryInfo Path_Integr_Astm = new DirectoryInfo(folderPath_Integr_astm);
            DirectoryInfo Path_Integr_En = new DirectoryInfo(folderPath_Integr_en);
            DirectoryInfo Path_Physical_Astm = new DirectoryInfo(folderPath_Physical_astm);
            DirectoryInfo Path_Physical_En = new DirectoryInfo(folderPath_Physical_en);

            DirectoryInfo Path_Physical_Astm_pdf = new DirectoryInfo(folderPath_Physical_astm_pdf);
            DirectoryInfo Path_Physical_En_pdf = new DirectoryInfo(folderPath_Physical_en_pdf);

            if (Path_Integr_Astm.Exists == false)
            {
                Path_Integr_Astm.Create();
            }

            if (Path_Integr_En.Exists == false)
            {
                Path_Integr_En.Create();
            }

            if (Path_Physical_Astm.Exists == false)
            {
                Path_Physical_Astm.Create();
            }

            if (Path_Physical_En.Exists == false)
            {
                Path_Physical_En.Create();
            }

            if (Path_Physical_Astm_pdf.Exists == false)
            {
                Path_Physical_Astm_pdf.Create();
            }

            if (Path_Physical_En_pdf.Exists == false)
            {
                Path_Physical_En_pdf.Create();
            }
        }

        public static void Destroy()
        {
            if (DB.Connect.State == ConnectionState.Open)
            {
                DB.Close();
            }

            TotalLog[ELogTag.Note] = $"Destroy application resource";
        }
    }

    public class AppSettings
    {
        public string BomPath { get; set; }

        public string FinalPath { get; set; }

        public AppSettings()
        {
            BomPath = AppRes.Ini.GetString("Settings", "BomPath");
            FinalPath = AppRes.Ini.GetString("Settings", "FinalPath");
        }
    }
}
