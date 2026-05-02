using CommunityToolkit.Maui;
using MauiIcons.Material;
using Microsoft.Extensions.Logging;
using PandaCubeTimer.Data;
using PandaCubeTimer.Data.Repositories;
using PandaCubeTimer.Helpers;
using PandaCubeTimer.Messages;
using PandaCubeTimer.Models;
using PandaCubeTimer.Services;
using PandaCubeTimer.Stores;
using PandaCubeTimer.ViewModels;
using PandaCubeTimer.ViewModels.ControlsVMs;
using PandaCubeTimer.Views;
using PandaCubeTimer.Views.Controls;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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
        builder.Services.AddTransient<ISolveStatsService, SolveStatsService>();
        builder.Services.AddTransient<IPandaCubeTimer_API>();
        builder.Services.AddSingleton<ILastSolveStore, LastSolveStore>();
        builder.Services.AddSingleton<ActiveSessionStore>();

        builder.Services.AddTransient<DisciplineRepository>();
        builder.Services.AddTransient<SessionRepository>();
        builder.Services.AddTransient<PuzzleSolveRepository>();

        builder.Services.AddTransient<TimerView>();
        builder.Services.AddTransient<InspectionView>();
        builder.Services.AddTransient<CountingTimerView>();
        builder.Services.AddTransient<SettingsView>();
        builder.Services.AddTransient<OLLTrainingsView>();
        builder.Services.AddTransient<PLLTrainingsView>();
        builder.Services.AddTransient<SolvesView>();
        builder.Services.AddTransient<StatsView>();
        
        builder.Services.AddSingleton<ActiveSessionBar>();
        builder.Services.AddSingleton<SessionsView>();
        
        builder.Services.AddTransient<TimerViewModel>();
        builder.Services.AddTransient<InspectionViewModel>();
        builder.Services.AddTransient<CountingTimerViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<SolvesViewModel>();
        builder.Services.AddTransient<StatsViewModel>();
        builder.Services.AddTransient<PllTrainingsViewModel>();
        builder.Services.AddTransient<OllTrainingsViewModel>();
        
        builder.Services.AddSingleton<ActiveSessionBarViewModel>();
        builder.Services.AddSingleton<SessionsViewModel>();
        
        ConfigureLogging(builder);
        
        var app = builder.Build();
        
        

        

        Task.Run(async () => await IntializeDbAsync(app.Services)).Wait();
        Task.Run(async () => await InitializeStartupSessionAsync(app.Services)).Wait();

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

    private static async Task IntializeDbAsync(IServiceProvider services)
    {
        CubeTimerDb db = services.GetRequiredService<CubeTimerDb>();
        await db.InitializeAsync();
        
        DisciplineRepository disciplineRepository = services.GetRequiredService<DisciplineRepository>();
        await disciplineRepository.SeedDisciplinesAsync();
        
        SessionRepository sessionRepository = services.GetRequiredService<SessionRepository>();
        await sessionRepository.SeedDefaultSessionAsync();
    }

    private static async Task InitializeStartupSessionAsync(IServiceProvider services)
    {
        var appSettingsService = services.GetRequiredService<IAppSettingsService>();
        var sessionRepository = services.GetRequiredService<SessionRepository>();
        var activeSessionStore = services.GetRequiredService<ActiveSessionStore>();
        
        Guid sessionId = Guid.Parse(appSettingsService.StartupSessionId);
        
        Session sessionById;
        try
        {
            sessionById = await sessionRepository.GetSessionByIdAsync(sessionId);
        }
        catch (Exception ex)
        {
            Log.Logger.Error("Failed to get session by id: " + ex.Message);
            sessionById = await sessionRepository.GetSessionByIdAsync(Session.DefaultSessionId);
        }
        
        await activeSessionStore.SetSessionAsync(sessionById);
    }
}