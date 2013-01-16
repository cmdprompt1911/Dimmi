using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dimmi.DataInterfaces;
using Dimmi.Models.Domain;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Dimmi.Data
{
    public class ReviewStatisticsRepository : IReviewStatisticsRepository
    {
        DBRepository.MongoRepository<ReviewData> _reviewsRepository;
        DBRepository.MongoRepository<ReviewStatisticData> _allTimeStatsRepository;
        DBRepository.MongoRepository<ReviewStatisticData> _rollingStatsRepository;

        public ReviewStatisticsRepository()
        {
            _reviewsRepository = new DBRepository.MongoRepository<ReviewData>("Reviews");
            _allTimeStatsRepository = new DBRepository.MongoRepository<ReviewStatisticData>("AllTimeStats");
            _rollingStatsRepository = new DBRepository.MongoRepository<ReviewStatisticData>("Rolling30Stats");
        }

        public ReviewStatisticData Get(Guid userId, bool last30days)
        {
            var query = Query.EQ("userId", userId.ToString());
            if(!last30days)
                return _allTimeStatsRepository.Collection.FindOneAs<ReviewStatisticData>(query);
            else
                return _rollingStatsRepository.Collection.FindOneAs<ReviewStatisticData>(query);
        }


        public List<ReviewStatisticData> GetPageFromAllTimeStats(int pageNumber, int pageSize)
        {



            var filter = new BaseFilter() { CurrentPage = pageNumber, ItemsPerPage = pageSize };
            List<ReviewStatisticData> items = GetItemsByFilter(filter, SortBy.Descending("score"));
            return items;
        }

        public List<ReviewStatisticData> GetCurrentTop(int count)
        {
            var resultItems = new List<ReviewStatisticData>();
            var cursor = _rollingStatsRepository.Collection.FindAllAs<ReviewStatisticData>();
            cursor.SetSortOrder(SortBy.Descending("score"));
            cursor.SetLimit(count);
            resultItems.AddRange(cursor);
            return resultItems;
        }
        
        public void Recalculate()
        {
            RecalculateAll();
            Recalculate30();
            
        }


        private void Recalculate30()
        {
            var operations = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("createdDate", new BsonDocument ( "$gte", DateTime.UtcNow.AddDays(-30) ))),
                new BsonDocument("$group", new BsonDocument { { "_id", new BsonDocument("user", "$user") }, { "userName", new BsonDocument("$first", "$userName") }, { "numReviews", new BsonDocument("$sum", 1) }, { "numLikes", new BsonDocument("$sum", 1) }}),
            };

            var operations2 = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("createdDate", new BsonDocument ( "$gte", DateTime.UtcNow.AddDays(-30) ))),
                new BsonDocument("$unwind", "$likes"),
                new BsonDocument("$group", new BsonDocument { { "_id", new BsonDocument("user", "$user") }, { "userName", new BsonDocument("$first", "$userName") }, { "numReviews", new BsonDocument("$sum", 1) }, { "numLikes", new BsonDocument("$sum", 1) }}),
            };


            var results = _reviewsRepository.Collection.Aggregate(operations);
            var results2 = _reviewsRepository.Collection.Aggregate(operations2);

            List<ReviewStatisticData> stats = new List<ReviewStatisticData>();
            //drop the table
            _rollingStatsRepository.Collection.Drop();


            foreach (BsonDocument doc in results.ResultDocuments)
            {
                ReviewStatisticData stat = new ReviewStatisticData();

                BsonValue root; // = new BsonObjectId(;
                doc.TryGetValue("_id", out root);
                BsonDocument valreviewuser = root.AsBsonDocument;
                stat.userId = Guid.Parse(valreviewuser.GetValue("user").AsString);

                BsonValue valreviewusername; // = new BsonObjectId(;
                doc.TryGetValue("userName", out valreviewusername);
                stat.userName = valreviewusername.AsString;

                BsonValue valreviewnumreviews; // = new BsonObjectId(;
                doc.TryGetValue("numReviews", out valreviewnumreviews);
                stat.numReviews = valreviewnumreviews.AsInt32;

                foreach (BsonDocument doc2 in results2.ResultDocuments)
                {
                    BsonValue root2; // = new BsonObjectId(;
                    doc2.TryGetValue("_id", out root2);
                    BsonDocument valreviewuser2 = root2.AsBsonDocument;


                    if (valreviewuser.Equals(valreviewuser2))
                    {
                        BsonValue valreviewnumlikes; // = new BsonObjectId(;
                        doc2.TryGetValue("numLikes", out valreviewnumlikes);
                        stat.numLikes = valreviewnumlikes.AsInt32;
                        break;
                    }
                }
                stat.score = (stat.numReviews + stat.numLikes) * 100;
                stats.Add(stat);
            }
            stats.Sort((b, a) => a.score.CompareTo(b.score));
            _rollingStatsRepository.Collection.InsertBatch(stats);

        }

        private void RecalculateAll()
        {
            var operations = new BsonDocument[]
            {
                //new BsonDocument("$match", new BsonDocument("createdDate", new BsonDocument ( "$gte", DateTime.UtcNow.AddDays(-30) ))),
                new BsonDocument("$group", new BsonDocument { { "_id", new BsonDocument("user", "$user") }, { "userName", new BsonDocument("$first", "$userName") }, { "numReviews", new BsonDocument("$sum", 1) }, { "numLikes", new BsonDocument("$sum", 1) }}),

            };

            var operations2 = new BsonDocument[]
            {
                //new BsonDocument("$match", new BsonDocument("createdDate", new BsonDocument ( "$gte", DateTime.UtcNow.AddDays(-30) ))),
                new BsonDocument("$unwind", "$likes"),
                new BsonDocument("$group", new BsonDocument { { "_id", new BsonDocument("user", "$user") }, { "userName", new BsonDocument("$first", "$userName") }, { "numReviews", new BsonDocument("$sum", 1) }, { "numLikes", new BsonDocument("$sum", 1) }}),
            };

            



            var results = _reviewsRepository.Collection.Aggregate(operations);
            var results2 = _reviewsRepository.Collection.Aggregate(operations2);


            List<ReviewStatisticData> stats = new List<ReviewStatisticData>();
            //drop the table
            _allTimeStatsRepository.Collection.Drop();


            foreach (BsonDocument doc in results.ResultDocuments)
            {
                ReviewStatisticData stat = new ReviewStatisticData();

                BsonValue root; // = new BsonObjectId(;
                doc.TryGetValue("_id", out root);
                BsonDocument valreviewuser = root.AsBsonDocument;
                stat.userId = Guid.Parse(valreviewuser.GetValue("user").AsString);

                BsonValue valreviewusername; // = new BsonObjectId(;
                doc.TryGetValue("userName", out valreviewusername);
                stat.userName = valreviewusername.AsString;

                BsonValue valreviewnumreviews; // = new BsonObjectId(;
                doc.TryGetValue("numReviews", out valreviewnumreviews);
                stat.numReviews = valreviewnumreviews.AsInt32;

                foreach (BsonDocument doc2 in results2.ResultDocuments)
                {
                    BsonValue root2; // = new BsonObjectId(;
                    doc2.TryGetValue("_id", out root2);
                    BsonDocument valreviewuser2 = root2.AsBsonDocument;


                    if (valreviewuser.Equals(valreviewuser2))
                    {
                        BsonValue valreviewnumlikes; // = new BsonObjectId(;
                        doc2.TryGetValue("numLikes", out valreviewnumlikes);
                        stat.numLikes = valreviewnumlikes.AsInt32;
                        break;
                    }
                }
                stat.score = (stat.numReviews + stat.numLikes) * 100;
                stats.Add(stat);
            }

            stats.Sort((b, a) => a.score.CompareTo(b.score));
            _allTimeStatsRepository.Collection.InsertBatch(stats);
        }

        private List<ReviewStatisticData> GetItemsByFilter(BaseFilter filter, SortByBuilder sort)
        {
            var resultItems = new List<ReviewStatisticData>();
            var cursor = _allTimeStatsRepository.Collection.FindAllAs<ReviewStatisticData>();
           

            cursor.SetSortOrder(sort);
            if (filter.IsNeedPaging)
            {
                cursor.SetSkip(filter.Skip).SetLimit(filter.Take);
                filter.TotalCount = (int)cursor.Count();
            }

            resultItems.AddRange(cursor);

            return resultItems;
        }


        public class BaseFilter
        {
            private int _itemsPerPage = 10;
            private int _skip = 0;
            private int _currentPage = 1;

            public BaseFilter()
            {
                IsNeedPaging = true;
            }

            public int Skip
            {
                get
                {
                    if (_skip == 0)
                        _skip = (CurrentPage - 1) * _itemsPerPage;
                    return _skip;
                }
                set
                {
                    _skip = value;
                }
            }

            public int Take
            {
                get
                {
                    return _itemsPerPage;
                }
                set
                {
                    _itemsPerPage = value;
                }
            }

            public bool IsNeedPaging { get; set; }

            public int TotalCount { get; set; }

            public int CurrentPage
            {
                get
                {
                    return _currentPage;
                }
                set
                {
                    _currentPage = value;
                }
            }

            public int ItemsPerPage
            {
                get
                {
                    return _itemsPerPage;
                }
                set
                {
                    _itemsPerPage = value;
                }
            }

            public int TotalPagesCount
            {
                get
                {
                    return TotalCount / ItemsPerPage +
                                     ((TotalCount % ItemsPerPage > 0) ? 1 : 0);
                }
            }
        }
    }
}