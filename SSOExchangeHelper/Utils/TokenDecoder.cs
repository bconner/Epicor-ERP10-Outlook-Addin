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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace Utils
{
    public class TokenDecoder
    {
        public static Encoding TextEncoding = Encoding.UTF8;

        private static char Base64PadCharacter = '=';
        private static char Base64Character62 = '+';
        private static char Base64Character63 = '/';
        private static char Base64UrlCharacter62 = '-';
        private static char Base64UrlCharacter63 = '_';

        private static byte[] DecodeBytes(string arg)
        {
            if (String.IsNullOrEmpty(arg))
            {
                throw new ApplicationException("String to decode cannot be null or empty.");
            }

            StringBuilder s = new StringBuilder(arg);
            s.Replace(Base64UrlCharacter62, Base64Character62);
            s.Replace(Base64UrlCharacter63, Base64Character63);

            int pad = s.Length % 4;
            s.Append(Base64PadCharacter, (pad == 0) ? 0 : 4 - pad);

            return Convert.FromBase64String(s.ToString());
        }

        private static string Base64Decode(string arg)
        {
            return TextEncoding.GetString(DecodeBytes(arg));
        }

        public static JsonToken Decode(string rawToken)
        {
            string[] tokenParts = rawToken.Split('.');

            if (tokenParts.Length != 3)
            {
                throw new ApplicationException("Token must have three parts separated by '.' characters.");
            }

            string encodedHeader = tokenParts[0];
            string encodedPayload = tokenParts[1];
            string signature = tokenParts[2];

            string decodedHeader = Base64Decode(encodedHeader);
            string decodedPayload = Base64Decode(encodedPayload);

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            Dictionary<string, string> header = serializer.Deserialize<Dictionary<string, string>>(decodedHeader);
            Dictionary<string, string> payload = serializer.Deserialize<Dictionary<string, string>>(decodedPayload);

            return new JsonToken(header, payload, signature);
        }
    }
}
