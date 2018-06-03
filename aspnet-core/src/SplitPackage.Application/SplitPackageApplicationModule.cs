using Abp.AutoMapper;
using Abp.Events.Bus.Handlers;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Microsoft.AspNetCore.Hosting;
using SplitPackage.Authorization;
using SplitPackage.Cache;
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
            Configuration.UnitOfWork.ConventionalUowSelectors.Add(type =>
            {
                var result = type.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof(IAsyncEventHandler<>));
                return result;
            });
            IocManager.Register(typeof(ManageCache), Abp.Dependency.DependencyLifeStyle.Singleton);
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(SplitPackageApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );
        }
    }
}
