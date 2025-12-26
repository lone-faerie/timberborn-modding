using Bindito.Core;
using Timberborn.Beavers;
using Timberborn.TemplateInstantiation;

namespace Mods.BetterHealthcare.Scripts.Healthcare
{
    [Context("Game")]
    public class HealthcareConfigurator : Configurator
    {
        protected override void Configure()
        {
            Bind<Injurable>().AsTransient();
            Bind<InjuryNeedEnabler>().AsTransient();
            MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
        }

        private static TemplateModule ProvideTemplateModule()
        {
            TemplateModule.Builder builder = new TemplateModule.Builder();
            builder.AddDecorator<BeaverSpec, Injurable>();
            builder.AddDecorator<Injurable, InjuryNeedEnabler>();
            return builder.Build();
        }
    }
}