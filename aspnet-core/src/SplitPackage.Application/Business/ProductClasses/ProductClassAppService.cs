using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.ProductClasses.Dto;
using SplitPackage.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.ProductClasses
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_ProductClasses)]
    public class ProductClassAppService : ApplicationService, IProductClassAppService
    {
        public ProductClassAppService()
        {

        }

        [AbpAllowAnonymous]
        public async Task<List<OptionDto>> Query(QueryRequire<long> req) => throw new NotImplementedException();
    }
}
