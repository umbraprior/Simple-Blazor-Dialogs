using Microsoft.Extensions.DependencyInjection;
using Simple.Blazor.Dialogs.Services;
using Simple.Blazor.Dialogs.Models;

namespace Simple.Blazor.Dialogs.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Simple Blazor Dialogs services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddSimpleBlazorDialogs(this IServiceCollection services)
    {
        services.AddSingleton<DialogService>();
        return services;
    }

    /// <summary>
    /// Adds Simple Blazor Dialogs services with configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Configuration action</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddSimpleBlazorDialogs(
        this IServiceCollection services, 
        Action<DialogConfiguration> configure)
    {
        var config = new DialogConfiguration();
        configure(config);
        
        services.AddSingleton(config);
        services.AddSingleton<DialogService>();
        return services;
    }
}

/// <summary>
/// Configuration options for Simple Blazor Dialogs
/// </summary>
public class DialogConfiguration
{
    public DialogTheme DefaultTheme { get; set; } = DialogTheme.Dark;
    public DialogAnimation DefaultAnimation { get; set; } = DialogAnimation.FadeAndScale;
} 