using NUnit.Framework;
using NetArchTest.Rules;
using NetSdrClientApp.Networking;
using System.Reflection;

namespace NetSdrClientAppTests
{
    public class ArchitectureTests
    {
        [Test]
        public void Networking_Should_Not_Directly_Use_Console()
        {
            var result = Types.InAssembly(typeof(BaseClient).Assembly)
                              .That()
                              .ResideInNamespace("NetSdrClientApp.Networking")
                              .ShouldNot()
                              .HaveDependencyOn("System.Console")
                              .GetResult();

            Assert.IsTrue(result.IsSuccessful, "Networking layer should not write to Console directly!");
        }

        [Test]
        public void Interfaces_Should_Start_With_I()
        {
            var result = Types.InAssembly(typeof(BaseClient).Assembly)
                              .That()
                              .AreInterfaces()
                              .Should()
                              .HaveNameStartingWith("I")
                              .GetResult();

            Assert.IsTrue(result.IsSuccessful);
        }
    }
}
