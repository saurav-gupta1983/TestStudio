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
    class createTestRunsAPI
    {
        #region Variables

        string SessionID = string.Empty;
        DateTime init, stop;
        TimeSpan difference;

        static string WebServiceAPI = APICollection.createTestRuns.ToString();
        static string TestCaseID = string.Empty, ScenarioDescription = string.Empty, Result = string.Empty, Remarks = string.Empty, WebService_Response_Dump = string.Empty;
        static Logger loggerObj;
        static DateTime API_Invoke_Time, API_Exit_Time;
        static TimeSpan API_Execution_Time_ms;
        string[] PerformanceData = null;
        int indexPerformanceData = 0;

        #endregion

        #region createTestRuns


        public void createTestRuns(Logger loggerObj, string[] OptionalParams)
        {
            Console.WriteLine("\n" + DateTime.Now + " : " + "|> createTestRuns");

            createTestRunsAPI.loggerObj = loggerObj;

            string[] filePaths = null;

            try
            {
                if (StudioHome.isPerformanceSuiteRunning)
                {
                    string userNo = OptionalParams[2] + @"\";
                    filePaths = Directory.GetFiles(TS_AutomationStudio.Resources.createTestRuns.InputFilesLocation + userNo, "*.xml");
                    PerformanceData = new string[filePaths.Length];
                    Console.WriteLine("\nUser = " + OptionalParams[2]);


                }
                else
                {
                    filePaths = Directory.GetFiles(TS_AutomationStudio.Resources.createTestRuns.InputFilesLocation, "*.xml");

                }

            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Console.WriteLine("\nSEVERE ERROR : Input Files Location : " + TS_AutomationStudio.Resources.createTestRuns.InputFilesLocation + " does not exist !");
                return;
            }


            loggerObj.addRedContent1(WebServiceAPI);

            foreach (string file in filePaths)
            {
                #region Execute

                #region Read ScriptData

                #region Variables
                
                #region <ScriptData> variables

                string UserName = string.Empty;
                string Password = string.Empty;
                string ExpectedResult = string.Empty;
                string sessionID = string.Empty; 

                #endregion

                #region <SearchObject> Variables

                string AssignedTo = string.Empty;
                bool AssignToPrimaryOwner = false;
                bool CopyAutomationDataFromTC = false;
                bool CopyExecutionModeFromTC = false;
                bool CopyFileInfoFromTC = false;
                bool CopyOwnerInfoFromParent = false;
                bool CopyStepInfoFromTC = false;
                bool NoCopyTCOwner = false;
                string Status = string.Empty;
                bool isStaticSuite = false;
                bool saveConfig = false;
                string SuiteName = string.Empty;
                string SuiteRunID = string.Empty;
                string TestRunName = string.Empty;
                bool UseSuiteRunID = false;
                bool UseSearchCriterion = false;
                #endregion

                #endregion

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

                XmlNode expectedResult = doc.GetElementsByTagName("ExpectedResult")[0];
                ExpectedResult = expectedResult.InnerText;

                XmlNode inputxml = doc.GetElementsByTagName("InputXML")[0];

                XmlNodeList SearchList = inputxml.SelectNodes("/TestCase/InputXML/SearchList/SearchObject");
                int count = SearchList.Count;

                XmlNode SearchObject = null;
                //SearchCriterion[] sclist = new SearchCriterion[count];
                int i = 0;
                SearchObject = doc.GetElementsByTagName("SearchObject")[0];
                //do
                //{
                //list of <SearchObject>
                XmlNodeList SearchObjectChildren = SearchObject.ChildNodes;
                

                TS_AutomationStudio.TSWSRef_PROD_Environment.createTestRuns logObj = new TS_AutomationStudio.TSWSRef_PROD_Environment.createTestRuns();
                SearchCriterion sc = new SearchCriterion();
                ReferenceTR refTR = new ReferenceTR();
                selectedConfigs[] selConfigsList = null;

                //for every <SearchObject>
                for (int CCount = 0; CCount < SearchObjectChildren.Count; CCount++)
                {
                    //match the name of first (CCount = 0) child of <SearchObject>
                    switch (SearchObjectChildren[CCount].Name)
                    {
                        case "AssignedTo":
                            refTR.assignedTo = SearchObjectChildren[CCount].InnerText;
                            break;
                        case "AssignToPrimaryOwner":
                            bool.TryParse(SearchObjectChildren[CCount].InnerText, out AssignToPrimaryOwner);
                            refTR.assignToPrimaryOwner = AssignToPrimaryOwner;
                            break;
                        case "CopyAutomationDataFromTC":
                            bool.TryParse(SearchObjectChildren[CCount].InnerText, out CopyAutomationDataFromTC);
                            refTR.copyAutomationDataFromTC = CopyAutomationDataFromTC;
                            break;
                        case "CopyExecutionModeFromTC":
                            bool.TryParse(SearchObjectChildren[CCount].InnerText, out CopyExecutionModeFromTC);
                            refTR.copyExecutionModeFromTC = CopyExecutionModeFromTC;
                            break;
                        case "CopyFileInfoFromTC":
                            bool.TryParse(SearchObjectChildren[CCount].InnerText, out CopyFileInfoFromTC);
                            refTR.copyFileInfoFromTC = CopyFileInfoFromTC;
                            break;
                        case "CopyOwnerInfoFromParent":
                            bool.TryParse(SearchObjectChildren[CCount].InnerText, out CopyOwnerInfoFromParent);
                            refTR.copyOwnerInfoFromParent = CopyOwnerInfoFromParent;
                            break;
                        case "CopyStepInfoFromTC":
                            bool.TryParse(SearchObjectChildren[CCount].InnerText, out CopyStepInfoFromTC);
                            refTR.copyStepsInfoFromTC = CopyStepInfoFromTC;
                            break;
                        case "NoCopyTCOwner":
                            bool.TryParse(SearchObjectChildren[CCount].InnerText, out NoCopyTCOwner);
                            refTR.noCopyTCOwner = NoCopyTCOwner;
                            break;
                        case "isStaticSuite":
                            bool.TryParse(SearchObjectChildren[CCount].InnerText, out isStaticSuite);
                            refTR.staticSuite = isStaticSuite;
                            break;
                        case "eEndDate":
                            long eEndDate = 0;
                            long.TryParse(SearchObjectChildren[CCount].InnerText, out eEndDate);
                            refTR.eenddate = eEndDate;
                            break;
                        case "eStartDate":
                            long eStartDate = 0;
                            long.TryParse(SearchObjectChildren[CCount].InnerText, out eStartDate);
                            refTR.estartdate = eStartDate;
                            break;
                        case "Status":
                            refTR.status = SearchObjectChildren[CCount].InnerText;
                            break;
                        case "SuiteName":
                            refTR.suiteName = SearchObjectChildren[CCount].InnerText;
                            break;
                        case "SuiteRunID":
                            refTR.suiteRunID = SearchObjectChildren[CCount].InnerText;
                            break;
                        case "BugsID":
                            string bugsID_csv = SearchObjectChildren[CCount].InnerText;
                            if (bugsID_csv.Equals(string.Empty))
                            {
                                refTR.bugsid = null;
                                break;
                            }
                            string[] bugsID_arr = bugsID_csv.Split(',');
                            refTR.bugsid = bugsID_arr;
                            break;
                        case "TestRunName":
                            refTR.testrunname = SearchObjectChildren[CCount].InnerText;
                            break;
                        case "Time":
                            int Time = 0;
                            Int32.TryParse(SearchObjectChildren[CCount].InnerText, out Time);
                            refTR.time = Time;
                            break;
                        case "UseSuiteRunID":
                            bool.TryParse(SearchObjectChildren[CCount].InnerText, out UseSuiteRunID);
                            refTR.useSuiteRunID = UseSuiteRunID;
                            break;
                        case "saveConfig":
                            bool.TryParse(SearchObjectChildren[CCount].InnerText, out saveConfig);
                            refTR.saveConfig = saveConfig;
                            break;
                        case "IDListArray":
                                int[] idLIST = null;
                                string idlist_str = SearchObjectChildren[CCount].InnerText;
                                if (idlist_str.Equals(string.Empty))
                                {
                                    logObj.idList = null;
                                    break;

                                }

                                string[] idlist_csv = idlist_str.Split(',');
                                idLIST = new int[idlist_csv.Length];
                                int idX = 0;
                                foreach (string id in idlist_csv)
                                {
                                    idLIST[idX] = Int32.Parse(id.ToString());
                                    idX++;
                                }

                                logObj.idList = idLIST;
                            break;
                        case "UseSearchCriterion":
                            bool.TryParse(SearchObjectChildren[CCount].InnerText, out UseSearchCriterion);
                            break;
                        case "SelectedConfigs":
                            XmlNodeList SelectConfigNodesList = SearchObjectChildren[CCount].ChildNodes;
                            int indexOfSelectConfigNodes = 0;
                            selConfigsList = new selectedConfigs[SelectConfigNodesList.Count];
                            selectedConfigs selConfigs = null;
                            foreach (XmlNode SELCON in SelectConfigNodesList)
                            {
                                #region <SelectConfig>

                                selConfigs = new selectedConfigs();
                                XmlNodeList Configs = SELCON.ChildNodes;
                                int configCount = Configs.Count;

                                for (int selConChild = 0; selConChild < configCount; selConChild++)
                                {

                                    switch (Configs[selConChild].Name)
                                    {
                                        case "Build":
                                            selConfigs.build = Configs[selConChild].InnerText;
                                            break;
                                        case "MileStone":
                                            selConfigs.milestone = Configs[selConChild].InnerText;
                                            break;
                                        case "Product":
                                            IDNameObject prodObj = new IDNameObject();
                                            prodObj.name = Configs[selConChild].InnerText;
                                            selConfigs.product = prodObj;
                                            break;
                                        case "ProductVersion":
                                            IDNameObject prodVer = new IDNameObject();
                                            prodVer.name = Configs[selConChild].InnerText;
                                            selConfigs.selVersion = prodVer;
                                            break;
                                        case "GlobalAttributes":
                                            // XmlNodeList GlobalAttributesChildren = Configs[selConChild].ChildNodes;

                                            #region Global Attributes

                                            XmlNodeList AttributeNodes = Configs[selConChild].ChildNodes;
                                            int AttributeCount = AttributeNodes.Count;
                                            TSWSRef_PROD_Environment.Attribute[] AttributesList = new TSWSRef_PROD_Environment.Attribute[AttributeCount];

                                            int attrIndex = 0; // index of AttributeList

                                            //pick up each <Attribute> node
                                            foreach (XmlNode attributeNode in AttributeNodes)
                                            {
                                                TSWSRef_PROD_Environment.Attribute attrObj = new TSWSRef_PROD_Environment.Attribute();

                                                //pickup each childnode of <Attribute>
                                                XmlNodeList AttributeNodeChildren = attributeNode.ChildNodes;
                                                for (int attributeNodeChild = 0; attributeNodeChild < AttributeNodeChildren.Count; attributeNodeChild++)
                                                {

                                                    switch (AttributeNodeChildren[attributeNodeChild].Name)
                                                    {
                                                        case "externalID":
                                                            attrObj.externalID = AttributeNodeChildren[attributeNodeChild].InnerText;
                                                            break;
                                                        case "ID":
                                                            int Id = 0;
                                                            Int32.TryParse(AttributeNodeChildren[attributeNodeChild].InnerText, out Id);
                                                            attrObj.id = Id;
                                                            break;
                                                        case "Key":
                                                            attrObj.key = AttributeNodeChildren[attributeNodeChild].InnerText;
                                                            break;
                                                        case "Type":
                                                            int type = 0;
                                                            Int32.TryParse(AttributeNodeChildren[attributeNodeChild].InnerText, out type);
                                                            attrObj.type = type;
                                                            break;
                                                        case "AttributeValues":
                                                            XmlNodeList AttributeValuesNodes = AttributeNodeChildren[attributeNodeChild].ChildNodes;
                                                            AttrValue[] AttributeValuesList = new AttrValue[AttributeValuesNodes.Count];
                                                            int AttributeValuesListIndex = 0;
                                                            foreach (XmlNode attributeValueNode in AttributeValuesNodes)
                                                            {
                                                                AttrValue attrValObj = new AttrValue();
                                                                //pickup each childnode of <AtrValue>
                                                                XmlNodeList AttributeValueNodeChildren = attributeValueNode.ChildNodes;
                                                                for (int attributeValueNodeChild = 0; attributeValueNodeChild < AttributeValueNodeChildren.Count; attributeValueNodeChild++)
                                                                {
                                                                    switch (AttributeValueNodeChildren[attributeValueNodeChild].Name)
                                                                    {
                                                                        case "externalId":
                                                                            attrValObj.externalID = AttributeValueNodeChildren[attributeValueNodeChild].InnerText;
                                                                            break;
                                                                        case "value":
                                                                            attrValObj.value = AttributeValueNodeChildren[attributeValueNodeChild].InnerText;
                                                                            break;
                                                                        case "id":
                                                                            int id = 0;
                                                                            Int32.TryParse(AttributeValueNodeChildren[attributeValueNodeChild].InnerText, out id);
                                                                            attrObj.id = id;
                                                                            break;
                                                                        case "Selected":
                                                                            bool Selected = false;
                                                                            bool.TryParse(AttributeValueNodeChildren[attributeValueNodeChild].InnerText, out Selected);
                                                                            attrValObj.selected = Selected;
                                                                            break;
                                                                        case "visible":
                                                                            bool visible = false;
                                                                            bool.TryParse(AttributeValueNodeChildren[attributeValueNodeChild].InnerText, out visible);
                                                                            attrValObj.visible = visible;
                                                                            break;
                                                                        default:
                                                                            break;
                                                                    }
                                                                }

                                                                AttributeValuesList[AttributeValuesListIndex] = attrValObj;
                                                                AttributeValuesListIndex++;
                                                            }
                                                            attrObj.values = AttributeValuesList;
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }

                                                AttributesList[attrIndex] = attrObj;
                                                attrIndex++;

                                            }

                                            selConfigs.globalAttrs = AttributesList;


                                            #endregion
                                            break;
                                        default:
                                            break;
                                    }
                                }


                                #endregion                            

                                //Add the selConfigs object to the ConfigList

                                selConfigsList[indexOfSelectConfigNodes] = selConfigs;
                                indexOfSelectConfigNodes++;

                            }
                            break;
                        case "filterList":
                            if (!UseSearchCriterion)
                            {
                                sc.filterList = null;
                                break;
                            }
                            //gives all <filter> nodes
                            XmlNodeList Filters = SearchObjectChildren[CCount].ChildNodes;
                            int filterCount = Filters.Count;
                            Filter[] filterList = new Filter[filterCount];

                            int f = 0; // index of filterList

                            //pick up each <filter> node
                            foreach (XmlNode filterNode in Filters)
                            {
                                Filter filterObj = new Filter();

                                //pickup each childnode of <filter>
                                XmlNodeList filterNodeChildren = filterNode.ChildNodes;
                                for (int filterNodeChild = 0; filterNodeChild < filterNodeChildren.Count; filterNodeChild++)
                                {

                                    switch (filterNodeChildren[filterNodeChild].Name)
                                    {
                                        case "fieldName":
                                            filterObj.fieldName = filterNodeChildren[filterNodeChild].InnerText;
                                            break;
                                        case "operator":
                                            filterObj.@operator = filterNodeChildren[filterNodeChild].InnerText;
                                            break;
                                        case "fieldValues":
                                            XmlNodeList fieldValues = filterNodeChildren[filterNodeChild].ChildNodes;
                                            string[] fieldVALUES = new string[fieldValues.Count];
                                            int nFieldVal = 0;
                                            foreach (XmlNode fieldValue in fieldValues)
                                            {
                                                fieldVALUES[nFieldVal] = fieldValue.InnerText;
                                                nFieldVal++;
                                            }
                                            filterObj.fieldValue = fieldVALUES;
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                filterList[f] = filterObj;
                                f++;

                            }

                            sc.filterList = filterList;

                            break;
                        case "filterRelationship":
                            if (!UseSearchCriterion)
                            {
                                sc.filterList = null;
                                break;
                            }
                            sc.filterRelationship = SearchObjectChildren[CCount].InnerText;
                            break;
                        default:
                            break;
                    }



                }

                if (!UseSearchCriterion)
                {
                    logObj.sc = null;

                }
                if (UseSearchCriterion)
                {
                    logObj.sc = sc;

                }
                //sclist[i] = sc;
                SearchObject = SearchObject.NextSibling;
                i++;

                //} while (i < count && SearchObject!=null);


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

                //selectedConfigs[] selConfigsList = new selectedConfigs[1];
                //selConfigsList[0] = selConfigs;
                //TS_AutomationStudio.TSWSRef_PROD_Environment.deleteData logObj = new TS_AutomationStudio.TSWSRef_PROD_Environment.deleteData();
                logObj.SessionID = SessionID;
                logObj.refTR = refTR;
                logObj.selectedConfigs = selConfigsList;
                

                createTestRunsRequest lr = new createTestRunsRequest(logObj);


                init = StopWatch.Start();
                API_Invoke_Time = init;


                createTestRunsResponse1 lres = null;

                try
                {
                    lres = port.createTestRuns(lr);
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

                WebService_Response_Dump = lres.createTestRunsResponse.@return;
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
