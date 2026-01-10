using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetSdrClientApp.Networking
{
    public class TcpClientWrapper : BaseClient, ITcpClient
    {
        private readonly string _host;
        private readonly int _port;
        private TcpClient? _tcpClient;
        private NetworkStream? _stream;

        public bool Connected => _tcpClient != null && _tcpClient.Connected && _stream != null;

        public TcpClientWrapper(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public void Connect()
        {
            if (Connected) return;

            _tcpClient = new TcpClient();
            try
            {
                _tcpClient.Connect(_host, _port);
                _stream = _tcpClient.GetStream();
                // Console.WriteLine removed
                _ = StartListeningAsync();
            }
            catch (Exception)
            {
                // Console.WriteLine removed
            }
        }

        public void Disconnect()
        {
            if (Connected)
            {
                CancelToken();
                _stream?.Close();
                _tcpClient?.Close();
                _tcpClient = null;
                _stream = null;
                // Console.WriteLine removed
            }
        }

        public async Task SendMessageAsync(byte[] data)
        {
            if (Connected && _stream != null && _stream.CanWrite)
            {
                // Console.WriteLine removed
                await _stream.WriteAsync(data, 0, data.Length);
            }
            else
            {
                throw new InvalidOperationException("Not connected to a server.");
            }
        }

        public async Task SendMessageAsync(string str)
        {
            var data = Encoding.UTF8.GetBytes(str);
            await SendMessageAsync(data);
        }

        private async Task StartListeningAsync()
        {
            if (Connected && _stream != null && _stream.CanRead)
            {
                await RunListenerLoop(async (token) =>
                {
                    byte[] buffer = new byte[8194];
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);
                    if (bytesRead > 0)
                    {
                        OnMessageReceived(buffer.AsSpan(0, bytesRead).ToArray());
                    }
                });
            }
            else
            {
                throw new InvalidOperationException("Not connected to a server.");
            }
        }
    }
}
