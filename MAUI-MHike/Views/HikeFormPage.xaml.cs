using MAUI_MHike.Models;
using MAUI_MHike.Services;
using System.Globalization;

namespace MAUI_MHike.Views;

[QueryProperty(nameof(Mode), "Mode")]
[QueryProperty(nameof(HikeId), "HikeId")]
public partial class HikeFormPage : ContentPage
{
    private readonly IHikeRepository _repo;
    public string Mode { get; set; } = "add";
    public string? HikeId { get; set; }
    private Hike? _editing;
    private string? _pendingPhotoPath;

    public HikeFormPage(IHikeRepository repo)
    {
        InitializeComponent();
        _repo = repo;

        DifficultyPicker.SelectedIndex = 2;
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
                _pendingPhotoPath = _editing.PhotoPath;
                LoadPhoto(_pendingPhotoPath);
                return;
            }
        }

        TitleLabel.Text = "Add Hike";
        LoadPhoto(null);
    }

    private void LoadPhoto(string? path)
    {
        if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            PhotoImage.Source = ImageSource.FromFile(path);
        else
            PhotoImage.Source = null;
    }

    // --- Photo pick ---
    private async void OnChoosePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            var file = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Choose a photo",
                FileTypes = FilePickerFileType.Images
            });
            if (file == null) return;

            // Copy to app storage
            var saved = await SaveFileToAppDataAsync(file.FullPath);
            _pendingPhotoPath = saved;
            LoadPhoto(saved);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to choose photo: {ex.Message}", "OK");
        }
    }

    // --- Photo capture ---
    private async void OnTakePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await DisplayAlert("Not supported", "Camera capture not supported on this device.", "OK");
                return;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo == null) return;

            var saved = await SaveFileToAppDataAsync(photo.FullPath);
            _pendingPhotoPath = saved;
            LoadPhoto(saved);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to take photo: {ex.Message}", "OK");
        }
    }

    private async Task<string> SaveFileToAppDataAsync(string sourcePath)
    {
        var ext = Path.GetExtension(sourcePath);
        var fileName = $"hike_{Guid.NewGuid():N}{ext}";
        var dest = Path.Combine(FileSystem.AppDataDirectory, fileName);

        await using var src = File.OpenRead(sourcePath);
        await using var dst = File.Open(dest, FileMode.Create, FileAccess.Write, FileShare.None);
        await src.CopyToAsync(dst);

        return dest;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        { await DisplayAlert("Required", "Name is required", "OK"); return; }
        if (string.IsNullOrWhiteSpace(LocationEntry.Text))
        { await DisplayAlert("Required", "Location is required", "OK"); return; }
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
            _editing.PhotoPath = _pendingPhotoPath;

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
                Description = DescEditor.Text?.Trim() ?? string.Empty,
                PhotoPath = _pendingPhotoPath
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