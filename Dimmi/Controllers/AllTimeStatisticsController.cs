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
    public class AllTimeStatisticsController : ApiController
    {
        static readonly IReviewStatisticsRepository repository = new ReviewStatisticsRepository();

        static readonly UsersController _usersController = new UsersController();


        public UserStatistic Get(Guid userId)
        {
            if (!_usersController.IsUserValid(Request))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            UserStatisticData allTime = repository.GetAllTimeForUser(userId);
            MonthlyUserStatisticData month = repository.GetMonthlyForUser(userId, DateTime.UtcNow.Month, DateTime.UtcNow.Year);

            UserStatistic ret = AutoMapper.Mapper.Map<UserStatisticData, UserStatistic>(allTime);
            ret.currentMonthScore = month.score;

            if (ret == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return ret;
        }

        public IEnumerable<UserStatistic> GetCurrentTop(int count)
        {
            if (!_usersController.IsUserValid(Request))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            if (count > 50)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }
            List<UserStatisticData> statisticData = repository.GetCurrentTop(count);

            List<UserStatistic> stats = PopulateData(statisticData);

            if (stats == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return stats;
        }

        public IEnumerable<UserStatistic> GetPage(int pageNumber, int pageSize)
        {
            if (!_usersController.IsUserValid(Request))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            if (pageSize > 50)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }

            List < UserStatisticData > statisticData = repository.GetPageFromAllTimeStats(pageNumber, pageSize);

            List<UserStatistic> stats = PopulateData(statisticData);

            if (statisticData == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return stats;
        }

        private List<UserStatistic> PopulateData(List<UserStatisticData> statdata)
        {


            List<UserStatistic> output = new List<UserStatistic>();
            foreach (UserStatisticData rd in statdata)
            {
                output.Add(PopulateData(rd));
            }
            return output;
        }

        private UserStatistic PopulateData(UserStatisticData statData)
        {

            UserStatistic stat = AutoMapper.Mapper.Map<UserStatisticData, UserStatistic>(statData);
            return stat;
        }

    }
}
