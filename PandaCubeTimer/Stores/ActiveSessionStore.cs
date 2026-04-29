using CommunityToolkit.Mvvm.Messaging;
using PandaCubeTimer.Messages;
using PandaCubeTimer.Models;

namespace PandaCubeTimer.Stores;

public class ActiveSessionStore
{
    private readonly IAppSettingsService _appSettingsService;
    
    
    public Session? CurrentSession { get; private set; }


    
    public ActiveSessionStore(IAppSettingsService appSettingsService)
    {
        _appSettingsService = appSettingsService;
    }
    
    
    
    public void SetSession(Session newSession)
    {
        CurrentSession = newSession;
            
        WeakReferenceMessenger.Default.Send(new ActiveSessionChangedMessage(newSession));
        _appSettingsService.StartupSessionId = CurrentSession.Id.ToString();
    }
}