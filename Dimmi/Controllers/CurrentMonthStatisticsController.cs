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
    public class CurrentMonthStatisticsController : ApiController
    {
        static readonly IReviewStatisticsRepository repository = new ReviewStatisticsRepository();

        static readonly UsersController _usersController = new UsersController();


        public IEnumerable<MonthlyUserStatistic> Get(int pageNumber, int pageSize, int month, int year)
        {
            if (!_usersController.IsUserValid(Request))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            if (pageSize > 50)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }
            List < MonthlyUserStatisticData> monthly = repository.GetPageFromMonthlyStats(pageNumber, pageSize, month, year);

            List<MonthlyUserStatistic> ret = PopulateData(monthly);


            if (ret == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return ret;
        }

        public IEnumerable<MonthlyUserStatistic> Get(int pageNumber, int pageSize, Guid userId, int month, int year)
        {
            if (!_usersController.IsUserValid(Request))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            if (pageSize > 50)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }

            List<MonthlyUserStatisticData> monthly = repository.GetPageFromMonthlyStats(pageNumber, pageSize, month, year);

            List<MonthlyUserStatistic> ret = PopulateData(monthly);


            if (ret == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return ret;
        }

        public IEnumerable<MonthlyUserStatistic> Get(Guid userId)
        {
            if (!_usersController.IsUserValid(Request))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }



            List<MonthlyUserStatisticData> monthly = repository.GetMonthlyForUser(userId);

            List<MonthlyUserStatistic> ret = PopulateData(monthly);


            if (ret == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return ret;
        }

        public MonthlyUserStatistic Get( Guid userId, int month, int year)
        {
            if (!_usersController.IsUserValid(Request))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            MonthlyUserStatisticData monthly = repository.GetMonthlyForUser(userId, month, year);

            MonthlyUserStatistic ret = PopulateData(monthly);


            if (ret == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return ret;
        }

        private List<MonthlyUserStatistic> PopulateData(List<MonthlyUserStatisticData> statdata)
        {


            List<MonthlyUserStatistic> output = new List<MonthlyUserStatistic>();
            foreach (MonthlyUserStatisticData rd in statdata)
            {
                output.Add(PopulateData(rd));
            }
            return output;
        }

        private MonthlyUserStatistic PopulateData(MonthlyUserStatisticData statData)
        {

            MonthlyUserStatistic stat = AutoMapper.Mapper.Map<MonthlyUserStatisticData, MonthlyUserStatistic>(statData);
            return stat;
        }

    }
}
