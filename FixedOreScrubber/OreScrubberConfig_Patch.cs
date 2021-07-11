// ----------------------------------------------------------------------------

using HarmonyLib;
using UnityEngine;

// ----------------------------------------------------------------------------

namespace OniMods.FixedOreScrubber
{
    [HarmonyPatch(typeof(OreScrubberConfig))]
    [HarmonyPatch(nameof(OreScrubberConfig.ConfigureBuildingTemplate))]
    public static class OreScrubberConfig_ConfigureBuildingTemplate
    {
        static void Postfix(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<OreScrubber>().diseaseRemovalCount = FixedOreScrubberMod.Options.GermRemovalAmount;

            if (FixedOreScrubberMod.Options.AllowStorageEjection)
            {         
                go.AddOrGet<DropAllWorkable>();
            }
        }
    }
}
