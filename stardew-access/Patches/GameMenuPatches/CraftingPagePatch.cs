using StardewValley;
using stardew_access.Utils;
using StardewValley.Menus;
using StardewValley.Objects;
using stardew_access.Translation;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

namespace stardew_access.Patches
{
    internal class CraftingPagePatch : IPatch
    {
        internal static int currentSelectedCraftingRecipe = -1;
        internal static bool isSelectingRecipe = false;

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(CraftingPage), nameof(CraftingPage.draw), new Type[] { typeof(SpriteBatch) }),
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

                InventoryUtils.NarrateHoveredSlot(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in crafting page patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void HandleKeyBinds(CraftingPage __instance, int ___currentCraftingPage)
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
                CycleThroughRecipes(__instance.pagesOfCraftingRecipes, ___currentCraftingPage, __instance);
                Task.Delay(200).ContinueWith(_ => { isSelectingRecipe = false; });
            }
        }

        private static bool NarrateMenuButtons(CraftingPage __instance, int x, int y)
        {
            string? toSpeak = null;
            bool isDropItemButton = false;

            if (__instance.upButton != null && __instance.upButton.containsPoint(x, y))
            {
                toSpeak = Translator.Instance.Translate("menu-crafting_page-previous_recipe_list_button");
            }
            else if (__instance.downButton != null && __instance.downButton.containsPoint(x, y))
            {
                toSpeak = Translator.Instance.Translate("menu-crafting_page-next_recipe_list_button");
            }
            else if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
            {
                toSpeak = Translator.Instance.Translate("common-ui-trashcan_button");
            }
            else if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
            {
                toSpeak = Translator.Instance.Translate("common-ui-drop_item_button");
                isDropItemButton = true;
            }
            else
            {
                return false;
            }

            if (MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true))
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

                MainClass.ScreenReader.SayWithMenuChecker(Translator.Instance.Translate("menu-crafting_page-unknown_recipe"), true);
                return true;
            }

            Item producesItem = ___hoverRecipe.createItem();
            string name = ___hoverRecipe.DisplayName;
            int numberOfProduce = ___hoverRecipe.numberProducedPerCraft;
            string description = ___hoverRecipe.description;
            string ingredients = InventoryUtils.GetIngredientsFromRecipe(___hoverRecipe);
            string buffs = $"{InventoryUtils.GetHealthNStaminaFromItem(producesItem)}, {InventoryUtils.GetBuffsFromItem(producesItem)}";
            int isCraftable = ___hoverRecipe.doesFarmerHaveIngredientsInInventory(GetContainerContents(__instance._materialContainers)) ? 1 : 0;

            string toSpeak = Translator.Instance.Translate("menu-cragting_page-recipe_info",
                new {
                    produce_count = numberOfProduce,
                    name = name,
                    is_craftable = isCraftable,
                    ingredients = ingredients,
                    description = description,
                    buffs = buffs
                });

            MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);

            return true;
        }

        private static void CycleThroughRecipes(List<Dictionary<ClickableTextureComponent, CraftingRecipe>> pagesOfCraftingRecipes, int ___currentCraftingPage, CraftingPage __instance)
        {
            currentSelectedCraftingRecipe++;
            if (currentSelectedCraftingRecipe < 0 || currentSelectedCraftingRecipe >= pagesOfCraftingRecipes[0].Count)
                currentSelectedCraftingRecipe = 0;

            __instance.setCurrentlySnappedComponentTo(pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(currentSelectedCraftingRecipe).Key.myID);
            pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(currentSelectedCraftingRecipe).Key.snapMouseCursorToCenter();

            // Skip if recipe is not unlocked/unknown
            if (pagesOfCraftingRecipes[___currentCraftingPage].ElementAt(currentSelectedCraftingRecipe).Key.hoverText.Equals("ghosted"))
                CycleThroughRecipes(pagesOfCraftingRecipes, ___currentCraftingPage, __instance);
        }

        // This method is used to get the inventory items to check if the player has enough ingredients for a recipe
        // Taken from CraftingPage.cs -> 169 line
        internal static IList<Item>? GetContainerContents(List<Chest> materialContainers)
        {
            if (materialContainers == null)
            {
                return null;
            }
            List<Item> items = new();
            for (int i = 0; i < materialContainers.Count; i++)
            {
                items.AddRange(materialContainers[i].items);
            }
            return items;
        }

        internal static void Cleanup()
        {
            currentSelectedCraftingRecipe = -1;
            isSelectingRecipe = false;
        }
    }
}
