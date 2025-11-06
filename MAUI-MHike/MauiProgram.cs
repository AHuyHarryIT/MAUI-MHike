using MAUI_MHike.Services;
using Microsoft.Extensions.Logging;

namespace MAUI_MHike
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Services
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
            builder.Services.AddSingleton<IHikeRepository, HikeRepository>();

            // Pages (optional DI)
            builder.Services.AddTransient<Views.MainPage>();
            builder.Services.AddTransient<Views.HikeFormPage>();

            return builder.Build();
        }
    }
}
