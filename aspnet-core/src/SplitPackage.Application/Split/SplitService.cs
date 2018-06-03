using Abp.Application.Services;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Logging;
using SplitPackage.Business;
using SplitPackage.Business.Logistics;
using SplitPackage.Business.SplitRules;
using SplitPackage.Split.Dto;
using SplitPackage.Split.RuleModels;
using SplitPackage.Split.SplitModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using SplitPackage.Cache.Dto;
using SplitPackage.Cache;

namespace SplitPackage.Split
{
    public class SplitService : ISplitService,ITransientDependency
    {
        private readonly ManageCache _cacheManager;

        public SplitService(ManageCache cacheManager) {
            this._cacheManager = cacheManager;
        }

        private async Task<SpliterV1> GetSpliter(int? tenantId)
        {
            var setting = await this._cacheManager.GetSplitPackageSettingAsync(tenantId);
            return new SpliterV1(setting.OwnLogistics, setting.Relateds);
        }

        private async Task<List<string>> GetRulePTIds(int? tenantId)
        {
            var setting = await this._cacheManager.GetSplitPackageSettingAsync(tenantId);
            var productClass = await this._cacheManager.GetProductClassAsync();
            var rulePTIds = setting.OwnLogistics.SelectMany(o => o.LogisticChannels.SelectMany(oi => oi.SplitRules.SelectMany(oii => oii.ProductClasses.Select(oiii => oiii.PTId))))
                .Distinct().ToList();
            var result = (from rule in rulePTIds
             join pc in productClass on rule equals pc.PTId
             where pc.IsActive
             select rule).ToList();
            return result;
        }

        private async Task<IList<LogisticCacheDto>> GetOwnLogistics(int? tenantId)
        {
            var setting = await this._cacheManager.GetSplitPackageSettingAsync(tenantId);
            return setting.OwnLogistics;
        }

        protected async Task<Tuple<bool, string>> ValidRequire<T>(T request, int? tenantId) where T : BaseRequest
        {
            //非空验证
            if (request == null)
            {
                return Tuple.Create(false, "Request model is null");
            }
            // 待拆分商品清单有效性验证
            if ((request.ProList == null) || (request.ProList.Count <= 0))
            {
                return Tuple.Create(false, "Product list is null");
            }
            //商品价格
            if (request.ProList.Any(o => o.ProPrice <= 0))
            {
                return Tuple.Create(false, "商品价格必须大于0");
            }
            //商品重量
            if (request.ProList.Any(o => o.Weight <= 0))
            {
                return Tuple.Create(false, "商品重量必须大于0");
            }
            //商品数量
            if (request.ProList.Any(o => o.Quantity <= 0))
            {
                return Tuple.Create(false, "商品数量必须大于0");
            }
            if (request.ProList.Any(o => string.IsNullOrEmpty(o.PTId)))
            {
                return Tuple.Create(false, "缺少PTId");
            }
            var requestPTIds = request.ProList.Select(o => o.PTId).Distinct().ToList();
            var includePTIds = await this.GetRulePTIds(tenantId);
            var unDeployPTIds = requestPTIds.Where(o => !includePTIds.Contains(o)).ToList();
            if (unDeployPTIds.Count > 0)
            {
                return Tuple.Create(false, string.Format("不存在PTId:{0}", string.Join(",", unDeployPTIds.Distinct())));
            }
            if (request is SplitWithExpRequest1)
            {
                List<string> requestLogistics = (request as SplitWithExpRequest1).logistics;
                if (requestLogistics == null || requestLogistics.Count == 0)
                {
                    return Tuple.Create(false, "请提供指定物流商");
                }
                var logisticsCodes = requestLogistics.Distinct();
                var includeLogisticsCodes = (await this.GetOwnLogistics(tenantId)).Select(o=>o.LogisticCode).Distinct();
                var unDeploylogisticsIds = logisticsCodes.Where(o => !includeLogisticsCodes.Contains(o)).ToList();
                if (unDeploylogisticsIds.Count > 0)
                {
                    return Tuple.Create(false, string.Format("指定物流商:{0}不存在", string.Join(",", unDeploylogisticsIds)));
                }
            }
            return Tuple.Create(true, string.Empty);
        }

        [UnitOfWork]
        public async Task<Tuple<string, SplitedOrder>> Split(SplitRequest request, int? tenantId)
        {
            var validResult = await this.ValidRequire(request,tenantId);
            if (!validResult.Item1)
            {
                return Tuple.Create<string, SplitedOrder>(validResult.Item2, null);
            }
            var spliter = await this.GetSpliter(tenantId);
            var result = spliter.Split(request.OrderId, request.ProList, request.TotalQuantity, request.Type);
            return Tuple.Create(string.Empty, result);
        }

        [UnitOfWork]
        public async Task<Tuple<string, SplitedOrder>> SplitWithOrganization1(SplitWithExpRequest1 request, int? tenantId)
        {
            var validResult = await this.ValidRequire(request,tenantId);
            if (!validResult.Item1)
            {
                return Tuple.Create<string, SplitedOrder>(validResult.Item2, null);
            }
            var spliter = await this.GetSpliter(tenantId);
            SplitedOrder result = spliter.SplitWithOrganization1(request.OrderId.ToString(), request.ProList, request.TotalQuantity, request.logistics);
            return Tuple.Create(string.Empty, result);
        }

        [UnitOfWork]
        public async Task<Tuple<string, List<LogisticsModel>>> GetLogisticsList(int? tenantId)
        {
            return Tuple.Create(string.Empty, (await this.GetOwnLogistics(tenantId)).Select(o => new LogisticsModel()
            {
                ID = o.LogisticCode,
                Name = o.CorporationName,
                URL = o.CorporationUrl,
                LogoURL = o.LogoURL
            }).ToList());
        }

        public async Task<List<ProductSortDto>> GetProductClass()
        {
            var productClass = await this._cacheManager.GetProductClassAsync();
            productClass = productClass.Where(o => o.IsActive).ToList();
            return productClass.GroupBy(o => new { o.ProductSortId, o.SortName }).Select(o=>new ProductSortDto() {
                SortName = o.Key.SortName,
                Items = o.Select(oi=>new ProductClassDto() {
                    PTId = oi.PTId,
                    ClassName = oi.ClassName
                }).ToList()
            }).ToList();
        }
    }
}
