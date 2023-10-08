using StardewModdingAPI.Events;

namespace stardew_access.Features;

public class FeatureManager
{
    public static void UpdateAll(object? sender, UpdateTickedEventArgs e)
    {
        List<FeatureBase> allFeatures = new()
        {
            ReadTile.Instance,
            GridMovement.Instance,
            TileViewer.Instance,
            ObjectTracker.Instance,
            GameStateNarrator.Instance,
            Warnings.Instance,
            Radar.Instance,
        };

        foreach (FeatureBase feature in allFeatures)
        {
            try
            {
                feature.Update(sender, e);
            }
            catch (Exception exception)
            {
                Log.Error( $"An error occurred while updating {feature.GetType().FullName} feature:\n{exception.Message}\n{exception.StackTrace}");
                throw;
            }
        }
    }
}