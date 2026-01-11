namespace EchoTcpServer;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        EchoServer server = new EchoServer(5000);

        // Start the server
        _ = Task.Run(() => server.StartAsync());

        string host = "127.0.0.1"; 
        int port = 60000;          
        int intervalMilliseconds = 3000;

        using (var sender = new UdpTimedSender(host, port))
        {
            Console.WriteLine("Press 'q' to quit...");
            sender.StartSending(intervalMilliseconds);

            while (Console.ReadKey(intercept: true).Key != ConsoleKey.Q)
            {
                // Wait for Q
            }

            sender.StopSending();
            server.Stop();
        }
    }
}

public class UdpTimedSender : IDisposable
{
    private readonly string _host;
    private readonly int _port;
    private readonly UdpClient _udpClient;
    private Timer? _timer;

    public UdpTimedSender(string host, int port)
    {
        _host = host;
        _port = port;
        _udpClient = new UdpClient();
    }

    public void StartSending(int intervalMilliseconds)
    {
        if (_timer != null) return;
        _timer = new Timer(SendMessageCallback, null, 0, intervalMilliseconds);
    }

    private void SendMessageCallback(object? state)
    {
        try {
            byte[] msg = Encoding.UTF8.GetBytes("Ping");
            var endpoint = new IPEndPoint(IPAddress.Parse(_host), _port);
            _udpClient.Send(msg, msg.Length, endpoint);
            Console.WriteLine($"UDP Ping sent.");
        } catch { }
    }

    public void StopSending() { _timer?.Dispose(); _timer = null; }
    public void Dispose() { StopSending(); _udpClient.Dispose(); }
}
