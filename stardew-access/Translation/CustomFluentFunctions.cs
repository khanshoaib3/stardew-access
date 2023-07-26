using Shockah.ProjectFluent;
using StardewModdingAPI;

namespace stardew_access
{
    internal class CustomFluentFunctions
    {
        private IManifest ModManifest { get; set; }
        private IFluentApi FluentApi { get; set; }

        internal CustomFluentFunctions(IManifest ModManifest, IFluentApi FluentApi)
        {
            this.ModManifest = ModManifest;
            this.FluentApi = FluentApi;
        }

        internal IEnumerable<(IManifest mod, string name, FluentFunction function)> GetAll()
        {
            yield return (ModManifest, "EMPTYSTRING", EmptyString);
            yield return (ModManifest, "SIGNOFNUMBER", SignOfNumber);
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
    }
}
