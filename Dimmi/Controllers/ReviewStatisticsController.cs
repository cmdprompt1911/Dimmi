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
    public class ReviewStatisticsController : ApiController
    {
        static readonly IReviewStatisticsRepository repository = new ReviewStatisticsRepository();

        static readonly UsersController _usersController = new UsersController();


        public ReviewStatistic Get(Guid user, Guid userId, string sessionToken)
        {
            if (!_usersController.IsUserValid(userId, sessionToken))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            ReviewStatisticData allTime = repository.Get(user, false);
            ReviewStatisticData last30 = repository.Get(user, true);

            ReviewStatistic ret = AutoMapper.Mapper.Map<ReviewStatisticData, ReviewStatistic>(allTime);
            ret.Last30Score = last30.score;

            if (ret == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return ret;
        }

        public IEnumerable<ReviewStatistic> GetCurrentTop(int count, Guid userId, string sessionToken)
        {
            if (!_usersController.IsUserValid(userId, sessionToken))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            if (count > 50)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }
            IEnumerable<ReviewStatisticData> statisticData = repository.GetCurrentTop(count);
            AutoMapper.Mapper.CreateMap<IEnumerable<ReviewStatisticData>, IEnumerable<ReviewStatistic>>();
            IEnumerable<ReviewStatistic> userStats = AutoMapper.Mapper.Map<IEnumerable<ReviewStatisticData>, IEnumerable<ReviewStatistic>>(statisticData);

            if (statisticData == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return userStats;
        }

        public IEnumerable<ReviewStatistic> GetPage(int pageNumber, int pageSize, Guid userId, string sessionToken)
        {
            if (!_usersController.IsUserValid(userId, sessionToken))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            if (pageSize > 50)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }

            IEnumerable < ReviewStatisticData > statisticData = repository.GetPageFromAllTimeStats(pageNumber, pageSize);
            AutoMapper.Mapper.CreateMap<IEnumerable<ReviewStatisticData>, IEnumerable<ReviewStatistic>>();
            IEnumerable<ReviewStatistic> stats = AutoMapper.Mapper.Map<IEnumerable<ReviewStatisticData>, IEnumerable<ReviewStatistic>>(statisticData);

            if (statisticData == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return stats;
        }

    }
}
