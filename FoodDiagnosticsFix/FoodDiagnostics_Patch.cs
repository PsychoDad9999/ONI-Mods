// ----------------------------------------------------------------------------

using System.Collections.Generic;

using HarmonyLib;
using Klei.AI;
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
           

            if (__instance.tracker.GetDataTimeLength() < 10f)
            {
                __result.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
                __result.Message = UI.COLONY_DIAGNOSTICS.NO_DATA;
            }
            else
            {
                List<MinionIdentity> dupes = Components.LiveMinionIdentities.GetWorldItems(__instance.worldID);
                float trackerSampleCountSeconds = Traverse.Create<FoodDiagnostic>().Field("trackerSampleCountSeconds").GetValue<float>();
                float requiredCaloriesPerCycle = GetRequiredFoodPerCycleByAttributeModifier(dupes);

                int daysReserve = 3; // show warning if food doesn't last for 3 days

                if (requiredCaloriesPerCycle * daysReserve > __instance.tracker.GetAverageValue(trackerSampleCountSeconds))
                {
                    __result.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
                    float currentValue = __instance.tracker.GetCurrentValue();

                    string text = MISC.NOTIFICATIONS.FOODLOW.TOOLTIP;
                    text = text.Replace("{0}", GameUtil.GetFormattedCalories(currentValue));
                    text = text.Replace("{1}", GameUtil.GetFormattedCalories(requiredCaloriesPerCycle));

                    __result.Message = text;
                }
            }

            return false; // skip execution of the original method
        }



        private static float ToCaloriesPerCycle(float caloriesPerSec)
        {
            return caloriesPerSec * 600f;
        }


        /// <summary>
        ///  Get required food per cycle by attribute modifiers
        /// </summary>
        /// <returns>Returns calories per cycle</returns>
        private static float GetRequiredFoodPerCycleByAttributeModifier(List<MinionIdentity> dupes)
        {
            float totalCalories = 0;

            if (dupes != null)
            {               
                foreach (MinionIdentity dupe in dupes)
                {
                    float caloriesPerSecond = dupe.GetAmounts().Get(Db.Get().Amounts.Calories).GetDelta();

                    // "tummyless" attribute adds float.PositiveInfinity
                    if (caloriesPerSecond != float.PositiveInfinity)
                    {
                        totalCalories += ToCaloriesPerCycle(caloriesPerSecond);
                    }
                }
            }

            return Mathf.Abs(totalCalories);
        }


        /// <summary>
        /// Get required food per cycle by difficulty setting
        /// </summary>
        /// <returns>Returns calories per cycle</returns>
        private static float GetRequiredFoodPerClycleByDifficultySetting(List<MinionIdentity> dupes)
        {
            if(dupes != null)
            {
                return GetRequiredFoodPerDupeByDifficultySetting() * dupes.Count;
            }
            else
            {
                return 0f;
            }
        }


        private static float GetRequiredFoodPerDupeByDifficultySetting()
        {
            SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.CalorieBurn);

            switch (currentQualitySetting.id)
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
