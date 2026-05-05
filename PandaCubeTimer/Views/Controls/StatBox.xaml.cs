namespace PandaCubeTimer.Views.Controls;

public partial class StatBox : ContentView
{
    // 1. Bindable Property for the Title (e.g., "Ao5")
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        propertyName: nameof(Title),
        returnType: typeof(string),
        declaringType: typeof(StatBox),
        defaultValue: string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    // 2. Bindable Property for the Value (e.g., "12.45")
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        propertyName: nameof(Value),
        returnType: typeof(string),
        declaringType: typeof(StatBox),
        defaultValue: "-");

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public StatBox()
    {
        InitializeComponent();
    }
}