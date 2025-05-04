using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace studentsapi.Tests.TestData
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "Local";

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
        ) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing or invalid token"));
            }

            var token = authHeader["Bearer ".Length..].Trim();
            List<Claim> claims;

            switch (token)
            {
                case "admin-token":
                    claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "AdminUser"),
                new Claim(ClaimTypes.Role, "Admin")
            };
                    break;

                case "manager-token":
                    claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "ManagerUser"),
                new Claim(ClaimTypes.Role, "Manager")
            };
                    break;

                case "guest-token":
                    claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "GuestUser"),
                new Claim(ClaimTypes.Role, "Guest")
            };
                    break;

                default:
                    return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
            }

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}