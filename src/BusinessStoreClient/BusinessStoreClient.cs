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
using BusinessStoreClient.Internal;

namespace BusinessStoreClient
{
    public class BusinessStoreClient
    {

        private const string ResourceUrl = "https://onestore.microsoft.com";
        private const string baseUri = "https://bspmts.mp.microsoft.com/V1";

        private string authHeader;
        private string _clientid;
        private string _authority;
        private string _clientsecret;
        private HttpClient Restclient;
        private string token;

        //private Task _aquireTokenTask;

        public BusinessStoreClient(string clientId, string authority, string clientSecret)
        {
            _clientid = clientId;
            _authority = authority;
            _clientsecret = clientSecret;
            //_aquireTokenTask = AcquireTokenAsync().Wait() ;// authority,clientId,clientSecret);
        }

        public async Task<InventoryResultSet> GetInventoryAsync()
        {
            //await _aquireTokenTask;
            var url = $"{baseUri}/Inventory?maxresults=2&licenseTypes=offline";
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
            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        public async Task AcquireTokenAsync()
        {
            AuthenticationContext context = new AuthenticationContext(_authority, true);
            var credential = new ClientCredential(_clientid, _clientsecret);
            var handler = new HttpAuthenticationHandler(ResourceUrl, context, credential);
            try
            {
            var result = await context.AcquireTokenAsync(ResourceUrl, credential);//.ConfigureAwait(false); ;
            token = result.AccessToken;
            authHeader = result.CreateAuthorizationHeader();
            //client = new HttpClient();
            Restclient = new HttpClient(handler);

            Debug.WriteLine(token + "\n");

            Restclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Restclient.DefaultRequestHeaders.Add("Authorization", authHeader);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        //public async Task AcquireTokenAsync()
        //{
        //    AuthenticationContext context = new AuthenticationContext(_authority, true);
        //    credential = new ClientCredential(_clientId, _clientSecret);
        //    handler = new HttpAuthenticationHandler(ResourceUrl, context, credential);

        //    var result = await context.AcquireTokenAsync(ResourceUrl, credential);
        //    //var result = await context.AcquireTokenAsync(ResourceUrl, ClientId, new Uri(RedirectUrl), new PlatformParameters(PromptBehavior.Always));
        //    token = result.AccessToken;
        //    authHeader = result.CreateAuthorizationHeader();
        //    client = new HttpClient();
        //    //client = new HttpClient(handler);

        //    Console.WriteLine(token + "\n");

        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //    client.DefaultRequestHeaders.Add("Authorization", authHeader);
        //    //var url = $"{baseUri}/Products/a/b";
        //    //var response = await client.GetAsync(url);
        //}



    }
}
