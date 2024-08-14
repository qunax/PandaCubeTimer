using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PandaCubeTimer.Views;

namespace PandaCubeTimer.ViewModels;

public partial class CountingTimerViewModel : BaseViewModel
{
    [ObservableProperty]
    //[NotifyPropertyChangedFor(nameof(CurrentCountingTime))]
    private Stopwatch _stopwatch = new();
    
    [ObservableProperty]
    private TimeSpan _elapsed;

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private string _elapsedTime;



    public CountingTimerViewModel()
    {
        Start();
    }



    [RelayCommand]
    private async Task StopTimerAsync()
    {
        Stop();
        try
        {
            await Shell.Current.GoToAsync($"..", false, new Dictionary<string, object>
            {
                {"LastSolveTime", _elapsedTime}
            });
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
        _stopwatch.Restart();

        IsRunning = true;

        Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ElapsedTime = FormatElapsedTime(_stopwatch.Elapsed);
            });
            return _isRunning;
        });
    }

    
    
    private void Stop()
    {
        _stopwatch.Stop();
        IsRunning = false;
    }
    
    
    
    private string FormatElapsedTime(TimeSpan timeSpan)
    {
        return timeSpan.ToString(@"mm\:ss\.fff");
    }
}