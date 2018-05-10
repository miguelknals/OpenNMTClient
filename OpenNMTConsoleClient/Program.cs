using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Web.Script.Serialization; // para serializer need to now add reference System.Web.Extensions

namespace OpenNMTConsoleClient
{
    public static class Globals
    {
        public const Int32 BUFFER_SIZE = 10; // Unmodifiable
        public static String FILE_NAME = "Output.txt"; // Modifiable
        public static readonly String CODE_PREFIX = "US-"; // Unmodifiable
        public static String  INPUTFILENAME =@"c:\tmp\tmp.txt";
        public static String HOST="www.mknals.com";
        public static Int16 PORT=4300    ;
     }
     class CommandArguments {
       public string CAinput {get; set;}
       public bool todoOk {get; set;}
       public string host {get; set;}
       public int port {get; set;}
       public int file {get; set;}
      

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
     class Program
    {
        static void Main(string[] args)
        {
            string CommandArguments = "-h www.mkanls.com -p 4301 -f c:\tmp\tmp.txt [-EOFT]";


            string INPfile = Globals.INPUTFILENAME;
            string INPfilePath = Path.GetDirectoryName(INPfile);
            string INPfileFilenameWithoutExtension = Path.GetFileNameWithoutExtension(INPfile);
            string[] idioma = new string[10];
            string auxSidioma = ""; int nidioma = -1; int currentidioma = -1;
            
            StreamReader sr = new StreamReader(INPfile);

            var serializer = new JavaScriptSerializer();
            var src = new SourceONMT() { src = "test" };
            Console.WriteLine(serializer.Serialize(src));
            var lista2src = new List<SourceONMT>();
            lista2src.Add(src);
            Console.WriteLine(serializer.Serialize(lista2src));
            var listasrc = new ListaSorce();
            listasrc.entries = new List<SourceONMT>();
            listasrc.entries.Add(src);
            
            Console.WriteLine(serializer.Serialize(listasrc));
            Console.ReadLine();
            string json = "[" + serializer.Serialize(src) + "]";
            // string json = @"[{""src"":""Aprovat inicialment el projecte.""}]";
                                       
        }
    }
}
