using StardewValley;

namespace stardew_access.Utils
{
    internal static class NPCUtils
    {
        internal static void GhostNPC(NPC? npc, bool sameTile = false, int delay = 100)
        {
            if (npc != null && !npc.IsInvisible)
            {
                if (sameTile && (npc.Tile != Game1.player.Tile)) return;
                npc.IsInvisible = true;
                _ = UnghostNPC(npc, delay);
            }
        }

        internal static async Task UnghostNPC(NPC? npc, int delay)
        {
            await Task.Delay(delay);
            if (npc is not null)
            {
                npc.IsInvisible = false;
            }
        }
    }
}
