using DevExpress.XtraSpreadsheet.PrintLayoutEngine;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;

namespace Sgs.ReportIntegration
{
    public class ChemicalQuery
    {
        public ChemicalMainDataSet MainSet { get; set; }

        public ChemicalImageDataSet ImageSet { get; set; }

        public ChemicalItemJoinDataSet JoinSet { get; set; }

        public ChemicalP2DataSet P2Set { get; set; }

        public ChemicalP2ExtendDataSet P2ExtendSet { get; set; }

        public ProfJobDataSet ProfJobSet { get; set; }

        public ProfJobSchemeDataSet ProfJobSchemeSet { get; set; }

        public CtrlEditChemicalUs CtrlUs { get; set; }

        public CtrlEditChemicalEu CtrlEu { get; set; }

        private bool local;

        private ProductDataSet productSet;

        private PartDataSet partSet;

        public ChemicalQuery(bool local = false)
        {
            this.local = local;

            if (local == true)
            {
                MainSet = new ChemicalMainDataSet(AppRes.DB.Connect, null, null);
                ImageSet = new ChemicalImageDataSet(AppRes.DB.Connect, null, null);
                JoinSet = new ChemicalItemJoinDataSet(AppRes.DB.Connect, null, null);
                P2Set = new ChemicalP2DataSet(AppRes.DB.Connect, null, null);
                P2ExtendSet = new ChemicalP2ExtendDataSet(AppRes.DB.Connect, null, null);
                ProfJobSet = new ProfJobDataSet(AppRes.DB.Connect, null, null);
                ProfJobSchemeSet = new ProfJobSchemeDataSet(AppRes.DB.Connect, null, null);
                CtrlUs = null;
                CtrlEu = null;
            }

            productSet = new ProductDataSet(AppRes.DB.Connect, null, null);
            partSet = new PartDataSet(AppRes.DB.Connect, null, null);
        }

        public void Insert(EReportArea AreaNo, string extendJobNo, SqlTransaction trans = null)
        {
            string sSchemCode = "";
            string sSubJobNo = "";
            int iChemRowCount = 0;
            int iChemSubJobRowCount = 0;
            List<string> sListSchem = new List<string>();
            List<string> sListSubJobNo = new List<string>();

            //string val = sListSchem[0];

            EReportArea area = AreaNo;

            if (local == false)
            {
                trans = AppRes.DB.BeginTrans();
            }

            try
            {
                //ProfJobSchemeSet.JobNo = extendJobNo;
                ProfJobSchemeSet.ProjJobNo = extendJobNo;

                //if (ProfJobSchemeSet.ProjJobNo.Contains("AYHA21-06897"))
                //if (ProfJobSchemeSet.ProjJobNo.Contains("AYHA21-17276") || ProfJobSchemeSet.ProjJobNo.Contains("AYHA21-11157"))
               // {
                //   string test = "";
                //}

                //ProfJobSchemeSet.SelectDistinct_Aurora(trans);
                ProfJobSchemeSet.SelectDistinctProjJob_Aurora(trans);
                //ProfJobSchemeSet.Fetch();
                iChemRowCount = ProfJobSchemeSet.RowCount;

                //MainSet.RecNo = ProfJobSchemeSet.SAMPLEIDENT;
                //MainSet.SelectRecno(trans);

                if (iChemRowCount == 1)
                {
                    ProfJobSchemeSet.SelectDistinctProjJob_Aurora(trans);
                    ProfJobSchemeSet.Fetch(0);

                    MainSet.RecNo = ProfJobSchemeSet.SAMPLEIDENT;
                    MainSet.SelectRecno(trans);
                    
                    //ProfJobSchemeSet.SelectSampleident(trans);
                    //ProfJobSchemeSet.SelectSampleident_Aurora(trans);
                    ProfJobSchemeSet.SelectSampleidentProj_Aurora(trans);
                    ProfJobSchemeSet.Fetch(0, 0, "no", "substrate");

                    //InsertPage2Extend(area, extendJobNo, ProfJobSchemeSet.SAMPLEIDENT, "substrate", trans);
                    if (MainSet.Empty == true)
                    {
                        InsertMain(area, extendJobNo, ProfJobSchemeSet.SAMPLEIDENT, trans);
                        InsertJoin(trans);
                    }
                    InsertImage(trans);
                    InsertPage2(area, trans);
                }
                else if (iChemRowCount > 1)
                {
                    // Count SubJob
                    ProfJobSchemeSet.SelectDistinctSubProjJob_Aurora(trans);
                    iChemSubJobRowCount = ProfJobSchemeSet.RowCount;

                    // Insert SubJob
                    for (int i = 0; i < iChemSubJobRowCount; i++)
                    {
                        ProfJobSchemeSet.SelectDistinctSubProjJob_Aurora(trans);
                        ProfJobSchemeSet.Fetch(i, 0, "check", "");
                        sSchemCode = ProfJobSchemeSet.Sch_Code;
                        sSubJobNo = ProfJobSchemeSet.SAMPLEIDENT;
                        sListSchem.Add(sSchemCode);
                        sListSubJobNo.Add(sSubJobNo);

                        MainSet.RecNo = ProfJobSchemeSet.SAMPLEIDENT;
                        MainSet.SelectRecno(trans);

                        if (sSchemCode.Substring(0, 8) == "HCEECPSC")
                        {
                            if (sSchemCode.Equals("HCEECPSC07"))
                            {
                                InsertPage2Extend(area, extendJobNo, ProfJobSchemeSet.SAMPLEIDENT, "substrate", trans);
                            }
                            else if (sSchemCode.Equals("HCEECPSC08"))
                            {
                                InsertPage2Extend(area, extendJobNo, ProfJobSchemeSet.SAMPLEIDENT, "substrate", trans);
                            }
                            else if (sSchemCode.Equals("HCEECPSC09"))
                            {
                                InsertPage2Extend(area, extendJobNo, ProfJobSchemeSet.SAMPLEIDENT, "surface", trans);
                            }
                            else
                            {
                                InsertPage2Extend(area, extendJobNo, ProfJobSchemeSet.SAMPLEIDENT, "substrate", trans);
                            }
                        }
                        
                        if (MainSet.Empty == true)
                        {
                            InsertMain(area, extendJobNo, ProfJobSchemeSet.SAMPLEIDENT, trans);
                            InsertJoin(trans);
                        }
                        /*
                        InsertImage(trans);
                        InsertPage2(area, trans);
                        */
                    }

                    // Insert MainJob - Matching Job
                    for (int j = 0; j < iChemSubJobRowCount; j++)
                    {
                        ProfJobSchemeSet.SelectDistinctMainProjJob_Aurora(j + 1, trans);
                        ProfJobSchemeSet.Fetch(0, 0, "check", "");

                        MainSet.RecNo = ProfJobSchemeSet.SAMPLEIDENT;
                        MainSet.SelectRecno(trans);

                        /*
                        if (ProfJobSchemeSet.SAMPLEIDENT.Contains("AYN21-051339")) 
                        {
                            string test = "";
                        }
                        */

                        if (sListSchem[j].Substring(0, 8) == "HCEECPSC")
                        {
                            if (sListSchem[j].Equals("HCEECPSC07"))
                            {
                                UpdatePage2Extend(sListSubJobNo[j], ProfJobSchemeSet.SAMPLEIDENT, trans);
                                ProfJobSchemeSet.SelectSampleidentProj_Aurora(trans);
                                ProfJobSchemeSet.Fetch(0, 0, "no", "substrate");
                            }
                            else if (sListSchem[j].Equals("HCEECPSC08"))
                            {
                                UpdatePage2Extend(sListSubJobNo[j], ProfJobSchemeSet.SAMPLEIDENT, trans);
                                ProfJobSchemeSet.SelectSampleidentProj_Aurora(trans);
                                ProfJobSchemeSet.Fetch(0, 0, "no", "substrate");
                            }
                            else if (sListSchem[j].Equals("HCEECPSC09"))
                            {
                                UpdatePage2Extend(sListSubJobNo[j], ProfJobSchemeSet.SAMPLEIDENT, trans);
                                ProfJobSchemeSet.SelectSampleidentProj_Aurora(trans);
                                ProfJobSchemeSet.Fetch(0, 0, "no", "surface");
                            }
                            else
                            {
                                UpdatePage2Extend(sListSubJobNo[j], ProfJobSchemeSet.SAMPLEIDENT, trans);
                                ProfJobSchemeSet.SelectSampleidentProj_Aurora(trans);
                                ProfJobSchemeSet.Fetch(0, 0, "no", "substrate");
                            }
                        }
                        if (MainSet.Empty == true)
                        {
                            ProfJobSet.JobNo = ProfJobSchemeSet.SAMPLEIDENT;
                            ProfJobSet.Select_Distinct_Sampleident_Profjob_Aurora(trans);
                            ProfJobSet.Fetch(j);

                            InsertMain(area, extendJobNo, ProfJobSchemeSet.SAMPLEIDENT, trans);
                            InsertJoin(trans);
                        }
                        InsertImage(trans);
                        InsertPage2(area, trans);
                    }

                    // Insert Another MainJob - Non Matching Job
                    for (int k = 0; k < iChemRowCount - (iChemSubJobRowCount * 2); k++)
                    {
                        //ProfJobSchemeSet.SelectDistinctMainProjJob_Aurora(k + iChemSubJobRowCount, trans);
                        //ProfJobSchemeSet.Fetch(0, 0, "check", "");
                        ProfJobSchemeSet.SelectSampleidentProj_Aurora(k + 1,trans);
                        ProfJobSchemeSet.Fetch(0, 0, "no", "substrate");

                        MainSet.RecNo = ProfJobSchemeSet.SAMPLEIDENT;
                        MainSet.SelectRecno(trans);

                        //InsertPage2Extend(area, extendJobNo, ProfJobSchemeSet.SAMPLEIDENT, "substrate", trans);
                        if (MainSet.Empty == true)
                        {
                            InsertMain(area, extendJobNo, ProfJobSchemeSet.SAMPLEIDENT, trans);
                            InsertJoin(trans);
                        }
                        InsertImage(trans);
                        InsertPage2(area, trans);
                    }
                }
                // Else Case - Nothing
                else
                {
                    Console.WriteLine("Chemical Insert : " + extendJobNo + " - Else Case!");
                }

                if (local == false)
                {
                    SetReportValidation(trans);
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
                throw new Exception("Can't call ChemicalQuery.Update() method in Local transaction mode!");
            }

            EReportArea area = MainSet.AreaNo;
            SqlTransaction trans = AppRes.DB.BeginTrans();

            try
            {
                SaveMain(area, trans);
                SavePage2(area, trans);

                AppRes.DB.CommitTrans();
            }
            catch
            {
                AppRes.DB.RollbackTrans();
            }
        }

        public void Delete(SqlTransaction trans = null)
        {
            if (local == true)
            {
                throw new Exception("Can't call ChemicalQuery.Delete() method in Local transaction mode!");
            }

            string mainNo = MainSet.RecNo;
            trans = AppRes.DB.BeginTrans();

            try
            {
                ImageSet.RecNo = mainNo;
                ImageSet.Delete(trans);
                JoinSet.RecNo = mainNo;
                JoinSet.Delete(trans);
                P2Set.MainNo = mainNo;
                P2Set.Delete(trans);
                P2ExtendSet.RecNo = mainNo;
                P2ExtendSet.Delete(trans);
                MainSet.Delete(trans);
                ResetReportValidation(trans);

                AppRes.DB.CommitTrans();
            }
            catch
            {
                AppRes.DB.RollbackTrans();
            }
        }

        private void InsertMain(EReportArea area, string extendJobNo, string sampleIdent, SqlTransaction trans)
        {
            //MainSet.RecNo = ProfJobSet.JobNo;
            MainSet.RecNo = ProfJobSchemeSet.SAMPLEIDENT;
            MainSet.RegTime = ProfJobSet.RegTime;
            MainSet.ReceivedTime = ProfJobSet.ReceivedTime;
            MainSet.RequiredTime = ProfJobSet.RequiredTime;
            MainSet.ReportedTime = ProfJobSet.ReportedTime;
            MainSet.Approval = (string.IsNullOrWhiteSpace(ProfJobSet.StaffNo) == true) ? false : true;
            MainSet.LeadType = ProfJobSchemeSet.Lead;
            //MainSet.AreaNo = ProfJobSet.AreaNo;
            MainSet.AreaNo = area;
            MainSet.StaffNo = ProfJobSet.StaffNo;
            MainSet.MaterialNo = "";
            MainSet.P1ClientNo = ProfJobSet.ClientNo;
            MainSet.P1ClientName = ProfJobSet.ClientName;
            MainSet.P1ClientAddress = ProfJobSet.ClientAddress;
            MainSet.P1FileNo = ProfJobSet.FileNo;
            MainSet.P1SampleDescription = ProfJobSet.SampleRemark;
            MainSet.P1ItemNo = ProfJobSet.ItemNo;
            MainSet.P1OrderNo = "-";
            MainSet.P1Manufacturer = ProfJobSet.Manufacturer;
            MainSet.P1CountryOfOrigin = ProfJobSet.CountryOfOrigin;
            MainSet.P1CountryOfDestination = "-";
            MainSet.P1ReceivedDate = $"{ProfJobSet.ReceivedTime:yyyy. MM. dd}";
            MainSet.P1TestPeriod = $"{ProfJobSet.ReceivedTime:yyyy. MM. dd}  to  {ProfJobSet.RequiredTime:yyyy. MM. dd}";
            MainSet.P1TestMethod = "For further details, please refer to following page(s)";
            MainSet.P1TestResults = "For further details, please refer to following page(s)";
            //MainSet.P1Comments = ProfJobSet.ReportComments;
            MainSet.P1Comments = "The results shown in this test report refer only to the sample(s) tested unless\r\n" +
                                 "otherwise stated.\r\n" +
                                 "This test report is not related to Korea Laboratory Accreditation Scheme.";
            MainSet.P1Name = "";

            if (area == EReportArea.US)
            {
                MainSet.P1TestRequested =
                    "Selected test(s) as requested by applicant for compliance with Public Law 110-314(Consumer Product Safety Improvement Act of 2008, CPSIA):-\r\n" +
                    "- To determine Heavy Elements in the submitted samples with reference to ASTM F963-16\r\n" +
                    "    4.3.5.2-Heavy Metal in Substrate Materials";
                MainSet.P1Conclusion = "\r\n\r\n-\r\nPASS";
                MainSet.P2Description2 = "Method: With reference to ASTM F963-16 Clause 8.3. Analysis was performed by ICP-OES.";
                MainSet.P2Description3 =
                    "1. Black textile\r\n\r\n" +
                    "Note:    -   Soluble results shown are of the adjusted analytical result.\r\n" +
                    "         -   ND = Not Detected(<MDL)";

                if (string.IsNullOrWhiteSpace(extendJobNo) == false)
                {
                    if  (MainSet.LeadType == ELeadType.Substrate)
                    {
                        MainSet.P2Description1 = "ASTM F963-16, Clause 4.3.5.2 - Heavy Elements in Toys Substrate Materials";
                    }
                    else
                    {
                        MainSet.P2Description1 = "ASTM F963-17, Clause 4.3.5.1 - Heavy Elements in Paint/Similar Coating Materials";
                    }

                    MainSet.P2Description4 =
                        "Method(non-metallic materials): CPSC-CH-E1002-08.3 - Standard Operation Procedure for Determining Total Lead(Pb) in Non-Metal Children Product. Analysis was performed by ICP-OES.";
                }
                else
                {
                    MainSet.P2Description1 = "";
                    MainSet.P2Description4 = "";
                }

                MainSet.P3Description1 = "";
            }
            else
            {
                MainSet.P1TestRequested =
                    "EN71-3:2013+A3:2018-Migration of certain elements\r\n" +
                    "(By first action method testing only)";
                MainSet.P1Conclusion = "PASS";
                MainSet.P2Description1 = "EN71-3:2013+A3:2018 - Migration of certain elements";
                MainSet.P2Description2 = "Method : With reference to EN71-3:2013+A3:2018. Analysis of general elements was performed by ICP-OES.";
                MainSet.P2Description3 = ProfJobSet.SampleDescription;
                MainSet.P2Description4 = "";
                MainSet.P3Description1 =
                    "Note. 1. mg/kg = milligram per kilogram\r\n" +
                    "      2. ND = Not Detected(< MDL)\r\n" +
                    "      3. 1% = 10000 mg/kg = 10000 ppm\r\n" +
                    "      4. Soluble Chromium(III) = Soluble Total Chromium – Soluble Chromium(VI)\r\n" +
                    "      5. ^ = Confirmation test of soluble organic tin is not required in case of soluble tin, after conversion, does not exceed the soluble organic tin requirement as specified in EN71-3:2019.";
            }

            MainSet.Insert(trans);
        }

        private void InsertJoin(SqlTransaction trans)
        {
            string[] items = MainSet.P1ItemNo.Split(',');

            //JoinSet.RecNo = MainSet.RecNo;
            JoinSet.RecNo = ProfJobSchemeSet.SAMPLEIDENT;
            foreach (string item in items)
            {
                JoinSet.PartNo = item.Trim();
                JoinSet.Insert(trans);
            }
        }

        private void InsertImage(SqlTransaction trans)
        {
            ImageSet.RecNo = ProfJobSchemeSet.SAMPLEIDENT;
            ImageSet.Select(trans);

            if (ImageSet.Empty == true) 
            {
                ImageSet.RecNo = ProfJobSchemeSet.SAMPLEIDENT;
                ImageSet.Signature = null;
                ImageSet.Picture = ProfJobSet.Image;
                ImageSet.Insert(trans);
            }
        }

        private void InsertPage2(EReportArea area, SqlTransaction trans)
        {
            P2Set.MainNo = ProfJobSchemeSet.SAMPLEIDENT;
            P2Set.Select(trans);

            if (P2Set.Empty == true)
            {
                for (int i = 0; i < ProfJobSchemeSet.RowCount; i++)
                {
                    ProfJobSchemeSet.Fetch(i);

                    if (i == 0)
                    {
                        P2Set.MainNo = ProfJobSchemeSet.SAMPLEIDENT;
                        P2Set.Name = "Mass of trace amount (mg)";
                        P2Set.LoValue = "--";
                        P2Set.HiValue = "--";
                        P2Set.ReportValue = "--";
                        P2Set.Sch_Code = ProfJobSchemeSet.Sch_Code;
                        P2Set.Sampleident = ProfJobSchemeSet.SAMPLEIDENT;
                        if (string.IsNullOrWhiteSpace(ProfJobSchemeSet.DESCRIPTION_4) == true)
                        {
                            P2Set.FormatValue = "--";
                        }
                        else
                        {
                            P2Set.FormatValue = ProfJobSchemeSet.DESCRIPTION_4;
                        }
                        P2Set.Insert(trans);
                    }
                    if (area == EReportArea.EU) 
                    {
                        // Cr의(Total) 제외요청으로 인한 로직 추가
                        if (ProfJobSchemeSet.Name.Equals("Chromium (Cr)"))
                        {
                            continue;
                        }
                    }
                    
                    P2Set.MainNo = ProfJobSchemeSet.SAMPLEIDENT;
                    P2Set.Name = ProfJobSchemeSet.Name;
                    P2Set.LoValue = ProfJobSchemeSet.LoValue;
                    P2Set.HiValue = ProfJobSchemeSet.HiValue;
                    P2Set.ReportValue = ProfJobSchemeSet.ReportValue;
                    P2Set.FormatValue = ProfJobSchemeSet.FormatValue;
                    P2Set.Sch_Code = ProfJobSchemeSet.Sch_Code;
                    P2Set.Sampleident = ProfJobSchemeSet.SAMPLEIDENT;
                    P2Set.Insert(trans);
                }
            }
        }

        private void InsertPage2Extend(EReportArea area, string ProjJob, string sampleIdent, string sChkJobCase ,SqlTransaction trans)
        {
            //ProfJobSchemeSet.JobNo = jobNo;
            ProfJobSchemeSet.ProjJobNo = ProjJob;
            ProfJobSchemeSet.SAMPLEIDENT = sampleIdent;
            //ProfJobSchemeSet.SelectSampleident(trans);
            //ProfJobSchemeSet.SelectSampleident_Aurora(trans);
            ProfJobSchemeSet.SelectSampleidentProj_Aurora(trans);

            if (sChkJobCase.Equals("surface"))
            {
                ProfJobSchemeSet.Fetch(0, 0, "no", "surface");
            }
            else if (sChkJobCase.Equals("substrate"))
            {
                ProfJobSchemeSet.Fetch(0, 0, "no", "substrate");
            }
            else
            {
                // Error Case
                Console.WriteLine("InsertPage2Extend - Error Case!");
            }

            //ProfJobSchemeSet.Fetch();
            // AYN19 - 043579
            P2ExtendSet.RecNo = sampleIdent;
            P2ExtendSet.Select(trans);

            if (P2ExtendSet.Empty == true)
            {
                if (ProfJobSchemeSet.Empty == false)
                {
                    P2ExtendSet.RecNo = ProfJobSchemeSet.SAMPLEIDENT;
                    P2ExtendSet.Name = ProfJobSchemeSet.Name;
                    P2ExtendSet.LoValue = ProfJobSchemeSet.LoValue;
                    P2ExtendSet.HiValue = ProfJobSchemeSet.HiValue;
                    P2ExtendSet.ReportValue = ProfJobSchemeSet.ReportValue;
                    P2ExtendSet.FormatValue = ProfJobSchemeSet.FormatValue;
                    P2ExtendSet.Sch_Code = ProfJobSchemeSet.Sch_Code;
                    P2ExtendSet.Insert(trans);
                }
            }
        }

        private void UpdatePage2Extend(string sSubJobNo, string sampleIdent, SqlTransaction trans)
        {   
            ProfJobSchemeSet.SAMPLEIDENT = sampleIdent;
            P2ExtendSet.RecNo = sSubJobNo;
            P2ExtendSet.Select(trans);

            if (P2ExtendSet.Empty == false)
            {
                P2ExtendSet.RecNo = ProfJobSchemeSet.SAMPLEIDENT;
                P2ExtendSet.SubJobRecNo = sSubJobNo;
                P2ExtendSet.Update_SubJobNo(trans);
            }
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
            if (area == EReportArea.US)
            {
                List<ChemicalPage2ExtendRow> extendRows = CtrlUs.P2ExtendRows;

                foreach (ChemicalPage2ExtendRow row in extendRows)
                {
                    P2ExtendSet.RecNo = row.RecNo;
                    P2ExtendSet.FormatValue = row.FormatValue;
                    P2ExtendSet.Update(trans);
                }
            }

            List<ChemicalPage2Row> rows = (area == EReportArea.US) ? CtrlUs.P2Rows : CtrlEu.P2Rows;

            foreach (ChemicalPage2Row row in rows)
            {
                P2Set.RecNo = row.RecNo;
                P2Set.FormatValue = row.FormatValue;
                P2Set.Update(trans);
            }
        }

        private void SetReportValidation(SqlTransaction trans)
        {
            partSet.Update(ProfJobSet.AreaNo, ProfJobSet.ItemNo.Split(','), ProfJobSet.JobNo, trans);
            productSet.UpdateValidSet(trans);
        }

        private void ResetReportValidation(SqlTransaction trans)
        {
            partSet.Update(MainSet.AreaNo, MainSet.P1ItemNo.Split(','), trans);
            productSet.UpdateValidReset(trans);
        }
    }
}
