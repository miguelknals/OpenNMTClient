// (c) 2018 miguel canals -  www.mknals.com - MIT License
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Text.RegularExpressions;



// Request validation disable for <xxx> allowed.
// https://stackoverflow.com/questions/3656523/how-to-disable-html-tag-validation-on-a-specific-textbox
namespace OpenNMTWebClient
{
    public partial class ONMTSimpleRESTWebClient : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                lnkShowHidePanel.Text = "Click here to display REST settings";
                pnlSettings.Visible = false;
                lblCookie.Text = ""; lblOut.Text = "";
                // Let's see If I have a cookie with settings
                HttpCookie myCookie = Request.Cookies["ONMTCookie"];                
                if (myCookie != null) {
                    // cookie format OPENÇhostÇportÇch
                    string s = myCookie.Value;
                    string[] words = s.Split('Ç');
                    if (words.Count() == 6) // Cookie has 6 items
                    {
                        if (words[0].CompareTo("OPEN")==0) // first is open
                        {
                            txtHost.Text = words[1]; txtPort.Text = words[2];
                            chkSendAll.Checked = words[3] == "1";  // true if 1
                            chkAddInfo.Checked = words[4] == "1";  // true if 1
                            chkSegmentBasedOnNewline.Checked = words[5] == "1";  // true if 1
                        }
                    }
                    else
                    {
                        lblCookie.Text += "<font color='red'>Ops.. wrong cookie.</font>" + "<br>";
                    }                   
                }

                lblCookie.Text += "Enter your settings and save them if you want so.";
                return;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            // before we do antying we will verifiy if we have a valid URL
              
                try { 
                    var kk= new Uri(string.Format("http://{0}:{1}/", txtHost.Text, txtPort.Text)); }
                catch {
                    pnlSettings.Visible = true ;
                    lnkShowHidePanel.Text = "Click here to hide REST settings";
                    lblCookie.Text = "<font color='red'>Change (and save) connections setings as I cannot create a valid URL</font> ";
                    return;
                }



        
            StringBuilder sbSalida = new StringBuilder("");
            lblOut.Text = "";  
            string auxS = "";
            string inputText = "";
            string[] sentences;
            sbSalida.Append("Input text (uncoded):<br />");
            inputText = txtToT.Text;
            // ... 

            char[] delim = ".?!;".ToCharArray(); // delimiters end of sentence
            if (chkSegmentBasedOnNewline.Checked == false)
            {
                foreach (char c in delim)
                {
                    inputText = inputText.Replace(c.ToString(), c.ToString() + Environment.NewLine);
                }
                // remove New line from \d\.Enviroment.Newline\d p.e. 334.\r\n\344

                string pattern = @"\d\.(\n|\r|\r\n)\d";
                var maches = Regex.Matches(inputText, pattern);
                foreach (Match mach in maches)
                {
                    auxS = mach.Groups[0].Value;
                    string auxS2 = auxS.Replace("\r\n", "");
                    auxS2 = auxS2.Replace("\n", "");
                    auxS2 = auxS2.Replace("\r", "");
                    inputText = inputText.Replace(auxS, auxS2);
                }
                // elipses
                var regex = new Regex(@"\.(\n|\r|\r\n)\.");
                Match match = regex.Match(inputText);
                if (match.Success)
                {
                    string strControl;
                    do
                    {
                        strControl = inputText;
                        inputText = inputText.Replace(".\r\n.", "..");
                    }
                    while (strControl.CompareTo(inputText) != 0);
                }
            
            
            }

            // apostrophe quotes
            // \u2018  LEFT SINGLE QUOTATION MARK	‘
            // \u2019 ’ 
            // \u201C LEFT DOUBLE QUOTATION MARK	“ 	
            // \u201D RIGHT DOUBLE QUOTATION MARK	”
            char[] tipochar = new char[] { '\u2018',
                '\u2019',  '\u201C', '\u201D' };
            int index = inputText.IndexOfAny(tipochar);
            if (index != -1)
            {
                inputText = inputText.Replace("\u2018", "'");
                inputText = inputText.Replace("\u2019", "'");
                inputText = inputText.Replace("\u201C", "\"");
                inputText = inputText.Replace("\u201D", "\"");

            }

            string[] separators = { Environment.NewLine }; // only one
            sentences = inputText.Split(separators, StringSplitOptions.RemoveEmptyEntries); // splitting
            sentences = sentences.Where(x => !string.IsNullOrEmpty(x.Trim())).ToArray(); // removing blanks

            for (int i=0 ; i < sentences.Length ; i += 1)
            {
                sentences[i] = sentences[i].Trim();
            }
            
            var RESTClientData = new RESTClientDataC(sentences); // only parameter senteces
            var rClient = new RESTClient(txtHost.Text, Int32.Parse(txtPort.Text)); // Rest client
            
            if (chkSendAll.Checked) { // sen all as 1 REST request
                rClient.TranslateRESTClientData(RESTClientData);
            } else { // Send senteces 1 by 1
                rClient.TranslateRESTClientData1by1(RESTClientData);
            }
             
            
            if (RESTClientData.todoOKREST) // translation was fine
            {
                Console.WriteLine("OK");
                // Translation inside RESTClientDATA
                // we need to search in each RESTSEentence
                
                for (int i = 0; i < RESTClientData.ListSourceONMT.Count; i++)
                {
                    if (chkAddInfo.Checked) { 
                    auxS = "<br>{1}<br>{0}<br><b>{2}</b> - ({3})<br />";
                    sbSalida.Append(string.Format(auxS,
                        RESTClientData.ListSourceONMT[i].src,
                        RESTClientData.ListTargetONMT[i][0].src, 
                        RESTClientData.ListTargetONMT[i][0].tgt,
                        RESTClientData.ListTargetONMT[i][0].pred_score));
                    } else {
                        auxS = "<br>{0}<br><b>{1}</b><br>";
                        sbSalida.Append(string.Format(auxS,
                            RESTClientData.ListSourceONMT[i].src,
                            RESTClientData.ListTargetONMT[i][0].tgt));
                    }

                                        
                }
                                
            }
            else
            {
                sbSalida.Append("Translation ERROR");
                sbSalida.Append("Translation ERROR in RESTClientData.todoOKREST" + "<br>");
                sbSalida.Append(RESTClientData.infoREST + "<br>");
            }

            
            lblOut.Text = sbSalida.ToString()+"<br>" ;
            // Aquí el código para llamar 
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblCookie.Text = "";
            Regex tengocedidlla = new Regex("ç");
            if (tengocedidlla.IsMatch(txtHost.Text + txtPort.Text)) {
                lblCookie.Text = "<font color='red'>You cannot specifiy 'Ç' as it is used as separator</font>";
                return;
            }
            int x;
           if  (!(int.TryParse(txtPort.Text, out x) && x > 0))
           {
               lblCookie.Text = String.Format("<font color='red'>Cannot convert '{0}' to a valid positive integer</font>", txtPort.Text);
                return;
            }
            
             
            DateTime now = DateTime.Now;
            HttpCookie myCookie = Request.Cookies["ONMTCookie"];
            string textValue = "OPENÇ" + txtHost.Text + "Ç" + x.ToString() +
                "Ç" + (chkSendAll.Checked ? "1" : "0" ) +
                "Ç" + (chkAddInfo.Checked ? "1" : "0") + // if true 1 else 0
                "Ç" + ( chkSegmentBasedOnNewline.Checked ? "1" : "0"); // if true 1 else 0
            if (myCookie != null) 
            {
                myCookie.Value = textValue;
                myCookie.Expires = now.AddYears(50); // Don't forget to reset the Expires property!
                Response.SetCookie(myCookie);
                lblCookie.Text = "<font color='green'>The cookie has been updated.</font>";
            }
            else
            { // does not exist
                myCookie = new HttpCookie("ONMTCookie");
                myCookie.Value = textValue;             // Set the cookie value.            
                myCookie.Expires = now.AddYears(50); // For a cookie to effectively never expire                
                Response.Cookies.Add(myCookie); // Add the cookie.
                lblCookie.Text = "<font color='green'>The cookie has been written.</font>";
            }

        }

        protected void lnkShowHidePanel_Click(object sender, EventArgs e)
        {
            if (pnlSettings.Visible) {
                pnlSettings.Visible = false;
                lnkShowHidePanel.Text = "Click here to display REST settings";

            } else {
                pnlSettings.Visible = true; 
                lnkShowHidePanel.Text = "Click here to hide REST settings";


            }
        }
    
        
    }
}