using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PandaCubeTimer.Data;
using PandaCubeTimer.Helpers;
using PandaCubeTimer.Models;
using PandaCubeTimer.Views;

namespace PandaCubeTimer.ViewModels;

public partial class CountingTimerViewModel : BaseViewModel
{
    [ObservableProperty]
    //[NotifyPropertyChangedFor(nameof(CurrentCountingTime))]
    private Stopwatch _stopwatch = new();
    
    // [ObservableProperty]
    // private TimeSpan _elapsed;

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
                SolveTimeSeconds = Stopwatch.Elapsed.TotalSeconds,
                IsPlusTwo = false,
                IsDNF = false,
                Scramble = "test Scramble",
                DateTime = DateTime.Now,
                Comment = "test comment"
            };
            await _cubeTimerDb.Connection.InsertAsync(currentSolve);
            
            // check if solve is in database and refreshing it (why not)
            // passing it as a parameter back to TimerViewModel for display and further manipulation
            //var test = await _cubeTimerDb.Connection.Table<PuzzleSolve>().ToListAsync();
            PuzzleSolve checkedFromDb = await _cubeTimerDb.Connection.Table<PuzzleSolve>().Where(x => x.Id == currentSolve.Id).FirstAsync();
            LastSolveStore.LastPuzzleSolve = checkedFromDb;
            await Shell.Current.GoToAsync($"{nameof(TimerView)}", false);
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
        Stopwatch.Restart();

        IsRunning = true;

        Device.StartTimer(TimeSpan.FromMilliseconds(10), () =>
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ElapsedTime = FormatElapsedTime(Stopwatch.Elapsed);
            });
            return IsRunning;
        });
    }

    
    
    private void Stop()
    {
        Stopwatch.Stop();
        IsRunning = false;
    }
    
    
    
    private string FormatElapsedTime(TimeSpan timeSpan)
    {
        return timeSpan.ToString(@"mm\:ss\.fff");
    }
}