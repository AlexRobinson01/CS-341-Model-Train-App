using ModelTrain.Screens;
using ModelTrain.Services;
namespace ModelTrain
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //CHANGE new NavigationPage(new YOURSCREEN()) to view screen
            MainPage = new NavigationPage(new Login());

            DeviceOrientation.SetPortrait();
        }
    }
}
