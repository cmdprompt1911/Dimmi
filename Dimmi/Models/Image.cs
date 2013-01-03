using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models
{
    public class Image
    {
        public string uid { get; set; }
        public string name { get; set; }
        public string data { get; set; }
        public int type { get; set; }
        public int @fileType { get; set; }
        public string fileTypeName { get; set; }
        public DateTime dateCreated { get; set; }
    }
}