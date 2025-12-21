using Timberborn.CoreUI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mods.BetterHealthcare.Scripts.DropdownSystem
{
    public class AdvancedDropdownInitializer : IVisualElementInitializer
    {
        private readonly AdvancedDropdownListDrawer _advancedDropdownListDrawer;
        
        public AdvancedDropdownInitializer(AdvancedDropdownListDrawer advancedDropdownListDrawer)
        {
            _advancedDropdownListDrawer = advancedDropdownListDrawer;
        }
        
        public void InitializeVisualElement(VisualElement visualElement)
        {
            if (!(visualElement is AdvancedDropdown dropdown))
                return;
            dropdown.Initialize(_advancedDropdownListDrawer);
        }
    }
}