using Abp.Configuration;
using Abp.Localization;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SplitPackage.Web.Host.Startup
{
    public class DefaultRequestCultureProvider : RequestCultureProvider
    {
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var settingManager = httpContext.RequestServices.GetRequiredService<ISettingManager>();

            var culture = await settingManager.GetSettingValueForApplicationAsync(LocalizationSettingNames.DefaultLanguage);

            if (culture.IsNullOrEmpty())
            {
                return null;
            }

            return new ProviderCultureResult(culture, culture);
        }
    }
}
