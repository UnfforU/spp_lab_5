using DependencyInjectionContainerLib.API.Parameters;
using DependencyInjectionContainerLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib
{
    public interface IDependenciesConfiguration
    {
        public Dictionary<Type, List<Implementation>> Dependencies { get; }

        //Register a new dependency
        void Register<TDependency, TImplementation>(TTL _ttl, ServiceImplementations _number = ServiceImplementations.None)
            where TDependency : class
            where TImplementation : TDependency;

        //Register a new dependency
        void Register(Type _dependencyType, Type _implementationType, TTL _ttl, ServiceImplementations _number = ServiceImplementations.None);
    }
}
