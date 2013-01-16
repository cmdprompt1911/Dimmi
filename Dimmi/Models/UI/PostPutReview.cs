using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models.UI
{
    public class PostPutReview
    {
        public Guid userId { get; set; }
        public string sessionToken { get; set; }
        public Review review { get; set; }
    }
}