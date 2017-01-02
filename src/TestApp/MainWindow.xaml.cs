﻿using BusinessStoreClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            InventoryResultSet inventoryResultSet;
            ProductPackageSet productPackgeSet;
            ProductDetails productDetails;
            OfflineLicense offlineLicense;

            BusinessStoreClient.BusinessStoreClient client = new BusinessStoreClient.BusinessStoreClient(ClientId,Authority,ClientSecret);

            
            inventoryResultSet = await client.GetInventoryAsync();
            txtResult.Text = String.Format("RESULT: {0} entries found.\n", inventoryResultSet.InventoryEntries.Count());
            foreach (InventoryEntryDetails entry in inventoryResultSet.InventoryEntries)
            {
                productDetails = await client.GetProductDetailsAsync(entry.ProductKey);
                txtResult.Text = String.Format("PRODUCTDETAILS:{0}\n", productDetails.PackageFamilyName) + txtResult.Text;
                productPackgeSet = await client.GetProductPackagesAsync(entry.ProductKey);
                txtResult.Text = String.Format("PRODUCTSETS FOUND FOR THIS PRODUCT:{0}\n", productPackgeSet.ProductPackages.Count) + txtResult.Text;
                if (entry.LicenseType == LicenseType.Offline)
                { 
                    offlineLicense = await client.GetOffLineLicenseAsync(entry.ProductKey, productPackgeSet.ProductPackages[0]);
                    txtResult.Text = String.Format("OFFLINELICENSE FOUND:{0}\n", offlineLicense.LicenseInstanceId) + txtResult.Text;
                }
                //now download everything in the correct folder
                //save the offlinelicense blob in an xml file
            }
        }
    }
}
