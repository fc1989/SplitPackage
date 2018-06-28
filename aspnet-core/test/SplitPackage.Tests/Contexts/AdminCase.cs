using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SplitPackage.Tests.Contexts
{
    public class AdminCase : BaseCaseContext
    {

    }

    [CollectionDefinition("admin collection")]
    public class AdminCollection : ICollectionFixture<AdminCase>
    {

    }
}
