namespace ModelTrain.Screens
{
    /*
     * This screen has effectively been disabled for the foreseeable future,
     * due to time constraints and us not seeing it as a critical feature
     * for getting the program working. We may come back to this screen in the future,
     * hence why we're not deleting it, but it's not currently accessible in the app.
     * 
     * 
     * 
     * This class is the background functionality/methods for the Collaborative Storage page
     * Author: Taylor Showalter and Alex Robinson
     * Date: November 13th, 2024
     */
    public partial class CollaborativeStorage : ContentPage
    {
        public CollaborativeStorage()
        {
            InitializeComponent();
        }

        private PersonalProject _selectedProject;
        private Frame _selectedFrame;

        private async void OnProjectTapped(object sender, EventArgs e)
        {
            // If old selection, reset color to white
            if (_selectedFrame != null)
            {
                _selectedFrame.BackgroundColor = Colors.White;
            }

            // Get tapped Frame and find its bound Project
            var frame = sender as Frame;
            if (frame?.BindingContext is PersonalProject tappedProject)
            {
                _selectedProject = tappedProject;
                _selectedFrame = frame;

                // Apply bounce effect
                await _selectedFrame.ScaleTo(0.95, 50, Easing.CubicIn);  // Scale down slightly
                await _selectedFrame.ScaleTo(1.0, 50, Easing.CubicOut);  // Scale back

                // Set background color of the selected frame to provide feedback
                _selectedFrame.BackgroundColor = Colors.LightGray;
            }
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            // Navigate to the Track Editor page
            if (_selectedProject != null)
            {
                await Navigation.PushAsync(new TrackEditor(_selectedProject));
            }
            else
            {
                await DisplayAlert("No Project Selected",
                    "Please select a project to edit.", "OK");
            }
        }

        private async void OnConfigureButtonClicked(object sender, EventArgs e)
        {
            // Navigate to the Properties page
            await Navigation.PushAsync(new PropertiesScreen());
        }
        private async void OnHomeButtonClicked(object sender, EventArgs e)
        {
            // Clear the navigation stack by setting a new NavigationPage
            // with HomeScreen as the root
            Application.Current.MainPage = new NavigationPage(new HomeScreen());
        }


    }
}