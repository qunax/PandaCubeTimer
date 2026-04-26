using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PandaCubeTimer.Data.Repositories;
using PandaCubeTimer.Models;
using PandaCubeTimer.Models.DTOs;
using PandaCubeTimer.Views.Popups;

namespace PandaCubeTimer.ViewModels;

public partial class SessionsViewModel : BaseViewModel
{
    private readonly ILogger<SessionsViewModel> _logger;
    private readonly SessionRepository _sessionRepository;
    private readonly DisciplineRepository _disciplineRepository;
    
    
    
    [ObservableProperty] 
    private bool _isRefreshing;
    
    [ObservableProperty]
    private ObservableCollection<SessionDTO> _sessions = new();
    
    [ObservableProperty]
    private SessionDTO? _selectedSession;
    
    
    
    public SessionsViewModel(SessionRepository repository,
        DisciplineRepository disciplineRepository, 
        ILogger<SessionsViewModel> logger)
    {
        _sessionRepository = repository;
        _disciplineRepository = disciplineRepository;
        _logger = logger;
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
    private void SelectSession(SessionDTO session)
    {
        this.SelectedSession = session;
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
            await LoadSessionsFromDbAsync();
        }
    }




    private async Task LoadSessionsFromDbAsync()
    {
        List<SessionDTO> sessions = await _sessionRepository.GetAllSessionsAsync();
        Sessions = new ObservableCollection<SessionDTO>(sessions);
    }
}