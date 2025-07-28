using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core;
using PandaCubeTimer.ViewModels;

namespace PandaCubeTimer.Views;

public partial class InspectionView : ContentPage
{
    public InspectionView(InspectionViewModel vm)
    {
        InitializeComponent();
        this.BindingContext = vm;
    }
    
    private void TimeText_OnLoaded(object? sender, EventArgs e)
    {
        TimeText.ScaleTo(1.3);
    }
}