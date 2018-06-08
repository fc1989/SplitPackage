using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.IdentityFramework;
using Abp.Localization;
using Microsoft.AspNetCore.Identity;
using SplitPackage.Authorization.Roles;
using SplitPackage.Authorization.Users;
using SplitPackage.Business;
using SplitPackage.MultiTenancy.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SplitPackage.Domain.Logistic;

namespace SplitPackage.MultiTenancy
{
    public class TenantService : ITenantService, ITransientDependency
    {
        private readonly TenantManager _tenantManager;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ILocalizationManager _localizationManager;
        private readonly IRepository<LogisticChannel, long> _lcRepository;
        private readonly IRepository<TenantLogisticChannel, long> _tlcRepository;
        private readonly IEventBus _eventBus;

        public TenantService(
            TenantManager tenantManager,
            UserManager userManager,
            RoleManager roleManager,
            IPasswordHasher<User> passwordHasher,
            IUnitOfWorkManager unitOfWorkManager,
            ILocalizationManager localizationManager,
            IRepository<LogisticChannel, long> lcRepository,
            IRepository<TenantLogisticChannel, long> tlcRepository,
            IEventBus eventBus)
        {
            this._tenantManager = tenantManager;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._passwordHasher = passwordHasher;
            this._unitOfWorkManager = unitOfWorkManager;
            this._localizationManager = localizationManager;
            this._lcRepository = lcRepository;
            this._tlcRepository = tlcRepository;
            this._eventBus = eventBus;
        }

        private void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(_localizationManager);
        }

        public async Task<bool> CreateTenant(SynchronizeTenantDto dto)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                var tenant = new Tenant()
                {
                    TenancyName = dto.TenantName,
                    Name = dto.TenantName,
                    ApiKey = dto.TenantName,
                    OtherSystemId = null
                };
                await _tenantManager.CreateAsync(tenant);
                await _unitOfWorkManager.Current.SaveChangesAsync(); // To get new tenant's id.
                // We are working entities of new tenant, so changing tenant filter
                using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                {
                    // Create static roles for new tenant
                    CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); // To get static role ids
                    // Grant all permissions to admin role
                    var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                    await _roleManager.GrantAllPermissionsAsync(adminRole);
                    // Create admin user for the tenant
                    var adminUser = User.CreateTenantAdminUser(tenant.Id, "admin@admin.com");
                    adminUser.Password = _passwordHasher.HashPassword(adminUser, User.DefaultPassword);
                    CheckErrors(await _userManager.CreateAsync(adminUser));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); // To get admin user's id
                    // Assign admin user to role!
                    CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                }
                List<long> addSet = new List<long>();
                foreach (var item in dto.Channels)
                {
                    var channel = await this._lcRepository.GetAll().IgnoreQueryFilters().FirstAsync(o => o.LogisticBy.LogisticCode == item.LogisticCode 
                    && o.ChannelName == item.LogisticChannelCode);
                    var tlc = new TenantLogisticChannel()
                    {
                        TenantId = tenant.Id,
                        LogisticChannelId = channel.Id
                    };
                    if (item.StepWeight > 0)
                    {
                        tlc.LogisticChannelChange = JsonConvert.SerializeObject(new ChangeFreightRule()
                        {
                            WeightChargeRules = new List<WeightFreight>()
                            {
                                new WeightFreight()
                                {
                                    Currency = "AUD",
                                    Unit = "g",
                                    StartingWeight = item.StartingWeight,
                                    EndWeight = 1000000,
                                    StartingPrice = item.StartingPrice,
                                    StepWeight = item.StepWeight,
                                    CostPrice = item.Price,
                                    Price = item.Price
                                }
                            }
                        });
                    }
                    await this._tlcRepository.InsertAsync(tlc);
                    addSet.Add(channel.Id);
                }
                await this._eventBus.TriggerAsync(new TenantImportChannelEvent()
                {
                    TenantId = tenant.Id,
                    AddChannelIds = addSet
                });
                return true;
            }
        }
    }
}
