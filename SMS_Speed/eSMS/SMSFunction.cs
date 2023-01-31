using Newtonsoft.Json;
using SMS_Speed.DTO;
using SMS_Speed.Utility;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMS_Speed.eSMS
{
    internal class SMSFunction
    {
    
        public static SMSResponseDTO SendSMS(MemberDTO customer, string BrandName, int SmsType, string Content)
        {
            string APIKey = Properties.Settings.Default.ApiKey;
            string SecretKey = Properties.Settings.Default.SecretKey;
            string APIUrl = Properties.Settings.Default.APIUrl;

            string subUrl = "/json/SendMultipleMessage_V4_post/";
            string url = APIUrl + subUrl;
            string strResult = string.Empty;

            string postData = $"Phone={customer.HomeTele}"
                               + $"&ApiKey={APIKey}"
            + $"&SecretKey={SecretKey}"
            + $"&Content={Content}"
            + $"&Brandname={BrandName}"
            + $"&SmsType={SmsType}";

            byte[] data = Encoding.ASCII.GetBytes(postData);

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

        public static SMSResponseDTO TestSendSMS()
        {
            string APIKey = "E0BF300460D2DE65489DB31934A7B4";
            string SecretKey = "3219839723390BC6133415A002FA2D";
            string APIUrl = Properties.Settings.Default.APIUrl;

            string testPhone = "0901312607";
            string BrandName = "Baotrixemay";
            string SmsType = "2";
            string Content = "Cam on quy khach da su dung dich vu cua chung toi. Chuc quy khach mot ngay tot lanh!";
            string subUrl = "/json/SendMultipleMessage_V4_post/";
            string url = APIUrl + subUrl;
            UTF8Encoding encoding = new UTF8Encoding();

            string strResult = string.Empty;


            string postData = $"Phone={testPhone}"
                               + $"&ApiKey={APIKey}"
            + $"&SecretKey={SecretKey}"
            + $"&Content={Content}"
            + $"&Brandname={BrandName}"
            + $"&SmsType={SmsType}";

            byte[] data = Encoding.ASCII.GetBytes(postData);
            
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

        public static CheckSMSDTO checkSMS(string SMSID)
        {
            try
            {
                string APIKey = Properties.Settings.Default.ApiKey;
                string SecretKey = Properties.Settings.Default.SecretKey;
                string APIUrl = Properties.Settings.Default.APIUrl;

                string subUrl = $"/json/GetSendStatus?RefId={SMSID}&ApiKey={APIKey}&SecretKey={SecretKey}";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(APIUrl + subUrl);
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 300000;

                var httpWebResponse = (WebResponse)httpWebRequest.GetResponse();
                using(var srCheck = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8)) { 
                    return JsonConvert.DeserializeObject<CheckSMSDTO>(srCheck.ReadToEnd());
                }

                
            }
            catch (Exception ex)
            {
                Globals.WriteLog($"CHECK SMS ERROR: {ex.Message}");
                return null;
            }
        }
        public static List<BrandnameDTO> getBrandNames()
        {
            try
            {
                string APIKey = Properties.Settings.Default.ApiKey;
                string SecretKey = Properties.Settings.Default.SecretKey;
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
                //MessageBox.Show(res.ErrorMessage, res.CodeResult.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                Globals.WriteLog($"API GET TEMPLATE ERROR CODE: {res.CodeResult}, ERROR MESSAGE: {res.ErrorMessage}");
                return res.BrandnameTemplates;
            }
        }
}
}
