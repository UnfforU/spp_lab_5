using DependencyInjectionContainerLib.Block;
using NUnit.Framework;

namespace DependencyInjectionContainerTests
{
    public class DependenciesConfigurationTest
    {
        private DependenciesConfiguration Configuration;

        [SetUp]
        public void Setup()
        {
            Configuration = new DependenciesConfiguration();

        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}