using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Patches;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access
{
    internal class HarmonyPatches
    {
        internal static void Initialize(Harmony harmony)
        {
            #region Dialogue Patches
            harmony.Patch(
                   original: AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.draw), new Type[] { typeof(SpriteBatch) }),
                   postfix: new HarmonyMethod(typeof(DialoguePatches), nameof(DialoguePatches.DialoguePatch))
                );

            harmony.Patch(
               original: AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.receiveLeftClick)),
               postfix: new HarmonyMethod(typeof(DialoguePatches), nameof(DialoguePatches.ClearDialogueString))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.drawHoverText), new Type[] { typeof(SpriteBatch), typeof(string), typeof(SpriteFont), typeof(int), typeof(int), typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe), typeof(IList<Item>) }),
                postfix: new HarmonyMethod(typeof(DialoguePatches), nameof(DialoguePatches.HoverTextPatch))
            );
            #endregion

            #region Title Menu Patches
            harmony.Patch(
                    original: AccessTools.Method(typeof(TitleMenu), nameof(TitleMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(TitleMenuPatches), nameof(TitleMenuPatches.TitleMenuPatch))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(LoadGameMenu.SaveFileSlot), nameof(LoadGameMenu.SaveFileSlot.Draw), new Type[] { typeof(SpriteBatch), typeof(int) }),
                postfix: new HarmonyMethod(typeof(TitleMenuPatches), nameof(TitleMenuPatches.LoadGameMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CharacterCustomization), nameof(CharacterCustomization.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(TitleMenuPatches), nameof(TitleMenuPatches.NewGameMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CoopMenu), nameof(CoopMenu.update), new Type[] { typeof(GameTime) }),
                postfix: new HarmonyMethod(typeof(TitleMenuPatches), nameof(TitleMenuPatches.CoopMenuPatch))
            );
            #endregion

            #region Game Menu Patches
            harmony.Patch(
                original: AccessTools.Method(typeof(GameMenu), nameof(GameMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.GameMenuPatch))
            );

            harmony.Patch(
                    original: AccessTools.Method(typeof(OptionsPage), nameof(OptionsPage.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.OptionsPagePatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ExitPage), nameof(ExitPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.ExitPagePatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingPage), nameof(CraftingPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.CraftingPagePatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.InventoryPagePatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.ItemGrabMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.GeodeMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.ShopMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(SocialPage), nameof(SocialPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.SocialPagePatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(JunimoNoteMenu), nameof(JunimoNoteMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatches), nameof(GameMenuPatches.JunimoNoteMenuPatch))
            );
            #endregion

            #region Menu Patches
            harmony.Patch(
                    original: AccessTools.Method(typeof(LetterViewerMenu), nameof(LetterViewerMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.LetterViewerMenuPatch))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(ShippingMenu), nameof(ShippingMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.ShippingMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.LevelUpMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ConfirmationDialog), nameof(ConfirmationDialog.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.ConfirmationDialogPatch))
            );

            harmony.Patch(
                original: AccessTools.Constructor(typeof(NamingMenu), new Type[] { typeof(NamingMenu.doneNamingBehavior), typeof(string), typeof(string) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.NamingMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(MineElevatorMenu), nameof(MineElevatorMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.MineElevatorMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(LanguageSelectionMenu), nameof(LanguageSelectionMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.LanguageSelectionMenuPatch))
            );
            #endregion

            #region Quest Patches
            harmony.Patch(
                    original: AccessTools.Method(typeof(SpecialOrdersBoard), nameof(SpecialOrdersBoard.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(QuestPatches), nameof(QuestPatches.SpecialOrdersBoardPatch))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(QuestLog), nameof(QuestLog.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(QuestPatches), nameof(QuestPatches.QuestLogPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Billboard), nameof(Billboard.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(QuestPatches), nameof(QuestPatches.BillboardPatch))
            );
            #endregion

            #region Chat Menu Patches
            harmony.Patch(
                    original: AccessTools.Method(typeof(ChatBox), nameof(ChatBox.update), new Type[] { typeof(GameTime) }),
                    postfix: new HarmonyMethod(typeof(ChatManuPatches), nameof(ChatManuPatches.ChatBoxPatch))
                );
            #endregion

            #region On Menu CLose Patch
            harmony.Patch(
                    original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.exitThisMenu)),
                    postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.IClickableMenuOnExitPatch))
                );
            harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), nameof(Game1.exitActiveMenu)),
                    prefix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.Game1ExitActiveMenuPatch))
                );
            #endregion

            #region Animal and Building Menu

            harmony.Patch(
                    original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    prefix: new HarmonyMethod(typeof(BuildingNAnimalMenuPatches), nameof(BuildingNAnimalMenuPatches.CarpenterMenuPatch))
                );

            #endregion

            harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), nameof(Game1.playSound)),
                    prefix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.PlaySoundPatch))
                );

            harmony.Patch(
                    original: AccessTools.Method(typeof(InstanceGame), nameof(InstanceGame.Exit)),
                    prefix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.ExitEventPatch))
                );
        }
    }
}
