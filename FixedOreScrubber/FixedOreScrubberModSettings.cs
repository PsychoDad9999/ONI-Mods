// ----------------------------------------------------------------------------

using Newtonsoft.Json;
using PeterHan.PLib;

// ----------------------------------------------------------------------------

namespace OniMods.FixedOreScrubber
{
    [JsonObject(MemberSerialization.OptIn)]
    public class FixedOreScrubberModSettings
    {
        [Option("Allow storage ejection", "Adds a button to the ore scrubber to safely eject all resources.")]
        [JsonProperty]
        public bool AllowStorageEjection { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public FixedOreScrubberModSettings()
        {
            AllowStorageEjection = true; // default if the config doesn't exist
        }
    }
}
