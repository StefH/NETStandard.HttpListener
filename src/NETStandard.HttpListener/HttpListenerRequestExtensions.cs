﻿using System.Collections.Generic;
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
            var data = HttpUtility.ParseQueryString(query);

            var values = new Dictionary<string, string>();

            foreach (var valuePair in data)
            {
                var value = HttpUtility.UrlDecode(valuePair.Value);
                values[valuePair.Key] = valuePair.Value;
            }

            return values;
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

            var data = HttpUtility.ParseQueryString(content);

            var values = new Dictionary<string, string>();

            foreach (var valuePair in data)
            {
                var value = HttpUtility.UrlDecode(valuePair.Value);
                values[valuePair.Key] = valuePair.Value;
            }

            return values;
        }
    }
}