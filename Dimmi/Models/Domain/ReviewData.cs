using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;


namespace Dimmi.Models.Domain
{
    public class ReviewData : BaseEntity
    {
        public ReviewData()
        {
            parentName = String.Empty;
            text = String.Empty;
            reviewableType = String.Empty;
            userName = String.Empty;
            fBFeedPostId = String.Empty;
            fBTimelinePostId = String.Empty;
            parentName = String.Empty;
            rating = 0;
            comments = new List<CommentData>();
            likes = new List<LikeData>();

        }
        [BsonDefaultValue("")]
        public string text { get; set; }
        [BsonDefaultValue(0)]
        public double rating { get; set; }

        [BsonDefaultValue("")]
        public string reviewableType { get; set; }
        public DateTime lastModified { get; set; }
        public DateTime createdDate { get; set; }
        [BsonIgnoreIfNull]
        public Guid parentReviewableId { get; set; }
        [BsonDefaultValue("")]
        public string parentName { get; set; }
        public Guid user { get; set; }
        [BsonDefaultValue("")]
        public string userName { get; set; }
        [BsonDefaultValue("")]
        public string fBFeedPostId { get; set; }
        [BsonDefaultValue("")]
        public string fBTimelinePostId { get; set; }
        
              

        public List<CommentData> comments { get; set; }
        public List<LikeData> likes { get; set; }

        //[BsonDefaultValue("")]
        //public string name { get; set; }
        //[BsonDefaultValue("")]
        //public string description { get; set; }
        //[BsonDefaultValue("")]
        //public string description2 { get; set; }
        //[BsonDefaultValue("")]
        //public string outsideCode { get; set; }
        //[BsonDefaultValue("")]
        //public string outsideCodeType { get; set; }
        //public bool isStaffReviewed { get; set; }
        //[BsonDefaultValue("")]
        //public string issuerCountry { get; set; }
        //[BsonDefaultValue("")]
        //public string issuerCountryCode { get; set; }
        
    }
}