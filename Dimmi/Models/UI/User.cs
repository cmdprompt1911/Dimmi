using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace Dimmi.Models.UI
{
    public class User : BaseEntity
    {
        public string sessionToken { get; set; }
        public string oauthId { get; set; }
        public string emailAddress { get; set; }
        public DateTime lastLogin { get; set; }
        public DateTime createdDate { get; set; }
        public string locale { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int timezoneFromUTC { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string location { get; set; }
        public string fBUsername { get; set; }
        public string fBLink { get; set; }

    }
}