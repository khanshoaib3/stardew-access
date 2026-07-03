using System.Collections.Generic;

namespace stardew_access.Features.Navigator;

/// <summary>
/// Rappresenta una mappa navigabile come destinazione di Livello 1.
/// Contiene una lista di punti di interesse (Livello 2) raggiungibili nella mappa.
/// I dati vengono caricati da assets/destinations.json al momento del SaveLoaded
/// e le coordinate vengono risolte a runtime tramite DestinationRegistry.ResolveCoordinates().
/// </summary>
public class MapDestination
{
    /// <summary>Nome leggibile mostrato nel menu Livello 1 (es. "Pelican Town").</summary>
    public string MapDisplayName { get; init; } = string.Empty;

    /// <summary>Nome interno della GameLocation SMAPI (es. "Town").</summary>
    public string MapLocationName { get; init; } = string.Empty;

    /// <summary>
    /// Lista dei punti di interesse accessibili nella mappa.
    /// Viene filtrata a soli POI risolti con successo dopo ResolveCoordinates().
    /// </summary>
    public List<PointOfInterest> PointsOfInterest { get; init; } = new();
}
