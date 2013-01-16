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
using Dimmi.Models.Domain;
using Dimmi.Controllers;
using System.Collections;


namespace Dimmi.Data
{
    

    public class ReviewableRepository : IReviewableRepository
    {
        private readonly DBRepository.MongoRepository<ReviewableData> _reviewableRepository;
        private readonly DBRepository.MongoRepository<ImageData> _imagesRepository;
        private readonly DBRepository.MongoRepository<ReviewData> _reviewsRepository;
        private const string TypeDiscriminatorField = "_t";

        public ReviewableRepository()
        {
            _reviewableRepository = new DBRepository.MongoRepository<ReviewableData>("Reviewables");
            _imagesRepository = new DBRepository.MongoRepository<ImageData>("Images");
            _reviewsRepository = new DBRepository.MongoRepository<ReviewData>("Reviews");
        }

        public IEnumerable<ReviewableData> Get()
        {

            IEnumerable<ReviewableData> reviewables = _reviewableRepository.Collection.FindAll().SetSortOrder(SortBy.Descending("createdDate")).ToList();
            return reviewables;
        }

        public IEnumerable<ReviewableData> GetByType(string type)
        {
            var query = Query.EQ("reviewableType", type);
            IEnumerable<ReviewableData> reviewables = _reviewableRepository.Collection.FindAs<ReviewableData>(query).ToList();
            return reviewables;
        }

        public IEnumerable<ReviewableData> Get(string[] reviewables)
        {
            var query = Query.In("_id", new BsonArray(reviewables));
            List<ReviewableData> rdata = _reviewableRepository.Collection.Find(query).ToList();
            return rdata;
        }

        public IEnumerable<ReviewableData> GetByName(string name, Guid userId)
        {
            Hashtable results = Search.IndexManager.searchReviewables(name);
            string[] ids = new string[results.Count];
            results.Keys.CopyTo(ids, 0);
            var query = Query.In("_id", new BsonArray(ids));
            List<ReviewableData> rdata = _reviewableRepository.Collection.Find(query).ToList();
            //rdata.Sort(

            //var keywordRegEx = BsonRegularExpression.Create(name,"-i");
            //var query = Query.Or(Query.Matches("name", keywordRegEx),
            //    Query.Matches("description", keywordRegEx),
            //    Query.Matches("parentName", keywordRegEx));

            var result = _reviewableRepository.Collection.FindAs<ReviewableData>(query).ToList();

            return result;
        }

        public IEnumerable<ReviewableData> GetByNameByType(string name, string type, Guid userId)
        {
            Hashtable results = Search.IndexManager.searchReviewablesByReviewablesType(name, type);
            string[] ids = new string[results.Count];
            results.Keys.CopyTo(ids, 0);

            var query = Query.In("_id", new BsonArray(ids));
            List<ReviewableData> rdata = _reviewableRepository.Collection.Find(query).ToList();
            List<ReviewableData> result = new List<ReviewableData>();
            foreach (ReviewableData rd in rdata)
            {
                for(int i=0; i<= ids.Length-1;i++)
                {
                    if(ids[i].Equals(rd.id.ToString()))
                    {
                        rd.searchScore = (float)results[rd.id.ToString()];
                        //result.Add(rd);
                        break;
                    }
                }
            }
            rdata.Sort((a, b) => b.searchScore.CompareTo(a.searchScore));
            return rdata;
            //rdata.Sort(delegate(ReviewableData p1, ReviewableData p2) { return p1.searchScore.CompareTo(p2.searchScore); });

            //var sorted = from ReviewableData in rdata orderby ReviewableData.searchScore descending select ReviewableData;

            //return sorted.ToList();
            //var result = _reviewableRepository.Collection.FindAs<ReviewableData>(query).ToList();

            //return result;
        }


        //public IEnumerable<ReviewableData> GetByNameByType(string name, string type, Guid userId)
        //{
        //    var keywordRegEx = BsonRegularExpression.Create(name, "-i");
        //    var query = Query.And(Query.Or(Query.Matches("name", keywordRegEx),
        //        Query.Matches("description", keywordRegEx),
        //        Query.Matches("parentName", keywordRegEx)), Query.EQ("reviewableType", type.Trim().ToLower()));

        //    var result = _reviewableRepository.Collection.FindAs<ReviewableData>(query).ToList();

        //    return result;
        //}

        public ReviewableData Get(Guid id, Guid userId)
        {
            var query = Query.EQ("_id", id.ToString());

            var result = _reviewableRepository.Collection.FindOneAs<ReviewableData>(query);
            //Reviewable newR = CopyFromDataToModel(result);
            
            //newR = GetComputedValues(newR, userId);
            //return newR;
            return result;
        }


        public ReviewableData Add(ReviewableData reviewable, Guid userId)
        {
           
            Guid newId = Guid.NewGuid();
            reviewable.id = newId;
            //newRD.id = newId;
            _reviewableRepository.Collection.Insert(reviewable);
            
            ReviewableData output = Get(newId, userId);
            Search.IndexManager.threadproc_update(output);
            return output;
        }

        public ReviewableData Update(ReviewableData reviewable, Guid userId, bool updateSearchIndex)
        {
            

            //newRd.images = imgIds.ToArray();
            _reviewableRepository.Collection.Save(reviewable);
            ReviewableData output = Get(reviewable.id, userId);
            if(updateSearchIndex)
                Search.IndexManager.threadproc_update(output);
            return output;
        }

        public ImageData AddImage(ImageData image, Guid productId)
        {
            var query = Query.EQ("_id", productId.ToString());
            ReviewableData toUpdate = _reviewableRepository.Collection.FindOne(query);

            List<string> imageIds = toUpdate.images.ToList();
            //imageIds.Add(image.id.ToString());

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

        public void UpdateStatistics(Guid reviewableId)
        {
            //return;
            var operations = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("parentReviewableId", reviewableId.ToString() )),
                new BsonDocument("$group", new BsonDocument { { "_id", new BsonDocument("product", "$parentName") }, { "parentId", new BsonDocument("$first", "$parentReviewableId") }, { "numReviews", new BsonDocument("$sum", 1) }, { "composite", new BsonDocument("$avg", "$rating") }}),

            };

            DBRepository.MongoRepository<ReviewData> _reviewsRepository = new DBRepository.MongoRepository<ReviewData>("Reviews");
            DBRepository.MongoRepository<ReviewableData> _reviewablesRepository = new DBRepository.MongoRepository<ReviewableData>("Reviewables");

            var results = _reviewsRepository.Collection.Aggregate(operations);
            if (results.ResultDocuments.Count() != 0)
            {
                BsonDocument doc = results.ResultDocuments.Take<BsonDocument>(1).First();


                BsonValue val;
                doc.TryGetValue("parentId", out val);
                Guid o = Guid.Parse(val.AsString);
                ReviewableData r = Get(o, Guid.Empty);
                doc.TryGetValue("numReviews", out val);
                r.numReviews = val.AsInt32;
                doc.TryGetValue("composite", out val);
                r.compositRating = val.AsDouble;
                Update(r, Guid.Empty, false);

            }
        }


    }
}