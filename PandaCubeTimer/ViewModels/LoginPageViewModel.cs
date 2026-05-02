using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PandaCubeTimer.Services;

namespace PandaCubeTimer.ViewModels;

public partial class LoginPageViewModel : BaseViewModel
{
    private readonly ILogger<LoginPageViewModel> _logger;
    private readonly IPandaCubeTimer_API _api;

    
    
    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _password;

    
    
    public LoginPageViewModel(IPandaCubeTimer_API api, ILogger<LoginPageViewModel> logger)
    {
        _api = api;
        _logger = logger;
    }

    
    
    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Alert!", "Enter a username and a password, please", "OK");
            return;
        }

        IsBusy = true;

        try
        {
            var request = new UserLoginDTO 
            { 
                Username = this.Username, 
                Password = this.Password,
                DeviceName = DeviceInfo.Name 
            };

            var response = await _api.Login(request);

            if (response != null && !string.IsNullOrEmpty(response.AccessToken))
            {
                
                // Bypass SecureStorage (Keychain) for iOS Simulator AND Mac Catalyst
                // because both require Apple Developer certificates for local debugging.
                // and i dont have paid account
                bool bypassSecureStorage = (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.DeviceType == DeviceType.Virtual) || 
                                           DeviceInfo.Platform == DevicePlatform.MacCatalyst;
                
                if (bypassSecureStorage)
                {
                    Preferences.Default.Set("access_token", response.AccessToken);
                    Preferences.Default.Set("refresh_token", response.RefreshToken);
                    Preferences.Default.Set("user_id", response.UserId.ToString());
                }
                else
                {
                    // Safe and encrypted storage for Android and real physical iPhones
                    await SecureStorage.Default.SetAsync("access_token", response.AccessToken);
                    await SecureStorage.Default.SetAsync("refresh_token", response.RefreshToken);
                    await SecureStorage.Default.SetAsync("user_id", response.UserId.ToString());
                }
                
                Application.Current.MainPage = new AppShell(); 
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Login error: " + ex.Message);
            await Application.Current.MainPage.DisplayAlert("Error", 
                "An error occured while logging in: " + ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}