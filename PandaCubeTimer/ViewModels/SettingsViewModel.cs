using CommunityToolkit.Mvvm.ComponentModel;

namespace PandaCubeTimer.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly IAppSettingsService _appSettingsService;
    
    
    
    public bool IsInspectionTurnedOn
    {
        get => _appSettingsService.IsInspectionTurnedOn;
        set
        {
            if (_appSettingsService.IsInspectionTurnedOn != value)
            {
                OnPropertyChanging();
                _appSettingsService.IsInspectionTurnedOn = value;
                OnPropertyChanged();   
            }
        }
    }


    public SettingsViewModel(IAppSettingsService appSettingsService)
    {
        _appSettingsService = appSettingsService;
    }
}