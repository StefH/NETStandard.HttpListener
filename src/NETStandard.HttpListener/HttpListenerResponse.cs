using System.IO;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using System.Diagnostics;

namespace System.Net.Http
{
    public sealed class HttpListenerResponse : IDisposable
    {
        private readonly TcpClientAdapter _client;

        internal HttpListenerResponse(HttpListenerRequest request, TcpClientAdapter client)
        {
            Headers = new HttpListenerResponseHeaders(this);

            _client = client;
            Request = request;
        }

        internal void Initialize()
        {
            OutputStream = new MemoryStream();

            Version = Request.Version;
            StatusCode = 200;
            ReasonPhrase = "OK";
        }

        HttpListenerRequest Request { get; set; }

        /// <summary>
        /// Gets the headers of the HTTP response.
        /// </summary>
        public HttpListenerResponseHeaders Headers { get; }

        /// <summary>
        /// Gets the stream containing the content of this response.
        /// </summary>
        public Stream OutputStream { get; private set; }

        /// <summary>
        /// Gets or sets the HTTP version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the HTTP reason phrase.
        /// </summary>
        public string ReasonPhrase { get; set; }

        private async Task SendMessage()
        {
                var outputStream = OutputStream as MemoryStream;
                outputStream.Seek(0, SeekOrigin.Begin);

                var socketStream = _client.GetOutputStream();

                string header = $"{Version} {StatusCode} {ReasonPhrase}\r\n" +
                                Headers +
                                $"Content-Length: {outputStream.Length}\r\n" +
                                "\r\n";
                byte[] headerArray = Encoding.UTF8.GetBytes(header);

                await socketStream.WriteAsync(headerArray, 0, headerArray.Length);
            try
            {
                await outputStream.CopyToAsync(socketStream);
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                await socketStream.FlushAsync();
            }
        }

        /// <summary>
        /// Writes a string to OutputStream.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Task WriteContentAsync(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            return OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Closes this response and sends it.
        /// </summary>
        public async void Close()
        {
            await SendMessage();
            CloseSocket();
        }

        internal void CloseSocket()
        {
            _client.Dispose();
        }

        /// <summary>
        /// Writes a HTTP redirect response.
        /// </summary>
        /// <param name="redirectLocation"></param>
        /// <returns></returns>
        [PublicAPI]
        public async Task RedirectAsync(Uri redirectLocation)
        {
            var outputStream = _client.GetOutputStream();

            StatusCode = 301;
            ReasonPhrase = "Moved permanently";
            Headers.Location = redirectLocation;

            string header = $"{Version} {StatusCode} {ReasonPhrase}\r\n" +
                            $"Location: {Headers.Location}" +
                            "Content-Length: 0\r\n" +
                            "Connection: close\r\n" +
                            "\r\n";

            byte[] headerArray = Encoding.UTF8.GetBytes(header);
            await outputStream.WriteAsync(headerArray, 0, headerArray.Length);
            await outputStream.FlushAsync();

        }

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                Close();

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~HttpListenerResponse() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
