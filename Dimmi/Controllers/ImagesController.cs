using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Dimmi.Models.UI;
using Dimmi.Models.Domain;
using Dimmi.Data;
using System.IO;
using Dimmi.DataInterfaces;
using MongoDB.Bson;


namespace Dimmi.Controllers
{
    public class ImagesController : ApiController
    {
        static readonly IImageRepository repository = new ImageRepository();

        public HttpResponseMessage Get(Guid id)
        {
            ImageData img = repository.Get(id);
            if (img == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            else
            {
                byte[] data = img.data;

                MemoryStream ms = new MemoryStream(data);
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(data);
                result.Content.Headers.ContentType =
                                    new MediaTypeHeaderValue("image/" + img.fileType);


                //result.Content = new StreamContent(ms);
                //result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream"); //"application/octet-stream"
                //result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline"); //"attachement"
                //result.Content.Headers.ContentDisposition.FileName = img.uid.ToLower() + "." + img.fileTypeName;
                return result;
            }


        }

        public IEnumerable<Image> GetTopForEachCategory(int count)
        {
            List<ImageData> imagesDatas = (List<ImageData>)repository.GetTopForEachCategory();
            if (imagesDatas == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            List<Image> images = AutoMapper.Mapper.Map<List<ImageData>, List<Image>>(imagesDatas);
            return images;
        }

        
    }
}
