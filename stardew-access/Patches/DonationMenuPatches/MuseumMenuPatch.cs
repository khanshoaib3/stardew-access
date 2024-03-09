using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Locations;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace stardew_access.Patches;

internal class MuseumMenuPatch : IPatch
{
    private static bool isMoving;

    private static readonly (int x, int y)[] DonationTiles =
    {
        (26, 5), (26, 6), (26, 7), (26, 8), (26, 9), (26, 10), (26, 11),
        (29, 5), (30, 5), (31, 5), (32, 5), (33, 5), (34, 5), (35, 5), (36, 5),
        (28, 6), (29, 6), (30, 6), (31, 6), (32, 6), (33, 6), (34, 6), (35, 6), (36, 6), (37, 6),
        (28, 9), (29, 9), (30, 9), (31, 9), (32, 9), (33, 9), (34, 9), (35, 9), (36, 9),
        (28, 10), (29, 10), (30, 10), (31, 10), (32, 10), (33, 10), (34, 10), (35, 10), (36, 10),
        (30, 13), (31, 13), (32, 13), (33, 13), (34, 13),
        (30, 14), (31, 14), (32, 14), (33, 14), (34, 14),
        (28, 15), (29, 15), (30, 15), (31, 15), (32, 15), (33, 15), (34, 15), (35, 15), (36, 15),
        (28, 16), (29, 16), (30, 16), (31, 16), (32, 16), (33, 16), (34, 16), (35, 16), (36, 16),
        (39, 6), (40, 6), (41, 6), (42, 6), (43, 6), (44, 6), (45, 6), (46, 6),
        (39, 7), (40, 7), (41, 7), (42, 7), (43, 7), (44, 7), (45, 7), (46, 7),
        (48, 5), (48, 6), (48, 7),
        (42, 15), (43, 15), (44, 15), (45, 15), (46, 15), (47, 15),
        (42, 16), (43, 16), (44, 16), (45, 16), (46, 16), (47, 16),
    };

    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(MuseumMenu), "draw"),
            postfix: new HarmonyMethod(typeof(MuseumMenuPatch), nameof(DrawPatch))
        );

        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(MuseumMenu), "receiveKeyPress"),
            prefix: new HarmonyMethod(typeof(MuseumMenuPatch), nameof(ReceiveKeyPressPatch))
        );
    }

    private static bool ReceiveKeyPressPatch()
    {
        try
        {
            if (isMoving) return false;

            if (!isMoving)
            {
                isMoving = true;
                Task.Delay(200).ContinueWith(_ => { isMoving = false; });
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in MuseumMenuPatch->ReceiveKeyPressPatch():\n{e.Message}\n{e.StackTrace}");
        }

        return true;
    }

    private static void DrawPatch(MuseumMenu __instance, bool ___holdingMuseumPiece)
    {
        try
        {
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

            NarrateMuseumInventory(__instance, x, y);

            NarratePlayerInventory(__instance, x, y, ___holdingMuseumPiece);
        }
        catch (Exception e)
        {
            Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
        }
    }

    private static void NarrateMuseumInventory(MuseumMenu __instance, int x, int y)
    {
        if (__instance.heldItem == null) return;

        string toSpeak = "";
        object? translationToken = null;
        int tileX = (int)(Utility.ModifyCoordinateFromUIScale(x) + Game1.viewport.X) / 64;
        int tileY = (int)(Utility.ModifyCoordinateFromUIScale(y) + Game1.viewport.Y) / 64;

        if (((LibraryMuseum)Game1.currentLocation).isTileSuitableForMuseumPiece(tileX, tileY))
        {
            toSpeak = "menu-museum-slot_info";
            translationToken = new { x_position = tileX, y_position = tileY };
        }

        MainClass.ScreenReader.TranslateAndSayWithMenuChecker(toSpeak, true, translationToken);
    }

    private static void NarratePlayerInventory(MuseumMenu __instance, int x, int y, bool ___holdingMuseumPiece)
    {
        if (__instance.heldItem != null) return;

        if (NarrateHoveredButtons(__instance, x, y)) return;

        string highlightedItemPrefix = Translator.Instance.Translate(
            "menu-donation_common-donatable_item_in_inventory-prefix",
            new { content = "" }, TranslationCategory.Menu);
        int hoveredItemIndex = InventoryUtils.NarrateHoveredSlotAndReturnIndex(__instance.inventory,
            highlightedItemPrefix: highlightedItemPrefix);
        if (hoveredItemIndex != -999)
        {
            bool isPrimaryInfoKeyPressed =
                MainClass.Config.PrimaryInfoKey.JustPressed(); // For donating hovered item

            if (isPrimaryInfoKeyPressed && hoveredItemIndex >= 0 &&
                hoveredItemIndex < __instance.inventory.actualInventory.Count &&
                __instance.inventory.actualInventory[hoveredItemIndex] != null)
            {
                ManuallyDonateItem(__instance, hoveredItemIndex, ___holdingMuseumPiece);
            }
        }
    }

    private static bool NarrateHoveredButtons(MuseumMenu __instance, int x, int y)
    {
        string translationKey = "";
        bool isDropItemButton = false;

        if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
        {
            translationKey = "common-ui-ok_button";
        }
        else if (__instance.dropItemInvisibleButton != null &&
                 __instance.dropItemInvisibleButton.containsPoint(x, y))
        {
            translationKey = "common-ui-drop_item_button";
            isDropItemButton = true;
        }
        else
        {
            return false;
        }

        if (!MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true)) return true;
        if (isDropItemButton) Game1.playSound("drop_item");

        return true;
    }

    private static void ManuallyDonateItem(MuseumMenu __instance, int i, bool ___holdingMuseumPiece)
    {
        foreach (var (x, y) in DonationTiles)
        {
            #region Manually donates the hovered item (MuseumMenu->receiveLeftClick())

            int tileX = x;
            int tileY = y;

            ((LibraryMuseum)Game1.currentLocation).museumPieces.Add(new Vector2(tileX, tileY),
                ((Object)__instance.inventory.actualInventory[i]).ItemId);
            if (((LibraryMuseum)Game1.currentLocation).isTileSuitableForMuseumPiece(tileX, tileY) &&
                ((LibraryMuseum)Game1.currentLocation).isItemSuitableForDonation(__instance.inventory
                    .actualInventory[i]))
            {
                string qualifiedItemId = __instance.inventory.actualInventory[i].QualifiedItemId;
                int rewardsCount = ((LibraryMuseum)Game1.currentLocation).getRewardsForPlayer(Game1.player).Count; ((LibraryMuseum)Game1.currentLocation).museumPieces.Add(new Vector2(tileX, tileY), ((Object)__instance.inventory.actualInventory[i]).ItemId);
                Game1.playSound("stoneStep");
                if (((LibraryMuseum)Game1.currentLocation).getRewardsForPlayer(Game1.player).Count > rewardsCount && !___holdingMuseumPiece)
                {
                    __instance.sparkleText = new SparklingText(Game1.dialogueFont, Game1.content.LoadString("Strings\\StringsFromCSFiles:NewReward"), Color.MediumSpringGreen, Color.White);
                    Game1.playSound("reward");
                    __instance.globalLocationOfSparklingArtifact = new Vector2( tileX * 64 + 32 - __instance.sparkleText.textWidth / 2f, tileY * 64 - 48);
                }
                else
                {
                    Game1.playSound("newArtifact");
                }

                Game1.player.completeQuest("24");
                --__instance.inventory.actualInventory[i].Stack;
                if (__instance.inventory.actualInventory[i].Stack <= 0)
                {
                    __instance.inventory.actualInventory[i] = null;
                }

                int pieces = ((LibraryMuseum)Game1.currentLocation).museumPieces.Length;
                if (!___holdingMuseumPiece)
                {
                    Game1.stats.checkForArchaeologyAchievements();
                    if (pieces == LibraryMuseum.totalArtifacts)
                        GlobalChatInfoMessage("MuseumComplete", Game1.player.farmName.Value);
                    else if (pieces == 40)
                        GlobalChatInfoMessage("Museum40", Game1.player.farmName.Value);
                    else
                        GlobalChatInfoMessage("donation", Game1.player.Name, "object:" + qualifiedItemId);
                }

                __instance.ReturnToDonatableItems();
                break;
            }

            #endregion
        }
    }

    #region These methods are taken from the game's source code, https://github.com/veywrn/StardewValley/blob/3ff171b6e9e6839555d7881a391b624ccd820a83/StardewValley/Multiplayer.cs#L1331-L1395

    private static void GlobalChatInfoMessage(string messageKey, params string[] args)
    {
        if (!Game1.IsMultiplayer && Game1.multiplayerMode == 0)
            return;
        ReceiveChatInfoMessage(Game1.player, messageKey, args);
        SendChatInfoMessage(messageKey, args);
    }

    private static void SendChatInfoMessage(string messageKey, params string[] args)
    {
        if (Game1.IsClient)
        {
            Game1.client.sendMessage(15, messageKey, args);
        }
        else
        {
            if (!Game1.IsServer) return;
            foreach (long key in Game1.otherFarmers.Keys)
                Game1.server.sendMessage(key, 15, Game1.player, messageKey, args);
        }
    }

    private static void ReceiveChatInfoMessage(Farmer sourceFarmer, string messageKey, string[] args)
    {
        if (Game1.chatBox == null) return;
        try
        {
            string[] array = args.Select((Func<string, string>)(arg =>
            {
                if (arg.StartsWith("achievement:"))
                {
                    int int32 = Convert.ToInt32(arg.Substring("achievement:".Length));
                    return DataLoader.Achievements(Game1.content)[int32].Split('^')[0];
                }

                return arg.StartsWith("object:")
                    ? ItemRegistry.Create(arg.Substring("object:".Length)).DisplayName
                    : arg;
            })).ToArray();
            Game1.chatBox.addInfoMessage(
                Game1.content.LoadString("Strings\\UI:Chat_" + messageKey, array));
        }
        #if DEBUG
        catch (ContentLoadException ex)
        {
            Log.Debug($"ContentLoadException in ReceiveChatInfoMessage: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Log.Debug($"FormatException in ReceiveChatInfoMessage: {ex.Message}");
        }
        catch (OverflowException ex)
        {
            Log.Debug($"OverflowException in ReceiveChatInfoMessage: {ex.Message}");
        }
        catch (KeyNotFoundException ex)
        {
            Log.Debug($"KeyNotFoundException in ReceiveChatInfoMessage: {ex.Message}");
        }
        #else
        catch (ContentLoadException) { }
        catch (FormatException) { }
        catch (OverflowException) { }
        catch (KeyNotFoundException) { }
        #endif
    }

        #endregion
    }