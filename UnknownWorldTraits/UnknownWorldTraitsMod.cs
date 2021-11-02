// ----------------------------------------------------------------------------

using HarmonyLib;
using PeterHan.PLib.Options;

// ----------------------------------------------------------------------------

namespace OniMods.UnknownWorldTraits
{
    public class UnknownWorldTraitsMod : KMod.UserMod2
    {
        internal static string UnknownTraitText { get; } = "Unknown Trait";

        internal static string UnknownWorldTraitsText { get; } = "Unknown World Traits";      

        internal static string ClassifiedInformationText { get; } = "Classified Information";
        

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
