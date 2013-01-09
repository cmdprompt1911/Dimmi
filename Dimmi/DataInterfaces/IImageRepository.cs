using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dimmi.Models;
using MongoDB.Bson;

namespace Dimmi.DataInterfaces
{

    interface IImageRepository
    {
        Image Add(Image image);
        Image Get(Guid uid);
        IEnumerable<Image> GetTopForEachCategory();
    }
}
