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
        DBRepository.MongoRepository<UserStatisticData> _allTimeStatsRepository;
        DBRepository.MongoRepository<UserStatisticData> _rollingStatsRepository;
        DBRepository.MongoRepository<UserStatisticData> _monthlyStatsRepository;

        public ReviewStatisticsRepository()
        {
            _reviewsRepository = new DBRepository.MongoRepository<ReviewData>("Reviews");
            _allTimeStatsRepository = new DBRepository.MongoRepository<UserStatisticData>("AllTimeStats");
            _rollingStatsRepository = new DBRepository.MongoRepository<UserStatisticData>("Rolling30Stats");
            _monthlyStatsRepository = new DBRepository.MongoRepository<UserStatisticData>("MonthlyStats");
        }

        public UserStatisticData GetAllTimeForUser(Guid userId)
        {


                var query = Query.EQ("userId", userId.ToString());
                return _allTimeStatsRepository.Collection.FindOneAs<UserStatisticData>(query);

        }

        public MonthlyUserStatisticData GetMonthlyForUser(Guid userId, int month, int year)
        {
            var query = Query.And(Query.EQ("userId", userId.ToString()), Query.EQ("month", month), Query.EQ("year", year));
            return _monthlyStatsRepository.Collection.FindOneAs<MonthlyUserStatisticData>(query);
            
        }

        public List<MonthlyUserStatisticData> GetMonthlyForUser(Guid userId)
        {
            var query = Query.EQ("userId", userId.ToString());
            return _monthlyStatsRepository.Collection.FindAs<MonthlyUserStatisticData>(query).SetSortOrder(SortBy.Descending("firstDayOfMonth")).ToList();

        }

        public List<MonthlyUserStatisticData> GetPageFromMonthlyStats(int pageNumber, int pageSize, int month, int year)
        {
            var filter = new BaseFilter() { CurrentPage = pageNumber, ItemsPerPage = pageSize };
            List<MonthlyUserStatisticData> items = GetItemsByFilterCM(filter, SortBy.Descending("score"), month, year);

            //DateTime previousPeriod = new DateTime(year, month, 1).AddDays(-1);



            return items;
        }


        public List<UserStatisticData> GetPageFromAllTimeStats(int pageNumber, int pageSize)
        {
            var filter = new BaseFilter() { CurrentPage = pageNumber, ItemsPerPage = pageSize };
            List<UserStatisticData> items = GetItemsByFilterAT(filter, SortBy.Descending("score"));
            return items;
        }

        public List<UserStatisticData> GetCurrentTop(int count)
        {
            var resultItems = new List<UserStatisticData>();
            var cursor = _rollingStatsRepository.Collection.FindAllAs<UserStatisticData>();
            cursor.SetSortOrder(SortBy.Descending("score"));
            cursor.SetLimit(count);
            resultItems.AddRange(cursor);
            return resultItems;
        }
        
        public void Recalculate()
        {
            RecalculateAll();
            Recalculate30();
            _monthlyStatsRepository.Collection.Drop();
            RecalculateMonth(12, 2012);
            RecalculateMonth(11, 2012);


            //ReviewData topDate = (ReviewData)_reviewsRepository.Collection.FindAll().SetSortOrder(SortBy.Descending("createdDate")).Take<ReviewData>(1);

            //int year = topDate.createdDate.Year;
            //int month = topDate.createdDate.Month;

            for (int i = 1; i <= 12; i++) //complete hack for 2013 (no chron job ability on server)
            {
                RecalculateMonth(i, 2013);
            }
            RecalculateMVP();
            
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

            List<UserStatisticData> stats = new List<UserStatisticData>();
            //drop the table
            _rollingStatsRepository.Collection.Drop();


            foreach (BsonDocument doc in results.ResultDocuments)
            {
                UserStatisticData stat = new UserStatisticData();

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
            //now calculate the rankings
            int rankCounter = 1;
            for (int i = 0; i < stats.Count(); i++)
            {
                UserStatisticData stat = stats.ElementAt(i);
                if (i > 0)
                {

                    UserStatisticData prvstat = stats.ElementAt(i - 1);
                    if (prvstat.score > stat.score)
                    {
                        rankCounter++;
                    }
                    stat.rank = rankCounter;
                }
                else
                {
                    stat.rank = rankCounter;
                }
            }
            _rollingStatsRepository.Collection.InsertBatch(stats);

        }

        private void RecalculateMonth( int month, int year)
        {
            DateTime firstDayOftheMonth = new DateTime(year, month, 1, 0,0,1, DateTimeKind.Utc);
            DateTime lastDayOftheMonth = new DateTime(year, month, 1, 23, 59, 59, DateTimeKind.Utc).AddMonths(1).AddDays(-1);
            

            var operations = new BsonDocument[]
            {
                new BsonDocument 
                            {
                                    {
                                        "$match", new BsonDocument
                                                    {
                                                        {"createdDate", new BsonDocument { {"$gte", firstDayOftheMonth}, {"$lte", lastDayOftheMonth} }}
                                                    }

                                    }
                            },
               new BsonDocument
                            {
                                    {
                                        "$group", new BsonDocument
                                                    {
                                                        { "_id", new BsonDocument
                                                                {
                                                                    {"user", "$user"}
                                                                }
                                                        },
                                                        { "userName", new BsonDocument("$first", "$userName") }, 
                                                        { "numReviews", new BsonDocument("$sum", 1) }, 
                                                        { "numLikes", new BsonDocument("$sum", 1) },
                                                        { "Year", new BsonDocument("$first", new BsonDocument (  "$year", "$createdDate" ) )},
                                                        { "Month", new BsonDocument("$first", new BsonDocument (  "$month", "$createdDate" ) )},
                                                                
                                                    }
                                      }
                            }


               //new BsonDocument("$match", new BsonDocument("createdDate", new BsonDocument ( "$gte", firstDayOftheMonth ))),
               //new BsonDocument("$group", new BsonDocument { { "_id", new BsonDocument("user", "$user") }, { "userName", new BsonDocument("$first", "$userName") }, { "numReviews", new BsonDocument("$sum", 1) }, { "numLikes", new BsonDocument("$sum", 1) }}),
            };

            var operations2 = new BsonDocument[]
            {
                //new BsonDocument("$match", new BsonDocument("createdDate", new BsonDocument ( "$gte", firstDayOftheMonth ))),

                new BsonDocument("$unwind", "$likes"),
                                new BsonDocument 
                            {
                                    {
                                        "$match", new BsonDocument
                                                    {
                                                        {"likes.when", new BsonDocument { {"$gte", firstDayOftheMonth}, {"$lte", lastDayOftheMonth} }}
                                                    }

                                    }
                            },
                new BsonDocument("$group", new BsonDocument { { "_id", new BsonDocument("user", "$user") }, 
                                                        { "userName", new BsonDocument("$first", "$userName") }, 
                                                        { "numReviews", new BsonDocument("$sum", 1) }, 
                                                        { "numLikes", new BsonDocument("$sum", 1) },
                                                        { "Year", new BsonDocument("$first", new BsonDocument (  "$year", "$likes.when" ) )},
                                                        { "Month", new BsonDocument("$first", new BsonDocument (  "$month", "$likes.when" ) )},
                }),
            };


            var results = _reviewsRepository.Collection.Aggregate(operations);
            var results2 = _reviewsRepository.Collection.Aggregate(operations2);

            List<MonthlyUserStatisticData> stats = new List<MonthlyUserStatisticData>();
            //drop the table
            //_monthlyStatsRepository.Collection.Drop();
            
            //for(int i=0; i<= results.ResultDocuments.Count()-1; i++)
            //{
            //    BsonDocument doc = results.ResultDocuments.ElementAt(i);
            foreach (BsonDocument doc in results.ResultDocuments)
            {
                MonthlyUserStatisticData stat = new MonthlyUserStatisticData();

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

                BsonValue valreviewmonth; // = new BsonObjectId(;
                doc.TryGetValue("Month", out valreviewmonth);
                int mon = valreviewmonth.AsInt32;
                stat.month = mon;
                switch (mon)
                {
                    case 1:
                        stat.monthName = "Jan";
                        break;
                    case 2:
                        stat.monthName = "Feb";
                        break;
                    case 3:
                        stat.monthName = "Mar";
                        break;
                    case 4:
                        stat.monthName = "April";
                        break;
                    case 5:
                        stat.monthName = "May";
                        break;
                    case 6:
                        stat.monthName = "June";
                        break;
                    case 7:
                        stat.monthName = "July";
                        break;
                    case 8:
                        stat.monthName = "Aug";
                        break;
                    case 9:
                        stat.monthName = "Sept";
                        break;
                    case 10:
                        stat.monthName = "Oct";
                        break;
                    case 11:
                        stat.monthName = "Nov";
                        break;
                    case 12:
                        stat.monthName = "Dec";
                        break;
                }
                

                BsonValue valreviewyear; // = new BsonObjectId(;
                doc.TryGetValue("Year", out valreviewyear);
                stat.year = valreviewyear.AsInt32;

                
                stat.lastDayOfMonth = lastDayOftheMonth;
                stat.firstDayOfMonth = firstDayOftheMonth;
                //stat.lastDayOfMonth = d;

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
            //now sort them...
            stats.Sort((b, a) => a.score.CompareTo(b.score));

            //now calculate the rankings
            int rankCounter = 1;
            for(int i=0; i<stats.Count(); i++)
            {
                MonthlyUserStatisticData stat = stats.ElementAt(i);
                if (i > 0)
                {
                    
                    MonthlyUserStatisticData prvstat = stats.ElementAt(i - 1);
                    if (prvstat.score > stat.score)
                    {
                        rankCounter++;
                    }
                    stat.rank = rankCounter;
                }
                else
                {
                    stat.rank = rankCounter;
                }
            }
            _monthlyStatsRepository.Collection.InsertBatch(stats);
            
            //string[] indexes = new string[] {"userId", "month", "year"};
            //IndexKeysBuilder ikb = new IndexKeysBuilder();

            //IndexKeys i1 = ikb.Ascending(new string[] { "userId" });
            //_monthlyStatsRepository.Collection.EnsureIndex();

        }

        private void RecalculateMVP()
        {
            int mvp1 = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MVPTier1"]);
            int mvp2 = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MVPTier2"]);
            int mvp3 = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MVPTier3"]);
            var operations = new BsonDocument[]
            {
                new BsonDocument 
                            {
                                    {
                                        "$match", new BsonDocument
                                                    {
                                                        {"rank", new BsonDocument { {"$lte", mvp1} }},
                                                        {"lastDayOfMonth", new BsonDocument { {"$lte", DateTime.UtcNow} }}
                                                    }

                                    }
                            },
               new BsonDocument
                            {
                                    {
                                        "$group", new BsonDocument
                                                    {
                                                        { "_id", new BsonDocument
                                                                {
                                                                    {"user", "$userId"}
                                                                }
                                                        },
                                                        { "userName", new BsonDocument("$first", "$userName") }, 
                                                        { "mvpGold", new BsonDocument("$sum", 1) }, 
                                                        
                                                                
                                                    }
                                      }
                            }

  
            };

            var operations2 = new BsonDocument[]
            {
                new BsonDocument 
                            {
                                    {
                                        "$match", new BsonDocument
                                                    {
                                                        {"rank", new BsonDocument { {"$gt", mvp1}, {"$lte", mvp2} }},
                                                        {"lastDayOfMonth", new BsonDocument { {"$lte", DateTime.UtcNow} }}
                                                    }

                                    }
                            },
               new BsonDocument
                            {
                                    {
                                        "$group", new BsonDocument
                                                    {
                                                        { "_id", new BsonDocument
                                                                {
                                                                    {"user", "$userId"}
                                                                }
                                                        },
                                                        { "userName", new BsonDocument("$first", "$userName") }, 
                                                        { "mvpSilver", new BsonDocument("$sum", 1) }, 
                                                        
                                                                
                                                    }
                                      }
                            }

  
            };

            var operations3 = new BsonDocument[]
            {
                new BsonDocument 
                            {
                                    {
                                        "$match", new BsonDocument
                                                    {
                                                        {"rank", new BsonDocument { {"$gt", mvp2}, {"$lte", mvp3} }},
                                                        {"lastDayOfMonth", new BsonDocument { {"$lte", DateTime.UtcNow} }}
                                                    }

                                    }
                            },
               new BsonDocument
                            {
                                    {
                                        "$group", new BsonDocument
                                                    {
                                                        { "_id", new BsonDocument
                                                                {
                                                                    {"user", "$userId"}
                                                                }
                                                        },
                                                        { "userName", new BsonDocument("$first", "$userName") }, 
                                                        { "mvpBronze", new BsonDocument("$sum", 1) }, 
                                                        
                                                                
                                                    }
                                      }
                            }

  
            };


            var results = _monthlyStatsRepository.Collection.Aggregate(operations);
            var results2 = _monthlyStatsRepository.Collection.Aggregate(operations2);
            var results3 = _monthlyStatsRepository.Collection.Aggregate(operations3);

            foreach (BsonDocument doc in results.ResultDocuments)
            {
                 BsonValue root; // = new BsonObjectId(;
                doc.TryGetValue("_id", out root);
                BsonDocument valreviewuser = root.AsBsonDocument;

                var query = Query.EQ("userId", valreviewuser.GetValue("user").AsString);
                UserStatisticData userStat = _allTimeStatsRepository.Collection.FindOneAs<UserStatisticData>(query);

                BsonValue valuserGold; // = new BsonObjectId(;
                doc.TryGetValue("mvpGold", out valuserGold);
                userStat.mvp1Count = valuserGold.AsInt32;

                _allTimeStatsRepository.Collection.Save(userStat);

            }

            foreach (BsonDocument doc in results2.ResultDocuments)
            {
                BsonValue root; // = new BsonObjectId(;
                doc.TryGetValue("_id", out root);
                BsonDocument valreviewuser = root.AsBsonDocument;

                var query = Query.EQ("userId", valreviewuser.GetValue("user").AsString);
                UserStatisticData userStat = _allTimeStatsRepository.Collection.FindOneAs<UserStatisticData>(query);

                BsonValue valuserSilver; // = new BsonObjectId(;
                doc.TryGetValue("mvpSilver", out valuserSilver);
                userStat.mvp2Count = valuserSilver.AsInt32;

                _allTimeStatsRepository.Collection.Save(userStat);

            }

            foreach (BsonDocument doc in results3.ResultDocuments)
            {
                BsonValue root; // = new BsonObjectId(;
                doc.TryGetValue("_id", out root);
                BsonDocument valreviewuser = root.AsBsonDocument;

                var query = Query.EQ("userId", valreviewuser.GetValue("user").AsString);
                UserStatisticData userStat = _allTimeStatsRepository.Collection.FindOneAs<UserStatisticData>(query);

                BsonValue valuserBronze; // = new BsonObjectId(;
                doc.TryGetValue("mvpBronze", out valuserBronze);
                userStat.mvp2Count = valuserBronze.AsInt32;

                _allTimeStatsRepository.Collection.Save(userStat);

            }
            

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


            List<UserStatisticData> stats = new List<UserStatisticData>();
            //drop the table
            _allTimeStatsRepository.Collection.Drop();


            foreach (BsonDocument doc in results.ResultDocuments)
            {
                UserStatisticData stat = new UserStatisticData();

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

            //now calculate the rankings
            int rankCounter = 1;
            for (int i = 0; i < stats.Count(); i++)
            {
                UserStatisticData stat = stats.ElementAt(i);
                if (i > 0)
                {

                    UserStatisticData prvstat = stats.ElementAt(i - 1);
                    if (prvstat.score > stat.score)
                    {
                        rankCounter++;
                    }
                    stat.rank = rankCounter;
                }
                else
                {
                    stat.rank = rankCounter;
                }
            }
            _allTimeStatsRepository.Collection.InsertBatch(stats);

            //string[] indexes = new string[] { "userId" };

            //_allTimeStatsRepository.Collection.EnsureIndex(indexes);

        }

        private List<MonthlyUserStatisticData> GetItemsByFilterCM(BaseFilter filter, SortByBuilder sort, int month, int year)
        {
            var resultItems = new List<MonthlyUserStatisticData>();
            var query = Query.And(Query.EQ("month", month), Query.EQ("year", year));
            var cursor = _monthlyStatsRepository.Collection.FindAs<MonthlyUserStatisticData>(query);


            cursor.SetSortOrder(sort);
            if (filter.IsNeedPaging)
            {
                cursor.SetSkip(filter.Skip).SetLimit(filter.Take);
                filter.TotalCount = (int)cursor.Count();
            }

            resultItems.AddRange(cursor);

            return resultItems;
        }

        private List<UserStatisticData> GetItemsByFilterAT(BaseFilter filter, SortByBuilder sort)
        {
            var resultItems = new List<UserStatisticData>();
            var cursor = _allTimeStatsRepository.Collection.FindAllAs<UserStatisticData>();
           

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