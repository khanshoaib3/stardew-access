using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

using stardew_access.Translation;

namespace stardew_access.Features.Navigator;

/// <summary>
/// Menu testuale navigabile a 2 livelli per la selezione della destinazione.
/// </summary>
public class NavigatorMenu : IClickableMenu
{
    // Enum interno per lo stato del menu
    private enum MenuLevel { Level1, Level2 }

    private readonly IReadOnlyList<MapDestination> _maps;
    private readonly System.Action<MapDestination, PointOfInterest> _onConfirm;
    private readonly System.Action<string> _onSpeak;

    private MenuLevel _currentLevel = MenuLevel.Level1;
    private int _mapIndex;     // indice selezionato in Level1
    private int _poiIndex;     // indice selezionato in Level2

    // Dimensioni layout
    private const int ItemHeight = 56;
    private const int MenuPadding = 32;
    private const int TitleHeight = 64;
    private const int MaxVisibleItems = 12;

    public NavigatorMenu(
        IReadOnlyList<MapDestination> maps,
        System.Action<MapDestination, PointOfInterest> onConfirm,
        System.Action<string> onSpeak)
        : base(
            x: Game1.uiViewport.Width / 2 - 300,
            y: Game1.uiViewport.Height / 2 - (Math.Min(maps.Count, MaxVisibleItems) * ItemHeight / 2) - MenuPadding,
            width: 600,
            height: Math.Min(maps.Count, MaxVisibleItems) * ItemHeight + MenuPadding * 2 + TitleHeight,
            showUpperRightCloseButton: true)
    {
        _maps = maps;
        _onConfirm = onConfirm;
        _onSpeak = onSpeak;
        _mapIndex = 0;
        _poiIndex = 0;

        // Annuncio apertura — Livello 1
        if (_maps.Count > 0)
            _onSpeak(Translator.Instance.Translate("menu-navigator-choose_map_speak", new { map_name = _maps[0].MapDisplayName }, TranslationCategory.Menu));
        else
            _onSpeak(Translator.Instance.Translate("menu-navigator-no_destinations", TranslationCategory.Menu));
    }

    /// <summary>
    /// Gestisce input da tastiera: frecce per navigare, Enter per confermare,
    /// Escape per annullare o tornare al livello precedente.
    /// </summary>
    public override void receiveKeyPress(Keys key)
    {
        if (_maps.Count == 0)
        {
            if (key == Keys.Escape) exitThisMenu();
            return;
        }

        switch (key)
        {
            case Keys.Up:
            case Keys.W:
                MoveCursor(-1);
                break;

            case Keys.Down:
            case Keys.S:
                MoveCursor(1);
                break;

            case Keys.Enter:
                ConfirmSelection();
                break;

            case Keys.Escape:
                HandleEscape();
                break;
        }
    }

    /// <summary>Supporto gamepad D-Pad + A/B.</summary>
    public override void receiveGamePadButton(Buttons b)
    {
        switch (b)
        {
            case Buttons.DPadUp:
                MoveCursor(-1);
                break;
            case Buttons.DPadDown:
                MoveCursor(1);
                break;
            case Buttons.A:
                ConfirmSelection();
                break;
            case Buttons.B:
                HandleEscape();
                break;
        }
    }

    public override void draw(SpriteBatch b)
    {
        // Sfondo menu
        drawTextureBox(b, xPositionOnScreen, yPositionOnScreen, width, height, Color.White);

        // Titolo differenziato per livello
        string title = _currentLevel == MenuLevel.Level1
            ? Translator.Instance.Translate("menu-navigator-choose_map", TranslationCategory.Menu)
            : Translator.Instance.Translate("menu-navigator-choose_point", new { map_name = CurrentMap.MapDisplayName }, TranslationCategory.Menu);

        Utility.drawTextWithShadow(b, title,
            Game1.dialogueFont,
            new Vector2(xPositionOnScreen + MenuPadding, yPositionOnScreen + MenuPadding),
            Color.Black);

        // Lista voci del livello corrente
        var items = GetCurrentItems();
        int startY = yPositionOnScreen + MenuPadding + TitleHeight;
        int currentIndex = _currentLevel == MenuLevel.Level1 ? _mapIndex : _poiIndex;
        int scrollOffset = GetScrollOffset(currentIndex, items.Count);
        int visibleCount = Math.Min(items.Count, MaxVisibleItems);

        for (int i = 0; i < visibleCount; i++)
        {
            int idx = i + scrollOffset;
            if (idx >= items.Count) break;

            bool isSelected = idx == currentIndex;
            Color textColor = isSelected ? Color.DarkBlue : Color.Black;
            string prefix = isSelected ? "▶ " : "  ";

            Utility.drawTextWithShadow(b,
                prefix + items[idx],
                Game1.dialogueFont,
                new Vector2(xPositionOnScreen + MenuPadding, startY + i * ItemHeight),
                textColor);
        }

        // Frecce scroll
        if (items.Count > MaxVisibleItems)
        {
            if (scrollOffset > 0)
                Utility.drawTextWithShadow(b, "▲", Game1.dialogueFont,
                    new Vector2(xPositionOnScreen + width - MenuPadding - 20, startY - 20), Color.Gray);
            if (scrollOffset + MaxVisibleItems < items.Count)
                Utility.drawTextWithShadow(b, "▼", Game1.dialogueFont,
                    new Vector2(xPositionOnScreen + width - MenuPadding - 20, startY + visibleCount * ItemHeight), Color.Gray);
        }

        // Indicatore livello attuale (in basso a destra)
        string levelIndicator = _currentLevel == MenuLevel.Level1
            ? Translator.Instance.Translate("menu-navigator-indicator_map", TranslationCategory.Menu)
            : Translator.Instance.Translate("menu-navigator-indicator_point", TranslationCategory.Menu);
        Utility.drawTextWithShadow(b, levelIndicator, Game1.smallFont,
            new Vector2(xPositionOnScreen + width - MenuPadding - 80,
                        yPositionOnScreen + height - MenuPadding - 20),
            Color.Gray);

        base.draw(b);
        drawMouse(b);
    }

    // ─── Logica interna ───────────────────────────────────────────────────────

    private MapDestination CurrentMap => _maps[_mapIndex];

    /// <summary>Restituisce le stringhe da mostrare nel livello corrente.</summary>
    private List<string> GetCurrentItems()
    {
        if (_currentLevel == MenuLevel.Level1)
        {
            var names = new List<string>(_maps.Count);
            foreach (var m in _maps) names.Add(m.MapDisplayName);
            return names;
        }
        else
        {
            var poi = CurrentMap.PointsOfInterest;
            var names = new List<string>(poi.Count);
            foreach (var p in poi) names.Add(p.DisplayName);
            return names;
        }
    }

    private void MoveCursor(int direction)
    {
        if (_currentLevel == MenuLevel.Level1)
        {
            if (_maps.Count == 0) return;
            _mapIndex = (_mapIndex + direction + _maps.Count) % _maps.Count;
            _onSpeak(CurrentMap.MapDisplayName);
        }
        else
        {
            var poi = CurrentMap.PointsOfInterest;
            if (poi.Count == 0) return;
            _poiIndex = (_poiIndex + direction + poi.Count) % poi.Count;
            _onSpeak(poi[_poiIndex].DisplayName);
        }
    }

    private void ConfirmSelection()
    {
        if (_maps.Count == 0) return;

        if (_currentLevel == MenuLevel.Level1)
        {
            var map = CurrentMap;
            var poi = map.PointsOfInterest;

            // Mappa con 1 solo POI → auto-skip Livello 2, navigazione diretta
            if (poi.Count == 1)
            {
                exitThisMenu();
                _onSpeak(Translator.Instance.Translate("menu-navigator-navigating_to", new { map_name = map.MapDisplayName, poi_name = poi[0].DisplayName }, TranslationCategory.Menu));
                _onConfirm(map, poi[0]);
                return;
            }

            // Mappa con più POI → scendi al Livello 2
            _currentLevel = MenuLevel.Level2;
            _poiIndex = 0;
            ResizeForLevel2(poi.Count);

            string firstPoiName = poi.Count > 0 ? poi[0].DisplayName : Translator.Instance.Translate("menu-navigator-no_point", TranslationCategory.Menu);
            _onSpeak(Translator.Instance.Translate("menu-navigator-choose_point_speak", new { map_name = map.MapDisplayName, poi_name = firstPoiName }, TranslationCategory.Menu));
        }
        else
        {
            // Livello 2: conferma POI selezionato
            var map = CurrentMap;
            var poi = map.PointsOfInterest;
            if (poi.Count == 0 || _poiIndex >= poi.Count) return;

            PointOfInterest chosen = poi[_poiIndex];
            exitThisMenu();
            _onConfirm(map, chosen);
        }
    }

    private void HandleEscape()
    {
        if (_currentLevel == MenuLevel.Level2)
        {
            // Torna al Livello 1 senza modifiche
            _currentLevel = MenuLevel.Level1;
            _poiIndex = 0;
            ResizeForLevel1();
            _onSpeak(Translator.Instance.Translate("menu-navigator-choose_map_speak", new { map_name = CurrentMap.MapDisplayName }, TranslationCategory.Menu));
        }
        else
        {
            // Livello 1 → chiude il menu
            exitThisMenu();
        }
    }

    // Ridimensiona il menu in base al numero di voci del livello corrente
    private void ResizeForLevel2(int poiCount)
    {
        int visibleItems = Math.Min(poiCount, MaxVisibleItems);
        height = visibleItems * ItemHeight + MenuPadding * 2 + TitleHeight;
        yPositionOnScreen = Game1.uiViewport.Height / 2 - height / 2;
    }

    private void ResizeForLevel1()
    {
        int visibleItems = Math.Min(_maps.Count, MaxVisibleItems);
        height = visibleItems * ItemHeight + MenuPadding * 2 + TitleHeight;
        yPositionOnScreen = Game1.uiViewport.Height / 2 - height / 2;
    }

    // Calcola offset scroll per tenere la selezione visibile a centro schermo
    private static int GetScrollOffset(int selectedIndex, int itemCount)
    {
        if (selectedIndex < MaxVisibleItems) return 0;
        int maxOffset = itemCount - MaxVisibleItems;
        return Math.Min(selectedIndex - MaxVisibleItems / 2, maxOffset);
    }
}
