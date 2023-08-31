using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Patches;
using StardewValley.Menus;
using StardewValley.Minigames;

namespace stardew_access
{
    internal class HarmonyPatches
    {
        internal static void Initialize(Harmony harmony)
        {
            #region Title Menu Patches
            harmony.Patch(
                original: AccessTools.Method(typeof(CharacterCustomization), nameof(CharacterCustomization.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(CharacterCustomizationMenuPatch), nameof(CharacterCustomizationMenuPatch.DrawPatch))
            );

            #endregion

            #region Mini Games
            harmony.Patch(
                        original: AccessTools.Method(typeof(Intro), nameof(Intro.draw), new Type[] { typeof(SpriteBatch) }),
                        postfix: new HarmonyMethod(typeof(IntroPatch), nameof(IntroPatch.DrawPatch))
                    );

            harmony.Patch(
                        original: AccessTools.Method(typeof(GrandpaStory), nameof(GrandpaStory.draw), new Type[] { typeof(SpriteBatch) }),
                        postfix: new HarmonyMethod(typeof(GrandpaStoryPatch), nameof(GrandpaStoryPatch.DrawPatch))
                    );

            harmony.Patch(
                        original: AccessTools.Method(typeof(BobberBar), nameof(BobberBar.update)),
                        postfix: new HarmonyMethod(typeof(FishingMiniGamePatch), nameof(FishingMiniGamePatch.BobberBarPatch))
                    );

            #endregion
        }
    }
}
