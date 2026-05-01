using PandaCubeTimer.Stores;

namespace PandaCubeTimer.ViewModels.ControlsVMs;

public partial class ActiveSessionBarViewModel : BaseViewModel
{
    public ActiveSessionStore ActiveSessionStore { get; private set; }
    
    
    
    public ActiveSessionBarViewModel(ActiveSessionStore activeSessionStore)
    {
        ActiveSessionStore = activeSessionStore;
    }
}