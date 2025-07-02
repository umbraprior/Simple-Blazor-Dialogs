using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using Simple.Blazor.Dialogs.Models;

namespace Simple.Blazor.Dialogs.Services;

public class DialogService : IDisposable
{
    private readonly ObservableCollection<DialogItem> _dialogs = new();
    private readonly object _lock = new object();
    
    public IReadOnlyCollection<DialogItem> Dialogs => _dialogs;
    
    public DialogTheme Theme { get; private set; } = DialogTheme.Dark;
    
    public DialogAnimation Animation { get; private set; } = DialogAnimation.FadeAndScale;
    
    public event Action? OnDialogsChanged;

    public DialogService()
    {
        // Pre-warm the system
        Task.Run(async () =>
        {
            await Task.Delay(200);
            OnDialogsChanged?.Invoke();
        });
    }

    public string ShowDialog(
        bool closeOnClickOutside = true, 
        bool showCloseButton = true, 
        string? outlineColor = null,
        DialogSize size = DialogSize.Medium,
        string? customSize = null,
        DialogColor color = DialogColor.Default,
        DialogBackgroundEffect backgroundEffect = DialogBackgroundEffect.Dim,
        bool enableScroller = true,
        bool enableFocusTrap = true,
        bool disableBackgroundScrolling = true)
    {
        var dialog = new DialogItem
        {
            CloseOnClickOutside = closeOnClickOutside,
            ShowCloseButton = showCloseButton,
            OutlineColor = outlineColor,
            Size = size,
            CustomSize = customSize,
            Color = color,
            BackgroundEffect = backgroundEffect,
            EnableScroller = enableScroller,
            EnableFocusTrap = enableFocusTrap,
            DisableBackgroundScrolling = disableBackgroundScrolling
        };

        return ShowDialog(dialog);
    }

    public string ShowDialog(DialogItem dialog)
    {
        lock (_lock)
        {
            // Check for background effect stacking logic
            if (dialog.BackgroundEffect != DialogBackgroundEffect.None)
            {
                var existingEffectDialog = _dialogs.FirstOrDefault(d => 
                    d.BackgroundEffect != DialogBackgroundEffect.None && 
                    d.IsVisible && 
                    !d.IsRemoving);
                
                if (existingEffectDialog != null)
                {
                    // If there's already a dialog with a background effect, use None for this one
                    dialog.BackgroundEffect = DialogBackgroundEffect.None;
                }
            }
            
            _dialogs.Add(dialog);
            
            // Show the dialog with a slight delay for animation
            Task.Run(async () =>
            {
                await Task.Delay(50);
                dialog.IsVisible = true;
                TriggerUIUpdate();
            });
            
            TriggerUIUpdate();
            return dialog.Id;
        }
    }

    public string ShowDialog<TComponent>() where TComponent : ComponentBase
    {
        return ShowDialog<TComponent>(new Dictionary<string, object>());
    }

    public string ShowDialog<TComponent>(Dictionary<string, object> parameters) where TComponent : ComponentBase
    {
        return ShowDialog<TComponent>(parameters, EventCallback.Empty);
    }

    public string ShowDialog<TComponent>(EventCallback onClose) where TComponent : ComponentBase
    {
        return ShowDialog<TComponent>(new Dictionary<string, object>(), onClose);
    }

    public string ShowDialog<TComponent>(Dictionary<string, object> parameters, EventCallback onClose) where TComponent : ComponentBase
    {
        var dialog = new DialogItem
        {
            ContentComponentType = typeof(TComponent),
            ContentParameters = parameters,
            OnCloseCallback = onClose
        };
        return ShowDialog(dialog);
    }

    public string ShowDialog<TComponent>(
        Dictionary<string, object> parameters,
        EventCallback onClose,
        bool closeOnClickOutside = true, 
        bool showCloseButton = true, 
        string? outlineColor = null,
        DialogSize size = DialogSize.Medium,
        string? customSize = null,
        DialogColor color = DialogColor.Default,
        DialogBackgroundEffect backgroundEffect = DialogBackgroundEffect.Dim,
        bool enableScroller = true,
        bool enableFocusTrap = true,
        bool disableBackgroundScrolling = true) where TComponent : ComponentBase
    {
        var dialog = new DialogItem
        {
            ContentComponentType = typeof(TComponent),
            ContentParameters = parameters,
            OnCloseCallback = onClose,
            CloseOnClickOutside = closeOnClickOutside,
            ShowCloseButton = showCloseButton,
            OutlineColor = outlineColor,
            Size = size,
            CustomSize = customSize,
            Color = color,
            BackgroundEffect = backgroundEffect,
            EnableScroller = enableScroller,
            EnableFocusTrap = enableFocusTrap,
            DisableBackgroundScrolling = disableBackgroundScrolling
        };
        return ShowDialog(dialog);
    }

    public void RemoveDialog(string dialogId)
    {
        lock (_lock)
        {
            var dialog = _dialogs.FirstOrDefault(d => d.Id == dialogId);
            if (dialog != null)
            {
                dialog.IsRemoving = true;
                TriggerUIUpdate();
                
                // Remove after animation completes
                Task.Run(async () =>
                {
                    await Task.Delay(300); // Match animation duration
                    lock (_lock)
                    {
                        _dialogs.Remove(dialog);
                        TriggerUIUpdate();
                    }
                });
            }
        }
    }

    public void RemoveAll()
    {
        lock (_lock)
        {
            var dialogIds = _dialogs.Select(d => d.Id).ToList();
            foreach (var id in dialogIds)
            {
                RemoveDialog(id);
            }
        }
    }

    public DialogItem? GetDialog(string dialogId)
    {
        lock (_lock)
        {
            return _dialogs.FirstOrDefault(d => d.Id == dialogId);
        }
    }

    public void SetTheme(DialogTheme theme)
    {
        Theme = theme;
        TriggerUIUpdate();
    }

    public void SetAnimation(DialogAnimation animation)
    {
        Animation = animation;
        TriggerUIUpdate();
    }

    public void ResizeDialog(string dialogId, DialogSize newSize, string? customSize = null)
    {
        lock (_lock)
        {
            var dialog = _dialogs.FirstOrDefault(d => d.Id == dialogId);
            if (dialog != null)
            {
                dialog.IsResizing = true;
                dialog.Size = newSize;
                dialog.CustomSize = customSize;
                TriggerUIUpdate();
                
                // Reset resizing state after animation completes
                Task.Run(async () =>
                {
                    await Task.Delay(300); // Match CSS transition duration
                    lock (_lock)
                    {
                        dialog.IsResizing = false;
                        TriggerUIUpdate();
                    }
                });
            }
        }
    }

    public void ResizeDialog(string dialogId, string customSize)
    {
        ResizeDialog(dialogId, DialogSize.Custom, customSize);
    }

    // Helper method for content components to resize their containing dialog
    public void ResizeCurrentDialog(DialogSize newSize, string? customSize = null)
    {
        // Find the most recently created visible dialog (assumes it's the "current" one)
        lock (_lock)
        {
            var currentDialog = _dialogs
                .Where(d => d.IsVisible && !d.IsRemoving)
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefault();
                
            if (currentDialog != null)
            {
                ResizeDialog(currentDialog.Id, newSize, customSize);
            }
        }
    }

    public void ResizeCurrentDialog(string customSize)
    {
        ResizeCurrentDialog(DialogSize.Custom, customSize);
    }

    // Color update functionality
    public void UpdateDialogColor(string dialogId, DialogColor color, string? outlineColor = null)
    {
        lock (_lock)
        {
            var dialog = _dialogs.FirstOrDefault(d => d.Id == dialogId);
            if (dialog != null)
            {
                dialog.Color = color;
                dialog.OutlineColor = outlineColor;
                TriggerUIUpdate();
            }
        }
    }

    public void UpdateCurrentDialogColor(DialogColor color, string? outlineColor = null)
    {
        // Find the most recently created visible dialog (assumes it's the "current" one)
        lock (_lock)
        {
            var currentDialog = _dialogs
                .Where(d => d.IsVisible && !d.IsRemoving)
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefault();
                
            if (currentDialog != null)
            {
                UpdateDialogColor(currentDialog.Id, color, outlineColor);
            }
        }
    }

    // Content update functionality
    public void UpdateDialogContent<TComponent>(string dialogId) where TComponent : ComponentBase
    {
        UpdateDialogContent<TComponent>(dialogId, new Dictionary<string, object>());
    }

    public void UpdateDialogContent<TComponent>(string dialogId, Dictionary<string, object> parameters) where TComponent : ComponentBase
    {
        lock (_lock)
        {
            var dialog = _dialogs.FirstOrDefault(d => d.Id == dialogId);
            if (dialog != null)
            {
                dialog.ContentComponentType = typeof(TComponent);
                dialog.ContentParameters = parameters;
                TriggerUIUpdate();
            }
        }
    }

    public void UpdateCurrentDialogContent<TComponent>() where TComponent : ComponentBase
    {
        UpdateCurrentDialogContent<TComponent>(new Dictionary<string, object>());
    }

    public void UpdateCurrentDialogContent<TComponent>(Dictionary<string, object> parameters) where TComponent : ComponentBase
    {
        // Find the most recently created visible dialog (assumes it's the "current" one)
        lock (_lock)
        {
            var currentDialog = _dialogs
                .Where(d => d.IsVisible && !d.IsRemoving)
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefault();
                
            if (currentDialog != null)
            {
                UpdateDialogContent<TComponent>(currentDialog.Id, parameters);
            }
        }
        }

    // Modular content switching using component type names
    public void UpdateCurrentDialogContent(string componentTypeName)
    {
        var componentType = ResolveComponentType(componentTypeName);
        if (componentType != null)
        {
            UpdateCurrentDialogContentByType(componentType);
        }
    }

    public void UpdateDialogContent(string dialogId, string componentTypeName)
    {
        var componentType = ResolveComponentType(componentTypeName);
        if (componentType != null)
        {
            UpdateDialogContentByType(dialogId, componentType);
        }
    }

    public void UpdateCurrentDialogContent(string componentTypeName, Dictionary<string, object> parameters)
    {
        var componentType = ResolveComponentType(componentTypeName);
        if (componentType != null)
        {
            UpdateCurrentDialogContentByType(componentType, parameters);
        }
    }

    public void UpdateDialogContent(string dialogId, string componentTypeName, Dictionary<string, object> parameters)
    {
        var componentType = ResolveComponentType(componentTypeName);
        if (componentType != null)
        {
            UpdateDialogContentByType(dialogId, componentType, parameters);
        }
    }

    // Helper method to resolve component type from string name
    private Type? ResolveComponentType(string componentTypeName)
    {
        try
        {
            // Try to find the type in all loaded assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            foreach (var assembly in assemblies)
            {
                // Try exact match first
                var type = assembly.GetType(componentTypeName);
                if (type != null && typeof(ComponentBase).IsAssignableFrom(type))
                {
                    return type;
                }
                
                // Try searching by simple name (without namespace)
                var types = assembly.GetTypes().Where(t => 
                    t.Name.Equals(componentTypeName, StringComparison.OrdinalIgnoreCase) && 
                    typeof(ComponentBase).IsAssignableFrom(t));
                
                var matchedType = types.FirstOrDefault();
                if (matchedType != null)
                {
                    return matchedType;
                }
            }
        }
        catch (Exception)
        {
            // If reflection fails, return null
        }
        
        return null;
    }

    // Helper methods for type-based updates
    private void UpdateCurrentDialogContentByType(Type componentType, Dictionary<string, object>? parameters = null)
    {
        lock (_lock)
        {
            var currentDialog = _dialogs
                .Where(d => d.IsVisible && !d.IsRemoving)
                .OrderByDescending(d => d.CreatedAt)
                .FirstOrDefault();
                
            if (currentDialog != null)
            {
                UpdateDialogContentByType(currentDialog.Id, componentType, parameters);
            }
        }
    }

    private void UpdateDialogContentByType(string dialogId, Type componentType, Dictionary<string, object>? parameters = null)
    {
        lock (_lock)
        {
            var dialog = _dialogs.FirstOrDefault(d => d.Id == dialogId);
            if (dialog != null)
            {
                dialog.ContentComponentType = componentType;
                dialog.ContentParameters = parameters ?? new Dictionary<string, object>();
                TriggerUIUpdate();
            }
        }
    }

    public string GetDialogStyle(DialogItem dialog)
    {
        var baseStyle = "border-radius: 0.5rem; box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05); border: 1px solid; overflow: hidden; position: relative; display: flex; flex-direction: column;";
        
        // Add smooth resize transitions
        baseStyle += " transition: width 0.3s ease-in-out, height 0.3s ease-in-out, min-width 0.3s ease-in-out, min-height 0.3s ease-in-out, max-width 0.3s ease-in-out, max-height 0.3s ease-in-out;";
        
        // Add size styling
        baseStyle += GetSizeStyle(dialog);
        
        // Add theme and color styling
        var themeStyle = Theme switch
        {
            DialogTheme.Light => GetLightDialogStyle(dialog),
            DialogTheme.Dark => GetDarkDialogStyle(dialog),
            _ => GetDarkDialogStyle(dialog)
        };
        
        return baseStyle + themeStyle;
    }

    private string GetSizeStyle(DialogItem dialog)
    {
        return dialog.Size switch
        {
            DialogSize.Small => " width: 350px; min-width: 300px; min-height: 200px; max-height: 60vh;",
            DialogSize.Medium => " width: 500px; min-width: 400px; min-height: 300px; max-height: 70vh;",
            DialogSize.Large => " width: 700px; min-width: 600px; min-height: 400px; max-height: 80vh;",
            DialogSize.ExtraLarge => " width: 900px; min-width: 800px; min-height: 500px; max-height: 85vh;",
            DialogSize.Custom when !string.IsNullOrEmpty(dialog.CustomSize) => $" {dialog.CustomSize};",
            DialogSize.Custom => " width: 500px; min-width: 400px; min-height: 300px; max-height: 70vh;", // fallback
            _ => " width: 500px; min-width: 400px; min-height: 300px; max-height: 70vh;"
        };
    }

    private string GetLightDialogStyle(DialogItem dialog)
    {
        var borderColor = GetBorderColor(dialog, "#e5e7eb");
        return $" background: white; border-color: {borderColor}; color: #1f2937;";
    }

    private string GetDarkDialogStyle(DialogItem dialog)
    {
        var borderColor = GetBorderColor(dialog, "#374151");
        return $" background: #1f2937; border-color: {borderColor}; color: #f9fafb;";
    }

    private string GetBorderColor(DialogItem dialog, string defaultColor)
    {
        return dialog.Color switch
        {
            DialogColor.Success => "#10b981",
            DialogColor.Error => "#ef4444",
            DialogColor.Warning => "#f59e0b",
            DialogColor.Info => "#8b5cf6",
            DialogColor.Primary => "#3b82f6",
            DialogColor.Custom when !string.IsNullOrEmpty(dialog.OutlineColor) => dialog.OutlineColor,
            _ => defaultColor
        };
    }

    public string GetCloseButtonColor()
    {
        return Theme switch
        {
            DialogTheme.Light => "#6b7280",
            DialogTheme.Dark => "#9ca3af",
            _ => "#9ca3af"
        };
    }

    private void TriggerUIUpdate()
    {
        OnDialogsChanged?.Invoke();
    }

    // Static method for JavaScript callback
    [Microsoft.JSInterop.JSInvokable]
    public static void HandleEscapeKey(string dialogId)
    {
        // This will be called from JavaScript when Escape is pressed
        Current?.RemoveDialog(dialogId);
    }
    
    // Static instance for JavaScript callbacks
    public static DialogService? Current { get; private set; }
    
    public void SetAsCurrent()
    {
        Current = this;
    }

    public void Dispose()
    {
        if (Current == this)
        {
            Current = null;
        }
        _dialogs.Clear();
    }
} 