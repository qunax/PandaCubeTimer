using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PandaCubeTimer.Converters;
using PandaCubeTimer.Data;
using PandaCubeTimer.Helpers;
using PandaCubeTimer.Models;
using PandaCubeTimer.Views;
using TNoodle.Puzzles;


namespace PandaCubeTimer.ViewModels;

public partial class TimerViewModel : BaseViewModel
{
    private const string DEFAULT_TIME_TEXT = "Tap to start";
    
    
    private SolveTimeConverter _solveTimeConverter;
    private readonly CubeTimerDb _cubeTimerDb;
    
    [ObservableProperty]
    private string _scramble = null!;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ArePenaltiesVisible))]
    [NotifyPropertyChangedFor(nameof(TimeTextToDisplay))]
    private PuzzleSolve? _currentlyMadeSolve;
    
    public string TimeTextToDisplay
    {
        get
        {
            if(CurrentlyMadeSolve is null)
                return DEFAULT_TIME_TEXT;

            return _solveTimeConverter?.DoubleToStringSeconds(CurrentlyMadeSolve?.SolveTimeSeconds) ?? "Unexpected Error Occured";
        }
    }

    public bool ArePenaltiesVisible => CurrentlyMadeSolve != null;


    
    public TimerViewModel(CubeTimerDb db)
    {
        _solveTimeConverter = new SolveTimeConverter(); 
        _cubeTimerDb = db;
        _currentlyMadeSolve = LastSolveStore.LastPuzzleSolve;
        
        ClearNavStack();
    }
    
    

    [RelayCommand]
    private async Task StartTimerAsync()
    {
         //List<PuzzleSolve> test = await _cubeTimerDb.Connection.Table<PuzzleSolve>().ToListAsync();
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            await Shell.Current.GoToAsync($"{nameof(CountingTimerView)}", false);
            //Dispose();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("IM TRYING:" + ex.Message);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to start timer: {ex.Message}", "Ok");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GenerateScramble()
    {
        await Task.Run(() =>
        {
            Puzzle puzzle = new ThreeByThreeCubePuzzle();
            Random random = new Random();

            string scramble = puzzle.GenerateWcaScramble(random);
            Scramble = scramble;
        });
    }

    [RelayCommand]
    private async Task PenalizeLastSolve(SolvePenalty penalty)
    {
        PuzzleSolve currentlyMadeSolve = await _cubeTimerDb.Connection.Table<PuzzleSolve>().OrderBy(s => s.DateTime).FirstOrDefaultAsync();
        if (CurrentlyMadeSolve == null)
            return;
        
        switch (penalty)
        {
            case SolvePenalty.NoPenalty:
                CurrentlyMadeSolve.IsPlusTwo = false;
                CurrentlyMadeSolve.IsDNF = false;
                break;
            case SolvePenalty.DNF:
                CurrentlyMadeSolve.IsPlusTwo = false;
                CurrentlyMadeSolve.IsDNF = true;
                break;
            case SolvePenalty.PlusTwo:
                CurrentlyMadeSolve.IsPlusTwo = true;
                CurrentlyMadeSolve.IsDNF = false;
                break;
            case SolvePenalty.Delete:
                await _cubeTimerDb.Connection.DeleteAsync(CurrentlyMadeSolve!);
                CurrentlyMadeSolve = null;
                return;
        }

        await _cubeTimerDb.Connection.UpdateAsync(CurrentlyMadeSolve);
    }


    /// <summary>
    /// Resets Shell completely for disabling navigation back from this page. 
    /// </summary>
    private void ClearNavStack()
    {
        var stack = Shell.Current.Navigation.NavigationStack.ToArray();
        if (stack.Length > 1)
        {
            Application.Current.MainPage = new AppShell();
        }
    }
}