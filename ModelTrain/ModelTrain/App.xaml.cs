using ModelTrain.Screens;
namespace ModelTrain
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //DO NOT CHANGE new NavigationPage(new YOURSCREEN()) to view screen
            MainPage = new AppShell();

            //MainPage = new AppShell();
            //// Navigate to the login page when the app starts
            //Shell.Current.GoToAsync("//screen");
        }
    }
}
