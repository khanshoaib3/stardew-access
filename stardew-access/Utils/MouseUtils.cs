using StardewValley;

namespace stardew_access.Utils
{
    internal static class MouseUtils
    {
        private static bool wasRecentlyClicked = false;

        internal static async void SimulateMouseClicksWithDelay(Action<int, int>? leftClickHandler, Action<int, int>? rightClickHandler)
        {
            if (!wasRecentlyClicked && SimulateMouseClicks(leftClickHandler, rightClickHandler))
            {
                wasRecentlyClicked = true;
                await Task.Delay(300);
                wasRecentlyClicked = false;
            }
        }

        internal static bool SimulateMouseClicks(Action<int, int>? leftClickHandler, Action<int, int>? rightClickHandler)
        {
            int mouseX = Game1.getMouseX(true);
            int mouseY = Game1.getMouseY(true);

            if (leftClickHandler != null && (MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed()))
            {
#if DEBUG
                Log.Debug("Simulating left mouse click");
#endif
                leftClickHandler(mouseX, mouseY);
                return true;
            }
            else if (rightClickHandler != null && (MainClass.Config.RightClickMainKey.JustPressed() || MainClass.Config.RightClickAlternateKey.JustPressed()))
            {
#if DEBUG
                Log.Debug("Simulating right mouse click");
#endif
                rightClickHandler(mouseX, mouseY);
                return true;
            }

            return false;
        }
    }
}