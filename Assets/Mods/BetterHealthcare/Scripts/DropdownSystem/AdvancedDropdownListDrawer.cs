using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mods.BetterHealthcare.Scripts.DropdownSystem
{
    public class AdvancedDropdownListDrawer : ILoadableSingleton, IInputProcessor
    {
        private static readonly int MaxHeight = 500;
        private readonly InputService _inputService;
        private readonly ScrollBarInitializationService _scrollBarInitializationService;
        private readonly RootVisualElementProvider _rootVisualElementProvider;
        private VisualElement _root;
        private ScrollView _items;
        private VisualElement _parent;
        private int _isMouseOver;

        public AdvancedDropdownListDrawer(
                InputService inputService,
                ScrollBarInitializationService scrollBarInitializationService,
                RootVisualElementProvider rootVisualElementProvider)
        {
            _inputService = inputService;
            _scrollBarInitializationService = scrollBarInitializationService;
            _rootVisualElementProvider = rootVisualElementProvider;
        }

        public bool DropdownVisible => _root.IsDisplayed();
        
        public void Load()
        {
            _root = _rootVisualElementProvider
                    .Create(nameof(AdvancedDropdownListDrawer), "Core/AdvancedDropdownItems", 2)
                    .Q<VisualElement>("AdvancedDropdownItemsWrapper");
            _items = _root.Q<ScrollView>("AdvancedDropdownItems");
            _scrollBarInitializationService.InitializeVisualElement(_items);
            _root.ToggleDisplayStyle(false);
        }

        public bool ProcessInput()
        {
            if (!_inputService.Cancel && (!_inputService.MainMouseButtonDown && !_inputService.ScrollWheelActive || _isMouseOver != 0))
                return false;
            HideDropdown();
            return true;
        }

        public void ShowDropdown(VisualElement parent, IEnumerable<AdvancedDropdownElement> items)
        {
            Debug.Log("Advanced ShowDropdown");
            HideDropdown();
            _parent = parent;
            _inputService.AddInputProcessor(this);
            _parent.RegisterCallback<MouseEnterEvent>(_ => MouseEntered());
            _parent.RegisterCallback<MouseLeaveEvent>(_ => MouseLeft());
            _items.RegisterCallback<MouseEnterEvent>(_ => MouseEntered());
            _items.RegisterCallback<MouseLeaveEvent>(_ => MouseLeft());
            _isMouseOver = 1;
            _root.ToggleDisplayStyle(true);
            foreach (AdvancedDropdownElement child in items)
            {
                _items.Add(child.Content);
            }
                
            CalculateDimensions();
        }

        public void HideDropdown()
        {
            if (!DropdownVisible)
                return;
            _inputService.RemoveInputProcessor(this);
            _items.Clear();
            _root.ToggleDisplayStyle(false);
            _parent.UnregisterCallback<MouseEnterEvent>(_ => MouseEntered());
            _parent.UnregisterCallback<MouseLeaveEvent>(_ => MouseLeft());
            _items.UnregisterCallback<MouseEnterEvent>(_ => MouseEntered());
            _items.UnregisterCallback<MouseLeaveEvent>(_ => MouseLeft());
            _isMouseOver = 0;
            _parent = null;
        }

        private void MouseEntered() => ++_isMouseOver;

        private void MouseLeft() => --_isMouseOver;

        private void CalculateDimensions()
        {
            Vector2 world = _parent.LocalToWorld(_parent.resolvedStyle.translate);
            _root.style.left = world.x;
            _root.style.width = _parent.resolvedStyle.width;
            float num1 = world.y + _parent.resolvedStyle.height;
            float height = _root.parent.resolvedStyle.height;
            int num2 = height - num1 > 150.0f ? 1 : 0;
            float val2 = num2 != 0 ? (float) (height - num1 - 20.0f) : world.y + 20f;
            _root.style.maxHeight = Math.Min(MaxHeight, val2);
            if (num2 != 0)
            {
                _root.style.top = num1;
                _root.style.bottom = new StyleLength(StyleKeyword.Auto);
            } else
            {
                _root.style.top = new StyleLength(StyleKeyword.Auto);
                _root.style.bottom = height - world.y;
            }
        }
    }
}