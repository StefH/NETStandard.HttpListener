namespace System.Net.Http
{
    public static class HttpMethods
    {
        public static readonly string Get = "GET";
        public static readonly string Post = "POST";
        public static readonly string Put = "PUT";
        public static readonly string Patch = "PATCH";
        public static readonly string Delete = "DELETE";
        public static readonly string Copy = "COPY";
        public static readonly string Head = "HEAD";
        public static readonly string Options = "OPTIONS";
        public static readonly string Link = "LINK";
        public static readonly string Unlink = "UNLINK";
        public static readonly string Purge = "PURGE";
        public static readonly string Lock = "LOCK";
        public static readonly string Unlock = "UNLOCK";
        public static readonly string Propfind = "PROPFIND";
        public static readonly string View = "VIEW";
        public static readonly string Trace = "TRACE";
        public static readonly string Connect = "CONNECT";

        public static bool CanHaveContent(string method)
            => method != Get
            && method != Copy
            && method != Head
            && method != Purge
            && method != Trace
            && method != Connect;
    }
}
