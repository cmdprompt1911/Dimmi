using Dimmi.Models.Domain;
using Dimmi.Models.UI;
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
using AutoMapper;
using Dimmi.Search;

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
            SetUpAutoMapperMappings();
            RecalculateStats();

            IndexManager.build_lucene_index(this.Application);
               
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            var fiveminAgo = DateTime.Now.AddMinutes(-5);
            if(lastRan > fiveminAgo)
            {
                RecalculateStats();
                IndexManager.build_lucene_index(this.Application);
            }
        }

        private void RecalculateStats()
        {
            ReviewStatisticsRepository rp = new ReviewStatisticsRepository();
            rp.Recalculate();
            lastRan = DateTime.Now;
        }

        private void SetUpAutoMapperMappings()
        {
            Mapper.CreateMap<string, string>().ConvertUsing<NullStringConverter>();

            AutoMapper.Mapper.CreateMap<ReviewData, Review>()
                .ForMember(dto => dto.images, opt => opt.Ignore());

            AutoMapper.Mapper.CreateMap<Review, ReviewData>()
                .ForSourceMember(dto => dto.images, opt => opt.Ignore())
                .ForSourceMember(dto => dto.createdDate, opt => opt.Ignore())
                .ForSourceMember(dto => dto.lastModified, opt => opt.Ignore())
                .ForMember(dto => dto.parentName, opt => opt.ResolveUsing<ParentNameResolver>());

            AutoMapper.Mapper.CreateMap<ReviewableData, Review>()
                .ForMember(dto => dto.images, opt => opt.ResolveUsing<ImageResolverReviewableDataToReview>())
                .ForMember(dto => dto.providedByBizId, opt => opt.ResolveUsing<ProvidedByBizIdResolver>())
                .ForMember(dto => dto.providedByBizName, opt => opt.ResolveUsing<ProvidedByBizNameResolver>())
                .ForMember(dto => dto.parentReviewableId, opt => opt.ResolveUsing<ParentReviewableIdResolver>())
                .ForMember(dto => dto.createdDate, opt => opt.Ignore())
                .ForMember(dto => dto.lastModified, opt => opt.Ignore())
                .ForMember(dto => dto.id, opt => opt.Ignore())
                .ForSourceMember(dto => dto.createdDate, opt => opt.Ignore());

            AutoMapper.Mapper.CreateMap<ReviewableData, Reviewable>()
                .ForMember(dto => dto.images, opt => opt.ResolveUsing<ImageResolverReviewableDataToReviewable>());
            AutoMapper.Mapper.CreateMap<Reviewable, ReviewableData>()
                .ForSourceMember(dto => dto.images, opt => opt.Ignore());
            

            AutoMapper.Mapper.CreateMap<ImageData, Image>();
            AutoMapper.Mapper.CreateMap<Image, ImageData>();

            AutoMapper.Mapper.CreateMap<UserData, User>();
            AutoMapper.Mapper.CreateMap<User, UserData>()
                .ForMember(dto => dto.sessionMaterial, opt => opt.ResolveUsing<SessionMaterialResolver>())
                .ForMember(dto => dto.expires, opt => opt.ResolveUsing<SessionExpiresResolver>())
                .ForMember(dto => dto.sessionToken, opt => opt.ResolveUsing<SessionTokenResolver>())
                .ForMember(dto => dto.lastLogin, opt => opt.ResolveUsing<LastLoginResolver>())
                .ForMember(dto => dto.createdDate, opt => opt.ResolveUsing<UserCreatedDateResolver>());

            AutoMapper.Mapper.CreateMap<UserStatisticData, UserStatistic>();
            AutoMapper.Mapper.CreateMap<UserStatistic, UserStatisticData>();

            AutoMapper.Mapper.CreateMap<MonthlyUserStatisticData, MonthlyUserStatistic>();
            AutoMapper.Mapper.CreateMap<MonthlyUserStatistic, MonthlyUserStatisticData>();

            AutoMapper.Mapper.CreateMap<LikeData, Like>();
            AutoMapper.Mapper.CreateMap<Like, LikeData>();

            AutoMapper.Mapper.CreateMap<CommentData, Comment>();
            AutoMapper.Mapper.CreateMap<Comment, CommentData>();


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

            if (!BsonClassMap.IsClassMapRegistered(typeof(CommentData)))
            {
                BsonClassMap.RegisterClassMap<CommentData>(cm =>
                {
                    cm.AutoMap();
                    cm.GetMemberMap(c => c.commentBy).SetRepresentation(BsonType.String);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(MonthlyUserStatisticData)))
            {
                BsonClassMap.RegisterClassMap<MonthlyUserStatisticData>(cm =>
                {
                    cm.AutoMap();
                    cm.GetMemberMap(c => c.userId).SetRepresentation(BsonType.String);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(LikeData)))
            {
                BsonClassMap.RegisterClassMap<LikeData>(cm =>
                {
                    cm.AutoMap();
                    cm.GetMemberMap(c => c.likedBy).SetRepresentation(BsonType.String);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(UserStatisticData)))
            {
                BsonClassMap.RegisterClassMap<UserStatisticData>(cm =>
                {
                    cm.AutoMap();
                    cm.GetMemberMap(c => c.userId).SetRepresentation(BsonType.String);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(ReviewData)))
            {
                BsonClassMap.RegisterClassMap<ReviewData>(cm =>
                {
                    cm.AutoMap();
                    cm.GetMemberMap(c => c.user).SetRepresentation(BsonType.String);
                    cm.GetMemberMap(c => c.parentReviewableId).SetRepresentation(BsonType.String);

                });
            }


            if (!BsonClassMap.IsClassMapRegistered(typeof(ReviewableData)))
            {
                BsonClassMap.RegisterClassMap<ReviewableData>(cm =>
                {
                    cm.AutoMap();
                    cm.GetMemberMap(c => c.parentReviewableId).SetRepresentation(BsonType.String);
                });
            }
        }
    }

    //public class ImageResolverReviewDataToReview : ValueResolver<ReviewData, List<Image>>
    //{
    //    protected override List<Image> ResolveCore(ReviewData source)
    //    {
    //        Controllers.ReviewsController rc = new Controllers.ReviewsController();

    //        return rc.GetImagesFromStringArray(source.images);
    //    }
    //}

    public class ImageResolverReviewableDataToReview : ValueResolver<ReviewableData, List<Image>>
    {
        protected override List<Image> ResolveCore(ReviewableData source)
        {
            Controllers.ReviewsController rc = new Controllers.ReviewsController();

            List<Image> results = rc.GetImagesFromStringArray(source.images);

            return results;
        }
    }

    public class ImageResolverReviewableDataToReviewable : ValueResolver<ReviewableData, List<Image>>
    {
        protected override List<Image> ResolveCore(ReviewableData source)
        {
            Controllers.ReviewablesController rc = new Controllers.ReviewablesController();

            List<Image> results = rc.GetImagesFromStringArray(source.images);

            return results;
        }
    }

    public class ProvidedByBizIdResolver : ValueResolver<ReviewableData, Guid>
    {
        protected override Guid ResolveCore(ReviewableData source)
        {
            return source.parentReviewableId;
        }
    }

    public class ProvidedByBizNameResolver : ValueResolver<ReviewableData, string>
    {
        protected override string ResolveCore(ReviewableData source)
        {
            return source.parentName;
        }
    }

    public class ParentReviewableIdResolver : ValueResolver<ReviewableData, Guid>
    {
        protected override Guid ResolveCore(ReviewableData source)
        {
            return source.id;
        }
    }

    public class ParentNameResolver : ValueResolver<Review, string>
    {
        protected override string ResolveCore(Review source)
        {
            ReviewableRepository rr = new ReviewableRepository();
            ReviewableData rd = rr.Get(source.parentReviewableId, Guid.Empty);
            return rd.name;
        }
    }
    
    public class SessionMaterialResolver : ValueResolver<User, string>
    {
        protected override string ResolveCore(User source)
        {
            UserRepository ur = new UserRepository();
            UserData ud = ur.GetByUserId(source.id);
            if (ud != null)
                return ud.sessionMaterial;
            else
                return "";
        }   
    }

    public class SessionTokenResolver : ValueResolver<User, string>
    {
        protected override string ResolveCore(User source)
        {
            UserRepository ur = new UserRepository();
            UserData ud = ur.GetByUserId(source.id);
            if (ud != null)
                return ud.sessionToken;
            else
                return "";
        }
    }

    public class LastLoginResolver : ValueResolver<User, DateTime>
    {
        protected override DateTime ResolveCore(User source)
        {
            UserRepository ur = new UserRepository();
            UserData ud = ur.GetByUserId(source.id);
            
                return ud.lastLogin;
            
                
        }
    }
    public class SessionExpiresResolver : ValueResolver<User, DateTime>
    {
        protected override DateTime ResolveCore(User source)
        {
            UserRepository ur = new UserRepository();
            UserData ud = ur.GetByUserId(source.id);

            return ud.expires;


        }
    }

    public class UserCreatedDateResolver : ValueResolver<User, DateTime>
    {
        protected override DateTime ResolveCore(User source)
        {
            UserRepository ur = new UserRepository();
            UserData ud = ur.GetByUserId(source.id);

            return ud.createdDate;


        }
    }

    public class NullStringConverter : TypeConverter<string, string>
    {
        protected override string ConvertCore(string source)
        {
            return source ?? string.Empty;
        }
    }

    
}