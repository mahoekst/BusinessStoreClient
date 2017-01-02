using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessStoreClient
{
    public class ProductPackageSet
    {
        public string PackageSetId { get; set; }
        public List<ProductPackageDetails> ProductPackages { get; set; }
    }
}
