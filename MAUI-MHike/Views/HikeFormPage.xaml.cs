using MAUI_MHike.Models;
using MAUI_MHike.Services;
using System.Globalization;

namespace MAUI_MHike.Views;


[QueryProperty(nameof(Mode), "Mode")]
[QueryProperty(nameof(HikeId), "HikeId")]
public partial class HikeFormPage : ContentPage
{
    private readonly IHikeRepository _repo;

    public string Mode { get; set; } = "add";  // "add" | "edit"
    public string? HikeId { get; set; }
    private Hike? _editing;

    public HikeFormPage(IHikeRepository repo)
    {
        InitializeComponent();
        _repo = repo;

        DifficultyPicker.SelectedIndex = 2; // default 3 - Medium
        DatePicker.Date = DateTime.Today;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (Mode == "edit" && !string.IsNullOrWhiteSpace(HikeId))
        {
            _editing = await _repo.GetByIdAsync(HikeId);
            if (_editing != null)
            {
                TitleLabel.Text = "Edit Hike";
                NameEntry.Text = _editing.Name;
                LocationEntry.Text = _editing.Location;
                DatePicker.Date = _editing.Date;
                ParkingSwitch.IsToggled = _editing.Parking;
                LengthEntry.Text = _editing.LengthKm.ToString(CultureInfo.InvariantCulture);
                DescEditor.Text = _editing.Description;
                DifficultyPicker.SelectedIndex = Math.Clamp(_editing.Difficulty - 1, 0, 4);
                return;
            }
        }

        TitleLabel.Text = "Add Hike";
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text)) { await DisplayAlert("Required", "Name is required", "OK"); return; }
        if (string.IsNullOrWhiteSpace(LocationEntry.Text)) { await DisplayAlert("Required", "Location is required", "OK"); return; }
        if (string.IsNullOrWhiteSpace(LengthEntry.Text) || !double.TryParse(LengthEntry.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var lenKm))
        { await DisplayAlert("Invalid", "Length (km) must be a number", "OK"); return; }

        var difficulty = (DifficultyPicker.SelectedIndex >= 0 ? DifficultyPicker.SelectedIndex + 1 : 3);

        if (_editing != null)
        {
            _editing.Name = NameEntry.Text!.Trim();
            _editing.Location = LocationEntry.Text!.Trim();
            _editing.Date = DatePicker.Date;
            _editing.Parking = ParkingSwitch.IsToggled;
            _editing.LengthKm = lenKm;
            _editing.Difficulty = difficulty;
            _editing.Description = DescEditor.Text?.Trim() ?? string.Empty;

            await _repo.UpdateAsync(_editing);
            await DisplayAlert("Updated", "Hike updated.", "OK");
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
            await _repo.InsertAsync(newHike);
            await DisplayAlert("Saved", "Hike added.", "OK");
        }

        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_editing == null) return;
        var ok = await DisplayAlert("Delete", $"Delete \"{_editing.Name}\"?", "Delete", "Cancel");
        if (!ok) return;

        await _repo.DeleteAsync(_editing.Id);
        await DisplayAlert("Deleted", "Hike deleted.", "OK");
        await Shell.Current.GoToAsync("..");
    }
    private async void OnOpenObservations(object sender, EventArgs e)
    {
        if (_editing == null)
        {
            await DisplayAlert("Not available", "Save the hike first, then add observations.", "OK");
            return;
        }
        await Shell.Current.GoToAsync(nameof(ObservationListPage), true, new Dictionary<string, object> {
        { "HikeId", _editing.Id }
    });
    }
}