using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace System.Net.Http
{
    public class HttpListenerHeaders : Dictionary<string, string>
    {
        private HttpListenerHeaderValueCollection<string> _contentType;
        private HttpListenerHeaderValueCollection<string> _connection;
        private string _contentMd5;
        private bool? _isRequestHeaders;

        private HttpListenerResponse _response;

        public HttpListenerHeaders() : base(StringComparer.OrdinalIgnoreCase)
        { }

        internal void ParseHeaderLines(IEnumerable<string> lines)
        {
            foreach (var headerLine in lines)
            {
                var parts = headerLine.Split(new char[] { ':' }, 2);
                var key = parts[0];
                var value = parts[1].Trim();
                Add(key, value);
            }
        }

        private string MakeHeaderString()
        {
            var sb = new StringBuilder();
            foreach (var header in this)
            {
                sb.Append($"{header.Key}: {header.Value}\r\n");
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return MakeHeaderString();
        }

        [PublicAPI]
        public HttpListenerHeaderValueCollection<string> Connection => _connection ?? (_connection = new HttpListenerHeaderValueCollection<string>(this, "Connection"));

        #region Content Headers

        public HttpListenerHeaderValueCollection<string> ContentType => _contentType ?? (_contentType = new HttpListenerHeaderValueCollection<string>(this, "Content-Type"));

        public long ContentLength
        {
            get
            {
                if (IsRequestHeaders)
                {
                    string headerValue;
                    if (TryGetValue("Content-Length", out headerValue))
                    {
                        return int.Parse(headerValue);
                    }
                    return 0;
                }

                var response = GetResponse();
                return response.OutputStream.Length;
            }
        }

        [PublicAPI]
        public string ContentMd5
        {
            get
            {
                if (_contentMd5 == null)
                {
                    if (TryGetValue("Content-MD5", out _contentMd5))
                    {
                        return _contentMd5;
                    }
                }
                return null;
            }

            #endregion
        }

        private bool IsRequestHeaders
        {
            get
            {
                if (_isRequestHeaders == null)
                {
                    _isRequestHeaders = this is HttpListenerRequestHeaders;
                }
                return _isRequestHeaders.GetValueOrDefault();
            }
        }

        private HttpListenerResponse GetResponse()
        {
            if (_response == null)
            {
                var headers = this as HttpListenerResponseHeaders;
                _response = headers.Response;
            }
            return _response;
        }
    }
}
