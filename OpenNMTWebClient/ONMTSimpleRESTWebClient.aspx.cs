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
            // sbSalida.Append(txtToT.Text + "<br />");
            // auxS += "Input text (HTML encoded):<br />";
            // auxS += Server.HtmlEncode(txtToT.Text) +"<br />";
            inputText = txtToT.Text;
            // ... 

            char[] delim = ".?!;".ToCharArray(); // delimiters end of sentence
            foreach (char c in delim)
            {
                inputText = inputText.Replace(c.ToString(), c.ToString() + Environment.NewLine);
            }
            // remove New line from \d\.Enviroment.Newline\d

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
            //
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
            // dividing
            sentences = inputText.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            // removing blank entries
            sentences = sentences.Where(x => !string.IsNullOrEmpty(x.Trim())).ToArray();
            //foreach (string s in sentences) {
            //    sbSalida.Append(string.Format("Sentence: {0} <br />" , s));
            // }

            var RESTClientData = new RESTClientDataC()
            { // SL and indicator
                sentenceList = sentences,
                todoOK = false,
                SentecesListREST = new List<RESTSentence>()
            };
            var rClient = new RESTClient("www.mknals.com", 4031);
            rClient.TranslateRESTClientData(RESTClientData);

            if (RESTClientData.todoOK)
            {
                Console.WriteLine("OK");
                // Translation inside RESTClientDATA
                // we need to search in each RESTSEentence
                foreach (RESTSentence RS in RESTClientData.SentecesListREST)
                {
                    Console.WriteLine(RS.TargetTranslationONMTREST.tgt);
                    auxS = "<br>{0}<br><b>{1}</b> - ({2})<br />";
                    sbSalida.Append(string.Format(auxS,
                        RS.sourceREST,
                        RS.TargetTranslationONMTREST.tgt,
                        RS.TargetTranslationONMTREST.pred_score));

                }
                Console.WriteLine("End");
            }
            else
            {
                Console.WriteLine("Translation ERROR");
            }



            lblOut.Text = sbSalida.ToString();
            // Aquí el código para llamar 
        }

      

        
    }
}