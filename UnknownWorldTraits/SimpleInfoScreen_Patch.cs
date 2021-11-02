// ----------------------------------------------------------------------------

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

            // Get op_Implicit MethodInfo
            MethodInfo stringEntry_Implicit = typeof(StringEntry).GetMethod("op_Implicit");

            if (worldTraitGetName != null && worldTraitGetDescription != null && stringEntry_Implicit != null)
            {
                for (int i = 0; i < codes.Count; i++)
                {
                    // Patch 
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
                }
            }

            return codes.AsEnumerable();
        }
    }
}
