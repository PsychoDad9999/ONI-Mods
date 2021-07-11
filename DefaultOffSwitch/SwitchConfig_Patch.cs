// ----------------------------------------------------------------------------

using HarmonyLib;
using UnityEngine;

// ----------------------------------------------------------------------------

namespace OniMods.DefaultOffSwitch
{
    [HarmonyPatch(typeof(SwitchConfig))]
    [HarmonyPatch(nameof(SwitchConfig.DoPostConfigureComplete))]
    static class SwitchConfig_Patch
    {
        /// <summary>
        /// SwitchConfig.DoPostConfigureComplete Postfix
        /// </summary>
        static void Postfix(GameObject go)
        {           
            // Set default state
            go.AddOrGet<Switch>().defaultState = DefaultOffSwitch.Options.PowerSwitchDefaultState == DefaultOffSwitchSettings.SwitchState.On;
        }
    }
}
