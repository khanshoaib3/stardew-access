using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace stardew_access.Features.Navigator;

/// <summary>
/// Carica e mantiene il registro delle mappe navigabili e dei loro punti di interesse
/// da assets/navigator_destinations.json. Caricato una volta al SaveLoaded.
/// </summary>
public class DestinationRegistry
{
    private readonly string _assetsPath;
    private List<MapDestination> _maps = new();

    /// <summary>Lista delle mappe disponibili, con solo POI risolti con successo.</summary>
    public IReadOnlyList<MapDestination> Maps => _maps;

    public DestinationRegistry(string assetsPath)
    {
        _assetsPath = assetsPath;
    }

    /// <summary>
    /// Carica il registro da navigator_destinations.json.
    /// </summary>
    public void Load()
    {
        string filePath = Path.Combine(_assetsPath, "navigator_destinations.json");

        if (!File.Exists(filePath))
        {
            Log.Warn($"navigator_destinations.json non trovato in: {filePath}");
            _maps = new();
            return;
        }

        try
        {
            string json = File.ReadAllText(filePath);
            var loaded = JsonSerializer.Deserialize<List<MapDestination>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _maps = loaded ?? new();
            int totalPoi = _maps.Sum(m => m.PointsOfInterest.Count);
            Log.Debug($"Caricate {_maps.Count} mappe, {totalPoi} punti di interesse per il navigatore.");
        }
        catch (JsonException ex)
        {
            Log.Error($"Errore parsing navigator_destinations.json: {ex.Message}");
            _maps = new();
        }
    }

    /// <summary>
    /// Risolve le coordinate di arrivo per tutti i POI.
    /// </summary>
    public void ResolveCoordinates(RouteEngine routeEngine)
    {
        int resolvedCount = 0;
        int failedCount = 0;

        foreach (MapDestination map in _maps)
        {
            foreach (PointOfInterest poi in map.PointsOfInterest)
            {
                if (!string.IsNullOrEmpty(poi.TargetLocationName))
                {
                    // ── Strategia 1: warp diretto ────────────────────────────────────────
                    Point? entrance = routeEngine.GetEntranceTile(map.MapLocationName, poi.TargetLocationName);

                    if (entrance.HasValue)
                    {
                        poi.ResolvedArrivalTile = entrance.Value;
                        poi.ResolvedLocationName = map.MapLocationName;
                        poi.IsResolved = true;
                        resolvedCount++;
                    }
                    else
                    {
                        // ── Strategia 2: warp inverso ─────────────────────────────────────
                        Point? exitTile = routeEngine.GetExitTile(poi.TargetLocationName, map.MapLocationName);

                        if (exitTile.HasValue)
                        {
                            poi.ResolvedArrivalTile = exitTile.Value;
                            poi.ResolvedLocationName = map.MapLocationName;
                            poi.IsResolved = true;
                            resolvedCount++;
                        }
                        else
                        {
                            poi.IsResolved = false;
                            failedCount++;
                        }
                    }
                }
                else if (poi.ArrivalX.HasValue && poi.ArrivalY.HasValue)
                {
                    // Coordinate esplicite
                    poi.ResolvedArrivalTile = new Point(poi.ArrivalX.Value, poi.ArrivalY.Value);
                    poi.ResolvedLocationName = map.MapLocationName;
                    poi.IsResolved = true;
                    resolvedCount++;
                }
                else
                {
                    poi.IsResolved = false;
                    failedCount++;
                }
            }
        }

        // Filtra le mappe senza POI risolti
        int mapsBeforeFilter = _maps.Count;
        _maps = _maps
            .Select(m => new MapDestination
            {
                MapDisplayName = m.MapDisplayName,
                MapLocationName = m.MapLocationName,
                PointsOfInterest = m.PointsOfInterest.Where(p => p.IsResolved).ToList()
            })
            .Where(m => m.PointsOfInterest.Count > 0)
            .ToList();

        int mapsAfterFilter = _maps.Count;
        int removedMaps = mapsBeforeFilter - mapsAfterFilter;

        Log.Debug(
            $"Risoluzione coordinate completata: {resolvedCount} POI risolti, " +
            $"{failedCount} falliti, {removedMaps} mappe rimosse per 0 POI disponibili. " +
            $"Mappe disponibili per il navigatore: {mapsAfterFilter}.");
    }
}
