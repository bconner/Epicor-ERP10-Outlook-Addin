//The MIT License (MIT)

//Copyright (c) Microsoft Corporation

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using E10OutlookConnectorWeb.Models;
using Microsoft.Exchange.WebServices.Auth.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Utils;

namespace E10OutlookConnectorWeb.Controllers
{
    public class E10Controller : ApiController
    {
        static Dictionary<string, string> idCache;

        // Static constructor
        static E10Controller()
        {
            idCache = new Dictionary<string, string>();
        }

        public async Task<HttpResponseMessage> Post(ServiceRequest serviceRequest)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            // ensure we have a valid Identity Token, i.e. the user belongs to this exchange
            var idToken = TokenDecoder.Decode(serviceRequest.token);
            var token = (AppIdentityToken)AuthToken.Parse(serviceRequest.token);

            try
            {
                // Validate the user identity token. 
                token.Validate(new Uri(Config.Audience));

                // If the token is invalid, Validate will throw an exception. If the service reaches
                // this line, the token is valid.
                string credentials = string.Empty;

                // Check to see if the uniqued ID is in the cache.
                if (idCache.ContainsKey(token.UniqueUserIdentification))
                {
                    credentials = idCache[token.UniqueUserIdentification];
                }
                // If the unique ID is not found, check to see if the request contains credentials.
                else if (!string.IsNullOrEmpty(serviceRequest.userName) && !string.IsNullOrEmpty(serviceRequest.password))
                {
                    credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", serviceRequest.userName, serviceRequest.password)));
                    idCache.Add(token.UniqueUserIdentification, credentials);
                }
                else
                {
                    response.StatusCode = HttpStatusCode.Unauthorized;
                }

                if (!string.IsNullOrEmpty(credentials))
                {
                    using (var client = new HttpClient())
                    {

                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, @"https://40.86.103.253/ERP101500/api/v1/Erp.Bo.CustomerSvc/Customers?$filter=CustID eq 'Addison'");

                        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                        // added to avoid local host ssl certificate errors - should be removed once 'proper' certificate is in place.
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                        response = await client.SendAsync(request);
                    }
                }
            }
            catch (TokenValidationException ex)
            {
                response.StatusCode = HttpStatusCode.Unauthorized;
            }

            return response;
        }
    }
}