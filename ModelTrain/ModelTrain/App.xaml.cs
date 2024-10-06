namespace ModelTrain
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            // Navigate to the login page when the app starts
            Shell.Current.GoToAsync("//screen");
        }
    }
}
