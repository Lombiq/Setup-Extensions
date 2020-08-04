using Lombiq.SetupExtensions.Apis.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using static Lombiq.SetupExtensions.Constants.ConfigurationKeys;

namespace Lombiq.SetupExtensions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures Orchard Core CMS with all API requests as authorized, i.e. all API endpoints will be accessible
        /// without further authorization if the specified configuration value (<see
        /// cref="AuthorizeOrchardApiRequests"/>) is set to "true".
        /// </summary>
        /// <param name="services">Service collection instance from the Startup class' ConfigureServices
        /// method.</param>
        /// <param name="configuration">
        /// Application configuration where to read the <see cref="AuthorizeOrchardApiRequests"/> key from.
        /// </param>
        /// <param name="orchardCoreBuilder">Optional action to define further Orchard CMS configuration.</param>
        public static IServiceCollection AddOrchardCmsWithAuthorizedApiRequestsIfEnabled(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<OrchardCoreBuilder> orchardCoreBuilder = default) =>
            configuration.GetValue<bool>(AuthorizeOrchardApiRequests) ?
                services.AddOrchardCms(builder =>
                {
                    builder
                        .AddSetupFeatures("OrchardCore.Tenants")
                        .ConfigureServices(services =>
                        {
                            services.AddScoped<IAuthorizationHandler, PermissionContextAuthorizationHandler>(sp =>
                            {
                                return new PermissionContextAuthorizationHandler(
                                    sp.GetRequiredService<IHttpContextAccessor>(),
                                    new ConcurrentDictionary<string, PermissionsContext>());
                            });

                            services.AddAuthentication((options) =>
                            {
                                options.AddScheme<AlwaysLoggedInApiAuthenticationHandler>("Api", null);
                            });
                        })
                        .Configure(appBuilder => appBuilder.UseAuthorization());

                    orchardCoreBuilder?.Invoke(builder);
                }) :
                services.AddOrchardCms(orchardCoreBuilder);
    }
}
