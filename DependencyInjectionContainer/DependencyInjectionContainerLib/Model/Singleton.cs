using DependencyInjectionContainerLib.API.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib.Model
{
    public class Singleton
    {
        private  object instance;
        private  ServiceImplementations number;

        public object Instance => instance;
        public ServiceImplementations Number => number;

        public Singleton(object _instance, ServiceImplementations _number)
        {
            instance = _instance;
            number = _number;
        }
    }
}
