using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace SMS_Speed.Utility
{
    internal class CallAPIController
    {
        public static void InitiateSSLTrust()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback =
                   new RemoteCertificateValidationCallback(
                        delegate
                        { return true; }
                    );
            }
            catch
            {

            }
        }

        public static string CallAPI(string apiLink, string data, string contentType, string method, out string result)
        {
            result = string.Empty;
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiLink);    
                httpWebRequest.ContentType = contentType;
                httpWebRequest.Method = method;
                httpWebRequest.Proxy = new WebProxy();//no proxy

                if (!string.IsNullOrEmpty(data))
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(data);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                InitiateSSLTrust();//bypass SSL
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

            }
            catch (WebException ex)
            {
                return ex.Message;
            }
            return "";
        }


    }
}
