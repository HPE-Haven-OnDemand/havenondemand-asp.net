# ASP.NET (C#) client library for Haven OnDemand.

Official ASP.NET client library to help with calling [Haven OnDemand APIs](http://havenondemand.com).

## What is Haven OnDemand?
Haven OnDemand is a set of over 70 APIs for handling all sorts of unstructured data. Here are just some of our APIs' capabilities:
* Speech to text
* OCR
* Text extraction
* Indexing documents
* Smart search
* Language identification
* Concept extraction
* Sentiment analysis
* Web crawlers
* Machine learning

For a full list of all the APIs and to try them out, check out https://www.havenondemand.com/developer/apis

## Overview
The library contains 2 packages:

HODClient package for sending HTTP GET/POST requests to Haven OnDemand APIs.

HODResponseParser package for parsing JSON responses from Haven OnDemand APIs. To use the HODResponseParser, you will need the NewtonSoft.json dll for .NET.

HODClient library supports the .NET 4.5 and above.

----
## Integrate HODClient into an ASP.NET project
1. Right click on the project's References folder and select "Manage Nuget Packages...".
>![](/images/managenuget.jpg)

2. Select Browse and choose nuget.org for Package source then type in the search field "HavenOnDemand"
>![](/images/installhodnuget.jpg)

3. Select a package support ASP.NET and click Install.

----
## Using HODClient package
```
using HOD.Client;
HODClient client = new HODClient("API_KEY", "v1");
```
where you replace "API_KEY" with your API key found [here](https://www.havenondemand.com/account/api-keys.html). `version` is an *optional* parameter which can be either `"v1"` or `"v2"`, but defaults to `"v1"` if not specified.

## Using HODClient with proxies
```   
MyProxy myProxy = new MyProxy(new Uri("proxyaddress:port"));
// if require login credentials
myProxy.Credentials = new System.Net.NetworkCredential("username", "password", "domain");
HODClient client = new HODClient("your-api-key", myProxy);
```
## Define and implement callback functions
You will need to implement callback functions to receive responses from Haven OnDemand server
```
client.requestCompletedWithContent += client_requestCompletedWithContent;
client.requestCompletedWithJobID += client_requestCompletedWithJobID;
client.onErrorOccurred += client_onErrorOccurred;
``` 

When you call the GetRequest() or PostRequest() with the ASYNC mode, the response will be returned to this callback function. The response is a JSON string containing the jobID.
```
private void HodClient_requestCompletedWithJobID(string response)
{
    // use the HODResponseParser to parse the response
    string jobID = parser.ParseJobID(response);
}
``` 

When you call the GetRequest() or PostRequest() with the SYNC mode, or call the GetJobResult() or GetJobStatus() functions, the response will be returned to this callback function. The response is a JSON string containing the actual result of the service.
```
private void HodClient_requestCompletedWithContent(string response)
{
    // use the HODResponseParser to parse the response
}
``` 

If there was an error occurred, the error message will be returned to this callback function.
```
private void HodClient_onErrorOccurred(string errorMessage)
{
    
}
```

## Sending requests to the API - GET and POST
You can send requests to the API with either a GET or POST request, where POST requests are required for uploading files and recommended for larger size queries and GET requests are recommended for smaller size queries.

### Function GetRequest
```
async Task GetRequest(Dictionary<String, Object> Params, String hodApp, REQ_MODE mode)
```

* `Params` is a dictionary object containing key/value pair parameters to be sent to a Haven OnDemand API, where the keys are the name of parameters of that API. 

>Note: For a value with its type is an array<>, the value must be defined in a List\<object\>. 
```
var entity_type = new List<object>();
entity_type.Add("people_eng");
entity_type.Add("places_eng");
var Params = new Dictionary<string, object>()
{
    {"url", "http://www.cnn.com" },
    {"entity_type", entity_type }
};
```

* `hodApp` a string to identify a Haven OnDemand API. E.g. "extractentities". Current supported apps are listed in the HODApps class.
* `mode` [REQ_MODE.ASYNC | REQ_MODE.SYNC]: specifies API call as Asynchronous or Synchronous.

*Example code:*
```
// Call the Entity Extraction API to find people and places from CNN and BBC website
String hodApp = HODApps.ENTITY_EXTRACTION;

var urls = new List<object>();
urls.Add("http://www.cnn.com");
urls.Add("http://www.bbc.com");

var entity_type = new List<object>();
entity_type.Add("people_eng");
entity_type.Add("places_eng");

var Params = new Dictionary<string, object>()
{
    {"url", urls },
    {"entity_type", entity_type }
};

await client.GetRequest(Params, hodApp, HODClient.REQ_MODE.SYNC);
```

### Function PostRequest
```
async Task PostRequest(Dictionary<String, Object> Params, String hodApp, REQ_MODE mode)
```

* `Params` a Dictionary object containing key/value pair parameters to be sent to a Haven OnDemand API, where the keys are the parameters of that API
----
> Note 1: If the key is the "file" parameter, the value must be a Dictionary<string,object> object, where string will be the filename and object is the file InputStream.
```
Dictionary<string, object> file = new Dictionary<string, object>();
file.Add(File1.PostedFile.FileName, File1.PostedFile.InputStream);
```
----
> Note 2: For a value with its type is an array<>, the value must be defined in a List\<object\>. 
```
var entity_type = new List<object>();
entity_type.Add("people_eng");
entity_type.Add("places_eng");

var Params = new Dictionary<string, object>()
{
    {"file", file },
    {"entity_type", entity_type }
};

await client.PostRequest(Params, HODApps.ENTITY_EXTRACTION, HODClient.REQ_MODE.SYNC);
```
----
* `hodApp` a string to identify a Haven OnDemand API. E.g. "ocrdocument". Current supported apps are listed in the HODApps class.

* `mode` [REQ_MODE.SYNC | REQ_MODE.ASYNC]: specifies API call as Asynchronous or Synchronous.

*Example code:*

```
// Call the OCR Document API to scan text from an image file
String hodApp = HODApps.OCR_DOCUMENT;
Dictionary<string, object> file = new Dictionary<string, object>();
file.Add(File1.PostedFile.FileName, File1.PostedFile.InputStream);
var Params =  new Dictionary<String,Object>
{
    {"file", file},
    {"mode", "document_photo"}
};
await client.PostRequest(Params, hodApp, HODClient.REQ_MODE.ASYNC);
```

### Function GetJobResult
```
async Task GetJobResult(String jobID)
```

* `jobID` the jobID returned from a Haven OnDemand API upon an asynchronous call.

*Example code:*
```
// Parse a JSON string contained a jobID and call the function to get the actual content from Haven OnDemand server
void client_requestCompletedWithJobID(string response)
{
    string jobID = parser.ParseJobID(response);
    if (jobID.Length > 0)
        await client.GetJobResult(jobID);
}
```

### Function GetJobStatus
```
async Task GetJobStatus(String jobID)
```
* `jobID1 the job ID returned from an Haven OnDemand API upon an asynchronous call.

*Example code:*

```
// Parse a JSON string contained a jobID and call the function to get the status of a call from Haven OnDemand API 
void client_requestCompletedWithJobID(string response)
{
    string jobID = parser.ParseJobID(response);
    if (jobID.Length > 0)
        await hodClient.GetJobStatus(jobID);
}
``` 

## Using HODResponseParser package
```
using HOD.Response.Parser;

HODResponseParser parser = new HODResponseParser();
```

### Function ParseJobID
```
string ParseJobID(string response)
```
* `response` a json string returned from an asynchronous API call.

*Return value:*
* The jobID or an empty string if not found.

*Example code:*
```
void client_requestCompletedWithJobID(string response)
{
    string jobID = parser.ParseJobID(response);
    if (jobID != "")
        await client.GetJobResult(jobID);
}
```

## Parse Haven OnDemand APIs' response

*Example code:*

```
// 
void hodClient_requestCompletedWithContent(string response)
{
    OCRDocumentResponse resp = parser.ParseOCRDocumentResponse(ref response);
    if (resp != null)
    {
        string text = "";
        foreach (OCRDocumentResponse.TextBlock obj in resp.text_block)
        {
            text += String.Format("Recognized text: {0}\n", obj.text);
            text += String.Format("Top/Left corner: {0}/{1}\n", obj.left, obj.top);
            text += String.Format("Width/Height: {0}/{1}\n", obj.width, obj.height);
        }
	Response.Write(text);
    }
    else
    {
        var errors = parser.GetLastError();
        foreach (HODErrorObject err in errors)
        {
            if (err.error == HODErrorCode.QUEUED)
            {
                // Task is in queue. Let's wait for a few second then call GetJobStatus() again
                await client.GetJobStatus(err.jobID);
                break;
            }
            else if (err.error == HODErrorCode.IN_PROGRESS)
            {
                // Task is In Progress. Let's wait for some time then call GetJobStatus() gain
                await client.GetJobStatus(err.jobID);
                break;
            }
            else // It is an error. Let's print out the error code, reason and detail
            {
                var text += err.error.ToString() + "<br/>";
                text += err.reason + "<br/>";
                text += err.detail + "<br/>";
		Response.Write(text);
            }
	}
    }
}
```

---
**Function ParseCustomResponse**
```
object ParseCustomResponse<T>(jsonStr)
```

* `<T>`: a custom class object.
* `jsonStr` a json string returned from Haven OnDemand APIs.

*Example code:*
```
// define a custom class for Query Text Index API response
public class QueryIndexResponse
{
    public List<Documents> documents;
    public int totalhits { get; set; }
    public class Documents
    {
        public string reference { get; set; } 
        public string index { get; set; }
        public double weight { get; set; } 
        public List<string> from { get; set; } 
        public List<string> to { get; set; } 
        public List<string> sent { get; set; } 
        public List<string> subject { get; set; } 
        public List<string> attachment { get; set; }
        public List<string> hasattachments { get; set; }
        public List<string> content_type { get; set; }
        public string content { get; set; } 
    }
}
void client_requestCompletedWithContent(string response)
{
    QueryIndexResponse resp = (QueryIndexResponse) parser.ParseCustomResponse<QueryIndexResponse>(ref response);
    if (resp != null)
    {
        foreach (QueryIndexResponse.Documents doc in resp.documents)
        {
	    // walk thru documents array
            var reference = doc.reference;
            var index = doc.index;
            var weight = doc.weight;
            if (doc.from != null)
            {
                var from = doc.from[0];
            }
            if (doc.to != null)
                var to = doc.to[0];
            
            // parse any other values
        }
    }
    else
    {
        var errors = parser.GetLastError();
        foreach (HODErrorObject err in errors)
        {
            if (err.error == HODErrorCode.QUEUED)
            {
                // Task is in queue. Let's wait for a few second then call GetJobStatus() again
                client.GetJobStatus(err.jobID);
                break;
            }
            else if (err.error == HODErrorCode.IN_PROGRESS)
            {
                // Task is In Progress. Let's wait for some time then call GetJobStatus() again
                client.GetJobStatus(err.jobID);
                break;
            }
            else // It is an error. Let's print out the error code, reason and detail
            {
                var text += err.error.ToString() + "<br/>";
                text += err.reason + "<br/>";
                text += err.detail + "<br/>";
		Response.Write(text);
            }
	}
    }
}
```

---
## Example: Example\SpeechRecognition Demo: 

**How to post a file and call the Speech Recognition API to extract text from speech from a media file. If the checkbox "Call Entity Extraction on result" is set, call the Entity Extraction API to extract people and places and companies_eng from the recognized text**


## Example: Example\SentimentAnalysis Demo:
 
**How to post multiple files and call the Sentiment Analysis API to analyze sentimental statements from files of input text**


## License
Licensed under the MIT License.