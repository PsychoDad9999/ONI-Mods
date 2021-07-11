// ----------------------------------------------------------------------------

using Newtonsoft.Json;
using PeterHan.PLib.Options;

// ----------------------------------------------------------------------------

namespace OniMods.FixedOreScrubber
{
    [RestartRequired]
    [JsonObject(MemberSerialization.OptIn)]
    public class FixedOreScrubberModSettings
    {
        [Option("Allow storage ejection", "Adds a button to the ore scrubber to safely eject the chlorine gas.")]
        [JsonProperty]
        public bool AllowStorageEjection { get; set; }

        [Option("Germs removal per use", "Amount of germs to remove per use.")]
        [JsonProperty]
        [Limit(480000, 5000000)]
        public int GermRemovalAmount { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public FixedOreScrubberModSettings()
        {
            AllowStorageEjection = true; // default if the config doesn't exist
            GermRemovalAmount = 480000;
        }
    }
}
