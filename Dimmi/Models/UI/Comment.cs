using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models.UI
{ 
    public class Comment
    {
        public Guid commentBy { get; set; }
        public string comment { get; set; }
        public string commentByName { get; set; }
        public DateTime when { get; set; }
    }
}