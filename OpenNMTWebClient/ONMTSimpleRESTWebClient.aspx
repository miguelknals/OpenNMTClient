<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ONMTSimpleRESTWebClient.aspx.cs" Inherits="OpenNMTWebClient.ONMTSimpleRESTWebClient" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <br />
        OpenNMT CA-&gt;ES model. Type or paste some text.<br />
    
    </div>
        <asp:TextBox ID="txtToT" runat="server" Height="202px" TextMode="MultiLine" Width="368px">Aprovat inicialment el projecte. Parlem una mica de català. Com m&#39;agrada veure els partits del Barça.</asp:TextBox>
&nbsp;<br />
        <asp:Button ID="btnRunTranslation" runat="server" OnClick="Button1_Click" Text="Run Translation" />
        <asp:Label ID="lblOut" runat="server" Text="lblOut"></asp:Label>
    </form>
</body>
</html>
