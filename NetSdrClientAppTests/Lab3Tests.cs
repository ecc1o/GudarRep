using NUnit.Framework;
using NetSdrClientApp.Networking;
using NetSdrClientApp;
using System.Net;

namespace NetSdrClientAppTests
{
    public class Lab3Tests
    {
        [Test]
        public void TcpClientWrapper_Constructor_ShouldInitializeCorrectly()
        {
            var client = new TcpClientWrapper("127.0.0.1", 8080);
            Assert.IsNotNull(client);
        }

        [Test]
        public void UdpClientWrapper_Constructor_ShouldInitializeCorrectly()
        {
            var client = new UdpClientWrapper(1234);
            Assert.IsNotNull(client);
        }

        [Test]
        public void NetSdrClient_Constructor_ShouldInitializeWithWrappers()
        {
            var tcp = new TcpClientWrapper("127.0.0.1", 5000);
            var udp = new UdpClientWrapper(6000);
            var sdrClient = new NetSdrClient(tcp, udp);
            Assert.IsNotNull(sdrClient);
        }

        [Test]
        public void UdpClientWrapper_Constructor_WithAnotherValidPort_ShouldWork()
        {
             var client = new UdpClientWrapper(11111);
             Assert.IsNotNull(client);
        }
    }
}
