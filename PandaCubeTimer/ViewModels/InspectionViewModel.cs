using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PandaCubeTimer.Models;
using PandaCubeTimer.Views;

namespace PandaCubeTimer.ViewModels;

public partial class InspectionViewModel : BaseViewModel
{
    private const int INSPECTION_TICKS = 15;
    
    
    
    private readonly Stopwatch _stopwatch = new();
    
    
    
    [ObservableProperty]
    private int _remainedTicks;

    /// <summary>
    /// also shows if there's penalty
    /// </summary>
    [ObservableProperty] 
    private string _remainedTicksText;

    [ObservableProperty]
    private SolvePenalty _inspectionPenalty = SolvePenalty.NoPenalty;
    
    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty] 
    private Color _timerColor;
    
    
    
    public InspectionViewModel()
    {
        _remainedTicks = INSPECTION_TICKS;
        
        if (Application.Current?.RequestedTheme == AppTheme.Dark)
        {
            _timerColor = Colors.White;
        }
        else
        {
            _timerColor = Colors.Black;
        }
        
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
            await Shell.Current.GoToAsync($"{nameof(CountingTimerView)}", false, new Dictionary<string, object>
            {
                {nameof(CountingTimerViewModel.InspectionPenalty), InspectionPenalty}
            });
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
    private void ChangeColorOnPress()
    {
        this.TimerColor = Colors.Green;
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
                
                if (RemainedTicks == 7)
                    TimerColor = Colors.Orange;

                if (RemainedTicks == 3)
                    TimerColor = Colors.Red;

                if (RemainedTicks == 0)
                {
                    InspectionPenalty = SolvePenalty.PlusTwo;
                    RemainedTicksText = "+2";
                }

                if (RemainedTicks == -3)
                {
                    InspectionPenalty = SolvePenalty.DNF;
                    RemainedTicksText = "DNF";
                    StopInspection();
                }
                    
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