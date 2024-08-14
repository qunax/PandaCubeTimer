using Microsoft.Extensions.Logging;
using PandaCubeTimer.ViewModels;
using PandaCubeTimer.Views;
using Plugin.Maui.KeyListener;

namespace PandaCubeTimer;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseKeyListener()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddTransient<TimerView>();
        builder.Services.AddTransient<CountingTimerView>();
        
        builder.Services.AddTransient<TimerViewModel>();
        builder.Services.AddTransient<CountingTimerViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}