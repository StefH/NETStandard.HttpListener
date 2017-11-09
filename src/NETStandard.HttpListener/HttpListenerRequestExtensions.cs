using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpListenerRequestExtensions
    {
        /// <summary>
        /// Parses the query parameters.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public static IDictionary<string, string> ParseQueryParameters(this Uri uri)
        {
            var query = uri.Query;
            return ParseToDictionary(query);
        }

        /// <summary>
        /// Reads URL encoded content from the request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<IDictionary<string, string>> ReadUrlEncodedContentAsync(this HttpListenerRequest request)
        {
            string content = await request.ReadContentAsStringAsync();
            if (content == null)
            {
                return null;
            }
            return ParseToDictionary(content);
        }

        private static IDictionary<string, string> ParseToDictionary(string content)
        {
            var data = HttpUtility.ParseQueryString(content);

            var values = new Dictionary<string, string>();

            foreach (var valuePair in data)
            {
                var value = HttpUtility.UrlDecode(valuePair.Value);
                values[valuePair.Key] = value;
            }

            return values;
        }
    }
}
