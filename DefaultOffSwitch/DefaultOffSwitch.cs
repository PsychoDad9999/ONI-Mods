// ----------------------------------------------------------------------------

using HarmonyLib;

using PeterHan.PLib.Options;

// ----------------------------------------------------------------------------

namespace OniMods.DefaultOffSwitch
{
    public class DefaultOffSwitch : KMod.UserMod2
    {
        /// <summary>
        /// Mod Options
        /// </summary>
        internal static DefaultOffSwitchSettings Options { get; private set; }

        /// <summary>
        /// Register Options on mod load
        /// </summary>
        public override void OnLoad(Harmony harmony)
        {
            harmony.PatchAll();

            // Registering for the config screen
            new POptions().RegisterOptions(this, typeof(DefaultOffSwitchSettings));
            
            // Read Settings
            Options = POptions.ReadSettings<DefaultOffSwitchSettings>() ?? new DefaultOffSwitchSettings();
        }
    }
}
