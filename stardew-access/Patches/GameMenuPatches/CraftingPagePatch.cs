using StardewValley;
using stardew_access.Utils;
using StardewValley.Menus;
using HarmonyLib;
using StardewValley.Inventories;

namespace stardew_access.Patches;

internal class CraftingPagePatch : IPatch
{
    private static int _currentSelectedSnapComponent = -1;
    private static int _previouslyActivePage = 0;
    private static bool _isSelectingRecipe = false;

    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(CraftingPage), "draw"),
            postfix: new HarmonyMethod(typeof(CraftingPagePatch), nameof(CraftingPagePatch.DrawPatch))
        );
    }

    internal static void DrawPatch(CraftingPage __instance, CraftingRecipe ___hoverRecipe, int ___currentCraftingPage)
    {
        try
        {
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

            HandleKeyBinds(__instance, ___currentCraftingPage);

            if (NarrateMenuButtons(__instance, x, y))
            {
                return;
            }

            if (NarrateHoveredRecipe(__instance, ___currentCraftingPage, ___hoverRecipe, x, y))
            {
                return;
            }

            InventoryUtils.NarrateHoveredSlot(__instance.inventory);
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in crafting page patch:\n{e.Message}\n{e.StackTrace}");
        }
    }

    private static void HandleKeyBinds(CraftingPage __instance, int ___currentCraftingPage)
    {
        bool isLeftShiftPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);

        if (MainClass.Config.SnapToFirstSecondaryInventorySlotKey.JustPressed() && __instance.pagesOfCraftingRecipes[___currentCraftingPage].Count > 0)
        {
            // snap to first crafting recipe
            __instance.setCurrentlySnappedComponentTo(__instance.pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(0).Key.myID);
            __instance.pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(0).Key.snapMouseCursorToCenter();
            _currentSelectedSnapComponent = 0;
        }
        else if (MainClass.Config.SnapToFirstInventorySlotKey.JustPressed() && __instance.inventory.inventory.Count > 0)
        {
            // snap to first inventory slot
            __instance.setCurrentlySnappedComponentTo(__instance.inventory.inventory[0].myID);
            __instance.inventory.inventory[0].snapMouseCursorToCenter();
            _currentSelectedSnapComponent = -1;
        }
        else if (MainClass.Config.CraftingMenuCycleThroughRecipesKey.JustPressed() && !isLeftShiftPressed && !_isSelectingRecipe)
        {
            _isSelectingRecipe = true;
            CycleThroughRecipes(___currentCraftingPage, __instance, true);
            Task.Delay(200).ContinueWith(_ => { _isSelectingRecipe = false; });
        }
        else if (MainClass.Config.CraftingMenuCycleThroughRecipesKey.JustPressed() && isLeftShiftPressed && !_isSelectingRecipe)
        {
            _isSelectingRecipe = true;
            CycleThroughRecipes(___currentCraftingPage, __instance, false);
            Task.Delay(200).ContinueWith(_ => { _isSelectingRecipe = false; });
        }
    }

    private static bool NarrateMenuButtons(CraftingPage __instance, int x, int y)
    {
        string? translationKey = null;
        bool isDropItemButton = false;

        if (__instance.upButton != null && __instance.upButton.containsPoint(x, y))
        {
            translationKey = "menu-crafting_page-previous_recipe_list_button";
        }
        else if (__instance.downButton != null && __instance.downButton.containsPoint(x, y))
        {
            translationKey = "menu-crafting_page-next_recipe_list_button";
        }
        else if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
        {
            translationKey = "common-ui-trashcan_button";
        }
        else if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
        {
            translationKey = "common-ui-drop_item_button";
            isDropItemButton = true;
        }
        else
        {
            return false;
        }

        if (MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true))
            if (isDropItemButton) Game1.playSound("drop_item");

        return true;
    }

    private static bool NarrateHoveredRecipe(CraftingPage __instance, int ___currentCraftingPage, CraftingRecipe ___hoverRecipe, int x, int y)
    {
        if (___hoverRecipe == null)
        {
            var isRecipeInFocus = false;
            foreach (var item in __instance.pagesOfCraftingRecipes[___currentCraftingPage])
            {
                if (!item.Key.containsPoint(x, y))
                    continue;

                isRecipeInFocus = true;
                break;
            }

            if (!isRecipeInFocus)
                return false;

            MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-crafting_page-unknown_recipe", true);
            return true;
        }

        Item producesItem = ___hoverRecipe.createItem();
        string translationKey = "menu-crafting_page-recipe_info";
        object translationTokens = new
            {
                produce_count = ___hoverRecipe.numberProducedPerCraft,
                name = ___hoverRecipe.DisplayName,
                is_craftable = ___hoverRecipe.doesFarmerHaveIngredientsInInventory(GetContainerContents(__instance._materialContainers)) ? 1 : 0,
                ingredients = InventoryUtils.GetIngredientsFromRecipe(___hoverRecipe),
                ___hoverRecipe.description,
                buffs = $"{InventoryUtils.GetHealthNStaminaFromItem(producesItem)}, {InventoryUtils.GetBuffsFromItem(producesItem)}"
            };

        MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens);

        return true;
    }

    private static void CycleThroughRecipes(int ___currentCraftingPage,
                                            CraftingPage __instance,
                                            bool goForward)
    {
        _currentSelectedSnapComponent += goForward ? 1 : -1;
        if (_currentSelectedSnapComponent < -1)
        {
            _currentSelectedSnapComponent = __instance.pagesOfCraftingRecipes[___currentCraftingPage].Count;
        }
        else if (_currentSelectedSnapComponent > __instance.pagesOfCraftingRecipes[___currentCraftingPage].Count)
        {
            _currentSelectedSnapComponent = -1;
        }

        if (_previouslyActivePage != ___currentCraftingPage)
        {
            _previouslyActivePage = ___currentCraftingPage;
            _currentSelectedSnapComponent = 0;
        }

        // Snaps to the previous recipe list button if visible
        if (_currentSelectedSnapComponent == -1)
        {
            if (__instance.upButton != null && ___currentCraftingPage > 0)
            {
                __instance.setCurrentlySnappedComponentTo(__instance.upButton.myID);
                __instance.upButton.snapMouseCursorToCenter();
            }
            else
            {
                CycleThroughRecipes(___currentCraftingPage, __instance, goForward);
            }
            return;
        }

        // Snaps to the next recipe list button if visible
        if (_currentSelectedSnapComponent == __instance.pagesOfCraftingRecipes[___currentCraftingPage].Count)
        {
            if (__instance.downButton != null && ___currentCraftingPage < __instance.pagesOfCraftingRecipes.Count - 1)
            {
                __instance.setCurrentlySnappedComponentTo(__instance.downButton.myID);
                __instance.downButton.snapMouseCursorToCenter();
            }
            else
            {
                CycleThroughRecipes(___currentCraftingPage, __instance, goForward);
            }
            return;
        }

        __instance.setCurrentlySnappedComponentTo(__instance.pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(_currentSelectedSnapComponent).Key.myID);
        __instance.pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(_currentSelectedSnapComponent).Key.snapMouseCursorToCenter();

        // Skip if recipe is not unlocked/unknown
        if (__instance.pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(_currentSelectedSnapComponent).Key.hoverText.Equals("ghosted"))
            CycleThroughRecipes(___currentCraftingPage, __instance, goForward);
    }

    // This method is used to get the inventory items to check if the player has enough ingredients for a recipe
    // Taken from CraftingPage.cs -> 169 line
    internal static IList<Item>? GetContainerContents(List<IInventory> materialContainers)
    {
        if (materialContainers == null)
        {
            return null;
        }
        List<Item> list = [];
	    foreach (IInventory materialContainer in materialContainers)
	    {
	        list.AddRange(materialContainer);
        }
        return list;
    }

    internal static void Cleanup()
    {
        _currentSelectedSnapComponent = -1;
        _previouslyActivePage = 0;
        _isSelectingRecipe = false;
    }
}
