﻿// ----------------------------------------------------------------------------

using Newtonsoft.Json;
using PeterHan.PLib;
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

        /// <summary>
        /// Ctor
        /// </summary>
        public FixedOreScrubberModSettings()
        {
            AllowStorageEjection = true; // default if the config doesn't exist
        }
    }
}
