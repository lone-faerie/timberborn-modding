using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.Persistence;
using Timberborn.ScienceSystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.Workshops;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Mods.BetterHealthcare.Scripts.Workshops
{
    public class RecipeUnlockingService : ISaveableSingleton, ILoadableSingleton
    {
        private static readonly SingletonKey RecipeUnlockingServiceKey = new(nameof(RecipeUnlockingService));
        private static readonly ListKey<string> UnlockedRecipesKey = new("UnlockedRecipes");

        private readonly ISingletonLoader _singletonLoader;
        private readonly ScienceService _scienceService;
        private readonly EventBus _eventBus;
        private readonly LockedRecipeSpecService _lockedRecipeSpecService;
        private readonly TemplateNameMapper _templateNameMapper;
        private readonly HashSet<string> _unlockedRecipes = new();

        internal static RecipeUnlockingService Instance;

        public RecipeUnlockingService(ISingletonLoader singletonLoader,
                                      ScienceService scienceService,
                                      EventBus eventBus,
                                      LockedRecipeSpecService lockedRecipeSpecService,
                                      TemplateNameMapper templateNameMapper)
        {
            _singletonLoader = singletonLoader;
            _scienceService = scienceService;
            _eventBus = eventBus;
            _lockedRecipeSpecService = lockedRecipeSpecService;
            _templateNameMapper = templateNameMapper;
            Instance ??= this;
        }

        public void Save(ISingletonSaver singletonSaver)
        {
            IObjectSaver objectSaver = singletonSaver.GetSingleton(RecipeUnlockingServiceKey);
            objectSaver.Set(UnlockedRecipesKey, (IReadOnlyCollection<string>) _unlockedRecipes);
        }

        public void Load()
        {
            if (!_singletonLoader.TryGetSingleton(RecipeUnlockingServiceKey, out var objectLoader))
                return;
            _unlockedRecipes.AddRange(objectLoader.Get(UnlockedRecipesKey));
        }

        public bool Unlocked(RecipeSpec recipeSpec)
        {
            return Unlocked(recipeSpec.Id, _lockedRecipeSpecService.GetLockedRecipe(recipeSpec.Id));
        }

        public bool Unlocked(string recipeId, LockedRecipeSpec lockedRecipeSpec)
        {
            return _unlockedRecipes.Contains(recipeId) || lockedRecipeSpec is null || lockedRecipeSpec.ScienceCost == 0;
        }

        public void Unlock(RecipeSpec recipeSpec)
        {
            LockedRecipeSpec lockedRecipeSpec = _lockedRecipeSpecService.GetLockedRecipe(recipeSpec.Id);
            if (!Unlockable(lockedRecipeSpec))
                throw new ArgumentException($"Can't unlock {recipeSpec.Id}, not enough science points!");
            _scienceService.SubtractPoints(lockedRecipeSpec.ScienceCost);
            UnlockIgnoringCost(recipeSpec);
        }

        public void UnlockIgnoringCost(RecipeSpec recipeSpec)
        {
            _unlockedRecipes.Add(recipeSpec.Id);
            _eventBus.Post(new RecipeUnlockedEvent(recipeSpec));
        }

        public bool Unlockable(RecipeSpec recipeSpec)
        {
            if (!_lockedRecipeSpecService.IsLockedRecipe(recipeSpec.Id))
                return true;
            return Unlockable(_lockedRecipeSpecService.GetLockedRecipe(recipeSpec.Id));
        }

        public bool Unlockable(LockedRecipeSpec lockedRecipeSpec)
        {
            return _scienceService.SciencePoints >= lockedRecipeSpec.ScienceCost;
        }

        public int GetUnlockCost(RecipeSpec recipeSpec)
        {
            if (recipeSpec is null || Unlocked(recipeSpec))
                return 0;
            return _lockedRecipeSpecService.GetLockedRecipe(recipeSpec.Id).ScienceCost;
        }

        public string GetTooltip(RecipeSpec recipeSpec)
        {
            if (recipeSpec is null)
                return "";
            LockedRecipeSpec lockedRecipe = _lockedRecipeSpecService.GetLockedRecipe(recipeSpec.Id);
            if (Unlocked(recipeSpec.Id, lockedRecipe))
            {
                Debug.Log("Already unlocked");
                return "";
            }
            return "Unlock";
        }

        internal void ClearUnlockedRecipes()
        {
            _unlockedRecipes.Clear();
        }
    }
}