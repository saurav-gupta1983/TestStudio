using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TS_AutomationStudio.TSWSRef_PROD_Environment;
using TS_AutomationStudio.Resources;
using System.IO;
using System.Xml;
using TS_AutomationStudio.Test_Cases_PROD_Environment;
using TS_AutomationStudio.LogManager;

namespace TS_AutomationStudio.Test_Cases_PROD_Environment
{
    class createTestSuiteAPI
    {
        #region Variables

        string SessionID = string.Empty;
        DateTime init, stop;
        TimeSpan difference;

        static string WebServiceAPI = APICollection.createTestSuite.ToString();
        static string TestCaseID = string.Empty, ScenarioDescription = string.Empty, Result = string.Empty, Remarks = string.Empty, WebService_Response_Dump = string.Empty;
        static Logger loggerObj;
        static DateTime API_Invoke_Time, API_Exit_Time;
        static TimeSpan API_Execution_Time_ms;
        string[] PerformanceData = null;
        int indexPerformanceData = 0;

        #endregion

        #region createTestSuite


        public void createTestSuite(Logger loggerObj, string[] OptionalParams)
        {
            Console.WriteLine("\n" + DateTime.Now + " : " + "|> createTestSuite");

            createTestSuiteAPI.loggerObj = loggerObj;

            string[] filePaths = null;

            try
            {
                if (StudioHome.isPerformanceSuiteRunning)
                {
                    string userNo = OptionalParams[2] + @"\";
                    filePaths = Directory.GetFiles(TS_AutomationStudio.Resources.createTestSuite.InputFilesLocation + userNo, "*.xml");
                    PerformanceData = new string[filePaths.Length];
                    Console.WriteLine("\nUser = " + OptionalParams[2]);


                }
                else
                {
                    filePaths = Directory.GetFiles(TS_AutomationStudio.Resources.createTestSuite.InputFilesLocation, "*.xml");

                }

            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Console.WriteLine("\nSEVERE ERROR : Input Files Location : " + TS_AutomationStudio.Resources.createTestSuite.InputFilesLocation + " does not exist !");
                return;
            }


            loggerObj.addRedContent1(WebServiceAPI);

            foreach (string  file in filePaths)
            {
                #region Execute

                #region Read ScriptData

                //Variables
                string UserName = string.Empty;
                string Password = string.Empty;
                string ExpectedResult = string.Empty;
                string sessionID = string.Empty;
                bool allowDup = false;
               
                // Read test Input Data
                TextReader tr = new StreamReader(file);
                string xmlin = tr.ReadToEnd();
                tr.Close();

                //Load Test Input Data into XML Doc

                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.LoadXml(xmlin);

                }
                catch (Exception)
                {
                    Console.WriteLine("\nSEVERE ERROR : Input Script - '" + file + "' is Not a Valid XML File!");
                    continue;
                }

                //Read ScriptData Tags

                XmlNode testcaseID = doc.GetElementsByTagName("TestCaseID")[0];
                TestCaseID = testcaseID.InnerText;

                XmlNode testcaseScenario = doc.GetElementsByTagName("TestCaseScenario")[0];
                ScenarioDescription = testcaseScenario.InnerText;

                XmlNode username = doc.GetElementsByTagName("username")[0];
                UserName = username.InnerText;

                XmlNode password = doc.GetElementsByTagName("password")[0];
                Password = password.InnerText;

                XmlNode sessionid = doc.GetElementsByTagName("sessionID")[0];
                sessionID = sessionid.InnerText;

                XmlNode allowDuplicateInSuites = doc.GetElementsByTagName("allowDuplicateInSuites")[0];
                Boolean.TryParse(allowDuplicateInSuites.InnerText, out allowDup);

                XmlNode expectedResult = doc.GetElementsByTagName("ExpectedResult")[0];
                ExpectedResult = expectedResult.InnerText;

                XmlNode inputxml = doc.GetElementsByTagName("InputXML")[0];

                XmlNodeList SearchList = inputxml.SelectNodes("/TestCase/InputXML/SearchList/SearchObject");
                int count = SearchList.Count;

                XmlNode SearchObject = null;
                TS_AutomationStudio.TSWSRef_PROD_Environment.createTestSuite logObj = new TS_AutomationStudio.TSWSRef_PROD_Environment.createTestSuite();
                TSInfo[] sclist = new TSInfo[count];
                //SearchCriterion[] sclist = new SearchCriterion[count];
                int i = 0;
                SearchObject = doc.GetElementsByTagName("SearchObject")[0];
                do
                {
                    //list of <SearchObject>
                     XmlNodeList SearchObjectChildren = SearchObject.ChildNodes;

                     TSInfo sc = new TSInfo();
                     string[] tc_ids_str = null;
                     string[] ts_ids_str = null;
                     string[] TCApprovers_str = null;
                     string[] TCReviewers_str = null;
                     //for every <SearchObject>
                     for (int CCount = 0; CCount < SearchObjectChildren.Count; CCount++)
                     {
                         //match the name of first (CCount = 0) child of <SearchObject>
                         switch (SearchObjectChildren[CCount].Name)
                         {
                             case "Title":
                                 sc.title = SearchObjectChildren[CCount].InnerText;
                                 break;
                             case "Description":
                                 sc.desc = SearchObjectChildren[CCount].InnerText;
                                 break;
                             case "Product":
                                 IDNameObject prodName = new IDNameObject();
                                 prodName.name = SearchObjectChildren[CCount].InnerText;
                                 sc.product = prodName;
                                 break;
                             case "Owner":
                                 string owners_csv = SearchObjectChildren[CCount].InnerText;
                                 string[] owners_str = owners_csv.Split(',');
                                 IDNameObject[] owners = new IDNameObject[owners_str.Length];
                                 int owner_idx = 0;
                                 foreach (string item in owners_str)
                                 {
                                     IDNameObject obj = new IDNameObject();
                                     obj.name = item;
                                     owners[owner_idx] = obj;
                                     owner_idx++;
                                 }
                                 sc.owners = owners;
                                 break;
                             case "tc_ids":
                                 string ts_ids_csv = SearchObjectChildren[CCount].InnerText;
                                 if (ts_ids_csv.Equals(string.Empty))
                                 {
                                     ts_ids_str = null;
                                     break;
                                 }
                                 ts_ids_str = ts_ids_csv.Split(',');
                                 break;
                             case "ts_ids":
                                 string tc_ids_csv = SearchObjectChildren[CCount].InnerText;
                                 if (tc_ids_csv.Equals(string.Empty))
                                 {
                                     tc_ids_str = null;
                                     break;
                                 }
                                 tc_ids_str = tc_ids_csv.Split(',');
                                 break;
                             case "TCApprovers":
                                 string TCApprovers_csv = SearchObjectChildren[CCount].InnerText;
                                 if (TCApprovers_csv.Equals(string.Empty))
                                 {
                                     TCApprovers_str = null;
                                     break;
                                 }

                                 TCApprovers_str = TCApprovers_csv.Split(',');
                                 break;
                             case "TCReviewers":
                                 string TCReviewers_csv = SearchObjectChildren[CCount].InnerText;
                                 if (TCReviewers_csv.Equals(string.Empty))
                                 {
                                     TCReviewers_str = null;
                                     break;
                                 }
                                 TCReviewers_str = TCReviewers_csv.Split(',');
                                 break;
                             default:
                                 break;
                         }

                     }

                     TSEntity[] tsEntity = null;
                     int maxIndexLength = 0;
                     if (tc_ids_str != null)
                     {
                         maxIndexLength += tc_ids_str.Length;
                     }
                     if (ts_ids_str != null)
                     {
                         maxIndexLength += ts_ids_str.Length;
                     }
                     if (TCApprovers_str!=null)
                     {
                         maxIndexLength += 1;
                     }
                     if (TCReviewers_str != null)
                     {
                         maxIndexLength += 1;
                     }
                     
                     tsEntity = new TSEntity[maxIndexLength];
                     int tsE_idx = 0;

                     if (tc_ids_str != null)
                     {
                         #region Parse TcIds

                         foreach (string tcStr in tc_ids_str)
                         {
                             TSEntity tsE = new TSEntity();
                             TempTCInfo tcInfo = new TempTCInfo();
                             tcInfo.tcID = tcStr;
                             tsE.tc = tcInfo;
                             tsEntity[tsE_idx] = tsE;
                             tsE_idx++;
                         }

                         #endregion   

                     }

                     if (ts_ids_str != null)
                     {
                         #region parse TsIds

                         foreach (string tsStr in ts_ids_str)
                         {
                             TSEntity tsE = new TSEntity();
                             TSInfo tsInfo = new TSInfo();
                             tsInfo.tsID = tsStr;
                             tsE.ts = tsInfo;
                             tsEntity[tsE_idx] = tsE;
                             tsE_idx++;
                         }

                         #endregion

                     }

                     if(TCApprovers_str!=null)
                     {
                         #region Parse TcApprovers
                         // Parse TcApprovers
                         IDNameObject[] TcApproversList = new IDNameObject[TCApprovers_str.Length];
                         int TcApproversListIndex = 0;
                         foreach (string tcAppr in TCApprovers_str)
                         {
                             IDNameObject apprName = new IDNameObject();
                             apprName.name = tcAppr;
                             TcApproversList[TcApproversListIndex] = apprName;
                             TcApproversListIndex++;

                         }
                         // Set Approvers List

                         TSEntity tsEA = new TSEntity();
                         TempTCInfo tcInfoA = new TempTCInfo();
                         tcInfoA.approver = TcApproversList;
                         tsEA.tc = tcInfoA;
                         tsEntity[tsE_idx] = tsEA;
                         tsE_idx++;

                         // Set Approvers List

                         #endregion

                     }

                     if (TCReviewers_str != null)
                     {
                         #region Parse TcReviewers
                         // Parse TcReviewers
                         IDNameObject[] TcReviewersList = new IDNameObject[TCReviewers_str.Length];
                         int TcReviewersListIndex = 0;
                         foreach (string tcReviewer in TCApprovers_str)
                         {
                             IDNameObject revrName = new IDNameObject();
                             revrName.name = tcReviewer;
                             TcReviewersList[TcReviewersListIndex] = revrName;
                             TcReviewersListIndex++;

                         }
                         // Set Reviewers List

                         TSEntity tsER = new TSEntity();
                         TempTCInfo tcInfoR = new TempTCInfo();
                         tcInfoR.reviewer = TcReviewersList;
                         tsER.tc = tcInfoR;
                         tsEntity[tsE_idx] = tsER;
                         tsE_idx++;

                         // Set Reviewers List

                         #endregion

                     }

                     sc.tsEntity = tsEntity;
                     sclist[i] = sc;
                     SearchObject = SearchObject.NextSibling;
                     i++;

                } while (i < count && SearchObject!=null);
               
                

                #endregion

                //Initiate Login

                Console.WriteLine("\n" + DateTime.Now + " : " + "START: " + TestCaseID + "\n File: " + file);

                TestStudio port = null;

                SessionID = string.Empty;

                if (sessionID.Equals(string.Empty))
                {
                    SessionID = TS_AutomationStudio_PROD_Environment.Common.LoginUser(UserName, Password, out port);


                }
                else
                {
                    port = new TestStudioClient();
                    SessionID = sessionID;
                }

                logObj.SessionID = SessionID;
                logObj.tsList = sclist;
                logObj.allowDuplicateInSuites = allowDup;
                createTestSuiteRequest lr = new createTestSuiteRequest(logObj);

                //sessionDetailsRequest lr = new sessionDetailsRequest(logObj);

                init = StopWatch.Start();
                API_Invoke_Time = init;


                createTestSuiteResponse lres = null;

                try
                {
                    lres = port.createTestSuite(lr);
                }
                catch (Exception ex)
                {
                    Result = "Fail";
                    Remarks = "Webservice Exception : " + ex.Message + " Stack Trace : " + ex.StackTrace;
                    stop = StopWatch.Stop();
                    API_Exit_Time = stop;
                    difference = stop - init;
                    API_Execution_Time_ms = difference;

                    goto skipvalidation;

                }

                stop = StopWatch.Stop();
                API_Exit_Time = stop;
                difference = stop - init;
                API_Execution_Time_ms = difference;

                if (StudioHome.isPerformanceSuiteRunning)
                {
                    PerformanceData[indexPerformanceData] = API_Execution_Time_ms.Seconds.ToString();
                    indexPerformanceData++;

                }

                #region Webservice Response Validation

                TSResult[] returnedScList = lres.createTestSuiteResponse1;
                StringBuilder parsedOutput = new StringBuilder();
                if (returnedScList != null)
                {
                    for (int tsCount = 0; tsCount < returnedScList.Length; tsCount++)
                    {
                        parsedOutput.AppendLine("<Suite>");
                        parsedOutput.AppendLine("<SuiteDisplayName>" + returnedScList[tsCount].ts_displayName + @"</SuiteDisplayName>");
                        parsedOutput.AppendLine("<ErrorCode>" + returnedScList[tsCount].errorEvent.errorCode + @"</ErrorCode>");
                        parsedOutput.AppendLine("<ErrorMessage>" + returnedScList[tsCount].errorEvent.errorCode + @"</ErrorMessage>");
                        parsedOutput.AppendLine("</Suite>");
                    }
                }
                string returnedScListToXML = @"<CreateTestSuite>" + parsedOutput.ToString() + "</CreateTestSuite>";
                WebService_Response_Dump = returnedScListToXML;
                XmlDocument webresDoc = new XmlDocument();
                webresDoc.LoadXml(WebService_Response_Dump);

                XmlNode errorCode_ResultList = webresDoc.SelectSingleNode("/resultlist/result/error/errorCode");
                XmlNode errorMsg_ResultList = webresDoc.SelectSingleNode("/resultlist/result/error/errorMessage");
                XmlNode errorCode__ERROR = webresDoc.SelectSingleNode("/error/errorCode");
                XmlNode errorMsg_ERROR = webresDoc.SelectSingleNode("/error/errorMessage");

                if (errorCode_ResultList != null)
                {
                    int eCode = Int32.Parse(errorCode_ResultList.InnerText);
                    if (eCode == 0)
                    {
                        Result = "Pass";
                        Remarks = "NA";

                    }
                    else
                    {
                        Result = "Fail";
                        if (errorMsg_ResultList != null)
                        {
                            Remarks = errorMsg_ResultList.InnerText;
                        }
                        else
                        {
                            Remarks = "";
                        }


                    }
                }
                else if (errorCode__ERROR != null)
                {
                    int eCode = Int32.Parse(errorCode__ERROR.InnerText);
                    if (eCode == 0)
                    {
                        Result = "Pass";
                        Remarks = "NA";

                    }
                    else
                    {
                        Result = "Fail";
                        if (errorMsg_ERROR != null)
                        {
                            Remarks = errorMsg_ERROR.InnerText;
                        }
                        else
                        {
                            Remarks = "";
                        }


                    }
                }
                else
                {
                    Result = "Pass";
                    Remarks = "NA";
                }



                #endregion

            skipvalidation:

                loggerObj.addGreenContent(TestCaseID, ScenarioDescription + " |>File : " + file, API_Invoke_Time, API_Exit_Time, API_Execution_Time_ms, Result, ExpectedResult, Remarks, WebService_Response_Dump);

                Console.WriteLine("\n" + DateTime.Now + " : " + "END: " + TestCaseID);

                #endregion

            }

            if (StudioHome.isPerformanceSuiteRunning)
            {
                StringBuilder TimeData = new StringBuilder();
                foreach (string Record in PerformanceData)
                {
                    TimeData.Append("<" + WebServiceAPI + ">");
                    TimeData.Append(Record);
                    TimeData.Append("</" + WebServiceAPI + ">");
                    Common.UpdateTimeStringBuilder(TimeData.ToString());

                }
            }

            //loggerObj.addRedContent2();
        }

        #endregion

       
    }
}
