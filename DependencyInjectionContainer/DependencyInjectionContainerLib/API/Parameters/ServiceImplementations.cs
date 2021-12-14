using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainerLib.API.Parameters
{
    //Implementation number
    [Flags]
    public enum ServiceImplementations
    {
        None = 1,
        First = 2,
        Second = 4,
        Any = None | First | Second
    }
}
