using stardew_access.Utils;
using static stardew_access.Utils.MovementHelpers;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using System.Text;
using System.Threading.Tasks;

namespace stardew_access.Features.Tracker
{
    internal class TileTrackerBase
    {

        public SortedList<string, Dictionary<string, SpecialObject>> Objects = new();

        public TileTrackerBase(object? arg = null)
        {
            this.FindObjects(arg);
        }

        public virtual void FindObjects(object? arg = null)
        {
            
        }

        public Boolean HasObjects()
        {
            return Objects.Any();
        }

        public SortedList<string, Dictionary<string, SpecialObject>> GetObjects()
        {
            return Objects;
        }

        public void AddFocusableObject(string category, string name, Vector2 tile, NPC? character = null)
        {

            if (!Objects.ContainsKey(category)) {
                Objects.Add(category, new());
            }

            SpecialObject sObject = new(name, tile);

            if(character != null) {
                sObject.character = character;
            }

            if(Objects[category].ContainsKey(name)) {
                sObject = GetClosest(sObject, Objects[category][name]);
            }

            Objects[category][name] = sObject;

        }

        public static SpecialObject GetClosest(SpecialObject item1, SpecialObject item2)
        {

            Vector2 player_tile = Game1.player.getTileLocation();

            double collide_distance = GetDistance(player_tile, item2.TileLocation);
            double new_distance = GetDistance(player_tile, item1.TileLocation);

            if (new_distance < collide_distance) {
                return item1;
            }
            return item2;
        }

    }
}
