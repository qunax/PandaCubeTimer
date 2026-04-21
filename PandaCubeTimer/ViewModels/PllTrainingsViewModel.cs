using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PandaCubeTimer.Models.Tutorials;

namespace PandaCubeTimer.ViewModels;

public partial class PllTrainingsViewModel : BaseViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAlgsListEmpty))]
    private ObservableCollection<TutorialAlgoDTO> _algs = new();
    
    [ObservableProperty]
    private TutorialAlgoDTO? _selectedAlg;

    public bool IsAlgsListEmpty => Algs.Count < 1; 
    
    
    
    public PllTrainingsViewModel()
    {
    }

    
    
    [RelayCommand]
    private async Task LoadAlgorithmsAsync()
    {
        try
        {
            await using var stream =
                await FileSystem.OpenAppPackageFileAsync("all_pll_tutorial_algs.json");
            using var reader = new StreamReader(stream);

            var jsonContent = await reader.ReadToEndAsync();

            var algorithms = JsonSerializer.Deserialize<List<TutorialAlgoDTO>>(jsonContent);
            if (algorithms != null)
            {
                Algs.Clear();
                foreach (var algo in algorithms)
                {
                    Algs.Add(algo);
                }
            }
        }
        catch (Exception ex)
        {
            
        }
    }
}