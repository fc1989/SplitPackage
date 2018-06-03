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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.ProductSorts
{
    [AbpAuthorize(PermissionNames.Pages_Admin_ProductSorts)]
    public class ProductSortAppService : AsyncCrudAppService<ProductSort, ProductSortDto, long, ProductSortSearchFilter, CreateProductSortDto, UpdateProductSortDto>
    {
        private readonly IRepository<ProductClass, long> _pcRepository;

        public ProductSortAppService(IRepository<ProductSort, long> repository, IRepository<ProductClass, long> pcRepository) : base(repository)
        {
            this._pcRepository = pcRepository;
        }

        protected override IQueryable<ProductSort> CreateFilteredQuery(ProductSortSearchFilter input)
        {
            var param = Expression.Parameter(typeof(ProductSort), "o");
            Expression filter = Expression.Constant(true);
            if (!string.IsNullOrEmpty(input.SortName))
            {
                Expression right  = Expression.Call(Expression.Property(param, nameof(ProductSort.SortName)), typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), Expression.Constant(input.SortName));
                filter = Expression.AndAlso(filter, right);
            }
            if (!string.IsNullOrEmpty(input.PTId) || !string.IsNullOrEmpty(input.ClassName))
            {
                var pcParam = Expression.Parameter(typeof(ProductClass), "oi");
                Expression pcfilter = null;
                if (!string.IsNullOrEmpty(input.PTId))
                {
                    pcfilter = Expression.Call(Expression.Property(pcParam, nameof(ProductClass.PTId)), typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), Expression.Constant(input.PTId));
                }
                if (!string.IsNullOrEmpty(input.ClassName))
                {
                    Expression pcright = Expression.Call(Expression.Property(pcParam, nameof(ProductClass.ClassName)), typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), Expression.Constant(input.ClassName));
                    pcfilter = pcfilter == null ? pcright : Expression.AndAlso(pcfilter,pcright);
                }
                var anyMethod = typeof(Enumerable).GetMethods().Where(o => o.Name.Equals("Any") && o.GetParameters().Count()>1).FirstOrDefault();
                var any = anyMethod.MakeGenericMethod(typeof(ProductClass));
                Expression right = Expression.Call(any, Expression.Property(param, nameof(ProductSort.Items)), Expression.Lambda<Func<ProductClass,bool>>(pcfilter, pcParam));
                filter = Expression.AndAlso(filter, right);
            }
            return Repository.GetAll().Where(Expression.Lambda<Func<ProductSort,bool>>(filter,param));
        }

        public async Task<bool> Verify(string flag)
        {
            var count = await this.Repository.GetAll().Where(o => o.SortName == flag).CountAsync();
            if (count > 0)
                return false;
            else
                return true;
        }
    }
}
