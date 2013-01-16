using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models.UI
{
    public class Review : BaseEntity
    {
        public string text { get; set; }
        public double rating { get; set; }
        public string reviewableType { get; set; }
        public DateTime lastModified { get; set; }
        public DateTime createdDate { get; set; }
        public Guid parentReviewableId { get; set; }
        public Guid user { get; set; }
        public string userName { get; set; }
        public string fBFeedPostId { get; set; }
        public string fBTimelinePostId { get; set; }
        public List<Comment> comments { get; set; }
        public List<Like> likes { get; set; }
        //computed, set when retrieved.
        public bool hasReviewed { get; set; }
        public Guid hasReviewedId { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        
        
        
        


        //FROM REVIEWABLE
        public string name { get; set; }
        public string description { get; set; }
        public string description2 { get; set; }
        public string outsideCode { get; set; }
        public string outsideCodeType { get; set; }
        public string issuerCountry { get; set; }
        public string issuerCountryCode { get; set; }
        public bool isStaffReviewed { get; set; }
        public Guid providedByBizId { get; set; }
        public string providedByBizName { get; set; }
        //have to manually map from String[]
        public List<Image> images { get; set; }




        

        
        
    }
}