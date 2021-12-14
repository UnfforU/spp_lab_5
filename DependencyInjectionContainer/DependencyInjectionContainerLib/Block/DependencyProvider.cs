using DependencyInjectionContainerLib.API.Attributes;
using DependencyInjectionContainerLib.API.Parameters;
using DependencyInjectionContainerLib.API.Validator;
using DependencyInjectionContainerLib.Helper;
using DependencyInjectionContainerLib.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib.Block
{
    public class DependencyProvider : IDependencyProvider
    {
        private readonly IDependenciesConfiguration configuration;
        private readonly Dictionary<Type, List<Singleton>> singletons;

        public DependencyProvider(IDependenciesConfiguration _configuration)
        {
            // Check if configuration is valid
            IValidator configValidator = new Validator(configuration);
            if (!configValidator.IsValid())
            {
                throw new ArgumentException("Invalid configuration!");
            }

            configuration = _configuration;
            singletons = new Dictionary<Type, List<Singleton>>();
        }

        //Resolve a dependency
        public TDependency Resolve<TDependency>(ServiceImplementations number = ServiceImplementations.Any) where TDependency : class
        {
            return (TDependency)Resolve(typeof(TDependency), number);
        }

        //Resolve a dependency
        public object Resolve(Type dependencyType, ServiceImplementations number = ServiceImplementations.Any)
        {
            object result;

            // Case if dependency type is enumerable
            if (IsEnumerable(dependencyType))
            {
                result = CreateEnumerable(dependencyType.GetGenericArguments()[0]);
            }
            // Other cases
            else
            {
                Implementation container = GetContainerByDependencyType(dependencyType, number);
                Type type = GetGeneratedType(dependencyType, container.ImplementationType);

                result = ResolveNonEnumerable(type, container.TimeToLive, dependencyType, container.ImplementationNumber);
            }

            return result;
        }

        //Get container by dependency type
        private Implementation GetContainerByDependencyType(Type dependencyType, ServiceImplementations number)
        {
            Implementation container;

            // Case if dependency type is generics
            if (dependencyType.IsGenericType)
            {
                container = GetLastContainer(dependencyType, number);
                container ??= GetLastContainer(dependencyType.GetGenericTypeDefinition(), number);
            }
            // Other cases
            else
            {
                container = GetLastContainer(dependencyType, number);
            }

            return container;
        }

        //Get last container
        private Implementation GetLastContainer(Type dependencyType, ServiceImplementations number)
        {
            if (configuration.Dependencies.ContainsKey(dependencyType))
            {
                return configuration.Dependencies[dependencyType].FindLast(container => number.HasFlag(container.ImplementationNumber));
            }

            return null;
        }

        //Get generated class
        private Type GetGeneratedType(Type dependencyType, Type implementationType)
        {
            // Case if dependency type is generics
            if (dependencyType.IsGenericType && implementationType.IsGenericTypeDefinition)
            {
                return implementationType.MakeGenericType(dependencyType.GetGenericArguments());
            }

            return implementationType;
        }


        //Check that dependency type is enumerable
        private bool IsEnumerable(Type dependencyType)
        {
            return dependencyType.GetInterfaces().Any(i => i.Name == "IEnumerable");
        }

        //Create dependency for enumerable object
        private IList CreateEnumerable(Type dependencyType)
        {
            var result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(dependencyType));
            var containers = configuration.Dependencies[dependencyType];

            // Resolve all base types and add them to list
            foreach (var container in containers)
            {
                var instance = ResolveNonEnumerable(container.ImplementationType, container.TimeToLive, dependencyType, container.ImplementationNumber);
                result.Add(instance);
            }

            return result;
        }

        //Resolve non enumerable class
        private object ResolveNonEnumerable(Type implementationType, TTL ttl, Type dependencyType, ServiceImplementations number)
        {
            // Case if it isn't singleton
            if (ttl != TTL.Singleton)
            {
                return CreateInstance(implementationType);
            }

            // Other case (lock for multi thread)
            lock (configuration)
            {
                // Case if singleton exists
                if (IsInSingletons(dependencyType, implementationType, number))
                {
                    return singletons[dependencyType].Find(singletonContainer => number.HasFlag(singletonContainer.Number)).Instance;
                }

                // Create a new singleton object
                var result = CreateInstance(implementationType);
                AddToSingletons(dependencyType, result, number);

                return result;
            }
        }

        //Create a new dependency instance
        private object CreateInstance(Type implementationType)
        {
            var constructors = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            foreach (var constructor in constructors)
            {
                var generatedParams = new List<dynamic>();

                // Get all parameters and proccess nested types
                var constructorParams = constructor.GetParameters();
                foreach (var parameterInfo in constructorParams)
                {
                    dynamic parameter;
                    if (parameterInfo.ParameterType.IsInterface)
                    {
                        var number = parameterInfo.GetCustomAttribute<DependencyKeyAttribute>()?.Number ?? ServiceImplementations.Any;
                        parameter = Resolve(parameterInfo.ParameterType, number);
                    }
                    else
                    {
                        break;
                    }

                    generatedParams.Add(parameter);
                }

                return constructor.Invoke(generatedParams.ToArray());
            }

            throw new ArgumentException("Can't create class instance!");
        }

        //Сheck that singleton exists
        private bool IsInSingletons(Type dependencyType, Type implementationType, ServiceImplementations number)
        {
            var list = singletons.ContainsKey(dependencyType) ? singletons[dependencyType] : null;

            return list?.Find(container => number.HasFlag(container.Number) && container.Instance.GetType() == implementationType) != null;
        }

        //Add a new sigleton object
        private void AddToSingletons(Type dependencyType, object implementation, ServiceImplementations number)
        {
            if (singletons.ContainsKey(dependencyType))
            {
                singletons[dependencyType].Add(new Singleton(implementation, number));
            }
            else
            {
                singletons.Add(dependencyType, new List<Singleton>() { new Singleton(implementation, number) });
            }
        }
    }
}
