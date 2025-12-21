using Mods.BetterHealthcare.Scripts.DropdownSystem;
using System;
using Timberborn.BatchControl;
using Timberborn.Workshops;
using UnityEngine.UIElements;

namespace Mods.BetterHealthcare.Scripts.WorkshopsUI
{
    public class ManufactoryBatchControlRowItem : IBatchControlRowItem, IClearableBatchControlRowItem
    {
        private readonly AdvancedDropdown _advancedDropdown;
        private readonly Manufactory _manufactory;

        public VisualElement Root { get; }

        public ManufactoryBatchControlRowItem(
                VisualElement root,
                AdvancedDropdown advancedDropdown,
                Manufactory manufactory)
        {
            Root = root;
            _advancedDropdown = advancedDropdown;
            _manufactory = manufactory;
            _manufactory.RecipeChanged += OnProductionRecipeChanged;
        }

        public void ClearRowItem()
        {
            _manufactory.RecipeChanged -= OnProductionRecipeChanged;
        }

        private void OnProductionRecipeChanged(object sender, EventArgs e)
        {
            _advancedDropdown.RefreshContent();
        }
    }
}