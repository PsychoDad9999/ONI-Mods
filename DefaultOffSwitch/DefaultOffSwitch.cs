// ----------------------------------------------------------------------------

using PeterHan.PLib.Options;

// ----------------------------------------------------------------------------

namespace OniMods.DefaultOffSwitch
{
    public static class DefaultOffSwitch
    {
        /// <summary>
        /// Mod Options
        /// </summary>
        internal static DefaultOffSwitchSettings Options { get; private set; }

        /// <summary>
        /// Register Options on mod load
        /// </summary>
        public static void OnLoad()
        {
            // Registering for the config screen
            POptions.RegisterOptions(typeof(DefaultOffSwitchSettings));

            // Read Settings
            Options = POptions.ReadSettings<DefaultOffSwitchSettings>() ?? new DefaultOffSwitchSettings();
        }
    }
}
