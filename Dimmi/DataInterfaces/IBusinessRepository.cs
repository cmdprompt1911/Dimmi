using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dimmi.Models;

namespace Dimmi.DataInterfaces
{
    interface IBusinessRepository
    {
        IEnumerable<Business> Get();
        Business Get(int companyId);
        Business Add(Business business);
        Image AddImage(Image data, int companyId);
        IEnumerable<Image> GetImages(int companyId);
    }
}
