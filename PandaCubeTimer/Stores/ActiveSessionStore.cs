using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Messaging;
using PandaCubeTimer.Data.Repositories;
using PandaCubeTimer.Messages;
using PandaCubeTimer.Models;
using PandaCubeTimer.Models.DTOs;

namespace PandaCubeTimer.Stores;

public class ActiveSessionStore : INotifyPropertyChanged
{
    private readonly IAppSettingsService _appSettingsService;
    private readonly SessionRepository _sessionRepository;
    
    
    
    private Session? _session;
    public Session? CurrentSession
    {
        get => _session;
        private set
        {
            _session = value;
            OnPropertyChanged();
        }
    }

    
    
    private SessionDTO? _sessionDTO;
    public SessionDTO? CurrentSessionDTO
    {
        get => _sessionDTO;
        private set
        {
            _sessionDTO = value;
            OnPropertyChanged();
        }
    }



    public ActiveSessionStore(IAppSettingsService appSettingsService,
                            SessionRepository  sessionRepository)
    {
        _appSettingsService = appSettingsService;
        _sessionRepository = sessionRepository;
    }
    
    
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    
    
    public async Task SetSessionAsync(Session newSession)
    {
        CurrentSession = newSession;
        CurrentSessionDTO = await _sessionRepository.GetSessionDTOByIdAsync(newSession.Id);
            
        WeakReferenceMessenger.Default.Send(new ActiveSessionChangedMessage(newSession));
        _appSettingsService.StartupSessionId = CurrentSession.Id.ToString();
    }
}