using Abp.AspNetCore.TestBase;
using Abp.Authorization.Users;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using SplitPackage.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SplitPackage.Tests
{
    public abstract class SplitPackageSettingBase : SplitPackageTestBase, IDisposable
    {
        public SplitPackageSettingBase()
        {
        }

        public int? GetTenantId()
        {
            return this.AbpSession.TenantId;
        }

        public T ResolveService<T>()
        {
            return this.Resolve<T>();
        }

        public void Dispose()
        {
        }
    }

    public class AstraeaAssistantSetting : SplitPackageSettingBase
    {
        public AstraeaAssistantSetting()
        {
            LoginAsTenant("AstraeaAssistant", AbpUserBase.AdminUserName);
        }
    }

    [CollectionDefinition("AstraeaAssistant collection")]
    public class SplitPackageSettingCollection : ICollectionFixture<AstraeaAssistantSetting>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
