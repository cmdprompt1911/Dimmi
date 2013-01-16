using System;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models.Domain
{
    public class ReviewStatisticData : BaseEntity
    {
        public ReviewStatisticData()
        {
            numReviews = 0;
            numLikes = 0;
            score = 0;
        }

        public Guid userId { get; set; }
        [BsonDefaultValue("")]
        public string userName { get; set; }
        public int numReviews { get; set; }
        public int numLikes { get; set; }
        public int score { get; set; }
    }
}