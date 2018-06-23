<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ONMTSimpleRESTWebClient.aspx.cs" Inherits="OpenNMTWebClient.ONMTSimpleRESTWebClient" ValidateRequest="false" %>
<link href="OpenNMTWebClient.css" rel="stylesheet" type="text/css" />
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            height: 212px;
        }
        .auto-style3 {
            height: 23px;
        }
        .auto-style4 {
            height: 26px;
        }
        .auto-style5 {
            height: 43px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <table style="width:100%;">
            <tr>
                <td></td>
                <td><h2><b>OpenNMTWebClient - C# OpenNMT REST API Web Client</b></h2></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    OpenNMTWebClientis a no-so-simple vs2013 C# asp.net .net 4.5.1 API REST client to call an OpenNMT REST server.
                    <br />
                    <br />
                    <b>By default this page uses a CATALAN -&gt; SPANISH model. This model uses the Diari Oficial de la Generalitat de Catalunya corpus. You can try to translate from catalan to spanish for instance any text from:</b><br />
                    <br />
                    <a href="http://dogc.gencat.cat/ca/pdogc_canals_interns/pdogc_resultats_fitxa/?action=fitxa&amp;documentId=820643&amp;language=ca_ES">http://dogc.gencat.cat/ca/pdogc_canals_interns/pdogc_resultats_fitxa/?action=fitxa&amp;documentId=820643&amp;language=ca_ES</a><br />
                    <br />
                    This is proof of concept/development server, no availability or perfomance guarantee.<br />
                    <br />
                    If you have an OpenNMT API REST Server, you can use it. Click on the client settings hyperlink below.
                    The code expects a case feature mode in the target side. Tokenization, if any, on the server side. <br />
                    <br />
                    Expected command in the server:<br />
&nbsp;<br />
                    th tools/rest_translation_server.lua -host&nbsp; your.host.com -port nnn -case_feature true \<br />
                    [-joiner_annotate true -joiner ￭ -mode aggressive -replace_unk -segment_numbers] \<br />
                    -model /your/model.t7 [-gpuid 1]<br />
                    <br />
                    - You can find the source and the readme file in <a href="https://github.com/miguelknals/OpenNMTClient">https://github.com/miguelknals/OpenNMTClient</a><br />
                    <br />
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style3"></td>
                <td class="auto-style3">
                    <asp:LinkButton ID="lnkShowHidePanel" runat="server" OnClick="lnkShowHidePanel_Click">LinkButton</asp:LinkButton>
                </td>
                <td class="auto-style3"></td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    <asp:Panel ID="pnlSettings" runat="server">
                        <table style="width:100%;">
                            <tr>
                                <td>REST host</td>
                                <td>
                                    <asp:TextBox ID="txtHost" runat="server" Width="598px">16.6.19.12</asp:TextBox>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td class="auto-style4">Port</td>
                                <td class="auto-style4">
                                    <asp:TextBox ID="txtPort" runat="server">4031</asp:TextBox>
                                </td>
                                <td class="auto-style4"></td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>
                                    <asp:CheckBox ID="chkSendAll" runat="server" Text="Send all senteces in 1 REST call. Warning: Consumes more memory on the server (otherwise 1 REST call/per sentence). " />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>
                                    <asp:CheckBox ID="chkAddInfo" runat="server" Text="Additional data traffic info" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td class="auto-style5"></td>
                                <td class="auto-style5">
                                    <asp:CheckBox ID="chkSegmentBasedOnNewline" runat="server" Text="Sentence segmentation ONLY based on newline (code does not to segment senteces i.e. based in full stops)." />
                                </td>
                                <td class="auto-style5"></td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>
                                    &nbsp;</td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>
                                    <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save variables (cookie)" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                                <td>
                                    <asp:Label ID="lblCookie" runat="server" Text="lblCookie"></asp:Label>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
                <td>&nbsp;</td>
            </tr>
            </table>
        <table>
            <tr>
                <td>Type or paste some text. By default there is a sentence segmentation. Performance depends of GPU use in server and single line/multiple line calls</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="auto-style1">
        <asp:TextBox ID="txtToT" runat="server" Height="233px" TextMode="MultiLine" Width="745px"></asp:TextBox>
                </td>
                <td class="auto-style1">
                    &nbsp;</td>
                <td class="auto-style1"></td>
            </tr>
            <tr>
                <td>
        <asp:Button ID="btnRunTranslation" runat="server" OnClick="Button1_Click" Text="Run Translation" />
                </td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
        <asp:Label ID="lblOut" runat="server" Text="lblOut"></asp:Label>
                    <br />
                    <br />
                    (c) 2018 miguel canals -&nbsp; <a href="http://www.mknals.com">www.mknals.com</a> - MIT License</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
        </table>
        <br />
        <br />
    
    </div>
&nbsp;<br />
    </form>
</body>
</html>
