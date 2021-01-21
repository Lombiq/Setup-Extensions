using Lombiq.SetupExtensions.Apis.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using static Lombiq.SetupExtensions.Constants.ConfigurationKeys;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configures Orchard Core with all API requests as authorized, i.e. all API endpoints will be accessible
        /// without further authorization if the specified configuration value ( <see
        /// cref="AuthorizeOrchardApiRequests"/>) is set to <see langword="true" />.
        /// </summary>
        /// <param name="configuration">
        /// Application configuration where to read the <see cref="AuthorizeOrchardApiRequests"/> key from.
        /// </param>
        public static OrchardCoreBuilder AddAuthorizedApiRequestsIfEnabled(
            this OrchardCoreBuilder builder,
            IConfiguration configuration) =>
            configuration.GetValue<bool>(AuthorizeOrchardApiRequests)
                ? builder.AddAuthorizedApiRequests()
                : builder;

        /// <summary>
        /// Configures Orchard Core with all API requests as authorized, i.e. all API endpoints will be accessible
        /// without further authorization.
        /// </summary>
        public static OrchardCoreBuilder AddAuthorizedApiRequests(this OrchardCoreBuilder builder) =>
            builder
                .AddSetupFeatures("OrchardCore.Tenants")
                .ConfigureServices(services =>
                {
                    services.AddScoped<IAuthorizationHandler, PermissionContextAuthorizationHandler>(sp =>
                        new PermissionContextAuthorizationHandler(
                            sp.GetRequiredService<IHttpContextAccessor>(),
                            new ConcurrentDictionary<string, PermissionsContext>()));

                    services.AddAuthentication(
                        options => options.AddScheme<AlwaysLoggedInApiAuthenticationHandler>("Api", null));
                })
                .Configure(appBuilder => appBuilder.UseAuthorization());
    }
}
