// ----------------------------------------------------------------------------

using HarmonyLib;
using PeterHan.PLib.Options;

// ----------------------------------------------------------------------------

namespace OniMods.UnknownWorldTraits
{
    public class UnknownWorldTraitsMod : KMod.UserMod2
    {
        internal static string WorldTraitReplacementText { get; } = "[REDACTED]";

        internal static string ToolTipReplacementText { get; } = "Classified Information";
        

        /// <summary>
        /// Register Options on mod load
        /// </summary>
        public override void OnLoad(Harmony harmony)
        {
            harmony.PatchAll();

            // Registering for the config screen
            new POptions().RegisterOptions(this, typeof(UnknownWorldTraitsModSettings));
        }
    }
}
