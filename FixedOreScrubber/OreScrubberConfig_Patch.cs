// ----------------------------------------------------------------------------

using PeterHan.PLib.Options;
using Harmony;
using UnityEngine;

// ----------------------------------------------------------------------------

namespace OniMods.FixedOreScrubber
{
    [HarmonyPatch(typeof(OreScrubberConfig))]
    [HarmonyPatch(nameof(OreScrubberConfig.ConfigureBuildingTemplate))]
    static class OreScrubberConfig_ConfigureBuildingTemplate
    {
        static void Postfix(GameObject go, Tag prefab_tag)
        {
            if(POptions.ReadSettings<FixedOreScrubberModSettings>()?.AllowStorageEjection == true)            
            {
                go.AddOrGet<DropAllWorkable>();
            }
        }
    }    
}
