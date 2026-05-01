using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PandaCubeTimer.ViewModels;
using PandaCubeTimer.ViewModels.ControlsVMs;

namespace PandaCubeTimer.Views;

public partial class SolvesView : ContentPage
{
    public SolvesView(SolvesViewModel viewModel,
                    ActiveSessionBarViewModel  activeSessionBarViewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
        ActiveSessionBar.BindingContext = activeSessionBarViewModel;
    }
}