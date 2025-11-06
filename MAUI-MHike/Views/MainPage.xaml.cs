using MAUI_MHike.Models;
using System.Collections.ObjectModel;

namespace MAUI_MHike.Views;


public partial class MainPage : ContentPage
{
    public ObservableCollection<Hike> Hikes { get; } = new();

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;

        // Dummy data (until persistence wired)
        Hikes.Add(new Hike { Name = "Langbiang Peak", Location = "Da Lat", Date = DateTime.Today.AddDays(-30), Parking = true, LengthKm = 12.5, Difficulty = 5, Description = "Pine forest" });
        Hikes.Add(new Hike { Name = "Ba Den Mountain", Location = "Tay Ninh", Date = DateTime.Today.AddDays(-10), Parking = false, LengthKm = 6.3, Difficulty = 3, Description = "Sunrise!" });
        Hikes.Add(new Hike { Name = "Fansipan Trail", Location = "Lao Cai", Date = DateTime.Today, Parking = true, LengthKm = 14.0, Difficulty = 5, Description = "Roof of Indochina" });
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HikeFormPage), true, new Dictionary<string, object>
    {
        { "Mode", "add" }
    });
    }

    private async void OnSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection?.FirstOrDefault() is Hike h)
        {
            await Shell.Current.GoToAsync(nameof(HikeFormPage), true, new Dictionary<string, object>
        {
            { "Mode", "edit" },
            { "Item", h }
        });
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}