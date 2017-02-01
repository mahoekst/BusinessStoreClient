using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessStore
{
    public class ProductPackageDetails
    {
        public List<FrameworkPackageDetails> FrameworkDependencyPackages { get; set; }
        public string ContentId { get; set; }
        public string PackageId { get; set; }
        public PackageLocation Location { get; set; }
        public string PackageFullName { get; set; }
        public string PackageIdentityName { get; set; }
        public List<ProductArchitectures> Architectures { get; set; }
        public ProductPackageFormat PackageFormat { get; set; }
        public List<ProductPlatform> Platforms { get; set; }
        public long FileSize { get; set; }
        public int PackageRank { get; set; }
        public VersionInfo Version { get; set; }
        public string FileHash { get; set; }
        public string FileHashAlgorithm { get; set; }
    }
}
