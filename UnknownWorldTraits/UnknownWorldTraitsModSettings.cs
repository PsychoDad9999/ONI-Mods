// ----------------------------------------------------------------------------

using Newtonsoft.Json;
using PeterHan.PLib;

// ----------------------------------------------------------------------------

namespace OniMods.UnknownWorldTraits
{
    /// <summary>
    /// Mod Settings
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class UnknownWorldTraitsModSettings
    {
        [Option("Display trait color", "Show trait color hint.")]
        [JsonProperty]
        public bool ShowTraitColor { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public UnknownWorldTraitsModSettings()
        {
            ShowTraitColor = true; // default if the config doesn't exist
        }
    }
}
