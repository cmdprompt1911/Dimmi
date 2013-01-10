using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dimmi.Models;
using Dimmi.Data;
using Dimmi.DataInterfaces;
using MongoDB.Bson;
namespace Dimmi.Controllers
{
    public class ReviewStatisticsController : ApiController
    {
        static readonly IReviewStatisticsRepository repository = new ReviewStatisticsRepository();

        public ReviewStatistic Get(Guid userId)
        {
            ReviewStatistic userData = repository.Get(userId);
            if (userData == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return userData;
        }

        public IEnumerable<ReviewStatistic> GetCurrentTop(int count)
        {
            if (count > 50)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }
            IEnumerable<ReviewStatistic> statisticData = repository.GetCurrentTop(count);

            if (statisticData == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return statisticData;
        }

        public IEnumerable<ReviewStatistic> GetPage(int pageNumber, int pageSize)
        {
            if (pageSize > 50)
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }
            IEnumerable<ReviewStatistic> statisticData = repository.GetPageFromAllTimeStats(pageNumber, pageSize);

            if (statisticData == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            return statisticData;
        }

    }
}
