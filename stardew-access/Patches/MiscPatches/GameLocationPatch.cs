using HarmonyLib;
using stardew_access.Utils;
using StardewValley;
using System.Reflection;
using xTile;

namespace stardew_access.Patches
{
    internal class GameLocationPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            // Retrieve the LoadMap method info from GameLocation
            MethodInfo? loadMapMethod = typeof(GameLocation).GetMethod("loadMap");

            if (loadMapMethod != null)
            {
                // Create a postfix method
                var postfix = new HarmonyMethod(typeof(GameLocationPatch), nameof(GameLocationLoadMapPatch));
                
                // Apply the patch
                harmony.Patch(loadMapMethod, postfix: postfix);
                Log.Verbose("GameLocationPatch: Successfully patched LoadMap method");
            }
            else
            {
                Log.Error("GameLocationPatch: Could not find the LoadMap method. Exiting patching sequence.");
            }
        }

        /// <summary>
        /// Postfix for GameLocation.LoadMap method.
        /// </summary>
        /// <param name="__instance">The GameLocation instance.</param>
        private static void GameLocationLoadMapPatch(GameLocation __instance)
        {
            // Update the map in the TileUtils dictionary
            if (__instance.map != null)
            {
                TileUtils.MapNames[__instance.map] = __instance.NameOrUniqueName;
                #if DEBUG
                Log.Verbose($"GameLocationPatch: Updated map and name for {__instance.NameOrUniqueName}");
                #endif
            }
        }
    }
}
