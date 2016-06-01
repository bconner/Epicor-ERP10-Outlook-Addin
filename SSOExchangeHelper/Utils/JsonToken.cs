﻿//The MIT License (MIT)

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

using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Utils
{
    public class JsonToken
    {
        public bool IsValid;
        public Dictionary<string, string> headerClaims;
        public Dictionary<string, string> payloadClaims;
        public string signature;
        public Dictionary<string, string> appContext;

        private void ValidateHeaderClaim(string key, string value)
        {
            if (!this.headerClaims.ContainsKey(key))
            {
                throw new ApplicationException(String.Format("Header does not contain \"{0}\" claim.", key));
            }

            if (!value.Equals(this.headerClaims[key]))
            {
                throw new ApplicationException(String.Format("\"{0}\" claim must be \"{0}\".", key, value));
            }
        }

        private void ValidateHeader()
        {
            ValidateHeaderClaim("typ", "JWT");
            ValidateHeaderClaim("alg", "RS256");

            if (!this.headerClaims.ContainsKey("x5t"))
            {
                throw new ApplicationException(String.Format("Header does not contain \"{0}\" claim.", "x5t"));
            }
        }
        private void ValidateLifetime()
        {
            if (!this.payloadClaims.ContainsKey("nbf"))
            {
                throw new ApplicationException(
                  String.Format("The \"{0}\" claim is missing from the token.", "nbf"));
            }

            if (!this.payloadClaims.ContainsKey("exp"))
            {
                throw new ApplicationException(
                  String.Format("The \"{0}\" claim is missing from the token.", "exp"));
            }

            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            TimeSpan padding = new TimeSpan(0, 5, 0);

            DateTime validFrom = unixEpoch.AddSeconds(int.Parse(this.payloadClaims["nbf"]));
            DateTime validTo = unixEpoch.AddSeconds(int.Parse(this.payloadClaims["exp"]));

            DateTime now = DateTime.UtcNow;

            if (now < (validFrom - padding))
            {
                throw new ApplicationException(String.Format("The token is not valid until {0}.", validFrom));
            }

            if (now > (validTo + padding))
            {
                throw new ApplicationException(String.Format("The token is not valid after {0}.", validFrom));
            }
        }
        private void ValidateMetadataLocation()
        {
            if (!this.appContext.ContainsKey("amurl"))
            {
                throw new ApplicationException(String.Format("The \"{0}\" claim is missing from the token.", "amurl"));
            }
        }



        private void ValidateAudience()
        {
            if (!this.payloadClaims.ContainsKey("aud"))
            {
                throw new ApplicationException(String.Format("The \"{0}\" claim is missing from the application context.", "aud"));
            }

        }



        public JsonToken(Dictionary<string, string> header, Dictionary<string, string> payload, string signature)
        {

            // Assume that the token is invalid to start out.
            this.IsValid = false;

            // Set the private dictionaries that contain the claims.
            this.headerClaims = header;
            this.payloadClaims = payload;
            this.signature = signature;

            // If there is no "appctx" claim in the token, throw an ApplicationException.
            if (!this.payloadClaims.ContainsKey("appctx"))
            {
                throw new ApplicationException(String.Format("The {0} claim is not present.", "appctx"));
            }

            appContext = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(payload["appctx"]);


            // Validate the header fields.
            this.ValidateHeader();

            // Determine whether the token is within its valid time.
            this.ValidateLifetime();

            // Validate that the token was sent to the correct URL.
            this.ValidateAudience();

            // Make sure that the appctx contains an authentication
            // metadata location.
            this.ValidateMetadataLocation();

            // If the token passes all the validation checks, we
            // can assume that it is valid.
            this.IsValid = true;
        }

    }
}
