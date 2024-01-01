using StardewModdingAPI.Events;
using StardewValley;

namespace stardew_access.Features;

public abstract class FeatureBase
{
    public static FeatureBase Instance => throw new Exception("Override Instance property!!");

    public abstract void Update(object? sender, UpdateTickedEventArgs e);

    public virtual bool OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        return false;
    }

    public virtual void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
    }

    public virtual void OnPlayerWarped(object? sender, WarpedEventArgs e)
    {
    }
}