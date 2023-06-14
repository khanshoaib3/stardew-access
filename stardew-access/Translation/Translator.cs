using Shockah.ProjectFluent;
using StardewModdingAPI;

namespace stardew_access
{
    internal class Translator
    {
        private static Translator? instance = null;
        private IFluent<string> Fluent { get; set; } = null!;

        private Translator() { }

        public static Translator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Translator();
                }

                return instance;
            }
        }

        public void Initialize(IManifest modManifest)
        {
            IFluentApi? fluentApi = null;
            if (MainClass.ModHelper != null)
            {
                fluentApi = MainClass.ModHelper.ModRegistry.GetApi<IFluentApi>("Shockah.ProjectFluent");
            }

            if (fluentApi != null)
            {
                Fluent = fluentApi.GetLocalizationsForCurrentLocale(modManifest);
                foreach ( var customFunction in new CustomFluentFunctions(modManifest, fluentApi).GetAll())
                {
                    fluentApi.RegisterFunction( customFunction.mod, customFunction.name, customFunction.function);
                }
            }
            else
            {
                MainClass.ErrorLog("Unable to initialize fluent api");
            }
        }

        public string Translate(string translationKey)
        {
            if (Fluent != null)
                return Fluent.Get(translationKey);

            MainClass.ErrorLog("Fluent not initialized!");

            return translationKey;
        }

        public string Translate(string translationKey, object? tokens)
        {
            if (Fluent != null)
                return Fluent.Get(translationKey, tokens);

            MainClass.ErrorLog("Fluent not initialized!");

            return translationKey;
        }
    }
}
