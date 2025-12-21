using HarmonyLib;
using Timberborn.EnterableSystem;
using Timberborn.WorkshopsEffects;
using Timberborn.WorkSystem;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace Mods.BetterHealthcare.Scripts.WorkshopsEffects
{
    [HarmonyPatch(typeof(WorkshopWorkerHider), nameof(WorkshopWorkerHider.OnEntererAssignedToSlot))]
    static class WorkshopWorkerHiderPatch
    {
        static bool Prefix(WorkshopWorkerHider __instance, object sender, Enterer e)
        {
            Worker component = e.GetComponent<Worker>();
            Workplace component2 = __instance.GetComponent<Workplace>();
            return component is null || component.Workplace == component2;
        }
    }
}