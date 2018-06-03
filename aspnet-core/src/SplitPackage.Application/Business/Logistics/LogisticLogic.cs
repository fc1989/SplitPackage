using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Business.Logistics.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.Logistics
{
    public class LogisticLogic : ILogisticLogic, ITransientDependency
    {
        private readonly IRepository<Logistic, long> _repository;
        private readonly IRepository<LogisticChannel, long> _logisticChannelRepository;
        private readonly IRepository<TenantLogisticChannel, long> _tenantLogisticChannelRepository;
        private readonly IRepository<LogisticRelated, long> _logisticRelatedRepository;
        public IObjectMapper ObjectMapper { get; set; }

        public LogisticLogic(
            IRepository<Logistic, long> repository,
            IRepository<LogisticChannel, long> logisticChannelRepository,
            IRepository<TenantLogisticChannel, long> tenantLogisticChannelRepository,
            IRepository<LogisticRelated, long> logisticRelatedRepository) {
            this._repository = repository;
            this._logisticChannelRepository = logisticChannelRepository;
            this._tenantLogisticChannelRepository = tenantLogisticChannelRepository;
            this._logisticRelatedRepository = logisticRelatedRepository;
            this.ObjectMapper = NullObjectMapper.Instance;
        }

        public async Task<Logistic> Create(CreateLogisticDto input, int? tenantId)
        {
            var entity = ObjectMapper.Map<Logistic>(input);
            entity.TenantId = tenantId;
            if (tenantId.HasValue)
            {
                entity.LogisticCode = string.Format("{0}_{1}", tenantId.Value, entity.LogisticCode);
            }
            await this._repository.InsertAsync(entity);
            return entity;
        }

        [UnitOfWork]
        public IQueryable<Logistic> GetQuery(int? tenantId)
        {
            IQueryable<Logistic> query;
            if (tenantId.HasValue)
            {
                var LogisticQuery = from l in this._repository.GetAll().IgnoreQueryFilters()
                        join ll in _logisticChannelRepository.GetAll().IgnoreQueryFilters() on l.Id equals ll.LogisticId into left1
                        from tb1 in left1.DefaultIfEmpty()
                        join tl in _tenantLogisticChannelRepository.GetAll().IgnoreQueryFilters() on tb1.Id equals tl.LogisticChannelId into left2
                        from tb2 in left2.DefaultIfEmpty()
                        where l.TenantId == tenantId || tb2.TenantId == tenantId
                        select l.Id;
                query = this._repository.GetAll().IgnoreQueryFilters().Where(o => LogisticQuery.Contains(o.Id));
            }
            else
            {
                query = this._repository.GetAll().Where(o=>o.TenantId == tenantId);
            }
            return query;
        }

        public async Task<bool> Verify(int? tenantId, string logisticCode)
        {
            if (tenantId.HasValue)
            {
                logisticCode = string.Format("{0}_{1}", tenantId.Value, logisticCode);
            }
            var count = await this._repository.GetAll().Where(o => o.LogisticCode == logisticCode).CountAsync();
            return count > 0 ? false: true;
        }

        public IQueryable<LogisticChannel> GetLogisticChannels(int? tenantId)
        {
            IQueryable<LogisticChannel> query;
            if (tenantId.HasValue)
            {
                query = from l in this._repository.GetAll().IgnoreQueryFilters()
                        join ll in _logisticChannelRepository.GetAll().IgnoreQueryFilters() on l.Id equals ll.LogisticId
                        join tl in _tenantLogisticChannelRepository.GetAll().IgnoreQueryFilters() on ll.Id equals tl.LogisticChannelId into left1
                        from tb in left1.DefaultIfEmpty()
                        where !l.IsDeleted && !ll.IsDeleted && (l.TenantId == tenantId || tb.TenantId == tenantId)
                        select ll;
            }
            else
            {
                query = this._logisticChannelRepository.GetAll().Where(o => o.TenantId == tenantId);
            }
            return query;
        }

        public IQueryable<TenantLogisticChannel> GetTenantLogisticChannels(int? tenantId)
        {
            return this._tenantLogisticChannelRepository.GetAll().IgnoreQueryFilters().Where(o => o.TenantId == tenantId);
        }

        [UnitOfWork]
        public IQueryable<LogisticRelated> GetLogisticRelateds(int? tenantId)
        {
            return this._logisticRelatedRepository.GetAll().IgnoreQueryFilters().Where(o => o.TenantId == tenantId);
        }
    }
}
