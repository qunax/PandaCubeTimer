using PandaCubeTimer.ViewModels;
using PandaCubeTimer.Views;

namespace PandaCubeTimer;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        Routing.RegisterRoute($"{nameof(CountingTimerView)}", typeof(CountingTimerView));
        Routing.RegisterRoute($"{nameof(TimerView)}", typeof(TimerView));

    }
}