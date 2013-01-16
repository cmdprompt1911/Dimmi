using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dimmi.Models.Domain;

namespace Dimmi.DataInterfaces
{
    public interface IReviewStatisticsRepository
    {
        ReviewStatisticData Get(Guid userId, bool last30days);
        List<ReviewStatisticData> GetPageFromAllTimeStats(int pageNumber, int pageSize);
        List<ReviewStatisticData> GetCurrentTop(int count);
        void Recalculate();
    }
}