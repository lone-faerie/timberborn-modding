using Bindito.Core;
using Mods.BetterHealthcare.Scripts.EffectWorkshops;
using Timberborn.TemplateInstantiation;

namespace Mods.BetterHealthcare.Scripts.EffectWorkshopsUI
{
    [Context("Game")]
    public class EffectWorkshopsUIConfigurator : Configurator
    {
        protected override void Configure()
        {
            Bind<EffectWorkshopDescriber>().AsTransient();
            MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
        }

        private static TemplateModule ProvideTemplateModule()
        {
            TemplateModule.Builder builder = new TemplateModule.Builder();
            builder.AddDecorator<ManufactoryEffectProducer, EffectWorkshopDescriber>();
            return builder.Build();
        }
    }
}