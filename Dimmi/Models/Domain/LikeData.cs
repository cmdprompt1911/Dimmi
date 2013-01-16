using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models.Domain
{ 
    public class LikeData
    {
        public LikeData()
        {

            likedByName = String.Empty;
        }
        public Guid likedBy { get; set; }
        [BsonDefaultValue("")]
        public string likedByName { get; set; }
        public DateTime when { get; set; }
    }
}