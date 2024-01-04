﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using Ulee.Utils;

namespace Sgs.ReportIntegration
{
    #region Enum
    public enum EReportArea
    {
        [Description("None")]
        None = -1,
        [Description("ASTM")]
        US = 0,
        [Description("EN")]
        EU = 1
    }

    public enum EReportAuthority
    {
        [Description("None")]
        None = -1,
        [Description("Administrator")]
        Admin = 0,
        [Description("Manager")]
        Manager = 1
    }

    public enum EReportApproval
    {
        [Description("None")]
        None = -1,
        [Description("No")]
        NotApproved = 0,
        [Description("Yes")]
        Approved = 1
    }

    public enum EReportType
    {
        [Description("None")]
        None = -1,
        [Description("Physical")]
        Physical = 0,
        [Description("Chemical")]
        Chemical = 1,
        [Description("Integration")]
        Integration = 2
    }

    public enum ELeadType
    {
        [Description("None")]
        None = -1,
        [Description("Substrate")]
        Substrate = 0,
        [Description("Surface")]
        Surface = 1
    }

    #endregion

    #region GridTypeFormat
    class ReportAreaFormat : IFormatProvider, ICustomFormatter
    {
        public ReportAreaFormat()
        {
        }

        public object GetFormat(Type type)
        {
            return this;
        }

        public string Format(string formatString, object arg, IFormatProvider formatProvider)
        {
            return ((EReportArea)arg).ToDescription();
        }
    }

    class ReportDateTimeFormat : IFormatProvider, ICustomFormatter
    {
        public ReportDateTimeFormat()
        {
        }

        public object GetFormat(Type type)
        {
            return this;
        }

        public string Format(string formatString, object arg, IFormatProvider formatProvider)
        {
            string time = arg.ToString();

            return time.Substring(2, 17);
        }
    }
    #endregion

    #region Class
    public class BomColumns
    {
        public Int64 RecNo { get; set; }
        public DateTime RegTime { get; set; }
        public EReportArea AreaNo { get; set; }
        public string FName { get; set; }
        public string FPath { get; set; }

        public List<ProductColumns> Products { get; set; }

        public BomColumns()
        {
            Products = new List<ProductColumns>();
            Clear();
        }

        public void Clear()
        {
            RecNo = 0;
            RegTime = DateTime.Now;
            AreaNo = EReportArea.None;
            FName = "";
            FPath = "";

            foreach (ProductColumns product in Products)
            {
                product.Clear();
            }

            Products.Clear();
        }

        public void Add(ProductColumns col)
        {
            Products.Add(col);
        }
    }

    public class ProductColumns
    {
        public Int64 RecNo { get; set; }
        public Int64 BomNo { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Bitmap Image { get; set; }

        public List<PartColumns> Parts { get; set; }

        public ProductColumns()
        {
            Parts = new List<PartColumns>();
            Clear();
        }

        public void Clear()
        {
            RecNo = 0;
            BomNo = 0;
            Code = "";
            Name = "";
            Image = null;

            Parts.Clear();
        }

        public void Add(PartColumns col)
        {
            Parts.Add(col);
        }
    }

    public class PartColumns
    {
        public Int64 RecNo { get; set; }
        public Int64 ProductNo { get; set; }
        public string Name { get; set; }
        public string MaterialNo { get; set; }
        public string MaterialName { get; set; }

        public PartColumns()
        {
            RecNo = 0;
            ProductNo = 0;
            Name = "";
            MaterialNo = "";
            MaterialName = "";
        }
    }

    public class IntegrationT1Row
    {
        public int No { get; set; }

        public bool Line { get; set; }

        public string Requested { get; set; }

        public string Conclusion { get; set; }

        public IntegrationT1Row()
        {
            No = 0;
            Line = false;
            Requested = "";
            Conclusion = "";
        }
    }

    public class IntegrationT2Row
    {
        public int No { get; set; }

        public bool Line { get; set; }

        public string Clause { get; set; }

        public string Description { get; set; }

        public string Result { get; set; }

        public IntegrationT2Row()
        {
            No = 0;
            Line = false;
            Clause = "";
            Description = "";
            Result = "";
        }
    }
    public class IntegrationT6Row
    {
        public int No { get; set; }

        public bool Line { get; set; }

        public string TestItem { get; set; }

        public string Result { get; set; }

        public string Requirement { get; set; }

        public IntegrationT6Row()
        {
            No = 0;
            Line = false;
            TestItem = "";
            Result = "";
            Requirement = "";
        }
    }

    public class PhysicalPage2Row
    {
        public int No { get; set; }

        public bool Line { get; set; }

        public string Requested { get; set; }

        public string Conclusion { get; set; }

        public PhysicalPage2Row()
        {
            No = 0;
            Line = false;
            Requested = "";
            Conclusion = "";
        }
    }

    public class PhysicalPage3Row
    {
        public int No { get; set; }

        public bool Line { get; set; }

        public string Clause { get; set; }

        public string Description { get; set; }

        public string Result { get; set; }

        public string Comment { get; set; }

        public PhysicalPage3Row()
        {
            No = 0;
            Line = false;
            Clause = "";
            Description = "";
            Result = "";
            Comment = "";
        }
    }

    public class PhysicalPage4Row
    {
        public int No { get; set; }

        public bool Line { get; set; }

        public string Sample { get; set; }

        public string BurningRate { get; set; }

        public string Result { get; set; }

        public PhysicalPage4Row()
        {
            No = 0;
            Line = false;
            Sample = "";
            BurningRate = "";
            Result = "";
        }
    }

    public class PhysicalPage4_4_4_2_2_4Row
    {
        public int No { get; set; }

        public bool Line { get; set; }

        public string Sample { get; set; }

        public string Clause { get; set; }

        public string Desc { get; set; }

        public string BurningRate { get; set; }

        public string Note { get; set; }

        public PhysicalPage4_4_4_2_2_4Row()
        {
            No = 0;
            Line = false;
            Sample = "";
            BurningRate = "";
            Note = "";
            Clause = "";
            Desc = "";
        }
    }

    public class PhysicalPage4_5_4Row
    {
        public int No { get; set; }

        public bool Line { get; set; }

        public string Sample { get; set; }

        public string Clause { get; set; }

        public string Desc { get; set; }

        public string BurningRate { get; set; }

        public string Result { get; set; }

        public string Note { get; set; }

        public PhysicalPage4_5_4Row()
        {
            No = 0;
            Line = false;
            Sample = "";
            BurningRate = "";
            Note = "";
            Clause = "";
            Desc = "";
        }
    }
    /*
    public class PhysicalPage4_4_4_2_3Row
    {
        public int No { get; set; }

        public bool Line { get; set; }

        public string Sample { get; set; }

        public string BurningRate { get; set; }

        public string Note { get; set; }

        public PhysicalPage4_4_4_2_3Row()
        {
            No = 0;
            Line = false;
            Sample = "";
            BurningRate = "";
            Note = "";
        }
    }

    public class PhysicalPage4_4_4_2_4Row
    {
        public int No { get; set; }

        public bool Line { get; set; }

        public string Sample { get; set; }

        public string BurningRate { get; set; }

        public string Note { get; set; }

        public PhysicalPage4_4_4_2_4Row()
        {
            No = 0;
            Line = false;
            Sample = "";
            BurningRate = "";
            Note = "";
        }
    }
    */
    public class PhysicalPage5Row
    {
        public int No { get; set; }

        public bool Line { get; set; }

        public string TestItem { get; set; }

        public string Result { get; set; }

        public string Requirement { get; set; }

        public PhysicalPage5Row()
        {
            No = 0;
            Line = false;
            TestItem = "";
            Result = "";
            Requirement = "";
        }
    }

    public class PhysicalPage6Row
    {
        public int No { get; set; }

        public bool Line { get; set; }

        public string TestItem { get; set; }

        public string Result { get; set; }

        public string Requirement { get; set; }

        public string Note { get; set; }

        public string Description { get; set; }

        public PhysicalPage6Row()
        {
            No = 0;
            Line = false;
            TestItem = "";
            Result = "";
            Requirement = "";
            Note = "";
            Description = "";
        }
    }

    public class ChemicalPage2Row
    {
        public Int64 RecNo { get; set; }

        public string HiLimit { get; set; }

        public string LoLimit { get; set; }

        public string ReportLimit { get; set; }

        public string FormatValue { get; set; }

        public string Name { get; set; }

        public ChemicalPage2Row()
        {
            RecNo = 0;
            HiLimit = "";
            LoLimit = "";
            ReportLimit = "";
            FormatValue = "";
            Name = "";
        }
    }

    public class ChemicalPage2ExtendRow
    {
        public string RecNo { get; set; }

        public string TotalLimit { get; set; }

        public string ReportLimit { get; set; }

        public string Message { get; set; }

        public string FormatValue { get; set; }

        public string Name { get; set; }

        public ChemicalPage2ExtendRow()
        {
            RecNo = "";
            TotalLimit = "";
            ReportLimit = "";
            Message = "";
            FormatValue = "";
            Name = "";
        }
    }
    #endregion
}
