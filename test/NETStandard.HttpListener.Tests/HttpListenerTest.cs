using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NETStandard.HttpListener.Tests
{
    public class HttpListenerTest
    {
        private string address;
        private Uri url;

        private System.Net.Http.HttpListener StartHttpListener(int port, EventHandler<HttpListenerRequestEventArgs> requestHandler = null)
        {
            address = "127.0.0.1";
            url = (new UriBuilder($"http://{address}:{port}/test")).Uri;

            var listener = new System.Net.Http.HttpListener(IPAddress.Parse(address), port);
            if (requestHandler != null)
            {
                listener.Request += requestHandler;
            }
            listener.Start();

            return listener;
        }

        [Fact(DisplayName = "GET Request")]
        public async Task GetRequest()
        {
            var listener = StartHttpListener(8081, async (sender, e) =>
            {
                var request = e.Request;
                var response = e.Response;

                var x = request.Headers.AcceptEncoding;

                response.Headers.ContentType.Add("application/text");

                var bytes = Encoding.UTF8.GetBytes("Hello World!");
                await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                await response.OutputStream.FlushAsync();

                response.Close();
            });

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
            }

            listener.Close();
        }

        [Fact(DisplayName = "POST Request")]
        public async Task PostRequest()
        {
            var listener = StartHttpListener(8082, async (sender, e) =>
            {
                var request = e.Request;

                string content;
                using (var streamReader = new StreamReader(request.InputStream))
                {
                    content = await streamReader.ReadToEndAsync();
                }

                var response = e.Response;

                response.Headers.ContentType.Add("text");

                var bytes = Encoding.UTF8.GetBytes(content);
                await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                await response.OutputStream.FlushAsync();

                response.Close();
            });

            using (var client = new HttpClient())
            {
                var requestContent = new StringContent("Hey", Encoding.UTF8);
                var response = await client.PostAsync(url, requestContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                Assert.Equal(responseContent, "Hey");
            }

            listener.Close();
        }

        [Fact]
        public async Task PostRequest2()
        {
            var listener = StartHttpListener(8083, async (sender, e) =>
            {
                var request = e.Request;

                string content;
                using (var streamReader = new StreamReader(request.InputStream))
                {
                    content = await streamReader.ReadToEndAsync();
                }

                var response = e.Response;

                var bytes = Encoding.UTF8.GetBytes(content);
                await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                await response.OutputStream.FlushAsync();

                response.Close();
            });

            var webRequest = WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.Headers[HttpRequestHeader.ContentLength] = "3";

            Stream dataStream = await webRequest.GetRequestStreamAsync();
            dataStream.Write(Encoding.ASCII.GetBytes("01"), 0, 2);
            dataStream.Flush();
            
            byte[] buffer = new byte[3];
            webRequest.GetResponseAsync().ContinueWith(tc =>
            {
                WebResponse webResponse = tc.Result;
                using (Stream data = webResponse.GetResponseStream())
                {
                    data.Read(buffer, 0, 3);
                }
            }).Wait(1000);

            // No data is returned because the client aborts
            Assert.Equal(new byte[] { 0, 0, 0 }, buffer);

            listener.Close();
        }

        [Fact(Skip = "Not functional")]
        public async Task Request()
        {
            var port = 8083;
            var listener = StartHttpListener(port);
            //listener.Get(new Uri("/test", UriKind.Relative), async (request, response) =>
            //{
            //    await response.WriteAsync($"Hello from server at: {DateTime.Now}\r\n");

            //    //await Task.Delay(10000);
            //});

            using (var client = new HttpClient())
            {
                try
                {
                    //client.Timeout = TimeSpan.FromSeconds(2);

                    //var url = (new UriBuilder($"http://{address}:{port}/foo")).Uri;

                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();
                }
                catch (Exception exc)
                {

                }
            }

            listener.Close();
        }
    }
}
