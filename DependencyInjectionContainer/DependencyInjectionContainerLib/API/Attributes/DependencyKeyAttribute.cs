using DependencyInjectionContainerLib.API.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib.API.Attributes
{
    //Custom attribute for dependency provider
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyKeyAttribute : Attribute
    {
        private ServiceImplementations number;

        public ServiceImplementations Number => number;

        public DependencyKeyAttribute(ServiceImplementations _number)
        {
            number = _number;
        }
    }
}
