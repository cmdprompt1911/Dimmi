using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models
{
    public class ReviewBase : ReviewableBase
    {
        public ReviewBase()
        {
            userName = String.Empty;
            text = String.Empty;
            fBFeedPostId = String.Empty;
            fBTimelinePostId = String.Empty;
            providedByBizName = String.Empty;
            comments = new List<Comment>();
            likes = new List<Like>();
        }
        

        public Guid user { get; set; }
        [BsonDefaultValue("")]
        public string userName { get; set; }
        [BsonDefaultValue("")]
        public string text { get; set; }
        [BsonDefaultValue("")]
        public string fBFeedPostId { get; set; }
        public double rating { get; set; }
        [BsonDefaultValue("")]
        public string fBTimelinePostId { get; set; }
        [BsonIgnoreIfNull]
        public List<Comment> comments { get; set; }
        [BsonIgnoreIfNull]
        public Guid providedByBizId { get; set; }
        [BsonDefaultValue("")]
        public string providedByBizName { get; set; }
        [BsonIgnoreIfNull]
        public List<Like> likes { get; set; }
    }
}