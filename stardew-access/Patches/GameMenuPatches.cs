
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace stardew_access.Patches
{
    internal class GameMenuPatches
    {
        internal static string hoveredItemQueryKey = "";
        internal static string geodeMenuQueryKey = "";
        internal static string gameMenuQueryKey = "";
        internal static string itemGrabMenuQueryKey = "";
        internal static string craftingPageQueryKey = "";
        internal static string inventoryPageQueryKey = "";
        internal static string exitPageQueryKey = "";
        internal static string optionsPageQueryKey = "";
        internal static string shopMenuQueryKey = "";
        internal static int currentSelectedCraftingRecipe = -1;
        internal static bool isSelectingRecipe = false;

        internal static void ShopMenuPatch(ShopMenu __instance)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y; // Mouse x and y position
                bool isIPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.I);
                bool isLeftShiftPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);

                if (isLeftShiftPressed && isIPressed && __instance.inventory.inventory.Count>0)
                {
                    __instance.inventory.inventory[0].snapMouseCursorToCenter();
                    __instance.setCurrentlySnappedComponentTo(__instance.inventory.inventory[0].myID);
                }
                else if (!isLeftShiftPressed && isIPressed && __instance.forSaleButtons.Count>0)
                {
                    __instance.forSaleButtons[0].snapMouseCursorToCenter();
                    __instance.setCurrentlySnappedComponentTo(__instance.forSaleButtons[0].myID);
                }

                #region Narrate buttons in the menu
                if (__instance.inventory.dropItemInvisibleButton != null && __instance.inventory.dropItemInvisibleButton.containsPoint(x, y))
                {
                    string toSpeak = "Drop Item";
                    if (shopMenuQueryKey != toSpeak)
                    {
                        shopMenuQueryKey = toSpeak;
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                        Game1.playSound("drop_item");
                    }
                    return;
                }
                if (__instance.upArrow != null && __instance.upArrow.containsPoint(x, y))
                {
                    string toSpeak = "Up Arrow Button";
                    if (shopMenuQueryKey != toSpeak)
                    {
                        shopMenuQueryKey = toSpeak;
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }
                if (__instance.downArrow != null && __instance.downArrow.containsPoint(x, y))
                {
                    string toSpeak = "Down Arrow Button";
                    if (shopMenuQueryKey != toSpeak)
                    {
                        shopMenuQueryKey = toSpeak;
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }
                #endregion

                #region Narrate hovered item
                if (narrateHoveredItemInInventory(__instance.inventory.inventory, __instance.inventory.actualInventory, x, y, hoverPrice: __instance.hoverPrice))
                {
                    shopMenuQueryKey = "";
                    return;
                }
                #endregion

                #region Narrate hovered selling item
                if (__instance.hoveredItem != null)
                {
                    string name = __instance.hoveredItem.DisplayName;
                    string price = $"Buy Price: {__instance.hoverPrice} g";
                    string description = __instance.hoveredItem.getDescription();
                    string requirements = "";

                    #region Narrate required items for item
                    int itemIndex = -1, itemAmount = 5;

                    if (__instance.itemPriceAndStock[__instance.hoveredItem].Length > 2)
                        itemIndex = __instance.itemPriceAndStock[__instance.hoveredItem][2];

                    if (__instance.itemPriceAndStock[__instance.hoveredItem].Length > 3)
                        itemAmount = __instance.itemPriceAndStock[__instance.hoveredItem][3];

                    if (itemIndex != -1)
                    {
                        string itemName = Game1.objectInformation[itemIndex].Split('/')[0];

                        if (itemAmount != -1)
                            requirements = $"Required: {itemAmount} {itemName}";
                        else
                            requirements = $"Required: {itemName}";
                    } 
                    #endregion

                    string toSpeak = $"{name}, {requirements}, {price}, \n\t{description}";
                    if (shopMenuQueryKey != toSpeak)
                    {
                        shopMenuQueryKey = toSpeak;
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void GameMenuPatch(GameMenu __instance)
        {
            try
            {
                // Continue if only in the Inventory Page or Crafting Page
                if (__instance.currentTab != 0 && __instance.currentTab != 4 && __instance.currentTab != 6 && __instance.currentTab != 7)
                    return;

                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y; // Mouse x and y position

                for(int i=0; i<__instance.tabs.Count; i++)
                {
                    if(__instance.tabs[i].containsPoint(x, y))
                    {
                        string toSpeak = $"{GameMenu.getLabelOfTabFromIndex(i)} Tab";
                        if (gameMenuQueryKey != toSpeak)
                        {
                            gameMenuQueryKey = toSpeak;
                            MainClass.screenReader.Say(toSpeak, true);
                        }
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void GeodeMenuPatch(GeodeMenu __instance)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y; // Mouse x and y position

                #region Narrate the treasure recieved on breaking the geode
                if (__instance.geodeTreasure != null)
                {
                    string name = __instance.geodeTreasure.DisplayName;
                    int stack = __instance.geodeTreasure.Stack;

                    string toSpeak = $"Recieved {stack} {name}";

                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                } 
                #endregion

                #region Narrate hovered buttons in the menu
                if (__instance.geodeSpot != null && __instance.geodeSpot.containsPoint(x, y))
                {
                    string toSpeak = "Place geode here";
                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
                {
                    string toSpeak = "Drop item here";

                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        MainClass.screenReader.Say(toSpeak, true);
                        Game1.playSound("drop_item");
                    }
                    return;
                }

                if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
                {
                    string toSpeak = "Trash can";

                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    string toSpeak = "Ok button";

                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }
                #endregion

                #region Narrate hovered item
                if(narrateHoveredItemInInventory(__instance.inventory.inventory, __instance.inventory.actualInventory, x, y))
                    geodeMenuQueryKey = "";
                #endregion
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void ItemGrabMenuPatch(ItemGrabMenu __instance)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y; // Mouse x and y position
                bool isIPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.I);
                bool isLeftShiftPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);

                if(isLeftShiftPressed && isIPressed && __instance.inventory.inventory.Count > 0)
                {
                    __instance.setCurrentlySnappedComponentTo(__instance.inventory.inventory[0].myID);
                    __instance.inventory.inventory[0].snapMouseCursorToCenter();
                }
                else if(!isLeftShiftPressed && isIPressed && __instance.ItemsToGrabMenu.inventory.Count > 0 && !__instance.shippingBin)
                {
                    __instance.setCurrentlySnappedComponentTo(__instance.ItemsToGrabMenu.inventory[0].myID);
                    __instance.ItemsToGrabMenu.inventory[0].snapMouseCursorToCenter();
                }

                #region Narrate buttons in the menu
                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    string toSpeak = "Ok Button";
                    if(itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        hoveredItemQueryKey = "";
                        gameMenuQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }
                if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
                {
                    string toSpeak = "Trash Can";
                    if (itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.organizeButton != null && __instance.organizeButton.containsPoint(x, y))
                {
                    string toSpeak = "Organize Button";
                    if (itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.fillStacksButton != null && __instance.fillStacksButton.containsPoint(x, y))
                {
                    string toSpeak = "Add to existing stacks button";
                    if (itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.specialButton != null && __instance.specialButton.containsPoint(x, y))
                {
                    string toSpeak = "Special Button";
                    if (itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.colorPickerToggleButton != null && __instance.colorPickerToggleButton.containsPoint(x, y))
                {

                    string toSpeak = "Color Picker: " + (__instance.chestColorPicker.visible?"Enabled":"Disabled");
                    if (itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.junimoNoteIcon != null && __instance.junimoNoteIcon.containsPoint(x, y))
                {

                    string toSpeak = "Community Center Button";
                    if (itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
                {
                    string toSpeak = "Drop Item";
                    if (itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                        Game1.playSound("drop_item");
                    }
                    return;
                }

                // FIXME
                /*if (__instance.discreteColorPickerCC.Count > 0) {
                    for (int i = 0; i < __instance.discreteColorPickerCC.Count; i++)
                    {
                        if (__instance.discreteColorPickerCC[i].containsPoint(x, y))
                        {
                            MainClass.monitor.Log(i.ToString(), LogLevel.Debug);
                            string toSpeak = getChestColorName(i);
                            if (itemGrabMenuQueryKey != toSpeak)
                            {
                                itemGrabMenuQueryKey = toSpeak;
                                gameMenuQueryKey = "";
                                hoveredItemQueryKey = "";
                                ScreenReader.say(toSpeak, true);
                                Game1.playSound("sa_drop_item");
                            }
                            return;
                        }
                    }
                }*/
                #endregion

                #region Narrate the last shipped item if in the shipping bin
                if (__instance.shippingBin && Game1.getFarm().lastItemShipped != null && __instance.lastShippedHolder.containsPoint(x, y))
                {
                    Item lastShippedItem = Game1.getFarm().lastItemShipped;
                    string name = lastShippedItem.DisplayName;
                    int count = lastShippedItem.Stack;

                    string toSpeak = $"Last Shipped: {count} {name}";

                    if (itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                } 
                #endregion

                #region Narrate hovered item
                if(narrateHoveredItemInInventory(__instance.inventory.inventory, __instance.inventory.actualInventory, x, y, true))
                {
                    gameMenuQueryKey = "";
                    itemGrabMenuQueryKey = "";
                    return;
                }

                if (narrateHoveredItemInInventory(__instance.ItemsToGrabMenu.inventory, __instance.ItemsToGrabMenu.actualInventory, x, y, true))
                {
                    gameMenuQueryKey = "";
                    itemGrabMenuQueryKey = "";
                    return;
                }

                #endregion
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        // TODO Add color names
        private static string getChestColorName(int i)
        {
            string toReturn = "";
            switch (i)
            {
                case 0:
                    toReturn = "Default chest color";
                    break;
                case 1:
                    toReturn = "Default chest color";
                    break;
                case 2:
                    toReturn = "Default chest color";
                    break;
                case 3:
                    toReturn = "Default chest color";
                    break;
                case 4:
                    toReturn = "Default chest color";
                    break;
                case 5:
                    toReturn = "Default chest color";
                    break;
                case 6:
                    toReturn = "Default chest color";
                    break;
                case 7:
                    toReturn = "Default chest color";
                    break;
                case 8:
                    toReturn = "Default chest color";
                    break;
                case 9:
                    toReturn = "Default chest color";
                    break;
                case 10:
                    toReturn = "Default chest color";
                    break;
                case 11:
                    toReturn = "Default chest color";
                    break;
                case 12:
                    toReturn = "Default chest color";
                    break;
                case 13:
                    toReturn = "Default chest color";
                    break;
                case 14:
                    toReturn = "Default chest color";
                    break;
                case 15:
                    toReturn = "Default chest color";
                    break;
                case 16:
                    toReturn = "Default chest color";
                    break;
                case 17:
                    toReturn = "Default chest color";
                    break;
                case 18:
                    toReturn = "Default chest color";
                    break;
                case 19:
                    toReturn = "Default chest color";
                    break;
                case 20:
                    toReturn = "Default chest color";
                    break;
            }
            return toReturn;
        }

        internal static void CraftingPagePatch(CraftingPage __instance, CraftingRecipe ___hoverRecipe, int ___currentCraftingPage)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y; // Mouse x and y position
                bool isIPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.I);
                bool isCPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.C);
                bool isLeftShiftPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);

                if (isLeftShiftPressed && isIPressed && __instance.inventory.inventory.Count > 0)
                {
                    // snap to first inventory slot
                    __instance.setCurrentlySnappedComponentTo(__instance.inventory.inventory[0].myID);
                    __instance.inventory.inventory[0].snapMouseCursorToCenter();
                    currentSelectedCraftingRecipe = -1;
                }
                else if (!isLeftShiftPressed && isIPressed && __instance.pagesOfCraftingRecipes[___currentCraftingPage].Count>0)
                {
                    // snap to first crafting recipe
                    __instance.setCurrentlySnappedComponentTo(__instance.pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(0).Key.myID);
                    __instance.pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(0).Key.snapMouseCursorToCenter();
                    currentSelectedCraftingRecipe = 0;
                } 
                else if (isCPressed && !isSelectingRecipe)
                {
                    _ = CycleThroughRecipies(__instance.pagesOfCraftingRecipes, ___currentCraftingPage, __instance);
                }

                #region Narrate buttons in the menu
                if (__instance.upButton != null && __instance.upButton.containsPoint(x, y))
                {
                    string toSpeak = "Previous Recipe List";
                    if (craftingPageQueryKey != toSpeak)
                    {
                        craftingPageQueryKey = toSpeak;
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.downButton != null && __instance.downButton.containsPoint(x, y))
                {
                    string toSpeak = "Next Recipe List";
                    if (craftingPageQueryKey != toSpeak)
                    {
                        craftingPageQueryKey = toSpeak;
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.trashCan.containsPoint(x, y))
                {
                    string toSpeak = "Trash Can";
                    if (craftingPageQueryKey != toSpeak)
                    {
                        craftingPageQueryKey = toSpeak;
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.dropItemInvisibleButton.containsPoint(x, y))
                {
                    string toSpeak = "Drop Item";
                    if (craftingPageQueryKey != toSpeak)
                    {
                        craftingPageQueryKey = toSpeak;
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                        Game1.playSound("drop_item");
                    }
                    return;
                }
                #endregion

                #region Narrate hovered recipe
                if (___hoverRecipe != null)
                {
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
                    if (producesItem is StardewValley.Object && (producesItem as StardewValley.Object).Edibility != -300)
                    {
                        int stamina_recovery = (producesItem as StardewValley.Object).staminaRecoveredOnConsumption();
                        buffs += $"{stamina_recovery} Energy";
                        if (stamina_recovery >= 0)
                        {
                            int health_recovery = (producesItem as StardewValley.Object).healthRecoveredOnConsumption();
                            buffs += $"\n{health_recovery} Health";
                        }
                    }
                    // These variables are taken from the game's code itself (IClickableMenu.cs -> 1016 line)
                    bool edibleItem = producesItem != null && producesItem is StardewValley.Object && (int)(producesItem as StardewValley.Object).edibility != -300;
                    string[] buffIconsToDisplay = (edibleItem && Game1.objectInformation[(producesItem as StardewValley.Object).parentSheetIndex].Split('/').Length > 7) ? producesItem.ModifyItemBuffs(Game1.objectInformation[(producesItem as StardewValley.Object).parentSheetIndex].Split('/')[7].Split(' ')) : null;
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
                    #endregion


                    string toSpeak = $"{numberOfProduce} {name}, {craftable}, \n\t{ingredients}, \n\t{description} \n\t{buffs}";

                    if (craftingPageQueryKey != toSpeak)
                    {
                        craftingPageQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }
                #endregion

                #region Narrate hovered item
                if (narrateHoveredItemInInventory(__instance.inventory.inventory, __instance.inventory.actualInventory, x, y))
                {
                    gameMenuQueryKey = "";
                    craftingPageQueryKey = "";
                    return;
                }
                #endregion
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        private static async Task CycleThroughRecipies(List<Dictionary<ClickableTextureComponent, CraftingRecipe>> pagesOfCraftingRecipes, int ___currentCraftingPage, CraftingPage __instance)
        {
            isSelectingRecipe = true;

            currentSelectedCraftingRecipe++;
            if (currentSelectedCraftingRecipe < 0 || currentSelectedCraftingRecipe >= pagesOfCraftingRecipes[0].Count)
                currentSelectedCraftingRecipe = 0;

            __instance.setCurrentlySnappedComponentTo(pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(currentSelectedCraftingRecipe).Key.myID);
            pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(currentSelectedCraftingRecipe).Key.snapMouseCursorToCenter();

            await Task.Delay(200);
            isSelectingRecipe = false;
        }

        // This method is used to get the inventory items to check if the player has enough ingredients for a recipe
        // Taken from CraftingPage.cs -> 169 line
        internal static IList<Item> getContainerContents(List<Chest> materialContainers)
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

        internal static void InventoryPagePatch(InventoryPage __instance)
        {
            try
            {
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y; // Mouse x and y position

                #region Narrate buttons in the menu
                if (__instance.inventory.dropItemInvisibleButton != null && __instance.inventory.dropItemInvisibleButton.containsPoint(x, y))
                {
                    string toSpeak = "Drop Item";
                    if (inventoryPageQueryKey != toSpeak)
                    {
                        inventoryPageQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                        Game1.playSound("drop_item");
                    }
                    return;
                }

                if (__instance.organizeButton != null && __instance.organizeButton.containsPoint(x, y))
                {
                    string toSpeak = "Organize Inventory Button";
                    if (inventoryPageQueryKey != toSpeak)
                    {
                        inventoryPageQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
                {
                    string toSpeak = "Trash Can";
                    if (inventoryPageQueryKey != toSpeak)
                    {
                        inventoryPageQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.organizeButton != null && __instance.organizeButton.containsPoint(x, y))
                {
                    string toSpeak = "Organize Button";
                    if (itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.junimoNoteIcon != null && __instance.junimoNoteIcon.containsPoint(x, y))
                {

                    string toSpeak = "Community Center Button";
                    if (itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }
                #endregion

                #region Narrate equipment slots
                for (int i=0; i<__instance.equipmentIcons.Count; i++)
                {
                    if (__instance.equipmentIcons[i].containsPoint(x, y))
                    {
                        string toSpeak = "";

                        #region Get name and description of the item
                        switch (__instance.equipmentIcons[i].name)
                        {
                            case "Hat":
                                {
                                    if (Game1.player.hat.Value != null)
                                    {
                                        toSpeak = $"{Game1.player.hat.Value.DisplayName}, {Game1.player.hat.Value.getDescription()}";
                                    }
                                    else
                                    {
                                        toSpeak = "Hat slot";
                                    }
                                }
                                break;
                            case "Left Ring":
                                {
                                    if (Game1.player.leftRing.Value != null)
                                    {
                                        toSpeak = $"{Game1.player.leftRing.Value.DisplayName}, {Game1.player.leftRing.Value.getDescription()}";
                                    }
                                    else
                                    {
                                        toSpeak = "Left Ring slot";
                                    }
                                }
                                break;
                            case "Right Ring":
                                {
                                    if (Game1.player.rightRing.Value != null)
                                    {
                                        toSpeak = $"{Game1.player.rightRing.Value.DisplayName}, {Game1.player.rightRing.Value.getDescription()}";
                                    }
                                    else
                                    {
                                        toSpeak = "Right ring slot";
                                    }
                                }
                                break;
                            case "Boots":
                                {
                                    if (Game1.player.boots.Value != null)
                                    {
                                        toSpeak = $"{Game1.player.boots.Value.DisplayName}, {Game1.player.boots.Value.getDescription()}";
                                    }
                                    else
                                    {
                                        toSpeak = "Boots slot";
                                    }
                                }
                                break;
                            case "Shirt":
                                {
                                    if (Game1.player.shirtItem.Value != null)
                                    {
                                        toSpeak = $"{Game1.player.shirtItem.Value.DisplayName}, {Game1.player.shirtItem.Value.getDescription()}";
                                    }
                                    else
                                    {
                                        toSpeak = "Shirt slot";
                                    }
                                }
                                break;
                            case "Pants":
                                {
                                    if (Game1.player.pantsItem.Value != null)
                                    {
                                        toSpeak = $"{Game1.player.pantsItem.Value.DisplayName}, {Game1.player.pantsItem.Value.getDescription()}";
                                    }
                                    else
                                    {
                                        toSpeak = "Pants slot";
                                    }
                                }
                                break;
                        } 
                        #endregion

                        if (inventoryPageQueryKey != toSpeak)
                        {
                            inventoryPageQueryKey = toSpeak;
                            gameMenuQueryKey = "";
                            hoveredItemQueryKey = "";
                            MainClass.screenReader.Say(toSpeak, true);
                        }
                        return;
                    }
                }
                #endregion

                #region Narrate hovered item
                if (narrateHoveredItemInInventory(__instance.inventory.inventory, __instance.inventory.actualInventory, x, y, true))
                {
                    gameMenuQueryKey = "";
                    inventoryPageQueryKey = "";
                    return;
                }
                #endregion
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void OptionsPagePatch(OptionsPage __instance)
        {
            try
            {
                int currentItemIndex = Math.Max(0, Math.Min(__instance.options.Count - 7, __instance.currentItemIndex));
                int x = Game1.getMousePosition(true).X, y = Game1.getMousePosition(true).Y;
                for (int i = 0; i < __instance.optionSlots.Count; i++)
                {
                    if (__instance.optionSlots[i].bounds.Contains(x, y) && currentItemIndex + i < __instance.options.Count && __instance.options[currentItemIndex + i].bounds.Contains(x - __instance.optionSlots[i].bounds.X, y - __instance.optionSlots[i].bounds.Y))
                    {
                        OptionsElement optionsElement = __instance.options[currentItemIndex + i];
                        string toSpeak = optionsElement.label;

                        if (optionsElement is OptionsButton)
                            toSpeak = $" {toSpeak} Button";
                        else if (optionsElement is OptionsCheckbox)
                            toSpeak = ((optionsElement as OptionsCheckbox).isChecked ? "Enabled" : "Disabled") + $" {toSpeak} Checkbox";
                        else if (optionsElement is OptionsDropDown)
                            toSpeak = $"{toSpeak} Dropdown, option {(optionsElement as OptionsDropDown).dropDownDisplayOptions[(optionsElement as OptionsDropDown).selectedOption]} selected";
                        else if (optionsElement is OptionsSlider)
                            toSpeak = $"{(optionsElement as OptionsSlider).value}% {toSpeak} Slider";
                        else if (optionsElement is OptionsPlusMinus)
                            toSpeak = $"{(optionsElement as OptionsPlusMinus).displayOptions[(optionsElement as OptionsPlusMinus).selected]} selected of {toSpeak}";
                        else if (optionsElement is OptionsInputListener)
                        {
                            string buttons = "";
                            (optionsElement as OptionsInputListener).buttonNames.ForEach(name => { buttons += $", {name}"; });
                            toSpeak = $"{toSpeak} is bound to {buttons}. Left click to change.";
                        }
                        else
                        {
                            if (toSpeak.Contains(":"))
                                toSpeak = toSpeak.Replace(":", "");

                            toSpeak = $"{toSpeak} Options:";
                        }

                        if (optionsPageQueryKey != toSpeak)
                        {
                            gameMenuQueryKey = "";
                            optionsPageQueryKey = toSpeak;
                            MainClass.screenReader.Say(toSpeak, true);
                        }
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void ExitPagePatch(ExitPage __instance)
        {
            try
            {
                if (__instance.exitToTitle.visible &&
                        __instance.exitToTitle.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    string toSpeak = "Exit to Title Button";
                    if (exitPageQueryKey != toSpeak)
                    {
                        gameMenuQueryKey = "";
                        exitPageQueryKey = toSpeak;
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }
                if (__instance.exitToDesktop.visible &&
                    __instance.exitToDesktop.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    string toSpeak = "Exit to Desktop Button";
                    if (exitPageQueryKey != toSpeak)
                    {
                        gameMenuQueryKey = "";
                        exitPageQueryKey = toSpeak;
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return;
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static bool narrateHoveredItemInInventory(List<ClickableComponent> inventory, IList<Item> actualInventory, int x, int y, bool giveExtraDetails = false, int hoverPrice = -1, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1)
        {
            #region Narrate hovered item
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].containsPoint(x, y))
                {
                    string toSpeak = "";
                    if ((i + 1) <= actualInventory.Count)
                    {
                        if (actualInventory[i] != null)
                        {
                            string name = actualInventory[i].DisplayName;
                            int stack = actualInventory[i].Stack;
                            string quality = "";
                            string healthNStamine = "";
                            string buffs = "";
                            string description = "";
                            string price = "";
                            string requirements = "";

                            #region Add quality of item
                            if (actualInventory[i] is StardewValley.Object && (actualInventory[i] as StardewValley.Object).quality > 0)
                            {
                                int qualityIndex = (actualInventory[i] as StardewValley.Object).quality;
                                if (qualityIndex == 1)
                                {
                                    quality = "Silver quality";
                                }
                                else if (qualityIndex == 2 || qualityIndex == 3)
                                {
                                    quality = "Gold quality";
                                }
                                else if (qualityIndex >= 4)
                                {
                                    quality = "Iridium quality";
                                }
                            }
                            #endregion

                            if (giveExtraDetails)
                            {
                                description = actualInventory[i].getDescription();
                                #region Add health & stamina provided by the item
                                if (actualInventory[i] is StardewValley.Object && (actualInventory[i] as StardewValley.Object).Edibility != -300)
                                {
                                    int stamina_recovery = (actualInventory[i] as StardewValley.Object).staminaRecoveredOnConsumption();
                                    healthNStamine += $"{stamina_recovery} Energy";
                                    if (stamina_recovery >= 0)
                                    {
                                        int health_recovery = (actualInventory[i] as StardewValley.Object).healthRecoveredOnConsumption();
                                        healthNStamine += $"\n\t{health_recovery} Health";
                                    }
                                }
                                #endregion

                                #region Add buff items (effects like +1 walking speed)
                                // These variables are taken from the game's code itself (IClickableMenu.cs -> 1016 line)
                                bool edibleItem = actualInventory[i] != null && actualInventory[i] is StardewValley.Object && (int)(actualInventory[i] as StardewValley.Object).edibility != -300;
                                string[] buffIconsToDisplay = (edibleItem && Game1.objectInformation[(actualInventory[i] as StardewValley.Object).parentSheetIndex].Split('/').Length > 7) ? actualInventory[i].ModifyItemBuffs(Game1.objectInformation[(actualInventory[i] as StardewValley.Object).parentSheetIndex].Split('/')[7].Split(' ')) : null;
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
                                }
                                #endregion 
                            }

                            #region Narrate hovered required ingredients
                            if (extraItemToShowIndex != -1)
                            {
                                string itemName = Game1.objectInformation[extraItemToShowIndex].Split('/')[0];

                                if(extraItemToShowAmount != -1)
                                    requirements = $"Required: {extraItemToShowAmount} {itemName}";
                                else
                                    requirements = $"Required: {itemName}";
                            }
                            #endregion

                            if (hoverPrice != -1)
                            {
                                price = $"Sell Price: {hoverPrice} g";
                            }

                            if (giveExtraDetails)
                            {
                                if (stack > 1)
                                    toSpeak = $"{stack} {name} {quality}, \n{requirements}, \n{price}, \n{description}, \n{healthNStamine}, \n{buffs}";
                                else
                                    toSpeak = $"{name} {quality}, \n{requirements}, \n{price}, \n{description}, \n{healthNStamine}, \n{buffs}"; 
                            }
                            else
                            {
                                if (stack > 1)
                                    toSpeak = $"{stack} {name} {quality}, \n{requirements}, \n{price}";
                                else
                                    toSpeak = $"{name} {quality}, \n{requirements}, \n{price}";
                            }
                        }
                        else
                        {
                            // For empty slot
                            toSpeak = "Empty Slot";
                        }
                    }
                    else
                    {
                        // For empty slot
                        toSpeak = "Empty Slot";
                    }

                    if (hoveredItemQueryKey != $"{toSpeak}:{i}")
                    {
                        hoveredItemQueryKey = $"{toSpeak}:{i}";
                        MainClass.screenReader.Say(toSpeak, true);
                    }
                    return true;
                }
            }
            #endregion
            return false;
        }
    }
}
