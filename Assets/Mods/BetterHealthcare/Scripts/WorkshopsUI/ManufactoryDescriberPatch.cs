using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Mods.BetterHealthcare.Scripts.ScienceWorkshops;
using Timberborn.Workshops;
using Timberborn.WorkshopsUI;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace Mods.BetterHealthcare.Scripts.WorkshopsUI
{
    [HarmonyPatch(typeof(ManufactoryDescriber), nameof(ManufactoryDescriber.GetInputs))]
    static class ManufactoryDescriberPatch
    {
        static IEnumerable<VisualElement> GetInputs(ManufactoryDescriber __instance, ScienceRecipe recipe)
        {
            string tooltip = __instance._loc.T(ManufactoryDescriber.SciencePointsLocKey);
            string amount = recipe.ConsumedSciencePoints.ToString();
            Debug.Log($"{amount} science");
            yield return __instance._describedAmountFactory.CreatePlain(ManufactoryDescriber.ScienceClass, amount, tooltip);
        }
        
        static void Postfix(ManufactoryDescriber __instance, RecipeSpec productionRecipe, ref IEnumerable<VisualElement> __result)
        {
            if (!__instance.TryGetComponent<ManufactoryScienceConsumer>(out var component))
                return;
            foreach (ScienceRecipe recipe in component.Recipes)
            {
                if (recipe.RecipeId == productionRecipe.Id)
                {
                    __result = __result.Concat(GetInputs(__instance, recipe));
                    return;
                }
            }
        }
    }
}