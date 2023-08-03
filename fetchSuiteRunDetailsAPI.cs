////////////////////////////////////////////////////////////////////////////////////////////////////
// 
// Description: TS Automation Studio - A 100% Data Driven Framework for Smoke Testing, Code Coverage
//                                     & Performance Testing.
//
// Author: Aman K. Gupta
// ADOBENET\amkumarg
// Revision History (DD/MM/YYYY): 28/06/2010 
//
////////////////////////////////////////////////////////////////////////////////////////////////////
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
    class fetchSuiteRunDetailsAPI
    {
        #region Variables

        string SessionID = string.Empty;
        DateTime init, stop;
        TimeSpan difference;

        static string WebServiceAPI = APICollection.fetchSuiteRunDetails.ToString();
        static string TestCaseID = string.Empty, ScenarioDescription = string.Empty, Result = string.Empty, Remarks = string.Empty, WebService_Response_Dump = string.Empty;
        static Logger loggerObj;
        static DateTime API_Invoke_Time, API_Exit_Time;
        static TimeSpan API_Execution_Time_ms;
        string[] PerformanceData = null;
        int indexPerformanceData = 0;

        #endregion

        #region fetchSuiteRunDetails


        public void fetchSuiteRunDetails(Logger loggerObj, string[] OptionalParams)
        {
            Console.WriteLine("\n" + DateTime.Now + " : " + "|> fetchSuiteRunDetails");

            fetchSuiteRunDetailsAPI.loggerObj = loggerObj;

            string[] filePaths = null;

            try
            {
                if (StudioHome.isPerformanceSuiteRunning)
                {
                    string userNo = OptionalParams[2] + @"\";
                    filePaths = Directory.GetFiles(TS_AutomationStudio.Resources.fetchSuiteRunDetails.InputFilesLocation + userNo, "*.xml");
                    PerformanceData = new string[filePaths.Length];
                    Console.WriteLine("\nUser = " + OptionalParams[2]);


                }
                else
                {
                    filePaths = Directory.GetFiles(TS_AutomationStudio.Resources.fetchSuiteRunDetails.InputFilesLocation, "*.xml");

                }
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Console.WriteLine("\nSEVERE ERROR : Input Files Location : " + TS_AutomationStudio.Resources.fetchSuiteRunDetails.InputFilesLocation + " does not exist !");
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
                int TSR_ID = 0;
                int TS_Depth = 0;
                bool fetchConfig = false;

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

                XmlNode tsr_id = doc.GetElementsByTagName("tsr_id")[0];
                Int32.TryParse(tsr_id.InnerText, out TSR_ID);

                XmlNode treeDepth = doc.GetElementsByTagName("treeDepth")[0];
                Int32.TryParse(treeDepth.InnerText, out TS_Depth);

                XmlNode FetchConfig = doc.GetElementsByTagName("fetchConfig")[0];
                Boolean.TryParse(FetchConfig.InnerText, out fetchConfig);

                XmlNode expectedResult = doc.GetElementsByTagName("ExpectedResult")[0];
                ExpectedResult = expectedResult.InnerText;


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

                TS_AutomationStudio.TSWSRef_PROD_Environment.fetchSuiteRunDetails logObj = new TS_AutomationStudio.TSWSRef_PROD_Environment.fetchSuiteRunDetails();
                logObj.tsr_id = TSR_ID;
                logObj.treeDepth = TS_Depth;
                logObj.SessionID = SessionID;
                logObj.fetchConfig = fetchConfig;

                fetchSuiteRunDetailsRequest lr = new fetchSuiteRunDetailsRequest(logObj);

                init = StopWatch.Start();
                API_Invoke_Time = init;


                fetchSuiteRunDetailsResponse1 lres = null;

                try
                {
                    lres = port.fetchSuiteRunDetails(lr);
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

                SuiteRun returnedScList = lres.fetchSuiteRunDetailsResponse.@return;
                StringBuilder parsedOutput = new StringBuilder();
                if (returnedScList!=null)
                {
                    parsedOutput.AppendLine("<Record>" + "Suite Run Name : " + returnedScList.suiteRunDisplayName + " , Creation Date : " + returnedScList.creationDate + " , Title : " + returnedScList.title + " , IsDeleted : " + returnedScList.isDeleted + @"</Record>");

                }
                string returnedScListToXML = @"<SuiteRunInfo>" + parsedOutput.ToString() + "</SuiteRunInfo>";
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
