using StardewModdingAPI;
namespace stardew_access
{
    /// <summary>
    /// Provides logging functionality.
    /// </summary>
    internal static class Log
    {
        private static IMonitor? _monitor;
        private static IMonitor Monitor
        {
            get => _monitor ?? throw new InvalidOperationException("Monitor has not been initialized.");
            set => _monitor = value;
        }

        /// <summary>
        /// Initializes the logger with the provided monitor.
        /// </summary>
        /// <param name="monitor">The monitor to use for logging.</param>
        internal static void Init(IMonitor monitor)
        {
            Monitor = monitor;
        }

        /// <summary>
        /// Logs a message with the specified log level.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="logLevel">The log level of the message.</param>
        /// <param name="once">If true, the message will only be logged once per session.</param>
        private static void LogMessage(string message, LogLevel logLevel, bool once = false)
        {
            if (once)
            {
                Monitor.LogOnce(message, logLevel);
            }
            else
            {
                Monitor.Log(message, logLevel);
            }
        }

        /// <summary>
        /// Logs a trace level message that appears in the log (and console in the for developers version) only if SMAPI's verbose logging option is enabled. Meant for diagnostic information needed to troubleshoot, which doesn't need to be logged in most cases. Players can enable verbose logging by editing the smapi-internal/StardewModdingAPI.config.json file.
        /// </summary>
        /// <param name="message">The message to log.</param>
        internal static void Verbose(string message)
        {
            Monitor.VerboseLog(message);
        }

        /// <summary>
        /// Logs tracing info intended for developers, usually troubleshooting details that are useful when someone sends you their error log. Trace messages won't appear in the console window by default (unless you have the "SMAPI for developers" version), though they're always written to the log file.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="once">If true, the message will only be logged once per session.</param>
        internal static void Trace(string message, bool once = false)
        {
            LogMessage(message, LogLevel.Trace, once);
        }

        /// <summary>
        /// Logs troubleshooting info that may be relevant to the player.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="once">If true, the message will only be logged once per session.</param>
        internal static void Debug(string message, bool once = false)
        {
            LogMessage(message, LogLevel.Debug, once);
        }

        /// <summary>
        /// Logs info relevant to the player. This should be used judiciously.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="once">If true, the message will only be logged once per session.</param>
        internal static void Info(string message, bool once = false)
        {
            LogMessage(message, LogLevel.Info, once);
        }

        /// <summary>
        /// Logs an issue the player should be aware of. This should be used rarely.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="once">If true, the message will only be logged once per session.</param>
        internal static void Warn(string message, bool once = false)
        {
            LogMessage(message, LogLevel.Warn, once);
        }

        /// <summary>
        /// Logs a message indicating something went wrong.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="once">If true, the message will only be logged once per session.</param>
        internal static void Error(string message, bool once = false)
        {
            LogMessage(message, LogLevel.Error, once);
        }

        /// <summary>
        /// Logs important information to highlight for the player when player action is needed (e.g. new version available). This should be used rarely to avoid alert fatigue.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="once">If true, the message will only be logged once per session.</param>
        internal static void Alert(string message, bool once = false)
        {
            LogMessage(message, LogLevel.Alert, once);
        }
    }
}
