using Lombiq.SetupExtensions.Apis.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using static Lombiq.SetupExtensions.Constants.ConfigurationKeys;

namespace Microsoft.Extensions.DependencyInjection;

public static class OrchardCoreBuilderExtensions
{
    /// <summary>
    /// Configures all API requests as authorized, i.e. all API endpoints will be accessible without further
    /// authorization if the specified configuration value (<see cref="AuthorizeOrchardApiRequests"/>) is set to
    /// <see langword="true" />.
    /// </summary>
    /// <param name="configuration">
    /// Application configuration where to read the <see cref="AuthorizeOrchardApiRequests"/> key from.
    /// </param>
    public static OrchardCoreBuilder AuthorizeApiRequestsIfEnabled(
        this OrchardCoreBuilder builder,
        IConfiguration configuration) =>
        configuration.GetValue<bool>(AuthorizeOrchardApiRequests)
            ? builder.AuthorizeApiRequests()
            : builder;

    /// <summary>
    /// Configures all API requests as authorized, i.e. all API endpoints will be accessible without further
    /// authorization.
    /// </summary>
    /// <remarks>
    /// <para>Copied from SiteStartup.cs in OrchardCore.Tests.Apis.Context with modifications.</para>
    /// </remarks>
    public static OrchardCoreBuilder AuthorizeApiRequests(this OrchardCoreBuilder builder) =>
        builder
            .AddSetupFeatures("OrchardCore.Tenants")
            .ConfigureServices(services =>
            {
                services.AddScoped<IAuthorizationHandler, PermissionContextAuthorizationHandler>(sp =>
                    new PermissionContextAuthorizationHandler(
                        sp.GetRequiredService<IHttpContextAccessor>(),
                        new ConcurrentDictionary<string, PermissionsContext>()));

                services.AddAuthentication(
                    options => options.AddScheme<AlwaysLoggedInApiAuthenticationHandler>("Api", displayName: null));
            })
            .Configure(appBuilder => appBuilder.UseAuthorization());
}
