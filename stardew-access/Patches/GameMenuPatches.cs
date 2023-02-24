using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

namespace stardew_access.Patches
{
    // Menus in the game menu i.e., the menu which opens when we press `e`
    internal class GameMenuPatches
    {
        internal static string hoveredItemQueryKey = "";
        internal static string gameMenuQueryKey = "";
        internal static string craftingPageQueryKey = "";
        internal static string inventoryPageQueryKey = "";
        internal static string exitPageQueryKey = "";
        internal static string optionsPageQueryKey = "";
        internal static string socialPageQuery = "";
        internal static string profilePageQuery = "";
        internal static int currentSelectedCraftingRecipe = -1;
        internal static bool isSelectingRecipe = false;


        internal static void SocialPagePatch(SocialPage __instance, List<ClickableTextureComponent> ___sprites, int ___slotPosition, List<string> ___kidsNames)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                for (int i = ___slotPosition; i < ___slotPosition + 5; i++)
                {
                    if (i < ___sprites.Count)
                    {
                        if (__instance.names[i] is string)
                        {
                            #region  For NPCs
                            if (__instance.characterSlots[i].bounds.Contains(Game1.getMouseX(true), Game1.getMouseY(true)))
                            {
                                string name = $"{__instance.names[i] as string}";
                                int heartLevel = Game1.player.getFriendshipHeartLevelForNPC(name);
                                bool datable = SocialPage.isDatable(name);
                                Friendship friendship = __instance.getFriendship(name);
                                int giftsThisWeek = friendship.GiftsThisWeek;
                                bool hasTalked = Game1.player.hasPlayerTalkedToNPC(name);
                                bool spouse = friendship.IsMarried();
                                bool housemate = spouse && SocialPage.isRoommateOfAnyone(name);
                                ___kidsNames.Add("Robin");
                                ___kidsNames.Add("Pierre");
                                ___kidsNames.Add("Caroline");
                                ___kidsNames.Add("Jodi");
                                ___kidsNames.Add("Kent");
                                ___kidsNames.Add("George");
                                ___kidsNames.Add("Evelyn");
                                ___kidsNames.Add("Demetrius");



                                string toSpeak = $"{name}";

                                if (!hasTalked)
                                {
                                    toSpeak = $"{toSpeak}, not talked yet";
                                }


                                if (datable | housemate)
                                {
                                    string text2 = (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.pt) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635") : ((__instance.getGender(name) == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').First() : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').Last());
                                    if (housemate)
                                    {
                                        text2 = Game1.content.LoadString("Strings\\StringsFromCSFiles:Housemate");
                                    }
                                    else if (spouse)
                                    {
                                        text2 = ((__instance.getGender(name) == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11636") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11637"));
                                    }
                                    else if (__instance.isMarriedToAnyone(name))
                                    {
                                        text2 = ((__instance.getGender(name) == 0) ? Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_MaleNPC") : Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_FemaleNPC"));
                                    }
                                    else if (!Game1.player.isMarried() && friendship.IsDating())
                                    {
                                        text2 = ((__instance.getGender(name) == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11639") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11640"));
                                    }
                                    else if (__instance.getFriendship(name).IsDivorced())
                                    {
                                        text2 = ((__instance.getGender(name) == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11642") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11643"));
                                    }

                                    toSpeak = $"{toSpeak}, {text2}";
                                }
                                if (!__instance.getFriendship(name).IsMarried() && ___kidsNames.Contains(name))
                                {
                                    toSpeak = $"{toSpeak}, married";
                                }
                                if (spouse)
                                {
                                    toSpeak = $"{toSpeak}, spouse";
                                }
                                else if (friendship.IsDating())
                                {
                                    toSpeak = $"{toSpeak}, dating";
                                }

                                toSpeak = $"{toSpeak}, {heartLevel} hearts, {giftsThisWeek} gifts given this week.";


                                if (socialPageQuery != toSpeak)
                                {
                                    socialPageQuery = toSpeak;
                                    MainClass.ScreenReader.Say(toSpeak, true);
                                }
                                return;
                            }
                            #endregion
                        }
                        else if (__instance.names[i] is long)
                        {
                            #region  For Farmers

                            long farmerID = (long)__instance.names[i];
                            Farmer farmer = Game1.getFarmerMaybeOffline(farmerID);
                            if (farmer != null)
                            {
                                int gender = (!farmer.IsMale) ? 1 : 0;
                                ClickableTextureComponent clickableTextureComponent = ___sprites[i];
                                if (clickableTextureComponent.containsPoint(x, y))
                                {
                                    Friendship friendship = Game1.player.team.GetFriendship(Game1.player.UniqueMultiplayerID, farmerID);
                                    bool spouse = friendship.IsMarried();
                                    string toSpeak = "";

                                    string text2 = (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.pt) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635") : ((gender == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').First() : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').Last());
                                    if (spouse)
                                    {
                                        text2 = ((gender == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11636") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11637"));
                                    }
                                    else if (farmer.isMarried() && !farmer.hasRoommate())
                                    {
                                        text2 = ((gender == 0) ? Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_MaleNPC") : Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_FemaleNPC"));
                                    }
                                    else if (!Game1.player.isMarried() && friendship.IsDating())
                                    {
                                        text2 = ((gender == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11639") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11640"));
                                    }
                                    else if (friendship.IsDivorced())
                                    {
                                        text2 = ((gender == 0) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11642") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11643"));
                                    }

                                    toSpeak = $"{farmer.displayName}, {text2}";

                                    if (socialPageQuery != toSpeak)
                                    {
                                        socialPageQuery = toSpeak;
                                        MainClass.ScreenReader.Say(toSpeak, true);
                                    }
                                    return;
                                }
                            }

                            #endregion
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void GameMenuPatch(GameMenu __instance)
        {
            try
            {
                // Continue if only in the Inventory Page or Crafting Page
                if (__instance.currentTab != 0 && __instance.currentTab != 4 && __instance.currentTab != 6 && __instance.currentTab != 7)
                    return;

                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                for (int i = 0; i < __instance.tabs.Count; i++)
                {
                    if (__instance.tabs[i].containsPoint(x, y))
                    {
                        string toSpeak = $"{GameMenu.getLabelOfTabFromIndex(i)} Tab";
                        if (gameMenuQueryKey != toSpeak)
                        {
                            MainClass.DebugLog("here");
                            gameMenuQueryKey = toSpeak;
                            MainClass.ScreenReader.Say(toSpeak, true);
                        }
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void CraftingPagePatch(CraftingPage __instance, CraftingRecipe ___hoverRecipe, int ___currentCraftingPage)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (MainClass.Config.SnapToFirstInventorySlotKey.JustPressed() && __instance.inventory.inventory.Count > 0)
                {
                    // snap to first inventory slot
                    __instance.setCurrentlySnappedComponentTo(__instance.inventory.inventory[0].myID);
                    __instance.inventory.inventory[0].snapMouseCursorToCenter();
                    currentSelectedCraftingRecipe = -1;
                }
                else if (MainClass.Config.SnapToFirstSecondaryInventorySlotKey.JustPressed() && __instance.pagesOfCraftingRecipes[___currentCraftingPage].Count > 0)
                {
                    // snap to first crafting recipe
                    __instance.setCurrentlySnappedComponentTo(__instance.pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(0).Key.myID);
                    __instance.pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(0).Key.snapMouseCursorToCenter();
                    currentSelectedCraftingRecipe = 0;
                }
                else if (MainClass.Config.CraftingMenuCycleThroughRecipiesKey.JustPressed() && !isSelectingRecipe)
                {
                    isSelectingRecipe = true;
                    CycleThroughRecipies(__instance.pagesOfCraftingRecipes, ___currentCraftingPage, __instance);
                    Task.Delay(200).ContinueWith(_ => { isSelectingRecipe = false; });
                }

                #region Narrate buttons in the menu
                if (__instance.upButton != null && __instance.upButton.containsPoint(x, y))
                {
                    string toSpeak = "Previous Recipe List";
                    if (craftingPageQueryKey != toSpeak)
                    {
                        craftingPageQueryKey = toSpeak;
                        hoveredItemQueryKey = "";
                        MainClass.ScreenReader.Say(toSpeak, true);
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
                        MainClass.ScreenReader.Say(toSpeak, true);
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
                        MainClass.ScreenReader.Say(toSpeak, true);
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
                        MainClass.ScreenReader.Say(toSpeak, true);
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
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                    return;
                }
                else
                {
                    var isRecipeInFocus = false;
                    foreach (var item in __instance.pagesOfCraftingRecipes[___currentCraftingPage])
                    {
                        if (item.Key.containsPoint(x, y))
                        {
                            isRecipeInFocus = true;
                            break;
                        }
                    }

                    if (isRecipeInFocus)
                    {
                        string query = $"unknown recipe:{__instance.getCurrentlySnappedComponent().myID}";

                        if (craftingPageQueryKey != query)
                        {
                            craftingPageQueryKey = query;
                            gameMenuQueryKey = "";
                            hoveredItemQueryKey = "";
                            MainClass.ScreenReader.Say("unknown recipe", true);
                        }
                        return;
                    }
                }
                #endregion

                #region Narrate hovered item
                if (InventoryUtils.narrateHoveredSlot(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y))
                {
                    gameMenuQueryKey = "";
                    craftingPageQueryKey = "";
                    return;
                }
                #endregion
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
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

        internal static void InventoryPagePatch(InventoryPage __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                #region Narrate buttons in the menu
                if (__instance.inventory.dropItemInvisibleButton != null && __instance.inventory.dropItemInvisibleButton.containsPoint(x, y))
                {
                    string toSpeak = "Drop Item";
                    if (inventoryPageQueryKey != toSpeak)
                    {
                        inventoryPageQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.ScreenReader.Say(toSpeak, true);
                        Game1.playSound("drop_item");
                    }
                }

                if (__instance.organizeButton != null && __instance.organizeButton.containsPoint(x, y))
                {
                    string toSpeak = "Organize Inventory Button";
                    if (inventoryPageQueryKey != toSpeak)
                    {
                        inventoryPageQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                }

                if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
                {
                    string toSpeak = "Trash Can";
                    if (inventoryPageQueryKey != toSpeak)
                    {
                        inventoryPageQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                }

                if (__instance.organizeButton != null && __instance.organizeButton.containsPoint(x, y))
                {
                    string toSpeak = "Organize Button";
                    if (inventoryPageQueryKey != toSpeak)
                    {
                        inventoryPageQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                }

                if (__instance.junimoNoteIcon != null && __instance.junimoNoteIcon.containsPoint(x, y))
                {

                    string toSpeak = "Community Center Button";
                    if (inventoryPageQueryKey != toSpeak)
                    {
                        inventoryPageQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                }
                #endregion

                #region Narrate equipment slots
                for (int i = 0; i < __instance.equipmentIcons.Count; i++)
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
                            MainClass.ScreenReader.Say(toSpeak, true);
                        }
                    }
                }
                #endregion

                #region Narrate hovered item
                if (InventoryUtils.narrateHoveredSlot(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y, true))
                {
                    gameMenuQueryKey = "";
                    inventoryPageQueryKey = "";
                }
                #endregion

                if (MainClass.Config.MoneyKey.JustPressed())
                {
                    string farmName = Game1.content.LoadString("Strings\\UI:Inventory_FarmName", Game1.player.farmName.Value);
                    string currentFunds = Game1.content.LoadString("Strings\\UI:Inventory_CurrentFunds" + (Game1.player.useSeparateWallets ? "_Separate" : ""), Utility.getNumberWithCommas(Game1.player.Money));
                    string totalEarnings = Game1.content.LoadString("Strings\\UI:Inventory_TotalEarnings" + (Game1.player.useSeparateWallets ? "_Separate" : ""), Utility.getNumberWithCommas((int)Game1.player.totalMoneyEarned));
                    int festivalScore = Game1.player.festivalScore;
                    int walnut = Game1.netWorldState.Value.GoldenWalnuts.Value;
                    int qiGems = Game1.player.QiGems;
                    int qiCoins = Game1.player.clubCoins;

                    string toSpeak = $"{farmName}\n{currentFunds}\n{totalEarnings}";

                    if (festivalScore > 0)
                        toSpeak = $"{toSpeak}\nFestival Score: {festivalScore}";

                    if (walnut > 0)
                        toSpeak = $"{toSpeak}\nGolden Walnut: {walnut}";

                    if (qiGems > 0)
                        toSpeak = $"{toSpeak}\nQi Gems: {qiGems}";

                    if (qiCoins > 0)
                        toSpeak = $"{toSpeak}\nQi Club Coins: {qiCoins}";

                    MainClass.ScreenReader.Say(toSpeak, true);
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void OptionsPagePatch(OptionsPage __instance)
        {
            try
            {
                int currentItemIndex = Math.Max(0, Math.Min(__instance.options.Count - 7, __instance.currentItemIndex));
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                for (int i = 0; i < __instance.optionSlots.Count; i++)
                {
                    if (__instance.optionSlots[i].bounds.Contains(x, y) && currentItemIndex + i < __instance.options.Count && __instance.options[currentItemIndex + i].bounds.Contains(x - __instance.optionSlots[i].bounds.X, y - __instance.optionSlots[i].bounds.Y))
                    {
                        OptionsElement optionsElement = __instance.options[currentItemIndex + i];
                        string toSpeak = optionsElement.label;

                        if (optionsElement is OptionsButton)
                            toSpeak = $" {toSpeak} Button";
                        else if (optionsElement is OptionsCheckbox)
                            toSpeak = (((OptionsCheckbox)optionsElement).isChecked ? "Enabled" : "Disabled") + $" {toSpeak} Checkbox";
                        else if (optionsElement is OptionsDropDown)
                            toSpeak = $"{toSpeak} Dropdown, option {((OptionsDropDown)optionsElement).dropDownDisplayOptions[((OptionsDropDown)optionsElement).selectedOption]} selected";
                        else if (optionsElement is OptionsSlider)
                            toSpeak = $"{((OptionsSlider)optionsElement).value}% {toSpeak} Slider";
                        else if (optionsElement is OptionsPlusMinus)
                            toSpeak = $"{((OptionsPlusMinus)optionsElement).displayOptions[((OptionsPlusMinus)optionsElement).selected]} selected of {toSpeak}";
                        else if (optionsElement is OptionsInputListener)
                        {
                            string buttons = "";
                            ((OptionsInputListener)optionsElement).buttonNames.ForEach(name => { buttons += $", {name}"; });
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
                            MainClass.ScreenReader.Say(toSpeak, true);
                        }
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void ExitPagePatch(ExitPage __instance)
        {
            try
            {
                if (__instance.exitToTitle.visible &&
                        __instance.exitToTitle.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    string toSpeak = "Exit to Title Button";
                    if (exitPageQueryKey != toSpeak)
                    {
                        gameMenuQueryKey = "";
                        exitPageQueryKey = toSpeak;
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                    return;
                }
                if (__instance.exitToDesktop.visible &&
                    __instance.exitToDesktop.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                {
                    string toSpeak = "Exit to Desktop Button";
                    if (exitPageQueryKey != toSpeak)
                    {
                        gameMenuQueryKey = "";
                        exitPageQueryKey = toSpeak;
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                    return;
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
