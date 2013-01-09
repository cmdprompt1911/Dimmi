using Dimmi.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Dimmi.Data
{
    public class DBRepository
    {
        public class MongoRepository<T> where T : BaseEntity
        {
            internal MongoDatabase Database { get; set; }

            public MongoRepository(string collectionName)
            {
                Database = MongoDatabaseHelper.GetMongoDatabase();
                Collection = Database.GetCollection<T>(collectionName);
            }

            public MongoCollection<T> Collection { get; private set; }
        }

        public static class MongoDatabaseHelper
        {
            public static MongoDatabase GetMongoDatabase()
            {
                var connectionString = ConfigurationManager.ConnectionStrings["Mongo"].ConnectionString;
                var connection = new MongoConnectionStringBuilder(connectionString);
                var server = MongoServer.Create(connection);
                return server.GetDatabase(connection.DatabaseName);
            }
        }
    }
}