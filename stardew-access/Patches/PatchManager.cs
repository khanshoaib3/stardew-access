using HarmonyLib;

namespace stardew_access.Patches
{
    internal class PatchManager
    {
        public static void PatchAll(Harmony harmony)
        {
            List<IPatch> allPatches =
            [
                // Bundle Menu Patches
                new JojaCDMenuPatch(),
                new JunimoNoteMenuPatch(),
                // Donation Menu Patches
                new FieldOfficeMenuPatch(),
                new MuseumMenuPatch(),
                // Game Menu Patches
                new CollectionsPagePatch(),
                new CraftingPagePatch(),
                new ExitPagePatch(),
                new GameMenuPatch(),
                new InventoryPagePatch(),
                new OptionsPagePatch(),
                new SocialPagePatch(),
                // Menus With Inventory
                new ForgeMenuPatch(),
                new GeodeMenuPatch(),
                new ItemGrabMenuPatch(),
                new QuestContainerMenuPatch(),
                new ShopMenuPatch(),
                new TailoringMenuPatch(),
                // Misc Patches
                new ChatBoxPatch(),
                new DialogueBoxPatch(),
                new Game1Patch(),
                new GameLocationPatch(),
                new IClickableMenuPatch(),
                new InstanceGamePatch(),
                new NPCPatch(),
                new TextBoxPatch(),
                new TextEntryMenuPatch(),
                new TileMapPatch(),
                new TrashBearPatch(),
                // Other Menu Patches
                new AnimalQueryMenuPatch(),
                new CarpenterMenuPatch(),
                new ChooseFromListMenuPatch(),
                new ConfirmationDialogMenuPatch(),
                new ItemListMenuPatch(),
                new LanguageSelectionMenuPatch(),
                new LetterViewerMenuPatch(),
                new LevelUpMenuPatch(),
                new MineElevatorMenuPatch(),
                new NamingMenuPatch(),
                new NumberSelectionMenuPatch(),
                new PondQueryMenuPatch(),
                new PurchaseAnimalsMenuPatch(),
                new ShippingMenuPatch(),
                new TitleTextInputMenuPatch(),
                // Quest Patches
                new BillboardPatch(),
                new QuestLogPatch(),
                new SpecialOrdersBoardPatch(),
                // Title Menu Patches
                new AdvancedGameOptionsPatch(),
                new CharacterCustomizationMenuPatch(),
                new CoopMenuPatch(),
                new LoadGameMenuPatch(),
                new TitleMenuPatch(),
                new FarmHandMenuPatch(),
                // Mini Game Patches
                new FishingMiniGamePatch(),
                new GrandpaStoryPatch(),
                new IntroPatch(),
            ];

            foreach (IPatch patch in allPatches)
            {
                try
                {
                    patch.Apply(harmony);
                }
                catch (Exception e)
                {
                    Log.Error($"An error occurred while applying {patch.GetType().FullName} patch:\n{e.Message}\n{e.StackTrace}");
                }
            }
        }
    }
}
