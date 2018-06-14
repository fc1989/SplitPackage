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
using Abp.UI;

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
            return new SpliterV1(setting.OwnLogistics, setting.Relateds.Select(o=>o.Logistics).ToList());
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
            if (setting == null)
            {
                return new List<LogisticCacheDto>();
            }
            return setting?.OwnLogistics;
        }

        protected async Task ValidRequire<T>(T request, int? tenantId) where T : BaseRequest
        {
            //非空验证
            if (request == null)
            {
                throw new UserFriendlyException((int)ResultCode.BadRequest_ParamEmpty, "request model is not null");
            }
            // 待拆分商品清单有效性验证
            if ((request.ProList == null) || (request.ProList.Count <= 0))
            {
                throw new UserFriendlyException((int)ResultCode.BadRequest_ParamConstraint, "product list's length must more than zero");
            }
            //商品价格
            if (request.ProList.Any(o => o.ProPrice <= 0))
            {
                throw new UserFriendlyException((int)ResultCode.BadRequest_ParamConstraint, "product's price must more then zero");
            }
            //商品重量
            if (request.ProList.Any(o => o.Weight <= 0))
            {
                throw new UserFriendlyException((int)ResultCode.BadRequest_ParamConstraint, "product's weight must more then zero");
            }
            //商品数量
            if (request.ProList.Any(o => o.Quantity <= 0))
            {
                throw new UserFriendlyException((int)ResultCode.BadRequest_ParamConstraint, "product's quantity must more then zero");
            }
            if (request.ProList.Any(o => string.IsNullOrEmpty(o.PTId)))
            {
                throw new UserFriendlyException((int)ResultCode.BadRequest_ParamConstraint, "product's ptid is necessary");
            }
            if (request is SplitWithExpRequest1)
            {
                List<string> requestLogistics = (request as SplitWithExpRequest1).logistics;
                if (requestLogistics == null || requestLogistics.Count == 0)
                {
                    throw new UserFriendlyException((int)ResultCode.BadRequest_ParamConstraint, "logistic must more then zero");
                }
                var logisticsCodes = requestLogistics.Distinct();
                var includeLogisticsCodes = (await this.GetOwnLogistics(tenantId)).Select(o => o.LogisticCode).Distinct();
                var unDeploylogisticsIds = logisticsCodes.Where(o => !includeLogisticsCodes.Contains(o)).ToList();
                if (unDeploylogisticsIds.Count > 0)
                {
                    throw new UserFriendlyException((int)ResultCode.BadRequest_ParamConstraint, string.Format("the following logistics providers do not exist:{0}", string.Join(",", unDeploylogisticsIds)));
                }
            }
            var requestPTIds = request.ProList.Select(o => o.PTId).Distinct().ToList();
            var includePTIds = await this.GetRulePTIds(tenantId);
            var unDeployPTIds = requestPTIds.Where(o => !includePTIds.Contains(o)).ToList();
            if (unDeployPTIds.Count > 0)
            {
                throw new UserFriendlyException((int)ResultCode.BadRequest_ParamConstraint, string.Format("the following ptid no corresponding rules:{0}", string.Join(",", unDeployPTIds.Distinct())));
            }
        }

        [UnitOfWork]
        public async Task<SplitedOrder> Split(SplitRequest request, int? tenantId)
        {
            await this.ValidRequire(request,tenantId);
            var spliter = await this.GetSpliter(tenantId);
            return spliter.Split(request.OrderId, request.ProList, request.TotalQuantity, request.Type);
        }

        [UnitOfWork]
        public async Task<SplitedOrder> SplitWithOrganization1(SplitWithExpRequest1 request, int? tenantId)
        {
            await this.ValidRequire(request,tenantId);
            var spliter = await this.GetSpliter(tenantId);
            return spliter.SplitWithOrganization1(request.OrderId.ToString(), request.ProList, request.TotalQuantity, request.logistics);
        }

        [UnitOfWork]
        public async Task<List<LogisticsModel>> GetLogisticsList(int? tenantId)
        {
            return (await this.GetOwnLogistics(tenantId)).Select(o => new LogisticsModel()
            {
                ID = o.LogisticCode,
                Name = o.CorporationName,
                URL = o.CorporationUrl,
                LogoURL = o.LogoURL
            }).ToList();
        }

        public async Task<List<ProductSortSimpleDto1>> GetProductClass()
        {
            var productClass = await this._cacheManager.GetProductClassAsync();
            productClass = productClass.Where(o => o.IsActive).ToList();
            return productClass.GroupBy(o => new { o.ProductSortId, o.SortName }).Select(o=>new ProductSortSimpleDto1() {
                SortName = o.Key.SortName,
                Items = o.Select(oi=>new ProductClassSimpleDto() {
                    PTId = oi.PTId,
                    ClassName = oi.ClassName
                }).ToList()
            }).ToList();
        }
    }
}
