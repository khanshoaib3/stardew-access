using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class SocialPagePatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(SocialPage), "draw"),
            postfix: new HarmonyMethod(typeof(SocialPagePatch), nameof(SocialPagePatch.DrawPatch))
        );
    }

    private static void DrawPatch(SocialPage __instance, List<ClickableTextureComponent> ___sprites,
        int ___slotPosition)
    {
        try
        {
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
            for (int i = ___slotPosition; i < ___slotPosition + 5 && i < ___sprites.Count; i++)
            {
                if (!__instance.characterSlots[i].bounds.Contains(x, y))
                    continue;
                SocialPage.SocialEntry entry = __instance.GetSocialEntry(i);
                
                string name = entry.Character.displayName;
                int has_talked = !entry.IsMet ? 2 : Game1.player.hasPlayerTalkedToNPC(entry.Character.Name) ? 1 : 0;
                string? relationship_status = GetRelationshipStatus(entry) ?? "";
                int gifts_this_week = entry.Friendship?.GiftsThisWeek ?? 0;
                int heart_level = entry.HeartLevel;
                if (entry.IsPlayer)
                {
                    MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-social_page-player_info", true, new
                    {
                        name,
                        relationship_status
                    });
                } else {
                    MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-social_page-npc_info", true, new
                    {
                        name,
                        has_talked,
                        relationship_status,
                        heart_level,
                        gifts_this_week
                    });
                }
                break;
            }
            
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in social page patch:\n{e.Message}\n{e.StackTrace}");
        }
    }

    static string? GetRelationshipStatus(SocialPage.SocialEntry entry)
    {
        // Copied (and modified) from StardewValley.Menus.SocialPage.DrawNPCSlot
        Gender gender = entry.Gender;
        bool datable = entry.IsDatable;
        bool isDating = entry.IsDatingCurrentPlayer();
        bool isCurrentSpouse = entry.IsMarriedToCurrentPlayer();
        bool housemate = entry.IsRoommateForCurrentPlayer();
        string? relationship_status = null;
        if (!entry.IsPlayer)
        {
            if (datable || housemate)
            {
                relationship_status = ((!Game1.content.ShouldUseGenderedCharacterTranslations()) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635") : ((gender == Gender.Male) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/')[0] : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').Last()));
                if (housemate)
                {
                    relationship_status = Game1.content.LoadString("Strings\\StringsFromCSFiles:Housemate");
                }
                else if (isCurrentSpouse)
                {
                    relationship_status = ((gender == Gender.Male) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11636") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11637"));
                }
                else if (entry.IsMarriedToAnyone())
                {
                    relationship_status = ((gender == Gender.Male) ? Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_MaleNPC") : Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_FemaleNPC"));
                }
                else if (!Game1.player.isMarriedOrRoommates() && isDating)
                {
                    relationship_status = ((gender == Gender.Male) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11639") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11640"));
                }
                else if (entry.IsDivorcedFromCurrentPlayer())
                {
                    relationship_status = ((gender == Gender.Male) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11642") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11643"));
                }
            }
        } else {
            Farmer farmer = (Farmer)entry.Character;
            relationship_status = ((!Game1.content.ShouldUseGenderedCharacterTranslations()) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635") : ((gender == Gender.Male) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/')[0] : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').Last()));
            if (isCurrentSpouse)
            {
                relationship_status = ((gender == Gender.Male) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11636") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11637"));
            }
            else if (farmer.isMarriedOrRoommates() && !farmer.hasRoommate())
            {
                relationship_status = ((gender == Gender.Male) ? Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_MaleNPC") : Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_FemaleNPC"));
            }
            else if (!Game1.player.isMarriedOrRoommates() && entry.IsDatingCurrentPlayer())
            {
                relationship_status = ((gender == Gender.Male) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11639") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11640"));
            }
            else if (entry.IsDivorcedFromCurrentPlayer())
            {
                relationship_status = ((gender == Gender.Male) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11642") : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11643"));
            }
        }
        return !String.IsNullOrWhiteSpace(relationship_status) ? relationship_status : null;
    }
}
