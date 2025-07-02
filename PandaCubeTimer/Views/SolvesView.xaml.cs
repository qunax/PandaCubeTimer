using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PandaCubeTimer.ViewModels;

namespace PandaCubeTimer.Views;

public partial class SolvesView : ContentPage
{
    public SolvesView(SolvesViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }
}