using System.Collections.ObjectModel;
using MAUI_MHike.Models;
using MAUI_MHike.Services;

namespace MAUI_MHike.Views;

public partial class MainPage : ContentPage
{
    private readonly IHikeRepository _repo;
    public ObservableCollection<Hike> Hikes { get; } = new();

    // cache all rows from DB; we filter this list
    private List<Hike> _all = new();

    public MainPage(IHikeRepository repo)
    {
        InitializeComponent();
        _repo = repo;
        BindingContext = this;

        // Defaults
        ParkingPicker.SelectedIndex = 0; // Any
        DiffMinPicker.SelectedIndex = 0; // Any
        DiffMaxPicker.SelectedIndex = 0; // Any

        FromDatePicker.Date = DateTime.Today.AddMonths(-1);
        ToDatePicker.Date = DateTime.Today;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        _all = await _repo.GetAllAsync(); // get all; filter in memory
        ApplyFiltersAndSearch();
        if (RefreshContainer.IsRefreshing) RefreshContainer.IsRefreshing = false;
    }

    // === UI handlers ===

    private void OnToggleFiltersClicked(object sender, EventArgs e)
    {
        FiltersPanel.IsVisible = !FiltersPanel.IsVisible;
    }

    private void OnFilterChanged(object? sender, EventArgs e)
    {
        ApplyFiltersAndSearch();
    }

    private void OnClearFiltersClicked(object sender, EventArgs e)
    {
        UseDateRange.IsChecked = false;
        FromDatePicker.Date = DateTime.Today.AddMonths(-1);
        ToDatePicker.Date = DateTime.Today;
        ParkingPicker.SelectedIndex = 0;
        DiffMinPicker.SelectedIndex = 0;
        DiffMaxPicker.SelectedIndex = 0;
        SearchBar.Text = string.Empty;
        ApplyFiltersAndSearch();
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFiltersAndSearch();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HikeFormPage), true, new Dictionary<string, object> {
            { "Mode", "add" }
        });
    }

    private async void OnItemTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Element el || el.BindingContext is not Hike h) return;
        await Shell.Current.GoToAsync(nameof(HikeFormPage), true, new Dictionary<string, object> {
            { "Mode", "edit" },
            { "HikeId", h.Id }
        });
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await ReloadAsync();
    }

    private async void OnDeleteSwipeInvoked(object sender, EventArgs e)
    {
        if (sender is not SwipeItem si || si.BindingContext is not Hike h) return;
        var ok = await DisplayAlert("Delete", $"Delete \"{h.Name}\"?", "Delete", "Cancel");
        if (!ok) return;
        await _repo.DeleteAsync(h.Id);
        _all.RemoveAll(x => x.Id == h.Id);
        ApplyFiltersAndSearch();
        if (si.Parent is SwipeItems s && s.Parent is SwipeView v) v.Close();
    }

    // === Filtering logic ===

    private void ApplyFiltersAndSearch()
    {
        IEnumerable<Hike> q = _all;

        // Search
        var text = (SearchBar.Text ?? "").Trim();
        if (text.Length > 0)
        {
            var lower = text.ToLowerInvariant();
            q = q.Where(h =>
                (h.Name?.ToLowerInvariant().Contains(lower) ?? false) ||
                (h.Location?.ToLowerInvariant().Contains(lower) ?? false));
        }

        // Date range (only if enabled)
        if (UseDateRange.IsChecked)
        {
            var from = FromDatePicker.Date.Date;
            var to = ToDatePicker.Date.Date;
            if (from > to)
            {
                // auto-fix inverted range
                (from, to) = (to, from);
            }
            q = q.Where(h => h.Date.Date >= from && h.Date.Date <= to);
        }

        // Parking
        switch (ParkingPicker.SelectedIndex)
        {
            case 1: q = q.Where(h => h.Parking); break;     // Yes
            case 2: q = q.Where(h => !h.Parking); break;    // No
                                                            // 0 = Any
        }

        // Difficulty min/max
        int? dmin = ParsePickerInt(DiffMinPicker.SelectedItem);
        int? dmax = ParsePickerInt(DiffMaxPicker.SelectedItem);
        if (dmin.HasValue && dmax.HasValue && dmin > dmax) (dmin, dmax) = (dmax, dmin);
        if (dmin.HasValue) q = q.Where(h => h.Difficulty >= dmin.Value);
        if (dmax.HasValue) q = q.Where(h => h.Difficulty <= dmax.Value);

        // Apply
        var result = q.OrderByDescending(h => h.DateIso).ToList();
        Hikes.Clear();
        foreach (var h in result) Hikes.Add(h);
    }

    private int? ParsePickerInt(object item)
    {
        var s = item as string;
        if (string.IsNullOrWhiteSpace(s) || s.Equals("Any", StringComparison.OrdinalIgnoreCase)) return null;
        return int.TryParse(s, out var n) ? n : null;
    }
}