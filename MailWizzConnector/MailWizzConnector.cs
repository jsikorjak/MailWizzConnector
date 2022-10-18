using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace MailWizzConnector
{
    public partial class MailWizzConnector
    {
        protected string _ApiUrlRoot;
        protected string _ApiKey;

        #region Properties encapsulators

        public string ApiUrlRoot
        {
            get
            {
                return this._ApiUrlRoot;
            }
            set
            {
                this._ApiUrlRoot = value;
            }
        }

        public string ApiKey
        {
            get
            {
                return this._ApiKey;
            }
            set
            {
                this.ApiKey = value;
            }
        }

        #endregion

        public MailWizzConnector()
        {

        }

        public MailWizzConnector(string ApiUrlRoot, string ApiKey)
        {
            this._ApiUrlRoot = ApiUrlRoot;
            this._ApiKey = ApiKey;
        }
    }
}