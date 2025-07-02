using CommunityToolkit.Maui;
using MauiIcons.Material;
using Microsoft.Extensions.Logging;
using PandaCubeTimer.Data;
using PandaCubeTimer.ViewModels;
using PandaCubeTimer.Views;

namespace PandaCubeTimer;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMaterialMauiIcons()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<CubeTimerDb>();

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
        builder.Services.AddTransient<SolvesViewModel>();
        
#if DEBUG
        builder.Logging.AddDebug();
#endif
        var app = builder.Build();
        
        // Ensure database is created
        //using var scope = app.Services.CreateScope();
        //var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        //db.Database.EnsureCreated();

        return app;
    }
}