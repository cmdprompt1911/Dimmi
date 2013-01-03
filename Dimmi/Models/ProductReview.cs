using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models
{
    public class ProductReview
    {
        public int reviewId { get; set; }
        public int reviewInstanceId { get; set; }
        public int owner { get; set; }
        public string ownerName { get; set; }
        public int productId { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
        public DateTime created { get; set; }
        public DateTime lastUpdated { get; set; }
        public string comment { get; set; }
        public double rating { get; set; }
        public double compositeRating { get; set; }
        public int latestReviewInstanceId { get; set; }
        public string image { get; set; }
        public string imageFileType { get; set; }
        public string fBFeedPostId { get; set; }
        public string fBTimelinePostId { get; set; }
    }
}