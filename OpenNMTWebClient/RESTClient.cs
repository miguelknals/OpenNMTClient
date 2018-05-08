using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net; // para cliente y funciones http
using System.Net.Http; // para cliente y funciones http
using System.Net.Http.Headers; // algunas características Headers
using System.Web.Script.Serialization; // para serializer


namespace OpenNMTWebClient
{
    class RESTClientDataC
    {
        public string[] sentenceList { get; set; }
        public Boolean todoOK { get; set; }
        public List<RESTSentence> SentecesListREST { get; set; }
    }
    public class RESTSentence //REST info for the sentence
    {
        public TargetTranslationONMT TargetTranslationONMTREST { get; set; }
        public string sourceREST;
        public bool todoOKREST { get; set; }
        public string infoREST { get; set; }
    }


    public class SourceONMT // text to be translate
    {
        public string src { get; set; }
    }
    public class TargetTranslationONMT
    {
        public int n_best { get; set; }
        public double pred_score { get; set; }
        public string src { get; set; }
        public string tgt { get; set; }
    }

    class RESTClient
    {
        string host;
        int port;
        HttpClient client;
        public RESTClient(string hhost, int pport)
        {
            host = hhost;
            port = pport;
            client = new HttpClient(); // client here as any funciton can use it
            client.BaseAddress = new Uri(string.Format("http://{0}:{1}/", host, port));

            // client.BaseAddress = new Uri("http://www.mknals.com:4031/");
        }
        public void TranslateRESTClientData(RESTClientDataC RESTClientDATA)
        {
            string t = string.Format("http://{0}:{1}/", host, port); // target translation
            foreach (string s in RESTClientDATA.sentenceList)
            {
                var i = new RESTSentence() { sourceREST = s }; //REST info for the sentence

                i = TranslateSentence(i);
                RESTClientDATA.SentecesListREST.Add(i);
            }

            RESTClientDATA.todoOK = true;
        }
        public RESTSentence TranslateSentence(RESTSentence i)
        {
            i.todoOKREST = true; // optimism
            i.infoREST = "";
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var TT = new TargetTranslationONMT { tgt = "" };

            try
            {
                var src = new SourceONMT() { src = i.sourceREST };
                var serializer = new JavaScriptSerializer();
                string json = "[" + serializer.Serialize(src) + "]";
                i.infoREST += string.Format("Json2Rest -> {0}" +"<br>", json); 
                var SC = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(
                    "translator/translate", SC).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    i.infoREST += "Response OK"; 
                    string data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    i.infoREST += string.Format("Json2Rest -> {0}" + "<br>", data); 
                    data = data.Replace("[[", ""); data = data.Replace("]]", "");
                    i.infoREST += string.Format("Json2Rest [[ stripped -> {0}" + "<br>", data); 
                    JavaScriptSerializer JSserializer = new JavaScriptSerializer();
                    TT = JSserializer.Deserialize<TargetTranslationONMT>(data);
                    i.infoREST += string.Format("Target serialized OK!" + "<br>", data); 
                }
                else
                {
                    i.infoREST += "ERROR in response. ";
                    i.todoOKREST = false;
                }


                Console.WriteLine("Trad ->{0}", TT.tgt);
                Console.WriteLine("................");
                Console.WriteLine("{0}/{1}", TT.n_best, TT.pred_score, TT.src);
                Console.WriteLine("................");
                Console.WriteLine();
                i.TargetTranslationONMTREST = TT; //Save REST RESULT

            }
            catch (Exception e)
            {
                i.todoOKREST = false; // error
                i.infoREST += e.Message + Environment.NewLine;
                Console.WriteLine(e.Message);
            }
            return i;
        }


    }
}