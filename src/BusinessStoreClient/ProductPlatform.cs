﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessStore
{
    public class ProductPlatform
    {
        public string PlatformName { get; set; }
        public VersionInfo MinVersion { get; set; }
        public VersionInfo MaxTestedVersion { get; set; }
    }
}
