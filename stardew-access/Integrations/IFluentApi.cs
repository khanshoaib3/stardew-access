using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;

namespace stardew_access.Integrations
{
    /// <summary>The Project Fluent API which other mods can access.</summary>
    public interface IFluentApi
    {
                /// <summary>Get an <see cref="IFluent{}"/> instance that allows retrieving Project Fluent translations for the current locale.</summary>
        /// <remarks>The returned instance's locale will change automatically if the <see cref="CurrentLocale"/> changes.</remarks>
        /// <param name="mod">The mod for which to retrieve the translations.</param>
        /// <param name="file">An optional file name to retrieve the translations from.</param>
        IFluent<string> GetLocalizationsForCurrentLocale(IManifest mod, string? file = null);
    }

    /// <summary>A type allowing access to Project Fluent translations.</summary>
    /// <typeparam name="Key">The type of values this instance allows retrieving translations for.</typeparam>
    public interface IFluent<Key>
    {
        /// <summary>Returns whether a given key has a translation provided.</summary>
        /// <param name="key">The key to retrieve a translation for.</param>
        bool ContainsKey(Key key);

        /// <summary>Returns a translation for a given key.</summary>
        /// <param name="key">The key to retrieve a translation for.</param>
        string Get(Key key) => Get(key, null);

        /// <summary>Returns a translation for a given key.</summary>
        /// <param name="key">The key to retrieve a translation for.</param>
        /// <param name="tokens">An object containing token key/value pairs. This can be an anonymous object (like <c>new { value = 42, name = "Cranberries" }</c>), a dictionary, or a class instance.</param>
        string Get(Key key, object? tokens);

        /// <summary>Returns a translation for a given key.</summary>
        /// <param name="key">The key to retrieve a translation for.</param>
        string this[Key key] => Get(key, null);
    }
}
