using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dimmi.Models;

namespace Dimmi.DataInterfaces
{
    interface IUserRepository
    {
        User Get(string emailAddress);
        IEnumerable<User> GetList();
        void UpdateLoginTimeStamp(string emailAddress);
        User Update(User user);
        User Add(User user);

    }
}
