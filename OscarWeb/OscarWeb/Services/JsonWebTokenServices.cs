using System;
using System.Collections.Generic;

using JWT;
using JWT.Algorithms;
using JWT.Serializers;

using OscarWeb.Constants;

namespace OscarWeb.Services
{
    /// <summary>
    /// Class for creating JSON web tokens which are required to consume any of the Web APIs
    /// </summary>
    public static class JsonWebTokenServices
    {
        /// <summary>
        /// Creates a JSON web token
        /// </summary>
        /// <returns></returns>
        public static string CreateJsonWebtoken()
        {
            return JsonWebTokenServices.CreateToken(DateTime.UtcNow);
        }

        /// <summary>
        /// Creates a JSON web token - used for unit testing by allowing us to inject a Datetime into the method.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string CreateJsonWebtoken(DateTime dt)
        {
            return JsonWebTokenServices.CreateToken(dt);
        }
        
        private static string CreateToken(DateTime dt)
        {
            double timestamp = (dt - new DateTime(1970, 1, 1)).TotalSeconds;
            Dictionary<string, object> payload = new Dictionary<string, object>()
            {
                {"iat", timestamp},
                {"subscriber", WebServiceConstants.Subscriber}
            };
            var encoder = new JwtEncoder(new HMACSHA256Algorithm(), new JsonNetSerializer(), new JwtBase64UrlEncoder());
            string token = encoder.Encode(payload, WebServiceConstants.PrivateKey);
            return token;
        }
    }
}
