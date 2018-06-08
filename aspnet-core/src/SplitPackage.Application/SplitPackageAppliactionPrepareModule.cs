using Abp.Modules;
using SplitPackage.Cache;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage
{
    [DependsOn(typeof(SplitPackageApplicationModule))]
    public class SplitPackageAppliactionPrepareModule:AbpModule
    {
        public override void PostInitialize()
        {
            var init = IocManager.Resolve<ManageCache>();
            init.InitCache();
        }
    }
}
