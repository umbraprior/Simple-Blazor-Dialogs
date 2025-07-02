# Simple Blazor Dialogs - Sample Project

This is a simple Blazor Server application that demonstrates all the features of the Simple Blazor Dialogs library.

## Features Demonstrated

- ✅ Basic dialog creation
- ✅ Different dialog sizes (Small, Medium, Large, XL, Custom)
- ✅ Dialog colors (Success, Error, Warning, Info, Primary, Custom)
- ✅ Background effects (Dim, Blur, None)
- ✅ Dynamic updates (resize, color changes, content switching)
- ✅ Theme switching (Light/Dark)
- ✅ Animation options
- ✅ Configuration options (close button, click outside)

## How to Run

1. Make sure you have .NET 8.0 SDK installed
2. Navigate to this directory in your terminal `Simple-Blazor-Dialogs/src/Samples/DIALOGSAMPLEPROJ`
3. Run the application:

```bash
dotnet run
```

4. Open your browser to `https://localhost:7001` or `http://localhost:5001`

## Testing the Features

The home page contains organized sections with buttons to test different aspects of the dialog system:

- **Basic Dialogs**: Test simple dialog creation and basic options
- **Dialog Sizes**: Test all available size options
- **Dialog Colors**: Test different color themes and custom colors
- **Background Effects**: Test dim, blur, and no background effects
- **Dynamic Updates**: Test real-time updates to existing dialogs
- **Settings**: Change global theme and animation settings

## Components Used

- `SimpleContentComponent`: Basic content for testing
- `SampleContentComponent`: Parameterized content with title and message
- `UpdatableContentComponent`: Interactive component for testing dynamic updates

## Notes

This is a minimal sample app focused on testing dialog functionality. It uses Bootstrap for basic styling and demonstrates practical usage patterns for the Simple Blazor Dialogs library. 