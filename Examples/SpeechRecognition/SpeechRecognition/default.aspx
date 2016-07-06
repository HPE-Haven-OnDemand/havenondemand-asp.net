<%@ Page Language="C#" AutoEventWireup="true" Async="true" AsyncTimeout="4000" CodeBehind="default.aspx.cs" Inherits="SpeechRecognition._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>HOD Speech Recognition</title>
</head>
<body>
    <h2>HOD Speech Recognition Demo</h2>
    <form id="form1" method="post" enctype="multipart/form-data" runat="server">
    <div>
        <span>Select a media file: </span>
        <input type="file" id="File1" name="File1" runat="server" /> <br/>
        <span>Select the speech language: </span>
        <asp:DropDownList ID="speechlanguage" runat="server">
            <asp:ListItem Value="en-US" Text="Eng US" />
            <asp:ListItem Value="en-US-tel" Text="eng US Tel" />
            <asp:ListItem Value="en-GB" Text="Eng GB" />
            <asp:ListItem Value="en-GB-tel" Text="Eng GB Tel" />
            <asp:ListItem Value="en-AU" Text="Eng Australian" />
            <asp:ListItem Value="en-CA" Text="English Canadian" />
            <asp:ListItem Value="es-ES" Text="Spanish Spain" />
            <asp:ListItem Value="es-ES-tel" Text="Spanish Spain Tel" />
            <asp:ListItem Value="es-LA" Text="Spanish LATAM" />
            <asp:ListItem Value="es-LA-tel" Text="Spanish LATAM Tel" />
            <asp:ListItem Value="de-DE" Text="Dutch" />
            <asp:ListItem Value="fr-FR" Text="French" />
            <asp:ListItem Value="fr-FR-tel" Text="French Tel" />
            <asp:ListItem Value="it-IT" Text="Italian" />
            <asp:ListItem Value="zh-CN" Text="Chinese Mandarin" />
            <asp:ListItem Value="ru-RU" Text="Rusian" />
            <asp:ListItem Value="pt-BR" Text="Portuguese" />
            <asp:ListItem Value="nl-NL" Text="Netherland" />
            <asp:ListItem Value="fa-IR" Text="Farsi Iran" />
            <asp:ListItem Value="ar-MSA" Text="Arabic MSA" />
            <asp:ListItem Value="ja-JP" Text="Japanese" />
        </asp:DropDownList><br/>
        <span>Call Entity Extraction on result: </span>
        <input type="checkbox" id="chainapi" runat="server" /><br/>
        <asp:Button ID="Button1" Text="Post" runat="server" onClick="uploadButton_Clicked"/>
    </div>
    </form>
    <br/>
    <div>Result:</div>
    <div id="result" runat="server" />
</body>
</html>
