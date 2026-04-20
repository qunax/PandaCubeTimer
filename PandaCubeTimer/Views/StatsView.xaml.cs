using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PandaCubeTimer.ViewModels;

namespace PandaCubeTimer.Views;

public partial class StatsView : ContentPage
{
    public StatsView(StatsViewModel statsViewModel)
    {
        InitializeComponent();
        BindingContext = statsViewModel;
    }
}