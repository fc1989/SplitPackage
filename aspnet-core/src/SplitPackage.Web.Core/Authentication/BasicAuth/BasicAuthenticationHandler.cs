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
using Abp.Logging;

namespace SplitPackage.Authentication.BasicAuth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private const string AuthorizationHeaderName = "Authorization";
        private const string BasicSchemeName = "Basic";
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<User, long> _userRepository;

        public BasicAuthenticationHandler(
            IOptionsMonitor<BasicAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IRepository<Tenant> tenantRepository,
            IRepository<User, long> userRepository)
            : base(options, logger, encoder, clock)
        {
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
        }

        [UnitOfWork]
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                //Authorization header not in request
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

            try
            {
                if (string.IsNullOrEmpty(headerValue.Parameter))
                {
                    return AuthenticateResult.Fail(new Abp.UI.UserFriendlyException((int)Split.Dto.ResultCode.Auth_InvalidToken, "Invalid token"));
                }
                byte[] headerValueBytes = Convert.FromBase64String(headerValue.Parameter);
                string userAndPassword = Encoding.UTF8.GetString(headerValueBytes);
                string[] parts = userAndPassword.Split(':');
                if (parts.Length != 2)
                {
                    return AuthenticateResult.Fail(new Abp.UI.UserFriendlyException((int)Split.Dto.ResultCode.Auth_InvalidAutheHeader, "Invalid Basic authentication header"));
                }
                string tenancyName = parts[0];
                string apiKey = parts[1];

                string userId = "";
                string tenantId = "";
                if (!string.IsNullOrEmpty(tenancyName))
                {
                    var tenants = await _tenantRepository.GetAll().IgnoreQueryFilters()
                        .Where(o => o.ApiKey == apiKey && o.TenancyName == tenancyName && !o.IsDeleted && o.IsActive).ToListAsync();
                    if (tenants.Count == 0)
                    {
                        return AuthenticateResult.Fail(new Abp.UI.UserFriendlyException((int)Split.Dto.ResultCode.Auth_InvalidInput, "Invalid tenancyname or apikey"));
                    }
                    if (!tenants[0].IsActive)
                    {
                        return AuthenticateResult.Fail(new Abp.UI.UserFriendlyException((int)Split.Dto.ResultCode.Auth_RefuseAuthorization, "tenant is banish"));
                    }
                    var user = await this._userRepository.GetAll().IgnoreQueryFilters().SingleAsync(o => o.UserName == "admin" && o.TenantId == tenants[0].Id);
                    tenantId = tenants[0].Id.ToString();
                    userId = user.Id.ToString();
                }
                else if (apiKey.Equals(Options.SystemApiKey))
                {
                    var user = await this._userRepository.GetAll().IgnoreQueryFilters().SingleAsync(o => o.UserName == "admin" && o.TenantId == null);
                    userId = user.Id.ToString();
                }
                else
                {
                    return AuthenticateResult.Fail(new Abp.UI.UserFriendlyException((int)Split.Dto.ResultCode.Auth_InvalidInput, "Invalid tenancyname or apikey"));
                }
                var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(AbpClaimTypes.TenantId, tenantId),
                    new Claim(AbpClaimTypes.UserName, StaticRoleNames.Host.Admin)
                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex) when (ex is FormatException || ex is DecoderFallbackException)
            {
                return AuthenticateResult.Fail(new Abp.UI.UserFriendlyException((int)Split.Dto.ResultCode.Auth_InvalidToken, "Invalid token"));
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error("basic auth", ex);
                return AuthenticateResult.Fail(new Abp.UI.UserFriendlyException((int)Split.Dto.ResultCode.SytemError, "Invalid auth request"));
            }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"Basic realm=\"split\", charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }
    }
}
