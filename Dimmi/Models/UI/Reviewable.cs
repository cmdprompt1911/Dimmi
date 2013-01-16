using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models.UI
{
    public class Reviewable : BaseEntity
    {
        public string name { get; set; }
        public string description { get; set; }
        public string description2 { get; set; }
        public string outsideCode { get; set; }
        public string outsideCodeType { get; set; }
        public string reviewableType { get; set; }
        public string issuerCountry { get; set; }
        public string issuerCountryCode { get; set; }
        public DateTime lastModified { get; set; }
        public DateTime createdDate { get; set; }
        public Guid parentReviewableId { get; set; }
        public string parentName { get; set; }
        public int numReviews { get; set; }
        public double compositRating { get; set; }
        public bool isStaffReviewed { get; set; }

        //computed, set when retrieved.
        public bool hasReviewed { get; set; }
        public Guid hasReviewedId { get; set; }
        //have to manually map from String[]
        public List<Image> images { get; set; }
    }
}