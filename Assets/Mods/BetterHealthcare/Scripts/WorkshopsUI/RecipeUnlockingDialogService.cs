using Mods.BetterHealthcare.Scripts.Workshops;
using System;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.Workshops;

namespace Mods.BetterHealthcare.Scripts.WorkshopsUI
{
    public class RecipeUnlockingDialogService
    {
        private static readonly string CantUnlockLocKey = "Manufactory.RecipeUnlock.CantUnlock";
        private static readonly string UnlockPromptLocKey = "Manufactory.RecipeUnlock.Prompt";
        private static readonly string InstantUnlockKey = "InstantUnlock";

        private readonly DialogBoxShower _dialogBoxShower;
        private readonly InputService _inputService;
        private readonly RecipeUnlockingService _recipeUnlockingService;
        private readonly ILoc _loc;

        public RecipeUnlockingDialogService(
                DialogBoxShower dialogBoxShower,
                InputService inputService,
                RecipeUnlockingService recipeUnlockingService,
                ILoc loc)
        {
            _dialogBoxShower = dialogBoxShower;
            _inputService = inputService;
            _recipeUnlockingService = recipeUnlockingService;
            _loc = loc;
        }

        public bool TryToUnlockRecipe(RecipeSpec recipeSpec)
        {
            return TryToUnlockRecipe(recipeSpec, () => { });
        }

        public bool TryToUnlockRecipe(RecipeSpec recipeSpec, Action callback)
        {
            if (_recipeUnlockingService.Unlocked(recipeSpec))
            {
                callback();
                return true;
            }
            if (_inputService.IsKeyHeld(InstantUnlockKey))
            {
                UnlockIgnoringScienceCost(recipeSpec, callback);
                return true;
            }
            if (_recipeUnlockingService.Unlockable(recipeSpec))
            {
                AskForUnlockingConfirmation(recipeSpec, callback);
                return true;
            }
            ShowInsufficientSciencePointsMessage();
            return false;
        }

        public bool IsRecipeUnlocked(RecipeSpec recipeSpec)
        {
            return _recipeUnlockingService.Unlocked(recipeSpec);
        }

        public int GetRecipeUnlockCost(RecipeSpec recipeSpec)
        {
            return _recipeUnlockingService.GetUnlockCost(recipeSpec);
        }

        private void UnlockIgnoringScienceCost(RecipeSpec recipeSpec, Action callback)
        {
            _recipeUnlockingService.UnlockIgnoringCost(recipeSpec);
            callback();
        }

        private void AskForUnlockingConfirmation(RecipeSpec recipeSpec, Action callback)
        {
            _dialogBoxShower.Create().SetMessage(GetUnlockPromptMessage(recipeSpec)).SetConfirmButton(() =>
            {
                _recipeUnlockingService.Unlock(recipeSpec);
                callback();
            }).SetDefaultCancelButton().Show();
        }

        private void ShowInsufficientSciencePointsMessage()
        {
            _dialogBoxShower.Create().SetLocalizedMessage(CantUnlockLocKey).Show();
        }

        private string GetUnlockPromptMessage(RecipeSpec recipeSpec)
        {
            return _loc.T<int>(UnlockPromptLocKey, _recipeUnlockingService.GetUnlockCost(recipeSpec));
        }
    }
}