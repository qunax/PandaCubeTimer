using System.Diagnostics;
using CommunityToolkit.Maui.Core;
using MauiIcons.Core;
using PandaCubeTimer.ViewModels;

namespace PandaCubeTimer.Views;

public partial class TimerView : ContentPage
{
    public TimerView(TimerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        
        _ = new MauiIcon(); //done for making visible to page because of bug marked by the author of the nuget
    }
}