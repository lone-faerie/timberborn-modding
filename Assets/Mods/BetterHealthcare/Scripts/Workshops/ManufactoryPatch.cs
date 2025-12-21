using HarmonyLib;
using Timberborn.Workshops;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace Mods.BetterHealthcare.Scripts.Workshops
{
    [HarmonyPatch(typeof(Manufactory))]
    static class ManufactoryPatch
    {
        [HarmonyPatch(nameof(Manufactory.Load))]
        static void Postfix(Manufactory __instance)
        {
            if (!__instance.HasCurrentRecipe)
                return;
            if (!RecipeUnlockingService.Instance.Unlocked(__instance.CurrentRecipe))
                __instance.SetRecipe(null);
        }
    }
}