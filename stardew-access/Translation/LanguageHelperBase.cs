using Shockah.ProjectFluent;
using stardew_access.Utils;
using StardewModdingAPI;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace stardew_access.Translation
{
    public abstract class LanguageHelperBase : ILanguageHelper
    {
        private const string HelpersDir = "assets/translation";
        protected Dictionary<string, object?>? LocaleData;
        protected Dictionary<string, object?>? LocaleRegionalData;
        protected readonly ConcurrentDictionary<(int?, string), string> _pluralizationCache = new();

        internal readonly string Locale;
        protected IFluentApi? FluentApi { get; set; }

        protected LanguageHelperBase(string locale)
        {
            this.FluentApi ??= MainClass.ModHelper!.ModRegistry.GetApi<IFluentApi>("Shockah.ProjectFluent");
            this.Locale = locale;
            LoadLocaleData();
        }

        private void LoadLocaleData()
        {
            string baseLocale = Locale.Split('-')[0].ToLower();

            // Load base locale data
            string filename = $"{baseLocale}.json";
            if (JsonLoader.TryLoadJsonAsDictionary(filename, out Dictionary<string, object?>? result, HelpersDir))
            {
                this.LocaleData = result;
            }
            else
            {
                throw new FileNotFoundException($"Could not load JSON file: {filename}");
            }

            // Load regional locale data if it exists
            if (Locale.Contains('-'))
            {
                string regionalFilename = $"{Locale.ToLower()}.json";
                if (JsonLoader.TryLoadJsonAsDictionary(regionalFilename, out result, HelpersDir))
                {
                    this.LocaleRegionalData = result;
                }
                else
                {
                    // Log a debug message if the regional file is not found
                    Log.Debug($"Regional JSON file not found: {regionalFilename}. Continuing without regional data.");
                    this.LocaleRegionalData = null; // No regional data available
                }
            }
        }

        public virtual (int?, string) GetCacheKey(int? count, string word)
        {
            // By default, don't consider the count in the cache key
            return (null, word);
        }

        // Other common methods can be added here

        public virtual IFluentFunctionValue Pluralize(
            IGameLocale locale,
            IManifest mod,
            IReadOnlyList<IFluentFunctionValue> positionalArguments,
            IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments
        )
        {
            #if DEBUG
            Log.Verbose("In LanguageHelperBase.Pluralize");
            #endif
            if (FluentApi is null) throw new InvalidOperationException($"{MethodBase.GetCurrentMethod()!.Name} cannot be called before FluentApi has been initialized.");

            // Check if the locale matches with the loaded language helper
            string passedLocale = locale.ToString()?[..2] ?? "";
            string loadedLocale = this.Locale[..2];

            if (!passedLocale.Equals(loadedLocale, StringComparison.OrdinalIgnoreCase))
            {
                Log.Warn($"Locale mismatch detected. Passed locale: {locale}, Loaded helper locale: {Locale}. Returning the original word.", true);
                return FluentApi.CreateStringValue(positionalArguments[1].AsString());
            }

            int? count = positionalArguments[0].AsIntOrNull();
            string word = positionalArguments[1].AsString();
            var key = GetCacheKey(count, word);

            if (!InPluralizationCache(key, out string? pluralizedWord) || pluralizedWord == null)
            {
                #if DEBUG
                Log.Verbose($"\"{word}\" not in cache; calling `Pluralize` for {Locale}");
                #endif
                pluralizedWord = Pluralize(count, word);
                _pluralizationCache.TryAdd(key, pluralizedWord);
                #if DEBUG
                Log.Trace($"Translated ({count}, {word}) and added result to cache.", true);
                #endif
            }

            var translation = FluentApi.CreateStringValue(pluralizedWord);
            #if DEBUG
            Log.Verbose($"LanguageHelperBase.Pluralize returning \"{translation}\"");
            #endif
            return translation;
        }

        public abstract string Pluralize(int? count, string word, string? prefix = null);

        public virtual bool InPluralizationCache((int? count, string word) key, out string? cached)
        {
            return _pluralizationCache.TryGetValue(key, out cached);
        }

        // Other abstract or virtual methods defined in ILanguageHelper
    }
}