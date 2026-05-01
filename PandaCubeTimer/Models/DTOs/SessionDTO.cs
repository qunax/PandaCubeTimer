using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PandaCubeTimer.Models.DTOs;

public partial class SessionDTO : ObservableObject, INotifyPropertyChanged
{
    [ObservableProperty] 
    private Guid _id;

    [ObservableProperty] 
    private string _name = null!;
    
    [ObservableProperty] 
    private string _disciplineId = null!;
    
    [ObservableProperty] 
    private bool _isSelected;
    
    [ObservableProperty] 
    private string? _disciplineName;
    
    [ObservableProperty] 
    private ObservableCollection<PuzzleSolve>? _solves;
}