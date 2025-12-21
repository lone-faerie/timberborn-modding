using Timberborn.EnterableSystem;

namespace Mods.BetterHealthcare.Scripts.VisitableSystem
{
    public class VisitorRemovedEventArgs
    {
        public Enterer Enterer { get; }
        public bool IsWorker { get; }

        public VisitorRemovedEventArgs(Enterer enterer, bool isWorker)
        {
            Enterer = enterer;
            IsWorker = isWorker;
        }
    }
}