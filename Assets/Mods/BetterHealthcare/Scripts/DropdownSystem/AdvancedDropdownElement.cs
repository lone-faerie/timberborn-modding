using System;
using UnityEngine.UIElements;

namespace Mods.BetterHealthcare.Scripts.DropdownSystem
{
    public struct AdvancedDropdownElement
    {
        public VisualElement Content { get; }
        public string Tooltip { get; }
        public VisualElement TooltipElement { get; }
        
        public Func<bool> IsLockedGetter { get; }
        
        public AdvancedDropdownElement(VisualElement content, Func<bool> isLockedGetter = null)
        {
            Content = content;
            Tooltip = null;
            TooltipElement = null;
            if (isLockedGetter is not null)
                IsLockedGetter = isLockedGetter;
            else
                IsLockedGetter = () => false;
        }
        
        public AdvancedDropdownElement(VisualElement content, string tooltip, Func<bool> isLockedGetter = null)
        {
            Content = content;
            Tooltip = tooltip;
            TooltipElement = null;
            if (isLockedGetter is not null)
                IsLockedGetter = isLockedGetter;
            else
                IsLockedGetter = () => false;
        }

        public AdvancedDropdownElement(VisualElement content, VisualElement tooltip, Func<bool> isLockedGetter = null)
        {
            Content = content;
            Tooltip = null;
            TooltipElement = tooltip;
            if (isLockedGetter is not null)
                IsLockedGetter = isLockedGetter;
            else
                IsLockedGetter = () => false;
        }
    }
}