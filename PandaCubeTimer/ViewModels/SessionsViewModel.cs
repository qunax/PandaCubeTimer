using System.Collections.ObjectModel;
using System.ComponentModel;
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
using PandaCubeTimer.Services;
using PandaCubeTimer.Stores;
using PandaCubeTimer.Views.Popups;

namespace PandaCubeTimer.ViewModels;

public partial class SessionsViewModel : BaseViewModel
{
    private readonly ILogger<SessionsViewModel> _logger;
    private readonly IPandaCubeTimer_API _pandaCubeTimerAPI;
    private readonly SessionRepository _sessionRepository;
    private readonly DisciplineRepository _disciplineRepository;
    private readonly ActiveSessionStore _activeSessionStore;
    private readonly UserInfoStore _userInfoStore;


    [ObservableProperty]
    private bool _isRefreshing;
    
    [ObservableProperty]
    private ObservableCollection<SessionDTO> _sessions = new();
    
    public bool IsSyncVisible => _userInfoStore.IsLoggedIn;
    
    
    
    public SessionsViewModel(IPandaCubeTimer_API api,
        SessionRepository repository,
        DisciplineRepository disciplineRepository, 
        ActiveSessionStore activeSessionStore,
        UserInfoStore userInfoStore,
        ILogger<SessionsViewModel> logger)
    {
        _pandaCubeTimerAPI = api;
        _sessionRepository = repository;
        _disciplineRepository = disciplineRepository;
        _activeSessionStore = activeSessionStore;
        _userInfoStore = userInfoStore;
        _logger = logger;
        
        ConfigureMessageReceiving();
        _userInfoStore.PropertyChanged += OnStorePropertyChanged;
    }
    
    private void ConfigureMessageReceiving()
    {
        // reload solves for selected session:
        WeakReferenceMessenger.Default.Register<ActiveSessionChangedMessage>(this, (r, m) =>
        {
            OnActiveSessionChangedReceived(m.Value);
        });
    }
    
    
    
    private void OnStorePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_userInfoStore.IsLoggedIn))
        {
            OnPropertyChanged(nameof(IsSyncVisible)); 
        }
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
            await LoadSessionsAndUpdateCurrentAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SelectSessionAsync(SessionDTO session)
    {
        if (IsBusy)
            return;
        
        IsBusy = true;

        try
        {
            await _activeSessionStore.SetSessionAsync(session.ToModel());
            UpdateActiveSessionSelectedState();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting session: " + ex.Message);
            await Shell.Current.DisplayAlert("Error!", "Unable to select session: " + ex.Message, "Ok");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    [RelayCommand]
    private async Task AddSessionAsync()
    {
        if (IsBusy)
            return;
        
        IsBusy = true;
        try
        {
            var disciplines = await _disciplineRepository.GetAllDisciplinesAsync();

            var popup = new NewSessionPopup(disciplines);
            var result = await Shell.Current.CurrentPage.ShowPopupAsync(popup);

            if (result is Session newSession)
            {
                await _sessionRepository.InsertAsync(newSession);
                await _activeSessionStore.SetSessionAsync(newSession);
                await LoadSessionsAndUpdateCurrentAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding session: " + ex.Message);
            await Shell.Current.DisplayAlert("Error!", "Unable to add session", "Ok");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteSessionAsync(SessionDTO session)
    {
        if (IsBusy)
            return;
        
        IsBusy = true;
        try
        {
            await _sessionRepository.DeleteAsync(session.Id);
            await LoadSessionsAndUpdateCurrentAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting session: " + ex.Message);
            await Application.Current.MainPage.DisplayAlert("Session Deletion Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    [RelayCommand]
    private async Task StartSyncFlowAsync()
    {
        IsBusy = true;
        
        try
        {
            var sessionsToSync = await _sessionRepository.GetSessionsForSync();
            var sessionsToAccept = await _pandaCubeTimerAPI.SyncFull(sessionsToSync);

            foreach (var sessionDto in sessionsToAccept)
            {
                try
                {
                    await _sessionRepository.GetSessionByIdAsync(sessionDto.Id);
                }
                catch (Exception ex)
                {
                    await _sessionRepository.InsertAsync(sessionDto.ToModel());
                    _logger.LogInformation($"Session from server added: {sessionDto.Id}");
                }
            }

            await LoadSessionsAndUpdateCurrentAsync();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Sync Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }




    private async Task LoadSessionsAndUpdateCurrentAsync()
    {
        try
        {
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
            IsRefreshing = false;
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