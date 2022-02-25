// ----------------------------------------------------------------------------

using System.Collections.Generic;

using HarmonyLib;
using Klei.CustomSettings;
using STRINGS;
using UnityEngine;

// ----------------------------------------------------------------------------

namespace OniMods.FoodDiagnosticsFix
{
    [HarmonyPatch(typeof(FoodDiagnostic))]
    [HarmonyPatch("CheckEnoughFood")]
    static class FoodDiagnostic_CheckEnoughFood_Patch
    {
        /// <summary>
        /// FoodDiagnostic.CheckEnoughFood Prefix
        /// </summary>        
        static bool Prefix(FoodDiagnostic __instance, ref ColonyDiagnostic.DiagnosticResult __result)
        {            
            __result = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);

            List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(__instance.worldID);
            if (__instance.tracker.GetDataTimeLength() < 10f)
            {
                __result.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
                __result.Message = UI.COLONY_DIAGNOSTICS.NO_DATA;
            }
            else
            {
                float trackerSampleCountSeconds = Traverse.Create<FoodDiagnostic>().Field("trackerSampleCountSeconds").GetValue<float>();
                float caloriesPerDupePerDay = GetRequiredFoodPerDupe();

                int daysReserve = 3; // show warning if food doesn't last for 3 days
                int dupesCount = worldItems.Count;

                if (caloriesPerDupePerDay * dupesCount * (float)daysReserve > __instance.tracker.GetAverageValue(trackerSampleCountSeconds))
                {
                    __result.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
                    float currentValue = __instance.tracker.GetCurrentValue();
                    float minRequiredValue = dupesCount * caloriesPerDupePerDay;

                    string formattedCalories = GameUtil.GetFormattedCalories(currentValue);
                    string formattedCalories2 = GameUtil.GetFormattedCalories(Mathf.Abs(minRequiredValue));

                    string text = MISC.NOTIFICATIONS.FOODLOW.TOOLTIP;
                    text = text.Replace("{0}", formattedCalories);
                    text = text.Replace("{1}", formattedCalories2);

                    __result.Message = text;              
                }
            }

            return false; // skip execution of the original method
        }



        static float GetRequiredFoodPerDupe()
        {
            SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.CalorieBurn);

            switch(currentQualitySetting.id)
            {
                case "VeryHard":
                    return 2000000f;  //2000Kcal

                case "Hard":
                    return 1500000f;  //1500Kcal

                default:
                    return 1000000f; //1000Kcal

                case "Easy":
                    return 500000f;  //500Kcal

                case "Disabled":
                    return 0f;
            }           
        }
    }    
}
