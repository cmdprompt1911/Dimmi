using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dimmi.Models.Domain;

namespace Dimmi.DataInterfaces
{
    public interface IUserRepository
    {
        UserData Get(string emailAddress);
        UserData Get(string oathId, string emailAddress);
        UserData Get(string oathId, string emailAddress, DateTime lastAccessed);
        UserData GetByUserId(Guid userId);
        IEnumerable<UserData> GetList();
        void UpdateLoginTimeStamp(string emailAddress);
        UserData Update(UserData user);
        UserData Add(UserData user);

    }
}
