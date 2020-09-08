// ----------------------------------------------------------------------------

using System;
using System.Reflection;

using Harmony;

// ----------------------------------------------------------------------------

namespace OniMods.PacuPathingFix
{
    [HarmonyPatch(typeof(GameNavGrids.SwimValidator))]
    [HarmonyPatch(nameof(GameNavGrids.SwimValidator.UpdateCell))]
    static class SwimValidator_Patch
    {
        /// <summary>
        /// Protected Base Method
        /// </summary>
        private static readonly MethodInfo _navTableValidatorIsClearBaseMethod = AccessTools.Method(typeof(NavTableValidator), "IsClear");

        /// <summary>
        /// GameNavGrids.SwimValidator.UpdateCell Prefix
        /// </summary>        
        static bool Prefix(GameNavGrids.SwimValidator __instance, int cell, NavTable nav_table, CellOffset[] bounding_offsets)
        {
            bool? isClear = NavTableValidator_IsClear(__instance, cell, bounding_offsets, false);

            if (!isClear.HasValue) // failed to access base method
                return true;       // skip patch and call original method

            bool is_valid = (Grid.IsSubstantialLiquid(cell, 0.9f) || (Grid.IsLiquid(cell) && (IsLiquidAbove(cell) || IsLiquidBelow(cell)))) && isClear.Value;
            nav_table.SetValid(cell, NavType.Swim, is_valid);

            return false; // skip execution of the original method
        }
        

        /// <summary>
        /// Check if there is a liquid above the cell
        /// </summary>        
        /// <returns>true if there is a liquid, otherwise false</returns>
        private static bool IsLiquidAbove(int cell)
        {
            int cellAbove = Grid.CellAbove(cell);

            return Grid.IsValidCell(cellAbove) && Grid.IsLiquid(cellAbove);
        }

        /// <summary>
        /// Check if there is a liquid below the cell
        /// </summary>
        /// <returns>true if there is a liquid, otherwise false</returns>
        private static bool IsLiquidBelow(int cell)
        {
            int cellBelow = Grid.CellBelow(cell);

            return Grid.IsValidCell(cellBelow) && Grid.IsLiquid(cellBelow);
        }

        /// <summary>
        /// Forwarded to NavTableValidator.IsClear        
        /// </summary>
        /// <returns>Returns result of NavTableValidator.IsClear Method or null in case of an error</returns>
        private static bool? NavTableValidator_IsClear(GameNavGrids.SwimValidator instance, int cell, CellOffset[] bounding_offsets, bool isDupe)
        {            
            try
            {
                if (_navTableValidatorIsClearBaseMethod?.Invoke(instance, new object[] { cell, bounding_offsets, isDupe }) is bool result)
                    return result;

                return null;
            }
            catch(Exception)
            {
                return null;
            }                      
        }
    }
}
