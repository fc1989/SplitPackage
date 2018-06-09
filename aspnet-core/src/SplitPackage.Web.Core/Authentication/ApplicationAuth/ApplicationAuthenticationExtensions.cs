using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace SplitPackage.Authentication.ApplicationAuth
{
    public static class ApplicationAuthenticationExtensions
    {
        public static AuthenticationBuilder AddApplication(this AuthenticationBuilder builder)
        {
            return AddApplication(builder, ApplicationAuthenticationDefaults.AuthenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddApplication(this AuthenticationBuilder builder, string authenticationScheme)
        {
            return AddApplication(builder, authenticationScheme, _ => { });
        }

        public static AuthenticationBuilder AddApplication(this AuthenticationBuilder builder, Action<ApplicationAuthenticationOptions> configureOptions)
        {
            return AddApplication(builder, ApplicationAuthenticationDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddApplication(this AuthenticationBuilder builder, string authenticationScheme, Action<ApplicationAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<ApplicationAuthenticationOptions, ApplicationAuthenticationHandler>(
                authenticationScheme, configureOptions);
        }
    }
}
