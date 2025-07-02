using System.ComponentModel;
using Microsoft.AspNetCore.Components;

namespace Simple.Blazor.Dialogs.Models;

public class DialogItem : INotifyPropertyChanged
{
    private bool _isVisible = false;
    private bool _isRemoving = false;
    private bool _isResizing = false;

    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public bool IsVisible 
    { 
        get => _isVisible;
        set
        {
            if (_isVisible != value)
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }
    }
    
    public bool IsRemoving 
    { 
        get => _isRemoving;
        set
        {
            if (_isRemoving != value)
            {
                _isRemoving = value;
                OnPropertyChanged();
            }
        }
    }
    
    public bool IsResizing 
    { 
        get => _isResizing;
        set
        {
            if (_isResizing != value)
            {
                _isResizing = value;
                OnPropertyChanged();
            }
        }
    }
    
    public bool CloseOnClickOutside { get; set; } = true;
    public bool ShowCloseButton { get; set; } = true;
    public string? OutlineColor { get; set; } = null;
    public DialogSize Size { get; set; } = DialogSize.Medium;
    public string? CustomSize { get; set; } = null;
    public DialogColor Color { get; set; } = DialogColor.Default;
    public DialogBackgroundEffect BackgroundEffect { get; set; } = DialogBackgroundEffect.Dim;
    public bool EnableScroller { get; set; } = true;
    public bool EnableFocusTrap { get; set; } = true;
    public bool DisableBackgroundScrolling { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public Dictionary<string, object> Data { get; set; } = new();
    
    public Type? ContentComponentType { get; set; } = null;
    public Dictionary<string, object> ContentParameters { get; set; } = new();
    public EventCallback OnCloseCallback { get; set; } = default;



    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
} 