using CommunityToolkit.Mvvm.Input;
using PandaCubeTimer.Services;
using PandaCubeTimer.Stores;

namespace PandaCubeTimer.ViewModels;

public partial class AppShellViewModel : BaseViewModel
{
    private readonly AuthStorageService  _authStorageService;
    
    public  UserInfoStore UserInfoStore { get; }

    
    
    public AppShellViewModel(UserInfoStore userInfoStore, AuthStorageService authStorageService)
    {
        _authStorageService =  authStorageService;
        UserInfoStore = userInfoStore;
    }

    
    
    [RelayCommand]
    private async Task Logout()
    {
        await _authStorageService.ClearAuthDataAsync();
        UserInfoStore.Username = null;
    }
    
    [RelayCommand]
    private async Task GoToLoginAsync()
    {
        // Close the flyout menu before navigating
        Shell.Current.FlyoutIsPresented = false;
        await Shell.Current.GoToAsync("//LoginPage" );
    }
}