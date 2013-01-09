using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models
{
    public class ReviewableData : ReviewableBase
    {
        public string[] images { get; set; }
    }
}