using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;

namespace Dimmi.Models.Domain
{
    public class ReviewableData : BaseEntity
    {
        public ReviewableData()
        {
            parentName = String.Empty;
            name = String.Empty;
            description = String.Empty;
            description2 = String.Empty;
            outsideCode = String.Empty;
            outsideCodeType = String.Empty;
            reviewableType = String.Empty;
            issuerCountry = String.Empty;
            issuerCountryCode = "";
            parentName = String.Empty;
            isStaffReviewed = false;
        }
        [BsonDefaultValue("")]
        public string name { get; set; }
        [BsonDefaultValue("")]
        public string description { get; set; }
        [BsonDefaultValue("")]
        public string description2 { get; set; }
        [BsonDefaultValue("")]
        public string outsideCode { get; set; }
        [BsonDefaultValue("")]
        public string outsideCodeType { get; set; }
        [BsonDefaultValue("")]
        public string reviewableType { get; set; }
        [BsonDefaultValue("")]
        public string issuerCountry { get; set; }
        [BsonDefaultValue("")]
        public string issuerCountryCode { get; set; }
        public DateTime lastModified { get; set; }
        public DateTime createdDate { get; set; }
        [BsonIgnoreIfNull]
        public Guid parentReviewableId { get; set; }
        [BsonDefaultValue("")]
        public string parentName { get; set; }
        public int numReviews { get; set; }
        public double compositRating { get; set; }
        [BsonDefaultValue(false)]
        public bool isStaffReviewed { get; set; }
                
        public string[] images { get; set; }

        [BsonIgnore]
        public double searchScore { get; set; }
    }
}