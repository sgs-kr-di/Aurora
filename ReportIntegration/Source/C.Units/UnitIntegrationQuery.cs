﻿using DevExpress.Utils.Extensions;
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

        public IntegrationT61DataSet T61Set { get; set; }

        public IntegrationT7DataSet T7Set { get; set; }

        public PhysicalP2DataSet P2Set { get; set; }

        public PhysicalP3DataSet P3Set { get; set; }

        public PhysicalP40DataSet P40Set { get; set; }

        public PhysicalP41DataSet P41Set { get; set; }

        public PhysicalP42DataSet P42Set { get; set; }

        public PhysicalP45DataSet P45Set { get; set; }

        public PhysicalP5DataSet P5Set { get; set; }

        public PhysicalP6DataSet P6Set { get; set; }

        public IntegrationLimitEnDataSet LimitEnSet { get; set; }

        public IntegrationResultEnDataSet ResultEnSet { get; set; }

        public IntegrationLimitASTMDataSet LimitASTMSet { get; set; }

        public IntegrationResultASTMDataSet ResultASTMSet { get; set; }

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

        private bool bChkNoCoating = false, bChkCoating = false;

        private bool bSubstratePlasticLeadCheck;

        private bool bSubstrateMetalLeadCheck;

        private int surfaceLeadIndex;

        private int surfaceResultIndex;

        private int substrateLeadIndex;

        private int substrateResultIndex;

        int iSaveLoopResultCnt = 0;

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
                P45Set = new PhysicalP45DataSet(AppRes.DB.Connect, null, null);
                P5Set = new PhysicalP5DataSet(AppRes.DB.Connect, null, null);
                P6Set = new PhysicalP6DataSet(AppRes.DB.Connect, null, null);
                T6Set = new IntegrationT6DataSet(AppRes.DB.Connect, null, null);
                T61Set = new IntegrationT61DataSet(AppRes.DB.Connect, null, null);
                T7Set = new IntegrationT7DataSet(AppRes.DB.Connect, null, null);
                LimitEnSet = new IntegrationLimitEnDataSet(AppRes.DB.Connect, null, null);
                ResultEnSet = new IntegrationResultEnDataSet(AppRes.DB.Connect, null, null);
                LimitASTMSet = new IntegrationLimitASTMDataSet(AppRes.DB.Connect, null, null);
                ResultASTMSet = new IntegrationResultASTMDataSet(AppRes.DB.Connect, null, null);
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
                InsertT1(area, trans);          // page 2
                InsertT2(area, trans);          // page 3 US tb_integt2
                InsertT3(area, trans);          // 
                InsertT4(area, trans);          // page 4 EN
                InsertT5(area, trans);          // page 4 US, EN
                InsertT6(area, trans);          // page 8
                InsertT61(area, trans);         // page 8-1
                InsertT7(area, trans);          // page 5, 6
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
                T61Set.MainNo = mainNo;
                T61Set.Delete(trans);
                T7Set.MainNo = mainNo;
                T7Set.Delete(trans);

                LimitASTMSet.MainNo = mainNo;
                LimitASTMSet.Delete(trans);
                ResultASTMSet.MainNo = mainNo;
                ResultASTMSet.Delete(trans);

                ResultASTMSet.Delete_TB_INTEG_LEAD_LIMIT_ASTM(trans);
                ResultASTMSet.Delete_TB_INTEG_LEAD_RESULT_ASTM(trans);
                ResultASTMSet.Delete_TB_INTEG_LIMIT_ASTM(trans);
                ResultASTMSet.Delete_TB_INTEG_PHT_ASTM(trans);

                LimitEnSet.MainNo = mainNo;
                LimitEnSet.Delete(trans);
                ResultEnSet.MainNo = mainNo;
                ResultEnSet.Delete(trans);
                ResultEnSet.Delete_Tin(trans);
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
            string fileNoDate = $"F690101/LF-CTS{phyMainSet.P1FileNo} dated {phyMainSet.RequiredTime.ToString("yyyy-MM-dd")}";

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
            //MainSet.P1ItemNo = ProfJobSet.ItemNo;
            MainSet.P1ItemNo = phyMainSet.P1ItemNo;
            //MainSet.P1OrderNo = ProfJobSet.OrderNo; // 원래 값은 "-"이 입력됨.
            MainSet.P1OrderNo = "-"; // -로 다시 변경해달라고 하여 수정함.
            MainSet.P1Packaging = "Yes, provided";
            MainSet.P1Instruction = "Not provided";
            MainSet.P1Buyer = "-";
            //MainSet.P1Manufacturer = ProfJobSet.Manufacturer;
            MainSet.P1Manufacturer = phyMainSet.P1Manufacturer;

            if (string.IsNullOrEmpty(phyMainSet.P1CountryOfOrigin))
            {
                MainSet.P1CountryOfOrigin = "-";
            }
            else 
            {
                MainSet.P1CountryOfOrigin = phyMainSet.P1CountryOfOrigin;
            }
                
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
                "provided by client. Please refer to following page(s)";

            //MainSet.P1Comments = ProfJobSet.ReportComments;
            /*
            MainSet.P1Comments =
                "The results shown in this test report refer only to the sample(s)\r\n" +
                "tested unless otherwise stated.\r\n" +
                "this test report is not related to Korea Laboratory\r\n" +
                "Accrediation Scheme.";
            
            MainSet.P1Comments = "The results shown in this test report refer only to the sample(s) tested unless\r\n" +
                                 "otherwise stated.\r\n" +
                                 "This test report is not related to Korea Laboratory Accreditation Scheme.";
            */

            MainSet.P1Comments = "The results shown in this test report refer only to the sample(s) tested unless\r\n" +
                     "otherwise stated.\r\n" +
                     "This test report is not related to Korea Laboratory Accreditation Scheme.\r\n" +
                     "The statement of conformity was made on the requested specification or\r\n" +
                     "standard. The decision rule would be based on the binary statement (Pass/Fail)\r\n" +
                     "according to ILAC-G8:09/2019 guideline 4.2.1 without taking measurement\r\n" +
                     "uncertainty into account by applicant's agreement.";

            /*
             The results shown in this test report refer only to the sample(s) tested unless otherwise stated.
             This test report is not related to Korea Laboratory Accreditation Scheme.
             The statement of conformity was made on the requested specification or standard. The decision rule would be based on the binary statement (Pass/Fail) according to ILAC-G8:09/2019 guideline 4.2.1 without taking measurement uncertainty into account by applicant's agreement.
             */
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
                MainSet.Description1 = "As specified in ASTM F963-23 standard consumer safety specification on toy safety.";
                MainSet.Description2 =
                    $"The below results are extracted from the test report number {fileNoDate}\r\n"+
                    $"where the samples are claimed to be identical.";
                //$"Picture of Sample as copied from the test report no.\r\n" +
                //$"{fileNoDate}";
                //MainSet.Description3 = $"N/A = Only applicable clauses were shown               **Visual Examination";
                MainSet.Description3 = $"N.B. Only applicable clauses were shown               *Visual Examination";
                MainSet.Description4 = $"Flammability Test(Clauses 4.2)";
                //MainSet.Description5 =
                //    "*Burning rate has been rounded to the nearest one tenth of an inch per second.\r\n\r\n" +
                //    "Requirement: A toy / component is considered a \"flammable solid\" if it ignites and burns with a self-sustaining\r\n" +
                //    "             flame at a rate greater than 0.1 in./s along its major axis.";
                MainSet.Description5 =
                    "*Burning rate has been rounded to the nearest one tenth of an inch per second.\r\n" +
                    "SE = Self-Exinguished\r\n" +
                    "DNI = Did Not Ignite\r\n" +
                    "NA = A major dimension of 1 in. (25 mm) or less.";
                MainSet.Description6 =
                    "Heavy Elements(Clause 4.3.5)\r\n\r\n" +            
                    "ASTM F963-23, Clause 4.3.5.1 - Heavy Elements in Paint/Similar Surface Coating Materials";
                MainSet.Description7 =
                    "Method: With reference to CPSC-CH-E1003-09.1 - Standard Operating Procedure for Determing Heavy Metal in Paint and Other Similar Surface Coatings. Analysis was performed by ICP-OES.";
                MainSet.Description8 = "Method: With reference to ASTM F963-23 Clause 8.3. Analysis was performed by ICP-OES.";
                MainSet.Description9 =
                    "Note: - Soluble results shown are of the adjusted analytical result.\r\n" +
                    "      - ND = Not Detected(<MDL)";
                //MainSet.Description10 =
                //    "Heavy Elements(Clause 4.3.5)\r\n\r\n" +
                //    "ASTM F963-17, Clause 4.3.5.1 - Heavy Elements in Paint/Similar Surface Coating Materials";
                MainSet.Description10 =
                    "Heavy Elements(Clause 4.3.5)\r\n\r\n" +
                    "ASTM F963-23, Clause 4.3.5.2 - Heavy Elements in Toys Substrate Materials";

                //MainSet.Description11 =
                //    "Method: With reference to CPSC-CH-E1003-09.1 - Standard Operating Procedure for Determing Heavy Metal in Paint and Other Similar Surface Coatings. Analysis was performed by ICP-OES.";
                MainSet.Description11 =
                    "Method (non-metallic materials): CPSC-CH-E1002-08.3 -Standard Operation Procedure for Determining Total Lead\r\n" +
                    "(Pb) in Non-Metal Children Product. Analysis was performed by ICP-OES.";

                MainSet.Description12 = "Method: With reference to ASTM F963-23 Clause 8.3. Analysis was performed by ICP-OES.";
                //MainSet.Description13 =
                //    "      - Soluble results shown are of the adjusted analytical result.\r\n" +
                //    "      - ND = Not Detected(<MDL)\r\n" +
                //    "      - MDL = Method Detection Limit";
                MainSet.Description13 =
                    "      - Soluble results shown are of the adjusted analytical result.\r\n" +
                    "      - N.D. = Not Detected(<MDL)\r\n" +
                    "      - MDL = Method Detection Limit";
                MainSet.Description14 = "Stuffing Materials(Clause 4.3.7)";
                MainSet.Description15 =
                    "Method: With reference to ASTM F963-23 Clause 8.29. Visual inspection is performed using a stereo widefield microscope, or equivalent, at 10 x magnification and adequate ilumination.";
                MainSet.Description16 = $"Picture of Sample as copied from the test report no.\r\n{fileNoDate}";
                MainSet.Description17 = phyMainSet.P1FileNo;
                MainSet.Description18 = $"1. % = percentage by weight\r\n" +
                                        $"2. 1 % = 10000 ppm (mg/kg)\r\n" +
                                        $"3. N.D. = Not Detected\r\n" +
                                        $"4. Method Detection Limit for each phthalate = 0.015 %";
                MainSet.Description19 =
                    "      - N.D. = Not Detected(<MDL)\r\n" +
                    "      - MDL = Method Detection Limit";
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
                //MainSet.Description3 = "As specified in European standard on safety of toys EN71 PART 2: 2011+A1:2014";
                MainSet.Description3 = "As specified in European standard on safety of toys EN71 PART 2: 2020";
                //MainSet.Description4 = "* Surface Flash of Pile Fabrics(Clause 4.1)";
                MainSet.Description4 = "4.1 General requirement_Surface Flash of Pile Fabrics";
                //MainSet.Description5 = "** Soft-filled toys(animals and doll, etc.) with a piled or textile surface (Clause 4.5)";
                MainSet.Description5 = "4.5 Soft-filled toys";
                MainSet.Description6 =
                    "SE = Self-Extinguishing\r\n" +
                    "DNI = Did not Ignite\r\n" +
                    "FAIL: Exceed the limit\r\n" +
                    "* The sample(s) was (were) not tested as its maximum dimension is 150 mm or less\r\n" +
                    "Requirement: The rate of spread of flame on the surface of toy shall not be greater than 30 mm/sec\r\n\r\n" +
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
                //MainSet.Description9 = "Method : With reference to EN71-3:2019. Analysis of general elements was performed by ICP-OES and Chromium(III) was obtained by calcuration, chromium(VI) was analyzed by IC-UV/VIS.";
                MainSet.Description9 = "Method : With reference to EN71-3:2019+A1:2021. Analysis of general elements was performed by ICP-OES and Chromium (III) was obtained by calculation, chromium (VI) was analyzed by IC-UV/VIS. Organic Tin was analyzed by GC-MS";
                MainSet.Description14 = "Method : With reference to EN71-3:2019+A1:2021. Analysis of general elements was performed by ICP-OES and Chromium (III) was obtained by calculation, chromium (VI) was analyzed by IC-UV/VIS.";
                MainSet.Description10 =
                    "Note. 1. mg/kg = milligram per kilogram\r\n" +
                    "      2. N.D. = Not Detected ( < Reporting Limit )\r\n" +
                    "      3. 1% = 10,000 mg/kg = 10,000 ppm\r\n" +
                    "      4.Soluble Chromium(III) = Soluble Total Chromium - Soluble Chromium(IV)\r\n" +
                    //"      5. ^ = The test result of soluble organic tin was derived from soluble tin screening and then confirmation test\r\n" +
                    //"         for soluble organic tin on component exceeding the screening limit of 4.9 mg/kg soluble Sn";
                    //"      5. ^ = Confirmation test of soluble organic tin is not reguired in case of\r\n" +
                    //"      soluble tin, after conversion, does not exceed the soluble organic tin\r\n" +
                    //"      reguirement as specitied in EN71-3 : 2019";
                    "      5. ^ = The test result of soluble organic tin was derived from soluble tin screening and then confirmation\r\n" +
                    "      test for soluble organic tin on component exceeding the screening limit of 4.9 mg/kg soluble Sn.";
                MainSet.Description11 = $"Picture of Sample as copied from the test report no.\r\n{fileNoDate}";
                MainSet.Description12 = phyMainSet.P1FileNo;
                MainSet.Description13 =
                    "NSFO = No Surface Flash Occurred\r\n" +
                    "SFO = Surface Flash Occurred";
                //MainSet.Description14 = "";
                MainSet.Description15 =
                    "1. The UKCA marking should be at least 5 mm in height - unless a different minimum dimension is specified in the" +
                    " relevant legislation. The UKCA marking should be easily visible, legible (from 1 January 2023 it must be permanently" +
                    " attached). To reduce or enlarge the size of your marking, the letters forming the UKCA marking must be in proportion to" +
                    " the version.\r\n\r\n" +
                    "2. UK importer shall label company’s details, including company’s name and a contact address after 1 January 2021." +
                    " Until 31 December 2022, UK importer can provide these details on the accompanying documentation rather on the" +
                    " good itself.\r\n\r\n" +
                    "3. Manufacturers must ensure that their toys bear a type, batch, serial or model number or other element allowing" +
                    " their identification, or where the size or nature of the toy does not allow it, that the required information is provided on" +
                    " the packaging or in a document accompanying the toy.";
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
                //P2Set.MainNo = ProductSet.PhyJobNo;
                //P2Set.Select(trans);

                //for (int i = 0; i < P2Set.RowCount; i++)
                //{
                //    P2Set.Fetch(i);

                //    T1Set.No = P2Set.No;
                //    T1Set.Line = P2Set.Line;
                //    T1Set.Requested = P2Set.Requested;
                //    T1Set.Conclusion = P2Set.Conclusion;
                //    T1Set.Insert(trans);
                //}
                
                T1Set.MainNo = MainSet.RecNo;
                T1Set.No = 0;
                T1Set.Line = false;
                T1Set.Requested = "US Public Law 110-314 (Consumer Product Safety Improvement Act of 2008, CPSIA):";
                T1Set.Conclusion = "-";
                T1Set.Insert(trans);
                                
                T1Set.No = 1;
                T1Set.Line = false;
                T1Set.Requested = "- ASTM F963-23 : Standard Consumer Safety Specification on Toy Safety";
                T1Set.Conclusion = "PASS";
                T1Set.Insert(trans);
                
                T1Set.No = 1;
                T1Set.Line = false;
                T1Set.Requested = "- Flammability of toys (16 C.F.R. 1500.44)";
                T1Set.Conclusion = "PASS";
                T1Set.Insert(trans);

                T1Set.No = 2;
                T1Set.Line = false;
                T1Set.Requested = "- Small part (16 C.F.R. 1501)";
                T1Set.Conclusion = "PASS";
                T1Set.Insert(trans);

                T1Set.No = 3;
                T1Set.Line = false;
                T1Set.Requested = "- Sharp points and edges (16 C.F.R. 1500.48 and 49)";
                T1Set.Conclusion = "PASS";
                T1Set.Insert(trans);
                
            }
            else
            {
                bool bChkConclusion = true;

                T1Set.MainNo = MainSet.RecNo;
                P2Set.MainNo = ProductSet.PhyJobNo;
                P2Set.Select(trans);

                for (int i = 0; i < P2Set.RowCount; i++)
                {
                    P2Set.Fetch(i);

                    if (P2Set.Conclusion.ToUpper().Trim().Equals("FAIL"))
                    {
                        bChkConclusion = false;
                    }

                    T1Set.No = P2Set.No;
                    T1Set.Line = P2Set.Line;
                    T1Set.Requested = P2Set.Requested;
                    T1Set.Conclusion = P2Set.Conclusion;
                    T1Set.Insert(trans);
                    //T1Set.No = P2Set.No;
                    //T1Set.Line = P2Set.Line;
                    //T1Set.Requested = "EN 71 Part 1:2014+A1:2018 - Mechanical and Physical Properties";
                    //T1Set.Conclusion = "PASS";
                    //T1Set.Insert(trans);

                    //T1Set.No = 1;
                    //T1Set.Line = false;
                    //T1Set.Requested = "EN 71 Part 2:2011+A1:2014 - Flammability of Toys";
                    //T1Set.Conclusion = "PASS";
                    //T1Set.Insert(trans);

                    //T1Set.No = 2;
                    //T1Set.Line = false;
                    //T1Set.Requested = "Labeling requirement (Washing/Cleaning Label, CE mark, importer / manufacturer mark (name, address), product identification) according to the Directive 2009/48/EC-Safety of toys";
                    //T1Set.Conclusion = "See note 1*";
                    //T1Set.Insert(trans);

                    //T1Set.No = 3;
                    //T1Set.Line = false;
                    ////T1Set.Requested = "Directive 2009/48/EC and its amendment Council Directive (EU) 2017/738, Commision Directive (EU) 2018/725-EN71-3:2019 - Migration of certain elements(By first action method testing only)";
                    //T1Set.Requested = "Directive 2009/48/EC and its amendment Council Directive (EU) 2017/738, Commision Directive (EU) 2018/725-EN71-3:2019 - Migration of certain elements";
                    //T1Set.Conclusion = "PASS";
                    //T1Set.Insert(trans);
                }

                T1Set.No = (P2Set.No+1);
                T1Set.Line = false;
                T1Set.Requested = "Directive 2009/48/EC and its amendment Council Directive (EU) 2017/738, Commission Directive (EU) 2019/1922 - EN71-3:2019+A1:2021 - Migration of certain elements";
                
                if (bChkConclusion)
                {
                    T1Set.Conclusion = "PASS";
                }
                else
                {
                    T1Set.Conclusion = "FAIL";
                }
                T1Set.Insert(trans);
            }
        }

        private void InsertT2(EReportArea area, SqlTransaction trans)
        {
            P3Set.MainNo = ProductSet.PhyJobNo;

            if (area == EReportArea.US)
            {
                //P3Set.No = 0;
                P3Set.SelectPhymain_P3(trans);

                for (int i = 0; i < P3Set.RowCount; i++)
                {
                    P3Set.Fetch(i);

                    if (string.IsNullOrEmpty(P3Set.Result))
                    {
                        P3Set.Result = "";
                    }

                    T2Set.MainNo = MainSet.RecNo;
                    T2Set.No = P3Set.No;
                    T2Set.Line = P3Set.Line;
                    T2Set.Clause = P3Set.Clause;
                    T2Set.Description = P3Set.Description;
                    T2Set.Result = P3Set.Result;
                    T2Set.Insert(trans);
                }

                //T2Set.MainNo = MainSet.RecNo;
                //P3Set.SelectPhymainNo_No(trans);

                //for (int i = 0; i < P3Set.RowCount; i++)
                //{
                //    P3Set.Fetch(i);

                //    if (P3Set.RecNo != 0)
                //    {
                //        if (string.IsNullOrEmpty(P3Set.Result)) 
                //        {
                //            P3Set.Result = "";
                //        }

                //        T2Set.MainNo = MainSet.RecNo;
                //        T2Set.No = P3Set.No;
                //        T2Set.Line = P3Set.Line;
                //        T2Set.Clause = P3Set.Clause;
                //        T2Set.Description = P3Set.Description;
                //        T2Set.Result = P3Set.Result;
                //        T2Set.Insert(trans);
                //    }
                //}

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
                //P3Set.No = 0;
                P3Set.SelectPhymain_P3(trans);

                for (int i = 0; i < P3Set.RowCount; i++)
                {
                    P3Set.Fetch(i);

                    T2Set.MainNo = MainSet.RecNo;
                    T2Set.No = P3Set.No;
                    T2Set.Line = P3Set.Line;
                    T2Set.Clause = P3Set.Clause;
                    T2Set.Description = P3Set.Description;
                    T2Set.Result = P3Set.Result;
                    T2Set.Insert(trans);
                }

                ///*
                //T2Set.MainNo = MainSet.RecNo;
                //T2Set.No = 0;
                //T2Set.Line = false;
                //T2Set.Clause = "4";
                //T2Set.Description = "General requirements";
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
                //T2Set.Description = "Material cleanliness";
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
                //T2Set.Clause = " 4.7";
                //T2Set.Description = "Edges";
                //T2Set.Result = "Pass";
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
                //T2Set.Line = true;
                //T2Set.Clause = " 4.8";
                //T2Set.Description = "Points and metallic wires";
                //T2Set.Result = "Pass";
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
                //T2Set.Clause = "5";
                //T2Set.Description = "Toys intended for children under 36 months";
                //T2Set.Result = "-";
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
                //T2Set.Clause = " 5.1";
                //T2Set.Description = "General requirements";
                //T2Set.Result = "Pass";
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
                //T2Set.Description = "5.1a Small part requirements on toys & removable components";
                //T2Set.Result = "Pass";
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
                //T2Set.Clause = "";
                //T2Set.Description = "     (Test method 8.2)";
                //T2Set.Result = "-";
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
                //T2Set.Clause = "";
                //T2Set.Description = "5.1b Torque test(Test method 8.3)";
                //T2Set.Result = "Pass";
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
                //T2Set.Clause = "";
                //T2Set.Description = "     Tension test(Test method 8.4)";
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
                //T2Set.Clause = "";
                //T2Set.Description = "     Drop test(Test method 8.5)";
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
                //T2Set.Clause = "";
                //T2Set.Description = "     Impact test(Test method 8.7)";
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
                //T2Set.Clause = "";
                //T2Set.Description = "     Sharp edge(Test method 8.11)";
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
                //T2Set.Line = false;
                //T2Set.Clause = "";
                //T2Set.Description = "     Sharp point(Test method 8.12)";
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
                //T2Set.Clause = " 5.2";
                //T2Set.Description = "Soft-filled toys and soft-filled parts of a toy";
                //T2Set.Result = "Pass";
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
                //T2Set.Line = false;
                //T2Set.Clause = " 5.4";
                //T2Set.Description = "Cords, chains and electrical cables in toys";
                //T2Set.Result = "Pass";
                //T2Set.Insert(trans);
                //*/
            }
        }

        private void InsertT3(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US) return;

            //P3Set.No = 0;
            //P3Set.SelectPhymain_P3(trans);            
            P40Set.MainNo = ProductSet.PhyJobNo;
            P40Set.Select(trans);

            for (int i = 0; i < P40Set.RowCount; i++)
            {
                P40Set.Fetch(i);

                T3Set.MainNo = MainSet.RecNo;
                T3Set.No = P40Set.No;
                T3Set.Line = P40Set.Line;
                T3Set.Clause = P40Set.Clause;
                T3Set.Description = P40Set.Description;
                T3Set.Result = P40Set.Result;
                T3Set.Insert(trans);
            }

            //T3Set.MainNo = MainSet.RecNo;
            //T3Set.No = 0;
            //T3Set.Line = false;
            //T3Set.Clause = "4.1";
            //T3Set.Description = "General requirements";
            //T3Set.Result = "Pass(See note *)";
            //T3Set.Insert(trans);

            //T3Set.No = 1;
            //T3Set.Line = false;
            //T3Set.Clause = "4.5";
            //T3Set.Description = "Soft-filled toys(animal and doll, etc.) with apiled or textile surface";
            //T3Set.Result = "Pass(See note **)";
            //T3Set.Insert(trans);
        }

        private void InsertT4(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US) return;

            P41Set.MainNo = ProductSet.PhyJobNo;
            P41Set.Select(trans);

            for (int i = 0; i < P41Set.RowCount; i++)
            {
                P41Set.Fetch(i);

                T4Set.MainNo = MainSet.RecNo;
                T4Set.No = P41Set.No;
                T4Set.Line = P41Set.Line;
                T4Set.Sample = P41Set.Sample;
                T4Set.Result = P41Set.Result;  // grid는 result이지만 db column은 burningrate.
                T4Set.Insert(trans);
            }
            
            /*
            T4Set.No = 0;
            T4Set.Line = false;
            T4Set.Sample = P41Set.Sample;
            T4Set.Result = "NSFO";            
            */
        }

        private void InsertT5(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US)
            {
                P41Set.MainNo = ProductSet.PhyJobNo;
                P41Set.Select(trans);

                for (int i = 0; i < P41Set.RowCount; i++)
                {
                    P41Set.Fetch(i);

                    if (string.IsNullOrEmpty(P41Set.Result))
                    {
                        P41Set.Result = "";
                    }

                    T5Set.MainNo = MainSet.RecNo;
                    T5Set.No = P41Set.No;
                    T5Set.Line = P41Set.Line;
                    T5Set.Sample = P41Set.Sample;
                    T5Set.BurningRate = P41Set.BurningRate;
                    T5Set.Result = P41Set.Result;
                    T5Set.Insert(trans);
                }
            }
            else
            {
                P45Set.MainNo = ProductSet.PhyJobNo;
                P45Set.Select(trans);

                for (int i = 0; i < P45Set.RowCount; i++)
                {
                    P45Set.Fetch(i);

                    T5Set.MainNo = MainSet.RecNo;
                    T5Set.No = P45Set.No;
                    T5Set.Line = P45Set.Line;
                    T5Set.Sample = P45Set.Sample;
                    T5Set.BurningRate = P45Set.BurningRate;
                    T5Set.Result = P45Set.Result;
                    T5Set.Insert(trans);
                }
                
                /*
                T5Set.No = 0;
                T5Set.Line = false;
                T5Set.Sample = " toy";
                T5Set.BurningRate = "4.2";
                */
            }
        }

        private void InsertT6(EReportArea area, SqlTransaction trans)
        {
            P5Set.MainNo = ProductSet.PhyJobNo;

            //P5Set.Fetch(i);
            //T6Set.MainNo = MainSet.RecNo;
            //T6Set.No = P5Set.No;
            //T6Set.Line = P5Set.Line;
            //T6Set.TestItem = P5Set.TestItem;
            //T6Set.Result = P5Set.Result;
            //T6Set.Requirement = P5Set.Requirement;
            //T6Set.Insert(trans);

            if (area == EReportArea.US)
            {
                P5Set.Select(trans);

                for (int i = 0; i < P5Set.RowCount; i++)
                {
                    //T6Set.No = P5Set.No;
                    //P5Set.SelectPhymainNo_No(trans);
                    P5Set.Fetch(i);
                    T6Set.No = P5Set.No;
                    T6Set.MainNo = MainSet.RecNo;
                    T6Set.Line = P5Set.Line;
                    T6Set.TestItem = P5Set.TestItem;
                    T6Set.Result = P5Set.Result;
                    T6Set.Requirement = P5Set.Requirement;
                    T6Set.Insert(trans);
                }

                //T6Set.MainNo = MainSet.RecNo;
                //T6Set.No = 0;
                //T6Set.Line = false;
                ///*
                //T6Set.TestItem =
                //    "   1. Objectionable matter originating from\r\n" +
                //    "      Insect, bird and rodent or other animal\r\n" +
                //    "      infestation";
                //T6Set.Result = "Absent";
                //T6Set.Requirement = "Absent";
                //*/

                //T6Set.MainNo = MainSet.RecNo;
                //P5Set.No = 0;
                //P5Set.SelectPhymainNo_No(trans);
                //P5Set.Fetch();

                //T6Set.No = P5Set.No;
                //T6Set.Line = P5Set.Line;
                //T6Set.TestItem = P5Set.TestItem;
                //T6Set.Result = P5Set.Result;
                //T6Set.Requirement = P5Set.Requirement;
                //T6Set.Insert(trans);

                //P5Set.No = 1;
                //P5Set.SelectPhymainNo_No(trans);
                //P5Set.Fetch();

                //T6Set.No = P5Set.No;
                //T6Set.Line = P5Set.Line;
                //T6Set.TestItem = P5Set.TestItem;
                //T6Set.Result = P5Set.Result;
                //T6Set.Requirement = P5Set.Requirement;
                //T6Set.Insert(trans);

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
                P5Set.MainNo = ProductSet.PhyJobNo;
                P5Set.Select(trans);

                for (int i = 0; i < P5Set.RowCount; i++)
                {
                    P5Set.Fetch(i);
                    T6Set.MainNo = MainSet.RecNo;                    
                    T6Set.No = P5Set.No;
                    T6Set.Line = P5Set.Line;
                    T6Set.TestItem = P5Set.TestItem;
                    T6Set.Result = P5Set.Result;
                    T6Set.Requirement = P5Set.Requirement;
                    T6Set.Insert(trans);
                }

                //P5Set.No = 0;
                //P5Set.SelectPhymainNo_No(trans);
                //P5Set.Fetch();

                //T6Set.MainNo = MainSet.RecNo;
                //T6Set.MainNo = MainSet.RecNo;
                //T6Set.No = P5Set.No;
                //T6Set.Line = P5Set.Line;
                //T6Set.TestItem = P5Set.TestItem;
                //T6Set.Result = P5Set.Result;
                //T6Set.Requirement = P5Set.Requirement;
                //T6Set.Insert(trans);

                ///*
                //T6Set.No = 0;
                //T6Set.Line = true;
                //T6Set.TestItem = "Washing/Cleaning instruction";
                //T6Set.Result = "Present";
                //T6Set.Requirement = "Affixed label and Hangtag";
                //T6Set.Insert(trans);
                //*/

                //P5Set.No = 1;
                //P5Set.SelectPhymainNo_No(trans);
                //P5Set.Fetch();

                //T6Set.No = P5Set.No;
                //T6Set.Line = P5Set.Line;
                //T6Set.TestItem = P5Set.TestItem;
                //T6Set.Result = P5Set.Result;
                //T6Set.Requirement = P5Set.Requirement;
                //T6Set.Insert(trans);

                ///*
                //T6Set.No = 1;
                //T6Set.Line = true;
                //T6Set.TestItem = "CE mark";
                //T6Set.Result = "Present";
                //T6Set.Requirement = "Affixed label and Hangtag";
                //T6Set.Insert(trans);
                //*/

                //P5Set.No = 2;
                //P5Set.SelectPhymainNo_No(trans);
                //P5Set.Fetch();

                //T6Set.No = P5Set.No;
                //T6Set.Line = P5Set.Line;
                //T6Set.TestItem = P5Set.TestItem;
                //T6Set.Result = P5Set.Result;
                //T6Set.Requirement = P5Set.Requirement;
                //T6Set.Insert(trans);

                ///*
                //T6Set.No = 2;
                //T6Set.Line = true;
                //T6Set.TestItem = "Importer's Name & Address";
                //T6Set.Result = "Present";
                //T6Set.Requirement = "Affixed label and Hangtag";
                //T6Set.Insert(trans);
                //*/

                //P5Set.No = 3;
                //P5Set.SelectPhymainNo_No(trans);
                //P5Set.Fetch();

                //T6Set.No = P5Set.No;
                //T6Set.Line = P5Set.Line;
                //T6Set.TestItem = P5Set.TestItem;
                //T6Set.Result = P5Set.Result;
                //T6Set.Requirement = P5Set.Requirement;
                //T6Set.Insert(trans);

                ///*
                //T6Set.No = 3;
                //T6Set.Line = true;
                //T6Set.TestItem = "Manufacturer's Name & Address";
                //T6Set.Result = "Present";
                //T6Set.Requirement = "Affixed label and Hangtag";
                //T6Set.Insert(trans);
                //*/

                //P5Set.No = 4;
                //P5Set.SelectPhymainNo_No(trans);
                //P5Set.Fetch();

                //T6Set.No = P5Set.No;
                //T6Set.Line = P5Set.Line;
                //T6Set.TestItem = P5Set.TestItem;
                //T6Set.Result = P5Set.Result;
                //T6Set.Requirement = P5Set.Requirement;
                //T6Set.Insert(trans);

                ///*
                //T6Set.No = 4;
                //T6Set.Line = true;
                //T6Set.TestItem = "Product ID";
                //T6Set.Result = "Present";
                //T6Set.Requirement = "Affixed label and Hangtag";
                //T6Set.Insert(trans);
                //*/
            }
        }

        private void InsertT61(EReportArea area, SqlTransaction trans)
        {
            //P5Set.MainNo = ProductSet.PhyJobNo;
            //P5Set.Select(trans);

            if (area == EReportArea.US)
            {

            }
            else
            {
                P6Set.MainNo = ProductSet.PhyJobNo;
                P6Set.Select(trans);

                for (int i = 0; i < P6Set.RowCount; i++)
                {
                    P6Set.Fetch(i);
                    T61Set.MainNo = MainSet.RecNo;
                    T61Set.No = P6Set.No;
                    T61Set.Line = P6Set.Line;
                    T61Set.TestItem = P6Set.TestItem;
                    T61Set.Result = P6Set.Result;
                    T61Set.Requirement = P6Set.Requirement;
                    T61Set.Note = "";
                    T61Set.Description = "";
                    T61Set.Insert(trans);
                }
            }
        }

        private void InsertT7(EReportArea area, SqlTransaction trans)
        {
            int iSaveLoopIntegrationCnt = 0, iSaveLoopHyphenCnt = 0;            
            int iCountHypen = 0;
            string sP1FileNo = "";
            string sSampleDescription = "";
            iSaveLoopResultCnt = 0;

            bSubstrateMetalLeadCheck = false;
            bSubstratePlasticLeadCheck = false;

            surfaceLeadIndex = 0;
            surfaceResultIndex = 0;

            substrateLeadIndex = 0;
            substrateResultIndex = 0;

            Dictionary<string, int> dicTinNo = new Dictionary<string, int>(); // 시료에서 추가 분석된 Lead의 No와 동일하게 매칭하기 위한 Dictionary 변수

            partSet.ProductNo = ProductSet.RecNo;
            partSet.Select(trans);

            for (int i=0; i < partSet.RowCount; i++)
            {
                //var vDictionaryReportInsertNo = new Dictionary<string, int>();
                partSet.Fetch(i);

                if (string.IsNullOrWhiteSpace(partSet.JobNo) == false)
                {
                    cheMainSet.Select(partSet.JobNo, trans);
                    cheMainSet.Fetch();
                    if (cheMainSet.Empty == false)
                    {
                        iSaveLoopIntegrationCnt = iSaveLoopIntegrationCnt + 1;

                        cheP2Set.MainNo = partSet.JobNo;

                        if (area == EReportArea.US)
                        {
                            cheP2Set.Select_Che2Sampleident_RESULT_ASTM(trans);
                        }
                        else if(area == EReportArea.EU)
                        {
                            cheP2Set.Select_Che2Sampleident_HYPEN_EN(trans);
                        }
                        else 
                        {
                            Console.WriteLine("Area 미분류");                            
                        }

                        for (int j = 0; j < cheP2Set.RowCount; j++)
                        {
                            if (area == EReportArea.US)
                            {
                                cheP2Set.Fetch(j, 0, "Integr_ASTM");
                            }
                            else if (area == EReportArea.EU)
                            {
                                cheP2Set.Fetch(j, 0, "Integr_EN");
                            }
                            
                            //cheMainSet.Fetch();

                            ProfJobSet.JobNo = cheMainSet.RecNo;
                            //ProfJobSet.Select_TopOne_Sampleident_Aurora(trans);
                            ProfJobSet.Select_TopOne_Sampleident_KRCTS01(trans);
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
                            //if (!ProfJobSet.SampleDescription.Equals(""))
                            //{
                            //    sSampleDescription = ProfJobSet.SampleDescription[0].ToString().ToUpper() + ProfJobSet.SampleDescription.Substring(1).ToLower();
                            //    T7Set.Description = sSampleDescription;
                            //}
                            //else
                            //{
                            //    T7Set.Description = "";
                            //}
                            T7Set.Description = cheP2Set.SampleDescription.ToUpper();
                            //T7Set.Description = ProfJobSet.SampleDescription;
                            T7Set.Name = partSet.Name;
                            T7Set.MaterialNo = partSet.MaterialNo;
                            //T7Set.ReportNo = cheMainSet.P1FileNo;
                            T7Set.ReportNo = sP1FileNo;
                            //T7Set.IssuedDate = cheMainSet.RegTime.ToString("yyyy.MM.dd");
                            T7Set.IssuedDate = cheMainSet.RequiredTime.ToString("yyyy.MM.dd");
                            T7Set.Insert(trans);
                            //vDictionaryReportInsertNo.Add(cheMainSet.RecNo, iSaveLoopIntegrationCnt);

                        }

                        iSaveLoopResultCnt = iSaveLoopResultCnt + 1;

                        switch (area)
                        {
                            case EReportArea.US:
                                cheMainSet.RecNo = partSet.JobNo;

                                for (int k = 0; k < cheMainSet.RowCount; k++)
                                {
                                    iSaveLoopResultCnt = iSaveLoopResultCnt + k;
                                    //if (i == 0) InsertLimitASTM(cheMainSet.RecNo, cheMainSet.P1FileNo, trans);
                                    InsertLimitASTM(cheMainSet.RecNo, cheMainSet.P1FileNo, trans);
                                    InsertResultAstm_New(iSaveLoopResultCnt, cheMainSet.RecNo, cheMainSet.P1FileNo, dicTinNo, trans);

                                    //InsertResultAstm(cheMainSet.RecNo, cheMainSet.LeadType, trans);
                                    //iSaveLoopResultCnt = iSaveLoopResultCnt + k;
                                    ////InsertSubstrateLeadResultAstm
                                    //if (i == 0) InsertLimitASTM(cheMainSet.RecNo, trans);
                                    //InsertResultEn(iSaveLoopResultCnt, cheMainSet.RecNo, trans);
                                }
                                break;

                            case EReportArea.EU:
                                cheMainSet.RecNo = partSet.JobNo;
                                //cheP2Set.Select_Che2Sampleident_HYPEN_EN(trans);
                                //cheP2Set.Select(trans);
                                //cheP2Set.Fetch();

                                for (int k = 0; k < cheMainSet.RowCount; k++)
                                {
                                    iSaveLoopResultCnt = iSaveLoopResultCnt + k;

                                    if (i == 0) InsertLimitEn(cheMainSet.RecNo, trans);
                                    InsertResultEn(iSaveLoopResultCnt, cheMainSet.RecNo, trans);
                                }
                                break;
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
                    /*
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
             */
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

        private void InsertResultAstm_New(int index, string recNo, string fileNo, Dictionary<string, int> dicTinNo, SqlTransaction trans)
        {
            //Dictionary<string, int> dicTinNo = new Dictionary<string, int>(); // 시료에서 추가 분석된 Lead의 No와 동일하게 매칭하기 위한 Dictionary 변수
            cheP2Set.MainNo = recNo;
            //cheP2Set.Pro_Proj = fileNo;
            cheP2Set.Select_Che2Sampleident_RESULT_ASTM(trans);
            ResultASTMSet.MainNo = MainSet.RecNo;

            if (cheP2Set.Empty == false)
            {
                for (int i = 0; i < cheP2Set.RowCount; i++)
                {
                    if (i > 0)
                    {
                        index = index + 1;
                        iSaveLoopResultCnt = index;
                    }

                    cheP2Set.Fetch(i, 0, "Integr_ASTM");

                    if (cheP2Set.Sch_Code.Equals("HCEEASTMICP_09"))
                    {
                        ResultASTMSet.Pro_proj = cheP2Set.Pro_Proj;
                        ResultASTMSet.Sampleident = cheP2Set.Sampleident;
                        ResultASTMSet.Sch_code = cheP2Set.Sch_Code;
                        ResultASTMSet.Sam_description = cheP2Set.SampleDescription;
                        ResultASTMSet.Sam_remarks = cheP2Set.SampleRemarks;
                        ResultASTMSet.No = index;
                        ResultASTMSet.Mg = cheP2Set.Mg;
                        ResultASTMSet.Pb = cheP2Set.Pb;
                        ResultASTMSet.Sb = cheP2Set.Sb;
                        ResultASTMSet.As = cheP2Set.As;
                        ResultASTMSet.Ba = cheP2Set.Ba;
                        ResultASTMSet.Cd = cheP2Set.Cd;
                        ResultASTMSet.Cr = cheP2Set.Cr;
                        ResultASTMSet.Hg = cheP2Set.Hg;
                        ResultASTMSet.Se = cheP2Set.Se;
                        ResultASTMSet.Insert(trans);
                        try
                        {
                            dicTinNo.Add(cheP2Set.Pro_Proj + "-1", index);
                        }
                        catch (Exception e) 
                        {

                        }
                    }
                }
            }

            cheP2Set.Pro_Proj = fileNo + "-1";
            cheP2Set.Select_TB_CHEP2_LEAD_LIMIT_ASTM(trans);
            if (cheP2Set.Empty == false)
            {
                for (int i = 0; i < cheP2Set.RowCount; i++)
                {
                    cheP2Set.Fetch(i, 0, "Integr_ASTM_Lead_Limit");

                    if (!cheP2Set.Sch_Code.Equals("HCEEASTMICP_09") && !cheP2Set.Sch_Code.Equals("HCEEPHTHALATE_09"))
                    {
                        ResultASTMSet.Pro_proj = cheP2Set.Pro_Proj;
                        ResultASTMSet.Sampleident = cheP2Set.Sampleident;
                        ResultASTMSet.lovalue = cheP2Set.LoValue;
                        ResultASTMSet.hivalue = cheP2Set.HiValue;
                        ResultASTMSet.reportvalue = cheP2Set.ReportValue;
                        ResultASTMSet.Sch_code = cheP2Set.Sch_Code;
                        ResultASTMSet.Sam_remarks = cheP2Set.SampleRemarks;
                        ResultASTMSet.Insert_TB_INTEG_LEAD_LIMIT_ASTM(trans);
                    }
                }
            }

            cheP2Set.Pro_Proj = fileNo + "-1";
            cheP2Set.Select_TB_CHEP2_LEAD_RESULT_ASTM(trans);
            if (cheP2Set.Empty == false)
            {
                for (int i = 0; i < cheP2Set.RowCount; i++)
                {
                    cheP2Set.Fetch(i, 0, "Integr_ASTM_Lead_Result");

                    if (!cheP2Set.Sch_Code.Equals("HCEEASTMICP_09") && !cheP2Set.Sch_Code.Equals("HCEEPHTHALATE_09"))
                    {
                        ResultASTMSet.Pro_proj = cheP2Set.Pro_Proj;
                        ResultASTMSet.Sampleident = cheP2Set.Sampleident;
                        //ResultASTMSet.No = index;

                        int No_Calc = Convert.ToInt32(ResultASTMSet.Sampleident.Substring(ResultASTMSet.Sampleident.Length - 1));

                        if (No_Calc > 1)
                        {
                            ResultASTMSet.No = (dicTinNo[cheP2Set.Pro_Proj] + No_Calc - 1);
                        }
                        else
                        {
                            ResultASTMSet.No = dicTinNo[cheP2Set.Pro_Proj];
                        }

                        ResultASTMSet.Pb = cheP2Set.Pb;
                        ResultASTMSet.Sch_code = cheP2Set.Sch_Code;
                        ResultASTMSet.Sam_description = cheP2Set.SampleDescription;
                        ResultASTMSet.Sam_remarks = cheP2Set.SampleRemarks;

                        ResultASTMSet.Insert_TB_INTEG_LEAD_RESULT_ASTM(trans);
                    }
                }
            }

            cheP2Set.Pro_Proj = fileNo + "-1";
            cheP2Set.Select_TB_CHEPPHT_ASTM(trans);
            if (cheP2Set.Empty == false)
            {
                for (int i = 0; i < cheP2Set.RowCount; i++)
                {
                    cheP2Set.Fetch(i, 0, "Integr_ASTM_Pht");

                    if (cheP2Set.Sch_Code.Equals("HCEEPHTHALATE_09"))
                    {
                        ResultASTMSet.Pro_proj = cheP2Set.Pro_Proj;
                        ResultASTMSet.Sampleident = cheP2Set.Sampleident;
                        ResultASTMSet.Sch_code = cheP2Set.Sch_Code;
                        ResultASTMSet.Sam_description = cheP2Set.SampleDescription;
                        ResultASTMSet.Sam_remarks = cheP2Set.SampleRemarks;
                        //ResultASTMSet.No = index;

                        int No_Calc = Convert.ToInt32(ResultASTMSet.Sampleident.Substring(ResultASTMSet.Sampleident.Length - 1));

                        if (No_Calc > 1)
                        {
                            ResultASTMSet.No = (dicTinNo[cheP2Set.Pro_Proj] + No_Calc - 1);
                        }
                        else
                        {
                            ResultASTMSet.No = dicTinNo[cheP2Set.Pro_Proj];
                        }
                        
                        ResultASTMSet.DBP = cheP2Set.DBP;
                        ResultASTMSet.BBP = cheP2Set.DBP;
                        ResultASTMSet.DEHP = cheP2Set.DEHP;
                        ResultASTMSet.DINP = cheP2Set.DINP;
                        ResultASTMSet.DCHP = cheP2Set.DCHP;
                        ResultASTMSet.DnHP = cheP2Set.DnHP;
                        ResultASTMSet.DIBP = cheP2Set.DIBP;
                        ResultASTMSet.DnPP = cheP2Set.DnPP;
                        ResultASTMSet.DNOP = cheP2Set.DNOP;
                        ResultASTMSet.DIDP = cheP2Set.DIDP;

                        ResultASTMSet.Insert_TB_INTEG_PHT_ASTM(trans);
                    }
                }
            }
        }

        private void InsertLimitASTM(string recNo, string fileNo, SqlTransaction trans)
        {
            //if (bChkNoCoating == true) return;
            //if (bChkCoating == true) return;

            /*
            if (bChkCoating && bChkNoCoating) return;

            cheP2Set.MainNo = recNo;
            //cheP2Set.Select(trans);
            cheP2Set.Select_Limit_ASTM(trans);

            if (cheP2Set.Empty == false)
            {
                for (int i = 0; i < cheP2Set.RowCount; i++)
                {
                    cheP2Set.Fetch(i);

                    LimitASTMSet.MainNo = MainSet.RecNo;
                    LimitASTMSet.Sampleident = cheP2Set.Sampleident;
                    LimitASTMSet.Pro_proj = cheP2Set.Pro_Proj;
                    LimitASTMSet.Name = cheP2Set.Name;
                    LimitASTMSet.LoValue = cheP2Set.LoValue;
                    LimitASTMSet.HiValue = cheP2Set.HiValue;
                    LimitASTMSet.ReportValue = cheP2Set.ReportValue;
                    LimitASTMSet.Sch_code = cheP2Set.Sch_Code;
                    LimitASTMSet.Sam_remarks = cheP2Set.SampleRemarks;

                    if ((LimitASTMSet.Sam_remarks.ToLower() == "textile" || LimitASTMSet.Sam_remarks.ToLower() == "plastic" || LimitASTMSet.Sam_remarks.ToLower() == "metal") && bChkNoCoating)
                    {
                        break;
                    }

                    if ((LimitASTMSet.Sam_remarks.ToLower() == "coating") && bChkCoating)
                    {
                        break;
                    }

                    LimitASTMSet.Insert(trans);
                }

                if (LimitASTMSet.Sam_remarks.ToLower() == "textile" || LimitASTMSet.Sam_remarks.ToLower() == "plastic" || LimitASTMSet.Sam_remarks.ToLower() == "metal")
                {
                    bChkNoCoating = true;
                }

                if ((LimitASTMSet.Sam_remarks.ToLower() == "coating"))
                {
                    bChkCoating = true;
                }
                


            }
            
            //cheP2Set.Pro_Proj = fileNo;
            //cheP2Set.Select_Limit_FileNo_ASTM(trans);

            //if (cheP2Set.Empty == false)
            //{
            //    for (int i = 0; i < cheP2Set.RowCount; i++)
            //    {
            //        cheP2Set.Fetch(i);

            //        LimitASTMSet.MainNo = MainSet.RecNo;
            //        LimitASTMSet.Sampleident = cheP2Set.Sampleident;
            //        LimitASTMSet.Pro_proj = cheP2Set.Pro_Proj;
            //        LimitASTMSet.Name = cheP2Set.Name;
            //        LimitASTMSet.LoValue = cheP2Set.LoValue;
            //        LimitASTMSet.HiValue = cheP2Set.HiValue;
            //        LimitASTMSet.ReportValue = cheP2Set.ReportValue;
            //        LimitASTMSet.Sch_code = cheP2Set.Sch_Code;
            //        LimitASTMSet.Sam_remarks = cheP2Set.SampleRemarks;
            //        LimitASTMSet.Insert(trans);
            //    }
            //}
            */

            cheP2Set.MainNo = recNo;            
            cheP2Set.Select_Limit_ASTM(trans);

            if (cheP2Set.Empty == false)
            {
                for (int i = 0; i < cheP2Set.RowCount; i++)
                {
                    cheP2Set.Fetch(i);

                    LimitASTMSet.MainNo = MainSet.RecNo;
                    LimitASTMSet.Sampleident = cheP2Set.Sampleident;
                    LimitASTMSet.Pro_proj = cheP2Set.Pro_Proj;
                    LimitASTMSet.Name = cheP2Set.Name;
                    LimitASTMSet.LoValue = cheP2Set.LoValue;
                    LimitASTMSet.HiValue = cheP2Set.HiValue;
                    LimitASTMSet.ReportValue = cheP2Set.ReportValue;
                    LimitASTMSet.Sch_code = cheP2Set.Sch_Code;
                    LimitASTMSet.Sam_remarks = cheP2Set.SampleRemarks;

                    cheP2ExtendSet.MainNo = MainSet.RecNo;
                    cheP2ExtendSet.SampleRemarks = cheP2Set.SampleRemarks.ToLower().Trim();
                    cheP2ExtendSet.Select_Limit_Sam_remarks_ASTM(trans);

                    if (cheP2ExtendSet.RowCount < 8)
                    {
                        if (cheP2Set.SampleRemarks.ToLower().Trim().Equals("textile")) 
                        {                            
                            cheP2ExtendSet.Select_Limit_Sam_remarks_Plastic_ASTM(trans);
                            if (cheP2ExtendSet.RowCount < 8)
                            {
                                LimitASTMSet.Insert(trans);
                            }
                        }
                        else if (cheP2Set.SampleRemarks.ToLower().Trim().Equals("plastic"))
                        {
                            cheP2ExtendSet.Select_Limit_Sam_remarks_Textile_ASTM(trans);
                            if (cheP2ExtendSet.RowCount < 8)
                            {
                                LimitASTMSet.Insert(trans);
                            }
                        }
                        else if (cheP2Set.SampleRemarks.ToLower().Trim().Equals("coating"))
                        {
                            LimitASTMSet.Insert(trans);
                        }
                    }
                }
            }
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
            Dictionary<string, int> dicTinNo = new Dictionary<string, int>(); // 시료에서 추가 분석된 Tin의 No와 동일하게 매칭하기 위한 Dictionary 변수
            cheP2Set.MainNo = recNo;
            cheP2Set.Select_Che2Sampleident_HYPEN_EN(trans);
            ResultEnSet.MainNo = MainSet.RecNo;
            //ResultEnSet.Select(trans);

            //if (ResultEnSet.Empty == true)
            //{
                if (cheP2Set.Empty == false)
                {
                    //ResultEnSet.MainNo = recNo;
                    //ResultEnSet.MainNo = MainSet.RecNo;
                    //ResultEnSet.No = index + 1;
                    //ResultEnSet.Mg = "--";

                    for (int i = 0; i < cheP2Set.RowCount; i++)
                    {
                        if (i > 0)
                        {
                            index = index + 1;
                            iSaveLoopResultCnt = index;
                        }

                        cheP2Set.Fetch(i, 0, "Integr_EN");

                        if (cheP2Set.Sch_Code.Equals("HCEEENICP_15_02"))
                        {
                            if (string.IsNullOrEmpty(cheP2Set.OrgTin) == false)
                            {
                                if (cheP2Set.OrgTin.Equals("N.D."))
                                {
                                    dicTinNo.Add(cheP2Set.Sampleident, index);
                                }
                            }

                            ResultEnSet.Sampleident = cheP2Set.Sampleident;
                            ResultEnSet.Sch_Code = cheP2Set.Sch_Code;
                            ResultEnSet.SampleDescription = cheP2Set.SampleDescription;
                            ResultEnSet.No = index;
                            //ResultEnSet.No = cheP2Set.No;
                            ResultEnSet.Mg = cheP2Set.Mg;
                            ResultEnSet.Ai = cheP2Set.Ai;
                            ResultEnSet.Sb = cheP2Set.Sb;
                            ResultEnSet.As = cheP2Set.As;
                            ResultEnSet.Ba = cheP2Set.Ba;
                            ResultEnSet.B = cheP2Set.B;
                            ResultEnSet.Cd = cheP2Set.Cd;
                            ResultEnSet.Cr3 = cheP2Set.Cr3;
                            ResultEnSet.Cr6 = cheP2Set.Cr6;
                            ResultEnSet.Co = cheP2Set.Co;
                            ResultEnSet.Cu = cheP2Set.Cu;
                            ResultEnSet.Pb = cheP2Set.Pb;
                            ResultEnSet.Mn = cheP2Set.Mn;
                            ResultEnSet.Hg = cheP2Set.Hg;
                            ResultEnSet.Ni = cheP2Set.Ni;
                            ResultEnSet.Se = cheP2Set.Se;
                            ResultEnSet.Sr = cheP2Set.Sr;
                            ResultEnSet.Sn = cheP2Set.Sn;
                            ResultEnSet.OrgTin = cheP2Set.OrgTin;
                            ResultEnSet.Zn = cheP2Set.Zn;
                            ResultEnSet.Insert(trans);
                            ResultEnSet.Insert_HYPHEN(trans);
                        }
                    }
                }

                cheP2Set.MainNo = recNo;
                cheP2Set.Select_Che2Sampleident_HYPEN_EN_Tin(trans);

                if (cheP2Set.Empty == false)
                {
                    //ResultEnSet.MainNo = MainSet.RecNo;
                    //ResultEnSet.No = index;

                    for (int i = 0; i < cheP2Set.RowCount; i++)
                    {
                        cheP2Set.Fetch(i, 0, "Integr_EN_Tin");

                        if (cheP2Set.Sch_Code.Equals("HCEEORGANOTIN_11_01"))
                        {
                            // Sampleident의 맨마지막 값 추후에 10의 자리인 경우도 찾아야 한다면 10자리의 수가 0인지 체크하여 가져오기. 그 이후도 동일한 로직으로                        
                            ResultEnSet.Sampleident = cheP2Set.Sampleident;
                            ResultEnSet.Sch_Code = cheP2Set.Sch_Code;
                            ResultEnSet.SampleDescription = cheP2Set.SampleDescription;
                            ResultEnSet.No = dicTinNo[cheP2Set.Sampleident];
                            //ResultEnSet.No = cheP2Set.No;
                            ResultEnSet.DMT = cheP2Set.DMT;
                            ResultEnSet.MET = cheP2Set.MET;
                            ResultEnSet.DProT = cheP2Set.DProT;
                            ResultEnSet.MBT = cheP2Set.MBT;
                            ResultEnSet.DBT = cheP2Set.DBT;
                            ResultEnSet.TBT = cheP2Set.TBT;
                            ResultEnSet.MOT = cheP2Set.MOT;
                            ResultEnSet.DOT = cheP2Set.DOT;
                            ResultEnSet.TeBT = cheP2Set.TeBT;
                            ResultEnSet.DPhT = cheP2Set.DPhT;
                            ResultEnSet.TPhT = cheP2Set.TPhT;
                            ResultEnSet.Insert_Result_Tin(trans);
                        }
                    }
                }
            //}
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

