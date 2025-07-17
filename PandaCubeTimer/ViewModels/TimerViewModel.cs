using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PandaCubeTimer.Data;
using PandaCubeTimer.Models;
using PandaCubeTimer.Views;
using TNoodle.Puzzles;


namespace PandaCubeTimer.ViewModels;

[QueryProperty($"{nameof(TimerViewModel.LastSolveTime)}", "LastSolveTime")]
public partial class TimerViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _scramble = null!;
    
    [ObservableProperty]
    private string _lastSolveTime;
    
    private readonly CubeTimerDb _cubeTimerDb;


    public TimerViewModel(CubeTimerDb db)
    {
        _cubeTimerDb = db;
        _lastSolveTime = "Tap to start";
    }
    
    

    [RelayCommand]
    private async Task StartTimerAsync()
    {
         List<PuzzleSolve> test = await _cubeTimerDb.Connection.Table<PuzzleSolve>().ToListAsync();
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
}