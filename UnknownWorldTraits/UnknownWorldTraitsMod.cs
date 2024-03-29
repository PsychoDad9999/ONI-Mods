﻿// ----------------------------------------------------------------------------

using HarmonyLib;

// ----------------------------------------------------------------------------

namespace OniMods.UnknownWorldTraits
{
    public class UnknownWorldTraitsMod : KMod.UserMod2
    {
        internal static string UnknownTraitText { get; } = "Unknown Trait";

        internal static string UnknownWorldTraitsText { get; } = "Unknown World Traits";      

        internal static string ClassifiedInformationText { get; } = "Classified Information";

        internal static string SpriteName { get; } = "unknown";

        internal static string SpriteNameAsFilePath { get; } = @"/unknown";


        /// <summary>
        /// Register Options on mod load
        /// </summary>
        public override void OnLoad(Harmony harmony)
        {
            harmony.PatchAll();            
        }
    }
}
