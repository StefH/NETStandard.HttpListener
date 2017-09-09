using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace System.Net.Http
{
    public sealed class HttpListenerRequest
    {
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        private readonly TcpClientAdapter _client;

        internal HttpListenerRequest(TcpClientAdapter client)
        {
            _client = client;

            Headers = new HttpListenerRequestHeaders(this);
        }

        internal async Task ProcessAsync()
        {
            var reader = new StreamReader(_client.GetInputStream());

            StringBuilder request = await ReadRequest(reader);

            var localEndpoint = _client.LocalEndPoint;
            var remoteEnpoint = _client.RemoteEndPoint;

            // TODO : This code needs to be rewritten and simplified.
            string[] requestLines = request.ToString().Split(CharConstants.NL);
            string requestMethod = requestLines[0].TrimEnd(CharConstants.CR);
            string[] requestParts = requestMethod.Split(CharConstants.Space);

            LocalEndpoint = localEndpoint;
            RemoteEndpoint = remoteEnpoint;

            string[] lines = request.ToString().Split(new[] { CharConstants.CR, CharConstants.NL }, StringSplitOptions.RemoveEmptyEntries);

            ParseHeaders(lines);
            ParseRequestLine(lines);

            await PrepareInputStream(reader);
        }

        private void ParseRequestLine(string[] lines)
        {
            var line = lines.ElementAt(0).Split(new[] { CharConstants.Space }, StringSplitOptions.RemoveEmptyEntries);

            var url = new UriBuilder(Headers.Host + line[1]).Uri;
            var httpMethod = line[0];

            Version = line[2];
            HttpMethod = httpMethod;
            Url = url;
        }

        private async Task PrepareInputStream(StreamReader reader)
        {
            if (HttpMethods.CanHaveContent(HttpMethod))
            {
                int contentLength = (int)Headers.ContentLength;

                if (contentLength > 0)
                {
                    char[] buffer = new char[contentLength];

                    int bytesRead = await reader.ReadBlockAsync(buffer, 0, contentLength);

                    InputStream = new MemoryStream(DefaultEncoding.GetBytes(buffer));
                }
            }
        }

        private void ParseHeaders(IEnumerable<string> lines)
        {
            lines = lines.Skip(1);
            Headers.ParseHeaderLines(lines);
        }

        private static async Task<StringBuilder> ReadRequest(StreamReader reader)
        {
            var request = new StringBuilder();

            string line;
            while ((line = await reader.ReadLineAsync()) != string.Empty && line != null)
            {
                request.AppendLine(line);
            }

            // var requestStr = request.ToString();
            return request;
        }

        /// <summary>
        /// Gets the endpoint of the listener that received the request.
        /// </summary>
        [PublicAPI]
        public IPEndPoint LocalEndpoint { get; private set; }

        /// <summary>
        /// Gets the endpoint that sent the request.
        /// </summary>
        [PublicAPI]
        public IPEndPoint RemoteEndpoint { get; private set; }

        /// <summary>
        /// Gets the URI send with the request.
        /// </summary>
        [PublicAPI]
        public Uri Url { get; private set; }

        /// <summary>
        /// Gets the HTTP method.
        /// </summary>
        [PublicAPI]
        public string HttpMethod { get; private set; }

        /// <summary>
        /// Gets the headers of the HTTP request.
        /// </summary>
        [PublicAPI]
        public HttpListenerRequestHeaders Headers { get; }

        /// <summary>
        /// Gets the stream containing the content sent with the request.
        /// </summary>
        [PublicAPI]
        public Stream InputStream { get; private set; }

        /// <summary>
        /// Gets the HTTP version.
        /// </summary>
        [PublicAPI]
        public string Version { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the request was sent locally or not.
        /// </summary>
        [PublicAPI]
        public bool IsLocal => RemoteEndpoint.Address.Equals(LocalEndpoint.Address);

        /// <summary>
        /// Reads the content of the request as a string.
        /// </summary>
        [PublicAPI]
        public async Task<string> ReadContentAsStringAsync()
        {
            if (InputStream == null)
            {
                return null;
            }

            long length = InputStream.Length;
            byte[] buffer = new byte[length];

            await InputStream.ReadAsync(buffer, 0, (int)length);
            return DefaultEncoding.GetString(buffer);
        }
    }
}