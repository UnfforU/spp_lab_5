using DependencyInjectionContainerLib.API.Validator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib.Helper
{
    public class Validator : IValidator
    {
        private readonly IDependenciesConfiguration configuration;
        private readonly Stack<Type> nestedTypes;

        public Validator(IDependenciesConfiguration _configuration)
        {
            configuration = _configuration;
            nestedTypes = new Stack<Type>();
        }

        //Check if configuration is valid
        public bool IsValid()
        {
            return configuration.Dependencies.Values.All(implementations => implementations
                                                    .All(implementation => CanBeCreatedCheck(implementation.ImplementationType)));
        }

        //Check that intance can be created
        private bool CanBeCreatedCheck(Type instanceType)
        {
            nestedTypes.Push(instanceType);

            //Get all constructors
            var constructors = instanceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            foreach(var constructor in constructors)
            {

                //Get all parameters in current constructor
                var parameneters = constructor.GetParameters();
                foreach(var parameter in parameneters)
                {
                    Type parameterType;

                    //Check that constructor contains inteface types
                    if (parameter.ParameterType.ContainsGenericParameters)
                    {
                        parameterType = parameter.ParameterType.GetInterfaces()[0];
                    }

                    // Check that constructor contains generic types
                    else if (parameter.ParameterType.GetInterfaces().Any(i => i.Name == "IEnumerable"))
                    {
                        parameterType = parameter.ParameterType.GetGenericArguments()[0];
                    }
                    // Other cases
                    else
                    {
                        parameterType = parameter.ParameterType;
                    }

                    // Check that necessary type exists in container
                    if (parameterType.IsInterface && IsExists(parameterType))
                    {
                        continue;
                    }

                    nestedTypes.Pop();
                    return false;
                }
            }

            nestedTypes.Pop();
            return true;
        }

        //Check that necessary type exists in container
        private bool IsExists(Type type)
        {
            return configuration.Dependencies.ContainsKey(type);
        }
    }
}
