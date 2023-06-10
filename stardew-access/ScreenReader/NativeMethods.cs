using System.Runtime.InteropServices;

namespace stardew_access.ScreenReader
{
    public static class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool SetDllDirectory(string lpPathName);
    }
}