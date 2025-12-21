using UnityEngine.UIElements;

namespace Mods.BetterHealthcare.Scripts.DropdownSystem
{
    public interface IAdvancedTooltipDropdownProvider : IAdvancedDropdownProvider
    {
        VisualElement GetTooltip(string value);
    }
}