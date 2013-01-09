using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models
{
    public class ReviewableBase : BaseEntity
    {
        public ReviewableBase()
        {
            parentName = String.Empty;
            name = String.Empty;
            description = String.Empty;
            description2 = String.Empty;
            outsideCode = String.Empty;
            outsideCodeType = String.Empty;
            reviewableType = String.Empty;
            issuerCountry = String.Empty;
            issuerCountryCode = String.Empty;
            parentName = String.Empty;
            isStaffReviewed = false;
            hasReviewed = false;
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
                
        //computed, set when retrieved.
        [BsonDefaultValue(false)]
        public bool hasReviewed { get; set; }
        [BsonIgnoreIfNull]
        public Guid hasReviewedId { get; set; }
    }
}