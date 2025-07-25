using PandaCubeTimer.ViewModels;
using PandaCubeTimer.Views;

namespace PandaCubeTimer;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        Routing.RegisterRoute($"{nameof(CountingTimerView)}", typeof(CountingTimerView));
        Routing.RegisterRoute($"{nameof(InspectionView)}", typeof(InspectionView));
        Routing.RegisterRoute($"{nameof(TimerView)}", typeof(TimerView));
        Routing.RegisterRoute($"{nameof(SettingsView)}", typeof(SettingsView));
        Routing.RegisterRoute($"{nameof(OLLTrainingsView)}", typeof(OLLTrainingsView));
        Routing.RegisterRoute($"{nameof(PLLTrainingsView)}", typeof(PLLTrainingsView));
        Routing.RegisterRoute($"{nameof(SolvesView)}", typeof(SolvesView));
        Routing.RegisterRoute($"{nameof(StatsView)}", typeof(StatsView));
    }
}