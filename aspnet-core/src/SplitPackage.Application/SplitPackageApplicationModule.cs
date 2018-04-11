using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Microsoft.AspNetCore.Hosting;
using SplitPackage.Authorization;
using SplitPackage.Split;

namespace SplitPackage
{
    [DependsOn(
        typeof(SplitPackageCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class SplitPackageApplicationModule : AbpModule
    {
        private readonly IHostingEnvironment _env;

        public SplitPackageApplicationModule(IHostingEnvironment env)
        {
            _env = env;
        }

        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<SplitPackageAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(SplitPackageApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );

            IocManager.Resolve<ISplitAppService>().Initialize(System.IO.Path.Combine(this._env.ContentRootPath, "SplitPackageRules"));
        }
    }
}
