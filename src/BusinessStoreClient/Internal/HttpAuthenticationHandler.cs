using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessStoreClient.Internal
{
    internal class HttpAuthenticationHandler : DelegatingHandler
    {
        private const string AuthScheme = "bearer";
        private readonly string resource2;
        private readonly AuthenticationContext context2;
        private readonly ClientCredential credential2;

        public HttpAuthenticationHandler(string resource, AuthenticationContext context, ClientCredential credential)
        {
            this.resource2 = resource;
            this.context2 = context;
            this.credential2 = credential;
            this.InnerHandler = new HttpClientHandler();
        }

        private async Task<string> GetTokenAsync()
        {
            var token = await context2.AcquireTokenAsync(resource2, credential2);
            return token.AccessToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await GetTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue(AuthScheme, token);
            return await base.SendAsync(request, cancellationToken);
        }
    }

}
