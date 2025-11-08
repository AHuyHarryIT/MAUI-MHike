using MAUI_MHike.Services;
using Microsoft.Extensions.Logging;

namespace MAUI_MHike;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>() // App constructor will now get IServiceProvider
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // your existing DI registrations
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        builder.Services.AddSingleton<IHikeRepository, HikeRepository>();
        builder.Services.AddSingleton<IObservationRepository, ObservationRepository>();

        builder.Services.AddTransient<Views.MainPage>();
        builder.Services.AddTransient<Views.HikeFormPage>();
        builder.Services.AddTransient<Views.ObservationListPage>();
        builder.Services.AddTransient<Views.ObservationFormPage>();

        return builder.Build();
    }
}