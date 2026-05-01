using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PandaCubeTimer.ViewModels;
using PandaCubeTimer.ViewModels.ControlsVMs;

namespace PandaCubeTimer.Views;

public partial class StatsView : ContentPage
{
    public StatsView(StatsViewModel statsViewModel,
                    ActiveSessionBarViewModel  activeSessionBarViewModel)
    {
        InitializeComponent();
        BindingContext = statsViewModel;
        ActiveSessionBar.BindingContext = activeSessionBarViewModel;
    }
}