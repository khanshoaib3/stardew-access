namespace stardew_access.Features;

public class FeatureManager
{
    public static void UpdateAll()
    {
        List<FeatureBase> allFeatures = new()
        {
            ReadTile.Instance,
            Warnings.Instance,
        };

        foreach (FeatureBase feature in allFeatures)
        {
            try
            {
                feature.Update();
            }
            catch (Exception e)
            {
                Log.Error( $"An error occurred while updating {feature.GetType().FullName} feature:\n{e.Message}\n{e.StackTrace}");
                throw;
            }
        }
    }
}