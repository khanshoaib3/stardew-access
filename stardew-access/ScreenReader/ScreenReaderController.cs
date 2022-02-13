using System.Runtime.InteropServices;

namespace stardew_access.ScreenReader
{
    public class ScreenReaderController
    {
        public IScreenReader Initialize()
        {
            IScreenReader ScreenReader = new ScreenReaderWindows(); // Default is windows

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ScreenReaderWindows screenReaderWindows = new ScreenReaderWindows();
                screenReaderWindows.InitializeScreenReader();

                ScreenReader = screenReaderWindows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                ScreenReaderLinux screenReaderLinux = new ScreenReaderLinux();
                screenReaderLinux.InitializeScreenReader();

                ScreenReader = screenReaderLinux;
            }
            else
            {
                ScreenReader.InitializeScreenReader();
            }

            return ScreenReader;
        }
    }
}