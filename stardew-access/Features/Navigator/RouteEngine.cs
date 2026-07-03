using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using stardew_access.Features.Navigator;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Pathfinding;

namespace stardew_access.Features.Navigator;

/// <summary>
/// Costruisce dinamicamente il grafo delle connessioni tra mappe leggendo
/// Game1.locations al SaveLoaded, poi calcola percorsi con BFS.
/// Il grafo non è hardcodato: funziona con qualsiasi mod che aggiunga location.
/// </summary>
public class RouteEngine
{
    // Nodo del grafo: mappa sorgente → lista di archi verso altre mappe
    private readonly Dictionary<string, List<WarpEdge>> _graph = new();

    private sealed record WarpEdge(
        string FromLocation,
        // Coordinate clampate ai limiti validi della mappa (usate da PathFindController)
        int FromX,
        int FromY,
        string ToLocation,
        // Coordinate di arrivo nella mappa di destinazione (da warp.TargetX/Y)
        int ToX,
        int ToY,
        // True se il tile warp original era fuori dai limiti mappa (bordo mappa)
        bool IsBorderWarp = false,
        // 0=sinistra, 1=destra, 2=sopra, 3=sotto — valido solo se IsBorderWarp
        int BorderDirection = -1
    );

    public RouteEngine()
    {
    }

    /// <summary>
    /// Costruisce il grafo leggendo tutti i warp di tutte le GameLocation caricate.
    /// Da chiamare su SaveLoaded, quando Game1.locations è popolato.
    /// Le coordinate dei warp al bordo mappa (X=-1, X>=mapWidth, ecc.) vengono
    /// clampate al tile valido più vicino per PathFindController; IsBorderWarp=true
    /// segnala che serve un push manuale dopo il completamento del PathFindController.
    /// </summary>
    public void BuildGraph()
    {
        _graph.Clear();

        foreach (GameLocation location in Game1.locations)
        {
            string fromName = location.Name;
            if (!_graph.ContainsKey(fromName))
                _graph[fromName] = new();

            // Dimensioni mappa per clamping coordinate border warp
            int mapW = -1;
            int mapH = -1;
            try
            {
                if (location.Map?.Layers?.Count > 0)
                {
                    mapW = location.Map.Layers[0].LayerWidth;
                    mapH = location.Map.Layers[0].LayerHeight;
                }
            }
            catch
            {
                // Map non caricata: usiamo coordinate originali senza clamping
            }

            foreach (Warp warp in location.warps)
            {
                int clampedX = warp.X;
                int clampedY = warp.Y;
                bool isBorder = false;
                int borderDir = -1;

                if (mapW > 0 && mapH > 0)
                {
                    // Bordo sinistro: X=-1 → il giocatore esce a sinistra
                    if (warp.X < 0) { clampedX = 0; isBorder = true; borderDir = 0; }
                    // Bordo destro: X>=mapW → il giocatore esce a destra
                    else if (warp.X >= mapW) { clampedX = mapW - 1; isBorder = true; borderDir = 1; }

                    // Bordo superiore: Y=-1 → il giocatore esce in alto
                    if (warp.Y < 0) { clampedY = 0; isBorder = true; if (borderDir < 0) borderDir = 2; }
                    // Bordo inferiore: Y>=mapH → il giocatore esce in basso
                    else if (warp.Y >= mapH) { clampedY = mapH - 1; isBorder = true; if (borderDir < 0) borderDir = 3; }
                }

                _graph[fromName].Add(new WarpEdge(
                    fromName, clampedX, clampedY,
                    warp.TargetName, warp.TargetX, warp.TargetY,
                    isBorder, borderDir
                ));
            }

            // CORREZIONE v2.1: aggiunge porte degli edifici come archi del grafo.
            // SDV gestisce l'accesso a FarmHouse, FishShop, ecc. tramite Building.humanDoor
            // (BuildingData.HumanDoor + building.tileX/tileY), NON tramite Warp in location.warps.
            // Senza questo loop, GetEntranceTile() non trova questi edifici → POI non risolti.
            if (location.buildings?.Count > 0)
            {
                foreach (Building building in location.buildings)
                {
                    try
                    {
                        string? indoorsName = building.GetIndoorsName();
                        if (string.IsNullOrEmpty(indoorsName)) continue;

                        var buildingData = building.GetData();
                        if (buildingData == null) continue;

                        // HumanDoor è il tile di ingresso relativo al top-left dell'edificio.
                        // Point.Zero → porta non configurata per questo tipo di edificio → skip.
                        Point door = buildingData.HumanDoor;
                        if (door == Point.Zero) continue;

                        int doorX = building.tileX.Value + door.X;
                        int doorY = building.tileY.Value + door.Y;

                        // Aggiunge solo se non già presente (evita duplicati con il loop warps)
                        if (!_graph[fromName].Any(e => e.ToLocation == indoorsName))
                        {
                            _graph[fromName].Add(new WarpEdge(
                                fromName, doorX, doorY,
                                indoorsName,
                                0, 0
                            ));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Trace($"Errore elaborazione porta edificio in {fromName}: {ex.Message}");
                    }
                }
            }
        }

        int totalEdges = CountEdges();
        Log.Debug($"Grafo costruito: {_graph.Count} location, {totalEdges} connessioni.");

        // Log diagnostico: lista tutte le location connesse per verifica destinations.json
        var connectedList = string.Join(", ", _graph.Keys.OrderBy(k => k));
        Log.Debug($"Location nel grafo: {connectedList}");
    }

    /// <summary>
    /// Calcola il percorso minimo (warp-only) da fromLocation a toLocationName usando BFS.
    /// Restituisce la sequenza di RouteStep (solo step di warp, nessuno step finale IsFinal).
    /// Se fromLocation == toLocationName: restituisce lista vuota (già sulla mappa, nessun warp necessario).
    /// Se non esiste percorso: restituisce null.
    /// </summary>
    public List<RouteStep>? FindRoute(string fromLocation, string toLocationName)
    {
        // Caso banale: già nella mappa giusta — nessun warp necessario
        if (fromLocation == toLocationName)
        {
            return new List<RouteStep>();
        }

        if (!_graph.ContainsKey(fromLocation))
        {
            Log.Warn($"Location di partenza non nel grafo: {fromLocation}");
            return null;
        }

        // BFS: cerca il cammino minimo in termini di salti di mappa
        var visited = new HashSet<string>();
        // Coda: (location corrente, path di WarpEdge percorse)
        var queue = new Queue<(string Location, List<WarpEdge> Path)>();
        queue.Enqueue((fromLocation, new()));
        visited.Add(fromLocation);

        while (queue.Count > 0)
        {
            var (current, path) = queue.Dequeue();

            if (!_graph.TryGetValue(current, out var edges))
                continue;

            foreach (WarpEdge edge in edges)
            {
                if (visited.Contains(edge.ToLocation))
                    continue;

                var newPath = new List<WarpEdge>(path) { edge };

                if (edge.ToLocation == toLocationName)
                {
                    // Percorso trovato: converti in RouteStep
                    return BuildSteps(newPath);
                }

                visited.Add(edge.ToLocation);
                queue.Enqueue((edge.ToLocation, newPath));
            }
        }

        Log.Warn($"Nessun percorso da {fromLocation} a {toLocationName}.");
        return null;
    }

    /// <summary>
    /// Restituisce il tile (clampato) del warp che connette fromMap a toLocation
    /// nel grafo warp. Usato da DestinationRegistry.ResolveCoordinates() per
    /// determinare le coordinate di arrivo dei POI con TargetLocationName.
    /// Restituisce null se la connessione non esiste nel grafo.
    /// </summary>
    public Point? GetEntranceTile(string fromMap, string toLocation)
    {
        if (!_graph.TryGetValue(fromMap, out var edges))
        {
            return null;
        }

        WarpEdge? edge = edges.FirstOrDefault(e => e.ToLocation == toLocation);
        if (edge == null)
        {
            return null;
        }

        return new Point(edge.FromX, edge.FromY);
    }

    /// <summary>
    /// Strategia "warp inverso"
    /// </summary>
    public Point? GetExitTile(string fromLocation, string toMap)
    {
        if (!_graph.TryGetValue(fromLocation, out var edges))
        {
            return null;
        }

        WarpEdge? edge = edges.FirstOrDefault(e => e.ToLocation == toMap);
        if (edge == null)
        {
            return null;
        }

        return new Point(edge.ToX, edge.ToY);
    }

    /// <summary>
    /// Converte la lista di WarpEdge BFS in RouteStep leggibili dal Navigator.
    /// </summary>
    private static List<RouteStep> BuildSteps(List<WarpEdge> edges)
    {
        var steps = new List<RouteStep>();

        for (int i = 0; i < edges.Count; i++)
        {
            WarpEdge edge = edges[i];

            steps.Add(new RouteStep
            {
                LocationName = edge.FromLocation,
                TargetTile = new Point(edge.FromX, edge.FromY),
                NextLocationName = edge.ToLocation,
                IsFinal = false,
                IsBorderWarp = edge.IsBorderWarp,
                BorderDirection = edge.BorderDirection,
                WarpTargetX = edge.ToX,
                WarpTargetY = edge.ToY
            });
        }

        return steps;
    }

    private int CountEdges()
    {
        int n = 0;
        foreach (var list in _graph.Values) n += list.Count;
        return n;
    }
}
