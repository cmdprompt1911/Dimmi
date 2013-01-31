using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dimmi.Models.Domain;

namespace Dimmi.DataInterfaces
{
    public interface IReviewStatisticsRepository
    {
        UserStatisticData GetAllTimeForUser(Guid userId);
        MonthlyUserStatisticData GetMonthlyForUser(Guid userId, int month, int year);
        List<MonthlyUserStatisticData> GetMonthlyForUser(Guid userId);
        List<UserStatisticData> GetPageFromAllTimeStats(int pageNumber, int pageSize);
        List<MonthlyUserStatisticData> GetPageFromMonthlyStats(int pageNumber, int pageSize, int month, int year);
        List<UserStatisticData> GetCurrentTop(int count);
        void Recalculate();
    }
}