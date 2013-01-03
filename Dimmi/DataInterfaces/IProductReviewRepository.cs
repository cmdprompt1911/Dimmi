using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dimmi.Models;

namespace Dimmi.DataInterfaces
{
    interface IProductReviewRepository
    {
        ProductReview Add(ProductReview review);
        IEnumerable<ProductReview> GetByProductId(int productId);
        IEnumerable<ProductReview> GetByOwner(int userid);
        IEnumerable<ProductReview> GetByIdByOwner(int productId, int userid);
        ProductReview Get(int reviewId);
        ProductReview Update(ProductReview productReview);

    }
}
