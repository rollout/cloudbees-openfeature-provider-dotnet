using System;
using CloudBees.OpenFeature.Provider;
using OpenFeature;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddOpenFeatureCloudBees(this IServiceCollection services, Action<CloudBeesOptions> configure)
        {
            var cloudBeesOptions = new CloudBeesOptions();
            configure.Invoke(cloudBeesOptions);

            if (string.IsNullOrWhiteSpace(cloudBeesOptions.ApiKey))
            {
                throw new ApplicationException("Missing CloudBees API Key");
            }

            var provider = new CloudBeesProvider(cloudBeesOptions.ApiKey);

            // Set the provider using async lifecycle call
            Task.Run(async () =>
            {
                await Api.Instance.SetProviderAsync(provider);
            }).GetAwaiter().GetResult();

            services.AddSingleton(_ => Api.Instance.GetClient());
        }
    }
}
