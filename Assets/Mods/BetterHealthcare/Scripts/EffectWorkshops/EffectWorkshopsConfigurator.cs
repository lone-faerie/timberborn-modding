using Bindito.Core;
using Mods.BetterHealthcare.Scripts.VisitableSystem;
using Timberborn.TemplateInstantiation;

namespace Mods.BetterHealthcare.Scripts.EffectWorkshops
{
    [Context("Game")]
    public class EffectWorkshopsConfigurator : Configurator
    {
        protected override void Configure()
        {
            Bind<ManufactoryEffectProducer>().AsTransient();
            Bind<EffectWorkshopNeedBehavior>().AsTransient();
            MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
        }

        private static TemplateModule ProvideTemplateModule()
        {
            TemplateModule.Builder builder = new TemplateModule.Builder();
            builder.AddDecorator<ManufactoryEffectProducerSpec, ManufactoryEffectProducer>();
            builder.AddDecorator<ManufactoryEffectProducer, VisitorManufactoryLimiter>();
            builder.AddDecorator<ManufactoryEffectProducer, EffectWorkshopNeedBehavior>();
            return builder.Build();
        }
    }
}