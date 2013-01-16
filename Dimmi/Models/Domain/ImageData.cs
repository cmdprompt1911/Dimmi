﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;



namespace Dimmi.Models.Domain
{
    public class ImageData : BaseEntity
    {
        public ImageData()
        {
            category = String.Empty;
        }
        
        public string description { get; set; }
        public byte[] data { get; set; }
        public string fileType { get; set; }
        public DateTime dateCreated { get; set; }
        [BsonDefaultValue("")]
        public string category { get; set; }
        public int type { get; set; } //only used by some objects.
    }
}