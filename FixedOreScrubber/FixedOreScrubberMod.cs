// ----------------------------------------------------------------------------

using HarmonyLib;

using PeterHan.PLib.Options;

// ----------------------------------------------------------------------------

namespace OniMods.FixedOreScrubber
{
    public class FixedOreScrubberMod : KMod.UserMod2
    {
        internal static FixedOreScrubberModSettings Options { get; private set; }

        public override void OnLoad(Harmony harmony)
        {
            harmony.PatchAll();

            // Registering for the config screen
            new POptions().RegisterOptions(this, typeof(FixedOreScrubberModSettings));

            // Read Settings
            Options = POptions.ReadSettings<FixedOreScrubberModSettings>() ?? new FixedOreScrubberModSettings();
        }
    }
}
