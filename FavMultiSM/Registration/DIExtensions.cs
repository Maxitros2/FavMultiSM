using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FavMultiSM.Registration
{
    public static class DIExtensions
    {
        public static IServiceCollection AddCustomServices<TService>(this IServiceCollection services) where TService : class
            => services.AddCustomServices(new Assembly[] { typeof(TService).Assembly });
        
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IEnumerable<Type> handlerAssemblyMarkerTypes)
            => services.AddCustomServices(handlerAssemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            foreach(var assemblie in assemblies)
            {                
                var allTypes = assemblie.DefinedTypes.Where(x => !x.IsInterface && !x.IsAbstract);                
                foreach(var type in allTypes.Where(x=>x.IsAssignableTo(typeof(ITrancientService))))
                {
                    services.AddTransient(type.AsType());
                }                
                foreach (var type in allTypes.Where(x => x.IsAssignableTo(typeof(ISingletoneService))))
                {
                    services.AddSingleton(type.AsType());
                }
            }
            return services;
        }        
    }
    
}
