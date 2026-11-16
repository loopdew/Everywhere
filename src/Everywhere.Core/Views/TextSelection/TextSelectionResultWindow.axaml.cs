using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Everywhere.AI;
using Everywhere.Chat;
using Everywhere.Interop;
using ShadUI;

namespace Everywhere.Views;

/// <summary>
/// A standalone floating window for presenting text-selection AI results with pinning,
/// streaming markdown display, follow-up questioning, and one-click transfer to the main chat.
/// </summary>
public sealed partial class TextSelectionResultWindow : ShadWindow
{
    private const int EdgeMargin = 8;

    public static readonly StyledProperty<string> ActionTitleProperty =
        AvaloniaProperty.Register<TextSelectionResultWindow, string>(nameof(ActionTitle), string.Empty);

    public string ActionTitle
    {
        get => GetValue(ActionTitleProperty);
        set => SetValue(ActionTitleProperty, value);
    }

    public static readonly StyledProperty<string?> AssistantNameProperty =
        AvaloniaProperty.Register<TextSelectionResultWindow, string?>(nameof(AssistantName));

    public string? AssistantName
    {
        get => GetValue(AssistantNameProperty);
        set => SetValue(AssistantNameProperty, value);
    }

    public static readonly StyledProperty<string> SelectedTextProperty =
        AvaloniaProperty.Register<TextSelectionResultWindow, string>(nameof(SelectedText), string.Empty);

    public string SelectedText
    {
        get => GetValue(SelectedTextProperty);
        set => SetValue(SelectedTextProperty, value);
    }

    public static readonly StyledProperty<ChatContext?> ChatContextProperty =
        AvaloniaProperty.Register<TextSelectionResultWindow, ChatContext?>(nameof(ChatContext));

    public ChatContext? ChatContext
    {
        get => GetValue(ChatContextProperty);
        set => SetValue(ChatContextProperty, value);
    }

    public static readonly StyledProperty<bool> IsPinnedProperty =
        AvaloniaProperty.Register<TextSelectionResultWindow, bool>(nameof(IsPinned));

    public bool IsPinned
    {
        get => GetValue(IsPinnedProperty);
        set => SetValue(IsPinnedProperty, value);
    }

    /// <summary>
    /// Raised when the user requests transferring the current session into the main chat window.
    /// </summary>
    public event Action<ChatContext>? OpenInMainRequested;

    private readonly IChatService _chatService;
    private readonly IWindowHelper _windowHelper;
    private CustomAssistant? _assistant;

    static TextSelectionResultWindow()
    {
        IsPinnedProperty.Changed.AddClassHandler<TextSelectionResultWindow>((w, e) =>
        {
            if (e.NewValue is bool isPinned)
            {
                w.Topmost = isPinned;
            }
        });
    }

    public TextSelectionResultWindow(IChatService chatService, IWindowHelper windowHelper)
    {
        _chatService = chatService;
        _windowHelper = windowHelper;
        InitializeComponent();

        windowHelper.SetWindowProperties(this, inputMode: WindowInputMode.Normal, layer: WindowLayer.Normal);
        Deactivated += HandleDeactivated;
        AddHandler(KeyDownEvent, HandleWindowKeyDown, RoutingStrategies.Tunnel);
    }

    /// <summary>
    /// Shows the standalone result window for the given chat context and selection snapshot.
    /// </summary>
    public void ShowFor(
        ChatContext chatContext,
        string actionTitle,
        string selectedText,
        CustomAssistant? assistant,
        PixelPoint anchor)
    {
        Dispatcher.UIThread.VerifyAccess();

        ChatContext = chatContext;
        ActionTitle = actionTitle;
        SelectedText = selectedText;
        AssistantName = assistant?.Name;
        _assistant = assistant;
        FollowUpTextBox.Text = string.Empty;

        // Position near the anchor, clamped to the target screen's working area
        var size = new PixelSize((int)(Width * DesktopScaling), (int)(Height * DesktopScaling));
        var screen = Screens.ScreenFromPoint(anchor) ?? Screens.Primary ?? Screens.All.FirstOrDefault();
        var workingArea = screen?.WorkingArea ?? new PixelRect(anchor, size);

        var targetX = anchor.X + 24;
        var targetY = anchor.Y + 24;

        if (targetX + size.Width + EdgeMargin > workingArea.Right)
        {
            targetX = workingArea.Right - size.Width - EdgeMargin;
        }

        if (targetY + size.Height + EdgeMargin > workingArea.Bottom)
        {
            targetY = workingArea.Bottom - size.Height - EdgeMargin;
        }

        targetX = Math.Max(workingArea.X + EdgeMargin, targetX);
        targetY = Math.Max(workingArea.Y + EdgeMargin, targetY);

        Position = new PixelPoint(targetX, targetY);

        if (!IsVisible)
        {
            Show();
        }

        _windowHelper.BringToForeground(this);
        FollowUpTextBox.Focus();
    }

    private void HandleWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && !e.KeyModifiers.HasFlag(KeyModifiers.Shift) && !e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            e.Handled = true;
            Hide();
        }
    }

    private void HandleDeactivated(object? sender, EventArgs e)
    {
        if (!IsPinned && IsVisible)
        {
            // Automatically close unpinned result window when losing focus
            Hide();
        }
    }

    private void HandleOpenInMainClick(object? sender, RoutedEventArgs e)
    {
        if (ChatContext is { } context)
        {
            OpenInMainRequested?.Invoke(context);
        }
        Hide();
    }

    private async void HandleCopyResultClick(object? sender, RoutedEventArgs e)
    {
        var context = ChatContext;
        if (context is null) return;

        var lastAssistant = context.DisplayItems
            .Select(node => node.Message)
            .OfType<AssistantChatMessage>()
            .LastOrDefault();

        var text = lastAssistant?.ToString();
        if (string.IsNullOrWhiteSpace(text)) return;

        await App.Clipboard.SetTextAsync(text);
        ToastManager.Success(LocaleKey.TextSelectionResultWindow_Copied.I18N());
    }

    private void HandleFollowUpKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && !e.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            e.Handled = true;
            SendFollowUp();
        }
    }

    private void HandleSendClick(object? sender, RoutedEventArgs e)
    {
        SendFollowUp();
    }

    private void SendFollowUp()
    {
        var text = FollowUpTextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(text) || ChatContext is null) return;

        FollowUpTextBox.Text = string.Empty;
        var userMessage = new UserChatMessage(text, []);
        _chatService.SendMessage(ChatContext, userMessage, _assistant);

        // Scroll to bottom
        Dispatcher.UIThread.Post(() => MessageScrollViewer.ScrollToEnd(), DispatcherPriority.Background);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (e.CloseReason is not WindowCloseReason.ApplicationShutdown and not WindowCloseReason.OSShutdown)
        {
            e.Cancel = true;
            Hide();
        }
        base.OnClosing(e);
    }
}
