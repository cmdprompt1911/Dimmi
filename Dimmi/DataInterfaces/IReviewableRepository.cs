using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Dimmi.Models;

namespace Dimmi.DataInterfaces
{
    interface IReviewableRepository
    {
        IEnumerable<Reviewable> GetByName(string name, Guid userId);
        IEnumerable<Reviewable> GetByNameByType(string name, string type, Guid userId);
        IEnumerable<Reviewable> Get();
        Reviewable Get(Guid reviewableId, Guid userId);
        Reviewable Add(Reviewable item, Guid userId);
        Image AddImage(Image data, Guid reviewableId);
        Reviewable Update(Reviewable reviewable, Guid userId);
        //IEnumerable<Image> GetImages(int productId);
        //void Remove(int id);
        //bool Update(Product item);

    }
}
