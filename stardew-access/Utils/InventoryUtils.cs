using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Utils
{
    internal class InventoryUtils
    {
        internal static string hoveredItemQueryKey = "";
        internal static int prevSlotIndex = -999;

        internal static bool narrateHoveredSlot(InventoryMenu inventoryMenu, List<ClickableComponent> inventory, IList<Item> actualInventory, int x, int y,
                bool? giveExtraDetails = null, int hoverPrice = -1, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1,
                bool handleHighlightedItem = false, String highlightedItemPrefix = "", String highlightedItemSuffix = "")
        {
            if (narrateHoveredSlotAndReturnIndex(inventoryMenu, inventory, actualInventory, x, y,
                giveExtraDetails, hoverPrice, extraItemToShowIndex, extraItemToShowAmount,
                handleHighlightedItem, highlightedItemPrefix, highlightedItemSuffix) == -999)
                return false;

            return true;
        }

        internal static int narrateHoveredSlotAndReturnIndex(InventoryMenu inventoryMenu, List<ClickableComponent> inventory, IList<Item> actualInventory, int x, int y,
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
                    checkAndSpeak(Translator.Instance.Translate("menu-inventory-empty_slot-name"), i);
                    prevSlotIndex = i;
                    return i;
                }

                bool isHighlighted = inventoryMenu.highlightMethod(actualInventory[i]);

                string namePrefix = handleHighlightedItemPrefix(isHighlighted, highlightedItemPrefix);
                string nameSuffix = $"{handleHighlightedItemSuffix(isHighlighted, highlightedItemSuffix)}{handleUnHighlightedItem(isHighlighted, i)}";
                int stack = actualInventory[i].Stack;
                string name = Translator.Instance.Translate("common-util-pluralize_name", new {item_count = stack, name = actualInventory[i].DisplayName});
                name = $"{namePrefix}{name}{nameSuffix}";
                string quality = getQualityFromItem(actualInventory[i]);
                string healthNStamina = getHealthNStaminaFromItem(actualInventory[i]);
                string buffs = getBuffsFromItem(actualInventory[i]);
                string description = actualInventory[i].getDescription();
                string price = getPrice(hoverPrice);
                string requirements = getExtraItemInfo(extraItemToShowIndex, extraItemToShowAmount);

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

                checkAndSpeak(toSpeak, i);
                prevSlotIndex = i;
                return i;
            }

            // If no slot is hovered
            return -999;
        }
        
        private static void checkAndSpeak(String toSpeak, int hoveredInventoryIndex)
        {
            if (hoveredItemQueryKey == $"{toSpeak}:{hoveredInventoryIndex}") return;
            
            hoveredItemQueryKey = $"{toSpeak}:{hoveredInventoryIndex}";
            MainClass.ScreenReader.Say(toSpeak, true);
        }

        private static String getQualityFromItem(Item item)
        {
            if (item is not StardewValley.Object || ((StardewValley.Object)item).Quality <= 0)
                return "";

            return Translator.Instance.Translate("item-quality_type", new {quality_index = ((StardewValley.Object)item).Quality});
        }

        private static String getHealthNStaminaFromItem(Item item)
        {
            if (item is not StardewValley.Object || ((StardewValley.Object)item).Edibility == -300)
                return "";

            int stamina_recovery = ((StardewValley.Object)item).staminaRecoveredOnConsumption();
            int health_recovery = ((StardewValley.Object)item).healthRecoveredOnConsumption();
            return Translator.Instance.Translate("item-stamina_and_health_recovery_on_consumption", new {stamina_amount = stamina_recovery, health_amount = health_recovery});
        }

        private static String getBuffsFromItem(Item item)
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

        private static String getExtraItemInfo(int itemIndex, int itemAmount)
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

        private static String getPrice(int price)
        {
            if (price == -1) return "";
            
            return Translator.Instance.Translate("item-sell_price_info", new {price = price});
        }

        private static String handleHighlightedItemPrefix(bool isHighlighted, String prefix)
        {
            if (MainClass.Config.DisableInventoryVerbosity) return "";
            if (!isHighlighted) return "";

            return prefix;
        }

        private static String handleHighlightedItemSuffix(bool isHighlighted, String suffix)
        {
            if (MainClass.Config.DisableInventoryVerbosity) return "";
            if (!isHighlighted) return "";

            return suffix;
        }

        private static String handleUnHighlightedItem(bool isHighlighted, int hoveredInventoryIndex)
        {
            if (isHighlighted) return "";
            
            if (prevSlotIndex != hoveredInventoryIndex)
                Game1.playSound("invalid-selection");

            if (MainClass.Config.DisableInventoryVerbosity) return "";
            return Translator.Instance.Translate("item-suffix-not_usable_here", new {content = ""});
        }

        internal static void Cleanup()
        {
            hoveredItemQueryKey = "";
            prevSlotIndex = -999;
        }
    }
}
