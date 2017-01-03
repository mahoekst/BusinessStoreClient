using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;


namespace BusinessStore
{
    public class BusinessStoreClient
    {

        private const string ResourceUrl = "https://onestore.microsoft.com";
        private const string baseUri = "https://bspmts.mp.microsoft.com/V1";

        private string _clientid;
        private string _authority;
        private string _clientsecret;

        private HttpClient Restclient;
        private string authHeader;
        private string token;

        //private Task _aquireTokenTask;

        public BusinessStoreClient(string clientId, string authority, string clientSecret)
        {
            _clientid = clientId;
            _authority = authority;
            _clientsecret = clientSecret;
            AcquireTokenAsync().Wait();
        }

        public async Task<InventoryResultSet> GetInventoryAsync()
        {
            var url = $"{baseUri}/Inventory?maxresults=25&licenseTypes=offline";
            var response = await Restclient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var s = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(s);
                InventoryResultSet inventoryResultSet = JsonConvert.DeserializeObject<InventoryResultSet>(s);
                return inventoryResultSet;
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }

        }

        public async Task<ProductDetails> GetProductDetailsAsync(ProductKey productKey)
        {
            var url = $"{baseUri}/Products/{productKey.ProductId}/{productKey.SkuId}";
            var response = await Restclient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var s = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("PRODUCTDETAIL:");
                Debug.WriteLine(s);
                ProductDetails productDetails = JsonConvert.DeserializeObject<ProductDetails>(s);
                return productDetails;
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        public async Task<ProductPackageSet> GetProductPackagesAsync(ProductKey productKey)
        {
            var url = $"{baseUri}/Products/{productKey.ProductId}/{productKey.SkuId}/Packages";
            var response = await Restclient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var s = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("PRODUCTPACKAGES:");
                Debug.WriteLine(s);
                ProductPackageSet productPackageSet = JsonConvert.DeserializeObject<ProductPackageSet>(s);
                return productPackageSet;
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        public async Task<OfflineLicense> GetOffLineLicenseAsync(ProductKey productKey, ProductPackageDetails productPackageDetails)
        {
            var url = $"{baseUri}/Products/{productKey.ProductId}/{productKey.SkuId}/OfflineLicense/{productPackageDetails.ContentId}";
            var response = await Restclient.PostAsync(url, null);
            if (response.IsSuccessStatusCode)
            {
                var s = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("OFFLINELICENSE:");
                Debug.WriteLine(s);
                OfflineLicense offlineLicense = JsonConvert.DeserializeObject<OfflineLicense>(s);
                return offlineLicense;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                //only offline apps will have a offlinelicence. Check your licensetype before calling this method
                //to optimize your application
                return null;
            }
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        public async Task AcquireTokenAsync()
        {
            AuthenticationContext context = new AuthenticationContext(_authority, true);
            var credential = new ClientCredential(_clientid, _clientsecret);
            
            try
            {
                var result = await context.AcquireTokenAsync(ResourceUrl, credential).ConfigureAwait(false);
                token = result.AccessToken;
                authHeader = result.CreateAuthorizationHeader();
                Restclient = new HttpClient();
                
                Debug.WriteLine(token + "\n");

                Restclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                Restclient.DefaultRequestHeaders.Add("Authorization", authHeader);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
