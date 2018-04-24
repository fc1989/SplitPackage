using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Business.ProductClasses.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.ProductClasses
{
    public class ProductClassAppService : AsyncCrudAppService<ProductClass, ProductClassDto, long, PagedResultRequestDto, CreateProductClassDto, UpdateProductClassDto>, IProductClassAppService
    {
        public ProductClassAppService(IRepository<ProductClass, long> repository) : base(repository)
        {

        }

        public async Task<bool> Verify(string ptid)
        {
            var count = await this.Repository.GetAll().Where(o => o.PTId == ptid).CountAsync();
            if (count > 0)
                return false;
            else
                return true;
        }


        public async Task<object> Query(QueryRequire<long> req)
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
            return await this.Repository.GetAll().Where(filter).Take(20).Select(o=>new {
                value = o.Id,
                label = string.Format("{0}[{1}]",o.ClassName,o.PTId)
            }).ToListAsync();
        }
    }
}
