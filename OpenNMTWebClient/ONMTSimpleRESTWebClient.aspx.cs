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
                lblOut.Text = "Pls, type/paste some text.";
                return;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            StringBuilder sbSalida = new StringBuilder("");
              
            string auxS = "";
            string inputText = "";
            string[] sentences;
            sbSalida.Append("Input text (uncoded):<br />");
            inputText = txtToT.Text;
            // ... 

            char[] delim = ".?!;".ToCharArray(); // delimiters end of sentence
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

            var RESTClientData = new RESTClientDataC(sentences); // only parameter senteces
            var rClient = new RESTClient("www.mknals.com", 4031); // Rest client
            //rClient.TranslateRESTClientData(RESTClientData);
            rClient.TranslateRESTClientData1by1(RESTClientData);

            if (RESTClientData.todoOKREST) // translation was fine
            {
                Console.WriteLine("OK");
                // Translation inside RESTClientDATA
                // we need to search in each RESTSEentence
                for (int i = 0; i < RESTClientData.ListSourceONMT.Count; i++)
                {
                    auxS = "<br>{1}<br>{0}<br><b>{2}</b> - ({3})<br />";
                    sbSalida.Append(string.Format(auxS,
                        RESTClientData.ListSourceONMT[i].src,
                        RESTClientData.ListTargetONMT[i][0].src, 
                        RESTClientData.ListTargetONMT[i][0].tgt,
                        RESTClientData.ListTargetONMT[i][0].pred_score));
                    
                }
                                
            }
            else
            {
                Console.WriteLine("Translation ERROR");
                sbSalida.Append("Translation ERROR in RESTClientData.todoOKREST" + "<br>");
                sbSalida.Append(RESTClientData.infoREST + "<br>");
            }

            
            lblOut.Text = sbSalida.ToString()+"<br>" ;
            // Aquí el código para llamar 
        }

      

        
    }
}