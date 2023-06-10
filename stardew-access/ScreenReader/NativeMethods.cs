using System.Runtime.InteropServices;

namespace stardew_access.ScreenReader
{
    public static class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool SetDllDirectory(string lpPathName);
    }
}