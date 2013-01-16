using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using Dimmi.Models.Domain;

namespace Dimmi.DataInterfaces
{
    interface IReviewableRepository
    {
        IEnumerable<ReviewableData> GetByName(string name, Guid userId);
        IEnumerable<ReviewableData> GetByNameByType(string name, string type, Guid userId);
        IEnumerable<ReviewableData> Get();
        IEnumerable<ReviewableData> GetByType(string type);
        IEnumerable<ReviewableData> Get(string[] reviewables);
        ReviewableData Get(Guid reviewableId, Guid userId);
        ReviewableData Add(ReviewableData item, Guid userId);
        ImageData AddImage(ImageData data, Guid reviewableId);
        ReviewableData Update(ReviewableData reviewable, Guid userId, bool updateSearchIndex);
        //IEnumerable<Image> GetImages(int productId);
        //void Remove(int id);
        //bool Update(Product item);

    }
}
