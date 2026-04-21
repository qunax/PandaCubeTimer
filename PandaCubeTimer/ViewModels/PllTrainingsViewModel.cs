using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using PandaCubeTimer.Models.Tutorials;

namespace PandaCubeTimer.ViewModels;

public partial class PllTrainingsViewModel : BaseViewModel
{
    private readonly ILogger _logger;
    
    
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAlgsListEmpty))]
    private ObservableCollection<TutorialAlgoDTO> _algs = new();
    
    [ObservableProperty]
    private TutorialAlgoDTO? _selectedAlg;

    public bool IsAlgsListEmpty => Algs.Count < 1; 
    
    
    
    public PllTrainingsViewModel(ILogger<PllTrainingsViewModel> logger)
    {
        _logger = logger;
        
        Algs = new ObservableCollection<TutorialAlgoDTO>();
        //otherwise doesnt update value automatically:
        Algs.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(IsAlgsListEmpty));
        };
    }

    
    
    [RelayCommand]
    private async Task LoadAlgorithmsAsync()
    {
        try
        {
            Algs.Clear();
            await using var stream =
                await FileSystem.OpenAppPackageFileAsync("all_pll_tutorial_algs.json");
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
            _logger.LogError(ex, "Failed to load PLL algorithms.");
        }
    }
}