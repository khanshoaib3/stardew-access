using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using stardew_access.Patches;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;

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

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.drawAboveAlwaysFrontLayer)),
                postfix: new HarmonyMethod(typeof(DialoguePatches), nameof(DialoguePatches.drawAboveAlwaysFrontLayerPatch))
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
                postfix: new HarmonyMethod(typeof(CharacterCustomizationMenuPatch), nameof(CharacterCustomizationMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CoopMenu), nameof(CoopMenu.update), new Type[] { typeof(GameTime) }),
                postfix: new HarmonyMethod(typeof(TitleMenuPatches), nameof(TitleMenuPatches.CoopMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(AdvancedGameOptions), nameof(AdvancedGameOptions.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(TitleMenuPatches), nameof(TitleMenuPatches.AdvancedGameOptionsPatch))
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
                postfix: new HarmonyMethod(typeof(CraftingPagePatch), nameof(CraftingPagePatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(InventoryPagePatch), nameof(InventoryPagePatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ItemGrabMenuPatch), nameof(ItemGrabMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(GeodeMenu), nameof(GeodeMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(GeodeMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ShopMenuPatch), nameof(ShopMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(SocialPage), nameof(SocialPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(SocialPagePatch), nameof(SocialPagePatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CollectionsPage), nameof(CollectionsPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(CollectionsPagePatch), nameof(CollectionsPagePatch.DrawPatch))
            );
            #endregion

            #region Bundle Menu Patches
            harmony.Patch(
                original: AccessTools.Method(typeof(JunimoNoteMenu), nameof(JunimoNoteMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(BundleMenuPatches), nameof(BundleMenuPatches.JunimoNoteMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(JojaCDMenu), nameof(JojaCDMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(BundleMenuPatches), nameof(BundleMenuPatches.JojaCDMenuPatch))
            );
            #endregion

            #region Menu Patches
            harmony.Patch(
                    original: AccessTools.Method(typeof(LetterViewerMenu), nameof(LetterViewerMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(DialoguePatches), nameof(DialoguePatches.LetterViewerMenuPatch))
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
                original: AccessTools.Method(typeof(TitleTextInputMenu), nameof(TitleTextInputMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.TitleTextInputMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(NamingMenu), nameof(NamingMenu.draw), new Type[] { typeof(SpriteBatch) }),
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

            harmony.Patch(
                original: AccessTools.Method(typeof(MuseumMenu), nameof(MuseumMenu.receiveKeyPress), new Type[] { typeof(Keys) }),
                prefix: new HarmonyMethod(typeof(DonationMenuPatches), nameof(DonationMenuPatches.MuseumMenuKeyPressPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ChooseFromListMenu), nameof(ChooseFromListMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.ChooseFromListMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(TailoringMenu), nameof(TailoringMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.TailoringMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(PondQueryMenu), nameof(PondQueryMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.PondQueryMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.ForgeMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ItemListMenu), nameof(ItemListMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.ItemListMenuPatch))
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
                    postfix: new HarmonyMethod(typeof(ChatMenuPatches), nameof(ChatMenuPatches.ChatBoxPatch))
                );
            #endregion

            #region On Menu CLose Patch
            harmony.Patch(
                    original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.exitThisMenu)),
                    postfix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(IClickableMenuPatch.ExitThisMenuPatch))
                );
            harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), nameof(Game1.exitActiveMenu)),
                    prefix: new HarmonyMethod(typeof(MenuPatches), nameof(MenuPatches.Game1ExitActiveMenuPatch))
                );
            #endregion

            #region Animal and Building Menu

            harmony.Patch(
                    original: AccessTools.Method(typeof(CarpenterMenu), nameof(CarpenterMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    prefix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(CarpenterMenuPatch.DrawPatch))
                );

            harmony.Patch(
                    original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    prefix: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(PurchaseAnimalsMenuPatch.DrawPatch))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(AnimalQueryMenuPatch.DrawPatch))
            );

            #endregion

            #region Donation Menus
            harmony.Patch(
                original: AccessTools.Method(typeof(MuseumMenu), nameof(MuseumMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(DonationMenuPatches), nameof(DonationMenuPatches.MuseumMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FieldOfficeMenu), nameof(FieldOfficeMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(DonationMenuPatches), nameof(DonationMenuPatches.FieldOfficeMenuPatch))
            );
            #endregion

            #region Mini Games
            harmony.Patch(
                        original: AccessTools.Method(typeof(Intro), nameof(Intro.draw), new Type[] { typeof(SpriteBatch) }),
                        postfix: new HarmonyMethod(typeof(MiniGamesPatches), nameof(MiniGamesPatches.IntroPatch))
                    );

            harmony.Patch(
                        original: AccessTools.Method(typeof(GrandpaStory), nameof(GrandpaStory.draw), new Type[] { typeof(SpriteBatch) }),
                        postfix: new HarmonyMethod(typeof(MiniGamesPatches), nameof(MiniGamesPatches.GrandpaStoryPatch))
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

            harmony.Patch(
                    original: AccessTools.Method(typeof(TextBox), nameof(TextBox.Draw)),
                    prefix: new HarmonyMethod(typeof(TextBoxPatch), nameof(TextBoxPatch.DrawPatch))
                );
        }
    }
}
