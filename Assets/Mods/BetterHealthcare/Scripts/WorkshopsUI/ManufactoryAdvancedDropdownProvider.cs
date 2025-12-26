using Mods.BetterHealthcare.Scripts.DropdownSystem;
using Mods.BetterHealthcare.Scripts.Workshops;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;
using Timberborn.StatusSystemUI;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using Timberborn.Workshops;
using UnityEngine;
using UnityEngine.UIElements;

namespace Mods.BetterHealthcare.Scripts.WorkshopsUI
{
    public class ManufactoryAdvancedDropdownProvider :
        BaseComponent,
        IAwakableComponent,
        IInitializableEntity,
        IAdvancedDropdownProvider,
        IAdvancedTooltipDropdownProvider
    {
        private static readonly string NoRecipeItemLocKey = "Manufactory.NoRecipeOption";
        private static readonly string RecipeUnlockTooltipLocKey = "Manufactory.RecipeUnlock.Tooltip";
        private readonly StatusListFragment _statusListFragment;
        private readonly RecipeUnlockingDialogService _recipeUnlockingDialogService;
        private readonly VisualElementLoader _visualElementLoader;
        private readonly ILoc _loc;
        private Manufactory _manufactory;
        private readonly List<string> _items = new();
        
        public ManufactoryAdvancedDropdownProvider(
                StatusListFragment statusListFragment,
                RecipeUnlockingDialogService recipeUnlockingDialogService,
                VisualElementLoader visualElementLoader,
                ILoc loc)
        {
            _statusListFragment = statusListFragment;
            _recipeUnlockingDialogService = recipeUnlockingDialogService;
            _visualElementLoader = visualElementLoader;
            _loc = loc;
        }

        public IReadOnlyList<string> Items
        {
            get => _items.AsReadOnlyList();
        }

        public void Awake() => _manufactory = GetComponent<Manufactory>();

        public void InitializeEntity()
        {
            ImmutableArray<RecipeSpec> productionRecipes = _manufactory.ProductionRecipes;
            _items.Add(NoRecipeItemLocKey);
            _items.AddRange(productionRecipes.Select(recipe => recipe.DisplayLocKey));
        }

        public string GetValue()
        {
            return _manufactory.CurrentRecipe?.DisplayLocKey ?? NoRecipeItemLocKey;
        }

        public void SetValue(string value, Action callback)
        {
            if (value == NoRecipeItemLocKey)
            {
                SetValueCallback(GetRecipeSpec(value), callback);
                return;
            }
            RecipeSpec recipeSpec = GetRecipeSpec(value);
            _recipeUnlockingDialogService.TryToUnlockRecipe(recipeSpec, () => SetValueCallback(recipeSpec, callback));
        }

        private void SetValueCallback(RecipeSpec recipeSpec, Action callback)
        {
            _manufactory.SetRecipe(recipeSpec);
            _statusListFragment.UpdateFragment();
            callback();
        }

        public string FormatDisplayText(string value) => value;

        public Sprite GetIcon(string value) => GetRecipeSpec(value)?.UIIcon?.Value;

        public string GetTooltipText(string value) => null;

        public Func<TooltipContent> GetTooltipGetter(string value)
        {
            if (value == NoRecipeItemLocKey)
            {
                return TooltipContent.CreateEmpty;
            }
            RecipeSpec recipeSpec = GetRecipeSpec(value);
            VisualElement e = _visualElementLoader.LoadVisualElement("Game/ScienceCostTooltip");
            e.Q<Label>("TooltipText").text = _loc.T(RecipeUnlockTooltipLocKey);
            return () =>
            {
                int unlockCost = _recipeUnlockingDialogService.GetRecipeUnlockCost(recipeSpec);
                if (unlockCost == 0)
                {
                    return TooltipContent.CreateEmpty();
                }
                e.Q<Label>("ScienceCost").text = NumberFormatter.Format(unlockCost);
                return TooltipContent.CreateInstant(() => e);
            };
        }

        public Func<bool> GetIsLockedGetter(string value)
        {
            if (value == NoRecipeItemLocKey)
                return () => false;
            RecipeSpec recipeSpec = GetRecipeSpec(value);
            return () => !_recipeUnlockingDialogService.IsRecipeUnlocked(recipeSpec);
        }

        public Func<bool> GetClickAction(string value)
        {
            if (value == NoRecipeItemLocKey)
                return () => true;
            RecipeSpec recipeSpec = GetRecipeSpec(value);
            return () => _recipeUnlockingDialogService.TryToUnlockRecipe(recipeSpec);
        }

        private RecipeSpec GetRecipeSpec(string value)
        {
            return _manufactory.ProductionRecipes.SingleOrDefault(recipe => recipe.DisplayLocKey == value);
        }
    }
}