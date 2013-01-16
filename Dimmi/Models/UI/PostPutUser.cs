using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models.UI
{
    public class PostPutUser
    {
        public Guid userId { get; set; }
        public string sessionToken { get; set; }
        public User user { get; set; }
    }
}