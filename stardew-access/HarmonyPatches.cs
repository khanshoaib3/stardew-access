using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using stardew_access.Patches;
using StardewValley;
using StardewValley.Characters;
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
                   postfix: new HarmonyMethod(typeof(DialogueBoxPatch), nameof(DialogueBoxPatch.DrawPatch))
                );

            harmony.Patch(
               original: AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.receiveLeftClick)),
               postfix: new HarmonyMethod(typeof(DialogueBoxPatch), nameof(DialogueBoxPatch.RecieveLeftClickPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.drawHoverText), new Type[] { typeof(SpriteBatch), typeof(string), typeof(SpriteFont), typeof(int), typeof(int), typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe), typeof(IList<Item>) }),
                postfix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(IClickableMenuPatch.DrawHoverTextPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.drawAboveAlwaysFrontLayer)),
                postfix: new HarmonyMethod(typeof(NPCPatch), nameof(NPCPatch.DrawAboveAlwaysFrontLayerPatch))
            );
            #endregion

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

            #region Game Menu Patches
            harmony.Patch(
                original: AccessTools.Method(typeof(GameMenu), nameof(GameMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatch), nameof(GameMenuPatch.DrawPatch))
            );

            harmony.Patch(
                    original: AccessTools.Method(typeof(OptionsPage), nameof(OptionsPage.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(OptionsPagePatch), nameof(OptionsPagePatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ExitPage), nameof(ExitPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ExitPagePatch), nameof(ExitPagePatch.DrawPatch))
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
                postfix: new HarmonyMethod(typeof(JunimoNoteMenuPatch), nameof(JunimoNoteMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(JojaCDMenu), nameof(JojaCDMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(JojaCDMenuPatch), nameof(JojaCDMenuPatch.DrawPatch))
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
                original: AccessTools.Method(typeof(TailoringMenu), nameof(TailoringMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(TailoringMenuPatch), nameof(TailoringMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(PondQueryMenu), nameof(PondQueryMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(PondQueryMenuPatch), nameof(PondQueryMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ForgeMenuPatch), nameof(ForgeMenuPatch.DrawPatch))
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

            harmony.Patch(
                original: AccessTools.Method(typeof(Billboard), nameof(Billboard.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(BillboardPatch), nameof(BillboardPatch.DrawPatch))
            );
            #endregion

            #region Chat Menu Patches
            harmony.Patch(
                    original: AccessTools.Method(typeof(ChatBox), nameof(ChatBox.update), new Type[] { typeof(GameTime) }),
                    postfix: new HarmonyMethod(typeof(ChatBoxPatch), nameof(ChatBoxPatch.UpdatePatch))
                );
            #endregion

            #region On Menu CLose Patch
            harmony.Patch(
                    original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.exitThisMenu)),
                    postfix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(IClickableMenuPatch.ExitThisMenuPatch))
                );

            harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), nameof(Game1.exitActiveMenu)),
                    prefix: new HarmonyMethod(typeof(Game1Patch), nameof(Game1Patch.ExitActiveMenuPatch))
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
                postfix: new HarmonyMethod(typeof(MuseumMenuPatch), nameof(MuseumMenuPatch.DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(MuseumMenu), nameof(MuseumMenu.receiveKeyPress), new Type[] { typeof(Keys) }),
                prefix: new HarmonyMethod(typeof(MuseumMenuPatch), nameof(MuseumMenuPatch.RecieveKeyPressPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FieldOfficeMenu), nameof(FieldOfficeMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(FieldOfficeMenuPatch), nameof(FieldOfficeMenuPatch.DrawPatch))
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

            harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), nameof(Game1.playSound)),
                    prefix: new HarmonyMethod(typeof(Game1Patch), nameof(Game1Patch.PlaySoundPatch))
                );

            harmony.Patch(
                    original: AccessTools.Method(typeof(InstanceGame), nameof(InstanceGame.Exit)),
                    prefix: new HarmonyMethod(typeof(InstanceGamePatch), nameof(InstanceGamePatch.ExitPatch))
                );

            harmony.Patch(
                    original: AccessTools.Method(typeof(TextBox), nameof(TextBox.Draw)),
                    prefix: new HarmonyMethod(typeof(TextBoxPatch), nameof(TextBoxPatch.DrawPatch))
                );

            harmony.Patch(
                    original: AccessTools.Method(typeof(TextEntryMenu), nameof(TextEntryMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    prefix: new HarmonyMethod(typeof(TextEntryMenuPatch), nameof(TextEntryMenuPatch.DrawPatch))
                );

            harmony.Patch(
                    original: AccessTools.Method(typeof(TextEntryMenu), nameof(TextEntryMenu.Close)),
                    prefix: new HarmonyMethod(typeof(TextEntryMenuPatch), nameof(TextEntryMenuPatch.ClosePatch))
                );

            harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), nameof(Game1.closeTextEntry)),
                    prefix: new HarmonyMethod(typeof(Game1Patch), nameof(Game1Patch.CloseTextEntryPatch))
                );

            harmony.Patch(
                    original: AccessTools.Method(typeof(TrashBear), nameof(TrashBear.checkAction)),
                    postfix: new HarmonyMethod(typeof(TrashBearPatch), nameof(TrashBearPatch.CheckActionPatch))
                );
        }
    }
}
