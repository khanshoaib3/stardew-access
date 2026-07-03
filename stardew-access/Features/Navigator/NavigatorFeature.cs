using System;
using System.IO;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace stardew_access.Features.Navigator;

public class NavigatorFeature : FeatureBase
{
    public new static NavigatorFeature Instance { get; private set; } = null!;

    private readonly RouteEngine _routeEngine;
    private readonly Navigator _navigator;
    private readonly DestinationRegistry _destinationRegistry;

    public NavigatorFeature()
    {
        Instance = this;

        _routeEngine = new RouteEngine();
        _navigator = new Navigator();

        // Carica il registro dei POI da assets/navigator_destinations.json
        string assetsPath = Path.Combine(MainClass.ModHelper!.DirectoryPath, "assets");
        _destinationRegistry = new DestinationRegistry(assetsPath);
        _destinationRegistry.Load();

        // Si registra agli eventi del ciclo di vita necessari
        MainClass.ModHelper.Events.GameLoop.SaveLoaded += OnSaveLoaded;

        // Registra il comando di console SMAPI per il test di fallback deterministico
        MainClass.ModHelper.ConsoleCommands.Add("navigator_test_fallback", "Testa il funzionamento del fallback di collisione per BusStop (11,6)", (cmd, args) =>
        {
            if (!Context.IsWorldReady)
            {
                Log.Info("[TEST] Il mondo deve essere caricato con un salvataggio attivo per eseguire questo test.");
                return;
            }
            GameLocation busStop = Game1.getLocationFromName("BusStop");
            if (busStop == null)
            {
                Log.Error("[TEST] Mappa BusStop non trovata!");
                return;
            }
            
            Microsoft.Xna.Framework.Point target = new Microsoft.Xna.Framework.Point(11, 6);
            Microsoft.Xna.Framework.Point result = _navigator.TestGetNearestPassableTile(busStop, target, 35, 30);
            
            Log.Info($"[TEST] Target originale: {target} | Risultato: {result}");
            
            bool targetCollides = _navigator.TestIsTileCollidingInGame(busStop, target.X, target.Y);
            bool resultCollides = _navigator.TestIsTileCollidingInGame(busStop, result.X, result.Y);
            
            if (targetCollides && !resultCollides && result != target)
            {
                Log.Info("TEST RISULTATO: PASS ✅");
            }
            else
            {
                Log.Error($"TEST RISULTATO: FAIL ❌ (targetCollides={targetCollides}, resultCollides={resultCollides}, result={result})");
            }
        });
    }

    public override void Update(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady) return;
        _navigator.OnUpdateTicked(e);
    }

    public override bool OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady) return false;

        // Se la navigazione è attiva, intercetta i tasti di movimento manuali per annullare
        if (_navigator.IsActive)
        {
            _navigator.OnButtonPressed(e);
        }

        // Tasto di apertura menu Navigator
        if (MainClass.Config.NavigatorMenuKey.JustPressed())
        {
            if (!Context.IsPlayerFree) return false;

            Game1.activeClickableMenu = new NavigatorMenu(
                _destinationRegistry.Maps,
                (map, poi) => _navigator.StartNavigation(map, poi, _routeEngine),
                text => MainClass.ScreenReader.Say(text, true)
            );
            return true;
        }

        return false;
    }

    public override void OnPlayerWarped(object? sender, WarpedEventArgs e)
    {
        if (!Context.IsWorldReady) return;
        _navigator.OnPlayerWarped(e);
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        // Ricostruisce il grafo al caricamento del salvataggio
        _routeEngine.BuildGraph();
        _destinationRegistry.ResolveCoordinates(_routeEngine);
    }
}
