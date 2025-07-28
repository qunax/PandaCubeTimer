namespace PandaCubeTimer;


public interface IAppSettingsService
{
    public bool IsInspectionTurnedOn {get; set;}
}



public class AppSettingsService : IAppSettingsService
{
    private const string IdIsInspectionTurnedOn = "is_inspection_turned_on";
    private const bool IsInspectionTurnedOnDefault = true;

    public bool IsInspectionTurnedOn
    {
        get => Preferences.Get(IdIsInspectionTurnedOn, IsInspectionTurnedOnDefault);
        set => Preferences.Set(IdIsInspectionTurnedOn, value);
    }
}