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

    private void PointerGestureRecognizer_OnPointerPressed(object? sender, PointerEventArgs e)
    {
        //throw new NotImplementedException();
        // use it to change color of clock
        // TODO: settings turn inspection off (just put if on navigation in TimerView)
        // TODO: refactor all the properties of CountingTimerVM because for example
        // IsRunnning doesnt make sense for me now
        // TODO: old timer is obsolete (though keeps working, interesing)
    }
}