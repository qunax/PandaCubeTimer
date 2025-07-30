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
    private const string DefaultTimeText = "Tap to start";
    
    
    private readonly SolveToTimeConverter _solveToTimeConverter;
    private readonly CubeTimerDb _cubeTimerDb;
    private readonly IAppSettingsService _appSettingsService;
    private readonly ILastSolveStore _lastSolveStore;
    private readonly SolvePenalty? _lastSolveInspectionPenalty;
    
    
    
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
                return DefaultTimeText;

            return _solveToTimeConverter.PuzzleSolveToTimeToDisplay(CurrentlyMadeSolve);
        }
    }

    public bool ArePenaltiesVisible => CurrentlyMadeSolve != null;
    
    public SolvePenalty LastSolveInspectionPenalty => _lastSolveInspectionPenalty ?? SolvePenalty.NoPenalty;


    
    public TimerViewModel(CubeTimerDb db, IAppSettingsService appSettingsService, ILastSolveStore lastSolveStore)
    {
        _solveToTimeConverter = new SolveToTimeConverter(); 
        _cubeTimerDb = db;
        _appSettingsService = appSettingsService;
        
        // copying some important info about solve and its inspection from previous pages
        // done this way because of nevigation limitations (i have to clean stack navigating here
        _lastSolveStore = lastSolveStore;
        _currentlyMadeSolve = lastSolveStore.LastPuzzleSolve;
        _lastSolveInspectionPenalty = lastSolveStore.InspectionPenalty;
        
        
        ClearNavStack();
    }
    
    

    [RelayCommand]
    private async Task StartTimerAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _lastSolveStore.ClearData();
            _lastSolveStore.SolveScramble = Scramble;
            if (_appSettingsService.IsInspectionTurnedOn)
            {
                await Shell.Current.GoToAsync($"{nameof(InspectionView)}", false);
            }
            else
            {
                await Shell.Current.GoToAsync($"{nameof(CountingTimerView)}", false);

            }
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
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            if (CurrentlyMadeSolve == null)
                return;

            OnPropertyChanging(nameof(TimeTextToDisplay));
            switch (penalty)
            {
                case SolvePenalty.NoPenalty:
                    //remain +2 if it was on inspection
                    if (LastSolveInspectionPenalty == SolvePenalty.PlusTwo)
                    {
                        CurrentlyMadeSolve.IsPlusTwo = true;
                        CurrentlyMadeSolve.IsDNF = false;
                    }
                    else
                    {
                        CurrentlyMadeSolve.IsPlusTwo = false;
                        CurrentlyMadeSolve.IsDNF = false;   
                    }
                    break;
                case SolvePenalty.DNF:
                    CurrentlyMadeSolve.IsPlusTwo = false;
                    CurrentlyMadeSolve.IsDNF = true;
                    break;
                case SolvePenalty.PlusTwo:
                    // +2 plus +2 gives DNF penalty
                    if (LastSolveInspectionPenalty == SolvePenalty.PlusTwo)
                    {
                        CurrentlyMadeSolve.IsPlusTwo = false;
                        CurrentlyMadeSolve.IsDNF = true;
                    }
                    else
                    {
                        CurrentlyMadeSolve.IsPlusTwo = true;
                        CurrentlyMadeSolve.IsDNF = false;
                    }
                    break;
                case SolvePenalty.Delete:
                    await _cubeTimerDb.Connection.DeleteAsync(CurrentlyMadeSolve!);
                    CurrentlyMadeSolve = null;
                    return;
            }

            // idk automatic update of this property doesnt work so had to 
            // add manual raise of PropertyChanged (as well as PropertyChanging)
            OnPropertyChanged(nameof(TimeTextToDisplay));
            await _cubeTimerDb.Connection.UpdateAsync(CurrentlyMadeSolve);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("IM TRYING:" + ex.Message);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to penalize solve: {ex.Message}", "Ok");
        }
        finally
        {
            IsBusy = false;
        }
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