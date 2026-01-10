using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class EchoServer
{
    private readonly int _port;
    private TcpListener _listener;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public EchoServer(int port)
    {
        _port = port;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public async Task StartAsync()
    {
        _listener = new TcpListener(IPAddress.Any, _port);
        _listener.Start();
        Console.WriteLine($"Server started on port {_port}.");

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected.");

                _ = Task.Run(async () => 
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        await HandleClientAsync(stream, _cancellationTokenSource.Token);
                    }
                    client.Close();
                    Console.WriteLine("Client disconnected.");
                });
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Accept error: {ex.Message}");
            }
        }
        Console.WriteLine("Server shutdown.");
    }

    public async Task HandleClientAsync(Stream stream, CancellationToken token)
    {
        try
        {
            byte[] buffer = new byte[8192];
            int bytesRead;

            while (!token.IsCancellationRequested && 
                   (bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
            {
                await stream.WriteAsync(buffer, 0, bytesRead, token);
                Console.WriteLine($"Echoed {bytesRead} bytes.");
            }
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        _listener.Stop();
        _cancellationTokenSource.Dispose();
        Console.WriteLine("Server stopped.");
    }
}
