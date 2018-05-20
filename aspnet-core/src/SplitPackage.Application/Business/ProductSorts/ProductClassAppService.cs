using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.Dto;
using SplitPackage.Business.ProductSorts.Dto;
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
    public class ProductClassAppService : AsyncCrudAppService<ProductClass, ProductClassDto, long, PagedResultRequestDto, CreateProductClassDto, UpdateProductClassDto>
    {
        public ProductClassAppService(IRepository<ProductClass, long> repository) : base(repository)
        {

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
            var tenantId = AbpSession.TenantId;
            var query = this.Repository.GetAll().Where(o=> o.IsActive);
            query.Include(p => p.ProductSortBy);
            return await Task.FromResult(query.GroupBy(o => o.ProductSortBy).Select(o => new Option()
            {
                Value = o.Key.Id.ToString(),
                label = o.Key.SortName,
                Children = o.Select(oi => new Option()
                {
                    Value = oi.PTId,
                    label = oi.ClassName,
                }).ToList()
            }).ToList());
        }
    }
}
