using Harmony;
using Klei;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;


namespace Fixed_Ore_Scrubber
{
    public class Patches
    {

        [HarmonyPatch(typeof(OreScrubber.States))]
        [HarmonyPatch(nameof(OreScrubber.States.InitializeStates))]
        static class OreScrubber_States_InitializeStates_Patch
        {
            /// <summary>
            /// States.notready field
            /// </summary>
            private static FieldInfo f_notready = AccessTools.Field(typeof(OreScrubber.States), nameof(OreScrubber.States.notready));

            /// <summary>
            /// States.ready field
            /// </summary>
            private static FieldInfo f_ready = AccessTools.Field(typeof(OreScrubber.States), nameof(OreScrubber.States.ready));

            /// <summary>
            /// Set TargetState of WorkableStopTransition to States.notready field
            /// </summary>
            /// <param name="instructions"></param>
            /// <returns></returns>
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
                                    codes[i].operand = f_notready;
                                    break;
                                }
                            }
                        }
                    }
                }
                
                return codes.AsEnumerable();
            }
        }
  

        [HarmonyPatch(typeof(OreScrubber.Work))]
        [HarmonyPatch("OnCompleteWork")]
        static class OreScrubber_Work_OnCompleteWork_Patch
        {
            static void Prefix(Worker worker)
            {
                GameObject obj = worker?.gameObject;
                PrimaryElement dupe = obj?.GetComponent<PrimaryElement>();              

                if (dupe != null)
                {
                    // Mirror dupes facing direction
                    dupe.GetComponent<Facing>()?.SetFacing(true);                                       
                }
            }
        }
    }
}
