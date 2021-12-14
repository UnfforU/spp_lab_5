using DependencyInjectionContainerLib.API.Parameters;
using System;

namespace DependencyInjectionContainerLib
{
    public interface IDependencyProvider
    {
        //Resolve a dependency
        TDependency Resolve<TDependency>(ServiceImplementations number = ServiceImplementations.Any) where TDependency : class;

        //Resolve a dependency
        object Resolve(Type dependencyType, ServiceImplementations number = ServiceImplementations.Any);

    }
}
