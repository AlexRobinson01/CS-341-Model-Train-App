using ModelTrain.Screens;
using ModelTrain.Services;
namespace ModelTrain
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Retrieve the saved theme preference
            string theme = Preferences.Get("UserTheme", "Light");

            if (theme == "Dark")
            {
                UserAppTheme = AppTheme.Dark;
            }
            else
            {
                UserAppTheme = AppTheme.Light;
            }

            //CHANGE new NavigationPage(new YOURSCREEN()) to view screen
            MainPage = new NavigationPage(new Login());

            DeviceOrientation.SetPortrait();
        }
    }
}
