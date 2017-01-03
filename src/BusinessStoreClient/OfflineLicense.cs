using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessStore
{
    public class OfflineLicense
    {
        public ProductKey ProductKey { get; set; }
        public string LicenseBlob { get; set; }
        public string LicenseInstanceId { get; set; }
        public string RequestorId { get; set; }
        public string ContentId { get; set; }

    }
}
