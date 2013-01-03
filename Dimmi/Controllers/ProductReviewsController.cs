using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dimmi.Models;
using Dimmi.Data;
using Dimmi.DataInterfaces;


namespace Dimmi.Controllers
{
    public class ProductReviewsController : ApiController
    {
        static readonly IProductReviewRepository repository = new ProductReviewRepository();

        public ProductReview Get(int reviewId)
        {
            ProductReview review = repository.Get(reviewId);
            if (review == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return review;
        }

        public IEnumerable<ProductReview> GetByProductId(int productId)
        {
            IEnumerable<ProductReview> reviews = repository.GetByProductId(productId);

            if (reviews == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return reviews;
        }

        public IEnumerable<ProductReview> GetByOwner(int userId)
        {
            IEnumerable<ProductReview> reviews = repository.GetByOwner(userId);
            if (reviews == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return reviews;
        }

        public IEnumerable<ProductReview> GetByOwner(int productId, int userId)
        {
            IEnumerable<ProductReview> reviews = repository.GetByIdByOwner(productId, userId);
            if (reviews == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return reviews;
        }

        public HttpResponseMessage Post(ProductReview review)
        {
            review = repository.Add(review);
            if (review == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            var response = Request.CreateResponse<ProductReview>(HttpStatusCode.Created, review);

            string uri = Url.Link("DefaultApi", new { id = review.reviewId });
            response.Headers.Location = new Uri(uri);
            return response;

        }

        public void Put(ProductReview review)
        {
            review = repository.Update(review);
            if (review == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

        }

    }
}
