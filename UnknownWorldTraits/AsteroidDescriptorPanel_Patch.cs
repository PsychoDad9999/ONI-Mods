// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;
using ProcGen;
using STRINGS;

// ----------------------------------------------------------------------------

namespace OniMods.UnknownWorldTraits
{
    static class AsteroidDescriptorPanel_Patch
    {
        #region SpawnAsteroidLine Patch

        [HarmonyPatch(typeof(AsteroidDescriptorPanel))]
        [HarmonyPatch("SpawnAsteroidLine")]
        static class SpawnAsteroidLine_Patch
        {
            /// <summary>
            /// This Transpiler modifies the Asteroid Panels on the left hand side of the Destination Selection Screen (Spaced Out DLC)
            /// The transpiler removes the World Trait from the Panel-ToolTip and changes the color of the trait indicator
            /// </summary>
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

                MethodInfo toolTip_SetSimpleTooltip = typeof(ToolTip).GetMethod(nameof(ToolTip.SetSimpleTooltip));

                object stringRef = null;
                Label? targetJumpLabel = null;

                // GetAccessor of ProcGen.WorldTrait.colorHex Property
                MethodInfo worldTrait_GetHexColor = typeof(WorldTrait).GetProperty(nameof(WorldTrait.colorHex))?.GetAccessors()?.Where(x => x.ReturnType != typeof(void)).FirstOrDefault();

                bool isToolTipPatched = false;
                bool isColorPatched = false;

                // Patch the ToolTip
                if (toolTip_SetSimpleTooltip != null && worldTrait_GetHexColor != null)
                {
                    for (int idx = codes.Count - 1; idx > 0 && (!isToolTipPatched || !isColorPatched); idx--)
                    {
                        if (codes[idx].opcode == OpCodes.Callvirt && codes[idx].operand as MethodInfo == toolTip_SetSimpleTooltip)
                        {
                            // SetSimpleTooltip(text) was called. Get ldloc.s reference and label from previous opcodes
                            if (idx > 1)
                            {
                                if (codes[idx - 1].opcode == OpCodes.Ldloc_S && codes[idx - 2].opcode == OpCodes.Ldloc_S)
                                {
                                    stringRef = codes[idx - 1].operand;
                                    targetJumpLabel = codes[idx - 2].labels.FirstOrDefault();
                                }
                            }
                        }
                        else if (codes[idx].opcode == OpCodes.Br_S)
                        {
                            // overwrite the local string variable before the jump instruction. This way we can preserve the "No Trait"-codepath
                            if (codes[idx].operand is Label label && targetJumpLabel.HasValue && label == targetJumpLabel)
                            {
                                if (stringRef != null)
                                {
                                    List<CodeInstruction> insertInstructions = new List<CodeInstruction>()
                                    {
                                        new CodeInstruction(OpCodes.Ldstr, UnknownWorldTraitsMod.UnknownWorldTraitsText),
                                        new CodeInstruction(OpCodes.Stloc_S, stringRef),
                                    };

                                    codes.InsertRange(idx, insertInstructions);
                                    // No need to modify the index after inserting instructions, because the loop runs backwards.
                                    isToolTipPatched = true;
                                    continue;
                                }
                            }
                        }
                        else // Patch the color of the circle here
                        {
                            if (codes[idx].opcode == OpCodes.Callvirt && codes[idx].operand as MethodInfo == worldTrait_GetHexColor)
                            {
                                if (idx > 0)
                                {
                                    if (codes[idx - 1].opcode == OpCodes.Ldloc_S)
                                    {
                                        codes[idx - 1] = new CodeInstruction(OpCodes.Nop);
                                        codes[idx] = new CodeInstruction(OpCodes.Ldstr, "FFFFFF"); // Use White Color
                                        isColorPatched = true;
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }

                return codes.AsEnumerable();
            }
        }

        #endregion
    
        #region SetTraitDescriptors Patch

        [HarmonyPatch(typeof(AsteroidDescriptorPanel))]
        [HarmonyPatch(nameof(AsteroidDescriptorPanel.SetTraitDescriptors))]
        static class SetTraitDescriptors_Patch
        {
            /// <summary>
            /// Patch AsteroidDescriptorPanel.SetTraitDescriptors to modify the displayed Trait Descriptors
            /// This is used in the details panel on the right hand side of the Destination Selection Screen (Spaced Out DLC and Base Game)
            /// </summary>        
            static void Prefix(ref IList<AsteroidDescriptor> descriptors)
            {
                // Create a new List because we want to modify the argument without modifing
                // the original list in the ColonyDestinationAsteroidBeltData object
                IList<AsteroidDescriptor> modifiedDescriptors = new List<AsteroidDescriptor>();

                foreach (AsteroidDescriptor descriptor in descriptors)
                {
                    // copy structure
                    AsteroidDescriptor modifiedDescriptor = new AsteroidDescriptor(descriptor.text, descriptor.tooltip, descriptor.associatedColor, descriptor.bands);

                    if (modifiedDescriptor.text != WORLD_TRAITS.NO_TRAITS.NAME)
                    {
                        modifiedDescriptor.text = UnknownWorldTraitsMod.UnknownTraitText;
                        modifiedDescriptor.tooltip = String.Empty;
                        modifiedDescriptor.associatedColor = new UnityEngine.Color(255, 255, 255);
                    }

                    modifiedDescriptors.Add(modifiedDescriptor);                    
                }

                // pass the modified descriptor list to the original function
                descriptors = modifiedDescriptors;
            }
        }

        #endregion
    }
}
