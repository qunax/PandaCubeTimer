using PandaCubeTimer.ViewModels;

namespace PandaCubeTimer;

public partial class App : Application
{
    public App(AppShellViewModel shellViewModel)
    {
        InitializeComponent();

        MainPage = new AppShell(shellViewModel);
    }
}