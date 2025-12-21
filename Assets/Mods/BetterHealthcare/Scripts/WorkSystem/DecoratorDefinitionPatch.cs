using HarmonyLib;
using System;
using Timberborn.TemplateInstantiation;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming

namespace Mods.BetterHealthcare.Scripts.WorkSystem
{
    [HarmonyPatch(typeof(DecoratorDefinition), nameof(DecoratorDefinition.CreateSingleton))]
    static class DecoratorDefinitionPatch
    {
        static void Prefix(bool __runOriginal, ref Type decoratorType)
        {
            if (!__runOriginal)
                return;
            if (decoratorType == typeof(Timberborn.WorkSystem.WorkRefuser))
                decoratorType = typeof(WorkRefuser);
        }
    }
}