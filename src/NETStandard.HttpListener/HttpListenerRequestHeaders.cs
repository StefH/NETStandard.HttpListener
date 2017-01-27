using JetBrains.Annotations;

namespace System.Net.Http
{
    public sealed class HttpListenerRequestHeaders : HttpListenerHeaders
    {
        private HttpListenerHeaderValueCollection<string> _accept;
        private HttpListenerHeaderValueCollection<string> _acceptCharset;
        private HttpListenerHeaderValueCollection<string> _acceptLanguage;
        private HttpListenerHeaderValueCollection<string> _acceptEncoding;
        private DateTime _acceptDatetime;
        private string _host;

        public HttpListenerRequestHeaders(HttpListenerRequest request)
        {
            Request = request;
        }

        public string Host
        {
            get
            {
                if (_host == null)
                {
                    string hostString;
                    if (TryGetValue("Host", out hostString))
                    {
                        _host = hostString;
                    }
                }
                return _host;
            }
        }

        #region Accept Headers
        [PublicAPI]
        public HttpListenerHeaderValueCollection<string> Accept => _accept ?? (_accept = new HttpListenerHeaderValueCollection<string>(this, "Accept"));

        public HttpListenerHeaderValueCollection<string> AcceptEncoding => _acceptEncoding ?? (_acceptEncoding = new HttpListenerHeaderValueCollection<string>(this, "Accept-Encoding"));

        [PublicAPI]
        public HttpListenerHeaderValueCollection<string> AcceptCharset => _acceptCharset ?? (_acceptCharset = new HttpListenerHeaderValueCollection<string>(this, "Accept-Charset"));

        [PublicAPI]
        public HttpListenerHeaderValueCollection<string> AcceptLanguage => _acceptLanguage ?? (_acceptLanguage = new HttpListenerHeaderValueCollection<string>(this, "Accept-Language"));

        [PublicAPI]
        public DateTime AcceptDateTime
        {
            get
            {
                if (_acceptDatetime == default(DateTime))
                {
                    string headerValue;
                    if(TryGetValue("Accept-Datetime", out headerValue))
                    {
                        _acceptDatetime = DateTime.Parse(headerValue);
                    }
                }
                return _acceptDatetime;
            }
        }

        private HttpListenerRequest Request { get; }

        #endregion
    }
}