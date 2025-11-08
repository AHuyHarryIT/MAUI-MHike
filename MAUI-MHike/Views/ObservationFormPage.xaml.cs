using MAUI_MHike.Models;
using MAUI_MHike.Services;

namespace MAUI_MHike.Views;

[QueryProperty(nameof(Mode), "Mode")]
[QueryProperty(nameof(HikeId), "HikeId")]
[QueryProperty(nameof(ObsId), "ObsId")]
public partial class ObservationFormPage : ContentPage
{
    private readonly IObservationRepository _repo;
    public string Mode { get; set; } = "add";
    public string? HikeId { get; set; }
    public string? ObsId { get; set; }

    private Observation? _editing;
    private long _nowSec;

    public ObservationFormPage(IObservationRepository repo)
    {
        InitializeComponent();
        _repo = repo;
        _nowSec = DateTimeOffset.Now.ToUnixTimeSeconds();
        TimeLabel.Text = $"Time: {DateTimeOffset.FromUnixTimeSeconds(_nowSec):yyyy-MM-dd HH:mm}";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (Mode == "edit" && !string.IsNullOrWhiteSpace(ObsId))
        {
            _editing = await _repo.GetByIdAsync(ObsId);
            if (_editing != null)
            {
                NoteEditor.Text = _editing.Note;
                CommentsEditor.Text = _editing.Comments;
                _nowSec = _editing.TimeSec; // keep original
                TimeLabel.Text = $"Time: {DateTimeOffset.FromUnixTimeSeconds(_nowSec):yyyy-MM-dd HH:mm}";
                Title = "Edit Observation";
                return;
            }
        }
        Title = "Add Observation";
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var note = (NoteEditor.Text ?? "").Trim();
        var comments = (CommentsEditor.Text ?? "").Trim();
        if (string.IsNullOrWhiteSpace(note))
        {
            await DisplayAlert("Required", "Note is required.", "OK");
            return;
        }

        if (_editing != null)
        {
            _editing.Note = note;
            _editing.Comments = string.IsNullOrEmpty(comments) ? null : comments;
            await _repo.UpdateAsync(_editing);
            await DisplayAlert("Updated", "Observation updated.", "OK");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(HikeId))
            {
                await DisplayAlert("Error", "Hike not found.", "OK");
                return;
            }
            var ob = new Observation
            {
                HikeId = HikeId,
                Note = note,
                Comments = string.IsNullOrEmpty(comments) ? null : comments,
                TimeSec = _nowSec
            };
            await _repo.InsertAsync(ob);
            await DisplayAlert("Saved", "Observation added.", "OK");
        }

        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
