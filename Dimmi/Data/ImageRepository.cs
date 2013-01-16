using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Dimmi.Models.Domain;
using Dimmi.DataInterfaces;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Dimmi.Data
{
    public class ImageRepository : IImageRepository
    {
        private readonly DBRepository.MongoRepository<ImageData> _imageRepository;
        private const string TypeDiscriminatorField = "_t";

        public ImageRepository()
        {

            _imageRepository = new DBRepository.MongoRepository<ImageData>("Images");
        }

        public ImageData Get(Guid id)
        {
            var query = Query.EQ("_id", id.ToString());
            ImageData idata = _imageRepository.Collection.FindOne(query);
            return idata;
        }

        public IEnumerable<ImageData> Get(string[] images)
        {
            var query = Query.In("_id", new BsonArray(images));
            List<ImageData> idata = _imageRepository.Collection.Find(query).ToList();
            return idata;
        }

        public ImageData Add(ImageData image)
        {
            
            Guid newId = Guid.NewGuid(); //.GenerateNewId();
            image.id = newId;
            _imageRepository.Collection.Save(image);
            return Get(newId);
        }

        public IEnumerable<ImageData> GetTopForEachCategory()
        {

            var operations = new BsonDocument[]
            {
                //new BsonDocument("$match", new BsonDocument("words", new BsonDocument("$all", new BsonArray { "XXS", "SMT" }))),
                //new BsonDocument("$project", new BsonDocument { { "_id", 1 }, { "words", 1 } }),
                //new BsonDocument("$unwind", "$words"),
                //new BsonDocument("$group", new BsonDocument { { "_id", new BsonDocument("tags", "$words") }, { "count", new BsonDocument("$sum", 1) } }),
                //new BsonDocument("$sort", new BsonDocument("count", 1)),
                //new BsonDocument("$limit", 5)
                //new BsonDocument("$project", new BsonDocument { { "_id", 1 }, { "type", 1 }, { "dateCreated", 1 } }),
                //new BsonDocument("$group", new BsonDocument { { "type", "$type" } }),
                new BsonDocument("$sort", new BsonDocument { { "type", -1 }, { "dateCreated", -1 } }),
                new BsonDocument("$group", new BsonDocument { { "_id", new BsonDocument("type", "$type") }, { "iid", new BsonDocument("$first", "$_id") }, { "dateCreated", new BsonDocument("$first", "$dateCreated") }, { "description", new BsonDocument("$first", "$description") }, { "category", new BsonDocument("$first", "$category") }}),
                //new BsonDocument("$project", new BsonDocument { { "id", 1 }, { "type", 1 }, { "dateCreated", 1 } }),
                //new BsonDocument("$sort", new BsonDocument("dateCreated", -1)),
                //new BsonDocument("$limit", 5)
            };

            

            var results =_imageRepository.Collection.Aggregate(operations);
            List<string> ids = new List<string>();

            foreach (BsonDocument doc in results.ResultDocuments)
            {
                BsonValue val; // = new BsonObjectId(;
                doc.TryGetValue("iid", out val);
                //Guid o = Guid.Parse(val.AsString);
                ids.Add(val.AsString);
            }

            var query = Query.In("_id", new BsonArray(ids.ToArray()));
            List<ImageData> output = _imageRepository.Collection.Find(query).ToList();

            return output;

            //var matchingExamples = results.ResultDocuments.Select(BsonSerializer.Deserialize<Image>).ToList();

            //return matchingExamples;
        }

        //public static Image CopyFromDataToModel(ImageData id)
        //{
        //    Image i = new Image();
        //    i.category = id.category;
        //    i.data = id.data;
        //    i.dateCreated = id.dateCreated;
        //    i.description = id.description;
        //    i.fileType = id.fileType;
        //    i.id = id.id.ToString();

        //    return i;

        //}

        //public static ImageData CopyFromModelToData(Image i)
        //{
        //    ImageData id = new ImageData();
        //    id.category = i.category;
        //    id.data = i.data;
        //    id.dateCreated = i.dateCreated;
        //    id.description = i.description;
        //    id.fileType = i.fileType;
        //    id.id = Guid.Parse(i.id);
        //    return id;

        //}
    }
}