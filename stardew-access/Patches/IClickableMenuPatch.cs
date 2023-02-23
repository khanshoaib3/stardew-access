using StardewValley.Menus;

namespace stardew_access.Patches
{
    // These patches are global, i.e. work on every menus
    internal class IClickableMenuPatch
    {
        internal static void ExitThisMenuPatch(IClickableMenu __instance)
        {
            try
            {
                Cleanup(__instance);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void Cleanup(IClickableMenu menu)
        {
            if (menu is LetterViewerMenu)
            {
                DialoguePatches.currentLetterText = " ";
            }
            else if (menu is LevelUpMenu)
            {
                MenuPatches.currentLevelUpTitle = " ";
            }
            else if (menu is Billboard)
            {
                QuestPatches.currentDailyQuestText = " ";
            }
            else if (menu is GameMenu)
            {
                GameMenuPatches.gameMenuQueryKey = "";
                GameMenuPatches.craftingPageQueryKey = "";
                GameMenuPatches.inventoryPageQueryKey = "";
                GameMenuPatches.exitPageQueryKey = "";
                GameMenuPatches.optionsPageQueryKey = "";
                GameMenuPatches.socialPageQuery = "";
                GameMenuPatches.currentSelectedCraftingRecipe = -1;
                GameMenuPatches.isSelectingRecipe = false;
            }
            else if (menu is JunimoNoteMenu)
            {
                BundleMenuPatches.currentIngredientListItem = -1;
                BundleMenuPatches.currentIngredientInputSlot = -1;
                BundleMenuPatches.currentInventorySlot = -1;
                BundleMenuPatches.junimoNoteMenuQuery = "";
            }
            else if (menu is ShopMenu)
            {
                GameMenuPatches.shopMenuQueryKey = "";
            }
            else if (menu is ItemGrabMenu)
            {
                GameMenuPatches.itemGrabMenuQueryKey = "";
            }
            else if (menu is GeodeMenu)
            {
                GameMenuPatches.geodeMenuQueryKey = "";
            }
            else if (menu is CarpenterMenu)
            {
                CarpenterMenuPatch.carpenterMenuQuery = "";
                CarpenterMenuPatch.isUpgrading = false;
                CarpenterMenuPatch.isDemolishing = false;
                CarpenterMenuPatch.isPainting = false;
                CarpenterMenuPatch.isMoving = false;
                CarpenterMenuPatch.isConstructing = false;
                CarpenterMenuPatch.carpenterMenu = null;
            }
            else if (menu is PurchaseAnimalsMenu)
            {
                PurchaseAnimalsMenuPatch.purchaseAnimalMenuQuery = "";
                PurchaseAnimalsMenuPatch.firstTimeInNamingMenu = true;
                PurchaseAnimalsMenuPatch.purchaseAnimalsMenu = null;
            }
            else if (menu is AnimalQueryMenu)
            {
                AnimalQueryMenuPatch.animalQueryMenuQuery = "";
                AnimalQueryMenuPatch.animalQueryMenu = null;
            }
            else if (menu is DialogueBox)
            {
                DialoguePatches.isDialogueAppearingFirstTime = true;
                DialoguePatches.currentDialogue = " ";
            }
            else if (menu is JojaCDMenu)
            {
                BundleMenuPatches.jojaCDMenuQuery = "";
            }
            else if (menu is QuestLog)
            {
                QuestPatches.questLogQuery = " ";
            }
            else if (menu is TailoringMenu)
            {
                MenuPatches.tailoringMenuQuery = " ";
            }
            else if (menu is ForgeMenu)
            {
                MenuPatches.forgeMenuQuery = " ";
            }
            else if (menu is ItemListMenu)
            {
                MenuPatches.itemListMenuQuery = " ";
            }
            else if (menu is FieldOfficeMenu)
            {
                DonationMenuPatches.fieldOfficeMenuQuery = " ";
            }
            else if (menu is MuseumMenu)
            {
                DonationMenuPatches.museumQueryKey = " ";
            }
            else if (menu is PondQueryMenu)
            {
                MenuPatches.pondQueryMenuQuery = " ";
            }

            InventoryUtils.hoveredItemQueryKey = "";
            InventoryUtils.prevSlotIndex = -999;
            TextBoxPatch.activeTextBoxes = "";
        }
    }
}
