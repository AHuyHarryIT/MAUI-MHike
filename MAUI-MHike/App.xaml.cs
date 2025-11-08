using MAUI_MHike.Services;

namespace MAUI_MHike;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public App(IServiceProvider provider)
    {
        InitializeComponent();

        // store service provider for global access
        Services = provider;

        // initialize database
        var db = provider.GetRequiredService<IDatabaseService>();
        Task.Run(() => db.InitAsync()).Wait();

        MainPage = new AppShell();
    }
}
