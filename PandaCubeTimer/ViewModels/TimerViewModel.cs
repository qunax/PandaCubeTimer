using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PandaCubeTimer.Services;
using PandaCubeTimer.Views;
using SharpHook;
using SharpHook.Native;
using SharpHook.Reactive;
using TNoodle.Puzzles;

namespace PandaCubeTimer.ViewModels;

[QueryProperty($"{nameof(TimerViewModel.LastSolveTime)}", "LastSolveTime")]
public partial class TimerViewModel : BaseViewModel
{
    // private readonly KeyboardBehavior _keyboardBehavior;
    //private readonly IKeyboardService _keyboardService;
    //private readonly TaskPoolGlobalHook _hook;

    [ObservableProperty]
    private string _scramble = null!;
    
    [ObservableProperty]
    private string _lastSolveTime;
    


    public TimerViewModel(IKeyboardService keyboardService)
    {
        //_keyboardService = keyboardService;

        _lastSolveTime = "Tap to start";

        // _keyboardBehavior = new KeyboardBehavior();
        // _keyboardBehavior.KeyDown += (sender, args) =>
        // {
        //     Debug.WriteLine($"KeyDown: {args.Keys.ToString()}");
        //     Debug.WriteLine($"KeyDown Char: {args.KeyChar}");
        // };
        // _keyboardBehavior.KeyUp += (sender, args) =>
        // {
        //     Debug.WriteLine($"KeyUp: {args.Keys.ToString()}");
        //     Debug.WriteLine($"KeyUp Char: {args.KeyChar}");
        // };
        GenerateScrambleCommand.Execute(null);

        //_keyboardService.Hook.KeyPressed += Hook_KeyPressed;
        //_hook = new TaskPoolGlobalHook();
        //_hook.KeyPressed += Hook_KeyPressed;
        //_hook.RunAsync();
    }

    private void  Hook_KeyPressed(object? sender, SharpHook.KeyboardHookEventArgs e)
    {
        switch (e.Data.KeyCode)
        {
            case KeyCode.VcSpace:
                StartTimerCommand.Execute(null);
                break;
            default:
                break;
        }
    }

    [RelayCommand]
    private async Task StartTimerAsync()
    {
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
    private void GenerateScramble()
    {
        Puzzle puzzle = new ThreeByThreeCubePuzzle();
        Random random = new Random(2017);

        string scramble = puzzle.GenerateWcaScramble(random);
        Scramble = scramble;
    }



    //public void Dispose()
    //{
    //    _keyboardService.Hook.KeyPressed -= Hook_KeyPressed;
    //}
}