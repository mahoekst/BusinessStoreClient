using BusinessStore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ClientId = "21f30783-2e22-4b96-b159-bf573e34c018";
        private const string RedirectUrl = "http://azureAADTestApp";             // Can be anything but must match what you entered in AAD
        private const string Authority = "https://login.windows.net/hoekstraonline.onmicrosoft.com";
        private const string ClientSecret = "hiRARQSQwtI8yUDKjlfORN+3+VIim14kNxca+nC1TWA=";

        private const string downloaddirectory = "c:\\scratch\\businesstore\\";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            string targetdir = "";
            string targetfile = "";
            InventoryResultSet inventoryResultSet;
            ProductPackageSet productPackageSet;
            ProductDetails productDetails;
            OfflineLicense offlineLicense;

            BusinessStoreClient client = new BusinessStoreClient(ClientId,Authority,ClientSecret);

            
            inventoryResultSet = await client.GetInventoryAsync();
            txtResult.Text = String.Format("RESULT: {0} entries found.\n", inventoryResultSet.InventoryEntries.Count());
            foreach (InventoryEntryDetails entry in inventoryResultSet.InventoryEntries)
            {
                productDetails = await client.GetProductDetailsAsync(entry.ProductKey);
                txtResult.Text = String.Format("PRODUCTDETAILS:{0}\n", productDetails.PackageFamilyName) + txtResult.Text;
                productPackageSet = await client.GetProductPackagesAsync(entry.ProductKey);
                txtResult.Text = String.Format("PRODUCTSETS FOUND FOR THIS PRODUCT:{0}\n", productPackageSet.ProductPackages.Count) + txtResult.Text;
                targetdir = downloaddirectory + productDetails.PackageFamilyName;
                if (!Directory.Exists(targetdir))
                {
                    Directory.CreateDirectory(targetdir);
                }

                if (entry.LicenseType == LicenseType.Offline)
                { 
                    offlineLicense = await client.GetOffLineLicenseAsync(entry.ProductKey, productPackageSet.ProductPackages[0]);
                    txtResult.Text = String.Format("OFFLINELICENSE FOUND:{0}\n", offlineLicense.LicenseInstanceId) + txtResult.Text;
                    targetfile = targetdir + "\\" + "offlinelicense.bin";
                    File.WriteAllText(targetfile, offlineLicense.LicenseBlob);
                }
                foreach (var productPackage in productPackageSet.ProductPackages)
                {
                    HttpClient httpClient = new HttpClient();
                    HttpResponseMessage response = await httpClient.GetAsync(productPackage.Location.Url);
                    var httpStream = await response.Content.ReadAsStreamAsync();
 

                        string archdir ="";
                        foreach (ProductArchitectures arc in productPackage.Architectures)
                        {
                            archdir = archdir + arc;
                        }
                        archdir = archdir + "\\";

                        targetdir = downloaddirectory + productDetails.PackageFamilyName + "\\" + productPackage.Platforms[0].PlatformName + "\\" +  archdir + productPackage.Platforms[0].MinVersion.Build + "\\" + productPackage.Platforms[0].MaxTestedVersion.Build;

                    
                    if (!Directory.Exists(targetdir))
                    {
                        Directory.CreateDirectory(targetdir);
                    }

                    if (productPackage.PackageFullName != "")
                    {
                        targetfile = targetdir + "\\" + productPackage.PackageFullName + "." + productPackage.PackageFormat;
                    }
                    else
                    {
                        targetfile = targetdir + "\\" + productPackage.PackageIdentityName + "." + productPackage.PackageFormat;

                    }

                    using (var fileStream = File.Create(targetfile))
                    using (var reader = new StreamReader(httpStream))
                    {
                        httpStream.CopyTo(fileStream);
                        fileStream.Flush();
                    }
                    httpStream.Close();
                    httpStream.Dispose();
                    httpStream = null;
                    response.Dispose();
                    response = null;
                    httpClient.Dispose();
                    httpClient = null;

                    if (productPackage.FrameworkDependencyPackages.Count>0)
                    {
                        if (!Directory.Exists(targetdir+"\\dependencies"))
                        {
                            Directory.CreateDirectory(targetdir+"\\dependencies");
                        }
                        foreach (FrameworkPackageDetails frameworkPackageDetails in productPackage.FrameworkDependencyPackages)
                        {
                            txtResult.Text = String.Format("DOWNLOADING DEPENDENCY:{0}\n", frameworkPackageDetails.PackageFullName) + txtResult.Text;

                            HttpClient httpClientDependency = new HttpClient();
                            HttpResponseMessage dependencyresponse = await httpClientDependency.GetAsync(frameworkPackageDetails.Location.Url);
                            var httpDependencyStream = await dependencyresponse.Content.ReadAsStreamAsync();
                            targetfile = targetdir + "\\dependencies\\" + "\\" + frameworkPackageDetails.PackageFullName + "." + frameworkPackageDetails.PackageFormat;
                            using (var fileStream = File.Create(targetfile))
                            using (var reader = new StreamReader(httpDependencyStream))
                            {
                                httpDependencyStream.CopyTo(fileStream);
                                fileStream.Flush();
                            }
                            httpDependencyStream.Close();
                            httpDependencyStream.Dispose();
                            httpDependencyStream = null;
                            dependencyresponse.Dispose();
                            dependencyresponse = null;
                            httpClientDependency.Dispose();
                            httpClientDependency = null;
                        }
                        


                    }
                }


                //now download everything in the correct folder
                //save the offlinelicense blob in an xml file
            }
        }
    }
}
