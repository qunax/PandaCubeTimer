using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PandaCubeTimer.ViewModels;

namespace PandaCubeTimer.Views;

public partial class SessionsView : ContentPage
{
    public SessionsView(SessionsViewModel sessionsViewModel)
    {
        InitializeComponent();
        BindingContext = sessionsViewModel;
    }
}