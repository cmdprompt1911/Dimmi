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
    public class ProductsController : ApiController
    {
        static readonly IProductRepository repository = new ProductRepository();


        public Product Get(int id, int userId)
        {
            Product product = repository.Get(id, userId);

            
            if (product == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return product;
        }

        public IEnumerable<Product> GetByName(string name, int userId)
        {
            IEnumerable<Product> products = repository.GetByName(name, userId);
            if (products == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return products;
        }

        public HttpResponseMessage Post(Product product)
        {
            product = repository.Add(product);
            if (product == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            var response = Request.CreateResponse<Product>(HttpStatusCode.Created, product);

            string uri = Url.Link("DefaultApi", new { id = product.id });
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
