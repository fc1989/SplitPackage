using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Domain.Uow;
using Abp.Modules;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SplitPackage.Split;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static SplitPackage.Tests.Split.SplitAppService_Tesst;

namespace SplitPackage.Tests.Split
{
    public class SplitAppService_Tesst : SplitPackageTestBase
    {
        private readonly ISplitAppService _splitAppService;

        public SplitAppService_Tesst(TestContext testContext) :base(testContext)
        {
            _splitAppService = Resolve<ISplitAppService>();
        }

        [Fact]
        public void SplitTest()
        {
            var result = this._splitAppService.SplitWithOrganization("123", new List<Product>() {
                new Product()
                {
                    ProNo = "132",
                    SkuNo = "9310160820488",
                    Quantity = 8,
                    ProName = "Elevit 爱乐维 备孕孕妇补充叶酸 100片",
                    ProPrice = 10,
                    Weight = 10
                }
            }, 8, "CNP Express", "标准型");
            Assert.Equal(2, result.OrderList.Count);
            //按件数，件数多的优先
            //按重量，重量大的优先
            //按混装种类，种类多的优先
        }

        [Fact]
        public void SplitWithExp1Test()
        {
            List<RuleEntity> rules = new List<RuleEntity>();
            Logistic l = Spliter.GetLogisticcDic()[Logistic.GetLogisticName("CNP Express", "标准型")];
            rules.AddRange(l.RuleSequenceDic.Values);
            var result = this._splitAppService.SplitWithOrganization1("123", new List<Product>() {
                new Product()
                {
                    ProNo = "132",
                    SkuNo = "9310160820488",
                    Quantity = 8,
                    ProName = "Elevit 爱乐维 备孕孕妇补充叶酸 100片",
                    ProPrice = 10,
                    Weight = 10,
                    PTId = 101990401
                }
            }, 8, rules);
            Assert.Equal("123", result.OrderId);
            Assert.Equal(2, result.OrderList.Count);
            Assert.Equal("标准型", result.OrderList[0].GradeName);
            Assert.Equal("CNP Express", result.OrderList[0].LogisticsName);
            Assert.Equal("12301", result.OrderList[0].Id);
            Assert.Single(result.OrderList[0].ProList);
            Assert.Equal(5, result.OrderList[0].ProList[0].Quantity);
            Assert.Equal("标准型", result.OrderList[1].GradeName);
            Assert.Equal("CNP Express", result.OrderList[1].LogisticsName);
            Assert.Equal("12302", result.OrderList[1].Id);
            Assert.Single(result.OrderList[1].ProList);
            Assert.Equal(3, result.OrderList[1].ProList[0].Quantity);
        }
    }
}
