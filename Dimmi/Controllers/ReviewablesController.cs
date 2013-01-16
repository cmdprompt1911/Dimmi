using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dimmi.Data;
using Dimmi.DataInterfaces;
using MongoDB.Bson;
using Dimmi.Models.UI;
using Dimmi.Models.Domain;
using System.Web;



namespace Dimmi.Controllers
{
    public class ReviewablesController : ApiController
    {
        static readonly IReviewableRepository repository = new ReviewableRepository();
        static readonly IImageRepository _imagesRepository = new ImageRepository();
        static readonly IReviewRepository _reviewRepository = new ReviewRepository();

        static readonly UsersController _usersController = new UsersController();


        public Reviewable Get(Guid id, Guid userId, string sessionToken)
        {
            if(!_usersController.IsUserValid(userId, sessionToken))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            ReviewableData reviewableData = repository.Get(id, userId);
            if (reviewableData == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            Reviewable reviewable = PopulateData(reviewableData);

            reviewable = GetComputedValues(reviewable, userId);

            return reviewable;
        }

        public IEnumerable<Reviewable> GetByName(string name, Guid userId, string sessionToken)
        {
            if (!_usersController.IsUserValid(userId, sessionToken))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            List<ReviewableData> reviewableData = (List < ReviewableData > )repository.GetByName(HttpUtility.UrlDecode(name), userId);
            if (reviewableData == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            List<Reviewable> reviewables = PopulateData(reviewableData);
            reviewables = GetComputedValues(reviewables, userId);

            return reviewables;
        }

        public IEnumerable<Reviewable> GetByNameByType(string name, string type, Guid userId, string sessionToken)
        {
            
            if (!_usersController.IsUserValid(userId, sessionToken))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            List<ReviewableData> reviewableData = (List<ReviewableData>)repository.GetByNameByType(HttpUtility.UrlDecode(name), type, userId);
            if (reviewableData == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            List<Reviewable> reviewables = PopulateData(reviewableData);
            reviewables = GetComputedValues(reviewables, userId);

            return reviewables;
        }

        public HttpResponseMessage Post(PostPutReviewable reviewable)
        {
            if (!_usersController.IsUserValid(reviewable.userId, reviewable.sessionToken))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            if (reviewable == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            //map the reviewable to reviewableData
            ReviewableData reviewableData = AutoMapper.Mapper.Map<Reviewable, ReviewableData>(reviewable.reviewable);

            //first save the images and store the Guid's
            List<string> ids = new List<string>();
            if (reviewable.reviewable.images != null)
            {
                foreach (Image i in reviewable.reviewable.images)
                {
                    Guid newImgId = Guid.NewGuid();

                    i.id = newImgId;
                    //map each image to an imagedata
                    AutoMapper.Mapper.CreateMap<Image, ImageData>();
                    ImageData imageData = AutoMapper.Mapper.Map<Image, ImageData>(i);

                    //save each imagedata
                    imageData = _imagesRepository.Add(imageData);
                    //store each id in the string array
                    ids.Add(imageData.id.ToString());
                }
            }
            //add the image ids to the reviewableData string array
            reviewableData.images = ids.ToArray();

            //set the created and lastmodified dates
            reviewableData.createdDate = DateTime.UtcNow;
            reviewableData.lastModified = reviewableData.createdDate;

            //now save the reviewable
            reviewableData = repository.Add(reviewableData, reviewable.userId);

            //now transform the reviewableData back to a reviewable
            AutoMapper.Mapper.CreateMap<ReviewableData, Reviewable>();
            reviewable.reviewable = AutoMapper.Mapper.Map<ReviewableData, Reviewable>(reviewableData);
            //reviewable = CopyImagesfromDomainToUI(reviewableData, reviewable);
            reviewable.reviewable = GetComputedValues(reviewable.reviewable, reviewable.userId);

            var response = Request.CreateResponse<Reviewable>(HttpStatusCode.Created, reviewable.reviewable);

            string uri = Url.Link("DefaultApi", new { id = reviewable.reviewable.id });
            response.Headers.Location = new Uri(uri);
            return response;

        }

        public List<Image> GetImagesFromStringArray(string[] imageIds)
        {
            List<ImageData> imagesDatas = (List<ImageData>)_imagesRepository.Get(imageIds);
            List<Image> results = new List<Image>();
            if (imagesDatas != null && imagesDatas.Count > 0)
            {
                foreach (ImageData id in imagesDatas)
                {
                    results.Add(AutoMapper.Mapper.Map<ImageData, Image>(id));
                }
                //List<Image> result = AutoMapper.Mapper.Map<List<ImageData>, List<Image>>(imagesDatas);
                return results;
            }
            else
            {
                return new List<Image>();
            }

        }

        private List<Reviewable> PopulateData(List<ReviewableData> reviewableDatas)
        {


            List<Reviewable> output = new List<Reviewable>();
            foreach (ReviewableData rd in reviewableDatas)
            {
                output.Add(PopulateData(rd));
            }
            return output;
        }

        private Reviewable PopulateData(ReviewableData reviewableData)
        {

            Reviewable reviewable = AutoMapper.Mapper.Map<ReviewableData, Reviewable>(reviewableData);
            return reviewable;
        }

        private Reviewable GetComputedValues(Reviewable reviewable, Guid userId)
        {
            if (userId.Equals(Guid.Empty))
                return reviewable;

            IEnumerable<ReviewData> review = _reviewRepository.GetByIdByOwner(reviewable.id, userId);
            
            if (review != null && review.Count() > 0)
            {
                reviewable.hasReviewed = true;
                reviewable.hasReviewedId = review.Last<ReviewData>().id;
            }
            else
            {
                reviewable.hasReviewed = false;
                reviewable.hasReviewedId = Guid.Empty;
            }
            return reviewable;
        }

        private List<Reviewable> GetComputedValues(List<Reviewable> reviewables, Guid userId)
        {
            List<Reviewable> output = new List<Reviewable>();

            foreach (Reviewable r in reviewables)
            {
                output.Add(GetComputedValues(r, userId));
            }
            return output;
        }

    }
}
