using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PandaCubeTimer.ViewModels;

public partial class BaseViewModel : ObservableObject, INotifyPropertyChanged
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;


    
    public bool IsNotBusy => !_isBusy;



    // public event PropertyChangedEventHandler PropertyChanged;
    //
    //
    //
    // protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    // {
    //     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    // }
}