using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Mods.BetterHealthcare.Scripts.ScienceWorkshops
{
    [Context("Game")]
    public class ScienceWorkshopsConfigurator : Configurator
    {
        protected override void Configure()
        {
            Bind<NotEnoughScienceStatus>().AsTransient();
            Bind<ManufactoryScienceConsumer>().AsTransient();
            Bind<ScienceManufactoryLimiter>().AsTransient();

            MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
        }

        private static TemplateModule ProvideTemplateModule()
        {
            TemplateModule.Builder builder = new TemplateModule.Builder();
            builder.AddDecorator<ManufactoryScienceConsumerSpec, ManufactoryScienceConsumer>();
            builder.AddDecorator<ManufactoryScienceConsumer, ScienceManufactoryLimiter>();
            builder.AddDecorator<ManufactoryScienceConsumer, NotEnoughScienceStatus>();
            return builder.Build();
        }
    }
}