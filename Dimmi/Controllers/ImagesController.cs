using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Dimmi.Models;
using Dimmi.Data;
using System.IO;
using Dimmi.DataInterfaces;


namespace Dimmi.Controllers
{
    public class ImagesController : ApiController
    {
        static readonly IImageRepository repository = new ImageRepository();

        public HttpResponseMessage Get(string id)
        {
            Image img = repository.Get(id);
            if (img == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            else
            {
                byte[] data = Convert.FromBase64String(img.data);

                MemoryStream ms = new MemoryStream(data);
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(data);
                result.Content.Headers.ContentType =
                                    new MediaTypeHeaderValue("image/" + img.fileTypeName);


                //result.Content = new StreamContent(ms);
                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream"); //"application/octet-stream"
                //result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline"); //"attachement"
                //result.Content.Headers.ContentDisposition.FileName = img.uid.ToLower() + "." + img.fileTypeName;
                return result;
            }


        }

        public IEnumerable<Image> GetCountForEachCategory(int count)
        {
            IEnumerable<Image> images = repository.GetCountForEachCategory(count);
            if (images == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            else
            {
                return images;
            }
        }

        
    }
}
