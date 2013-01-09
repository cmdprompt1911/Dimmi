using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dimmi.Data;
using Dimmi.DataInterfaces;
using MongoDB.Bson;
using Dimmi.Models;



namespace Dimmi.Controllers
{
    public class ReviewablesController : ApiController
    {
        static readonly IReviewableRepository repository = new ReviewableRepository();


        public Reviewable Get(Guid id, Guid userId)
        {
            //ObjectId uid = ObjectId.Parse(userId);
            Reviewable reviewable = repository.Get(id, userId);

            
            if (reviewable == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return reviewable;
        }

        public IEnumerable<Reviewable> GetByName(string name, Guid userId)
        {
            //ObjectId uid = ObjectId.Parse(userId);
            IEnumerable<Reviewable> reviewable = repository.GetByName(name, userId);
            if (reviewable == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return reviewable;
        }

        public IEnumerable<Reviewable> GetByNameByType(string name, string type, Guid userId)
        {
            //ObjectId uid = ObjectId.Parse(userId);
            IEnumerable<Reviewable> reviewable = repository.GetByNameByType(name, type, userId);
            if (reviewable == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return reviewable;
        }

        public HttpResponseMessage Post(Reviewable reviewable, Guid userId)
        {
            
            reviewable = repository.Add(reviewable, userId);
            if (reviewable == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            var response = Request.CreateResponse<Reviewable>(HttpStatusCode.Created, reviewable);

            string uri = Url.Link("DefaultApi", new { id = reviewable.id });
            response.Headers.Location = new Uri(uri);
            return response;

        }

        //public int AddProduct(Product product)
        //{
        //    int newProductId = ProductDAL.AddProduct(product.code, product.name, product.description, product.countryCode, product.manufacturerId, product.modelNum);
        //    return newProductId;
        //}

    }
}
