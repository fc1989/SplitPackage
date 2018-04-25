using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using SplitPackage.Business.Products.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Abp.Authorization;
using SplitPackage.Authorization;

namespace SplitPackage.Business.Products
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_Products)]
    public class ProductAppService : AsyncCrudAppService<Product, ProductDto, long, PagedResultRequestDto, CreateProductDto, UpdateProductDto>, IProductAppService
    {
        protected IRepository<ProductProductClass, long> PPCRepository;
        protected static IMapper PMapper = new MapperConfiguration(cfg => {
            cfg.CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ProductClassIds,
                    opt => opt.MapFrom(src => src.ProductClasses.Select(o => o.ProductClassId).ToList()));
            cfg.CreateMap<UpdateProductDto, Product>();
        }).CreateMapper();

        public ProductAppService(IRepository<Product, long> repository, IRepository<ProductProductClass, long> ppcRepository) : base(repository)
        {
            this.PPCRepository = ppcRepository;
        }

        public override async Task<PagedResultDto<ProductDto>> GetAll(PagedResultRequestDto input)
        {
            CheckGetAllPermission();

            var query = CreateFilteredQuery(input);

            var totalCount = await AsyncQueryableExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncQueryableExecuter.ToListAsync(query.Include(p=>p.ProductClasses));

            return new PagedResultDto<ProductDto>(
                totalCount,
                entities.Select(o=> PMapper.Map<ProductDto>(o)).ToList()
            );
        }

        public override async Task<ProductDto> Create(CreateProductDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);
            entity.TenantId = AbpSession.TenantId;
            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();
            foreach (var item in input.ProductClassIds ?? new List<long>())
            {
                await this.PPCRepository.InsertAsync(new ProductProductClass()
                {
                    ProductId = entity.Id,
                    ProductClassId = item
                });
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            return PMapper.Map<ProductDto>(entity);
        }

        public override async Task<ProductDto> Update(UpdateProductDto input)
        {
            CheckUpdatePermission();

            var entity = this.Repository.GetAll().Include(p=>p.ProductClasses).FirstOrDefault(o => o.Id == input.Id);
            PMapper.Map(input, entity);

            //insert
            var icl = input.ProductClassIds.Where(o => !entity.ProductClasses.Any(oi => oi.ProductClassId == o)).ToList();
            foreach (var item in icl)
            {
                await this.PPCRepository.InsertAsync(new ProductProductClass()
                {
                    ProductId = entity.Id,
                    ProductClassId = item
                });
            }

            //delete
            var dcl = entity.ProductClasses.Where(o => !input.ProductClassIds.Contains(o.ProductClassId)).Select(o=>o.Id).ToList();
            if (dcl.Count > 0)
            {
                await this.PPCRepository.DeleteAsync(o => dcl.Contains(o.Id));
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return PMapper.Map<ProductDto>(entity);
        }

        public async Task<bool> Verify(string sku)
        {
            var count = await this.Repository.GetAll().Where(o=>o.Sku == sku).CountAsync();
            if (count > 0)
                return false;
            else
                return true;
        }
    }
}
