using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dimmi.Models;

namespace Dimmi.DataInterfaces
{
    interface IImageRepository
    {
        //Image Add(Image image);
        Image Get(string uid);
        IEnumerable<Image> GetCountForEachCategory(int count);
    }
}
