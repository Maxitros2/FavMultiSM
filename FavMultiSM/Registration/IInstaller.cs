using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FavMultiSM.Registration
{
    public interface IInstaller
    {
        void AddServices(IServiceCollection services, IConfiguration configuration);
        public int Order => -1;
    }
}
