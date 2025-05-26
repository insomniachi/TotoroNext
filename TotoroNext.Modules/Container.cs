using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotoroNext.Module;
public static class Container
{
    public static IServiceProvider Services { get; private set; } = null!;

    public static void ConfigureServices(IServiceProvider services)
    {
        Services = services;
    }
}
