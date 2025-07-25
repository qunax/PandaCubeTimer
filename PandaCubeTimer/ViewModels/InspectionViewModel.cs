using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PandaCubeTimer.Views;

namespace PandaCubeTimer.ViewModels;

public partial class InspectionViewModel : BaseViewModel
{
    private const int INSPECTION_TICKS = 15;
    
    
    
    private readonly Stopwatch _stopwatch = new();
    
    
    
    [ObservableProperty]
    private int _remainedTicks;
    
    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty] 
    private Color _timerColor;
    
    
    
    public InspectionViewModel()
    {
        _remainedTicks = INSPECTION_TICKS;
        _timerColor = Colors.White;
        StartInspection();
    }
    
    
    
    [RelayCommand]
    private async Task StartTimerAsync()
    {
        if (IsBusy)
            return;
        
        try
        {
            IsBusy = true;
            StopInspection();
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

    [RelayCommand]
    private async Task ChangeColorOnPress()
    {
        this.TimerColor = Colors.Red;
    }
    
    
    
    private void StartInspection()
    {
        _stopwatch.Restart();

        IsRunning = true;

        Device.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                //ElapsedTime = FormatElapsedTime(_stopwatch.Elapsed);
                RemainedTicks--;
            });
            return IsRunning;
        });
    }
    
    private void StopInspection()
    {
        _stopwatch.Stop();
        IsRunning = false;
    }
}