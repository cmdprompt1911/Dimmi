﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dimmi.Models
{
    public class Reviewable : ReviewableBase
    {
        public List<Image> images { get; set; }
    }
}