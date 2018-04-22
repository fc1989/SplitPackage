using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Business.ProductClasses.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
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


        public async Task<object> Query(string flag)
        {
            return await this.Repository.GetAll().Where(o => o.PTId.StartsWith(flag) || o.ClassName.StartsWith(flag)).Take(20).Select(o=>new {
                value = o.Id,
                label = string.Format("{0}[{1}]",o.ClassName,o.PTId)
            }).ToListAsync();
        }
    }
}
