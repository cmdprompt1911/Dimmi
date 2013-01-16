using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models.UI
{ 
    public class Like
    {
        public Guid likedBy { get; set; }
        public string likedByName { get; set; }
        public DateTime when { get; set; }
    }
}