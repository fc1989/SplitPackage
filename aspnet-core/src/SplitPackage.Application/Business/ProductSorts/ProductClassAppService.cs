using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.Dto;
using SplitPackage.Business.ProductSorts.Dto;
using SplitPackage.Cache;
using SplitPackage.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.ProductSorts
{

    [AbpAuthorize(PermissionNames.Pages_Admin_ProductSorts)]
    public class ProductClassAppService : AsyncCrudAppService<ProductClass, ProductClassDto, long, ProductClassSearchFilter, CreateProductClassDto, UpdateProductClassDto>
    {
        private readonly ManageCache _manageCache;

        public ProductClassAppService(IRepository<ProductClass, long> repository,
            ManageCache manageCache) : base(repository)
        {
            this._manageCache = manageCache;
        }

        protected override IQueryable<ProductClass> CreateFilteredQuery(ProductClassSearchFilter input)
        {
            return base.CreateFilteredQuery(input).Where(input.GenerateFilter());
        }

        public async override Task Delete(EntityDto<long> input)
        {
            await this.Repository.DeleteAsync(o => o.Id == input.Id);
        }

        [AbpAllowAnonymous]
        public async Task<List<OptionDto<string>>> Query(QueryRequire<long> req)
        {
            Expression<Func<ProductClass, bool>> filter;
            if (!string.IsNullOrEmpty(req.Flag) && (req.Ids == null || req.Ids.Count == 0))
            {
                filter = o => o.PTId.StartsWith(req.Flag) || o.ClassName.StartsWith(req.Flag);
            }
            else if (string.IsNullOrEmpty(req.Flag) && (req.Ids != null || req.Ids.Count > 0))
            {
                filter = o => req.Ids.Contains(o.Id);
            }
            else if (string.IsNullOrEmpty(req.Flag) && (req.Ids == null || req.Ids.Count == 0))
            {
                filter = o => true;
            }
            else
            {
                filter = o => o.PTId.StartsWith(req.Flag) || o.ClassName.StartsWith(req.Flag) || req.Ids.Contains(o.Id);
            }
            return await this.Repository.GetAll().Where(filter).Take(20).Select(o => new OptionDto<string>
            {
                Value = o.Id.ToString(),
                Label = string.Format("{0}[{1}]", o.ClassName, o.PTId)
            }).ToListAsync();
        }

        [AbpAllowAnonymous]
        public async Task<List<Option>> GetOptional()
        {
            var pcSet = await this._manageCache.GetProductClassAsync();
            return await Task.FromResult(pcSet.GroupBy(o => new {
                o.ProductSortId,
                o.SortName
            }).OrderBy(o=>o.Key.ProductSortId).Select(o => new Option()
            {
                Value = o.Key.ProductSortId.ToString(),
                label = o.Key.SortName,
                Children = o.OrderBy(oi=>oi.ClassName).Select(oi => new Option()
                {
                    Value = oi.PTId,
                    label = oi.ClassName,
                }).ToList()
            }).ToList());
        }

        public async Task<bool> VerifyPTId(long productSortId, string ptid)
        {
            var count = await this.Repository.GetAll().Where(o => o.ProductSortId != productSortId && o.PTId == ptid).CountAsync();
            if (count > 0)
                return false;
            else
                return true;
        }

        public async Task<bool> VerifyClassName(long productSortId, string className)
        {
            var count = await this.Repository.GetAll().Where(o => o.ProductSortId == productSortId && o.ClassName == className).CountAsync();
            if (count > 0)
                return false;
            else
                return true;
        }
    }
}
