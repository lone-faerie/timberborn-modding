using System.Collections.Generic;
using Mods.BetterHealthcare.Scripts.VisitableSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.TimeSystem;
using UnityEngine.UIElements;

namespace Mods.BetterHealthcare.Scripts.VisitableSystemUI
{
    public class VisitableLoadRateFragment : IEntityPanelFragment
    {
        private readonly VisualElementLoader _visualElementLoader;
        private readonly IDayNightCycle _dayNightCycle;

        private VisualElement _root;
        private readonly List<LoadRate> _loadRates = new();
        private VisitableLoadRate _visitableLoadRate;

        public VisitableLoadRateFragment(VisualElementLoader visualElementLoader, IDayNightCycle dayNightCycle)
        {
            _visualElementLoader = visualElementLoader;
            _dayNightCycle = dayNightCycle;
        }
        
        public VisualElement InitializeFragment()
        {
            _root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/AttractionLoadRateFragment");
            VisualElement visualElement1 = _root.Q("LoadRates");
            for (int index = 0; index < 24; ++index)
            {
                VisualElement visualElement2 = _visualElementLoader.LoadVisualElement("Game/AttractionLoadRate");
                visualElement1.Add(visualElement2);
                _loadRates.Add(new LoadRate {Rate = visualElement2.Q("Rate"), CurrentHourMarker = visualElement2.Q("CurrentHourMarker")});
            }
            _root.ToggleDisplayStyle(false);
            return _root;
        }

        public void ShowFragment(BaseComponent entity)
        {
            _visitableLoadRate = entity.GetComponent<VisitableLoadRate>();
        }

        public void ClearFragment()
        {
            _visitableLoadRate = null;
        }

        public void UpdateFragment()
        {
            if (_visitableLoadRate is not null && _visitableLoadRate.Enabled)
            {
                for (int hour = 0; hour < _loadRates.Count; ++hour)
                    UpdateLoadRate(hour);
                _root.ToggleDisplayStyle(true);
            }
            else
            {
                _root.ToggleDisplayStyle(false);
            }
        }

        private void UpdateLoadRate(int hour)
        {
            LoadRate loadRate1 = _loadRates[hour];
            float loadRate2 = _visitableLoadRate.GetLoadRate(hour);
            loadRate1.Rate.style.height = new StyleLength(Length.Percent(loadRate2 * 100f));
            bool visible = hour == (int)_dayNightCycle.HoursPassedToday;
            loadRate1.CurrentHourMarker.ToggleDisplayStyle(visible);
        }

        private readonly struct LoadRate
        {
            public VisualElement Rate { get; init; }

            public VisualElement CurrentHourMarker { get; init; }
        }
    }
}