using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMS_Speed
{
    internal class Globals
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string RSAEncryption(string strText)
        {
            var publicKey = "<RSAKeyValue><Modulus>cqndQeH+clUoOn+cKRB3K/sRtX6TOfqu2vjeLSPSc+SzpI52yOA4BedE7dp2tlA9A46pi0WP18HOCAoZXy4qHA2ri7DOnsKX8Mg1Vr2KPAMl3YFWqhk/S99+4a/dpKDwrnRNi5kv0i2mllN5x5ZcZ9E7Y1e8nm9FGdIJCxA+XiM=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            var testData = Encoding.UTF8.GetBytes(strText);
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    rsa.FromXmlString(publicKey.ToString());
                    var encryptedData = rsa.Encrypt(testData, true);
                    var base64Encrypted = Convert.ToBase64String(encryptedData);
                    return base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        public static string RSADecryption(string strText)
        {
            var privateKey = "<RSAKeyValue><Modulus>cqndQeH+clUoOn+cKRB3K/sRtX6TOfqu2vjeLSPSc+SzpI52yOA4BedE7dp2tlA9A46pi0WP18HOCAoZXy4qHA2ri7DOnsKX8Mg1Vr2KPAMl3YFWqhk/S99+4a/dpKDwrnRNi5kv0i2mllN5x5ZcZ9E7Y1e8nm9FGdIJCxA+XiM=</Modulus><Exponent>AQAB</Exponent><P>zuYlR922YFu/MakBNDKCHn83FkYMQCaFvcDoUX4TZ4R2Qjg+acUjXzScV41Ul/mWedBwlXcGQ/epoB4OsOQkxQ==</P><Q>jeAVdokpxC+pKhKTAGFEXq7Z4Sji6UUrhf3ARcfa4v7hQEMqTlcui7jp9/kCz25feCpmzCPjg1E26mkWRLU1xw==</Q><DP>YHvO8t6fx/vBA4WOvCq5p0MoC0kLOXc9cyncrPQgVGvfQi48XNLEFgfQyLttsZmA5LmhZvIkh9mczsB1lWQvCQ==</DP><DQ>RP81cPBD36VOH6fo1cZ3+ZQPYfEAaXG6OO+vEkCfssVBxn7jlDXR7SGAp5fyRe7nfwkf9Sd+/d4BVv7EVaXLAQ==</DQ><InverseQ>grNU3qASSC4QYF7X6BB+lxIP3rHbaN0zSeTJtt0jJMNHA48PDv6FrGMj6KPWK0pDDPxKrTdEXD5JixSc8iR+gg==</InverseQ><D>B6P4AV7cxKOWBafhMP9O4ZheSri/eLqSkjbJHzrm2CAiNFHl6ma+dO4/MpY/GNDp7+W+uHAPMLJSV0jM/gGmfpbRAP7WGOaRMToBNwxHV/dwVqnNzjAS6pd8TJGt8lF6AbQla3uSABbyG/YXb59BXKEivPDOuCoFbY+tQTb/Tek=</D></RSAKeyValue>";
            var testData = Encoding.UTF8.GetBytes(strText);
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    var base64Encrypted = strText;
                    rsa.FromXmlString(privateKey);
                    var resultBytes = Convert.FromBase64String(base64Encrypted);
                    var decryptedBytes = rsa.Decrypt(resultBytes, true);
                    var decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                    return decryptedData.ToString();
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        public static bool VerifyMessage(string originalMessage, string signedMessage)
        {
            bool verified;
            try
            {
                var publicKey = "<RSAKeyValue><Modulus>cqndQeH+clUoOn+cKRB3K/sRtX6TOfqu2vjeLSPSc+SzpI52yOA4BedE7dp2tlA9A46pi0WP18HOCAoZXy4qHA2ri7DOnsKX8Mg1Vr2KPAMl3YFWqhk/S99+4a/dpKDwrnRNi5kv0i2mllN5x5ZcZ9E7Y1e8nm9FGdIJCxA+XiM=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
                rsa.FromXmlString(publicKey);
                verified = rsa.VerifyData(Encoding.UTF8.GetBytes(originalMessage), CryptoConfig.MapNameToOID("SHA1"), Convert.FromBase64String(signedMessage));
            }
            catch (Exception)
            {
                verified = false;
            }

            return verified;
        }

        public static string ObjectToJsonString(object obj)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(obj);
            return JSONString;
        }

        public static string SignMessage(string message)
        {
            string signedMessage;
            try
            {
                var privateKey = "<RSAKeyValue><Modulus>cqndQeH+clUoOn+cKRB3K/sRtX6TOfqu2vjeLSPSc+SzpI52yOA4BedE7dp2tlA9A46pi0WP18HOCAoZXy4qHA2ri7DOnsKX8Mg1Vr2KPAMl3YFWqhk/S99+4a/dpKDwrnRNi5kv0i2mllN5x5ZcZ9E7Y1e8nm9FGdIJCxA+XiM=</Modulus><Exponent>AQAB</Exponent><P>zuYlR922YFu/MakBNDKCHn83FkYMQCaFvcDoUX4TZ4R2Qjg+acUjXzScV41Ul/mWedBwlXcGQ/epoB4OsOQkxQ==</P><Q>jeAVdokpxC+pKhKTAGFEXq7Z4Sji6UUrhf3ARcfa4v7hQEMqTlcui7jp9/kCz25feCpmzCPjg1E26mkWRLU1xw==</Q><DP>YHvO8t6fx/vBA4WOvCq5p0MoC0kLOXc9cyncrPQgVGvfQi48XNLEFgfQyLttsZmA5LmhZvIkh9mczsB1lWQvCQ==</DP><DQ>RP81cPBD36VOH6fo1cZ3+ZQPYfEAaXG6OO+vEkCfssVBxn7jlDXR7SGAp5fyRe7nfwkf9Sd+/d4BVv7EVaXLAQ==</DQ><InverseQ>grNU3qASSC4QYF7X6BB+lxIP3rHbaN0zSeTJtt0jJMNHA48PDv6FrGMj6KPWK0pDDPxKrTdEXD5JixSc8iR+gg==</InverseQ><D>B6P4AV7cxKOWBafhMP9O4ZheSri/eLqSkjbJHzrm2CAiNFHl6ma+dO4/MpY/GNDp7+W+uHAPMLJSV0jM/gGmfpbRAP7WGOaRMToBNwxHV/dwVqnNzjAS6pd8TJGt8lF6AbQla3uSABbyG/YXb59BXKEivPDOuCoFbY+tQTb/Tek=</D></RSAKeyValue>";
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
                rsa.FromXmlString(privateKey);
                signedMessage = Convert.ToBase64String(rsa.SignData(Encoding.UTF8.GetBytes(message), CryptoConfig.MapNameToOID("SHA1")));
            }
            catch (Exception)
            {
                signedMessage = string.Empty;
            }
            return signedMessage;

        }

        public static string HMACSHA256(string text, string key)
        {
            Encoding encoding = Encoding.UTF8;
            Byte[] textBytes = encoding.GetBytes(text);
            Byte[] keyBytes = encoding.GetBytes(key);
            Byte[] hashBytes;
            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }


        public static string ConvertToUnsign(string str)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = str.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty)
                        .Replace('\u0111', 'd').Replace('\u0110', 'D').Replace("'", "''");
        }

        public static string CUTSTRING(string strText, int MaxLength)
        {
            return (strText.Length <= MaxLength ? strText : strText.Substring(0, MaxLength));
        }

        public static string VARCHARNULL(string strText)
        {
            return strText != "" ? "'" + strText + "'" : "NULL";
        }

        public static void WriteLog(string LogText)
        {
            try
            {
                if (!Directory.Exists(Application.StartupPath + "\\Log"))
                    Directory.CreateDirectory(Application.StartupPath + "\\Log");
                StreamWriter sw = new StreamWriter(Application.StartupPath + "\\Log\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", true);
                sw.WriteLine("[" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "] " + LogText + "\n");
                sw.Close();
            }
            catch (Exception)
            {

            }
        }

        private static void InitiateSSLTrust()
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
                WriteLog($"API Error: API Link {apiLink}, Method {method}, Error {ex.Message}");
                return ex.Message;
            }
            return "";
        }
    }
}
