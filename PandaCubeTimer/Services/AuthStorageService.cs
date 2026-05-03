namespace PandaCubeTimer.Services;

public class AuthStorageService
{
    private bool BypassSecureStorage => 
        (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.DeviceType == DeviceType.Virtual) || 
        DeviceInfo.Platform == DevicePlatform.MacCatalyst;

    public async Task SaveAuthDataAsync(string token, string refreshToken, string userId, string username)
    {
        if (BypassSecureStorage)
        {
            Preferences.Default.Set("access_token", token);
            Preferences.Default.Set("refresh_token", refreshToken);
            Preferences.Default.Set("user_id", userId);
            Preferences.Default.Set("username", username);
        }
        else
        {
            await SecureStorage.Default.SetAsync("access_token", token);
            await SecureStorage.Default.SetAsync("refresh_token", refreshToken);
            await SecureStorage.Default.SetAsync("user_id", userId);
            await SecureStorage.Default.SetAsync("username", username);
        }
    }

    public async Task<string?> GetTokenAsync() => 
        BypassSecureStorage ? Preferences.Default.Get("access_token", string.Empty) : await SecureStorage.Default.GetAsync("access_token");

    public async Task<string?> GetRefreshTokenAsync() => 
        BypassSecureStorage ? Preferences.Default.Get("refresh_token", string.Empty) : await SecureStorage.Default.GetAsync("refresh_token");
    
    public async Task<string?> GetUsernameAsync() => 
        BypassSecureStorage ? Preferences.Default.Get("username", string.Empty) : await SecureStorage.Default.GetAsync("username");

    public async Task ClearAuthDataAsync()
    {
        if (BypassSecureStorage)
        {
            Preferences.Default.Set("access_token", string.Empty);
            Preferences.Default.Set("refresh_token", string.Empty);
            Preferences.Default.Set("user_id", string.Empty);
            Preferences.Default.Set("username", string.Empty);
        }
        else
        {
            await SecureStorage.Default.SetAsync("access_token", string.Empty);
            await SecureStorage.Default.SetAsync("refresh_token", string.Empty);
            await SecureStorage.Default.SetAsync("user_id", string.Empty);
            await SecureStorage.Default.SetAsync("username", string.Empty);
        }
    }
}