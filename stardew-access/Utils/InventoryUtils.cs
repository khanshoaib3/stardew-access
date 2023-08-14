using StardewValley;
using StardewValley.Menus;
using stardew_access.Translation;

namespace stardew_access.Utils
{
    internal class InventoryUtils
    {
        internal static int prevSlotIndex = -999;

        internal static bool NarrateHoveredSlot(InventoryMenu inventoryMenu, List<ClickableComponent> inventory, IList<Item> actualInventory, int x, int y,
                bool? giveExtraDetails = null, int hoverPrice = -1, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1,
                bool handleHighlightedItem = false, String highlightedItemPrefix = "", String highlightedItemSuffix = "")
        {
            if (NarrateHoveredSlotAndReturnIndex(inventoryMenu, inventory, actualInventory, x, y,
                giveExtraDetails, hoverPrice, extraItemToShowIndex, extraItemToShowAmount,
                handleHighlightedItem, highlightedItemPrefix, highlightedItemSuffix) == -999)
                return false;

            return true;
        }

        internal static int NarrateHoveredSlotAndReturnIndex(InventoryMenu inventoryMenu, List<ClickableComponent> inventory, IList<Item> actualInventory, int x, int y,
                bool? giveExtraDetails = null, int hoverPrice = -1, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1,
                bool handleHighlightedItem = false, String highlightedItemPrefix = "", String highlightedItemSuffix = "")
        {
            giveExtraDetails ??= !MainClass.Config.DisableInventoryVerbosity;
            for (int i = 0; i < inventory.Count; i++)
            {
                if (!inventory[i].containsPoint(x, y)) continue;

                if ((i + 1) > actualInventory.Count || actualInventory[i] == null)
                {
                    // For empty slot
                    CheckAndSpeak(Translator.Instance.Translate("menu-inventory-empty_slot-name"), i);
                    prevSlotIndex = i;
                    return i;
                }

                bool isHighlighted = inventoryMenu.highlightMethod(actualInventory[i]);

                string namePrefix = HandleHighlightedItemPrefix(isHighlighted, highlightedItemPrefix);
                string nameSuffix = $"{HandleHighlightedItemSuffix(isHighlighted, highlightedItemSuffix)}{HandleUnHighlightedItem(isHighlighted, i)}";
                int stack = actualInventory[i].Stack;
                string name = Translator.Instance.Translate("common-util-pluralize_name", new {item_count = stack, name = actualInventory[i].DisplayName});
                name = $"{namePrefix}{name}{nameSuffix}";
                string quality = GetQualityFromItem(actualInventory[i]);
                string healthNStamina = GetHealthNStaminaFromItem(actualInventory[i]);
                string buffs = GetBuffsFromItem(actualInventory[i]);
                string description = actualInventory[i].getDescription();
                string price = GetPrice(hoverPrice);
                string requirements = GetExtraItemInfo(extraItemToShowIndex, extraItemToShowAmount);

                string details;
                string toSpeak = name;
                // TODO remove , from here and buffs
                if (giveExtraDetails == true)
                {
                    details = string.Join(", ", new string[] { quality, requirements, price, description, healthNStamina, buffs }.Where(c => !string.IsNullOrEmpty(c)));
                }
                else
                {
                    details = string.Join(", ", new string[] { quality, requirements, price }.Where(c => !string.IsNullOrEmpty(c)));
                }
                if (!string.IsNullOrEmpty(details))
                    toSpeak = $"{toSpeak}, {details}";

                CheckAndSpeak(toSpeak, i);
                prevSlotIndex = i;
                return i;
            }

            // If no slot is hovered
            return -999;
        }
        
        internal static String GetQualityFromItem(Item item)
        {
            if (item is not StardewValley.Object)
                return "";

            return GetQualityFromIndex(((StardewValley.Object)item).Quality);
        }
        
        internal static String GetQualityFromIndex(int qualityIndex)
        {
            if (qualityIndex <= 0)
                return "";

            return Translator.Instance.Translate("item-quality_type", new {quality_index = qualityIndex});
        }

        internal static String GetHealthNStaminaFromItem(Item item)
        {
            if (item is not StardewValley.Object || ((StardewValley.Object)item).Edibility == -300)
                return "";

            int stamina_recovery = ((StardewValley.Object)item).staminaRecoveredOnConsumption();
            int health_recovery = ((StardewValley.Object)item).healthRecoveredOnConsumption();
            return Translator.Instance.Translate("item-stamina_and_health_recovery_on_consumption", new {stamina_amount = stamina_recovery, health_amount = health_recovery});
        }

        internal static String GetBuffsFromItem(Item item)
        {
            if (item == null) return "";
            if (item is not StardewValley.Object) return "";
            if (((StardewValley.Object)item) == null) return "";

            // These variables are taken from the game's code itself (IClickableMenu.cs -> 1016 line)
            bool edibleItem = (int)((StardewValley.Object)item).Edibility != -300;
            string[]? buffIconsToDisplay = (edibleItem && Game1.objectInformation[((StardewValley.Object)item).ParentSheetIndex].Split('/').Length > 7)
                ? item.ModifyItemBuffs(Game1.objectInformation[((StardewValley.Object)item).ParentSheetIndex].Split('/')[7].Split(' '))
                : null;

            if (buffIconsToDisplay == null)
                return "";

            String toReturn = "";
            for (int j = 0; j < buffIconsToDisplay.Length; j++)
            {
                string buffName = ((Convert.ToInt32(buffIconsToDisplay[j]) > 0) ? "+" : "") + buffIconsToDisplay[j] + " ";
                if (j <= 11)
                {
                    buffName = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + j, buffName);
                }
                try
                {
                    int count = int.Parse(buffName[..buffName.IndexOf(' ')]);
                    if (count != 0)
                        toReturn += $"{buffName}, ";
                }
                catch (Exception) { }
            }
            return toReturn;
        }

        internal static String GetExtraItemInfo(int itemIndex, int itemAmount)
        {
            if (itemIndex == -1) return "";

            string itemName = Game1.objectInformation[itemIndex].Split('/')[0];

            if (itemAmount != -1)
                return Translator.Instance.Translate("item-required_item_info",
                        new
                        {
                            name = Translator.Instance.Translate("common-util-pluralize_name", new {name = itemName, item_count = itemAmount})
                        });
            else
                return Translator.Instance.Translate("item-required_item_info", new {name = itemName});
        }

        internal static String GetIngredientsFromRecipe(CraftingRecipe recipe)
        {
            if (recipe is null) return "";

            List<string> ingredientList = new();
            for (int i = 0; i < recipe.recipeList.Count; i++)
            {
                int recipeCount = recipe.recipeList.ElementAt(i).Value;
                int recipeItem = recipe.recipeList.ElementAt(i).Key;
                string recipeName = recipe.getNameFromIndex(recipeItem);

                ingredientList.Add($"{recipeCount} {recipeName}");
            }

            return string.Join(", ", ingredientList);
        }

        internal static String GetPrice(int price)
        {
            if (price == -1) return "";
            
            return Translator.Instance.Translate("item-sell_price_info", new { price });
        }

        internal static String HandleHighlightedItemPrefix(bool isHighlighted, String prefix)
        {
            if (MainClass.Config.DisableInventoryVerbosity) return "";
            if (!isHighlighted) return "";

            return prefix;
        }

        internal static String HandleHighlightedItemSuffix(bool isHighlighted, String suffix)
        {
            if (MainClass.Config.DisableInventoryVerbosity) return "";
            if (!isHighlighted) return "";

            return suffix;
        }

        internal static String HandleUnHighlightedItem(bool isHighlighted, int hoveredInventoryIndex)
        {
            if (isHighlighted) return "";
            
            if (prevSlotIndex != hoveredInventoryIndex)
                Game1.playSound("invalid-selection");

            if (MainClass.Config.DisableInventoryVerbosity) return "";
            return Translator.Instance.Translate("item-suffix-not_usable_here", new {content = ""});
        }

        internal static void Cleanup()
        {
            prevSlotIndex = -999;
        }

        private static void CheckAndSpeak(String toSpeak, int hoveredInventoryIndex)
        {
            MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true, $"{toSpeak}:{hoveredInventoryIndex}");
        }
    }
}
