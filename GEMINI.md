# GEMINI.md — stardew-access Fork Guidelines

This document provides context, architectural constraints, rules, and workflows for any AI assistant developing on this repository.

---

## 1. Contesto del progetto

### Scopo della mod
`stardew-access` è un mod di accessibilità per Stardew Valley che consente a giocatori non vedenti o ipovedenti di giocare in modo autonomo. La mod integra screen reader (tramite la libreria TOWK.Utility.CrossSpeak) e fornisce descrizioni audio dei menu, del posizionamento dei tile, dei radar e del movimento del giocatore.

### Tecnologie e Dipendenze
- **Linguaggio**: C#
- **Framework**: .NET 6.0
- **Ecosistema**: SMAPI (Stardew Modding API) versione 4.0+
- **Librerie Chiave**:
  - `TOWK.Utility.CrossSpeak` (per l'interfaccia screen reader)
  - `Lib.Harmony` (per i patch a runtime del codice del gioco)
  - `KdTree` (per l'indicizzazione spaziale dei tile calpestabili)
  - `ProjectFluent` (per la localizzazione dinamica tramite file `.ftl`)

### Relazione e Ruolo del Fork
Questo repository è un **fork** di `stardew-access/stardew-access` (branch attivo: `feature/navigator`, originato da `development`). 
- **Vincolo Upstream**: Qualsiasi modifica in questo fork deve rimanere rigorosamente compatibile con il progetto principale upstream.
- **Obiettivo**: Implementare ed affinare nuove funzionalità (come il modulo Navigator) mantenendo i pattern originali per facilitare una futura pull request (come PR #549).

---

## 2. Struttura del codice

Il progetto è suddiviso nella cartella `stardew-access` con la seguente struttura:

- [stardew-access/](file:///C:/Users/nemex/OneDrive/Documenti/GitHub/stardew-access-fork/stardew-access/)
  - [Commands/](file:///C:/Users/nemex/OneDrive/Documenti/GitHub/stardew-access-fork/stardew-access/Commands/): Gestione comandi di console SMAPI.
  - [Features/](file:///C:/Users/nemex/OneDrive/Documenti/GitHub/stardew-access-fork/stardew-access/Features/): Moduli funzionali del mod (es. `ObjectTracker.cs`, `GridMovement.cs`, `Radar.cs`).
    - [Navigator/](file:///C:/Users/nemex/OneDrive/Documenti/GitHub/stardew-access-fork/stardew-access/Features/Navigator/): Sottocartella per il modulo Navigator.
    - [FeatureBase.cs](file:///C:/Users/nemex/OneDrive/Documenti/GitHub/stardew-access-fork/stardew-access/Features/FeatureBase.cs): Classe base astratta per il ciclo di vita dei moduli.
    - [FeatureManager.cs](file:///C:/Users/nemex/OneDrive/Documenti/GitHub/stardew-access-fork/stardew-access/Features/FeatureManager.cs): Gestore centrale che registra e propaga gli eventi SMAPI ai vari moduli.
  - [Framework/](file:///C:/Users/nemex/OneDrive/Documenti/GitHub/stardew-access-fork/stardew-access/Framework/): Modelli di dati e strutture core.
  - [Patches/](file:///C:/Users/nemex/OneDrive/Documenti/GitHub/stardew-access-fork/stardew-access/Patches/): Patch Harmony per deviare l'esecuzione del codice di gioco originale.
  - [ScreenReader/](file:///C:/Users/nemex/OneDrive/Documenti/GitHub/stardew-access-fork/stardew-access/ScreenReader/): Wrapper e interfacce di sintesi vocale (`IScreenReader.cs`, `ScreenReaderImpl.cs`).
  - [Tiles/](file:///C:/Users/nemex/OneDrive/Documenti/GitHub/stardew-access-fork/stardew-access/Tiles/): Gestione spaziale della griglia calpestabile (`AccessibleTileManager.cs`).
  - [Translation/](file:///C:/Users/nemex/OneDrive/Documenti/GitHub/stardew-access-fork/stardew-access/Translation/): Integrazione Project Fluent.
  - [assets/](file:///C:/Users/nemex/OneDrive/Documenti/GitHub/stardew-access-fork/stardew-access/assets/): File di configurazione statica del navigatore (`navigator_destinations.json`).
  - [i18n/](file:///C:/Users/nemex/OneDrive/Documenti/GitHub/stardew-access-fork/stardew-access/i18n/): File di localizzazione `.ftl` (Project Fluent).

---

## 3. Build, test e deploy locale

### Prerequisiti
- .NET 6.0 SDK installato.
- Gioco Stardew Valley installato.
- File `stardew-access.csproj.user` configurato per indicare la cartella del gioco:
  ```xml
  <Project>
    <PropertyGroup>
      <GamePath>C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley</GamePath>
    </PropertyGroup>
  </Project>
  ```

### Comandi Build
Esegui i comandi nella root del repository:

- **Build Debug**:
  ```powershell
  & "C:\Users\nemex\AppData\Local\dotnet\dotnet.exe" build stardew-access.sln
  ```
- **Build Release**:
  ```powershell
  & "C:\Users\nemex\AppData\Local\dotnet\dotnet.exe" build stardew-access.sln --configuration Release
  ```

### Deploy Locale
Il pacchetto `Pathoschild.Stardew.ModBuildConfig` copia automaticamente l'output compilato nella cartella `Mods/` di Stardew Valley indicata nel file `.csproj.user`.

---

## 4. Regole di sviluppo permanenti

- **Always** verificare che la compilazione Release termini con 0 errori e 0 avvisi prima di committare.
- **Never** committare modifiche a file machine-specific come `stardew-access.csproj.user`. Devono rimanere untracked.
- **Must** utilizzare **Conventional Commits** per ogni commit (es. `feat(navigator): ...`, `fix(patches): ...`).
- **Must** ereditare da `FeatureBase` per implementare nuove caratteristiche e integrarle tramite `FeatureManager.cs`.
- **Must** usare Project Fluent per ogni stringa rivolta all'utente, posizionando le traduzioni nei file `.ftl` dentro `stardew-access/i18n/`.
- **Always** chiedere conferma esplicita all'utente prima di applicare refactoring architetturali o modifiche strutturali ai patch Harmony.
- **Never** assumere dettagli di gioco non documentati: in caso di dubbi sui tile o logiche di warp, investigare o chiedere chiarimenti.

---

## 5. Pattern architetturali e convenzioni interne

### Gestione dei Moduli (Feature Lifecycle)
Il ciclo di vita di ogni funzionalità è regolato da `FeatureBase.cs`. Ogni modulo deve implementare:
- `Update`: per il codice eseguito ad ogni tick di gioco.
- `OnButtonPressed`: per catturare input specifici del giocatore.
- `OnPlayerWarped`: per rispondere ai cambi di mappa.

### Sistema i18n con Project Fluent
`stardew-access` si affida a Project Fluent. Per recuperare le traduzioni:
```csharp
Translator.Instance.Translate("chiave-fluent", new { parametro = valore }, TranslationCategory.Menu)
```
Tutte le chiavi e i relativi testi devono essere presenti in `i18n/en.ftl` (e negli altri file di traduzione).

### Logger Centralizzato
Non usare `Console.WriteLine` o `this.Monitor.Log` direttamente nel codice dei moduli. Usa la classe statica `Log` interna:
```csharp
Log.Debug("messaggio");
Log.Error("errore");
```

---

## 6. Git workflow e release management

### Convenzioni Commit
Tutti i commit devono seguire il formato:
`<tipo>(<ambito>): <descrizione>`
Tipi supportati: `feat`, `fix`, `docs`, `refactor`, `perf`, `chore`.

### Semantic Versioning & Tags
- La versione è determinata da `manifest.json`.
- I tag devono utilizzare la convenzione `vMAJOR.MINOR.PATCH` (es. `v1.7.0`).
- Pre-release contrassegnate con suffissi `-beta.X` o `-alpha.X`.
- I tag sono considerati immutabili. Non riscrivere mai la history dei tag su origin.

---

## 7. Stato attuale e prossimi passi

- **Versione Corrente**: `1.7.0-beta.2` (tracciata nel manifest.json).
- **Prossimi punti di attenzione**:
  - Garantire che le modifiche apportate nel modulo `Navigator` rimangano pulite e compatibili con il modulo principale.
  - Testare accuratamente le regressioni sul movimento dei tile in presenza di mod che espandono la mappa.
