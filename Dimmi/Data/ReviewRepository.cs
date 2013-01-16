using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dimmi.Models.Domain;
using Dimmi.DataInterfaces;
using MongoDB.Bson;
using MongoDB.Driver.Builders;


namespace Dimmi.Data
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DBRepository.MongoRepository<ReviewData> _reviewRepository;
        private readonly DBRepository.MongoRepository<ImageData> _imagesRepository;
        private readonly DBRepository.MongoRepository<ReviewableData> _reviewableRepository;
        private ReviewableRepository _rr;

        public ReviewRepository()
        {
            _reviewRepository = new DBRepository.MongoRepository<ReviewData>("Reviews");
            _imagesRepository = new DBRepository.MongoRepository<ImageData>("Images");
            _reviewableRepository = new DBRepository.MongoRepository<ReviewableData>("Reviewables");
            _rr = new ReviewableRepository();
        }


        public ReviewData Add(ReviewData review)
        {
            review.createdDate = DateTime.UtcNow;
            review.lastModified = DateTime.UtcNow;
            //review = CopyFromReviewableToNew(review);
            //ReviewData newR = CopyFromModelToData(review);
            Guid newId = Guid.NewGuid();
            review.id = newId;
            _reviewRepository.Collection.Insert(review);
            _rr.UpdateStatistics(review.parentReviewableId);
            return Get(newId);
        }

        public IEnumerable<ReviewData> GetByReviewableId(Guid reviewableId)
        {
            var query = Query.EQ("parentReviewableId", reviewableId.ToString());
            var result = _reviewRepository.Collection.FindAs<ReviewData>(query).ToList();
            //List<ReviewData> output = new List<ReviewData>();
            //foreach (ReviewData rd in result)
            //{
            //    Review newR = CopyFromDataToModel(rd);
            //    output.Add(newR);
            //}
            //return output;
            return (IEnumerable<ReviewData>)result;
        }


        public IEnumerable<ReviewData> GetByOwner(Guid userId)
        {
            var query = Query.EQ("user", userId.ToString());
            var result = _reviewRepository.Collection.FindAs<ReviewData>(query).ToList();
            //List<Review> output = new List<Review>();
            //foreach (ReviewData rd in result)
            //{
            //    Review newR = CopyFromDataToModel(rd);
            //    output.Add(newR);
            //}
            //return output;
            return result;
        }

        public IEnumerable<ReviewData> GetByIdByOwner(Guid reviewableId, Guid userId)
        {
            var query = Query.And(Query.EQ("parentReviewableId", reviewableId.ToString()), Query.EQ("user", userId.ToString()));
            var result = _reviewRepository.Collection.Find(query).ToList();
            //List<Review> output = new List<Review>();
            //foreach (ReviewData rd in result)
            //{
            //    Review newR = CopyFromDataToModel(rd);
            //    output.Add(newR);
            //}
            //return output;
            return result;
        }

        public IEnumerable<ReviewData> GetByReviewType(string type, Guid userId)
        {
            var query = Query.EQ("type", type);
            var result = _reviewRepository.Collection.Find(query).ToList();
            //List<Review> output = new List<Review>();
            //foreach (ReviewData rd in result)
            //{
            //    Review newR = CopyFromDataToModel(rd);
            //    output.Add(newR);
            //}
            //return output;
            return result;
        }

        public ReviewData Get(Guid reviewId)
        {
            var query = Query.EQ("_id", reviewId.ToString());
            var result = _reviewRepository.Collection.FindOneAs<ReviewData>(query);
            //Review newR = CopyFromDataToModel(result);
            //return newR;
            return result;
        }

        public IEnumerable<ReviewData> Get()
        {
            var result = _reviewRepository.Collection.FindAllAs<ReviewData>().ToList();
            //List<Review> output = new List<Review>();
            //foreach (ReviewData rd in result)
            //{
            //    Review newR = CopyFromDataToModel(rd);
            //    output.Add(newR);
            //}
            //return output;
            return result;
        }

        public ReviewData Update(ReviewData review)
        {
            //Guid newId = Guid.GenerateNewId();
            //review.lastModified = DateTime.UtcNow;
            //ReviewData newR = this.CopyFromModelToData(review);

            
            _reviewRepository.Collection.Save(review);
            _rr.UpdateStatistics(review.parentReviewableId);
            return Get(review.id);
        }

        //private Review CopyFromDataToModel(ReviewData r)
        //{
        //    Review newR = new Review();

        //    newR.id = r.id;
        //    newR.outsideCode = r.outsideCode;
        //    newR.parentReviewableId = r.parentReviewableId;
        //    newR.description = r.description;
        //    newR.description2 = r.description2;
        //    newR.isStaffReviewed = r.isStaffReviewed;
        //    newR.issuerCountry = r.issuerCountry;
        //    newR.issuerCountryCode = r.issuerCountryCode;
        //    newR.lastModified = r.lastModified;
        //    newR.createdDate = r.createdDate;
        //    newR.isStaffReviewed = r.isStaffReviewed;
        //    newR.name = r.name;
        //    newR.numReviews = r.numReviews;
        //    newR.compositRating = r.compositRating;
        //    newR.parentName = r.parentName;
        //    newR.reviewableType = r.reviewableType;
        //    newR.outsideCodeType = r.outsideCodeType;
        //    newR.user = r.user;
        //    newR.userName = r.userName;
        //    newR.text = r.text;
        //    newR.fBFeedPostId = r.fBFeedPostId;
        //    newR.fBTimelinePostId = r.fBTimelinePostId;
        //    newR.comments = r.comments;
        //    newR.rating = r.rating;
        //    newR.providedByBizId = r.providedByBizId;
        //    newR.providedByBizName = r.providedByBizName;
        //    newR.likes = r.likes;


        //    var query = Query.EQ("_id", r.parentReviewableId.ToString());
        //    var reviewable = _reviewableRepository.Collection.FindOneAs<ReviewableData>(query);

        //    var query2 = Query.In("_id", new BsonArray(reviewable.images));
                
        //    List<Image> images = _imagesRepository.Collection.Find(query2).ToList();
            

        //    if(images != null && images.Count > 0)
        //    {
        //        newR.images = images;
        //    }
        //    else
        //    {
        //        newR.images = new List<Image>();
        //    }
        //    return newR;

        //}

        //private ReviewData CopyFromModelToData(Review r)
        //{
        //    ReviewData newRD = new ReviewData();
        //    newRD.id = r.id;
        //    newRD.outsideCode = r.outsideCode;
        //    newRD.parentReviewableId = r.parentReviewableId;
        //    newRD.description = r.description;
        //    newRD.description2 = r.description2;
        //    newRD.isStaffReviewed = r.isStaffReviewed;
        //    newRD.issuerCountry = r.issuerCountry;
        //    newRD.issuerCountryCode = r.issuerCountryCode;
        //    newRD.lastModified = r.lastModified;
        //    newRD.createdDate = r.createdDate;
        //    newRD.isStaffReviewed = r.isStaffReviewed;
        //    newRD.name = r.name;
        //    newRD.numReviews = r.numReviews;
        //    newRD.compositRating = r.compositRating;
        //    newRD.parentName = r.parentName;
        //    newRD.reviewableType = r.reviewableType;
        //    newRD.outsideCodeType = r.outsideCodeType;
        //    newRD.user = r.user;
        //    newRD.userName = r.userName;
        //    newRD.text = r.text;
        //    newRD.fBFeedPostId = r.fBFeedPostId;
        //    newRD.fBTimelinePostId = r.fBTimelinePostId;
        //    newRD.comments = r.comments;
        //    newRD.rating = r.rating;
        //    newRD.providedByBizId = r.providedByBizId;
        //    newRD.providedByBizName = r.providedByBizName;
        //    newRD.likes = r.likes;

        //    return newRD;

        //}

        //private Review CopyFromReviewableToNew(Review input)
        //{
        //    ReviewableRepository rr = new ReviewableRepository();
        //    Reviewable r = rr.Get(input.parentReviewableId, input.user);


        //    input.outsideCode = r.outsideCode;
            
        //    input.description = r.description;
        //    input.description2 = r.description2;
        //    input.isStaffReviewed = r.isStaffReviewed;
        //    input.issuerCountry = r.issuerCountry;
        //    input.issuerCountryCode = r.issuerCountryCode;
        //    input.isStaffReviewed = r.isStaffReviewed;
        //    input.name = r.name;
        //    input.numReviews = 0;
        //    input.compositRating = 0;
        //    input.parentName = r.name;
        //    input.reviewableType = r.reviewableType;
        //    input.outsideCodeType = r.outsideCodeType;
        //    input.providedByBizId = r.parentReviewableId;
        //    input.providedByBizName = r.parentName;
        //    return input;
        //}
        

    }
}