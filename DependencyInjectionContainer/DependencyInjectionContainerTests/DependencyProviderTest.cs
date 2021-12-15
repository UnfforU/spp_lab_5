using DependencyInjectionContainerLib.API.Parameters;
using DependencyInjectionContainerLib.Block;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerTests
{
    public class DependencyProviderTest
    {
        private DependenciesConfiguration Configuration;

        [SetUp]
        public void Setup()
        {
            Configuration = new DependenciesConfiguration();

        }

        [Test]
        public void TwoImplementationsWithoutNestedInterfaceTest()
        {
            Configuration.Register<ICommand, MyCommand1>(TTL.Singleton, ServiceImplementations.First);
            Configuration.Register<ICommand, MyCommand2>(TTL.InstancePerDependency, ServiceImplementations.Second);

            var provider = new DependencyProvider(Configuration);

            int expected = 1;
            var actual = provider.Resolve<ICommand>(ServiceImplementations.First);

            Assert.AreEqual(expected, actual.TestCommand());

            expected = 2;
            actual = provider.Resolve<ICommand>(ServiceImplementations.Second);

            Assert.AreEqual(expected, actual.TestCommand());
        }

        [Test]
        public void TwoImplementationsWithNestedInterfaceTest()
        {
            Configuration.Register<IRepository, Repository>(TTL.InstancePerDependency);
            Configuration.Register<IChat, TelegramChat>(TTL.Singleton, ServiceImplementations.First);
            Configuration.Register<IChat, VkChat>(TTL.InstancePerDependency, ServiceImplementations.Second);

            var provider = new DependencyProvider(Configuration);

            string expected = "Telegram";
            var actual = provider.Resolve<IChat>(ServiceImplementations.First);

            Assert.AreEqual(expected, actual.SendMessage());

            expected = "Vk";
            actual = provider.Resolve<IChat>(ServiceImplementations.Second);

            Assert.AreEqual(expected, actual.SendMessage());
        }

        [Test]
        public void OneImplementationWithCustomAttribute()
        {
            Configuration.Register<IChat, TelegramChat>(TTL.Singleton, ServiceImplementations.First);
            Configuration.Register<IChat, VkChat>(TTL.InstancePerDependency, ServiceImplementations.Second);
            Configuration.Register<IRepository, Repository>(TTL.Singleton);
            Configuration.Register<IFirstChat, FirstChat>(TTL.InstancePerDependency);

            var provider = new DependencyProvider(Configuration);

            string expected = "DependencyInjectionContainerTests.TelegramChat";
            var actual = provider.Resolve<IFirstChat>();

            Assert.IsInstanceOf(typeof(TelegramChat), actual.GetChat());
            Assert.AreEqual(expected, actual.GetChat().ToString());
        }
    }
}
