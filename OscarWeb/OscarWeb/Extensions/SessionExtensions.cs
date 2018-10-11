using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace OscarWeb.Extensions
{
    /// <summary>
    /// Extensions for handling Session state. Implements (de)serialisation of objects that are
    /// are saved / retrieved from session storage. 
    /// </summary>
    public static  class SessionExtensions
    {
        /// <summary>
        /// Set a value to session storage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        } 

        /// <summary>
        /// Get a value from session storage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) :
                JsonConvert.DeserializeObject<T>(value);
        }
    }
}
