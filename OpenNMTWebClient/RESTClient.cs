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
        public string[] rawsentences { get; set; }
        public List<SourceONMT> ListSourceONMT { get; set; } // Json is returned as a list
        public List<List<TargetONMT>> ListTargetONMT { get; set; } // Json is returned as a list of lists
        public bool todoOKREST { get; set; }
        public string infoREST { get; set; }
        public RESTClientDataC (string[] sentences) {
            rawsentences = sentences;
            ListTargetONMT = new List<List<TargetONMT>>();
            ListSourceONMT = new List<SourceONMT>();
            todoOKREST = false; infoREST = "";
        }
    }
    public class SourceONMT // text to be translate
    {
        public string src { get; set; }
    }
    public class TargetONMT
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
        public RESTClientDataC TranslateRESTClientData1by1 (RESTClientDataC RESTClientDATA)
        {
            // Instead of run all sentences at one, we call TranslateRESTClientData
            // with just one sentence, and the response is add manually to the 
            // RESTClientDATA. 
            string[] sentence ={""}; // I will pass just one sentence
            foreach (string s in RESTClientDATA.rawsentences) {
                sentence[0]= s;                
                RESTClientDataC auxRESTClientData = new RESTClientDataC (sentence);
                auxRESTClientData= TranslateRESTClientData(auxRESTClientData);
                
                if (auxRESTClientData.todoOKREST )
                {            
                    RESTClientDATA.todoOKREST=auxRESTClientData.todoOKREST;
                    RESTClientDATA.infoREST += auxRESTClientData.infoREST;
                    // we will add just the first and only item of the list to
                    // the source and target lists                    
                    RESTClientDATA.ListSourceONMT.Add(auxRESTClientData.ListSourceONMT[0]);
                    RESTClientDATA.ListTargetONMT.Add(auxRESTClientData.ListTargetONMT[0]);                    

                } else { // abort
                    RESTClientDATA.todoOKREST= false;
                    RESTClientDATA.infoREST += auxRESTClientData.infoREST;
                    break;
                } 
            }

            return RESTClientDATA;

            
        }
        public RESTClientDataC TranslateRESTClientData(RESTClientDataC RESTClientDATA)
        {
            string t = string.Format("http://{0}:{1}/", host, port); // target translation
            // we need to iniatalize the RESTClientData classs
            foreach (string s in RESTClientDATA.rawsentences)
            {
                var tgt = new List<TargetONMT>();
                var src = new SourceONMT();                
                src.src = s; // to be translated
                RESTClientDATA.ListSourceONMT.Add(src); // now with the sentence
                RESTClientDATA.ListTargetONMT.Add(tgt);  // empty                
            }
            // lets translate we will create the rest call
            try
            {
                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(RESTClientDATA.ListSourceONMT);
                RESTClientDATA.infoREST = string.Format("Json2Rest -> {0}" + "<br>", json);
                var SC = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(
                    "translator/translate", SC).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    RESTClientDATA.infoREST += "Response OK" + "<br>";
                    string data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    RESTClientDATA.infoREST += string.Format("Json2Rest -> {0}" + "<br>", data);
                    JavaScriptSerializer JSserializer = new JavaScriptSerializer();
                    // var nose = new List<List<TargetONMT>>(); // 
                    // nose =  JSserializer.Deserialize<List<List<TargetONMT>>>(data);
                    RESTClientDATA.ListTargetONMT = JSserializer.Deserialize<List<List<TargetONMT>>>(data); 
                    RESTClientDATA.infoREST += string.Format("Target serialized OK!" + "<br>", data);
                    RESTClientDATA.todoOKREST = true; 
                }
                else
                {
                    RESTClientDATA.infoREST += "ERROR in response. " + "<br>";
                    RESTClientDATA.todoOKREST = false;
                }
            }
            catch (Exception e)
            {
                RESTClientDATA.infoREST += "FATAL ERROR in response. " + e.Message + "<br>";
                RESTClientDATA.todoOKREST = false;

            }
            return RESTClientDATA;                    

        }
        

    }
}