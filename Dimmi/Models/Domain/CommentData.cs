using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models.Domain
{ 
    public class CommentData
    {
        public CommentData()
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