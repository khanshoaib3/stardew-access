using System.Globalization;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;

namespace stardew_access.Integrations
{
    /// <summary>A delegate implementing a custom function, available to Project Fluent translations.</summary>
    /// <param name="locale">The locale the translation is being provided for.</param>
    /// <param name="mod">The mod the translation is being provided for.</param>
    /// <param name="positionalArguments">A list of positional arguments passed to the function.</param>
    /// <param name="namedArguments">A list of named arguments passed to the function.</param>
    /// <returns>The resulting value of the function.</returns>
    public delegate IFluentFunctionValue FluentFunction(
        IGameLocale locale,
        IManifest mod,
        IReadOnlyList<IFluentFunctionValue> positionalArguments,
        IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments
    );

    /// <summary>An instance representing a specific game locale, be it a built-in one or a mod-provided one.</summary>
    public interface IGameLocale
    {
        /// <summary>The locale code of this locale (for example, <c>en-US</c>).</summary>
        string LocaleCode { get; }

        /// <summary>The <see cref="System.Globalization.CultureInfo"/> for this locale.</summary>
        CultureInfo CultureInfo => new(LocaleCode);
    }

    /// <summary>The Project Fluent API which other mods can access.</summary>
    public interface IFluentApi
    {
        /// <summary>Get an <see cref="IFluent{}"/> instance that allows retrieving Project Fluent translations for the current locale.</summary>
        /// <remarks>The returned instance's locale will change automatically if the <see cref="CurrentLocale"/> changes.</remarks>
        /// <param name="mod">The mod for which to retrieve the translations.</param>
        /// <param name="file">An optional file name to retrieve the translations from.</param>
        IFluent<string> GetLocalizationsForCurrentLocale(IManifest mod, string? file = null);


		/// <summary>The default locale of the game (en-US).</summary>
		IGameLocale DefaultLocale { get; }

		/// <summary>The current locale of the game.</summary>
		IGameLocale CurrentLocale { get; }

        #region Custom Fluent functions

        /// <summary>Create a <see cref="string"/> value usable with Project Fluent functions.</summary>
        /// <param name="value">The <see cref="string"/> value.</param>
        IFluentFunctionValue CreateStringValue(string value);

        /// <summary>Create a <see cref="int"/> value usable with Project Fluent functions.</summary>
        /// <param name="value">The <see cref="int"/> value.</param>
        IFluentFunctionValue CreateIntValue(int value);

        /// <summary>Create a <see cref="long"/> value usable with Project Fluent functions.</summary>
        /// <param name="value">The <see cref="long"/> value.</param>
        IFluentFunctionValue CreateLongValue(long value);

        /// <summary>Create a <see cref="float"/> value usable with Project Fluent functions.</summary>
        /// <param name="value">The <see cref="float"/> value.</param>
        IFluentFunctionValue CreateFloatValue(float value);

        /// <summary>Create a <see cref="double"/> value usable with Project Fluent functions.</summary>
        /// <param name="value">The <see cref="double"/> value.</param>
        IFluentFunctionValue CreateDoubleValue(double value);

        /// <summary>Register a new function for the Project Fluent translations.</summary>
        /// <remarks>
        /// The registered function will be available by the translations in the form of:
        /// <list type="bullet">
        /// <item><c>CAPITALIZED_UNDERSCORED_MOD_ID_AND_THEN_THE_FUNCTION_NAME</c> in all contexts</item>
        /// <item><c>FUNCTION_NAME</c> in the <paramref name="mod"/> context.</item>
        /// </list>
        /// </remarks>
        /// <param name="mod">The mod you want to register the function for. Keep in mind other mods can also access the function, if they provide the fully qualified name.</param>
        /// <param name="name">The name of the function.<br/>Fluent function names can only contain uppercase letters, digits, and the <c>_</c> and <c>-</c> characters. They must also start with an uppercase letter.</param>
        /// <param name="function">The function to register.</param>
        void RegisterFunction(IManifest mod, string name, FluentFunction function);

        /// <summary>Unregister a Project Fluent translation function.</summary>
        /// <param name="mod">The mod you want to unregister the function for.</param>
        /// <param name="name">The name of the function.<br/>Fluent function names can only contain uppercase letters, digits, and the <c>_</c> and <c>-</c> characters. They must also start with an uppercase letter.</param>
        void UnregisterFunction(IManifest mod, string name);

        #endregion
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

    /// <summary>A value usable with the Project Fluent functions.</summary>
    /// <remarks>
    /// Although this interface is exposed in the API, and some methods take those as values,
    /// you should almost never create custom types implementing this interface (unless you know what you are doing).
    /// </remarks>
    public interface IFluentFunctionValue
    {
        /// <summary>Returns the value as the underlying Project Fluent library type. This is an implementation detail, and should not be used, unless you know what you are doing.</summary>
        object /* IFluentType */
        AsFluentValue();

        /// <summary>Returns the value as a <see cref="string"/>.</summary>
        string AsString();

        /// <summary>Returns the value as an <see cref="int"/>, or <c>null</c> if it cannot be converted.</summary>
        int? AsIntOrNull();

        /// <summary>Returns the value as a <see cref="long"/>, or <c>null</c> if it cannot be converted.</summary>
        long? AsLongOrNull();

        /// <summary>Returns the value as a <see cref="float"/>, or <c>null</c> if it cannot be converted.</summary>
        float? AsFloatOrNull();

        /// <summary>Returns the value as a <see cref="double"/>, or <c>null</c> if it cannot be converted.</summary>
        double? AsDoubleOrNull();
    }
}
