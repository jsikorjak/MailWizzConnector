using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MailWizzConnector.Types
{
    public class Subscriber
    {
        public Subscriber(SubscriberResponse response)
        {
            this._AllFields = response.data.record;
        }

        public Subscriber(Dictionary<string, string> response)
        {
            this._AllFields = response;
        }

        protected Dictionary<string, string> _AllFields;
        protected static string[] TypedFields = new string[] { "EMAIL", "subscriber_uid", "status", "source", "ip_address" };

        public string Email
        {
            get
            {
                return _AllFields["EMAIL"];
            }
        }

        public string SubscriberUid
        {
            get
            {
                return _AllFields["subscriber_uid"];
            }
        }

        public bool IsSubscribed
        {
            get
            {
                return _AllFields["status"].ToLower() == "confirmed";
            }
        }

        public string Source
        {
            get
            {
                return _AllFields["source"];
            }
        }

        public string IpAddress
        {
            get
            {
                return _AllFields["ip_address"];
            }
        }

        public Dictionary<string, string> CustomFields
        {
            get
            {
                return _AllFields.Where(q => !TypedFields.Contains(q.Key)).ToDictionary(q => q.Key, q => q.Value);
            }
        }
    }

    public class SubscriberResponse : Response
    {
        public SubscriberResponseData data { get; set; }
    }

    public class SubscriberResponseData
    {
        public Dictionary<string, string> record { get; set; }
    }

    public class SubscriberUidResponse : Response
    {
        public SubscriberUidResponseData data { get; set; }
    }

    public class SubscriberUidResponseData
    {
        public string subscriber_uid { get; set; }
        public string status { get; set; }
        public DateTime date_added { get; set; }
    }

    public class SubscriberSearchResponse: Response
    {
        public SubscriberSearchResponseData data { get; set; }
    }

    public class SubscriberSearchResponseData
    {
        public List<Dictionary<string, string>> records { get; set; }
    }
}