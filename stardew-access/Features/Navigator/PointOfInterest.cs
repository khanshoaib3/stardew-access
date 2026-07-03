using Microsoft.Xna.Framework;
using System.Text.Json.Serialization;

namespace stardew_access.Features.Navigator;

/// <summary>
/// Rappresenta un punto di interesse raggiungibile all'interno di una mappa.
/// Due modalità di risoluzione coordinate:
/// - TargetLocationName valorizzato: le coordinate di ingresso vengono risolte
///   a runtime dal grafo warp di RouteEngine (per edifici/location separate).
/// - ArrivalX + ArrivalY valorizzati: coordinate esplicite nella mappa parent
///   (per punti che non sono location separate).
/// IsResolved diventa true dopo la chiamata a DestinationRegistry.ResolveCoordinates().
/// </summary>
public class PointOfInterest
{
    /// <summary>Nome leggibile mostrato nel menu Livello 2 (es. "Negozio di Pierre").</summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>
    /// Nome interno della GameLocation di destinazione finale (es. "SeedShop").
    /// Se valorizzato, le coordinate di arrivo vengono risolte dal grafo warp.
    /// Mutualmente esclusivo con ArrivalX/ArrivalY espliciti.
    /// </summary>
    public string? TargetLocationName { get; init; }

    /// <summary>
    /// Coordinata X esplicita del tile di arrivo nella mappa parent.
    /// Usato solo se TargetLocationName è null.
    /// </summary>
    public int? ArrivalX { get; init; }

    /// <summary>
    /// Coordinata Y esplicita del tile di arrivo nella mappa parent.
    /// Usato solo se TargetLocationName è null.
    /// </summary>
    public int? ArrivalY { get; init; }

    /// <summary>
    /// Tile di arrivo risolto a runtime. Valorizzato da DestinationRegistry.ResolveCoordinates().
    /// Non serializzato in JSON.
    /// </summary>
    [JsonIgnore]
    public Point ResolvedArrivalTile { get; set; }

    /// <summary>
    /// Nome della location in cui si trova il tile di arrivo.
    /// Per POI con TargetLocationName: è la mappa parent (ingresso edificio).
    /// Per POI con coordinate esplicite: è la mappa parent.
    /// Valorizzato da DestinationRegistry.ResolveCoordinates().
    /// </summary>
    [JsonIgnore]
    public string ResolvedLocationName { get; set; } = string.Empty;

    /// <summary>
    /// True se le coordinate sono state risolte con successo.
    /// POI con IsResolved=false sono esclusi dal menu.
    /// </summary>
    [JsonIgnore]
    public bool IsResolved { get; set; }
}
