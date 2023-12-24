using Shockah.ProjectFluent;
using stardew_access.Utils;
using StardewModdingAPI;
using System;
using System.Reflection;
using System.Text;
namespace stardew_access.Translation
{
    internal class CustomFluentFunctions
    {
        private IManifest ModManifest { get; set; }
        private IFluentApi FluentApi { get; set; }
        private static readonly Dictionary<string, Type> languageHelpers = [];
        private static ILanguageHelper? currentLanguageHelper = null;

        internal CustomFluentFunctions(IManifest ModManifest, IFluentApi FluentApi)
        {
            this.ModManifest = ModManifest;
            this.FluentApi = FluentApi;
        }

        internal IEnumerable<(IManifest mod, string name, FluentFunction function)> GetAll()
        {
            yield return (ModManifest, "EMPTYSTRING", EmptyString);
            yield return (ModManifest, "SIGNOFNUMBER", SignOfNumber);
            yield return (ModManifest, "PLURALIZE", Pluralize);
        }

        /// <summary>
        /// Returns an empty string which can be used in places where a character is required.
        /// <para>
        /// For example: {$value ->
        ///     [0] {EMPTYSTRING()}
        ///     *[other] {$value}
        /// }
        /// In the above example, if the value given is zero,
        /// it will print nothing and it will print any other value just fine.
        /// </para>
        /// </summary>
        /// <returns>An empty string</returns>
        private IFluentFunctionValue EmptyString(
            IGameLocale locale,
            IManifest mod,
            IReadOnlyList<IFluentFunctionValue> positionalArguments,
            IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments
        )
        {
            return FluentApi.CreateStringValue("");
        }

        /// <summary>
        /// Determines the sign of a number and returns the corresponding value.
        /// </summary>
        /// <param name="positionalArguments">(At 0th index) The number to check for the sign</param>
        /// <returns><para>
        ///     A string value representing the sign of the number.
        ///     Possible return values:
        ///     - "negative" if the number is negative
        ///     - "positive" if the number is positive
        ///     - "zero" if the number is zero
        ///     - "unknown" if no positional arguments are provided or if the provided argument is not a valid number
        /// 
        ///     Note: Returning an integer value instead of a string value would prevent proper distinction in the Fluent file (.ftl) (only for custom functions).
        /// </para></returns>
        private IFluentFunctionValue SignOfNumber(
            IGameLocale locale,
            IManifest mod,
            IReadOnlyList<IFluentFunctionValue> positionalArguments,
            IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments
        )
        {
            if (positionalArguments.Count <= 0)
                return FluentApi.CreateStringValue("unknown");

            var value = positionalArguments[0].AsFloatOrNull();
            if (value != null)
            {
                if (value > 0)
                {
                    return FluentApi.CreateStringValue("positive");
                }
                if (value < 0)
                {
                    return FluentApi.CreateStringValue("negative");
                }

                return FluentApi.CreateStringValue("zero");
            }

            return FluentApi.CreateStringValue("unknown");
        }

        internal static void RegisterLanguageHelper(string locale, Type helperType)
        {
            var implementsInterface = typeof(ILanguageHelper).IsAssignableFrom(helperType);
            var inheritsFromBase = MiscUtils.InheritsFrom(helperType, typeof(LanguageHelperBase));
            Log.Verbose($"Registered language helper for locale '{locale}': Type: {helperType.Name}, Implements ILanguageHelper: {implementsInterface}, Inherits from LanguageHelperBase: {inheritsFromBase}");
            if (!implementsInterface)
            {
                throw new ArgumentException("Provided type does not implement ILanguageHelper.");
            }
            languageHelpers[locale] = helperType;
        }

        internal void LoadLanguageHelper()
        {
            var currentLocale = FluentApi.CurrentLocale;
            var defaultLocale = FluentApi.DefaultLocale;
            Log.Debug($"CurrentLocale is {currentLocale}; DefaultLocale is {defaultLocale}.");

            // Local function to load a helper for a locale
            static bool     TryLoadHelperForLocale(string? locale)
            {
                // This should never happen
                if (locale is null) throw new ArgumentNullException(nameof(locale), "Locale cannot be null.");
                // Attempt to load locale-specific helper (e.g., "en-us")
                if (languageHelpers.TryGetValue(locale, out Type? helperType) && helperType != null)
                {
                    currentLanguageHelper = (ILanguageHelper?)Activator.CreateInstance(helperType);
                    #if DEBUG
                    Log.Verbose($"Loaded LanguageHelper for {locale}");
                    #endif
                    return true;
                }

                // Attempt to load general helper (e.g., "en") if specific one wasn't found
                string generalLocale = locale.Split('-')[0];
                if (generalLocale != locale && languageHelpers.TryGetValue(generalLocale, out helperType))
                {
                    currentLanguageHelper = (ILanguageHelper?)Activator.CreateInstance(helperType);
                    #if DEBUG
                    Log.Verbose($"Loaded LanguageHelper for {generalLocale}");
                    #endif
                    return true;
                }

                #if DEBUG
                Log.Verbose($"Failed to loada helper for {locale} or {generalLocale}");
                #endif
                return false;
            }

            // Try to load helper for current locale
            if (TryLoadHelperForLocale(currentLocale.ToString()))
            {
                return;
            }

            // If that fails, try to load helper for default locale
            if (defaultLocale is not null && TryLoadHelperForLocale(defaultLocale.ToString()))
            {
                return;
            }

            throw new NotSupportedException($"No language helper registered for either the current locale ({currentLocale}) or the default locale ({defaultLocale}).");
        }

        public virtual IFluentFunctionValue Pluralize(
            IGameLocale locale,
            IManifest mod,
            IReadOnlyList<IFluentFunctionValue> positionalArguments,
            IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments
        )
        {
            if (currentLanguageHelper == null)
            {
                throw new InvalidOperationException("Language helper not loaded. Ensure LoadLanguageHelper is called before invoking pluralization.");
            }
            
            if (positionalArguments[1].AsString() == "FluentErrType") return FluentApi.CreateStringValue("");
            return currentLanguageHelper.Pluralize(locale, mod, positionalArguments, namedArguments);
        }
    }
}