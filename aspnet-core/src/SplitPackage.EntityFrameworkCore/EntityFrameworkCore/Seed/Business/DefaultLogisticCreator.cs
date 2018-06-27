using SplitPackage.Business;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using SplitPackage.EntityFrameworkCore.Seed.Business.RuleModels;
using System.Xml.Serialization;
using System.Text;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class DefaultLogisticCreator
    {
        private readonly SplitPackageDbContext _context;

        public DefaultLogisticCreator(SplitPackageDbContext context)
        {
            this._context = context;
        }

        public void Create()
        {
            if (_context.Logistics.Any())
            {
                return;
            }
            string initXmlDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "xml");
            var rules = this.LoadRules(initXmlDirectory);
            foreach (var item in rules)
            {
                var l = new Logistic()
                {
                    CorporationName = item.OrganizationName,
                    LogisticCode = item.OrganizationName,
                    CorporationUrl = item.URL,
                    LogoURL = item.LogoURL,
                    IsDeleted = false
                };
                List<LogisticChannel> lcSet = new List<LogisticChannel>();
                foreach (var subo in item.SubOrganizations)
                {
                    foreach (var rule in subo.Rules)
                    {
                        var lc = new LogisticChannel()
                        {
                            ChannelName = rule.SubBusinessName,
                            Type = ChannelType.CC,
                            Way = ChargeWay.ChargeByWeight,
                            IsDeleted = false,
                            LogisticBy = l
                        };
                        lc.WeightFreights = new List<WeightFreight>()
                        {
                            new WeightFreight(){
                                Currency = "AUD",
                                Unit = "g",
                                StartingWeight = rule.StartingWeight,
                                EndWeight = 10000000,
                                StartingPrice = rule.StartingPrice,
                                StepWeight = rule.StepWeight,
                                CostPrice = rule.Price,
                                Price = rule.Price,
                                LogisticChannelBy = lc,
                                IsDeleted = false
                            }
                        };
                        var srSet = new List<SplitRule>();
                        rule.MixRule.ForEach(o=> {
                            var sr = new SplitRule() {
                                RuleName = o.MRId.ToString(),
                                MaxPackage = o.LimitedQuantity,
                                MaxWeight = o.LimitedWeight,
                                MaxTax = o.TaxThreshold,
                                MaxPrice = o.LimitedMaxPrice,
                                CreationTime = DateTime.Now,
                                LogisticChannelBy = lc,
                            };
                            sr.ProductClasses = o.RuleItems.Select(oi => new SplitRuleItem() {
                                SplitRuleBy = sr,
                                StintMark = oi.PTId.ToString(),
                                MaxNum = oi.MaxQuantity,
                                MinNum = oi.MinQuantity
                            }).ToList();
                            srSet.Add(sr);
                        });
                        lc.SplitRules = srSet;
                        lcSet.Add(lc);
                    }
                }
                l.LogisticChannels = lcSet;
                _context.Logistics.Add(l);
            }
            _context.SaveChanges();
        }

        private T LoadXmlFile<T>(string filePath)
        {
            T result;
            StreamReader fs = null;
            var serializer = new XmlSerializer(typeof(T));
            try
            {
                fs = new StreamReader(filePath, Encoding.UTF8);
                result = (T)serializer.Deserialize(fs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                {
                    // 关闭文件
                    fs.Close();
                }
            }

            //Log.Info("文件读取结束");
            return result;
        }

        private List<SplitPackageConfig> LoadRules(string folder)
        {
            List<SplitPackageConfig> rules = new List<SplitPackageConfig>();
            string rulesFolder = folder;
            if (!Directory.Exists(folder))
            {
                return rules;
            }
            string[] files = Directory.GetFiles(rulesFolder, "*.xml", SearchOption.AllDirectories);
            foreach (string filePath in files)
            {
                SplitPackageConfig rule = this.LoadXmlFile<SplitPackageConfig>(filePath);
                rules.Add(rule);
            }
            return rules;
        }
    }
}
