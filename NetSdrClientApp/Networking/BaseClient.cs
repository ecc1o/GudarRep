using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetSdrClientApp.Networking
{
    public abstract class BaseClient
    {
        protected CancellationTokenSource _cts;

        public event EventHandler<byte[]>? MessageReceived;

        protected void OnMessageReceived(byte[] data)
        {
            MessageReceived?.Invoke(this, data);
        }

      protected async Task RunListenerLoop(Func<CancellationToken, Task> listenAction)
        {
            try
            {
                _cts = new CancellationTokenSource();
                // Console.WriteLine("Starting listening for incoming messages."); // <--- DELETED

                while (!_cts.Token.IsCancellationRequested)
                {
                    await listenAction(_cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error in listening loop: {ex.Message}"); // <--- DELETED
            }
            finally
            {
                // Console.WriteLine("Listener stopped."); // <--- DELETED
            }
        }

        protected void CancelToken()
        {
            _cts?.Cancel();
            _cts = null;
        }
    }
}
