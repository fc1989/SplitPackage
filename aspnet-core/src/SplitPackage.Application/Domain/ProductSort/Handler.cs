using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Handlers;
using SplitPackage.Business;
using SplitPackage.Domain.ProductSort;
using SplitPackage.Cache;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SplitPackage.Cache.Dto;
using Microsoft.EntityFrameworkCore;
using Abp.Events.Bus.Entities;

namespace SplitPackage.Domain.ProductSort
{
    public class Handler :
        IAsyncEventHandler<EntityUpdatedEventData<Business.ProductSort>>,
        IAsyncEventHandler<EntityDeletedEventData<Business.ProductSort>>,
        IAsyncEventHandler<EntityCreatedEventData<ProductClass>>,
        IAsyncEventHandler<EntityUpdatedEventData<ProductClass>>,
        IAsyncEventHandler<EntityDeletedEventData<ProductClass>>, ITransientDependency
    {
        private readonly IRepository<Business.ProductSort, long> _productSortRepository;
        private readonly IRepository<ProductClass, long> _productClassRepository;
        private readonly ManageCache _manageCache;

        public Handler(IRepository<Business.ProductSort, long> productSortRepository,
            IRepository<ProductClass, long> productClassRepository,
            ManageCache manageCache)
        {
            this._productSortRepository = productSortRepository;
            this._productClassRepository = productClassRepository;
            this._manageCache = manageCache;
        }

        public async Task HandleEventAsync(EntityUpdatedEventData<Business.ProductSort> eventData)
        {
            var pcSet = await this._manageCache.GetProductClassAsync();
            if (eventData.Entity.IsActive)
            {
                bool isContain = pcSet.Any(o => o.ProductSortId == eventData.Entity.Id);
                if (isContain)
                {
                    pcSet.Where(o => o.ProductSortId == eventData.Entity.Id).ToList().ForEach(o =>
                    {
                        o.SortName = eventData.Entity.SortName;
                    });
                }
                else
                {
                    var addPcSet = await this._productClassRepository.GetAllListAsync(o => o.ProductSortId == eventData.Entity.Id);
                    pcSet = pcSet.Union(addPcSet.Select(o => new ProductClassCacheDto()
                    {
                        ProductSortId = o.ProductSortId,
                        ProductClassId = o.Id,
                        SortName = eventData.Entity.SortName,
                        ClassName = o.ClassName,
                        PTId = o.PTId,
                        PostTaxRate = o.PostTaxRate,
                        BCTaxRate = o.BCTaxRate
                    })).ToList();
                }
            }
            else
            {
                pcSet = pcSet.Where(o => o.ProductSortId != eventData.Entity.Id).ToList();
            }
            await this._manageCache.SetProductClassAsync(pcSet);
        }

        public async Task HandleEventAsync(EntityDeletedEventData<Business.ProductSort> eventData)
        {
            var pcSet = await this._manageCache.GetProductClassAsync();
            pcSet = pcSet.Where(o => o.ProductSortId != eventData.Entity.Id).ToList();
            await this._manageCache.SetProductClassAsync(pcSet);
        }

        public async Task HandleEventAsync(EntityCreatedEventData<ProductClass> eventData)
        {
            var pcSet = await this._manageCache.GetProductClassAsync();
            var ps = await this._productSortRepository.GetAsync(eventData.Entity.ProductSortId);
            pcSet.Add(new ProductClassCacheDto()
            {
                ProductSortId = eventData.Entity.ProductSortId,
                ProductClassId = eventData.Entity.Id,
                SortName = ps.SortName,
                ClassName = eventData.Entity.ClassName,
                PTId = eventData.Entity.PTId,
                PostTaxRate = eventData.Entity.PostTaxRate,
                BCTaxRate = eventData.Entity.BCTaxRate
            });
            await this._manageCache.SetProductClassAsync(pcSet);
        }

        public async Task HandleEventAsync(EntityUpdatedEventData<ProductClass> eventData)
        {
            var pcSet = await this._manageCache.GetProductClassAsync();
            var pc = pcSet.Where(o=>o.ProductClassId == eventData.Entity.Id).FirstOrDefault();
            if (eventData.Entity.IsActive)
            {
                if (pc == null)
                {
                    var ps = await this._productSortRepository.GetAsync(eventData.Entity.ProductSortId);
                    pcSet.Add(new ProductClassCacheDto()
                    {
                        ProductSortId = eventData.Entity.ProductSortId,
                        ProductClassId = eventData.Entity.Id,
                        SortName = ps.SortName,
                        ClassName = eventData.Entity.ClassName,
                        PTId = eventData.Entity.PTId,
                        PostTaxRate = eventData.Entity.PostTaxRate,
                        BCTaxRate = eventData.Entity.BCTaxRate
                    });
                }
                else
                {
                    pc.ClassName = eventData.Entity.ClassName;
                    pc.PTId = eventData.Entity.PTId;
                    pc.PostTaxRate = eventData.Entity.PostTaxRate;
                    pc.BCTaxRate = eventData.Entity.BCTaxRate;
                }
            }
            else
            {
                pcSet.Remove(pc);
            }
            await this._manageCache.SetProductClassAsync(pcSet);
        }

        public async Task HandleEventAsync(EntityDeletedEventData<ProductClass> eventData)
        {
            var pcSet = await this._manageCache.GetProductClassAsync();
            pcSet = pcSet.Where(o => o.ProductClassId != eventData.Entity.Id).ToList();
            await this._manageCache.SetProductClassAsync(pcSet);
        }
    }
}
