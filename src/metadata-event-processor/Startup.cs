using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MetadataEventApi.Services;

[assembly: FunctionsStartup(typeof(MetadataEventApi.Startup))]

namespace MetadataEventApi
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IMetadataProcessor,MetadataProcessor>();
        }
    }
}
