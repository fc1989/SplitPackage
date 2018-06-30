using System;
using System.Collections.Generic;
using System.Text;
using SplitPackage.EntityFrameworkCore;
using Xunit;

namespace SplitPackage.Tests.Contexts
{
    public class AdminCase : BaseCaseContext
    {
        public AdminCase():base()
        {

        }

        public override void InitData(SplitPackageDbContext context)
        {

        }
    }

    [CollectionDefinition("admin collection")]
    public class AdminCollection : ICollectionFixture<AdminCase>
    {

    }
}
