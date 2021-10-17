// ----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text.RegularExpressions;

using HarmonyLib;
using Klei.CustomSettings;
using PeterHan.PLib.Options;
using STRINGS;

// ----------------------------------------------------------------------------

namespace OniMods.UnknownWorldTraits
{    
    [HarmonyPatch(typeof(ColonyDestinationSelectScreen))]
    [HarmonyPatch("SettingChanged")]
    static class ColonyDestinationSelectScreen_SettingChanged_Patch
    {
        /// <summary>
        /// Patch ColonyDestinationSelectScreen.SettingChanged to replace displayed Trait Descriptors
        /// </summary>        
        static void Postfix(ref NewGameSettingsPanel ___newGameSettings, ref DestinationSelectPanel ___destinationMapPanel, ref AsteroidDescriptorPanel ___startLocationProperties)
        {
            string setting = ___newGameSettings.GetSetting(CustomGameSettingConfigs.ClusterLayout);
            int.TryParse(___newGameSettings.GetSetting(CustomGameSettingConfigs.WorldgenSeed), out int result);
            ColonyDestinationAsteroidBeltData colonyDestinationAsteroidData = ___destinationMapPanel.SelectAsteroid(setting, result);

            ___startLocationProperties.SetDescriptors(GetModifiedTraitDescriptors(colonyDestinationAsteroidData.GetTraitDescriptors()));
        }


        /// <summary>
        /// Get modified Trait Descriptors
        /// </summary>
        /// <param name="traitDescriptors">List of Trait descriptors</param>
        /// <returns>Returns a List of modified Trait Descriptors</returns>
        private static IList<AsteroidDescriptor> GetModifiedTraitDescriptors(IList<AsteroidDescriptor> traitDescriptors)
        {            
            // Read Mod Settings
            UnknownWorldTraitsModSettings modSettings = POptions.ReadSettings<UnknownWorldTraitsModSettings>() ?? new UnknownWorldTraitsModSettings();
           
            for (int i = 0; i < traitDescriptors.Count; i++)
            {                
                if (traitDescriptors[i].text == null || traitDescriptors[i].text.Length == 0)
                    continue;
               
                // skip if world has no Traits, e.g. Terra
                if (traitDescriptors[i].text == WORLD_TRAITS.NO_TRAITS.NAME)
                    continue;

                // skip colony names. Only world traits starts with <color>
                if (!traitDescriptors[i].text.StartsWith("<color="))
                    continue;

                AsteroidDescriptor traitDescriptor;
                traitDescriptor.text = CreateTraitDescriptorText(traitDescriptors[i].text, UnknownWorldTraitsMod.WorldTraitReplacementText, modSettings.ShowTraitColor);
                traitDescriptor.tooltip = UnknownWorldTraitsMod.ToolTipReplacementText;
                traitDescriptor.bands = traitDescriptors[i].bands;

                traitDescriptors[i] = traitDescriptor;
            }

            return traitDescriptors;
        }


        /// <summary>
        /// Create Trait Descriptor Text
        /// </summary>
        /// <param name="originalTraitDescriptorText">Original color formated text descriptor, e.g.<color=#FFFFFF>Test</color></param>
        /// <param name="newDescriptorText">New Text, e.g. Test2</param>
        /// <param name="useTraitTextColor">True to use color formatation of original text descriptor</param>
        /// <returns>Returns either a text descriptor or a color formated text descriptor</returns>        
        private static string CreateTraitDescriptorText(string originalTraitDescriptorText, string newDescriptorText, bool useTraitTextColor)
        {
            if(useTraitTextColor)
            {
                string colorHex = GetHexColorFromDescriptor(originalTraitDescriptorText);

                if(colorHex != null)
                {
                    return string.Format("<color=#{1}>{0}</color>", newDescriptorText, colorHex);                    
                }                
            }

            return newDescriptorText;            
        }


        /// <summary>
        /// Get Hex Color Value from descriptor
        /// </summary>        
        /// <param name="colorFormatedText">Color formated text descriptor, e.g.<color=#FFFFFF>Test</color></param>
        /// <returns>hex color string or null</returns>
        /// <example>Returns FFFFFF if colorFormatedText is <color=#FFFFFF>Test</color></example>
        private static string GetHexColorFromDescriptor(string colorFormatedText)
        {
            string pattern = @"<color=#([0-9a-fA-F]{6})>.*<\/color>";

            Match match = Regex.Match(colorFormatedText, pattern);
                
            if (match.Success && match.Groups.Count > 1)
            {                    
                return match.Groups[1].Value;
            }
            else
            {
                return null;
            }            
        }
    }
}
