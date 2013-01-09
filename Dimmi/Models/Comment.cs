using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models
{ 
    public class Comment
    {
        public Comment()
        {
            comment = String.Empty;
            commentByName = String.Empty;
        }

        public Guid commentBy { get; set; }
        [BsonDefaultValue("")]
        public string comment { get; set; }
        [BsonDefaultValue("")]
        public string commentByName { get; set; }
        public DateTime when { get; set; }
    }
}