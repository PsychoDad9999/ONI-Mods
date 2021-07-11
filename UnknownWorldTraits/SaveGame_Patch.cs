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

    [HarmonyPatch(typeof(SaveGame))]
    [HarmonyPatch(nameof(SaveGame.GetColonyToolTip))]
    static class SaveGame_GetColonyToolTip_Patch
    {
        /// <summary>
        /// Patch GetColonyToolTip to hide world traits on Colony-ToolTip
        /// </summary>
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            // Get WorldTrait.Name Property
            PropertyInfo worldTraitNamePropInfo = typeof(WorldTrait).GetProperty(nameof(WorldTrait.name));

            // Get "Get-Accessors" of WorldTrait.Name Property
            MethodInfo worldTraitGetName = worldTraitNamePropInfo?.GetAccessors()?.Where(x => x.ReturnType != typeof(void)).FirstOrDefault();

            // Get op_Implicit MethodInfo
            MethodInfo stringEntry_Implicit = typeof(StringEntry).GetMethod("op_Implicit");

            if (worldTraitGetName != null && stringEntry_Implicit != null)
            {
                for (int i = 0; i < codes.Count; i++)
                {
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
                                codes[i] = new CodeInstruction(OpCodes.Ldstr, "[REDACTED]");
                                codes[i + 1] = new CodeInstruction(OpCodes.Nop);
                                codes[i + 2] = new CodeInstruction(OpCodes.Nop);
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
