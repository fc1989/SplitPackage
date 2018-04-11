using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace SplitPackage.EntityFrameworkCore
{
    public static class SplitPackageDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<SplitPackageDbContext> builder, string connectionString)
        {
            builder.UseMySql(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<SplitPackageDbContext> builder, DbConnection connection)
        {
            builder.UseMySql(connection);
        }
    }
}
