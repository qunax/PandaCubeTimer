using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PandaCubeTimer.Views.Controls;

public partial class TouchOverlayButton : ContentView
{
    public static readonly BindableProperty PressedCommandProperty =
        BindableProperty.Create(nameof(PressedCommand), typeof(ICommand), typeof(TouchOverlayButton));

    public ICommand PressedCommand
    {
        get => (ICommand)GetValue(PressedCommandProperty);
        set => SetValue(PressedCommandProperty, value);
    }

    public static readonly BindableProperty ReleasedCommandProperty =
        BindableProperty.Create(nameof(ReleasedCommand), typeof(ICommand), typeof(TouchOverlayButton));

    public ICommand ReleasedCommand
    {
        get => (ICommand)GetValue(ReleasedCommandProperty);
        set => SetValue(ReleasedCommandProperty, value);
    }

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(TouchOverlayButton));

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public TouchOverlayButton()
    {
        InitializeComponent();
    }
}