using Bindito.Core;
using Mods.BetterHealthcare.Scripts.Common;
using Timberborn.CoreUI;

namespace Mods.BetterHealthcare.Scripts.DropdownSystem
{
    [Context("MainMenu")]
    [Context("Game")]
    [Context("MapEditor")]
    public class DropdownSystemConfigurator : Configurator
    {
        protected override void Configure()
        {
            Bind<StaticVisualElementLoader>().AsSingleton();
            Bind<AdvancedDropdownItemsSetter>().AsSingleton();
            Bind<AdvancedDropdownListDrawer>().AsSingleton();
            MultiBind<IVisualElementInitializer>().To<AdvancedDropdownInitializer>().AsSingleton();
        }
    }
}