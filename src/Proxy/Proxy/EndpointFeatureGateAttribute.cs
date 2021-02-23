using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReverseProxyPOC.Proxy.Proxy
{
    /// <summary>
    /// An attribute that can be placed on controllers and actions to require all or any of a set of features to be enabled.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class EndpointFeatureGateAttribute : ActionFilterAttribute
    {
        // ProxyAwareFeatureGateAttribute
        // EndpointIsEnabledAttribute
        // Endpoint
        // APIProxyAttribute
        // ConditionalEndpointAttribute
        // ConditionalOnFeatureEndpointAttribute
        // FeatureGateAttribute
        // EndpointEnabledFeatureAttribute
        // EnableApiEndpointAttribute
        // AllowApiEndpointThroughProxyAttribute
        // Route
        // AllowRoute
        // ProxyRouteSettingsAttribute
        // ProxyRouteFeatureAttribute
        // ProxyRouteEndpoint
        // RouteEnabledSelector

        /// <summary>
        /// Creates an attribute that will gate actions unless all the provided feature(s) are enabled.
        /// </summary>
        /// <param name="features">The names of the features that the attribute will represent.</param>
        public EndpointFeatureGateAttribute(bool proxyingAllowed = true, params string[] features)
            : this(RequirementType.All, proxyingAllowed, features)
        {
        }

        /// <summary>
        /// Creates an attribute that can be used to gate actions. The gate can be configured to require all or any of the provided feature(s) to pass.
        /// </summary>
        /// <param name="requirementType">Specifies whether all or any of the provided features should be enabled in order to pass.</param>
        /// <param name="features">The names of the features that the attribute will represent.</param>
        public EndpointFeatureGateAttribute(RequirementType requirementType, bool proxyingAllowed, params string[] features)
        {
            if (features == null || features.Length == 0)
            {
                throw new ArgumentNullException(nameof(features));
            }

            Features = features;
            ProxyingAllowed = proxyingAllowed;
            RequirementType = requirementType;
        }

        /// <summary>
        /// Creates an attribute that will gate actions unless all the provided feature(s) are enabled.
        /// </summary>
        /// <param name="features">A set of enums representing the features that the attribute will represent.</param>
        public EndpointFeatureGateAttribute(bool proxyingAllowed = true, params object[] features)
            : this(RequirementType.All, proxyingAllowed, features)
        {
        }

        /// <summary>
        /// Creates an attribute that can be used to gate actions. The gate can be configured to require all or any of the provided feature(s) to pass.
        /// </summary>
        /// <param name="requirementType">Specifies whether all or any of the provided features should be enabled in order to pass.</param>
        /// <param name="features">A set of enums representing the features that the attribute will represent.</param>
        public EndpointFeatureGateAttribute(RequirementType requirementType, bool proxyingAllowed = true, params object[] features)
        {
            if (features == null || features.Length == 0)
            {
                throw new ArgumentNullException(nameof(features));
            }

            var fs = new List<string>();

            foreach (object feature in features)
            {
                var type = feature.GetType();

                if (!type.IsEnum)
                {
                    // invalid
                    throw new ArgumentException("The provided features must be enums.", nameof(features));
                }

                fs.Add(Enum.GetName(feature.GetType(), feature));
            }

            Features = fs;

            RequirementType = requirementType;

            ProxyingAllowed = proxyingAllowed;
        }

        /// <summary>
        /// The name of the features that the feature attribute will activate for.
        /// </summary>
        public IEnumerable<string> Features { get; }

        /// <summary>
        /// Controls whether any or all features in <see cref="Features"/> should be enabled to pass.
        /// </summary>
        public RequirementType RequirementType { get; }

        /// <summary>
        /// Controls whether the route are allowed through the proxy or not.
        /// </summary>
        public bool ProxyingAllowed { get; }

        /// <summary>
        /// Performs controller action pre-procesing to ensure that at least one of the specified features are enabled.
        /// </summary>
        /// <param name="context">The context of the MVC action.</param>
        /// <param name="next">The action delegate.</param>
        /// <returns>Returns a task representing the action execution unit of work.</returns>
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var featureManager = context.HttpContext.RequestServices.GetRequiredService<IFeatureManagerSnapshot>();

            // Enabled state is determined by either 'any' or 'all' features being enabled.
            bool enabled = RequirementType == RequirementType.All ?
                             await Features.All(async feature => await featureManager.IsEnabledAsync(feature).ConfigureAwait(false)) :
                             await Features.Any(async feature => await featureManager.IsEnabledAsync(feature).ConfigureAwait(false));

            if (enabled)
            {
                await next().ConfigureAwait(false);
            }
            else
            {
                var disabledFeaturesHandler = context.HttpContext.RequestServices.GetService<IDisabledFeaturesHandler>() ?? new NotFoundDisabledFeaturesHandler();

                await disabledFeaturesHandler.HandleDisabledFeatures(Features, context).ConfigureAwait(false);
            }
        }
    }
}
