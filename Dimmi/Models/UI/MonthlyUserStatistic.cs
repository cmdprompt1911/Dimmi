using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;

namespace Dimmi.Models.UI
{
    public class MonthlyUserStatistic : BaseEntity
    {

        public Guid userId { get; set; }
        public string userName { get; set; }
        public int numReviews { get; set; }
        public int numLikes { get; set; }
        public int score { get; set; }
        public string monthName { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int rank { get; set; }
        public DateTime firstDayOfMonth { get; set; }
        public DateTime lastDayOfMonth { get; set; }
   
    }
}