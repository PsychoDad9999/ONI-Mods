﻿// ----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;
using ProcGen;

// ----------------------------------------------------------------------------

namespace OniMods.UnknownWorldTraits
{
    [HarmonyPatch(typeof(SimpleInfoScreen))]
    [HarmonyPatch("RefreshWorld")]
    static class SimpleInfoScreen_Patch
    {
        /// <summary>
        /// Patch SimpleInfoScreen.RefreshWorld to hide the world traits on the Starmap
        /// </summary>
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            // Get "Get-Accessors" of WorldTrait.Name Property
            MethodInfo worldTraitGetName = typeof(WorldTrait).GetProperty(nameof(WorldTrait.name))?.GetAccessors()?.Where(x => x.ReturnType != typeof(void)).FirstOrDefault();

            // Get "Get-Accessors" of WorldTrait.Description Property
            MethodInfo worldTraitGetDescription = typeof(WorldTrait).GetProperty(nameof(WorldTrait.description))?.GetAccessors()?.Where(x => x.ReturnType != typeof(void)).FirstOrDefault();

            // Get "Get-Accessors" of WorldTrait.colorHex Property
            MethodInfo worldTraitGetColorHex = typeof(WorldTrait).GetProperty(nameof(WorldTrait.colorHex))?.GetAccessors()?.Where(x => x.ReturnType != typeof(void)).FirstOrDefault();

            // Get WorldTrait.filePath Field
            FieldInfo worldTraitGetFilePath = AccessTools.Field(typeof(WorldTrait), nameof(WorldTrait.filePath));

            // Get op_Implicit MethodInfo
            MethodInfo stringEntry_Implicit = typeof(StringEntry).GetMethod("op_Implicit");

            if (worldTraitGetName != null && worldTraitGetDescription != null && worldTraitGetColorHex != null && worldTraitGetFilePath != null && stringEntry_Implicit != null)
            {
                for (int i = 0; i < codes.Count; i++)
                {
                    // Patch String
                    if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand as MethodInfo == worldTraitGetName)
                    {
                        if (i > 0 && i <= codes.Count - 3)
                        {
                            // make some consistency checks
                            CodeInstruction previousCodeInstruction = codes[i - 1];
                            CodeInstruction lastCodeInstruction = codes[i + 2];

                            if (previousCodeInstruction.opcode == OpCodes.Ldloc_S &&
                                lastCodeInstruction.opcode == OpCodes.Call && lastCodeInstruction.operand as MethodInfo == stringEntry_Implicit)
                            {
                                // Patch ToolTip
                                codes[i - 1] = new CodeInstruction(OpCodes.Nop);
                                codes[i] = new CodeInstruction(OpCodes.Ldstr, UnknownWorldTraitsMod.UnknownTraitText);
                                codes[i + 1] = new CodeInstruction(OpCodes.Nop);
                                codes[i + 2] = new CodeInstruction(OpCodes.Nop);

                                i += 2;
                                continue;
                            }
                        }
                    }

                    // Patch Tool Tip
                    if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand as MethodInfo == worldTraitGetDescription)
                    {
                        if (i > 0 && i <= codes.Count - 3)
                        {
                            // make some consistency checks
                            CodeInstruction previousCodeInstruction = codes[i - 1];
                            CodeInstruction lastCodeInstruction = codes[i + 2];

                            if (previousCodeInstruction.opcode == OpCodes.Ldloc_S &&
                                lastCodeInstruction.opcode == OpCodes.Call && lastCodeInstruction.operand as MethodInfo == stringEntry_Implicit)
                            {
                                // Patch ToolTip
                                codes[i - 1] = new CodeInstruction(OpCodes.Nop);
                                codes[i] = new CodeInstruction(OpCodes.Ldstr, UnknownWorldTraitsMod.ClassifiedInformationText);
                                codes[i + 1] = new CodeInstruction(OpCodes.Nop);
                                codes[i + 2] = new CodeInstruction(OpCodes.Nop);

                                i += 2;
                                continue;
                            }
                        }
                    }

                    // Patch Icon color
                    if (codes[i].opcode == OpCodes.Callvirt && codes[i].operand as MethodInfo == worldTraitGetColorHex)
                    {
                        if (i > 0)
                        {
                            if (codes[i - 1].opcode == OpCodes.Ldloc_S)
                            {
                                codes[i - 1] = new CodeInstruction(OpCodes.Nop);
                                codes[i] = new CodeInstruction(OpCodes.Ldstr, "000000"); // Use Black Color
                                continue;
                            }
                        }
                    }

                    // Patch Icon
                    if (codes[i].opcode == OpCodes.Ldfld && codes[i].operand as FieldInfo == worldTraitGetFilePath)
                    {
                        if (i > 0)
                        {
                            if (codes[i - 1].opcode == OpCodes.Ldloc_S)
                            {
                                codes[i - 1] = new CodeInstruction(OpCodes.Nop);
                                codes[i] = new CodeInstruction(OpCodes.Ldstr, UnknownWorldTraitsMod.SpriteNameAsFilePath);
                                continue;
                            }
                        }
                    }
                }
            }

            return codes.AsEnumerable();
        }
    }
}
