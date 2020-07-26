// ----------------------------------------------------------------------------

using PeterHan.PLib.Options;

// ----------------------------------------------------------------------------

namespace OniMods.FixedOreScrubber
{
    public static class FixedOreScrubberMod
    {
        public static void OnLoad()
        {
            POptions.RegisterOptions(typeof(FixedOreScrubberModSettings));
        }
    }
}
