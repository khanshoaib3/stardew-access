using Microsoft.Xna.Framework;

namespace stardew_access.Features.Navigator;

/// <summary>
/// Rappresenta un singolo passo del percorso multimappa calcolato da RouteEngine.
/// Ogni step corrisponde a: "raggiungi il tile warp X,Y nella mappa corrente
/// per passare alla mappa successiva".
/// </summary>
public class RouteStep
{
    /// <summary>Nome della mappa in cui si trova questo step.</summary>
    public string LocationName { get; init; } = string.Empty;

    /// <summary>
    /// Tile obiettivo da raggiungere in questa mappa.
    /// Per step intermedi: è il tile del warp verso la mappa successiva.
    /// Per l'ultimo step: è il tile di arrivo della destinazione finale.
    /// </summary>
    public Point TargetTile { get; init; }

    /// <summary>Nome della mappa di destinazione dopo aver attraversato questo step.</summary>
    public string NextLocationName { get; init; } = string.Empty;

    /// <summary>True se questo è l'ultimo step del percorso (arrivo a destinazione).</summary>
    public bool IsFinal { get; init; }

    // --- Border warp data (solo per step che attraversano il bordo della mappa) ---

    /// <summary>
    /// True se il tile warp originale era fuori dai limiti della mappa (X &lt; 0, X &gt;= width, ecc.).
    /// PathFindController usa TargetTile (clampato), ma dopo il completamento occorre
    /// un push esplicito per attraversare il confine e attivare il warp SDV.
    /// </summary>
    public bool IsBorderWarp { get; init; }

    /// <summary>
    /// Direzione del confine da attraversare: 0=sinistra, 1=destra, 2=sopra, 3=sotto.
    /// Valido solo se IsBorderWarp = true.
    /// </summary>
    public int BorderDirection { get; init; }

    /// <summary>Coordinata X del tile di arrivo nella mappa di destinazione (warp.TargetX).</summary>
    public int WarpTargetX { get; init; }

    /// <summary>Coordinata Y del tile di arrivo nella mappa di destinazione (warp.TargetY).</summary>
    public int WarpTargetY { get; init; }
}
