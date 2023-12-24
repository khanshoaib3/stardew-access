using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using stardew_access.Translation;
using stardew_access.Utils;
using System.Text;

namespace stardew_access.Tiles;

public class AccessibleTile : ConditionalBase
{
	public class JsonSerializerFormat
	{
		public string? NameOrTranslationKey { get; set; }
		public string? DynamicNameOrTranslationKey { get; set; }
		public int[]? X { get; set; }
		public int[]? Y { get; set; }
		public string? DynamicCoordinates { get; set; }
		public string? Category { get; set; }
		public string[]? WithMods { get; set; }
		public string[]? Conditions { get; set; }
		public bool? IsEvent { get; set; }
	}

	private readonly string? StaticNameOrTranslationKey;
	private readonly string? DynamicNameOrTranslationKey;
	private readonly Func<ConditionalBase, string>? DynamicNameOrTranslationKeyFunc;
	public string NameOrTranslationKey
	{
		get
		{
			if (DynamicNameOrTranslationKeyFunc != null)
			{
				return DynamicNameOrTranslationKeyFunc(this);
			}
			else if (StaticNameOrTranslationKey != null)
			{
				return StaticNameOrTranslationKey;
			}
			else
			{
				return "Unnamed";
			}
		}
	}

	private readonly Vector2[] StaticCoordinates;
	private readonly string? DynamicCoordinates;
	private readonly Func<ConditionalBase, Vector2[]>? DynamicCoordinatesFunc;
	public Vector2[] Coordinates
	{
		get
		{
			// Prioritize DynamicCoordinatesFunc if it exists
			if (DynamicCoordinatesFunc != null)
			{
				return DynamicCoordinatesFunc(this);
			}
			else
			{
				// Otherwise, return the StaticCoordinates array, empty or not
				return StaticCoordinates;
			}
		}
	}

	private readonly string[]? _withMods;
	private readonly string[]? _conditions;
	public readonly CATEGORY Category;
	public readonly bool IsEvent ;

	public (string NameOrTranslationKey, CATEGORY Category) NameAndCategory
	{
		get
		{
			return (
                Translator.Instance.Translate(
                    NameOrTranslationKey, 
                    translationCategory: TranslationCategory.StaticTiles,
                    disableWarning: true
                ),
                Category
            );
		}
	}

	// Super constructor
	public AccessibleTile(
		string? staticNameOrTranslationKey = null,
		string? dynamicNameOrTranslationKey = null,
		Vector2[]? staticCoordinates = null,
		string? dynamicCoordinates = null,
		CATEGORY? category = null,
		string[]? conditions = null,
		string[]? withMods = null,
		bool isEvent = false
	) : base(conditions, withMods)
	{
		// Error handling for invalid combinations
		if (staticNameOrTranslationKey == null && dynamicNameOrTranslationKey == null)
		{
			throw new ArgumentException("At least one of static or dynamic name must be provided.");
		}

		if (!(staticCoordinates == null ^ dynamicCoordinates == null))
		{
			throw new ArgumentException("Exactly one of static or dynamic coordinates must be provided.");
		}

		// Set properties
		StaticNameOrTranslationKey = staticNameOrTranslationKey;
		DynamicNameOrTranslationKey = dynamicNameOrTranslationKey;
		if (DynamicNameOrTranslationKey != null)
		{
			if (!AccessibleTileHelpers.TryGetNameHelper(DynamicNameOrTranslationKey, out DynamicNameOrTranslationKeyFunc))
			{
				throw new ArgumentException($"No helper function found for name or translation key: {DynamicNameOrTranslationKey}");
			}
		}

		StaticCoordinates = staticCoordinates ?? [];
		DynamicCoordinates = dynamicCoordinates;
		if (DynamicCoordinates != null)
		{
			if (!AccessibleTileHelpers.TryGetCoordinatesHelper(DynamicCoordinates, out DynamicCoordinatesFunc))
			{
				throw new ArgumentException($"No helper function found for coordinates: {DynamicCoordinates}");
			}
		}
		
		Category = category ?? CATEGORY.Others;
		IsEvent = isEvent;
		_withMods = withMods;
		_conditions = conditions;
	}

	public override string ToString()
	{
		StringBuilder sb = new();
		sb.Append("AccessibleTile { ");
		sb.Append($"{NameOrTranslationKey}:{Category} ");

		// Iterate through and append each coordinate pair
		if (Coordinates != null)
        {
            sb.Append("at (");
            bool first = true;
            foreach (Vector2 coordinate in Coordinates)
            {
                if (!first)
                {
                    sb.Append(", ");
                }
                first = false;
                sb.Append($"{coordinate.X}, {coordinate.Y}");
            }
            sb.Append(')');
        }

		// ... append other properties or fields ...
		sb.Append(" }");
		return sb.ToString();
	}

	public JsonSerializerFormat SerializableFormat => new()
	{
		NameOrTranslationKey = StaticNameOrTranslationKey,
		DynamicNameOrTranslationKey = DynamicNameOrTranslationKey, // Added here
		X = StaticCoordinates?.Select(v => (int)v.X).Distinct().ToArray(),
		Y = StaticCoordinates?.Select(v => (int)v.Y).Distinct().ToArray(),
		DynamicCoordinates = DynamicCoordinates,
		Category = Category?.Key,
		WithMods = _withMods,
		Conditions = _conditions,
		IsEvent = IsEvent
	};

    public static AccessibleTile FromJObject(JObject jObject)
    {
        // Attempt to get string values, handling nulls
		#pragma warning disable CA1507 // Use nameof to express symbol names
        string? staticNameOrTranslationKey = jObject["NameOrTranslationKey"]?.Value<string>();
		#pragma warning restore CA1507 // Use nameof to express symbol names
        string? dynamicNameOrTranslationKey = jObject["DynamicNameOrTranslationKey"]?.Value<string>();
        string? dynamicCoordinates = jObject["DynamicCoordinates"]?.Value<string>();
        string? categoryKey = jObject["Category"]?.Value<string>();

        // Convert string arrays to string[]?, handling nulls
        string[]? withMods = jObject["WithMods"]?.ToObject<string[]>();
        string[]? conditions = jObject["Conditions"]?.ToObject<string[]>();
        bool? isEvent = jObject["IsEvent"]?.Value<bool>();

        // Parse X and Y arrays, handling potential errors and nulls
        int[]? xValues = jObject["X"]?.ToObject<int[]>();
        int[]? yValues = jObject["Y"]?.ToObject<int[]>();

        // Generate all combinations of X and Y values
        Vector2[]? staticCoordinates = null;
        if (xValues != null && yValues != null)
        {
            int totalCoordinates = xValues.Length * yValues.Length;
            staticCoordinates = new Vector2[totalCoordinates];
            int index = 0;
            foreach (int y in yValues)
            {
                foreach (int x in xValues)
                {
                    staticCoordinates[index++] = new Vector2(x, y);
                }
            }
        }

        // CATEGORY handling
        CATEGORY category = (!string.IsNullOrEmpty(categoryKey))
                            ? CATEGORY.FromString(categoryKey)
                            : CATEGORY.Others;

        // Pass parsed values to the constructor, which will handle invalid combinations
        return new AccessibleTile(
            staticNameOrTranslationKey,
            dynamicNameOrTranslationKey,
            staticCoordinates,
            dynamicCoordinates,
            category,
            conditions,
            withMods,
            isEvent ?? false  // Default to false if IsEvent is null
        );
    }

}
