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
                    original: AccessTools.Method(typeof(TitleMenu), nameof(TitleMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(TitleMenuPatch), nameof(TitleMenuPatch.DrawPatch))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(LoadGameMenu.SaveFileSlot), nameof(LoadGameMenu.SaveFileSlot.Draw), new Type[] { typeof(SpriteBatch), typeof(int) }),
                postfix: new HarmonyMethod(typeof(LoadGameMenuPatch), nameof(LoadGameMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CharacterCustomization), nameof(CharacterCustomization.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(CharacterCustomizationMenuPatch), nameof(CharacterCustomizationMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CoopMenu), nameof(CoopMenu.update), new Type[] { typeof(GameTime) }),
                postfix: new HarmonyMethod(typeof(CoopMenuPatch), nameof(CoopMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(AdvancedGameOptions), nameof(AdvancedGameOptions.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(AdvancedGameOptionsPatch), nameof(AdvancedGameOptionsPatch.DrawPatch))
            );
            #endregion

            #region Menu Patches
            harmony.Patch(
                    original: AccessTools.Method(typeof(LetterViewerMenu), nameof(LetterViewerMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(LetterViwerMenuPatch), nameof(LetterViwerMenuPatch.DrawPatch))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(ShippingMenu), nameof(ShippingMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ShippingMenuPatch), nameof(ShippingMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(LevelUpMenuPatch), nameof(LevelUpMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ConfirmationDialog), nameof(ConfirmationDialog.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ConfirmationDialogMenuPatch), nameof(ConfirmationDialogMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(TitleTextInputMenu), nameof(TitleTextInputMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(TitleTextInputMenuPatch), nameof(TitleTextInputMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(NamingMenu), nameof(NamingMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(NamingMenuPatch), nameof(NamingMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(NumberSelectionMenu), nameof(NumberSelectionMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(NumberSelectionMenuPatch), nameof(NumberSelectionMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(MineElevatorMenu), nameof(MineElevatorMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MineElevatorMenuPatch), nameof(MineElevatorMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(LanguageSelectionMenu), nameof(LanguageSelectionMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(LanguageSelectionMenuPatch), nameof(LanguageSelectionMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ChooseFromListMenu), nameof(ChooseFromListMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ChooseFromListMenuPatch), nameof(ChooseFromListMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(PondQueryMenu), nameof(PondQueryMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(PondQueryMenuPatch), nameof(PondQueryMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ItemListMenu), nameof(ItemListMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ItemListMenuPatch), nameof(ItemListMenuPatch.DrawPatch))
            );
            #endregion

            #region Quest Patches
            harmony.Patch(
                    original: AccessTools.Method(typeof(SpecialOrdersBoard), nameof(SpecialOrdersBoard.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(SpecialOrdersBoardPatch), nameof(SpecialOrdersBoardPatch.DrawPatch))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(QuestLog), nameof(QuestLog.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(QuestLogPatch), nameof(QuestLogPatch.DrawPatch))
            );

            #endregion

            #region Animal and Building Menu

            harmony.Patch(
                    original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    prefix: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.DrawPatch))
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
