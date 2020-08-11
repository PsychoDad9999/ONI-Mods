// ----------------------------------------------------------------------------

using PeterHan.PLib.Options;

// ----------------------------------------------------------------------------

namespace OniMods.UnknownWorldTraits
{
    public static class UnknownWorldTraitsMod
    {   
        public static void OnLoad()
        {
            // Registering for the config screen
            POptions.RegisterOptions(typeof(UnknownWorldTraitsModSettings));
        }
    }
}
