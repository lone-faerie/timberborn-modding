namespace Mods.BetterHealthcare.Scripts.Healthcare
{
    public class InjurableInjuryChangedEvent
    {
        public Injurable Injurable { get; }

        public InjurableInjuryChangedEvent(Injurable injurable)
        {
            Injurable = injurable;
        }
    }
}