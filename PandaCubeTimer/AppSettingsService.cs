using PandaCubeTimer.Models;

namespace PandaCubeTimer;


public interface IAppSettingsService
{
    public bool IsInspectionTurnedOn {get; set;}
    public string StartupSessionId {get; set;}
}



public class AppSettingsService : IAppSettingsService
{
    private const string IdIsInspectionTurnedOn = "is_inspection_turned_on";
    private const bool IsInspectionTurnedOnDefault = true;
    
    private const string IdStartupSessionId = "startup_session_id";
    private const string StartupSessionIdDefault = "00000000-0000-0000-0000-000000000001";

    
    
    public bool IsInspectionTurnedOn
    {
        get => Preferences.Get(IdIsInspectionTurnedOn, IsInspectionTurnedOnDefault);
        set => Preferences.Set(IdIsInspectionTurnedOn, value);
    }

    public string StartupSessionId
    {
        get => Preferences.Get(IdStartupSessionId, StartupSessionIdDefault);
        set => Preferences.Set(IdStartupSessionId, value);
    }
}