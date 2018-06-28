using SplitPackage.Split;
using SplitPackage.Tests.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SplitPackage.Tests.SplitV1
{
    [Collection("Assistant collection")]
    public class SplitService_Sku_Test
    {
        private readonly ISplitService _splitService;
        private readonly AssistantCase _context;

        public SplitService_Sku_Test(Xunit.Abstractions.ITestOutputHelper output, AssistantCase context)
        {
            this._context = context;
            this._splitService = context.ResolveService<ISplitService>();
        }

        /// <summary>
        /// 替换商品明细的ptid,拆单成功
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ReplaceSku_SplitSuccess_Test()
        {

        }

        /// <summary>
        /// 替换商品明细的ptid,拆单失败,
        /// 使用请求ptid拆单
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ReplaceSku_SplitFailure_Test()
        {

        }

        /// <summary>
        /// 根据sku规则拆单成功
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SkuRule_SplitSuccess_Test()
        {

        }

        /// <summary>
        /// 根据sku规则拆单失败,
        /// 使用sku对应ptid拆单
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Split_SplitFailure_Test()
        {

        }
    }
}
