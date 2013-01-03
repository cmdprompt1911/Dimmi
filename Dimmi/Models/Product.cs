using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models
{
    public class Product
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string code { get; set; }
        public string codeType { get; set; }
        public string status { get; set; }
        public string issuerCountryCode { get; set; }
        public string issuerCountry { get; set; }
        public DateTime lastModifiedUTC { get; set; }
        public string manufacturerid { get; set; }
        public string modelNum { get; set; }
        public int companyId { get; set; }
        public string companyName { get; set; }
        public int numReviews { get; set; }
        public double compositRating { get; set; }
        public bool hasReviewed { get; set; }
        public int hasReviewedId { get; set; }
        public string image { get; set; }
        public string imageFileType { get; set; }
        public bool isStaffReviewed { get; set; }
    }
}