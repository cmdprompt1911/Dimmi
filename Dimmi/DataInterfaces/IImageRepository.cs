using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dimmi.Models.Domain;
using MongoDB.Bson;

namespace Dimmi.DataInterfaces
{

    interface IImageRepository
    {
        ImageData Add(ImageData image);
        ImageData Get(Guid uid);
        IEnumerable<ImageData> Get(string[] images);
        IEnumerable<ImageData> GetTopForEachCategory();
    }
}
