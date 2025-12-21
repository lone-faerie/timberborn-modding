using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Mods.BetterHealthcare.Scripts.ScienceWorkshops;
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
        static IEnumerable<VisualElement> GetInputs(ManufactoryDescriber __instance, ManufactoryScienceConsumer component)
        {
            string tooltip = __instance._loc.T(ManufactoryDescriber.SciencePointsLocKey);
            foreach (var recipe in component.Recipes)
            {
                string amount = recipe.ConsumedSciencePoints.ToString();
                Debug.Log($"{amount} science");
                yield return __instance._describedAmountFactory.CreatePlain(ManufactoryDescriber.ScienceClass, amount, tooltip);
            }
        }
        
        static void Postfix(ManufactoryDescriber __instance, ref IEnumerable<VisualElement> __result)
        {
            if (!__instance.TryGetComponent<ManufactoryScienceConsumer>(out var component))
                return;
            __result = __result.Concat(GetInputs(__instance, component));
        }
    }
}