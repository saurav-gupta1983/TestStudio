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
    class fetchSuiteRunDetailsExAPI
    {
        #region Variables

        string SessionID = string.Empty;
        DateTime init, stop;
        TimeSpan difference;

        static string WebServiceAPI = APICollection.fetchSuiteRunDetailsEx.ToString();
        static string TestCaseID = string.Empty, ScenarioDescription = string.Empty, Result = string.Empty, Remarks = string.Empty, WebService_Response_Dump = string.Empty;
        static Logger loggerObj;
        static DateTime API_Invoke_Time, API_Exit_Time;
        static TimeSpan API_Execution_Time_ms;
        string[] PerformanceData = null;
        int indexPerformanceData = 0;

        #endregion

        #region fetchSuiteRunDetailsEx


        public void fetchSuiteRunDetailsEx(Logger loggerObj, string[] OptionalParams)
        {
            Console.WriteLine("\n" + DateTime.Now + " : " + "|> fetchSuiteRunDetailsEx");

            fetchSuiteRunDetailsExAPI.loggerObj = loggerObj;

            string[] filePaths = null;

            try
            {
                if (StudioHome.isPerformanceSuiteRunning)
                {
                    string userNo = OptionalParams[2] + @"\";
                    filePaths = Directory.GetFiles(TS_AutomationStudio.Resources.fetchSuiteRunDetailsEx.InputFilesLocation + userNo, "*.xml");
                    PerformanceData = new string[filePaths.Length];
                    Console.WriteLine("\nUser = " + OptionalParams[2]);


                }
                else
                {
                    filePaths = Directory.GetFiles(TS_AutomationStudio.Resources.fetchSuiteRunDetailsEx.InputFilesLocation, "*.xml");

                }

            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Console.WriteLine("\nSEVERE ERROR : Input Files Location : " + TS_AutomationStudio.Resources.fetchSuiteRunDetailsEx.InputFilesLocation + " does not exist !");
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
                bool fetchChildTR = false;
                bool fetchChildTSR = false;
                bool fetchTRConfig = false;
                bool fetchOwners = false;
                int pageNumber = 0;
                int pageSize = 0;

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

                XmlNode FetchChildTR = doc.GetElementsByTagName("fetchChildTR")[0];
                Boolean.TryParse(FetchChildTR.InnerText, out fetchChildTR);

                XmlNode FetchChildTSR = doc.GetElementsByTagName("fetchChildTSR")[0];
                Boolean.TryParse(FetchChildTSR.InnerText, out fetchChildTSR);

                XmlNode FetchTRConfig = doc.GetElementsByTagName("fetchTRConfig")[0];
                Boolean.TryParse(FetchTRConfig.InnerText, out fetchTRConfig);

                XmlNode FetchOwners = doc.GetElementsByTagName("fetchOwners")[0];
                Boolean.TryParse(FetchOwners.InnerText, out fetchOwners);

                XmlNode PageNumber = doc.GetElementsByTagName("pageNumber")[0];
                Int32.TryParse(PageNumber.InnerText, out pageNumber);

                XmlNode PageSize = doc.GetElementsByTagName("pageSize")[0];
                Int32.TryParse(PageSize.InnerText, out pageSize);

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

                TS_AutomationStudio.TSWSRef_PROD_Environment.fetchSuiteRunDetailsEx logObj = new TS_AutomationStudio.TSWSRef_PROD_Environment.fetchSuiteRunDetailsEx();
                logObj.tsr_id = TSR_ID;
                logObj.fetchChildTR = fetchChildTR;
                logObj.SessionID = SessionID;
                logObj.fetchChildTSR = fetchChildTSR;
                logObj.fetchOwners = fetchOwners;
                logObj.fetchTRConfig = fetchTRConfig;
                logObj.pageNumber = pageNumber;
                logObj.pageSize = pageSize;

                fetchSuiteRunDetailsExRequest lr = new fetchSuiteRunDetailsExRequest(logObj);

                init = StopWatch.Start();
                API_Invoke_Time = init;


                fetchSuiteRunDetailsExResponse1 lres = null;

                try
                {
                    lres = port.fetchSuiteRunDetailsEx(lr);
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

                TSRDetailsPgdObj returnedScList = lres.fetchSuiteRunDetailsExResponse.@return;
                TSREntity[] TsInfoList = returnedScList.list;
                StringBuilder parsedOutput = new StringBuilder();
                if (TsInfoList!=null)
                {
                    parsedOutput.AppendLine("<SuiteRunDisplayName>" + TsInfoList[0].tsr.suiteRunDisplayName + @"</SuiteRunDisplayName>");
                
                    parsedOutput.AppendLine("<RunDetails>");
                    for (int tsCount = 1; tsCount < TsInfoList.Length; tsCount++)
                    {
                        if (TsInfoList[tsCount].tr!= null)
                        {
                            parsedOutput.AppendLine("<Run>" + TsInfoList[tsCount].tr.id.ToString() + @"</Run>");

                        }

                    }
                    parsedOutput.AppendLine("</RunDetails>");
               }
                string returnedScListToXML = @"<TestSuiteDetailsEx>" + parsedOutput.ToString() + "</TestSuiteDetailsEx>";
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
