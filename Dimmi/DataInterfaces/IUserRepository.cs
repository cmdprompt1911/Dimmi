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
        void UpdateLoginTimeStamp(string emailAddress);
        User UpdateFromFBData(User user);
        User AddFromFBData(User user);

    }
}
