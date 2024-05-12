using StardewValley;
using StardewValley.GameData.Objects;
using System.Text.RegularExpressions;

namespace stardew_access.Utils;

public static class ObjectUtils
{
    public static ObjectData? GetObjectById(string? objectId)
    {
        if (string.IsNullOrWhiteSpace(objectId)) return null;
        if (objectId.StartsWith("(BC)")) return null;

        if (objectId.StartsWith("(O)"))
            objectId = objectId[3..]; // "(O)" is 3 characters long, so start after it

        if (!Game1.objectData.TryGetValue(objectId, out ObjectData? objectInfo) || objectInfo is null)
            Log.Error($"Object ID {objectId} does not exist.", once: true);

        return objectInfo;
    }

    public static ObjectData? GetObjectById(int objectId) => GetObjectById(objectId.ToString());

    // Standard version for string ids
    public static string NormalizeQualifiedItemID(string qualifiedItemID, bool indexOnly = false)
    {
        // Regex pattern to match valid inputs and extract important bits
        string pattern = @"^(\(([A-Z]+)\)|([A-Z]+))?((\-?[\d]+)|([\w]+))$";
        Match match = Regex.Match(qualifiedItemID, pattern);
        if (match.Success)
        {
            // Grab whichever is the valid index string
            string index = match.Groups[5].Success ? match.Groups[5].Value : match.Groups[6].Value;
            if (indexOnly || match.Groups[1].Value == "")
                return index;

            string category = match.Groups[2].Value != "" ? match.Groups[2].Value : match.Groups[3].Value;
            return $"{category}{index}";
        }
        else
        {
            // Throw an exception if the match fails
            throw new ArgumentException("Input string format is invalid.", nameof(qualifiedItemID));
        }
    }

    // Overload for integer index
    public static string NormalizeQualifiedItemID(int index, bool indexOnly = false)
    {
        // Convert integer to string and (potentially) call the string overload
        return indexOnly ? index.ToString() :NormalizeQualifiedItemID(index.ToString());
    }
}
