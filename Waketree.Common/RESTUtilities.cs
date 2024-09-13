using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Waketree.Common
{
    public static class RESTUtilities
    {
        private static HttpClient? httpClient = null;

        public static async Task<Tuple<bool, HttpStatusCode, T?>> GetCallRestServiceAsync<T>(string apiUrl)
        {
            var json = string.Empty;
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            bool isSuccessStatusCode = false;

            if (RESTUtilities.httpClient == null)
            {
                RESTUtilities.httpClient = new HttpClient();
            }

            try
            {
                using (var response = await RESTUtilities.httpClient.GetAsync(apiUrl))
                {
                    json = await response.Content.ReadAsStringAsync();
                    statusCode = response.StatusCode;
                    isSuccessStatusCode = response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return new Tuple<bool, HttpStatusCode, T?>(false,
                    HttpStatusCode.BadRequest,
                    default(T?));
            }

            if (isSuccessStatusCode)
            {
                try
                {
                    return new Tuple<bool, HttpStatusCode, T?>(isSuccessStatusCode,
                        statusCode,
                        JsonConvert.DeserializeObject<T?>(json));
                }
                catch
                {
                    return new Tuple<bool, HttpStatusCode, T?>(false,
                        HttpStatusCode.InternalServerError,
                        default(T?));
                }
            }
            return new Tuple<bool, HttpStatusCode, T?>(false,
                HttpStatusCode.BadRequest,
                default(T?));
        }

        public static async Task<Tuple<bool, HttpStatusCode, T?>> PostCallRestServiceAsync<T, U>(string apiUrl, U postData)
        {
            var json = string.Empty;
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            bool isSuccessStatusCode = false;
            var postDataJson = JsonConvert.SerializeObject(postData);
            var httpContent = new StringContent(
                postDataJson, 
                Encoding.UTF8, 
                "application/json");

            if (RESTUtilities.httpClient == null)
            {
                RESTUtilities.httpClient = new HttpClient();
            }

            try
            {
                using (var response = await RESTUtilities.httpClient.PostAsync(apiUrl, httpContent))
                {
                    json = await response.Content.ReadAsStringAsync();
                    statusCode = response.StatusCode;
                    isSuccessStatusCode = response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return new Tuple<bool, HttpStatusCode, T?>(false,
                    HttpStatusCode.BadRequest,
                    default(T?));
            }

            if (isSuccessStatusCode)
            {
                try
                {
                    return new Tuple<bool, HttpStatusCode, T?>(isSuccessStatusCode,
                        statusCode,
                        JsonConvert.DeserializeObject<T?>(json));
                }
                catch
                {
                    return new Tuple<bool, HttpStatusCode, T?>(false,
                        HttpStatusCode.InternalServerError,
                        default(T?));
                }
            }
            return new Tuple<bool, HttpStatusCode, T?>(false,
                HttpStatusCode.BadRequest,
                default(T?));
        }
    }
}
