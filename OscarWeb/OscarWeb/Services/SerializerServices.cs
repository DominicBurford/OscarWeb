using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace OscarWeb.Services
{
    public class SerializerServices
    {
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Deserialise the specified JSON string to the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonObject">JSON representation of the object to deserialize</param>
        /// <returns></returns>
        public T DeserializeObject<T>(string jsonObject)
        {
            T result = default(T);
            if (!string.IsNullOrEmpty(jsonObject))
            {
                result = JsonConvert.DeserializeObject<T>(jsonObject);
            }
            return result;
        }

        /// <summary>
        /// Serialize the object and handle date formates. The JavascriptSerializer() doesn't handle
        /// .NET DateTimes correctly so need to use JsonConvert instead
        /// </summary>
        /// <param name="objtoserialize">The object to serialize</param>
        /// <returns>JSON representation of the object</returns>
        public string SerializeObject(object objtoserialize)
        {
            var isoConvert = new IsoDateTimeConverter { DateTimeFormat = SerializerServices.DateFormat };
            return JsonConvert.SerializeObject(objtoserialize, isoConvert);
        }
    }
}
