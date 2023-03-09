using StardewValley;
using stardew_access.Features;
using StardewValley.Menus;
using StardewValley.Objects;

namespace stardew_access.Patches
{
    internal class CraftingPagePatch
    {
        internal static string hoveredItemQueryKey = "";
        internal static string craftingPageQueryKey = "";
        internal static int currentSelectedCraftingRecipe = -1;
        internal static bool isSelectingRecipe = false;

        internal static void DrawPatch(CraftingPage __instance, CraftingRecipe ___hoverRecipe, int ___currentCraftingPage)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                handleKeyBinds(__instance, ___currentCraftingPage);

                if (narrateMenuButtons(__instance, x, y))
                {
                    return;
                }

                if (narrateHoveredRecipe(__instance, ___currentCraftingPage, ___hoverRecipe, x, y))
                {
                    return;
                }

                if (InventoryUtils.narrateHoveredSlot(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y))
                {
                    craftingPageQueryKey = "";
                    return;
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void handleKeyBinds(CraftingPage __instance, int ___currentCraftingPage)
        {
            if (MainClass.Config.SnapToFirstSecondaryInventorySlotKey.JustPressed() && __instance.pagesOfCraftingRecipes[___currentCraftingPage].Count > 0)
            {
                // snap to first crafting recipe
                __instance.setCurrentlySnappedComponentTo(__instance.pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(0).Key.myID);
                __instance.pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(0).Key.snapMouseCursorToCenter();
                currentSelectedCraftingRecipe = 0;
            }
            else if (MainClass.Config.SnapToFirstInventorySlotKey.JustPressed() && __instance.inventory.inventory.Count > 0)
            {
                // snap to first inventory slot
                __instance.setCurrentlySnappedComponentTo(__instance.inventory.inventory[0].myID);
                __instance.inventory.inventory[0].snapMouseCursorToCenter();
                currentSelectedCraftingRecipe = -1;
            }
            else if (MainClass.Config.CraftingMenuCycleThroughRecipiesKey.JustPressed() && !isSelectingRecipe)
            {
                isSelectingRecipe = true;
                CycleThroughRecipies(__instance.pagesOfCraftingRecipes, ___currentCraftingPage, __instance);
                Task.Delay(200).ContinueWith(_ => { isSelectingRecipe = false; });
            }
        }

        private static bool narrateMenuButtons(CraftingPage __instance, int x, int y)
        {
            string? toSpeak = null;
            bool isDropItemButton = false;

            if (__instance.upButton != null && __instance.upButton.containsPoint(x, y))
            {
                toSpeak = "Previous Recipe List";
            }
            else if (__instance.downButton != null && __instance.downButton.containsPoint(x, y))
            {
                toSpeak = "Next Recipe List";
            }
            else if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
            {
                toSpeak = "Trash Can";
            }
            else if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
            {
                toSpeak = "Drop Item";
                isDropItemButton = true;
            }
            else
            {
                return false;
            }

            if (toSpeak != null && craftingPageQueryKey != toSpeak)
            {
                craftingPageQueryKey = toSpeak;
                hoveredItemQueryKey = "";
                MainClass.ScreenReader.Say(toSpeak, true);
                if (isDropItemButton) Game1.playSound("drop_item");
            }

            return true;
        }

        private static bool narrateHoveredRecipe(CraftingPage __instance, int ___currentCraftingPage, CraftingRecipe ___hoverRecipe, int x, int y)
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

                string query = $"unknown recipe:{__instance.getCurrentlySnappedComponent().myID}";

                if (craftingPageQueryKey != query)
                {
                    craftingPageQueryKey = query;
                    hoveredItemQueryKey = "";
                    MainClass.ScreenReader.Say("unknown recipe", true);
                }
                return true;
            }

            string name = ___hoverRecipe.DisplayName;
            int numberOfProduce = ___hoverRecipe.numberProducedPerCraft;
            string description = "";
            string ingredients = "";
            string buffs = "";
            string craftable = "";

            description = $"Description:\n{___hoverRecipe.description}";
            craftable = ___hoverRecipe.doesFarmerHaveIngredientsInInventory(getContainerContents(__instance._materialContainers)) ? "Craftable" : "Not Craftable";

            #region Crafting ingredients
            ingredients = "Ingredients:\n";
            for (int i = 0; i < ___hoverRecipe.recipeList.Count; i++)
            {
                int recipeCount = ___hoverRecipe.recipeList.ElementAt(i).Value;
                int recipeItem = ___hoverRecipe.recipeList.ElementAt(i).Key;
                string recipeName = ___hoverRecipe.getNameFromIndex(recipeItem);

                ingredients += $" ,{recipeCount} {recipeName}";
            }
            #endregion

            #region Health & stamina and buff items (effects like +1 walking speed)
            Item producesItem = ___hoverRecipe.createItem();
            if (producesItem is StardewValley.Object producesItemObject)
            {
                if (producesItemObject.Edibility != -300)
                {
                    int stamina_recovery = producesItemObject.staminaRecoveredOnConsumption();
                    buffs += $"{stamina_recovery} Energy";
                    if (stamina_recovery >= 0)
                    {
                        int health_recovery = producesItemObject.healthRecoveredOnConsumption();
                        buffs += $"\n{health_recovery} Health";
                    }
                }
                // These variables are taken from the game's code itself (IClickableMenu.cs -> 1016 line)
                bool edibleItem = producesItem != null && (int)producesItemObject.Edibility != -300;
                string[]? buffIconsToDisplay = (producesItem != null && edibleItem && Game1.objectInformation[producesItemObject.ParentSheetIndex].Split('/').Length > 7)
                    ? producesItem.ModifyItemBuffs(Game1.objectInformation[producesItemObject.ParentSheetIndex].Split('/')[7].Split(' '))
                    : null;

                if (buffIconsToDisplay != null)
                {
                    for (int j = 0; j < buffIconsToDisplay.Length; j++)
                    {
                        string buffName = ((Convert.ToInt32(buffIconsToDisplay[j]) > 0) ? "+" : "") + buffIconsToDisplay[j] + " ";
                        if (j <= 11)
                        {
                            buffName = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + j, buffName);
                        }
                        try
                        {
                            int count = int.Parse(buffName.Substring(0, buffName.IndexOf(' ')));
                            if (count != 0)
                                buffs += $"{buffName}\n";
                        }
                        catch (Exception) { }
                    }

                    buffs = $"Buffs and boosts:\n {buffs}";
                }
            }
            #endregion


            string toSpeak = $"{numberOfProduce} {name}, {craftable}, \n\t{ingredients}, \n\t{description} \n\t{buffs}";

            if (craftingPageQueryKey != toSpeak)
            {
                craftingPageQueryKey = toSpeak;
                hoveredItemQueryKey = "";
                MainClass.ScreenReader.Say(toSpeak, true);
            }

            return true;
        }

        private static void CycleThroughRecipies(List<Dictionary<ClickableTextureComponent, CraftingRecipe>> pagesOfCraftingRecipes, int ___currentCraftingPage, CraftingPage __instance)
        {
            currentSelectedCraftingRecipe++;
            if (currentSelectedCraftingRecipe < 0 || currentSelectedCraftingRecipe >= pagesOfCraftingRecipes[0].Count)
                currentSelectedCraftingRecipe = 0;

            __instance.setCurrentlySnappedComponentTo(pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(currentSelectedCraftingRecipe).Key.myID);
            pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(currentSelectedCraftingRecipe).Key.snapMouseCursorToCenter();

            // Skip if recipe is not unlocked/unknown
            if (pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(currentSelectedCraftingRecipe).Key.hoverText.Equals("ghosted"))
                CycleThroughRecipies(pagesOfCraftingRecipes, ___currentCraftingPage, __instance);
        }

        // This method is used to get the inventory items to check if the player has enough ingredients for a recipe
        // Taken from CraftingPage.cs -> 169 line
        internal static IList<Item>? getContainerContents(List<Chest> materialContainers)
        {
            if (materialContainers == null)
            {
                return null;
            }
            List<Item> items = new List<Item>();
            for (int i = 0; i < materialContainers.Count; i++)
            {
                items.AddRange(materialContainers[i].items);
            }
            return items;
        }

        internal static void Cleanup()
        {
            hoveredItemQueryKey = "";
            craftingPageQueryKey = "";
            currentSelectedCraftingRecipe = -1;
            isSelectingRecipe = false;
        }
    }
}
