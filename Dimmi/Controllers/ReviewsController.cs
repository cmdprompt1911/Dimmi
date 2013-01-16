using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dimmi.Models.UI;
using Dimmi.Models.Domain;
using Dimmi.Data;
using Dimmi.DataInterfaces;
using MongoDB.Bson;


namespace Dimmi.Controllers
{
    public class ReviewsController : ApiController
    {
        static readonly IReviewRepository repository = new ReviewRepository();
        static readonly IReviewableRepository _reviewableRepository = new ReviewableRepository();
        static readonly IImageRepository _imagesRepository = new ImageRepository();

        static readonly UsersController _usersController = new UsersController();

        public Review Get(Guid reviewId, Guid userId, string sessionToken)
        {
            if (!_usersController.IsUserValid(userId, sessionToken))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            //get data
            ReviewData reviewData = repository.Get(reviewId);

            //populate the review object
            Review review = PopulateData(reviewData);

            if (review == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return review; 
        }

        public IEnumerable<Review> GetByReviewableId(Guid reviewableId, Guid userId, string sessionToken)
        {
            if (!_usersController.IsUserValid(userId, sessionToken))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            List<ReviewData> reviewDatas = (List<ReviewData>)repository.GetByReviewableId(reviewableId);

            if (reviewDatas == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            
            List<Review> output = PopulateData(reviewDatas);
            return output;
        }

        public IEnumerable<Review> GetByOwner(Guid userId, string sessionToken)
        {
            if (!_usersController.IsUserValid(userId, sessionToken))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            List<ReviewData> reviewDatas = (List<ReviewData>)repository.GetByOwner(userId);
            if (reviewDatas == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            List<Review> output = PopulateData(reviewDatas);
            return output;
        }

        public IEnumerable<Review> GetByOwner(Guid reviewableId, Guid owner, Guid userId, string sessionToken)
        {
            if (!_usersController.IsUserValid(userId, sessionToken))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            List<ReviewData> reviewDatas = (List<ReviewData>)repository.GetByIdByOwner(reviewableId, owner);
            if (reviewDatas == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            List<Review> output = PopulateData(reviewDatas);
            return output;
        }

        private List<Review> PopulateData(List<ReviewData> reviewDatas)
        {
            //collect the reviews parentreviewableids
            List<String> parentIds = new List<string>();
            foreach (ReviewData rd in reviewDatas)
            {
                parentIds.Add(rd.parentReviewableId.ToString());
            }
            //retreive the matching parent reviewableDatas
            List<ReviewableData> reviewablesData = (List<ReviewableData>)_reviewableRepository.Get(parentIds.ToArray());

            List<Review> output = new List<Review>();
            foreach (ReviewData rd in reviewDatas)
            {
                //find the matching parent
                foreach (ReviewableData rable in reviewablesData)
                {
                    if (rd.parentReviewableId == rable.id)
                    {

                        //populate review data
                        Review review = AutoMapper.Mapper.Map<ReviewData, Review>(rd);
                        //populate parent reviewable data
                        AutoMapper.Mapper.Map(rable, review, typeof(ReviewableData), typeof(Review));

                        output.Add(review);
                        break;
                    }
                }
            }
            return output;
        }

        private Review PopulateData(ReviewData reviewData)
        {
            //retreive the matching parent reviewableData
            ReviewableData reviewableData = _reviewableRepository.Get(reviewData.parentReviewableId, Guid.Empty);

            //populate parent reviewable data
            //Review review = AutoMapper.Mapper.Map<ReviewableData, Review>(reviewableData);
            //populate review data
            //review = AutoMapper.Mapper.Map<ReviewData, Review>(reviewData);

            Review review = AutoMapper.Mapper.Map<ReviewData, Review>(reviewData);
            AutoMapper.Mapper.Map(reviewableData, review, typeof(ReviewableData), typeof(Review));
            //populate images from parent reviewable
            //review = CopyImagesFromDomainToUI(reviewableData, review);


            return review;
        }

        public HttpResponseMessage Post(PostPutReview review)
        {
            if (!_usersController.IsUserValid(review.userId, review.sessionToken))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            ReviewData check = repository.Get(review.review.id);
            if (check != null)
            {
                throw new HttpResponseException(HttpStatusCode.MethodNotAllowed);
            }
            ReviewData reviewData = AutoMapper.Mapper.Map<Review, ReviewData>(review.review);
            reviewData = repository.Add(reviewData);
            if (reviewData == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            Review output = PopulateData(reviewData);

            var response = Request.CreateResponse<Review>(HttpStatusCode.Created, output);

            string uri = Url.Link("DefaultApi", new { id = output.id });
            response.Headers.Location = new Uri(uri);
            return response;

        }

        public void Put(PostPutReview review)
        {

            if (review == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            if (!_usersController.IsUserValid(review.userId, review.sessionToken))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            ReviewData serverData = repository.Get(review.review.id);
            if (serverData == null) //trying to "update" an object that doesn't exist.
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }
            
            ReviewData reviewData = AutoMapper.Mapper.Map<Review, ReviewData>(review.review);
            
            
            if (reviewData.comments.Count == serverData.comments.Count && reviewData.user == review.review.user)
                reviewData.lastModified = DateTime.UtcNow; //review updated, not just added a comment - and the user is the updater...

            repository.Update(reviewData);
            
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

 

    }
}
