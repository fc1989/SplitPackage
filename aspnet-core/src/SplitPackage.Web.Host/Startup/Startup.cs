using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Castle.Facilities.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.Extensions;
using SplitPackage.Authentication.JwtBearer;
using SplitPackage.Configuration;
using SplitPackage.Identity;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Runtime.ExceptionServices;
using SplitPackage.Authentication.BasicAuth;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc;
using Abp.Logging;
using SplitPackage.Authentication.ApplicationAuth;

#if FEATURE_SIGNALR
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using Abp.Owin;
using SplitPackage.Owin;
#elif FEATURE_SIGNALR_ASPNETCORE
using Abp.AspNetCore.SignalR.Hubs;
#endif

namespace SplitPackage.Web.Host.Startup
{
    public class Startup
    {
        private const string _defaultCorsPolicyName = "localhost";

        private readonly IConfigurationRoot _appConfiguration;

        public Startup(IHostingEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // MVC
            services.AddMvc(
                options => options.Filters.Add(new CorsAuthorizationFilterFactory(_defaultCorsPolicyName))
            );

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

#if FEATURE_SIGNALR_ASPNETCORE
            services.AddSignalR();
#endif

            // Configure CORS for angular2 UI
            services.AddCors(
                options => options.AddPolicy(
                    _defaultCorsPolicyName,
                    builder => builder
                        .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            _appConfiguration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                )
            );

            // Swagger - Enable this line and the related lines in Configure method to enable swagger UI
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("open", new Info { Title = "open for tenant", Version = "v1" });
                options.SwaggerDoc("private", new Info { Title = "internal api", Version = "v1" });
                options.DocInclusionPredicate((docName, description) =>
                {
                    if (docName == "open")
                    {
                        var attrs = description.ControllerAttributes().OfType<ApiExplorerSettingsAttribute>();
                        var isTrue = attrs.Any(o => o.GroupName == "open");
                        return isTrue;
                    }
                    return true;
                });

                // Define the BearerAuth scheme that's in use
                options.AddSecurityDefinition("basicAuth", new BasicAuthScheme()
                {
                    Description = "basic Authorization header using the Basic scheme. Example: \"Authorization: Basic {token}\"",
                });
                options.AddSecurityDefinition("bearerAuth", new ApiKeyScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                // Assign scope requirements to operations based on AuthorizeAttribute
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            // Configure Abp and Dependency Injection
            return services.AddAbp<SplitPackageWebHostModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                )
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAbp(options => { options.UseAbpRequestLocalization = false; }); // Initializes ABP framework.

            app.UseCors(_defaultCorsPolicyName); // Enable CORS!

            app.UseStaticFiles();

            app.UseAuthentication();

            if (!_appConfiguration.GetValue<bool>("Logging:IgnoreOpenRequest"))
            {
                app.UseMiddleware<RequestLoggingMiddleware>();
            }

            app.Use(async (context, next) =>
             {
                 if (context.Request.Headers.ContainsKey("Authorization"))
                 {
                     var scheme = context.Request.Headers["Authorization"].ToString().Split(' ')[0];
                     if (scheme.Equals(JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
                     {
                         scheme = JwtBearerDefaults.AuthenticationScheme;
                     }
                     else if (scheme.Equals(BasicAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
                     {
                         scheme = BasicAuthenticationDefaults.AuthenticationScheme;
                     }
                     else if (scheme.Equals(ApplicationAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
                     {
                         scheme = ApplicationAuthenticationDefaults.AuthenticationScheme;
                     }
                     AuthenticateResult result = await context.AuthenticateAsync(scheme);
                     if (result.Succeeded && result.Principal.Identity.IsAuthenticated)
                     {
                         if (result?.Principal != null)
                         {
                             context.User = result.Principal;
                         }
                     }
                     //else if (result.Failure != null)
                     //{
                     //    // Rethrow, let the exception page handle it.
                     //    ExceptionDispatchInfo.Capture(result.Failure).Throw();
                     //}
                     else
                     {
                         await context.ChallengeAsync(scheme);
                         return;
                     }
                 }
                 await next();
             });

            app.UseAbpRequestLocalization();

#if FEATURE_SIGNALR
            // Integrate with OWIN
            app.UseAppBuilder(ConfigureOwinServices);
#elif FEATURE_SIGNALR_ASPNETCORE
            app.UseSignalR(routes =>
            {
                routes.MapHub<AbpCommonHub>("/signalr");
            });
#endif

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "{area}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();
            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(options =>
            {
                //定制化swagger ui
                //options.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("SplitPackage.Web.Host.SwaggerIndex.html");
                //options.InjectOnCompleteJavaScript("/swagger/ui/abp.js");
                //options.InjectOnCompleteJavaScript("/swagger/ui/on-complete.js");
                options.SwaggerEndpoint("/swagger/open/swagger.json", "SplitPackage Open API V1");
                options.SwaggerEndpoint("/swagger/private/swagger.json", "SplitPackage API V1");
            }); // URL: /swagger
        }

#if FEATURE_SIGNALR
        private static void ConfigureOwinServices(IAppBuilder app)
        {
            app.Properties["host.AppName"] = "SplitPackage";

            app.UseAbp();
            
            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                    EnableJSONP = true
                };
                map.RunSignalR(hubConfiguration);
            });
        }
#endif
    }
}
