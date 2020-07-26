// ----------------------------------------------------------------------------

using PeterHan.PLib.Options;

// ----------------------------------------------------------------------------

namespace OniMods.FixedOreScrubber
{
    public static class FixedOreScrubberMod
    {
        internal static FixedOreScrubberModSettings Options { get; private set; }

        public static void OnLoad()
        {
            POptions.RegisterOptions(typeof(FixedOreScrubberModSettings));
            
            Options = POptions.ReadSettingsForAssembly<FixedOreScrubberModSettings>() ?? new FixedOreScrubberModSettings();
        }
    }
}
