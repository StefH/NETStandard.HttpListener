namespace System.Net.Http
{
    /// <summary>
    /// Source: http://stackoverflow.com/questions/20268544/portable-class-library-pcl-version-of-httputility-parsequerystring 
    /// </summary>
    internal static class HttpUtility
    {
        public static string UrlEncode(string s)
        {
            string result;
            while ((result = Uri.EscapeDataString(s)) != s)
                s = result;
            return result;
        }

        public static string UrlDecode(string s)
        {
            string result;
            while ((result = Uri.UnescapeDataString(s)) != s)
                s = result;
            return result;
        }

        public static HttpValueCollection ParseQueryString(string query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (query.Length > 0 && query[0] == '?')
            {
                query = query.Substring(1);
            }

            return new HttpValueCollection(query, true);
        }
    }
}