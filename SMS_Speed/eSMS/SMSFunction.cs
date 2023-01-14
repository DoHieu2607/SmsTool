using Newtonsoft.Json;
using SMS_Speed.DTO;
using SMS_Speed.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMS_Speed.eSMS
{
    internal class SMSFunction
    {
        public static ResponseDTO SendSMSXml(List<MemberDTO> customer)
        {
            /*  string APIKey = Config.ReadConfigFile()["ApiKey"];
              string SecretKey = Config.ReadConfigFile()["SecretKey"];
              string SmsType = Config.ReadConfigFile()["SmsType"];
              string BrandName = Config.ReadConfigFile()["BrandName"];
              string Content = Config.ReadConfigFile()["Content"];*/

            string APIKey = Properties.Settings.Default.ApiKey;
            string SecretKey = Properties.Settings.Default.SecretKey;
            string SmsType = Properties.Settings.Default.SmsType;
            string BrandName = Properties.Settings.Default.BrandName;
            string Content = Properties.Settings.Default.TemplateDay;


            string url = "http://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4/";
            UTF8Encoding encoding = new UTF8Encoding();

            string strResult = string.Empty;

            string customers = "";


            for (int i = 0; i < customer.Count(); i++)
            {
                customers = customers + @"<CUSTOMER>"
                                + "<PHONE>" + customer[i].HomeTele + "</PHONE>"
                                + "</CUSTOMER>";

            }
            string SampleXml = @"<RQST>"
                               + "<APIKEY>" + APIKey + "</APIKEY>"
                               + "<SECRETKEY>" + SecretKey + "</SECRETKEY>"
                               + "<ISFLASH>0</ISFLASH>"
                               + "<BRANDNAME>"+ BrandName + "</BRANDNAME>" 
                               + "<SMSTYPE>"+ SmsType + "</SMSTYPE>"                             
                               + "<CONTACTS>" + customers + "</CONTACTS>"
                               + "<CONTENT>" + Content + "</CONTENT>"
           + "</RQST>";

            string postData = SampleXml.Trim().ToString();
            byte[] data = encoding.GetBytes(postData);

            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "POST";
            webrequest.Timeout = 500000;
            webrequest.ContentType = "application/x-www-form-urlencoded";
            webrequest.ContentLength = data.Length;

            Stream newStream = webrequest.GetRequestStream();
            newStream.Write(data, 0, data.Length);
            newStream.Close();

            HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();

            Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader loResponseStream = new StreamReader(webresponse.GetResponseStream(), enc);
            strResult = loResponseStream.ReadToEnd();
            loResponseStream.Close();
            webresponse.Close();

            return JsonConvert.DeserializeObject<ResponseDTO>(strResult); ;
        }

        public static string CheckBalance()
        {
            object body = new
            {
                ApiKey = Properties.Settings.Default.ApiKey,
                SecretKey = Properties.Settings.Default.SecretKey,
            };

            string jsonBody = JsonConvert.SerializeObject(body);
            string url = "http://rest.esms.vn/MainService.svc/json/GetBalance_json";
            string result;
            string err = CallAPIController.CallAPI(url, jsonBody, "application/json", "POST", out result);
            if (!err.Equals(""))
            {
                MessageBox.Show(err, "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            BalanceDTO balance = JsonConvert.DeserializeObject<BalanceDTO>(result);
            if (balance.CodeResponse == 100)
            {
                return balance.Balance + "VND";
            }
            else return "Error";
        }


      /*  public static ResponseDTO SendSMSJson(List<MemberDTO> customers)
        {
            string apiUrl = "http://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4_post_json/";
            try
            {
                for (int i = 0; i < customers.Count; i++)
                {
                    object body = new
                    {
                        ApiKey = Config.ReadConfigFile()["ApiKey"],
                        SecretKey = Config.ReadConfigFile()["SecretKey"],
                        SmsType = Config.ReadConfigFile()["SmsType"],
                        Brandname = Config.ReadConfigFile()["BrandName"],
                        IsUnicode = Config.ReadConfigFile()["IsUnicode"],
                        SandBox = Config.ReadConfigFile()["SandBox"],
                        RequestId = Guid.NewGuid().ToString(),
                        Phone = customers[i].HomeTele,
                        Content = Config.ReadConfigFile()["Content"],
                    };
                    string jsonBody = JsonConvert.SerializeObject(body);
                    string result;
                    string err = CallAPIController.CallAPI(apiUrl, jsonBody, "application/json", "POST", out result);
                    if (!err.Equals(""))
                    {
                        return JsonConvert.DeserializeObject<ResponseDTO>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); null;
                return null;
            }
        }*/

        public static List<string> GetTemplate()
        {
            object body = new
            {
                ApiKey = Properties.Settings.Default.ApiKey,
                SecretKey = Properties.Settings.Default.SecretKey,
                SmsType = Properties.Settings.Default.SmsType,
                Brandname = Properties.Settings.Default.BrandName
            };

            string jsonBody = JsonConvert.SerializeObject(body);
            string url = "http://rest.esms.vn/MainService.svc/json/GetTemplate/";
            string result;
            string err = CallAPIController.CallAPI(url, jsonBody, "application/json", "POST", out result);
            if (!err.Equals(""))
            {
                MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return JsonConvert.DeserializeObject<TemplateDTO>(result).BrandnameTemplates.Select(S=>S.TempContent).ToList();
        }
}
}
