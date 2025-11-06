using MAUI_MHike.Models;
using System.Globalization;

namespace MAUI_MHike.Views;


[QueryProperty(nameof(Mode), "Mode")]
[QueryProperty(nameof(Item), "Item")]
public partial class HikeFormPage : ContentPage
{
    public string Mode { get; set; } = "add"; // "add" or "edit"
    public Hike? Item { get; set; }

    public HikeFormPage()
    {
        InitializeComponent();
        DifficultyPicker.SelectedIndex = 2; // default "3 - Medium"
        DatePicker.Date = DateTime.Today;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (Mode == "edit" && Item != null)
        {
            TitleLabel.Text = "Edit Hike";
            NameEntry.Text = Item.Name;
            LocationEntry.Text = Item.Location;
            DatePicker.Date = Item.Date;
            ParkingSwitch.IsToggled = Item.Parking;
            LengthEntry.Text = Item.LengthKm.ToString(CultureInfo.InvariantCulture);
            DescEditor.Text = Item.Description;
            DifficultyPicker.SelectedIndex = Math.Clamp(Item.Difficulty - 1, 0, 4);
        }
        else
        {
            TitleLabel.Text = "Add Hike";
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // basic validation
        if (string.IsNullOrWhiteSpace(NameEntry.Text)) { await DisplayAlert("Required", "Name is required", "OK"); return; }
        if (string.IsNullOrWhiteSpace(LocationEntry.Text)) { await DisplayAlert("Required", "Location is required", "OK"); return; }
        if (string.IsNullOrWhiteSpace(LengthEntry.Text) || !double.TryParse(LengthEntry.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var lenKm))
        { await DisplayAlert("Invalid", "Length (km) is required and must be a number", "OK"); return; }

        int difficulty = (DifficultyPicker.SelectedIndex >= 0 ? DifficultyPicker.SelectedIndex + 1 : 3);

        if (Mode == "edit" && Item != null)
        {
            Item.Name = NameEntry.Text!.Trim();
            Item.Location = LocationEntry.Text!.Trim();
            Item.Date = DatePicker.Date;
            Item.Parking = ParkingSwitch.IsToggled;
            Item.LengthKm = lenKm;
            Item.Difficulty = difficulty;
            Item.Description = DescEditor.Text?.Trim() ?? string.Empty;

            // later: update in SQLite, then pop
            await DisplayAlert("Updated", "Hike updated (UI-only).", "OK");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            var newHike = new Hike
            {
                Name = NameEntry.Text!.Trim(),
                Location = LocationEntry.Text!.Trim(),
                Date = DatePicker.Date,
                Parking = ParkingSwitch.IsToggled,
                LengthKm = lenKm,
                Difficulty = difficulty,
                Description = DescEditor.Text?.Trim() ?? string.Empty
            };

            // UI-only add: find MainPage and add to collection
            if (Shell.Current?.CurrentPage?.Navigation?.NavigationStack?.FirstOrDefault() is MainPage main)
            {
                main.Hikes.Add(newHike);
            }
            await DisplayAlert("Saved", "Hike added (UI-only).", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}