using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mods.BetterHealthcare.Scripts.Common
{
    public static class HarmonyPatcher
    {
        public static void Patch(string patchId, IEnumerable<Type> patches)
        {
            if (Harmony.HasAnyPatches(patchId))
                return;
            var harmony = new Harmony(patchId);
            foreach (var patch in patches)
            {
                harmony.CreateClassProcessor(patch).Patch();
                Debug.Log("Harmony patch applied: " + patch);
            }
        }
    }
}