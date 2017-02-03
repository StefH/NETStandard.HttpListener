﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace System.Net.Http
{
    public sealed class HttpListenerRequest
    {
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
            var requestLines = request.ToString().Split('\n');
            string requestMethod = requestLines[0].TrimEnd('\r');
            string[] requestParts = requestMethod.Split(' ');

            LocalEndpoint = localEndpoint;
            RemoteEndpoint = remoteEnpoint;

            string[] lines = request.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            ParseHeaders(lines);
            ParseRequestLine(lines);

            await PrepareInputStream(reader);
        }

        private void ParseRequestLine(string[] lines)
        {
            var line = lines.ElementAt(0).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var url = new UriBuilder(Headers.Host + line[1]).Uri;
            var httpMethod = line[0];

            Version = line[2];
            Method = httpMethod;
            RequestUri = url;
        }

        private async Task PrepareInputStream(StreamReader reader)
        {
            if (Method == HttpMethods.Post || Method == HttpMethods.Put || Method == HttpMethods.Patch)
            {
                Encoding encoding = Encoding.UTF8;

                var contentLength = (int)Headers.ContentLength;

                char[] buffer = new char[contentLength];

                await reader.ReadBlockAsync(buffer, 0, contentLength);

                InputStream = new MemoryStream(encoding.GetBytes(buffer));
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
            while ((line = await reader.ReadLineAsync()) != "")
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
        public Uri RequestUri { get; private set; }

        /// <summary>
        /// Gets the HTTP method.
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// Gets the headers of the HTTP request.
        /// </summary>
        public HttpListenerRequestHeaders Headers { get; }

        /// <summary>
        /// Gets the stream containing the content sent with the request.
        /// </summary>
        public Stream InputStream { get; private set; }

        /// <summary>
        /// Gets the HTTP version.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the request was sent locally or not.
        /// </summary>
        [PublicAPI]
        public bool IsLocal => RemoteEndpoint.Address.Equals(LocalEndpoint.Address);

        /// <summary>
        /// Reads the content of the request as a string.
        /// </summary>
        /// <returns></returns>
        public async Task<string> ReadContentAsStringAsync()
        {
            var length = InputStream.Length;
            byte[] buffer = new byte[length];
            await InputStream.ReadAsync(buffer, 0, (int)length);
            return Encoding.UTF8.GetString(buffer);
        }
    }
}