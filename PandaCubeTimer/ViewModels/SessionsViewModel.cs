using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using PandaCubeTimer.Data.Repositories;
using PandaCubeTimer.Helpers;
using PandaCubeTimer.Messages;
using PandaCubeTimer.Models;
using PandaCubeTimer.Models.DTOs;
using PandaCubeTimer.Stores;
using PandaCubeTimer.Views.Popups;

namespace PandaCubeTimer.ViewModels;

public partial class SessionsViewModel : BaseViewModel
{
    private readonly ILogger<SessionsViewModel> _logger;
    private readonly SessionRepository _sessionRepository;
    private readonly DisciplineRepository _disciplineRepository;
    private readonly ActiveSessionStore _activeSessionStore;
    
    
    
    [ObservableProperty] 
    private bool _isRefreshing;
    
    [ObservableProperty]
    private ObservableCollection<SessionDTO> _sessions = new();
    
    
    
    public SessionsViewModel(SessionRepository repository,
        DisciplineRepository disciplineRepository, 
        ActiveSessionStore activeSessionStore,
        ILogger<SessionsViewModel> logger)
    {
        _sessionRepository = repository;
        _disciplineRepository = disciplineRepository;
        _activeSessionStore = activeSessionStore;
        _logger = logger;
        
        ConfigureMessageRecieving();
    }
    
    private void ConfigureMessageRecieving()
    {
        // reload solves for selected session:
        WeakReferenceMessenger.Default.Register<ActiveSessionChangedMessage>(this, (r, m) =>
        {
            OnActiveSessionChangedReceived(m.Value);
        });
    }
    
    
    
    private async void OnActiveSessionChangedReceived(Session messageValue)
    {
        UpdateActiveSessionSelectedState();
    }

    

    [RelayCommand]
    private async Task LoadSessionsAsync()
    {
        if(IsBusy)
            return;
        
        try
        {
            IsBusy = true;
            IsRefreshing = true;
            
            await LoadSessionsFromDbAsync();
            UpdateActiveSessionSelectedState();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading sessions");
            await Shell.Current.DisplayAlert("Error!", 
                $"Unable to load sessions: {ex.Message}", "Ok");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task SelectSessionAsync(SessionDTO session)
    {
        await _activeSessionStore.SetSessionAsync(session.ToModel());
        UpdateActiveSessionSelectedState();
    }
    
    [RelayCommand]
    public async Task AddSessionAsync()
    {
        var disciplines = await _disciplineRepository.GetAllDisciplinesAsync();

        var popup = new NewSessionPopup(disciplines);
        var result = await Shell.Current.CurrentPage.ShowPopupAsync(popup);

        if (result is Session newSession)
        {
            await _sessionRepository.InsertAsync(newSession);
            await _activeSessionStore.SetSessionAsync(newSession);
            await LoadSessionsAsync();
        }
    }




    private async Task LoadSessionsFromDbAsync()
    {
        List<SessionDTO> sessions = await _sessionRepository.GetAllSessionsDTOsAsync();
        Sessions = new ObservableCollection<SessionDTO>(sessions);
    }

    private void UpdateActiveSessionSelectedState()
    {
        if (_activeSessionStore.CurrentSession is null)
            return;
        
        foreach (var sessionDto in Sessions)
        {
            if (sessionDto.Id == _activeSessionStore.CurrentSession.Id)
                sessionDto.IsSelected = true;
            else
                sessionDto.IsSelected = false;
        }
    }
}