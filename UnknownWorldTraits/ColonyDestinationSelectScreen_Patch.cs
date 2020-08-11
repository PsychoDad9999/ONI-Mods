// ----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text.RegularExpressions;

using Harmony;
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
        static void Postfix(ref NewGameSettingsPanel ___newGameSettings, ref DestinationSelectPanel ___destinationMapPanel, ref AsteroidDescriptorPanel ___startLocationProperties)
        {
            string setting = ___newGameSettings.GetSetting(CustomGameSettingConfigs.World);
            int.TryParse(___newGameSettings.GetSetting(CustomGameSettingConfigs.WorldgenSeed), out int result);
            ColonyDestinationAsteroidData colonyDestinationAsteroidData = ___destinationMapPanel.SelectAsteroid(setting, result);

            ___startLocationProperties.SetDescriptors(ModifyTraitDescriptors(colonyDestinationAsteroidData.GetTraitDescriptors()));
        }


        private static IList<AsteroidDescriptor> ModifyTraitDescriptors(IList<AsteroidDescriptor> descriptors)
        {
            // Read Mod Settings
            UnknownWorldTraitsModSettings modSettings = POptions.ReadSettings<UnknownWorldTraitsModSettings>() ?? new UnknownWorldTraitsModSettings();                        

            for (int i = 0; i < descriptors.Count; i++)
            {
                if (descriptors[i].text == WORLD_TRAITS.NO_TRAITS.NAME)
                    continue;

                AsteroidDescriptor traitDescriptor;
                traitDescriptor.text = ChangeTraitDescriptorText(descriptors[i].text, "???", modSettings.ShowTraitColor);
                traitDescriptor.tooltip = "Unknown world trait";
                traitDescriptor.bands = descriptors[i].bands;

                descriptors[i] = traitDescriptor;
            }

            return descriptors;
        }



        private static string ChangeTraitDescriptorText(string originalTraitDescriptorText, string newTraitDescriptorText, bool useTraitTextColor)
        {
            if(useTraitTextColor)
            {
                string colorHex = GetHexColor(originalTraitDescriptorText);

                if(colorHex != null)
                {
                    return string.Format("<color=#{1}>{0}</color>", newTraitDescriptorText, colorHex);                    
                }                
            }

            return newTraitDescriptorText;            
        }



        private static string GetHexColor(string text)
        {
            string pattern = @"<color=#([0-9a-fA-F]{6})>.*<\/color>";

            Match match = Regex.Match(text, pattern);
                
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
