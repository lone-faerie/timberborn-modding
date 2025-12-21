using HarmonyLib;
using Timberborn.BatchControl;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace Mods.BetterHealthcare.Scripts.WorkplacesBatchControl
{
    [HarmonyPatch(typeof(BatchControlModule.Builder), nameof(BatchControlModule.Builder.AddTab))]
    static class BatchControlModulePatch
    {
        static bool Prefix(BatchControlTab tab, int order)
        {
            return tab is not Timberborn.WorkplacesBatchControl.WorkplacesBatchControlTab;
        }
    }
}