namespace ModelTrain.Screens;

/*
 * This class is the background functionality/methods for the personal projects page
 * Author: Andrew Martin
 * Date: October 6, 2024
 */
public partial class PersonalProjects : ContentPage
{
    public PersonalProjects()
    {
        InitializeComponent();

        collectionView.ItemsSource = new List<PersonalProject>
        {
        new PersonalProject { ProjectName = "Project 1", DateCreated = DateTime.Now.ToString("MM/dd/yyyy"), ProjectID = "0001", Track = new Model.Track.TrackBase()},
        new PersonalProject { ProjectName = "Project 2", DateCreated = DateTime.Now.ToString("MM/dd/yyyy"), ProjectID = "0002", Track = new Model.Track.TrackBase()},
        new PersonalProject { ProjectName = "Project 3", DateCreated = DateTime.Now.ToString("MM/dd/yyyy"), ProjectID = "0003", Track = new Model.Track.TrackBase()},
        new PersonalProject { ProjectName = "Project 4", DateCreated = DateTime.Now.ToString("MM/dd/yyyy"), ProjectID = "0004", Track = new Model.Track.TrackBase()}
        };
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
            await DisplayAlert("No Project Selected", "Please select a project to edit.", "OK");
        }
    }

    private async void OnConfigureButtonClicked(object sender, EventArgs e)
    {
        // Navigate to the Properties page
        await Navigation.PushAsync(new PropertiesScreen());
    }
    private async void OnHomeButtonClicked(object sender, EventArgs e)
    {
        // Clear the navigation stack by setting a new NavigationPage with HomeScreen as the root
        Application.Current.MainPage = new NavigationPage(new HomeScreen());
    }

}