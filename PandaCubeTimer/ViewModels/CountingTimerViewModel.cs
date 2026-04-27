using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PandaCubeTimer.Data;
using PandaCubeTimer.Data.Repositories;
using PandaCubeTimer.Helpers;
using PandaCubeTimer.Models;
using PandaCubeTimer.Stores;
using PandaCubeTimer.Views;

namespace PandaCubeTimer.ViewModels;

//[QueryProperty(nameof(CountingTimerViewModel.InspectionPenalty), nameof(CountingTimerViewModel.InspectionPenalty))]
//[QueryProperty(nameof(CountingTimerViewModel.CurrentSolveScramble), nameof(CountingTimerViewModel.CurrentSolveScramble))]
public partial class CountingTimerViewModel : BaseViewModel
{
    //private readonly CubeTimerDb _cubeTimerDb;
    private readonly PuzzleSolveRepository  _puzzleSolveRepository;
    private readonly ILastSolveStore _lastSolveStore;
    private readonly ActiveSessionStore  _activeSessionStore;
    private SolvePenalty? _inspectionPenalty;
    private readonly string _currentSolveScramble;
    
    [ObservableProperty]
    private Stopwatch _stopwatch = new();

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private string _elapsedTime;



    public CountingTimerViewModel(PuzzleSolveRepository puzzleSolveRepository,
                                  ILastSolveStore lastSolveStore,
                                  ActiveSessionStore activeSessionStore)
    {
        _puzzleSolveRepository = puzzleSolveRepository;
        _activeSessionStore = activeSessionStore;

        //getting passed parameters from store
        _inspectionPenalty = lastSolveStore.InspectionPenalty ?? SolvePenalty.NoPenalty;
        _currentSolveScramble = lastSolveStore.SolveScramble ?? throw new NullReferenceException("Error when passing scramble while navigating.");
        _lastSolveStore = lastSolveStore;
        
        Start();
    }



    [RelayCommand]
    private async Task StopTimerAsync()
    {
        Stop();
        try
        {
            PuzzleSolve currentSolve = new PuzzleSolve() { 
                DisciplineId = WcaDisciplines.Cube3x3,
                SessionId = _activeSessionStore.CurrentSession.Id,
                SolveTimeSeconds = Stopwatch.Elapsed.TotalSeconds,
                IsPlusTwo = _inspectionPenalty == SolvePenalty.PlusTwo,
                IsDNF = _inspectionPenalty == SolvePenalty.DNF,
                Scramble = _currentSolveScramble,
                DateTime = DateTime.Now,
                Comment = "test comment"
            };
            await _puzzleSolveRepository.CreatePuzzleSolveAsync(currentSolve);
            
            // check if solve is in database and refreshing it (why not)
            // passing it as a parameter back to TimerViewModel for display and further manipulation
            PuzzleSolve checkedFromDb = await _puzzleSolveRepository.GetPuzzleSolveAsync(currentSolve.Id);
            _lastSolveStore.LastPuzzleSolve = checkedFromDb;
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