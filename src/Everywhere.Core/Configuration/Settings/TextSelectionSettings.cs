using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Everywhere.ProcessIsolation.Automation;
using Everywhere.TextSelection;
using Everywhere.Views;
using Lucide.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using ShadUI;

namespace Everywhere.Configuration;

/// <summary>
/// Stores selection behavior and independently persisted platform application rules.
/// </summary>
[GeneratedSettingsItems]
public sealed partial class TextSelectionSettings(IServiceProvider serviceProvider) : SettingsBase(serviceProvider), ISettingsCategory
{
    [SettingsItemIgnore]
    public int Index => 5;

    [SettingsItemIgnore]
    public LucideIconKind Icon => LucideIconKind.TextSelect;

    [SettingsItemIgnore]
    public IDynamicLocaleKey TitleKey { get; } = new DynamicLocaleKey(LocaleKey.SettingsCategory_Settings_TextSelection_Header);

    [SettingsItemIgnore]
    public IDynamicLocaleKey? DescriptionKey { get; } = new DynamicLocaleKey(LocaleKey.SettingsCategory_Settings_TextSelection_Description);

    public const int MaxVisibleActionCount = 12;

    /// <summary>
    /// Gets or sets the last acknowledged toolbar intent, independently of automatic attachment capture.
    /// </summary>
    [ObservableProperty]
    [SettingsItemIgnore]
    public partial bool IsToolbarEnabled { get; set; }

    [JsonIgnore]
    [DynamicLocaleKey(LocaleKey.TextSelectionSettings_IsToolbarEnabled_Header)]
    [SettingsItem(Group = LocaleKey.TextSelectionSettings_Group_Toolbar)]
    public SettingsControl<AsyncToggleSwitch> ToolbarMonitoringControl { get; } = new(
        sp =>
        {
            var controller = sp.GetRequiredService<TextSelectionController>();
            var control = new AsyncToggleSwitch
            {
                Command = controller.SetToolbarEnabledCommand,
                [!ToggleButton.IsCheckedProperty] = CompiledBinding.Create(
                    (TextSelectionController source) => source.IsToolbarEnabled,
                    source: controller,
                    mode: BindingMode.OneWay),
                [!AsyncToggleSwitch.IsBusyProperty] = CompiledBinding.Create(
                    (TextSelectionController source) => source.IsBusy,
                    source: controller),
            };
            return control;
        },
        isCacheEnabled: false);

    [JsonIgnore]
    [DynamicLocaleKey(LocaleKey.TextSelectionSettings_ToolbarPreview_Header)]
    [SettingsItem(
        IsVisibleBindingPath = nameof(IsToolbarEnabled),
        Classes = ["FullWidth", "NoHeading"],
        Group = LocaleKey.TextSelectionSettings_Group_Toolbar)]
    public SettingsControl<TextSelectionToolbarPreview> ToolbarPreview { get; } = new();

    /// <summary>
    /// Gets or sets the toolbar opacity, from 0.1 to fully opaque.
    /// </summary>
    [DynamicLocaleKey(LocaleKey.TextSelectionSettings_ToolbarOpacity_Header)]
    [SettingsDoubleItem(
        IsVisibleBindingPath = nameof(IsToolbarEnabled),
        Min = 0.1,
        Max = 1,
        Step = 0.05,
        IsTextBoxVisible = false,
        Group = LocaleKey.TextSelectionSettings_Group_Toolbar)]
    public double ToolbarOpacity
    {
        get;
        set => SetProperty(ref field, double.IsNaN(value) ? 1d : Math.Clamp(value, 0.1d, 1d));
    } = 1d;

    /// <summary>
    /// Gets or sets whether the toolbar fades as the pointer moves away.
    /// </summary>
    [ObservableProperty]
    [DynamicLocaleKey(
        LocaleKey.TextSelectionSettings_IsToolbarOpacityAdaptive_Header,
        LocaleKey.TextSelectionSettings_IsToolbarOpacityAdaptive_Description)]
    [SettingsItem(
        IsVisibleBindingPath = nameof(IsToolbarEnabled),
        Group = LocaleKey.TextSelectionSettings_Group_Toolbar)]
    public partial bool IsToolbarOpacityAdaptive { get; set; }

    /// <summary>
    /// Gets or sets whether the freeform chat is enabled.
    /// </summary>
    [ObservableProperty]
    [DynamicLocaleKey(
        LocaleKey.TextSelectionSettings_IsFreeformChatEnabled_Header,
        LocaleKey.TextSelectionSettings_IsFreeformChatEnabled_Description)]
    [SettingsItem(
        IsVisibleBindingPath = nameof(IsToolbarEnabled),
        Group = LocaleKey.TextSelectionSettings_Group_Toolbar)]
    public partial bool IsFreeformChatEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of tools shown directly. Remaining tools are available in the menu.
    /// </summary>
    [ObservableProperty]
    [DynamicLocaleKey(LocaleKey.TextSelectionSettings_VisibleActionCount_Header)]
    [SettingsIntegerItem(
        IsVisibleBindingPath = nameof(IsToolbarEnabled),
        Min = 0,
        Max = MaxVisibleActionCount,
        IsTextBoxVisible = false,
        Group = LocaleKey.TextSelectionSettings_Group_Toolbar)]
    public partial int VisibleActionCount { get; set; } = 5;

    /// <summary>
    /// Gets or sets how the text selection toolbar or assistant is triggered.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsModifierTriggerMode))]
    [DynamicLocaleKey(
        LocaleKey.TextSelectionSettings_TriggerMode_Header,
        LocaleKey.TextSelectionSettings_TriggerMode_Description)]
    [SettingsItem(
        IsVisibleBindingPath = nameof(IsToolbarEnabled),
        Group = LocaleKey.TextSelectionSettings_Group_Toolbar)]
    public partial TextSelectionTriggerMode TriggerMode { get; set; } = TextSelectionTriggerMode.Always;

    /// <summary>
    /// Gets or sets the modifier key required when TriggerMode is ModifierKey.
    /// </summary>
    [ObservableProperty]
    [DynamicLocaleKey(
        LocaleKey.TextSelectionSettings_TriggerModifierKey_Header,
        LocaleKey.TextSelectionSettings_TriggerModifierKey_Description)]
    [SettingsItem(
        IsVisibleBindingPath = nameof(IsModifierTriggerMode),
        Group = LocaleKey.TextSelectionSettings_Group_Toolbar)]
    public partial KeyModifiers TriggerModifierKey { get; set; } = KeyModifiers.Alt;

    [JsonIgnore]
    public bool IsModifierTriggerMode => IsToolbarEnabled && TriggerMode == TextSelectionTriggerMode.ModifierKey;

    /// <summary>
    /// Gets or sets whether AI results are displayed in a compact standalone window or the main chat window.
    /// </summary>
    [ObservableProperty]
    [DynamicLocaleKey(
        LocaleKey.TextSelectionSettings_ResultWindowMode_Header,
        LocaleKey.TextSelectionSettings_ResultWindowMode_Description)]
    [SettingsItem(
        IsVisibleBindingPath = nameof(IsToolbarEnabled),
        Group = LocaleKey.TextSelectionSettings_Group_Toolbar)]
    public partial TextSelectionResultWindowMode ResultWindowMode { get; set; } = TextSelectionResultWindowMode.Standalone;

    /// <summary>
    /// Gets or sets whether the toolbar follows the selection cursor or remembers its last dragged position.
    /// </summary>
    [ObservableProperty]
    [DynamicLocaleKey(
        LocaleKey.TextSelectionSettings_PositionMode_Header,
        LocaleKey.TextSelectionSettings_PositionMode_Description)]
    [SettingsItem(
        IsVisibleBindingPath = nameof(IsToolbarEnabled),
        Group = LocaleKey.TextSelectionSettings_Group_Toolbar)]
    public partial TextSelectionPositionMode PositionMode { get; set; } = TextSelectionPositionMode.FollowCursor;

    /// <summary>
    /// Persisted physical pixel position when PositionMode is RememberLast.
    /// </summary>
    [ObservableProperty]
    [SettingsItemIgnore]
    public partial PixelPoint? LastToolbarPosition { get; set; }

    [SettingsItemIgnore]
    public ObservableCollection<TextSelectionToolbarAction> ToolbarActions { get; } = [];

    [JsonIgnore]
    [DynamicLocaleKey(LocaleKey.Empty)]
    [SettingsItem(
        IsVisibleBindingPath = nameof(IsToolbarEnabled),
        Classes = ["FullWidth", "NoHeading"],
        Group = LocaleKey.TextSelectionSettings_Group_Toolbar)]
    public SettingsControl<TextSelectionToolbarActionsControl> ToolbarActionsControl { get; } = new();

    /// <summary>
    /// Gets or sets the last acknowledged automatic attachment intent.
    /// </summary>
    [ObservableProperty]
    [SettingsItemIgnore]
    public partial bool IsAutomaticAttachmentEnabled { get; set; }

    [JsonIgnore]
    [DynamicLocaleKey(
        LocaleKey.TextSelectionSettings_IsAutomaticAttachmentEnabled_Header,
        LocaleKey.TextSelectionSettings_IsAutomaticAttachmentEnabled_Description)]
    [SettingsItem(Group = LocaleKey.TextSelectionSettings_Group_ChatWindow)]
    public SettingsControl<AsyncToggleSwitch> AutomaticAttachmentControl { get; } = new(
        sp =>
        {
            var controller = sp.GetRequiredService<TextSelectionController>();
            var control = new AsyncToggleSwitch
            {
                Command = controller.SetAutomaticAttachmentEnabledCommand,
                [!ToggleButton.IsCheckedProperty] = CompiledBinding.Create(
                    (TextSelectionController source) => source.IsAutomaticAttachmentEnabled,
                    source: controller,
                    mode: BindingMode.OneWay),
                [!AsyncToggleSwitch.IsBusyProperty] = CompiledBinding.Create(
                    (TextSelectionController source) => source.IsBusy,
                    source: controller),
            };
            return control;
        },
        isCacheEnabled: false);

    [ObservableProperty]
    [DynamicLocaleKey(
        LocaleKey.TextSelectionSettings_IsFullscreenExcluded_Header,
        LocaleKey.TextSelectionSettings_IsFullscreenExcluded_Description)]
    [SettingsItem(Group = LocaleKey.TextSelectionSettings_Group_Advanced)]
    public partial bool IsFullscreenExcluded { get; set; } = true;

    [JsonIgnore]
    [DynamicLocaleKey(LocaleKey.Empty)]
    [SettingsItem(Classes = ["FullWidth", "NoHeading"], Group = LocaleKey.TextSelectionSettings_Group_Advanced)]
    public SettingsControl<TextSelectionApplicationsControl> ExcludedApplicationsControl => new(sp =>
    {
        var control = sp.GetRequiredService<TextSelectionApplicationsControl>();
        control.HeaderKey = new DynamicLocaleKey(LocaleKey.TextSelectionSettings_ExcludedApplications_Header);
        control.DescriptionKey = new DynamicLocaleKey(LocaleKey.TextSelectionSettings_ExcludedApplications_Description);
        control.IsCompatibilityHintVisible = true;
        control[!TextSelectionApplicationsControl.ApplicationsProperty] = CompiledBinding.Create(
            (TextSelectionApplicationSettings settings) => settings.ExcludedApplications,
            source: Platform,
            mode: BindingMode.TwoWay);
        return control;
    });

    [JsonIgnore]
    [DynamicLocaleKey(LocaleKey.Empty)]
    [SettingsItem(
        IsVisibleBindingPath = nameof(IsToolbarEnabled),
        Classes = ["FullWidth", "NoHeading"],
        Group = LocaleKey.TextSelectionSettings_Group_Advanced)]
    public SettingsControl<TextSelectionApplicationsControl> ToolbarExcludedApplicationsControl => new(sp =>
    {
        var control = sp.GetRequiredService<TextSelectionApplicationsControl>();
        control.HeaderKey = new DynamicLocaleKey(LocaleKey.TextSelectionSettings_ToolbarExcludedApplications_Header);
        control.DescriptionKey = new DynamicLocaleKey(LocaleKey.TextSelectionSettings_ToolbarExcludedApplications_Description);
        control[!TextSelectionApplicationsControl.ApplicationsProperty] = CompiledBinding.Create(
            (TextSelectionApplicationSettings settings) => settings.ToolbarExcludedApplications,
            source: Platform,
            mode: BindingMode.TwoWay);
        return control;
    });

    // Both subtrees remain in the serialized model so importing settings never discards another platform's rules.
    [SettingsItemIgnore]
    public WindowsTextSelectionSettings Windows { get; } = new();

    [SettingsItemIgnore]
    public MacTextSelectionSettings Mac { get; } = new();

    [SettingsItemIgnore]
    public LinuxTextSelectionSettings Linux { get; } = new();

    [JsonIgnore]
    [SettingsItemIgnore]
    public TextSelectionApplicationSettings Platform
    {
        get
        {
#if MACOS
            return Mac;
#elif WINDOWS
            return Windows;
#elif LINUX
            return Linux;
#else
#error Implement text-selection application settings for this platform.
#endif
        }
    }

    /// <summary>
    /// Creates the native monitoring policy for the current platform.
    /// </summary>
    public TextSelectionMonitoringConfiguration CreateMonitoringConfiguration()
    {
        return new TextSelectionMonitoringConfiguration
        {
            IsFullscreenApplicationExcluded = IsFullscreenExcluded,
            ExcludedApplications = Platform.ExcludedApplications,
            CopyFallbackExcludedApplications = Platform.CopyFallbackExcludedApplications,
            TriggerMode = TriggerMode,
            TriggerModifierKey = TriggerModifierKey,
        };
    }
}

/// <summary>
/// Stores exact native application identities. Updates replace arrays atomically for settings observation.
/// </summary>
public abstract partial class TextSelectionApplicationSettings : ObservableObject
{
    [ObservableProperty]
    public partial string[] ExcludedApplications { get; set; } = [];

    /// <summary>
    /// Applications in which only the floating toolbar is suppressed; automatic attachment remains available.
    /// </summary>
    [ObservableProperty]
    public partial string[] ToolbarExcludedApplications { get; set; } = [];

    /// <summary>
    /// Retains existing compatibility overrides without exposing an editor in this release.
    /// </summary>
    [ObservableProperty]
    public partial string[] CopyFallbackExcludedApplications { get; set; } = [];

    /// <summary>
    /// Matches an identity captured with the selection, including manually entered executable names on Windows.
    /// </summary>
    public bool IsToolbarExcluded(string? identity)
    {
        if (string.IsNullOrEmpty(identity))
        {
            return false;
        }

#if WINDOWS
        var processName = GetFileName(identity);
        return ToolbarExcludedApplications.Any(application =>
            string.Equals(application, identity, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(application, processName, StringComparison.OrdinalIgnoreCase));
#elif MACOS
        return ToolbarExcludedApplications.Contains(identity, StringComparer.Ordinal);
#elif LINUX
        var processName = GetFileName(identity);
        return ToolbarExcludedApplications.Any(application =>
            string.Equals(application, identity, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(application, processName, StringComparison.OrdinalIgnoreCase));
#else
#error Implement toolbar application identity matching for this platform.
#endif
    }

    private static string GetFileName(string path)
    {
        var lastSeparator = Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
        return lastSeparator >= 0 ? path[(lastSeparator + 1)..] : path;
    }
}

/// <summary>
/// Windows rules match absolute executable paths or executable file names, ignoring case.
/// </summary>
public sealed class WindowsTextSelectionSettings : TextSelectionApplicationSettings;

/// <summary>
/// macOS rules match exact, case-sensitive application bundle identifiers.
/// </summary>
public sealed class MacTextSelectionSettings : TextSelectionApplicationSettings;

/// <summary>
/// Linux rules match executable names or paths.
/// </summary>
public sealed class LinuxTextSelectionSettings : TextSelectionApplicationSettings;