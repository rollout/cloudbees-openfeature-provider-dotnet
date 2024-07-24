using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Io.Rollout.Rox.Core.Context;
using Io.Rollout.Rox.Server;
using OpenFeature;
using OpenFeature.Constant;
using OpenFeature.Model;

namespace CloudBees.OpenFeature.Provider
{
    public class CloudBeesProvider : FeatureProvider
    {
        private readonly CloudBeesOptions _options;
        private ProviderStatus _providerStatus = ProviderStatus.NotReady;
        private readonly Metadata _metadata = new Metadata("CloudBees Provider");

        public CloudBeesProvider(CloudBeesOptions options)
        {
            if (string.IsNullOrEmpty(options.ApiKey))
            {
                throw new ArgumentNullException(nameof(options.ApiKey));
            }

            _options = options;
        }

        public override async Task Initialize(EvaluationContext context)
        {
            await Rox.Setup(_options.ApiKey);
            _providerStatus = Rox.State == RoxState.Corrupted ? ProviderStatus.Error : ProviderStatus.Ready;
        }

        /// <summary>
        /// Call this once when shutting down the application.
        /// </summary>
        public override async Task Shutdown()
        {
            await Rox.Shutdown();
            _providerStatus = ProviderStatus.NotReady;
        }

        public override Metadata GetMetadata() => _metadata;
        
        public override ProviderStatus GetStatus() => _providerStatus;

        public override Task<ResolutionDetails<bool>> ResolveBooleanValue(string flagKey, bool defaultValue, EvaluationContext context = null)
        {
            var evaluationResult = Rox.DynamicApi().IsEnabled(flagKey, defaultValue, TransformContext(context));
            return Task.FromResult(new ResolutionDetails<bool>(flagKey, evaluationResult));
        }

        public override Task<ResolutionDetails<string>> ResolveStringValue(string flagKey, string defaultValue, EvaluationContext context = null)
        {
            var evaluationResult = Rox.DynamicApi().GetValue(flagKey, defaultValue, TransformContext(context));
            return Task.FromResult(new ResolutionDetails<string>(flagKey, evaluationResult ?? defaultValue));
        }

        public override Task<ResolutionDetails<int>> ResolveIntegerValue(string flagKey, int defaultValue, EvaluationContext context = null)
        {
            var evaluationResult = Rox.DynamicApi().GetInt(flagKey, defaultValue, TransformContext(context));
            return Task.FromResult(new ResolutionDetails<int>(flagKey, evaluationResult));
        }

        public override Task<ResolutionDetails<double>> ResolveDoubleValue(string flagKey, double defaultValue, EvaluationContext context = null)
        {
            var evaluationResult = Rox.DynamicApi().GetDouble(flagKey, defaultValue, TransformContext(context));
            return Task.FromResult(new ResolutionDetails<double>(flagKey, evaluationResult));
        }

        public override Task<ResolutionDetails<Value>> ResolveStructureValue(string flagKey, Value defaultValue, EvaluationContext context = null)
        {
            throw new NotImplementedException("Not implemented - CloudBees feature management does not support an object type. Only String, Number and Boolean");
        }

        private static IContext TransformContext(EvaluationContext context)
        {
            return new ContextBuilder().Build(context == null 
                ? new Dictionary<string, object>() 
                : context.AsDictionary().ToDictionary(x => x.Key, x => x.Value.AsObject));
        }
    }
}