using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;

namespace Dimmi.Models.Domain
{
    public class MonthlyUserStatisticData : BaseEntity
    {
        public MonthlyUserStatisticData()
        {
            numReviews = 0;
            numLikes = 0;
            score = 0;
            year = 0;
            month = 0;
            rank = 0;
            userName = String.Empty;
            monthName = String.Empty;

        }

        public Guid userId { get; set; }
        [BsonDefaultValue("")]
        public string userName { get; set; }
        public int numReviews { get; set; }
        public int numLikes { get; set; }
        public int score { get; set; }
        [BsonDefaultValue("")]
        public string monthName { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public int rank { get; set; }
        public DateTime firstDayOfMonth { get; set; }
        public DateTime lastDayOfMonth { get; set; }
   
    }
}