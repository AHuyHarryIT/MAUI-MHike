namespace MAUI_MHike;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(Views.HikeFormPage), typeof(Views.HikeFormPage));
        Routing.RegisterRoute(nameof(Views.ObservationListPage), typeof(Views.ObservationListPage));
        Routing.RegisterRoute(nameof(Views.ObservationFormPage), typeof(Views.ObservationFormPage));
    }
}

