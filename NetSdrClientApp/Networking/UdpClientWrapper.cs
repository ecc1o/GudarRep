using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetSdrClientApp.Networking
{
    public class UdpClientWrapper : BaseClient, IUdpClient
    {
        private readonly IPEndPoint _localEndPoint;
        private UdpClient? _udpClient;


        public UdpClientWrapper(int port)
        {
            int validPort = port < 0 ? 0 : port;
            _localEndPoint = new IPEndPoint(IPAddress.Any, validPort);
        }

        public async Task StartListeningAsync()
        {
            _udpClient = new UdpClient(_localEndPoint);

            await RunListenerLoop(async (token) =>
            {
                var result = await _udpClient.ReceiveAsync(token);
                OnMessageReceived(result.Buffer);
            });
        }

        public void Exit()
        {
            CancelToken();
            _udpClient?.Close();
            _udpClient = null;
        }
    }
}
