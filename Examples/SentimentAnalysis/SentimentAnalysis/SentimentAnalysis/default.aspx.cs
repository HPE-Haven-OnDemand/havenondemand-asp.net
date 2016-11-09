using HOD.Client;
using HOD.Response.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentimentAnalysis
{
    public partial class _default : System.Web.UI.Page
    {
        HODClient client = null;
        HODResponseParser parser = null;
        string hodApp = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            client = new HODClient("you-api-key", "v1");
            client.onErrorOccurred += Client_onErrorOccurred;
            client.requestCompletedWithContent += Client_requestCompletedWithContent;
            parser = new HODResponseParser();
        }

        protected void uploadButton_Clicked(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(CallHodSentimentAnalysisAsync));
        }

        private async Task CallHodSentimentAnalysisAsync()
        {
            hodApp = HODApps.ANALYZE_SENTIMENT;
            if ((File1.PostedFile != null) && (File1.PostedFile.ContentLength > 0))
            {
                HttpFileCollection uploadedFiles = Request.Files;
                Dictionary<string, object> files = new Dictionary<string, object>();
                for (int i = 0; i < uploadedFiles.Count; i++)
                {
                    HttpPostedFile postedFile = uploadedFiles[i];
                    files.Add(postedFile.FileName, postedFile.InputStream);
                }
                Dictionary<string, object> queryMap = new Dictionary<string, object>();
                queryMap.Add("file", files);
                queryMap.Add("language", textlanguage.SelectedValue);
                await client.PostRequest(queryMap, hodApp, false);
            }
            else if (inputtext.Value.Length > 0)
            {
                Dictionary<string, object> queryMap = new Dictionary<string, object>();
                queryMap.Add("text", inputtext.Value);
                queryMap.Add("language", textlanguage.SelectedValue);
                await client.GetRequest(queryMap, hodApp, false);
            }
        }
        
        private void Client_onErrorOccurred(string errorMessage)
        {
            result.InnerHtml = errorMessage;
        }

        private void Client_requestCompletedWithContent(string response)
        {
            if (hodApp == HODApps.ANALYZE_SENTIMENT)
            {
                SentimentAnalysisResponse res = parser.ParseSentimentAnalysisResponse(ref response);
                if (res != null)
                {
                    string r = "<div style='font-size:1.2em;color:blue'>Positive:<br/>";
                    foreach (SentimentAnalysisResponse.Entity ent in res.positive)
                    {
                        if (ent.topic != null)
                            r += "<span>Topic: " + ent.topic + "</span><br/>";
                        if (ent.sentiment != null)
                            r += "<span>Sentiment: " + ent.sentiment + "</span><br/>";
                        r += "<span>Score: " + ent.score.ToString() + "</span><br/>";
                        r += "<span>Doc#.: " + ent.documentIndex.ToString() + "</span><br/>";
                        r += "--------<br/>";
                    }
                    r += "</div><div style='font-size:1.2em;color:red'>Negative:<br/>";
                    foreach (SentimentAnalysisResponse.Entity ent in res.negative)
                    {
                        if (ent.topic != null)
                            r += "<span>Topic: " + ent.topic + "</span><br/>";
                        if (ent.sentiment != null)
                            r += "<span>Sentiment: " + ent.sentiment + "</span><br/>";
                        r += "<span>Score: " + ent.score.ToString() + "</span><br/>";
                        r += "<span>Doc#.: " + ent.documentIndex.ToString() + "</span><br/>";
                        r += "--------<br/>";
                    }
                    result.InnerHtml = r + "</div>";
                }
                else
                {
                    var errors = parser.GetLastError();
                    foreach (HODErrorObject err in errors)
                    {
                        if (err.error == HODErrorCode.QUEUED)
                        {
                            result.InnerHtml = "queued: " + err.reason + "<br/>" + err.detail;
                            break;
                        }
                        else if (err.error == HODErrorCode.IN_PROGRESS)
                        {
                            result.InnerHtml = "in progress: " + err.reason + "<br/>" + err.detail;
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