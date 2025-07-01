using System.Diagnostics;
using MauiIcons.Core;
using PandaCubeTimer.ViewModels;
using Plugin.Maui.KeyListener;

namespace PandaCubeTimer.Views;

public partial class TimerView : ContentPage
{
    KeyboardBehavior keyboardBehavior = new ();
    
    
    public TimerView(TimerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        
        // _keyboardBehavior = new KeyboardBehavior();
        // this.Behaviors.Add(_keyboardBehavior);
        // // _keyboardBehavior.KeyDown += (sender, args) =>
        // // {
        // //     Debug.WriteLine($"KeyDown: {args.Keys.ToString()}");
        // //     Debug.WriteLine($"KeyDown Char: {args.KeyChar}");
        // // };
        // _keyboardBehavior.KeyUp += (sender, args) =>
        // {
        //     Debug.WriteLine($"KeyUp: {args.Keys.ToString()}");
        //     Debug.WriteLine($"KeyUp Char: {args.KeyChar}");
        // };
        
        _ = new MauiIcon(); //done for making visible to page because of bug marked by the author of the nuget
    }
    
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        keyboardBehavior.KeyDown += OnKeyDown;
        keyboardBehavior.KeyUp += OnKeyUp;
        this.Behaviors.Add(keyboardBehavior);

        base.OnNavigatedTo(args);
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        keyboardBehavior.KeyDown -= OnKeyDown;
        keyboardBehavior.KeyUp -= OnKeyUp;
        this.Behaviors.Remove(keyboardBehavior);

        base.OnNavigatedFrom(args);
    }

    void OnKeyUp(object sender, KeyPressedEventArgs args)
    {
        // do something
    }

    void OnKeyDown(object sender, KeyPressedEventArgs args)
    {
        // do something
    }
}