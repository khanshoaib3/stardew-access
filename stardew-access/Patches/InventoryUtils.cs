
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class InventoryUtils
    {
        internal static string hoveredItemQueryKey = "";
        internal static int prevSlotIndex = -999;

        internal static bool narrateHoveredItemInInventory(InventoryMenu inventoryMenu, List<ClickableComponent> inventory, IList<Item> actualInventory, int x, int y, bool giveExtraDetails = false, int hoverPrice = -1, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (!inventory[i].containsPoint(x, y)) continue;

                if ((i + 1) > actualInventory.Count || actualInventory[i] == null)
                {
                    // For empty slot
                    checkAndSpeak("Empty Slot", i);
                    prevSlotIndex = i;
                    return true;
                }

                string toSpeak = "";
                string name = $"{actualInventory[i].DisplayName}{handleUnHighlightedItem(inventoryMenu.highlightMethod(actualInventory[i]), i)}";
                int stack = actualInventory[i].Stack;
                string quality = getQualityFromItem(actualInventory[i]);
                string healthNStamine = getHealthNStaminaFromItem(actualInventory[i]);
                string buffs = getBuffsFromItem(actualInventory[i]);
                string description = actualInventory[i].getDescription();
                string price = getPrice(hoverPrice);
                string requirements = getExtraItemInfo(extraItemToShowIndex, extraItemToShowAmount);

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


                checkAndSpeak(toSpeak, i);
                prevSlotIndex = i;
                return true;
            }

            // If no slot is hovered
            return false;
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

            int qualityIndex = ((StardewValley.Object)item).Quality;
            if (qualityIndex == 1)
            {
                return "Silver quality";
            }
            else if (qualityIndex == 2 || qualityIndex == 3)
            {
                return "Gold quality";
            }
            else if (qualityIndex >= 4)
            {
                return "Iridium quality";
            }

            return "";
        }

        private static String getHealthNStaminaFromItem(Item item)
        {
            if (item is not StardewValley.Object || ((StardewValley.Object)item).Edibility == -300)
                return "";

            String toReturn = "";
            int stamina_recovery = ((StardewValley.Object)item).staminaRecoveredOnConsumption();
            toReturn += $"{stamina_recovery} Energy";

            if (stamina_recovery < 0) return toReturn;

            int health_recovery = ((StardewValley.Object)item).healthRecoveredOnConsumption();
            toReturn += $"\n\t{health_recovery} Health";

            return toReturn;
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
                    int count = int.Parse(buffName.Substring(0, buffName.IndexOf(' ')));
                    if (count != 0)
                        toReturn += $"{buffName}\n";
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
                return $"Required: {itemAmount} {itemName}";
            else
                return $"Required: {itemName}";
        }

        private static String getPrice(int price)
        {
            if (price == -1) return "";
            
            return $"Sell Price: {price} g";
        }

        private static String handleUnHighlightedItem(bool isHighlighted, int hoveredInventoryIndex)
        {
            if (isHighlighted) return "";
            
            if (prevSlotIndex != hoveredInventoryIndex)
                Game1.playSound("invalid-selection");

            return " not usable here";
        }
    }
}
