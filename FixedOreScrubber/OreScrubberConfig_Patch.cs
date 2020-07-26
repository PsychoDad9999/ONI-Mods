// ----------------------------------------------------------------------------

using Harmony;
using UnityEngine;

using PeterHan.PLib.Options;

// ----------------------------------------------------------------------------

namespace OniMods.FixedOreScrubber
{
    [HarmonyPatch(typeof(OreScrubberConfig))]
    [HarmonyPatch(nameof(OreScrubberConfig.ConfigureBuildingTemplate))]
    static class OreScrubberConfig_ConfigureBuildingTemplate
    {
        static void Postfix(GameObject go, Tag prefab_tag)
        {
            if(FixedOreScrubberMod.Options.AllowStorageEjection)
            {         
                go.AddOrGet<DropAllWorkable>();
            }
        }
    }
}
