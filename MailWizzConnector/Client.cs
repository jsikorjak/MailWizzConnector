using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace MailWizzConnector
{
    public partial class MailWizzConnector
    {
        protected T Invoke<T>(string EndpointUrl, string Method, Dictionary<string, string> Data = null)
        {
            string response = Invoke(EndpointUrl, Method, Data);

            return new JavaScriptSerializer().Deserialize<T>(response);
        }

        protected string Invoke(string EndpointUrl, string Method, Dictionary<string, string> FormData = null)
        {
            WebClient wc = new WebClient();

            wc.Headers["X-MW-PUBLIC-KEY"] = ApiKey;

            JavaScriptSerializer jss = new JavaScriptSerializer();

            Uri baseUri = new Uri(_ApiUrlRoot);
            Uri targetUri = new Uri(baseUri, EndpointUrl);

            string response;

            if (Method.ToUpper() == "GET")
            {
                response = wc.DownloadString(targetUri.ToString());
            }
            else
            {
                string serializedFormData = FormData != null ? string.Join("&", FormData.Select(q => string.Format("{0}={1}", HttpUtility.UrlEncode(q.Key), HttpUtility.UrlEncode(q.Value)))) : "";

                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                response = wc.UploadString(targetUri.ToString(), Method, serializedFormData);
            }

            return response;
        }
    }
}