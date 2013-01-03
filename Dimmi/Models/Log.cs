using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models
{
    public class Log
    {
        public string message { get; set; }
        public Exception ex { get; set; }
        public string caller { get; set; }
    
    }
}