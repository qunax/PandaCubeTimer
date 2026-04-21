using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PandaCubeTimer.Models.Tutorials;

namespace PandaCubeTimer.ViewModels;

public partial class OllTrainingsViewModel : BaseViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAlgsListEmpty))]
    private ObservableCollection<TutorialAlgoDTO> _algs;
    
    [ObservableProperty]
    private TutorialAlgoDTO? _selectedAlg;

    public bool IsAlgsListEmpty => Algs.Count < 1; 
    
    
    
    public OllTrainingsViewModel()
    {
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
            
        }
    }
}