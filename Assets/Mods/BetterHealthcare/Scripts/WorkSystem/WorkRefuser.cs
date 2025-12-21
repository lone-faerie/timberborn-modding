using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.NeedSpecs;
using Timberborn.NeedSystem;
using Timberborn.WorkSystem;
using UnityEngine;

namespace Mods.BetterHealthcare.Scripts.WorkSystem
{
    public class WorkRefuser : Timberborn.WorkSystem.WorkRefuser, IAwakableComponent
    {
        private readonly HashSet<NeedPreventingWorkOverrideSpec> _activeOverrides = new();
        
        public new void Awake()
        {
            base.Awake();
            _needManager.NeedChangedActiveState += OnNeedChangedActiveState;
            InitializeOverrides();
        }

        public new bool ShouldRefuseWork()
        {
            Debug.Log("My ShouldRefuseWork");
            string needId = null;
            foreach (NeedSpec needSpec in _needManager.NeedSpecs)
            {
                if (_needManager.NeedIsInCriticalState(needSpec.Id)
                    && needSpec.HasSpec<NeedPreventingWorkSpec>())
                {
                    needId = needSpec.Id;
                    break;
                }
            }
            if (string.IsNullOrEmpty(needId))
                return false;
            foreach (NeedPreventingWorkOverrideSpec overrideSpec in _activeOverrides)
            {
                if (overrideSpec.OverrideNeedIds.Contains(needId))
                {
                    Debug.Log("NeedPreventingWork overridden");
                    return false;
                }
            }
            return true;
        }

        private void OnNeedChangedActiveState(object sender, NeedChangedActiveStateEventArgs e)
        {
            NeedPreventingWorkOverrideSpec overrideSpec = e.NeedSpec.GetSpec<NeedPreventingWorkOverrideSpec>();
            if (overrideSpec is null)
                return;
            if (e.IsActive)
                _activeOverrides.Add(overrideSpec);
            else
                _activeOverrides.Remove(overrideSpec);
        }
        
        private void InitializeOverrides()
        {
            foreach (NeedSpec needSpec in _needManager.NeedSpecs)
            {
                if (!_needManager.NeedIsActive(needSpec.Id))
                    continue;
                NeedPreventingWorkOverrideSpec overrideSpec = needSpec.GetSpec<NeedPreventingWorkOverrideSpec>();
                if (overrideSpec is not null)
                    _activeOverrides.Add(overrideSpec);
            }
        }
    }
}