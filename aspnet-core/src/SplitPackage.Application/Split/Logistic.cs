using SplitPackage.Cache.Dto;
using SplitPackage.Split.RuleModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.Split
{
    public class Logistic
    {
        public Logistic(Organization subOrganization, SplitPackageConfig organization)
        {
            this.LogisticName = Logistic.GetLogisticName(organization.OrganizationName, subOrganization.GradeName);

        }

        public static string GetLogisticName(string organizationName, string gradeName)
        {
            return string.Format("{0}（{1}）", organizationName, gradeName);
        }

        public string LogisticName { get; private set; }

        //public List<Order> Split(Basket baseket);

        /// <summary>
        /// 子业务集合
        /// Key：RuleID
        /// Value：RuleSequence对象
        /// </summary>
        public Dictionary<string, RuleEntity> RuleSequenceDic { get; private set; }

        public void AddRuleSequenceDic(RuleEntity rule)
        {
            if (this.RuleSequenceDic == null)
            {
                this.RuleSequenceDic = new Dictionary<string, RuleEntity>();
            }

            this.RuleSequenceDic.Add(rule.Key, rule);
        }

        public Logistic(SplitPackage.Business.Logistic logistic)
        {
            this.LogisticName = logistic.LogisticCode;
            this.RuleSequenceDic = new Dictionary<string, RuleEntity>();
            foreach (var item in logistic.LogisticChannels)
            {
                this.RuleSequenceDic.Add(item.ChannelName, new RuleEntity(item));
            }
        }

        public Logistic(LogisticCacheDto logistic)
        {
            this.LogisticName = logistic.LogisticCode;
            this.RuleSequenceDic = new Dictionary<string, RuleEntity>();
            foreach (var item in logistic.LogisticChannels)
            {
                this.RuleSequenceDic.Add(item.ChannelName, new RuleEntity(item, logistic));
            }
        }
    }
}
