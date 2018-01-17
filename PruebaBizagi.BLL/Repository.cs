using Newtonsoft.Json;
using PruebaBizagi.BLL.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PruebaBizagi.BLL
{
    public class Repository
    {
        public PadronResponse getPadron(string cedula)
        {
            var results = new PadronResponse();
            string webAddr = "http://172.24.126.56:8088/fermat/checkPadron.jsp";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
            

                string json = "{ \"numero_documento\" : \""+ cedula + "\"}";

                streamWriter.Write(json);
                streamWriter.Flush();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var responseText = streamReader.ReadToEnd();
                if (responseText!=null)
                {
                    PadronResponse myDeserializedObjList = (PadronResponse)Newtonsoft.Json.JsonConvert.DeserializeObject(responseText, typeof(PadronResponse));
                    results = myDeserializedObjList;
                }
               
                
            }

            return results;
        }
    
    }
}
