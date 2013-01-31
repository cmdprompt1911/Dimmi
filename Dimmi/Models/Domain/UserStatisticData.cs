using System;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models.Domain
{
    public class UserStatisticData : BaseEntity
    {
        public UserStatisticData()
        {
            numReviews = 0;
            numLikes = 0;
            score = 0;
            rank = 0;
            mvp1Count = 0;
            mvp2Count = 0;
            mvp3Count = 0;
        }

        public Guid userId { get; set; }
        [BsonDefaultValue("")]
        public string userName { get; set; }
        public int numReviews { get; set; }
        public int numLikes { get; set; }
        public int score { get; set; }
        public int rank { get; set; }
        public int mvp1Count { get; set; }
        public int mvp2Count { get; set; }
        public int mvp3Count { get; set; }
    }
}