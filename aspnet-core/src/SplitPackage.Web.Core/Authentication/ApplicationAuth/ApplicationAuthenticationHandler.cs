using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using SplitPackage.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Linq;
using SplitPackage.Authorization.Users;
using Abp.Runtime.Security;
using Abp.Domain.Uow;
using SplitPackage.Authorization.Roles;

namespace SplitPackage.Authentication.ApplicationAuth
{
    public class ApplicationAuthenticationHandler : AuthenticationHandler<ApplicationAuthenticationOptions>
    {
        private const string AuthorizationHeaderName = "Authorization";
        private const string BasicSchemeName = "App";
        private readonly IRepository<OtherSystem, long> _osRepository;
        private readonly IRepository<User, long> _userRepository;

        public ApplicationAuthenticationHandler(
            IOptionsMonitor<ApplicationAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IRepository<OtherSystem, long> osRepository,
            IRepository<User, long> userRepository)
            : base(options, logger, encoder, clock)
        {
            this._osRepository = osRepository;
            this._userRepository = userRepository;
        }

        [UnitOfWork]
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                return AuthenticateResult.NoResult();
            }

            if (!AuthenticationHeaderValue.TryParse(Request.Headers[AuthorizationHeaderName], out AuthenticationHeaderValue headerValue))
            {
                //Invalid Authorization header
                return AuthenticateResult.NoResult();
            }

            if (!BasicSchemeName.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                //Not Basic authentication header
                return AuthenticateResult.NoResult();
            }

            byte[] headerValueBytes = Convert.FromBase64String(headerValue.Parameter);
            string userAndPassword = Encoding.UTF8.GetString(headerValueBytes);
            string[] parts = userAndPassword.Split(':');
            if (parts.Length != 2)
            {
                return AuthenticateResult.Fail("Invalid Basic authentication header");
            }
            string otherSystemName = parts[0];
            string certificate = parts[1];

            var os = await this._osRepository.FirstOrDefaultAsync(o => o.SystemName == otherSystemName && o.Certificate == certificate);
            if (os == null)
            {
                return AuthenticateResult.Fail("os is banish");
            }
            var user = await this._userRepository.SingleAsync(o => o.UserName == StaticRoleNames.Host.Admin && o.TenantId == null);
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(AbpClaimTypes.UserName, StaticRoleNames.Host.Admin),
                new Claim("SplitPackageOtherSystemId", os.Id.ToString())
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"App realm=\"split\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }
    }
}
