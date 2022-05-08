using StardewModdingAPI.Utilities;

namespace stardew_access
{
    internal class ModConfig
    {
        public Boolean VerboseCoordinates { get; set; } = true;
        public Boolean ReadTile { get; set; } = true;
        public Boolean SnapMouse { get; set; } = true;
        public Boolean Radar { get; set; } = false;
        public Boolean RadarStereoSound { get; set; } = true;
        public Boolean ReadFlooring { get; set; } = false;

        #region KeyBinds

        // https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Input#SButton button key codes
        public KeybindList LeftClickMainKey { get; set; } = KeybindList.Parse("LeftControl + Enter");
        public KeybindList RightClickMainKey { get; set; } = KeybindList.Parse("LeftShift + Enter");
        public KeybindList LeftClickAlternateKey { get; set; } = KeybindList.Parse("OemOpenBrackets");
        public KeybindList RightClickAlternateKey { get; set; } = KeybindList.Parse("OemCloseBrackets");
        public KeybindList HealthNStaminaKey { get; set; } = KeybindList.Parse("H");
        public KeybindList PositionKey { get; set; } = KeybindList.Parse("K");
        public KeybindList LocationKey { get; set; } = KeybindList.Parse("LeftAlt + K");
        public KeybindList MoneyKey { get; set; } = KeybindList.Parse("R");
        public KeybindList TimeNSeasonKey { get; set; } = KeybindList.Parse("Q");
        public KeybindList ReadTileKey { get; set; } = KeybindList.Parse("J");
        public KeybindList ReadStandingTileKey { get; set; } = KeybindList.Parse("LeftAlt + J");

        //Tile viewer keys
        public KeybindList TileCursorUpKey { get; set; } = KeybindList.Parse("Up");
        public KeybindList TileCursorRightKey { get; set; } = KeybindList.Parse("Right");
        public KeybindList TileCursorDownKey { get; set; } = KeybindList.Parse("Down");
        public KeybindList TileCursorLeftKey { get; set; } = KeybindList.Parse("Left");
        public KeybindList TileCursorPreciseUpKey { get; set; } = KeybindList.Parse("LeftShift + Up");
        public KeybindList TileCursorPreciseRightKey { get; set; } = KeybindList.Parse("LeftShift + Right");
        public KeybindList TileCursorPreciseDownKey { get; set; } = KeybindList.Parse("LeftShift + Down");
        public KeybindList TileCursorPreciseLeftKey { get; set; } = KeybindList.Parse("LeftShift + Left");

        #endregion

        // TODO Add the exclusion and focus list too
        // public String ExclusionList { get; set; } = "test";
    }
}
