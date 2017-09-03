using System;
using System.Net;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WindowsUniversalApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private HttpListener listener;

        public Uri Url { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            int port = 18081;

            listener = new HttpListener(IPAddress.Any, port);
            listener.Request += requestHandler;
            listener.Start();

            bool isListening = listener.IsListening;
        }

        private async void requestHandler(object sender, HttpListenerRequestEventArgs e)
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
                var name = data["name"];

                var content = $"<h2>Hi, {name}! Nice to meet you.</h2>";

                await response.WriteContentAsync(MakeDocument(content));

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    var dialog = new MessageDialog($"Hi, {name}! Nice to meet you.");
                    await dialog.ShowAsync();
                });
            }
            else
            {
                response.MethodNotAllowed();
            }

            response.Close();
        }

        private string MakeDocument(object content)
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

        ~MainPage()
        {
            listener.Close();
        }
    }
}