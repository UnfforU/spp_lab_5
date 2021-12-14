using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib.API.Validator
{
    public interface IValidator
    {
        //Check if configuration is valid
        bool IsValid();
    }
}
