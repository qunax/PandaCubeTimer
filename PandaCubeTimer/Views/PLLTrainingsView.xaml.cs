using PandaCubeTimer.ViewModels;

namespace PandaCubeTimer.Views;

public partial class PLLTrainingsView : ContentPage
{
    public PLLTrainingsView(PllTrainingsViewModel vm)
    {
        InitializeComponent();
        this.BindingContext = vm;
    }
}