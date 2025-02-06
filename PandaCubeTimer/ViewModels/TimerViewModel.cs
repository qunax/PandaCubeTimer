using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PandaCubeTimer.Views;

namespace PandaCubeTimer.ViewModels;

[QueryProperty($"{nameof(TimerViewModel.LastSolveTime)}", "LastSolveTime")]
public partial class TimerViewModel : BaseViewModel
{
    // private readonly KeyboardBehavior _keyboardBehavior;
    
    [ObservableProperty]
    private string _lastSolveTime;
    


    public TimerViewModel()
    {
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
}