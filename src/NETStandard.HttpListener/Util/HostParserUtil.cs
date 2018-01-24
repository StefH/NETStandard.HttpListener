using System;

namespace NETStandard.HttpListener.Util
{
    public static class HostParserUtil
    {
        public static Uri Parse(string host, string path)
        {
            if (!(host.StartsWith("http://") || host.StartsWith("https://")))
            {
                host = $"http://{host}";
            }

            return new UriBuilder($"{host}{path}").Uri;
        }
    }
}