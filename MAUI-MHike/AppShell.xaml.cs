namespace MAUI_MHike
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Views.HikeFormPage), typeof(Views.HikeFormPage));
        }
    }
}
