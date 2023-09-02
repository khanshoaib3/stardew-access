using Shockah.ProjectFluent;
using StardewModdingAPI;

namespace stardew_access.Translation
{
    public enum TranslationCategory
    {
        MENU,
        DEFAULT
    }

    internal class Translator
    {
        private static Translator? _instance = null;
        private IFluent<string>? DefaultEntries { get; set; }
        private IFluent<string>? MenuEntries { get; set; }
        private static readonly object _instanceLock = new();
        private Translator() { }
        internal CustomFluentFunctions? CustomFunctions;

        public static Translator Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    _instance ??= new Translator();
                    return _instance;
                }
            }
        }

        public void Initialize(IManifest modManifest)
        {
#if DEBUG
            Log.Debug("Initializing FluentApi");
#endif
            IFluentApi? fluentApi = MainClass.ModHelper?.ModRegistry.GetApi<IFluentApi>("Shockah.ProjectFluent");
            if (fluentApi != null)
            {
                DefaultEntries = fluentApi.GetLocalizationsForCurrentLocale(modManifest);
                MenuEntries = fluentApi.GetLocalizationsForCurrentLocale(modManifest, "menu");
#if DEBUG
                Log.Verbose("Registering custom fluent functions");
#endif
                CustomFunctions = new CustomFluentFunctions(modManifest, fluentApi);
                foreach (var (mod, name, function) in CustomFunctions.GetAll())
                {
                    fluentApi.RegisterFunction(mod, name, function);
#if DEBUG
                    Log.Verbose($"Registered function \"{name}\"");
#endif
                }
            }
            else
            {
                Log.Error("Unable to initialize fluent api", true);
            }
        }

        public string Translate(string translationKey, TranslationCategory translationCategory = TranslationCategory.DEFAULT, bool disableWarning = false)
        {
            IFluent<string>? requiredEntries = GetEntriesFromCategory(translationCategory);

            if (requiredEntries == null)
            {
                Log.Error("Fluent not initialized!", true);
                return translationKey;
            }

            if (requiredEntries.ContainsKey(translationKey))
            {
#if DEBUG
                Log.Verbose($"Translate: found translation key \"{translationKey}\"", true);
#endif
                return requiredEntries.Get(translationKey);
            }

            if (!disableWarning)
            {
                Log.Debug($"No translation available for key: {translationKey}", true);
            }

            return translationKey;
        }

        public string Translate(string translationKey, object? tokens, TranslationCategory translationCategory = TranslationCategory.DEFAULT, bool disableWarning = false)
        {
            IFluent<string>? requiredEntries = GetEntriesFromCategory(translationCategory);

            if (requiredEntries == null)
            {
                Log.Error("Fluent not initialized!", true);
                return translationKey;
            }

            if (requiredEntries.ContainsKey(translationKey))
            {
#if DEBUG
                if (tokens is Dictionary<string, object> dictTokens)
                {
                    Log.Verbose($"Translate with tokens: found translation key \"{translationKey}\" with tokens: {string.Join(", ", dictTokens.Select(kv => $"{kv.Key}: {kv.Value}"))}", true);
                }
                else
                {
                    var tokenStr = tokens is not null ? string.Join(", ", tokens.GetType().GetProperties().Select(prop => $"{prop.Name}: {prop.GetValue(tokens)}")) : "null";
                    Log.Verbose($"Translate with tokens: found translation key \"{translationKey}\" with tokens: {tokenStr}", true);
                }
#endif
                var result = requiredEntries.Get(translationKey, tokens);
#if DEBUG
                Log.Verbose($"Translated to: {result}", true);
#endif
                return result;
            }

            if (!disableWarning)
            {
                Log.Debug($"No translation available for key: {translationKey}", true);
            }

            return translationKey;
        }

        private IFluent<string>? GetEntriesFromCategory(TranslationCategory translationCategory) => translationCategory switch
        {
            TranslationCategory.MENU => MenuEntries,
            TranslationCategory.DEFAULT => DefaultEntries,
            _ => null
        };
    }
}
