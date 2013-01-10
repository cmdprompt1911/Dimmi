using Dimmi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Dimmi.Data;

namespace Dimmi
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        private static DateTime lastRan;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            RegisterSeralizationMappings();

            RecalculateStats();

           
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            var fiveminAgo = DateTime.Now.AddMinutes(-5);
            if(lastRan > fiveminAgo)
            {
                RecalculateStats();
            }
        }

        private void RecalculateStats()
        {
            ReviewStatisticsRepository rp = new ReviewStatisticsRepository();
            rp.Recalculate();
            lastRan = DateTime.Now;
        }

        private void RegisterSeralizationMappings()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(BaseEntity)))
            {
                BsonClassMap.RegisterClassMap<BaseEntity>(cm =>
                {
                    cm.AutoMap();
                    cm.GetMemberMap(c => c.id)
                      .SetRepresentation(
                          BsonType.String);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Comment)))
            {
                BsonClassMap.RegisterClassMap<Comment>(cm =>
                {
                    cm.AutoMap();
                    cm.GetMemberMap(c => c.commentBy).SetRepresentation(BsonType.String);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(Like)))
            {
                BsonClassMap.RegisterClassMap<Like>(cm =>
                {
                    cm.AutoMap();
                    cm.GetMemberMap(c => c.likedBy).SetRepresentation(BsonType.String);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(ReviewStatistic)))
            {
                BsonClassMap.RegisterClassMap<ReviewStatistic>(cm =>
                {
                    cm.AutoMap();
                    cm.GetMemberMap(c => c.userId).SetRepresentation(BsonType.String);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(ReviewBase)))
            {
                BsonClassMap.RegisterClassMap<ReviewBase>(cm =>
                {
                    cm.AutoMap();
                    cm.GetMemberMap(c => c.user).SetRepresentation(BsonType.String);
                    cm.GetMemberMap(c => c.providedByBizId).SetRepresentation(BsonType.String);

                });
            }


            if (!BsonClassMap.IsClassMapRegistered(typeof(ReviewableBase)))
            {
                BsonClassMap.RegisterClassMap<ReviewableBase>(cm =>
                {
                    cm.AutoMap();
                    cm.GetMemberMap(c => c.parentReviewableId).SetRepresentation(BsonType.String);
                    cm.GetMemberMap(c => c.hasReviewedId).SetRepresentation(BsonType.String);
                });
            }
        }
    }
}