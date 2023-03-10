namespace stardew_access.Patches
{
    internal class InstanceGamePatch
    {
        internal static void ExitPatch()
        {
            if (MainClass.ScreenReader != null)
                MainClass.ScreenReader.CloseScreenReader();
        }
    }
}
