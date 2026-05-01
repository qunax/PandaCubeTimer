using System.Diagnostics;
using CommunityToolkit.Maui.Core;
using MauiIcons.Core;
using PandaCubeTimer.ViewModels;
using PandaCubeTimer.ViewModels.ControlsVMs;

namespace PandaCubeTimer.Views;

public partial class TimerView : ContentPage
{
    public TimerView(TimerViewModel viewModel,
                    ActiveSessionBarViewModel  activeSessionBarViewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        ActiveSessionBar.BindingContext = activeSessionBarViewModel;
        
        _ = new MauiIcon(); //done for making visible to page because of bug marked by the author of the nuget
    }
}