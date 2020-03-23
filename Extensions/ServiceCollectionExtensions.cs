using Lombiq.SetupExtensions.Apis.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using static Lombiq.SetupExtensions.Constants.Configuration;

namespace Lombiq.SetupExtensions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures Orchard CMS with all API requests authorized if the specified configuration value
        /// (<see cref="AuthorizeOrchardApiRequests"/>) is set to "true".
        /// Also allows optional Orchard CMS configuration on top of that.
        /// Copied from SiteStartup.cs in OrchardCore.Tests.Apis.Context with modifications.
        /// </summary>
        /// <param name="services">Service collection instance from the Startup class' ConfigureServices method.</param>
        /// <param name="configuration">Application configuration.</param>
        /// <param name="orchardCoreBuilder">Optional action to define further Orchard CMS configuration.</param>
        /// <returns></returns>
        public static IServiceCollection AddOrchardCmsWithAuthorizedApiRequests(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<OrchardCoreBuilder> orchardCoreBuilder = default) =>
            configuration.GetValue<bool>(AuthorizeOrchardApiRequests) ?
                services.AddOrchardCms(builder =>
                {
                    builder
                        .AddSetupFeatures("OrchardCore.Tenants")
                        .ConfigureServices(collection =>
                        {
                            collection.AddScoped<IAuthorizationHandler, PermissionContextAuthorizationHandler>(sp =>
                            {
                                return new PermissionContextAuthorizationHandler(
                                    sp.GetRequiredService<IHttpContextAccessor>(),
                                    new ConcurrentDictionary<string, PermissionsContext>());
                            });

                            collection.AddAuthentication((options) =>
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
