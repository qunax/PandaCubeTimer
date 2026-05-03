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
using Refit;
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

        builder.Services.AddRefitClient<IPandaCubeTimer_API>()
            .ConfigureHttpClient(c => 
            {
                // Define the base URL of your API explicitly here.
                // For Android Emulator, use 10.0.2.2. For iOS Simulator, use localhost.
                c.BaseAddress = new Uri("http://localhost:5030"); 
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();;
        
        builder.Services.AddSingleton<CubeTimerDb>();
        
        builder.Services.AddTransient<AuthHeaderHandler>();
        
        builder.Services.AddSingleton<IAppSettingsService, AppSettingsService>();
        builder.Services.AddTransient<ISolveStatsService, SolveStatsService>();
        builder.Services.AddTransient<AuthStorageService>();
        
        builder.Services.AddSingleton<ILastSolveStore, LastSolveStore>();
        builder.Services.AddSingleton<ActiveSessionStore>();
        builder.Services.AddSingleton<UserInfoStore>();

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
        builder.Services.AddTransient<LoginPageView>();

        builder.Services.AddSingleton<AppShellViewModel>();
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
        builder.Services.AddTransient<LoginPageViewModel>();
        
        builder.Services.AddSingleton<ActiveSessionBarViewModel>();
        builder.Services.AddSingleton<SessionsViewModel>();
        
        ConfigureLogging(builder);
        
        var app = builder.Build();
        
        

        

        Task.Run(async () => await IntializeDbAsync(app.Services)).Wait();
        Task.Run(async () => await InitializeStartupSessionAsync(app.Services)).Wait();
        Task.Run(async () => await LoadUserInfoAsync(app.Services)).Wait();

        return app;
    }

    private static void ConfigureLogging(MauiAppBuilder builder)
    {
        string logDirectory = Path.Combine(FileSystem.AppDataDirectory, "Logs");
        string logFilePath = Path.Combine(logDirectory, "app_log_.txt");
        
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                path: logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,         
                fileSizeLimitBytes: 10485760,     
                rollOnFileSizeLimit: true    
            );

#if DEBUG
        loggerConfig.WriteTo.Debug(); 
#endif

        Log.Logger = loggerConfig.CreateLogger();
        
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

    private static async Task LoadUserInfoAsync(IServiceProvider services)
    {
        var authService = services.GetRequiredService<AuthStorageService>();
        var userInfoStore = services.GetRequiredService<UserInfoStore>();

        var token = await authService.GetTokenAsync();
        var refreshToken = await authService.GetRefreshTokenAsync();
        var username = await authService.GetUsernameAsync();
        if (string.IsNullOrEmpty(token)
            || string.IsNullOrEmpty(refreshToken)
            || string.IsNullOrEmpty(username))
        {
            await authService.ClearAuthDataAsync();
            return;
        }
        
        userInfoStore.Username = username;
    }
}