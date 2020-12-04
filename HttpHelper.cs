using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Demo002
{
   public class HttpHelper
    {
        public static string Get(String url, Encoding encode)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html, application/xhtml+xml, */*";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream rs = response.GetResponseStream();
            StreamReader sr = new StreamReader(rs, encode);
            var result = sr.ReadToEnd();
            sr.Close();
            rs.Close();
            return result;
        }

        public static string Post(string url,string json)
        {
            string value = string.Empty;

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    value = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
               
            }
            return value;
        }
    }
}
