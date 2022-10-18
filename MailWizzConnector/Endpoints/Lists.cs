using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MailWizzConnector
{
    public partial class MailWizzConnector
    {
        public void GetLists()
        {
            //For example get lists
            string Url = "http://your-domain-mailwizz.com/api/index.php/lists?page=1&per_page=10";
            string PublicKey = "Your public key";
    string PrivateKey = "Your private key";
    int timestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            //get request string to sign
            //don't forget about http encoding
            // %4f - wrong, %4F - ok, space "%20"-wrong, "+" - correctly ( "My first test" to "My+first+test")
            string urlToSing = $"GET {Url}&X-MW-PUBLIC-KEY={PublicKey}&X-MW-TIMESTAMP={timestamp}";
            HMACSHA1 hmac = new HMACSHA1(Encoding.ASCII.GetBytes(PrivateKey));
    // Compute the hash
    byte[] bSignature = hmac.ComputeHash(Encoding.ASCII.GetBytes(Url));
            StringBuilder sbSignature = new StringBuilder();
            foreach (byte b in bSignature) sbSignature.AppendFormat("{0:x2}", b);
            string signature = sbSignature.ToString();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Accept = "*/*";
            request.Method = "GET";
            request.Headers.Add("X-MW-PUBLIC-KEY", PublicKey);
            request.Headers.Add("X-MW-TIMESTAMP", timestamp.ToString());
            request.Headers.Add("X-MW-SIGNATURE", signature);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            StringBuilder output = new StringBuilder();
            output.Append(reader.ReadToEnd());
        }
    }
}