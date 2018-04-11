using Abp.Dependency;
using SplitPackage.Split.RuleModels;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Split
{
    public class SplitAppService : ISplitAppService, ISingletonDependency
    {
        public Dictionary<string, Logistic> GetLogisticcDic()
        {
            return Spliter.GetLogisticcDic();
        }

        public List<LogisticsModel> GetLogisticsList()
        {
            return Spliter.GetLogisticsList();
        }

        public void Initialize(string folderPath)
        {
            Spliter.Initialize(folderPath);
        }

        public SplitedOrder Split(string orderId, List<Product> productList, int totalQuantity, int splitType)
        {
            return Spliter.Split(orderId, productList, totalQuantity, splitType);
        }

        public SplitedOrder SplitWithOrganization(string orderId, List<Product> productList, int totalQuantity, string logisticsName, string gradeName)
        {
            return Spliter.SplitWithOrganization(orderId, productList, totalQuantity, logisticsName, gradeName);
        }

        public SplitedOrder SplitWithOrganization1(string orderId, List<Product> productList, int totalQuantity, List<RuleEntity> relst)
        {
            return Spliter.SplitWithOrganization1(orderId, productList, totalQuantity, relst);
        }
    }
}
