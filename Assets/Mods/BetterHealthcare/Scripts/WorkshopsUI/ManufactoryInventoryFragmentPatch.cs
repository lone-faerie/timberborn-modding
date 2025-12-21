using HarmonyLib;
using Mods.BetterHealthcare.Scripts.Common;
using Mods.BetterHealthcare.Scripts.EffectWorkshops;
using Timberborn.CoreUI;
using Timberborn.Workshops;
using Timberborn.WorkshopsUI;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace Mods.BetterHealthcare.Scripts.WorkshopsUI
{
    [HarmonyPatch(typeof(ManufactoryInventoryFragment), nameof(ManufactoryInventoryFragment.UpdateFragment))]
    static class ManufactoryInventoryFragmentPatch
    {
        static void Postfix(ManufactoryInventoryFragment __instance)
        {
            if (!__instance._root.IsDisplayed())
                return;
            RecipeSpec currentRecipe = __instance._manufactory.CurrentRecipe;
            if (currentRecipe is not null && currentRecipe.IsEffect()  && __instance._isEmpty.IsDisplayed())
                __instance._root.ToggleDisplayStyle(false);
        }
    }
}