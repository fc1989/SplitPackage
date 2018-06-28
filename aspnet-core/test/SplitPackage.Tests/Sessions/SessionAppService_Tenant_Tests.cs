using Shouldly;
using SplitPackage.Sessions;
using SplitPackage.Tests.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SplitPackage.Tests.Sessions
{
    [Collection("Assistant collection")]
    public class SessionAppService_Tenant_Tests
    {
        private readonly ISessionAppService _sessionAppService;
        private readonly AssistantCase _context;

        public SessionAppService_Tenant_Tests(Xunit.Abstractions.ITestOutputHelper output, AssistantCase context)
        {
            this._context = context;
            this._sessionAppService = this._context.ResolveService<ISessionAppService>();
        }

        [Fact]
        public async Task Should_Get_Current_User_And_Tenant_When_Logged_In_As_Tenant()
        {
            // Act
            var output = await _sessionAppService.GetCurrentLoginInformations();

            // Assert
            var currentUser = await this._context.GetCurrentUserAsync();
            var currentTenant = await this._context.GetCurrentTenantAsync();

            output.User.ShouldNotBe(null);
            output.User.Name.ShouldBe(currentUser.Name);

            output.Tenant.ShouldNotBe(null);
            output.Tenant.Name.ShouldBe(currentTenant.Name);
        }
    }
}
