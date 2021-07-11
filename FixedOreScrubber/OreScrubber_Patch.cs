// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;
using UnityEngine;

// ----------------------------------------------------------------------------

namespace OniMods.FixedOreScrubber
{
    [HarmonyPatch()]
    public static class ScrubOreReactable_InternalCanBegin_Patch
    {
        /// <summary>
        /// Get Nested Type
        /// </summary>
        /// <returns>InternalCanBegin MethodInfo</returns>
        static MethodInfo TargetMethod()
        {
            return typeof(OreScrubber).GetNestedType("ScrubOreReactable", BindingFlags.NonPublic).GetMethod("InternalCanBegin");
        }

        /// <summary>
        /// Swap the dupes facing before starting the reactable
        /// </summary>
        static void Prefix(GameObject new_reactor, Navigator.ActiveTransition transition)
        {
            PrimaryElement dupe = new_reactor?.GetComponent<PrimaryElement>();

            if (dupe != null)
            {
                Navigator navigator = dupe.GetComponent<Navigator>();

                if (navigator != null)
                {
                    NavGrid.Transition nextTransition = navigator.GetNextTransition();

                    if (nextTransition.x < 0)
                    {
                        // Mirror dupes facing direction to the initial direction
                        dupe.GetComponent<Facing>()?.SetFacing(true);
                    }
                }
            }
        }
    }


    [HarmonyPatch(typeof(OreScrubber.States))]
    [HarmonyPatch(nameof(OreScrubber.States.InitializeStates))]
    public static class OreScrubber_States_InitializeStates_Patch
    {
        /// <summary>
        /// States.notready field
        /// </summary>
        private static readonly FieldInfo f_notready = AccessTools.Field(typeof(OreScrubber.States), nameof(OreScrubber.States.notready));

        /// <summary>
        /// States.ready field
        /// </summary>
        private static readonly FieldInfo f_ready = AccessTools.Field(typeof(OreScrubber.States), nameof(OreScrubber.States.ready));

        /// <summary>            
        /// Change TargetState of Occupied-WorkableStopTransition, so ScrubOreReactable is repeatable
        /// </summary>
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            if (f_notready != null && f_ready != null)
            {
                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldfld && codes[i].operand as FieldInfo == f_ready)
                    {
                        int iNext = i + 1;

                        if (iNext < codes.Count && codes[iNext].operand is MethodInfo nextOperand)
                        {
                            // check if next instruction is WorkableStopTransition method
                            if (nextOperand.Name == nameof(GameStateMachine<OreScrubber.States, OreScrubber.SMInstance, OreScrubber>.State.WorkableStopTransition))
                            {
                                // Replace f_ready with f_notready
                                codes[i] = new CodeInstruction(OpCodes.Ldfld, f_notready);
                                break;
                            }
                        }
                    }
                }
            }

            return codes.AsEnumerable();
        }
    }
}
