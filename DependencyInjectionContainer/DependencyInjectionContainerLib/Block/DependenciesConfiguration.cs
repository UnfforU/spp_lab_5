using DependencyInjectionContainerLib.API.Parameters;
using DependencyInjectionContainerLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib.Block
{
    public class DependenciesConfiguration : IDependenciesConfiguration
    {
        public Dictionary<Type, List<Implementation>> Dependencies { get; set; }

        public DependenciesConfiguration()
        {
            Dependencies = new Dictionary<Type, List<Implementation>>();
        }

        //Register a new dependency
        public void Register<TDependency, TImplementation>(TTL _ttl, ServiceImplementations _number = ServiceImplementations.None)
            where TDependency : class
            where TImplementation : TDependency
        {
            Register(typeof(TDependency), typeof(TImplementation), _ttl, _number);
        
        }

        //Register a new dependency
        public void Register(Type _dependencyType, Type _implementationType, TTL _ttl, ServiceImplementations _number = ServiceImplementations.None)
        {
            // Check that implementation and dependency are compatible
            if (!IsCompatible(_dependencyType, _implementationType))
            {
                throw new ArgumentException("Incompatible types!");
            }

            var container = new Implementation(_implementationType, _ttl, _number);

            // Check that dependency has already exist
            if (Dependencies.ContainsKey(_dependencyType))
            {
                var index = Dependencies[_dependencyType].FindIndex(elem => elem.ImplementationType == container.ImplementationType);

                // Remove a duplicate dependency
                if (index != -1)
                {
                    Dependencies[_dependencyType].RemoveAt(index);
                }

                // Add a new dependency
                Dependencies[_dependencyType].Add(container);

            }
            // Add a new dependency
            else
            {
                Dependencies.Add(_dependencyType, new List<Implementation>() { container });
            }
        }

        //Check that implementation and dependency are compatible
        private bool IsCompatible(Type dependencyType, Type implementationType)
        {
            return implementationType.IsAssignableFrom(dependencyType) || implementationType.GetInterfaces().Any(i => i.ToString() == dependencyType.ToString());
        }
    }
}
