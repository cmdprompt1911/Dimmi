using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models.UI
{
    public class UserStatistic : BaseEntity
    {
        public Guid userId { get; set; }
        public string userName { get; set; }
        public int numReviews { get; set; }
        public int numLikes { get; set; }
        public int score { get; set; }
        public int currentMonthScore { get; set; }
        public int rank { get; set; }
        public int mvp1Count { get; set; }
        public int mvp2Count { get; set; }
        public int mvp3Count { get; set; }
    }
}