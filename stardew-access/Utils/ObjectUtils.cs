using StardewValley;
using StardewValley.GameData.Objects;

namespace stardew_access.Utils;

public static class ObjectUtils
{
    public static ObjectData GetObjectById(string objectId)
    {
        if (objectId.StartsWith("(O)"))
            objectId = objectId.Substring(3); // "(O)" is 3 characters long, so start after it
        if (!Game1.objectData.TryGetValue(objectId, out ObjectData? objectInfo) || objectInfo is null)
            throw new ArgumentException($"Object ID {objectId} does not exist.");

        return objectInfo;

    }

    public static ObjectData GetObjectById(int objectId) => GetObjectById(objectId.ToString());
}