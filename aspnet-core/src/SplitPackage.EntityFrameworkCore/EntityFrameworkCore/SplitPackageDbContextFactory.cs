using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SplitPackage.Configuration;
using SplitPackage.Web;

namespace SplitPackage.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class SplitPackageDbContextFactory : IDesignTimeDbContextFactory<SplitPackageDbContext>
    {
        public SplitPackageDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SplitPackageDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            SplitPackageDbContextConfigurer.Configure(builder, configuration.GetConnectionString(SplitPackageConsts.ConnectionStringName));

            return new SplitPackageDbContext(builder.Options);
        }
    }
}
