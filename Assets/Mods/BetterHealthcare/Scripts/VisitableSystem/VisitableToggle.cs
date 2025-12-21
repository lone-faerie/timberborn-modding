using System;

namespace Mods.BetterHealthcare.Scripts.VisitableSystem
{
    public class VisitableToggle
    {
        public event EventHandler StateChanged;
        
        public bool Paused { get; private set; }

        public void ResumeVisitable()
        {
            if (!Paused)
                return;
            Paused = false;
            EventHandler stateChanged = StateChanged;
            if (stateChanged == null)
                return;
            stateChanged(this, EventArgs.Empty);
        }

        public void PauseVisitable()
        {
            if (Paused)
                return;
            Paused = true;
            EventHandler stateChanged = StateChanged;
            if (stateChanged == null)
                return;
            stateChanged(this, EventArgs.Empty);
        }

        public void ToggleVisitable(bool paused)
        {
            if (paused)
                PauseVisitable();
            else
                ResumeVisitable();
        }
    }
}