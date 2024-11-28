using ModelTrain.Screens;
using ModelTrain.Services;
using ModelTrain.Model;

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
        protected override async void OnStart()
        {
            base.OnStart();

            string email = await SecureStorage.GetAsync("UserEmail");
            string password = await SecureStorage.GetAsync("UserPassword");

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                // Validate stored credentials
                if (await BusinessLogic.Instance.ValidateLoginInput(email, password))
                {
                    // Navigate to the Home Screen
                    MainPage = new NavigationPage(new HomeScreen());
                    return;
                }
            }

            // Navigate to the Login screen if no valid credentials
            MainPage = new NavigationPage(new Login());
        }

    }
}
