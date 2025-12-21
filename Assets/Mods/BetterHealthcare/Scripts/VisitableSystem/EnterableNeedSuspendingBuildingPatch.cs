using HarmonyLib;
using Timberborn.EnterableSystem;
using Timberborn.NeedSuspending;
using Timberborn.WorkSystem;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace Mods.BetterHealthcare.Scripts.VisitableSystem
{
    [HarmonyPatch(typeof(EntererNeedSuspendingBuilding))]
    static class EnterableNeedSuspendingBuildingPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(EntererNeedSuspendingBuilding.OnEntererAdded))]
        static bool Prefix1(EntererNeedSuspendingBuilding __instance, object sender, EntererAddedEventArgs e)
        {
            return ShouldRunOriginal(__instance, e.Enterer);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(nameof(EntererNeedSuspendingBuilding.OnEntererRemoved))]
        static bool Prefix2(EntererNeedSuspendingBuilding __instance, object sender, EntererRemovedEventArgs e)
        {
            return ShouldRunOriginal(__instance, e.Enterer);
        }

        private static bool ShouldRunOriginal(EntererNeedSuspendingBuilding __instance, Enterer enterer)
        {
            return !__instance.HasComponent<Visitable>()
                   || !__instance.TryGetComponent<Workplace>(out var workplace)
                   || !enterer.TryGetComponent<Worker>(out var worker)
                   || worker.Workplace != workplace;
        }
    }
}