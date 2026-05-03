using CommunityToolkit.Mvvm.ComponentModel;

namespace PandaCubeTimer.Stores;

public partial class UserInfoStore : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsLoggedIn))]
    [NotifyPropertyChangedFor(nameof(IsLoggedOut))]
    private string? _username = null;

    public bool IsLoggedIn => !string.IsNullOrEmpty(Username);
    public bool IsLoggedOut => !IsLoggedIn;
}