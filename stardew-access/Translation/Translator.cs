using Shockah.ProjectFluent;
using StardewModdingAPI;

namespace stardew_access.Translation
{
    internal class Translator
    {
        private static Translator? instance = null;
        private IFluent<string>? Fluent { get; set; }
        private static readonly object instanceLock = new   ();
        private Translator() { }

        public static Translator Instance
        {
            get
            {
                lock (instanceLock)
                {
                    instance ??= new Translator();
                    return instance;
                }
            }
        }

        public void Initialize(IManifest modManifest)
        {
            IFluentApi? fluentApi = MainClass.ModHelper?.ModRegistry.GetApi<IFluentApi>("Shockah.ProjectFluent");

            if (fluentApi != null)
            {
                Fluent = fluentApi.GetLocalizationsForCurrentLocale(modManifest);
                foreach ( var (mod, name, function) in new CustomFluentFunctions(modManifest, fluentApi).GetAll())
                {
                    fluentApi.RegisterFunction( mod, name, function);
                }
            }
            else
            {
                Log.Error("Unable to initialize fluent api", true);
            }
        }

        public string Translate(string translationKey, bool disableWarning = false)
        {
            if (Fluent == null)
            {
                Log.Error("Fluent not initialized!", true);
                return translationKey;
            }

            if (Fluent.ContainsKey(translationKey))
            {
                return Fluent.Get(translationKey);
            }

            if (!disableWarning)
            {
                Log.Debug($"No translation available for key: {translationKey}", true);
            }

            return translationKey;
        }

        public string Translate(string translationKey, object? tokens, bool disableWarning = false)
        {
            if (Fluent == null)
            {
                Log.Error("Fluent not initialized!", true);
                return translationKey;
            }

            if (Fluent.ContainsKey(translationKey))
            {
                return Fluent.Get(translationKey, tokens);
            }

            if (!disableWarning)
            {
                Log.Debug($"No translation available for key: {translationKey}", true);
            }

            return translationKey;
        }
    }
}
