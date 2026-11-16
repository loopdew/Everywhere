namespace Everywhere.TextSelection;

/// <summary>
/// Specifies how text selection detection or the toolbar is triggered.
/// </summary>
public enum TextSelectionTriggerMode
{
    /// <summary>
    /// Automatically detects and pops up the toolbar whenever text is selected.
    /// </summary>
    [DynamicLocaleKey(LocaleKey.TextSelectionTriggerMode_Always)]
    Always = 0,

    /// <summary>
    /// Only triggers detection or pops up the toolbar if the configured modifier key is held during selection.
    /// </summary>
    [DynamicLocaleKey(LocaleKey.TextSelectionTriggerMode_ModifierKey)]
    ModifierKey = 1,

    /// <summary>
    /// Suppresses automatic popups on selection; requires invoking via explicit global shortcut.
    /// </summary>
    [DynamicLocaleKey(LocaleKey.TextSelectionTriggerMode_ShortcutOnly)]
    ShortcutOnly = 2,
}

/// <summary>
/// Specifies where AI execution results are displayed for text selection actions.
/// </summary>
public enum TextSelectionResultWindowMode
{
    /// <summary>
    /// Presents results in a compact, floating standalone window with pinning, streaming, and follow-up support.
    /// </summary>
    [DynamicLocaleKey(LocaleKey.TextSelectionResultWindowMode_Standalone)]
    Standalone = 0,

    /// <summary>
    /// Directly opens and runs the action within the main chat window.
    /// </summary>
    [DynamicLocaleKey(LocaleKey.TextSelectionResultWindowMode_MainChatWindow)]
    MainChatWindow = 1,
}

/// <summary>
/// Specifies the placement strategy for the text selection toolbar.
/// </summary>
public enum TextSelectionPositionMode
{
    /// <summary>
    /// Positions the toolbar near the cursor/selection anchor.
    /// </summary>
    [DynamicLocaleKey(LocaleKey.TextSelectionPositionMode_FollowCursor)]
    FollowCursor = 0,

    /// <summary>
    /// Remembers and restores the last dragged location of the toolbar across appearances.
    /// </summary>
    [DynamicLocaleKey(LocaleKey.TextSelectionPositionMode_RememberLast)]
    RememberLast = 1,
}
