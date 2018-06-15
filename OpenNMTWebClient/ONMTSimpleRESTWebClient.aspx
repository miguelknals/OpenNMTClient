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
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <table style="width:100%;">
            <tr>
                <td></td>
                <td><h2><b>Simple OpenNMTWeb C# Client</b></h2></td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>
                    This page is an vs2013 C# asp.net .net 4.5.1 sample to call an OpenNMT REST server.
                    <br />
                    <br />
                    - The default 
                    REST server is inside my intranet. Notice is a development server. <b>No service guarantee at all</b>. Usually ES->CA or ES->PT<br />
                    <br />
                    - But the idea is that you can set your OpenNMT REST server you can access from the web server.
                    The code expects a case feature mode in the target side. Tokenization, if any, on the server side. <br />
                    <br />
                    Expected command in the server:
                    <br />
                    th tools/rest_translation_server.lua -host&nbsp; your.host.com -port nnn -case_feature true \<br />
                    [-joiner_annotate true -joiner ￭ -mode aggressive -replace_unk -segment_numbers] \<br />
                    -model /your/model.t7 [-gpuid 1]<br />
                    <br />
                    - You can find the source in <a href="https://github.com/miguelknals/OpenNMTClient">https://github.com/miguelknals/OpenNMTClient</a><br />
                    <br />
                    - Any info, pls, let me know infomknals at gmail.com.<br />
                    <br />
                    - (c) 2018 miguel canals -&nbsp; <a href="http://www.mknals.com">www.mknals.com</a> - MIT License<br />
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
                                <td>Port</td>
                                <td>
                                    <asp:TextBox ID="txtPort" runat="server">4031</asp:TextBox>
                                </td>
                                <td>&nbsp;</td>
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
                                <td>&nbsp;</td>
                                <td>
                                    <asp:CheckBox ID="chkSegmentBasedOnNewline" runat="server" Text="Sentence segmentation ONLY based on newline (code does not to segment senteces i.e. based in full stops)." />
                                </td>
                                <td>&nbsp;</td>
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
                                    <asp:Button ID="btnSave" runat="server" OnClick="btnSave_Click" Text="Save variables" />
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
&nbsp;<br />
        <table>
            <tr>
                <td>Type or paste some SPANISH text.<br />
                    -Target language probably it should be portuguese or catalan.<br />
                    -Each &lt;LF&gt;&lt;CR&gt; is considered an end of sentence, but also, a BASIC segmentation<br />
&nbsp;is in place by default.</td>
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
                </td>
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
