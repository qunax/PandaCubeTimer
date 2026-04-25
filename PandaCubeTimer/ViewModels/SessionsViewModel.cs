using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PandaCubeTimer.Data.Repositories;
using PandaCubeTimer.Models;
using PandaCubeTimer.Models.DTOs;

namespace PandaCubeTimer.ViewModels;

public partial class SessionsViewModel : BaseViewModel
{
    private readonly ILogger<SessionsViewModel> _logger;
    private readonly SessionRepository _sessionRepository;
    
    
    
    [ObservableProperty] 
    private bool _isRefreshing;
    
    [ObservableProperty]
    private ObservableCollection<SessionDTO> _sessions = new();
    
    [ObservableProperty]
    private SessionDTO? _selectedSession;
    
    
    
    public SessionsViewModel(SessionRepository repository, ILogger<SessionsViewModel> logger)
    {
        _sessionRepository = repository;
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




    private async Task LoadSessionsFromDbAsync()
    {
        List<SessionDTO> sessions = await _sessionRepository.GetAllSessionsAsync();
        Sessions = new ObservableCollection<SessionDTO>(sessions);
    }
}