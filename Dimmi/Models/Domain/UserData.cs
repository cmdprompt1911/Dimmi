﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace Dimmi.Models.Domain
{
    public class UserData : BaseEntity
    {
        public UserData()
        {
            locale = String.Empty;
            firstName = String.Empty;
            lastName = String.Empty;
            name = String.Empty;
            gender = String.Empty;
            location = String.Empty;
            fBUsername = String.Empty;
            fBLink = String.Empty;
            sessionMaterial = String.Empty;
            sessionToken = String.Empty;
            oauthId = String.Empty;
            emailAddress = String.Empty;
        }
        [BsonDefaultValue("")]
        public string oauthId { get; set; }
        [BsonDefaultValue("")]
        public string sessionToken { get; set; }
        [BsonDefaultValue("")]
        public string sessionMaterial { get; set; }
        public DateTime expires { get; set; }
        [BsonDefaultValue("")]
        public string emailAddress { get; set; }
        public DateTime lastLogin { get; set; }
        public DateTime createdDate { get; set; }
        [BsonDefaultValue("")]
        public string locale { get; set; }
        [BsonDefaultValue("")]
        public string firstName { get; set; }
        [BsonDefaultValue("")]
        public string lastName { get; set; }
        public int timezoneFromUTC { get; set; }
        [BsonDefaultValue("")]
        public string name { get; set; }
        [BsonDefaultValue("")]
        public string gender { get; set; }
        [BsonDefaultValue("")]
        public string location { get; set; }
        [BsonDefaultValue("")]
        public string fBUsername { get; set; }
        [BsonDefaultValue("")]
        public string fBLink { get; set; }

    }
}