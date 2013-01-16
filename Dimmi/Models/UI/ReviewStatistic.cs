using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models.UI
{
    public class ReviewStatistic : BaseEntity
    {
        public Guid userId { get; set; }
        public string userName { get; set; }
        public int numReviews { get; set; }
        public int numLikes { get; set; }
        public int score { get; set; }
        public int Last30Score { get; set; }
    }
}