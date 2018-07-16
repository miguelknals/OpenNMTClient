// (c) 2018 miguel canals -  www.mknals.com - MIT License
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Web.Script.Serialization; // para serializer need to now add reference System.Web.Extensions
// using OpenNMTWebClient;
      

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
        public string info { get; set; }
        string[] args { get; set; }
        public classCommandArguments(string[] args)
        { // default constructor uses args[] and if todoK 
            string nl = Environment.NewLine; // string parseInfo = "";
            string help = "";
            help += nl;
            help += "This program will read an inputfile to be translated. Each line must " +nl;
            help += "contain ONE sentence to translate. " + nl;
            help += nl;
            help += "You need and OpenNMT API Server (host and port)" + nl;
            help += nl;
            help += "It will create 3 files. inputfile.src.txt, inputfile.tgt.txt and inputfile.json" + nl;
            help += nl;
            help += "Valid format is:" + nl;
            help += "OpenNMTConsoleClient -h host -p port -f inputfile " + nl;
            
            this.todoOK = true; // optimism
            this.info = "";
            if (args.Length == 0)
            {
                this.info = "You have to provide a OpenNMT host and port and an input file. " +nl; 
                this.info += help;
                this.todoOK = false;
                return;
            }
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
                                
            }
            if (!(bHost && bPort && bFile))
            {
                // Console.WriteLine("Parser error " + nl + parseInfo);
                this.todoOK = false;
                
                // string CommandArguments = "-h www.mkanls.com -p 4301 -f c:\tmp\tmp.txt"
                this.info += help;
                

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
            Console.WriteLine("OpnNMTConsoleClient (c) 2018 miguel canals -  www.mknals.com - MIT License");
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
            OUTfilesrc = Path.GetFileNameWithoutExtension(INPfile) + ".src" + Path.GetExtension(INPfile);
            if (Path.GetDirectoryName(INPfile) != "") {
                OUTfilesrc = Path.GetDirectoryName(INPfile) + "\\" + OUTfilesrc; }

            string OUTfiletgt;
            OUTfiletgt = Path.GetFileNameWithoutExtension(INPfile) + ".tgt" + Path.GetExtension(INPfile);
            if (Path.GetDirectoryName(INPfile) != "")
            {
                OUTfiletgt = Path.GetDirectoryName(INPfile) + "\\" + OUTfiletgt;
            }

            string OUTfilejson;
            OUTfilejson = Path.GetFileNameWithoutExtension(INPfile) + ".json";
            if (Path.GetDirectoryName(INPfile) != "")
            {
                OUTfilejson = Path.GetDirectoryName(INPfile) + "\\" + OUTfilejson;
            }

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
                int nlines = 0;
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
                            nlines += 1;
                            // Console.WriteLine(line); 
                        }

                    }
                    Console.WriteLine(string.Format("File {0} -> {1} lines", INPfile, nlines));
                    
                    sr.Close();
                }
                // Just for info
                Console.WriteLine("Detected encoding ->  {0}.", GetFileEncoding(INPfile));
                
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
            rClient.TranslateRESTClientData1by1(RESTClientData);
            
            // Use rClient.TranslateRESTClientData(RESTClientData); not good idea for a text file

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

            
            // Only for debug           
                       
            //if (RESTClientData.todoOKREST) // translation was fine
            //{
            ////   Console.WriteLine("OK");
            ////    // Translation inside RESTClientDATA
           //     // we need to search in each RESTSEentence
           //     bool verbose = true;
           //     verbose = false;
           //     for (int i = 0; i < RESTClientData.ListSourceONMT.Count; i++)
           //     {
           //         if (verbose)
           //         {
           //             string auxS = "<br>{1}<br>{0}<br><b>{2}</b> - ({3})<br>";
           //             auxS = string.Format(auxS,
           //                 RESTClientData.ListSourceONMT[i].src,
           //                 RESTClientData.ListTargetONMT[i][0].src,
           //                 RESTClientData.ListTargetONMT[i][0].tgt,
           //                 RESTClientData.ListTargetONMT[i][0].pred_score);
           //             auxS = auxS.Replace("<br>", Environment.NewLine);
           //             auxS = auxS.Replace("<b>", "");
           //             auxS = auxS.Replace("</b>", "");
           //             Console.WriteLine(auxS);
           //             
           //         }
           //         else
           //         {
           //             string auxS = "<br>{0}<br><b>{1}</b><br>";
           //             auxS= string.Format(auxS,
           //                 RESTClientData.ListSourceONMT[i].src,
           //                 RESTClientData.ListTargetONMT[i][0].tgt);
           //             auxS = auxS.Replace("<br>", Environment.NewLine);
           //             auxS = auxS.Replace("<b>", "");
           //             auxS = auxS.Replace("</b>", "");
           //             Console.WriteLine(auxS);
           //         }
           //
           //
           //     }
           //
           // }
           // else
           // {
           //     Console.WriteLine("Translation ERROR");
           //     Console.WriteLine("Translation ERROR in RESTClientData.todoOKREST" + "<br>");
           //     Console.WriteLine(RESTClientData.infoREST + "<br>");
           // }
           // Console.ReadLine();
            Console.WriteLine("Done!");

        }
        public static Encoding GetFileEncoding(string srcFile)
        {
            // Based on 
            // https://weblog.west-wind.com/posts/2007/Nov/28/Detecting-Text-Encoding-for-StreamReader
            // *** Use Default of Encoding.Default (Ansi CodePage)

            Encoding enc = Encoding.Default;
            // *** Detect byte order mark if any - otherwise assume default

            byte[] buffer = new byte[5];
            FileStream file = new FileStream(srcFile, FileMode.Open);
            file.Read(buffer, 0, 5);
            file.Close();


            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                enc = Encoding.UTF8;

            else if (buffer[0] == 0xfe && buffer[1] == 0xff)
                enc = Encoding.Unicode;

            else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                enc = Encoding.UTF32;

            else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                enc = Encoding.UTF7;
            return enc;

        }
    }
}
