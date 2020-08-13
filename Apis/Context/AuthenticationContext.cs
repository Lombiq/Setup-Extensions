using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrchardCore.Security;
using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

// Copied from AuthenticationContext.cs in OrchardCore.Tests.Apis.Context with minor modifications.
namespace Lombiq.SetupExtensions.Apis.Context
{
    internal class PermissionContextAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly PermissionsContext _permissionsContext;


        public PermissionContextAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IDictionary<string, PermissionsContext> permissionsContexts)
        {
            _permissionsContext = new PermissionsContext();

            var requestContext = httpContextAccessor.HttpContext.Request;

            if (requestContext?.Headers.ContainsKey("PermissionsContext") == true &&
                permissionsContexts.TryGetValue(requestContext.Headers["PermissionsContext"], out var permissionsContext))
            {
                _permissionsContext = permissionsContext;
            }
        }

        public PermissionContextAuthorizationHandler(PermissionsContext permissionsContext)
        {
            _permissionsContext = permissionsContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var permissions = (_permissionsContext.AuthorizedPermissions ?? Enumerable.Empty<Permission>()).ToList();

            if (!_permissionsContext.UsePermissionsContext)
            {
                context.Succeed(requirement);
            }
            else if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }

    internal class AlwaysLoggedInApiAuthenticationHandler : AuthenticationHandler<ApiAuthorizationOptions>
    {
        public AlwaysLoggedInApiAuthenticationHandler(
            IOptionsMonitor<ApiAuthorizationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync() =>
            Task.FromResult(
                AuthenticateResult.Success(
                    new AuthenticationTicket(
                        new System.Security.Claims.ClaimsPrincipal(new AlwaysLoggedInIdentity()), "Api")));
    }

    internal class PermissionsContext
    {
        public IEnumerable<Permission> AuthorizedPermissions { get; set; } = Enumerable.Empty<Permission>();

        public bool UsePermissionsContext { get; set; }
    }

    internal class AlwaysLoggedInIdentity : IIdentity
    {
        public string AuthenticationType => "Always Authenticated";

        public bool IsAuthenticated => true;

        public string Name => "Lombiq Technologies";
    }
}
