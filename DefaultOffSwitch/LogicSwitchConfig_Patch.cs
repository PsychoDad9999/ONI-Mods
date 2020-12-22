// ----------------------------------------------------------------------------

using Harmony;
using UnityEngine;

// ----------------------------------------------------------------------------

namespace OniMods.DefaultOffSwitch
{
    [HarmonyPatch(typeof(LogicSwitchConfig))]
    [HarmonyPatch(nameof(LogicSwitchConfig.DoPostConfigureComplete))]
    static class LogicSwitchConfig_DoPostConfigureComplete_Patch
    {
        /// <summary>
        /// LogicSwitchConfig.DoPostConfigureComplete Postfix
        /// </summary>
        static void Postfix(GameObject go)
        {
            // Set default state
            go.AddOrGet<LogicSwitch>().defaultState = DefaultOffSwitch.Options.SignalSwitchDefaultState == DefaultOffSwitchSettings.SwitchState.On;
        }
    }
}
