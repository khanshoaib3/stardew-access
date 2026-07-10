using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

// TODO Figure out why the map page isn't detected in IClickableMenuPatch::DrawPatch()
public class MapPagePatch : IPatch
{
    private enum MapDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    private static bool _isCycling = false;

    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(MapPage), "draw"),
            postfix: new HarmonyMethod(typeof(MapPagePatch), nameof(DrawPatch))
        );
    }

    private static void DrawPatch(MapPage __instance)
    {
        try
        {
            HandleKeyBinds(__instance);
            MainClass.ScreenReader.SayWithMenuChecker(__instance.hoverText, true);
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in map page patch:\n{e.Message}\n{e.StackTrace}");
            throw;
        }
    }

    private static void HandleKeyBinds(MapPage __instance)
    {
        if (_isCycling || __instance.points.Count == 0)
            return;

        MapDirection? direction = null;
        if (MainClass.Config.MapPageMoveRightKey.JustPressed())
            direction = MapDirection.Right;
        else if (MainClass.Config.MapPageMoveLeftKey.JustPressed())
            direction = MapDirection.Left;
        else if (MainClass.Config.MapPageMoveUpKey.JustPressed())
            direction = MapDirection.Up;
        else if (MainClass.Config.MapPageMoveDownKey.JustPressed())
            direction = MapDirection.Down;

        if (direction == null)
            return;

        _isCycling = true;
        MoveToNearestLocation(__instance, direction.Value);
        Task.Delay(200).ContinueWith(_ => { _isCycling = false; });
    }

    private static void MoveToNearestLocation(MapPage __instance, MapDirection direction)
    {
        List<ClickableComponent> locations = __instance.points.Values.ToList();
        if (locations.Count == 0)
            return;

        ClickableComponent current = GetCurrentLocation(__instance, locations);
        ClickableComponent? target = FindNearestInDirection(current, locations, direction)
            ?? FindWrapAroundLocation(current, locations, direction);

        if (target == null)
            return;

        __instance.setCurrentlySnappedComponentTo(target.myID);
        target.snapMouseCursorToCenter();
        __instance.performHoverAction(Game1.getMouseX(true), Game1.getMouseY(true));
    }

    private static ClickableComponent GetCurrentLocation(MapPage __instance, List<ClickableComponent> locations)
    {
        ClickableComponent? snapped = __instance.currentlySnappedComponent;
        if (snapped != null)
        {
            ClickableComponent? match = locations.FirstOrDefault(point => point.myID == snapped.myID);
            if (match != null)
                return match;
        }

        int mouseX = Game1.getMouseX(true);
        int mouseY = Game1.getMouseY(true);
        ClickableComponent? hovered = locations.FirstOrDefault(point => point.containsPoint(mouseX, mouseY));
        if (hovered != null)
            return hovered;

        // Fallback: closest point to the mouse, so the first keypress always has an origin.
        return locations
            .OrderBy(point => DistanceSquared(GetCenter(point), new Vector2(mouseX, mouseY)))
            .First();
    }

    private static ClickableComponent? FindNearestInDirection(
        ClickableComponent current,
        List<ClickableComponent> locations,
        MapDirection direction)
    {
        Vector2 origin = GetCenter(current);
        ClickableComponent? best = null;
        float bestScore = float.MaxValue;

        foreach (ClickableComponent candidate in locations)
        {
            if (candidate.myID == current.myID)
                continue;

            Vector2 delta = GetCenter(candidate) - origin;
            if (!TryScoreCandidate(delta, direction, out float score))
                continue;

            if (score < bestScore)
            {
                bestScore = score;
                best = candidate;
            }
        }

        return best;
    }

    private static ClickableComponent? FindWrapAroundLocation(
        ClickableComponent current,
        List<ClickableComponent> locations,
        MapDirection direction)
    {
        Vector2 origin = GetCenter(current);

        return direction switch
        {
            MapDirection.Right => locations
                .Where(point => point.myID != current.myID)
                .OrderBy(point => GetCenter(point).X)
                .ThenBy(point => Math.Abs(GetCenter(point).Y - origin.Y))
                .FirstOrDefault(),
            MapDirection.Left => locations
                .Where(point => point.myID != current.myID)
                .OrderByDescending(point => GetCenter(point).X)
                .ThenBy(point => Math.Abs(GetCenter(point).Y - origin.Y))
                .FirstOrDefault(),
            MapDirection.Down => locations
                .Where(point => point.myID != current.myID)
                .OrderBy(point => GetCenter(point).Y)
                .ThenBy(point => Math.Abs(GetCenter(point).X - origin.X))
                .FirstOrDefault(),
            MapDirection.Up => locations
                .Where(point => point.myID != current.myID)
                .OrderByDescending(point => GetCenter(point).Y)
                .ThenBy(point => Math.Abs(GetCenter(point).X - origin.X))
                .FirstOrDefault(),
            _ => null
        };
    }

    private static bool TryScoreCandidate(Vector2 delta, MapDirection direction, out float score)
    {
        // Prefer candidates mostly aligned with the pressed direction.
        // Secondary axis is penalized so diagonal jumps feel less random.
        const float secondaryPenalty = 2.5f;
        float primary;
        float secondary;

        switch (direction)
        {
            case MapDirection.Right:
                primary = delta.X;
                secondary = Math.Abs(delta.Y);
                break;
            case MapDirection.Left:
                primary = -delta.X;
                secondary = Math.Abs(delta.Y);
                break;
            case MapDirection.Down:
                primary = delta.Y;
                secondary = Math.Abs(delta.X);
                break;
            case MapDirection.Up:
                primary = -delta.Y;
                secondary = Math.Abs(delta.X);
                break;
            default:
                score = float.MaxValue;
                return false;
        }

        if (primary <= 0)
        {
            score = float.MaxValue;
            return false;
        }

        score = primary + secondary * secondaryPenalty;
        return true;
    }

    private static Vector2 GetCenter(ClickableComponent component)
        => new(component.bounds.Center.X, component.bounds.Center.Y);

    private static float DistanceSquared(Vector2 a, Vector2 b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        return dx * dx + dy * dy;
    }

    internal static void Cleanup()
    {
        _isCycling = false;
    }
}
