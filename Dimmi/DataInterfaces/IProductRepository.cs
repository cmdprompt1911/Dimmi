using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dimmi.Models;

namespace Dimmi.DataInterfaces
{
    interface IProductRepository
    {
        IEnumerable<Product> GetByName(string name, int userId);
        Product Get(int id, int userId);
        Product Add(Product item);
        Image AddImage(Image data, int productId);
        IEnumerable<Image> GetImages(int productId);
        //void Remove(int id);
        //bool Update(Product item);

    }
}
