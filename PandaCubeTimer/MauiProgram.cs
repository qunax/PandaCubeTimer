using CommunityToolkit.Maui;
using MauiIcons.Material;
using Microsoft.Extensions.Logging;
using PandaCubeTimer.Data;
using PandaCubeTimer.Helpers;
using PandaCubeTimer.ViewModels;
using PandaCubeTimer.Views;
using Serilog;

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
        builder.Services.AddSingleton<IAppSettingsService, AppSettingsService>();
        builder.Services.AddSingleton<ILastSolveStore, LastSolveStore>();

        builder.Services.AddTransient<TimerView>();
        builder.Services.AddTransient<InspectionView>();
        builder.Services.AddTransient<CountingTimerView>();
        builder.Services.AddTransient<SettingsView>();
        builder.Services.AddTransient<OLLTrainingsView>();
        builder.Services.AddTransient<PLLTrainingsView>();
        builder.Services.AddTransient<SolvesView>();
        builder.Services.AddTransient<StatsView>();
        
        builder.Services.AddTransient<TimerViewModel>();
        builder.Services.AddTransient<InspectionViewModel>();
        builder.Services.AddTransient<CountingTimerViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<SolvesViewModel>();
        builder.Services.AddTransient<StatsViewModel>();
        builder.Services.AddTransient<PllTrainingsViewModel>();
        builder.Services.AddTransient<OllTrainingsViewModel>();
        
        ConfigureLogging(builder);
        
        var app = builder.Build();
        
        // Ensure database is created
        //using var scope = app.Services.CreateScope();
        //var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        //db.Database.EnsureCreated();

        return app;
    }

    private static void ConfigureLogging(MauiAppBuilder builder)
    {
        string logDirectory = Path.Combine(FileSystem.AppDataDirectory, "Logs");
        string logFilePath = Path.Combine(logDirectory, "app_log_.txt");
        
        // Настраиваем базовую конфигурацию Serilog
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Debug() // Для дебага ловим всё
            .WriteTo.File(
                path: logFilePath,
                rollingInterval: RollingInterval.Day, // Новый файл каждый день
                retainedFileCountLimit: 7,            // Храним только последние 7 дней
                fileSizeLimitBytes: 10485760,         // Ограничение размера файла (10 МБ)
                rollOnFileSizeLimit: true             // Если превысит 10мб, создаст новый
            );

        // Добавляем вывод в консоль Rider ТОЛЬКО если мы в режиме отладки
#if DEBUG
        loggerConfig.WriteTo.Debug(); 
#endif

        Log.Logger = loggerConfig.CreateLogger();
        
        // 3. Подключаем логгер к MAUI
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(dispose: true);
    }
}