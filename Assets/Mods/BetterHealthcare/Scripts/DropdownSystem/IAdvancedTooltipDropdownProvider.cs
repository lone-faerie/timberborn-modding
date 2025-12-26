using System;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Mods.BetterHealthcare.Scripts.DropdownSystem
{
    public interface IAdvancedTooltipDropdownProvider : IAdvancedDropdownProvider
    {
        Func<TooltipContent> GetTooltipGetter(string value);
    }
}