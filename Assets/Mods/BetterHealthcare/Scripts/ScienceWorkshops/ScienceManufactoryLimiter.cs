using Timberborn.BaseComponentSystem;
using Timberborn.Workshops;

namespace Mods.BetterHealthcare.Scripts.ScienceWorkshops
{
    public class ScienceManufactoryLimiter : BaseComponent, IAwakableComponent, IManufactoryLimiter
    {
        private ManufactoryScienceConsumer _manufactoryScienceConsumer;
        
        public void Awake()
        {
            _manufactoryScienceConsumer = GetComponent<ManufactoryScienceConsumer>();
        }

        public float ProductionEfficiency() => _manufactoryScienceConsumer.NotEnoughScience ? 0.0f : 1f;

        public float MaxProductionProgressChange() => _manufactoryScienceConsumer.NotEnoughScience ? 0.0f : 1f;
    }
}