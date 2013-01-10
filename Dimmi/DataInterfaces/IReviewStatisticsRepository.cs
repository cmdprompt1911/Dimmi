using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dimmi.Models;

namespace Dimmi.DataInterfaces
{
    public interface IReviewStatisticsRepository
    {
        ReviewStatistic Get(Guid userId);
        List<ReviewStatistic> GetPageFromAllTimeStats(int pageNumber, int pageSize);
        List<ReviewStatistic> GetCurrentTop(int count);
        void Recalculate();
    }
}