using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PandaCubeTimer.ViewModels;

namespace PandaCubeTimer.Views;

public partial class OLLTrainingsView : ContentPage
{
    public OLLTrainingsView(OllTrainingsViewModel vm)
    {
        InitializeComponent();
        this.BindingContext = vm;
    }
}