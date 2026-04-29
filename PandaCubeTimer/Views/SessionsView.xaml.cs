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
    
    // protected override void OnAppearing()
    // {
    //     base.OnAppearing();
    //
    //     // Как только страница появляется, проверяем, выбрана ли сессия во ViewModel
    //     if (BindingContext is SessionsViewModel vm && vm.SelectedSession != null)
    //     {
    //         // Быстро сбрасываем и возвращаем SelectedItem в UI.
    //         // Так как навигация теперь висит на TapGestureRecognizer, 
    //         // этот трюк НЕ вызовет случайного перехода на экран таймера!
    //         SessionsList.SelectedItem = null;
    //         SessionsList.SelectedItem = vm.SelectedSession;
    //     }
    // }
}