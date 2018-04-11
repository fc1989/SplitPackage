using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using SplitPackage.Configuration;

namespace SplitPackage.Web.Host.Startup
{
    [DependsOn(
       typeof(SplitPackageWebCoreModule))]
    public class SplitPackageWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public SplitPackageWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SplitPackageWebHostModule).GetAssembly());
        }
    }
}
