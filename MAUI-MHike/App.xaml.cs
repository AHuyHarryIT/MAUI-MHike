using MAUI_MHike.Services;

namespace MAUI_MHike
{
    public partial class App : Application
    {
        public App(IDatabaseService db)
        {
            InitializeComponent();

            Task.Run(() => db.InitAsync()).Wait();

            MainPage = new AppShell();
        }
    }
}
