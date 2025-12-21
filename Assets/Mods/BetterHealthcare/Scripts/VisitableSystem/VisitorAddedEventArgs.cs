using Timberborn.EnterableSystem;

namespace Mods.BetterHealthcare.Scripts.VisitableSystem
{
    public class VisitorAddedEventArgs
    {
        public Enterer Enterer { get; }
        public bool IsWorker { get; }

        public VisitorAddedEventArgs(Enterer enterer, bool isWorker)
        {
            Enterer = enterer;
            IsWorker = isWorker;
        }
    }
}