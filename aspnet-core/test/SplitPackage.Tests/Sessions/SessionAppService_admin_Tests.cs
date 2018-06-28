using System.Threading.Tasks;
using Shouldly;
using Xunit;
using SplitPackage.Sessions;
using SplitPackage.Tests.Contexts;

namespace SplitPackage.Tests.Sessions
{
    [Collection("ReadStateless collection")]
    public class SessionAppService_Admin_Tests
    {
        private readonly ISessionAppService _sessionAppService;
        private readonly ReadStatelessCase _context;

        public SessionAppService_Admin_Tests(Xunit.Abstractions.ITestOutputHelper output, ReadStatelessCase context)
        {
            this._context = context;
            this._sessionAppService = this._context.ResolveService<ISessionAppService>();
        }

        [MultiTenantFact]
        public async Task Should_Get_Current_User_When_Logged_In_As_Host()
        {
            // Act
            var output = await _sessionAppService.GetCurrentLoginInformations();

            // Assert
            var currentUser = await this._context.GetCurrentUserAsync();
            output.User.ShouldNotBe(null);
            output.User.Name.ShouldBe(currentUser.Name);
            output.User.Surname.ShouldBe(currentUser.Surname);

            output.Tenant.ShouldBe(null);
        }
    }
}
