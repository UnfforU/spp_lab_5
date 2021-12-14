using DependencyInjectionContainerLib.API.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib.Model
{
    public class Implementation
    {
        private  Type implementationType;
        private  TTL timeToLive;
        private  ServiceImplementations implementationNumber;

        public Type ImplementationType => implementationType;
        public TTL TimeToLive => timeToLive;
        public ServiceImplementations ImplementationNumber => implementationNumber;

        public Implementation(Type _implementationType, TTL _ttl, ServiceImplementations _number)
        {
            implementationType = _implementationType;
            timeToLive = _ttl;
            implementationNumber = _number;
        }
    }
}
