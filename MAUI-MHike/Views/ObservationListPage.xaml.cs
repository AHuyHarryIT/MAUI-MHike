using System.Collections.ObjectModel;
using MAUI_MHike.Models;
using MAUI_MHike.Services;

namespace MAUI_MHike.Views;

[QueryProperty(nameof(HikeId), "HikeId")]
public partial class ObservationListPage : ContentPage
{
    private readonly IObservationRepository _repo;
    public string? HikeId { get; set; }
    public ObservableCollection<Observation> Items { get; } = new();

    public ObservationListPage()
        : this(App.Services.GetRequiredService<IObservationRepository>()) { }

    public ObservationListPage(IObservationRepository repo)
    {
        InitializeComponent();
        _repo = repo;
        ObsView.ItemsSource = Items;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        if (string.IsNullOrWhiteSpace(HikeId)) return;
        var list = await _repo.ListByHikeAsync(HikeId);
        Items.Clear();
        foreach (var o in list) Items.Add(o);
        RefreshContainer.IsRefreshing = false;
        Hdr.Text = $"Observations ({Items.Count})";
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await ReloadAsync();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ObservationFormPage), true, new Dictionary<string, object> {
            { "Mode", "add" },
            { "HikeId", HikeId! }
        });
    }

    private async void OnItemTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Element el || el.BindingContext is not Observation ob) return;
        await Shell.Current.GoToAsync(nameof(ObservationFormPage), true, new Dictionary<string, object> {
            { "Mode", "edit" },
            { "ObsId", ob.Id },
            { "HikeId", ob.HikeId }
        });
    }

    private async void OnDeleteSwipeInvoked(object sender, EventArgs e)
    {
        if (sender is not SwipeItem si || si.BindingContext is not Observation ob) return;
        var ok = await DisplayAlert("Delete", "Delete this observation?", "Delete", "Cancel");
        if (!ok) return;
        await _repo.DeleteAsync(ob.Id);
        Items.Remove(ob);
        if (si.Parent is SwipeItems s && s.Parent is SwipeView v) v.Close();
    }
}
