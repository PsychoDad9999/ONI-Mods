// ----------------------------------------------------------------------------

using PeterHan.PLib.Options;

// ----------------------------------------------------------------------------

namespace OniMods.UnknownWorldTraits
{
    public static class UnknownWorldTraitsMod
    {   
        /// <summary>
        /// Register Options on mod load
        /// </summary>
        public static void OnLoad()
        {
            // Registering for the config screen
            POptions.RegisterOptions(typeof(UnknownWorldTraitsModSettings));
        }
    }
}
