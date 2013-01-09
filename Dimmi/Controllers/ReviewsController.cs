using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dimmi.Models;
using Dimmi.Data;
using Dimmi.DataInterfaces;
using MongoDB.Bson;


namespace Dimmi.Controllers
{
    public class ReviewsController : ApiController
    {
        static readonly IReviewRepository repository = new ReviewRepository();

        public Review Get(Guid reviewId)
        {
            Review review = repository.Get(reviewId);
            if (review == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return review; 
        }

        public IEnumerable<Review> GetByReviewableId(Guid reviewableId)
        {
            IEnumerable<Review> reviews = repository.GetByReviewableId(reviewableId);

            if (reviews == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return reviews;
        }

        public IEnumerable<Review> GetByOwner(Guid userId)
        {
            IEnumerable<Review> reviews = repository.GetByOwner(userId);
            if (reviews == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return reviews;
        }

        public IEnumerable<Review> GetByOwner(Guid reviewableId, Guid userId)
        {
            IEnumerable<Review> reviews = repository.GetByIdByOwner(reviewableId, userId);
            if (reviews == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return reviews;
        }

        public HttpResponseMessage Post(Review review)
        {
            Review check = repository.Get(review.id);
            if (check != null)
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
            }

            review = repository.Add(review);
            if (review == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            var response = Request.CreateResponse<Review>(HttpStatusCode.Created, review);

            string uri = Url.Link("DefaultApi", new { id = review.id });
            response.Headers.Location = new Uri(uri);
            return response;

        }

        public void Put(Review review)
        {
            if (review == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            review = repository.Update(review);
            
        }

    }
}
