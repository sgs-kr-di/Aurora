using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Resources;
using Ulee.Database.SqlServer;
using Ulee.Utils;

namespace Sgs.ReportIntegration
{
    public class AppDatabase : UlSqlServer
    {
        public SqlConnection Connect { get { return connect; } }

        public AppDatabase(string connectString = null) : base(connectString)
        {
        }

        public new void Open()
        {
            base.Open();
            AppRes.DbLog[ELogTag.Note] = $"Open MS-SQL Server";
        }

        public new void Close()
        {
            base.Close();
            AppRes.DbLog[ELogTag.Note] = $"Close MS-SQL Server";
        }
    }

    public class BomDataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public DateTime RegTime { get; set; }

        public EReportArea AreaNo { get; set; }

        public string FName { get; set; }

        public string FPath { get; set; }

        public string FullFName
        {
            get 
            {
                string name;

                if ((string.IsNullOrWhiteSpace(FPath) == true) ||
                    (string.IsNullOrWhiteSpace(FName) == true))
                {
                    name = "";
                }
                else
                {
                    name = Path.Combine(FPath, FName);
                }

                return name; 
            }
            set 
            {
                FName = Path.GetFileName(value);
                FPath = Path.GetDirectoryName(value); 
            }
        }

        public string From { get; set; }

        public string To { get; set; }

        public BomDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            string sql = " select * from TB_BOM where pk_recno>0 ";

            if (AreaNo != EReportArea.None)
            {
                sql += $" and areano={(int)AreaNo} ";
            }
            if (string.IsNullOrWhiteSpace(FName) == false)
            {
                sql += $" and fname like '{FName}%%' ";
            }
            if (string.IsNullOrWhiteSpace(From) == false)
            {
                if (From == To)
                {
                    sql += $" and regtime like '{From}%%' ";
                }
                else
                {
                    sql += $" and (regtime>='{From} 00:00:00.000' ";
                    sql += $" and regtime<='{To} 23:59:59.999') ";
                }
            }

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_BOM values " +
                $" ('{RegTime.ToString(AppRes.csDateTimeFormat)}', {(int)AreaNo}, '{FName}', '{FPath}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_BOM " +
                $" where pk_recno={RecNo} ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                RegTime = DateTime.Now;
                AreaNo = EReportArea.US;
                FName = "";
                FPath = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            RegTime = Convert.ToDateTime(row["regtime"]);
            AreaNo = (EReportArea)Convert.ToInt32(row["areano"]);
            FName = Convert.ToString(row["fname"]);
            FPath = Convert.ToString(row["fpath"]);
        }
    }

    public class ProductDataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public Int64 BomNo { get; set; }

        public DateTime RegTime { get; set; }

        public bool Valid { get; set; }

        public EReportArea AreaNo { get; set; }

        public string Code { get; set; }

        public string PhyJobNo { get; set; }

        public string IntegJobNo { get; set; }

        public string Name { get; set; }

        public Bitmap Image { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        private ImageConverter imageConvert;

        public ProductDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
            imageConvert = new ImageConverter();
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select t2.regtime, t1.* from TB_PRODUCT t1" +
                $" join TB_BOM t2 on t2.pk_recno=t1.fk_bomno  " + 
                $" where t1.fk_bomno={BomNo} ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectDetail(SqlTransaction trans = null)
        {
            string sql =
                $" select t2.regtime, t1.* from TB_PRODUCT t1 " +
                $" join TB_BOM t2 on t2.pk_recno=t1.fk_bomno  " +
                $" where t1.valid={Convert.ToInt32(Valid)} ";

            if (string.IsNullOrWhiteSpace(IntegJobNo) == false)
            {
                sql += $" and t1.integjobno='{IntegJobNo}' ";
            }
            else
            {
                if (AreaNo != EReportArea.None)
                {
                    sql += $" and t1.areano={(int)AreaNo} ";
                }
                if (string.IsNullOrWhiteSpace(Code) == false)
                {
                    sql += $" and t1.itemno='{Code}' ";
                }

                sql += $" and t1.phyjobno<>'' and t1.integjobno='' ";
            }

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PRODUCT values " +
                $" ({BomNo}, {Convert.ToInt32(Valid)}, {(int)AreaNo}, " +
                $" '{Code}', '{PhyJobNo}', '{IntegJobNo}', '{Name}', @image);  " +
                $" select cast(scope_identity() as bigint); ";

            byte[] imageRaw = (Image == null) ? null : (byte[])imageConvert.ConvertTo(Image, typeof(byte[]));

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;

                command.Parameters.Clear();
                command.Parameters.Add("@image", SqlDbType.Image);
                command.Parameters["@image"].Value = imageRaw;

                RecNo = (Int64)command.ExecuteScalar();

                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void UpdatePhyJobNoSet(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PRODUCT set phyjobno='{PhyJobNo}' " +
                $" where itemno='{Code}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void UpdatePhyJobNoReset(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PRODUCT set phyjobno='' " +
                $" where phyjobno='{PhyJobNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void UpdateIntegJobNoSet(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PRODUCT set integjobno='{IntegJobNo}' " +
                $" where areano={(int)AreaNo} and itemno='{Code}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void UpdateIntegJobNoReset(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PRODUCT set integjobno='' " +
                $" where integjobno='{IntegJobNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void UpdateValid(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PRODUCT set valid={Convert.ToInt32(Valid)} " +
                $" where pk_recno={RecNo} ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void UpdateValidSet(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PRODUCT set valid=1 " +
                $" where valid=0 and phyjobno<>'' and integjobno<>'' and " +
                $" pk_recno not in (select distinct fk_productno from TB_PART " +
                $" where fk_productno=TB_PRODUCT.pk_recno and jobno='') ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void UpdateValidReset(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PRODUCT set valid=0 " +
                $" where valid=1 and (phyjobno='' or integjobno='' or pk_recno in " +
                $" (select distinct fk_productno from TB_PART  " +
                $" where fk_productno=TB_PRODUCT.pk_recno and jobno='')) ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PRODUCT " +
                $" where fk_bomno={BomNo} ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                RegTime = DateTime.Now;
                BomNo = 0;
                Valid = false;
                AreaNo = EReportArea.None;
                Code = "";
                PhyJobNo = "";
                IntegJobNo = "";
                Name = "";
                Image = null;
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            RegTime = Convert.ToDateTime(row["regtime"]);
            BomNo = Convert.ToInt64(row["fk_bomno"]);
            Valid = Convert.ToBoolean(row["valid"]);
            AreaNo = (EReportArea)Convert.ToInt32(row["areano"]);
            Code = Convert.ToString(row["itemno"]);
            PhyJobNo = Convert.ToString(row["phyjobno"]);
            IntegJobNo = Convert.ToString(row["integjobno"]);
            Name = Convert.ToString(row["name"]);
            byte[] imageRaw = (byte[])row["image"];

            if (imageRaw == null) 
                Image = null;
            else 
                Image = new Bitmap(new MemoryStream(imageRaw));
        }
    }

    public class PartDataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public Int64 ProductNo { get; set; }

        public string JobNo { get; set; }

        public string MaterialNo { get; set; }

        public string Name { get; set; }

        public string MaterialName { get; set; }

        public EReportArea AreaNo { get; set; }

        public PartDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select t1.*, t2.areano from TB_PART t1 " +
                $" join TB_PRODUCT t2 on t2.pk_recno=t1.fk_productno " +
                $" where t1.fk_productno={ProductNo} ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public bool IsAllJobNoValid(SqlTransaction trans = null)
        {
            string sql =
                $" select t1.*, t2.areano from TB_PART t1 " +
                $" join TB_PRODUCT t2 on t2.pk_recno=t1.fk_productno " +
                $" where t1.fk_productno={ProductNo} and t1.jobno='' ";

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.Fill(dataSet);

            return (Empty == true) ? true : false;
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PART values " +
                $" ({ProductNo}, '{JobNo}', '{MaterialNo}', '{Name}', '{MaterialName}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update(EReportArea area, string[] items, SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PART set jobno='' from TB_PRODUCT t1 " +
                $" where t1.pk_recno=TB_PART.fk_productno and t1.areano={(int)area} " +
                $" and TB_PART.jobno<>'' and TB_PART.materialno in ('!@#$%'";

            foreach (string item in items)
            {
                sql += $",'{item.Trim()}'";
            }
            sql += ")";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update(EReportArea area, string[] items, string jobNo, SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PART set jobno='{jobNo}' from TB_PRODUCT t1 " +
                $" where t1.pk_recno=TB_PART.fk_productno and t1.areano={(int)area} " +
                $" and TB_PART.jobno='' and TB_PART.materialno in ('!@#$%'";

            foreach (string item in items)
            {
                sql += $",'{item.Trim()}'";
            }
            sql += ")";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql = 
                $" delete from TB_PART " +
                $" where fk_productno={ProductNo} ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                ProductNo = 0;
                JobNo = "";
                MaterialNo = "";
                Name = "";
                MaterialName = "";
                AreaNo = EReportArea.None;
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            ProductNo = Convert.ToInt64(row["fk_productno"]);
            JobNo = Convert.ToString(row["jobno"]);
            MaterialNo = Convert.ToString(row["materialno"]);
            Name = Convert.ToString(row["name"]);
            MaterialName = Convert.ToString(row["materialname"]);
            AreaNo = (EReportArea)Convert.ToInt32(row["areano"]);
        }
    }

    public class StaffDataSet : UlSqlDataSet
    {
        public string LabNo { get; set; }

        public string StaffNo { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Title { get; set; }

        public string Passwd { get; set; }

        public string FName { get; set; }

        public EReportAuthority Authority { get; set; }

        public Bitmap Signature { get; private set; }

        public StaffDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_STAFF " +
                $" where staff_code='{StaffNo}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_STAFF values (" +
                $" '{LabNo}', '{StaffNo}', '{FirstName}', '{LastName}', '{Title}', " +
                $" '{Passwd}', 0, 0, 0, '', 0, '', '{FName}', '', {(int)Authority}) ";
            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update(EReportArea area, string[] items, SqlTransaction trans = null)
        {
            string sql =
                $" update TB_STAFF set " +
                $" first_name='{FirstName}', last_name='{LastName}', title='{Title}', " +
                $" password='{Passwd}', picturefile=='{FName}', authority={(int)Authority} " +
                $" where staff_code='{StaffNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                LabNo = "";
                StaffNo = "";
                FirstName = "";
                LastName = "";
                Title = "";
                Passwd = "";
                FName = "";
                Authority = EReportAuthority.None;
                Signature = null;
            }
        }

        public void Fetch(DataRow row)
        {
            LabNo = Convert.ToString(row["labcode"]);
            StaffNo = Convert.ToString(row["staff_code"]);
            FirstName = Convert.ToString(row["first_name"]);
            LastName = Convert.ToString(row["last_name"]);
            Title = Convert.ToString(row["title"]);
            Passwd = Convert.ToString(row["password"]);
            FName = Convert.ToString(row["picturefile"]);
            Authority = (EReportAuthority)Convert.ToInt32(row["authority"]);

            if (string.IsNullOrWhiteSpace(FName) == false)
            {
                Signature = new Bitmap(FName);
            }
            else
            {
                Signature = null;
            }
        }
    }

    public class IntegrationReportDataSet : UlSqlDataSet
    {
        public string RecNo { get; set; }

        public string RecNo_Physical { get; set; }

        public string RecNo_Chemical { get; set; }

        public string Pro_proj { get; set; }

        public IntegrationReportDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGMAIN where pk_recno='{RecNo}'; " +
                $" select * from TB_INTEGT1 where fk_integmainno='{RecNo}'; " +
                $" select * from TB_INTEGT2 where fk_integmainno='{RecNo}'; " +
                $" select * from TB_INTEGT3 where fk_integmainno='{RecNo}'; " +
                $" select * from TB_INTEGT4 where fk_integmainno='{RecNo}'; " +
                $" select * from TB_INTEGT5 where fk_integmainno='{RecNo}'; " +
                $" select * from TB_INTEGT6 where fk_integmainno='{RecNo}' and result <> ''; " +
                $" select * from TB_INTEGT7 where fk_integmainno='{RecNo}' and no >= 1 and no <= 15; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}'; " +
                //$" select top 5 * from TB_INTEGRESULT_EN where fk_integmainno='{RecNo}'; " +
                $" select top 5 * from TB_INTEGRESULT_HYPHEN_EN where fk_integmainno='{RecNo}'; " +
                $" select * from TB_INTEGLEADLIMIT_ASTM where fk_integmainno='{RecNo}' and leadtype=1; " +
                $" select * from TB_INTEGLEADRESULT_ASTM where fk_integmainno='{RecNo}' and leadtype=1; " +
                $" select * from TB_INTEGLIMIT_ASTM where fk_integmainno='{RecNo}' and leadtype=1; " +
                $" select * from TB_INTEGRESULT_ASTM where fk_integmainno='{RecNo}' and leadtype=1; " +
                $" select * from TB_INTEGLEADLIMIT_ASTM where fk_integmainno='{RecNo}' and leadtype<>1 and reportcase = 'Plastic'; " +
                $" select * from TB_INTEGLEADRESULT_ASTM where fk_integmainno='{RecNo}' and leadtype<>1 and reportcase = 'Plastic'; " +
                $" select * from TB_INTEGLIMIT_ASTM where fk_integmainno='{RecNo}' and leadtype<>1; " +
                $" select * from TB_INTEGRESULT_ASTM where fk_integmainno='{RecNo}' and leadtype<>1; " +
                $" select * from TB_INTEGIMAGE where pk_recno='{RecNo}'; " +
                // Report 6페이지 이상 출력
                /*
                $" select * from TB_INTEGRESULT_EN where fk_integmainno='{RecNo}' and (no>=6 and no<=10);   " +
                $" select * from TB_INTEGRESULT_EN where fk_integmainno='{RecNo}' and (no>=11 and no<=15);  " +
                $" select * from TB_INTEGRESULT_EN where fk_integmainno='{RecNo}' and (no>=16 and no<=20);  " +
                $" select * from TB_INTEGRESULT_EN where fk_integmainno='{RecNo}' and (no>=21 and no<=25);  " +
                $" select * from TB_INTEGRESULT_EN where fk_integmainno='{RecNo}' and (no>=26 and no<=30);  " +
                */
                $" select * from TB_INTEGRESULT_HYPHEN_EN where fk_integmainno='{RecNo}' and (no>=6 and no<=10);   " +
                $" select * from TB_INTEGRESULT_HYPHEN_EN where fk_integmainno='{RecNo}' and (no>=11 and no<=15);  " +
                $" select * from TB_INTEGRESULT_HYPHEN_EN where fk_integmainno='{RecNo}' and (no>=16 and no<=20);  " +
                $" select * from TB_INTEGRESULT_HYPHEN_EN where fk_integmainno='{RecNo}' and (no>=21 and no<=25);  " +
                $" select * from TB_INTEGRESULT_HYPHEN_EN where fk_integmainno='{RecNo}' and (no>=26 and no<=30);  " +
                // Report 6페이지 이상 출력

                // Report limit  출력 - 시작
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Al)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(As)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(B)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Ba)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Cd)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Co)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Cr)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(III)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(VI)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Cu)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Hg)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Mn)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Ni)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Pb)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Sb)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Se)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Sn)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Sr)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Zn)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%Organic%'; " +
                // Report limit  출력 - 끝

                // ASTM Substrate Lead Metal  출력 - 시작
                $" select * from TB_INTEGLEADLIMIT_ASTM where fk_integmainno='{RecNo}' and leadtype<>1 and reportcase = 'Metal'; " +
                $" select * from TB_INTEGLEADRESULT_ASTM where fk_integmainno='{RecNo}' and leadtype<>1 and reportcase = 'Metal'; " +
                // ASTM Substrate Lead Metal  출력 - 끝

                $" select * from TB_INTEGT61 where fk_integmainno='{RecNo}' and result <> '' ; " +

                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 1 AND 1;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 2 AND 2;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 3 AND 3;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 4 AND 4;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 5 AND 5;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 6 AND 6;" +

                // Report limit  출력 - 시작
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(MET)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(DBT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(TBT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(TeBT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(MOT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(DOT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(DProT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(DPhT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(TPhT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(DMT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(MBT)%'; " +

                $" select * from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] where fk_integmainno='{RecNo}' and (no>=1 and no<=5);   " +
                $" select * from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] where fk_integmainno='{RecNo}' and (no>=6 and no<=10);   " +
                $" select * from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] where fk_integmainno='{RecNo}' and (no>=11 and no<=15);   " +
                $" select * from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] where fk_integmainno='{RecNo}' and (no>=16 and no<=20);   " +
                $" select * from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] where fk_integmainno='{RecNo}' and (no>=21 and no<=25);   " +
                $" select * from TB_INTEGT7 where fk_integmainno='{RecNo}' and no >= 16 and no <= 30; " +
                $" select * from TB_INTEGT2 where (LTRIM(clause) like '4%' or clause = '') and fk_integmainno = '{RecNo}'; " +
                $" select * from TB_INTEGT2 where NOT(LTRIM(clause) like '4%' or clause = '') and fk_integmainno = '{RecNo}'; " +

                // NoCoating_NoLead_Limit
                $" select * from TB_INTEG_LIMIT_ASTM where fk_integmainno = '{RecNo}' and sam_remarks <> 'coating' order by" +
                $" (case when name = 'Pb' then 1 " +
                $" when name = 'Sb' then 2 " +
                $" when name = 'As' then 3 " +
                $" when name = 'Ba' then 4 " +
                $" when name = 'Cd' then 5 " +
                $" when name = 'Cr' then 6 " +
                $" when name = 'Hg' then 7 " +
                $" when name = 'Se' then 8 " +
                $" else 9 end ); " +

                // Report Not Lead Result 출력 - coating인 것과 coating 아닌 것 순으로
                // NoCoating_NoLead_Result
                //$" select * from TB_CHEP2_RESULT_ASTM where pro_proj like '{Pro_proj}%%' and sam_remarks = 'coating';   " +
                $" select * from TB_INTEG_RESULT_ASTM where fk_integmainno = '{RecNo}' and sam_remarks <> 'coating';   " +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEG_PHT_ASTM] WHERE fk_integmainno = '{RecNo}' and sam_remarks = 'plastic') t WHERE t.rownum BETWEEN 1 AND 1;";
            /*
             * 
             *  $" select * from TB_PHYP3 where (LTRIM(clause) like '4%' or clause = '') and fk_phymainno = '{RecNo}'; " +
                $" select * from TB_PHYP3 where NOT(LTRIM(clause) like '4%' or clause = '') and fk_phymainno = '{RecNo}'; ";
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 1 AND 5;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 6 AND 10;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 11 AND 15;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 16 AND 20;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 21 AND 25;"
             */

            dataSet.Clear();
            dataSet.Tables.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_US(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGMAIN where pk_recno='{RecNo}'; " +
                $" select * from TB_INTEGT1 where fk_integmainno='{RecNo}'; " +
                $" select * from TB_INTEGT2 where fk_integmainno='{RecNo}'; " +
                $" select * from TB_INTEGT3 where fk_integmainno='{RecNo}'; " +
                $" select * from TB_INTEGT4 where fk_integmainno='{RecNo}'; " +
                $" select * from TB_INTEGT5 where fk_integmainno='{RecNo}'; " +
                $" select * from TB_INTEGT6 where fk_integmainno='{RecNo}' and result <> ''; " +
                $" select * from TB_INTEGT7 where fk_integmainno='{RecNo}' and no >= 1 and no <= 15; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}'; " +
                //$" select top 5 * from TB_INTEGRESULT_EN where fk_integmainno='{RecNo}'; " +
                $" select top 5 * from TB_INTEGRESULT_HYPHEN_EN where fk_integmainno='{RecNo}'; " +
                $" select * from TB_INTEGLEADLIMIT_ASTM where fk_integmainno='{RecNo}' and leadtype=1; " +
                $" select * from TB_INTEGLEADRESULT_ASTM where fk_integmainno='{RecNo}' and leadtype=1; " +
                $" select * from TB_INTEGLIMIT_ASTM where fk_integmainno='{RecNo}' and leadtype=1; " +
                $" select * from TB_INTEGRESULT_ASTM where fk_integmainno='{RecNo}' and leadtype=1; " +
                $" select * from TB_INTEGLEADLIMIT_ASTM where fk_integmainno='{RecNo}' and leadtype<>1 and reportcase = 'Plastic'; " +
                $" select * from TB_INTEGLEADRESULT_ASTM where fk_integmainno='{RecNo}' and leadtype<>1 and reportcase = 'Plastic'; " +
                $" select * from TB_INTEGLIMIT_ASTM where fk_integmainno='{RecNo}' and leadtype<>1; " +
                $" select * from TB_INTEGRESULT_ASTM where fk_integmainno='{RecNo}' and leadtype<>1; " +
                $" select * from TB_INTEGIMAGE where pk_recno='{RecNo}'; " +
                // Report 6페이지 이상 출력
                /*
                $" select * from TB_INTEGRESULT_EN where fk_integmainno='{RecNo}' and (no>=6 and no<=10);   " +
                $" select * from TB_INTEGRESULT_EN where fk_integmainno='{RecNo}' and (no>=11 and no<=15);  " +
                $" select * from TB_INTEGRESULT_EN where fk_integmainno='{RecNo}' and (no>=16 and no<=20);  " +
                $" select * from TB_INTEGRESULT_EN where fk_integmainno='{RecNo}' and (no>=21 and no<=25);  " +
                $" select * from TB_INTEGRESULT_EN where fk_integmainno='{RecNo}' and (no>=26 and no<=30);  " +
                */
                $" select * from TB_INTEGRESULT_HYPHEN_EN where fk_integmainno='{RecNo}' and (no>=6 and no<=10);   " +
                $" select * from TB_INTEGRESULT_HYPHEN_EN where fk_integmainno='{RecNo}' and (no>=11 and no<=15);  " +
                $" select * from TB_INTEGRESULT_HYPHEN_EN where fk_integmainno='{RecNo}' and (no>=16 and no<=20);  " +
                $" select * from TB_INTEGRESULT_HYPHEN_EN where fk_integmainno='{RecNo}' and (no>=21 and no<=25);  " +
                $" select * from TB_INTEGRESULT_HYPHEN_EN where fk_integmainno='{RecNo}' and (no>=26 and no<=30);  " +
                // Report 6페이지 이상 출력

                // Report limit  출력 - 시작
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Al)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(As)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(B)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Ba)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Cd)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Co)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Cr)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(III)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(VI)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Cu)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Hg)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Mn)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Ni)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Pb)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Sb)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Se)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Sn)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Sr)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%(Zn)%'; " +
                $" select * from TB_INTEGLIMIT_EN where fk_integmainno='{RecNo}' and name like '%Organic%'; " +
                // Report limit  출력 - 끝

                // ASTM Substrate Lead Metal  출력 - 시작
                $" select * from TB_INTEGLEADLIMIT_ASTM where fk_integmainno='{RecNo}' and leadtype<>1 and reportcase = 'Metal'; " +
                $" select * from TB_INTEGLEADRESULT_ASTM where fk_integmainno='{RecNo}' and leadtype<>1 and reportcase = 'Metal'; " +
                // ASTM Substrate Lead Metal  출력 - 끝

                $" select * from TB_INTEGT61 where fk_integmainno='{RecNo}' and result <> '' ; " +

                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 1 AND 1;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 2 AND 2;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 3 AND 3;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 4 AND 4;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 5 AND 5;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEGTIN_EN] WHERE fk_integmainno='{RecNo}') t WHERE t.rownum BETWEEN 6 AND 6;" +

                // Report limit  출력 - 시작
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(MET)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(DBT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(TBT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(TeBT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(MOT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(DOT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(DProT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(DPhT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(TPhT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(DMT)%'; " +
                $" select b.* from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] as a inner join TB_CHEP2 as b on LEFT(a.sampleident,12) = b.fk_chemainno where a.fk_integmainno='{RecNo}' and b.name like '%(MBT)%'; " +

                $" select * from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] where fk_integmainno='{RecNo}' and (no>=1 and no<=5);   " +
                $" select * from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] where fk_integmainno='{RecNo}' and (no>=6 and no<=10);   " +
                $" select * from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] where fk_integmainno='{RecNo}' and (no>=11 and no<=15);   " +
                $" select * from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] where fk_integmainno='{RecNo}' and (no>=16 and no<=20);   " +
                $" select * from [ReportIntegration].[dbo].[TB_INTEGTIN_EN] where fk_integmainno='{RecNo}' and (no>=21 and no<=25);   " +
                $" select * from TB_INTEGT7 where fk_integmainno='{RecNo}' and no >= 16 and no <= 30; " +
                $" select * from TB_INTEGT2 where (LTRIM(clause) like '4%' or clause = '') and fk_integmainno = '{RecNo}'; " +
                $" select * from TB_INTEGT2 where NOT(LTRIM(clause) like '4%' or clause = '') and fk_integmainno = '{RecNo}'; " +

                // Report Lead Limit 출력 - coating, plastic, metal 순으로
                $" select top 1 * from TB_INTEG_LEAD_LIMIT_ASTM where fk_integmainno='{RecNo}' and sam_remarks = 'coating';   " +
                $" select top 1 * from TB_INTEG_LEAD_LIMIT_ASTM where fk_integmainno='{RecNo}' and sam_remarks = 'plastic';   " +
                $" select top 1 * from TB_INTEG_LEAD_LIMIT_ASTM where fk_integmainno='{RecNo}' and sam_remarks = 'metal';   " +

                // Report Lead Result 출력 - coating, plastic, metal 순으로
                $" select * from TB_INTEG_LEAD_RESULT_ASTM where fk_integmainno='{RecNo}' and sam_remarks = 'coating'; " +
                $" select * from TB_INTEG_LEAD_RESULT_ASTM where fk_integmainno='{RecNo}' and sam_remarks = 'plastic';   " +
                $" select * from TB_INTEG_LEAD_RESULT_ASTM where fk_integmainno='{RecNo}' and sam_remarks = 'metal';   " +
                
                // Report Not Lead LIMIT 출력 - coating인 것과 coating 아닌 것 순으로
                $" select * from TB_INTEG_LIMIT_ASTM where fk_integmainno='{RecNo}' and sam_remarks = 'coating' order by" +
                $" (case when name = 'Pb' then 1 " +
                $" when name = 'Sb' then 2 " +
                $" when name = 'As' then 3 " +
                $" when name = 'Ba' then 4 " +
                $" when name = 'Cd' then 5 " +
                $" when name = 'Cr' then 6 " +
                $" when name = 'Hg' then 7 " +
                $" when name = 'Se' then 8 " +
                $" else 9 end ); " +

                $" select * from TB_INTEG_LIMIT_ASTM where fk_integmainno='{RecNo}' and sam_remarks <> 'coating' order by" +
                $" (case when name = 'Pb' then 1 " +
                $" when name = 'Sb' then 2 " +
                $" when name = 'As' then 3 " +
                $" when name = 'Ba' then 4 " +
                $" when name = 'Cd' then 5 " +
                $" when name = 'Cr' then 6 " +
                $" when name = 'Hg' then 7 " +
                $" when name = 'Se' then 8 " +
                $" else 9 end ); " +

                // Report Not Lead Result 출력 - coating인 것과 coating 아닌 것 순으로
                $" select * from TB_INTEG_RESULT_ASTM where fk_integmainno='{RecNo}' and sam_remarks = 'coating';   " +
                $" select * from TB_INTEG_RESULT_ASTM where fk_integmainno='{RecNo}' and sam_remarks <> 'coating';   " +
                $" select * from TB_INTEG_LEAD_RESULT_ASTM where fk_integmainno='{RecNo}' and sam_remarks <> 'coating';   " +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_INTEG_PHT_ASTM] where fk_integmainno='{RecNo}' and sam_remarks = 'plastic') t WHERE t.rownum BETWEEN 1 AND 3;";

            dataSet.Clear();
            dataSet.Tables.Clear();
            dataAdapter.Fill(dataSet);
        }
    }



    public class IntegrationMainDataSet : UlSqlDataSet
    {
        public string RecNo { get; set; }

        public DateTime RegTime { get; set; }

        public DateTime ReceivedTime { get; set; }

        public DateTime RequiredTime { get; set; }

        public DateTime ReportedTime { get; set; }

        public bool Approval { get; set; }

        public EReportArea AreaNo { get; set; }

        public string StaffNo { get; set; }

        public string StaffName { get; set; }

        public string ProductNo { get; set; }

        public string P1ClientNo { get; set; }

        public string P1ClientName { get; set; }

        public string P1ClientAddress { get; set; }

        public string P1FileNo { get; set; }

        public string P1SampleDescription { get; set; }

        public string P1DetailOfSample { get; set; }

        public string P1ItemNo { get; set; }

        public string P1OrderNo { get; set; }

        public string P1Packaging { get; set; }

        public string P1Instruction { get; set; }

        public string P1Buyer { get; set; }

        public string P1Manufacturer { get; set; }

        public string P1CountryOfOrigin { get; set; }

        public string P1CountryOfDestination { get; set; }

        public string P1LabeledAge { get; set; }

        public string P1TestAge { get; set; }

        public string P1AssessedAge { get; set; }

        public string P1ReceivedDate { get; set; }

        public string P1TestPeriod { get; set; }

        public string P1TestMethod { get; set; }

        public string P1TestResults { get; set; }

        public string P1Comments { get; set; }

        public string Description1 { get; set; }

        public string Description2 { get; set; }

        public string Description3 { get; set; }

        public string Description4 { get; set; }

        public string Description5 { get; set; }

        public string Description6 { get; set; }

        public string Description7 { get; set; }

        public string Description8 { get; set; }

        public string Description9 { get; set; }

        public string Description10 { get; set; }

        public string Description11 { get; set; }

        public string Description12 { get; set; }

        public string Description13 { get; set; }

        public string Description14 { get; set; }

        public string Description15 { get; set; }

        public string Description16 { get; set; }

        public string Description17 { get; set; }

        public string Description18 { get; set; }

        public string Description19 { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public EReportApproval ReportApproval { get; set; }

        public IntegrationMainDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            bool wholeRecNo = false;
            string sql = " select * from TB_INTEGMAIN ";

            if (string.IsNullOrWhiteSpace(RecNo) == false)
            {
                if (RecNo[0] == '*')
                {
                    RecNo = RecNo.Remove(0, 1);
                    wholeRecNo = true;
                }
            }

            if (wholeRecNo == true)
            {
                sql += $" where pk_recno='{RecNo}' ";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(RecNo) == true)
                {
                    sql += " where pk_recno<>'' ";
                }
                else
                {
                    sql += $" where pk_recno like '{RecNo}%%' ";
                }

                if (ReportApproval != EReportApproval.None)
                {
                    sql += $" and approval={(int)ReportApproval} ";
                }
                if (AreaNo != EReportArea.None)
                {
                    sql += $" and areano={(int)AreaNo} ";
                }
                if (string.IsNullOrWhiteSpace(ProductNo) == false)
                {
                    sql += $" and productno like '{ProductNo}%%' ";
                }
                if (string.IsNullOrWhiteSpace(From) == false)
                {
                    if (From == To)
                    {
                        sql += $" and regtime like '{From}%%' ";
                    }
                    else
                    {
                        sql += $" and (regtime>='{From} 00:00:00.000' ";
                        sql += $" and regtime<='{To} 23:59:59.999') ";
                    }
                }
            }

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_INTEGMAIN values ('{RecNo}', " +
                $" '{RegTime.ToString(AppRes.csDateTimeFormat)}', '{ReceivedTime.ToString(AppRes.csDateTimeFormat)}', " +
                $" '{RequiredTime.ToString(AppRes.csDateTimeFormat)}', '{ReportedTime.ToString(AppRes.csDateTimeFormat)}', " +
                $" {Convert.ToInt32(Approval)}, {(int)AreaNo}, '{StaffNo.Replace("'", "''")}', '{StaffName.Replace("'", "''")}', " +
                $" '{ProductNo.Replace("'", "''")}', '{P1ClientNo.Replace("'", "''")}', '{P1ClientName.Replace("'", "''")}', " +
                $" '{P1ClientAddress.Replace("'", "''")}', '{P1FileNo.Replace("'", "''")}', '{P1SampleDescription.Replace("'", "''")}', " +
                $" '{P1DetailOfSample.Replace("'", "''")}', '{P1ItemNo.Replace("'", "''")}', '{P1OrderNo.Replace("'", "''")}', " +
                $" '{P1Packaging.Replace("'", "''")}', '{P1Instruction.Replace("'", "''")}', '{P1Buyer.Replace("'", "''")}', " +
                $" '{P1Manufacturer.Replace("'", "''")}', '{P1CountryOfOrigin.Replace("'", "''")}', '{P1CountryOfDestination.Replace("'", "''")}', " +
                $" '{P1LabeledAge.Replace("'", "''")}', '{P1TestAge.Replace("'", "''")}', '{P1AssessedAge.Replace("'", "''")}', " +
                $" '{P1ReceivedDate.Replace("'", "''")}', '{P1TestPeriod.Replace("'", "''")}', '{P1TestMethod.Replace("'", "''")}', " +
                $" '{P1TestResults.Replace("'", "''")}', '{P1Comments.Replace("'", "''")}', '{Description1.Replace("'", "''")}', " +
                $" '{Description2.Replace("'", "''")}', '{Description3.Replace("'", "''")}', '{Description4.Replace("'", "''")}', " +
                $" '{Description5.Replace("'", "''")}', '{Description6.Replace("'", "''")}', '{Description7.Replace("'", "''")}', " +
                $" '{Description8.Replace("'", "''")}', '{Description9.Replace("'", "''")}', '{Description10.Replace("'", "''")}', " +
                $" '{Description11.Replace("'", "''")}', '{Description12.Replace("'", "''")}', '{Description13.Replace("'", "''")}', " +
                $" '{Description14.Replace("'", "''")}', '{Description15.Replace("'", "''")}', '{Description16.Replace("'", "''")}', " +
                $" '{Description17.Replace("'", "''")}', '{Description18.Replace("'", "''")}', '{Description19.Replace("'", "''")}') ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                Console.WriteLine(sql);
                RollbackTrans(trans, e);
            }
        }

        public void Update(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_INTEGMAIN set approval={Convert.ToInt32(Approval)}, areano={(int)AreaNo}, staffno='{StaffNo.Replace("'", "''")}', productno='{ProductNo.Replace("'", "''")}', " +
                $" p1clientno='{P1ClientNo.Replace("'", "''")}', p1clientname='{P1ClientName.Replace("'", "''")}', p1clientaddress='{P1ClientAddress.Replace("'", "''")}', p1fileno='{P1FileNo.Replace("'", "''")}', " +
                $" p1sampledesc='{P1SampleDescription.Replace("'", "''")}', p1detailsample='{P1DetailOfSample.Replace("'", "''")}', p1itemno='{P1ItemNo.Replace("'", "''")}', p1orderno='{P1OrderNo.Replace("'", "''")}', " +
                $" p1packaging='{P1Packaging.Replace("'", "''")}', p1instruction='{P1Instruction.Replace("'", "''")}', p1buyer='{P1Buyer.Replace("'", "''")}', p1manufacturer='{P1Manufacturer.Replace("'", "''")}', " +
                $" p1countryorigin='{P1CountryOfOrigin.Replace("'", "''")}', p1countrydest='{P1CountryOfDestination.Replace("'", "''")}', p1labelage='{P1LabeledAge.Replace("'", "''")}', " +
                $" p1testage='{P1TestAge.Replace("'", "''")}', p1assessedage='{P1AssessedAge.Replace("'", "''")}', p1recevdate='{P1ReceivedDate.Replace("'", "''")}', p1testperiod='{P1TestPeriod.Replace("'", "''")}', " +
                $" p1testmethod='{P1TestMethod.Replace("'", "''")}', p1testresult='{P1TestResults.Replace("'", "''")}', p1comment='{P1Comments.Replace("'", "''")}', staffname='{StaffName.Replace("'", "''")}', " +
                $" desc01='{Description1.Replace("'", "''")}', desc02='{Description2.Replace("'", "''")}', desc03='{Description3.Replace("'", "''")}', desc04='{Description4.Replace("'", "''")}', " +
                $" desc05='{Description5.Replace("'", "''")}', desc06='{Description6.Replace("'", "''")}', desc07='{Description7.Replace("'", "''")}', desc08='{Description8.Replace("'", "''")}', " +
                $" desc09='{Description9.Replace("'", "''")}', desc10='{Description10.Replace("'", "''")}', desc11='{Description11.Replace("'", "''")}', desc12='{Description12.Replace("'", "''")}', " +
                $" desc13='{Description13.Replace("'", "''")}', desc14='{Description14.Replace("'", "''")}', desc15='{Description15.Replace("'", "''")}', desc16='{Description16.Replace("'", "''")}', " +
                $" desc17='{Description17.Replace("'", "''")}', desc18='{Description18.Replace("'", "''")}', desc19='{Description19.Replace("'", "''")}' " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void UpdateApproval(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_INTEGMAIN set " +
                $" approval={Convert.ToInt32(Approval)}, staffno='{StaffNo}', staffname='{StaffName}' " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGMAIN " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = "";
                RegTime = DateTime.Now;
                ReceivedTime = DateTime.Now;
                RequiredTime = DateTime.Now;
                ReportedTime = DateTime.Now;
                Approval = false;
                AreaNo = EReportArea.None;
                StaffNo = "";
                StaffName = "";
                ProductNo = "";
                P1ClientNo = "";
                P1ClientName = "";
                P1ClientAddress = "";
                P1FileNo = "";
                P1SampleDescription = "";
                P1DetailOfSample = "";
                P1ItemNo = "";
                P1OrderNo = "";
                P1Packaging = "";
                P1Instruction = "";
                P1Buyer = "";
                P1Manufacturer = "";
                P1CountryOfOrigin = "";
                P1CountryOfDestination = "";
                P1LabeledAge = "";
                P1TestAge = "";
                P1AssessedAge = "";
                P1ReceivedDate = "";
                P1TestPeriod = "";
                P1TestMethod = "";
                P1TestResults = "";
                P1Comments = "";
                Description1 = "";
                Description2 = "";
                Description3 = "";
                Description4 = "";
                Description5 = "";
                Description6 = "";
                Description7 = "";
                Description8 = "";
                Description9 = "";
                Description10 = "";
                Description11 = "";
                Description12 = "";
                Description13 = "";
                Description14 = "";
                Description15 = "";
                Description16 = "";
                Description17 = "";
                Description18 = "";
                Description19 = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToString(row["pk_recno"]);
            RegTime = Convert.ToDateTime(row["regtime"]);
            ReceivedTime = Convert.ToDateTime(row["receivedtime"]);
            RequiredTime = Convert.ToDateTime(row["requiredtime"]);
            ReportedTime = Convert.ToDateTime(row["reportedtime"]);
            Approval = Convert.ToBoolean(row["approval"]);
            AreaNo = (EReportArea)Convert.ToInt32(row["areano"]);
            StaffNo = Convert.ToString(row["staffno"]);
            StaffName = Convert.ToString(row["staffname"]);
            ProductNo = Convert.ToString(row["productno"]);
            P1ClientNo = Convert.ToString(row["p1clientno"]);
            P1ClientName = Convert.ToString(row["p1clientname"]);
            P1ClientAddress = Convert.ToString(row["p1clientaddress"]);
            P1FileNo = Convert.ToString(row["p1fileno"]);
            P1SampleDescription = Convert.ToString(row["p1sampledesc"]);
            P1DetailOfSample = Convert.ToString(row["p1detailsample"]);
            P1ItemNo = Convert.ToString(row["p1itemno"]);
            P1OrderNo = Convert.ToString(row["p1orderno"]);
            P1Packaging = Convert.ToString(row["p1packaging"]);
            P1Instruction = Convert.ToString(row["p1instruction"]);
            P1Buyer = Convert.ToString(row["p1buyer"]);
            P1Manufacturer = Convert.ToString(row["p1manufacturer"]);
            P1CountryOfOrigin = Convert.ToString(row["p1countryorigin"]);
            P1CountryOfDestination = Convert.ToString(row["p1countrydest"]);
            P1LabeledAge = Convert.ToString(row["p1labelage"]);
            P1TestAge = Convert.ToString(row["p1testage"]);
            P1AssessedAge = Convert.ToString(row["p1assessedage"]);
            P1ReceivedDate = Convert.ToString(row["p1recevdate"]);
            P1TestPeriod = Convert.ToString(row["p1testperiod"]);
            P1TestMethod = Convert.ToString(row["p1testmethod"]);
            P1TestResults = Convert.ToString(row["p1testresult"]);
            P1Comments = Convert.ToString(row["p1comment"]);
            Description1 = Convert.ToString(row["desc01"]);
            Description2 = Convert.ToString(row["desc02"]);
            Description3 = Convert.ToString(row["desc03"]);
            Description4 = Convert.ToString(row["desc04"]);
            Description5 = Convert.ToString(row["desc05"]);
            Description6 = Convert.ToString(row["desc06"]);
            Description7 = Convert.ToString(row["desc07"]);
            Description8 = Convert.ToString(row["desc08"]);
            Description9 = Convert.ToString(row["desc09"]);
            Description10 = Convert.ToString(row["desc10"]);
            Description11 = Convert.ToString(row["desc11"]);
            Description12 = Convert.ToString(row["desc12"]);
            Description13 = Convert.ToString(row["desc13"]);
            Description14 = Convert.ToString(row["desc14"]);
            Description15 = Convert.ToString(row["desc15"]);
            Description16 = Convert.ToString(row["desc16"]);
            Description17 = Convert.ToString(row["desc17"]);
            Description18 = Convert.ToString(row["desc18"]);
            Description19 = Convert.ToString(row["desc19"]);
        }
    }

    public class IntegrationT1DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string Requested { get; set; }

        public string Conclusion { get; set; }

        public IntegrationT1DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGT1 " +
                $" where fk_integmainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_INTEGT1 values('{MainNo}', {No}, {Convert.ToInt32(Line)}, " +
                $" '{Requested.Replace("'", "''")}', '{Conclusion.Replace("'", "''")}');  " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGT1          " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                Requested = "";
                Conclusion = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_integmainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            Requested = Convert.ToString(row["requested"]);
            Conclusion = Convert.ToString(row["conclusion"]);
        }
    }

    public class IntegrationT2DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string Clause { get; set; }

        public string Description { get; set; }

        public string Result { get; set; }

        public IntegrationT2DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGT2 " +
                $" where fk_integmainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_INTEGT2 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, '{Clause.Replace("'", "''")}', " +
                $" '{Description.Replace("'", "''")}', '{Result.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGT2          " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                Clause = "";
                Description = "";
                Result = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_integmainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            Clause = Convert.ToString(row["clause"]);
            Description = Convert.ToString(row["description"]);
            Result = Convert.ToString(row["result"]);
        }
    }

    public class IntegrationT3DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string Clause { get; set; }

        public string Description { get; set; }

        public string Result { get; set; }

        public IntegrationT3DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGT3 " +
                $" where fk_integmainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_INTEGT3 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, '{Clause.Replace("'", "''")}', " +
                $" '{Description.Replace("'", "''")}', '{Result.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGT3          " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                Clause = "";
                Description = "";
                Result = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_integmainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            Clause = Convert.ToString(row["clause"]);
            Description = Convert.ToString(row["description"]);
            Result = Convert.ToString(row["result"]);
        }
    }

    public class IntegrationT4DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string Sample { get; set; }

        public string Result { get; set; }

        public IntegrationT4DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGT4 " +
                $" where fk_integmainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_INTEGT4 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, " +
                $" '{Sample.Replace("'", "''")}', '{Result.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGT4          " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                Sample = "";
                Result = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_integmainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            Sample = Convert.ToString(row["sample"]);
            Result = Convert.ToString(row["result"]);
        }
    }

    public class IntegrationT5DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string Sample { get; set; }

        public string BurningRate { get; set; }

        public string Result { get; set; }

        public IntegrationT5DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGT5 " +
                $" where fk_integmainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_INTEGT5 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, " +
                $" '{Sample.Replace("'", "''")}', '{BurningRate.Replace("'", "''")}', '{Result.Replace("'", "''")}'); " +                
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGT5          " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                Sample = "";
                BurningRate = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_integmainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            Sample = Convert.ToString(row["sample"]);
            BurningRate = Convert.ToString(row["burningrate"]);
        }
    }

    public class IntegrationT6DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string TestItem { get; set; }

        public string Result { get; set; }

        public string Requirement { get; set; }

        public IntegrationT6DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGT6 " +
                $" where fk_integmainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_INTEGT6 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, '{TestItem.Replace("'", "''")}', " +
                $" '{Result.Replace("'", "''")}', '{Requirement.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGT6          " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                TestItem = "";
                Result = "";
                Requirement = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_integmainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            TestItem = Convert.ToString(row["testitem"]);
            Result = Convert.ToString(row["result"]);
            Requirement = Convert.ToString(row["requirement"]);
        }
    }

    public class IntegrationT61DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string TestItem { get; set; }

        public string Result { get; set; }

        public string Requirement { get; set; }

        public string Note { get; set; }

        public string Description { get; set; }


        public IntegrationT61DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGT61 " +
                $" where fk_integmainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_INTEGT61 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, '{TestItem.Replace("'", "''")}', " +
                $" '{Result.Replace("'", "''")}', '{Requirement.Replace("'", "''")}', '{Note.Replace("'", "''")}', '{Description.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGT61          " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                TestItem = "";
                Result = "";
                Requirement = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_integmainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            TestItem = Convert.ToString(row["testitem"]);
            Result = Convert.ToString(row["result"]);
            Requirement = Convert.ToString(row["requirement"]);
        }
    }

    public class IntegrationT7DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public string JobNo { get; set; }

        public int No { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string MaterialNo { get; set; }

        public string ReportNo { get; set; }

        public string IssuedDate { get; set; }

        public IntegrationT7DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGT7 " +
                $" where fk_integmainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_INTEGT7 values " +
                $" ('{MainNo}', '{JobNo}', {No}, '{Description.Replace("'", "''").Trim().ToUpper()}', " +
                $" '{Name.Replace("'", "''")}', '{MaterialNo.Replace("'", "''")}', " +
                $" '{ReportNo.Replace("'", "''")}', '{IssuedDate.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

                // 원본
                //$" insert into TB_INTEGT7 values " +
                //$" ('{MainNo}', '{JobNo}', {No}, '{Description.Replace("'", "''")}', " +
                //$" '{Name.Replace("'", "''")}', '{MaterialNo.Replace("'", "''")}', " +
                //$" '{ReportNo.Replace("'", "''")}', '{IssuedDate.Replace("'", "''")}'); " +
                //$" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGT7          " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                JobNo = "";
                No = 0;
                Description = "";
                Name = "";
                MaterialNo = "";
                ReportNo = "";
                IssuedDate = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_integmainno"]);
            JobNo = Convert.ToString(row["jobno"]);
            No = Convert.ToInt32(row["no"]);
            Description = Convert.ToString(row["description"]);
            Name = Convert.ToString(row["name"]);
            MaterialNo = Convert.ToString(row["materialno"]);
            ReportNo = Convert.ToString(row["reportno"]);
            IssuedDate = Convert.ToString(row["issueddate"]);
        }
    }

    public class IntegrationLimitEnDataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public string Name { get; set; }

        public string LoValue { get; set; }

        public string HiValue { get; set; }

        public string ReportValue { get; set; }

        public IntegrationLimitEnDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGLIMIT_EN " +
                $" where fk_integmainno='{MainNo}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_INTEGLIMIT_EN values " +
                $" ('{MainNo}', '{LoValue}', '{HiValue}', '{ReportValue}', '{Name}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGLIMIT_EN    " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                LoValue = "";
                HiValue = "";
                ReportValue = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_integmainno"]);
            LoValue = Convert.ToString(row["lovalue"]);
            HiValue = Convert.ToString(row["hivalue"]);
            ReportValue = Convert.ToString(row["reportvalue"]);
        }
    }

    public class IntegrationResultEnDataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public string Mg { get; set; }

        public string Ai { get; set; }

        public string As { get; set; }

        public string B { get; set; }

        public string Ba { get; set; }

        public string Cd { get; set; }

        public string Co { get; set; }

        public string Cr { get; set; }

        public string Cr3 { get; set; }

        public string Cr4 { get; set; }

        public string Cr6 { get; set; }

        public string Cu { get; set; }

        public string Hg { get; set; }

        public string Mn { get; set; }

        public string Ni { get; set; }

        public string Pb { get; set; }

        public string Sb { get; set; }

        public string Se { get; set; }

        public string Sn { get; set; }

        public string Sr { get; set; }

        public string Zn { get; set; }

        public string Tin { get; set; }

        public string OrgTin { get; set; }

        public string DBT { get; set; }

        public string DMT { get; set; }

        public string DOT { get; set; }

        public string DPhT { get; set; }

        public string DProT { get; set; }

        public string MBT { get; set; }

        public string MET { get; set; }

        public string MOT { get; set; }

        public string TBT { get; set; }

        public string TeBT { get; set; }

        public string TPhT { get; set; }

        public string BuT { get; set; }

        public string SampleDescription { get; set; }
        
        public string Sampleident { get; set; }
        
        public string Sch_Code { get; set; }

        public IntegrationResultEnDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGRESULT_EN " +
                $" where fk_integmainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            if (string.IsNullOrWhiteSpace(Cr6))
            {
                Cr6 = "N.D.";
            }

            //if (string.IsNullOrWhiteSpace(OrgTin) || OrgTin.Equals("N.D."))
            //{
            //    OrgTin = "--";
            //}

            //if (string.IsNullOrWhiteSpace(OrgTin))
            //{
            //    OrgTin = "--";
            //}

            string sql =
                $" insert into TB_INTEGRESULT_EN (fk_integmainno, sampleident, sch_code, sam_description, no, mg, ai, sb, \"as\", ba, b, cd, cr3, cr6, co, cu, pb, mn, hg, ni, se, sr ,sn, tin, zn)" +
                $" values " +
                $" ('{MainNo}', '{Sampleident}', '{Sch_Code}', '{SampleDescription}', '{No}', '{Mg}', '{Ai}', '{Sb}', '{As}', '{Ba}', '{B}', '{Cd}', '{Cr3}', '{Cr6}', '{Co}', '{Cu}', '{Pb}', '{Mn}', '{Hg}', '{Ni}', '{Se}', '{Sr}', '{Sn}', '{OrgTin}', '{Zn}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_HYPHEN(SqlTransaction trans = null)
        {
            if (string.IsNullOrWhiteSpace(Cr6))
            {
                Cr6 = "N.D.";
            }

            if (string.IsNullOrWhiteSpace(OrgTin))
            {
                OrgTin = "--";
            }

            string sql =
                $" insert into TB_INTEGRESULT_HYPHEN_EN (fk_integmainno, sampleident, sch_code, sam_description, no, mg, ai, sb, \"as\", ba, b, cd, cr3, cr6, co, cu, pb, mn, hg, ni, se, sr ,sn, tin, zn)" +
                $" values " +
                $" ('{MainNo}', '{Sampleident}', '{Sch_Code}', '{SampleDescription}', '{No}', '{Mg}', '{Ai}', '{Sb}', '{As}', '{Ba}', '{B}', '{Cd}', '{Cr3}', '{Cr6}', '{Co}', '{Cu}', '{Pb}', '{Mn}', '{Hg}', '{Ni}', '{Se}', '{Sr}', '{Sn}', '{OrgTin}', '{Zn}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_Result_Tin(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_INTEGTIN_EN (fk_integmainno, sampleident, sch_code, sam_description, no, DMT, MeT, DProT, BuT, DBT, TBT, MOT, DOT, TeBT, DPhT, TPhT)" +
                $" values " +
                $" ('{MainNo}', '{Sampleident}', '{Sch_Code}', '{SampleDescription}', '{No}', '{DMT}', '{MET}', '{DProT}', '{MBT}', '{DBT}', '{TBT}', '{MOT}', '{DOT}', '{TeBT}', '{DPhT}', '{TPhT}'); " +
                $" select cast(scope_identity() as bigint); ";
            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGRESULT_EN " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_Tin(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGTIN_EN " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_HYPHEN(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGRESULT_HYPHEN_EN " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }        

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Mg = "";
                Ai = "";
                As = "";
                B = "";
                Ba = "";
                Cd = "";
                Co = "";
                Cr = "";
                Cr3 = "";
                Cr4 = "";
                Cu = "";
                Hg = "";
                Mn = "";
                Ni = "";
                Pb = "";
                Sb = "";
                Se = "";
                Sn = "";
                Sr = "";
                Zn = "";
                Tin = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_integmainno"]);
            No = Convert.ToInt32(row["no"]);
            Mg = Convert.ToString(row["mg"]);
            Ai = Convert.ToString(row["ai"]);
            As = Convert.ToString(row["as"]);
            B = Convert.ToString(row["b"]);
            Ba = Convert.ToString(row["ba"]);
            Cd = Convert.ToString(row["cd"]);
            Co = Convert.ToString(row["co"]);
            Cr = Convert.ToString(row["cr"]);
            Cr3 = Convert.ToString(row["cr3"]);
            Cr4 = Convert.ToString(row["cr4"]);
            Cu = Convert.ToString(row["cu"]);
            Hg = Convert.ToString(row["hg"]);
            Mn = Convert.ToString(row["mn"]);
            Ni = Convert.ToString(row["ni"]);
            Pb = Convert.ToString(row["pb"]);
            Sb = Convert.ToString(row["sb"]);
            Se = Convert.ToString(row["se"]);
            Sn = Convert.ToString(row["sn"]);
            Sr = Convert.ToString(row["sr"]);
            Zn = Convert.ToString(row["zn"]);
            Tin = Convert.ToString(row["tin"]);
        }
    }

    public class IntegrationLimitASTMDataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public string Sampleident { get; set; }

        public string Pro_proj { get; set; }

        public string Name { get; set; }

        public string LoValue { get; set; }

        public string HiValue { get; set; }

        public string ReportValue { get; set; }

        public string Sch_code { get; set; }

        public string Sam_remarks { get; set; }

        public IntegrationLimitASTMDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEG_LIMIT_ASTM " +
                $" where fk_integmainno='{MainNo}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_INTEG_LIMIT_ASTM values " +
                $" ('{MainNo}', '{Sampleident}', '{Pro_proj}', '{Name}', '{LoValue}', '{HiValue}', '{ReportValue}', '{Sch_code}', '{Sam_remarks}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEG_LIMIT_ASTM    " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                Sampleident = "";
                Pro_proj = "";
                LoValue = "";
                HiValue = "";
                ReportValue = "";
                Sch_code = "";
                Sam_remarks = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_integmainno"]);
            Sampleident = Convert.ToString(row["sampleident"]);
            Pro_proj = Convert.ToString(row["pro_proj"]);
            Name = Convert.ToString(row["name"]);
            LoValue = Convert.ToString(row["lovalue"]);
            HiValue = Convert.ToString(row["hivalue"]);
            ReportValue = Convert.ToString(row["reportvalue"]);
            Sch_code = Convert.ToString(row["sch_code"]);
            Sam_remarks = Convert.ToString(row["sam_remarks"]);
        }
    }

    public class IntegrationLeadLimitAstmDataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public ELeadType LeadType { get; set; }

        public string LoValue { get; set; }

        public string HiValue { get; set; }

        public string ReportValue { get; set; }

        public string ReportCase { get; set; }

        public IntegrationLeadLimitAstmDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGLEADLIMIT_ASTM " +
                $" where fk_integmainno='{MainNo}' and leadtype={(int)LeadType} ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null, int iLeadtype = -1)
        {
            string sql =
                $" insert into TB_INTEGLEADLIMIT_ASTM values " +
                //$" ('{MainNo}', {(int)LeadType}, '{LoValue}', '{HiValue}', '{ReportValue}'); " +
                $" ('{MainNo}', {iLeadtype}, '{LoValue}', '{HiValue}', '{ReportValue}', '{ReportCase}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGLEADLIMIT_ASTM " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                LeadType = ELeadType.None;
                LoValue = "";
                HiValue = "";
                ReportValue = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_integmainno"]);
            LeadType = (ELeadType)Convert.ToInt32(row["leadtype"]);
            LoValue = Convert.ToString(row["lovalue"]);
            HiValue = Convert.ToString(row["hivalue"]);
            ReportValue = Convert.ToString(row["reportvalue"]);
        }
    }

    public class IntegrationLeadResultAstmDataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public ELeadType LeadType { get; set; }

        public string Pb { get; set; }

        public string ReportCase { get; set; }

        public IntegrationLeadResultAstmDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGLEADRESULT_ASTM " +
                $" where fk_integmainno='{MainNo}' and leadtype={(int)LeadType} ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null, int iLeadtype = -1)
        {
            string sql =
                $" insert into TB_INTEGLEADRESULT_ASTM values " +
                //$" ('{MainNo}', {No}, {(int)LeadType}, '{Pb}'); " +
                $" ('{MainNo}', {No}, {iLeadtype}, '{Pb}', '{ReportCase}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGLEADRESULT_ASTM " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                LeadType = ELeadType.None;
                Pb = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_integmainno"]);
            No = Convert.ToInt32(row["no"]);
            LeadType = (ELeadType)Convert.ToInt32(row["leadtype"]);
            Pb = Convert.ToString(row["pb"]);
        }
    }

    public class IntegrationLimitAstmDataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public ELeadType LeadType { get; set; }

        public string Name { get; set; }

        public string LoValue { get; set; }

        public string HiValue { get; set; }

        public string ReportValue { get; set; }

        public IntegrationLimitAstmDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGLIMIT_ASTM " +
                $" where fk_integmainno='{MainNo}' and leadtype={(int)LeadType} ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null, int iLeadtype = -1)
        {
            string sql =
                $" insert into TB_INTEGLIMIT_ASTM values " +
                //$" ('{MainNo}', {(int)LeadType}, '{Name}', '{LoValue}', '{HiValue}', '{ReportValue}'); " +
                $" ('{MainNo}', {iLeadtype}, '{Name}', '{LoValue}', '{HiValue}', '{ReportValue}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGLIMIT_ASTM " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                LeadType = ELeadType.None;
                Name = "";
                LoValue = "";
                HiValue = "";
                ReportValue = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_integmainno"]);
            LeadType = (ELeadType)Convert.ToInt32(row["leadtype"]);
            Name = Convert.ToString(row["name"]);
            LoValue = Convert.ToString(row["lovalue"]);
            HiValue = Convert.ToString(row["hivalue"]);
            ReportValue = Convert.ToString(row["reportvalue"]);
        }
    }

    public class IntegrationResultASTMDataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public string Pro_proj { get; set; }

        public string Sampleident { get; set; }

        public string Sch_code { get; set; }

        public string Sam_description { get; set; }

        public string Sam_remarks { get; set; }

        public ELeadType LeadType { get; set; }

        public int No { get; set; }

        public string Mg { get; set; }

        public string Pb { get; set; }

        public string Sb { get; set; }

        public string As { get; set; }

        public string Ba { get; set; }

        public string Cd { get; set; }

        public string Cr { get; set; }

        public string Hg { get; set; }

        public string Se { get; set; }

        public string lovalue { get; set; }

        public string hivalue { get; set; }

        public string reportvalue { get; set; }

        public string DBP { get; set; }

        public string BBP { get; set; }

        public string DEHP { get; set; }

        public string DINP { get; set; }

        public string DCHP { get; set; }

        public string DnHP { get; set; }

        public string DIBP { get; set; }

        public string DnPP { get; set; }

        public string DNOP { get; set; }

        public string DIDP { get; set; }

        public IntegrationResultASTMDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEG_RESULT_ASTM " +
                $" where fk_integmainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null, int iLeadType = -1)
        {
            string sql =

                $" insert into TB_INTEG_RESULT_ASTM (fk_integmainno, pro_proj, sampleident, sch_code, sam_description, sam_remarks, no, mg, pb, sb, \"as\", ba, cd, cr, hg, se)" +
                $" values " +
                $" ('{MainNo}', '{Pro_proj}', '{Sampleident}', '{Sch_code}', '{Sam_description}', '{Sam_remarks}', '{No}', '{Mg}', '{Pb}', '{Sb}', '{As}', '{Ba}', '{Cd}', '{Cr}', '{Hg}', '{Se}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_TB_INTEG_LEAD_LIMIT_ASTM(SqlTransaction trans = null)
        {
            string sql =

                $" insert into TB_INTEG_LEAD_LIMIT_ASTM (fk_integmainno, pro_proj, sampleident, lovalue, hivalue, reportvalue, sch_code, sam_remarks)" +
                $" values " +
                $" ('{MainNo}', '{Pro_proj}', '{Sampleident}', '{lovalue}', '{hivalue}', '{reportvalue}', '{Sch_code}', '{Sam_remarks}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_TB_INTEG_LEAD_RESULT_ASTM(SqlTransaction trans = null)
        {
            string sql =

                $" insert into TB_INTEG_LEAD_RESULT_ASTM (fk_integmainno, pro_proj, sampleident, no, pb, sch_code, sam_remarks, sam_description)" +
                $" values " +
                $" ('{MainNo}', '{Pro_proj}', '{Sampleident}', '{No}', '{Pb}', '{Sch_code}', '{Sam_remarks}', '{Sam_description}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_TB_INTEG_PHT_ASTM(SqlTransaction trans = null)
        {
            string sql =

                $" insert into TB_INTEG_PHT_ASTM (fk_integmainno, pro_proj, sampleident, sch_code, sam_description, sam_remarks, no, DBP, BBP, DEHP, DINP, DCHP, DnHP, DIBP, DnPP, DNOP, DIDP )" +
                $" values " +
                $" ('{MainNo}', '{Pro_proj}', '{Sampleident}', '{Sch_code}', '{Sam_description}', '{Sam_remarks}', '{No}', '{DBP}', '{BBP}', '{DEHP}', '{DINP}', '{DCHP}', '{DnHP}', '{DIBP}', '{DnPP}', '{DNOP}', '{DIDP}' ); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEG_RESULT_ASTM " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_TB_INTEG_PHT_ASTM(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEG_PHT_ASTM " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_TB_INTEG_LIMIT_ASTM(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEG_LIMIT_ASTM " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_TB_INTEG_LEAD_RESULT_ASTM(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEG_LEAD_RESULT_ASTM " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_TB_INTEG_LEAD_LIMIT_ASTM(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEG_LEAD_LIMIT_ASTM " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                Pro_proj = "";
                Sampleident = "";
                Sch_code = "";
                Sam_description = "";
                No = 0;
                Mg = "";
                Pb = "";
                Sb = "";
                As = "";
                Ba = "";
                Cd = "";
                Cr = "";
                Hg = "";
                Se = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_chemainno"]);
            Pro_proj = Convert.ToString(row["pro_proj"]);
            Sampleident = Convert.ToString(row["sampleident"]);
            Sch_code = Convert.ToString(row["sch_code"]);
            Sam_description = Convert.ToString(row["sam_description"]);
            No = Convert.ToInt32(row["no"]);
            Mg = Convert.ToString(row["mg"]);
            Pb = Convert.ToString(row["pb"]);
            Sb = Convert.ToString(row["sb"]);
            As = Convert.ToString(row["as"]);
            Ba = Convert.ToString(row["ba"]);
            Cd = Convert.ToString(row["cd"]);
            Cr = Convert.ToString(row["cr"]);
            Hg = Convert.ToString(row["hg"]);
            Se = Convert.ToString(row["se"]);
        }
    }

    public class IntegrationResultAstmDataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public ELeadType LeadType { get; set; }

        public string Mg { get; set; }
        
        public string As { get; set; }

        public string Ba { get; set; }

        public string Cd { get; set; }

        public string Cr { get; set; }

        public string Hg { get; set; }

        public string Pb { get; set; }

        public string Sb { get; set; }

        public string Se { get; set; }

        public IntegrationResultAstmDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGRESULT_ASTM " +
                $" where fk_integmainno='{MainNo}' and leadtype={(int)LeadType} " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null, int iLeadType = -1)
        {
            string sql =
                
                $" insert into TB_INTEGRESULT_ASTM values ( " +
                $" '{MainNo}', {No}, {iLeadType}, '{Mg}', '{As}', " +
                $" '{Ba}', '{Cd}', '{Cr}', '{Hg}', '{Pb}', '{Sb}', '{Se}') " +
                $" select cast(scope_identity() as bigint); ";

                /*
                $" insert into TB_INTEGRESULT_ASTM values ( " +
                //$" '{MainNo}', {No}, {(int)LeadType}, '{Mg}', '{As}', " +
                $" '{MainNo}', {No}, {iLeadType}, '{Mg}', '{Pb}', " +
                $" '{Sb}', '{As}', '{Ba}', '{Cd}', '{Cr}', '{Hg}', '{Se}') " +
                $" select cast(scope_identity() as bigint); ";
                */

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGRESULT_ASTM " +
                $" where fk_integmainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                LeadType = ELeadType.None;
                Mg = "";
                As = "";
                Ba = "";
                Cd = "";
                Cr = "";
                Hg = "";
                Pb = "";
                Sb = "";
                Se = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_integmainno"]);
            No = Convert.ToInt32(row["no"]);
            LeadType = (ELeadType)Convert.ToInt32(row["leadtype"]);
            Mg = Convert.ToString(row["mg"]);
            As = Convert.ToString(row["as"]);
            Ba = Convert.ToString(row["ba"]);
            Cd = Convert.ToString(row["cd"]);
            Cr = Convert.ToString(row["cr"]);
            Hg = Convert.ToString(row["hg"]);
            Pb = Convert.ToString(row["pb"]);
            Sb = Convert.ToString(row["sb"]);
            Se = Convert.ToString(row["se"]);
        }
    }

    public class PhysicalReportDataSet : UlSqlDataSet
    {
        public string RecNo { get; set; }

        public PhysicalReportDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYMAIN where pk_recno='{RecNo}'; " +
                $" select * from TB_PHYP2 where fk_phymainno='{RecNo}'; " +
                $" select * from TB_PHYP3 where fk_phymainno='{RecNo}'; " +
                $" select * from TB_PHYP40 where fk_phymainno='{RecNo}'; " +
                $" select * from TB_PHYP41 where fk_phymainno='{RecNo}'; " +
                $" select * from TB_PHYP42 where fk_phymainno='{RecNo}'; " +
                $" select * from TB_PHYP5 where fk_phymainno='{RecNo}'; " +
                $" select * from TB_PHYIMAGE where pk_recno='{RecNo}'; " +
                $" select * from TB_PHYRPTVIEW where fk_phymainno='{RecNo}' and page='5'; " +
                $" select * from TB_PHYRPTVIEW where fk_phymainno='{RecNo}' and page='6'; " +
                $" select * from TB_PHYP6 where fk_phymainno='{RecNo}'; " +
                $" select * from TB_PHYP45 where fk_phymainno='{RecNo}' and clause = '4.5'; " +
                $" select * from TB_PHYRPTVIEW where fk_phymainno='{RecNo}' and page='5_Note1'; " +
                $" select * from TB_PHYRPTVIEW where fk_phymainno='{RecNo}' and page='7'; " +
                $" select * from TB_PHYP7 where fk_phymainno='{RecNo}'; " +
                $" select * from TB_PHYRPTVIEW where fk_phymainno='{RecNo}' and page='4_Note1'; " +
                $" select * from TB_PHYP3 where (LTRIM(clause) like '4%' or clause = '') and fk_phymainno = '{RecNo}'; " +
                $" select * from TB_PHYP3 where NOT(LTRIM(clause) like '4%' or clause = '') and fk_phymainno = '{RecNo}'; ";

            dataSet.Clear();
            dataSet.Tables.Clear();
            dataAdapter.Fill(dataSet);
        }
    }

    public class IntegrationImageDataSet : UlSqlDataSet
    {
        public string RecNo { get; set; }

        public Bitmap Signature { get; set; }

        public Bitmap Picture { get; set; }

        private ImageConverter imageConvert;

        public IntegrationImageDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
            imageConvert = new ImageConverter();
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEGIMAGE " +
                $" where pk_recno='{RecNo}'  ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_INTEGIMAGE values " +
                $" ('{RecNo}', @signature, @picture) ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);

                command.CommandText = sql;
                command.Parameters.Clear();
                if (Signature == null)
                {
                    SqlParameter signatureParam = new SqlParameter("@signature", SqlDbType.Image);
                    signatureParam.Value = DBNull.Value;
                    command.Parameters.Add(signatureParam);
                }
                else
                {
                    command.Parameters.Add("@signature", SqlDbType.Image);
                    command.Parameters["@signature"].Value = (byte[])imageConvert.ConvertTo(Signature, typeof(byte[]));
                }

                if (Picture == null)
                {
                    SqlParameter pictureParam = new SqlParameter("@picture", SqlDbType.Image);
                    pictureParam.Value = DBNull.Value;
                    command.Parameters.Add(pictureParam);
                }
                else
                {
                    command.Parameters.Add("@picture", SqlDbType.Image);
                    command.Parameters["@picture"].Value = (byte[])imageConvert.ConvertTo(Picture, typeof(byte[]));
                }
                command.ExecuteNonQuery();

                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_INTEGIMAGE      " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = "";
                Signature = null;
                Picture = null;
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToString(row["pk_recno"]);

            if (row["signature"] == DBNull.Value) Signature = null;
            else
            {
                byte[] signatureRaw = (byte[])row["signature"];
                Signature = (signatureRaw == null) ? null : new Bitmap(new MemoryStream(signatureRaw));
            }

            if (row["picture"] == DBNull.Value) Picture = null;
            else
            {
                byte[] pictureRaw = (byte[])row["picture"];
                Picture = (pictureRaw == null) ? null : new Bitmap(new MemoryStream(pictureRaw));
            }
        }
    }

    public class PhysicalMainDataSet : UlSqlDataSet
    {
        public string RecNo { get; set; }

        public string Complete { get; set; }

        public DateTime RegTime { get; set; }

        public DateTime ReceivedTime { get; set; }

        public DateTime RequiredTime { get; set; }

        public DateTime ReportedTime { get; set; }

        public bool Approval { get; set; }

        public EReportArea AreaNo { get; set; }

        public string StaffNo { get; set; }

        public string ProductNo { get; set; }

        public string P1ClientNo { get; set; }

        public string P1ClientName { get; set; }

        public string P1ClientAddress { get; set; }

        public string P1FileNo { get; set; }

        public string P1SampleDescription { get; set; }

        public string P1DetailOfSample { get; set; }

        public string P1ItemNo { get; set; }

        public string P1OrderNo { get; set; }

        public string P1Packaging { get; set; }

        public string P1Instruction { get; set; }

        public string P1Buyer { get; set; }

        public string P1Manufacturer { get; set; }

        public string P1CountryOfOrigin { get; set; }

        public string P1CountryOfDestination { get; set; }

        public string P1LabeledAge { get; set; }

        public string P1TestAge { get; set; }

        public string P1AssessedAge { get; set; }

        public string P1ReceivedDate { get; set; }

        public string P1TestPeriod { get; set; }

        public string P1TestMethod { get; set; }

        public string P1TestResults { get; set; }

        public string P1Comments { get; set; }

        public string P2Name { get; set; }

        public string P3Description1 { get; set; }

        public string P3Description2 { get; set; }

        public string P4Description1 { get; set; }

        public string P4Description2 { get; set; }

        public string P4Description3 { get; set; }

        public string P4Description4 { get; set; }

        public string P5Description1 { get; set; }

        public string P5Description2 { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public EReportApproval ReportApproval { get; set; }

        public PhysicalMainDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            bool wholeRecNo = false;
            string sql = " select t1.*, t3.*, " +
                         " CASE WHEN (t2.complete = 1 and t2.complete_GiSool = 1) THEN 'yes' ELSE 'no' END as complete" +
                         " from TB_PHYMAIN t1" +
                         " INNER JOIN TB_PHYCOMPETE t2 ON (t1.pk_recno = t2.fk_phymainno)" +
                         " LEFT JOIN TB_PRODUCT t3 ON (t1.productno = t3.itemno)";

            if (string.IsNullOrWhiteSpace(RecNo) == false)
            {
                if (RecNo[0] == '*')
                {
                    RecNo = RecNo.Remove(0, 1);
                    wholeRecNo = true;
                }
            }

            if (wholeRecNo == true)
            {
                sql += $" where t1.pk_recno='{RecNo}' ";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(RecNo) == true)
                {
                    sql += " where t1.pk_recno<>'' ";
                }
                else
                {
                    sql += $" where t1.pk_recno like '{RecNo}%%' ";
                }
                if (ReportApproval != EReportApproval.None)
                {
                    sql += $" and t1.approval={(int)ReportApproval} ";
                }
                if (AreaNo != EReportArea.None)
                {
                    sql += $" and t1.areano={(int)AreaNo} ";
                }
                if (string.IsNullOrWhiteSpace(ProductNo) == false)
                {
                    sql += $" and t1.productno like '{ProductNo}%%' ";
                }
                if (string.IsNullOrWhiteSpace(P1FileNo) == false)
                {
                    sql += $" and t1.p1fileno like '%%{P1FileNo}' ";
                }
                if (string.IsNullOrWhiteSpace(From) == false)
                {
                    if (From == To)
                    {
                        sql += $" and t1.regtime like '{From}%%' ";
                    }
                    else
                    {
                        sql += $" and (t1.regtime>='{From} 00:00:00.000' ";
                        sql += $" and t1.regtime<='{To} 23:59:59.999') ";
                    }
                }
            }

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_ImportPysical(SqlTransaction trans = null)
        {
            bool wholeRecNo = false;
            string sql = " select t1.*, " +
                         " CASE WHEN (t2.complete = 1 and t2.complete_GiSool = 1) THEN 'yes' ELSE 'no' END as complete" +
                         " from TB_PHYMAIN t1" +
                         " INNER JOIN TB_PHYCOMPETE t2 ON (t1.pk_recno = t2.fk_phymainno)";

            if (string.IsNullOrWhiteSpace(RecNo) == false)
            {
                if (RecNo[0] == '*')
                {
                    RecNo = RecNo.Remove(0, 1);
                    wholeRecNo = true;
                }
            }

            if (wholeRecNo == true)
            {
                sql += $" where t1.pk_recno='{RecNo}' ";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(RecNo) == true)
                {
                    sql += " where t1.pk_recno<>'' ";
                }
                else
                {
                    sql += $" where t1.pk_recno like '{RecNo}%%' ";
                }
                if (ReportApproval != EReportApproval.None)
                {
                    sql += $" and t1.approval={(int)ReportApproval} ";
                }
                if (AreaNo != EReportArea.None)
                {
                    sql += $" and t1.areano={(int)AreaNo} ";
                }
                if (string.IsNullOrWhiteSpace(ProductNo) == false)
                {
                    sql += $" and t1.productno like '{ProductNo}%%' ";
                }
                if (string.IsNullOrWhiteSpace(From) == false)
                {
                    if (From == To)
                    {
                        sql += $" and t1.regtime like '{From}%%' ";
                    }
                    else
                    {
                        sql += $" and (t1.regtime>='{From} 00:00:00.000' ";
                        sql += $" and t1.regtime<='{To} 23:59:59.999') ";
                    }
                }
            }

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYMAIN values ('{RecNo}', " +
                $" '{RegTime.ToString(AppRes.csDateTimeFormat)}', '{ReceivedTime.ToString(AppRes.csDateTimeFormat)}', " +
                $" '{RequiredTime.ToString(AppRes.csDateTimeFormat)}', '{ReportedTime.ToString(AppRes.csDateTimeFormat)}', " +
                $" {Convert.ToInt32(Approval)}, {(int)AreaNo}, '{StaffNo.Replace("'", "''")}', '{ProductNo.Replace("'", "''")}', " +
                $" '{P1ClientNo.Replace("'", "''")}', '{P1ClientName.Replace("'", "''")}', '{P1ClientAddress.Replace("'", "''")}', " +
                $" '{P1FileNo.Replace("'", "''")}', '{P1SampleDescription.Replace("'", "''")}', '{P1DetailOfSample.Replace("'", "''")}', " +
                $" '{P1ItemNo.Replace("'", "''")}', '{P1OrderNo.Replace("'", "''")}', '{P1Packaging.Replace("'", "''")}', " +
                $" '{P1Instruction.Replace("'", "''")}', '{P1Buyer.Replace("'", "''")}', '{P1Manufacturer.Replace("'", "''")}', " +
                $" '{P1CountryOfOrigin.Replace("'", "''")}', '{P1CountryOfDestination.Replace("'", "''")}', '{P1LabeledAge.Replace("'", "''")}', " +
                $" '{P1TestAge.Replace("'", "''")}', '{P1AssessedAge.Replace("'", "''")}', '{P1ReceivedDate.Replace("'", "''")}', " +
                $" '{P1TestPeriod.Replace("'", "''")}', '{P1TestMethod.Replace("'", "''")}', '{P1TestResults.Replace("'", "''")}', " +
                $" '{P1Comments.Replace("'", "''")}', '{P2Name.Replace("'", "''")}', '{P3Description1.Replace("'", "''")}', " +
                $" '{P3Description2.Replace("'", "''")}', '{P4Description1.Replace("'", "''")}', '{P4Description2.Replace("'", "''")}', " +
                $" '{P4Description3.Replace("'", "''")}', '{P4Description4.Replace("'", "''")}', '{P5Description1.Replace("'", "''")}', " +
                $" '{P5Description2.Replace("'", "''")}', 'ASTM F963-23') ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PHYMAIN set approval={Convert.ToInt32(Approval)}, areano={(int)AreaNo}, staffno='{StaffNo.Replace("'", "''")}', productno='{ProductNo.Replace("'", "''")}', " +
                $" p1clientno='{P1ClientNo.Replace("'", "''")}', p1clientname='{P1ClientName.Replace("'", "''")}', p1clientaddress='{P1ClientAddress.Replace("'", "''")}', p1fileno='{P1FileNo.Replace("'", "''")}', " +
                $" p1sampledesc='{P1SampleDescription.Replace("'", "''")}', p1detailsample='{P1DetailOfSample.Replace("'", "''")}', p1itemno='{P1ItemNo.Replace("'", "''")}', p1orderno='{P1OrderNo.Replace("'", "''")}', " +
                $" p1packaging='{P1Packaging.Replace("'", "''")}', p1instruction='{P1Instruction.Replace("'", "''")}', p1buyer='{P1Buyer.Replace("'", "''")}', p1manufacturer='{P1Manufacturer.Replace("'", "''")}', " +
                $" p1countryorigin='{P1CountryOfOrigin.Replace("'", "''")}', p1countrydest='{P1CountryOfDestination.Replace("'", "''")}', p1labelage='{P1LabeledAge.Replace("'", "''")}', " +
                $" p1testage='{P1TestAge.Replace("'", "''")}', p1assessedage='{P1AssessedAge.Replace("'", "''")}', p1recevdate='{P1ReceivedDate.Replace("'", "''")}', p1testperiod='{P1TestPeriod.Replace("'", "''")}', " +
                $" p1testmethod='{P1TestMethod.Replace("'", "''")}', p1testresult='{P1TestResults.Replace("'", "''")}', p1comment='{P1Comments.Replace("'", "''")}', p2name='{P2Name.Replace("'", "''")}', " +
                $" p3desc1='{P3Description1.Replace("'", "''")}', p3desc2='{P3Description2.Replace("'", "''")}', p4desc1='{P4Description1.Replace("'", "''")}', p4desc2='{P4Description2.Replace("'", "''")}', " +
                $" p4desc3='{P4Description3.Replace("'", "''")}', p4desc4='{P4Description4.Replace("'", "''")}', p5desc1='{P5Description1.Replace("'", "''")}', p5desc2='{P5Description2.Replace("'", "''")}' " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void UpdateApproval(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PHYMAIN set " + 
                $" approval={Convert.ToInt32(Approval)}, staffno='{StaffNo}', p2name='{P2Name}' " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYMAIN   " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = "";
                RegTime = DateTime.Now;
                ReceivedTime = DateTime.Now;
                RequiredTime = DateTime.Now;
                ReportedTime = DateTime.Now;
                Approval = false;
                AreaNo = EReportArea.None;
                StaffNo = "";
                ProductNo = "";
                P1ClientNo = "";
                P1ClientName = "";
                P1ClientAddress = "";
                P1FileNo = "";
                P1SampleDescription = "";
                P1DetailOfSample = "";
                P1ItemNo = "";
                P1OrderNo = "";
                P1Packaging = "";
                P1Instruction = "";
                P1Buyer = "";
                P1Manufacturer = "";
                P1CountryOfOrigin = "";
                P1CountryOfDestination = "";
                P1LabeledAge = "";
                P1TestAge = "";
                P1AssessedAge = "";
                P1ReceivedDate = "";
                P1TestPeriod = "";
                P1TestMethod = "";
                P1TestResults = "";
                P1Comments = "";
                P2Name = "";
                P3Description1 = "";
                P3Description2 = "";
                P4Description1 = "";
                P4Description2 = "";
                P4Description3 = "";
                P5Description1 = "";
                P5Description2 = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToString(row["pk_recno"]);
            RegTime = Convert.ToDateTime(row["regtime"]);
            ReceivedTime = Convert.ToDateTime(row["receivedtime"]);
            RequiredTime = Convert.ToDateTime(row["requiredtime"]);
            ReportedTime = Convert.ToDateTime(row["reportedtime"]);
            Approval = Convert.ToBoolean(row["approval"]);
            AreaNo = (EReportArea)Convert.ToInt32(row["areano"]);
            StaffNo = Convert.ToString(row["staffno"]);
            ProductNo = Convert.ToString(row["productno"]);
            P1ClientNo = Convert.ToString(row["p1clientno"]);
            P1ClientName = Convert.ToString(row["p1clientname"]);
            P1ClientAddress = Convert.ToString(row["p1clientaddress"]);
            P1FileNo = Convert.ToString(row["p1fileno"]);
            P1SampleDescription = Convert.ToString(row["p1sampledesc"]);
            P1DetailOfSample = Convert.ToString(row["p1detailsample"]);
            P1ItemNo = Convert.ToString(row["p1itemno"]);
            P1OrderNo = Convert.ToString(row["p1orderno"]);
            P1Packaging = Convert.ToString(row["p1packaging"]);
            P1Instruction = Convert.ToString(row["p1instruction"]);
            P1Buyer = Convert.ToString(row["p1buyer"]);
            P1Manufacturer = Convert.ToString(row["p1manufacturer"]);
            P1CountryOfOrigin = Convert.ToString(row["p1countryorigin"]);
            P1CountryOfDestination = Convert.ToString(row["p1countrydest"]);
            P1LabeledAge = Convert.ToString(row["p1labelage"]);
            P1TestAge = Convert.ToString(row["p1testage"]);
            P1AssessedAge = Convert.ToString(row["p1assessedage"]);
            P1ReceivedDate = Convert.ToString(row["p1recevdate"]);
            P1TestPeriod = Convert.ToString(row["p1testperiod"]);
            P1TestMethod = Convert.ToString(row["p1testmethod"]);
            P1TestResults = Convert.ToString(row["p1testresult"]);
            P1Comments = Convert.ToString(row["p1comment"]);
            P2Name = Convert.ToString(row["p2name"]);
            P3Description1 = Convert.ToString(row["p3desc1"]);
            P3Description2 = Convert.ToString(row["p3desc2"]);
            P4Description1 = Convert.ToString(row["p4desc1"]);
            P4Description2 = Convert.ToString(row["p4desc2"]);
            P4Description3 = Convert.ToString(row["p4desc3"]);
            P4Description4 = Convert.ToString(row["p4desc4"]);
            P5Description1 = Convert.ToString(row["p5desc1"]);
            P5Description2 = Convert.ToString(row["p5desc2"]);
        }
    }

    public class PhysicalMainTapDataSet : UlSqlDataSet
    {
        public string RecNo { get; set; }

        public string Complete { get; set; }

        public DateTime RegTime { get; set; }

        public DateTime ReceivedTime { get; set; }

        public DateTime RequiredTime { get; set; }

        public DateTime ReportedTime { get; set; }

        public bool Approval { get; set; }

        public EReportArea AreaNo { get; set; }

        public string StaffNo { get; set; }

        public string ProductNo { get; set; }

        public string P1ClientNo { get; set; }

        public string P1ClientName { get; set; }

        public string P1ClientAddress { get; set; }

        public string P1FileNo { get; set; }

        public string P1SampleDescription { get; set; }

        public string P1DetailOfSample { get; set; }

        public string P1ItemNo { get; set; }

        public string P1OrderNo { get; set; }

        public string P1Packaging { get; set; }

        public string P1Instruction { get; set; }

        public string P1Buyer { get; set; }

        public string P1Manufacturer { get; set; }

        public string P1CountryOfOrigin { get; set; }

        public string P1CountryOfDestination { get; set; }

        public string P1LabeledAge { get; set; }

        public string P1TestAge { get; set; }

        public string P1AssessedAge { get; set; }

        public string P1ReceivedDate { get; set; }

        public string P1TestPeriod { get; set; }

        public string P1TestMethod { get; set; }

        public string P1TestResults { get; set; }

        public string P1Comments { get; set; }

        public string P2Name { get; set; }

        public string P3Description1 { get; set; }

        public string P3Description2 { get; set; }

        public string P4Description1 { get; set; }

        public string P4Description2 { get; set; }

        public string P4Description3 { get; set; }

        public string P4Description4 { get; set; }

        public string P5Description1 { get; set; }

        public string P5Description2 { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public EReportApproval ReportApproval { get; set; }

        public PhysicalMainTapDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            bool wholeRecNo = false;
            string sql = " select t1.*, t3.*, " +
                         " CASE WHEN (t2.complete = 1 and t2.complete_GiSool = 1) THEN 'yes' ELSE 'no' END as complete" +
                         " from TB_PHYMAIN t1" +
                         " JOIN TB_PHYCOMPETE t2 ON (t1.pk_recno = t2.fk_phymainno)" +
                         " JOIN TB_PRODUCT t3 ON (t1.productno = t3.itemno)";

            if (string.IsNullOrWhiteSpace(RecNo) == false)
            {
                if (RecNo[0] == '*')
                {
                    RecNo = RecNo.Remove(0, 1);
                    wholeRecNo = true;
                }
            }

            if (wholeRecNo == true)
            {
                sql += $" where t1.pk_recno='{RecNo}' ";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(RecNo) == true)
                {
                    sql += " where t1.pk_recno<>'' ";
                }
                else
                {
                    sql += $" where t1.pk_recno like '{RecNo}%%' ";
                }
                //if (ReportApproval != EReportApproval.None)
                //{
                //    sql += $" and t1.approval={(int)ReportApproval} ";
                //}
                //if (AreaNo != EReportArea.None)
                //{
                //    sql += $" and t1.areano={(int)AreaNo} ";
                //}
                if (string.IsNullOrWhiteSpace(ProductNo) == false)
                {
                    sql += $" and t1.productno like '{ProductNo}%%' ";
                }
                if (string.IsNullOrWhiteSpace(From) == false)
                {
                    if (From == To)
                    {
                        sql += $" and t1.regtime like '{From}%%' ";
                    }
                    else
                    {
                        sql += $" and (t1.regtime>='{From} 00:00:00.000' ";
                        sql += $" and t1.regtime<='{To} 23:59:59.999') ";
                    }
                }
            }

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_Left(SqlTransaction trans = null)
        {
            bool wholeRecNo = false;
            string sql = " select t1.*, t3.*, " +
                         " CASE WHEN (t2.complete = 1 and t2.complete_GiSool = 1) THEN 'yes' ELSE 'no' END as complete" +
                         " from TB_PHYMAIN t1" +
                         " JOIN TB_PHYCOMPETE t2 ON (t1.pk_recno = t2.fk_phymainno)" +
                         " LEFT JOIN TB_PRODUCT t3 ON (t1.productno = t3.itemno)";

            if (string.IsNullOrWhiteSpace(RecNo) == false)
            {
                if (RecNo[0] == '*')
                {
                    RecNo = RecNo.Remove(0, 1);
                    wholeRecNo = true;
                }
            }

            if (wholeRecNo == true)
            {
                sql += $" where t1.pk_recno='{RecNo}' ";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(RecNo) == true)
                {
                    sql += " where t1.pk_recno<>'' ";
                }
                else
                {
                    sql += $" where t1.pk_recno like '{RecNo}%%' ";
                }
                //if (ReportApproval != EReportApproval.None)
                //{
                //    sql += $" and t1.approval={(int)ReportApproval} ";
                //}
                //if (AreaNo != EReportArea.None)
                //{
                //    sql += $" and t1.areano={(int)AreaNo} ";
                //}
                if (string.IsNullOrWhiteSpace(ProductNo) == false)
                {
                    sql += $" and t1.productno like '{ProductNo}%%' ";
                }
                if (string.IsNullOrWhiteSpace(From) == false)
                {
                    if (From == To)
                    {
                        sql += $" and t1.regtime like '{From}%%' ";
                    }
                    else
                    {
                        sql += $" and (t1.regtime>='{From} 00:00:00.000' ";
                        sql += $" and t1.regtime<='{To} 23:59:59.999') ";
                    }
                }
            }

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYMAIN values ('{RecNo}', " +
                $" '{RegTime.ToString(AppRes.csDateTimeFormat)}', '{ReceivedTime.ToString(AppRes.csDateTimeFormat)}', " +
                $" '{RequiredTime.ToString(AppRes.csDateTimeFormat)}', '{ReportedTime.ToString(AppRes.csDateTimeFormat)}', " +
                $" {Convert.ToInt32(Approval)}, {(int)AreaNo}, '{StaffNo.Replace("'", "''")}', '{ProductNo.Replace("'", "''")}', " +
                $" '{P1ClientNo.Replace("'", "''")}', '{P1ClientName.Replace("'", "''")}', '{P1ClientAddress.Replace("'", "''")}', " +
                $" '{P1FileNo.Replace("'", "''")}', '{P1SampleDescription.Replace("'", "''")}', '{P1DetailOfSample.Replace("'", "''")}', " +
                $" '{P1ItemNo.Replace("'", "''")}', '{P1OrderNo.Replace("'", "''")}', '{P1Packaging.Replace("'", "''")}', " +
                $" '{P1Instruction.Replace("'", "''")}', '{P1Buyer.Replace("'", "''")}', '{P1Manufacturer.Replace("'", "''")}', " +
                $" '{P1CountryOfOrigin.Replace("'", "''")}', '{P1CountryOfDestination.Replace("'", "''")}', '{P1LabeledAge.Replace("'", "''")}', " +
                $" '{P1TestAge.Replace("'", "''")}', '{P1AssessedAge.Replace("'", "''")}', '{P1ReceivedDate.Replace("'", "''")}', " +
                $" '{P1TestPeriod.Replace("'", "''")}', '{P1TestMethod.Replace("'", "''")}', '{P1TestResults.Replace("'", "''")}', " +
                $" '{P1Comments.Replace("'", "''")}', '{P2Name.Replace("'", "''")}', '{P3Description1.Replace("'", "''")}', " +
                $" '{P3Description2.Replace("'", "''")}', '{P4Description1.Replace("'", "''")}', '{P4Description2.Replace("'", "''")}', " +
                $" '{P4Description3.Replace("'", "''")}', '{P4Description4.Replace("'", "''")}', '{P5Description1.Replace("'", "''")}', " +
                $" '{P5Description2.Replace("'", "''")}') ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PHYMAIN set approval={Convert.ToInt32(Approval)}, areano={(int)AreaNo}, staffno='{StaffNo.Replace("'", "''")}', productno='{ProductNo.Replace("'", "''")}', " +
                $" p1clientno='{P1ClientNo.Replace("'", "''")}', p1clientname='{P1ClientName.Replace("'", "''")}', p1clientaddress='{P1ClientAddress.Replace("'", "''")}', p1fileno='{P1FileNo.Replace("'", "''")}', " +
                $" p1sampledesc='{P1SampleDescription.Replace("'", "''")}', p1detailsample='{P1DetailOfSample.Replace("'", "''")}', p1itemno='{P1ItemNo.Replace("'", "''")}', p1orderno='{P1OrderNo.Replace("'", "''")}', " +
                $" p1packaging='{P1Packaging.Replace("'", "''")}', p1instruction='{P1Instruction.Replace("'", "''")}', p1buyer='{P1Buyer.Replace("'", "''")}', p1manufacturer='{P1Manufacturer.Replace("'", "''")}', " +
                $" p1countryorigin='{P1CountryOfOrigin.Replace("'", "''")}', p1countrydest='{P1CountryOfDestination.Replace("'", "''")}', p1labelage='{P1LabeledAge.Replace("'", "''")}', " +
                $" p1testage='{P1TestAge.Replace("'", "''")}', p1assessedage='{P1AssessedAge.Replace("'", "''")}', p1recevdate='{P1ReceivedDate.Replace("'", "''")}', p1testperiod='{P1TestPeriod.Replace("'", "''")}', " +
                $" p1testmethod='{P1TestMethod.Replace("'", "''")}', p1testresult='{P1TestResults.Replace("'", "''")}', p1comment='{P1Comments.Replace("'", "''")}', p2name='{P2Name.Replace("'", "''")}', " +
                $" p3desc1='{P3Description1.Replace("'", "''")}', p3desc2='{P3Description2.Replace("'", "''")}', p4desc1='{P4Description1.Replace("'", "''")}', p4desc2='{P4Description2.Replace("'", "''")}', " +
                $" p4desc3='{P4Description3.Replace("'", "''")}', p4desc4='{P4Description4.Replace("'", "''")}', p5desc1='{P5Description1.Replace("'", "''")}', p5desc2='{P5Description2.Replace("'", "''")}' " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void UpdateApproval(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PHYMAIN set " +
                $" approval={Convert.ToInt32(Approval)}, staffno='{StaffNo}', p2name='{P2Name}' " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYMAIN   " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = "";
                RegTime = DateTime.Now;
                ReceivedTime = DateTime.Now;
                RequiredTime = DateTime.Now;
                ReportedTime = DateTime.Now;
                Approval = false;
                AreaNo = EReportArea.None;
                StaffNo = "";
                ProductNo = "";
                P1ClientNo = "";
                P1ClientName = "";
                P1ClientAddress = "";
                P1FileNo = "";
                P1SampleDescription = "";
                P1DetailOfSample = "";
                P1ItemNo = "";
                P1OrderNo = "";
                P1Packaging = "";
                P1Instruction = "";
                P1Buyer = "";
                P1Manufacturer = "";
                P1CountryOfOrigin = "";
                P1CountryOfDestination = "";
                P1LabeledAge = "";
                P1TestAge = "";
                P1AssessedAge = "";
                P1ReceivedDate = "";
                P1TestPeriod = "";
                P1TestMethod = "";
                P1TestResults = "";
                P1Comments = "";
                P2Name = "";
                P3Description1 = "";
                P3Description2 = "";
                P4Description1 = "";
                P4Description2 = "";
                P4Description3 = "";
                P5Description1 = "";
                P5Description2 = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToString(row["pk_recno"]);
            RegTime = Convert.ToDateTime(row["regtime"]);
            ReceivedTime = Convert.ToDateTime(row["receivedtime"]);
            RequiredTime = Convert.ToDateTime(row["requiredtime"]);
            ReportedTime = Convert.ToDateTime(row["reportedtime"]);
            Approval = Convert.ToBoolean(row["approval"]);
            AreaNo = (EReportArea)Convert.ToInt32(row["areano"]);
            StaffNo = Convert.ToString(row["staffno"]);
            ProductNo = Convert.ToString(row["productno"]);
            P1ClientNo = Convert.ToString(row["p1clientno"]);
            P1ClientName = Convert.ToString(row["p1clientname"]);
            P1ClientAddress = Convert.ToString(row["p1clientaddress"]);
            P1FileNo = Convert.ToString(row["p1fileno"]);
            P1SampleDescription = Convert.ToString(row["p1sampledesc"]);
            P1DetailOfSample = Convert.ToString(row["p1detailsample"]);
            P1ItemNo = Convert.ToString(row["p1itemno"]);
            P1OrderNo = Convert.ToString(row["p1orderno"]);
            P1Packaging = Convert.ToString(row["p1packaging"]);
            P1Instruction = Convert.ToString(row["p1instruction"]);
            P1Buyer = Convert.ToString(row["p1buyer"]);
            P1Manufacturer = Convert.ToString(row["p1manufacturer"]);
            P1CountryOfOrigin = Convert.ToString(row["p1countryorigin"]);
            P1CountryOfDestination = Convert.ToString(row["p1countrydest"]);
            P1LabeledAge = Convert.ToString(row["p1labelage"]);
            P1TestAge = Convert.ToString(row["p1testage"]);
            P1AssessedAge = Convert.ToString(row["p1assessedage"]);
            P1ReceivedDate = Convert.ToString(row["p1recevdate"]);
            P1TestPeriod = Convert.ToString(row["p1testperiod"]);
            P1TestMethod = Convert.ToString(row["p1testmethod"]);
            P1TestResults = Convert.ToString(row["p1testresult"]);
            P1Comments = Convert.ToString(row["p1comment"]);
            P2Name = Convert.ToString(row["p2name"]);
            P3Description1 = Convert.ToString(row["p3desc1"]);
            P3Description2 = Convert.ToString(row["p3desc2"]);
            P4Description1 = Convert.ToString(row["p4desc1"]);
            P4Description2 = Convert.ToString(row["p4desc2"]);
            P4Description3 = Convert.ToString(row["p4desc3"]);
            P4Description4 = Convert.ToString(row["p4desc4"]);
            P5Description1 = Convert.ToString(row["p5desc1"]);
            P5Description2 = Convert.ToString(row["p5desc2"]);
        }
    }

    public class PhysicalP2DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string Requested { get; set; }

        public string Conclusion { get; set; }

        public string sDepartment { get; set; }
        
        public int iReportPage { get; set; }

        public string sReportCategory { get; set; }

        public bool bColumnLine { get; set; }

        public int iColumnNo { get; set; }

        public string sColumnName { get; set; }

        public string sColumnDesc { get; set; }

        public string sColumnClause { get; set; }

        public string sColumnResult { get; set; }

        public string sColumnRemark { get; set; }

        public string sColumnCase { get; set; }
        
        public string sButtonCase { get; set; }

        public string sSampleDescription { get; set; }

        public string sColumnComment { get; set; }

        public string sDetailOfSample { get; set; }

        public string sReportComments { get; set; }

        public string sLabeledAge { get; set; }

        public string sTestAge { get; set; }

        public string sRequestAge { get; set; }

        public string sTestRequested { get; set; }

        public string sConclusion { get; set; }

        public string sP5desc2 { get; set; }

        public int iComplete { get; set; }

        public int iComplete_Gisool { get; set; }

        public string sBurningRate { get; set; }

        public string sSample { get; set; }

        public string sResult { get; set; }

        public string sColumnDesc_4_3_7_Report_View { get; set; }

        public string sColumnDesc_4_3_7_Report_Page { get; set; }

        public string sColumnDesc_4_3_7_Result { get; set; }

        public string sP5desc3 { get; set; }

        public PhysicalP2DataSet P2Set { get; set; }

        public PhysicalP2DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP2 " +
                $" where fk_phymainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_Phy41(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP41 " +
                $" where fk_phymainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_ReportDesc(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_REPORTDESCRIPTION " +
                $" where department='{sDepartment}' " +
                $" and reportpage='{iReportPage}' " +
                $" and reportcategory='{sReportCategory}' " +
                $" and columnname='{sColumnName}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_ReportClause(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_REPORTDESCRIPTION " +
                $" where department='{sDepartment}' " +
                $" and reportpage='{iReportPage}' " +
                $" and reportcategory='{sReportCategory}' " +
                $" and columnname='{sColumnName}' " +
                $" and columnclause='{sColumnClause}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_ReportNonEmptyClause(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_REPORTDESCRIPTION " +
                $" where department='{sDepartment}' " +
                $" and reportpage='{iReportPage}' " +
                $" and reportcategory='{sReportCategory}' " +
                $" and columnname='{sColumnName}' " +
                $" and columnclause !='' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_ReportEmptyClause(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_REPORTDESCRIPTION " +
                $" where department='{sDepartment}' " +
                $" and reportpage='{iReportPage}' " +
                $" and reportcategory='{sReportCategory}' " +
                $" and columnname='{sColumnName}' " +
                $" and columnclause = '' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert_ReportDesc(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_REPORTDESCRIPTION values " +
                $" ('{sDepartment}', {iReportPage}, '{sReportCategory}', '{sColumnName}', '{sColumnDesc}', '') ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYP2 values('{MainNo}', {No}, {Convert.ToInt32(Line)}, " +
                $" '{Requested.Replace("'", "''")}', '{Conclusion.Replace("'", "''")}');  " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_PhyPage2(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYP2 values " +
                $" ('{MainNo}', {iColumnNo}, {Convert.ToInt32(bColumnLine)}, " +
                $" '{sTestRequested.Replace("'", "''")}', '{sConclusion.Replace("'", "''")}');  " +
                $" select cast(scope_identity() as bigint); ";

             SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_PhyPage3(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYP3 values " +
                $" ('{MainNo}', {iColumnNo}, {Convert.ToInt32(bColumnLine)}, '{sColumnClause.Replace("'", "''")}', " +
                $" '{sColumnDesc.Replace("'", "''")}', '{sColumnResult.Replace("'", "''")}', '{sColumnComment.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

             SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                //command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_PhyPage41(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYP41 values " +
                $" ('{MainNo}', {iColumnNo}, {Convert.ToInt32(bColumnLine)}, " +
                $" '{sSample.Replace("'", "''")}', '{sBurningRate.Replace("'", "''")}', '{sResult.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }
        public void Insert_ReportView(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYRPTVIEW values " +
                $" ('{MainNo}', {sColumnDesc_4_3_7_Report_View}, '{sColumnDesc_4_3_7_Report_Page}' ); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update_Main(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PHYMAIN set " +
                $" p1sampledesc='{sSampleDescription.Replace("'", "''")}', p1detailsample='{sDetailOfSample.Replace("'", "''")}', " +
                $" p1labelage='{sLabeledAge.Replace("'", "''")}', " +
                $" p1testage='{sRequestAge.Replace("'", "''")}', p1assessedage='{sTestAge.Replace("'", "''")}', p1comment='{sReportComments.Replace("'", "''")}' " +
                $" where pk_recno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update_Main_P5(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PHYMAIN set " +
                $" p5desc3='{sP5desc3.Replace("'", "''")}' " +
                $" where pk_recno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update_ReportDesc(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_REPORTDESCRIPTION values " +
                $" ('{sDepartment}', {iReportPage}, '{sReportCategory}', '{sColumnName}', '{sColumnDesc}', '') ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update_Polyester(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PHYMAIN set p5desc2='{sP5desc2.Replace("'", "''")}'" +
                $" where pk_recno='{MainNo}' ";

            SetTrans(trans);
            
            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update_Complete(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PHYCOMPETE set complete='{iComplete}'" +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update_Complete_Gisool(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PHYCOMPETE set complete_GiSool='{iComplete_Gisool}'" +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update_ReportView(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PHYRPTVIEW set reportview='{sColumnDesc_4_3_7_Report_View}', page='{sColumnDesc_4_3_7_Report_Page}' " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP2          " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_ReportDesc(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_REPORTDESCRIPTION " +
                $" where department='{sDepartment}'   AND" +
                $" reportpage = {iReportPage}         AND" +
                $" reportcategory = '{sReportCategory}' AND" +
                $" columnname = '{sColumnName}'         AND" +
                $" columndesc = '{sColumnDesc}'";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_PhyPage2(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP2          " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_PhyPage3(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP3          " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_PhyPage40(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP40          " +
                $" where fk_phymainno= '{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_PhyPage41(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP41          " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_PhyPage42(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP42          " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_PHYRPTVIEW(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYRPTVIEW    " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0, string sCase = "")
        {
            if (index < GetRowCount(tableNo))
            {
                if (sCase.Equals(""))
                {
                    Fetch(dataSet.Tables[tableNo].Rows[index]);
                } else if (sCase.Equals("1"))
                {
                    Fetch_ReportDesc(dataSet.Tables[tableNo].Rows[index]);
                }
                else
                {
                }
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                Requested = "";
                Conclusion = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_phymainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            Requested = Convert.ToString(row["requested"]);
            Conclusion = Convert.ToString(row["conclusion"]);
        }

        public void Fetch_ReportDesc(DataRow row)
        {
            sColumnDesc = Convert.ToString(row["columndesc"]);
            sColumnClause = Convert.ToString(row["columnclause"]);
        }
    }

    public class PhysicalP3DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string Clause { get; set; }

        public string Description { get; set; }

        public string Result { get; set; }

        public string Comment { get; set; }

        public PhysicalP3DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP3 " +
                $" where fk_phymainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectPhymainNo_No(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP3 " +
                $" where fk_phymainno='{MainNo}' " +
                $" and no='{No}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectPhymain_P3(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP3 " +
                $" where fk_phymainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYP3 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, '{Clause.Replace("'", "''")}', " +
                $" '{Description.Replace("'", "''")}', '{Result.Replace("'", "''")}', '{Comment.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                //command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP3          " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                Clause = "";
                Description = "";
                Result = "";
                Comment = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_phymainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            Clause = Convert.ToString(row["clause"]);
            Description = Convert.ToString(row["description"]);
            Result = Convert.ToString(row["result"]);
            Comment = Convert.ToString(row["comment"]);
        }
    }

    public class PhysicalP40DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string Clause { get; set; }

        public string Description { get; set; }

        public string Result { get; set; }

        public PhysicalP40DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP40 " +
                $" where fk_phymainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYP40 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, '{Clause.Replace("'", "''")}', " +
                $" '{Description.Replace("'", "''")}', '{Result.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP40         " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                Clause = "";
                Description = "";
                Result = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_phymainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            Clause = Convert.ToString(row["clause"]);
            Description = Convert.ToString(row["description"]);
            Result = Convert.ToString(row["result"]);
        }
    }

    public class PhysicalP41DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string Sample { get; set; }

        public string BurningRate { get; set; }

        public string Result { get; set; }

        public PhysicalP41DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP41 " +
                $" where fk_phymainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYP41 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, " +
                $" '{Sample.Replace("'", "''")}', '{BurningRate.Replace("'", "''")}', '{Result.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP41         " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                Sample = "";
                BurningRate = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_phymainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            Sample = Convert.ToString(row["sample"]);
            Result = Convert.ToString(row["result"]);
            BurningRate = Convert.ToString(row["burningrate"]);
        }
    }

    public class PhysicalP42DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string Sample { get; set; }

        public string BurningRate { get; set; }

        public PhysicalP42DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP42 " +
                $" where fk_phymainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        //public void Select_P45(SqlTransaction trans = null)
        //{
        //    SetTrans(trans);
        //    command.CommandText =
        //        $" select * from TB_PHYP45 " +
        //        $" where fk_phymainno='{MainNo}' " +
        //        $" order by no asc ";
        //    dataSet.Clear();
        //    dataAdapter.Fill(dataSet);
        //}

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYP42 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, " +
                $" '{Sample.Replace("'", "''")}', '{BurningRate.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP42         " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                Sample = "";
                BurningRate = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_phymainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            Sample = Convert.ToString(row["sample"]);
            BurningRate = Convert.ToString(row["burningrate"]);
        }
    }

    public class PhysicalP422DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string Sample { get; set; }

        public string Clause { get; set; }

        public string Desc { get; set; }

        public string BurningRate { get; set; }

        public string Note { get; set; }

        public PhysicalP422DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP422 " +
                $" where fk_phymainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYP422 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, " +
                $" '{Sample.Replace("'", "''")}', '{BurningRate.Replace("'", "''")}', '{Note.Replace("'", "''")}'," +
                $" '{Clause.Replace("'", "''")}', '{Desc.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP422        " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                Sample = "";
                BurningRate = "";
                Note = "";
                Clause = "";
                Desc = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_phymainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            Sample = Convert.ToString(row["sample"]);
            BurningRate = Convert.ToString(row["burningrate"]);
            Note = Convert.ToString(row["note"]);
            Clause = Convert.ToString(row["clause"]);
            Desc = Convert.ToString(row["description"]);
        }
    }

    public class PhysicalP423DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string Clause { get; set; }

        public string Desc { get; set; }

        public string Sample { get; set; }

        public string BurningRate { get; set; }

        public string Note { get; set; }

        public PhysicalP423DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP423 " +
                $" where fk_phymainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYP423 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, " +
                $" '{Sample.Replace("'", "''")}', '{BurningRate.Replace("'", "''")}', '{Note.Replace("'", "''")}'," +
                $" '{Clause.Replace("'", "''")}', '{Desc.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP423        " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                Sample = "";
                BurningRate = "";
                Note = "";
                Clause = "";
                Desc = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_phymainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            Sample = Convert.ToString(row["sample"]);
            BurningRate = Convert.ToString(row["burningrate"]);
            Note = Convert.ToString(row["note"]);
            Clause = Convert.ToString(row["clause"]);
            Desc = Convert.ToString(row["description"]);
        }
    }

    

    public class PhysicalP44DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string Sample { get; set; }

        public string Clause { get; set; }

        public string Desc { get; set; }

        public string BurningRate { get; set; }

        public string Note { get; set; }

        public PhysicalP44DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP44 " +
                $" where fk_phymainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYP44 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, " +
                $" '{Sample.Replace("'", "''")}', '{BurningRate.Replace("'", "''")}', '{Note.Replace("'", "''")}'," +
                $" '{Clause.Replace("'", "''")}', '{Desc.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP44        " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                Sample = "";
                BurningRate = "";
                Note = "";
                Clause = "";
                Desc = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_phymainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            Sample = Convert.ToString(row["sample"]);
            BurningRate = Convert.ToString(row["burningrate"]);
            Note = Convert.ToString(row["note"]);
            Clause = Convert.ToString(row["clause"]);
            Desc = Convert.ToString(row["description"]);
        }
    }

    public class PhysicalP45DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string Clause { get; set; }

        public string Desc { get; set; }

        public string Sample { get; set; }

        public string BurningRate { get; set; }

        public string Result { get; set; }

        public string Note { get; set; }

        public PhysicalP45DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP45 " +
                $" where fk_phymainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYP45 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, " +
                $" '{Sample.Replace("'", "''")}', '{BurningRate.Replace("'", "''")}', '{Result.Replace("'", "''")}', " +
                $" '{Note.Replace("'", "''")}'  , '{Clause.Replace("'", "''")}'     , '{Desc.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP45        " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                Sample = "";
                BurningRate = "";
                Result = "";
                Note = "";
                Clause = "";
                Desc = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_phymainno"]);
            No = Convert.ToInt32(row["no"]);
            Line = Convert.ToBoolean(row["line"]);
            Sample = Convert.ToString(row["sample"]);
            BurningRate = Convert.ToString(row["burningrate"]);
            Result = Convert.ToString(row["result"]);
            Note = Convert.ToString(row["note"]);
            Clause = Convert.ToString(row["clause"]);
            Desc = Convert.ToString(row["description"]);
        }
    }

    public class PhysicalP5DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string TestItem { get; set; }

        public string Result { get; set; }

        public string Requirement { get; set; }

        public int iReportView { get; set; }

        public string sPage { get; set; }

        public string sPhyComplete { get; set; }

        public string sPhyComplete_GiSool { get; set; }

        public string sResult { get; set; }

        public PhysicalP5DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP5 " +
                $" where fk_phymainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectPhymainNo_No(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP5 " +
                $" where fk_phymainno='{MainNo}' " +
                $" and no='{No}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_ReportView(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYRPTVIEW " +
                $" where fk_phymainno='{MainNo}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_PhyComplete(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYCOMPETE " +
                $" where fk_phymainno='{MainNo}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYP5 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, '{TestItem.Replace("'", "''")}', " +
                $" '{Result.Replace("'", "''")}', '{Requirement.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_ReportView(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYRPTVIEW values " +
                $" ('{MainNo}', {iReportView}, '{sPage}' ); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_PhyComplete(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYCOMPETE values " +
                $" ('{MainNo}', '{sPhyComplete}', '{sPhyComplete_GiSool}' ); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                //RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update_P5_Result(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_PHYP5 set result='{Result}' " +
                $" where fk_phymainno='{MainNo}' and line = '1'";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP5          " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_PHYRPTVIEW(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYRPTVIEW    " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_PHYCOMPLETE(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYCOMPETE     " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0, string sCase = "")
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index], sCase);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                TestItem = "";
                Result = "";
                Requirement = "";
            }
        }

        public void Fetch(DataRow row, string sCase = "")
        {
            if (sCase.Equals(""))
            {
                RecNo = Convert.ToInt64(row["pk_recno"]);
                MainNo = Convert.ToString(row["fk_phymainno"]);
                No = Convert.ToInt32(row["no"]);
                Line = Convert.ToBoolean(row["line"]);
                TestItem = Convert.ToString(row["testitem"]);
                Result = Convert.ToString(row["result"]);
                Requirement = Convert.ToString(row["requirement"]);
            }
            else if (sCase.Equals("ReportView"))
            {
                MainNo = Convert.ToString(row["fk_phymainno"]);
                iReportView = Convert.ToInt32(row["reportview"]);
            }
            else if (sCase.Equals("PhyComplete"))
            {
                MainNo = Convert.ToString(row["fk_phymainno"]);
                sPhyComplete = Convert.ToString(row["complete"]);
                sPhyComplete_GiSool = Convert.ToString(row["complete_GiSool"]);
            }
            else
            {
            }
        }
    }

    public class PhysicalP6DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public int No { get; set; }

        public bool Line { get; set; }

        public string TestItem { get; set; }

        public string Result { get; set; }

        public string Requirement { get; set; }

        public string Note { get; set; }

        public string Description { get; set; }

        public int iReportView { get; set; }

        public string sPhyComplete { get; set; }

        public string sPhyComplete_GiSool { get; set; }

        public PhysicalP6DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP6 " +
                $" where fk_phymainno='{MainNo}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectPhymainNo_No(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP6 " +
                $" where fk_phymainno='{MainNo}' " +
                $" and no='{No}' " +
                $" order by no asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }        

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYP6 values " +
                $" ('{MainNo}', {No}, {Convert.ToInt32(Line)}, '{TestItem.Replace("'", "''")}', " +
                $" '{Result.Replace("'", "''")}', '{Requirement.Replace("'", "''")}', '{Note.Replace("'", "''")}', '{Description.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP6          " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }        

        public void Fetch(int index = 0, int tableNo = 0, string sCase = "")
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index], sCase);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                No = 0;
                Line = false;
                TestItem = "";
                Result = "";
                Requirement = "";
            }
        }

        public void Fetch(DataRow row, string sCase = "")
        {
            if (sCase.Equals(""))
            {
                RecNo = Convert.ToInt64(row["pk_recno"]);
                MainNo = Convert.ToString(row["fk_phymainno"]);
                No = Convert.ToInt32(row["no"]);
                Line = Convert.ToBoolean(row["line"]);
                TestItem = Convert.ToString(row["testitem"]);
                Result = Convert.ToString(row["result"]);
                Requirement = Convert.ToString(row["requirement"]);
            }
            else if (sCase.Equals("ReportView"))
            {
                MainNo = Convert.ToString(row["fk_phymainno"]);
                iReportView = Convert.ToInt32(row["reportview"]);
            }
            else if (sCase.Equals("PhyComplete"))
            {
                MainNo = Convert.ToString(row["fk_phymainno"]);
                sPhyComplete = Convert.ToString(row["complete"]);
                sPhyComplete_GiSool = Convert.ToString(row["complete_GiSool"]);
            }
            else
            {
            }
        }
    }

    public class PhysicalP7DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public string sBasicInformation { get; set; }

        public string sHowtoComply { get; set; }

        public string sResults { get; set; }

        public PhysicalP7DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYP7 " +
                $" where fk_phymainno='{MainNo}' " +
                $" order by fk_phymainno asc ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYP7 values " +
                $" ('{MainNo}', " +
                $" '{sBasicInformation.Replace("'", "''")}', '{sHowtoComply.Replace("'", "''")}', '{sResults.Replace("'", "''")}'); " +
                $" select cast(scope_identity() as bigint); ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }
                
        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYP7          " +
                $" where fk_phymainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                sBasicInformation = "";
                sHowtoComply = "";
                sResults = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_phymainno"]);
            sBasicInformation = Convert.ToString(row["basicinformation"]);
            sHowtoComply = Convert.ToString(row["howtocomply"]);
            sResults = Convert.ToString(row["results"]);
        }
    }
    public class PhysicalImageDataSet : UlSqlDataSet
    {
        public string RecNo { get; set; }

        public Bitmap Signature { get; set; }

        public Bitmap Picture { get; set; }

        private ImageConverter imageConvert;

        public PhysicalImageDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
            imageConvert = new ImageConverter();
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_PHYIMAGE " +
                $" where pk_recno='{RecNo}'  ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_PHYIMAGE values " +
                $" ('{RecNo}', @signature, @picture) ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);

                command.CommandText = sql;
                command.Parameters.Clear();
                if (Signature == null)
                {
                    SqlParameter signatureParam = new SqlParameter("@signature", SqlDbType.Image);
                    signatureParam.Value = DBNull.Value;
                    command.Parameters.Add(signatureParam);
                }
                else
                {
                    command.Parameters.Add("@signature", SqlDbType.Image);
                    command.Parameters["@signature"].Value = (byte[])imageConvert.ConvertTo(Signature, typeof(byte[]));
                }

                if (Picture == null)
                {
                    SqlParameter pictureParam = new SqlParameter("@picture", SqlDbType.Image);
                    pictureParam.Value = DBNull.Value;
                    command.Parameters.Add(pictureParam);
                }
                else
                {
                    command.Parameters.Add("@picture", SqlDbType.Image);
                    command.Parameters["@picture"].Value = (byte[])imageConvert.ConvertTo(Picture, typeof(byte[]));
                }
                command.ExecuteNonQuery();

                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_PHYIMAGE      " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = "";
                Signature = null;
                Picture = null;
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToString(row["pk_recno"]);

            if (row["signature"] == DBNull.Value) Signature = null;
            else
            {
                byte[] signatureRaw = (byte[])row["signature"];
                Signature = (signatureRaw == null) ? null : new Bitmap(new MemoryStream(signatureRaw));
            }

            if (row["picture"] == DBNull.Value) Picture = null;
            else
            {
                byte[] pictureRaw = (byte[])row["picture"];
                Picture = (pictureRaw == null) ? null : new Bitmap(new MemoryStream(pictureRaw));
            }
        }
    }

    public class ChemicalReportDataSet : UlSqlDataSet
    {
        public string RecNo { get; set; }

        public string Pro_proj { get; set; }

        public string Sam_Remarks { get; set; }

        public ChemicalReportDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEMAIN where pk_recno='{RecNo}'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}'; " +
                $" select * from TB_CHEP2EXTEND where pk_recno='{RecNo}'; " +
                $" select * from TB_CHEIMAGE where pk_recno='{RecNo}'; " +

                // Report 6페이지 이상 출력
                $" select * from TB_CHEP2_HYPHEN_EN where fk_chemainno='{RecNo}' and (no>=1 and no<=4);   " +
                $" select * from TB_CHEP2_HYPHEN_EN where fk_chemainno='{RecNo}' and (no>=5 and no<=8);   " +
                $" select * from TB_CHEP2_HYPHEN_EN where fk_chemainno='{RecNo}' and (no>=9 and no<=12);  " +
                $" select * from TB_CHEP2_HYPHEN_EN where fk_chemainno='{RecNo}' and (no>=13 and no<=16);  " +
                $" select * from TB_CHEP2_HYPHEN_EN where fk_chemainno='{RecNo}' and (no>=17 and no<=20);  " +
                $" select * from TB_CHEP2_HYPHEN_EN where fk_chemainno='{RecNo}' and (no>=21 and no<=24);  " +
                // Report 6페이지 이상 출력

                // Report limit  출력 - 시작
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Al)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(As)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(B)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Ba)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Cd)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Co)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Cr)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(III)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(VI)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Cu)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Hg)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Mn)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Ni)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Pb)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Sb)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Se)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Sn)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Sr)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Zn)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%Organic Tin%'; " +
                // Report limit  출력 - 끝

                //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and (no>=1 and no<=4);   " +
                //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and (no>=5 and no<=8);   " +
                //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and (no>=9 and no<=12);   " +
                //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and (no>=13 and no<=16);   " +
                //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and (no>=17 and no<=20);   " +
                //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and (no>=21 and no<=24);   " +

                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_CHEPTIN_EN] WHERE fk_chemainno='{RecNo}') t WHERE t.rownum BETWEEN 1 AND 3;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_CHEPTIN_EN] WHERE fk_chemainno='{RecNo}') t WHERE t.rownum BETWEEN 4 AND 6;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_CHEPTIN_EN] WHERE fk_chemainno='{RecNo}') t WHERE t.rownum BETWEEN 7 AND 9;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_CHEPTIN_EN] WHERE fk_chemainno='{RecNo}') t WHERE t.rownum BETWEEN 10 AND 12;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_CHEPTIN_EN] WHERE fk_chemainno='{RecNo}') t WHERE t.rownum BETWEEN 13 AND 15;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_CHEPTIN_EN] WHERE fk_chemainno='{RecNo}') t WHERE t.rownum BETWEEN 16 AND 18;" +

                // Report limit  출력 - 시작
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(MET)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(DBT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(TBT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(TeBT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(MOT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(DOT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(DProT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(DPhT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(TPhT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(DMT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(MBT)%'; ";

            //// Report limit  출력 - 시작
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(DMT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(MeT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(DProT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(BuT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(DBT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(TBT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(MOT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(DOT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(TeBT)%'; ";
            // Report limit  출력 - 끝

            dataSet.Clear();
            dataSet.Tables.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_US(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEMAIN where pk_recno='{RecNo}'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}'; " +
                $" select * from TB_CHEP2EXTEND where pk_recno='{RecNo}'; " +
                $" select * from TB_CHEIMAGE where pk_recno='{RecNo}'; " +

                // Report Lead Limit 출력 - coating, plastic, metal 순으로
                $" select top 1 * from TB_CHEP2_LEAD_LIMIT_ASTM where pro_proj like '{Pro_proj}%%' and sam_remarks = 'coating';   " +
                $" select top 1 * from TB_CHEP2_LEAD_LIMIT_ASTM where pro_proj like '{Pro_proj}%%' and sam_remarks = 'plastic';   " +
                $" select top 1 * from TB_CHEP2_LEAD_LIMIT_ASTM where pro_proj like '{Pro_proj}%%' and sam_remarks = 'metal';   " +

                // Report Lead Result 출력 - coating, plastic, metal 순으로
                $" select * from TB_CHEP2_LEAD_RESULT_ASTM where pro_proj like '{Pro_proj}%%' and sam_remarks = 'coating'; " +
                $" select * from TB_CHEP2_LEAD_RESULT_ASTM where pro_proj like '{Pro_proj}%%' and sam_remarks = 'plastic';   " +
                $" select * from TB_CHEP2_LEAD_RESULT_ASTM where pro_proj like '{Pro_proj}%%' and sam_remarks = 'metal';   " +

                // Report Not Lead LIMIT 출력 - coating인 것과 coating 아닌 것 순으로
                $" select * from TB_CHEP2_LIMIT_ASTM where pro_proj like '{Pro_proj}%%' and sam_remarks = 'coating' order by" +
                $" (case when name = 'Pb' then 1 " +
                $" when name = 'Sb' then 2 " +
                $" when name = 'As' then 3 " +
                $" when name = 'Ba' then 4 " +
                $" when name = 'Cd' then 5 " +
                $" when name = 'Cr' then 6 " +
                $" when name = 'Hg' then 7 " +
                $" when name = 'Se' then 8 " +
                $" else 9 end ); " +

                $" select * from TB_CHEP2_LIMIT_ASTM where pro_proj like '{Pro_proj}%%' and sam_remarks <> 'coating' order by" +
                $" (case when name = 'Pb' then 1 " +
                $" when name = 'Sb' then 2 " +
                $" when name = 'As' then 3 " +
                $" when name = 'Ba' then 4 " +
                $" when name = 'Cd' then 5 " +
                $" when name = 'Cr' then 6 " +
                $" when name = 'Hg' then 7 " +
                $" when name = 'Se' then 8 " +
                $" else 9 end ); " +

                // Report Not Lead Result 출력 - coating인 것과 coating 아닌 것 순으로
                $" select * from TB_CHEP2_RESULT_ASTM where pro_proj like '{Pro_proj}%%' and sam_remarks = 'coating';   " +
                $" select * from TB_CHEP2_RESULT_ASTM where pro_proj like '{Pro_proj}%%' and sam_remarks <> 'coating';   " +
                $" select * from TB_CHEP2_LEAD_RESULT_ASTM where pro_proj like '{Pro_proj}%%' and sam_remarks <> 'coating';   " +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_CHEPPHT_ASTM] WHERE pro_proj like '{Pro_proj}%%' and sam_remarks = 'plastic') t WHERE t.rownum BETWEEN 1 AND 1;" +

                $" SELECT TOP 1 * from TB_CHEMAIN where p1fileno like '{Pro_proj}%%' order by p1fileno desc; " +
                $" SELECT TOP 1 * from TB_CHEMAIN where p1fileno like '{Pro_proj}%%' order by p1fileno asc;" +
                $" IF (CAST((SELECT COUNT(*) AS 'PK_CNT' from TB_CHEMAIN where p1fileno like '{Pro_proj}%%') AS INT)  > 1)" +
                $"      BEGIN" +
                $"          IF ((SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 requiredtime from TB_CHEMAIN where p1fileno = '{Pro_proj}'),1,10))AS 'requiredtime' FROM TB_CHEMAIN) > (SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 requiredtime from TB_CHEMAIN where p1fileno = '{Pro_proj}-1'),1,10))AS 'requiredTime' FROM TB_CHEMAIN))" +
                $"             BEGIN" +
                $"                  (SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 requiredtime from TB_CHEMAIN where p1fileno = '{Pro_proj}'),1,10))AS 'requiredtime' FROM TB_CHEMAIN)" +
                $"             END" +
                $"          ELSE" +
                $"             BEGIN" +
                $"                  (SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 requiredtime from TB_CHEMAIN where p1fileno = '{Pro_proj}-1'),1,10))AS 'requiredtime' FROM TB_CHEMAIN)" +
                $"             END" +
                $"      END" +
                $" ELSE" +
                $"      BEGIN" +
                $"             SELECT * from TB_CHEMAIN where p1fileno like '{Pro_proj}%%';" +
                $"      END " +
                $" IF (CAST((SELECT COUNT(*) AS 'PK_CNT' from TB_CHEMAIN where p1fileno like '{Pro_proj}%%') AS INT)  > 1)" +
                $"      BEGIN" +
                $"          IF ((SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 requiredtime from TB_CHEMAIN where p1fileno = '{Pro_proj}'),1,10))AS 'requiredtime' FROM TB_CHEMAIN) > (SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 requiredtime from TB_CHEMAIN where p1fileno = '{Pro_proj}-1'),1,10))AS 'requiredTime' FROM TB_CHEMAIN))" +
                $"             BEGIN" +
                $"                  SELECT CONCAT((SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 receivedtime from TB_CHEMAIN where p1fileno = '{Pro_proj}'),1,10))AS 'receivedtime' FROM TB_CHEMAIN), ' to ', (SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 requiredtime from TB_CHEMAIN where p1fileno = '{Pro_proj}'),1,10))AS 'requiredtime' FROM TB_CHEMAIN))  AS TestPeriod " +
                $"             END" +
                $"          ELSE" +
                $"             BEGIN" +
                $"                  SELECT CONCAT((SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 receivedtime from TB_CHEMAIN where p1fileno = '{Pro_proj}'),1,10))AS 'receivedtime' FROM TB_CHEMAIN), ' to ', (SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 requiredtime from TB_CHEMAIN where p1fileno = '{Pro_proj}-1'),1,10))AS 'requiredtime' FROM TB_CHEMAIN))  AS TestPeriod " +
                $"             END" +
                $"      END" +
                $" ELSE" +
                $"      BEGIN" +
                $"             SELECT CONCAT((SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 receivedtime from TB_CHEMAIN where p1fileno = '{Pro_proj}'),1,10))AS 'receivedtime' FROM TB_CHEMAIN), ' to ', (SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 requiredtime from TB_CHEMAIN where p1fileno = '{Pro_proj}'),1,10))AS 'requiredtime' FROM TB_CHEMAIN))  AS TestPeriod;" +
                $"      END ";

            /*
             IF (CAST((SELECT COUNT(*) AS 'PK_CNT' from TB_CHEMAIN where p1fileno like 'AYHA24-05210%%') AS INT)  > 1)
             BEGIN
               IF ((SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 requiredtime from TB_CHEMAIN where p1fileno = 'AYHA24-05210'),1,10))AS 'requiredtime' FROM TB_CHEMAIN) > (SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 requiredtime from TB_CHEMAIN where p1fileno = 'AYHA24-05210-1'),1,10))AS 'requiredTime' FROM TB_CHEMAIN))
                   BEGIN
                        (SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 requiredtime from TB_CHEMAIN where p1fileno = 'AYHA24-05210'),1,10))AS 'requiredtime' FROM TB_CHEMAIN)
                   END
                ELSE
                    BEGIN
                        (SELECT TOP 1 CONVERT(DATE, SUBSTRING((SELECT TOP 1 requiredtime from TB_CHEMAIN where p1fileno = 'AYHA24-05210-1'),1,10))AS 'requiredtime' FROM TB_CHEMAIN)
                    END
            END
            ELSE
            BEGIN
                SELECT * from TB_CHEMAIN where p1fileno like 'AYHA24-05210%%';
            END 
             */
            /*
                $" select * from TB_CHEP2_HYPHEN_EN where fk_chemainno='{RecNo}' and (no>=5 and no<=8);   " +
                $" select * from TB_CHEP2_HYPHEN_EN where fk_chemainno='{RecNo}' and (no>=9 and no<=12);  " +
                $" select * from TB_CHEP2_HYPHEN_EN where fk_chemainno='{RecNo}' and (no>=13 and no<=16);  " +
                $" select * from TB_CHEP2_HYPHEN_EN where fk_chemainno='{RecNo}' and (no>=17 and no<=20);  " +
                $" select * from TB_CHEP2_HYPHEN_EN where fk_chemainno='{RecNo}' and (no>=21 and no<=24);  " +
                // Report 6페이지 이상 출력

                // Report limit  출력 - 시작
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Al)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(As)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(B)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Ba)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Cd)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Co)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Cr)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(III)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(VI)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Cu)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Hg)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Mn)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Ni)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Pb)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Sb)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Se)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Sn)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Sr)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(Zn)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%Organic Tin%'; " +
                // Report limit  출력 - 끝

                //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and (no>=1 and no<=4);   " +
                //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and (no>=5 and no<=8);   " +
                //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and (no>=9 and no<=12);   " +
                //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and (no>=13 and no<=16);   " +
                //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and (no>=17 and no<=20);   " +
                //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and (no>=21 and no<=24);   " +

                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_CHEPTIN_EN] WHERE fk_chemainno='{RecNo}') t WHERE t.rownum BETWEEN 1 AND 3;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_CHEPTIN_EN] WHERE fk_chemainno='{RecNo}') t WHERE t.rownum BETWEEN 4 AND 6;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_CHEPTIN_EN] WHERE fk_chemainno='{RecNo}') t WHERE t.rownum BETWEEN 7 AND 9;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_CHEPTIN_EN] WHERE fk_chemainno='{RecNo}') t WHERE t.rownum BETWEEN 10 AND 12;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_CHEPTIN_EN] WHERE fk_chemainno='{RecNo}') t WHERE t.rownum BETWEEN 13 AND 15;" +
                $" SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY no asc) rownum, * FROM [ReportIntegration].[dbo].[TB_CHEPTIN_EN] WHERE fk_chemainno='{RecNo}') t WHERE t.rownum BETWEEN 16 AND 18;" +

                // Report limit  출력 - 시작
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(MET)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(DBT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(TBT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(TeBT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(MOT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(DOT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(DProT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(DPhT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(TPhT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(DMT)%'; " +
                $" select * from TB_CHEP2 where fk_chemainno='{RecNo}' and name like '%(MBT)%'; ";

            //// Report limit  출력 - 시작
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(DMT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(MeT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(DProT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(BuT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(DBT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(TBT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(MOT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(DOT)%'; " +
            //$" select * from TB_CHEPTIN_EN where fk_chemainno='{RecNo}' and name like '%(TeBT)%'; ";
            // Report limit  출력 - 끝
            */
            dataSet.Clear();
            dataSet.Tables.Clear();
            dataAdapter.Fill(dataSet);
        }
    }   

    public class ChemicalMainDataSet : UlSqlDataSet
    {
        public string RecNo { get; set; }

        public DateTime RegTime { get; set; }

        public DateTime ReceivedTime { get; set; }

        public DateTime RequiredTime { get; set; }

        public DateTime ReportedTime { get; set; }

        public bool Approval { get; set; }

        public ELeadType LeadType { get; set; }

        public EReportArea AreaNo { get; set; }

        public string StaffNo { get; set; }

        public string MaterialNo { get; set; }

        public string P1ClientNo { get; set; }

        public string P1ClientName { get; set; }

        public string P1ClientAddress { get; set; }

        public string P1FileNo { get; set; }

        public string P1SampleDescription { get; set; }

        public string P1ItemNo { get; set; }

        public string P1OrderNo { get; set; }

        public string P1Manufacturer { get; set; }

        public string P1CountryOfOrigin { get; set; }

        public string P1CountryOfDestination { get; set; }

        public string P1ReceivedDate { get; set; }

        public string P1TestPeriod { get; set; }

        public string P1TestMethod { get; set; }

        public string P1TestResults { get; set; }

        public string P1Comments { get; set; }

        public string P1TestRequested { get; set; }

        public string P1Conclusion { get; set; }

        public string P1Name { get; set; }

        public string P1SampleRemark { get; set; }

        public string P2Description1 { get; set; }

        public string P2Description2 { get; set; }

        public string P2Description3 { get; set; }

        public string P2Description4 { get; set; }

        public string P3Description1 { get; set; }

        public string P4Description1 { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public EReportApproval ReportApproval { get; set; }

        public ChemicalMainDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            string sql = " select t1.* from TB_CHEMAIN t1 ";

            if (string.IsNullOrWhiteSpace(MaterialNo) == false)
            {
                sql += " join TB_CHEPARTJOIN t2 on t2.pk_recno=t1.pk_recno ";
            }

            if (string.IsNullOrWhiteSpace(RecNo) == true)
            {
                sql += " where t1.pk_recno<>'' ";
            }
            else
            {
                sql += $" where t1.pk_recno like '{RecNo}%%' ";
            }

            if (ReportApproval != EReportApproval.None)
            {
                sql += $" and t1.approval={(int)ReportApproval} ";
            }
            if (AreaNo != EReportArea.None)
            {
                sql += $" and t1.areano={(int)AreaNo} ";
            }
            if (string.IsNullOrWhiteSpace(P1FileNo) == false)
            {
                sql += $" and t1.p1fileno like '%%{P1FileNo}' ";
            }
            if (string.IsNullOrWhiteSpace(From) == false)
            {
                if (From == To)
                {
                    sql += $" and t1.regtime like '{From}%%' ";
                }
                else
                {
                    sql += $" and (t1.regtime>='{From} 00:00:00.000' ";
                    sql += $" and t1.regtime<='{To} 23:59:59.999') ";
                }
            }
            if (string.IsNullOrWhiteSpace(MaterialNo) == false)
            {
                sql += $" and t2.pk_partno='{MaterialNo}' ";
            }

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectRecno(SqlTransaction trans = null)
        {
            string sql = $" select * from TB_CHEMAIN " +
                         $" where pk_recno = '{RecNo}' ";

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select(string recno, SqlTransaction trans = null)
        {
            string sql = 
                $" select t1.* from TB_CHEMAIN t1 " +
                $" where t1.pk_recno = '{recno}'    ";
            //$" where t1.pk_recno like '%{recno}%'    ";

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_Chep2(string recno, SqlTransaction trans = null)
        {            
            command.CommandText =
                $" select * from TB_CHEP2_HYPHEN_EN " +
                $" where fk_chemainno = '{recno}'";
            SetTrans(trans);
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_CHEMAIN values ( " +
                $" '{RecNo}', '{RegTime.ToString(AppRes.csDateTimeFormat)}', '{ReceivedTime.ToString(AppRes.csDateTimeFormat)}', " +
                $" '{RequiredTime.ToString(AppRes.csDateTimeFormat)}', '{ReportedTime.ToString(AppRes.csDateTimeFormat)}', " +
                $" {Convert.ToInt32(Approval)}, {(int)LeadType}, {(int)AreaNo}, '{StaffNo.Replace("'", "''")}', '{MaterialNo.Replace("'", "''")}', " +
                $" '{P1ClientNo.Replace("'", "''")}', '{P1ClientName.Replace("'", "''")}', '{P1ClientAddress.Replace("'", "''")}', " +
                $" '{P1FileNo.Replace("'", "''")}', '{P1SampleDescription.Replace("'", "''")}', '{P1ItemNo.Replace("'", "''")}', " +
                $" '{P1OrderNo.Replace("'", "''")}', '{P1Manufacturer.Replace("'", "''")}', '{P1CountryOfOrigin.Replace("'", "''")}', " +
                $" '{P1CountryOfDestination.Replace("'", "''")}', '{P1ReceivedDate.Replace("'", "''")}', '{P1TestPeriod.Replace("'", "''")}', " +
                $" '{P1TestMethod.Replace("'", "''")}', '{P1TestResults.Replace("'", "''")}', '{P1Comments.Replace("'", "''")}', " +
                $" '{P1TestRequested.Replace("'", "''")}', '{P1Conclusion.Replace("'", "''")}', '{P1Name.Replace("'", "''")}', " +
                $" '{P2Description1.Replace("'", "''")}', '{P2Description2.Replace("'", "''")}', '{P2Description3.Replace("'", "''")}', " +
                $" '{P2Description4.Replace("'", "''")}', '{P3Description1.Replace("'", "''")}', '{P4Description1.Replace("'", "''")}') ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_CHEMAIN set " +
                $" approval={Convert.ToInt32(Approval)}, areano={(int)AreaNo}, staffno='{StaffNo.Replace("'", "''")}', " +
                $" productno='{MaterialNo.Replace("'", "''")}', p1clientno='{P1ClientNo.Replace("'", "''")}', " +
                $" p1clientname='{P1ClientName.Replace("'", "''")}', p1clientaddress='{P1ClientAddress.Replace("'", "''")}', " +
                $" p1fileno='{P1FileNo.Replace("'", "''")}', p1sampledesc ='{P1SampleDescription.Replace("'", "''")}', " +
                $" p1itemno='{P1ItemNo.Replace("'", "''")}', p1orderno='{P1OrderNo.Replace("'", "''")}', " +
                $" p1manufacturer='{P1Manufacturer.Replace("'", "''")}', p1countryorigin='{P1CountryOfOrigin.Replace("'", "''")}', " +
                $" p1countrydest='{P1CountryOfDestination.Replace("'", "''")}', p1recevdate='{P1ReceivedDate.Replace("'", "''")}', " +
                $" p1testperiod='{P1TestPeriod.Replace("'", "''")}', p1testmethod='{P1TestMethod.Replace("'", "''")}', " +
                $" p1testresult='{P1TestResults.Replace("'", "''")}', p1comment='{P1Comments.Replace("'", "''")}', " +
                $" p1testrequested='{P1TestRequested.Replace("'", "''")}', p1conclusion='{P1Conclusion.Replace("'", "''")}', " +
                $" p1name='{P1Name.Replace("'", "''")}', p2desc1='{P2Description1.Replace("'", "''")}', " +
                $" p2desc2='{P2Description2.Replace("'", "''")}', p2desc3='{P2Description3.Replace("'", "''")}', " +
                $" p2desc4='{P2Description4.Replace("'", "''")}', p3desc1='{P3Description1.Replace("'", "''")}' " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void UpdateApproval(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_CHEMAIN set " +
                $" approval={Convert.ToInt32(Approval)}, staffno='{StaffNo}', p1name='{P1Name}' " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void UpdateP2Description3(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_CHEMAIN set " +
                $" p1testrequested='{P1TestRequested.Replace("'", "''")}', p1conclusion='{P1Conclusion.Replace("'", "''")}' " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void UpdateP2Description3_SubJob(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_CHEMAIN set " +
                $" p1testrequested='{P1TestRequested.Replace("'", "''")}', p1conclusion='{P1Conclusion.Replace("'", "''")}' " +
                $" where p1fileno = '{P1FileNo.Substring(0,12)}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_CHEMAIN " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            //int test = 0;
            //test = GetRowCount(tableNo);
            if (index < GetRowCount(tableNo))
            {                
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = "";
                RegTime = DateTime.Now;
                ReceivedTime = DateTime.Now;
                RequiredTime = DateTime.Now;
                ReportedTime = DateTime.Now;
                Approval = false;
                LeadType = ELeadType.None;
                AreaNo = EReportArea.None;
                StaffNo = "";
                MaterialNo = "";
                P1ClientNo = "";
                P1ClientName = "";
                P1ClientAddress = "";
                P1FileNo = "";
                P1SampleDescription = "";
                P1ItemNo = "";
                P1OrderNo = "";
                P1Manufacturer = "";
                P1CountryOfOrigin = "";
                P1CountryOfDestination = "";
                P1ReceivedDate = "";
                P1TestPeriod = "";
                P1TestMethod = "";
                P1TestResults = "";
                P1Comments = "";
                P1TestRequested = "";
                P1Conclusion = "";
                P1Name = "";
                P2Description1 = "";
                P2Description2 = "";
                P2Description3 = "";
                P2Description4 = "";
                P3Description1 = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToString(row["pk_recno"]).Trim();
            RegTime = Convert.ToDateTime(row["regtime"]);
            ReceivedTime = Convert.ToDateTime(row["receivedtime"]);
            RequiredTime = Convert.ToDateTime(row["requiredtime"]);
            ReportedTime = Convert.ToDateTime(row["reportedtime"]);
            Approval = Convert.ToBoolean(row["approval"]);
            LeadType = (ELeadType)Convert.ToInt32(row["leadtype"]);
            AreaNo = (EReportArea)Convert.ToInt32(row["areano"]);
            StaffNo = Convert.ToString(row["staffno"]);
            MaterialNo = Convert.ToString(row["productno"]);
            P1ClientNo = Convert.ToString(row["p1clientno"]);
            P1ClientName = Convert.ToString(row["p1clientname"]);
            P1ClientAddress = Convert.ToString(row["p1clientaddress"]);
            P1FileNo = Convert.ToString(row["p1fileno"]);
            P1SampleDescription = Convert.ToString(row["p1sampledesc"]);
            P1ItemNo = Convert.ToString(row["p1itemno"]);
            P1OrderNo = Convert.ToString(row["p1orderno"]);
            P1Manufacturer = Convert.ToString(row["p1manufacturer"]);
            P1CountryOfOrigin = Convert.ToString(row["p1countryorigin"]);
            P1CountryOfDestination = Convert.ToString(row["p1countrydest"]);
            P1ReceivedDate = Convert.ToString(row["p1recevdate"]);
            P1TestPeriod = Convert.ToString(row["p1testperiod"]);
            P1TestMethod = Convert.ToString(row["p1testmethod"]);
            P1TestResults = Convert.ToString(row["p1testresult"]);
            P1Comments = Convert.ToString(row["p1comment"]);
            P1TestRequested = Convert.ToString(row["p1testrequested"]);
            P1Conclusion = Convert.ToString(row["p1conclusion"]);
            P1Name = Convert.ToString(row["p1name"]);
            P2Description1 = Convert.ToString(row["p2desc1"]);
            P2Description2 = Convert.ToString(row["p2desc2"]);
            P2Description3 = Convert.ToString(row["p2desc3"]);
            P2Description4 = Convert.ToString(row["p2desc4"]);
            P3Description1 = Convert.ToString(row["p3desc1"]);
        }
    }

    public class ChemicalImageDataSet : UlSqlDataSet
    {
        public string RecNo { get; set; }

        public Bitmap Signature { get; set; }

        public Bitmap Picture { get; set; }

        private ImageConverter imageConvert;

        public ChemicalImageDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
            imageConvert = new ImageConverter();
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEIMAGE " +
                $" where pk_recno='{RecNo}'  ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_CHEIMAGE values    " +
                $" ('{RecNo}', @signature, @picture) ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                
                command.CommandText = sql;
                command.Parameters.Clear();

                if (Signature == null)
                {
                    SqlParameter signatureParam = new SqlParameter("@signature", SqlDbType.Image);
                    signatureParam.Value = DBNull.Value;
                    command.Parameters.Add(signatureParam);
                }
                else
                {
                    command.Parameters.Add("@signature", SqlDbType.Image);
                    command.Parameters["@signature"].Value = (byte[])imageConvert.ConvertTo(Signature, typeof(byte[]));
                }

                if (Picture == null)
                {
                    SqlParameter pictureParam = new SqlParameter("@picture", SqlDbType.Image);
                    pictureParam.Value = DBNull.Value;
                    command.Parameters.Add(pictureParam);
                }
                else
                {
                    command.Parameters.Add("@picture", SqlDbType.Image);
                    command.Parameters["@picture"].Value = (byte[])imageConvert.ConvertTo(Picture, typeof(byte[]));
                }

                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_CHEIMAGE " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = "";
                Signature = null;
                Picture = null;
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToString(row["pk_recno"]).Trim();


            if (row["signature"] == DBNull.Value) Signature = null;
            else
            {
                byte[] signatureRaw = (byte[])row["signature"];
                Signature = (signatureRaw == null) ? null : new Bitmap(new MemoryStream(signatureRaw));
            }

            if (row["picture"] == DBNull.Value) Picture = null;
            else
            {
                byte[] pictureRaw = (byte[])row["picture"];
                Picture = (pictureRaw == null) ? null : new Bitmap(new MemoryStream(pictureRaw));
            }
        }
    }

    public class ChemicalP2DataSet : UlSqlDataSet
    {
        public Int64 RecNo { get; set; }

        public string MainNo { get; set; }

        public string Name { get; set; }

        public string LoValue { get; set; }

        public string HiValue { get; set; }

        public string ReportValue { get; set; }

        public string FormatValue { get; set; }
        
        public string Sch_Code { get; set; }

        public string SampleDescription { get; set; }

        public string SampleRemarks { get; set; }

        public string Sampleident { get; set; }

        public string Pro_Proj { get; set; }

        public string FINALVALUE { get; set; }

        public string DESCRIPTION_4 { get; set; }

        public int No { get; set; }

        public string Mg { get; set; }

        public string Ai { get; set; }

        public string Sb { get; set; }

        public string As { get; set; }

        public string Ba { get; set; }

        public string B { get; set; }

        public string Cd { get; set; }

        public string Cr { get; set; }

        public string Cr3 { get; set; }

        public string Cr6 { get; set; }

        public string Co { get; set; }

        public string Cu { get; set; }

        public string Pb { get; set; }

        public string Mn { get; set; }

        public string Hg { get; set; }

        public string Ni { get; set; }

        public string Se { get; set; }

        public string Sr { get; set; }

        public string Sn { get; set; }

        public string OrgTin { get; set; }

        public string Zn { get; set; }

        public string Tin { get; set; }

        public string DBT { get; set; }

        public string DMT { get; set; }
        
        public string DOT { get; set; }
        
        public string DPhT { get; set; }
        
        public string DProT { get; set; }
        
        public string MBT { get; set; }
        
        public string MET { get; set; }
        
        public string MOT { get; set; }
        
        public string TBT { get; set; }
        
        public string TeBT { get; set; }
        
        public string TPhT { get; set; }

        public string DBP { get; set; }

        public string BBP { get; set; }

        public string DEHP { get; set; }

        public string DINP { get; set; }

        public string DCHP { get; set; }

        public string DnHP { get; set; }

        public string DIBP { get; set; }

        public string DnPP { get; set; }

        public string DNOP { get; set; }

        public string DIDP { get; set; }

        public string MigrationLimitName { get; set; }

        public string MigrationLimitLoValue { get; set; }

        public string MigrationLimitReportValue { get; set; }

        public string MigrationLimitHiValue { get; set; }

        public ChemicalP2DataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEP2 " +
                $" where fk_chemainno = '{MainNo}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_Limit_ASTM(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEP2_LIMIT_ASTM " +
                $" where fk_chemainno = '{MainNo}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_Limit_Sam_remarks_ASTM(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEP2_LIMIT_ASTM " +
                $" where fk_chemainno = '{MainNo}' and sam_remarks = '{SampleRemarks}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_Limit_FileNo_ASTM(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEP2_LIMIT_ASTM " +
                $" where pro_proj = '{Pro_Proj}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_Che2Sampleident(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEP2 " +
                $" where sampleident = '{Sampleident}' and sch_code = '{Sch_Code}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_Che2Sampleident_HYPEN_EN(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEP2_HYPHEN_EN " +
                $" where fk_chemainno = '{MainNo}'";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_Che2Sampleident_HYPEN_EN_Tin(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEPTIN_EN " +
                $" where fk_chemainno = '{MainNo}'";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_Che2Sampleident_RESULT_ASTM(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEP2_RESULT_ASTM " +
                $" where fk_chemainno = '{MainNo}'";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_Orderby(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEP2 " +
                $" where fk_chemainno like '%{MainNo}%' " +
                $" order by CASE " +
                $" when name like '%mg%' then 1 " +
                $" when name like '%Pb%' then 2 " +
                $" when name like '%Sb%' then 3 " +
                $" when name like '%As%' then 4 " +
                $" when name like '%Ba%' then 5 " +
                $" when name like '%Cd%' then 6 " +
                $" when name like '%Cr%' then 7 " +
                $" when name like '%Hg%' then 8 " +
                $" when name like '%Se%' then 9 end";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_TB_CHEP2_LEAD_LIMIT_ASTM(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEP2_LEAD_LIMIT_ASTM " +
                $" where pro_proj = '{Pro_Proj}'";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_TB_CHEP2_LEAD_RESULT_ASTM(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEP2_LEAD_RESULT_ASTM " +
                $" where pro_proj = '{Pro_Proj}'";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_TB_CHEPPHT_ASTM(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEPPHT_ASTM " +
                $" where pro_proj = '{Pro_Proj}'";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_TB_INTEG_LEAD_LIMIT_ASTM(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEG_LEAD_LIMIT_ASTM " +
                $" where fk_chemainno = '{MainNo}'";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_TB_INTEG_LEAD_RESULT_ASTM(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEG_LEAD_RESULT_ASTM " +
                $" where fk_chemainno = '{MainNo}'";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                /*
                 * 스킴과 sampleident 컬럼이 없을때 Insert query
                $" insert into TB_CHEP2 values " +
                $" ('{MainNo}', '{Name}', '{LoValue}', '{HiValue}', '{ReportValue}', '{FormatValue}'); " +
                $" select cast(scope_identity() as bigint); ";
                */

                $" insert into TB_CHEP2 values " +
                $" ('{MainNo}', '{Name}', '{LoValue}', '{HiValue}', '{ReportValue}', '{FormatValue}', '{Sch_Code}', '{Sampleident}', '{Pro_Proj}'); " +
                $" select cast(scope_identity() as bigint); ";
            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_Result(SqlTransaction trans = null)
        {
            if (string.IsNullOrWhiteSpace(Cr6))
            {
                Cr6 = "N.D.";
            }

            if (string.IsNullOrWhiteSpace(OrgTin))
            {
                OrgTin = "N.D.";
            }

            string sql =
                $" insert into TB_CHEP2_HYPHEN_EN (fk_chemainno, sampleident, sch_code, sam_description, no, mg, ai, sb, \"as\", ba, b, cd, cr3, cr6, co, cu, pb, mn, hg, ni, se, sr ,sn, orgtin, zn)" +
                $" values " +
                $" ('{MainNo}', '{Sampleident}', '{Sch_Code}', '{SampleDescription}', '{No}', '{Mg}', '{Ai}', '{Sb}', '{As}', '{Ba}', '{B}', '{Cd}', '{Cr3}', '{Cr6}', '{Co}', '{Cu}', '{Pb}', '{Mn}', '{Hg}', '{Ni}', '{Se}', '{Sr}', '{Sn}', '{OrgTin}', '{Zn}'); " +
                $" select cast(scope_identity() as bigint); ";
            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_Result_Tin(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_CHEPTIN_EN (fk_chemainno, sampleident, sch_code, sam_description, no, DMT, MeT, DProT, BuT, DBT, TBT, MOT, DOT, TeBT, DPhT, TPhT)" +
                $" values " +
                $" ('{MainNo}', '{Sampleident}', '{Sch_Code}', '{SampleDescription}', '{No}', '{DMT}', '{MET}', '{DProT}', '{MBT}', '{DBT}', '{TBT}', '{MOT}', '{DOT}', '{TeBT}', '{DPhT}', '{TPhT}'); " +
                $" select cast(scope_identity() as bigint); ";
            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_Result_Phthalates(SqlTransaction trans = null)
        {
            string sql =
                $" insert into [ReportIntegration].[dbo].[TB_CHEPPHT_ASTM] (fk_chemainno, pro_proj, sampleident ,sch_code ,sam_description, sam_remarks ,no ,DBP ,BBP ,DEHP ,DINP ,DCHP ,DnHP ,DIBP ,DnPP ,DNOP ,DIDP)" +
                $" values " +
                $" ('{MainNo}', '{Pro_Proj}', '{Sampleident}', '{Sch_Code}', '{SampleDescription}', '{SampleRemarks}', '{No}', '{DBP}', '{BBP}', '{DEHP}', '{DINP}', '{DCHP}', '{DnHP}', '{DIBP}', '{DnPP}', '{DNOP}', '{DIDP}'); " +
                $" select cast(scope_identity() as bigint); ";
            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_CHE_LEADLIMIT_ASTM(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_CHEP2_LEAD_LIMIT_ASTM (fk_chemainno, pro_proj, sampleident, lovalue, hivalue, reportvalue, sch_code, sam_remarks)" +
                $" values " +
                $" ('{MainNo}', '{Pro_Proj}', '{Sampleident}', '{LoValue}', '{HiValue}', '{ReportValue}', '{Sch_Code}', '{SampleRemarks}'); " +
                $" select cast(scope_identity() as bigint); ";
            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_CHE_LEADRESULT_ASTM(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_CHEP2_LEAD_RESULT_ASTM (fk_chemainno, pro_proj, sampleident, no, pb, sch_code, sam_remarks, sam_description)" +
                $" values " +
                $" ('{MainNo}', '{Pro_Proj}', '{Sampleident}', '{No}', '{Pb}', '{Sch_Code}', '{SampleRemarks}', '{SampleDescription}'); " +
                $" select cast(scope_identity() as bigint); ";
            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_TB_CHEP2_LIMIT_ASTM(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_CHEP2_LIMIT_ASTM (fk_chemainno, sampleident, pro_proj, name, lovalue, hivalue, reportvalue, sch_code, sam_remarks)" +
                $" values " +
                $" ('{MainNo}', '{Sampleident}', '{Pro_Proj}', '{MigrationLimitName}', '{MigrationLimitLoValue}', '{MigrationLimitHiValue}', '{MigrationLimitReportValue}', '{Sch_Code}', '{SampleRemarks}'); " +
                $" select cast(scope_identity() as bigint); ";
            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Insert_Result_ASTM(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_CHEP2_RESULT_ASTM (fk_chemainno, pro_proj, sampleident, sch_code, sam_description, sam_remarks, no, mg, pb, sb, \"as\", ba, cd, cr, hg, se)" +
                $" values " +
                $" ('{MainNo}', '{Pro_Proj}', '{Sampleident}', '{Sch_Code}', '{SampleDescription}', '{SampleRemarks}', '{No}', '{Mg}', '{Pb}', '{Sb}', '{As}', '{Ba}', '{Cd}', '{Cr}', '{Hg}', '{Se}'); " +
                $" select cast(scope_identity() as bigint); ";
            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                RecNo = (Int64)command.ExecuteScalar();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_CHEP2 set " +
                $" formatvalue='{FormatValue}' " +
                $" where pk_recno={RecNo} ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update_TB_CHEP2_HYPHEN_EN(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_CHEP2_HYPHEN_EN set " +
                $" orgtin = '{OrgTin}' " +
                $" where sampleident = '{Sampleident}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_CHEP2 " +
                $" where fk_chemainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_TB_CHEP2_HYPHEN_EN(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_CHEP2_HYPHEN_EN " +
                $" where fk_chemainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_TB_CHEPTIN_EN(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_CHEPTIN_EN " +
                $" where fk_chemainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_TB_CHEP2_RESULT_ASTM(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_CHEP2_RESULT_ASTM " +
                $" where fk_chemainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_TB_CHEP2_LEAD_LIMIT_ASTM(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_CHEP2_LEAD_LIMIT_ASTM " +
                $" where fk_chemainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }
        public void Delete_TB_CHEP2_LEAD_RESULT_ASTM(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_CHEP2_LEAD_RESULT_ASTM " +
                $" where fk_chemainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_TB_CHEP2_LIMIT_ASTM(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_CHEP2_LIMIT_ASTM " +
                $" where fk_chemainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete_TB_CHEPPHT_ASTM(SqlTransaction trans = null)
        {
            string sql =
                $" delete from [ReportIntegration].[dbo].[TB_CHEPPHT_ASTM] " +
                $" where fk_chemainno='{MainNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0, string sCase = "")
        {
            if (index < GetRowCount(tableNo) && sCase.Equals(""))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else if (index < GetRowCount(tableNo) && sCase.Equals("Integr_EN"))
            {
                Fetch_Integr_EN(dataSet.Tables[tableNo].Rows[index]);
            }
            else if (index < GetRowCount(tableNo) && sCase.Equals("Integr_EN_Tin"))
            {
                Fetch_Integr_EN_Tin(dataSet.Tables[tableNo].Rows[index]);
            }
            else if (index < GetRowCount(tableNo) && sCase.Equals("Integr_ASTM"))
            {
                Fetch_Integr_ASTM(dataSet.Tables[tableNo].Rows[index]);
            }
            else if (index < GetRowCount(tableNo) && sCase.Equals("Integr_ASTM_Lead_Limit"))
            {
                Fetch_Integr_ASTM_Lead_Limit(dataSet.Tables[tableNo].Rows[index]);
            }
            else if (index < GetRowCount(tableNo) && sCase.Equals("Integr_ASTM_Lead_Result"))
            {
                Fetch_Integr_ASTM_Lead_Result(dataSet.Tables[tableNo].Rows[index]);
            }
            else if (index < GetRowCount(tableNo) && sCase.Equals("Integr_ASTM_Pht"))
            {
                Fetch_Integr_ASTM_Pht(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = 0;
                MainNo = "";
                Name = "";
                LoValue = "";
                HiValue = "";
                ReportValue = "";
                FormatValue = "N.D.";
            }
        }

        public void Fetch(DataRow row)
        {
            try
            {
                RecNo = Convert.ToInt64(row["pk_recno"]);
                MainNo = Convert.ToString(row["fk_chemainno"]);
                Name = Convert.ToString(row["name"]);
                LoValue = Convert.ToString(row["lovalue"]);
                HiValue = Convert.ToString(row["hivalue"]);
                ReportValue = Convert.ToString(row["reportvalue"]);

                if (string.IsNullOrEmpty(FormatValue))
                {
                    FormatValue = "";
                }
                else 
                {
                    FormatValue = Convert.ToString(row["formatvalue"]);
                }
                Sch_Code = Convert.ToString(row["sch_code"]);
                Sampleident = Convert.ToString(row["sampleident"]);
                Pro_Proj = Convert.ToString(row["pro_proj"]);
                SampleRemarks = Convert.ToString(row["sam_remarks"]);
                
            }
            catch (Exception f) 
            {

            }            
        }

        public void Fetch_Integr_EN(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_chemainno"]);
            Sampleident = Convert.ToString(row["sampleident"]);
            Sch_Code = Convert.ToString(row["sch_code"]);
            SampleDescription = Convert.ToString(row["sam_description"]);
            No = Convert.ToInt32(row["no"]);
            Mg = Convert.ToString(row["mg"]);
            Ai = Convert.ToString(row["ai"]);
            Sb = Convert.ToString(row["sb"]);
            As = Convert.ToString(row["as"]);
            Ba = Convert.ToString(row["ba"]);
            B = Convert.ToString(row["b"]);
            Cd = Convert.ToString(row["cd"]);
            Cr3 = Convert.ToString(row["cr3"]);
            Cr6 = Convert.ToString(row["cr6"]);
            Co = Convert.ToString(row["co"]);
            Cu = Convert.ToString(row["cu"]);
            Pb = Convert.ToString(row["pb"]);
            Mn = Convert.ToString(row["mn"]);
            Hg = Convert.ToString(row["hg"]);
            Ni = Convert.ToString(row["ni"]);
            Se = Convert.ToString(row["se"]);
            Sr = Convert.ToString(row["sr"]);
            Sn = Convert.ToString(row["sn"]);
            OrgTin = Convert.ToString(row["orgtin"]);
            Zn = Convert.ToString(row["zn"]);
        }
        public void Fetch_Integr_EN_Tin(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_chemainno"]);
            Sampleident = Convert.ToString(row["sampleident"]);
            Sch_Code = Convert.ToString(row["sch_code"]);
            SampleDescription = Convert.ToString(row["sam_description"]);
            No = Convert.ToInt32(row["no"]);
            DMT = Convert.ToString(row["DMT"]);
            MET = Convert.ToString(row["MeT"]);
            DProT = Convert.ToString(row["DProT"]);
            MBT = Convert.ToString(row["BuT"]);
            DBT = Convert.ToString(row["DBT"]);
            TBT = Convert.ToString(row["TBT"]);
            MOT = Convert.ToString(row["MOT"]);
            DOT = Convert.ToString(row["DOT"]);
            TeBT = Convert.ToString(row["TeBT"]);
            DPhT = Convert.ToString(row["DPhT"]);
            TPhT = Convert.ToString(row["TPhT"]);
        }

        public void Fetch_Integr_ASTM(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_chemainno"]);
            Pro_Proj = Convert.ToString(row["pro_proj"]);
            Sampleident = Convert.ToString(row["sampleident"]);
            Sch_Code = Convert.ToString(row["sch_code"]);
            SampleDescription = Convert.ToString(row["sam_description"]);
            SampleRemarks = Convert.ToString(row["sam_remarks"]);
            No = Convert.ToInt32(row["no"]);
            Mg = Convert.ToString(row["mg"]);
            Pb = Convert.ToString(row["pb"]);
            Sb = Convert.ToString(row["sb"]);
            As = Convert.ToString(row["as"]);
            Ba = Convert.ToString(row["ba"]);
            Cd = Convert.ToString(row["cd"]);
            Cr = Convert.ToString(row["cr"]);
            Hg = Convert.ToString(row["hg"]);
            Se = Convert.ToString(row["se"]);
        }

        public void Fetch_Integr_ASTM_Lead_Limit(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_chemainno"]);            
            Pro_Proj = Convert.ToString(row["pro_proj"]);
            Sampleident = Convert.ToString(row["sampleident"]);
            LoValue = Convert.ToString(row["lovalue"]);
            HiValue = Convert.ToString(row["hivalue"]);
            ReportValue = Convert.ToString(row["reportvalue"]);
            Sch_Code = Convert.ToString(row["sch_code"]);
            SampleRemarks = Convert.ToString(row["sam_remarks"]);
        }       

        public void Fetch_Integr_ASTM_Lead_Result(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_chemainno"]);
            Pro_Proj = Convert.ToString(row["pro_proj"]);
            Sampleident = Convert.ToString(row["sampleident"]);
            No = Convert.ToInt32(row["no"]);
            Pb = Convert.ToString(row["pb"]);
            Sch_Code = Convert.ToString(row["sch_code"]);
            SampleRemarks = Convert.ToString(row["sam_remarks"]);
            SampleDescription = Convert.ToString(row["sam_description"]);
        }

        public void Fetch_Integr_ASTM_Pht(DataRow row)
        {
            RecNo = Convert.ToInt64(row["pk_recno"]);
            MainNo = Convert.ToString(row["fk_chemainno"]);
            Pro_Proj = Convert.ToString(row["pro_proj"]);
            Sampleident = Convert.ToString(row["sampleident"]);            
            Sch_Code = Convert.ToString(row["sch_code"]);
            SampleDescription = Convert.ToString(row["sam_description"]);
            SampleRemarks = Convert.ToString(row["sam_remarks"]);
            No = Convert.ToInt32(row["no"]);
            DBP = Convert.ToString(row["DBP"]);
            BBP = Convert.ToString(row["BBP"]);
            DEHP = Convert.ToString(row["DEHP"]);
            DINP = Convert.ToString(row["DINP"]);
            DCHP = Convert.ToString(row["DCHP"]);
            DnHP = Convert.ToString(row["DnHP"]);
            DIBP = Convert.ToString(row["DIBP"]);
            DnPP = Convert.ToString(row["DnPP"]);
            DNOP = Convert.ToString(row["DNOP"]);
            DIDP = Convert.ToString(row["DIDP"]);
        }
    }

    public class ChemicalP2ExtendDataSet : UlSqlDataSet
    {
        public string RecNo { get; set; }

        public string SubJobRecNo { get; set; }

        public string Name { get; set; }

        public string LoValue { get; set; }

        public string HiValue { get; set; }

        public string ReportValue { get; set; }

        public string FormatValue { get; set; }

        public string Sch_Code { get; set; }

        public string MainNo { get; set; }

        public string SampleRemarks { get; set; }

        public ChemicalP2ExtendDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEP2EXTEND " +
                $" where pk_recno='{RecNo}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_MainNo(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEP2EXTEND " +
                $" where recno_mainjob='{RecNo}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_Limit_Sam_remarks_ASTM(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEG_LIMIT_ASTM " +
                $" where fk_integmainno = '{MainNo}' and sam_remarks = '{SampleRemarks}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_Limit_Sam_remarks_Textile_ASTM(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEG_LIMIT_ASTM " +
                $" where fk_integmainno = '{MainNo}' and sam_remarks = 'textile' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_Limit_Sam_remarks_Plastic_ASTM(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_INTEG_LIMIT_ASTM " +
                $" where fk_integmainno = '{MainNo}' and sam_remarks = 'plastic' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_CHEP2EXTEND values " +
                $" ('{RecNo}', 'None', '{Name}', '{LoValue}', '{HiValue}', '{ReportValue}', '{FormatValue}', '{Sch_Code}') ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_CHEP2EXTEND set " +
                $" formatvalue='{FormatValue}' " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Update_SubJobNo(SqlTransaction trans = null)
        {
            string sql =
                $" update TB_CHEP2EXTEND set " +
                $" recno_mainjob='{RecNo}' " +
                $" where pk_recno='{SubJobRecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_CHEP2EXTEND " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = "";
                Name = "";
                LoValue = "";
                HiValue = "";
                ReportValue = "";
                FormatValue = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToString(row["pk_recno"]);
            Name = Convert.ToString(row["name"]);
            LoValue = Convert.ToString(row["lovalue"]);
            HiValue = Convert.ToString(row["hivalue"]);
            ReportValue = Convert.ToString(row["reportvalue"]);
            FormatValue = Convert.ToString(row["formatvalue"]);
            Sch_Code = Convert.ToString(row["sch_code"]);
        }
    }

    public class ChemicalItemJoinDataSet : UlSqlDataSet
    {
        public string RecNo { get; set; }

        public string PartNo { get; set; }

        public ChemicalItemJoinDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            command.CommandText =
                $" select * from TB_CHEPARTJOIN " +
                $" where partno='{PartNo}' ";
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Insert(SqlTransaction trans = null)
        {
            string sql =
                $" insert into TB_CHEPARTJOIN values('{RecNo}', '{PartNo}') ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            string sql =
                $" delete from TB_CHEPARTJOIN " +
                $" where pk_recno='{RecNo}' ";

            SetTrans(trans);

            try
            {
                BeginTrans(trans);
                command.CommandText = sql;
                command.ExecuteNonQuery();
                CommitTrans(trans);
            }
            catch (Exception e)
            {
                RollbackTrans(trans, e);
            }
        }

        public void Fetch(int index = 0, int tableNo = 0)
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index]);
            }
            else
            {
                RecNo = "";
                PartNo = "";
            }
        }

        public void Fetch(DataRow row)
        {
            RecNo = Convert.ToString(row["pk_recno"]);
            PartNo = Convert.ToString(row["partno"]);
        }
    }

    public class ProfJobImageDataSet : UlSqlDataSet 
    {
        public string JobNo { get; set; }

        public string PRO_JOB { get; set; }

        // PROFJOB.PRO_PROJ
        public string FileNo { get; set; }

        public string PHOTORTFKEY { get; set; }

        public Bitmap Image { get; set; }

        private ImageConverter imageConvert;

        public ProfJobImageDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
            imageConvert = new ImageConverter();
        }

        public void Select(SqlTransaction trans = null)
        {
            string sql =
                $" select * " +
                $" from [KRCTS01].[dbo].[USERPROFJOB_PHOTORTF] " +
                $" where PRO_JOB = '{JobNo}' and IDKEY = '{PHOTORTFKEY}'";

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();

            dataAdapter.Fill(dataSet);
        }

        public void Fetch(int index = 0, int tableNo = 0, string sQueryCase = "")
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index], sQueryCase);
            }
            else
            {   
                JobNo = "";
                FileNo = "";
                Image = null;
            }
        }

        public void Fetch(DataRow row, String sQueryCase = "")
        {
            JobNo = Convert.ToString(row["pro_job"]).Trim();
            //FileNo = Convert.ToString(row["pro_proj"]).Trim();
            PHOTORTFKEY = Convert.ToString(row["IDKEY"]).Trim();

            if (row["photo"] == DBNull.Value)
            {
                Image = null;
            }
            else
            {
                byte[] imageRaw = (byte[])row["photo"];

                if (imageRaw == null)
                    Image = null;
                else
                    Image = new Bitmap(new MemoryStream(imageRaw));
            }            
        }
    }

    public class ProfJobDataSet : UlSqlDataSet
    {
        public EReportType Type { get;set; }

        public EReportArea AreaNo { get; set; }

        public string OrderNo { get; set; }

        // CLIENT.CLI_CODE
        public string ClientNo { get; set; }

        // CLIENT.CLI_NAME
        public string ClientName { get; set; }

        // CLIENT.ADDRESS1 + ADDRESS2 + ADDRESS3 + STATE + COUNTRY
        public string ClientAddress { get; set; }

        // PROFJOB.PRO_JOB
        public string JobNo { get; set; }

        // PROFJOB.PRO_PROJ
        public string OmNo { get; set; }

        public string SAMPLEIDENT { get; set; }

        // PROFJOB.PRO_PROJ
        public string FileNo { get; set; }

        // PROFJOB.REGISTERED
        public DateTime RegTime { get; set; }

        // PROFJOB.RECEIVED
        public DateTime ReceivedTime { get; set; }

        // PROFJOB.REQUIRED
        public DateTime RequiredTime { get; set; }

        // PROFJOB.LASTREPORTED
        public DateTime ReportedTime { get; set; }

        // PROFJOB.VALIDATEBY
        public string StaffNo { get; set; }

        // PROFJOBUSER.JOBCOMMENTS
        public string ItemNo { get; set; }

        // PROFJOBUSER.COMMENTS1
        public string ReportComments { get; set; }

        // PROFJOB_CUIDUSER.SAM_REMARKS
        public string SampleRemark { get; set; }

        // PROFJOB_CUIDUSER.SAM_DESCRIPTION
        public string SampleDescription { get; set; }

        // PROFJOB_CUIDUSER.DESCRIPTION_1
        public string DetailOfSample { get; set; }

        // PROFJOB_CUIDUSER.DESCRIPTION_4
        public string Manufacturer { get; set; }

        // PROFJOB_CUIDUSER.DESCRIPTION_3
        public string CountryOfOrigin { get; set; }

        public string TESTCOMMENTS { get; set; }

        public string PHOTORTFKEY_QR { get; set; }

        public string PHOTORTFKEY_SAMPLE { get; set; }

        // USERPROFJOB_PHOTORTF.PHOTO
        public Bitmap Image { get; set; }

        private ImageConverter imageConvert;

        public string From { get; set; }

        public string To { get; set; }

        public bool ExtendASTM { get; set; }

        public ProfJobDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
            imageConvert = new ImageConverter();
        }

        public void Select(SqlTransaction trans = null)
        {
            //dataSet = new DataSet();
            //dataAdapter.InitializeLifetimeService();
            //dataAdapter.

            string sql =
                " SET ARITHABORT ON " +
                " select t2.cli_code, t2.cli_name, t2.address1, t2.address2,           " +
                "     t2.address3, t2.state, t2.country, t1.orderno, t1.pro_job,       " +
                "     t1.pro_proj, t1.notes1, t1.registered, t1.received, t1.required, " +
                "     t1.lastreported, t1.validatedby, t3.jobcomments, t3.comments1,   " +
                "     t4.sam_remarks, t4.sam_description, t4.description_1,            " +
                "     t4.description_3, t4.description_4, null as 'photo' " +
                //", t5.photo                     " +
                " from Aurora.dbo.PROFJOB t1                                                      " +
                "     join Aurora.dbo.CLIENT t2 on t2.cli_code=t1.cli_code                        " +
                "     join Aurora.dbo.PROFJOBUSER t3 on t3.pro_job=t1.pro_job                     " +
                "     join Aurora.dbo.PROFJOB_CUIDUSER t4 on t4.pro_job=t1.pro_job                " +
                //"     left join Aurora.dbo.USERPROFJOB_PHOTORTF t5 on t5.pro_job = t1.pro_job AND t1.LABCODE = t5.LABCODE AND(t3.PHOTORTFKEY = t5.IDKEY OR t4.PHOTORTFKEY = t5.IDKEY)" +
                " where t1.labcode<>''                                                 ";

            if (string.IsNullOrWhiteSpace(JobNo) == false)
            {
                sql += $" and t1.pro_job='{JobNo}' ";
                sql += $" AND t1.NOTES1 <> '6F'";
            }
            else
            {
                switch (Type)
                {
                    case EReportType.Physical:
                        if (string.IsNullOrWhiteSpace(ItemNo) == false)
                        {
                            sql += $" and t1.orderno like '{ItemNo}%%' and (t1.orderno like '%%-ASTM' or t1.orderno like '%%-EN') ";
                        }
                        if (AreaNo != EReportArea.None)
                        {
                            sql += $" and t1.orderno like '%%{AreaNo.ToDescription()}' ";
                        }
                        //sql += $" and t1.pro_job in (select distinct pro_job from PROFJOB_CUID_SCHEME_ANALYTE where formattedvalue is null and finalvalue is null)";
                        sql += $" AND (t1.FINALISED >= '1900-01-01' Or t1.completed >= '1900-01-01')";
                        break;

                    case EReportType.Chemical:
                        if (AreaNo == EReportArea.US)
                        {
                            if (ExtendASTM == true)
                            {
                                sql += $" and t1.notes1<>'HL_EN' ";
                            }
                            else
                            {
                                sql += $" and t1.notes1='HL_ASTM' ";
                            }
                        }
                        else if (AreaNo == EReportArea.EU)
                        {
                            //sql += $" and t1.notes1='HL_EN' ";
                            sql += $" and (t1.notes1 like '%HL_EN%' or t1.notes1='HL EN') ";
                        }
                        if (string.IsNullOrWhiteSpace(ItemNo) == false)
                        {
                            sql += $" and t3.jobcomments like '%%{ItemNo}%%' ";
                        }
                        //sql += $" and t1.pro_job not in (select distinct pro_job from PROFJOB_CUID_SCHEME_ANALYTE where formattedvalue is null and finalvalue is null)";
                        sql += $" AND (t1.FINALISED >= '1900-01-01' Or t1.completed >= '1900-01-01')";
                        break;

                    case EReportType.Integration:
                        if (string.IsNullOrWhiteSpace(ItemNo) == false)
                        {
                            sql += $" and t1.orderno like '{ItemNo}%%' ";
                        }
                        if (AreaNo != EReportArea.None)
                        {
                            sql += $" and t1.orderno like '%%{AreaNo.ToDescription()}%%' ";
                        }
                        sql += $" and t1.orderno like '%%COMBINE' ";
                        break;
                }

                if (string.IsNullOrWhiteSpace(From) == false)
                {
                    if (From == To)
                    {
                        sql += $" and t1.registered like '{From}%%' ";
                    }
                    else
                    {
                        sql += $" and (t1.registered>='{From} 00:00:00.000' ";
                        sql += $" and t1.registered<='{To} 23:59:59.999') ";
                    }
                }
            }
            sql += $" AND t2.LABCODE ='GLOBAL' AND t1.PRO_JOB LIKE 'AYN%'  ";
            sql += $" order by t1.pro_proj desc ";
            sql += $" SET ARITHABORT OFF ";

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            
            dataAdapter.Fill(dataSet);
        }

        public void Select_KRSCT01(SqlTransaction trans = null)
        {
            //dataSet = new DataSet();
            //dataAdapter.InitializeLifetimeService();
            //dataAdapter.

            string sql =
                //" SET ARITHABORT ON " +
                " select t2.cli_code, t2.cli_name, t2.address1, t2.address2,           " +
                "     t2.address3, t2.state, t2.country, t1.orderno, t1.pro_job,       " +
                "     t1.pro_proj, t1.notes1, t1.registered, t1.received, t1.required, " +
                "     t1.lastreported, t1.validatedby, t3.jobcomments, t3.comments1, t3.PHOTORTFKEY as PHOTORTFKEY_QR, t3.TESTCOMMENTS,  " +
                "     t4.sam_remarks, t4.sam_description, t4.description_1,            " +
                "     t4.description_3, t4.description_4, t4.PHOTORTFKEY as PHOTORTFKEY_SAMPLE, null as 'photo' " +
                //", t5.photo " +
                " from KRCTS01.dbo.PROFJOB t1                                                      " +
                "     join KRCTS01.dbo.CLIENT t2 on t2.cli_code=t1.cli_code                        " +
                "     join KRCTS01.dbo.PROFJOBUSER t3 on t3.pro_job=t1.pro_job                     " +
                "     join KRCTS01.dbo.PROFJOB_CUIDUSER t4 on t4.pro_job=t1.pro_job                " +
                //"     left join KRCTS01.dbo.USERPROFJOB_PHOTORTF t5 on t5.pro_job=t1.pro_job       " +
                //" from Aurora.dbo.PROFJOB t1                                                      " +
                //"     join Aurora.dbo.CLIENT t2 on t2.cli_code=t1.cli_code                        " +
                //"     join Aurora.dbo.PROFJOBUSER t3 on t3.pro_job=t1.pro_job                     " +
                //"     join Aurora.dbo.PROFJOB_CUIDUSER t4 on t4.pro_job=t1.pro_job                " +
                //"     left join Aurora.dbo.USERPROFJOB_PHOTORTF t5 on t5.pro_job=t1.pro_job       " +
                //"     left join Aurora.dbo.USERPROFJOB_PHOTORTF t5 on t5.pro_job = t1.pro_job AND t1.LABCODE = t5.LABCODE AND(t3.PHOTORTFKEY = t5.IDKEY OR t4.PHOTORTFKEY = t5.IDKEY)" +
                " where t1.labcode<>''                                                 ";

            if (string.IsNullOrWhiteSpace(JobNo) == false)
            {
                sql += $" and t1.pro_job='{JobNo}' ";
                sql += $" AND t1.NOTES1 <> '6F'";
            }
            else
            {
                switch (Type)
                {
                    case EReportType.Physical:
                        if (string.IsNullOrWhiteSpace(ItemNo) == false)
                        {
                            sql += $" and t1.orderno like '{ItemNo}%%' and (REPLACE(t1.orderno,' ','') like '%%-ASTM' or REPLACE(t1.orderno,' ','') like '%%-EN' OR REPLACE(t1.orderno,' ','') LIKE '%%-EN/UKCA') ";
                        }
                        if (AreaNo != EReportArea.None)
                        {
                            sql += $" and t1.orderno like '%%{AreaNo.ToDescription()}%%' ";
                        }
                        //sql += $" and t1.pro_job in (select distinct pro_job from PROFJOB_CUID_SCHEME_ANALYTE where formattedvalue is null and finalvalue is null)";
                        sql += $" AND (t1.FINALISED >= '1900-01-01' Or t1.completed >= '1900-01-01' and t1.LASTREPORTED >= '1900-01-01')";
                        break;

                    case EReportType.Chemical:
                        if (AreaNo == EReportArea.US)
                        {
                            if (ExtendASTM == true)
                            {
                                sql += $" and t1.notes1<>'HL_EN' ";
                            }
                            else
                            {
                                sql += $" and t1.notes1 like '%%HL_ASTM%%' ";
                            }
                        }
                        else if (AreaNo == EReportArea.EU)
                        {
                            //sql += $" and t1.notes1='HL_EN' ";
                            sql += $" and (t1.notes1 like '%HL_EN%' or t1.notes1='HL EN') ";
                        }
                        if (string.IsNullOrWhiteSpace(ItemNo) == false)
                        {
                            sql += $" and t3.jobcomments like '%%{ItemNo}%%' ";
                        }
                        //sql += $" and t1.pro_job not in (select distinct pro_job from PROFJOB_CUID_SCHEME_ANALYTE where formattedvalue is null and finalvalue is null)";
                        sql += $" AND (t1.FINALISED >= '1900-01-01' Or t1.completed >= '1900-01-01' and t1.LASTREPORTED >= '1900-01-01')";
                        sql += $" AND NOT (t1.pro_proj LIKE '%CI%')";
                        break;

                    case EReportType.Integration:
                        //if (AreaNo == EReportArea.US)
                        //{
                        //    sql += $" and (t1.notes1 like '%HL_ASTM%' or t1.notes1='HL ASTM') ";
                        //}
                        //else if (AreaNo == EReportArea.EU)
                        //{
                        //    //sql += $" and t1.notes1='HL_EN' ";
                        //    sql += $" and (t1.notes1 like '%HL_EN%' or t1.notes1='HL EN') ";
                        //}
                        if (string.IsNullOrWhiteSpace(ItemNo) == false)
                        {
                            sql += $" and t1.orderno like '{ItemNo}%%' ";
                        }
                        if (AreaNo != EReportArea.None)
                        {
                            sql += $" and t1.orderno like '%%{AreaNo.ToDescription()}%%' ";
                        }
                        sql += $" and t1.orderno like '%%COMBINE%%' ";
                        break;
                }

                if (string.IsNullOrWhiteSpace(From) == false)
                {
                    if (From == To)
                    {
                        sql += $" and t1.registered like '{From}%%' ";
                    }
                    else
                    {
                        sql += $" and (t1.registered>='{From} 00:00:00.000' ";
                        sql += $" and t1.registered<='{To} 23:59:59.999') ";
                    }
                }
            }
            sql += $" AND t2.LABCODE ='GLOBAL' AND t1.PRO_JOB LIKE 'AYN%'  ";
            sql += $" order by t1.pro_proj desc ";
            //sql += $" SET ARITHABORT OFF ";

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            //dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            dataAdapter.Fill(dataSet);
        }

        public void Select_Aurora(SqlTransaction trans = null)
        {
            string sql =
                " select t2.cli_code, t2.cli_name, t2.address1, t2.address2,           " +
                "     t2.address3, t2.state, t2.country, t1.orderno, t1.pro_job,       " +
                "     t1.pro_proj, t1.notes1, t1.registered, t1.received, t1.required, " +
                "     t1.lastreported, t1.validatedby, t3.jobcomments, t3.comments1,   " +
                "     t4.sam_remarks, t4.sam_description, t4.description_1,            " +
                "     t4.description_3, t4.description_4, t5.photo                     " +
                " from Aurora.dbo.PROFJOB t1                                                      " +
                "     join Aurora.dbo.CLIENT t2 on t2.cli_code=t1.cli_code                        " +
                "     join Aurora.dbo.PROFJOBUSER t3 on t3.pro_job=t1.pro_job                     " +
                "     join Aurora.dbo.PROFJOB_CUIDUSER t4 on t4.pro_job=t1.pro_job                " +
                "     left join Aurora.dbo.USERPROFJOB_PHOTORTF t5 on t5.pro_job=t1.pro_job       " +
                " where t1.labcode<>''                                                 ";

            if (string.IsNullOrWhiteSpace(JobNo) == false)
            {
                sql += $" and t1.pro_job='{JobNo}' ";
                //sql += $" AND t1.NOTES1 <> '6F'";
            }
            else
            {
                switch (Type)
                {
                    case EReportType.Physical:
                        if (string.IsNullOrWhiteSpace(ItemNo) == false)
                        {
                            sql += $" and t1.orderno like '{ItemNo}%%' and (t1.orderno like '%%-ASTM%' or t1.orderno like '%%-EN%') ";
                        }
                        if (AreaNo != EReportArea.None)
                        {
                            sql += $" and (t1.orderno like '%%{AreaNo.ToDescription()}' or t1.orderno like '%%{AreaNo.ToDescription()} ') ";
                        }
                        //sql += $" and t1.pro_job in (select distinct pro_job from PROFJOB_CUID_SCHEME_ANALYTE where formattedvalue is null and finalvalue is null)";
                        sql += $" AND (t1.FINALISED >= '1900-01-01' Or t1.completed >= '1900-01-01')";
                        break;

                    case EReportType.Chemical:
                        if (AreaNo == EReportArea.US)
                        {
                            if (ExtendASTM == true)
                            {
                                sql += $" and t1.notes1 NOT LIKE '%HL_EN%' ";
                            }
                            else
                            {
                                sql += $" and t1.notes1='HL_ASTM' ";
                            }
                        }
                        else if (AreaNo == EReportArea.EU)
                        {
                            sql += $" and (t1.notes1 like '%HL_EN%' or t1.notes1='HL EN')";
                        }
                        if (string.IsNullOrWhiteSpace(ItemNo) == false)
                        {
                            sql += $" and t3.jobcomments like '%%{ItemNo}%%' ";
                        }
                        //sql += $" and t1.pro_job not in (select distinct pro_job from PROFJOB_CUID_SCHEME_ANALYTE where formattedvalue is null and finalvalue is null)";
                        sql += $" AND (t1.FINALISED >= '1900-01-01' Or t1.completed >= '1900-01-01')";                        
                        break;

                    case EReportType.Integration:
                        if (string.IsNullOrWhiteSpace(ItemNo) == false)
                        {
                            sql += $" and t1.orderno like '{ItemNo}%%' ";
                        }
                        if (AreaNo != EReportArea.None)
                        {
                            sql += $" and t1.orderno like '%%{AreaNo.ToDescription()}%%' ";
                        }
                        sql += $" and t1.orderno like '%%COMBINE%%' ";
                        break;
                }

                if (string.IsNullOrWhiteSpace(From) == false)
                {
                    if (From == To)
                    {
                        sql += $" and t1.registered like '{From}%%' ";
                    }
                    else
                    {
                        sql += $" and (t1.registered>='{From} 00:00:00.000' ";
                        sql += $" and t1.registered<='{To} 23:59:59.999') ";
                    }
                }
            }
            sql += $" AND t1.NOTES1 <> '6F'";
            sql += $" order by t1.pro_proj desc ";

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_AuroraTopOne(SqlTransaction trans = null)
        {
            string sql =
                " select top 1 t2.cli_code, t2.cli_name, t2.address1, t2.address2,           " +
                "     t2.address3, t2.state, t2.country, t1.orderno, t1.pro_job,       " +
                "     t1.pro_proj, t1.notes1, t1.registered, t1.received, t1.required, " +
                "     t1.lastreported, t1.validatedby, t3.jobcomments, t3.comments1,   " +
                "     t4.sam_remarks, t4.sam_description, t4.description_1,            " +
                "     t4.description_3, t4.description_4, t5.photo                     " +
                " from Aurora.dbo.PROFJOB t1                                                      " +
                "     join Aurora.dbo.CLIENT t2 on t2.cli_code=t1.cli_code                        " +
                "     join Aurora.dbo.PROFJOBUSER t3 on t3.pro_job=t1.pro_job                     " +
                "     join Aurora.dbo.PROFJOB_CUIDUSER t4 on t4.pro_job=t1.pro_job                " +
                "     left join Aurora.dbo.USERPROFJOB_PHOTORTF t5 on t5.pro_job=t1.pro_job       " +
                " where t1.labcode<>''                                                 ";

            if (string.IsNullOrWhiteSpace(JobNo) == false && string.IsNullOrWhiteSpace(ItemNo) == false)
            {
                //sql += $" and t1.pro_job='{JobNo}' ";
                sql += $" AND t3.jobcomments like '%%{ItemNo}%%'";
                sql += $" AND t1.NOTES1 <> '6F'";
            }
            sql += $" order by t1.pro_proj desc ";

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_Distinct_Sampleident_Profjob_Aurora(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $" select distinct t6.SAMPLEIDENT, t2.cli_code, t2.cli_name, t2.address1, t2.address2,  " +
            $"    t2.address3, t2.state, t2.country, t1.orderno, t1.pro_job,                        " +
            $"     t1.pro_proj, t1.notes1, t1.registered, t1.received, t1.required,                 " +
            $"     t1.lastreported, t1.validatedby, t3.jobcomments, t3.comments1,                   " +
            $"     t4.sam_remarks, t4.sam_description, t4.description_1,                            " +
            $"     t4.description_3, t4.description_4, null as 'photo' " +
            //$", t5.photo                                     " +
            $"		FROM   aurora.dbo.profjob t1                                                    " +
            $"       JOIN aurora.dbo.client t2                                                      " +
            $"         ON t2.cli_code = t1.cli_code						" +
            $"       JOIN aurora.dbo.profjobuser t3						" +
            $"         ON t3.pro_job = t1.pro_job						" +
            $"       JOIN aurora.dbo.profjob_cuiduser t4				" +
            $"         ON t4.pro_job = t1.pro_job						" +
            //$"        JOIN Aurora.dbo.USERPROFJOB_PHOTORTF t5 " +
            //$"          ON t5.pro_job = t1.pro_job AND t1.LABCODE = t5.LABCODE AND(t3.PHOTORTFKEY = t5.IDKEY OR t4.PHOTORTFKEY = t5.IDKEY)" +
            $"	   JOIN aurora.dbo.profjob_cuid t6						" +
            $"         ON ( t1.labcode = t6.labcode						" +
            $"              AND t1.pro_job = t6.pro_job )				" +
            $"	   JOIN aurora.dbo.profjob_cuid_scheme t8				" +
            $"         ON ( t6.labcode = t8.labcode						" +
            $"              AND t6.pro_job = t8.pro_job					" +
            $"              AND t6.cuid = t8.cuid )						" +
            $"	  JOIN aurora.dbo.profjob_cuid_scheme_analyte t7		" +
            $"         ON ( t8.labcode = t7.labcode						" +
            $"              AND t8.pro_job = t7.pro_job					" +
            $"              AND t8.cuid = t7.cuid						" +
            $"              AND t8.sch_code = t7.sch_code				" +
            $"              AND t8.schversion = t7.schversion )			" +
            $" WHERE  t1.labcode <> ''									" +
            $"	   AND t6.SAMPLEIDENT = '{JobNo}'       	            " +
            $"       AND t1.notes1 <> 'HL_EN'							" +
            $"       AND ( t1.finalised >= '1900-01-01'					" +
            $"              OR t1.completed >= '1900-01-01' )			" +
            $"       AND t1.notes1 <> '6F'								" +
            $"	   AND t7.formattedvalue <> 'N.A.'						" +
            $"  ORDER  BY t1.pro_proj DESC 								";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_TopOne_Sampleident_Aurora(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"   SELECT top 1 t5.sampleident,								" +
            $"                t7.SAM_DESCRIPTION,                           " +
            $"                t1.PRO_JOB,                                   " +
            $"                t1.ORDERNO                                    " +
            $"	    FROM   Aurora.dbo.profjob t1                            " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID t5                      " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID_SCHEME t6               " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUIDUSER t7                  " +
            $"			 ON ( t7.labcode = t5.labcode                       " +
            $"				  AND t7.PRO_JOB = t5.PRO_JOB                   " +
            $"				  AND t7.CUID = t5.CUID )                       " +
            $"		   JOIN Aurora.dbo.profjob_cuid_scheme_analyte t2       " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"	WHERE  t5.PRO_JOB = '{JobNo}'                           " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"		   AND t2.FORMATTEDVALUE <> 'N.A.'                      " +
            $"		   AND t1.notes1 <> '6F'                                " ;

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Select_TopOne_Sampleident_KRCTS01(SqlTransaction trans = null)
        {
            string sql =
            //$"   SELECT TOP 1 t5.sampleident,								" +
            //$"                t7.SAM_DESCRIPTION,                           " +
            //$"                t1.PRO_JOB,                                   " +
            //$"                t1.ORDERNO                                    " +
            $"   SELECT TOP 1 * 								            " +
            $"	    FROM   KRCTS01.dbo.profjob t1                           " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID t5                     " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID_SCHEME t6              " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUIDUSER t7                 " +
            $"			 ON ( t7.labcode = t5.labcode                       " +
            $"				  AND t7.PRO_JOB = t5.PRO_JOB                   " +
            $"				  AND t7.CUID = t5.CUID )                       " +
            $"		   JOIN KRCTS01.dbo.profjob_cuid_scheme_analyte t2      " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"	WHERE  t5.PRO_JOB = '{JobNo}'                               " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"		   AND t2.FORMATTEDVALUE <> 'N.A.'                      " +
            $"		   AND t1.notes1 <> '6F'                                ";

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            //dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            dataAdapter.Fill(dataSet);
        }

        public void Select_Physical_Import(SqlTransaction trans = null)
        {
            string sql =
                " SET ARITHABORT ON " +
                " select t2.cli_code, t2.cli_name, t2.address1, t2.address2,           " +
                "     t2.address3, t2.state, t2.country, t1.orderno, t1.pro_job,       " +
                "     t1.pro_proj, t1.notes1, t1.registered, t1.received, t1.required, " +
                "     t1.lastreported, t1.validatedby, t3.jobcomments, t3.comments1, t3.TESTCOMMENTS,   " +
                "     t4.sam_remarks, t4.sam_description, t4.description_1,            " +
                //"     t4.description_3, t4.description_4, null as 'photo' " +
                "     t4.description_3, t4.description_4, t5.PHOTO " +
                //", t5.photo                     " +
                //" from Aurora.dbo.PROFJOB t1                                                      " +
                //"     join Aurora.dbo.CLIENT t2 on t2.cli_code=t1.cli_code                        " +
                //"     join Aurora.dbo.PROFJOBUSER t3 on t3.pro_job=t1.pro_job                     " +
                //"     join Aurora.dbo.PROFJOB_CUIDUSER t4 on t4.pro_job=t1.pro_job                " +
                //"     left join Aurora.dbo.USERPROFJOB_PHOTORTF t5 on t5.pro_job = t1.pro_job AND t1.LABCODE = t5.LABCODE AND(t3.PHOTORTFKEY = t5.IDKEY OR t4.PHOTORTFKEY = t5.IDKEY)" +
                " from KRCTS01.dbo.PROFJOB t1                                                      " +
                "     join KRCTS01.dbo.CLIENT t2 on t2.cli_code=t1.cli_code                        " +
                "     join KRCTS01.dbo.PROFJOBUSER t3 on t3.pro_job=t1.pro_job                     " +
                "     join KRCTS01.dbo.PROFJOB_CUIDUSER t4 on t4.pro_job=t1.pro_job                " +
                "     left join KRCTS01.dbo.USERPROFJOB_PHOTORTF t5 on t5.pro_job = t1.pro_job AND t1.LABCODE = t5.LABCODE AND(t3.PHOTORTFKEY = t5.IDKEY OR t4.PHOTORTFKEY = t5.IDKEY)       " +
                " where t1.labcode<>''                                                 ";

            if (string.IsNullOrWhiteSpace(JobNo) == false)
            {
                sql += $" and t1.pro_job like '%%{JobNo}' ";
                sql += $" AND t1.NOTES1 <> '6F'";
            }
            else
            {
                switch (Type)
                {
                    case EReportType.Physical:
                        if (string.IsNullOrWhiteSpace(ItemNo) == false)
                        {
                            sql += $" and t1.orderno like '%%{ItemNo}%%' and (t1.orderno like '%%-ASTM' or t1.orderno like '%%-EN') ";
                        }
                        if (string.IsNullOrWhiteSpace(OmNo) == false)
                        {
                            sql += $" and t1.PRO_PROJ like '%%{OmNo}' ";
                        }
                        if (AreaNo != EReportArea.None)
                        {
                            sql += $" and t1.orderno like '%%{AreaNo.ToDescription()}%%' ";
                        }
                        //sql += $" and t1.pro_job in (select distinct pro_job from PROFJOB_CUID_SCHEME_ANALYTE where formattedvalue is null and finalvalue is null)";
                        //sql += $" AND (t1.FINALISED >= '1900-01-01' Or t1.completed >= '1900-01-01')";
                        break;

                    case EReportType.Chemical:
                        if (AreaNo == EReportArea.US)
                        {
                            if (ExtendASTM == true)
                            {
                                sql += $" and t1.notes1<>'HL_EN' ";
                            }
                            else
                            {
                                sql += $" and t1.notes1='HL_ASTM' ";
                            }
                        }
                        else if (AreaNo == EReportArea.EU)
                        {
                            sql += $" and t1.notes1='HL_EN' ";
                        }
                        if (string.IsNullOrWhiteSpace(ItemNo) == false)
                        {
                            sql += $" and t3.jobcomments like '%%{ItemNo}%%' ";
                        }
                        //sql += $" and t1.pro_job not in (select distinct pro_job from PROFJOB_CUID_SCHEME_ANALYTE where formattedvalue is null and finalvalue is null)";
                        sql += $" AND (t1.FINALISED >= '1900-01-01' Or t1.completed >= '1900-01-01')";
                        break;

                    case EReportType.Integration:
                        if (string.IsNullOrWhiteSpace(ItemNo) == false)
                        {
                            sql += $" and t1.orderno like '{ItemNo}%%' ";
                        }
                        if (AreaNo != EReportArea.None)
                        {
                            sql += $" and t1.orderno like '%%{AreaNo.ToDescription()}%%' ";
                        }
                        sql += $" and t1.orderno like '%%COMBINE' ";
                        break;
                }

                if (string.IsNullOrWhiteSpace(From) == false)
                {
                    if (From == To)
                    {
                        sql += $" and t1.registered like '{From}%%' ";
                    }
                    else
                    {
                        sql += $" and (t1.registered>='{From} 00:00:00.000' ";
                        sql += $" and t1.registered<='{To} 23:59:59.999') ";
                    }
                }
            }
            sql += $" AND t2.LABCODE ='GLOBAL' AND t1.PRO_JOB LIKE 'AYN%'  ";
            sql += $" order by t1.pro_proj desc ";
            sql += $" SET ARITHABORT OFF ";

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            dataAdapter.Fill(dataSet);
        }

        public void Select_Chemical_Import(SqlTransaction trans = null)
        {
            string sql =
                //" SET ARITHABORT ON " +
                " select top 1 t2.cli_code, t2.cli_name, t2.address1, t2.address2,           " +
                "     t2.address3, t2.state, t2.country, t1.orderno, t1.pro_job,       " +
                "     t1.pro_proj, t1.notes1, t1.registered, t1.received, t1.required, " +
                "     t1.lastreported, t1.validatedby, t3.jobcomments, t3.comments1, t3.TESTCOMMENTS,   " +
                "     t4.sam_remarks, t4.sam_description, t4.description_1,            " +
                "     t4.description_3, t4.description_4, null as 'photo' " +
                //", t5.photo                     " +
                //" from Aurora.dbo.PROFJOB t1                                                      " +
                //"     join Aurora.dbo.CLIENT t2 on t2.cli_code=t1.cli_code                        " +
                //"     join Aurora.dbo.PROFJOBUSER t3 on t3.pro_job=t1.pro_job                     " +
                //"     join Aurora.dbo.PROFJOB_CUIDUSER t4 on t4.pro_job=t1.pro_job                " +
                //"     left join Aurora.dbo.USERPROFJOB_PHOTORTF t5 on t5.pro_job = t1.pro_job AND t1.LABCODE = t5.LABCODE AND(t3.PHOTORTFKEY = t5.IDKEY OR t4.PHOTORTFKEY = t5.IDKEY)" +
                " from KRCTS01.dbo.PROFJOB t1                                                      " +
                "     join KRCTS01.dbo.CLIENT t2 on t2.cli_code=t1.cli_code                        " +
                "     join KRCTS01.dbo.PROFJOBUSER t3 on t3.pro_job=t1.pro_job                     " +
                "     join KRCTS01.dbo.PROFJOB_CUIDUSER t4 on t4.pro_job=t1.pro_job                " +
                //"     left join KRCTS01.dbo.USERPROFJOB_PHOTORTF t5 on t5.pro_job=t1.pro_job       " +
                " where t1.labcode<>''                                                 ";

            if (string.IsNullOrWhiteSpace(JobNo) == false)
            {
                sql += $" and t1.pro_job like '%%{JobNo}' ";
                sql += $" AND t1.NOTES1 <> '6F'";
            }
            if (string.IsNullOrWhiteSpace(OmNo) == false)
            {
                sql += $" and t1.PRO_PROJ like '%%{OmNo}' ";
            }
            else
            {
                switch (Type)
                {
                    case EReportType.Physical:
                        if (string.IsNullOrWhiteSpace(ItemNo) == false)
                        {
                            sql += $" and t1.orderno like '%%{ItemNo}%%' and (t1.orderno like '%%-ASTM' or t1.orderno like '%%-EN') ";
                        }                        
                        if (AreaNo != EReportArea.None)
                        {
                            sql += $" and t1.orderno like '%%{AreaNo.ToDescription()}%%' ";
                        }
                        //sql += $" and t1.pro_job in (select distinct pro_job from PROFJOB_CUID_SCHEME_ANALYTE where formattedvalue is null and finalvalue is null)";
                        //sql += $" AND (t1.FINALISED >= '1900-01-01' Or t1.completed >= '1900-01-01')";
                        break;

                    case EReportType.Chemical:
                        if (AreaNo == EReportArea.US)
                        {
                            if (ExtendASTM == true)
                            {
                                sql += $" and t1.notes1<>'HL_EN' ";
                            }
                            else
                            {
                                sql += $" and t1.notes1 like '%%HL_ASTM' ";
                            }                            
                        }
                        else if (AreaNo == EReportArea.EU)
                        {
                            sql += $" and t1.notes1='HL_EN' ";
                        }
                        if (string.IsNullOrWhiteSpace(ItemNo) == false)
                        {
                            sql += $" and t3.jobcomments like '%%{ItemNo}%%' ";
                        }
                        //sql += $" and t1.pro_job not in (select distinct pro_job from PROFJOB_CUID_SCHEME_ANALYTE where formattedvalue is null and finalvalue is null)";
                        sql += $" AND (t1.FINALISED >= '1900-01-01' Or t1.completed >= '1900-01-01')";
                        break;

                    case EReportType.Integration:
                        if (string.IsNullOrWhiteSpace(ItemNo) == false)
                        {
                            sql += $" and t1.orderno like '{ItemNo}%%' ";
                        }
                        if (AreaNo != EReportArea.None)
                        {
                            sql += $" and t1.orderno like '%%{AreaNo.ToDescription()}%%' ";
                        }
                        sql += $" and t1.orderno like '%%COMBINE' ";
                        break;
                }

                if (string.IsNullOrWhiteSpace(From) == false)
                {
                    if (From == To)
                    {
                        sql += $" and t1.registered like '{From}%%' ";
                    }
                    else
                    {
                        sql += $" and (t1.registered>='{From} 00:00:00.000' ";
                        sql += $" and t1.registered<='{To} 23:59:59.999') ";
                    }
                }
            }
            sql += $" AND t2.LABCODE ='GLOBAL' AND t1.PRO_JOB LIKE 'AYN%'  ";
            sql += $" order by t1.pro_proj desc ";
            //sql += $" SET ARITHABORT OFF ";

            SetTrans(trans);
            command.CommandText = sql;
            dataSet.Clear();
            dataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            dataAdapter.Fill(dataSet);
        }

        public void Fetch(int index = 0, int tableNo = 0, string sQueryCase = "")
        {
            if (index < GetRowCount(tableNo))
            {
                Fetch(dataSet.Tables[tableNo].Rows[index], sQueryCase);
            }
            else
            {
                Type = EReportType.None;
                AreaNo = EReportArea.None;
                OrderNo = "";
                ClientNo = "";
                ClientName = "";
                ClientAddress = "";
                JobNo = "";
                FileNo = "";
                RegTime = DateTime.Now;
                ReceivedTime = DateTime.Now;
                RequiredTime = DateTime.Now;
                ReportedTime = DateTime.Now;
                StaffNo = "";
                ItemNo = "";
                ReportComments = "";
                SampleRemark = "";
                SampleDescription = "";
                DetailOfSample = "";
                Manufacturer = "";
                CountryOfOrigin = "";
                PHOTORTFKEY_QR = "";
                PHOTORTFKEY_SAMPLE = "";
                Image = null;
            }
        }

        public void Fetch(DataRow row, String sQueryCase = "")
        {
            if (sQueryCase.Equals("Select_TopOne"))
            {
                JobNo = Convert.ToString(row["pro_job"]).Trim();
                OrderNo = Convert.ToString(row["orderno"]).Trim();
                SAMPLEIDENT = Convert.ToString(row["SAMPLEIDENT"]);
                SampleDescription = Convert.ToString(row["sam_description"]);
            }
            else
            {
                AreaNo = EReportArea.None;
                OrderNo = Convert.ToString(row["orderno"]).Trim();
                ClientNo = Convert.ToString(row["cli_code"]).Trim();
                ClientName = Convert.ToString(row["cli_name"]).Trim();
                //ClientAddress = Convert.ToString(row["address1"]) + ", " +
                ClientAddress = Convert.ToString(row["address1"]) + "\r\n" +
                    Convert.ToString(row["address2"]) + "\r\n" +
                    Convert.ToString(row["address3"]);
                    //Convert.ToString(row["state"]) + "\r\n" +
                    //Convert.ToString(row["country"]);
                JobNo = Convert.ToString(row["pro_job"]).Trim();
                FileNo = Convert.ToString(row["pro_proj"]).Trim();
                OmNo = Convert.ToString(row["pro_proj"]).Trim();
                RegTime = Convert.ToDateTime(row["registered"]);
                ReceivedTime = Convert.ToDateTime(row["received"]);
                RequiredTime = Convert.ToDateTime(row["required"]);
                ReportedTime = Convert.ToDateTime(row["lastreported"]);
                StaffNo = Convert.ToString(row["validatedby"]).Trim();
                ReportComments = Convert.ToString(row["comments1"]);
                
                SampleDescription = Convert.ToString(row["sam_description"]);
                DetailOfSample = Convert.ToString(row["description_1"]);
                Manufacturer = Convert.ToString(row["description_4"]);
                CountryOfOrigin = Convert.ToString(row["description_3"]);
                try
                {
                    SampleRemark = Convert.ToString(row["sam_remarks"]);
                    TESTCOMMENTS = Convert.ToString(row["TESTCOMMENTS"]);
                    PHOTORTFKEY_QR = Convert.ToString(row["PHOTORTFKEY_QR"]);
                    PHOTORTFKEY_SAMPLE = Convert.ToString(row["PHOTORTFKEY_SAMPLE"]);
                }
                catch (Exception f) 
                {

                }                

                switch (Type)
                {
                    case EReportType.Physical:
                        ItemNo = Convert.ToString(row["orderno"]).Trim();
                        string[] strs1 = ItemNo.Split('-');

                        if (strs1.Length > 1)
                        {
                            //switch (strs1[1].ToUpper().Trim())
                            //{
                            //    case "ASTM":
                            //        AreaNo = EReportArea.US;
                            //        break;

                            //    case "EN":
                            //        AreaNo = EReportArea.EU;
                            //        break;

                            //    default:
                            //        AreaNo = EReportArea.None;
                            //        break;
                            //}

                            if (strs1[1].ToUpper().Contains("ASTM"))
                            {
                                AreaNo = EReportArea.US;
                            }
                            else if (strs1[1].ToUpper().Contains("EN") || strs1[2].ToUpper().Contains("EN"))
                            {
                                AreaNo = EReportArea.EU;
                            }
                            else
                            {
                                AreaNo = EReportArea.None;
                            }
                            ItemNo = strs1[0].Trim();
                        }                        
                        else
                        {
                            ItemNo = "";
                            AreaNo = EReportArea.None;
                        }
                        break;

                    case EReportType.Chemical:
                        //ItemNo = Convert.ToString(row["orderno"]).Trim();
                        ItemNo = Convert.ToString(row["jobcomments"]).Trim();

                        if (Convert.ToString(row["notes1"]).Trim().ToUpper().Contains("ASTM"))
                        {
                            AreaNo = EReportArea.US;
                        }
                        else if (Convert.ToString(row["notes1"]).Trim().ToUpper().Contains("EN"))
                        {
                            AreaNo = EReportArea.EU;
                        }
                        else
                        {
                            AreaNo = EReportArea.None;
                        }

                        //switch (Convert.ToString(row["notes1"]).Trim())
                        //{
                        //    case "HL_ASTM":
                        //        AreaNo = EReportArea.US;
                        //        break;

                        //    case "HL_EN":
                        //        AreaNo = EReportArea.EU;
                        //        break;

                        //    default:
                        //        AreaNo = EReportArea.None;
                        //        break;
                        //}
                        break;

                    case EReportType.Integration:
                        ItemNo = Convert.ToString(row["orderno"]).Trim();

                        //if (ItemNo.ToUpper().Contains("ASTM"))
                        //{
                        //    AreaNo = EReportArea.US;
                        //}
                        //else if (ItemNo.ToUpper().Contains("EN"))
                        //{
                        //    AreaNo = EReportArea.EU;
                        //}
                        //else 
                        //{
                        //    AreaNo = EReportArea.None;
                        //}

                        string[] strs2 = ItemNo.Split('-');

                        if (strs2.Length > 1)
                        {
                            string[] strs3 = strs2[1].Split(' ');

                            if (strs3.Length >= 2)
                            {
                                switch (strs3[0].ToUpper().Trim())
                                {
                                    case "ASTM":
                                        AreaNo = EReportArea.US;
                                        break;

                                    case "EN":
                                        AreaNo = EReportArea.EU;
                                        break;

                                    default:
                                        AreaNo = EReportArea.None;
                                        break;
                                }
                            }
                            else
                            {
                                AreaNo = EReportArea.None;
                            }

                            ItemNo = strs2[0].Trim();
                        }
                        else
                        {
                            ItemNo = "";
                            AreaNo = EReportArea.None;
                        }
                        break;
                }
                
                if (row["photo"] == DBNull.Value)
                {
                    Image = null;
                }
                else
                {
                    byte[] imageRaw = (byte[])row["photo"];

                    if (imageRaw == null)
                        Image = null;
                    else
                        Image = new Bitmap(new MemoryStream(imageRaw));
                }                
            }
        }
    }

    public class ProfJobSchemeDataSet : UlSqlDataSet
    {
        public DateTime RegTime { get; set; }

        public EReportArea Area { get; set; }

        public ELeadType Lead  { get; set; }

        public string JobNo { get; set; }

        public string ProjJobNo { get; set; }

        public string fileNo { get; set; }

        public string SAMPLEIDENT { get; set; }

        public string Name { get; set; }

        public string LoValue { get; set; }

        public string HiValue { get; set; }

        public string ReportValue { get; set; }

        public string FormatValue { get; set; }

        public string SampleDescription { get; set; }

        public string DESCRIPTION_4 { get; set; }

        public string Sch_Code { get; set; }

        public string SampleRemarks { get; set; }

        public string FINALVALUE { get; set; }

        public string WEIGHT { get; set; }

        public ProfJobSchemeDataSet(SqlConnection connect, SqlCommand command, SqlDataAdapter adapter)
            : base(connect, command, adapter)
        {
        }

        public void Select(SqlTransaction trans = null)
        {
            SetTrans(trans);
            /*
            command.CommandText =
                $" select t1.pro_job, t1.registered, t3.sch_code, " +
                $"     t4.description, t3.lvl1lowerlimit, t3.lvl1upperlimit, " +
                $"     t3.repdetlimit, t2.formattedvalue " +
                $" from PROFJOB t1 " +
                $"     Join PROFJOB_CUID_SCHEME_ANALYTE t2 on (t2.pro_job=t1.pro_job) " +
                $"     join PROFJOB_SCHEME_ANALYTE t3 on " +
                $"         (t3.labcode=t2.labcode and t3.pro_job=t2.pro_job and " +
                $"         t3.sch_code=t2.sch_code and t3.analytecode=t2.analytecode) " +
                $"     join SCHEME_ANALYTE t4 on " +
                $"         (t4.labcode=t2.labcode and t4.sch_code=t2.sch_code and " +
                $"         t4.schversion=t2.schversion and t4.analytecode=t2.analytecode) " +
                $" where t1.pro_job='{JobNo}' and t1.completed>'2000-01-01' " +
                $" order by t3.repsequence asc ";
            */

            command.CommandText =
        $"SELECT t1.pro_job,															" +
        $"   t5.CUID,                                                                         " +
        $"   t5.SAMPLEIDENT,                                                          " +
        $"   t1.registered,                                                                  " +
        $"   t3.sch_code,                                                                  " +
        $"   t4.description,                                                               " +
        $"   t3.lvl1lowerlimit,                                                             " +
        $"   t3.lvl1upperlimit,                                                            " +
        $"   t3.repdetlimit,                                                                " +
        $"   t2.formattedvalue                                                         " +
        $"	FROM   profjob t1                                                            " +
        $"		   JOIN PROFJOB_CUID t5                                           " +
        $"			 ON ( t1.LABCODE = t5.LABCODE                           " +
        $"				  AND t1.PRO_JOB = t5.PRO_JOB )                      " +
        $"		   JOIN PROFJOB_CUID_SCHEME t6                            " +
        $"			 ON ( t5.LABCODE = t6.LABCODE                          " +
        $"				  AND t5.PRO_JOB = t6.PRO_JOB                       " +
        $"				  AND t5.CUID = t6.CUID )                                  " +
        $"		   JOIN profjob_cuid_scheme_analyte t2                      " +
        $"			 ON ( t6.LABCODE = t2.LABCODE                          " +
        $"				  AND t6.pro_job = t2.pro_job                             " +
        $"				  AND t6.CUID = t2.CUID                                    " +
        $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
        $"				  AND t6.SCHVERSION = t2.SCHVERSION)         " +
        $"		   JOIN profjob_scheme_analyte t3                              " +
        $"			 ON ( t3.labcode = t2.labcode                                 " +
        $"				  AND t3.pro_job = t2.pro_job                             " +
        $"				  AND t3.sch_code = t2.sch_code                        " +
        $"				  AND t3.analytecode = t2.analytecode )            " +
        $"		   JOIN scheme_analyte t4                                           " +
        $"			 ON ( t4.labcode = t2.labcode                                 " +
        $"				  AND t4.sch_code = t2.sch_code                        " +
        $"				  AND t4.schversion = t2.schversion                    " +
        $"				  AND t4.analytecode = t2.analytecode )            " +
        $"	WHERE  t1.pro_job = '{JobNo}'                             " +
        $"		   AND t1.completed > '2000-01-01'                            " +
        $"	ORDER  BY t3.repsequence ASC                                     ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectDistinct(SqlTransaction trans = null)
        {
            SetTrans(trans);            

            command.CommandText =
            $"  SELECT distinct t1.pro_job,									" +
            $"                  t5.SAMPLEIDENT                              " +
            $"	FROM   profjob t1                                           " +
            $"		   JOIN PROFJOB_CUID t5                                 " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN PROFJOB_CUID_SCHEME t6                          " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN profjob_cuid_scheme_analyte t2                  " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN profjob_scheme_analyte t3                       " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN scheme_analyte t4                               " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.pro_job = '{JobNo}'                               " +
            $"		   AND t1.completed > '2000-01-01'                      ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectDistinct_Aurora(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"  SELECT distinct t1.pro_job,									" +
            $"                  t5.SAMPLEIDENT,                             " +
            $"                  t6.SCH_CODE                                 " +
            $"	FROM   Aurora.dbo.profjob t1                                " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID t5                      " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID_SCHEME t6               " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN Aurora.dbo.profjob_cuid_scheme_analyte t2       " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN Aurora.dbo.profjob_scheme_analyte t3            " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN Aurora.dbo.scheme_analyte t4                    " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.pro_job = '{JobNo}'                               " +
            $"		   AND t1.completed > '2000-01-01'                      ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectDistinctProjJob_KRSEC001(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"  SELECT distinct t1.pro_job,									" +
            $"                  t5.SAMPLEIDENT,                             " +
            $"                  t6.SCH_CODE                                 " +
            $"	FROM   Aurora.dbo.profjob t1                                " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID t5                      " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID_SCHEME t6               " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN Aurora.dbo.profjob_cuid_scheme_analyte t2       " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN Aurora.dbo.profjob_scheme_analyte t3            " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN Aurora.dbo.scheme_analyte t4                    " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.PRO_PROJ like '%%{ProjJobNo}%%'                   " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"		   AND t2.formattedvalue <> 'N.A.'                      ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectDistinctProjJob_Aurora(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"  SELECT distinct t1.pro_job,									" +
            $"                  t5.SAMPLEIDENT,                             " +
            $"                  t6.SCH_CODE                                 " +
            $"	FROM   Aurora.dbo.profjob t1                                " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID t5                      " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID_SCHEME t6               " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN Aurora.dbo.profjob_cuid_scheme_analyte t2       " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN Aurora.dbo.profjob_scheme_analyte t3            " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN Aurora.dbo.scheme_analyte t4                    " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.PRO_PROJ like '%%{ProjJobNo}%%'                   " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"		   AND t2.formattedvalue <> 'N.A.'                      ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectDistinctJob_Aurora(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"  SELECT distinct t1.pro_job,									" +
            $"                  t5.SAMPLEIDENT,                             " +
            $"                  t6.SCH_CODE                                 " +
            $"	FROM   KRCTS01.dbo.profjob t1                                " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID t5                      " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID_SCHEME t6               " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN KRCTS01.dbo.profjob_cuid_scheme_analyte t2       " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN KRCTS01.dbo.profjob_scheme_analyte t3            " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN KRCTS01.dbo.scheme_analyte t4                    " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.PRO_JOB like '%%{ProjJobNo}%%'                   " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"		   AND t2.formattedvalue <> 'N.A.'                      ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectDistinctSubProjJob_Aurora(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"  SELECT distinct t1.pro_job,									" +
            $"                  t5.SAMPLEIDENT,                             " +
            $"                  t6.SCH_CODE                                 " +
            $"	FROM   Aurora.dbo.profjob t1                                " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID t5                      " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID_SCHEME t6               " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN Aurora.dbo.profjob_cuid_scheme_analyte t2       " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN Aurora.dbo.profjob_scheme_analyte t3            " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN Aurora.dbo.scheme_analyte t4                    " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.PRO_PROJ like '%%{ProjJobNo}%%'                   " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"	       AND t1.NOTES1 = '6F'                                 " +
            $"		   AND t2.formattedvalue <> 'N.A.'                      ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectDistinctSubProJob_KRCTS01(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"  SELECT distinct t1.pro_job,									" +
            $"                  t5.SAMPLEIDENT,                             " +
            $"                  t6.SCH_CODE,                                " +
            $"                  t7.SAM_REMARKS                              " +
            $"	FROM   KRCTS01.dbo.profjob t1                               " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID t5                     " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID_SCHEME t6              " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN krcts01.dbo.profjob_cuiduser t7                 " +
            $"			 ON ( t7.labcode = t5.labcode                       " +
            $"				  AND t7.pro_job = t5.pro_job                   " +
            $"				  AND t7.cuid = t5.cuid )                       " +
            $"		   JOIN KRCTS01.dbo.profjob_cuid_scheme_analyte t2      " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN KRCTS01.dbo.profjob_scheme_analyte t3           " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN KRCTS01.dbo.scheme_analyte t4                   " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.PRO_JOB = '{JobNo}'                               " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"	       AND t1.NOTES1 <> '6F'                                " +
            $"		   AND t2.formattedvalue <> 'N.A.'                      ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectDistinctSubProJob_ASTM_KRCTS01(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"  SELECT distinct t1.pro_job,									" +
            $"                  t5.SAMPLEIDENT,                             " +
            $"                  t6.SCH_CODE,                                " +
            $"                  t7.SAM_REMARKS                              " +
            $"	FROM   KRCTS01.dbo.profjob t1                               " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID t5                     " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID_SCHEME t6              " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN krcts01.dbo.profjob_cuiduser t7                 " +
            $"			 ON ( t7.labcode = t5.labcode                       " +
            $"				  AND t7.pro_job = t5.pro_job                   " +
            $"				  AND t7.cuid = t5.cuid )                       " +
            $"		   JOIN KRCTS01.dbo.profjob_cuid_scheme_analyte t2      " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN KRCTS01.dbo.profjob_scheme_analyte t3           " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN KRCTS01.dbo.scheme_analyte t4                   " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.PRO_JOB = '{JobNo}'                               " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"	       AND t1.NOTES1 <> '6F'                                " +
            $"         AND t6.ANALYSED IS NOT NULL                          ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectDistinctJob_KRCTS01_SCHE_HCEEORGANOTIN_11_01(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"  SELECT distinct t1.pro_job,									" +
            $"                  t5.SAMPLEIDENT,                             " +
            $"                  t6.SCH_CODE                                 " +
            $"	FROM   KRCTS01.dbo.profjob t1                               " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID t5                     " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID_SCHEME t6              " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN KRCTS01.dbo.profjob_cuid_scheme_analyte t2      " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN KRCTS01.dbo.profjob_scheme_analyte t3           " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN KRCTS01.dbo.scheme_analyte t4                   " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	       WHERE  t1.PRO_JOB = '{JobNo}'                        " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"	       AND t1.NOTES1 <> '6F'                                " +
            $"	       AND t6.SCH_CODE = 'HCEEORGANOTIN_11_01'              " +
            $"		   AND t2.formattedvalue <> 'N.A.'                      ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectDistinctJob_KRCTS01_SCHE_1373_1372_1371(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"  SELECT distinct t1.pro_job,									" +
            $"                  t5.SAMPLEIDENT,                             " +
            $"                  t6.SCH_CODE                                 " +
            $"	FROM   KRCTS01.dbo.profjob t1                               " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID t5                     " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID_SCHEME t6              " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN KRCTS01.dbo.profjob_cuid_scheme_analyte t2      " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN KRCTS01.dbo.profjob_scheme_analyte t3           " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN KRCTS01.dbo.scheme_analyte t4                   " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	       WHERE  t1.PRO_JOB = '{JobNo}'                        " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"	       AND t1.NOTES1 <> '6F'                                " +
            $"	       AND (t6.SCH_CODE = 'HCEECPSC09' or t6.SCH_CODE = 'HCEECPSC08' or t6.SCH_CODE = 'HCEECPSC07') " +
            $"         AND t6.ANALYSED IS NOT NULL ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectDistinctJob_KRCTS01_SCHE_4487(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"  SELECT distinct t1.pro_job,									" +
            $"                  t5.SAMPLEIDENT,                             " +
            $"                  t6.SCH_CODE                                 " +
            $"	FROM   KRCTS01.dbo.profjob t1                               " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID t5                     " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID_SCHEME t6              " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN KRCTS01.dbo.profjob_cuid_scheme_analyte t2      " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN KRCTS01.dbo.profjob_scheme_analyte t3           " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN KRCTS01.dbo.scheme_analyte t4                   " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	       WHERE  t1.PRO_JOB = '{JobNo}'                        " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"	       AND t1.NOTES1 <> '6F'                                " +
            $"	       AND (t6.SCH_CODE = 'HCEEASTMICP_09') " +
            $"		   AND t2.formattedvalue <> 'N.A.'                      ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectDistinctMainProjJob_Aurora(int j, SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"  SELECT distinct t1.pro_job,									" +
            $"                  t5.SAMPLEIDENT,                             " +
            $"                  t6.sch_code                                 " +
            $"	FROM   Aurora.dbo.profjob t1                                " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID t5                      " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID_SCHEME t6               " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN Aurora.dbo.profjob_cuid_scheme_analyte t2       " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN Aurora.dbo.profjob_scheme_analyte t3            " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN Aurora.dbo.scheme_analyte t4                    " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.PRO_PROJ like '%%{ProjJobNo}%%'                   " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"		   AND t5.sampleident like '%.00{j}%'                   " +
            $"		   AND t2.formattedvalue <> 'N.A.'                      " +
            $"	       AND t1.NOTES1 <> '6F'                                 ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectDistinctMainProJob_Aurora(int j, SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"  SELECT distinct t1.pro_job,									" +
            $"                  t5.SAMPLEIDENT,                             " +
            $"                  t6.sch_code                                 " +
            $"	FROM   Aurora.dbo.profjob t1                                " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID t5                      " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID_SCHEME t6               " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN Aurora.dbo.profjob_cuid_scheme_analyte t2       " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN Aurora.dbo.profjob_scheme_analyte t3            " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN Aurora.dbo.scheme_analyte t4                    " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.PRO_JOB like '%%{ProjJobNo}%%'                   " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"		   AND t5.sampleident like '%.00{j}%'                   " +
            $"		   AND t2.formattedvalue <> 'N.A.'                      " +
            $"	       AND t1.NOTES1 <> '6F'                                 ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectSampleident(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"   SELECT t1.pro_job,											" +
            $"   t5.CUID,                                                   " +
            $"   t5.SAMPLEIDENT,                                            " +
            $"   t1.registered,                                             " +
            $"   t3.sch_code,                                               " +
            $"   t4.description,                                            " +
            $"   t3.lvl1lowerlimit,                                         " +
            $"   t3.lvl1upperlimit,                                         " +
            $"   t3.repdetlimit,                                            " +
            $"   t2.formattedvalue,                                         " +
            $"   t7.DESCRIPTION_4                                           " +
            $"	    FROM   profjob t1                                       " +
            $"		   JOIN PROFJOB_CUID t5                                 " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN PROFJOB_CUID_SCHEME t6                          " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN PROFJOB_CUIDUSER t7                             " +
            $"			 ON ( t7.labcode = t5.labcode                       " +
            $"				  AND t7.PRO_JOB = t5.PRO_JOB                   " +
            $"				  AND t7.CUID = t5.CUID )                       " +
            $"		   JOIN profjob_cuid_scheme_analyte t2                  " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN profjob_scheme_analyte t3                       " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN scheme_analyte t4                               " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.pro_job = '{JobNo}'                               " +
            $"	              AND t5.SAMPLEIDENT = '{SAMPLEIDENT}'          " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"	ORDER  BY t3.repsequence ASC                                ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectSampleident_Aurora(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"   SELECT t1.pro_job,											" +
            $"   t5.CUID,                                                   " +
            $"   t5.SAMPLEIDENT,                                            " +
            $"   t1.registered,                                             " +
            $"   t3.sch_code,                                               " +
            $"   t4.description,                                            " +
            $"   t3.lvl1lowerlimit,                                         " +
            $"   t3.lvl1upperlimit,                                         " +
            $"   t3.repdetlimit,                                            " +
            $"   t2.formattedvalue,                                         " +
            $"   t7.DESCRIPTION_4                                           " +
            $"	    FROM   Aurora.dbo.profjob t1                            " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID t5                      " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID_SCHEME t6               " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUIDUSER t7                  " +
            $"			 ON ( t7.labcode = t5.labcode                       " +
            $"				  AND t7.PRO_JOB = t5.PRO_JOB                   " +
            $"				  AND t7.CUID = t5.CUID )                       " +
            $"		   JOIN Aurora.dbo.profjob_cuid_scheme_analyte t2       " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN Aurora.dbo.profjob_scheme_analyte t3            " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN Aurora.dbo.scheme_analyte t4                    " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.pro_job = '{JobNo}'                               " +
            $"	       AND t5.SAMPLEIDENT = '{SAMPLEIDENT}'                 " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"		   AND t2.FORMATTEDVALUE <> 'N.A.'                      " +
            $"	ORDER  BY t3.repsequence ASC                                ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectSampleidentProj_Aurora(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"   SELECT t1.pro_job,											" +
            $"   t5.CUID,                                                   " +
            $"   t5.SAMPLEIDENT,                                            " +
            $"   t1.registered,                                             " +
            $"   t3.sch_code,                                               " +
            $"   t4.description,                                            " +
            $"   t3.lvl1lowerlimit,                                         " +
            $"   t3.lvl1upperlimit,                                         " +
            $"   t3.repdetlimit,                                            " +
            $"   t2.formattedvalue,                                         " +
            $"   t7.DESCRIPTION_4                                           " +
            $"	    FROM   Aurora.dbo.profjob t1                            " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID t5                      " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID_SCHEME t6               " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUIDUSER t7                  " +
            $"			 ON ( t7.labcode = t5.labcode                       " +
            $"				  AND t7.PRO_JOB = t5.PRO_JOB                   " +
            $"				  AND t7.CUID = t5.CUID )                       " +
            $"		   JOIN Aurora.dbo.profjob_cuid_scheme_analyte t2       " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN Aurora.dbo.profjob_scheme_analyte t3            " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN Aurora.dbo.scheme_analyte t4                    " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.pro_proj like '%%{ProjJobNo}%%'                   " +            
            $"	       AND t5.SAMPLEIDENT = '{SAMPLEIDENT}'                 " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"		   AND t2.FORMATTEDVALUE <> 'N.A.'                      " +
            $"	ORDER  BY t3.repsequence ASC                                ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectSampleidentProAll_KRCTS01(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"   SELECT t1.pro_job,											" +
            $"   t5.CUID,                                                   " +
            $"   t5.SAMPLEIDENT,                                            " +
            $"   t1.registered,                                             " +
            $"   t3.sch_code,                                               " +
            $"   t4.description,                                            " +
            $"   t3.lvl1lowerlimit,                                         " +
            $"   t3.lvl1upperlimit,                                         " +
            $"   t3.repdetlimit,                                            " +
            $"   t2.formattedvalue,                                         " +
            $"   t7.sam_description,                                        " +
            $"   t7.DESCRIPTION_4                                           " +
            $"	    FROM   KRCTS01.dbo.profjob t1                           " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID t5                     " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID_SCHEME t6              " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUIDUSER t7                 " +
            $"			 ON ( t7.labcode = t5.labcode                       " +
            $"				  AND t7.PRO_JOB = t5.PRO_JOB                   " +
            $"				  AND t7.CUID = t5.CUID )                       " +
            $"		   JOIN KRCTS01.dbo.profjob_cuid_scheme_analyte t2      " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN KRCTS01.dbo.profjob_scheme_analyte t3           " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN KRCTS01.dbo.scheme_analyte t4                   " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.pro_job = '{JobNo}'                               " +
            $"	       AND t5.SAMPLEIDENT = '{SAMPLEIDENT}'                 " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"		   AND t2.FORMATTEDVALUE <> 'N.A.'                      " +            
            $"	ORDER  BY t3.repsequence ASC                                ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectSampleidentPro_KRCTS01(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"   SELECT t1.pro_job,											" +
            $"   t1.PRO_PROJ,                                               " +
            $"   t5.CUID,                                                   " +
            $"   t5.SAMPLEIDENT,                                            " +
            $"   t1.registered,                                             " +
            $"   t4.sch_code,                                               " +
            $"   t4.description,                                            " +
            $"   t4.lvl1lowerlimit,                                         " +
            $"   t4.lvl1upperlimit,                                         " +
            $"   t4.repdetlimit,                                            " +
            $"   t2.formattedvalue,                                         " +
            $"   t7.sam_description,                                        " +
            $"   t7.SAM_REMARKS,                                            " +
            $"   t7.DESCRIPTION_4,                                          " +
            $"   t6.WEIGHT                                                  " +
            $"	    FROM   KRCTS01.dbo.profjob t1                           " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID t5                     " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID_SCHEME t6              " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUIDUSER t7                 " +
            $"			 ON ( t7.labcode = t5.labcode                       " +
            $"				  AND t7.PRO_JOB = t5.PRO_JOB                   " +
            $"				  AND t7.CUID = t5.CUID )                       " +
            $"		   JOIN KRCTS01.dbo.profjob_cuid_scheme_analyte t2      " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN KRCTS01.dbo.profjob_scheme_analyte t3           " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN KRCTS01.dbo.scheme_analyte t4                   " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.pro_job = '{JobNo}'                               " +
            $"	       AND t5.SAMPLEIDENT = '{SAMPLEIDENT}'                 " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"		   AND t2.FORMATTEDVALUE <> 'N.A.'                      " +
            $"		   AND t4.sch_code = '{Sch_Code}'                       " +
            $"	ORDER  BY t3.repsequence ASC                                ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectSampleidentPro_ASTM_KRCTS01(SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"   SELECT t1.pro_job,											" +
            $"   t1.PRO_PROJ,                                               " +
            $"   t5.CUID,                                                   " +
            $"   t5.SAMPLEIDENT,                                            " +
            $"   t1.registered,                                             " +
            $"   t4.sch_code,                                               " +
            $"   t4.description,                                            " +
            $"   t4.lvl1lowerlimit,                                         " +
            $"   t4.lvl1upperlimit,                                         " +
            $"   t2.FINALVALUE,                                             " +
            $"   t4.repdetlimit,                                            " +
            $"   t2.formattedvalue,                                         " +
            $"   t7.sam_description,                                        " +
            $"   t7.SAM_REMARKS,                                            " +
            $"   t7.DESCRIPTION_4,                                          " +
            $"   t6.WEIGHT                                                  " +
            $"	    FROM   KRCTS01.dbo.profjob t1                           " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID t5                     " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUID_SCHEME t6              " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN KRCTS01.dbo.PROFJOB_CUIDUSER t7                 " +
            $"			 ON ( t7.labcode = t5.labcode                       " +
            $"				  AND t7.PRO_JOB = t5.PRO_JOB                   " +
            $"				  AND t7.CUID = t5.CUID )                       " +
            $"		   JOIN KRCTS01.dbo.profjob_cuid_scheme_analyte t2      " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN KRCTS01.dbo.profjob_scheme_analyte t3           " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN KRCTS01.dbo.scheme_analyte t4                   " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.pro_job = '{JobNo}'                               " +
            $"	       AND t5.SAMPLEIDENT = '{SAMPLEIDENT}'                 " +
            $"		   AND t1.completed > '2000-01-01'                      " +            
            $"		   AND t4.sch_code = '{Sch_Code}'                       " +
            $"	ORDER  BY t3.repsequence ASC                                ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void SelectSampleidentProj_Aurora(int k, SqlTransaction trans = null)
        {
            SetTrans(trans);

            command.CommandText =
            $"   SELECT t1.pro_job,											" +
            $"   t5.CUID,                                                   " +
            $"   t5.SAMPLEIDENT,                                            " +
            $"   t1.registered,                                             " +
            $"   t3.sch_code,                                               " +
            $"   t4.description,                                            " +
            $"   t3.lvl1lowerlimit,                                         " +
            $"   t3.lvl1upperlimit,                                         " +
            $"   t3.repdetlimit,                                            " +
            $"   t2.formattedvalue,                                         " +
            $"   t7.DESCRIPTION_4                                           " +
            $"	    FROM   Aurora.dbo.profjob t1                            " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID t5                      " +
            $"			 ON ( t1.LABCODE = t5.LABCODE                       " +
            $"				  AND t1.PRO_JOB = t5.PRO_JOB )                 " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUID_SCHEME t6               " +
            $"			 ON ( t5.LABCODE = t6.LABCODE                       " +
            $"				  AND t5.PRO_JOB = t6.PRO_JOB                   " +
            $"				  AND t5.CUID = t6.CUID )                       " +
            $"		   JOIN Aurora.dbo.PROFJOB_CUIDUSER t7                  " +
            $"			 ON ( t7.labcode = t5.labcode                       " +
            $"				  AND t7.PRO_JOB = t5.PRO_JOB                   " +
            $"				  AND t7.CUID = t5.CUID )                       " +
            $"		   JOIN Aurora.dbo.profjob_cuid_scheme_analyte t2       " +
            $"			 ON ( t6.LABCODE = t2.LABCODE                       " +
            $"				  AND t6.pro_job = t2.pro_job                   " +
            $"				  AND t6.CUID = t2.CUID                         " +
            $"				  AND t6.SCH_CODE = t2.SCH_CODE                 " +
            $"				  AND t6.SCHVERSION = t2.SCHVERSION)            " +
            $"		   JOIN Aurora.dbo.profjob_scheme_analyte t3            " +
            $"			 ON ( t3.labcode = t2.labcode                       " +
            $"				  AND t3.pro_job = t2.pro_job                   " +
            $"				  AND t3.sch_code = t2.sch_code                 " +
            $"				  AND t3.analytecode = t2.analytecode )         " +
            $"		   JOIN Aurora.dbo.scheme_analyte t4                    " +
            $"			 ON ( t4.labcode = t2.labcode                       " +
            $"				  AND t4.sch_code = t2.sch_code                 " +
            $"				  AND t4.schversion = t2.schversion             " +
            $"				  AND t4.analytecode = t2.analytecode )         " +
            $"	WHERE  t1.pro_proj like '%%{ProjJobNo}%%'                   " +
            $"		   AND t5.sampleident like '%.00{k}%'                   " +
            //$"	       AND t5.SAMPLEIDENT = '{SAMPLEIDENT}'                 " +
            $"		   AND t1.completed > '2000-01-01'                      " +
            $"		   AND t2.FORMATTEDVALUE <> 'N.A.'                      " +
            $"	ORDER  BY t3.repsequence ASC                                ";

            dataSet.Clear();
            dataAdapter.Fill(dataSet);
        }

        public void Fetch(int index = 0, int tableNo = 0, string type = "", string sChkJobCase = "")
        {
            int iRowCount = 0;

            if (index < GetRowCount(tableNo))
            {
                iRowCount = GetRowCount(tableNo);
                Fetch(dataSet.Tables[tableNo].Rows[index], iRowCount, type, sChkJobCase);
            }
            else
            {
                Area = EReportArea.None;
                Lead = ELeadType.None;
                JobNo = "";
                Name = "";
                LoValue = "";
                HiValue = "";
                ReportValue = "";
                FormatValue = "";
            }
        }

        public void Fetch(DataRow row, int RowCount, string type = "", string sChkJobCase = "")
        {   
            if (RowCount < 3 && type.Equals("") || type.Equals("check"))
            {
                JobNo = Convert.ToString(row["pro_job"]);
                SAMPLEIDENT = Convert.ToString(row["SAMPLEIDENT"]);
                Sch_Code = Convert.ToString(row["sch_code"]);
                try
                {
                    SampleRemarks = Convert.ToString(row["SAM_REMARKS"]);
                }
                catch (Exception f)
                {

                }
            }
            else if (type.Equals("check"))
            {
                JobNo = Convert.ToString(row["pro_job"]);
                SAMPLEIDENT = Convert.ToString(row["SAMPLEIDENT"]);
                Sch_Code = Convert.ToString(row["sch_code"]);
                try
                {
                    SampleRemarks = Convert.ToString(row["SAM_REMARKS"]);
                }
                catch (Exception f)
                {

                }
            }
            else
            {
                JobNo = Convert.ToString(row["pro_job"]);
                fileNo = Convert.ToString(row["PRO_PROJ"]);
                SAMPLEIDENT = Convert.ToString(row["SAMPLEIDENT"]);
                RegTime = Convert.ToDateTime(row["registered"]);
                //String code = Convert.ToString(row["sch_code"]);
                Sch_Code = Convert.ToString(row["sch_code"]);
                Name = Convert.ToString(row["description"]);
                LoValue = Convert.ToString(row["lvl1lowerlimit"]);
                HiValue = Convert.ToString(row["lvl1upperlimit"]);
                ReportValue = Convert.ToString(row["repdetlimit"]);
                FormatValue = Convert.ToString(row["formattedvalue"]);
                SampleDescription = Convert.ToString(row["sam_description"]);
                DESCRIPTION_4 = Convert.ToString(row["DESCRIPTION_4"]);

                WEIGHT = Convert.ToString(row["WEIGHT"]);
                if (string.IsNullOrEmpty(WEIGHT))
                {
                    WEIGHT = "";
                }
                try
                {
                    SampleRemarks = Convert.ToString(row["SAM_REMARKS"]);
                    FINALVALUE = Convert.ToString(row["FINALVALUE"]);
                    
                }
                catch (Exception f) 
                {

                }                

                if (sChkJobCase == "surface")
                {
                    Lead = ELeadType.Surface;
                }
                else if (sChkJobCase == "substrate")
                {
                    Lead = ELeadType.Substrate;
                }
                else
                {
                    //Console.WriteLine("Fetch! - Else : " + sChkJobCase);
                }

                /*
                if (code.Substring(0, 8) == "HCEECPSC")
                {
                    if (code.Equals("HCEECPSC07") || code.Equals("HCEECPSC08"))
                    {
                        Lead = ELeadType.Substrate;
                    }
                    else if (code.Equals("HCEECPSC09"))
                    {
                        Lead = ELeadType.Surface;
                    }
                    else
                    {
                        Lead = ELeadType.None;
                    }
                    //Lead = (code == "HCEECPSC08") ? ELeadType.Substrate : ELeadType.Surface;
                }
                else
                {
                    Area = (code.Substring(4, 2) == "AS") ? EReportArea.US : EReportArea.EU;
                    Lead = ELeadType.None;
                }
                */

                if (string.IsNullOrWhiteSpace(LoValue) == true)
                {
                    LoValue = "--";
                }

                if (string.IsNullOrWhiteSpace(HiValue) == true)
                {
                    HiValue = "--";
                }

                if (string.IsNullOrWhiteSpace(ReportValue) == true)
                {
                    ReportValue = "--";
                }

                if (string.IsNullOrWhiteSpace(FormatValue) == true)
                {
                    FormatValue = "--";
                }

                if (string.IsNullOrWhiteSpace(FINALVALUE) == true)
                {
                    FINALVALUE = "0.00000001";
                }

                if (FormatValue.Substring(0, 1) == "<")
                {
                    FormatValue = "N.D.";
                }
            }
        }
    }
}



