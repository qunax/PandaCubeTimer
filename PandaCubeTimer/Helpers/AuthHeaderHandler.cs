using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using PandaCubeTimer.Services;

namespace PandaCubeTimer.Helpers;

public class AuthHeaderHandler : DelegatingHandler
{
    // Check if we need to use Preferences instead of SecureStorage (for MacCatalyst and iOS Simulators)
    private bool BypassSecureStorage => 
        (DeviceInfo.Platform == DevicePlatform.iOS && DeviceInfo.DeviceType == DeviceType.Virtual) || 
         DeviceInfo.Platform == DevicePlatform.MacCatalyst;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 1. Get token based on platform capabilities
        string? token = BypassSecureStorage 
            ? Preferences.Default.Get("access_token", string.Empty) 
            : await SecureStorage.Default.GetAsync("access_token");

        // 2. Attach token if available
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // 3. Global error handling
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            string errorMessage = "An unknown error occurred";

            try
            {
                var problem = JsonSerializer.Deserialize<ProblemDetails>(content);
                if (problem != null && !string.IsNullOrEmpty(problem.Detail))
                {
                    errorMessage = problem.Detail;
                }
            }
            catch
            {
                if (!string.IsNullOrEmpty(content))
                    errorMessage = content;
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await Application.Current.MainPage.DisplayAlert("Authorization Error", "Your session has expired. Please log in again.", "OK");
                    
                    // 4. Clear data on logout/expiration based on platform
                    if (BypassSecureStorage)
                    {
                        Preferences.Default.Remove("access_token");
                        Preferences.Default.Remove("refresh_token");
                        Preferences.Default.Remove("user_id");
                    }
                    else
                    {
                        SecureStorage.Default.Remove("access_token");
                        SecureStorage.Default.Remove("refresh_token");
                        SecureStorage.Default.Remove("user_id");
                    }
                    
                    Shell.Current.FlyoutIsPresented = false;
                    await Shell.Current.GoToAsync("//LoginPageAbsolute");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
                }
            });
        }

        return response;
    }
}