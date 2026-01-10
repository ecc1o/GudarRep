using NUnit.Framework;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EchoServerTests
{
    public class EchoServerTests
    {
        [Test]
        public async Task HandleClientAsync_ShouldEchoDataBack()
        {
            // Arrange 
            var server = new EchoServer(0); 
            
            byte[] inputData = Encoding.UTF8.GetBytes("Hello, World!");
            using var memoryStream = new MemoryStream();
            
            memoryStream.Write(inputData, 0, inputData.Length);
            memoryStream.Position = 0; 

            var cts = new CancellationTokenSource();

            // Act 
            await server.HandleClientAsync(memoryStream, cts.Token);

            // Assert
            
            byte[] resultBuffer = memoryStream.ToArray();
            string resultText = Encoding.UTF8.GetString(resultBuffer);

            Assert.IsTrue(resultText.Contains("Hello, World!"), "Stream should contain sent data");
            Assert.AreEqual(inputData.Length * 2, resultBuffer.Length, "Stream length should double (input + echo)");
        }
    }
}
