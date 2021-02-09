// ----------------------------------------------------------------------------

using Harmony;

// ----------------------------------------------------------------------------

namespace OniMods.PacuPathingFix
{
    [HarmonyPatch(typeof(GameNavGrids.SwimValidator))]
    [HarmonyPatch(nameof(GameNavGrids.SwimValidator.UpdateCell))]
    static class SwimValidator_Patch
    {        
        /// <summary>
        /// GameNavGrids.SwimValidator.UpdateCell Prefix
        /// </summary>        
        static bool Prefix(GameNavGrids.SwimValidator __instance, int cell, NavTable nav_table, CellOffset[] bounding_offsets)
        {
            bool? isClear = SwimValidatorHelper.NavTableValidator_IsClear(__instance, cell, bounding_offsets, false);

            if (!isClear.HasValue) // failed to access base method
                return true;       // skip patch and call original method

            bool is_swimmable = Grid.IsWorldValidCell(cell) && (Grid.IsSubstantialLiquid(cell, 0.9f) || (Grid.IsLiquid(cell) && (SwimValidatorHelper.IsLiquidAbove(cell) || SwimValidatorHelper.IsLiquidBelow(cell)))) && isClear.Value;
            nav_table.SetValid(cell, NavType.Swim, is_swimmable);

            return false; // skip execution of the original method
        }
    }
}
