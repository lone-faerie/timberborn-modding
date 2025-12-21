using System;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mods.BetterHealthcare.Scripts.DropdownSystem
{
    public class AdvancedDropdownItemsSetter
    {
        private readonly VisualElementLoader _visualElementLoader;
        private readonly ITooltipRegistrar _tooltipRegistrar;
        private readonly ILoc _loc;

        public AdvancedDropdownItemsSetter(
                VisualElementLoader visualElementLoader,
                ITooltipRegistrar tooltipRegistrar,
                ILoc loc)
        {
            _visualElementLoader = visualElementLoader;
            _tooltipRegistrar = tooltipRegistrar;
            _loc = loc;
        }

        public void SetItems(AdvancedDropdown advancedDropdown, IAdvancedDropdownProvider advancedDropdownProvider)
        {
            SetItems(advancedDropdown, advancedDropdownProvider, value => Create(
                             advancedDropdownProvider.FormatDisplayText(value),
                             advancedDropdownProvider.GetIcon(value),
                             tooltip: advancedDropdownProvider.GetTooltipText(value),
                             isLockedGetter: advancedDropdownProvider.GetIsLockedGetter(value)));
        }
        
        public void SetItems(AdvancedDropdown advancedDropdown, IAdvancedDropdownProvider advancedDropdownProvider, string itemClass)
        {
            SetItems(advancedDropdown, advancedDropdownProvider, value => Create(
                             advancedDropdownProvider.FormatDisplayText(value),
                             advancedDropdownProvider.GetIcon(value),
                             advancedDropdownProvider.GetTooltipText(value),
                             itemClass,
                             advancedDropdownProvider.GetIsLockedGetter(value)));
        }

        public void SetItems(AdvancedDropdown advancedDropdown, IAdvancedTooltipDropdownProvider advancedDropdownProvider)
        {
            SetItems(advancedDropdown, advancedDropdownProvider, value => Create(
                             advancedDropdownProvider.FormatDisplayText(value),
                             advancedDropdownProvider.GetIcon(value),
                             advancedDropdownProvider.GetTooltip(value),
                             isLockedGetter: advancedDropdownProvider.GetIsLockedGetter(value)));
        }

        public void SetLocalizableItems(AdvancedDropdown advancedDropdown, IAdvancedDropdownProvider advancedDropdownProvider)
        {
            SetItems(advancedDropdown, advancedDropdownProvider, value => CreateLocalizable(
                             advancedDropdownProvider.FormatDisplayText(value),
                             advancedDropdownProvider.GetIcon(value),
                             advancedDropdownProvider.GetTooltipText(value),
                             advancedDropdownProvider.GetIsLockedGetter(value)));
        }

        public void SetLocalizableItems(AdvancedDropdown advancedDropdown, IAdvancedTooltipDropdownProvider advancedDropdownProvider)
        {
            SetItems(advancedDropdown, advancedDropdownProvider, value => CreateLocalizable(
                             advancedDropdownProvider.FormatDisplayText(value),
                             advancedDropdownProvider.GetIcon(value),
                             advancedDropdownProvider.GetTooltip(value),
                             advancedDropdownProvider.GetIsLockedGetter(value)));
        }

        private void SetItems(
                AdvancedDropdown advancedDropdown,
                IAdvancedDropdownProvider advancedDropdownProvider,
                Func<string, AdvancedDropdownElement> visualElementGetter)
        {
            advancedDropdown.ClearItems();
            advancedDropdown.SetItems(advancedDropdownProvider, _tooltipRegistrar, visualElementGetter);
        }

        private AdvancedDropdownElement CreateLocalizable(string value, Sprite icon, string tooltip, Func<bool> isLockedGetter = null)
        {
            return Create(_loc.T(value), icon, tooltip, isLockedGetter: isLockedGetter);
        }

        private AdvancedDropdownElement CreateLocalizable(string value, Sprite icon, VisualElement tooltip, Func<bool> isLockedGetter = null)
        {
            return Create(_loc.T(value), icon, tooltip, isLockedGetter: isLockedGetter);
        }

        private AdvancedDropdownElement Create(string text, Sprite icon, string tooltip = null, string itemClass = null, Func<bool> isLockedGetter = null)
        {
            VisualElement content = CreateContent(text);
            if (!string.IsNullOrEmpty(itemClass))
                content.AddToClassList(itemClass);
            Image visualElement = content.Q<Image>("Icon");
            visualElement.sprite = icon;
            visualElement.ToggleDisplayStyle((bool) (UnityEngine.Object) icon);
            return new AdvancedDropdownElement(content, tooltip, isLockedGetter);
        }

        private AdvancedDropdownElement Create(string text, Sprite icon, VisualElement tooltip, string itemClass = null, Func<bool> isLockedGetter = null)
        {
            VisualElement content = CreateContent(text);
            if (!string.IsNullOrEmpty(itemClass))
                content.AddToClassList(itemClass);
            Image visualElement = content.Q<Image>("Icon");
            visualElement.sprite = icon;
            visualElement.ToggleDisplayStyle((bool) (UnityEngine.Object) icon);
            return new AdvancedDropdownElement(content, tooltip, isLockedGetter);
        }

        private AdvancedDropdownElement Create(string text)
        {
            return new AdvancedDropdownElement(CreateContent(text));
        }

        private VisualElement CreateContent(string text)
        {
            VisualElement e = _visualElementLoader.LoadVisualElement("Core/AdvancedDropdownItem");
            e.Q<Label>("Text").text = text;
            return e;
        }
    }
}