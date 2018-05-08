using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;

namespace SplitPackage.Dto
{
    public class PageSearchFilter<T> : PagedResultRequestDto where T : class
    {
        public Expression<Func<T, bool>> GenerateFilter()
        {
            var conditions = this.GetType().GetProperties().Where(o => !typeof(PagedResultRequestDto).GetProperties().Any(oi=>oi.Name.Equals(o.Name)));
            Expression filter = null;
            var param = Expression.Parameter(typeof(T), "o");
            foreach (var item in conditions)
            {
                object value = item.GetValue(this);
                if (value == null)
                {
                    continue;
                }
                var e = Expression.Call(Expression.Property(param, item.Name), item.PropertyType.GetMethod("Equals", new Type[]{ item.PropertyType}), Expression.Constant(value));
                if (filter == null)
                {
                    filter = e;
                }
                else
                {
                    filter = Expression.AndAlso(filter, e);
                }
            }
            return filter == null ? null : Expression.Lambda<Func<T, bool>>(filter, param);
        }
    }
}
