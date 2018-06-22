using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.Configuration;
using Abp.Localization;
using Abp.Net.Mail;

namespace SplitPackage.EntityFrameworkCore.Seed.Host
{
    public class DefaultSettingsCreator
    {
        private readonly SplitPackageDbContext _context;

        public DefaultSettingsCreator(SplitPackageDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            // Emailing
            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "admin@mydomain.com");
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "mydomain.com mailer");

            var entity = _context.Settings.IgnoreQueryFilters().FirstOrDefault(l => l.TenantId == null && l.UserId == null && l.Name == LocalizationSettingNames.DefaultLanguage&&l.Value=="zh-CN");
            if (entity != null)
            {
                _context.Settings.Remove(entity);
            }
            // Languages
            AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "zh-Hans");
        }

        private void AddSettingIfNotExists(string name, string value, int? tenantId = null)
        {
            if (_context.Settings.IgnoreQueryFilters().Any(s => s.Name == name && s.TenantId == tenantId && s.UserId == null))
            {
                return;
            }

            _context.Settings.Add(new Setting(tenantId, null, name, value));
            _context.SaveChanges();
        }
    }
}
