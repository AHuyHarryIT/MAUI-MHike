using MAUI_MHike.Models;
using MAUI_MHike.Services;
using System.Collections.ObjectModel;

namespace MAUI_MHike.Views;

public partial class MainPage : ContentPage
{
    private readonly IHikeRepository _repo;
    public ObservableCollection<Hike> Hikes { get; } = new();
    private string? _lastQuery;

    public MainPage(IHikeRepository repo)
    {
        InitializeComponent();
        _repo = repo;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ReloadAsync();
    }

    private async Task ReloadAsync(string? q = null)
    {
        if (q != null) _lastQuery = q;
        var list = await _repo.GetAllAsync(_lastQuery);
        Hikes.Clear();
        foreach (var h in list) Hikes.Add(h);
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HikeFormPage), true, new Dictionary<string, object> {
            { "Mode", "add" }
        });
    }

    private async void OnItemTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Element el) return;
        if (el.BindingContext is not Hike h) return;

        await Shell.Current.GoToAsync(nameof(HikeFormPage), true, new Dictionary<string, object> {
            { "Mode", "edit" },
            { "HikeId", h.Id }
        });
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await ReloadAsync();
        if (sender is RefreshView rv) rv.IsRefreshing = false;
    }

    private async void OnDeleteSwipeInvoked(object sender, EventArgs e)
    {
        if (sender is not SwipeItem swipeItem) return;
        if (swipeItem.BindingContext is not Hike h) return;

        var ok = await DisplayAlert("Delete", $"Delete \"{h.Name}\"?", "Delete", "Cancel");
        if (!ok) return;

        await _repo.DeleteAsync(h.Id);
        Hikes.Remove(h);

        // Close the swipe
        if (swipeItem.Parent is SwipeItems swipeItems && swipeItems.Parent is SwipeView swipeView)
            swipeView.Close();
    }

    private async void SearchBar_SearchButtonPressed(object sender, EventArgs e)
    {
        await ReloadAsync(((SearchBar)sender).Text);
    }
}