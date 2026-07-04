using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Pathfinding;

using stardew_access.Translation;

namespace stardew_access.Features.Navigator;

/// <summary>
/// State machine che gestisce le due fasi della navigazione automatica.
/// </summary>
public class Navigator
{
    private enum State { Idle, PhaseA_Pathfinding, PhaseA_WaitingWarp, PhaseB_Pathfinding, Arrived }

    private State _state = State.Idle;

    // ── Fase A: rotta BFS verso la mappa target ──
    private List<RouteStep>? _phaseARoute;
    private int _currentStepIndex;
    private Point _phaseATargetTile;

    // ── Fase B: navigazione al POI nella mappa corrente ──
    private Point _phaseBTargetTile;
    private string _phaseBTargetLocation = string.Empty;

    // ── Metadata della sessione corrente ──
    private string _mapDisplayName = string.Empty;    // per annuncio "Arrivato a [mappa]"
    private string _poiDisplayName = string.Empty;    // per annuncio "Arrivato a [POI]"

    // ── Meccanismo B: riferimento al controller attivo ──
    private PathFindController? _activeController;
    private bool _stepJustCompleted;

    // Fix A2: riassegnazioni null controller
    private int _controllerReassignCount;
    private const int MaxReassign = 5;   // aumentato da 3: margine di sicurezza post-warp

    // Sopprime Meccanismo A/B durante transizione warp
    private bool _warpInProgress;

    // Fix A1: border warp
    private bool _pendingBorderWarp;
    private string? _pendingBorderWarpTarget;
    private int _pendingBorderWarpTargetX;
    private int _pendingBorderWarpTargetY;

    // Periodo immune post-assegnazione controller
    private int _controllerImmuneTicksRemaining;

    // Timeout warp
    private const int WarpTimeoutTicks = 1800;
    private int _warpWaitTicks;

    // Timeout Fase B: massimo 1800 tick (30s) per trovare il tile POI
    private const int PhaseBTimeoutTicks = 1800;
    private int _phaseBTicks;

    // Fix B2: avvio Fase B differito di 1 tick dopo il warp.
    private bool _pendingStartPhaseB;
    private string? _pendingPhaseBLocationName;

    // Fix B3: avvio step successivo Fase A differito di 1 tick dopo il warp.
    private bool _pendingStartNextPhaseAStep;

    private int _footstepTickCount;

    public bool IsActive => _state != State.Idle;

    public Navigator()
    {
    }

    /// <summary>
    /// Avvia la sessione di navigazione verso una destinazione risolta.
    /// Calcola la rotta multimappa e decide se avviare la Fase A o direttamente la Fase B.
    /// </summary>
    public void StartNavigation(MapDestination map, PointOfInterest poi, RouteEngine routeEngine)
    {
        if (!poi.IsResolved)
        {
            Log.Warn($"Impossibile navigare verso {poi.DisplayName}: coordinate non risolte.");
            return;
        }

        Farmer player = Game1.player;
        if (player == null) return;

        string currentMap = player.currentLocation?.Name ?? string.Empty;
        _mapDisplayName = map.MapDisplayName;
        _poiDisplayName = poi.DisplayName;
        _phaseBTargetTile = poi.ResolvedArrivalTile;
        _phaseBTargetLocation = poi.ResolvedLocationName;

        Log.Debug($"Avvio navigazione: mappa='{_mapDisplayName}' POI='{_poiDisplayName}'");

        if (currentMap == _phaseBTargetLocation)
        {
            // Già sulla mappa corretta: skip Fase A, avvio immediato Fase B
            Log.Debug($"Già nella mappa target '{_phaseBTargetLocation}' — avvio diretto Fase B.");
            StartPhaseB();
        }
        else
        {
            // Mappe diverse: calcola rotta Fase A via RouteEngine
            List<RouteStep>? route = routeEngine.FindRoute(currentMap, _phaseBTargetLocation);

            if (route == null)
            {
                MainClass.ScreenReader.Say(Translator.Instance.Translate("menu-navigator-unreachable", new { map_name = _mapDisplayName }, TranslationCategory.Menu), true);
                CancelInternal();
                return;
            }

            _phaseARoute = route;
            _currentStepIndex = 0;
            _warpWaitTicks = 0;
            _warpInProgress = false;
            _pendingBorderWarp = false;

            Log.Debug($"Rotta calcolata: {_phaseARoute.Count} step warp.");
            _state = State.PhaseA_Pathfinding;

            ExecuteCurrentPhaseAStep();
        }
    }

    /// <summary>
    /// Interrompe bruscamente la navigazione, annullando il PathFindController se attivo.
    /// </summary>
    public void CancelNavigation(string reason)
    {
        if (!IsActive) return;

        Log.Debug($"Navigazione annullata: {reason}");
        MainClass.ScreenReader.Say(Translator.Instance.Translate("menu-navigator-cancelled", TranslationCategory.Menu), true);
        CancelInternal();
    }

    /// <summary>
    /// Da chiamare su GameLoop.UpdateTicked.
    /// Monitora la navigazione attiva e gestisce i meccanismi di fallimento e ripresa.
    /// </summary>
    public void OnUpdateTicked(UpdateTickedEventArgs e)
    {
        if (_state == State.Idle) return;

        // Fix B2: avvio differito Fase B — processato qui, 1+ tick dopo OnPlayerWarped
        if (_pendingStartPhaseB)
        {
            _pendingStartPhaseB = false;
            StartPhaseB(_pendingPhaseBLocationName);
            return;
        }

        // Fix B3: avvio differito step successivo Fase A — processato qui, 1+ tick dopo OnPlayerWarped
        if (_pendingStartNextPhaseAStep)
        {
            _pendingStartNextPhaseAStep = false;
            ExecuteCurrentPhaseAStep();
            return;
        }

        // Fix A1: border warp ha priorità assoluta
        if (_pendingBorderWarp)
        {
            HandlePendingBorderWarp();
            return;
        }

        Farmer player = Game1.player;
        if (player == null) return;

        // Gestione periodo di immunità post-assegnazione controller
        if (_controllerImmuneTicksRemaining > 0)
        {
            _controllerImmuneTicksRemaining--;
        }

        // Controllo di prossimità in Fase A per forzare il warp se il controller si blocca vicino al target
        if (_state == State.PhaseA_Pathfinding && _activeController != null && _phaseARoute != null && _currentStepIndex < _phaseARoute.Count)
        {
            RouteStep step = _phaseARoute[_currentStepIndex];
            int distX = Math.Abs((int)player.Tile.X - _phaseATargetTile.X);
            int distY = Math.Abs((int)player.Tile.Y - _phaseATargetTile.Y);

            if (distX <= 1 && distY <= 1)
            {
                Log.Debug($"[Navigator Proximity] Giocatore vicino al target ({player.Tile.X},{player.Tile.Y} vs {_phaseATargetTile.X},{_phaseATargetTile.Y}). Forzo completamento step.");
                
                _stepJustCompleted = true;
                _activeController = null;
                player.controller = null;

                _state = State.PhaseA_WaitingWarp;
                _warpWaitTicks = 0;
                _pendingBorderWarp = true;
                _pendingBorderWarpTarget = step.NextLocationName;
                _pendingBorderWarpTargetX = step.WarpTargetX;
                _pendingBorderWarpTargetY = step.WarpTargetY;
                return;
            }
        }

        // --- Meccanismo A: Intercettamento del completamento o dell'annullamento ---
        if (_state == State.PhaseA_Pathfinding || _state == State.PhaseB_Pathfinding)
        {
            if (player.controller == null)
            {
                if (_stepJustCompleted)
                {
                    // Il controller è terminato regolarmente (callback eseguiti)
                    _stepJustCompleted = false;
                }
                else if (_controllerImmuneTicksRemaining == 0)
                {
                    // Il controller è stato rimosso inaspettatamente (es. ostacolo, collisione, reset di gioco)
                    if (_state == State.PhaseA_Pathfinding)
                    {
                        // Meccanismo A (Fase A): riassegna lo step corrente fino a MaxReassign volte
                        _controllerReassignCount++;
                        if (_controllerReassignCount > MaxReassign)
                        {
                            Log.Warn("Meccanismo A: controller null persistente — annullo navigazione.");
                            CancelNavigation("riassegnazione controller fallita in Fase A");
                            return;
                        }

                        Log.Debug($"Meccanismo A: riassegnazione controller in Fase A (#{_controllerReassignCount})");
                        ExecuteCurrentPhaseAStep();
                    }
                    else
                    {
                        // Meccanismo B (Fase B): riassegna il target finale
                        _controllerReassignCount++;
                        if (_controllerReassignCount > MaxReassign)
                        {
                            Log.Warn("Meccanismo B: controller null persistente — annullo navigazione.");
                            CancelNavigation("riassegnazione controller fallita in Fase B");
                            return;
                        }

                        Log.Debug($"Meccanismo B: riassegnazione controller in Fase B (#{_controllerReassignCount})");
                        StartPhaseB();
                    }
                }
            }
            else
            {
                // Controller attivo: reset contatore tentativi
                _controllerReassignCount = 0;

                // Riproduzione effetti sonori passi coerenti col terreno
                _footstepTickCount++;
                if (_footstepTickCount >= 21) // 21 tick = ~350ms
                {
                    _footstepTickCount = 0;
                    try
                    {
                        player.currentLocation?.playTerrainSound(player.Tile);
                    }
                    catch { }
                }
            }
        }

        // --- Monitoraggio Timeout ---
        if (_state == State.PhaseA_WaitingWarp)
        {
            _warpWaitTicks++;

            // Meccanismo di recupero: se il giocatore si trova già nella mappa successiva
            // ma l'evento OnPlayerWarped non è scattato o è stato ignorato per via di 'Temp',
            // completiamo lo step a runtime.
            string currentLoc = Game1.currentLocation?.Name ?? string.Empty;
            if (_phaseARoute != null && _currentStepIndex < _phaseARoute.Count)
            {
                RouteStep completedStep = _phaseARoute[_currentStepIndex];
                if (!string.IsNullOrEmpty(currentLoc) &&
                    !currentLoc.Equals("Temp", StringComparison.OrdinalIgnoreCase) &&
                    !currentLoc.Equals("temp", StringComparison.OrdinalIgnoreCase) &&
                    currentLoc == completedStep.NextLocationName)
                {
                    Log.Debug($"[Navigator Recovery] Warp verso {currentLoc} rilevato in OnUpdateTicked.");
                    CompleteWarpStep(currentLoc);
                    return;
                }
            }

            if (_warpWaitTicks > WarpTimeoutTicks)
                CancelNavigation("timeout attesa warp");
        }

        if (_state == State.PhaseB_Pathfinding)
        {
            _phaseBTicks++;
            if (_phaseBTicks > PhaseBTimeoutTicks)
                CancelNavigation("timeout Fase B");
        }
    }

    /// <summary>
    /// Da chiamare su Player.Warped (solo durante Fase A).
    /// </summary>
    public void OnPlayerWarped(WarpedEventArgs e)
    {
        if (_state != State.PhaseA_WaitingWarp && _state != State.PhaseA_Pathfinding) return;
        if (_phaseARoute == null) return;
        if (_currentStepIndex < 0 || _currentStepIndex >= _phaseARoute.Count) return;

        RouteStep completedStep = _phaseARoute[_currentStepIndex];

        // Ignoriamo i warp verso location temporanee di caricamento
        if (e.NewLocation.Name.Equals("Temp", StringComparison.OrdinalIgnoreCase) || 
            e.NewLocation.Name.Equals("temp", StringComparison.OrdinalIgnoreCase))
        {
            Log.Debug("OnPlayerWarped ignorato: destinazione temporanea 'Temp'.");
            return;
        }

        // Verifica warp nella direzione attesa
        if (e.NewLocation.Name != completedStep.NextLocationName)
        {
            Log.Debug($"Warp verso {e.NewLocation.Name} invece di {completedStep.NextLocationName} — ignoro.");
            return;
        }

        CompleteWarpStep(e.NewLocation.Name);
    }

    private void CompleteWarpStep(string newLocationName)
    {
        _currentStepIndex++;
        _warpWaitTicks = 0;
        _warpInProgress = false;
        _pendingBorderWarp = false;
        _activeController = null;
        if (Game1.player?.controller != null)
            Game1.player.controller = null;

        if (_phaseARoute == null) return;

        // Controlla se Fase A è completata
        if (_currentStepIndex >= _phaseARoute.Count)
        {
            string arrivedAt = newLocationName;
            string expectedMap = _phaseARoute[_phaseARoute.Count - 1].NextLocationName;

            if (arrivedAt != expectedMap)
            {
                Log.Warn($"Verifica mappa fallita: atteso '{expectedMap}', corrente '{arrivedAt}' — annullo.");
                CancelNavigation("verifica mappa post-Fase A fallita");
                return;
            }

            Log.Debug($"Fase A completata. Mappa corrente verificata: {arrivedAt}");
            MainClass.ScreenReader.Say(Translator.Instance.Translate("menu-navigator-arrived_at_map", new { map_name = _mapDisplayName }, TranslationCategory.Menu), true);

            // Avvia Fase B al tick successivo (differito)
            _pendingStartPhaseB = true;
            _pendingPhaseBLocationName = arrivedAt;
            return;
        }

        // Ancora step di Fase A rimanenti
        MainClass.ScreenReader.Say(Translator.Instance.Translate("menu-navigator-crossing_map", new { map_name = newLocationName }, TranslationCategory.Menu), false);
        _state = State.PhaseA_Pathfinding;
        _pendingStartNextPhaseAStep = true;
    }

    /// <summary>
    /// Meccanismo A: cancella la sessione se l'utente preme un tasto movimento.
    /// </summary>
    public void OnButtonPressed(ButtonPressedEventArgs e)
    {
        if (!IsActive) return;

        if (_warpInProgress) return;

        bool isMovementKey = e.Button == SButton.W || e.Button == SButton.A || e.Button == SButton.S || e.Button == SButton.D ||
                             e.Button == SButton.Left || e.Button == SButton.Right || e.Button == SButton.Up || e.Button == SButton.Down ||
                             e.Button == SButton.LeftThumbstickUp || e.Button == SButton.LeftThumbstickDown ||
                             e.Button == SButton.LeftThumbstickLeft || e.Button == SButton.LeftThumbstickRight;

        if (isMovementKey)
        {
            CancelNavigation("input movimento manuale");
        }
    }

    // ─── Fase A: navigazione PathFindController ai warp ──────────────────────

    private void ExecuteCurrentPhaseAStep()
    {
        if (_phaseARoute == null || _currentStepIndex >= _phaseARoute.Count) return;

        Farmer player = Game1.player;
        GameLocation location = player.currentLocation;
        RouteStep step = _phaseARoute[_currentStepIndex];

        int targetX = step.TargetTile.X;
        int targetY = step.TargetTile.Y;
        bool isBorderWarp = step.IsBorderWarp;
        int borderDirection = step.BorderDirection;

        int mapW = GetMapWidth(location);
        int mapH = GetMapHeight(location);

        if (mapW > 0 && mapH > 0)
        {
            if (targetX < 0)
                { targetX = 0; isBorderWarp = true; borderDirection = 0; }
            else if (targetX >= mapW)
                { targetX = mapW - 1; isBorderWarp = true; borderDirection = 1; }

            if (targetY < 0)
                { targetY = 0; isBorderWarp = true; if (borderDirection < 0) borderDirection = 2; }
            else if (targetY >= mapH)
                { targetY = mapH - 1; isBorderWarp = true; if (borderDirection < 0) borderDirection = 3; }
        }

        // CORREZIONE CRITICA: Forziamo TUTTI i warp intermedi di Fase A a comportarsi come border warp.
        // Questo garantisce l'uso deterministico di Game1.warpFarmer() al completamento del percorso,
        // evitando che il giocatore rimanga congelato sul tile di warp se SDV non lo rileva da fermo.
        isBorderWarp = true;

        Point targetTile = new Point(targetX, targetY);

        // Fix: usa tile calpestabile adiacente se il
        // tile di warp è fisicamente bloccato in SDV
        targetTile = GetNearestPassableTile(
            location, targetTile, mapW, mapH);
        _phaseATargetTile = targetTile;

        _state = State.PhaseA_Pathfinding;

        try
        {
            _activeController = new PathFindController(
                player,
                location,
                targetTile,
                -1,
                endBehaviorFunction: (character, loc) =>
                {
                    _stepJustCompleted = true;

                    // Fix race condition: se il giocatore è già in una mappa diversa da quella
                    // in cui lo step è stato avviato, il warp naturale SDV è già avvenuto
                    // e OnPlayerWarped ha già pulito lo stato. Non sovrascrivere con stato obsoleto!
                    if (Game1.player?.currentLocation != location)
                    {
                        Log.Debug("endBehaviorFunction bypassato: warp già avvenuto.");
                        return;
                    }

                    // Essendo isBorderWarp = true, eseguiamo sempre il warp manuale via Game1.warpFarmer()
                    _state = State.PhaseA_WaitingWarp;
                    _warpWaitTicks = 0;
                    _pendingBorderWarp = true;
                    _pendingBorderWarpTarget = step.NextLocationName;
                    _pendingBorderWarpTargetX = step.WarpTargetX;
                    _pendingBorderWarpTargetY = step.WarpTargetY;
                    Log.Debug($"Fase A step completato: warp forzato verso {step.NextLocationName} ({step.WarpTargetX},{step.WarpTargetY})");
                }
            );

            player.controller = _activeController;
            _controllerImmuneTicksRemaining = 5;
        }
        catch (System.Exception ex)
        {
            Log.Error($"[Navigator] Eccezione nell'istanziazione di PathFindController in Fase A: {ex.Message}");
            CancelNavigation("Eccezione nell'avvio del pathfinder di Fase A");
            return;
        }

        string logSuffix = step.IsBorderWarp ? " [bordo]" : " [bordo forzato runtime]";
        Log.Debug($"Fase A step {_currentStepIndex + 1}/{_phaseARoute.Count}: '{step.LocationName}' → tile ({targetTile.X},{targetTile.Y})" + logSuffix);
    }

    // ─── Fase B: navigazione PathFindController al POI ───────────────────────

    private void StartPhaseB(string? currentLocationName = null)
    {
        Farmer player = Game1.player;
        string locationName = currentLocationName ?? Game1.currentLocation?.Name ?? string.Empty;

        // Verifica finale: siamo nella location giusta per il POI?
        if (locationName != _phaseBTargetLocation)
        {
            Log.Warn($"Fase B: location corrente '{locationName}' != attesa '{_phaseBTargetLocation}' — annullo.");
            CancelNavigation("Fase B: location corrente non corrisponde al target POI");
            return;
        }

        GameLocation location = Game1.currentLocation!;

        _state = State.PhaseB_Pathfinding;
        _phaseBTicks = 0;
        _controllerReassignCount = 0;

        try
        {
            _activeController = new PathFindController(
                player,
                location,
                _phaseBTargetTile,
                -1,
                endBehaviorFunction: (character, loc) =>
                {
                    // Fase B completata: annuncio arrivo al POI
                    _stepJustCompleted = true;
                    _state = State.Arrived;
                    MainClass.ScreenReader.Say(Translator.Instance.Translate("menu-navigator-arrived_at_poi", new { poi_name = _poiDisplayName }, TranslationCategory.Menu), true);
                    Log.Debug($"Fase B completata. Arrivato a '{_poiDisplayName}' @ ({_phaseBTargetTile.X},{_phaseBTargetTile.Y})");
                    _state = State.Idle;
                    _activeController = null;
                }
            );

            player.controller = _activeController;
            _controllerImmuneTicksRemaining = 5;
            Log.Debug($"Fase B avviata: tile ({_phaseBTargetTile.X},{_phaseBTargetTile.Y}) in '{_phaseBTargetLocation}'");
        }
        catch (System.Exception ex)
        {
            Log.Error($"[Navigator] Eccezione nell'istanziazione di PathFindController in Fase B: {ex.Message}");
            CancelNavigation("Eccezione nell'avvio del pathfinder di Fase B");
        }
    }

    // ─── Utility ─────────────────────────────────────────────────────────────

    private void CancelInternal()
    {
        PathFindController? controllerToRelease = _activeController;

        _state = State.Idle;
        _activeController = null;
        _stepJustCompleted = false;
        _warpInProgress = false;
        _controllerImmuneTicksRemaining = 0;
        _controllerReassignCount = 0;
        _warpWaitTicks = 0;
        _pendingBorderWarp = false;
        _pendingBorderWarpTarget = null;
        _pendingBorderWarpTargetX = 0;
        _pendingBorderWarpTargetY = 0;
        _phaseARoute = null;
        _currentStepIndex = 0;
        _phaseATargetTile = Point.Zero;
        _phaseBTicks = 0;
        _pendingStartPhaseB = false;
        _pendingPhaseBLocationName = null;
        _pendingStartNextPhaseAStep = false;
        _footstepTickCount = 0;
        _mapDisplayName = string.Empty;
        _poiDisplayName = string.Empty;

        ForceUnlockPlayer(controllerToRelease);
    }

    private void ForceUnlockPlayer(PathFindController? controllerToRelease)
    {
        try
        {
            Farmer? player = Game1.player;
            if (player == null) return;

            if (controllerToRelease != null && player.controller == controllerToRelease)
            {
                player.controller = null;
            }
            else if (player.controller != null && player.controller.GetType().FullName?.Contains("PathFindController") == true)
            {
                // Rilascia in modo sicuro qualsiasi pathfind controller residuo
                player.controller = null;
            }

            player.freezePause = 0;
            player.xVelocity = 0f;
            player.yVelocity = 0f;
            player.Halt();
        }
        catch (System.Exception ex)
        {
            Log.Debug($"[Navigator] Errore in ForceUnlockPlayer: {ex.Message}");
        }
    }

    /// <summary>
    /// Chiama Game1.warpFarmer per completare un border warp.
    /// </summary>
    private void HandlePendingBorderWarp()
    {
        if (!_pendingBorderWarp) return;
        if (string.IsNullOrEmpty(_pendingBorderWarpTarget)) return;

        _pendingBorderWarp = false;
        _warpInProgress = true;

        Log.Debug($"Border warp avviato: → {_pendingBorderWarpTarget} ({_pendingBorderWarpTargetX},{_pendingBorderWarpTargetY})");
        Game1.warpFarmer(_pendingBorderWarpTarget, _pendingBorderWarpTargetX, _pendingBorderWarpTargetY, false);
    }

    private int GetMapWidth(GameLocation location)
    {
        try
        {
            if (location?.Map?.Layers?.Count > 0)
                return location.Map.Layers[0].LayerWidth;
        }
        catch { }

        return location?.Name switch
        {
            "Farm" => 80,
            "BusStop" => 35,
            "Town" => 120,
            "Beach" => 104,
            "Mountain" => 135,
            "Forest" => 120,
            "Backwoods" => 50,
            "Woods" => 60,
            "Railroad" => 70,
            _ => -1
        };
    }

    private int GetMapHeight(GameLocation location)
    {
        try
        {
            if (location?.Map?.Layers?.Count > 0)
                return location.Map.Layers[0].LayerHeight;
        }
        catch { }

        return location?.Name switch
        {
            "Farm" => 80,
            "BusStop" => 30,
            "Town" => 110,
            "Beach" => 50,
            "Mountain" => 45,
            "Forest" => 120,
            "Backwoods" => 40,
            "Woods" => 32,
            "Railroad" => 65,
            _ => -1
        };
    }

    private bool IsTileCollidingInGame(GameLocation location, int x, int y)
    {
        try
        {
            Rectangle pb = Game1.player.GetBoundingBox();
            Rectangle tb = new Rectangle(x * 64, y * 64, pb.Width, pb.Height);
            return location.isCollidingPosition(tb, Game1.viewport, true, 0, false, Game1.player);
        }
        catch { return true; }
    }

    private Point GetNearestPassableTile(GameLocation location, Point targetTile, int mapW, int mapH)
    {
        if (!IsTileCollidingInGame(location, targetTile.X, targetTile.Y))
            return targetTile;

        Point[] offsets =
        {
            new Point(0,1), new Point(0,-1),
            new Point(1,0), new Point(-1,0),
            new Point(1,1), new Point(-1,1),
            new Point(1,-1), new Point(-1,-1)
        };

        for (int r = 1; r <= 3; r++)
        {
            foreach (Point o in offsets)
            {
                int nx = targetTile.X + o.X * r;
                int ny = targetTile.Y + o.Y * r;

                if (nx < 0 || ny < 0) continue;
                if (mapW > 0 && nx >= mapW) continue;
                if (mapH > 0 && ny >= mapH) continue;

                if (!IsTileCollidingInGame(location, nx, ny))
                {
                    Log.Debug($"[Nav] Tile warp ({targetTile.X},{targetTile.Y}) bloccato. Fallback: ({nx},{ny})");
                    return new Point(nx, ny);
                }
            }
        }

        Log.Warn($"[Nav] Nessun tile libero vicino a ({targetTile.X},{targetTile.Y}).");
        return targetTile;
    }

    public Point TestGetNearestPassableTile(GameLocation location, Point targetTile, int mapW, int mapH)
        => GetNearestPassableTile(location, targetTile, mapW, mapH);

    public bool TestIsTileCollidingInGame(GameLocation location, int x, int y)
        => IsTileCollidingInGame(location, x, y);
}
