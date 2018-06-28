using Abp.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SplitPackage.Tests.Contexts
{
    public class AssistantCase : BaseCaseContext
    {
        public AssistantCase():base()
        {
            LoginAsTenant("AstraeaAssistant", AbpUserBase.AdminUserName);
        }
    }

    [CollectionDefinition("Assistant collection")]
    public class AssistantCaseCollection : ICollectionFixture<AssistantCase>
    {

    }
}
