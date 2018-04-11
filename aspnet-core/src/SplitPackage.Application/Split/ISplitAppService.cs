using SplitPackage.Split.RuleModels;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Split
{
    public interface ISplitAppService
    {
        Dictionary<string, Logistic> GetLogisticcDic();
        List<LogisticsModel> GetLogisticsList();
        void Initialize(string folderPath);
        SplitedOrder Split(string orderId, List<Product> productList, int totalQuantity, int splitType);
        SplitedOrder SplitWithOrganization(string orderId, List<Product> productList, int totalQuantity, string logisticsName, string gradeName);
        SplitedOrder SplitWithOrganization1(string orderId, List<Product> productList, int totalQuantity, List<RuleEntity> relst);
    }
}
