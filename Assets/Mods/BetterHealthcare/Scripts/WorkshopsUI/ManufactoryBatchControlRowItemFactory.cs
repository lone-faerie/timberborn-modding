using Mods.BetterHealthcare.Scripts.DropdownSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using Timberborn.Workshops;
using UnityEngine.UIElements;

namespace Mods.BetterHealthcare.Scripts.WorkshopsUI
{
    public class ManufactoryBatchControlRowItemFactory
    {
        private static readonly string CurrentRecipeLocKey = "Manufactory.CurrentRecipe";

        private readonly VisualElementLoader _visualElementLoader;
        private readonly ITooltipRegistrar _tooltipRegistrar;
        private readonly AdvancedDropdownItemsSetter _advancedDropdownItemsSetter;
        private readonly ILoc _loc;

        internal static ManufactoryBatchControlRowItemFactory Instance { get; private set; }

        public ManufactoryBatchControlRowItemFactory(
                VisualElementLoader visualElementLoader,
                ITooltipRegistrar tooltipRegistrar,
                AdvancedDropdownItemsSetter advancedDropdownItemsSetter,
                ILoc loc)
        {
            _visualElementLoader = visualElementLoader;
            _tooltipRegistrar = tooltipRegistrar;
            _advancedDropdownItemsSetter = advancedDropdownItemsSetter;
            _loc = loc;
            Instance = this;
        }

        public IBatchControlRowItem Create(BaseComponent entity)
        {
            Manufactory component = entity.GetComponent<Manufactory>();
            if (component == null || component.ProductionRecipes.Length <= 1 || entity.GetComponent<ManufactoryTogglableRecipes>() is not null)
                return null;
            VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/BatchControl/AdvancedDropdownBatchControlRowItem");
            AdvancedDropdown dropdown = visualElement.Q<AdvancedDropdown>("AdvancedDropdown");
            ManufactoryAdvancedDropdownProvider manufactoryAdvancedDropdownProvider = component.GetComponent<ManufactoryAdvancedDropdownProvider>();
            _advancedDropdownItemsSetter.SetLocalizableItems(dropdown, manufactoryAdvancedDropdownProvider);
            _tooltipRegistrar.Register(dropdown, () => GetTooltipText(manufactoryAdvancedDropdownProvider));
            return new ManufactoryBatchControlRowItem(visualElement, dropdown, component);
        }

        private string GetTooltipText(ManufactoryAdvancedDropdownProvider manufactoryAdvancedDropdownProvider)
        {
            return $"{_loc.T(CurrentRecipeLocKey)} {_loc.T(manufactoryAdvancedDropdownProvider.GetValue())}";
        }
    }
}