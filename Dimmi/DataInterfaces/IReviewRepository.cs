using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dimmi.Models.Domain;
using MongoDB.Bson;

namespace Dimmi.DataInterfaces
{
    interface IReviewRepository
    {
        ReviewData Add(ReviewData review);
        IEnumerable<ReviewData> Get();
        IEnumerable<ReviewData> GetByReviewableId(Guid reviewAbleId);
        IEnumerable<ReviewData> GetByOwner(Guid userid);
        IEnumerable<ReviewData> GetByIdByOwner(Guid reviewAbleId, Guid userid);
        IEnumerable<ReviewData> GetByReviewType(string type, Guid userid);
        ReviewData Get(Guid reviewId);
        ReviewData Update(ReviewData review);

    }
}
