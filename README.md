# Simple Blazor Dialogs

A comprehensive modal dialog system for Blazor applications featuring:

- 🎨 **7 Dialog Colors**: Default, Success, Error, Warning, Info, Primary, & Custom
- 📏 **5 Sizes**: Small, Medium, Large, ExtraLarge, Custom  
- 🎭 **4 Animation Styles**: None, Fade, Scale, FadeAndScale
- 🌈 **2 Themes**: Light, Dark
- 🖼️ **3 Background Effects**: Dim, Blur, None
- 🔄 **Component Content**: Any Blazor component as dialog content
- ⚙️ **Smart Configuration**: Customizable behavior and appearance
- 🎯 **Interactive Features**: Focus traps, click-outside handling
- 📊 **Dynamic Management**: Resize *with animation*, update content, and state control

## Preview Gallery

<details>
<summary><strong>🎨 Dialog Types & Themes</strong></summary>

![demo](https://github.com/user-attachments/assets/612cff95-4702-4b03-81d2-fad659f594ff)

![blur](https://github.com/user-attachments/assets/6475e586-760d-418b-9c91-4ed25bf06a97)

![error](https://github.com/user-attachments/assets/793d2390-d13e-45f1-af1d-1007859905a5)

![large](https://github.com/user-attachments/assets/585161fb-b97e-4e0a-91e7-b5f45806979f)


</details>

## Installation

```bash
dotnet add package Simple.Blazor.Dialogs
```

## Quick Start

### 1. Register Services

```csharp
// Program.cs
using Simple.Blazor.Dialogs.Extensions;

builder.Services.AddSimpleBlazorDialogs();
```

### 2. Add CSS Reference

```html
<!-- index.html or _Host.cshtml -->
<link href="_content/Simple.Blazor.Dialogs/css/dialog.css" rel="stylesheet" />
```

### 3. Add Dialog Container

```razor
<!-- MainLayout.razor -->
@using Simple.Blazor.Dialogs.Components
<DialogContainer />
```

### 4. Use in Components

```csharp
@using Simple.Blazor.Dialogs.Services
@inject DialogService DialogService

<button @onclick="ShowDialog">Show Dialog</button>

@code {
    void ShowDialog()
    {
        DialogService.ShowDialog<MyComponent>();
    }
}
```

## Programmatic API

<details>
<summary><strong>📝 Basic Dialog Methods</strong></summary>

```csharp
// Simple component dialog
string dialogId = DialogService.ShowDialog<MyComponent>();

// Dialog with parameters
var parameters = new Dictionary<string, object> { ["Title"] = "Hello" };
DialogService.ShowDialog<MyComponent>(parameters);

// Dialog with close callback
DialogService.ShowDialog<MyComponent>(EventCallback.Factory.Create(this, OnDialogClosed));

// Dialog with custom size
DialogService.ShowDialog<MyComponent>(
    new Dictionary<string, object>(), 
    EventCallback.Empty, 
    size: DialogSize.Large
);

// Empty dialog for custom content
DialogService.ShowDialog(size: DialogSize.Medium, color: DialogColor.Success);
```

</details>

<details>
<summary><strong>🎛️ Advanced Dialog Creation</strong></summary>

```csharp
// Custom dialog with all parameters
var dialogId = DialogService.ShowDialog<MyComponent>(
    parameters: new Dictionary<string, object> { ["Data"] = myData },
    onClose: EventCallback.Factory.Create(this, HandleClose),
    closeOnClickOutside: false,
    showCloseButton: true,
    outlineColor: "#ff0000",
    size: DialogSize.Large,
    customSize: "800px",
    color: DialogColor.Primary,
    backgroundEffect: DialogBackgroundEffect.Blur,
    enableScroller: true,
    enableFocusTrap: true,
    disableBackgroundScrolling: true
);

// Dialog object with full control
var dialog = new DialogItem
{
    ContentComponentType = typeof(MyComponent),
    ContentParameters = new Dictionary<string, object> { ["UserId"] = 123 },
    Size = DialogSize.ExtraLarge,
    Color = DialogColor.Info,
    BackgroundEffect = DialogBackgroundEffect.Dim,
    CloseOnClickOutside = false,
    Data = new Dictionary<string, object> { ["sessionId"] = "abc123" }
};
var id = DialogService.ShowDialog(dialog);
```

</details>

<details>
<summary><strong>⚙️ Dialog Configuration</strong></summary>

```csharp
// Global settings
DialogService.SetTheme(DialogTheme.Light);
DialogService.SetAnimation(DialogAnimation.FadeAndScale);

// Size options: Small, Medium, Large, ExtraLarge, Custom
// Animation options: None, Fade, Scale, FadeAndScale
// Theme options: Light, Dark
// Color options: Default, Success, Error, Warning, Info, Primary, Custom
// Background Effect options: Dim, Blur, None
```

</details>

<details>
<summary><strong>🎯 Interactive Dialogs with Content</strong></summary>

```csharp
// Confirmation dialog component
DialogService.ShowDialog<ConfirmationDialog>(
    parameters: new Dictionary<string, object>
    {
        ["Message"] = "Are you sure you want to delete this item?",
        ["Title"] = "Confirm Delete",
        ["OnConfirm"] = EventCallback.Factory.Create(this, HandleConfirm),
        ["OnCancel"] = EventCallback.Factory.Create(this, HandleCancel)
    },
    size: DialogSize.Medium,
    color: DialogColor.Warning
);

// Form dialog with validation
DialogService.ShowDialog<UserFormDialog>(
    parameters: new Dictionary<string, object>
    {
        ["User"] = currentUser,
        ["OnSave"] = EventCallback.Factory.Create<User>(this, SaveUser),
        ["OnCancel"] = EventCallback.Factory.Create(this, CancelEdit)
    },
    closeOnClickOutside: false,
    size: DialogSize.Large,
    enableScroller: true
);

// Image gallery dialog
DialogService.ShowDialog<ImageGalleryDialog>(
    parameters: new Dictionary<string, object>
    {
        ["Images"] = imageList,
        ["StartIndex"] = selectedIndex
    },
    size: DialogSize.ExtraLarge,
    backgroundEffect: DialogBackgroundEffect.Blur,
    showCloseButton: true
);
```

</details>

<details>
<summary><strong>🔧 Dialog Management</strong></summary>

```csharp
// Remove specific dialog
DialogService.RemoveDialog(dialogId);

// Remove all dialogs
DialogService.RemoveAll();

// Update dialog content
DialogService.UpdateDialogContent<NewComponent>(dialogId);
DialogService.UpdateDialogContent<NewComponent>(dialogId, newParameters);

// Update current dialog content
DialogService.UpdateCurrentDialogContent<NewComponent>();
DialogService.UpdateCurrentDialogContent<NewComponent>(newParameters);

// Resize dialog
DialogService.ResizeDialog(dialogId, DialogSize.Large);
DialogService.ResizeDialog(dialogId, "600px");
DialogService.ResizeCurrentDialog(DialogSize.Medium);

// Update dialog color
DialogService.UpdateDialogColor(dialogId, DialogColor.Success);
DialogService.UpdateDialogColor(dialogId, DialogColor.Custom, "#ff6b6b");
DialogService.UpdateCurrentDialogColor(DialogColor.Warning);
DialogService.UpdateCurrentDialogColor(DialogColor.Custom, "#00ff00");

// Check if dialog exists
DialogItem? dialog = DialogService.GetDialog(dialogId);
bool isActive = dialog != null && dialog.IsVisible && !dialog.IsRemoving;
```

</details>

<details>
<summary><strong>📋 Dialog State Management</strong></summary>

```csharp
// Get all active dialogs
IReadOnlyCollection<DialogItem> activeDialogs = DialogService.Dialogs;

// Find specific dialogs
var successDialogs = DialogService.Dialogs.Where(d => d.Color == DialogColor.Success);
var largeDialogs = DialogService.Dialogs.Where(d => d.Size == DialogSize.Large);

// Dialog properties
var dialog = DialogService.GetDialog(dialogId);
if (dialog != null)
{
    bool isVisible = dialog.IsVisible;
    bool isRemoving = dialog.IsRemoving;
    bool isResizing = dialog.IsResizing;
    DateTime createdAt = dialog.CreatedAt;
    var customData = dialog.Data;
}
```

</details>

<details>
<summary><strong>🔄 Dynamic Content Updates</strong></summary>

```csharp
// Switch dialog content to different component
DialogService.UpdateCurrentDialogContent<LoginForm>();

// Update with new parameters
var loginParams = new Dictionary<string, object>
{
    ["Title"] = "Please Login",
    ["OnSuccess"] = EventCallback.Factory.Create(this, OnLoginSuccess),
    ["OnCancel"] = EventCallback.Factory.Create(this, OnLoginCancel)
};
DialogService.UpdateCurrentDialogContent<LoginForm>(loginParams);

// Update by component type name (useful for dynamic scenarios)
DialogService.UpdateCurrentDialogContent("MyApp.Components.DynamicForm", formParams);

// Resize after content update
DialogService.ResizeCurrentDialog(DialogSize.Large);
```

</details>

<details>
<summary><strong>🎨 Custom Styling</strong></summary>

```csharp
// Custom outline color
DialogService.ShowDialog<MyComponent>(
    outlineColor: "#ff6b6b",
    color: DialogColor.Custom
);

// Custom size
DialogService.ShowDialog<MyComponent>(
    size: DialogSize.Custom,
    customSize: "90vw"
);

// Get dialog CSS style (for advanced scenarios)
var dialog = DialogService.GetDialog(dialogId);
string cssStyle = DialogService.GetDialogStyle(dialog);
string closeButtonColor = DialogService.GetCloseButtonColor();
```

</details>

<details>
<summary><strong>🚀 Service Configuration (Program.cs)</strong></summary>

```csharp
// Basic registration
builder.Services.AddSimpleBlazorDialogs();

// With configuration
builder.Services.AddSimpleBlazorDialogs(config =>
{
    config.DefaultTheme = DialogTheme.Light;
    config.DefaultAnimation = DialogAnimation.Scale;
});
```

</details>

<details>
<summary><strong>📊 Events and Monitoring</strong></summary>

```csharp
// Subscribe to dialog changes
DialogService.OnDialogsChanged += () =>
{
    Console.WriteLine($"Dialog count: {DialogService.Dialogs.Count}");
    var activeDialogs = DialogService.Dialogs.Where(d => d.IsVisible && !d.IsRemoving);
    Console.WriteLine($"Active dialogs: {activeDialogs.Count()}");
};

// Access dialog properties
foreach (var dialog in DialogService.Dialogs)
{
    Console.WriteLine($"Dialog {dialog.Id}: {dialog.Size}, {dialog.Color}, Created: {dialog.CreatedAt}");
}
```

</details>

Take a look at the ***[sample project](https://github.com/umbraprior/Simple-Blazor-Dialogs/blob/main/src/Samples/DIALOGSAMPLEPROJ)***
