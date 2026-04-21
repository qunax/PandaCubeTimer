using PandaCubeTimer.ViewModels;

namespace PandaCubeTimer.Views;

public partial class OLLTrainingsView : ContentPage
{
    public OLLTrainingsView(OllTrainingsViewModel vm)
    {
        InitializeComponent();
        this.BindingContext = vm;
    }
}