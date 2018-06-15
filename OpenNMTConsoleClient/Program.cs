using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Web.Script.Serialization; // para serializer need to now add reference System.Web.Extensions
using OpenNMTWebClient;
      

namespace OpenNMTConsoleClient
{
    public static class Globals
    {
        public const Int32 BUFFER_SIZE = 10; // Unmodifiable
        public static String FILE_NAME = "Output.txt"; // Modifiable
        public static readonly String CODE_PREFIX = "US-"; // Unmodifiable
        // these come from command line.
        // public static String  INPUTFILENAME =@"c:\tmp\tmp.txt";
        // public static String HOST="www.mknals.com";
        // public static Int16 PORT=4300    ;
     }
    public class classCommandArguments
    {
        public bool todoOK { get; set; }
        public string inputFile { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
        public bool bBitxt { get; set; }
        public bool bJSON { get; set; }
        public bool bSenSeg { get; set; }
        public string info { get; set; }
        string[] args { get; set; }
        public classCommandArguments(string[] args)
        { // default constructor uses args[] and if todoK 
            this.todoOK = true; // optimism
            this.info = "";
            if (args.Length == 0)
            {
                this.info = "Please enter a numeric argument.";
                this.todoOK = false;
                return;
            }
            string nl = Environment.NewLine; // string parseInfo = "";
            bool bHost = false; bool bPort = false; bool bFile = false;
            // bool bBitxt = false; bool bJSON = false; bool bSenSeg = false;
            string Host = ""; int Port = 0; string inputFile = "";
            for (int i = 0; i < args.Count(); i++)
            {
                if (args[i].ToUpper().Equals("-H"))
                {
                    try { Host = args[i + 1]; args[i] = ""; args[i + 1] = ""; bHost = true; this.Host = Host; }
                    catch { this.info += "Host -> No info after -h parameter" + nl; }
                }
                if (args[i].ToUpper().Equals("-P"))
                {
                    try
                    {
                        string tmpS;
                        // int x;
                        tmpS = args[i + 1]; args[i] = ""; args[i + 1] = "";
                        if ((int.TryParse(tmpS, out Port) && Port > 0))
                        {
                            bPort = true; this.Port = Port;
                        }
                    }
                    catch { } // if bPort = false error.
                    this.info += (!bPort) ? "Port -> No info after -p parameter or cannot be converted to integer" + nl : "";
                }
                if (args[i].ToUpper().Equals("-F"))
                {
                    try
                    {
                        inputFile = args[i + 1]; args[i] = ""; args[i + 1] = "";
                        bFile = File.Exists(inputFile);
                        this.inputFile = inputFile;
                    }
                    catch { } // if bPort = false error.
                    this.info += (!bFile) ? string.Format(
                        "File -> No info after -f parameter of file '{0}' does not exist. ",
                        inputFile) + nl : "";
                }
                if (args[i].ToUpper().Equals("-BITEXT") | args[i].ToUpper().Equals("-B"))
                { this.bBitxt = true; args[i] = ""; }
                if (args[i].ToUpper().Equals("-JSON") | args[i].ToUpper().Equals("-J"))
                { this.bJSON = true; args[i] = ""; }
                if (args[i].ToUpper().Equals("-SENSEG") | args[i].ToUpper().Equals("-S"))
                { this.bJSON = true; args[i] = ""; }
                                
            }
            if (!(bHost && bPort && bFile && bFile))
            {
                // Console.WriteLine("Parser error " + nl + parseInfo);
                this.todoOK = false;
                // string CommandArguments = "-h www.mkanls.com -p 4301 -f c:\tmp\tmp.txt [-EOFT] ";
                this.info += nl;
                this.info += "Valid format is:" + nl;
                this.info += "OpenNMTConsoleClient -h host -p port -f inputfile [-B[ITEXT]] [-J[SON] [-S[ENSEG]" + nl;

            }

        

        }
    }    
     public class ListaSorce
     {
         public List<SourceONMT> entries { get; set; }
     }
     public class SourceONMT // text to be translate
     {
         [ScriptIgnore]
         public string ignora { get; set; }

         public string src { get; set; }
     }
    //
    // output Json
    //
     public class JsonItem
     {
         public int n_best { get; set; }
         public double pred_score { get; set; }
         public string src_untok { get; set; }
         public string src { get; set; }
         public string tgt { get; set; }
     }
     public class JsonFull
     {
         public List<JsonItem> ListJsonItems { get; set; } // List   
         public JsonFull()
         {
             ListJsonItems = new List<JsonItem>();            
        }
     }


     class Program    {
                
        static void Main(string[] args)
        {
            // string CommandArguments = "-h www.mkanls.com -p 4301 -f c:\tmp\tmp.txt [-EOFT] ";
            classCommandArguments argumentos = new classCommandArguments(args);
            if (argumentos.todoOK == false) {
                System.Console.WriteLine("Error parsing arguments");
                System.Console.WriteLine(argumentos.info);
                return;
            }


            string INPfile = argumentos.inputFile;
            string INPfilePath = Path.GetDirectoryName(INPfile);
            string INPfileFilenameWithoutExtension = Path.GetFileNameWithoutExtension(INPfile);
            // string[] idioma = new string[10];
            string RESThost = argumentos.Host;
            int RESTport = argumentos.Port;

            StreamReader sr = new StreamReader(INPfile, Encoding.UTF8, true); // input file
            string OUTfilesrc;
            OUTfilesrc = Path.GetDirectoryName(INPfile) + "\\" +
                Path.GetFileNameWithoutExtension(INPfile) + ".src" + 
                Path.GetExtension(INPfile);
            string OUTfiletgt;
            OUTfiletgt = Path.GetDirectoryName(INPfile) + "\\" +
                Path.GetFileNameWithoutExtension(INPfile) + ".tgt" +
                Path.GetExtension(INPfile);
            string OUTfilejson;
            OUTfilejson = Path.GetDirectoryName(INPfile) + "\\" +
                Path.GetFileNameWithoutExtension(INPfile) + ".json";
            
            List<String> listsentences = new List<String>();
            // apostrophe quotes
            //
            // \u2018  LEFT SINGLE QUOTATION MARK	‘
            // \u2019 ’ 
            // \u201C LEFT DOUBLE QUOTATION MARK	“ 	
            // \u201D RIGHT DOUBLE QUOTATION MARK	”
            char[] tipochar = new char[] { '\u2018',
                '\u2019',  '\u201C', '\u201D' };
            try
            {
                using (sr)
                {
                    while (sr.Peek() != -1)
                    {
                        // Read the stream to a string, and write the string to the console.
                        String line = sr.ReadLine(); 
                        //Console.WriteLine(line);                   
                        line= line.Trim();
                        if (line.Length > 0) 
                        { // we need a line
                            int index = line.IndexOfAny(tipochar); // quote replacing
                            if (index != -1) // cleanup
                            {
                                line = line.Replace("\u2018", "'");
                                line = line.Replace("\u2019", "'");
                                line = line.Replace("\u201C", "\"");
                                line = line.Replace("\u201D", "\"");

                            }
                            listsentences.Add(line); // add the sentence to list of senteces
                            Console.WriteLine(line); 
                        }

                    }
                    sr.Close();                    
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Fatal error.");
                Console.WriteLine(String.Format("The file {0} could not be read:",INPfile ));
                Console.WriteLine(e.Message);
                return;
            }
            // I have my list of sentences
            String[] sentences = listsentences.ToArray(); // List of strings to string array
            
            var RESTClientData = new RESTClientDataC(sentences); // only parameter senteces
            var rClient = new RESTClient(RESThost, RESTport); // Rest client
            if (false)
            { // sen all as 1 REST request
                rClient.TranslateRESTClientData(RESTClientData);
            }
            else
            { // Send senteces 1 by 1
                rClient.TranslateRESTClientData1by1(RESTClientData);
            }

            if (RESTClientData.todoOKREST) // translation was fine
            {
                // create a class with all the data I need
                JsonFull outputJson = new JsonFull();
                
                for (int i = 0; i < RESTClientData.ListSourceONMT.Count; i++)
                {
                    JsonItem ele = new JsonItem();
                    ele.n_best = RESTClientData.ListTargetONMT[i][0].n_best;
                    ele.pred_score = RESTClientData.ListTargetONMT[i][0].pred_score;
                    ele.src_untok = RESTClientData.ListSourceONMT[i].src;
                    ele.src = RESTClientData.ListTargetONMT[i][0].src;
                    ele.tgt = RESTClientData.ListTargetONMT[i][0].tgt;
                    outputJson.ListJsonItems.Add(ele);
                }
                
                StreamWriter swrOutjson = new StreamWriter(OUTfilejson);
                try
                {
                    using (swrOutjson)
                    {
                        var json = new JavaScriptSerializer().Serialize(outputJson);
                        swrOutjson.WriteLine(json);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Fatal error with output files.");
                    Console.WriteLine(String.Format("Files {0}", OUTfilejson));
                    Console.WriteLine(e.Message);
                    return;
                }

            }

            


            if (RESTClientData.todoOKREST) // translation was fine
            {
                StreamWriter swrOutsrc = new StreamWriter(OUTfilesrc);
                StreamWriter swrOuttgt = new StreamWriter(OUTfiletgt);
                
                try
                {
                    using (swrOutsrc)
                    {
                        using (swrOuttgt)
                        {
                            for (int i = 0; i < RESTClientData.ListSourceONMT.Count; i++)
                            {
                                swrOutsrc.WriteLine(RESTClientData.ListSourceONMT[i].src);
                                swrOuttgt.WriteLine(RESTClientData.ListTargetONMT[i][0].tgt);
                            }

                        }

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Fatal error with output files.");
                    Console.WriteLine(String.Format("Files {0}/{1}", OUTfilesrc, swrOuttgt));
                    Console.WriteLine(e.Message);
                    return;
                }            
            }
            else
            {
                Console.WriteLine("Translation ERROR");
                Console.WriteLine("Translation ERROR in RESTClientData.todoOKREST" + "<br>");
                Console.WriteLine(RESTClientData.infoREST + "<br>");

            }

            

            

            
            if (RESTClientData.todoOKREST) // translation was fine
            {
                Console.WriteLine("OK");
                // Translation inside RESTClientDATA
                // we need to search in each RESTSEentence
                bool verbose = true;
                verbose = false;
                for (int i = 0; i < RESTClientData.ListSourceONMT.Count; i++)
                {
                    if (verbose)
                    {
                        string auxS = "<br>{1}<br>{0}<br><b>{2}</b> - ({3})<br>";
                        auxS = string.Format(auxS,
                            RESTClientData.ListSourceONMT[i].src,
                            RESTClientData.ListTargetONMT[i][0].src,
                            RESTClientData.ListTargetONMT[i][0].tgt,
                            RESTClientData.ListTargetONMT[i][0].pred_score);
                        auxS = auxS.Replace("<br>", Environment.NewLine);
                        auxS = auxS.Replace("<b>", "");
                        auxS = auxS.Replace("</b>", "");
                        Console.WriteLine(auxS);
                        
                    }
                    else
                    {
                        string auxS = "<br>{0}<br><b>{1}</b><br>";
                        auxS= string.Format(auxS,
                            RESTClientData.ListSourceONMT[i].src,
                            RESTClientData.ListTargetONMT[i][0].tgt);
                        auxS = auxS.Replace("<br>", Environment.NewLine);
                        auxS = auxS.Replace("<b>", "");
                        auxS = auxS.Replace("</b>", "");
                        Console.WriteLine(auxS);
                    }


                }

            }
            else
            {
                Console.WriteLine("Translation ERROR");
                Console.WriteLine("Translation ERROR in RESTClientData.todoOKREST" + "<br>");
                Console.WriteLine(RESTClientData.infoREST + "<br>");
            }

            Console.ReadLine();

        }
    }
}
