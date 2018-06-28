using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SplitPackage.Tests.Contexts
{
    /// <summary>
    /// 不允许在测试用例中进行数据修改,包括登陆状态(默认系统管理员)
    /// 建议进行管理员用例测试用例
    /// </summary>
    public class ReadStatelessCase : BaseCaseContext
    {
        public ReadStatelessCase() : base()
        {

        }
    }

    [CollectionDefinition("ReadStateless collection")]
    public class ReadStatelessCollection : ICollectionFixture<ReadStatelessCase>
    {

    }
}
