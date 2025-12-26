using Mods.BetterHealthcare.Scripts.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Mods.BetterHealthcare.Scripts.DropdownSystem
{
    [UxmlElement]
    public partial class AdvancedDropdown : VisualElement, ILocalizableElement
    {
        private static readonly string SelectableClass = "dropdown__selectable";
        private static readonly string LockedItemClass = "dropdown-item--locked";
        [UxmlAttribute("label-loc-key")]
        private string _labelLocKey;
        [UxmlAttribute("buttons-only-selection")]
        private bool _buttonsOnlySelection;
        private AdvancedDropdownListDrawer _advancedDropdownListDrawer;
        private ITooltipRegistrar _tooltipRegistrar;
        private Button _selection;
        private VisualElement _selectedItem;
        private readonly List<string> _items = new();
        private readonly List<AdvancedDropdownElement> _elements = new();
        private IAdvancedDropdownProvider _advancedDropdownProvider;
        private Func<string, AdvancedDropdownElement> _elementGetter;
        
        public event EventHandler ValueChanged;

        public AdvancedDropdown()
        {
            VisualElementLoader visualElementLoader = StaticVisualElementLoader.Get();
            visualElementLoader.LoadVisualTreeAsset("Core/AdvancedDropdown").CloneTree(this);
            this.Q<Label>("Label").text = _labelLocKey;
        }
        
        public bool IsSet => true;

        public void Initialize(AdvancedDropdownListDrawer advancedDropdownListDrawer)
        {
            _advancedDropdownListDrawer = advancedDropdownListDrawer;
            _selectedItem = this.Q<VisualElement>("SelectedItemContent");
            _selection = this.Q<Button>("Selection");
            _selection.EnableInClassList(SelectableClass, !_buttonsOnlySelection);
            _selection.RegisterCallback<DetachFromPanelEvent>(_ => _advancedDropdownListDrawer.HideDropdown());
            if (_buttonsOnlySelection)
            {
                this.Q<Button>("ArrowDown").RegisterCallback<ClickEvent>(ToggleSelectionListDisplayStyle);
                this.Q<Button>("ArrowLeft").RegisterCallback<ClickEvent>(SelectPrevious);
                this.Q<Button>("ArrowRight").RegisterCallback<ClickEvent>(SelectNext);
            } else
            {
                this.Q<Button>("ArrowLeft").ToggleDisplayStyle(false);
                this.Q<Button>("ArrowRight").ToggleDisplayStyle(false);
                _selection.RegisterCallback<ClickEvent>(ToggleSelectionListDisplayStyle);
            }
        }
        
        public void Localize(ILoc loc)
        {
            Label visualElement = this.Q<Label>("Label");
            if (!string.IsNullOrEmpty(_labelLocKey))
                visualElement.text = loc.T(_labelLocKey);
            else
                visualElement.ToggleDisplayStyle(false);
        }

        public void SetItems(
                IAdvancedDropdownProvider advancedDropdownProvider,
                ITooltipRegistrar tooltipRegistrar,
                Func<string, AdvancedDropdownElement> elementGetter)
        {
            _tooltipRegistrar = tooltipRegistrar;
            _advancedDropdownProvider = advancedDropdownProvider;
            _elementGetter = elementGetter;
            UpdateSelectedValue();
        }

        public void ClearItems()
        {
            _advancedDropdownProvider = null;
            _elementGetter = null;
            _selectedItem.Clear();
            _items.Clear();
            _elements.Clear();
            _advancedDropdownListDrawer.HideDropdown();
        }

        public void RefreshContent()
        {
            foreach (AdvancedDropdownElement element in _elements)
            {
                if (element.IsLockedGetter())
                    element.Content.AddToClassList(LockedItemClass);
                else
                    element.Content.RemoveFromClassList(LockedItemClass);
            }
            UpdateSelectedValue();
        }

        private void ToggleSelectionListDisplayStyle(ClickEvent evt)
        {
            if (_advancedDropdownListDrawer.DropdownVisible)
            {
                _advancedDropdownListDrawer.HideDropdown();
            } else
            {
                AddItemsIfNeeded();
                _advancedDropdownListDrawer.ShowDropdown(_selection, _elements);
            }
        }

        private void SelectPrevious(ClickEvent evt)
        {
            AddItemsIfNeeded();
            if (_items.Count == 0)
                return;
            int index = _items.IndexOf(_advancedDropdownProvider.GetValue()) - 1;
            if (index < 0)
                index = _items.Count - 1;
            SetAndUpdate(_items[index]);
        }

        private void SelectNext(ClickEvent evt)
        {
            AddItemsIfNeeded();
            if (_items.Count == 0)
                return;
            int index = _items.IndexOf(_advancedDropdownProvider.GetValue()) + 1;
            if (index >= _items.Count)
                index = 0;
            SetAndUpdate(_items[index]);
        }

        private void AddItemsIfNeeded()
        {
            if (_elements.Count != 0)
                return;
            for (int index = 0; index < _advancedDropdownProvider.Items.Count; ++index)
            {
                string item = _advancedDropdownProvider.Items[index];
                AdvancedDropdownElement advancedDropdownElement = _elementGetter(item);
                advancedDropdownElement.Content.RegisterCallback<ClickEvent>(_ => SetAndUpdate(item));
                _tooltipRegistrar.Register(advancedDropdownElement.Content, advancedDropdownElement.TooltipGetter);
                if (advancedDropdownElement.IsLockedGetter())
                {
                    advancedDropdownElement.Content.AddToClassList(LockedItemClass);
                } else
                {
                    advancedDropdownElement.Content.RemoveFromClassList(LockedItemClass);
                }
                _items.Add(item);
                _elements.Add(advancedDropdownElement);
            }
        }

        private void SetAndUpdate(string newValue)
        {
            Debug.Log("SetAndUpdate");
            if (_advancedDropdownProvider.GetValue() == newValue)
            {
                Debug.Log($"{_advancedDropdownProvider.GetValue()} == {newValue}");
                return;
            }
            _advancedDropdownProvider.SetValue(newValue, () => SetAndUpdateCallback(newValue));
        }

        private void SetAndUpdateCallback(string newValue)
        {
            UpdateSelectedValue(newValue);
            _advancedDropdownListDrawer.HideDropdown();
            EventHandler valueChanged = ValueChanged;
            if (valueChanged == null)
                return;
            valueChanged(this, EventArgs.Empty);
        }

        private void UpdateSelectedValue() => UpdateSelectedValue(_advancedDropdownProvider.GetValue());
        
        private void UpdateSelectedValue(string newValue)
        {
            if (_selectedItem is null)
                return;
            _selectedItem.Clear();
            VisualElement content = _elementGetter(newValue).Content;
            content.SetEnabled(false);
            content.AddToClassList("dropdown-item--selected");
            _selectedItem.Add(content);
        }
    }
}