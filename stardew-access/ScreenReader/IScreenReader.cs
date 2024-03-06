using stardew_access.Translation;

namespace stardew_access.ScreenReader;

public interface IScreenReader
{
    public string PrevTextTile
    {
        get;
        set;
    }

    public string PrevMenuQueryText
    {
        get;
        set;
    }

    public string MenuPrefixText
    {
        get;
        set;
    }

    public string MenuSuffixText
    {
        get;
        set;
    }

    public string MenuPrefixNoQueryText
    {
        get;
        set;
    }

    public string MenuSuffixNoQueryText
    {
        get;
        set;
    }

    /// <summary>Initializes the screen reader.</summary>
    public void InitializeScreenReader();

    /// <summary>Closes the screen reader, this is important, call this function when closing the game.</summary>
    public void CloseScreenReader();

    /// <summary>Speaks the text via the loaded screen reader (if any).</summary>
    /// <param name="text">The text to be narrated.</param>
    /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
    /// <returns>true if the text was spoken otherwise false.</returns>
    public bool Say(string text, bool interrupt);

    /// <summary>
    /// Speaks the translated text via the loaded screen reader (if any).
    /// </summary>
    /// <param name="translationKey">The key used to look up the translation of the text to be narrated.</param>
    /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
    /// <param name="translationTokens">Optional tokens to be used for formatting the translation.</param>
    /// <param name="disableTranslationWarnings">Whether to disable translation not found warning or not.</param>
    /// <returns>true if the text was spoken otherwise, false.</returns>
    public bool TranslateAndSay(string translationKey, bool interrupt, object? translationTokens = null, TranslationCategory translationCategory = TranslationCategory.Default, bool disableTranslationWarnings = false);

    /// <summary>Speaks the text via the loaded screen reader (if any).
    /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.</summary>
    /// <param name="text">The text to be narrated.</param>
    /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
    /// <param name="customQuery">If set, uses this instead of <paramref name="text"/> as query to check whether to speak the text or not.</param>
    /// <returns>true if the text was spoken otherwise false.</returns>
    public bool SayWithChecker(string text, bool interrupt, string? customQuery = null);

    /// <summary>
    /// Speaks the translated text via the loaded screen reader (if any).
    /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
    /// </summary>
    /// <param name="translationKey">The key used to look up the translation of the text to be narrated.</param>
    /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
    /// <param name="translationTokens">Optional tokens to be used for formatting the translation.</param>
    /// <param name="customQuery">If set, uses this instead of the translated text as a query to check whether to speak the text or not.</param>
    /// <param name="disableTranslationWarnings">Whether to disable translation warnings or not.</param>
    /// <returns>true if the text was spoken otherwise, false.</returns>
    public bool TranslateAndSayWithChecker(string translationKey, bool interrupt, object? translationTokens = null, TranslationCategory translationCategory = TranslationCategory.Default, string? customQuery = null, bool disableTranslationWarnings = false);

    /// <summary>Speaks the text via the loaded screen reader (if any).
    /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
    /// <br/><br/>Use this when narrating hovered component in menus to avoid interference.</summary>
    /// <param name="text">The text to be narrated.</param>
    /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
    /// <param name="customQuery">If set, uses this instead of <paramref name="text"/> as query to check whether to speak the text or not.</param>
    /// <returns>true if the text was spoken otherwise false.</returns>
    public bool SayWithMenuChecker(string text, bool interrupt, string? customQuery = null);

    /// <summary>
    /// Speaks the translated text via the loaded screen reader (if any).
    /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
    /// <br/><br/>Use this method when narrating
    /// hovered components in menus to avoid interference.
    /// </summary>
    /// <param name="translationKey">The key used to look up the translation of the text to be narrated.</param>
    /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
    /// <param name="translationTokens">Optional tokens to be used for formatting the translation.</param>
    /// <param name="customQuery">If set, uses this instead of the translated text as a query to check whether to speak the text or not.</param>
    /// <param name="disableTranslationWarnings">Whether to disable translation warnings or not.</param>
    /// <returns>true if the text was spoken otherwise, false.</returns>
    public bool TranslateAndSayWithMenuChecker(string translationKey, bool interrupt, object? translationTokens = null, TranslationCategory translationCategory = TranslationCategory.Menu, string? customQuery = null, bool disableTranslationWarnings = false);

    /// <summary>Speaks the text via the loaded screen reader (if any).
    /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
    /// <br/><br/>Use this when narrating chat messages to avoid interference.</summary>
    /// <param name="text">The text to be narrated.</param>
    /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
    /// <returns>true if the text was spoken otherwise false.</returns>
    public bool SayWithChatChecker(string text, bool interrupt);

    /// <summary>Speaks the text via the loaded screen reader (if any).
    /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
    /// <br/><br/>Use this when narrating texts based on tile position to avoid interference.</summary>
    /// <param name="text">The text to be narrated.</param>
    /// <param name="x">The X location of tile.</param>
    /// <param name="y">The Y location of tile.</param>
    /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
    /// <returns>true if the text was spoken otherwise false.</returns>
    public bool SayWithTileQuery(string text, int x, int y, bool interrupt);
    
    /// <summary>
    /// Is called when a menu is closed and clears the menu queries.
    /// </summary>
    public void Cleanup();
}
