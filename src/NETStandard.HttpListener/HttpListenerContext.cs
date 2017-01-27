using JetBrains.Annotations;

namespace System.Net.Http
{
    public class HttpListenerContext
    {
        public HttpListenerContext(HttpListenerRequest request, HttpListenerResponse response)
        {
            Request = request;
            Response = response;
        }

        [PublicAPI]
        public HttpListenerRequest Request { get; }

        [PublicAPI]
        public HttpListenerResponse Response { get; }
    }
}