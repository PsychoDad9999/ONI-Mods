// ----------------------------------------------------------------------------

using Newtonsoft.Json;
using PeterHan.PLib;
using PeterHan.PLib.Options;
using PeterHan.PLib.UI;
using System.Reflection;
using UnityEngine;

// ----------------------------------------------------------------------------

namespace OniMods.FixedOreScrubber
{
    [RestartRequired]
    [JsonObject(MemberSerialization.OptIn)]
    public class FixedOreScrubberModSettings
    {
        [Option("Repeated scrubbing", "Dupes will repeat the decontamination if there are any germs left after using the ore scrubber.", "Dupes")]
        //[Option("Repeated scrubbing", "Dupes will repeat the decontamination if there are any germs left after using the ore scrubber.")]
        [JsonProperty]
        public bool RepeatedScubbing { get; set; }


        //[Option("Disease removal count", "Amount of germs that will be removed per use.", "Ore Scrubber")]
        [DynamicOption(typeof(GermsRemovalOptionsEntry), "Ore Scrubber")]
        [Limit(1, 2000000)]        
        [JsonProperty]
        public int DiseaseRemovalCount { get; set; }

        [Option("Reset to default", "Reset all settings to the default values", "Ore Scrubber")]
        //[Option("Reset to default", "Reset all settings to the default values")]
        public System.Action ResetToDefault => new System.Action(ResetSettingsToDefault);



        [Option("Safe", "Adds a button to the ore scrubber to safely eject the chlorine gas.", "Ore Scrubber")]
        //[Option("Safe storage ejection", "Adds a button to the ore scrubber to safely eject the chlorine gas.")]
        [JsonProperty]
        public bool AllowStorageEjection { get; set; }




        /// <summary>
        /// Ctor
        /// </summary>
        public FixedOreScrubberModSettings()
        {
            ResetSettingsToDefault();
        }

        public void ResetSettingsToDefault()
        {
            RepeatedScubbing = true;
            AllowStorageEjection = true; // default if the config doesn't exist
            DiseaseRemovalCount = 480000;
        }
    }  
}
