using DevExpress.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sgs.ReportIntegration
{
    public class IntegrationQuery
    {
        public IntegrationMainDataSet MainSet { get; set; }

        public IntegrationImageDataSet ImageSet { get; set; }

        public IntegrationT1DataSet T1Set { get; set; }

        public IntegrationT2DataSet T2Set { get; set; }

        public IntegrationT3DataSet T3Set { get; set; }

        public IntegrationT4DataSet T4Set { get; set; }

        public IntegrationT5DataSet T5Set { get; set; }

        public IntegrationT6DataSet T6Set { get; set; }

        public IntegrationT7DataSet T7Set { get; set; }

        public PhysicalP2DataSet P2Set { get; set; }

        public PhysicalP3DataSet P3Set { get; set; }

        public PhysicalP40DataSet P40Set { get; set; }

        public PhysicalP41DataSet P41Set { get; set; }

        public PhysicalP42DataSet P42Set { get; set; }

        public PhysicalP5DataSet P5Set { get; set; }

        public IntegrationLimitEnDataSet LimitEnSet { get; set; }

        public IntegrationResultEnDataSet ResultEnSet { get; set; }

        public IntegrationLeadLimitAstmDataSet SurfaceLeadLimitAstmSet { get; set; }

        public IntegrationLeadResultAstmDataSet SurfaceLeadResultAstmSet { get; set; }

        public IntegrationLimitAstmDataSet SurfaceLimitAstmSet { get; set; }

        public IntegrationResultAstmDataSet SurfaceResultAstmSet {get; set; }

        public IntegrationLeadLimitAstmDataSet SubstrateLeadLimitAstmSet { get; set; }

        public IntegrationLeadResultAstmDataSet SubstrateLeadResultAstmSet { get; set; }

        public IntegrationLimitAstmDataSet SubstrateLimitAstmSet { get; set; }

        public IntegrationResultAstmDataSet SubstrateResultAstmSet { get; set; }

        public ProfJobDataSet ProfJobSet { get; set; }

        public ProductDataSet ProductSet { get; set; }

        public PartDataSet partSet;
        
        private PhysicalMainDataSet phyMainSet;

        private PhysicalImageDataSet phyImageSet;

        private ChemicalMainDataSet cheMainSet;

        private ChemicalP2DataSet cheP2Set;

        private ChemicalP2ExtendDataSet cheP2ExtendSet;        

        public CtrlEditIntegrationUs CtrlUs { get; set; }

        public CtrlEditIntegrationEu CtrlEu { get; set; }

        private bool local;

        private bool bSubstratePlasticLeadCheck;

        private bool bSubstrateMetalLeadCheck;

        private int surfaceLeadIndex;

        private int surfaceResultIndex;

        private int substrateLeadIndex;

        private int substrateResultIndex;

        public IntegrationQuery(bool local = false)
        {
            this.local = local;

            if (local == true)
            {
                MainSet = new IntegrationMainDataSet(AppRes.DB.Connect, null, null);
                ImageSet = new IntegrationImageDataSet(AppRes.DB.Connect, null, null);
                T1Set = new IntegrationT1DataSet(AppRes.DB.Connect, null, null);
                T2Set = new IntegrationT2DataSet(AppRes.DB.Connect, null, null);
                T3Set = new IntegrationT3DataSet(AppRes.DB.Connect, null, null);
                T4Set = new IntegrationT4DataSet(AppRes.DB.Connect, null, null);
                T5Set = new IntegrationT5DataSet(AppRes.DB.Connect, null, null);                
                P2Set = new PhysicalP2DataSet(AppRes.DB.Connect, null, null);
                P3Set = new PhysicalP3DataSet(AppRes.DB.Connect, null, null);
                P40Set = new PhysicalP40DataSet(AppRes.DB.Connect, null, null);
                P41Set = new PhysicalP41DataSet(AppRes.DB.Connect, null, null);
                P42Set = new PhysicalP42DataSet(AppRes.DB.Connect, null, null);
                P5Set = new PhysicalP5DataSet(AppRes.DB.Connect, null, null);
                T6Set = new IntegrationT6DataSet(AppRes.DB.Connect, null, null);
                T7Set = new IntegrationT7DataSet(AppRes.DB.Connect, null, null);
                LimitEnSet = new IntegrationLimitEnDataSet(AppRes.DB.Connect, null, null);
                ResultEnSet = new IntegrationResultEnDataSet(AppRes.DB.Connect, null, null);
                SurfaceLeadLimitAstmSet = new IntegrationLeadLimitAstmDataSet(AppRes.DB.Connect, null, null);
                SurfaceLeadResultAstmSet = new IntegrationLeadResultAstmDataSet(AppRes.DB.Connect, null, null);
                SurfaceLimitAstmSet = new IntegrationLimitAstmDataSet(AppRes.DB.Connect, null, null);
                SurfaceResultAstmSet = new IntegrationResultAstmDataSet(AppRes.DB.Connect, null, null);
                SubstrateLeadLimitAstmSet = new IntegrationLeadLimitAstmDataSet(AppRes.DB.Connect, null, null);
                SubstrateLeadResultAstmSet = new IntegrationLeadResultAstmDataSet(AppRes.DB.Connect, null, null);
                SubstrateLimitAstmSet = new IntegrationLimitAstmDataSet(AppRes.DB.Connect, null, null);
                SubstrateResultAstmSet = new IntegrationResultAstmDataSet(AppRes.DB.Connect, null, null);
                ProductSet = new ProductDataSet(AppRes.DB.Connect, null, null);
                ProfJobSet = new ProfJobDataSet(AppRes.DB.Connect, null, null);
                CtrlUs = null;
                CtrlEu = null;
            }

            partSet = new PartDataSet(AppRes.DB.Connect, null, null);
            phyMainSet = new PhysicalMainDataSet(AppRes.DB.Connect, null, null);
            phyImageSet = new PhysicalImageDataSet(AppRes.DB.Connect, null, null);
            cheMainSet = new ChemicalMainDataSet(AppRes.DB.Connect, null, null);
            cheP2Set = new ChemicalP2DataSet(AppRes.DB.Connect, null, null);
            cheP2ExtendSet = new ChemicalP2ExtendDataSet(AppRes.DB.Connect, null, null);

            bSubstrateMetalLeadCheck = false;
            bSubstratePlasticLeadCheck = false;
            surfaceLeadIndex = 0;
            surfaceResultIndex = 0;
            substrateLeadIndex = 0;
            substrateResultIndex = 0;
        }

        public void Insert(SqlTransaction trans = null)
        {
            EReportArea area = ProfJobSet.AreaNo;

            if (local == false)
            {
                trans = AppRes.DB.BeginTrans();
            }
            // ASTM과EU case는 내부에서 area로 분기처리하여 입력된다.
            try
            {
                InsertMain(area, trans);        //
                InsertImage(trans);             //
                InsertT1(area, trans);          // page2
                InsertT2(area, trans);          // page3 US tb_integt2
                InsertT3(area, trans);          // 
                InsertT4(area, trans);          // page4 EN
                InsertT5(area, trans);          // page4 US, EN
                InsertT6(area, trans);          // page8
                InsertT7(area, trans);          // page5, 6
                UpdateProductSet(area, trans);

                if (local == false)
                {
                    AppRes.DB.CommitTrans();
                }
            }
            catch (Exception e)
            {
                if (local == false)
                {
                    AppRes.DB.RollbackTrans();
                }
                else
                {
                    throw e;
                }
            }
        }

        public void Update()
        {
            if (local == true)
            {
                throw new Exception("Can't call IntegrationQuery.Update() method in Local transaction mode!");
            }

            EReportArea area = MainSet.AreaNo;
            SqlTransaction trans = AppRes.DB.BeginTrans();

            try
            {
                SaveMain(area, trans);
                SaveT1(area, trans);
                SaveT2(area, trans);
                SaveT3(area, trans);
                SaveT4(area, trans);
                SaveT5(area, trans);
                SaveT6(area, trans);

                AppRes.DB.CommitTrans();
            }
            catch
            {
                AppRes.DB.RollbackTrans();
            }
        }

        public void Delete()
        {
            if (local == true)
            {
                throw new Exception("Can't call IntegrationQuery.Delete() method in Local transaction mode!");
            }

            string mainNo = MainSet.RecNo;
            SqlTransaction trans = AppRes.DB.BeginTrans();

            try
            {
                T1Set.MainNo = mainNo;
                T1Set.Delete(trans);
                T2Set.MainNo = mainNo;
                T2Set.Delete(trans);
                T3Set.MainNo = mainNo;
                T3Set.Delete(trans);
                T4Set.MainNo = mainNo;
                T4Set.Delete(trans);
                T5Set.MainNo = mainNo;
                T5Set.Delete(trans);
                T6Set.MainNo = mainNo;
                T6Set.Delete(trans);
                T7Set.MainNo = mainNo;
                T7Set.Delete(trans);
                LimitEnSet.MainNo = mainNo;
                LimitEnSet.Delete(trans);
                ResultEnSet.MainNo = mainNo;
                ResultEnSet.Delete(trans);
                // 추가된 delete hyphen
                ResultEnSet.MainNo = mainNo;
                ResultEnSet.Delete_HYPHEN(trans);
                SurfaceLeadLimitAstmSet.MainNo = mainNo;
                SurfaceLeadLimitAstmSet.Delete(trans);
                SurfaceLeadResultAstmSet.MainNo = mainNo;
                SurfaceLeadResultAstmSet.Delete(trans);
                SurfaceLimitAstmSet.MainNo = mainNo;
                SurfaceLimitAstmSet.Delete(trans);
                SurfaceResultAstmSet.MainNo = mainNo;
                SurfaceResultAstmSet.Delete(trans);
                SubstrateLeadLimitAstmSet.MainNo = mainNo;
                SubstrateLeadLimitAstmSet.Delete(trans);
                SubstrateLeadResultAstmSet.MainNo = mainNo;
                SubstrateLeadResultAstmSet.Delete(trans);
                SubstrateLimitAstmSet.MainNo = mainNo;
                SubstrateLimitAstmSet.Delete(trans);
                SubstrateResultAstmSet.MainNo = mainNo;
                SubstrateResultAstmSet.Delete(trans);
                ImageSet.RecNo = mainNo;
                ImageSet.Delete(trans);
                MainSet.RecNo = mainNo;
                MainSet.Delete(trans);

                ProductSet.IntegJobNo = mainNo;
                ProductSet.UpdateIntegJobNoReset(trans);
                ProductSet.UpdateValidReset(trans);

                AppRes.DB.CommitTrans();
            }
            catch
            {
                AppRes.DB.RollbackTrans();
            }
        }

        private void InsertMain(EReportArea area, SqlTransaction trans)
        {
            phyMainSet.RecNo = $"*{ProductSet.PhyJobNo}";
            phyMainSet.Select(trans);
            phyMainSet.Fetch();

            //string fileNoDate = $"F690101/LF-CTS{ProfJobSet.FileNo} dated {phyMainSet.ReportedTime.ToString("yyyy-MM-dd")}";
            string fileNoDate = $"F690101/LF-CTS{phyMainSet.P1FileNo} dated {phyMainSet.ReportedTime.ToString("yyyy-MM-dd")}";

            MainSet.RecNo = ProfJobSet.JobNo;
            MainSet.RegTime = ProfJobSet.RegTime;
            MainSet.ReceivedTime = ProfJobSet.ReceivedTime;
            MainSet.RequiredTime = ProfJobSet.RequiredTime;
            MainSet.ReportedTime = ProfJobSet.ReportedTime;
            MainSet.AreaNo = ProfJobSet.AreaNo;
            MainSet.StaffNo = ProfJobSet.StaffNo;
            MainSet.StaffName = "";
            MainSet.ProductNo = ProfJobSet.ItemNo;
            MainSet.P1ClientNo = ProfJobSet.ClientNo;
            MainSet.P1ClientName = ProfJobSet.ClientName;
            MainSet.P1ClientAddress = ProfJobSet.ClientAddress;
            MainSet.P1FileNo = ProfJobSet.FileNo;
            //MainSet.P1FileNo = phyMainSet.P1FileNo;
            //MainSet.P1SampleDescription = ProfJobSet.SampleRemark;
            //MainSet.P1DetailOfSample = ProfJobSet.DetailOfSample;
            MainSet.P1SampleDescription = phyMainSet.P1SampleDescription;
            MainSet.P1DetailOfSample = phyMainSet.P1DetailOfSample;
            MainSet.P1ItemNo = ProfJobSet.ItemNo;
            //MainSet.P1OrderNo = ProfJobSet.OrderNo; // 원래 값은 "-"이 입력됨.
            MainSet.P1OrderNo = "-"; // -로 다시 변경해달라고 하여 수정함.
            MainSet.P1Packaging = "Yes, provided";
            MainSet.P1Instruction = "Not provided";
            MainSet.P1Buyer = "-";
            //MainSet.P1Manufacturer = ProfJobSet.Manufacturer;
            MainSet.P1Manufacturer = phyMainSet.P1Manufacturer;
            MainSet.P1CountryOfOrigin = ProfJobSet.CountryOfOrigin;
            MainSet.P1CountryOfDestination = "-";
            //MainSet.P1LabeledAge = "None";
            //MainSet.P1TestAge = "None";
            //MainSet.P1AssessedAge = "All ages";
            MainSet.P1LabeledAge = phyMainSet.P1LabeledAge;
            MainSet.P1TestAge = phyMainSet.P1TestAge;
            MainSet.P1AssessedAge = phyMainSet.P1AssessedAge;
            MainSet.P1ReceivedDate = ProfJobSet.ReceivedTime.ToString("yyyy. MM. dd");

            // Need to modify TB_INTEGMAIN - p1testresult varchar(100) -> varchar(500) 
            MainSet.P1TestResults =
                "The results are extracted from various reports according to the declaration\r\n" +
                "provided by client."; //Please refer to following page(s)";

            //MainSet.P1Comments = ProfJobSet.ReportComments;
            /*
            MainSet.P1Comments =
                "The results shown in this test report refer only to the sample(s)\r\n" +
                "tested unless otherwise stated.\r\n" +
                "this test report is not related to Korea Laboratory\r\n" +
                "Accrediation Scheme.";
            */
            MainSet.P1Comments = "The results shown in this test report refer only to the sample(s) tested unless\r\n" +
                                 "otherwise stated.\r\n" +
                                 "This test report is not related to Korea Laboratory Accreditation Scheme.";
            MainSet.Approval = false;

            if (string.IsNullOrWhiteSpace(MainSet.StaffNo) == true)
            {
                MainSet.Approval = false;
            }
            else
            {
                MainSet.Approval = true;
            }

            if (area == EReportArea.US)
            {
                MainSet.P1TestPeriod = "";
                MainSet.P1TestMethod = "For further details, please refer to following page(s)";
                MainSet.Description1 = "As specified in ASTM F963-17 standard consumer safety specification on toy safety.";
                MainSet.Description2 =
                    //$"The below results are extracted from the test report number\r\n" +
                    $"Picture of Sample as copied from the test report no.\r\n" +
                    $"{fileNoDate}";
                //$"{fileNoDate} where the samples are claimed to be identical.";
                //MainSet.Description3 = $"N/A = Only applicable clauses were shown               **Visual Examination";
                MainSet.Description3 = $"N.B. Only applicable clauses were shown               **Visual Examination";
                MainSet.Description4 = $"Flammability Test(Clauses 4.2)";
                MainSet.Description5 =
                    "*Burning rate has been rounded to the nearest one tenth of an inch per second.\r\n\r\n" +
                    "Requirement: A toy / component is considered a \"flammable solid\" if it ignites and burns with a self-sustaining\r\n" +
                    "             flame at a rate greater than 0.1 in./s along its major axis.";
                MainSet.Description6 =
                    "Heavy Elements(Clause 4.3.5)\r\n\r\n" +            
                    "ASTM F963-17, Clause 4.3.5.1 - Heavy Elements in Paint/Similar Surface Coating Materials";
                MainSet.Description7 =
                    "Method: With reference to CPSC-CH-E1003-09.1 - Standard Operating Procedure for Determing Heavy Metal in Paint and Other Similar Surface Coatings. Analysis was performed by ICP-OES.";
                MainSet.Description8 = "Method: With reference to ASTM F963-17 Clause 8.3. Analysis was performed by ICP-OES.";
                MainSet.Description9 =
                    "Note: - Soluble results shown are of the adjusted analytical result.\r\n" +
                    "      - ND = Not Detected(<MDL)";
                //MainSet.Description10 =
                //    "Heavy Elements(Clause 4.3.5)\r\n\r\n" +
                //    "ASTM F963-17, Clause 4.3.5.1 - Heavy Elements in Paint/Similar Surface Coating Materials";
                MainSet.Description10 =
                    "Heavy Elements(Clause 4.3.5)\r\n\r\n" +
                    "ASTM F963-17, Clause 4.3.5.2 - Heavy Elements in Toys Substrate Materials";

                //MainSet.Description11 =
                //    "Method: With reference to CPSC-CH-E1003-09.1 - Standard Operating Procedure for Determing Heavy Metal in Paint and Other Similar Surface Coatings. Analysis was performed by ICP-OES.";
                MainSet.Description11 =
                    "Method (non-metallic materials): CPSC-CH-E1002-08.3 -Standard Operation Procedure for Determining Total Lead\r\n" +
                    "(Pb) in Non-Metal Children Product. Analysis was performed by ICP-OES.";

                MainSet.Description12 = "Method: With reference to ASTM F963-17 Clause 8.3. Analysis was performed by ICP-OES.";
                MainSet.Description13 =
                    "Note: - Soluble results shown are of the adjusted analytical result.\r\n" +
                    "      - ND = Not Detected(<MDL)";
                MainSet.Description14 = "Stuffing Materials(Clause 4.3.7)";
                MainSet.Description15 =
                    "Method: With reference to ASTM F963-17 Clause 8.29. Visual inspection is performed using a stereo widefield microscope, or equivalent, at 10 x magnification and adequate ilumination.";
                MainSet.Description16 = $"Picture of Sample as copied from the test report no.\r\n{fileNoDate}"; 
                MainSet.Description17 = "";
                MainSet.Description18 = "";
                MainSet.Description19 = "";
            }
            else
            {
                MainSet.P1TestPeriod = "Please refer to following page(s)";
                MainSet.P1TestMethod = "Please refer to following page(s)";
                MainSet.Description1 =
                    "European Standard on Safety of Toys\r\n" +
                    "- Mechanical & Physical Properties\r\n" +
                    "As specified in European standard on safety of toys EN 71 Part 1:2014+A1:2018";
                //MainSet.Description2 = $"This result copied from the test report no. {fileNoDate}";
                MainSet.Description2 = $"This result copied from the test report no. {fileNoDate}";
                MainSet.Description3 = "As specified in European standard on safety of toys EN71 PART 2: 2011+A1:2014";
                MainSet.Description4 = "* Surface Flash of Pile Fabrics(Clause 4.1)";
                MainSet.Description5 = "** Soft-filled toys(animals and doll, etc.) with a piled or textile surface (Clause 4.5)";
                MainSet.Description6 =
                    "NSFO = No surface flash occurred\r\n" +
                    "DNI = Did not ignite\r\n" +
                    "IBE = Ignite But Self-Extinguished\r\n" +
                    "N / A = Not applicable since the requirements of this sub - clause do not apply to toys with a greatest dimension of 150mm or less\r\n" +
                    "SE = Self - Extinguishing\r\n\r\n\r\n" +
                    "N.B. : Only applicable clauses were shown.";
                MainSet.Description7 = "Labeling requirement (Washing/Cleaning Label, CE mark, importer / manufacturer mark (name, address), product identification) according to the Directive 2009/48/EC - Safety of toys";
                MainSet.Description8 = phyMainSet.P5Description2;
                /*
                MainSet.Description8 =
                    "1. According to Directive 2009/48/EC, a toy intended for use by children under 36 months must be designed and\r\n" +
                    "   manufactured in such a way that it can be cleaned. A textile toy must, to this end, be washable, except if it\r\n" +
                    "   contains a mechanism that may be damaged if soak washed. The manufacturer should, if applicable, provide\r\n" +
                    "   instructions on how the toy has to be cleaned.\r\n\r\n" +
                    "2. CE marking should be visible from outside the packaging and its height must be at least 5 mm.\r\n\r\n" +
                    "3. Manufacturer's and Importer's name, registered trade name or registered trade mark and the address at which\r\n" +
                    "   the manufacturer can be contacted must be indicated on the toy or, where that is not possible, on its packaging\r\n" +
                    "   or in a document accompanying the toy.\r\n\r\n" +
                    "4. Manufacturers must ensure that their toys bear a type, batch, serial or model number or other element allowing\r\n" +
                    "   their identification, or where the size or nature of the toy does not allow it, that the required information is\r\n" +
                    "   provided on the packaging or in a document accompanying the toy.";
                */
                MainSet.Description9 = "Method : With reference to EN71-3:2019. Analysis of general elements was performed by ICP-OES and Chromium(III) was obtained by calcuration, chromium(VI) was analyzed by IC-UV/VIS.";
                MainSet.Description10 =
                    "Note. 1. mg/kg = milligram per kilogram\r\n" +
                    "      2. ND = Not Detected(<MDL)\r\n" +
                    "      3. 1% = 10000 mg/kg = 10000 ppm\r\n" +
                    "      4.Soluble Chromium(III) = Soluble Total Chromium - Soluble Chromium(IV)\r\n" +
                    //"      5. ^ = The test result of soluble organic tin was derived from soluble tin screening and then confirmation test\r\n" +
                    //"         for soluble organic tin on component exceeding the screening limit of 4.9 mg/kg soluble Sn";
                    //"      5. ^ = Confirmation test of soluble organic tin is not reguired in case of\r\n" +
                    //"      soluble tin, after conversion, does not exceed the soluble organic tin\r\n" +
                    //"      reguirement as specitied in EN71-3 : 2019";
                    "      5. ^ = Confirmation test of soluble organic tin is not required in case of soluble tin, after conversion, does not\r\n" +
                    "        exceed the soluble organic tin requirement as specified in EN71-3: 2019.";
                MainSet.Description11 = $"Picture of Sample as copied from the test report no.\r\n{fileNoDate}";                
                MainSet.Description12 = phyMainSet.P1FileNo;
                MainSet.Description13 = "";
                MainSet.Description14 = "";
                MainSet.Description15 = "";
                MainSet.Description16 = "";
                MainSet.Description17 = "";
                MainSet.Description18 = "";
                MainSet.Description19 = "";
            }
            MainSet.Insert(trans);
        }

        private void InsertImage(SqlTransaction trans)
        {
            phyImageSet.RecNo = phyMainSet.RecNo;
            phyImageSet.Select(trans);
            phyImageSet.Fetch();

            ImageSet.RecNo = MainSet.RecNo;
            ImageSet.Signature = null;
            ImageSet.Picture = phyImageSet.Picture;
            ImageSet.Insert(trans);
        }

        private void InsertT1(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US)
            {
                T1Set.MainNo = MainSet.RecNo;
                T1Set.No = 0;
                T1Set.Line = false;
                T1Set.Requested = "US Public Law 110-314(Comsumer Plroduct Safety Improvement Act of 2008, CPSIA):";
                T1Set.Conclusion = "-";
                T1Set.Insert(trans);
                                
                T1Set.No = 1;
                T1Set.Line = false;
                T1Set.Requested = "- ASTM F963-17: Standard Consumer Safety Specification on Toy Safety";
                //T1Set.Requested = "- ASTM F963-17: Standard Consumer Safety Specification on Toy Safety\r\n  (Excluding clause 4.3.5 Heavy Element)";
                T1Set.Conclusion = "PASS";
                T1Set.Insert(trans);
                
                T1Set.No = 1;
                T1Set.Line = false;
                T1Set.Requested = "Flammability of toys(16 C.F.R. 1500.44)";
                T1Set.Conclusion = "PASS";
                T1Set.Insert(trans);

                T1Set.No = 2;
                T1Set.Line = false;
                T1Set.Requested = "Small part(16 C.F.R. 1501)";
                T1Set.Conclusion = "PASS";
                T1Set.Insert(trans);

                T1Set.No = 3;
                T1Set.Line = false;
                T1Set.Requested = "Sharp points and edges(16 C.F.R. 1500.48 and 49)";
                T1Set.Conclusion = "PASS";
                T1Set.Insert(trans);
            }
            else
            {
                T1Set.MainNo = MainSet.RecNo;
                T1Set.No = 0;
                T1Set.Line = false;
                T1Set.Requested = "EN 71 Part 1:2014+A1:2018 - Mechanical and Physical Properties";
                T1Set.Conclusion = "PASS";
                T1Set.Insert(trans);

                T1Set.No = 1;
                T1Set.Line = false;
                T1Set.Requested = "EN 71 Part 2:2011+A1:2014 - Flammability of Toys";
                T1Set.Conclusion = "PASS";
                T1Set.Insert(trans);

                T1Set.No = 2;
                T1Set.Line = false;
                T1Set.Requested = "Labeling requirement (Washing/Cleaning Label, CE mark, importer / manufacturer mark (name, address), product identification) according to the Directive 2009/48/EC-Safety of toys";
                T1Set.Conclusion = "See note 1*";
                T1Set.Insert(trans);

                T1Set.No = 3;
                T1Set.Line = false;
                //T1Set.Requested = "Directive 2009/48/EC and its amendment Council Directive (EU) 2017/738, Commision Directive (EU) 2018/725-EN71-3:2019 - Migration of certain elements(By first action method testing only)";
                T1Set.Requested = "Directive 2009/48/EC and its amendment Council Directive (EU) 2017/738, Commision Directive (EU) 2018/725-EN71-3:2019 - Migration of certain elements";
                T1Set.Conclusion = "PASS";
                T1Set.Insert(trans);
            }
        }

        private void InsertT2(EReportArea area, SqlTransaction trans)
        {
            P3Set.MainNo = ProductSet.PhyJobNo;

            if (area == EReportArea.US)
            {
                T2Set.MainNo = MainSet.RecNo;

                for (int i = 0; i < 50; i++) 
                {
                    P3Set.No = i;
                    P3Set.SelectPhymainNo_No(trans);
                    P3Set.Fetch();

                    if (P3Set.RecNo != 0) 
                    {
                        T2Set.No = P3Set.No;
                        T2Set.Line = P3Set.Line;
                        T2Set.Clause = P3Set.Clause;
                        T2Set.Description = P3Set.Description;
                        T2Set.Result = P3Set.Result;
                        T2Set.Insert(trans);
                    }                    
                }

                //P3Set.No = 0;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.MainNo = MainSet.RecNo;
                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 0;
                //T2Set.Line = false;
                //T2Set.Clause = "4";
                //T2Set.Description = "Safety Requirements";
                //T2Set.Result = "-";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 1;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 1;
                //T2Set.Line = false;
                //T2Set.Clause = " 4.1";
                //T2Set.Description = "Material Quality**";
                //T2Set.Result = "Pass";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 2;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 2;
                //T2Set.Line = false;
                //T2Set.Clause = " 4.2";
                //T2Set.Description = "Flammability Test(16 C.F.R. 1500.44)";
                //T2Set.Result = "Pass(See Note 1)";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 3;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 3;
                //T2Set.Line = false;
                //T2Set.Clause = " 4.3";
                //T2Set.Description = "Toxicology";
                //T2Set.Result = "-";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 4;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 4;
                //T2Set.Line = false;
                //T2Set.Clause = " 4.3.5";
                //T2Set.Description = "Heavy Elements";
                //T2Set.Result = "";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 5;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 5;
                //T2Set.Line = false;
                //T2Set.Clause = "";
                //T2Set.Description = "4.3.5.1 Hravy Elements in Paint/Similar Coating Materials";
                //T2Set.Result = "";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 6;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 6;
                //T2Set.Line = false;
                //T2Set.Clause = "";
                //T2Set.Description = "4.3.5.2 Heavy Metal in Substrate Materials";
                //T2Set.Result = "";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 7;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 7;
                //T2Set.Line = false;
                //T2Set.Clause = " 4.3.7";
                //T2Set.Description = "Styffing Materials";
                //T2Set.Result = "Pass(See Note 2)";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 8;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 8;
                //T2Set.Line = false;
                //T2Set.Clause = " 4.6";
                //T2Set.Description = "Small Objects";
                //T2Set.Result = "-";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 9;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 9;
                //T2Set.Line = false;
                //T2Set.Clause = " 4.6.1";
                //T2Set.Description = "Small Objects";
                //T2Set.Result = "Pass";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 10;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 10;
                //T2Set.Line = false;
                //T2Set.Clause = " 4.7";
                //T2Set.Description = "Accessible Edges(16 C.F.R. 1500.49)";
                //T2Set.Result = "Pass";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 11;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 11;
                //T2Set.Line = false;
                //T2Set.Clause = " 4.9";
                //T2Set.Description = "Accessible Points(16 C.F.R. 1500.48)";
                //T2Set.Result = "Pass";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 12;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 12;
                //T2Set.Line = false;
                //T2Set.Clause = " 4.14";
                //T2Set.Description = "Cords, Straps and Elastic";
                //T2Set.Result = "Pass";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 13;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 13;
                //T2Set.Line = true;
                //T2Set.Clause = " 4.27";
                //T2Set.Description = "Stuffed and Beanbag-Type Toys";
                //T2Set.Result = "Pass";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 14;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 14;
                //T2Set.Line = false;
                //T2Set.Clause = "5";
                //T2Set.Description = "Safety Labeling Requirements";
                //T2Set.Result = "-";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 15;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 15;
                //T2Set.Line = true;
                //T2Set.Clause = " 4.2";
                //T2Set.Description = "Age Grading Labeling";
                //T2Set.Result = "Pass";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 16;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 16;
                //T2Set.Line = false;
                //T2Set.Clause = "7";
                //T2Set.Description = "Producer's Markings";
                //T2Set.Result = "-";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 17;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 17;
                //T2Set.Line = true;
                //T2Set.Clause = " 7.1";
                //T2Set.Description = "Producer's Markings";
                //T2Set.Result = "Present";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 18;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 18;
                //T2Set.Line = false;
                //T2Set.Clause = "8";
                //T2Set.Description = "Test Methods";
                //T2Set.Result = "-";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 19;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 19;
                //T2Set.Line = false;
                //T2Set.Clause = " 8.5";
                //T2Set.Description = "Normal Use Testing";
                //T2Set.Result = "Pass";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 20;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 20;
                //T2Set.Line = false;
                //T2Set.Clause = " 8.7";
                //T2Set.Description = "Impact Test";
                //T2Set.Result = "Pass";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 21;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 21;
                //T2Set.Line = false;
                //T2Set.Clause = " 8.8";
                //T2Set.Description = "Torque Test";
                //T2Set.Result = "Pass";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 22;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 22;
                //T2Set.Line = false;
                //T2Set.Clause = " 8.9";
                //T2Set.Description = "Tension Test";
                //T2Set.Result = "Pass";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 23;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 23;
                //T2Set.Line = false;
                //T2Set.Clause = " 8.23";
                //T2Set.Description = "Test for Loops and Cords";
                //T2Set.Result = "Pass";
                //T2Set.Insert(trans);
                //*/

                //P3Set.No = 24;
                //P3Set.SelectPhymainNo_No(trans);
                //P3Set.Fetch();

                //T2Set.No = P3Set.No;
                //T2Set.Line = P3Set.Line;
                //T2Set.Clause = P3Set.Clause;
                //T2Set.Description = P3Set.Description;
                //T2Set.Result = P3Set.Result;
                //T2Set.Insert(trans);

                ///*
                //T2Set.No = 24;
                //T2Set.Line = true;
                //T2Set.Clause = " 8.29";
                //T2Set.Description = "Stuffing Materials Evaluation";
                //T2Set.Result = "Pass";
                //T2Set.Insert(trans);
                //*/
            }
            else // EN
            {
                P3Set.No = 0;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.MainNo = MainSet.RecNo;
                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.MainNo = MainSet.RecNo;
                T2Set.No = 0;
                T2Set.Line = false;
                T2Set.Clause = "4";
                T2Set.Description = "General requirements";
                T2Set.Result = "-";
                T2Set.Insert(trans);
                */

                P3Set.No = 1;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.No = 1;
                T2Set.Line = false;
                T2Set.Clause = " 4.1";
                T2Set.Description = "Material cleanliness";
                T2Set.Result = "Pass";
                T2Set.Insert(trans);
                */

                P3Set.No = 2;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.No = 2;
                T2Set.Line = false;
                T2Set.Clause = " 4.7";
                T2Set.Description = "Edges";
                T2Set.Result = "Pass";
                T2Set.Insert(trans);
                */

                P3Set.No = 3;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.No = 3;
                T2Set.Line = true;
                T2Set.Clause = " 4.8";
                T2Set.Description = "Points and metallic wires";
                T2Set.Result = "Pass";
                T2Set.Insert(trans);
                */

                P3Set.No = 4;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.No = 4;
                T2Set.Line = false;
                T2Set.Clause = "5";
                T2Set.Description = "Toys intended for children under 36 months";
                T2Set.Result = "-";
                T2Set.Insert(trans);
                */

                P3Set.No = 5;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.No = 5;
                T2Set.Line = false;
                T2Set.Clause = " 5.1";
                T2Set.Description = "General requirements";
                T2Set.Result = "Pass";
                T2Set.Insert(trans);
                */

                P3Set.No = 6;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.No = 6;
                T2Set.Line = false;
                T2Set.Clause = "";
                T2Set.Description = "5.1a Small part requirements on toys & removable components";
                T2Set.Result = "Pass";
                T2Set.Insert(trans);
                */

                P3Set.No = 7;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.No = 7;
                T2Set.Line = false;
                T2Set.Clause = "";
                T2Set.Description = "     (Test method 8.2)";
                T2Set.Result = "-";
                T2Set.Insert(trans);
                */

                P3Set.No = 8;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.No = 8;
                T2Set.Line = false;
                T2Set.Clause = "";
                T2Set.Description = "5.1b Torque test(Test method 8.3)";
                T2Set.Result = "Pass";
                T2Set.Insert(trans);
                */

                P3Set.No = 9;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.No = 9;
                T2Set.Line = false;
                T2Set.Clause = "";
                T2Set.Description = "     Tension test(Test method 8.4)";
                T2Set.Result = "Pass";
                T2Set.Insert(trans);
                */

                P3Set.No = 10;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.No = 10;
                T2Set.Line = false;
                T2Set.Clause = "";
                T2Set.Description = "     Drop test(Test method 8.5)";
                T2Set.Result = "Pass";
                T2Set.Insert(trans);
                */

                P3Set.No = 11;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.No = 11;
                T2Set.Line = false;
                T2Set.Clause = "";
                T2Set.Description = "     Impact test(Test method 8.7)";
                T2Set.Result = "Pass";
                T2Set.Insert(trans);
                */

                P3Set.No = 12;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.No = 12;
                T2Set.Line = false;
                T2Set.Clause = "";
                T2Set.Description = "     Sharp edge(Test method 8.11)";
                T2Set.Result = "Pass";
                T2Set.Insert(trans);
                */

                P3Set.No = 13;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.No = 13;
                T2Set.Line = false;
                T2Set.Clause = "";
                T2Set.Description = "     Sharp point(Test method 8.12)";
                T2Set.Result = "Pass";
                T2Set.Insert(trans);
                */

                P3Set.No = 14;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.No = 14;
                T2Set.Line = false;
                T2Set.Clause = " 5.2";
                T2Set.Description = "Soft-filled toys and soft-filled parts of a toy";
                T2Set.Result = "Pass";
                T2Set.Insert(trans);
                */

                P3Set.No = 15;
                P3Set.SelectPhymainNo_No(trans);
                P3Set.Fetch();

                T2Set.No = P3Set.No;
                T2Set.Line = P3Set.Line;
                T2Set.Clause = P3Set.Clause;
                T2Set.Description = P3Set.Description;
                T2Set.Result = P3Set.Result;
                T2Set.Insert(trans);

                /*
                T2Set.No = 15;
                T2Set.Line = false;
                T2Set.Clause = " 5.4";
                T2Set.Description = "Cords, chains and electrical cables in toys";
                T2Set.Result = "Pass";
                T2Set.Insert(trans);
                */
            }
        }

        private void InsertT3(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US) return;

            T3Set.MainNo = MainSet.RecNo;
            T3Set.No = 0;
            T3Set.Line = false;
            T3Set.Clause = "4.1";
            T3Set.Description = "General requirements";
            T3Set.Result = "Pass(See note *)";
            T3Set.Insert(trans);

            T3Set.No = 1;
            T3Set.Line = false;
            T3Set.Clause = "4.5";
            T3Set.Description = "Soft-filled toys(animal and doll, etc.) with apiled or textile surface";
            T3Set.Result = "Pass(See note **)";
            T3Set.Insert(trans);
        }

        private void InsertT4(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US) return;

            P41Set.MainNo = ProductSet.PhyJobNo;
            P41Set.Select(trans);
            P41Set.Fetch();

            T4Set.MainNo = MainSet.RecNo;
            T4Set.No = P41Set.No;
            T4Set.Line = P41Set.Line;
            T4Set.Sample = P41Set.Sample;
            T4Set.Result = P41Set.BurningRate;  // grid는 result이지만 db column은 burningrate.
            /*
            T4Set.No = 0;
            T4Set.Line = false;
            T4Set.Sample = P41Set.Sample;
            T4Set.Result = "NSFO";            
            */
            T4Set.Insert(trans);
        }

        private void InsertT5(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US)
            {
                P41Set.MainNo = ProductSet.PhyJobNo;
                P41Set.Select(trans);
                P41Set.Fetch();

                T5Set.MainNo = MainSet.RecNo;
                T5Set.No = P41Set.No;
                T5Set.Line = P41Set.Line;
                T5Set.Sample = P41Set.Sample;
                T5Set.BurningRate = P41Set.BurningRate;
                T5Set.Insert(trans);
            }
            else
            {
                P42Set.MainNo = ProductSet.PhyJobNo;
                P42Set.Select(trans);
                P42Set.Fetch();

                T5Set.MainNo = MainSet.RecNo;
                T5Set.No = P42Set.No;
                T5Set.Line = P42Set.Line;
                T5Set.Sample = P42Set.Sample;
                T5Set.BurningRate = P42Set.BurningRate;
                /*
                T5Set.No = 0;
                T5Set.Line = false;
                T5Set.Sample = " toy";
                T5Set.BurningRate = "4.2";
                */
                T5Set.Insert(trans);
            }
        }

        private void InsertT6(EReportArea area, SqlTransaction trans)
        {
            P5Set.MainNo = ProductSet.PhyJobNo;

            if (area == EReportArea.US)
            {
                T6Set.MainNo = MainSet.RecNo;
                T6Set.No = 0;
                T6Set.Line = false;
                /*
                T6Set.TestItem =
                    "   1. Objectionable matter originating from\r\n" +
                    "      Insect, bird and rodent or other animal\r\n" +
                    "      infestation";
                T6Set.Result = "Absent";
                T6Set.Requirement = "Absent";
                */

                T6Set.MainNo = MainSet.RecNo;
                P5Set.No = 0;
                P5Set.SelectPhymainNo_No(trans);
                P5Set.Fetch();

                T6Set.No = P5Set.No;
                T6Set.Line = P5Set.Line;
                T6Set.TestItem = P5Set.TestItem;
                T6Set.Result = P5Set.Result;
                T6Set.Requirement = P5Set.Requirement;
                T6Set.Insert(trans);

                P5Set.No = 1;
                P5Set.SelectPhymainNo_No(trans);
                P5Set.Fetch();

                T6Set.No = P5Set.No;
                T6Set.Line = P5Set.Line;
                T6Set.TestItem = P5Set.TestItem;
                T6Set.Result = P5Set.Result;
                T6Set.Requirement = P5Set.Requirement;
                T6Set.Insert(trans);

                /*
                T6Set.No = 1;
                T6Set.Line = false;
                T6Set.TestItem = "Comment";
                T6Set.Result = "PASS";
                T6Set.Requirement = "-";
                T6Set.Insert(trans);
                */
            }
            else
            {
                P5Set.No = 0;
                P5Set.SelectPhymainNo_No(trans);
                P5Set.Fetch();

                T6Set.MainNo = MainSet.RecNo;
                T6Set.MainNo = MainSet.RecNo;
                T6Set.No = P5Set.No;
                T6Set.Line = P5Set.Line;
                T6Set.TestItem = P5Set.TestItem;
                T6Set.Result = P5Set.Result;
                T6Set.Requirement = P5Set.Requirement;
                T6Set.Insert(trans);

                /*
                T6Set.No = 0;
                T6Set.Line = true;
                T6Set.TestItem = "Washing/Cleaning instruction";
                T6Set.Result = "Present";
                T6Set.Requirement = "Affixed label and Hangtag";
                T6Set.Insert(trans);
                */

                P5Set.No = 1;
                P5Set.SelectPhymainNo_No(trans);
                P5Set.Fetch();

                T6Set.No = P5Set.No;
                T6Set.Line = P5Set.Line;
                T6Set.TestItem = P5Set.TestItem;
                T6Set.Result = P5Set.Result;
                T6Set.Requirement = P5Set.Requirement;
                T6Set.Insert(trans);

                /*
                T6Set.No = 1;
                T6Set.Line = true;
                T6Set.TestItem = "CE mark";
                T6Set.Result = "Present";
                T6Set.Requirement = "Affixed label and Hangtag";
                T6Set.Insert(trans);
                */

                P5Set.No = 2;
                P5Set.SelectPhymainNo_No(trans);
                P5Set.Fetch();

                T6Set.No = P5Set.No;
                T6Set.Line = P5Set.Line;
                T6Set.TestItem = P5Set.TestItem;
                T6Set.Result = P5Set.Result;
                T6Set.Requirement = P5Set.Requirement;
                T6Set.Insert(trans);

                /*
                T6Set.No = 2;
                T6Set.Line = true;
                T6Set.TestItem = "Importer's Name & Address";
                T6Set.Result = "Present";
                T6Set.Requirement = "Affixed label and Hangtag";
                T6Set.Insert(trans);
                */

                P5Set.No = 3;
                P5Set.SelectPhymainNo_No(trans);
                P5Set.Fetch();

                T6Set.No = P5Set.No;
                T6Set.Line = P5Set.Line;
                T6Set.TestItem = P5Set.TestItem;
                T6Set.Result = P5Set.Result;
                T6Set.Requirement = P5Set.Requirement;
                T6Set.Insert(trans);

                /*
                T6Set.No = 3;
                T6Set.Line = true;
                T6Set.TestItem = "Manufacturer's Name & Address";
                T6Set.Result = "Present";
                T6Set.Requirement = "Affixed label and Hangtag";
                T6Set.Insert(trans);
                */

                P5Set.No = 4;
                P5Set.SelectPhymainNo_No(trans);
                P5Set.Fetch();

                T6Set.No = P5Set.No;
                T6Set.Line = P5Set.Line;
                T6Set.TestItem = P5Set.TestItem;
                T6Set.Result = P5Set.Result;
                T6Set.Requirement = P5Set.Requirement;
                T6Set.Insert(trans);

                /*
                T6Set.No = 4;
                T6Set.Line = true;
                T6Set.TestItem = "Product ID";
                T6Set.Result = "Present";
                T6Set.Requirement = "Affixed label and Hangtag";
                T6Set.Insert(trans);
                */
            }
        }

        private void InsertT7(EReportArea area, SqlTransaction trans)
        {
            int iSaveLoopIntegrationCnt = 0, iSaveLoopResultCnt = 0, iSaveLoopHyphenCnt = 0;
            int iCountHypen = 0;
            string sP1FileNo = "";
            string sSampleDescription = "";

            bSubstrateMetalLeadCheck = false;
            bSubstratePlasticLeadCheck = false;

            surfaceLeadIndex = 0;
            surfaceResultIndex = 0;

            substrateLeadIndex = 0;
            substrateResultIndex = 0;

            partSet.ProductNo = ProductSet.RecNo;
            partSet.Select(trans);

            for (int i=0; i < partSet.RowCount; i++)
            {
                var vDictionaryReportInsertNo = new Dictionary<string, int>();
                partSet.Fetch(i);

                if (string.IsNullOrWhiteSpace(partSet.JobNo) == false)
                {
                    cheMainSet.Select(partSet.JobNo, trans);
                    cheMainSet.Fetch();
                    if (cheMainSet.Empty == false)
                    {
                        iSaveLoopIntegrationCnt = iSaveLoopIntegrationCnt + 1;

                        for (int j = 0; j < cheMainSet.RowCount; j++)
                        {
                            cheMainSet.Fetch(j);

                            ProfJobSet.JobNo = cheMainSet.RecNo;
                            ProfJobSet.Select_TopOne_Sampleident_Aurora(trans);
                            ProfJobSet.Fetch(0, 0, "Select_TopOne");

                            //iSaveLoopIntegrationCnt = iSaveLoopIntegrationCnt + j;
                            if (j > 0)
                            {
                                iSaveLoopIntegrationCnt = iSaveLoopIntegrationCnt + 1;
                            }

                            // Report No.에 Hypen 개수를 Count
                            iCountHypen = cheMainSet.P1FileNo.Split('-').Length - 1;

                            // Hypen의 개수가 1개 초과면 맨 뒤의 2개의 문자열 제거
                            if (iCountHypen > 1)
                            {
                                sP1FileNo = cheMainSet.P1FileNo.Remove(cheMainSet.P1FileNo.Length - 2);
                            }
                            else
                            {
                                sP1FileNo = cheMainSet.P1FileNo;
                            }

                            T7Set.MainNo = MainSet.RecNo;
                            //T7Set.JobNo = partSet.JobNo;
                            T7Set.JobNo = cheMainSet.RecNo;
                            //T7Set.No = i + 1;
                            T7Set.No = iSaveLoopIntegrationCnt;
                            //T7Set.Description = partSet.MaterialName;

                            // SampleDescription이 빈값인 경우가 있음. 아래의 Case 참고.
                            // sampleident      sam_description   pro_job         orderno
                            // AYN22-027324.001                   AYN22-027324    B1066-72
                            if (!ProfJobSet.SampleDescription.Equals(""))
                            {
                                sSampleDescription = ProfJobSet.SampleDescription[0].ToString().ToUpper() + ProfJobSet.SampleDescription.Substring(1).ToLower();
                                T7Set.Description = sSampleDescription;
                            }
                            else
                            {
                                T7Set.Description = "";
                            }
                            //T7Set.Description = ProfJobSet.SampleDescription;
                            T7Set.Name = partSet.Name;
                            T7Set.MaterialNo = partSet.MaterialNo;
                            //T7Set.ReportNo = cheMainSet.P1FileNo;
                            T7Set.ReportNo = sP1FileNo;
                            //T7Set.IssuedDate = cheMainSet.RegTime.ToString("yyyy.MM.dd");
                            T7Set.IssuedDate = cheMainSet.RequiredTime.ToString("yyyy.MM.dd");
                            T7Set.Insert(trans);
                            vDictionaryReportInsertNo.Add(cheMainSet.RecNo, iSaveLoopIntegrationCnt);
                        }

                        iSaveLoopResultCnt = iSaveLoopResultCnt + 1;

                        for (int k = 0; k < cheMainSet.RowCount; k++)
                        {
                            cheMainSet.Fetch(k);
                            iSaveLoopResultCnt = iSaveLoopResultCnt + k;
                            switch (area)
                            {
                                case EReportArea.US:
                                    InsertResultAstm(cheMainSet.RecNo, cheMainSet.LeadType, vDictionaryReportInsertNo[cheMainSet.RecNo], trans);
                                    break;

                                case EReportArea.EU:
                                    if (i == 0) InsertLimitEn(cheMainSet.RecNo, trans);
                                    InsertResultEn(iSaveLoopResultCnt, cheMainSet.RecNo, trans);
                                    break;
                            }
                        }
                    }
                }
            }
            switch (area)
            {
                /*
                case EReportArea.US:
                    InsertResultAstm(cheMainSet.RecNo, cheMainSet.LeadType, trans);
                    break;
                */
                case EReportArea.EU:
                    if (iSaveLoopResultCnt > 0 && iSaveLoopResultCnt < 5)
                    {
                        iSaveLoopHyphenCnt = 5 - iSaveLoopResultCnt;
                        for (int k = 0; k < iSaveLoopHyphenCnt; k++)
                        {
                            iSaveLoopResultCnt = iSaveLoopResultCnt + 1;
                            InsertResult_HYPHEN_En(iSaveLoopResultCnt, cheMainSet.RecNo, trans);
                        }
                    }
                    else if (iSaveLoopResultCnt > 0 && iSaveLoopResultCnt < 10)
                    {
                        iSaveLoopHyphenCnt = 10 - iSaveLoopResultCnt;
                        for (int k = 0; k < iSaveLoopHyphenCnt; k++)
                        {
                            iSaveLoopResultCnt = iSaveLoopResultCnt + 1;
                            InsertResult_HYPHEN_En(iSaveLoopResultCnt, cheMainSet.RecNo, trans);
                        }
                    }
                    else if (iSaveLoopResultCnt > 0 && iSaveLoopResultCnt < 15)
                    {
                        iSaveLoopHyphenCnt = 15 - iSaveLoopResultCnt;
                        for (int k = 0; k < iSaveLoopHyphenCnt; k++)
                        {
                            iSaveLoopResultCnt = iSaveLoopResultCnt + 1;
                            InsertResult_HYPHEN_En(iSaveLoopResultCnt, cheMainSet.RecNo, trans);
                        }
                    } 
                    else if (iSaveLoopResultCnt > 0 && iSaveLoopResultCnt < 20)
                    {
                        iSaveLoopHyphenCnt = 20 - iSaveLoopResultCnt;
                        for (int k = 0; k < iSaveLoopHyphenCnt; k++)
                        {
                            iSaveLoopResultCnt = iSaveLoopResultCnt + 1;
                            InsertResult_HYPHEN_En(iSaveLoopResultCnt, cheMainSet.RecNo, trans);
                        }
                    }
                    else if (iSaveLoopResultCnt > 0 && iSaveLoopResultCnt < 25)
                    {
                        iSaveLoopHyphenCnt = 25 - iSaveLoopResultCnt;
                        for (int k = 0; k < iSaveLoopHyphenCnt; k++)
                        {
                            iSaveLoopResultCnt = iSaveLoopResultCnt + 1;
                            InsertResult_HYPHEN_En(iSaveLoopResultCnt, cheMainSet.RecNo, trans);
                        }
                    }
                    else if (iSaveLoopResultCnt > 0 && iSaveLoopResultCnt < 30)
                    {
                        iSaveLoopHyphenCnt = 30 - iSaveLoopResultCnt;
                        for (int k = 0; k < iSaveLoopHyphenCnt; k++)
                        {
                            iSaveLoopResultCnt = iSaveLoopResultCnt + 1;
                            InsertResult_HYPHEN_En(iSaveLoopResultCnt, cheMainSet.RecNo, trans);
                        }
                    }
                    break;
            }
        }

        private void InsertResultAstm(string recNo, ELeadType type, int iReportInsertNo, SqlTransaction trans)
        {
            cheP2Set.MainNo = recNo;
            //cheP2Set.Select(trans);
            cheP2Set.Select_Orderby(trans);

            if (cheP2Set.Empty == false)
            {
                switch (type)
                {
                    case ELeadType.None:
                        InsertSubstrateResultAstm(trans, -1, iReportInsertNo);
                        break;

                    case ELeadType.Substrate:
                        InsertSubstrateResultAstm(trans, 0, iReportInsertNo);
                        break;

                    case ELeadType.Surface:
                        InsertSurfaceResultAstm(trans, 1, iReportInsertNo);
                        break;
                }
            }

            cheP2ExtendSet.RecNo = recNo;
            //cheP2ExtendSet.Select(trans);
            cheP2ExtendSet.Select_MainNo(trans);

            if (cheP2ExtendSet.Empty == false)
            {
                switch (type)
                {
                    case ELeadType.None:
                        InsertSubstrateLeadResultAstm(trans, -1, iReportInsertNo);
                        break;
                    case ELeadType.Substrate:
                        InsertSubstrateLeadResultAstm(trans, 0, iReportInsertNo);
                        break;
                    case ELeadType.Surface:
                        InsertSurfaceLeadResultAstm(trans, 1, iReportInsertNo);
                        break;
                }
            }
        }

        private void InsertSurfaceResultAstm(SqlTransaction trans, int iLeadType, int iReportInsertNo)
        {
            if (surfaceResultIndex == 0)
            {
                for (int i = 0; i < cheP2Set.RowCount; i++)
                {
                    cheP2Set.Fetch(i);
                    if (!cheP2Set.Name.Equals("Mass of trace amount (mg)"))
                    {
                        if (cheP2Set.Name.Contains("Pb"))
                        {
                            SurfaceLimitAstmSet.Name = "Pb";
                            SurfaceLimitAstmSet.HiValue = "90";
                            SurfaceLimitAstmSet.ReportValue = "5";
                        }
                        else if (cheP2Set.Name.Contains("(Sb)"))
                        {
                            SurfaceLimitAstmSet.Name = "Sb";
                            SurfaceLimitAstmSet.HiValue = "60";
                            SurfaceLimitAstmSet.ReportValue = "5";
                        }
                        else if (cheP2Set.Name.Contains("(As)"))
                        {
                            SurfaceLimitAstmSet.Name = "As";
                            SurfaceLimitAstmSet.HiValue = "25";
                            SurfaceLimitAstmSet.ReportValue = "2.5";
                        }
                        else if (cheP2Set.Name.Contains("(Ba)"))
                        {
                            SurfaceLimitAstmSet.Name = "Ba";
                            SurfaceLimitAstmSet.HiValue = "1000";
                            SurfaceLimitAstmSet.ReportValue = "10";
                        }
                        else if (cheP2Set.Name.Contains("(Cd)"))
                        {
                            SurfaceLimitAstmSet.Name = "Cd";
                            SurfaceLimitAstmSet.HiValue = "75";
                            SurfaceLimitAstmSet.ReportValue = "5";
                        }
                        else if (cheP2Set.Name.Contains("(Cr)"))
                        {
                            SurfaceLimitAstmSet.Name = "Cr";
                            SurfaceLimitAstmSet.HiValue = "60";
                            SurfaceLimitAstmSet.ReportValue = "2.5";
                        }
                        else if (cheP2Set.Name.Contains("(Hg)"))
                        {
                            SurfaceLimitAstmSet.Name = "Hg";
                            SurfaceLimitAstmSet.HiValue = "60";
                            SurfaceLimitAstmSet.ReportValue = "2.5";
                        }
                        else if (cheP2Set.Name.Contains("(Se)"))
                        {
                            SurfaceLimitAstmSet.Name = "Se";
                            SurfaceLimitAstmSet.HiValue = "500";
                            SurfaceLimitAstmSet.ReportValue = "10";
                        }

                        SurfaceLimitAstmSet.MainNo = MainSet.RecNo;
                        SurfaceLimitAstmSet.LeadType = ELeadType.Surface;
                        //SurfaceLimitAstmSet.Name = cheP2Set.Name;
                        SurfaceLimitAstmSet.LoValue = cheP2Set.LoValue;
                        //SurfaceLimitAstmSet.HiValue = cheP2Set.HiValue;
                        //SurfaceLimitAstmSet.ReportValue = cheP2Set.ReportValue;
                        SurfaceLimitAstmSet.Insert(trans, iLeadType);
                    }
                }
            }

            SurfaceResultAstmSet.MainNo = MainSet.RecNo;
            //SurfaceResultAstmSet.No = surfaceResultIndex + 1;
            SurfaceResultAstmSet.No = iReportInsertNo;

            /*
            SurfaceResultAstmSet.MainNo = MainSet.RecNo;
            SurfaceResultAstmSet.No = surfaceResultIndex + 1;
            SurfaceResultAstmSet.Mg = "--";
            */

            for (int i = 0; i < cheP2Set.RowCount; i++)
            {
                cheP2Set.Fetch(i);

                if (cheP2Set.FormatValue.Equals(""))
                {
                    cheP2Set.FormatValue = "--";
                }

                switch (i)
                {
                    case 0:
                        SurfaceResultAstmSet.Mg = cheP2Set.FormatValue;
                        break;

                    case 1:
                        SurfaceResultAstmSet.Pb = cheP2Set.FormatValue;
                        break;

                    case 2:
                        SurfaceResultAstmSet.Sb = cheP2Set.FormatValue;
                        break;

                    case 3:
                        SurfaceResultAstmSet.As = cheP2Set.FormatValue;
                        break;

                    case 4:
                        SurfaceResultAstmSet.Ba = cheP2Set.FormatValue;
                        break;

                    case 5:
                        SurfaceResultAstmSet.Cd = cheP2Set.FormatValue;
                        break;

                    case 6:
                        SurfaceResultAstmSet.Cr = cheP2Set.FormatValue;
                        break;

                    case 7:
                        SurfaceResultAstmSet.Hg = cheP2Set.FormatValue;
                        break;

                    case 8:
                        SurfaceResultAstmSet.Se = cheP2Set.FormatValue;
                        break;

                        /*
                        case 0:
                            SurfaceResultAstmSet.As = cheP2Set.FormatValue;
                            break;

                        case 1:
                            SurfaceResultAstmSet.Ba = cheP2Set.FormatValue;
                            break;

                        case 2:
                            SurfaceResultAstmSet.Cd = cheP2Set.FormatValue;
                            break;

                        case 3:
                            SurfaceResultAstmSet.Cr = cheP2Set.FormatValue;
                            break;

                        case 4:
                            SurfaceResultAstmSet.Hg = cheP2Set.FormatValue;
                            break;

                        case 5:
                            SurfaceResultAstmSet.Pb = cheP2Set.FormatValue;
                            break;

                        case 6:
                            SurfaceResultAstmSet.Sb = cheP2Set.FormatValue;
                            break;

                        case 7:
                            SurfaceResultAstmSet.Se = cheP2Set.FormatValue;
                            break;
                        */
                }
            }

            SurfaceResultAstmSet.Insert(trans, iLeadType);
            surfaceResultIndex++;
        }

        private void InsertSubstrateResultAstm(SqlTransaction trans, int iLeadType, int iReportInsertNo)
        {
            if (substrateResultIndex == 0)
            {
                for (int i = 0; i < cheP2Set.RowCount; i++)
                {
                    cheP2Set.Fetch(i);
                    if (! cheP2Set.Name.Equals("Mass of trace amount (mg)"))
                    {
                        if (cheP2Set.Name.Contains("Pb"))
                        {
                            SubstrateLimitAstmSet.Name = "Pb";
                            SubstrateLimitAstmSet.HiValue = "90";
                            SubstrateLimitAstmSet.ReportValue = "5";                           
                        }
                        else if (cheP2Set.Name.Contains("Sb"))
                        {
                            SubstrateLimitAstmSet.Name = "Sb";
                            SubstrateLimitAstmSet.HiValue = "60";
                            SubstrateLimitAstmSet.ReportValue = "5";
                        }
                        else if (cheP2Set.Name.Contains("As"))
                        {
                            SubstrateLimitAstmSet.Name = "As";
                            SubstrateLimitAstmSet.HiValue = "25";
                            SubstrateLimitAstmSet.ReportValue = "2.5";
                        }
                        else if (cheP2Set.Name.Contains("Ba"))
                        {
                            SubstrateLimitAstmSet.Name = "Ba";
                            SubstrateLimitAstmSet.HiValue = "1000";
                            SubstrateLimitAstmSet.ReportValue = "10";
                        }
                        else if (cheP2Set.Name.Contains("Cd"))
                        {
                            SubstrateLimitAstmSet.Name = "Cd";
                            SubstrateLimitAstmSet.HiValue = "75";
                            SubstrateLimitAstmSet.ReportValue = "5";
                        }
                        else if (cheP2Set.Name.Contains("Cr"))
                        {
                            SubstrateLimitAstmSet.Name = "Cr";
                            SubstrateLimitAstmSet.HiValue = "60";
                            SubstrateLimitAstmSet.ReportValue = "2.5";
                        }
                        else if (cheP2Set.Name.Contains("Hg"))
                        {
                            SubstrateLimitAstmSet.Name = "Hg";
                            SubstrateLimitAstmSet.HiValue = "60";
                            SubstrateLimitAstmSet.ReportValue = "2.5";
                        }
                        else if (cheP2Set.Name.Contains("Se"))
                        {
                            SubstrateLimitAstmSet.Name = "Se";
                            SubstrateLimitAstmSet.HiValue = "500";
                            SubstrateLimitAstmSet.ReportValue = "10";
                        }

                        SubstrateLimitAstmSet.MainNo = MainSet.RecNo;
                        SubstrateLimitAstmSet.LeadType = ELeadType.Substrate;
                        //SubstrateLimitAstmSet.Name = cheP2Set.Name;
                        SubstrateLimitAstmSet.LoValue = cheP2Set.LoValue;
                        //SubstrateLimitAstmSet.HiValue = cheP2Set.HiValue;
                        SubstrateLimitAstmSet.ReportValue = cheP2Set.ReportValue;
                        SubstrateLimitAstmSet.Insert(trans, iLeadType);
                    }
                }
            }

            SubstrateResultAstmSet.MainNo = MainSet.RecNo;
            //SubstrateResultAstmSet.No = substrateResultIndex + 1;
            SubstrateResultAstmSet.No = iReportInsertNo;

            /*
            SubstrateResultAstmSet.MainNo = MainSet.RecNo;
            SubstrateResultAstmSet.No = substrateResultIndex + 1;
            SubstrateResultAstmSet.Mg = "--";
            */

            for (int i = 0; i < cheP2Set.RowCount; i++)
            {
                cheP2Set.Fetch(i);

                if (cheP2Set.FormatValue.Equals(""))
                {
                    cheP2Set.FormatValue = "--";
                }

                switch (i)
                {
                    case 0:
                        SubstrateResultAstmSet.Mg = cheP2Set.FormatValue;
                        break;

                    case 1:
                        SubstrateResultAstmSet.Pb = cheP2Set.FormatValue;
                        break;

                    case 2:
                        SubstrateResultAstmSet.Sb = cheP2Set.FormatValue;
                        break;

                    case 3:
                        SubstrateResultAstmSet.As = cheP2Set.FormatValue;
                        break;

                    case 4:
                        SubstrateResultAstmSet.Ba = cheP2Set.FormatValue;
                        break;

                    case 5:
                        SubstrateResultAstmSet.Cd = cheP2Set.FormatValue;
                        break;

                    case 6:
                        SubstrateResultAstmSet.Cr = cheP2Set.FormatValue;
                        break;

                    case 7:
                        SubstrateResultAstmSet.Hg = cheP2Set.FormatValue;
                        break;

                    case 8:
                        SubstrateResultAstmSet.Se = cheP2Set.FormatValue;
                        break;

                /*
                    case 0:
                        SubstrateResultAstmSet.As = cheP2Set.FormatValue;
                        break;

                    case 1:
                        SubstrateResultAstmSet.Ba = cheP2Set.FormatValue;
                        break;

                    case 2:
                        SubstrateResultAstmSet.Cd = cheP2Set.FormatValue;
                        break;

                    case 3:
                        SubstrateResultAstmSet.Cr = cheP2Set.FormatValue;
                        break;

                    case 4:
                        SubstrateResultAstmSet.Hg = cheP2Set.FormatValue;
                        break;

                    case 5:
                        SubstrateResultAstmSet.Pb = cheP2Set.FormatValue;
                        break;

                    case 6:
                        SubstrateResultAstmSet.Sb = cheP2Set.FormatValue;
                        break;

                    case 7:
                        SubstrateResultAstmSet.Se = cheP2Set.FormatValue;
                        break;
                */
                }
            }

            SubstrateResultAstmSet.Insert(trans, iLeadType);
            substrateResultIndex++;
        }

        private void InsertSurfaceLeadResultAstm(SqlTransaction trans, int iLeadType, int iReportInsertNo)
        {
            if (surfaceLeadIndex == 0)
            {
                cheP2ExtendSet.Fetch();

                if (cheP2ExtendSet.LoValue.Equals(""))
                {
                    cheP2ExtendSet.LoValue = "--";
                }

                if (cheP2ExtendSet.ReportValue.Equals("") || cheP2ExtendSet.ReportValue.Equals("--"))
                {
                    cheP2ExtendSet.ReportValue = "20";
                }

                if (cheP2ExtendSet.HiValue.Equals("") || cheP2ExtendSet.HiValue.Equals("--"))
                {
                    cheP2ExtendSet.HiValue = "90";
                }

                SurfaceLeadLimitAstmSet.MainNo = MainSet.RecNo;
                SurfaceLeadLimitAstmSet.LeadType = ELeadType.Surface;
                SurfaceLeadLimitAstmSet.LoValue = cheP2ExtendSet.LoValue;
                SurfaceLeadLimitAstmSet.HiValue = cheP2ExtendSet.HiValue;
                SurfaceLeadLimitAstmSet.ReportValue = cheP2ExtendSet.ReportValue;
                SurfaceLeadLimitAstmSet.ReportCase = "Coating";
                SurfaceLeadLimitAstmSet.Insert(trans, iLeadType);
            }

            SurfaceLeadResultAstmSet.MainNo = MainSet.RecNo;
            //SurfaceLeadResultAstmSet.No = surfaceResultIndex;
            SurfaceLeadResultAstmSet.No = iReportInsertNo;
            SurfaceLeadResultAstmSet.LeadType = ELeadType.Surface;
            SurfaceLeadResultAstmSet.Pb = cheP2ExtendSet.FormatValue;
            SurfaceLeadResultAstmSet.ReportCase = "Coating";
            SurfaceLeadResultAstmSet.Insert(trans, iLeadType);

            surfaceLeadIndex++;
        }

        private void InsertSubstrateLeadResultAstm(SqlTransaction trans, int iLeadType, int iReportInsertNo)
        {
            cheP2ExtendSet.Fetch();

            //if (substrateLeadIndex == 0)
            //{
                if (cheP2ExtendSet.LoValue.Equals(""))
                {
                    cheP2ExtendSet.LoValue = "--";
                }

                if (cheP2ExtendSet.HiValue.Equals("") || cheP2ExtendSet.HiValue.Equals("--"))
                {
                    cheP2ExtendSet.HiValue = "100";
                }
            //}

            if (cheP2ExtendSet.Sch_Code.Equals("HCEECPSC07"))
            {
                //cheP2ExtendSet.ReportValue = "50";
                SubstrateLeadLimitAstmSet.ReportCase = "Metal";
                SubstrateLeadResultAstmSet.ReportCase = "Metal";

                if (bSubstrateMetalLeadCheck == false) 
                {
                    bSubstrateMetalLeadCheck = true;
                    
                    SubstrateLeadLimitAstmSet.MainNo = MainSet.RecNo;
                    SubstrateLeadLimitAstmSet.LeadType = ELeadType.Substrate;
                    SubstrateLeadLimitAstmSet.LoValue = cheP2ExtendSet.LoValue;
                    SubstrateLeadLimitAstmSet.HiValue = cheP2ExtendSet.HiValue;
                    SubstrateLeadLimitAstmSet.ReportValue = cheP2ExtendSet.ReportValue;

                    SubstrateLeadLimitAstmSet.Insert(trans, iLeadType);
                }
            }
            else if (cheP2ExtendSet.Sch_Code.Equals("HCEECPSC08")) 
            {
                //cheP2ExtendSet.ReportValue = "20";
                SubstrateLeadLimitAstmSet.ReportCase = "Plastic";
                SubstrateLeadResultAstmSet.ReportCase = "Plastic";

                if (bSubstratePlasticLeadCheck == false)
                {
                    bSubstratePlasticLeadCheck = true;

                    SubstrateLeadLimitAstmSet.MainNo = MainSet.RecNo;
                    SubstrateLeadLimitAstmSet.LeadType = ELeadType.Substrate;
                    SubstrateLeadLimitAstmSet.LoValue = cheP2ExtendSet.LoValue;
                    SubstrateLeadLimitAstmSet.HiValue = cheP2ExtendSet.HiValue;
                    SubstrateLeadLimitAstmSet.ReportValue = cheP2ExtendSet.ReportValue;

                    SubstrateLeadLimitAstmSet.Insert(trans, iLeadType);
                }
            }
            else
            {
                //cheP2ExtendSet.ReportValue = "20";
                //SubstrateLeadLimitAstmSet.ReportCase = "None";
                //SubstrateLeadResultAstmSet.ReportCase = "None";
            }

            SubstrateLeadResultAstmSet.MainNo = MainSet.RecNo;
            //SubstrateLeadResultAstmSet.No = substrateResultIndex;
            SubstrateLeadResultAstmSet.No = iReportInsertNo;
            SubstrateLeadResultAstmSet.LeadType = ELeadType.Substrate;
            SubstrateLeadResultAstmSet.Pb = cheP2ExtendSet.FormatValue;
            SubstrateLeadResultAstmSet.Insert(trans, iLeadType);

            substrateLeadIndex++;
        }

        private void InsertLimitEn(string recNo, SqlTransaction trans)
        {
            cheP2Set.MainNo = recNo;
            cheP2Set.Select(trans);

            if (cheP2Set.Empty == false)
            {
                for (int i=0; i<cheP2Set.RowCount; i++)
                {
                    cheP2Set.Fetch(i);

                    LimitEnSet.MainNo = MainSet.RecNo;
                    LimitEnSet.Name = cheP2Set.Name;
                    LimitEnSet.LoValue = cheP2Set.LoValue;
                    LimitEnSet.HiValue = cheP2Set.HiValue;
                    LimitEnSet.ReportValue = cheP2Set.ReportValue;
                    LimitEnSet.Insert(trans);
                }
            }
        }

        private void InsertResultEn(int index, string recNo, SqlTransaction trans)
        {
            cheP2Set.MainNo = recNo;
            cheP2Set.Select(trans);

            if (cheP2Set.Empty == false)
            {
                ResultEnSet.MainNo = MainSet.RecNo;
                //ResultEnSet.No = index + 1;
                ResultEnSet.No = index;
                //ResultEnSet.Mg = "--";

                for (int i = 0; i < cheP2Set.RowCount; i++)
                {
                    cheP2Set.Fetch(i);

                    switch (i)
                    {
                        case 0:
                            ResultEnSet.Mg = cheP2Set.FormatValue;
                            break;

                        case 1:
                            ResultEnSet.Ai = cheP2Set.FormatValue;
                            break;

                        case 2:
                            ResultEnSet.As = cheP2Set.FormatValue;
                            break;

                        case 3:
                            ResultEnSet.B = cheP2Set.FormatValue;
                            break;

                        case 4:
                            ResultEnSet.Ba = cheP2Set.FormatValue;
                            break;

                        case 5:
                            ResultEnSet.Cd = cheP2Set.FormatValue;
                            break;

                        case 6:
                            ResultEnSet.Co = cheP2Set.FormatValue;
                            break;

                        case 7:
                            //ResultEnSet.Cr = cheP2Set.FormatValue;
                            ResultEnSet.Cr3 = cheP2Set.FormatValue;
                            break;

                        case 8:
                            //ResultEnSet.Cr3 = cheP2Set.FormatValue;
                            ResultEnSet.Cr4 = cheP2Set.FormatValue;
                            break;

                        case 9:
                            //ResultEnSet.Cr4 = cheP2Set.FormatValue;
                            ResultEnSet.Cu = cheP2Set.FormatValue;
                            break;

                        case 10:
                            //ResultEnSet.Cu = cheP2Set.FormatValue;
                            ResultEnSet.Hg = cheP2Set.FormatValue;
                            break;

                        case 11:
                            //ResultEnSet.Hg = cheP2Set.FormatValue;
                            ResultEnSet.Mn = cheP2Set.FormatValue;
                            break;

                        case 12:
                            //ResultEnSet.Mn = cheP2Set.FormatValue;
                            ResultEnSet.Ni = cheP2Set.FormatValue;
                            break;

                        case 13:
                            //ResultEnSet.Ni = cheP2Set.FormatValue;
                            ResultEnSet.Pb = cheP2Set.FormatValue;
                            break;

                        case 14:
                            //ResultEnSet.Pb = cheP2Set.FormatValue;
                            ResultEnSet.Sb = cheP2Set.FormatValue;
                            break;

                        case 15:
                            //ResultEnSet.Sb = cheP2Set.FormatValue;
                            ResultEnSet.Se = cheP2Set.FormatValue;
                            break;

                        case 16:
                            //ResultEnSet.Se = cheP2Set.FormatValue;
                            ResultEnSet.Sn = cheP2Set.FormatValue;
                            break;

                        case 17:
                            //ResultEnSet.Sn = cheP2Set.FormatValue;
                            ResultEnSet.Sr = cheP2Set.FormatValue;
                            break;

                        case 18:
                            //ResultEnSet.Sr = cheP2Set.FormatValue;
                            ResultEnSet.Zn = cheP2Set.FormatValue;
                            break;

                        case 19:
                            //ResultEnSet.Zn = cheP2Set.FormatValue;
                            ResultEnSet.Tin = cheP2Set.FormatValue;

                            if (string.IsNullOrWhiteSpace(ResultEnSet.Tin))
                            {
                                ResultEnSet.Tin = "--";
                            }
                            break;

                        case 20:
                            break;
                            /*
                            ResultEnSet.Tin = cheP2Set.FormatValue;

                            if (string.IsNullOrWhiteSpace(ResultEnSet.Tin)) 
                            {
                                ResultEnSet.Tin = "--";
                            }
                            break;
                            */
                    }
                }
                ResultEnSet.Insert(trans);
                ResultEnSet.Insert_HYPHEN(trans);

                /*
                for (int i = 0; i < cheP2Set.RowCount; i++)
                {
                    cheP2Set.Fetch(i);

                    switch (i)
                    {
                        case 0:
                            ResultEnSet.Ai = cheP2Set.FormatValue;
                            break;

                        case 1:
                            ResultEnSet.As = cheP2Set.FormatValue;
                            break;

                        case 2:
                            ResultEnSet.B = cheP2Set.FormatValue;
                            break;

                        case 3:
                            ResultEnSet.Ba = cheP2Set.FormatValue;
                            break;

                        case 4:
                            ResultEnSet.Cd = cheP2Set.FormatValue;
                            break;

                        case 5:
                            ResultEnSet.Co = cheP2Set.FormatValue;
                            break;

                        case 6:
                            ResultEnSet.Cr = cheP2Set.FormatValue;
                            break;

                        case 7:
                            ResultEnSet.Cr3 = cheP2Set.FormatValue;
                            break;

                        case 8:
                            ResultEnSet.Cr4 = cheP2Set.FormatValue;
                            break;

                        case 9:
                            ResultEnSet.Cu = cheP2Set.FormatValue;
                            break;

                        case 10:
                            ResultEnSet.Hg = cheP2Set.FormatValue;
                            break;

                        case 11:
                            ResultEnSet.Mn = cheP2Set.FormatValue;
                            break;

                        case 12:
                            ResultEnSet.Ni = cheP2Set.FormatValue;
                            break;

                        case 13:
                            ResultEnSet.Pb = cheP2Set.FormatValue;
                            break;

                        case 14:
                            ResultEnSet.Sb = cheP2Set.FormatValue;
                            break;

                        case 15:
                            ResultEnSet.Se = cheP2Set.FormatValue;
                            break;

                        case 16:
                            ResultEnSet.Sn = cheP2Set.FormatValue;
                            break;

                        case 17:
                            ResultEnSet.Sr = cheP2Set.FormatValue;
                            break;

                        case 18:
                            ResultEnSet.Zn = cheP2Set.FormatValue;
                            break;

                        case 19:
                            ResultEnSet.Tin = cheP2Set.FormatValue;
                            break;
                    }
                }
                */
                //ResultEnSet.Insert(trans);
            }
        }

        private void InsertResult_HYPHEN_En(int index, string recNo, SqlTransaction trans)
        {
            cheP2Set.MainNo = recNo;
            cheP2Set.Select(trans);

            if (cheP2Set.Empty == false)
            {
                ResultEnSet.MainNo = MainSet.RecNo;                
                ResultEnSet.No = index;

                for (int i = 0; i < 21; i++)
                {
                    //cheP2Set.Fetch(i);

                    switch (i)
                    {
                        case 0:
                            //ResultEnSet.Mg = "--";
                            ResultEnSet.Mg = "";
                            break;

                        case 1:
                            //ResultEnSet.Ai = "--";
                            ResultEnSet.Ai = "";
                            break;

                        case 2:
                            //ResultEnSet.As = "--";
                            ResultEnSet.As = "";
                            break;

                        case 3:
                            //ResultEnSet.B = "--";
                            ResultEnSet.B = "";
                            break;

                        case 4:
                            //ResultEnSet.Ba = "--";
                            ResultEnSet.Ba = "";
                            break;

                        case 5:
                            //ResultEnSet.Cd = "--";
                            ResultEnSet.Cd = "";
                            break;

                        case 6:
                            //ResultEnSet.Co = "--";
                            ResultEnSet.Co = "";
                            break;

                        case 7:
                            //ResultEnSet.Cr = "--";
                            ResultEnSet.Cr = "";
                            break;

                        case 8:
                            //ResultEnSet.Cr3 = "--";
                            ResultEnSet.Cr3 = "";
                            break;

                        case 9:
                            //ResultEnSet.Cr4 = "--";
                            ResultEnSet.Cr4 = "";
                            break;

                        case 10:
                            //ResultEnSet.Cu = "--";
                            ResultEnSet.Cu = "";
                            break;

                        case 11:
                            //ResultEnSet.Hg = "--";
                            ResultEnSet.Hg = "";
                            break;

                        case 12:
                            //ResultEnSet.Mn = "--";
                            ResultEnSet.Mn = "";
                            break;

                        case 13:
                            //ResultEnSet.Ni = "--";
                            ResultEnSet.Ni = "";
                            break;

                        case 14:
                            //ResultEnSet.Pb = "--";
                            ResultEnSet.Pb = "";
                            break;

                        case 15:
                            //ResultEnSet.Sb = "--";
                            ResultEnSet.Sb = "";
                            break;

                        case 16:
                            //ResultEnSet.Se = "--";
                            ResultEnSet.Se = "";
                            break;

                        case 17:
                            //ResultEnSet.Sn = "--";
                            ResultEnSet.Sn = "";
                            break;

                        case 18:
                            //ResultEnSet.Sr = "--";
                            ResultEnSet.Sr = "";
                            break;

                        case 19:
                            //ResultEnSet.Zn = "--";
                            ResultEnSet.Zn = "";
                            break;

                        case 20:
                            //ResultEnSet.Tin = "--";
                            ResultEnSet.Tin = "";
                            break;
                    }
                }
                ResultEnSet.Insert_HYPHEN(trans);
            }
        }

        private void UpdateProductSet(EReportArea area, SqlTransaction trans)
        {
            if (local == true) return;

            ProductSet.AreaNo = area;
            ProductSet.IntegJobNo = ProfJobSet.JobNo;
            ProductSet.Code = ProfJobSet.ItemNo;
            ProductSet.UpdateIntegJobNoSet(trans);
            ProductSet.UpdateValidSet(trans);
        }

        private void SaveMain(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US)
                CtrlUs.SetControlToDataSet();
            else
                CtrlEu.SetControlToDataSet();

            MainSet.Update(trans);
        }

        private void SaveT1(EReportArea area, SqlTransaction trans)
        {
            List<IntegrationT1Row> rows = (area == EReportArea.US) ? CtrlUs.T1Rows : CtrlEu.T1Rows;

            T1Set.MainNo = MainSet.RecNo;
            T1Set.Delete(trans);

            foreach (IntegrationT1Row row in rows)
            {
                T1Set.No = row.No;
                T1Set.Line = row.Line;
                T1Set.Requested = row.Requested;
                T1Set.Conclusion = row.Conclusion;
                T1Set.Insert(trans);
            }
        }

        private void SaveT2(EReportArea area, SqlTransaction trans)
        {
            List<IntegrationT2Row> rows = (area == EReportArea.US) ? CtrlUs.T2Rows : CtrlEu.T2Rows;

            T2Set.MainNo = MainSet.RecNo;
            T2Set.Delete(trans);

            foreach (IntegrationT2Row row in rows)
            {
                T2Set.No = row.No;
                T2Set.Line = row.Line;
                T2Set.Clause = row.Clause;
                T2Set.Description = row.Description;
                T2Set.Result = row.Result;
                T2Set.Insert(trans);
            }
        }

        private void SaveT3(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US) return;

            List<IntegrationT2Row> rows = CtrlEu.T3Rows;
            T3Set.MainNo = MainSet.RecNo;
            T3Set.Delete(trans);

            foreach (IntegrationT2Row row in rows)
            {
                T3Set.No = row.No;
                T3Set.Line = row.Line;
                T3Set.Clause = row.Clause;
                T3Set.Description = row.Description;
                T3Set.Result = row.Result;
                T3Set.Insert(trans);
            }
        }

        private void SaveT4(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US) return;

            List<IntegrationT1Row> rows = CtrlEu.T4Rows;
            T4Set.MainNo = MainSet.RecNo;
            T4Set.Delete(trans);

            foreach (IntegrationT1Row row in rows)
            {
                T4Set.No = row.No;
                T4Set.Line = row.Line;
                T4Set.Sample = row.Requested;
                T4Set.Result = row.Conclusion;
                T4Set.Insert(trans);
            }
        }

        private void SaveT5(EReportArea area, SqlTransaction trans)
        {
            List<IntegrationT1Row> rows = (area == EReportArea.US) ? CtrlUs.T5Rows : CtrlEu.T5Rows;
            T5Set.MainNo = MainSet.RecNo;
            T5Set.Delete(trans);

            foreach (IntegrationT1Row row in rows)
            {
                T5Set.No = row.No;
                T5Set.Line = row.Line;
                T5Set.Sample = row.Requested;
                T5Set.BurningRate = row.Conclusion;
                T5Set.Insert(trans);
            }
        }

        private void SaveT6(EReportArea area, SqlTransaction trans)
        {
            List<IntegrationT6Row> rows = (area == EReportArea.US) ? CtrlUs.T6Rows : CtrlEu.T6Rows;
            T6Set.MainNo = MainSet.RecNo;
            T6Set.Delete(trans);

            foreach (IntegrationT6Row row in rows)
            {
                T6Set.No = row.No;
                T6Set.Line = row.Line;
                T6Set.TestItem = row.TestItem;
                T6Set.Result = row.Result;
                T6Set.Requirement = row.Requirement;
                T6Set.Insert(trans);
            }
        }
    }
}
