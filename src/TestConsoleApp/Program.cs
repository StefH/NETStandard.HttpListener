using System;
using System.Net;
using System.Net.Http;

namespace TestConsoleApp
{
    public class Program
    {
        private static void DemoCode()
        {
            var listener = new HttpListener(IPAddress.Parse("127.0.0.1"), 8081);
            try
            {
                listener.Request += async (sender, context) =>
                {
                    var request = context.Request;
                    var response = context.Response;
                    if (request.HttpMethod == HttpMethods.Get)
                    {
                        await response.WriteContentAsync($"Hello from Server at: {DateTime.Now}\r\n");
                    }
                    else
                    {
                        response.MethodNotAllowed();
                    }
                    // Close the HttpResponse to send it back to the client.
                    response.Close();
                };
                listener.Start();

                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }
            finally
            {
                listener.Close();
            }
        }

        public static void Main(string[] args)
        {
            int port = 18081;

            var listener = new HttpListener(IPAddress.Any, port);
            listener.Request += HandleRequest;
            listener.Start();

            Console.WriteLine($"Start an internet browser and navigate to http://localhost:{port}");

            Console.WriteLine("Press any key to stop listener");

            Console.ReadKey();
            listener.Close();
            listener.Dispose();
        }

        private static async void HandleRequest(object sender, HttpListenerRequestEventArgs e)
        {
            var request = e.Request;
            var response = e.Response;

            if (request.HttpMethod == HttpMethods.Get)
            {
                string content = @"<h2>Hello! What's your name?</h2>
                                <form method=""POST"" action=""/?test=2"">
                                    <input name=""name""></input>
                                    <button type=""submit"">Send</button>
                                </form>";

                await response.WriteContentAsync(MakeDocument(content));
            }
            else if (request.HttpMethod == HttpMethods.Post)
            {
                var param = request.Url.ParseQueryParameters();

                var data = await request.ReadUrlEncodedContentAsync();

                if (data != null && data.ContainsKey("name"))
                {
                    string name = data["name"];

                    string content = $"<h2>Hi, {name}! Nice to meet you.</h2>";

                    await response.WriteContentAsync(MakeDocument(content));

                    Console.WriteLine($"--> Hi, {name}! Nice to meet you.");
                }
                else
                {
                    await response.WriteContentAsync(MakeDocument("Please provide name."));

                    Console.WriteLine($"--> Please provide name.");
                }
            }
            else
            {
                response.MethodNotAllowed();
            }

            response.Close();
        }

        private static string MakeDocument(object content)
        {
            return @"<html>
                        <head>
                            <title>Test</title>
                        </head>
                        <body>" +
                            content +
                        @"</body>
                    </html>";
        }
    }
}
