using CommunityToolkit.Maui.Views;
using PandaCubeTimer.Models;

namespace PandaCubeTimer.Views.Popups;

public partial class NewSessionPopup : Popup
{
    public NewSessionPopup(List<Discipline> disciplines)
    {
        InitializeComponent();
        
        // Load the list into CollectionView
        DisciplinePicker.ItemsSource = disciplines;
        
        // Auto-select the first discipline (e.g., 3x3)
        if (disciplines.Count > 0)
        {
            DisciplinePicker.SelectedItem = disciplines[0];
        }
    }

    private void OnCancelClicked(object sender, EventArgs e) => Close(null);

    private async void OnCreateClicked(object sender, EventArgs e)
    {
        // 1. Валидация текста
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            NameErrorLabel.IsVisible = true;
            NameEntry.PlaceholderColor = Colors.Red; 
            await ShakeViewAsync(NameEntry);
            return; 
        }

        // 2. Проверка, что дисциплина точно выбрана (на всякий случай)
        if (DisciplinePicker.SelectedItem is not Discipline selectedDiscipline)
        {
            return;
        }

        Close(new Session 
        { 
            Name = NameEntry.Text, 
            DisciplineId = (DisciplinePicker.SelectedItem as Discipline).Id 
        });
    }
    
    // Optional: Hide the error as soon as the user starts typing
    public void OnNameEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        NameErrorLabel.IsVisible = false;
        
        // Reset placeholder color to default (usually Gray or AppTheme dependent)
        NameEntry.PlaceholderColor = Colors.Gray; 
    }

    // Helper method for the shake animation
    private async Task ShakeViewAsync(View view)
    {
        uint timeout = 50;
        await view.TranslateTo(-15, 0, timeout);
        await view.TranslateTo(15, 0, timeout);
        await view.TranslateTo(-10, 0, timeout);
        await view.TranslateTo(10, 0, timeout);
        await view.TranslateTo(-5, 0, timeout);
        await view.TranslateTo(5, 0, timeout);
        view.TranslationX = 0; // Reset
    }
}