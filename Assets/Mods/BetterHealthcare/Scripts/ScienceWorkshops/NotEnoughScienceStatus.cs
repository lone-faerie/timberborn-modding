using Timberborn.BaseComponentSystem;
using Timberborn.Localization;
using Timberborn.ScienceSystem;
using Timberborn.StatusSystem;

namespace Mods.BetterHealthcare.Scripts.ScienceWorkshops
{
    public class NotEnoughScienceStatus : BaseComponent, IAwakableComponent, IStartableComponent
    {
        private static readonly string NotEnoughScienceLocKey = "Status.Science.NotEnoughScience";
        private static readonly string NotEnoughScienceShortLocKey = "Status.Science.NotEnoughScience.Short";
        
        private readonly ILoc _loc;
        private ManufactoryScienceConsumer _manufactoryScienceConsumer;
        private StatusToggle _statusToggle;

        public NotEnoughScienceStatus(ILoc loc) => _loc = loc;

        public void Awake()
        {
            _manufactoryScienceConsumer = GetComponent<ManufactoryScienceConsumer>();
            _statusToggle = StatusToggle.CreateNormalStatusWithAlertAndFloatingIcon(
                "NotEnoughScience",
                _loc.T(NotEnoughScienceLocKey), 
                _loc.T(NotEnoughScienceShortLocKey), 
                0.2f);
            _manufactoryScienceConsumer.NotEnoughScienceChanged += OnNotEnoughScienceChanged;
        }

        public void Start() => GetComponent<StatusSubject>().RegisterStatus(_statusToggle);

        private void OnNotEnoughScienceChanged(object sender, NotEnoughScienceStateChangedEventArgs e)
        {
            if (e.NewState)
                _statusToggle.Activate();
            else
                _statusToggle.Deactivate();
        }
    }
}