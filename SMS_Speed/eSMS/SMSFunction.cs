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
    
        public static SMSResponseDTO SendSMS(List<MemberDTO> customer, string BrandName, int SmsType, string Content)
        {
            string APIKey = Properties.Settings.Default.ApiKey;
            string SecretKey = Properties.Settings.Default.SecretKey;


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
                               + $"<APIKEY>{APIKey}</APIKEY>"
                               + $"<SECRETKEY>{SecretKey}</SECRETKEY>"
                               + $"<ISFLASH>0</ISFLASH>"
                               + $"<BRANDNAME>{BrandName}</BRANDNAME>"
                               + $"<SMSTYPE>{SmsType}</SMSTYPE>"
                               + $"<CONTACTS>{customers}</CONTACTS>"
                               + $"<CONTENT>{Content}</CONTENT>"
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

            SMSResponseDTO res = JsonConvert.DeserializeObject<SMSResponseDTO>(strResult);
            return res;
        }

        //public static 
        public static List<BrandnameDTO> getBrandNames()
        {
            try
            {
                string APIKey = Properties.Settings.Default.ApiKey;
                string SecretKey = Properties.Settings.Default.SecretKey;
                string SmsType = Properties.Settings.Default.SmsType;
                string BrandName = Properties.Settings.Default.BrandName;
                string Content = Properties.Settings.Default.TemplateDay;
                string APIUrl = Properties.Settings.Default.APIUrl;

                string subUrl = $"/json/GetListBrandname/{APIKey}/{SecretKey}";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(APIUrl + subUrl);

                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = Properties.Settings.Default.TimeOut;

                var response = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    string json = reader.ReadToEnd();
                    BrandnameResponseDTO res = JsonConvert.DeserializeObject<BrandnameResponseDTO>(json);
                    if(res.CodeResponse == 100)
                    {
                        return res.ListBrandname;
                    }
                    else
                    {
                        Globals.WriteLog($"API Error Code: {res.CodeResponse}; Failed to get brandnames");
                        return null;
                    }
                }
            }
            catch(Exception e)
            {
                Globals.WriteLog($"API Get Error: {e.Message}");
                return null;
            }
            
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
            string err = Globals.CallAPI(url, jsonBody, "application/json", "POST", out result);
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

        public static List<TemplateDTO> GetTemplate(int smsType, string brandName)
        {
            object body = new
            {
                ApiKey = Properties.Settings.Default.ApiKey,
                SecretKey = Properties.Settings.Default.SecretKey,
                SmsType = smsType,
                Brandname = brandName
            };

            string jsonBody = JsonConvert.SerializeObject(body);
            string url = "http://rest.esms.vn/MainService.svc/json/GetTemplate/";
            string result;
            string err = Globals.CallAPI(url, jsonBody, "application/json", "POST", out result);
            if (!err.Equals(""))
            {
                MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            TemplateResponseDTO res = JsonConvert.DeserializeObject<TemplateResponseDTO>(result);
            if(res.CodeResult == 100)
            {
                return JsonConvert.DeserializeObject<TemplateResponseDTO>(result).BrandnameTemplates;
            }
            else
            {
                MessageBox.Show(res.ErrorMessage, res.CodeResult.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                Globals.WriteLog($"API GET TEMPLATE ERROR CODE: {res.CodeResult}, ERROR MESSAGE: {res.ErrorMessage}");
                return null;
            }
        }
}
}
