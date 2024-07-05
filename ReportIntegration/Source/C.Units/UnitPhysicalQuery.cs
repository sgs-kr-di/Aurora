using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;

namespace Sgs.ReportIntegration
{
    public class PhysicalQuery
    {
        public PhysicalMainDataSet MainSet { get; set; }

        public PhysicalImageDataSet ImageSet { get; set; }

        public PhysicalP2DataSet P2Set { get; set; }

        public PhysicalP3DataSet P3Set { get; set; }

        public PhysicalP40DataSet P40Set { get; set; }

        public PhysicalP41DataSet P41Set { get; set; }

        public PhysicalP42DataSet P42Set { get; set; }

        public PhysicalP44DataSet P44Set { get; set; }

        public PhysicalP45DataSet P45Set { get; set; }

        public PhysicalP5DataSet P5Set { get; set; }

        public PhysicalP6DataSet P6Set { get; set; }

        public PhysicalP7DataSet P7Set { get; set; }

        public ProfJobDataSet ProfJobSet { get; set; }

        public CtrlEditPhysicalUs CtrlUs { get; set; }

        public CtrlEditPhysicalEu CtrlEu { get; set; }

        private bool local;

        private ProductDataSet productSet;

        public PhysicalQuery(bool local = false)
        {
            this.local = local;

            if (local == true)
            {
                MainSet = new PhysicalMainDataSet(AppRes.DB.Connect, null, null);
                ImageSet = new PhysicalImageDataSet(AppRes.DB.Connect, null, null);
                P2Set = new PhysicalP2DataSet(AppRes.DB.Connect, null, null);
                P3Set = new PhysicalP3DataSet(AppRes.DB.Connect, null, null);
                P40Set = new PhysicalP40DataSet(AppRes.DB.Connect, null, null);
                P41Set = new PhysicalP41DataSet(AppRes.DB.Connect, null, null);
                P42Set = new PhysicalP42DataSet(AppRes.DB.Connect, null, null);
                P44Set = new PhysicalP44DataSet(AppRes.DB.Connect, null, null);
                P45Set = new PhysicalP45DataSet(AppRes.DB.Connect, null, null);
                P5Set = new PhysicalP5DataSet(AppRes.DB.Connect, null, null);
                P6Set = new PhysicalP6DataSet(AppRes.DB.Connect, null, null);
                P7Set = new PhysicalP7DataSet(AppRes.DB.Connect, null, null);
                ProfJobSet = new ProfJobDataSet(AppRes.DB.Connect, null, null);
                CtrlUs = null;
                CtrlEu = null;
            }
            else
            {
                productSet = new ProductDataSet(AppRes.DB.Connect, null, null);
            }
        }

        public void Insert(SqlTransaction trans = null)
        {
            EReportArea area = ProfJobSet.AreaNo;

            if (local == false)
            {
                trans = AppRes.DB.BeginTrans();
            }

            // ASTM, EN은 insert 안에서 분기처리하여 insert
            try
            {
                InsertMain(area, trans);
                InsertImage(trans);
                InsertPage2(area, trans);
                InsertPage3(area, trans);
                InsertPage4(area, trans);
                InsertPage5(area, trans);
                UpdateProductSet(trans);
                InsertPageComplete(area, trans);

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
                throw new Exception("Can't call PhysicalQuery.Update() method in Local transaction mode!");
            }

            EReportArea area = MainSet.AreaNo;
            SqlTransaction trans = AppRes.DB.BeginTrans();

            try
            {
                SaveMain(area, trans);
                SavePage2(area, trans);
                SavePage3(area, trans);
                SavePage4(area, trans);
                SavePage4_4(area, trans);
                SavePage4_5(area, trans);
                SavePage5(area, trans);
                SavePage6(area, trans);
                SavePageComplete(area, trans);

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
                throw new Exception("Can't call PhysicalQuery.Delete() method in Local transaction mode!");
            }

            string mainNo = MainSet.RecNo;
            SqlTransaction trans = AppRes.DB.BeginTrans();

            try
            {
                P2Set.MainNo = mainNo;
                P2Set.Delete(trans);
                P3Set.MainNo = mainNo;
                P3Set.Delete(trans);
                P40Set.MainNo = mainNo;
                P40Set.Delete(trans);
                P41Set.MainNo = mainNo;
                P41Set.Delete(trans);
                P42Set.MainNo = mainNo;
                P42Set.Delete(trans);
                P44Set.MainNo = mainNo;
                P44Set.Delete(trans);
                P45Set.MainNo = mainNo;
                P45Set.Delete(trans);
                P6Set.MainNo = mainNo;
                P6Set.Delete(trans);
                P5Set.MainNo = mainNo;
                P5Set.Delete(trans);
                P5Set.Delete_PHYRPTVIEW(trans);
                P5Set.Delete_PHYCOMPLETE(trans);
                P6Set.MainNo = mainNo;
                P6Set.Delete(trans);
                P7Set.MainNo = mainNo;
                P7Set.Delete(trans);
                ImageSet.RecNo = mainNo;
                ImageSet.Delete(trans);
                MainSet.Delete(trans);
                productSet.PhyJobNo = mainNo;
                productSet.UpdatePhyJobNoReset(trans);

                AppRes.DB.CommitTrans();
            }
            catch
            {
                AppRes.DB.RollbackTrans();
            }
        }

        public void Delete_ReportDesc() 
        {
            if (local == true)
            {
                throw new Exception("Can't call PhysicalQuery.Delete() method in Local transaction mode!");
            }

            SqlTransaction trans = AppRes.DB.BeginTrans();

            try
            {
                P2Set.sDepartment = "physical";
                P2Set.iReportPage = 1;
                P2Set.sReportCategory = "astm";
                P2Set.sColumnName = "detailofSample";
                P2Set.Delete_ReportDesc(trans);
                AppRes.DB.CommitTrans();
            }
            catch
            {
                AppRes.DB.RollbackTrans();
            }
        }

        private void InsertMain(EReportArea area, SqlTransaction trans)
        {
            MainSet.RecNo = ProfJobSet.JobNo;
            MainSet.RegTime = ProfJobSet.RegTime;
            MainSet.ReceivedTime = ProfJobSet.ReceivedTime;
            MainSet.RequiredTime = ProfJobSet.RequiredTime;
            MainSet.ReportedTime = ProfJobSet.ReportedTime;
            MainSet.Approval = false;
            MainSet.AreaNo = ProfJobSet.AreaNo;
            MainSet.StaffNo = ProfJobSet.StaffNo;
            MainSet.ProductNo = ProfJobSet.ItemNo;
            MainSet.P1ClientNo = ProfJobSet.ClientNo;
            MainSet.P1ClientName = ProfJobSet.ClientName;
            MainSet.P1ClientAddress = ProfJobSet.ClientAddress;
            MainSet.P1FileNo = ProfJobSet.FileNo;
            MainSet.P1SampleDescription = ProfJobSet.SampleRemark.ToUpper();
            MainSet.P1DetailOfSample = ProfJobSet.DetailOfSample;
            /*
            if (ProfJobSet.SampleRemark.Equals(""))
            {
                //MainSet.P1DetailOfSample = ProfJobSet.DetailOfSample;
                MainSet.P1DetailOfSample = "";
            }
            else 
            {
                MainSet.P1DetailOfSample = ProfJobSet.SampleRemark + " toy " + ProfJobSet.DetailOfSample;
            }
            */
            MainSet.P1ItemNo = ProfJobSet.ItemNo;
            MainSet.P1OrderNo = "-";
            MainSet.P1Packaging = "Yes, provided";
            MainSet.P1Instruction = "Not provided";
            MainSet.P1Buyer = "-";
            MainSet.P1Manufacturer = ProfJobSet.Manufacturer;
            MainSet.P1CountryOfOrigin = ProfJobSet.CountryOfOrigin;
            MainSet.P1CountryOfDestination = "-";
            MainSet.P1LabeledAge = "None";
            MainSet.P1TestAge = "None";
            MainSet.P1AssessedAge = "All ages";
            MainSet.P1ReceivedDate = ProfJobSet.ReceivedTime.ToString("yyyy. MM. dd");
            //MainSet.P1ReceivedDate = ProfJobSet.RequiredTime.ToString("yyyy. MM. dd");
            MainSet.P1TestPeriod = $"{ProfJobSet.ReceivedTime.ToString("yyyy. MM. dd")}  to  {ProfJobSet.RequiredTime.ToString("yyyy. MM. dd")}";
            MainSet.P1TestMethod = "For further details, please refer to following page(s)";
            MainSet.P1TestResults = "For further details, please refer to following page(s)";
            //MainSet.P1Comments = ProfJobSet.ReportComments;
            MainSet.P1Comments = "The results shown in this test report refer only to the sample(s) tested unless\r\n" +
                                 "otherwise stated.\r\n" +
                                 "This test report is not related to Korea Laboratory Accreditation Scheme.";
            MainSet.Approval = false;
            MainSet.P2Name = "";

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
                MainSet.P3Description1 = "As specified in ASTM F963-23 standard consumer safety specification on toys safety.";
                MainSet.P3Description2 =
                    //"N/A = Not Applicable                *Visual Examination\r\n" +
                    //"NT = Not tested as per client's request.\r\n\r\n" +
                    //"N.B. : - Only applicable clauses were shown";
                    "N/A = Not Applicable.       NT = Not tested as per client's request.       *Visual Examination\r\n\r\n" +
                    //"N.B. : - Only applicable clauses were shown";
                    "Only applicable clauses were shown";

                MainSet.P4Description1 = "Flammability (Clause 4.2)";
                MainSet.P4Description2 =
                    "*Burning rate has been rounded to the nearest one tenth of an inch per second.\r\n" +
                    "SE = Self-Extinguished\r\n" +
                    "DNI = Did Not Ignite\r\n" +
                    "NA = A major dimension of 1 in. (25 mm) or less.\r\n\r\n";
                //"Requirement:             A toy / component is considered a \"flammable solid\" if it ignites and burns with a self-sustaining \r\n\t\tflame at a rate greater than 0.1 in./s along its major axis.";
                MainSet.P4Description3 = "     A toy / component is considered a \"flammable solid\" if it ignites and burns with a self-sustaining \r\n" +
                                         "     flame at a rate greater than 0.1 in./s along its major axis.";
                MainSet.P4Description4 = "";

                MainSet.P5Description1 =
                    "Suffing Materials(Clause 4.3.7)";
                MainSet.P5Description2 = "Polyester fiber";
            }
            else
            {
                MainSet.P3Description1 =
                    "European Standard on Safety of Toys\r\n" +
                    "- Mechanical & Physical Properties\r\n" +
                    "As specified in European standard on safety of toys EN 71 Part 1:2014+A1:2018";
                MainSet.P3Description2 = "";

                MainSet.P4Description1 =
                    "- Flammability of Toys\r\n" +
                    "As specified in European standard on safety of toys EN71 PART 2: 2020";
                //MainSet.P4Description2 =
                //  "* Surface Flash of Pile Fabrics (Clause 4.1)";
                MainSet.P4Description2 =
                    "4.1 General requirement_Surface Flash of Pile Fabrics";
                MainSet.P4Description3 =
                    //"** Soft-filled toys(animals and doll, etc.) with a piled or textile surface (Clause 4.5)";
                    "** Soft-filled toys (Clause 4.5)";
                MainSet.P4Description4 =
                    "NSFO = No Surface Flash Occurred\r\n" +
                    //"DNI = Did not ignite\r\n" +
                    //"IBE = Ignite But Self-Extinguished\r\n" +
                    "SFO = Surface Flash Occurred\r\n";
                    //"N / A = Not applicable since the requirements of this sub - clause do not apply to toys with a greatest dimension of 150mm or less\r\n" +
                    //"SE = Self - Extinguishing\r\n\r\n\r\n" +
                    //"N.B. : Only applicable clauses were shown.";
                MainSet.P5Description1 =
                    "Labeling requirement (Washing/Cleaning Label, CE mark, importer / manufacturer mark (name, address), product identification) according to the Directive 2009/48/EC - Safety of toys";
                MainSet.P5Description2 =
                    /*
                    "1. According to Directive 2009/48/EC, a toy intended for use by children under 36 months must be designed and\r\n" +
                    "   manufactured in such a way that it can be cleaned. A textile toy must, to this end, be washable, except if it\r\n" +
                    "   contains a mechanism that may be damaged if soak washed. The manufacturer should, if applicable, provide\r\n" +
                    "   instructions on how the toy has to be cleaned.\r\n\r\n" +
                    */
                    "1. According to Directive 2009/48/EC, a toy intended for use by children under 36 months must be designed and manufactured in such a" +
                    " way that it can be cleaned. A textile toy must, to this end, be washable, except if it contains a mechanism that may be damaged if soak" +
                    " washed. The manufacturer should, if applicable, provide instructions on how the toy has to be cleaned. According to the GUIDANCE" +
                    " DOCUMENT ON THE APPLICATION OF DIRECTIVE 2009/48/EC ON THE SAFETY OF TOYS, manufacturer shall not label \"surface" +
                    " washing\" on textile toys which, under the TSD, need to be soak washable.\r\n\r\n" +
                    "2. CE marking should be visible from outside the packaging and its height must be at least 5 mm.\r\n\r\n" +
                    "3. Manufacturer's and Importer's name, registered trade name or registered trade mark and the address at which" +
                    " the manufacturer can be contacted must be indicated on the toy or, where that is not possible, on its packaging" +
                    " or in a document accompanying the toy.\r\n\r\n" +
                    "4. Manufacturers must ensure that their toys bear a type, batch, serial or model number or other element allowing" +
                    " their identification, or where the size or nature of the toy does not allow it, that the required information is" +
                    " provided on the packaging or in a document accompanying the toy.";
            }

            MainSet.Insert(trans);
        }

        private void InsertImage(SqlTransaction trans)
        {
            ImageSet.RecNo = MainSet.RecNo;
            ImageSet.Signature = null;
            ImageSet.Picture = ProfJobSet.Image;
            ImageSet.Insert(trans);
        }

        private void InsertPage2(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US)
            {
                P2Set.MainNo = MainSet.RecNo;
                P2Set.No = 0;
                P2Set.Line = false;
                P2Set.Requested = "US Public Law 110-314(Comsumer Product Safety Improvement Act of 2008, CPSIA):";
                P2Set.Conclusion = "-";
                P2Set.Insert(trans);

                P2Set.No = 1;
                P2Set.Line = false;
                P2Set.Requested = "- ASTM F963-17: Standard Consumer Safety Specification on Toy Safety\r\n  (Excluding clause 4.3.5 Heavy Element)";
                P2Set.Conclusion = "PASS";
                P2Set.Insert(trans);

                P2Set.No = 2;
                P2Set.Line = false;
                P2Set.Requested = "- Flammability of toys(16 C.F.R. 1500.44)";
                P2Set.Conclusion = "PASS";
                P2Set.Insert(trans);

                P2Set.No = 3;
                P2Set.Line = false;
                P2Set.Requested = "- Small part(16 C.F.R. 1501)";
                P2Set.Conclusion = "PASS";
                P2Set.Insert(trans);

                P2Set.No = 4;
                P2Set.Line = false;
                P2Set.Requested = "- Sharp points and edges(16 C.F.R. 1500.48 and 49)";
                P2Set.Conclusion = "PASS";
                P2Set.Insert(trans);
            }
            else
            {
                P2Set.MainNo = MainSet.RecNo;
                P2Set.No = 0;
                P2Set.Line = false;
                P2Set.Requested = "EN 71 Part 1:2014+A1:2018 - Mechanical and Physical Properties";
                P2Set.Conclusion = "PASS";
                P2Set.Insert(trans);

                P2Set.No = 1;
                P2Set.Line = false;
                P2Set.Requested = "EN 71 Part 2:2020 - Flammability of Toys";
                P2Set.Conclusion = "PASS";
                P2Set.Insert(trans);

                P2Set.No = 2;
                P2Set.Line = false;
                P2Set.Requested = "Labeling requirement (Washing/Cleaning Label, CE mark, importer / manufacturer mark (name, address), product identification) according to the Directive 2009/48/EC-Safety of toys";
                P2Set.Conclusion = "See note 1*";
                P2Set.Insert(trans);
            }
        }

        private void InsertPage3(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US)
            {
                P3Set.MainNo = MainSet.RecNo;
                P3Set.No = 0;
                P3Set.Line = false;
                P3Set.Clause = "4";
                P3Set.Description = "Safety Requirements";
                P3Set.Result = "-";
                P3Set.Comment = "";
                P3Set.Insert(trans);

                P3Set.No = 1;
                P3Set.Line = false;
                P3Set.Clause = " 4.1";
                P3Set.Description = "Material Quality**";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 2;
                P3Set.Line = false;
                P3Set.Clause = " 4.2";
                P3Set.Description = "Flammability Test(16 C.F.R. 1500.44)";
                P3Set.Result = "Pass(See Note 1)";
                P3Set.Insert(trans);

                P3Set.No = 3;
                P3Set.Line = false;
                P3Set.Clause = " 4.3";
                P3Set.Description = "Toxicology";
                P3Set.Result = "-";
                P3Set.Insert(trans);

                P3Set.No = 4;
                P3Set.Line = false;
                P3Set.Clause = " 4.3.5";
                P3Set.Description = "Heavy Elements";
                P3Set.Result = "NT";
                P3Set.Insert(trans);

                P3Set.No = 5;
                P3Set.Line = false;
                P3Set.Clause = "";
                P3Set.Description = "4.3.5.1 Heavy Elements in Paint/Similar Coating Materials";
                P3Set.Result = "";
                P3Set.Insert(trans);

                P3Set.No = 6;
                P3Set.Line = false;
                P3Set.Clause = "";
                P3Set.Description = "4.3.5.2 Heavy Metal in Substrate Materials";
                P3Set.Result = "";
                P3Set.Insert(trans);

                P3Set.No = 7;
                P3Set.Line = false;
                P3Set.Clause = " 4.3.7";
                P3Set.Description = "Styffing Materials";
                P3Set.Result = "Pass(See Note 2)";
                P3Set.Insert(trans);

                P3Set.No = 8;
                P3Set.Line = false;
                P3Set.Clause = " 4.6";
                P3Set.Description = "Small Objects";
                P3Set.Result = "-";
                P3Set.Insert(trans);

                P3Set.No = 9;
                P3Set.Line = false;
                P3Set.Clause = " 4.6.1";
                P3Set.Description = "Small Objects";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 10;
                P3Set.Line = false;
                P3Set.Clause = " 4.7";
                P3Set.Description = "Accessible Edges(16 C.F.R. 1500.49)";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 11;
                P3Set.Line = false;
                P3Set.Clause = " 4.9";
                P3Set.Description = "Accessible Points(16 C.F.R. 1500.48)";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 12;
                P3Set.Line = false;
                P3Set.Clause = " 4.14";
                P3Set.Description = "Cords, Straps and Elastic";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 13;
                P3Set.Line = true;
                P3Set.Clause = " 4.27";
                P3Set.Description = "Stuffed and Beanbag-Type Toys";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 14;
                P3Set.Line = false;
                P3Set.Clause = "5";
                P3Set.Description = "Safety Labeling Requirements";
                P3Set.Result = "-";
                P3Set.Insert(trans);

                P3Set.No = 15;
                P3Set.Line = true;
                P3Set.Clause = " 4.2";
                P3Set.Description = "Age Grading Labeling";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 16;
                P3Set.Line = false;
                P3Set.Clause = "7";
                P3Set.Description = "Producer's Markings";
                P3Set.Result = "-";
                P3Set.Insert(trans);

                P3Set.No = 17;
                P3Set.Line = true;
                P3Set.Clause = " 7.1";
                P3Set.Description = "Producer's Markings";
                P3Set.Result = "Present";
                P3Set.Insert(trans);

                P3Set.No = 18;
                P3Set.Line = false;
                P3Set.Clause = "8";
                P3Set.Description = "Test Methods";
                P3Set.Result = "-";
                P3Set.Insert(trans);

                P3Set.No = 19;
                P3Set.Line = false;
                P3Set.Clause = " 8.5";
                P3Set.Description = "Normal Use Testing";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 20;
                P3Set.Line = false;
                P3Set.Clause = " 8.7";
                P3Set.Description = "Impact Test";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 21;
                P3Set.Line = false;
                P3Set.Clause = " 8.8";
                P3Set.Description = "Torque Test";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 22;
                P3Set.Line = false;
                P3Set.Clause = " 8.9";
                P3Set.Description = "Tension Test";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 23;
                P3Set.Line = false;
                P3Set.Clause = " 8.23";
                P3Set.Description = "Test for Loops and Cords";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 24;
                P3Set.Line = true;
                P3Set.Clause = " 8.29";
                P3Set.Description = "Stuffing Materials Evaluation";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);
            }
            else
            {
                P3Set.MainNo = MainSet.RecNo;
                P3Set.No = 0;
                P3Set.Line = false;
                P3Set.Clause = "4";
                P3Set.Description = "General requirements";
                P3Set.Result = "-";
                P3Set.Comment = "";
                P3Set.Insert(trans);

                P3Set.No = 1;
                P3Set.Line = false;
                P3Set.Clause = " 4.1";
                P3Set.Description = "Material cleanliness";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 2;
                P3Set.Line = false;
                P3Set.Clause = " 4.7";
                P3Set.Description = "Edges";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 3;
                P3Set.Line = true;
                P3Set.Clause = " 4.8";
                P3Set.Description = "Points and metallic wires";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 4;
                P3Set.Line = false;
                P3Set.Clause = "5";
                P3Set.Description = "Toys intended for children under 36 months";
                P3Set.Result = "-";
                P3Set.Insert(trans);

                P3Set.No = 5;
                P3Set.Line = false;
                P3Set.Clause = " 5.1";
                P3Set.Description = "General requirements";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 6;
                P3Set.Line = false;
                P3Set.Clause = "                  5.1a";
                P3Set.Description = "Small part requirements on toys & removable components";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 7;
                P3Set.Line = false;
                P3Set.Clause = "";
                P3Set.Description = "     (Test method 8.2)";
                P3Set.Result = "-";
                P3Set.Insert(trans);

                P3Set.No = 8;
                P3Set.Line = false;
                P3Set.Clause = "                  5.1b";
                P3Set.Description = "Torque test(Test method 8.3)";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 9;
                P3Set.Line = false;
                P3Set.Clause = "";
                P3Set.Description = "     Tension test(Test method 8.4)";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 10;
                P3Set.Line = false;
                P3Set.Clause = "";
                P3Set.Description = "     Drop test(Test method 8.5)";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 11;
                P3Set.Line = false;
                P3Set.Clause = "";
                P3Set.Description = "     Impact test(Test method 8.7)";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 12;
                P3Set.Line = false;
                P3Set.Clause = "";
                P3Set.Description = "     Sharp edge(Test method 8.11)";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 13;
                P3Set.Line = false;
                P3Set.Clause = "";
                P3Set.Description = "     Sharp point(Test method 8.12)";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);

                P3Set.No = 14;
                P3Set.Line = false;
                P3Set.Clause = " 5.2";
                P3Set.Description = "Soft-filled toys and soft-filled parts of a toy";
                P3Set.Result = "Pass";
                P3Set.Insert(trans);
            }
        }

        private void InsertPage4(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US)
            {
                P41Set.MainNo = MainSet.RecNo;
                P41Set.No = 0;
                P41Set.Line = false;
                //P41Set.Sample = "Panda toy";
                if (ProfJobSet.SampleRemark.Equals(""))
                {
                    P41Set.Sample = "";
                }
                else
                {
                    P41Set.Sample = ProfJobSet.SampleRemark + " toy";
                }
                P41Set.BurningRate = "0.1*";
                P41Set.Result = "";
                P41Set.Insert(trans);
            }
            else
            {
                P40Set.MainNo = MainSet.RecNo;
                P40Set.No = 0;
                P40Set.Line = true;
                P40Set.Clause = "4.1";
                P40Set.Description = "General requirements";
                P40Set.Result = "Pass (See note *)";
                P40Set.Insert(trans);

                P40Set.No = 1;
                P40Set.Line = true;
                P40Set.Clause = "4.5";
                //P40Set.Description = "Soft - filled toys(animals and doll, etc.) with a piled or textile surface";
                P40Set.Description = "Soft-filled toys";
                P40Set.Result = "Pass (See note **)";
                P40Set.Insert(trans);

                if (ProfJobSet.SampleRemark.Equals(""))
                {
                    P41Set.Sample = "";
                    P42Set.Sample = "";
                }
                else
                {
                    P41Set.Sample = ProfJobSet.SampleRemark + " toy";
                    P42Set.Sample = ProfJobSet.SampleRemark + " toy";
                }

                P41Set.MainNo = MainSet.RecNo;
                P41Set.No = 0;
                P41Set.Line = false;
                //P41Set.Sample = "Santa mini toy";
                P41Set.BurningRate = "NSFO";    // burningRate는 Grid에서 Result로 출력된다.
                P41Set.Result = "";             // Result는 공백으로 처리.
                P41Set.Insert(trans);

                P42Set.MainNo = MainSet.RecNo;
                P42Set.No = 0;
                P42Set.Line = false;
                //P42Set.Sample = "Santa mini toy";
                //P42Set.BurningRate = "0";
                P42Set.BurningRate = "DNI";
                P42Set.Insert(trans);                
            }
        }

        private void InsertPage4_4(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US)
            {

            }
            else
            {
                P44Set.MainNo = MainSet.RecNo;
                P44Set.No = 0;
                P44Set.Line = false;
                P44Set.Sample = "Santa mini toy";
                P44Set.BurningRate = "DNI";
                P44Set.Note = "DNI = Did not Ignite\r\n" +
                               "FAIL: Exceed the limit\r\n\r\n" +
                               "N.B.: Only applicable clauses were shown.";
                P44Set.Insert(trans);
            }
        }

        private void InsertPage5(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US)
            {
                P5Set.MainNo = MainSet.RecNo;
                P5Set.No = 0;
                P5Set.Line = true;
                P5Set.TestItem =
                //    "   1. Objectionable matter originating from\r\n" +
                //    "      Insect, bird and rodent or other animal\r\n" +
                //    "      infestation";
                      "Objectionable matter originating from insect, bird, rodent, or other animal\r\n" +
                      "infestation and of contaminants, such as splinters, glass and metal chips";
                P5Set.Result = "Absent";
                P5Set.Requirement = "Absent";
                P5Set.Insert(trans);

                P5Set.No = 1;
                P5Set.Line = false;
                P5Set.TestItem = "Comment";
                P5Set.Result = "PASS";
                P5Set.Requirement = "-";
                P5Set.Insert(trans);

                P5Set.iReportView = 1;
                P5Set.Insert_ReportView(trans);
            }
            else
            {
                P5Set.MainNo = MainSet.RecNo;
                P5Set.No = 0;
                P5Set.Line = true;
                P5Set.TestItem = "Washing/Cleaning instruction";
                P5Set.Result = "Present";
                P5Set.Requirement = "Affixed label and Hangtag";
                P5Set.Insert(trans);

                P5Set.No = 1;
                P5Set.Line = true;
                P5Set.TestItem = "CE mark";
                P5Set.Result = "Present";
                P5Set.Requirement = "Affixed label and Hangtag";
                P5Set.Insert(trans);

                P5Set.No = 2;
                P5Set.Line = true;
                P5Set.TestItem = "Importer's Name & Address";
                P5Set.Result = "Present";
                P5Set.Requirement = "Affixed label and Hangtag";
                P5Set.Insert(trans);

                P5Set.No = 3;
                P5Set.Line = true;
                P5Set.TestItem = "Manufacturer's Name & Address";
                P5Set.Result = "Present";
                P5Set.Requirement = "Affixed label and Hangtag";
                P5Set.Insert(trans);

                P5Set.No = 4;
                P5Set.Line = true;
                P5Set.TestItem = "Product ID";
                P5Set.Result = "Present";
                P5Set.Requirement = "Affixed label and Hangtag";
                P5Set.Insert(trans);
            }
        }

        private void InsertPageComplete(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US)
            {
                P5Set.MainNo = MainSet.RecNo;
                P5Set.sPhyComplete = "0";
                P5Set.sPhyComplete_GiSool = "0";

                P5Set.Insert_PhyComplete(trans);
            }
            else
            {
                P5Set.MainNo = MainSet.RecNo;
                P5Set.sPhyComplete = "0";
                P5Set.sPhyComplete_GiSool = "0";

                P5Set.Insert_PhyComplete(trans);
            }
        }

        private void UpdateProductSet(SqlTransaction trans)
        {
            if (local == true) return;

            productSet.PhyJobNo = ProfJobSet.JobNo;
            productSet.Code = ProfJobSet.ItemNo;
            productSet.UpdatePhyJobNoSet(trans);
        }

        private void SaveMain(EReportArea area, SqlTransaction trans)
        {
            if (area == EReportArea.US)
                CtrlUs.SetControlToDataSet();
            else
                CtrlEu.SetControlToDataSet();

            MainSet.Update(trans);
        }

        private void SavePage2(EReportArea area, SqlTransaction trans)
        {
            List<PhysicalPage2Row> rows = (area == EReportArea.US) ? CtrlUs.P2Rows : CtrlEu.P2Rows;

            P2Set.MainNo = MainSet.RecNo;
            P2Set.Delete(trans);

            foreach (PhysicalPage2Row row in rows)
            {
                P2Set.No = row.No;
                P2Set.Line = row.Line;
                P2Set.Requested = row.Requested;
                P2Set.Conclusion = row.Conclusion;
                P2Set.Insert(trans);
            }
        }

        private void SavePage3(EReportArea area, SqlTransaction trans)
        {
            List<PhysicalPage3Row> rows = (area == EReportArea.US) ? CtrlUs.P3Rows : CtrlEu.P3Rows;

            P3Set.MainNo = MainSet.RecNo;
            P3Set.Delete(trans);

            foreach (PhysicalPage3Row row in rows)
            {
                P3Set.No = row.No;
                P3Set.Line = row.Line;
                P3Set.Clause = row.Clause;
                P3Set.Description = row.Description;
                P3Set.Result = row.Result;
                P3Set.Comment = row.Comment;
                P3Set.Insert(trans);
            }
        }

        private void SavePage4(EReportArea area, SqlTransaction trans)
        {
            List<PhysicalPage3Row> p40Rows = null;
            List<PhysicalPage4Row> p41Rows = null;
            List<PhysicalPage4Row> p42Rows = null;

            switch (area)

            {
                case EReportArea.US:
                    p40Rows = null;
                    p41Rows = CtrlUs.P4Rows;
                    p42Rows = null;
                    break;

                case EReportArea.EU:
                    p40Rows = CtrlEu.P40Rows;
                    p41Rows = CtrlEu.P41Rows;
                    p42Rows = CtrlEu.P42Rows;
                    break;
            }

            if (p40Rows != null)
            {
                P40Set.MainNo = MainSet.RecNo;
                P40Set.Delete(trans);

                foreach (PhysicalPage3Row row in p40Rows)
                {
                    P40Set.No = row.No;
                    P40Set.Line = row.Line;
                    P40Set.Clause = row.Clause;
                    P40Set.Description = row.Description;
                    P40Set.Result = row.Result;
                    P40Set.Insert(trans);
                }
            }

            P41Set.MainNo = MainSet.RecNo;
            P41Set.Delete(trans);

            if (p41Rows.Count > 1)
            {
                foreach (PhysicalPage4Row row in p41Rows)
                {
                    P41Set.No = row.No;
                    P41Set.Line = row.Line;
                    P41Set.Sample = row.Sample;
                    P41Set.BurningRate = row.BurningRate;
                    P41Set.Insert(trans);
                }
            }
            else if (p41Rows.Count == 1)
            {
                foreach (PhysicalPage4Row row in p41Rows)
                {
                    P41Set.No = row.No;
                    P41Set.Line = row.Line;
                    P41Set.Result = row.Result;
                    if (String.IsNullOrEmpty(MainSet.P1SampleDescription))
                    {
                        P41Set.Sample = row.Sample;
                    }
                    else 
                    {
                        P41Set.Sample = MainSet.P1SampleDescription + " toy";
                    }
                    P41Set.BurningRate = row.BurningRate;
                    P41Set.Insert(trans);
                }
            }
            else 
            {
                Console.WriteLine("Save, PhysicalPage4Row-Astm, Else Case!");
            }

            if (p42Rows != null)
            {
                P42Set.MainNo = MainSet.RecNo;
                P42Set.Delete(trans);

                if (p42Rows.Count > 1)
                {
                    foreach (PhysicalPage4Row row in p42Rows)
                    {
                        if ((string.IsNullOrWhiteSpace(row.Sample) == false) ||
                            (string.IsNullOrWhiteSpace(row.BurningRate) == false))
                        {
                            P42Set.No = row.No;
                            P42Set.Line = row.Line;
                            P42Set.Sample = row.Sample;
                            P42Set.BurningRate = row.BurningRate;
                            P42Set.Insert(trans);
                        }
                    }
                }
                else if (p42Rows.Count == 1)
                {
                    foreach (PhysicalPage4Row row in p42Rows)
                    {
                        if ((string.IsNullOrWhiteSpace(row.Sample) == false) ||
                            (string.IsNullOrWhiteSpace(row.BurningRate) == false))
                        {
                            P42Set.No = row.No;
                            P42Set.Line = row.Line;
                            if (String.IsNullOrEmpty(MainSet.P1SampleDescription))
                            {
                                P42Set.Sample = row.Sample;
                            }
                            else
                            {
                                P42Set.Sample = MainSet.P1SampleDescription + " toy";
                            }
                            //P42Set.Sample = row.Sample;
                            P42Set.BurningRate = row.BurningRate;
                            P42Set.Insert(trans);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Save, PhysicalPage4Row-EN, Else Case!");
                }
            }            

            /*
            if (p42Rows != null)
            {
                P42Set.MainNo = MainSet.RecNo;
                P42Set.Delete(trans);

                foreach (PhysicalPage4Row row in p42Rows)
                {
                    if ((string.IsNullOrWhiteSpace(row.Sample) == false) ||
                        (string.IsNullOrWhiteSpace(row.BurningRate) == false))
                    {
                        P42Set.No = row.No;
                        P42Set.Line = row.Line;
                        P42Set.Sample = row.Sample;
                        P42Set.BurningRate = row.BurningRate;
                        P42Set.Insert(trans);
                    }
                }
            }
            */
        }

        private void SavePage4_4(EReportArea area, SqlTransaction trans)
        {            
            List<PhysicalPage4_4_4_2_2_4Row> p4_4_4_2_2_2Row = null;
            List<PhysicalPage4_4_4_2_2_4Row> p4_4_4_2_2_3Row = null;
            List<PhysicalPage4_4_4_2_2_4Row> p4_4_4_2_2_4Row = null;

            switch (area)
            {
                case EReportArea.US:
                    //p40Rows = null;
                    //p41Rows = CtrlUs.P4Rows;
                    //p42Rows = null;
                    break;

                case EReportArea.EU:
                    p4_4_4_2_2_2Row = CtrlEu.P422Rows;
                    p4_4_4_2_2_3Row = CtrlEu.P423Rows;
                    p4_4_4_2_2_4Row = CtrlEu.P423Rows;
                    break;
            }

            if (area == EReportArea.EU) 
            {
                P44Set.MainNo = MainSet.RecNo;
                P44Set.Delete(trans);

                if (p4_4_4_2_2_2Row != null)
                {
                    foreach (PhysicalPage4_4_4_2_2_4Row row in p4_4_4_2_2_2Row)
                    {
                        P44Set.No = row.No;
                        P44Set.Line = row.Line;
                        P44Set.Sample = row.Sample;
                        P44Set.BurningRate = row.BurningRate;
                        P44Set.Note = row.Note;
                        P44Set.Insert(trans);
                    }
                }

                if (p4_4_4_2_2_3Row != null)
                {
                    foreach (PhysicalPage4_4_4_2_2_4Row row in p4_4_4_2_2_3Row)
                    {
                        P44Set.No = row.No;
                        P44Set.Line = row.Line;
                        P44Set.Sample = row.Sample;
                        P44Set.BurningRate = row.BurningRate;
                        P44Set.Note = row.Note;
                        P44Set.Insert(trans);
                    }
                }

                if (p4_4_4_2_2_4Row != null)
                {
                    foreach (PhysicalPage4_4_4_2_2_4Row row in p4_4_4_2_2_4Row)
                    {
                        P44Set.No = row.No;
                        P44Set.Line = row.Line;
                        P44Set.Sample = row.Sample;
                        P44Set.BurningRate = row.BurningRate;
                        P44Set.Note = row.Note;
                        P44Set.Insert(trans);
                    }
                }
            }
        }

        private void SavePage4_5(EReportArea area, SqlTransaction trans)
        {
            List<PhysicalPage4_5_4Row> p4_5_425Row = null;
            List<PhysicalPage4_5_4Row> p4_5_43Row = null;
            List<PhysicalPage4_5_4Row> p4_5_44Row = null;
            List<PhysicalPage4_5_4Row> p4_5_45Row = null;

            switch (area)
            {
                case EReportArea.US:
                    //p40Rows = null;
                    //p41Rows = CtrlUs.P4Rows;
                    //p42Rows = null;
                    break;

                case EReportArea.EU:
                    p4_5_425Row = CtrlEu.P45_425Rows;
                    p4_5_43Row = CtrlEu.P45_43Rows;
                    p4_5_44Row = CtrlEu.P45_44Rows;
                    p4_5_45Row = CtrlEu.P45_45Rows;
                    break;
            }

            if (area == EReportArea.EU)
            {
                P45Set.MainNo = MainSet.RecNo;
                P45Set.Delete(trans);

                if (p4_5_425Row != null)
                {
                    foreach (PhysicalPage4_5_4Row row in p4_5_425Row)
                    {
                        P45Set.No = row.No;
                        P45Set.Line = row.Line;
                        P45Set.Sample = row.Sample;
                        P45Set.Desc = row.Desc;
                        P45Set.Clause = row.Clause;
                        P45Set.BurningRate = row.BurningRate;
                        P45Set.Note = row.Note;
                        P45Set.Insert(trans);
                    }
                }

                if (p4_5_43Row != null)
                {
                    foreach (PhysicalPage4_5_4Row row in p4_5_43Row)
                    {
                        P45Set.No = row.No;
                        P45Set.Line = row.Line;
                        P45Set.Sample = row.Sample;
                        P45Set.Desc = row.Desc;
                        P45Set.Clause = row.Clause;
                        P45Set.BurningRate = row.BurningRate;
                        P45Set.Note = row.Note;
                        P45Set.Insert(trans);
                    }
                }

                if (p4_5_44Row != null)
                {
                    foreach (PhysicalPage4_5_4Row row in p4_5_44Row)
                    {
                        P45Set.No = row.No;
                        P45Set.Line = row.Line;
                        P45Set.Sample = row.Sample;
                        P45Set.Desc = row.Desc;
                        P45Set.Clause = row.Clause;
                        P45Set.BurningRate = row.BurningRate;
                        P45Set.Note = row.Note;
                        P45Set.Insert(trans);
                    }
                }

                if (p4_5_45Row != null)
                {
                    foreach (PhysicalPage4_5_4Row row in p4_5_45Row)
                    {
                        P45Set.No = row.No;
                        P45Set.Line = row.Line;
                        P45Set.Sample = row.Sample;
                        P45Set.Desc = row.Desc;
                        P45Set.Clause = row.Clause;
                        P45Set.BurningRate = row.BurningRate;
                        P45Set.Note = row.Note;
                        P45Set.Insert(trans);
                    }
                }
            }
        }

        private void SavePage5(EReportArea area, SqlTransaction trans)
        {
            List<PhysicalPage5Row> rows = (area == EReportArea.US) ? CtrlUs.P5Rows : CtrlEu.P5Rows;
            
            P5Set.MainNo = MainSet.RecNo;
            P5Set.Delete(trans);
            P5Set.Delete_PHYRPTVIEW(trans);

            foreach (PhysicalPage5Row row in rows)
            {
                P5Set.No = row.No;
                P5Set.Line = row.Line;
                P5Set.TestItem = row.TestItem;
                P5Set.Result = row.Result;
                P5Set.Requirement = row.Requirement;
                P5Set.Insert(trans);
            }

            CtrlUs.SetReportView();
            P5Set.sPage = "5_Note1";
            P5Set.Insert_ReportView(trans);
        }

        private void SavePage6(EReportArea area, SqlTransaction trans)
        {
            List<PhysicalPage6Row> rows = (area == EReportArea.EU) ? CtrlEu.P6Rows : null;

            if (area == EReportArea.EU)
            {
                P6Set.MainNo = MainSet.RecNo;
                P6Set.Delete(trans);

                foreach (PhysicalPage6Row row in rows)
                {
                    P6Set.No = row.No;
                    P6Set.Line = row.Line;
                    P6Set.TestItem = row.TestItem;
                    P6Set.Result = row.Result;
                    P6Set.Requirement = row.Requirement;
                    P6Set.Note = row.Note;
                    P6Set.Description = row.Description;
                    P6Set.Insert(trans);
                }
            }
        }

        private void SavePageComplete(EReportArea area, SqlTransaction trans) 
        {
            P5Set.MainNo = MainSet.RecNo;
            P5Set.Delete_PHYCOMPLETE(trans);

            if (area == EReportArea.US)
            {
                CtrlUs.SetPhyComplete();
            }
            else
            {
                CtrlEu.SetPhyComplete();
            }
            P5Set.Insert_PhyComplete(trans);
        }
    }
}