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

    private void TouchBehavior_OnTouchGestureCompleted(object? sender, TouchGestureCompletedEventArgs e)
    {
        //throw new NotImplementedException();
    }

    private void PointerGestureRecognizer_OnPointerPressed(object? sender, PointerEventArgs e)
    {
        //throw new NotImplementedException();
    }
}