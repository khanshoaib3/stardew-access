using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace stardew_access.Patches
{
    // Menus in the game menu i.e., the menu which opens when we press `e`
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
        internal static string socialPageQuery = "";
        internal static string profilePageQuery = "";
        internal static string junimoNoteMenuQuery = "";
        internal static string collectionsPageQuery = "";
        internal static int currentSelectedCraftingRecipe = -1;
        internal static bool isSelectingRecipe = false;
        internal static bool isUsingCustomButtons = false;
        internal static int currentIngredientListItem = -1, currentIngredientInputSlot = -1, currentInventorySlot = -1;

        internal static void CollectionsPagePatch(CollectionsPage __instance)
        {
            try
            {
                int x = Game1.getMousePosition().X, y = Game1.getMousePosition().Y;
                if (__instance.letterviewerSubMenu != null)
                {
                    DialoguePatches.NarrateLetterContent(__instance.letterviewerSubMenu);
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void JunimoNoteMenuPatch(JunimoNoteMenu __instance, bool ___specificBundlePage, int ___whichArea, Bundle ___currentPageBundle)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                if (!___specificBundlePage)
                {
                    currentIngredientListItem = -1;
                    isUsingCustomButtons = false;

                    string areaName = __instance.scrambledText ? CommunityCenter.getAreaEnglishDisplayNameFromNumber(___whichArea) : CommunityCenter.getAreaDisplayNameFromNumber(___whichArea);
                    if (__instance.scrambledText)
                    {
                        string toSpeak = "Scrambled Text";
                        if (junimoNoteMenuQuery != toSpeak)
                        {
                            junimoNoteMenuQuery = toSpeak;
                            MainClass.GetScreenReader().Say(toSpeak, true);
                        }
                        return;
                    }
                    for (int i = 0; i < __instance.bundles.Count; i++)
                    {
                        if (__instance.bundles[i].containsPoint(x, y))
                        {
                            string toSpeak = $"{__instance.bundles[i].name} bundle";
                            if (junimoNoteMenuQuery != toSpeak)
                            {
                                junimoNoteMenuQuery = toSpeak;
                                MainClass.GetScreenReader().Say(toSpeak, true);
                            }
                            return;
                        }
                    }
                    if (__instance.presentButton != null && __instance.presentButton.containsPoint(x, y))
                    {
                        string toSpeak = "Present Button";
                        if (junimoNoteMenuQuery != toSpeak)
                        {
                            junimoNoteMenuQuery = toSpeak;
                            MainClass.GetScreenReader().Say(toSpeak, true);
                        }
                        return;
                    }
                    if (__instance.fromGameMenu)
                    {
                        if (__instance.areaNextButton.visible && __instance.areaNextButton.containsPoint(x, y))
                        {
                            string toSpeak = "Next Area Button";
                            if (junimoNoteMenuQuery != toSpeak)
                            {
                                junimoNoteMenuQuery = toSpeak;
                                MainClass.GetScreenReader().Say(toSpeak, true);
                            }
                            return;
                        }
                        if (__instance.areaBackButton.visible && __instance.areaBackButton.containsPoint(x, y))
                        {
                            string toSpeak = "Previous Area Button";
                            if (junimoNoteMenuQuery != toSpeak)
                            {
                                junimoNoteMenuQuery = toSpeak;
                                MainClass.GetScreenReader().Say(toSpeak, true);
                            }
                            return;
                        }
                    }
                }
                else
                {
                    bool isIPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.I); // For the ingredients
                    bool isCPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.C); // For the items in inventory
                    bool isPPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.P); // For the Purchase Button
                    bool isVPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.V); // For the ingredient input slots
                    bool isBackPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Back); // For the back button
                    bool isLeftShiftPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);

                    if (isIPressed && !isUsingCustomButtons)
                    {
                        isUsingCustomButtons = true;
                        JunimoNoteCustomButtons(__instance, ___currentPageBundle, 0, isLeftShiftPressed);
                        Task.Delay(200).ContinueWith(_ => { isUsingCustomButtons = false; });
                    }
                    else if (isVPressed && !isUsingCustomButtons)
                    {
                        isUsingCustomButtons = true;
                        JunimoNoteCustomButtons(__instance, ___currentPageBundle, 1, isLeftShiftPressed);
                        Task.Delay(200).ContinueWith(_ => { isUsingCustomButtons = false; });
                    }
                    else if (isCPressed && !isUsingCustomButtons)
                    {
                        isUsingCustomButtons = true;
                        JunimoNoteCustomButtons(__instance, ___currentPageBundle, 2, isLeftShiftPressed);
                        Task.Delay(200).ContinueWith(_ => { isUsingCustomButtons = false; });
                    }
                    else if (isBackPressed && __instance.backButton != null && !__instance.backButton.containsPoint(x, y))
                    {
                        __instance.backButton.snapMouseCursorToCenter();
                        MainClass.GetScreenReader().Say("Back Button", true);
                    }
                    else if (isPPressed && __instance.purchaseButton != null && !__instance.purchaseButton.containsPoint(x, y))
                    {
                        __instance.purchaseButton.snapMouseCursorToCenter();
                        MainClass.GetScreenReader().Say("Purchase Button", true);
                    }
                }
                string reward = __instance.getRewardNameForArea(___whichArea);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void JunimoNoteCustomButtons(JunimoNoteMenu __instance, Bundle ___currentPageBundle, int signal, bool isLeftShiftPressed = false)
        {
            try
            {

                switch (signal)
                {
                    case 0: // For ingredient list
                        {
                            if (___currentPageBundle.ingredients.Count >= 0)
                            {
                                currentIngredientListItem = currentIngredientListItem + (isLeftShiftPressed ? -1 : 1);
                                if (currentIngredientListItem >= ___currentPageBundle.ingredients.Count)
                                    if (isLeftShiftPressed)
                                        currentIngredientListItem = ___currentPageBundle.ingredients.Count - 1;
                                    else
                                        currentIngredientListItem = 0;

                                if (currentIngredientListItem < 0)
                                    if (isLeftShiftPressed)
                                        currentIngredientListItem = ___currentPageBundle.ingredients.Count - 1;
                                    else
                                        currentIngredientListItem = 0;

                                ClickableTextureComponent c = __instance.ingredientList[currentIngredientListItem];
                                BundleIngredientDescription ingredient = ___currentPageBundle.ingredients[currentIngredientListItem];

                                Item item = new StardewValley.Object(ingredient.index, ingredient.stack, isRecipe: false, -1, ingredient.quality);
                                bool completed = false;
                                if (___currentPageBundle != null && ___currentPageBundle.ingredients != null && currentIngredientListItem < ___currentPageBundle.ingredients.Count && ___currentPageBundle.ingredients[currentIngredientListItem].completed)
                                {
                                    completed = true;
                                }

                                string toSpeak = item.DisplayName;

                                if (!completed)
                                {
                                    int quality = ingredient.quality;
                                    if (quality == 1)
                                    {
                                        toSpeak = $"Silver quality {toSpeak}";
                                    }
                                    else if (quality == 2 || quality == 3)
                                    {
                                        toSpeak = $"Gold quality {toSpeak}";
                                    }
                                    else if (quality >= 4)
                                    {
                                        toSpeak = $"Iridium quality {toSpeak}";
                                    }

                                    toSpeak = $"{ingredient.stack} {toSpeak}";
                                }

                                if (completed)
                                    toSpeak = $"Completed {toSpeak}";

                                c.snapMouseCursorToCenter();
                                MainClass.GetScreenReader().Say(toSpeak, true);
                            }
                        }
                        break;
                    case 1: // For input slot list
                        {
                            if (__instance.ingredientSlots.Count >= 0)
                            {
                                currentIngredientInputSlot = currentIngredientInputSlot + (isLeftShiftPressed ? -1 : 1);
                                if (currentIngredientInputSlot >= __instance.ingredientSlots.Count)
                                    if (isLeftShiftPressed)
                                        currentIngredientInputSlot = __instance.ingredientSlots.Count - 1;
                                    else
                                        currentIngredientInputSlot = 0;

                                if (currentIngredientInputSlot < 0)
                                    if (isLeftShiftPressed)
                                        currentIngredientInputSlot = __instance.ingredientSlots.Count - 1;
                                    else
                                        currentIngredientInputSlot = 0;

                                ClickableTextureComponent c = __instance.ingredientSlots[currentIngredientInputSlot];
                                Item item = c.item;
                                string toSpeak;

                                if (item == null)
                                {
                                    toSpeak = $"Input Slot {currentIngredientInputSlot + 1}";
                                }
                                else
                                {
                                    toSpeak = item.DisplayName;
                                }

                                c.snapMouseCursorToCenter();
                                MainClass.GetScreenReader().Say(toSpeak, true);
                            }
                        }
                        break;
                    case 2: // For inventory slots
                        {
                            if (__instance.inventory != null && __instance.inventory.actualInventory.Count >= 0)
                            {
                                int prevSlotIndex = currentInventorySlot;
                                currentInventorySlot = currentInventorySlot + (isLeftShiftPressed ? -1 : 1);
                                if (currentInventorySlot >= __instance.inventory.actualInventory.Count)
                                    if (isLeftShiftPressed)
                                        currentInventorySlot = __instance.inventory.actualInventory.Count - 1;
                                    else
                                        currentInventorySlot = 0;

                                if (currentInventorySlot < 0)
                                    if (isLeftShiftPressed)
                                        currentInventorySlot = __instance.inventory.actualInventory.Count - 1;
                                    else
                                        currentInventorySlot = 0;

                                Item item = __instance.inventory.actualInventory[currentInventorySlot];
                                ClickableComponent c = __instance.inventory.inventory[currentInventorySlot];
                                string toSpeak;
                                if (item != null)
                                {
                                    toSpeak = item.DisplayName;

                                    if ((item as StardewValley.Object) != null)
                                    {
                                        int quality = ((StardewValley.Object)item).Quality;
                                        if (quality == 1)
                                        {
                                            toSpeak = $"Silver quality {toSpeak}";
                                        }
                                        else if (quality == 2 || quality == 3)
                                        {
                                            toSpeak = $"Gold quality {toSpeak}";
                                        }
                                        else if (quality >= 4)
                                        {
                                            toSpeak = $"Iridium quality {toSpeak}";
                                        }
                                    }
                                    toSpeak = $"{item.Stack} {toSpeak}";

                                }
                                else
                                {
                                    toSpeak = "Empty Slot";
                                }
                                c.snapMouseCursorToCenter();
                                MainClass.GetScreenReader().Say(toSpeak, true);
                            }
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

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
                                    MainClass.GetScreenReader().Say(toSpeak, true);
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
                                        MainClass.GetScreenReader().Say(toSpeak, true);
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

        internal static void ShopMenuPatch(ShopMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                bool isIPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.I);
                bool isLeftShiftPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);

                if (isLeftShiftPressed && isIPressed && __instance.inventory.inventory.Count > 0)
                {
                    __instance.inventory.inventory[0].snapMouseCursorToCenter();
                    __instance.setCurrentlySnappedComponentTo(__instance.inventory.inventory[0].myID);
                }
                else if (!isLeftShiftPressed && isIPressed && __instance.forSaleButtons.Count > 0)
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
                    }
                }
                #endregion
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
                            gameMenuQueryKey = toSpeak;
                            MainClass.GetScreenReader().Say(toSpeak, true);
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

        internal static void GeodeMenuPatch(GeodeMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                #region Narrate the treasure recieved on breaking the geode
                if (__instance.geodeTreasure != null)
                {
                    string name = __instance.geodeTreasure.DisplayName;
                    int stack = __instance.geodeTreasure.Stack;

                    string toSpeak = $"Recieved {stack} {name}";

                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
                {
                    string toSpeak = "Drop item here";

                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    string toSpeak = "Ok button";

                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        MainClass.GetScreenReader().Say(toSpeak, true);
                    }
                    return;
                }
                #endregion

                #region Narrate hovered item
                if (narrateHoveredItemInInventory(__instance.inventory.inventory, __instance.inventory.actualInventory, x, y))
                    geodeMenuQueryKey = "";
                #endregion
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void ItemGrabMenuPatch(ItemGrabMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                bool isIPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.I);
                bool isLeftShiftPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);

                if (isLeftShiftPressed && isIPressed && __instance.inventory.inventory.Count > 0)
                {
                    __instance.setCurrentlySnappedComponentTo(__instance.inventory.inventory[0].myID);
                    __instance.inventory.inventory[0].snapMouseCursorToCenter();
                }
                else if (!isLeftShiftPressed && isIPressed && __instance.ItemsToGrabMenu.inventory.Count > 0 && !__instance.shippingBin)
                {
                    __instance.setCurrentlySnappedComponentTo(__instance.ItemsToGrabMenu.inventory[0].myID);
                    __instance.ItemsToGrabMenu.inventory[0].snapMouseCursorToCenter();
                }

                #region Narrate buttons in the menu
                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    string toSpeak = "Ok Button";
                    if (itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        hoveredItemQueryKey = "";
                        gameMenuQueryKey = "";
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.colorPickerToggleButton != null && __instance.colorPickerToggleButton.containsPoint(x, y))
                {

                    string toSpeak = "Color Picker: " + (__instance.chestColorPicker.visible ? "Enabled" : "Disabled");
                    if (itemGrabMenuQueryKey != toSpeak)
                    {
                        itemGrabMenuQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
                    }
                    return;
                }
                #endregion

                #region Narrate hovered item
                if (narrateHoveredItemInInventory(__instance.inventory.inventory, __instance.inventory.actualInventory, x, y, true))
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
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
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
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
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
                else if (!isLeftShiftPressed && isIPressed && __instance.pagesOfCraftingRecipes[___currentCraftingPage].Count > 0)
                {
                    // snap to first crafting recipe
                    __instance.setCurrentlySnappedComponentTo(__instance.pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(0).Key.myID);
                    __instance.pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(0).Key.snapMouseCursorToCenter();
                    currentSelectedCraftingRecipe = 0;
                }
                else if (isCPressed && !isSelectingRecipe)
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                    if (producesItem is StardewValley.Object && ((StardewValley.Object)producesItem).Edibility != -300)
                    {
                        int stamina_recovery = ((StardewValley.Object)producesItem).staminaRecoveredOnConsumption();
                        buffs += $"{stamina_recovery} Energy";
                        if (stamina_recovery >= 0)
                        {
                            int health_recovery = ((StardewValley.Object)producesItem).healthRecoveredOnConsumption();
                            buffs += $"\n{health_recovery} Health";
                        }
                    }
                    // These variables are taken from the game's code itself (IClickableMenu.cs -> 1016 line)
                    bool edibleItem = producesItem != null && producesItem is StardewValley.Object && (int)((StardewValley.Object)producesItem).edibility != -300;
                    string[]? buffIconsToDisplay = (edibleItem && Game1.objectInformation[((StardewValley.Object)producesItem).ParentSheetIndex].Split('/').Length > 7)
                        ? producesItem.ModifyItemBuffs(Game1.objectInformation[((StardewValley.Object)producesItem).ParentSheetIndex].Split('/')[7].Split(' '))
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
                    #endregion


                    string toSpeak = $"{numberOfProduce} {name}, {craftable}, \n\t{ingredients}, \n\t{description} \n\t{buffs}";

                    if (craftingPageQueryKey != toSpeak)
                    {
                        craftingPageQueryKey = toSpeak;
                        gameMenuQueryKey = "";
                        hoveredItemQueryKey = "";
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                            MainClass.GetScreenReader().Say("unknown recipe", true);
                        }
                        return;
                    }
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
                    }
                    return;
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
                            MainClass.GetScreenReader().Say(toSpeak, true);
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
                            MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
                    }
                    return;
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
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
                            if (actualInventory[i] is StardewValley.Object && ((StardewValley.Object)actualInventory[i]).Quality > 0)
                            {
                                int qualityIndex = ((StardewValley.Object)actualInventory[i]).Quality;
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
                                if (actualInventory[i] is StardewValley.Object && ((StardewValley.Object)actualInventory[i]).Edibility != -300)
                                {
                                    int stamina_recovery = ((StardewValley.Object)actualInventory[i]).staminaRecoveredOnConsumption();
                                    healthNStamine += $"{stamina_recovery} Energy";
                                    if (stamina_recovery >= 0)
                                    {
                                        int health_recovery = ((StardewValley.Object)actualInventory[i]).healthRecoveredOnConsumption();
                                        healthNStamine += $"\n\t{health_recovery} Health";
                                    }
                                }
                                #endregion

                                #region Add buff items (effects like +1 walking speed)
                                // These variables are taken from the game's code itself (IClickableMenu.cs -> 1016 line)
                                bool edibleItem = actualInventory[i] != null && actualInventory[i] is StardewValley.Object && (int)((StardewValley.Object)actualInventory[i]).edibility != -300;
                                string[]? buffIconsToDisplay = (edibleItem && Game1.objectInformation[((StardewValley.Object)actualInventory[i]).ParentSheetIndex].Split('/').Length > 7) ? actualInventory[i].ModifyItemBuffs(Game1.objectInformation[((StardewValley.Object)actualInventory[i]).ParentSheetIndex].Split('/')[7].Split(' ')) : null;
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

                                if (extraItemToShowAmount != -1)
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
                        MainClass.GetScreenReader().Say(toSpeak, true);
                    }
                    return true;
                }
            }
            #endregion
            return false;
        }
    }
}
