using SplitPackage.Cache.Dto;
using SplitPackage.Split.RuleModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.SplitV1
{
    public class Logistic
    {
        public string LogisticName { get; private set; }

        /// <summary>
        /// 子业务集合
        /// Key：RuleID
        /// Value：RuleSequence对象
        /// </summary>
        public Dictionary<string, RuleEntity> RuleSequenceDic { get; private set; }

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
