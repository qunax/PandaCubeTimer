using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mapster;
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
    private readonly PuzzleSolveRepository _solveRepository;
    private readonly DisciplineRepository _disciplineRepository;
    private readonly ActiveSessionStore _activeSessionStore;
    private readonly UserInfoStore _userInfoStore;


    [ObservableProperty]
    private bool _isRefreshing;
    
    [ObservableProperty]
    private ObservableCollection<SessionInAppDTO> _sessions = new();
    
    public bool IsSyncVisible => _userInfoStore.IsLoggedIn;
    
    
    
    public SessionsViewModel(IPandaCubeTimer_API api,
        SessionRepository sessionRepository,
        PuzzleSolveRepository solveRepository,
        DisciplineRepository disciplineRepository, 
        ActiveSessionStore activeSessionStore,
        UserInfoStore userInfoStore,
        ILogger<SessionsViewModel> logger)
    {
        _pandaCubeTimerAPI = api;
        _sessionRepository = sessionRepository;
        _solveRepository = solveRepository;
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
    private async Task SelectSessionAsync(SessionInAppDTO sessionInApp)
    {
        if (IsBusy)
            return;
        
        IsBusy = true;

        try
        {
            await _activeSessionStore.SetSessionAsync(sessionInApp.Id);
            await UpdateActiveSessionSelectedState();
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
                await _activeSessionStore.SetSessionAsync(newSession.Id);
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
    private async Task DeleteSessionAsync(SessionInAppDTO sessionInApp)
    {
        if (IsBusy)
            return;
        
        bool isConfirmed = await Application.Current.MainPage.DisplayAlert(
            "Delete Session", 
            "Are you sure you want to delete this session? It will be deleted across all devices permanently.", 
            "Delete",
            "Cancel" 
        );
        
        if (!isConfirmed)
            return;
        
        IsBusy = true;
        try
        {
            await _sessionRepository.DeleteAsync(sessionInApp.Id);
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
    private async Task SyncAllSessionsAndSolvesAsync()
    {
        if (IsBusy) return;
        
        try
        {
            IsBusy = true;

            // 1. Retrieve the Last Sync Time from local storage (default to MinValue if first time)
            DateTime lastSyncTime = Preferences.Get("LastSyncTimeUtc", DateTime.MinValue);

            // 2. Gather all local items that need to be sent to the server
            var unsyncedSessions = await _sessionRepository.GetUnsyncedSessionsAsync(); // WHERE IsSynced = 0
            var unsyncedSolves = await _solveRepository.GetUnsyncedSolvesAsync();       // WHERE IsSynced = 0

            // 3. Fire the request to the API
            var request = new CompleteTimerSyncRequest()
            {
                LastSyncTimeUtc = new DateTimeOffset(lastSyncTime, TimeSpan.Zero),
                UnsyncedSessions = unsyncedSessions.Select(s => s.Adapt<SessionDTO>()).ToList(),
                UnsyncedSolves = unsyncedSolves.Select(s => s.Adapt<SolveDTO>()).ToList()
            };

            var response = await _pandaCubeTimerAPI.CompleteTimerSync(request);

            // 4. Save new stuff from the server locally (Upsert)
            foreach (var serverSession in response.ServerSessions)
            {
                var existing = await _sessionRepository.GetSessionByIdAsync(serverSession.Id);
                var localModel = serverSession.Adapt<Session>();
                localModel.IsSynced = true; // It came from the server, so it is synced
                localModel.UpdatedAt = DateTime.UtcNow;

                if (existing == null)
                    await _sessionRepository.InsertAsync(localModel);
                else
                    await _sessionRepository.UpdateAsync(localModel);
            }

            foreach (var serverSolve in response.ServerSolves)
            {
                var existing = await _solveRepository.GetPuzzleSolveAsync(serverSolve.Id);
                var localModel = serverSolve.Adapt<PuzzleSolve>();
                localModel.IsSynced = true; 
                localModel.UpdatedAt = DateTime.UtcNow;

                if (existing == null)
                    await _solveRepository.InsertAsync(localModel);
                else
                    await _solveRepository.UpdateAsync(localModel);
            }

            // 5. Update local 'IsSynced' based on what the server acknowledged
            foreach (var sessionId in response.AcknowledgedSessionIds)
            {
                await _sessionRepository.MarkAsSyncedAsync(sessionId);
            }

            foreach (var solveId in response.AcknowledgedSolveIds)
            {
                await _solveRepository.MarkAsSyncedAsync(solveId);
            }

            // 6. Update LastSyncTime for the next time this runs
            Preferences.Set("LastSyncTimeUtc", response.ServerTimeUtc.UtcDateTime);

            // 7. Refresh the UI
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
            await UpdateActiveSessionSelectedState();
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
        List<SessionInAppDTO> sessions = await _sessionRepository.GetAllSessionsDTOsAsync();
        Sessions = new ObservableCollection<SessionInAppDTO>(sessions);
    }

    private async Task UpdateActiveSessionSelectedState()
    {
        //try to set the default as a current
        if (_activeSessionStore.CurrentSession is null)
        {
            await _activeSessionStore.SetSessionAsync(Session.DefaultSessionId);
        }

        if (_activeSessionStore.CurrentSession != null)
        {
            foreach (var sessionDto in Sessions)
            {
                if (sessionDto.Id == _activeSessionStore.CurrentSession.Id)
                    sessionDto.IsSelected = true;
                else
                    sessionDto.IsSelected = false;
            }   
        }
    }
}