using HOD.Client;
using HOD.Response.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpeechRecognition
{
    public partial class _default : System.Web.UI.Page
    {
        HODClient client = null;
        HODResponseParser parser = null;
        string hodApp = "";
        string jobID = "";
        string textResponse = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            client = new HODClient("your-api-key", "v1");
            client.onErrorOccurred += Client_onErrorOccurred;
            client.requestCompletedWithContent += Client_requestCompletedWithContent;
            client.requestCompletedWithJobID += Client_requestCompletedWithJobID;
            parser = new HODResponseParser();
        }

        protected void uploadButton_Clicked(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(CallHodSpeechRecognitionAsync));
        }

        private async Task CallHodSpeechRecognitionAsync()
        {
            if ((File1.PostedFile != null) && (File1.PostedFile.ContentLength > 0))
            {
                try
                {
                    Dictionary<string, object> queryMap = new Dictionary<string, object>();
                    Dictionary<string, object> file = new Dictionary<string, object>();
                    Stream str = File1.PostedFile.InputStream;
                    file.Add(File1.PostedFile.FileName, str);
                    queryMap.Add("file", file);
                    queryMap.Add("interval", -1);
                    queryMap.Add("language", speechlanguage.SelectedValue);
                    hodApp = HODApps.RECOGNIZE_SPEECH;
                    await client.PostRequest(queryMap, hodApp, true);
                }
                catch (Exception ex)
                {
                    result.InnerHtml = "Error: " + ex.Message;
                }
            }
            else
            {
                result.InnerHtml = "Please select a media file.";
            }
        }

        private async Task ChainHodCallAsync(string input)
        {
            Dictionary<string, object> queryMap = new Dictionary<string, object>();
            queryMap.Add("text", input);
            List<object> entities = new List<object>();
            entities.Add("people_eng");
            entities.Add("places_eng");
            entities.Add("companies_eng");
            queryMap.Add("entity_type", entities);
            queryMap.Add("unique_entities", true);
            hodApp = HODApps.ENTITY_EXTRACTION;
            await client.PostRequest(queryMap, hodApp, false);
        }

        private void Client_requestCompletedWithJobID(string response)
        {
            jobID = parser.ParseJobID(response);
            if (jobID.Length > 0)
                RegisterAsyncTask(new PageAsyncTask(CallGetJobStatusAsync));

        }
        
        private async Task CallGetJobStatusAsync()
        {
            await client.GetJobStatus(jobID);
        }

        private void Client_onErrorOccurred(string errorMessage)
        {
            result.InnerHtml = errorMessage;
        }

        private void Client_requestCompletedWithContent(string response)
        {
            if (hodApp == HODApps.RECOGNIZE_SPEECH)
            {
                SpeechRecognitionResponse resp = parser.ParseSpeechRegconitionResponse(ref response);
                if (resp != null)
                {
                    string text = "";
                    foreach (SpeechRecognitionResponse.Document doc in resp.document)
                    {
                        text += doc.content;
                    }

                    if (chainapi.Checked)
                    {
                        textResponse = text;
                        RegisterAsyncTask(new PageAsyncTask(() => ChainHodCallAsync(input: text)));
                    }
                    else
                        result.InnerHtml = text;
                }
                else
                {
                    var errors = parser.GetLastError();
                    foreach (HODErrorObject err in errors)
                    {
                        if (err.error == HODErrorCode.QUEUED)
                        {
                            result.InnerHtml = "QUEUED";
                            jobID = err.jobID;
                            Thread.Sleep(1000);
                            RegisterAsyncTask(new PageAsyncTask(CallGetJobStatusAsync));
                            break;
                        }
                        else if (err.error == HODErrorCode.IN_PROGRESS)
                        {
                            jobID = err.jobID;
                            Thread.Sleep(5000);
                            RegisterAsyncTask(new PageAsyncTask(CallGetJobStatusAsync));
                            break;
                        }
                        else if (err.error == HODErrorCode.NONSTANDARD_RESPONSE)
                        {
                            result.InnerHtml = "Error: " + response;
                        }
                        else
                        {
                            result.InnerHtml = "Error: " + err.reason;
                        }
                    }
                }
            }
            else if (hodApp == HODApps.ENTITY_EXTRACTION)
            {
                EntityExtractionResponse resp = parser.ParseEntityExtractionResponse(ref response);
                if (resp != null)
                {
                    string text = textResponse + "<br/><br/><div>";
                    if (resp.entities.Count > 0)
                    {
                        foreach (var entity in resp.entities)
                        {
                            if (entity.type == "companies_eng")
                            {
                                text += "<b>Company name:</b> " + entity.normalized_text + "</br>";
                                if (entity.additional_information != null)
                                {
                                    string url = "";
                                    if (entity.additional_information.wikipedia_eng != null)
                                    {
                                        text += "<b>Wiki page: </b><a href=\"";

                                        if (!entity.additional_information.wikipedia_eng.Contains("http"))
                                        {
                                            url = "http://" + entity.additional_information.wikipedia_eng;
                                        }
                                        else
                                        {
                                            url = entity.additional_information.wikipedia_eng;
                                        }
                                        text += url + "\">";
                                        text += url + "</a>";
                                        text += "</br>";
                                    }
                                    if (entity.additional_information.url_homepage != null)
                                    {
                                        text += "<b>Home page: </b><a href=\"";
                                        if (!entity.additional_information.url_homepage.Contains("http"))
                                        {
                                            url = "http://" + entity.additional_information.url_homepage;
                                        }
                                        else
                                        {
                                            url = entity.additional_information.url_homepage;
                                        }
                                        text += url + "\">";
                                        text += url + "</a>";
                                        text += "</br>";
                                    }
                                }
                            }
                            else if (entity.type == "places_eng")
                            {
                                text += "<b>Place name:</b> " + entity.normalized_text + "</br>";
                                if (entity.additional_information != null)
                                {
                                    string url = "";
                                    if (entity.additional_information.wikipedia_eng != null)
                                    {
                                        text += "<b>Wiki page: </b><a href=\"";
                                        if (!entity.additional_information.wikipedia_eng.Contains("http"))
                                        {
                                            url = "http://";
                                        }
                                        else
                                        {
                                            url = entity.additional_information.wikipedia_eng;
                                        }
                                        text += url + "\">";
                                        text += url + "</a>";
                                        text += "</br>";
                                    }
                                    if (entity.additional_information.image != null)
                                    {
                                        text += "<img src=\"";
                                        text += entity.additional_information.image + "\" height='400px'/>";
                                        text += "</br>";
                                    }
                                }
                            }
                            else if (entity.type == "people_eng")
                            {
                                text += entity.normalized_text + "</br>";
                                if (entity.additional_information != null)
                                {
                                    string url = "";
                                    if (entity.additional_information.wikipedia_eng != null)
                                    {
                                        text += "<b>Wiki page: </b><a href=\"";
                                        if (!entity.additional_information.wikipedia_eng.Contains("http"))
                                        {
                                            url = "http://";
                                        }
                                        else
                                        {
                                            url = entity.additional_information.wikipedia_eng;
                                        }
                                        text += url + "\">";
                                        text += url + "</a>";
                                        text += "</br>";
                                    }
                                    if (entity.additional_information.image != null)
                                    {
                                        text += "<img src=\"";
                                        text += entity.additional_information.image + "\" height='400px'/>";
                                        text += "</br>";
                                    }
                                }
                            }

                            text += "</br>";
                        }
                        text += "</div>";
                        result.InnerHtml = text;
                    }
                    else
                    {

                        text += "Not found</div>";
                        result.InnerHtml = text;
                    }
                }
                else
                {
                    var errors = parser.GetLastError();
                    foreach (HODErrorObject err in errors)
                    {
                        if (err.error == HODErrorCode.QUEUED)
                        {
                            result.InnerHtml = "QUEUED";
                            jobID = err.jobID;
                            Thread.Sleep(1000);
                            RegisterAsyncTask(new PageAsyncTask(CallGetJobStatusAsync));
                            break;
                        }
                        else if (err.error == HODErrorCode.IN_PROGRESS)
                        {
                            jobID = err.jobID;
                            Thread.Sleep(5000);
                            RegisterAsyncTask(new PageAsyncTask(CallGetJobStatusAsync));
                            break;
                        }
                        else if (err.error == HODErrorCode.NONSTANDARD_RESPONSE)
                        {
                            result.InnerHtml = "None standard resp: " + response + "<br/>" + err.detail;
                            break;
                        }
                        else
                        {
                            result.InnerHtml = "Error: " + err.reason + "<br/>" + err.detail;
                            break;
                        }
                    }
                }
            }
        }
    }
}