using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Othello.Engine;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAi(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IAi, SimpleAi>();
        return serviceCollection;
    }

    public static IServiceCollection AddGame(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<Game>();
        return serviceCollection;
    }
}
