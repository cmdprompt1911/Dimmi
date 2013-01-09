using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dimmi.Models;
using MongoDB.Bson;

namespace Dimmi.DataInterfaces
{
    interface IReviewRepository
    {
        Review Add(Review review);
        IEnumerable<Review> Get();
        IEnumerable<Review> GetByReviewableId(Guid reviewAbleId);
        IEnumerable<Review> GetByOwner(Guid userid);
        IEnumerable<Review> GetByIdByOwner(Guid reviewAbleId, Guid userid);
        IEnumerable<Review> GetByReviewType(string type, Guid userid);
        Review Get(Guid reviewId);
        Review Update(Review review);

    }
}
