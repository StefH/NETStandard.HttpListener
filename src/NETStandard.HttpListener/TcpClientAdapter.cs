using System.IO;
using System.Net.Sockets;

namespace System.Net.Http
{
    class TcpClientAdapter
    {
        private readonly TcpClient _tcpClient;

        public TcpClientAdapter(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;

            LocalEndPoint = (IPEndPoint)tcpClient.Client.LocalEndPoint;
            RemoteEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
        }

        public IPEndPoint LocalEndPoint
        {
            get;
            private set;
        }

        public IPEndPoint RemoteEndPoint
        {
            get;
            private set;
        }

        public Stream GetInputStream()
        {
            return _tcpClient.GetStream();
        }

        public Stream GetOutputStream()
        {
            return _tcpClient.GetStream();
        }

        public void Dispose()
        {
            _tcpClient.Dispose();
        }
    }
}