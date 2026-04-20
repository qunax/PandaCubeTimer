using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using PandaCubeTimer.Models.Tutorials;

namespace PandaCubeTimer.ViewModels;

public partial class PllTrainingsViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<TutorialAlgoDTO> _algos = new();
    
    
    public PllTrainingsViewModel()
    {
        FillWithPlls();
    }

    private void FillWithPlls()
    {
        Algos.Clear();
        
        Algos.Add(new TutorialAlgoDTO()
        {
            Name = "Aa-perm",
            Description = "Test Algo description",
            Algorithm = "Test Algo algorithm",
            AlgoImagePath = "aa-perm.png" 
        });
    }
}