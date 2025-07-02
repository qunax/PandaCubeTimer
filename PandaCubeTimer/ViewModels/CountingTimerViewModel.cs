using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PandaCubeTimer.Data;
using PandaCubeTimer.Models;
using PandaCubeTimer.Views;

namespace PandaCubeTimer.ViewModels;

public partial class CountingTimerViewModel : BaseViewModel
{
    [ObservableProperty]
    //[NotifyPropertyChangedFor(nameof(CurrentCountingTime))]
    private Stopwatch _stopwatch = new();
    
    [ObservableProperty]
    private TimeSpan _elapsed;

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private string _elapsedTime;

    private readonly CubeTimerDb _cubeTimerDb;



    public CountingTimerViewModel(CubeTimerDb database)
    {
        _cubeTimerDb = database;
        Start();
    }



    [RelayCommand]
    private async Task StopTimerAsync()
    {
        Stop();
        try
        {
            PuzzleSolve currentSolve = new PuzzleSolve() { 
                Discipline = "3x3",
                SessionId = 0,
                SolveTime = _stopwatch.Elapsed.TotalSeconds,
                IsPlusTwo = false,
                IsDNF = false,
                Scramble = "test Scramble",
                DateTime = DateTime.Now,
                Comment = "test comment"
            };
            await _cubeTimerDb.Connection.InsertAsync(currentSolve);
            await Shell.Current.GoToAsync($"{nameof(TimerView)}", false, new Dictionary<string, object>
            {
                {"LastSolveTime", _elapsedTime}
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"{nameof(CountingTimerViewModel)} {nameof(StopTimerAsync)}:" + ex.Message);
            await Shell.Current.DisplayAlert("Error!",
                $"Timer has crashed while stopping: {ex.Message}", "Ok");
        }
        finally
        {
            
        }
        

    }
    
    
    
    private void Start()
    {
        _stopwatch.Restart();

        IsRunning = true;

        Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ElapsedTime = FormatElapsedTime(_stopwatch.Elapsed);
            });
            return _isRunning;
        });
    }

    
    
    private void Stop()
    {
        _stopwatch.Stop();
        IsRunning = false;
    }
    
    
    
    private string FormatElapsedTime(TimeSpan timeSpan)
    {
        return timeSpan.ToString(@"mm\:ss\.fff");
    }
}