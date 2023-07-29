using Microsoft.Xna.Framework;
using StardewValley;
using stardew_access.Utils;
using StardewValley.Locations;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class MuseumMenuPatch
    {
        private static bool isMoving = false;
        private static readonly (int x, int y)[] donationTiles =
        {
            (26,5),(26,6),(26,7),(26,8),(26,9),(26,10),(26,11),
            (29,5),(30,5),(31,5),(32,5),(33,5),(34,5),(35,5),(36,5),
            (28,6),(29,6),(30,6),(31,6),(32,6),(33,6),(34,6),(35,6),(36,6),(37,6),
            (28,9),(29,9),(30,9),(31,9),(32,9),(33,9),(34,9),(35,9),(36,9),
            (28,10),(29,10),(30,10),(31,10),(32,10),(33,10),(34,10),(35,10),(36,10),
            (30,13),(31,13),(32,13),(33,13),(34,13),
            (30,14),(31,14),(32,14),(33,14),(34,14),
            (28,15),(29,15),(30,15),(31,15),(32,15),(33,15),(34,15),(35,15),(36,15),
            (28,16),(29,16),(30,16),(31,16),(32,16),(33,16),(34,16),(35,16),(36,16),
            (39,6),(40,6),(41,6),(42,6),(43,6),(44,6),(45,6),(46,6),
            (39,7),(40,7),(41,7),(42,7),(43,7),(44,7),(45,7),(46,7),
            (48,5),(48,6),(48,7),
            (42,15),(43,15),(44,15),(45,15),(46,15),(47,15),
            (42,16),(43,16),(44,16),(45,16),(46,16),(47,16),
        };

        internal static bool RecieveKeyPressPatch()
        {
            try
            {
                if (isMoving)
                    return false;

                if (!isMoving)
                {
                    isMoving = true;
                    Task.Delay(200).ContinueWith(_ => { isMoving = false; });
                }

            }
            catch (Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }

            return true;
        }

        internal static void DrawPatch(MuseumMenu __instance, bool ___holdingMuseumPiece)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                NarrateMuseumInventory(__instance, x, y);

                NarratePlayerInventory(__instance, x, y);
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
            int tileX = (int)(Utility.ModifyCoordinateFromUIScale(x) + (float)Game1.viewport.X) / 64;
            int tileY = (int)(Utility.ModifyCoordinateFromUIScale(y) + (float)Game1.viewport.Y) / 64;
            LibraryMuseum libraryMuseum = (LibraryMuseum)Game1.currentLocation;

            if (libraryMuseum.isTileSuitableForMuseumPiece(tileX, tileY))
                toSpeak = $"slot {tileX}x {tileY}y";

            MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
        }

        private static void NarratePlayerInventory(MuseumMenu __instance, int x, int y)
        {
            if (__instance.heldItem != null) return;

            if (NarrateHoveredButtons(__instance, x, y)) return;

            int hoveredItemIndex = InventoryUtils.NarrateHoveredSlotAndReturnIndex(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y,
                    handleHighlightedItem: true, highlightedItemPrefix: "Donatable ");
            if (hoveredItemIndex != -9999)
            {
                bool isPrimaryInfoKeyPressed = MainClass.Config.PrimaryInfoKey.JustPressed(); // For donating hovered item

                if (isPrimaryInfoKeyPressed && hoveredItemIndex >= 0 && hoveredItemIndex < __instance.inventory.actualInventory.Count && __instance.inventory.actualInventory[hoveredItemIndex] != null)
                {
                    ManuallyDonateItem(__instance, hoveredItemIndex);
                }
            }
        }

        private static bool NarrateHoveredButtons(MuseumMenu __instance, int x, int y)
        {
            string toSpeak = "";
            bool isDropItemButton = false;

            if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
            {
                toSpeak = "Ok button";
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

            MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
            if (isDropItemButton) Game1.playSound("drop_item");

            return true;
        }

        private static void ManuallyDonateItem(MuseumMenu __instance, int i)
        {
            foreach (var (x, y) in donationTiles)
            {
                #region Manually donates the hovered item (https://github.com/veywrn/StardewValley/blob/3ff171b6e9e6839555d7881a391b624ccd820a83/StardewValley/Menus/MuseumMenu.cs#L206-L247)
                int tileX = x;
                int tileY = y;

                if (((LibraryMuseum)Game1.currentLocation).isTileSuitableForMuseumPiece(tileX, tileY) && ((LibraryMuseum)Game1.currentLocation).isItemSuitableForDonation(__instance.inventory.actualInventory[i]))
                {
                    int objectID = __instance.inventory.actualInventory[i].ParentSheetIndex;
                    int rewardsCount = ((LibraryMuseum)Game1.currentLocation).getRewardsForPlayer(Game1.player).Count;
                    ((LibraryMuseum)Game1.currentLocation).museumPieces.Add(new Vector2(tileX, tileY), ((StardewValley.Object)__instance.inventory.actualInventory[i]).ParentSheetIndex);
                    Game1.playSound("stoneStep");
                    if (((LibraryMuseum)Game1.currentLocation).getRewardsForPlayer(Game1.player).Count > rewardsCount)
                    {
                        Game1.playSound("reward");
                    }
                    else
                    {
                        Game1.playSound("newArtifact");
                    }
                    Game1.player.completeQuest(24);
                    __instance.inventory.actualInventory[i].Stack--;
                    if (__instance.inventory.actualInventory[i].Stack <= 0)
                    {
                        __instance.inventory.actualInventory[i] = null;
                    }
                    int pieces = ((LibraryMuseum)Game1.currentLocation).museumPieces.Count();
                    Game1.stats.checkForArchaeologyAchievements();
                    switch (pieces)
                    {
                        case 95:
                            GlobalChatInfoMessage("MuseumComplete", Game1.player.farmName.Value);
                            break;
                        case 40:
                            GlobalChatInfoMessage("Museum40", Game1.player.farmName.Value);
                            break;
                        default:
                            GlobalChatInfoMessage("donation", Game1.player.Name, "object:" + objectID);
                            break;
                    }
                    break;
                }
                #endregion
            }
        }

        #region These methods are taken from the game's source code, https://github.com/veywrn/StardewValley/blob/3ff171b6e9e6839555d7881a391b624ccd820a83/StardewValley/Multiplayer.cs#L1331-L1395
        internal static void GlobalChatInfoMessage(string messageKey, params string[] args)
        {
            if (Game1.IsMultiplayer || Game1.multiplayerMode != 0)
            {
                ReceiveChatInfoMessage(Game1.player, messageKey, args);
                SendChatInfoMessage(messageKey, args);
            }
        }

        internal static void SendChatInfoMessage(string messageKey, params string[] args)
        {
            if (Game1.IsClient)
            {
                Game1.client.sendMessage(15, messageKey, args);
            }
            else if (Game1.IsServer)
            {
                foreach (long id in Game1.otherFarmers.Keys)
                {
                    Game1.server.sendMessage(id, 15, Game1.player, messageKey, args);
                }
            }
        }

        internal static void ReceiveChatInfoMessage(Farmer sourceFarmer, string messageKey, string[] args)
        {
            if (Game1.chatBox != null)
            {
                try
                {
                    string[] processedArgs = args.Select(delegate (string arg)
                    {
                        if (arg.StartsWith("achievement:"))
                        {
                            int key = Convert.ToInt32(arg["achievement:".Length..]);
                            return Game1.content.Load<Dictionary<int, string>>("Data\\Achievements")[key].Split('^')[0];
                        }
                        return arg.StartsWith("object:") ? new StardewValley.Object(Convert.ToInt32(arg["object:".Length..]), 1).DisplayName : arg;
                    }).ToArray();
                    ChatBox chatBox = Game1.chatBox;
                    LocalizedContentManager content = Game1.content;
                    string path = "Strings\\UI:Chat_" + messageKey;
                    object[] substitutions = processedArgs;
                    chatBox.addInfoMessage(content.LoadString(path, substitutions));
                }
                catch (Microsoft.Xna.Framework.Content.ContentLoadException)
                {
                }
                catch (FormatException)
                {
                }
                catch (OverflowException)
                {
                }
                catch (KeyNotFoundException)
                {
                }
            }
        }
        #endregion
    }
}
