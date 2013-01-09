using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
//using Dimmi.Models;
using Dimmi.DataInterfaces;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Dimmi.Models;


namespace Dimmi.Data
{
    

    public class ReviewableRepository : IReviewableRepository
    {
        private readonly DBRepository.MongoRepository<ReviewableData> _reviewableRepository;
        private readonly DBRepository.MongoRepository<Image> _imagesRepository;
        private readonly DBRepository.MongoRepository<ReviewData> _reviewsRepository;
        //private readonly DBRepository.MongoRepository<DataInterfaces.Business> _businessRepository;
        private const string TypeDiscriminatorField = "_t";

        public ReviewableRepository()
        {
            _reviewableRepository = new DBRepository.MongoRepository<ReviewableData>("Reviewables");
            _imagesRepository = new DBRepository.MongoRepository<Image>("Images");
            _reviewsRepository = new DBRepository.MongoRepository<ReviewData>("Reviews");
           // _businessRepository = new DBRepository.MongoRepository<DataInterfaces.Business>("Businesses");
        }

        public IEnumerable<Reviewable> Get()
        {
            IEnumerable<ReviewableData> reviewables = _reviewableRepository.Collection.FindAll().SetSortOrder(SortBy.Descending("createdDate")).Take(100).ToList();
            List<Reviewable> results = new List<Reviewable>();
            foreach (ReviewableData item in reviewables)
            {
                Reviewable rItem = this.CopyFromDataToModel(item);
                results.Add(rItem);
            }
            return results;
        }

        public IEnumerable<Reviewable> GetByName(string name, Guid userId)
        {
            var keywordRegEx = BsonRegularExpression.Create(name,"-i");
            var query = Query.Or(Query.Matches("name", keywordRegEx),
                Query.Matches("description", keywordRegEx),
                Query.Matches("parentName", keywordRegEx));

            var result = _reviewableRepository.Collection.FindAs<ReviewableData>(query).ToList();

            List<Reviewable> returnVal = new List<Reviewable>();
            foreach (ReviewableData rd in result)
            {
                Reviewable newR = CopyFromDataToModel(rd);
                
                newR = GetComputedValues(newR, userId);
                returnVal.Add(newR);
            }
            
            return returnVal;
        }

        public IEnumerable<Reviewable> GetByNameByType(string name, string type, Guid userId)
        {
            var keywordRegEx = BsonRegularExpression.Create(name, "-i");
            var query = Query.And(Query.Or(Query.Matches("name", keywordRegEx),
                Query.Matches("description", keywordRegEx),
                Query.Matches("parentName", keywordRegEx)), Query.EQ("reviewableType", type.Trim().ToLower()));

            var result = _reviewableRepository.Collection.FindAs<ReviewableData>(query).ToList();

            List<Reviewable> returnVal = new List<Reviewable>();
            foreach (ReviewableData rd in result)
            {
                Reviewable newR = CopyFromDataToModel(rd);
                
                newR = GetComputedValues(newR, userId);
                returnVal.Add(newR);
            }

            return returnVal;
        }

        public Reviewable Get(Guid id, Guid userId)
        {
            var query = Query.EQ("_id", id.ToString());

            var result = _reviewableRepository.Collection.FindOneAs<ReviewableData>(query);
            Reviewable newR = CopyFromDataToModel(result);
            
            newR = GetComputedValues(newR, userId);
            return newR;
        }


        public Reviewable Add(Reviewable reviewable, Guid userId)
        {
           
            //first save the images and store the Guid's
            List<string> ids = new List<string>();
            foreach (Image i in reviewable.images)
            {
                Guid newImgId = Guid.NewGuid();
                
                i.id = newImgId;
                _imagesRepository.Collection.Save(i);
                ids.Add(newImgId.ToString());
            }
            ReviewableData newRD = CopyFromModelToData(reviewable);
            newRD.images = ids.ToArray();
            Guid newId = Guid.NewGuid();
            newRD.id = newId;
            _reviewableRepository.Collection.Insert(newRD);

            return Get(newId, userId);
        }

        public Reviewable Update(Reviewable reviewable, Guid userId)
        {
            //Guid newId = Guid.GenerateNewId();
            ReviewableData newRd = this.CopyFromModelToData(reviewable);

            //got get the array of image ids already in the db
            var query = Query.EQ("_id", reviewable.id.ToString());
            string[] existingIds = _reviewableRepository.Collection.FindOne(query).images;

            List<string> imgIds = new List<string>();
            foreach (Image img in reviewable.images)
            {
                //ImageData id = ImageRepository.CopyFromModelToData(img);
                //does it already exist?
                if (existingIds.Contains(img.id.ToString()))
                {
                    //update it
                    _imagesRepository.Collection.Save(img);
                }
                else
                {
                    //create it
                    Guid newImgId = Guid.NewGuid();
                    img.id = newImgId;
                    _imagesRepository.Collection.Save(img);
                }
                imgIds.Add(img.id.ToString());
            }
            //now to remove any deleted images...
            foreach (string oid in existingIds)
            {
                if (!imgIds.Contains(oid))
                {
                    var query2 = Query.EQ("_id", oid.ToString());
                    _imagesRepository.Collection.Remove(query2);
                }
            }

            newRd.images = imgIds.ToArray();
            _reviewableRepository.Collection.Save(newRd);

            return Get(newRd.id, userId);
        }

        public Image AddImage(Image image, Guid productId)
        {
            var query = Query.EQ("_id", productId.ToString());
            ReviewableData toUpdate = _reviewableRepository.Collection.FindOne(query);

            List<string> imageIds = toUpdate.images.ToList();
            imageIds.Add(image.id.ToString());

            //save the image
            Guid newId = Guid.NewGuid();
            image.id = newId;
            imageIds.Add(image.id.ToString());
            _imagesRepository.Collection.Save(image);
            //save the product
            toUpdate.images = imageIds.ToArray();
            _reviewableRepository.Collection.Save(toUpdate);

            //go fetch the image
            var query2 = Query.EQ("_id", newId.ToString());
            image =_imagesRepository.Collection.FindOne(query);
            return image;

        }



        private Reviewable CopyFromDataToModel(ReviewableData r)
        {
            Reviewable newR = new Reviewable();

            newR.id = r.id;
            newR.outsideCode = r.outsideCode;
            newR.parentReviewableId = r.parentReviewableId;
            newR.description = r.description;
            newR.description2 = r.description2;
            newR.isStaffReviewed = r.isStaffReviewed;
            newR.issuerCountry = r.issuerCountry;
            newR.issuerCountryCode = r.issuerCountryCode;
            newR.lastModified = r.lastModified;
            newR.createdDate = r.createdDate;
            newR.isStaffReviewed = r.isStaffReviewed;
            newR.name = r.name;
            newR.numReviews = r.numReviews;
            newR.compositRating = r.compositRating;
            newR.parentName = r.parentName;
            newR.reviewableType = r.reviewableType;
            newR.outsideCodeType = r.outsideCodeType;

            if (r.images != null && r.images.Length > 0)
            {
                var query = Query.In("_id", new BsonArray(r.images));
                List<Image> images = _imagesRepository.Collection.Find(query).ToList();
                newR.images = images;
            }
            else
            {
                newR.images = new List<Models.Image>();
            }

            return newR;

        }

        private ReviewableData CopyFromModelToData(Reviewable r)
        {
            ReviewableData newRD = new ReviewableData();
            newRD.id = r.id;
            newRD.outsideCode = r.outsideCode;
            newRD.parentReviewableId = r.parentReviewableId;
            newRD.description = r.description;
            newRD.description2 = r.description2;
            newRD.isStaffReviewed = r.isStaffReviewed;
            newRD.issuerCountry = r.issuerCountry;
            newRD.issuerCountryCode = r.issuerCountryCode;
            newRD.lastModified = r.lastModified;
            newRD.createdDate = r.createdDate;
            newRD.isStaffReviewed = r.isStaffReviewed;
            newRD.name = r.name;
            newRD.numReviews = r.numReviews;
            newRD.compositRating = r.compositRating;
            newRD.parentName = r.parentName;
            newRD.reviewableType = r.reviewableType;
            newRD.outsideCodeType = r.outsideCodeType;
            
            List<string> imgIds = new List<string>();
            if (r.images != null && r.images.Count > 0)
            {
                imgIds.Add(r.id.ToString());
            }
            newRD.images = imgIds.ToArray();

            return newRD;

        }

        private Reviewable GetComputedValues(Reviewable reviewable, Guid userId)
        {
            if (userId.Equals(Guid.Empty))
                return reviewable;
            var query = Query.And(Query.EQ("user",userId.ToString()), Query.EQ("parentReviewableId",reviewable.id.ToString()));
            ReviewData review =_reviewsRepository.Collection.FindOneAs<ReviewData>(query);
            if (review != null)
            {
                reviewable.hasReviewed = true;
                reviewable.hasReviewedId = review.id;
            }
            else
            {
                reviewable.hasReviewed = false;
                reviewable.hasReviewedId = Guid.Empty;
            }
            return reviewable;
        }

        public void UpdateStatistics(Guid reviewableId)
        {
            //return;
            var operations = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("parentReviewableId", reviewableId.ToString() )),
                new BsonDocument("$group", new BsonDocument { { "_id", new BsonDocument("product", "$parentName") }, { "parentId", new BsonDocument("$first", "$parentReviewableId") }, { "numReviews", new BsonDocument("$sum", 1) }, { "composite", new BsonDocument("$avg", "$rating") }}),

            };

            DBRepository.MongoRepository<Models.ReviewData> _reviewsRepository = new DBRepository.MongoRepository<Models.ReviewData>("Reviews");
            DBRepository.MongoRepository<Models.ReviewableData> _reviewablesRepository = new DBRepository.MongoRepository<Models.ReviewableData>("Reviewables");

            var results = _reviewsRepository.Collection.Aggregate(operations);
            if (results.ResultDocuments.Count() != 0)
            {
                BsonDocument doc = results.ResultDocuments.Take<BsonDocument>(1).First();

                //foreach (BsonDocument doc in results.ResultDocuments)
                //{
                BsonValue val;
                doc.TryGetValue("parentId", out val);
                Guid o = Guid.Parse(val.AsString);
                Reviewable r = Get(o, Guid.Empty);
                doc.TryGetValue("numReviews", out val);
                r.numReviews = val.AsInt32;
                doc.TryGetValue("composite", out val);
                r.compositRating = val.AsDouble;
                Update(r, Guid.Empty);
                //}
            }
        }


    }
}