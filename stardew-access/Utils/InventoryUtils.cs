using StardewValley;
using StardewValley.Menus;
using stardew_access.Translation;
using StardewValley.Buffs;
using StardewValley.Tools;
using StardewValley.TokenizableStrings;

namespace stardew_access.Utils;

internal static class InventoryUtils
{
    internal static int prevSlotIndex = -999;
    private static int prevStack = 0;
    private static string prevName = "";
    private static string prevTranslatedName = "";
    private static int prev_stamina_recovery = 0;
    private static int prev_health_recovery = 0;
    private static string prev_stamina_and_health_recovery_on_consumption = "";

    internal static bool NarrateHoveredSlot(InventoryMenu? inventoryMenu,
        bool? giveExtraDetails = null,
        int hoverPrice = -1,
        string? extraItemToShowIndex = null,
        int extraItemToShowAmount = -1,
        string highlightedItemPrefix = "",
        string highlightedItemSuffix = "",
        int? hoverX = null,
        int? hoverY = null)
    {
        if (NarrateHoveredSlotAndReturnIndex(inventoryMenu,
                giveExtraDetails,
                hoverPrice,
                extraItemToShowIndex,
                extraItemToShowAmount,
                highlightedItemPrefix,
                highlightedItemSuffix,
                hoverX,
                hoverY) == -999)
        {
            return false;
        }

        return true;
    }

    internal static int NarrateHoveredSlotAndReturnIndex(InventoryMenu? inventoryMenu,
        bool? giveExtraDetails = null,
        int hoverPrice = -1,
        string? extraItemToShowIndex = null,
        int extraItemToShowAmount = -1,
        string highlightedItemPrefix = "",
        string highlightedItemSuffix = "",
        int? hoverX = null,
        int? hoverY = null)
    {
        giveExtraDetails ??= !MainClass.Config.DisableInventoryVerbosity;
        int mouseX = hoverX ?? Game1.getMouseX(true);
        int mouseY = hoverY ?? Game1.getMouseY(true);
        List<ClickableComponent> inventory = inventoryMenu!.inventory;
        IList<Item> actualInventory = inventoryMenu.actualInventory;

        for (int i = 0; i < inventory.Count; i++)
        {
            if (!inventory[i].containsPoint(mouseX, mouseY)) continue;

            if ((i + 1) > actualInventory.Count || actualInventory[i] == null)
            {
                // For empty slot
                CheckAndSpeak(Translator.Instance.Translate("inventory_util-empty_slot"), i);
                prevSlotIndex = i;
                return i;
            }

            bool? isHighlighted = inventoryMenu.highlightMethod(actualInventory[i]);

            string itemDetails = GetItemDetails(actualInventory[i],
                i,
                isHighlighted,
                (bool)giveExtraDetails, // giveExtraDetails is already converted to bool because of first statement in this method.
                hoverPrice,
                extraItemToShowIndex,
                extraItemToShowAmount,
                highlightedItemPrefix,
                highlightedItemSuffix);

            CheckAndSpeak(itemDetails, i);
            prevSlotIndex = i;
            return i;
        }

        // If no slot is hovered
        return -999;
    }

    internal static string GetItemDetails(Item item,
        int indexInInventory = -999,
        bool? isHighlighted = null,
        bool giveExtraDetails = false,
        int hoverPrice = -1,
        string? extraItemToShowIndex = null,
        int extraItemToShowAmount = -1,
        string highlightedItemPrefix = "",
        string highlightedItemSuffix = "",
        string[]? customBuffs = null)
    {
        string namePrefix = HandleHighlightedItemPrefix(isHighlighted, highlightedItemPrefix);
        string nameSuffix =
            $"{HandleHighlightedItemSuffix(isHighlighted, highlightedItemSuffix)}{HandleUnHighlightedItem(isHighlighted, indexInInventory)}";
        string name = GetPluralNameOfItem(item);
        name = $"{namePrefix}{name}{nameSuffix}";
        string quality = GetQualityFromItem(item);
        string healthNStamina = GetHealthNStaminaFromItem(item);
        string buffs = (customBuffs is not null)
            ? string.Join(", ", customBuffs)
            : GetBuffsFromItem(item);
        string description = item.getDescription();
        string price = GetPrice(hoverPrice);
        string requirements = GetExtraItemInfo(extraItemToShowIndex, extraItemToShowAmount);
        string enchants = GetEnchantmentsFromItem(item);

        string details;
        string toReturn = name;
        // TODO remove , from here and buffs
        if (giveExtraDetails)
        {
            details = string.Join(", ", new string[]
            {
                quality,
                enchants,
                requirements,
                price,
                description,
                healthNStamina,
                buffs
            }.Where(c => !string.IsNullOrEmpty(c)));
        }
        else
        {
            details = string.Join(", ", new string[]
            {
                quality,
                enchants,
                requirements,
                price
            }.Where(c => !string.IsNullOrEmpty(c)));
        }

        if (!string.IsNullOrEmpty(details))
            toReturn = $"{toReturn}, {details}";

        return toReturn;
    }
    internal static string GetPluralNameOfItem(Item item) => GetPluralNameOfItem(item.DisplayName, item.Stack);

    internal static string GetPluralNameOfItem(string itemName, int itemCount)
    {
        if (itemCount == prevStack && itemName == prevName)
        {
#if DEBUG
            Log.Trace($"Returning cached translation \"{prevTranslatedName}\" for stack \"{itemCount}\" and name \"{itemName}\"",
                true);
#endif
            itemName = prevTranslatedName;
        }
        else
        {
            prevStack = itemCount;
            prevName = itemName;
            itemName = Translator.Instance.Translate("common-util-pluralize_name", new Dictionary<string, object>
            {
                { "item_count", itemCount },
                { "name", itemName }
            });
            prevTranslatedName = itemName;
#if DEBUG
            Log.Verbose("Updated inventory translation cache");
#endif
        }

        return itemName;
    }

    internal static string GetQualityFromItem(Item item) => GetQualityFromIndex(item.Quality);

    internal static string GetQualityFromIndex(int qualityIndex) => qualityIndex > 0
        ? Translator.Instance.Translate("item-quality_type", new { quality_index = qualityIndex })
        : "";

    internal static string GetEnchantmentsFromItem(Item item)
    {
        if (item is not MeleeWeapon and not Tool) return "";

        List<string> enchantNames = [];

        var enchantList = (item is MeleeWeapon) ? (item as MeleeWeapon)!.enchantments : (item as Tool)!.enchantments;
        foreach (var enchantment in enchantList)
        {
            enchantNames.Add(enchantment.GetDisplayName());
        }

        return string.Join(", ", enchantNames);
    }

    internal static string GetHealthNStaminaFromItem(Item? item)
    {
        if (item is null or not StardewValley.Object || ((StardewValley.Object)item).Edibility == -300)
            return "";

        int stamina_recovery = ((StardewValley.Object)item).staminaRecoveredOnConsumption();
        int health_recovery = ((StardewValley.Object)item).healthRecoveredOnConsumption();
        if (stamina_recovery != prev_stamina_recovery || health_recovery != prev_health_recovery ||
            string.IsNullOrEmpty(prev_stamina_and_health_recovery_on_consumption))
        {
            prev_stamina_recovery = stamina_recovery;
            prev_health_recovery = health_recovery;
            prev_stamina_and_health_recovery_on_consumption = Translator.Instance.Translate(
                "item-stamina_and_health_recovery_on_consumption",
                new { stamina_amount = stamina_recovery, health_amount = health_recovery });
        }

        return prev_stamina_and_health_recovery_on_consumption;
    }

    internal static string GetBuffsFromItem(Item? item)
        => (item is null or not StardewValley.Object) ? "" : GetBuffsFromItem(item.QualifiedItemId);

    internal static string GetBuffsFromItem(string qualifiedItemId)
    {
        var buffs = ObjectUtils.GetObjectById(qualifiedItemId)?.Buffs;
        string[] buffIconsToDisplay;
        if (buffs != null && buffs.Any())
        {
            // TODO: investigate using non-legacy format???
            buffIconsToDisplay = buffs.SelectMany(buff => 
                new BuffEffects(buff.CustomAttributes).ToLegacyAttributeFormat()).ToArray();
        }
        else
        {
            buffIconsToDisplay = new string[0];
        }

        string toReturn = "";
        for (int j = 0; j < buffIconsToDisplay.Length; j++)
        {
            if (!int.TryParse(buffIconsToDisplay[j], out int buffValue))
            {
                buffValue = 0;
            }
            string buffName = ((buffValue > 0) ? "+" : "") + buffIconsToDisplay[j] + " ";

            if (j <= 11)
            {
                buffName = Game1.content.LoadString("strings\\UI:ItemHover_Buff" + j, buffName);
            }

            try
            {
                int count = int.Parse(buffName[..buffName.IndexOf(' ')]);
                if (count != 0)
                    toReturn += $"{buffName}, ";
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return toReturn;
    }

    internal static string GetExtraItemInfo(string? itemIndex, int? itemAmount)
    {
        if (itemIndex is null or "-1") return "";

        string? itemName = ObjectUtils.GetObjectById(itemIndex)?.DisplayName;
        if (itemName is null) return "";
        itemName = TokenParser.ParseText(itemName);

        if (itemAmount is null or <= 0)
            return Translator.Instance.Translate("item-required_item_info", new { name = itemName });

        return Translator.Instance.Translate("item-required_item_info",
            new
            {
                name = Translator.Instance.Translate("common-util-pluralize_name",
                    new { name = itemName, item_count = itemAmount })
            });
    }

    internal static string GetCraftingRecipeInfo(CraftingRecipe? recipe) => recipe is null ? ""
        : Translator.Instance.Translate("item-crafting_recipe_info", new
        {
            name = recipe.DisplayName,
            is_cooking_recipe = recipe.isCookingRecipe ? 1 : 0,
            recipe.description,
        });

    internal static string GetIngredientsFromRecipe(CraftingRecipe? recipe)
    {
        if (recipe is null) return "";

        List<string> ingredientList = [];
        for (int i = 0; i < recipe.recipeList.Count; i++)
        {
            int recipeCount = recipe.recipeList.ElementAt(i).Value;
            string recipeItem = recipe.recipeList.ElementAt(i).Key;
            string recipeName = recipe.getNameFromIndex(recipeItem);

            ingredientList.Add(GetPluralNameOfItem(recipeName, recipeCount));
        }

        return string.Join(", ", ingredientList);
    }

    internal static string GetPrice(int price) => price is -1 ? ""
        : Translator.Instance.Translate("item-sell_price_info", new { price });

    internal static string HandleHighlightedItemPrefix(bool? isHighlighted, string prefix)
    {
        if (isHighlighted == null) return "";
        if (MainClass.Config.DisableInventoryVerbosity) return "";
        if (isHighlighted == false) return "";

        return prefix;
    }

    internal static string HandleHighlightedItemSuffix(bool? isHighlighted, string suffix)
    {
        if (isHighlighted == null) return "";
        if (MainClass.Config.DisableInventoryVerbosity) return "";
        if (isHighlighted == false) return "";

        return suffix;
    }

    internal static string HandleUnHighlightedItem(bool? isHighlighted, int hoveredInventoryIndex)
    {
        if (isHighlighted is null or true) return "";

        if (prevSlotIndex != hoveredInventoryIndex)
            Game1.playSound("invalid-selection");

        if (MainClass.Config.DisableInventoryVerbosity) return "";
        return Translator.Instance.Translate("item-suffix-not_usable_here", new { content = "" });
    }

    internal static void Cleanup()
    {
        prevSlotIndex = -999;
    }

    private static void CheckAndSpeak(string toSpeak, int hoveredInventoryIndex)
        => MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true, $"{toSpeak}:{hoveredInventoryIndex}");
}
