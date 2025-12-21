using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mods.BetterHealthcare.Scripts.DropdownSystem
{
    public interface IAdvancedDropdownProvider
    {
        IReadOnlyList<string> Items { get; }

        string GetValue();

        void SetValue(string value, Action callback);

        string FormatDisplayText(string value);

        Sprite GetIcon(string value);

        string GetTooltipText(string value);

        Func<bool> GetIsLockedGetter(string value);
    }
}