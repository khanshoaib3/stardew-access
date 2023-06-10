using System.Runtime.InteropServices;

namespace stardew_access.ScreenReader
{
    public class ScreenReaderController
    {
        public static IScreenReader Initialize()
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
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                ScreenReaderMac screenReaderMac = new ScreenReaderMac();
                screenReaderMac.InitializeScreenReader();

                ScreenReader = screenReaderMac;
            }
            else
            {
                ScreenReader.InitializeScreenReader();
            }

            return ScreenReader;
        }
    }
}