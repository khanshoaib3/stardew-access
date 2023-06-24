namespace stardew_access.Patches
{
    internal class InstanceGamePatch
    {
        internal static void ExitPatch()
        {
            MainClass.ScreenReader?.CloseScreenReader();
        }
    }
}
