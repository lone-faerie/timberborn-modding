using Timberborn.CoreUI;

namespace Mods.BetterHealthcare.Scripts.Common
{
    public class StaticVisualElementLoader
    {
        private static VisualElementLoader _visualElementLoader;

        public StaticVisualElementLoader(VisualElementLoader visualElementLoader)
        {
            _visualElementLoader = visualElementLoader;
        }

        public static VisualElementLoader Get() => _visualElementLoader;
    }
}