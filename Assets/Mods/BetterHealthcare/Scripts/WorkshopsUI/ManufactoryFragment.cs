using Mods.BetterHealthcare.Scripts.DropdownSystem;
using Mods.BetterHealthcare.Scripts.Workshops;
using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using Timberborn.Workshops;
using UnityEngine.UIElements;

namespace Mods.BetterHealthcare.Scripts.WorkshopsUI
{
    public class ManufactoryFragment : IEntityPanelFragment
    {
        private readonly VisualElementLoader _visualElementLoader;
        private readonly ITooltipRegistrar _tooltipRegistrar;
        private readonly AdvancedDropdownItemsSetter _dropdownItemsSetter;
        private readonly EventBus _eventBus;

        private Manufactory _manufactory;
        private ManufactoryTogglableRecipes _manufactoryTogglableRecipes;
        private ManufactoryAdvancedDropdownProvider _manufactoryAdvancedDropdownProvider;
        private AdvancedDropdown _dropdown;
        private VisualElement _root;
        private bool _isAutomaticRecipeManufactory;
        
        public bool Visible 
        { 
            get 
            { 
                if (_manufactoryTogglableRecipes == null && _manufactory != null) 
                { 
                    return _manufactory.ProductionRecipes.Length > 1; 
                } 
                return false; 
            } 
        }
        
        public ManufactoryFragment(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, AdvancedDropdownItemsSetter dropdownItemsSetter, EventBus eventBus) 
        { 
            _visualElementLoader = visualElementLoader;
            _tooltipRegistrar = tooltipRegistrar;
            _dropdownItemsSetter = dropdownItemsSetter;
            _eventBus = eventBus;
        }

        public VisualElement InitializeFragment()
        {
            _root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/MyManufactoryFragment");
            _dropdown = _root.Q<AdvancedDropdown>("Recipes");
            _tooltipRegistrar.RegisterLocalizable(_dropdown, () => _manufactoryAdvancedDropdownProvider.GetValue());
            _root.ToggleDisplayStyle(visible: false);
            return _root;
        }

        public void ShowFragment(BaseComponent entity)
        {
            _manufactory = entity.GetComponent<Manufactory>();
            _manufactoryTogglableRecipes = entity.GetComponent<ManufactoryTogglableRecipes>();
            _isAutomaticRecipeManufactory = entity.GetComponent<AutomaticRecipeManufactory>();
            if (Visible && !_isAutomaticRecipeManufactory)
            {
                _manufactory.RecipeChanged += OnProductionRecipeChanged;
                _eventBus.Register(this);
                _manufactoryAdvancedDropdownProvider = _manufactory.GetComponent<ManufactoryAdvancedDropdownProvider>();
                _root.ToggleDisplayStyle(visible: true);
                _dropdownItemsSetter.SetLocalizableItems(_dropdown, _manufactoryAdvancedDropdownProvider);
            }
        }

        public void UpdateFragment()
        {
            _root.ToggleDisplayStyle(Visible && !_isAutomaticRecipeManufactory);
        }

        public void ClearFragment()
        {
            if (Visible)
            {
                _manufactory.RecipeChanged -= OnProductionRecipeChanged;
                _eventBus.Unregister(this);
            }
            _dropdown.ClearItems();
            _manufactory = null;
            _manufactoryTogglableRecipes = null;
            _manufactoryAdvancedDropdownProvider = null;
            _root.ToggleDisplayStyle(visible: false);
        }

        private void OnProductionRecipeChanged(object sender, EventArgs e)
        {
            _dropdown.RefreshContent();
        }

        [OnEvent]
        public void OnRecipeUnlocked(RecipeUnlockedEvent recipeUnlockedEvent)
        {
            _dropdown.RefreshContent();
        }
    }
}