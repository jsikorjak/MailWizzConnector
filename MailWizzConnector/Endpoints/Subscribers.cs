using MailWizzConnector.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace MailWizzConnector
{
    public partial class MailWizzConnector
    {
        /// <summary>
        /// Adds the new subscriber to a given list with an unformatted or no name
        /// </summary>
        /// <param name="ListUid"></param>
        /// <param name="Email"></param>
        /// <param name="Name"></param>
        /// <param name="ExtraFields"></param>
        public void Subscribe(string ListUid, string Email, string Name = null, Dictionary<string, string> ExtraFields = null)
        {
            string firstName = "";
            string lastName = Name ?? "";

            int spaceIndex = -1;

            if ((spaceIndex = Name.IndexOf(' ')) > -1)
            {
                firstName = Name.Substring(0, spaceIndex);
                lastName = Name.Substring(spaceIndex + 1);
            }

            Subscribe(ListUid, Email, firstName, lastName, ExtraFields);
        }

        /// <summary>
        /// Adds the new subscriber to a given list with both first and last name specified
        /// </summary>
        /// <param name="ListUid"></param>
        /// <param name="Email"></param>
        /// <param name="FirstName"></param>
        /// <param name="LastName"></param>
        /// <param name="ExtraFields"></param>
        public void Subscribe(string ListUid, string Email, string FirstName, string LastName, Dictionary<string, string> ExtraFields = null)
        {
            if (!ValidateEmail(Email))
            {
                throw new Exception($@"E-mail ""{Email}"" is not valid!");
            }

            string endpointUrl = $"lists/{ListUid}/subscribers";

            var data = new Dictionary<string, string> {
                {"EMAIL", Email},
                {"FNAME", FirstName ?? ""},
                {"LNAME", LastName ?? ""}
            };

            Invoke(endpointUrl, "POST", data);

        }

        /// <summary>
        /// Gets a subscriber profile. E-mail match is exact, case-insensitive
        /// </summary>
        /// <param name="ListUid"></param>
        /// <param name="Email"></param>
        /// <returns></returns>
        public Subscriber GetSubscriber(string ListUid, string Email)
        {
            if (!ValidateEmail(Email))
            {
                throw new Exception($@"E-mail ""{Email}"" is not valid!");
            }

            string subscriberUid = GetSubscriberUidByEmail(ListUid, Email);

            string endpointUrl = $"lists/{ListUid}/subscribers/{subscriberUid}";

            Subscriber subscriber = new Subscriber(Invoke<SubscriberResponse>(endpointUrl, "GET"));

            return subscriber;
        }
        
        /// <summary>
        /// Searches within given list for subscribers matching given fields. The fields values are exact, case-insensitive matches.
        /// </summary>
        /// <param name="ListUid"></param>
        /// <param name="FilteringFields">Dictionary of filtering values, e.g. {"EMAIL", "test@example.com"}</param>
        /// <returns></returns>
        public List<Subscriber> SearchSubscribers(string ListUid, Dictionary<string, string> FilteringFields)
        {
            string filterUrlParameters = string.Join("&", FilteringFields.Select(q => string.Format("{0}={1}", HttpUtility.UrlEncode(q.Key), HttpUtility.UrlEncode(q.Value))));

            string endpointUrl = $"lists/{ListUid}/subscribers/search-by-custom-fields?{filterUrlParameters}";

            List<Subscriber> result = new List<Subscriber>();

            var rawResult = Invoke<SubscriberSearchResponse>(endpointUrl, "GET");

            foreach (var item in rawResult.data.records)
            {
                result.Add(new Subscriber(item));
            }

            return result;
        }
        
        /// <summary>
        /// Unsubscribes 
        /// </summary>
        /// <param name="ListUid"></param>
        /// <param name="Email"></param>
        public void Unsubscribe(string ListUid, string Email)
        {
            string endpointUrl = $"lists/{ListUid}/subscribers";

            if (!ValidateEmail(Email))
            {
                throw new Exception($@"E-mail ""{Email}"" is not valid!");
            }

            string subscriberUid = GetSubscriberUidByEmail(ListUid, Email);

            endpointUrl = $"lists/{ListUid}/subscribers/{subscriberUid}/unsubscribe";

            Invoke(endpointUrl, "PUT");
        }

        protected string GetSubscriberUidByEmail(string ListUid, string Email)
        {
            string endpointUrl = $"lists/{ListUid}/subscribers/search-by-email?EMAIL={Email}";

            if (!ValidateEmail(Email))
            {
                throw new Exception($@"E-mail ""{Email}"" is not valid!");
            }

            SubscriberUidResponse subscriberUid = Invoke<SubscriberUidResponse>(endpointUrl, "GET");

            return subscriberUid.data.subscriber_uid;
        }

        protected bool ValidateEmail(string Email)
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            Regex r = new Regex(validEmailPattern, RegexOptions.IgnoreCase);

            return r.IsMatch(Email);
        }
    }
}