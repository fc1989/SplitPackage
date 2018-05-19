using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using SplitPackage.Authorization;
using SplitPackage.Business;
using SplitPackage.Business.LogisticChannels.Dto;
using SplitPackage.Business.NumFreights.Dto;
using SplitPackage.Business.Products.Dto;
using SplitPackage.Business.WeightFreights.Dto;
using SplitPackage.Split;
using System.Collections.Generic;
using System.Linq;

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

            IocManager.Resolve<ISplitService>().Initialize(System.IO.Path.Combine(this._env.ContentRootPath, "SplitPackageRules"));
        }
    }
}
