using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace SplitPackage.Localization
{
    public static class SplitPackageLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(SplitPackageConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(SplitPackageLocalizationConfigurer).GetAssembly(),
                        "SplitPackage.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
