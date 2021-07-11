// ----------------------------------------------------------------------------

using System;
using System.Reflection;

using HarmonyLib;

// ----------------------------------------------------------------------------

namespace OniMods.PacuPathingFix
{
    internal static class SwimValidatorHelper
    {
        /// <summary>
        /// Protected Base Method
        /// </summary>
        private static readonly MethodInfo _navTableValidatorIsClearBaseMethod = AccessTools.Method(typeof(NavTableValidator), "IsClear");

        /// <summary>
        /// Forwarded to NavTableValidator.IsClear        
        /// </summary>
        /// <returns>Returns result of NavTableValidator.IsClear Method or null in case of an error</returns>
        public static bool? NavTableValidator_IsClear(GameNavGrids.SwimValidator instance, int cell, CellOffset[] bounding_offsets, bool isDupe)
        {
            try
            {
                if (_navTableValidatorIsClearBaseMethod?.Invoke(instance, new object[] { cell, bounding_offsets, isDupe }) is bool result)
                    return result;

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Check if there is a liquid above the cell
        /// </summary>        
        /// <returns>true if there is a liquid, otherwise false</returns>
        public static bool IsLiquidAbove(int cell)
        {
            int cellAbove = Grid.CellAbove(cell);

            return Grid.IsValidCell(cellAbove) && Grid.IsLiquid(cellAbove);
        }

        /// <summary>
        /// Check if there is a liquid below the cell
        /// </summary>
        /// <returns>true if there is a liquid, otherwise false</returns>
        public static bool IsLiquidBelow(int cell)
        {
            int cellBelow = Grid.CellBelow(cell);

            return Grid.IsValidCell(cellBelow) && Grid.IsLiquid(cellBelow);
        }        
    }
}
