using StardewModdingAPI.Events;

namespace stardew_access.Features;

public abstract class FeatureBase
{
    public static FeatureBase Instance => throw new Exception("Override Instance property!!");

    public abstract void Update(object? sender, UpdateTickedEventArgs e);
}