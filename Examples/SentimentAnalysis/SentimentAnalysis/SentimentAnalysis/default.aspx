<%@ Page Language="C#" AutoEventWireup="true" Async="true" AsyncTimeout="3600" CodeBehind="default.aspx.cs" Inherits="SentimentAnalysis._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>HOD Client demo</title>
</head>
<body>
    <h2>HOD Sentiment Analisys Demo</h2>
    <form id="form1" method="post" enctype="multipart/form-data" runat="server">
    <div>
        <span>Select text files</span>
        <input type="file" id="File1" name="File1" multiple="Multiple" runat="server" /> <br/>
        <span>Or provide text</span>
        <input type="text" id="inputtext" name="text" size="100" runat="server"/> <br/>
        <span>Select the text language: </span>
        <asp:DropDownList ID="textlanguage" runat="server">
            <asp:ListItem Value="eng" Selected="True" Text="English" />
            <asp:ListItem Value="spa" Text="Spanish" />
            <asp:ListItem Value="fre" Text="French" />
            <asp:ListItem Value="ger" Text="German" />
            <asp:ListItem Value="ita" Text="Italian" />
            <asp:ListItem Value="chi" Text="Chinese" />
            <asp:ListItem Value="por" Text="Portuguese" />
            <asp:ListItem Value="rus" Text="Russian" />
            <asp:ListItem Value="cez" Text="Czech" />
            <asp:ListItem Value="tur" Text="Turkish" />
            <asp:ListItem Value="dut" Text="Dutch" />
        </asp:DropDownList><br/>
        <asp:Button ID="Button1" Text="Post" runat="server" onClick="uploadButton_Clicked"/>
    </div>
    </form>
    <br/>
    <div>Result:</div>
    <div id="result" runat="server" />
</body>
</html>
