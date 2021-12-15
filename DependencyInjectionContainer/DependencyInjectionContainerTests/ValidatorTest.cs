using DependencyInjectionContainerLib.API.Parameters;
using DependencyInjectionContainerLib.Block;
using DependencyInjectionContainerLib.Helper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerTests
{
    public class ValidatorTest
    {
        private DependenciesConfiguration Configuration;

        [SetUp]
        public void SetUp()
        {
            Configuration = new DependenciesConfiguration();
        }

        [Test]
        public void TwoImplementationsWithoutNestedInterfaceValidTest()
        {
            Configuration.Register<ICommand, MyCommand1>(TTL.Singleton);
            Configuration.Register<ICommand, MyCommand2>(TTL.InstancePerDependency);

            var validator = new Validator(Configuration);

            Assert.IsTrue(validator.IsValid());
        }

        [Test]
        public void TwoImplementationsWithNestedInterfaceValidTest()
        {
            Configuration.Register<IRepository, Repository>(TTL.InstancePerDependency);
            Configuration.Register<IChat, TelegramChat>(TTL.Singleton);
            Configuration.Register<IChat, VkChat>(TTL.InstancePerDependency);

            var validator = new Validator(Configuration);

            Assert.IsTrue(validator.IsValid());
        }

        [Test]
        public void TwoImplementationsWithNestedInterfaceInValidTest()
        {
            Configuration.Register<IChat, TelegramChat>(TTL.Singleton);
            Configuration.Register<IChat, VkChat>(TTL.InstancePerDependency);

            var validator = new Validator(Configuration);

            Assert.IsFalse(validator.IsValid());
        }
    }
}
