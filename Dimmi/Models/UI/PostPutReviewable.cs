using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models.UI
{
    public class PostPutReviewable
    {
        public Guid userId {get; set; }
        public string sessionToken {get; set; }
        public Reviewable reviewable { get; set; }
    }
}