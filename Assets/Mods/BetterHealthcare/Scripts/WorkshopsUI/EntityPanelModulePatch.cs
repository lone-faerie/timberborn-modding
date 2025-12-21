using HarmonyLib;
using Timberborn.EntityPanelSystem;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace Mods.BetterHealthcare.Scripts.WorkshopsUI
{
    [HarmonyPatch(typeof(EntityPanelModule.Builder), nameof(EntityPanelModule.Builder.AddMiddleFragment))]
    static class EntityPanelModulePatch
    {
        static bool Prefix(IEntityPanelFragment panelFragment, int order)
        {
            return panelFragment is not Timberborn.WorkshopsUI.ManufactoryFragment;
        }
    }
}