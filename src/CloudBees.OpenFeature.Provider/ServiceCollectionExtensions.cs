using System;
using CloudBees.OpenFeature.Provider;
using OpenFeature;

// ReSharper disable once CheckNamespace
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

            Api.Instance.SetProviderAsync(new CloudBeesProvider(cloudBeesOptions));
            services.AddSingleton(provider => Api.Instance.GetClient());
        }
    }
}