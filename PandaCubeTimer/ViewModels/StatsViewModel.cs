using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microcharts;
using PandaCubeTimer.Data.Repositories;
using PandaCubeTimer.Helpers;
using PandaCubeTimer.Messages;
using PandaCubeTimer.Models;
using PandaCubeTimer.Services;
using PandaCubeTimer.Stores;      
using SkiaSharp;

namespace PandaCubeTimer.ViewModels;

public partial class StatsViewModel : BaseViewModel
{
    private readonly PuzzleSolveRepository _solveRepository;
    private readonly ActiveSessionStore _activeSessionStore;
    private readonly ISolveStatsService _solveStatsService;
    
    

    [ObservableProperty] private bool _isRefreshing;
    
    [ObservableProperty] private int _solveCount;
    [ObservableProperty] private string _bestSingle = "-";
    [ObservableProperty] private string _sessionMean = "-";

    // Current Averages
    [ObservableProperty] private string _currentAo5 = "-";
    [ObservableProperty] private string _currentAo12 = "-";
    [ObservableProperty] private string _currentAo100 = "-";

    // Best Averages
    [ObservableProperty] private string _bestAo5 = "-";
    [ObservableProperty] private string _bestAo12 = "-";
    [ObservableProperty] private string _bestAo100 = "-";

    // Chart
    [ObservableProperty] private Chart _timeTrendChart;

    
    
    public StatsViewModel(PuzzleSolveRepository solveRepository,
        ActiveSessionStore activeSessionStore,
        ISolveStatsService solveStatsService)
    {
        _solveRepository = solveRepository;
        _activeSessionStore = activeSessionStore;
        _solveStatsService = solveStatsService;
        
        // 3. Listen to the Store for changes (adjust the event name if yours is slightly different)
        //_activeSessionStore.ActiveSessionChanged += async (s, e) => await CalculateStatsAsync();
        
        ConfigureMessageReceiving();
    }
    
    private void ConfigureMessageReceiving()
    {
        WeakReferenceMessenger.Default.Register<ActiveSessionChangedMessage>(this, (r, m) =>
        {
            //OnActiveSessionChangedReceived(m.Value);
        });
    }

    
    
    [RelayCommand]
    public async Task RefreshAsync()
    {
        if (IsBusy)
            return;
        
        try
        {
            IsRefreshing = true;
            IsBusy = true;
            await CalculateStatsAsync();
        }
        finally
        {
            IsRefreshing = false;
            IsBusy = false;
        }
    }

    private async Task CalculateStatsAsync()
    {
        // 4. Get the active session from the Store
        var currentSession = _activeSessionStore.CurrentSession; 
        
        if (currentSession == null)
        {
            ResetStats();
            TimeTrendChart = null;
            return;
        }

        // 5. Fetch the solves from the Repository using the active session's ID
        // (Adjust the method name if your repository uses something like GetAllBySessionIdAsync)
        var solves = await _solveRepository.GetSessionPuzzleSolvesAsync(currentSession.Id);
        
        if (solves == null || solves.Count == 0)
        {
            ResetStats();
            TimeTrendChart = null;
            return;
        }

        SolveCount = solves.Count;
        
        var validSolves = solves.Where(s => !s.IsDNF).ToList();
        if (validSolves.Any())
        {
            BestSingle = validSolves.Min(s => s.SolveTimeSeconds).FormatTime();
            SessionMean = validSolves.Average(s => s.SolveTimeSeconds).FormatTime();
        }

        // Calculate Current Averages
        CurrentAo5 = _solveStatsService.CalculateCurrentAverageOf(solves, 5);
        CurrentAo12 = _solveStatsService.CalculateCurrentAverageOf(solves, 12);
        CurrentAo100 = _solveStatsService.CalculateCurrentAverageOf(solves, 100);

        // Calculate Best Averages
        BestAo5 = _solveStatsService.CalculateBestAverageOf(solves, 5);
        BestAo12 = _solveStatsService.CalculateBestAverageOf(solves, 12);
        BestAo100 = _solveStatsService.CalculateBestAverageOf(solves, 100);

        // Update the Graph
        UpdateTimeTrendChart(validSolves);
    }

    

    

    

    private void UpdateTimeTrendChart(List<PuzzleSolve> validSolves)
    {
        var recentSolves = validSolves.TakeLast(50).ToList();
        var entries = new List<ChartEntry>();

        for (int i = 0; i < recentSolves.Count; i++)
        {
            var solve = recentSolves[i];
            entries.Add(new ChartEntry((float)solve.SolveTimeSeconds)
            {
                Color = SKColor.Parse("#3498db"),
                Label = i % 5 == 0 ? (i + 1).ToString() : "", 
                ValueLabel = solve.SolveTimeSeconds.FormatTime()
            });
        }

        TimeTrendChart = new LineChart
        {
            Entries = entries,
            LineMode = LineMode.Spline,
            LineSize = 6,
            PointMode = PointMode.Circle,
            PointSize = 12,
            BackgroundColor = SKColors.Transparent,
            LabelTextSize = 24,
            ValueLabelOrientation = Orientation.Horizontal
        };
    }

    private void ResetStats()
    {
        SolveCount = 0;
        BestSingle = SessionMean = CurrentAo5 = CurrentAo12 = CurrentAo100 = "-";
        BestAo5 = BestAo12 = BestAo100 = "-";
    }
}