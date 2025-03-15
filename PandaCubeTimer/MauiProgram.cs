using Microsoft.Extensions.Logging;
using PandaCubeTimer.Services;
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
        builder.Services.AddTransient<SettingsView>();
        builder.Services.AddTransient<OLLTrainingsView>();
        builder.Services.AddTransient<PLLTrainingsView>();
        builder.Services.AddTransient<SolvesView>();
        builder.Services.AddTransient<StatsView>();
        
        builder.Services.AddTransient<TimerViewModel>();
        builder.Services.AddTransient<CountingTimerViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        builder.Services.AddSingleton<IKeyboardService, KeyboardService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}