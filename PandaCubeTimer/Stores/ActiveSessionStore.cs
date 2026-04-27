using CommunityToolkit.Mvvm.Messaging;
using PandaCubeTimer.Messages;
using PandaCubeTimer.Models;

namespace PandaCubeTimer.Stores;

public class ActiveSessionStore
{
    public Session CurrentSession { get; private set; }

    public void SetSession(Session newSession)
    {
        CurrentSession = newSession;
            
        WeakReferenceMessenger.Default.Send(new ActiveSessionChangedMessage(newSession));
    }
}