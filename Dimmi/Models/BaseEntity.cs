using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization;

namespace Dimmi.Models
{
    
    public abstract class BaseEntity
    {
        [BsonId]
        public virtual Guid id
        {
            get;
            set;
        }
    }

}