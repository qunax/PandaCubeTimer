using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PandaCubeTimer.Models.Tutorials;

namespace PandaCubeTimer.ViewModels;

public partial class OllTrainingsViewModel : BaseViewModel
{
    private readonly ILogger _logger;
    
    
    
    [ObservableProperty]
    private ObservableCollection<TutorialAlgoDTO> _algs = new();
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAlgOpened))]
    private TutorialAlgoDTO? _selectedAlg;

    
    
    public bool IsAlgOpened => SelectedAlg != null;
    
    
    
    public OllTrainingsViewModel(ILogger<OllTrainingsViewModel> logger)
    {
        _logger = logger;
    }

    
    
    [RelayCommand]
    private async Task LoadAlgorithmsAsync()
    {
        if(IsBusy)
            return;
        
        if (Algs.Count == 57)
            return;

        try
        {
            IsBusy = true;
            Algs.Clear();
            
            await using var stream =
                await FileSystem.OpenAppPackageFileAsync("all_oll_tutorial_algs.json");
            using var reader = new StreamReader(stream);

            var jsonContent = await reader.ReadToEndAsync();

            var algorithms = JsonSerializer.Deserialize<List<TutorialAlgoDTO>>(jsonContent);
            if (algorithms != null)
            {
                foreach (var algo in algorithms)
                {
                    Algs.Add(algo);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load OLL algorithms.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void SelectAlgorithm(TutorialAlgoDTO algo)
    {
        SelectedAlg = algo;
    }
    
    [RelayCommand]
    private void CloseSelectedAlg()
    {
        SelectedAlg = null;
    }
}