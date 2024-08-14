using System.Diagnostics;
using PandaCubeTimer.ViewModels;
using Plugin.Maui.KeyListener;

namespace PandaCubeTimer.Views;

public partial class CountingTimerView : ContentPage
{
    
    
    
    public CountingTimerView(CountingTimerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        
        
    }

    private void TimeText_OnLoaded(object? sender, EventArgs e)
    {
        TimeText.ScaleTo(1.3);
    }
}