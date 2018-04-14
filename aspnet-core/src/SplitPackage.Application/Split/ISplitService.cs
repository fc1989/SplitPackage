using SplitPackage.Split.Dto;
using SplitPackage.Split.RuleModels;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Split
{
    public interface ISplitService
    {
        void Initialize(string folderPath);
        Tuple<string,SplitedOrder> Split(SplitRequest request);
        Tuple<string, SplitedOrder> SplitWithOrganization(SplitWithExpRequest request);
        Tuple<string, SplitedOrder> SplitWithOrganization1(SplitWithExpRequest1 request);
        Tuple<string, List<LogisticsModel>> GetLogisticsList(string userName);
    }
}
