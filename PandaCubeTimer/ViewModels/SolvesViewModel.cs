using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PandaCubeTimer.Data;
using PandaCubeTimer.Models;

namespace PandaCubeTimer.ViewModels;

public partial class SolvesViewModel : BaseViewModel
{
    private readonly CubeTimerDb _database;
    
    [ObservableProperty]
    private ObservableCollection<PuzzleSolve> _puzzleSolves = new ObservableCollection<PuzzleSolve>();
    
    [ObservableProperty] 
    private bool _isRefreshing;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOverlayVisible))]
    private PuzzleSolve? _selectedPuzzleSolve;
    
    public bool IsOverlayVisible => SelectedPuzzleSolve != null;
    
    
    
    public SolvesViewModel(CubeTimerDb database)
    {
        _database = database;
    }

    

    [RelayCommand]
    private async Task LoadSolves()
    {
        if(IsBusy)
            return;

        try
        {
            IsBusy = true;
            await LoadSolvesFromDbAsync();
        }
        catch (Exception exception)
        {
            await Shell.Current.DisplayAlert("Error!", 
                $"Unable to load solves: {exception.Message}", "Ok");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private void SelectPuzzleSolve(PuzzleSolve selectedSolve)
    {
        SelectedPuzzleSolve = selectedSolve;
    }

    [RelayCommand]
    private async Task DeleteSelectedPuzzleSolve()
    {
        if (SelectedPuzzleSolve == null)
            return;
        
        await _database.Connection.DeleteAsync(SelectedPuzzleSolve);
        PuzzleSolves.Remove(SelectedPuzzleSolve);
        SelectedPuzzleSolve = null;
    }

    [RelayCommand]
    private void ClosePuzzleSolve()
    {
        SelectedPuzzleSolve = null;
    }
    
    
    private async Task LoadSolvesFromDbAsync()
    {
        List<PuzzleSolve> solvesList = await _database.Connection.Table<PuzzleSolve>().OrderByDescending(s => s.DateTime).ToListAsync();
        PuzzleSolves = new ObservableCollection<PuzzleSolve>(solvesList);
    }
}