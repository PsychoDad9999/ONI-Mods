// ----------------------------------------------------------------------------

using Newtonsoft.Json;
using PeterHan.PLib;
using PeterHan.PLib.Options;

// ----------------------------------------------------------------------------

namespace OniMods.DefaultOffSwitch
{
    /// <summary>
    /// Mod Settings
    /// </summary>
    [RestartRequired]
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class DefaultOffSwitchSettings
    {
        private const string DEFAULT_STATE_CATEGORY = "Default Value";

        /// <summary>
        /// Switch Default State
        /// </summary>
        public enum SwitchState
        {
            [Option("Off")]
            Off,
            [Option("On")]
            On,
        }


        [Option("Signal Switch", "Default State when building a new Signal Switch.", DEFAULT_STATE_CATEGORY)]
        [JsonProperty]
        public SwitchState SignalSwitchDefaultState { get; set; }

        [Option("Power Switch", "Default State when building a new Power Switch.", DEFAULT_STATE_CATEGORY)]
        [JsonProperty]
        public SwitchState PowerSwitchDefaultState { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public DefaultOffSwitchSettings()
        {
            SignalSwitchDefaultState = SwitchState.Off;
            PowerSwitchDefaultState = SwitchState.Off;
        }
    }
}
