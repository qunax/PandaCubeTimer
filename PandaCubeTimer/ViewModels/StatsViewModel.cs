using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using PandaCubeTimer.Data.Repositories;
using PandaCubeTimer.Helpers;
using PandaCubeTimer.Messages;
using PandaCubeTimer.Models;
using PandaCubeTimer.Services;
using PandaCubeTimer.Stores;      
using SkiaSharp;
using LinearGradientPaint = Microsoft.Maui.Graphics.LinearGradientPaint;

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

    // Charts
    [ObservableProperty] private Chart _timeTrendChart;
    [ObservableProperty] private Chart _timeDistributionChart;
    
    // --- LiveCharts2: Time Trend Properties ---
    [ObservableProperty] private ISeries[] _timeTrendSeries;
    [ObservableProperty] private Axis[] _timeTrendXAxes;
    [ObservableProperty] private Axis[] _timeTrendYAxes;

    // --- LiveCharts2: Time Distribution Properties ---
    [ObservableProperty] private ISeries[] _timeDistributionSeries;
    [ObservableProperty] private Axis[] _timeDistributionXAxes;
    [ObservableProperty] private Axis[] _timeDistributionYAxes;

    
    
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
        UpdateTimeDistributionChart(validSolves);
        UpdateTimeTrendChart(validSolves);
    }
    
    
    private void UpdateTimeTrendChart(List<PuzzleSolve> validSolves)
    {
        var recentSolves = validSolves.ToList();
        if (!recentSolves.Any()) return;
        recentSolves.Reverse(); // reverse so the graph is from the oldest to the newest solve times

        // 1. Theme Check
        bool isDarkMode = Application.Current?.RequestedTheme == AppTheme.Dark;
        SKColor axisTextColor = isDarkMode ? SKColor.Parse("#A0A0A0") : SKColor.Parse("#666666");
        SKColor gridLineColor = isDarkMode ? SKColor.Parse("#33FFFFFF") : SKColor.Parse("#20000000"); // Semi-transparent
        SKColor primaryColor = SKColor.Parse("#B84B9E"); // The premium purple/pink

        // 2. Setup Series (The Line)
        TimeTrendSeries = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = recentSolves.Select(s => s.SolveTimeSeconds).ToArray(),
                Fill = new LiveChartsCore.SkiaSharpView.Painting.LinearGradientPaint(
                    new[] { primaryColor.WithAlpha(90), primaryColor.WithAlpha(0) }, // Gradient fade out
                    new SKPoint(0.5f, 0), 
                    new SKPoint(0.5f, 1)
                ),
                Stroke = new SolidColorPaint(primaryColor) { StrokeThickness = 3 },
                GeometrySize = 0, // Hides the dots completely
                LineSmoothness = 0.65, // Curves the line (Spline)
                YToolTipLabelFormatter = (chartPoint) => $"{chartPoint.Coordinate.PrimaryValue:0.00} s" // Tooltip on tap
            }
        };

        // 3. Setup Y-Axis (Values on the left)
        TimeTrendYAxes = new Axis[]
        {
            new Axis
            {
                LabelsPaint = new SolidColorPaint(axisTextColor),
                TextSize = 12,
                Labeler = value => value.FormatTime(), // Formats labels like "10.4"
                SeparatorsPaint = new SolidColorPaint(gridLineColor)
                {
                    StrokeThickness = 1,
                    PathEffect = new DashEffect(new float[] { 6, 6 }) // Dashed background lines
                }
            }
        };

        // 4. Setup X-Axis (Hidden, like in your reference image)
        TimeTrendXAxes = new Axis[] { new Axis { IsVisible = false } };
    }

    private void UpdateTimeDistributionChart(List<PuzzleSolve> validSolves)
    {
        // Group by seconds
        var distribution = validSolves
            .GroupBy(s => (double)Math.Floor(s.SolveTimeSeconds))
            .OrderBy(g => g.Key)
            .ToList();

        if (!distribution.Any()) return;

        bool isDarkMode = Application.Current?.RequestedTheme == AppTheme.Dark;
        SKColor axisTextColor = isDarkMode ? SKColor.Parse("#A0A0A0") : SKColor.Parse("#666666");
        SKColor barColor = SKColor.Parse("#B84B9E");

        var values = distribution.Select(g => (double)g.Count()).ToArray();
        var labels = distribution.Select(g => g.Key.FormatTime() + "+").ToArray();

        // 1. Setup Series (The Bars)
        TimeDistributionSeries = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Values = values,
                MaxBarWidth = 10, // Very thin, elegant bars
                Fill = new SolidColorPaint(barColor),
                DataLabelsPaint = new SolidColorPaint(barColor), // Labels on top of bars
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                DataLabelsSize = 12,
                DataLabelsFormatter = (point) => point.Coordinate.PrimaryValue.ToString() // Shows count integer
            }
        };

        // 2. Setup X-Axis (Bottom labels like "9+", "10+")
        TimeDistributionXAxes = new Axis[]
        {
            new Axis
            {
                Labels = labels,
                LabelsPaint = new SolidColorPaint(axisTextColor),
                TextSize = 12,
                SeparatorsPaint = null // No grid lines behind bars
            }
        };

        // 3. Setup Y-Axis (Hidden, because values are on top of bars)
        TimeDistributionYAxes = new Axis[] { new Axis { IsVisible = false } };
    }

    

    private void ResetStats()
    {
        SolveCount = 0;
        BestSingle = SessionMean = CurrentAo5 = CurrentAo12 = CurrentAo100 = "-";
        BestAo5 = BestAo12 = BestAo100 = "-";
    }
}