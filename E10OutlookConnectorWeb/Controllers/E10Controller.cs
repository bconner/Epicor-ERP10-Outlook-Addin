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
using Models;
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

        [HttpPost]
        [Route("api/e10/contact")]
        public async Task<HttpResponseMessage> Contact(ServiceRequest serviceRequest)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            // ensure we have a valid Identity Token, i.e. the user belongs to this exchange
            var token = (AppIdentityToken)AuthToken.Parse(serviceRequest.token);

            // demonstrates how to decode the token so that it can be inspected
            var idToken = TokenDecoder.Decode(serviceRequest.token);

            try
            {
                // Validate the user identity token. This validation ensures the request came from the Office add-in and not from a rogue request from another source.
                // This does not stop DOS but it does make sure the user does not have access to the Epicor APIs
                ValidateIdentity(token);

                // If the token is invalid, Validate will throw an exception. If the service reaches
                // this line, the token is valid.

                string credentials = string.Empty;

                // Check to see if the uniqued ID is in the cache.
                if (idCache.ContainsKey(token.UniqueUserIdentification))
                {
                    // the user has already logged in within Outlook session so use those credentials
                    credentials = idCache[token.UniqueUserIdentification];
                }
                // If the unique ID is not found, check to see if the request contains a username and password.
                else if (!string.IsNullOrEmpty(serviceRequest.userName) && !string.IsNullOrEmpty(serviceRequest.password))
                {
                    // if a username and password are present then convert to the format expected by the API, in this case Base64 - Basic Authentication
                    credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", serviceRequest.userName, serviceRequest.password)));
                    // cache the credentials in the static dictionary so that they can be retrieved on the next request. This session is valid for the lifetime of the user session.
                    idCache.Add(token.UniqueUserIdentification, credentials);
                }
                else
                {
                    // if this status code is changed to another value, then the check in the fail outcome in the add-in needs to be cahnged as well.
                    response.StatusCode = HttpStatusCode.Unauthorized;
                }

                // finally if the token is valid and credentials have been supplied then make the call to the Epicor API
                if (!string.IsNullOrEmpty(credentials))
                {
                    using (var client = new HttpClient())
                    {
                        // Note - once Epicor have an API endpoint that fits the data model requested by the add-in then the URL will need to change and the request.context value applied as a filter
                        var url = string.Format(@"{0}{1}'", " https://13.89.62.5/ERP101500/api/v1/BaqSvc/CustomerSnapshot?$filter=CustCnt_EMailAddress eq '", serviceRequest.context);

                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

                        // basic authorization
                        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                        // added to avoid local host ssl certificate errors - should be removed once 'proper' certificate is in place.
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                        // wait for the response
                        response = await client.SendAsync(request);
                    }
                }
            }
            catch (TokenValidationException ex)
            {
                // an error in the token suggests the user does not have authorization to call the API
                response.StatusCode = HttpStatusCode.Unauthorized;
            }
            catch (Exception ex)
            {
                // any other error needs to be handled gracefully and the 'correct' status code applied
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }

        [HttpPost]
        [Route("api/e10/values")]
        public async Task<HttpResponseMessage> Values(ServiceRequest serviceRequest)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            // ensure we have a valid Identity Token, i.e. the user belongs to this exchange
            var token = (AppIdentityToken)AuthToken.Parse(serviceRequest.token);

            // demonstrates how to decode the token so that it can be inspected
            var idToken = TokenDecoder.Decode(serviceRequest.token);

            try
            {
                // Validate the user identity token. This validation ensures the request came from the Office add-in and not from a rogue request from another source.
                // This does not stop DOS but it does make sure the user does not have access to the Epicor APIs
                ValidateIdentity(token);

                // If the token is invalid, Validate will throw an exception. If the service reaches
                // this line, the token is valid.

                string credentials = string.Empty;

                // Check to see if the uniqued ID is in the cache.
                if (idCache.ContainsKey(token.UniqueUserIdentification))
                {
                    // the user has already logged in within Outlook session so use those credentials
                    credentials = idCache[token.UniqueUserIdentification];
                }
                // If the unique ID is not found, check to see if the request contains a username and password.
                else if (!string.IsNullOrEmpty(serviceRequest.userName) && !string.IsNullOrEmpty(serviceRequest.password))
                {
                    // if a username and password are present then convert to the format expected by the API, in this case Base64 - Basic Authentication
                    credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", serviceRequest.userName, serviceRequest.password)));
                    // cache the credentials in the static dictionary so that they can be retrieved on the next request. This session is valid for the lifetime of the user session.
                    idCache.Add(token.UniqueUserIdentification, credentials);
                }
                else
                {
                    // if this status code is changed to another value, then the check in the fail outcome in the add-in needs to be cahnged as well.
                    response.StatusCode = HttpStatusCode.Unauthorized;
                }

                // finally if the token is valid and credentials have been supplied then make the call to the Epicor API
                if (!string.IsNullOrEmpty(credentials))
                {
                    using (var client = new HttpClient())
                    {
                        // Note - once Epicor have an API endpoint that fits the data model requested by the add-in then the URL will need to change and the request.context value applied as a filter
                        var url = string.Format(@"{0}{1}'", " https://13.89.62.5/ERP101500/api/v1/BaqSvc/CustomerSalesByFiscalYear?$orderBy=Calculated_FiscalYear$top=5&$filter=CustCnt_EMailAddress eq '", serviceRequest.context);

                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

                        // basic authorization
                        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                        // added to avoid local host ssl certificate errors - should be removed once 'proper' certificate is in place.
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                        // wait for the response
                        response = await client.SendAsync(request);
                    }
                }
            }
            catch (TokenValidationException ex)
            {
                // an error in the token suggests the user does not have authorization to call the API
                response.StatusCode = HttpStatusCode.Unauthorized;
            }
            catch (Exception ex)
            {
                // any other error needs to be handled gracefully and the 'correct' status code applied
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }

        private void ValidateIdentity(AppIdentityToken token)
        {
            for (int i = 0; i < Config.Audience.Length; i++)
            {
                try
                {
                    token.Validate(new Uri(Config.Audience[i]));
                    break;
                }
                catch (TokenValidationException ex)
                {
                    if (i == Config.Audience.Length - 1)
                    {
                        throw (ex);
                    }
                }
            }
        }
    }
}