using StardewValley;

namespace stardew_access.Utils
{
    public static class ObjectUtils
    {
        public static string GetObjectById(int objectId, int? field = 0)
        {
            if (objectId == -1)
            {
                return "";
            }

            if (Game1.objectInformation.TryGetValue(objectId, out string? objectInfo))
            {
                if (field == null)
                {
                    return objectInfo;
                }

                string[] objectFields = objectInfo.Split('/');
                return objectFields.Length > field ? objectFields[field.Value] : string.Empty;
            }
            else
            {
                throw new ArgumentException($"Object ID {objectId} does not exist.");
            }
        }
    }
}
